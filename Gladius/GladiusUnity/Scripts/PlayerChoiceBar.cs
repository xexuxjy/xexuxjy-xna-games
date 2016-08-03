using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;

public class PlayerChoiceBar : MonoBehaviour
{
    public void ShowHideSkillPanel(object o)
    {
    }

    //    public float GridMoveDelay = 0.2f;
    //    public float CurrentMoveDelay = 0.0f;
    //    public bool HaveMovedGrid = false;
    //    public CombatEngine CombatEngine;
    //    public CameraManager CameraManager;
    //    public Arena Arena;

    //    void Start()
    //    {
    //        m_uiCanvasPanel = GameObject.Find("UICanvasPanel");
    //        m_playerSkillsPanelRT = GameObject.Find("PlayerSkills").GetComponent<RectTransform>();

    //        for (int i = 0; i < numSkillSlots; ++i)
    //        {
    //            List<AttackSkill> skillList = new List<AttackSkill>();
    //            m_attackSkills.Add(skillList);
    //            Button button = GameObject.Find("SkillSlot" + (i + 1)).GetComponent<Button>();
    //            var handler = button.GetComponent<SkillButtonEventHandler>();
    //            handler.SkillList = skillList;
    //            handler.PlayerChoiceBar = this;
    //            handler.Button = button;
    //        }

    //        //m_confirmActionButton.onClick.AddListener(()=>OnConfirmAction());

    //        //m_skillChoiceScrollPanel = GameObject.Find("SkillChoicePanel");
    //        //m_skillChoiceSlotPrefab = GameObject.Find("SkillChoiceSlotPrefab");

    //        m_healthProgressBar = GladiusGlobals.FindChild("HealthProgress", transform).GetComponent<Progress>();
    //        m_affinityProgressBar = GladiusGlobals.FindChild("AffinityPointProgress", transform).GetComponent<Progress>();
    //        m_affinityArmourProgressBar = GladiusGlobals.FindChild("AffinityArmourProgress", transform).GetComponent<Progress>();
    //        m_skillPointProgressBar = GladiusGlobals.FindChild("SkillPointProgress", transform).GetComponent<Progress>();


    //        m_skillNameLabel = GladiusGlobals.FindChild("SkillNameLabel", transform).GetComponent<Text>();
    //        m_actorNameLabel = GladiusGlobals.FindChild("ActorNameLabel", transform).GetComponent<Text>();

    //        m_debugLabel = GladiusGlobals.FindChild("DebugText", transform).GetComponent<Text>();

    //        RegisterListeners();



    //        SetupDebugPanel();
    //    }

    //    private SkillButtonEventHandler m_lastHandlerCalled = null;


    //    public void ShowHideSkillPanel(SkillButtonEventHandler eventHandler)
    //    {
    //        if (m_lastHandlerCalled == eventHandler)
    //        {
    //            HideSkillPanel();
    //        }
    //        else
    //        {
    //            m_lastHandlerCalled = eventHandler;
    //            if (!m_skillChoiceScrollPanel.activeSelf)
    //            {
    //                m_skillChoiceScrollPanel.SetActive(true);
    //            }

    //            if (m_skillChoiceScrollPanel.activeSelf)
    //            {
    //                List<AttackSkill> skillList = eventHandler.SkillList;

    //                foreach (Transform t in m_skillChoiceScrollPanel.transform)
    //                {
    //                    Destroy(t.gameObject);
    //                }

    //                int count = 0;
    //                foreach (AttackSkill attackSkill in skillList)
    //                {
    //                    GameObject childPanel = Instantiate(m_skillChoiceSlotPrefab);
    //                    childPanel.SetActive(true);
    //                    SkillChoiceTileScript skillChoiceTileScript = childPanel.GetComponent<SkillChoiceTileScript>();
    //                    skillChoiceTileScript.PlayerChoiceBar = this;
    //                    skillChoiceTileScript.AttackSkill = attackSkill;
    //                    childPanel.transform.SetParent(m_skillChoiceScrollPanel.transform);

    //                    Button button = childPanel.GetComponent<Button>();
    //                    button.onClick.AddListener(() => { SkillSelection_Click(skillChoiceTileScript.AttackSkill); });
    //                    childPanel.GetComponentInChildren<Text>().text = GladiusGlobals.GameStateManager.LocalisationData[attackSkill.DisplayNameId];

    //                    if (!CurrentActor.IsSkillAvailable(attackSkill, false))
    //                    {
    //                        button.interactable = false;
    //                    }

