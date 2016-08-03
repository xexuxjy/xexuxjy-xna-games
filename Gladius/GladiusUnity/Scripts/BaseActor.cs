using UnityEngine;
using System.Collections.Generic;
using System;
using Gladius;

//namespace Gladius

public class BaseActor : MonoBehaviour
{
    public float MovementSpeed = 4f;
    public float TurnTime = 0.5f;
    public String m_name;

    public GameObject m_healthBar;
    public GladiusAnim m_animation;
    public Crowd Crowd;
    public CombatEngine CombatEngine;
    public CombatEngineUI CombatEngineUI;
    public Arena Arena;
    public CameraManager CameraManager;

    public MovementGrid MovementGrid
    {
        get; set;
    }

    public Color TeamColour
    { get { return Color.green; } }

    public bool UnitActive;

    public void Start()
    {
        m_animation = GetComponent<GladiusAnim>();
        m_animation.BaseActor = this;
        m_transformBone = GladiusGlobals.FuzzyFindChild("character", transform);
        m_zeroBone = GladiusGlobals.FuzzyFindChild("zero", transform);
        m_leftFootBone = GladiusGlobals.FuzzyFindChild("legBall_L", transform);
        m_rightFootBone = GladiusGlobals.FuzzyFindChild("legBall_R", transform);
        m_skullBone = GladiusGlobals.FuzzyFindChild("faceSkull", transform);
        m_cameraBone = GladiusGlobals.FuzzyFindChild("Camera", transform);
        if (m_cameraBone != null)
        {
            m_cameraBone.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        }
    }

    public void Update()
    {
        if(!DisableNormalUpdates)
        {
            UpdateModelTransform();
            CheckAnimationEvents();

            SetupHealthBar();


            if (UnitActive)
            {
                if (KnockedDown && m_knockDownTurns > 0)
                {
                    m_knockDownTurns--;
                    if (m_knockDownTurns == 0)
                    {
                        GetUp();
                    }
                }

                if (!TurnComplete)
                {
                    UpdateMovement();
                    //UpdateAttackSkill();
                }
                TurnComplete = CheckTurnComplete();

            }

            if (m_knockedBack)
            {
                UpdateKnockBack();
            }

            CheckState();
        }
    }


    public bool DisableNormalUpdates
    {
        get; set;
    }

    public void UpdateModelTransform()
    {
        Vector3 adjustedPos = _pos;
        if (m_zeroBone && m_leftFootBone && m_rightFootBone && m_transformBone)
        {
            // lower of foot positions.
            //float offset = ((m_zeroBone.position.y - m_leftFootBone.position.y) + (m_zeroBone.position.y - m_rightFootBone.position.y)) / 2.0f;
            //float offset = Math.Max(m_zeroBone.position.y - m_leftFootBone.position.y, m_zeroBone.position.y - m_rightFootBone.position.y);
            float offset = Math.Max(m_transformBone.position.y - m_leftFootBone.position.y, m_transformBone.position.y - m_rightFootBone.position.y);
            offset = Math.Max(offset, m_transformBone.position.y - m_zeroBone.position.y);
            adjustedPos.y += offset;
        }
        //transform.position = adjustedPos;
        // handle being in arena, or being in render window.
        if (transform.parent == null)
        {
            transform.position = adjustedPos;
        }
        else
        {
            transform.localPosition = adjustedPos;
        }
    }

    public void FocusCamera(CameraManager cameraManager)
    {
        if (cameraManager != null)
        {
            Transform target = m_cameraBone != null ? m_cameraBone : transform;
            //Transform target = m_zeroBone!= null ? m_zeroBone : transform;
            cameraManager.ReparentTarget(target);
        }
    }

    public void FocusCamera(CameraManager cameraManager,Vector3 forward)
    {
        if (cameraManager != null)
        {
            Transform target = m_cameraBone != null ? m_cameraBone : transform;
            cameraManager.ReparentTarget(target,forward);
        }
    }



    #region BasicSetup
    public void SetupMovementGrid()
    {
        UnityEngine.Object resourceToInstantiate = Resources.Load("prefabs/MovementGridPrefab");
        if (resourceToInstantiate)
        {
            GameObject movementGridGameObject = Instantiate(resourceToInstantiate) as GameObject;
            if (movementGridGameObject)
            {
                // keep it tidy
                MovementGrid = movementGridGameObject.GetComponent<MovementGrid>();
                MovementGrid.BaseActor = this;
                MovementGrid.Arena = Arena;
                MovementGrid.CombatEngine = CombatEngine;
            }
        }
    }

    public void SetStartPosition(Point p)
    {
        m_characterData.StartPosition = p;
        ArenaPoint = m_characterData.StartPosition.Value;
        MovementGrid.CurrentCursorPoint = ArenaPoint;
    }

    public void SetupCharacterData(CharacterData characterData)
    {
        SetupMovementGrid();

        m_characterData = characterData;
        CharacterMeshHolder meshHolder = GetComponent<CharacterMeshHolder>();
        meshHolder.SetupCharacterData(m_characterData, gameObject, false);

        gameObject.SetActive(true);

        if (TurnManager != null)
        {
            TurnManager.AddActor(this);
        }

        Health = MaxHealth = CON * 10;

        SetupSkills();
        QueueAnimation(ChooseIdleAim());
    }

    public void QueueAnimation(AnimationEnum animEnum)
    {
        if (m_animation != null && animEnum != AnimationEnum.None)
        {
            m_animation.QueueAnimation(animEnum);
        }
    }

