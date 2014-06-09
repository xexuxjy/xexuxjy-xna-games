using UnityEngine;
using System.Collections;
using Gladius.arena;
using Gladius;
using System.Collections.Generic;
using System.Text;
using System;

public class PlayerChoiceBar : MonoBehaviour
{
    public static Rect Rect = new Rect(20, 600, 0, 0);
    public static Rect ShieldBarRect = new Rect(0,0,140,50);
    public static Rect SwordBarRect = new Rect(ShieldBarRect.x + ShieldBarRect.width + 10, ShieldBarRect.y, ShieldBarRect.width, ShieldBarRect.height);
    public static Rect SkillBarRect = new Rect(SwordBarRect.x + SwordBarRect.width + 100,SwordBarRect.y,215,50);

    String atlasPath = "GladiusUI/Arena/ArenaUIAtlas";

    // Use this for initialization
    private void AddIconState(SkillIcon icon, SkillIconState state, String regionName)
    {
        m_iconStateRegionDictionary[new IconAndState(icon, state)] = regionName;
    }
    void Start()
    {
        TPackManager.load(atlasPath);

        AddIconState(SkillIcon.Move, SkillIconState.Available,"SkillMove.png");
        AddIconState(SkillIcon.Move, SkillIconState.Selected, "SkillMoveSelected.png");
        AddIconState(SkillIcon.Attack, SkillIconState.Available, "SkillAttack.png");
        AddIconState(SkillIcon.Attack, SkillIconState.Selected, "SkillAttackSelected.png");
        AddIconState(SkillIcon.Combo, SkillIconState.Available, "SkillCombo.png");
        AddIconState(SkillIcon.Combo, SkillIconState.Selected, "SkillComboSelected.png");
        AddIconState(SkillIcon.Combo, SkillIconState.Unavailable, "SkillComboUnavailable.png");
        AddIconState(SkillIcon.Special, SkillIconState.Available, "SkillSpecial.png");
        AddIconState(SkillIcon.Special, SkillIconState.Selected, "SkillSpecialSelected.png");
        AddIconState(SkillIcon.Special, SkillIconState.Unavailable, "SkillSpecialUnavailable.png");
        AddIconState(SkillIcon.Affinity, SkillIconState.Available, "SkillAffinity.png");
        AddIconState(SkillIcon.Affinity, SkillIconState.Selected, "SkillAffinitySelected.png");
        AddIconState(SkillIcon.Affinity, SkillIconState.Unavailable, "SkillAffinityUnavailable.png");

        m_largeFont = Resources.Load<Font>("GladiusUI/Arena/TREBUC");

        m_largeTextStyle = new GUIStyle();
        m_largeTextStyle.font = m_largeFont;
        m_largeTextStyle.fontSize = 16;
        m_largeTextStyle.fontStyle = FontStyle.Bold;


        m_smallTextStyle = new GUIStyle();
        m_smallTextStyle.font = m_largeFont;
        m_smallTextStyle.fontSize = 10;

        
        m_attackSkills = new List<List<AttackSkill>>();
        for (int i = 0; i < numSkillSlots; ++i)
        {
            m_attackSkills.Add(new List<AttackSkill>());
        }
        m_currentAttackSkillLine = new List<AttackSkill>(numSkillSlots);
        RegisterListeners();
    }

    const int numSkillSlots = 5;


    public void OnGUI()
    {
        DrawElement();
    }