    //                    count++;
    //                    if (count > 3)
    //                    {
    //                        break;
    //                    }

    //                }

    //                RectTransform rt = m_playerSkillsPanelRT;
    //                Vector3 offset = new Vector3(rt.rect.xMin, rt.rect.yMax, 0);
    //                m_skillChoiceScrollPanel.transform.position = rt.position + offset;
    //            }

    //        }
    //    }

    //    public void SkillSelection_Click(AttackSkill attackSkill)
    //    {
    //        CurrentlySelectedSkill = attackSkill;
    //        m_skillChoiceScrollPanel.SetActive(false);
    //    }

    //    public void HideSkillPanel()
    //    {
    //        foreach (Transform t in m_skillChoiceScrollPanel.transform)
    //        {
    //            Destroy(t.gameObject);
    //        }
    //        m_skillChoiceScrollPanel.SetActive(false);
    //        m_lastHandlerCalled = null;
    //    }


    //    public void OnConfirmAction()
    //    {
    //        SelectTargetAtPoint(CurrentActor.MovementGrid.CurrentCursorPoint);

    //        CurrentActor.ConfirmAction();
    //        if (CurrentActor.ActionConfirmed)
    //        {
    //            InputAllowed = false;
    //        }

    //    }

    //    //private void ConfirmActionButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //    //{
    //    //    CurrentActor.ConfirmAction();
    //    //    if (CurrentActor.ActionConfirmed)
    //    //    {
    //    //        InputAllowed = false;
    //    //    }
    //    //}

    //    public void Update()
    //    {
    //        if (HaveMovedGrid)
    //        {
    //            CurrentMoveDelay += Time.deltaTime;
    //            if (CurrentMoveDelay > GridMoveDelay)
    //            {
    //                HaveMovedGrid = false;
    //                CurrentMoveDelay = 0f;
    //            }

    //        }

    //        UpdateDebugControls();

    //        if (WasKeyPressed(KeyCode.U))
    //        {
    //            if (m_uiCanvasPanel != null)
    //            {
    //                m_uiCanvasPanel.SetActive(!m_uiCanvasPanel.activeSelf);
    //            }
    //        }


    //        if (CurrentActor != null && CurrentlySelectedSkill != null)
    //        {
    //            m_healthProgressBar.MaxValue = CurrentActor.MaxHealth;
    //            m_healthProgressBar.Value = CurrentActor.Health;
    //            m_skillNameLabel.text = GladiusGlobals.GameStateManager.LocalisationData[CurrentlySelectedSkill.DisplayNameId];
    //            m_skillPointProgressBar.Value = CurrentActor.ArenaSkillPoints;

    //            String spriteName = SpriteForDamageType(CurrentActor.WeaponAffinityType);
    //            if (spriteName != null)
    //            {
    //                m_affinityArmourProgressBar.enabled = true;
    //                m_affinityProgressBar.MaxValue = CurrentActor.MaxAffinity;
    //                m_affinityProgressBar.Value = CurrentActor.Affinity;
    //                //m_affinityProgressBar.ForegroundImage = spriteName;
    //            }
    //            else
    //            {
    //                m_affinityArmourProgressBar.enabled = false;
    //            }
    //        }

    //        if (CurrentActor != null)
    //        {
    //            StringBuilder sb = new StringBuilder();
    //            sb.AppendFormat("Current [{0}] HP[{1}] SP[{2}] [{3}]\n", CurrentActor.Name, CurrentActor.Health, CurrentActor.ArenaSkillPoints, CurrentActor.CurrentAnimEnum);
    //            sb.AppendFormat("Pos [{0},{1}] Height [{2}]\n", CurrentActor.ArenaPoint.X, CurrentActor.ArenaPoint.Y, Arena.GetHeightAtLocation(CurrentActor.ArenaPoint));
    //            sb.AppendFormat("Cursor Pos[{0},{1}] Height [{2}]\n", CurrentActor.MovementGrid.CurrentCursorPoint.X, CurrentActor.MovementGrid.CurrentCursorPoint.Y, Arena.GetHeightAtLocation(CurrentActor.MovementGrid.CurrentCursorPoint));
    //            BaseActor target = Arena.GetActorAtPosition(CurrentActor.MovementGrid.CurrentCursorPoint);
    //            if (target != null && CombatEngine != null)
    //            {
    //                sb.AppendFormat("Target [{0}] HP[{1}] SP[{2}]\n", target.Name, target.Health, target.ArenaSkillPoints);
    //                if (CombatEngine.IsValidTarget(CurrentActor, target, CurrentActor.CurrentAttackSkill))
    //                {
    //                    float damage = CombatEngine.CalculateExpectedDamage(CurrentActor, target, CurrentActor.CurrentAttackSkill);
    //                    sb.AppendFormat("Skill [{0}] Damage [{1}]", CurrentActor.CurrentAttackSkill.Name, damage);
    //                }
    //                else
    //                {
    //                    sb.Append("Invalid Target");
    //                }
    //            }
    //            m_debugLabel.text = sb.ToString();
    //        }