    public void SetupHealthBar()
    {
        //if (!m_healthBar)
        //{
        //    object temp = Resources.Load("Prefabs/HealthBarCanvasPrefab");
        //    GameObject healthBarPrefab = Resources.Load<GameObject>("Prefabs/HealthBarCanvasPrefab");
        //    if (healthBarPrefab != null)
        //    {
        //        m_healthBar = (GameObject)GameObject.Instantiate(healthBarPrefab);
        //        m_healthBar.name = Name + "-HealthBar";
        //        // parent it to the gui
        //        m_healthBar.transform.parent = m_zeroBone;
        //        m_healthBar.transform.localPosition = new Vector3(0, 0, 1.5f);
        //    }
        //    else
        //    {
        //        Debug.LogError("Can't find health bar prefab.");
        //    }
        //}

    }

    public void ClearWaypoints()
    {
        WayPointList.Clear();
        m_wayPointIndex = -1;
        FollowingWayPoints = false;

    }

    public void SetupSkills()
    {
        // simple for now.
        m_knownAttacks.Clear();
        foreach (string skillname in m_characterData.m_skillList)
        {
            if (AttackSkillDictionary.Data.ContainsKey(skillname))
            {
                m_knownAttacks.Add(AttackSkillDictionary.Data[skillname]);
            }
            else
            {
                Debug.LogWarning("Can't find key : " + skillname);
            }
        }

    }

    #endregion




    #region Combat
    private void DamagePoint()
    {
        // only do this at the animation hitpoint.
        if (CombatEngine != null && !DoneDamage && Target != null)
        {
            DoneDamage = true;
            CombatEngine.ResolveAttack(this, Target, CurrentAttackSkill);
        }
    }

    public void Block(BaseActor actor)
    {
        SnapToFace(actor);
        //QueueAnimation(AnimationEnum.BlockStart);
        //QueueAnimation(AnimationEnum.BlockPose);
        //QueueAnimation(AnimationEnum.BlockEnd);
        QueueAnimation(AnimationEnum.Block);
    }


    public void TakeDamage(AttackResult attackResult)
    {
        if (attackResult.resultType == AttackResultType.Blocked)
        {
            Block(attackResult.damageCauser);
        }
        else if (attackResult.resultType == AttackResultType.Miss)
        {
            StartMiss(attackResult.damageCauser);
        }
        else if (attackResult.resultType != AttackResultType.Miss)
        {
            Health -= attackResult.damageDone;
            UpdateThreatList(attackResult.damageCauser);

            // figure out where the attack came from, and how strong it was.

            Vector3 attackSide = (attackResult.damageCauser.Position - Position).normalized;

            float fwdDot = Vector3.Dot(attackSide, transform.forward);
            float sideDot = Vector3.Dot(attackSide, transform.right);

            AnimationEnum reactEnum = AnimationEnum.None;

            // main was front back
            if (Math.Abs(fwdDot) > Math.Abs(sideDot))
            {
                if (fwdDot >= 0)
                {
                    reactEnum = attackResult.isStrongAttack ? AnimationEnum.ReactHitStrongF : AnimationEnum.ReactHitWeakF;
                }
                else
                {
                    reactEnum = attackResult.isStrongAttack ? AnimationEnum.ReactHitStrongB : AnimationEnum.ReactHitWeakB;
                }
            }
            else
            {
                if (sideDot >= 0)
                {
                    reactEnum = attackResult.isStrongAttack ? AnimationEnum.ReactHitStrongR : AnimationEnum.ReactHitWeakR;
                }
                else
                {
                    reactEnum = attackResult.isStrongAttack ? AnimationEnum.ReactHitStrongL : AnimationEnum.ReactHitWeakL;
                }
            }

            QueueAnimation(reactEnum);
        }
    }

    private void UpdateThreatList(BaseActor actor)
    {
        // todo
    }

    private void UpdateKnockBack()
    {
        Vector3 targetPoint = Arena.GridPointToWorld(ArenaPoint);
        Vector3 diff = Position - targetPoint;
        diff.Normalize();
        float closeEnough = 0.01f;
        if (diff.sqrMagnitude < closeEnough)
        {
            m_knockedBack = false;
        }
        else
        {
            Position += diff * (float)Time.deltaTime * MovementSpeed;
        }

    }
    public void StartAttack()
    {
        if (CombatEngine.IsAttackerInRange(this, Target))
        {
            StartedAction = true;
            if (CurrentAttackSkill.FaceOnAttack)
            {
                SnapToFace(Target);
            }
            Target.SnapToFace(this);

            //ChooseAttackSkill();
            // use the combat engine to display skill choice
            if (CombatEngineUI != null)
            {
                CombatEngineUI.DrawFloatingText(this.Position, Color.yellow, CurrentAttackSkill.Name, 1f);
            }

            GladiusGlobals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Attack started on [{1}] Skill[{2}].", Name, Target != null ? Target.Name : "NoActorTarget", CurrentAttackSkill.Name));
            AnimationEnum attackAnim = CurrentAttackSkill.Animation != AnimationEnum.None ? CurrentAttackSkill.Animation : AnimationEnum.Attack1;
            QueueAnimation(attackAnim);
        }
    }

    public void StopAttack()
    {
        GladiusGlobals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Attack stopped.", Name));
        Target = null;
        CurrentAttackSkill = null;
        // FIXME - need to worry about out of turn attacks (ripostes, groups etc)
    }

    public void StartAttackIfInRange()
    {
        if (CombatEngine.IsAttackerInRange(this, Target))
        {
            if (CurrentAttackSkill != null)
            {
                if (!Attacking && !StartedAction)
                {
                    StartAttack();
                }
            }
        }
    }

    public bool Attacking
    {
        get
        {
            return IsAttackAnim(CurrentAnimEnum) || m_animation.AttackQueued();
        }
    }

