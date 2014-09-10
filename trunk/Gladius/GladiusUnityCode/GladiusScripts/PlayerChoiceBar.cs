using UnityEngine;
using System.Collections;
using Gladius.arena;
using Gladius;
using System.Collections.Generic;
using System.Text;
using System;

public class PlayerChoiceBar : MonoBehaviour
{



    // Use this for initialization
    private void AddIconState(SkillIcon icon, SkillIconState state, String regionName)
    {
        m_iconStateRegionDictionary[new IconAndState(icon, state)] = regionName;
    }

    void Start()
    {

        var control = gameObject.GetComponent<dfControl>();


        m_textureAtlas = control.GetManager().DefaultAtlas;
        m_healthProgressBar = GladiusGlobals.FindChild("HealthProgress",control.transform).GetComponent<dfProgressBar>();
        m_affinityProgressBar = GladiusGlobals.FindChild("AffinityPointProgress", control.transform).GetComponent<dfProgressBar>();
        m_affinityArmourProgressBar = GladiusGlobals.FindChild("AffinityArmourProgress", control.transform).GetComponent<dfProgressBar>();
        m_skillPointProgressBar = GladiusGlobals.FindChild("SkillPointProgress", control.transform).GetComponent<dfProgressBar>();


        m_skillSprites = new dfSprite[]{GladiusGlobals.FindChild("SkillSlot1", control.transform).GetComponent<dfSprite>(),
        GladiusGlobals.FindChild("SkillSlot2", control.transform).GetComponent<dfSprite>(),
        GladiusGlobals.FindChild("SkillSlot3", control.transform).GetComponent<dfSprite>(),
        GladiusGlobals.FindChild("SkillSlot4", control.transform).GetComponent<dfSprite>(),
        GladiusGlobals.FindChild("SkillSlot5", control.transform).GetComponent<dfSprite>()};


        m_skillNameLabel = GladiusGlobals.FindChild("SkillNameLabel", control.transform).GetComponent<dfLabel>();
        m_skillTypeLabel = GladiusGlobals.FindChild("SkillTypeLabel", control.transform).GetComponent<dfLabel>();
        m_actorNameLabel = GladiusGlobals.FindChild("ActorNameLabel", control.transform).GetComponent<dfLabel>();

        m_debugLabel = GladiusGlobals.FindChild("DebugTextRTLabel", control.transform).GetComponent<dfRichTextLabel>(); 

        //TPackManager.load(atlasPath);

        AddIconState(SkillIcon.Move, SkillIconState.Available, "SkillMove");
        AddIconState(SkillIcon.Move, SkillIconState.Selected, "SkillMoveSelected");
        AddIconState(SkillIcon.Move, SkillIconState.Unavailable, "SkillMoveUnavailable");
        AddIconState(SkillIcon.Attack, SkillIconState.Available, "SkillAttack");
        AddIconState(SkillIcon.Attack, SkillIconState.Selected, "SkillAttackSelected");
        AddIconState(SkillIcon.Attack, SkillIconState.Unavailable, "SkillAttackUnavailable");
        AddIconState(SkillIcon.Combo, SkillIconState.Available, "SkillCombo");
        AddIconState(SkillIcon.Combo, SkillIconState.Selected, "SkillComboSelected");
        AddIconState(SkillIcon.Combo, SkillIconState.Unavailable, "SkillComboUnavailable");
        AddIconState(SkillIcon.Special, SkillIconState.Available, "SkillSpecial");
        AddIconState(SkillIcon.Special, SkillIconState.Selected, "SkillSpecialSelected");
        AddIconState(SkillIcon.Special, SkillIconState.Unavailable, "SkillSpecialUnavailable");
        AddIconState(SkillIcon.Affinity, SkillIconState.Available, "SkillAffinity");
        AddIconState(SkillIcon.Affinity, SkillIconState.Selected, "SkillAffinitySelected");
        AddIconState(SkillIcon.Affinity, SkillIconState.Unavailable, "SkillAffinityUnavailable");

        m_attackSkills = new List<List<AttackSkill>>();
        for (int i = 0; i < numSkillSlots; ++i)
        {
            m_attackSkills.Add(new List<AttackSkill>());
        }
        m_currentAttackSkillLine = new List<AttackSkill>(numSkillSlots);
        RegisterListeners();
    }