    //        //HandleMovementGridAction();
    //        HandleMovementGridPad();
    //    }

    //    public void RegisterListeners()
    //    {
    //        //EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
    //        EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);
    //    }


    //    public void UnregisterListeners()
    //    {
    //        //EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);
    //    }


    //    void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
    //    {
    //        CurrentActor = e.New;
    //    }

    //    public void BuildDataForActor()
    //    {
    //        foreach (List<AttackSkill> list in m_attackSkills)
    //        {
    //            list.Clear();
    //        }

    //        CurrentActor.FilterAttackSkillsDisplay();

    //        foreach (AttackSkill attackSkill in CurrentActor.AvailableAttackSkills)
    //        {
    //            if (attackSkill.SkillRow >= 0 && attackSkill.SkillRow < m_attackSkills.Count)
    //            {
    //                m_attackSkills[attackSkill.SkillRow].Add(attackSkill);
    //            }
    //        }
    //        if (m_attackSkills.Count > 0 && m_attackSkills[0].Count > 0)
    //        {
    //            CurrentlySelectedSkill = m_attackSkills[0][0];
    //        }
    //        else
    //        {
    //            CurrentlySelectedSkill = null;
    //        }
    //        m_healthProgressBar.MaxValue = CurrentActor.MaxHealth;
    //        m_actorNameLabel.text = CurrentActor.Name;

    //        if (CurrentlySelectedSkill != null)
    //        {
    //            int useCost = CurrentlySelectedSkill.UseCost - 1;
    //            m_skillPointProgressBar.Value = useCost;
    //        }
    //    }


    //    private BaseActor m_currentActor;
    //    public BaseActor CurrentActor
    //    {
    //        get
    //        {
    //            return m_currentActor;
    //        }
    //        set
    //        {
    //            m_currentActor = value;
    //            BuildDataForActor();
    //            CurrentlySelectedSkill = CurrentActor.BasicAttackSkill;
    //            CurrentActor.CurrentAttackSkill = CurrentlySelectedSkill;
    //            CameraManager.CurrentCameraMode = GladiusCameraMode.Normal;
    //            CurrentActor.MovementGrid.CurrentCursorPoint = CurrentActor.ArenaPoint;
    //            CameraManager.ReparentTarget(m_currentActor.MovementGrid.MovementGridCursor.transform);
    //            Quaternion characterForward = Quaternion.LookRotation(m_currentActor.transform.forward);
    //            CameraManager.CameraTarget.transform.rotation = characterForward;
    //            InputAllowed = true;
    //        }
    //    }
    //    public AttackSkill CurrentlySelectedSkill
    //    {
    //        get
    //        {
    //            if (CurrentActor != null)
    //            {
    //                return CurrentActor.CurrentAttackSkill;
    //            }
    //            return null;
    //        }

    //        set
    //        {
    //            if (CurrentActor != null && value != null)
    //            {
    //                CurrentActor.CurrentAttackSkill = value;
    //                m_skillNameLabel.text = value.Name;
    //            }
    //        }
    //    }

    //    public void CancelAction()
    //    {
    //        CurrentActor.CancelAction();
    //    }

    //    public TurnManager TurnManager
    //    {
    //        get
    //        {
    //            return TurnManager;
    //        }
    //    }


    //    public Point ApplyMoveToGrid(Vector3 movement)
    //    {
    //        Vector3 v = movement;

    //        Point p = CurrentActor.MovementGrid.CurrentCursorPoint;

    //        if (v.sqrMagnitude > 0)
    //        {
    //            Vector3 camerav = CameraManager.transform.TransformDirection(v);

    //            camerav.y = 0;
    //            camerav.Normalize();