    public void StartDeath()
    {
        Dead = true;
        GladiusGlobals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Death started.", Name));
        QueueAnimation(AnimationEnum.Death);
        //TurnManager.RemoveActor(this);
    }

    public void StopDeath()
    {
        GladiusGlobals.EventLogger.LogEvent(EventTypes.Action, String.Format("[{0}] Death stopped.", Name));
        Arena.RemoveActor(this);

        // For now - we're invisible
        //Visible = false;
        //Enabled = false;
        //prefabGun.SetActiveRecursively(true);
    }

    public virtual void CheckState()
    {
        if (Health <= 0f && !Dead)
        {
            StartDeath();
        }
    }


    public void StartMiss(BaseActor attacker)
    {
        SnapToFace(attacker);
        //QueueAnimation(AnimationEnum.Miss);
    }

    public List<AttackSkill> AllAttackSkills
    {
        get
        {
            return m_knownAttacks;
        }
    }

    public List<AttackSkill> AvailableAttackSkills
    {
        get
        {
            return m_availableAttacks;
        }
    }

    public void ApplyModifiers(AttackSkill skill)
    {
        //need to change the implementation of this so its based on turns not elapsed time
        //foreach (GameObjectAttributeModifier modifier in skill.StatModifiers)
        //{
        //    GameObjectAttributeModifier modifierCopy = modifier.Copy();
        //    modifierCopy.ApplyTo(this);
        //    m_attributeModifiers.Add(modifierCopy);
        //}
        // different blocks may have different bonuses.
        TurnComplete = true;
    }

    private AttackSkill m_currentAttackSkill;
    public AttackSkill CurrentAttackSkill
    {
        get
        {
            return m_currentAttackSkill;
        }
        set
        {
            if (value == null)
            {
                int ibreak = 0;
            }
            m_currentAttackSkill = value;
        }
    }

    private AttackSkill m_basicAttackSkill;
    public AttackSkill BasicAttackSkill
    {
        get
        {
            if (m_basicAttackSkill == null)
            {
                for (int i = 0; i < m_availableAttacks.Count; ++i)
                {
                    AttackSkill attackSkill = m_availableAttacks[i];
                    if (attackSkill.SkillLevel == 0 && attackSkill.Type == "Attack" && attackSkill.SubType == "Standard")
                    {
                        m_basicAttackSkill = attackSkill;
                        break;
                    }
                }

            }
            return m_basicAttackSkill;
        }
    }

    public void FilterAttackSkills()
    {
        m_availableAttacks.Clear();
        foreach (AttackSkill skill in m_knownAttacks)
        {
            if(IsSkillAvailable(skill,false))
            {
                m_availableAttacks.Add(skill);
            }
        }
    }

    public void FilterAttackSkillsDisplay()
    {
        m_availableAttacks.Clear();
        foreach (AttackSkill skill in m_knownAttacks)
        {
            if (IsSkillAvailable(skill, true))
            {
                m_availableAttacks.Add(skill);
            }
        }
    }

    public bool IsSkillAvailable(AttackSkill skill,bool display)
    {
        if (!display && skill.UseCost <= ArenaSkillPoints)
        {
            return false;
        }
        if (skill.UseClass != AttackSkill.ForActorClass(ActorClass) && skill.UseClass != AttackSkill.ForActorClass(TemporaryActorClass))
        {
            return false;
        }
        return true;
    }



    public bool HaveAvailableSkillForRange(int range)
    {
        foreach (AttackSkill skill in m_availableAttacks)
        {
            if (skill.InRange(range) && skill.Available(this))
            {
                return true;
            }
        }
        return false;
    }

    public void ChooseTargetAndSkill()
    {
        // check for pass only skill.
        FilterAttackSkills();

        if (CurrentAttackSkill == null)
        {
            // choose basic attack skill.
            CurrentAttackSkill = BasicAttackSkill;
        }

        // if theres an actor nearby with low health , prioritise that target to finish it off.

        bool foundTargetInRange = false;

        if (TurnManager != null)
        {
            foreach (BaseActor actor in TurnManager.AllActors)
            {
                // reset threats
                UpdateThreat(actor, 0, false);
                if (CombatEngine != null && CombatEngine.IsValidTarget(this, actor, CurrentAttackSkill))
                {
                    int weighting = 0;
                    int distance = GladiusGlobals.PointDist2(this.ArenaPoint, actor.ArenaPoint);
                    // weight score based on how far away. (may need to confirm via pathfind as well).
                    int distanceAdjust = 10 - distance;
                    UpdateThreat(actor, distanceAdjust);

                    // look and see if we have an advantage from class (light v heavy, heavy v medium etc)
                    int categoryBonus = CombatEngine.GetClassAdvantage(this, actor);
                    if (categoryBonus < 0)
                    {
                        UpdateThreat(actor, -2);
                    }
                    else if (categoryBonus > 0)
                    {
                        UpdateThreat(actor, 2);
                    }


                    // do we have a skill 
                    if (HaveAvailableSkillForRange(distance))
                    {
                        foundTargetInRange = true;

                        // if we can't get there. then knock it down a lot.
                        // FIXME - this should be updated to take into account ranged skills...
                        if (Arena && Arena.DoesPathExist(ArenaPoint, actor.ArenaPoint))
                        {
                            UpdateThreat(actor, -10);
                        }
                        // and if it is a ranged skill then we need to do an arena los check.


                        // more tests on self/team buffing skills

                        // test on AoE skills.


                        // tests on affinity bonus to see if we can damage opponent with a particular skill?



                        // prioritise finishing off targets.
                        if (CombatEngine.IsNearDeath(actor))
                        {
                            UpdateThreat(actor, 5);
                        }
                        else
                        {
                            UpdateThreat(actor, 2);
                        }
                    }
                }

            }
        }
        // maybe adjust for height differnces as well? trickier as we need to check squares next to target (or not for ranged)
        // and look at moving to the square that gives the biggest advantage.


        // now choose one with highest score. (may want to randomise this slightly if a few are 'close'
        BaseActor result = null;
        int bestScore = 0;
        foreach (BaseActor actor in m_threatMap.Keys)
        {
            int score = m_threatMap[actor];
            if (score > bestScore)
            {
                bestScore = score;
                result = actor;
            }
        }

        if (result != null && !result.Dead)
        {
            CurrentAttackSkill = ChooseAttackSkillForTarget(result);
            // testing.
            CurrentAttackSkill = BasicAttackSkill;
            Target = result;
        }

    }