    const int numSkillSlots = 5;

    public void Update()
    {

        if (CurrentActor != null && CurrentlySelectedSkill != null)
        {
            m_healthProgressBar.MaxValue = CurrentActor.MaxHealth;
            m_healthProgressBar.Value = CurrentActor.Health;
            m_skillNameLabel.Text = CurrentlySelectedSkill.Name;
            m_skillTypeLabel.Text = m_skillGroupNames[m_actionCursor.X];
            m_skillPointProgressBar.Value = CurrentActor.ArenaSkillPoints;

            m_affinityProgressBar.MaxValue = CurrentActor.MaxAffinity;
            m_affinityProgressBar.Value = CurrentActor.Affinity;
            m_affinityProgressBar.ProgressSprite = SpriteForDamageType(CurrentActor.WeaponAffinityType);
        }


       
        if (CurrentActor != null)
        {
            StringBuilder sb = new StringBuilder(); 
            sb.AppendFormat("Current [{0}] HP[{1}] SP[{2}]\n",CurrentActor.Name,CurrentActor.Health,CurrentActor.ArenaSkillPoints);
            sb.AppendFormat("Pos [{0},{1}]\n", CurrentActor.ArenaPoint.X,CurrentActor.ArenaPoint.Y);
            sb.AppendFormat("Cursor Pos[{0},{1}]\n", MovementGrid.CurrentCursorPoint.X, MovementGrid.CurrentCursorPoint.X);
            BaseActor target = Arena.GetActorAtPosition(MovementGrid.CurrentCursorPoint);
            if(target != null)
            {
                sb.AppendFormat("Target [{0}] HP[{1}] SP[{2}]\n", target.Name, target.Health, target.ArenaSkillPoints);
                if (GladiusGlobals.CombatEngine.IsValidTarget(CurrentActor, target, CurrentActor.CurrentAttackSkill))
                {
                    float damage = GladiusGlobals.CombatEngine.CalculateExpectedDamage(CurrentActor, target, CurrentActor.CurrentAttackSkill);
                    sb.AppendFormat("Skill [{0}] Damage [{1}]",CurrentActor.CurrentAttackSkill.Name,damage);
                }
                else
                {
                    sb.Append("Invalid Target");
                }
            }
                m_debugLabel.Text = sb.ToString();
        }


    }