    //            if (Mathf.Abs(camerav.x) > Mathf.Abs(camerav.z))
    //            {
    //                if (camerav.x < 0)
    //                {
    //                    p.X--;
    //                }
    //                if (camerav.x > 0)
    //                {
    //                    p.X++;
    //                }
    //            }
    //            else
    //            {
    //                if (camerav.z < 0)
    //                {
    //                    p.Y--;
    //                }
    //                if (camerav.z > 0)
    //                {
    //                    p.Y++;
    //                }
    //            }

    //        }
    //        //Debug.Log("ApplyMoveToGrid " + button + "CP " + MovementGrid.CurrentPosition + "  P " + p + "  Fwd " + fwd);
    //        return p;


    //    }

    //    public void HandleMovementGridPad()
    //    {
    //        //Vector2 leftHandJoystickPosition = dfTouchJoystick.GetJoystickPosition("CharacterMovement");
    //        //Vector3 v3 = new Vector3(leftHandJoystickPosition.x, 0, leftHandJoystickPosition.y);
    //        //HandleMovementGridAction(v3);
    //    }

    //    private void HandleMovementGridAction(Vector3 v3)
    //    {
    //        if (!InputAllowed)
    //        {
    //            return;
    //        }

    //        if (v3.magnitude > 0f && HaveMovedGrid == false)
    //        {
    //            BaseActor target = Arena.GetActorAtPosition(CurrentActor.MovementGrid.CurrentCursorPoint);
    //            Point newPoint = ApplyMoveToGrid(v3);
    //            if (Arena.ValidPointInLevel(newPoint))
    //            {
    //                BaseActor newPointTarget = Arena.GetActorAtPosition(newPoint);
    //                // check to see if we're moving past target, though moving back to last waypoint is allowed..
    //                if (target != null && target != CurrentActor)
    //                {
    //                    if (CurrentActor.WayPointList.Count > 0)
    //                    {
    //                        Point lastWayPoint = CurrentActor.WayPointList[CurrentActor.WayPointList.Count - 1];
    //                        if (newPoint != lastWayPoint)
    //                        {
    //                            return;
    //                        }
    //                    }
    //                }

    //                HaveMovedGrid = true;
    //                Point currentCursorPoint = CurrentActor.MovementGrid.CurrentCursorPoint;
    //                if (!CurrentActor.ActionSelected)
    //                {
    //                    //if (!Arena.IsPointOccupied(newPoint))
    //                    if(newPointTarget == null)
    //                    {
    //                        CurrentActor.MovementGrid.CurrentCursorPoint = newPoint;

    //                        int pointIndex = CurrentActor.WayPointList.IndexOf(newPoint);
    //                        if (pointIndex == -1)
    //                        {
    //                            // Back to start point.
    //                            if (newPoint == CurrentActor.ArenaPoint)
    //                            {
    //                                CurrentActor.FocusCamera(CameraManager);
    //                                CurrentActor.WayPointList.Clear();
    //                            }
    //                            else
    //                            {
    //                                CurrentActor.MovementGrid.FocusCamera(CameraManager, v3);

    //                                if (CurrentActor.WayPointList.Count == 0 ||
    //                                    Arena.IsNextTo(CurrentActor.WayPointList[CurrentActor.WayPointList.Count - 1], newPoint))
    //                                {
    //                                    CurrentActor.AddWayPoint(newPoint);
    //                                }
    //                            }
    //                        }
    //                        else
    //                        {
    //                            int numRemove = CurrentActor.WayPointList.Count - pointIndex - 1;
    //                            // removerange includes the index point so step over.
    //                            CurrentActor.WayPointList.RemoveRange(pointIndex + 1, numRemove);
    //                        }

    //                    }
    //                    else
    //                    {
    //                        if (newPointTarget != CurrentActor)
    //                        {
    //                            SelectTargetAtPoint(newPoint);
    //                        }
    //                        else
    //                        {
    //                            CurrentActor.WayPointList.Clear();
    //                            CurrentActor.MovementGrid.CurrentCursorPoint = CurrentActor.ArenaPoint;
    //                        }
    //                    }