    public void UpdateThreat(BaseActor actor, int value, bool modify = true)
    {
        int val = 0;
        if (modify)
        {
            m_threatMap.TryGetValue(actor, out val);
            val += value;
        }
        else
        {
            val = value;
        }

        if (actor.Dead)
        {
            val = -1;
        }

        m_threatMap[actor] = val;
    }



    // called after we've chosen a target.
    private AttackSkill ChooseAttackSkillForTarget(BaseActor target)
    {
        AttackSkill result = null;
        if (target != null)
        {
            int distance = GladiusGlobals.PointDist2(this.ArenaPoint, target.ArenaPoint);
            float bestDamage = 0;
            float expectedDamage = 0.0f;
            foreach (AttackSkill skill in AllAttackSkills.FindAll(skill => HaveAvailableSkillForRange(distance) && skill.Available(this)))
            {
                // decide which one will do the most.
                if (CombatEngine != null)
                {
                    expectedDamage = CombatEngine.CalculateExpectedDamage(this, target, skill);
                }
                if (expectedDamage > bestDamage)
                {
                    bestDamage = expectedDamage;
                    result = skill;
                }
                //if(skill.CanDamageTarget(target))
                //{
                //    result = skill;
                //}


            }
        }
        return result;
    }


    public void BreakShield()
    {
        m_characterData.AddItemByNameAndLoc(null, ItemLocation.Shield);
        // update model
    }

    public void RestoreShield()
    {

    }

    public void BreakHelmet()
    {
        m_characterData.AddItemByNameAndLoc(null, ItemLocation.Helmet);
        // update model
    }

    public void QueueCounterAttack(BaseActor attacker)
    {

    }


    public void Knockdown(int numTurns)
    {
        KnockedDown = true;
        m_knockDownTurns = numTurns;
    }

    public bool KnockedDown
    {
        get; set;
    }

    public void GetUp()
    {
        KnockedDown = false;
        // queue getup anim
        QueueAnimation(AnimationEnum.MoveGetup);
    }

    public void Knockback(BaseActor attacker)
    {
        Point direction = GladiusGlobals.Subtract(attacker.ArenaPoint, ArenaPoint);
        direction = GladiusGlobals.CardinalNormalize(direction);
        Point newSquare = GladiusGlobals.Add(ArenaPoint, direction);
        if (!Arena.IsPointOccupied(newSquare))
        {
            m_knockedBack = true;
            m_knockBackArenaPoint = newSquare;

        }
    }


    #endregion

    #region Animation

    public void NotifyAnimationStarted(AnimationEnum animEnum)
    {

    }

    public void NotifyAnimationStopped(AnimationEnum animEnum)
    {
        if (IsMovementAnim(animEnum))
        {
            MovementAnimationComplete();
        }
    }


    public void CheckAnimationEvents()
    {
        if (m_animation != null && m_animation.CurrentAnimation != null)
        {
            if (m_animation.CurrentAnimation.EventOccurred("hit"))
            {
                DamagePoint();
            }
            if (m_animation.CurrentAnimation.EventOccurred("react"))
            {
                //Start
            }
            if (m_animation.CurrentAnimation.EventOccurred("detachShield"))
            {
            }
            if (m_animation.CurrentAnimation.EventOccurred("detachWeapon1"))
            {
            }
            if (m_animation.CurrentAnimation.EventOccurred("detachWeapon2"))
            {
            }
            if (m_animation.CurrentAnimation.EventOccurred("footStepL"))
            {
            }
            if (m_animation.CurrentAnimation.EventOccurred("footStepR"))
            {
            }
        }
    }

    public AnimationEnum ChooseIdleAim()
    {
        if (Arena == null)
        {
            return AnimationEnum.Idle;
        }
        if (PassSkillOnly())
        {
            return AnimationEnum.None;
        }
        if (Dead)
        {
            return AnimationEnum.IdleDeath;
        }
        if (KnockedDown)
        {
            return AnimationEnum.IdleKnockDown;
        }
        if (CombatEngine.IsNearDeath(this))
        {
            return AnimationEnum.IdleWounded;
        }
        if (Engaged)
        {
            return AnimationEnum.IdleEngaged;
        }
        return AnimationEnum.Idle;
    }

    public static bool IsClimbing(AnimationEnum animEnum)
    {
        switch (animEnum)
        {
            case AnimationEnum.MoveClimbHalf:
            case AnimationEnum.MoveJumpDownHalf:
                return true;
            default:
                return false;
        }
    }

    public static bool IsMovementAnim(AnimationEnum animEnum)
    {
        switch (animEnum)
        {
            case AnimationEnum.MoveRun:
            case AnimationEnum.MoveClimbHalf:
            case AnimationEnum.MoveJumpDownHalf:
            case AnimationEnum.MoveWalk:
                return true;
            default:
                return false;
        }
    }