    public void RegisterListeners()
    {
        EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
        EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);
    }


    public void UnregisterListeners()
    {
        EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);
    }


    void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
    {
        CurrentActor = e.New;
    }

    public void BuildDataForActor()
    {
        foreach (List<AttackSkill> list in m_attackSkills)
        {
            list.Clear();
        }
        m_currentAttackSkillLine.Clear();


        foreach (AttackSkill attackSkill in CurrentActor.AttackSkills)
        {
        	if(attackSkill.SkillRow >= 0 && attackSkill.SkillRow < m_attackSkills.Count)
        	{
            	m_attackSkills[attackSkill.SkillRow].Add(attackSkill);
            }
        }

        for (int i = 0; i < numSkillSlots; ++i)
        {
            if (m_attackSkills[i].Count == 0)
            {
                m_attackSkills[i].Add(GladiusGlobals.AttackSkillDictionary.Data["None"]);
            }
            m_currentAttackSkillLine.Add(m_attackSkills[i][0]);
        }


        m_actionCursor = new Point();


        m_healthProgressBar.MaxValue = CurrentActor.MaxHealth;
        InitialiseSkillSlots();

        m_actorNameLabel.Text = CurrentActor.Name;
        int useCost = CurrentlySelectedSkill.UseCost - 1;

        m_skillPointProgressBar.Value = useCost;
        
    }


    void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
    {
        if (!InputAllowed)
        {
            return;
        }

        HandleSkillChoiceAction(e);
        HandleMovementGridAction(e);

        //if (TurnManager.CurrentControlState == ControlState.ChoosingSkill)
        //{
        //    HandleSkillChoiceAction(e);
        //}
        //else if (TurnManager.CurrentControlState == ControlState.UsingGrid)
        //{
        //    HandleMovementGridAction(e);
        //}
    }



    private void HandleSkillChoiceAction(ActionButtonPressedArgs e)
    {
        switch (e.ActionButton)
        {

            case (ActionButton.Move2Left):
                {
                    SkillCursorLeft();
                    break;
                }
            case (ActionButton.Move2Right):
                {
                    SkillCursorRight();
                    break;
                }
            case (ActionButton.Move2Up):
                {
                    SkillCursorUp();
                    break;
                }
            case (ActionButton.Move2Down):
                {
                    SkillCursorDown();
                    break;
                }

            //case (ActionButton.ActionButton1):
            //    {
            //        if (CurrentlySelectedSkill.NeedsGrid)
            //        {
            //            //Debug.Log("Asking for grid");
            //            MovementGrid.CurrentCursorPoint = CurrentActor.ArenaPoint;
            //            CurrentActor.CurrentAttackSkill = CurrentlySelectedSkill;
            //            TurnManager.CurrentControlState = ControlState.UsingGrid;
            //            GladiusGlobals.MovementGrid.RebuildMesh = true;
            //        }
            //        else
            //        {
            //            //ConfirmAction();
            //        }
            //        break;
            //    }
            //// cancel
            //case (ActionButton.ActionButton2):
            //    {
            //        CancelAction();
            //        GladiusGlobals.MovementGrid.RebuildMesh = true;
            //        break;
            //    }

        }
    }


    private void UpdateSkillCursor(Point delta)
    {
        // update previous
        int currentX = m_actionCursor.X;

        SkillIconState iconState = SkillIconState.Available;
        AttackSkill skill = m_attackSkills[m_actionCursor.X][m_actionCursor.Y];
        
        if (!skill.Available(CurrentActor))
        {
            iconState = SkillIconState.Unavailable;
        }
        
        UpdateSkillIcon(m_actionCursor.X,iconState);

        m_actionCursor = GladiusGlobals.Add(m_actionCursor, delta);

        m_actionCursor.X = (m_actionCursor.X + m_attackSkills.Count) % m_attackSkills.Count;

        int newX = m_actionCursor.X;

        if (delta.X != 0)
        {
            m_actionCursor.Y = 0;
        }
        m_actionCursor.Y = (m_actionCursor.Y + m_attackSkills[m_actionCursor.X].Count) % m_attackSkills[m_actionCursor.X].Count;
        m_currentAttackSkillLine[m_actionCursor.X] = m_attackSkills[m_actionCursor.X][m_actionCursor.Y];
        CurrentActor.CurrentAttackSkill = CurrentlySelectedSkill;

        iconState = SkillIconState.Selected;
        if (!skill.Available(CurrentActor))
        {
            //iconState = SkillIconState.Unavailable;
        }

        UpdateSkillIcon(m_actionCursor.X,iconState);
    }


    public void SkillCursorLeft()
    {
        UpdateSkillCursor(new Point(-1, 0));
    }

    public void SkillCursorRight()
    {
        UpdateSkillCursor(new Point(1, 0));
    }

    public void SkillCursorUp()
    {
        UpdateSkillCursor(new Point(0, 1));
    }

    public void SkillCursorDown()
    {
        UpdateSkillCursor(new Point(0, -1));
    }





    public void UpdateSkillIcon(int slot, SkillIconState state)
    {
        try
        {
            String key;
            SkillIcon icon = SkillIcon.Move;
            switch (slot)
            {
                case 0:  
                    icon = SkillIcon.Move;
                    break;
                case 1:
                    icon = SkillIcon.Attack;
                    break;
                case 2:
                    icon = SkillIcon.Combo;
                    break;
                case 3:
                    icon = SkillIcon.Affinity;
                    break;
                case 4:
                    icon = SkillIcon.Special;
                    break;

            }

            if (m_iconStateRegionDictionary.TryGetValue(new IconAndState(icon, state), out key))
            {
                m_skillSprites[slot].SpriteName = key;
            }
        }
        catch (System.Exception ex)
        {
        }

    }

    private BaseActor m_currentActor;
    public BaseActor CurrentActor
    {
        get
        {
            return m_currentActor;
        }
        set
        {
            m_currentActor = value;
            //TurnManager.CurrentControlState = ControlState.ChoosingSkill;
            BuildDataForActor();
            CurrentActor.CurrentAttackSkill = CurrentlySelectedSkill;
            InputAllowed = true;
        }
    }

    public AttackSkill CurrentlySelectedSkill
    {
        get
        {
            return m_currentAttackSkillLine[m_actionCursor.X];
        }
    }

    public void CancelAction()
    {
        //if (TurnManager.CurrentControlState == ControlState.UsingGrid)
        //{
        //    CurrentActor.WayPointList.Clear();
        //    TurnManager.CurrentControlState = ControlState.ChoosingSkill;
        //}
        MovementGrid.RebuildMesh = true;
    }

    public TurnManager TurnManager
    {
        get
        {
            return GladiusGlobals.TurnManager;
        }
    }

    public void ConfirmAction()
    {
        if (CurrentActor.CurrentAttackSkill.Available(CurrentActor))
        {
            CurrentActor.ConfirmAttackSkill();
            InputAllowed = false;
        }
        else
        {
            // output some 'not available' text?
            //GladiusGlobals.CombatEngineUI.DrawFloatingText(CurrentActor.Position, Color.Red, "Skill Not Available.", 2f);
        }
        MovementGrid.RebuildMesh = true;
    }

    public MovementGrid MovementGrid
    {
        get { return GladiusGlobals.MovementGrid; }
    }

    public Arena Arena
    {
        get { return GladiusGlobals.Arena; }
    }


    public Point ApplyMoveToGrid(ActionButton button)
    {
        Vector3 v = Vector3.zero;

        Point p = MovementGrid.CurrentCursorPoint;
        if (button == ActionButton.Move1Left)
        {
            v = Vector3.left;
        }
        else if (button == ActionButton.Move1Right)
        {
            v = Vector3.right;
        }
        else if (button == ActionButton.Move1Up)
        {
            v = Vector3.forward;
        }
        else if (button == ActionButton.Move1Down)
        {
            v = Vector3.back;
        }


        if (v.sqrMagnitude > 0)
        {
            Vector3 camerav = GladiusGlobals.CameraManager.transform.TransformDirection(v);

            camerav.y = 0;
            camerav.Normalize();

            if (Mathf.Abs(camerav.x) > Mathf.Abs(camerav.z))
            {
                if (camerav.x < 0)
                {
                    p.X--;
                }
                if (camerav.x > 0)
                {
                    p.X++;
                }
            }
            else
            {
                if (camerav.z < 0)
                {
                    p.Y--;
                }
                if (camerav.z > 0)
                {
                    p.Y++;
                }
            }

        }
        //Debug.Log("ApplyMoveToGrid " + button + "CP " + MovementGrid.CurrentPosition + "  P " + p + "  Fwd " + fwd);
        return p;


    }

    private void InitialiseSkillSlots()
    {
        for (int i = 0; i < m_skillGroupNames.Length; ++i)
        {
            UpdateSkillIcon(i, m_currentAttackSkillLine[i].Available(CurrentActor)?SkillIconState.Available:SkillIconState.Unavailable);
        }
    }


    private void HandleMovementGridAction(ActionButtonPressedArgs e)
    {
        switch (e.ActionButton)
        {
            case (ActionButton.ActionButton1):
                //Debug.Log("ActionButton");

                int pathLength = CurrentActor.WayPointList.Count;
                if (pathLength == 0)
                {
                    pathLength = 1;
                }
                if (CurrentActor.CurrentAttackSkill.InRange(pathLength))
                {
                    if (MovementGrid.CursorOnTarget(CurrentActor) )
                    {
                        BaseActor target = Arena.GetActorAtPosition(MovementGrid.CurrentCursorPoint);
                        if (GladiusGlobals.CombatEngine.IsValidTarget(CurrentActor, target, CurrentActor.CurrentAttackSkill))
                        {
                            CurrentActor.Target = target;
                            ConfirmAction();

                        }
                    }
                    else
                    {
                        // this should be just a move...?
                        ConfirmAction();
                        //if (!CurrentActor.CurrentAttackSkill.IsOkWithNoTargets)
                        //{
                        //    // some indication here that we can't confirm the action at this point.
                        //}
                        //else
                        //{
                        //    ConfirmAction();
                        //}
                    }
                }
                break;
            case (ActionButton.ActionButton2):
                {
                    CancelAction();
                    break;
                }
            case (ActionButton.Move1Left):
            case (ActionButton.Move1Right):
            case (ActionButton.Move1Up):
            case (ActionButton.Move1Down):
                {
                    //Debug.Log("MoveButton");

                    Point p = ApplyMoveToGrid(e.ActionButton);
                    if (Arena.InLevel(p))
                    {
                        MovementGrid.CurrentCursorPoint = p;
                        SquareType st = Arena.GetSquareTypeAtLocation(MovementGrid.CurrentCursorPoint);
                        BaseActor target = Arena.GetActorAtPosition(MovementGrid.CurrentCursorPoint);
                        //int pathLength = 
                        CurrentActor.WayPointList.Clear();

                        Point adjustedPoint = MovementGrid.CurrentCursorPoint;
                        //if (Arena.IsPointOccupied(adjustedPoint))
                        //{
                        //    // instead find the nearest un-occupied square
                        //    adjustedPoint = Arena.PointNearestLocation(CurrentActor.ArenaPoint, adjustedPoint);

                        //}


                        Arena.FindPath(CurrentActor.ArenaPoint, adjustedPoint, CurrentActor.WayPointList);



                    }
                    break;
                }
        }
        MovementGrid.RebuildMesh = true;
    }

    public String SpriteForDamageType(DamageType type)
    {
        switch (type)
        {
            case (DamageType.Air):
                return "AirBar";
            case (DamageType.Dark):
                return "DarkBar";
            case (DamageType.Earth):
                return "EarthBar";
            case (DamageType.Fire):
                return "FireBar";
            case (DamageType.Light):
                return "LightBar";
            case (DamageType.Water):
                return "WaterBar";
        }
        return null;
    }


    public bool InputAllowed
    {
        get;
        set;
    }


    public struct IconAndState
    {
        public IconAndState(SkillIcon icon, SkillIconState state)
        {
            Icon = icon;
            State = state;
        }

        SkillIcon Icon;
        SkillIconState State;
    }

    String atlasPath = "GladiusUI/Arena/ArenaUIAtlas";

    dfProgressBar m_healthProgressBar;
    dfProgressBar m_affinityProgressBar;
    dfProgressBar m_affinityArmourProgressBar;
    dfProgressBar m_skillPointProgressBar;

    dfAtlas m_textureAtlas;
    dfSprite[] m_skillSprites;
    dfSprite m_skillCostSprite;

    dfLabel m_skillNameLabel;
    dfLabel m_skillTypeLabel;
    dfLabel m_actorNameLabel;

    dfRichTextLabel m_debugLabel;


    private String[] m_skillGroupNames = new String[] { "Move", "Attack", "Combo", "Special", "Affinity" };

    
    private Dictionary<IconAndState, String> m_iconStateRegionDictionary = new Dictionary<IconAndState, String>();

    List<List<AttackSkill>> m_attackSkills;
    List<AttackSkill> m_currentAttackSkillLine;

    Point m_actionCursor = new Point();
}