    //                }
    //                else
    //                {
    //                    // 2 stage thing here
    //                    if (GladiusGlobals.PathDistance(CurrentActor.ActionSelectedPoint, newPoint) <= 1)
    //                    {
    //                        SelectTargetAtPoint(newPoint);
    //                    }
    //                }
    //                Quaternion targetRot = Quaternion.LookRotation(v3);
    //                //targetRot *= Quaternion.LookRotation(CameraManager.CameraTarget.transform.forward);
    //                // we've doen this from the players forward.
    //                Point cameraDiff = new Point();
    //                if (CurrentActor.WayPointList.Count > 0)
    //                {
    //                    if (CurrentActor.WayPointList.Count == 1)
    //                    {
    //                        Point cursorPos = CurrentActor.WayPointList[0];
    //                        Point actorPos = CurrentActor.ArenaPoint;
    //                        cameraDiff = GladiusGlobals.Subtract(cursorPos, actorPos);
    //                        //targetRot = Quaternion.LookRotation(CurrentActor.transform.forward) * targetRot;

    //                    }
    //                    else
    //                    {
    //                        Point cursorPos = CurrentActor.WayPointList[CurrentActor.WayPointList.Count -1];
    //                        Point actorPos = CurrentActor.WayPointList[CurrentActor.WayPointList.Count - 2];
    //                        cameraDiff = GladiusGlobals.Subtract(cursorPos, actorPos);

    //                        //targetRot = Quaternion.LookRotation(CameraManager.CameraTarget.transform.forward) * targetRot;
    //                    }

    //                    targetRot = Quaternion.LookRotation(new Vector3(cameraDiff.X, 0, cameraDiff.Y));
    //                }

    //                // if we were going forward don't flip on going back?
    //                //if (v3 != Vector3.back)
    //                {
    //                    //CameraManager.CameraTarget.transform.localRotation = targetRot;
    //                    CameraManager.CameraTarget.transform.rotation = targetRot;
    //                }

    //                CurrentActor.MovementGrid.RebuildMesh = true;
    //            }
    //        }
    //    }

    //    private void SelectTargetAtPoint(Point p)
    //    {
    //        // point is occupied, if it's an enemy then we can use that as a valid target
    //        // and confirm the action
    //        BaseActor targetActor = Arena.GetActorAtPosition(p);
    //        if (CombatEngine.IsValidTarget(CurrentActor, targetActor, CurrentActor.CurrentAttackSkill))
    //        {
    //            CurrentActor.MovementGrid.CurrentCursorPoint = p;
    //            CurrentActor.Target = targetActor;
    //        }
    //    }

    //    public String SpriteForDamageType(DamageType type)
    //    {
    //        switch (type)
    //        {
    //            case (DamageType.Air):
    //                return "AirBar";
    //            case (DamageType.Dark):
    //                return "DarkBar";
    //            case (DamageType.Earth):
    //                return "EarthBar";
    //            case (DamageType.Fire):
    //                return "FireBar";
    //            case (DamageType.Light):
    //                return "LightBar";
    //            case (DamageType.Water):
    //                return "WaterBar";
    //        }
    //        return null;
    //    }


    //    public bool InputAllowed
    //    {
    //        get;
    //        set;
    //    }


    //    public void SetupDebugPanel()
    //    {
    //        //dfPanel debugPanel = GameObject.Find("DebugPanel").GetComponent<dfPanel>();
    //        //Button gridNextButton = debugPanel.Find<Button>("GridNext");
    //        //Button gridPrevButton = debugPanel.Find<Button>("GridPrev");
    //        //Button gridDefaultButton = debugPanel.Find<Button>("GridDefault");
    //        //Button gridFlipXButton = debugPanel.Find<Button>("FlipX");
    //        //Button gridFlipYButton = debugPanel.Find<Button>("FlipY");

    //        //gridNextButton.Click += GridNextButton_Click;
    //        //gridPrevButton.Click += GridPrevButton_Click;
    //        //gridDefaultButton.Click += GridDefaultButton_Click;
    //        //gridFlipXButton.Click += GridFlipXButton_Click;
    //        //gridFlipYButton.Click += GridFlipYButton_Click;

    //        //CameraManager.CurrentCameraMode = CameraMode.BirdsEye;
    //    }

    //    //private void GridFlipYButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //    //{
    //    //    Arena.FlipY();
    //    //}

    //    //private void GridFlipXButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //    //{
    //    //    Arena.FlipX();
    //    //}

    //    //private void GridDefaultButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //    //{
    //    //    Arena.RestoreDefaultGridFile();
    //    //}

    //    //private void GridPrevButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //    //{
    //    //    Arena.PrevGridFile();
    //    //}

    //    //private void GridNextButton_Click(dfControl control, dfMouseEventArgs mouseEvent)
    //    //{
    //    //    Arena.NextGridFile();
    //    //}