    public static bool IsIdleAnim(AnimationEnum animEnum)
    {
        switch (animEnum)
        {
            case AnimationEnum.Idle:
            case AnimationEnum.IdleEngaged:
            case AnimationEnum.IdlePassive:
            case AnimationEnum.IdleKnockDown:
            case AnimationEnum.IdleDeath:
            case AnimationEnum.IdleWounded:
                {
                    return true;
                }
            default:
                return false;
        }
    }

    public static bool IsAttackAnim(AnimationEnum animEnum)
    {
        switch (animEnum)
        {
            case AnimationEnum.Attack1:
            case AnimationEnum.Attack2:
            case AnimationEnum.Attack3:
            case AnimationEnum.SandToss:
                {
                    return true;
                }
            default:
                return false;
        }
    }

    public static bool IsHitResponseAnim(AnimationEnum animEnum)
    {
        switch (animEnum)
        {
            case AnimationEnum.ReactHitStrongB:
            case AnimationEnum.ReactHitStrongF:
            case AnimationEnum.ReactHitStrongL:
            case AnimationEnum.ReactHitStrongR:
            case AnimationEnum.ReactHitWeakB:
            case AnimationEnum.ReactHitWeakF:
            case AnimationEnum.ReactHitWeakL:
            case AnimationEnum.ReactHitWeakR:
                {
                    return true;
                }
            default:
                return false;
        }

    }

    #endregion

    #region TurnControl
    public bool CheckTurnComplete()
    {
        if (UnitActive && PlayerControlled && TurnManager != null && TurnManager.WaitingOnPlayerControl)
        {
            return false;
        }

        if (FollowingWayPoints && m_currentMovePoints > 0)
        {
            return false;
        }

        if (Attacking)
        {
            return false;
        }

        if (PassSkillOnly())
        {
            return true;
        }

        // if we're back to idle then turn is over
        return IsIdleAnim(CurrentAnimEnum);
    }

    public AnimationEnum CurrentAnimEnum
    {
        get { return m_animation != null ? m_animation.CurrentAnimEnum : AnimationEnum.None; }
    }

    public AnimationEnum NextAnimEnum
    {
        get { return m_animation != null ? m_animation.NextAnimEnum : AnimationEnum.None; }
    }


    public void Think()
    {
        // Debug.Log("Think");
        if (!PlayerControlled)
        {
            if (!CombatEngine.IsValidTarget(this, Target, CurrentAttackSkill))
            {
                Target = null;
            }

            if (!FollowingWayPoints)
            {
                ChooseTargetAndSkill();

                // Are we next to an enemy
                BaseActor enemy = Arena.NextToEnemy(this);
                if (enemy != null)
                {
                    Target = enemy;
                }
                else
                {
                    // pick random spot on arena and pathfind for now.

                    // todo - be a bit cleverer and try and flank enemy, or aim for highground.

                    Point result;
                    BaseActor target = Arena.FindNearestEnemy(this);
                    if (target != null)
                    {
                        Point nearestPoint = Arena.PointNearestLocation(ArenaPoint, target.ArenaPoint, false);
                        if (Arena.FindPath(ArenaPoint, nearestPoint, WayPointList))
                        {
                            ConfirmMove();
                        }
                    }
                }
            }
            ConfirmAttackSkill();
        }
    }

    private bool DoneDamage
    {
        get; set;
    }

    private bool TurnStarted
    { get; set; }

    public void ActorTurnStarted(BaseActor baseActor)
    {

    }

    public bool MovedThisRound
    {
        get;
        set;
    }

    public void RoundStarted()
    {
        MovedThisRound = false;
    }

    public void RoundEnded()
    {

    }


    public void StartTurn()
    {
        UnitActive = true;
        TurnComplete = false;
        TurnStarted = true;
        MovedThisRound = true;
        StartedAction = false;
        DoneDamage = false;
        ActionSelected = false;
        ActionConfirmed = false;
        Target = null;

        FocusCamera(CameraManager);
        MovementGrid.CurrentCursorPoint = ArenaPoint;

        if (MovementGrid != null)
        {
            MovementGrid.RebuildMesh = true;
        }
        m_currentMovePoints = m_totalMovePoints;
        ClearWaypoints();


        ArenaSkillPoints++;

        if (!PlayerControlled)
        {
            if (!PassSkillOnly())
            {
                Think();
            }
            else
            {
                //EndTurn();
            }
        }
    }

    public bool PassSkillOnly()
    {
        if (m_knownAttacks.Count == 1 && m_knownAttacks[0].Name == "Pass")
        {
            CurrentAttackSkill = m_knownAttacks[0];
            return true;
        }
        return false;
    }

    public Vector3 CharacterFocusPoint
    {
        get
        { return m_zeroBone.position; }
    }

    public bool ImmuneToDamageType(DamageType damageType)
    {
        return false;
    }

    public void ConfirmAttackSkill()
    {
        if (CurrentAttackSkill != null)
        {
            // adjust our skill points.
            ArenaSkillPoints -= CurrentAttackSkill.UseCost;
            //ArenaSkillPoints += CurrentAttackSkill.UseGain;

            if (UnitActive && PlayerControlled && TurnManager != null && TurnManager.WaitingOnPlayerControl)
            {
                TurnManager.WaitingOnPlayerControl = false;
            }
            if (CurrentAttackSkill.IsMoveToAttack)
            {
                if (WayPointList.Count > 0)
                {
                    ConfirmMove();
                }
                else
                {
                    StartAttackIfInRange();
                }
            }

            FocusCamera(CameraManager);
        }
    }

    public void EndTurn()
    {
        StopAttack();
        FaceCardinal();
        UnitActive = false;
        if (!Dead)
        {
            QueueAnimation(ChooseIdleAim());
        }
        if (MovementGrid != null)
        {
            MovementGrid.RebuildMesh = true;
        }

    }