    public void DrawElement()
    {
        try
        {
            if (CurrentActor != null)
            {
                //TextureRegion shieldRegion = m_atlas.GetRegion("ShieldSkillBar.png");
                Rect barRect = GladiusGlobals.AddRect(Rect,ShieldBarRect);
                //Debug.Log("GUI Draw");
                
                Vector2 textDims =  m_largeTextStyle.CalcSize(new GUIContent(CurrentActor.Name));
                
                Vector2 textPos = new Vector2(Rect.x + 5, Rect.y - textDims.y - 3);

                //m_largeText.text = CurrentActor.Name;
                //m_largeText.transform.position = new Vector3(textPos.x,textPos.y,0);
                //m_largeText.transform.position = new Vector3(10, 10, 0);

                GUI.Label(new Rect(textPos.x, textPos.y, textDims.x, textDims.y), CurrentActor.Name);


                //DrawSkillBar1("ShieldSkillBar.png", barRect, CurrentActor.ArmourAffinityType, CurrentActor.Health, CurrentActor.MaxHealth, CurrentActor.Affinity, CurrentActor.MaxAffinity);
                DrawSkillBar1("ShieldBar.png", barRect, CurrentActor.ArmourAffinityType, CurrentActor.Health, CurrentActor.MaxHealth, CurrentActor.Affinity, CurrentActor.MaxAffinity);

                barRect = GladiusGlobals.AddRect(Rect,SwordBarRect);
                //DrawSkillBar1("SwordSkillBar.png", barRect, CurrentActor.WeaponAffinityType, CurrentActor.ArenaSkillPoints, CurrentActor.MaxArenaSkillPoints, 1, 1);
                DrawSkillBar1("AttackBar.png", barRect, CurrentActor.WeaponAffinityType, CurrentActor.ArenaSkillPoints, CurrentActor.MaxArenaSkillPoints, 1, 1);
                barRect = GladiusGlobals.AddRect(Rect, SkillBarRect);

                DrawSkillBar2(barRect, m_currentAttackSkillLine, null, null);
            }

            Rect debugInfoRect = new Rect(50, 50, 200, 500);
            //GUI.Label(debugInfoRect, String.Format("MG : C[{0}] L[{1}] CF[{2}].",MovementGrid.CurrentPosition,MovementGrid.LastPosition,GladiusGlobals.CameraManager.transform.forward));

            Vector3 startPoint = GladiusGlobals.MovementGrid.CurrentV3 + Vector3.up;
            Ray ray = new Ray(startPoint, Vector3.down);
            String meshInfo = null;
            RaycastHit hitResult;
            Physics.Raycast(ray, out hitResult);
            if (hitResult.collider != null)
            {
                meshInfo = String.Format("Collided at point : " + hitResult.point);
            }
            else
            {
                meshInfo = "No Collision";
            }
            GUI.Label(debugInfoRect, meshInfo);



        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
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
            m_attackSkills[attackSkill.SkillRow].Add(attackSkill);
        }

        for (int i = 0; i < numSkillSlots; ++i)
        {
            m_currentAttackSkillLine.Add(m_attackSkills[i][0]);
        }
        m_actionCursor = new Point();
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

            case (ActionButton.Move1Left):
                {
                    SkillCursorLeft();
                    break;
                }
            case (ActionButton.Move1Right):
                {
                    SkillCursorRight();
                    break;
                }
            case (ActionButton.Move1Up):
                {
                    SkillCursorUp();
                    break;
                }
            case (ActionButton.Move1Down):
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
        m_actionCursor = GladiusGlobals.Add(m_actionCursor, delta);

        m_actionCursor.X = (m_actionCursor.X + m_attackSkills.Count) % m_attackSkills.Count;

        if (delta.X != 0)
        {
            m_actionCursor.Y = 0;
        }
        m_actionCursor.Y = (m_actionCursor.Y + m_attackSkills[m_actionCursor.X].Count) % m_attackSkills[m_actionCursor.X].Count;
        m_currentAttackSkillLine[m_actionCursor.X] = m_attackSkills[m_actionCursor.X][m_actionCursor.Y];
        CurrentActor.CurrentAttackSkill = CurrentlySelectedSkill;
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




    private void DrawSkillBar1(String regionName, Rect rect, DamageType damageType, float bar1Value, float bar1MaxValue, float bar2Value, float bar2MaxValue)
    {
        int smallCircleDiameter = 16;
        int smallircleYOffset = 0;

        Rect skillRect1Dims = new Rect(33, -7, 107, 16);
        Rect skillRect2Dims = new Rect(33, 15, 107, 16);


        Rect affinityRect = skillRect2Dims;
        GladiusGlobals.OffsetRect(ref rect, ref affinityRect);

        affinityRect.x -= ((float)smallCircleDiameter * 1.0f);
        affinityRect.y -= smallCircleDiameter / 2;
        affinityRect.width = affinityRect.height = smallCircleDiameter;

        Rect rect1 = new Rect(rect.x + skillRect1Dims.x, rect.y + skillRect1Dims.y + skillRect1Dims.height, skillRect1Dims.width, skillRect1Dims.height);
        Rect rect2 = new Rect(rect.x + skillRect2Dims.x, rect.y + skillRect2Dims.y + skillRect2Dims.height, skillRect2Dims.width, skillRect2Dims.height);

        TPackManager.draw(rect, atlasPath, regionName);
        
        DrawMiniBar(rect1, bar1Value, bar1MaxValue, Color.green, Color.black);
        DrawMiniBar(rect2, bar2Value, bar2MaxValue, Color.yellow, Color.black);

        //GUI.EndGroup();
    }


    private void DrawMiniBar(Rect baseRectangle, float val, float maxVal, Color color1, Color color2)
    {

        float fillPercentage = val / maxVal;

        float height = baseRectangle.height;
        float ypos = baseRectangle.y;
        float start = baseRectangle.x;
        
        float width = (fillPercentage * baseRectangle.width);
        Rect r1 = new Rect(start, ypos, width, height);
        GUI.DrawTexture(r1, GladiusGlobals.ColourTextureHelper.GetColourTexture(color1));
        start += width;
        width = baseRectangle.width - width;
        Rect r2 = new Rect(start, ypos, width, height);
    }

    private void DrawSkillBar2(Rect rect, List<AttackSkill> skills, StringBuilder bar1Text, StringBuilder bar2Text)
    {
    
        int circleRadius = 29;
        Rect skillRect1Dims = new Rect(161, 16, 115, 16);
        Rect skillRect2Dims = new Rect(26, 32, 250, 16);

        int xpad = 2;
        rect.x += xpad;
        rect.y -= 1;

        for (int i = 0; i < skills.Count; ++i)
        {
            AttackSkill skill = skills[i];
            Vector2 dims;
            Vector2 uv;

            SkillIconState iconState = (i == m_actionCursor.X) ? SkillIconState.Selected : SkillIconState.Available;
            if (!skill.Available(CurrentActor))
            {
                iconState = SkillIconState.Unavailable;
            }

            
            
            Rect dest = new Rect(rect.x + (i * (circleRadius + xpad)), rect.y + 2, circleRadius, circleRadius);
            DrawSkillIcon(dest,skill.SkillIcon, iconState);
            

            //GUI.DrawTextureWithTexCoords(dest, m_atlasTexture, src);

        }

        TPackManager.draw(rect, atlasPath, "SkillbarPart2.png");
        
        
        Vector2 pos = new Vector2(rect.x, rect.y);
        pos += new Vector2(skillRect1Dims.x, skillRect1Dims.y);
        
        //GUIContent labelName = new GUIContent(skills[m_actionCursor.X].Name);
        //GUI.Label(new Rect(pos.x, pos.y, 0, 0), labelName,m_smallTextStyle);


        //Vector2 x = m_smallTextStyle.CalcSize(labelName);

        //pos += x;
        
        GUIContent skillSlot = m_skillGroupNames[(int)CurrentlySelectedSkill.SkillIcon];
        GUI.Label(new Rect(pos.x, pos.y, 0, 0), skillSlot, m_smallTextStyle);



        String skillCostImage = null;
        if (CurrentlySelectedSkill.UseCost > 0 && CurrentlySelectedSkill.UseCost < 4)
        {
            Vector2 pos2 = new Vector2(rect.x+skillRect2Dims.x, rect.y+skillRect2Dims.y);
            GUIContent labelName = new GUIContent(skills[m_actionCursor.X].Name);
            GUI.Label(new Rect(pos2.x, pos2.y, 0, 0), labelName, m_smallTextStyle);
            Vector2 textDims = m_smallTextStyle.CalcSize(labelName);
            pos2.x += textDims.x+20;
            
            skillCostImage = m_skillCostImageNames[CurrentlySelectedSkill.UseCost - 1];
            Rect sd = new Rect(pos2.x, pos2.y, 64, 16);
            TPackManager.draw(sd, atlasPath, skillCostImage);
        }




    }

    private GUIContent[] m_skillGroupNames = new GUIContent[] { new GUIContent("Move"), new GUIContent("Attack"), new GUIContent("Combo"), new GUIContent("Special"), new GUIContent("Affinity") };

    private String[] m_skillCostImageNames = new String[]{"SkillDiamonds1.png","SkillDiamonds2.png","SkillDiamonds3.png","SkillDiamonds4.png"};

    public void DrawSkillIcon(Rect dest, SkillIcon icon, SkillIconState state)
    {
        try
        {
            String key;
            if (m_iconStateRegionDictionary.TryGetValue(new IconAndState(icon, state), out key))
            {
                TPackManager.draw(dest, atlasPath, key);
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
                        if (!CurrentActor.CurrentAttackSkill.IsOkWithNoTargets)
                        {
                            // some indication here that we can't confirm the action at this point.
                        }
                        else
                        {
                            ConfirmAction();
                        }
                    }
                }
                break;
            case (ActionButton.ActionButton2):
                {
                    CancelAction();
                    break;
                }
            case (ActionButton.Move2Left):
            case (ActionButton.Move2Right):
            case (ActionButton.Move2Up):
            case (ActionButton.Move2Down):
                {
                    //Debug.Log("MoveButton");

                    Point p = ApplyMoveToGrid(e.ActionButton);
                    if (Arena.InLevel(p))
                    {
                        Point lastPoint = MovementGrid.CurrentCursorPoint;
                        MovementGrid.CurrentCursorPoint = p;
                        SquareType st = Arena.GetSquareTypeAtLocation(MovementGrid.CurrentCursorPoint);
                        BaseActor target = Arena.GetActorAtPosition(MovementGrid.CurrentCursorPoint);
                        //int pathLength = 
                        CurrentActor.WayPointList.Clear();

                        Point adjustedPoint = MovementGrid.CurrentCursorPoint;
                        if (target != null && target != CurrentActor)
                        {
                            adjustedPoint = lastPoint;
                        }

                        Arena.FindPath(CurrentActor.ArenaPoint, adjustedPoint, CurrentActor.WayPointList);
                    }
                    break;
                }
        }
        MovementGrid.RebuildMesh = true;
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


    private Dictionary<IconAndState, String> m_iconStateRegionDictionary = new Dictionary<IconAndState, String>();

    List<List<AttackSkill>> m_attackSkills;
    List<AttackSkill> m_currentAttackSkillLine;

    Point m_actionCursor = new Point();



    // this may need to be somewhere more common...
    //Dictionary<DamageType, Texture2D> m_damageTypeTextures = new Dictionary<DamageType, Texture2D>();
    Dictionary<DamageType, Texture2D> m_damageTypeTextures = new Dictionary<DamageType, Texture2D>();

    GUIStyle m_largeTextStyle;
    GUIStyle m_smallTextStyle;


    GUIText m_largeText;
    GUIText m_smallText;


    Point m_topLeft = new Point(20, 700);
    Font m_smallFont;
    Font m_largeFont;
}