    //    public void OnGUI()
    //    {
    //        UpdateDebugControls();
    //    }

    //    // Update is called once per frame
    //    void UpdateDebugControls()
    //    {

    //        float moveRate = 0.3f;
    //        if (Input.GetKey(KeyCode.PageDown))
    //        {
    //            Camera.main.transform.position += new Vector3(0, -moveRate, 0);
    //        }
    //        if (Input.GetKey(KeyCode.PageUp))
    //        {
    //            Camera.main.transform.position += new Vector3(0, moveRate, 0);
    //        }
    //        if (Input.GetKey(KeyCode.LeftArrow))
    //        {
    //            Camera.main.transform.position += new Vector3(-moveRate, 0, 0);
    //        }
    //        if (Input.GetKey(KeyCode.RightArrow))
    //        {
    //            Camera.main.transform.position += new Vector3(moveRate, 0, 0);
    //        }
    //        if (Input.GetKey(KeyCode.UpArrow))
    //        {
    //            Camera.main.transform.position += new Vector3(0, 0, moveRate);
    //        }
    //        if (Input.GetKey(KeyCode.DownArrow))
    //        {
    //            Camera.main.transform.position += new Vector3(0, 0, -moveRate);
    //        }


    //        if (CurrentActor != null)
    //        {
    //            Vector3 movement = Vector3.zero;
    //            Point currentPoint = CurrentActor.MovementGrid.CurrentCursorPoint;
    //            if (WasKeyPressed(KeyCode.I))
    //            {
    //                movement = new Vector3(0, 0, 1);
    //            }
    //            if (WasKeyPressed(KeyCode.J))
    //            {
    //                movement = new Vector3(-1, 0, 0);
    //            }
    //            if (WasKeyPressed(KeyCode.K))
    //            {
    //                movement = new Vector3(0, 0, -1);
    //            }
    //            if (WasKeyPressed(KeyCode.L))
    //            {
    //                movement = new Vector3(1, 0, 0);
    //            }
    //            if (WasKeyPressed(KeyCode.O))
    //            {
    //                OnConfirmAction();
    //                //ConfirmActionButton_Click(null, null);
    //            }
    //            if (WasKeyPressed(KeyCode.P))
    //            {
    //                CancelAction();
    //            }

    //            if (movement.sqrMagnitude > 0f)
    //            {
    //                CameraManager.CurrentCameraMode = GladiusCameraMode.MovementCursor;
    //                HandleMovementGridAction(movement);
    //                //CurrentActor.MovementGrid.CurrentCursorPoint = GladiusGlobals.Add(currentPoint, newPointOffset);
    //                //CurrentActor.MovementGrid.RebuildActorTest();
    //                //HaveMovedGrid = true;
    //                //Debug.LogFormat("Actor {0} old {1} new {2}", CurrentActor.name, currentPoint, CurrentActor.MovementGrid.CurrentCursorPoint);
    //            }
    //        }

    //    }


    //    public bool WasKeyPressed(KeyCode keyCode)
    //    {
    //        bool currentState = false;
    //        bool newState = false;
    //        if (m_pressedDictionary.TryGetValue(keyCode, out currentState))
    //        {
    //            newState = Input.GetKeyDown(keyCode);
    //        }
    //        m_pressedDictionary[keyCode] = newState;
    //        return (!currentState && newState);
    //    }
    //    public Dictionary<KeyCode, bool> m_pressedDictionary = new Dictionary<KeyCode, bool>();

    //    GameObject m_uiCanvasPanel;

    //    Progress m_healthProgressBar;
    //    Progress m_affinityProgressBar;
    //    Progress m_affinityArmourProgressBar;
    //    Progress m_skillPointProgressBar;

    //    Text m_skillNameLabel;
    //    Text m_actorNameLabel;
    //    public GameObject m_skillChoiceScrollPanel;
    //    public GameObject m_skillChoiceSlotPrefab;

    //    Text m_debugLabel;

    //    private String[] m_skillGroupNames = new String[] { "Move", "Attack", "Combo", "Affinity", "Special" };

    //    List<List<AttackSkill>> m_attackSkills = new List<List<AttackSkill>>();

    //    List<Button> m_controlButtons = new List<Button>();

    //    private Button m_confirmActionButton;
    //    private RectTransform m_playerSkillsPanelRT;

    //    const int numSkillSlots = 5;

}