    public bool StartedAction
    {
        get; set;
    }

    private bool _tc;
    public bool TurnComplete
    {
        get
        {
            return _tc;
        }

        set
        {
            if (value)
            {
                int ibreak = 0;
            }
            _tc = value;
        }
    }

    public bool PlayerControlled
    {
        get;
        set;
    }

    public TurnManager TurnManager
    {
        get;
        set;
    }


    #endregion



    #region Movement

    private void MovementAnimationComplete()
    {
        if (HasCurrentWaypoint)
        {
            if (Arena.CanMoveActor(this, CurrentWaypoint))
            {
                Arena.MoveActor(this, CurrentWaypoint);
                ArenaPoint = CurrentWaypoint;

                // rebuild this now we've reached a point.
                MovementGrid.RebuildMesh = true;

                // we've moved one step so reduce our movement.
                m_currentMovePoints--;

                // check and see if we need to turn
                if (HasNextWaypoint)
                {
                    TurnIfNeeded(NextWaypoint);
                    AnimationEnum moveAnim = ChooseMovementAnim();
                    if (moveAnim != AnimationEnum.None)
                    {
                        QueueAnimation(moveAnim);
                    }
                    m_wayPointIndex++;

                }
                else
                {
                    FollowingWayPoints = false;
                    // Reached Destination.
                    StartAttackIfInRange();
                }

            }
            else
            {
                // we've been blocked from where we were hoping for.
                // clear state and force a rethink.
                // pop our character 'back' to last square.
                ArenaPoint = ArenaPoint;
                ClearWaypoints();
            }
        }
        else
        {
            FollowingWayPoints = false;
            // Reached Destination.
            StartAttackIfInRange();
        }
    }

    public bool HasNextWaypoint
    {
        get
        {
            return m_wayPointIndex > -1 && m_wayPointIndex < WayPointList.Count-1;
        }
    }


    public bool HasCurrentWaypoint
    {
        get
        {
            return m_wayPointIndex > -1 && m_wayPointIndex < WayPointList.Count;
        }
    }

    public Point CurrentWaypoint
    {
        get
        {
            if (HasCurrentWaypoint)
            {
                return WayPointList[m_wayPointIndex];
            }
            return new Point();
        }
    }

    public Point NextWaypoint
    {
        get
        {
            if (HasNextWaypoint)
            {
                return WayPointList[m_wayPointIndex + 1];
            }
            return new Point();
        }
        
    }


    public AnimationEnum ChooseMovementAnim()
    {
        AnimationEnum result = AnimationEnum.None;
        if (HasCurrentWaypoint)
        {
            float currentHeight = Arena.GetHeightAtLocation(ArenaPoint);
            float newHeight = Arena.GetHeightAtLocation(CurrentWaypoint);

            if (Arena.IsHeightChange(currentHeight, newHeight))
            {
                if (currentHeight < newHeight)
                {
                    result = AnimationEnum.MoveClimbHalf;
                    //ClimbUp();
                }
                else
                {
                    result = AnimationEnum.MoveJumpDownHalf;
                    //JumpDown();
                }
            }
            else
            {
                result = AnimationEnum.MoveRun;
            }
        }
        return result;
    }


    private void UpdateMovement()
    {
        if (Turning)
        {
            TurnTimer += Time.deltaTime;
            Rotation = Quaternion.Slerp(Rotation, TargetRotation, (TurnTimer / TurnTime));
            // close enough now to stop?
            //if (QuaternionHelper.FuzzyEquals(Rotation, TargetRotation))
            if (TurnTimer >= TurnTime)
            {
                Rotation = TargetRotation;
                Turning = false;
            }
        }
        else
        {
            Vector3 point = Arena.GridPointToWorld(ArenaPoint);
            //point += m_zeroBone.transform.localPosition;
            if (m_transformBone != null)
            {
                point += m_transformBone.transform.localPosition;
                //Position = point;
                //if (FollowingWayPoints)
                //if(m_animation != null && m_animation.CurrentAnimation != null && m_animation.CurrentAnimation.mTranslation > 0f)
                if (IsMovementAnim(m_animation.CurrentAnimEnum))
                {
                    if (IsClimbing(m_animation.CurrentAnimEnum))
                    {
                        int ibreak = 0;
                    }
                    //Position += transform.forward * (float)Time.deltaTime * MovementSpeed;
                    Position += transform.forward * (float)Time.deltaTime * m_animation.CurrentAnimation.mTranslation;
                }

            }        }
    }

    public void ClimbUp()
    {
        QueueAnimation(AnimationEnum.MoveClimbHalf);
    }

    public void JumpDown()
    {
        QueueAnimation(AnimationEnum.MoveJumpDownHalf);

    }


    public void TurnIfNeeded(Point p)
    {
        Quaternion currentFacing = transform.localRotation;
        Vector3 target = Arena.GridPointToWorld(p);
        Vector3 diff = target - Position;
        Quaternion newFacing = Quaternion.LookRotation(diff);
        if (currentFacing != newFacing)
        {
            FaceDirection(newFacing, TurnTime);
        }
    }

    public void SnapToFace(BaseActor actor)
    {
        if (actor != null)
        {
            Vector3 nextDiff = actor.Position - Position;
            nextDiff.y = 0;
            nextDiff.Normalize();
            OriginalRotation = Rotation;
            Rotation = Quaternion.LookRotation(nextDiff);
        }
    }

    public void FaceDirection(Quaternion newDirection, float turnSpeed)
    {
        if (!Turning)
        {
            Turning = true;
            OriginalRotation = Rotation;
            TargetRotation = newDirection;
            TurnTimer = 0f;
        }
        else
        {
            int ibreak = 0;
        }

    }

    public void FaceCardinal()
    {
        return;

        float minDot = float.MaxValue;
        Quaternion bestFace = Quaternion.identity;
        for (int i = 0; i < CardinalDirs.Length; ++i)
        {
            float dot = Quaternion.Dot(Rotation, CardinalDirs[i]);
            if (dot < minDot)
            {
                minDot = dot;
                bestFace = CardinalDirs[i];
            }
        }
        Rotation = bestFace;
    }

    public void AddWayPoint(Point p)
    {
        int pointIndex = WayPointList.IndexOf(p);
        if (pointIndex == -1)
        {
            if (p == ArenaPoint)
            {
                FocusCamera(CameraManager);
                ClearWaypoints();
            }
            else
            {
                MovementGrid.FocusCamera(CameraManager);

                if (WayPointList.Count == 0 ||
                    Arena.IsNextTo(WayPointList[WayPointList.Count - 1], p))
                {
                    WayPointList.Add(p);
                }
            }
        }
        else
        {
            // remove all up to the last
            int numRemove = WayPointList.Count - pointIndex - 1;
            // removerange includes the index point so step over.
            WayPointList.RemoveRange(pointIndex + 1, numRemove);
        }


        //if (!WayPointList.Contains(p))
        //{
        //    WayPointList.Add(p);
        //}
    }

    public List<Point> WayPointList
    {
        get { return m_wayPointList; }

    }

    private List<Point> OldWayPointList
    {
        get { return m_oldWayPointList; }

    }

    // 
    public void ConfirmMove()
    {
        if (WayPointList.Count > 0)
        {
            FollowingWayPoints = true;
            m_wayPointIndex = 0;
            if (HasCurrentWaypoint)
            {
                QueueAnimation(ChooseMovementAnim());
                TurnIfNeeded(CurrentWaypoint);
            }
        }
    }


    bool Turning
    {
        get;
        set;
    }


    public bool FollowingWayPoints
    {
        get;
        set;
    }

    public float TurnTimer
    {
        get;
        set;
    }

    public Quaternion Rotation
    {
        get
        {
            return transform.rotation;
        }
        set
        {
            transform.rotation = value;
        }
    }

    private Quaternion OriginalRotation
    {
        get;
        set;
    }

    private Quaternion TargetRotation
    {
        get;
        set;
    }


    #endregion







    public void Retreat(BaseActor attacker)
    {

    }

    #region Control

    public void CancelAction()
    {
        if (ActionSelected)
        {
            ActionSelected = false;
            MovementGrid.RebuildMesh = true;
        }
    }

    public bool NearbyTargets()
    {
        return Arena.PointNextToEnemy(this,ActionSelectedPoint);
    }

    public void ConfirmAction()
    {
        if (!ActionSelected)
        {
            ActionSelected = true;
            if (WayPointList.Count > 0)
            {
                ActionSelectedPoint = WayPointList[WayPointList.Count - 1];
            }
            // if there are no targets nearby then the confirm is a final confirm.
            if (!NearbyTargets())
            {
                ActionConfirmed = true;
            }
            // if we're on the target itself then that confirms as well.
            if (Target != null && Arena.GetActorAtPosition(MovementGrid.CurrentCursorPoint) == Target)
            {
                ActionConfirmed = true;
            }
        }
        else
        {
            ActionConfirmed = true;
        }
        MovementGrid.RebuildMesh = true;
    }

    public bool ActionSelected
    {
        get;
        private set;
    }

    public Point ActionSelectedPoint
    {
        get;
        private set;
    }

    private bool m_actionConfirmed;
    public bool ActionConfirmed
    {
        get { return m_actionConfirmed; }

        private set
        {
            if (value && CurrentAttackSkill.Available(this))
            {
                m_actionConfirmed = value;
                ConfirmAttackSkill();
            }
            else
            {
                m_actionConfirmed = false;
            }
        }
    }
    #endregion


    #region CharacterStats

    public String Name
    {
        get
        {
            return m_characterData != null ? m_characterData.Name : "Null character data";
        }
    }

    public GladiatorSchool School
    {
        get { return m_characterData.School; }
    }

    public int CON
    { get { return m_characterData.CON; } set { m_characterData.CON = value; } }

    public int PWR
    { get { return m_characterData.PWR; } set { m_characterData.PWR = value; } }

    public int ACC
    { get { return m_characterData.ACC; } set { m_characterData.ACC = value; } }

    public int DEF
    { get { return m_characterData.DEF; } set { m_characterData.DEF = value; } }

    public int INI
    {
        get
        {
            return m_characterData.INI + INIAddition;
        }
        set
        {
            m_characterData.INI = value;
        }
    }

    public int INIAddition
    {
        get
        {
            if (Crowd != null && Crowd.GetValueForSchool(m_characterData.School) > 75)
            {
                return 20;
            }
            return 0;
        }
    }


    public float MOVE
    { get { return m_characterData.MOV; } set { m_characterData.MOV = value; } }

    public ActorClassData ActorClassData
    {
        get { return m_characterData.ActorClassData; }
    }

    public int CurrentMovePoints
    {
        get { return m_currentMovePoints; }

    }

    public float MovePointMultiplier
    {
        get
        {
            if (Crowd != null && Crowd.GetValueForSchool(m_characterData.School) > 25)
            {
                return 1.3f;
            }
            return 1.0f;
        }

    }


    public float AffinityPointMultiplier
    {
        get
        {
            if (Crowd != null && Crowd.GetValueForSchool(m_characterData.School) > 50)
            {
                return 1.5f;
            }
            return 1.0f;
        }
    }



    public int TotalMovePoints
    {
        get { return (int)(m_totalMovePoints * MovePointMultiplier); }
    }

    public int Health
    {
        get;
        set;
    }

    public int MaxHealth
    {
        get;
        set;
    }

    public int ArenaSkillPoints
    {
        get;
        set;
    }

    public int MaxArenaSkillPoints
    {
        get { return 5; }
    }

    private int _affinity = 2;
    public int Affinity
    {
        get
        {
            return _affinity;
        }
        private set { _affinity = Math.Max(0, Math.Min(value, MaxAffinity)); }
    }


    public void AddAffinity(int val)
    {
        float newVal = val * AffinityPointMultiplier;
        Affinity += val;
    }


    public int MaxAffinity
    {
        get
        {
            //return m_attributeDictionary[GameObjectAttributeType.Affinity].MaxValue;
            return 5;
        }
    }



    public DamageType WeaponAffinityType
    {
        get
        {
            Item item = m_characterData.GetItemAtLocation(ItemLocation.Weapon);
            return item != null ? item.DamageType : DamageType.None;
        }
    }

    public DamageType ArmourAffinityType
    {
        get
        {
            Item item = m_characterData.GetItemAtLocation(ItemLocation.Armor);
            return item != null ? item.DamageType : DamageType.None;
        }
    }

    public bool Dead
    {
        get;
        set;
    }

    public String TeamName
    {
        get { return m_characterData.TeamName; }
    }

    public bool HasShield
    {
        get;
        set;
    }

    public bool IsTwoHanded
    { get; set; }


    public ActorClass ActorClass
    {
        get
        {
            return m_characterData.ActorClass;
        }
        //set;
    }

    // used for animal changes and the like.
    public ActorClass TemporaryActorClass
    {
        get;set;


    }


    public BaseActor Target
    {
        get; set;
    }

    Point m_arenaPoint;
    public Point ArenaPoint
    {
        get
        {
            return m_arenaPoint;
        }

        set
        {
            m_arenaPoint = value;
            if (Arena != null)
            {
                Position = Arena.GridPointToWorld(m_arenaPoint);
            }
            if (m_animation && m_animation.CurrentAnimEnum == AnimationEnum.MoveClimbHalf)
            {
                int ibreak = 0;
            }
            Quaternion q = transform.localRotation;
            Vector3 ea = q.eulerAngles;
            ea.x = 0f;
            q = Quaternion.Euler(ea);
            transform.localRotation = q;
        }


    }

    private Vector3 _pos;
    public Vector3 Position
    {
        get
        {
            return _pos;
            //return transform.position;
        }
        set
        {
            _pos = value;
            //transform.position = value;
        }
    }


    public bool Engaged
    {
        get
        {
            return Arena.NextToEnemy(this);
        }
    }

    public bool Climbing
    {
        get
        {
            return CurrentAnimEnum == AnimationEnum.MoveClimbHalf || CurrentAnimEnum == AnimationEnum.MoveJumpDownHalf;
        }
    }


    #endregion

    #region StatusChangers
    // we'll only get here after immunity checks and the like so don't re-check
    public void CauseStatus(String statusName, SkillStatus skillStatus)
    {





    }

    public bool ImmuneToStatus(String name)
    {
        // check all active and passive skills.
        foreach (AttackSkill skill in m_activeSkills)
        {
            if (skill.IsImmuneTo(name))
            {
                return true;
            }
        }
        foreach (AttackSkill skill in m_innateSkills)
        {
            if (skill.IsImmuneTo(name))
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateOngoingSkillStatuses(BaseActor source)
    {
        foreach (OngoingSkillStatus status in m_ongoingStatuses)
        {
            status.UpdateForSource(source);
        }
    }

    public void CancelSkillStatus(string statusName)
    {
        // go through all active skills.
        // if they're not permanent then remove the affected status..
        foreach (OngoingSkillStatus oss in m_ongoingStatuses)
        {
            if (!oss.Permanent)
            {
            }

        }
    }
    #endregion

    public CharacterData m_characterData;
    private Dictionary<BaseActor, int> m_threatMap = new Dictionary<BaseActor, int>();
    private List<Point> m_wayPointList = new List<Point>();
    private int m_wayPointIndex = 0;
    private List<Point> m_oldWayPointList = new List<Point>();

    private List<AttackSkill> m_knownAttacks = new List<AttackSkill>();
    private List<AttackSkill> m_availableAttacks = new List<AttackSkill>();

    public List<AttackSkill> m_innateSkills = new List<AttackSkill>();
    public List<AttackSkill> m_activeSkills = new List<AttackSkill>();

    public List<OngoingSkillStatus> m_ongoingStatuses = new List<OngoingSkillStatus>();


    private int m_currentMovePoints;
    private int m_totalMovePoints = 10;


    public GameObject m_projectileGameObject;

    private Transform m_projectileHandTransform;
    private Point m_knockBackArenaPoint = new Point();
    private bool m_knockedBack;

    private int m_knockDownTurns = 0;


    private Transform m_transformBone;
    private Transform m_zeroBone;
    private Transform m_leftFootBone;
    private Transform m_rightFootBone;
    private Transform m_skullBone;
    private Transform m_cameraBone;




    public const int MinLevel = 1;
    public const int MaxLevel = 15;

    static Quaternion[] CardinalDirs = new Quaternion[] {
                Quaternion.AngleAxis (0, Vector3.up),
                Quaternion.AngleAxis (180, Vector3.up),
                Quaternion.AngleAxis (90, Vector3.up),
                Quaternion.AngleAxis (270, Vector3.up)
        };

}



