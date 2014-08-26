using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gladius.arena
{
    public class TurnManager : MonoBehaviour
    {

        public TurnManager()
        {
            GladiusGlobals.TurnManager = this;
        }


        //public void DoProjectileCamera()
        //{
        //    Globals.CameraManager.SetStaticCamera();
        //    // focus on point midway between two characters.
        //    Vector3 a = CurrentActor.GetProjectile().Position;

        //    Matrix model = CurrentActor.GetProjectile().World;

        //    Vector3 forward = model.Forward;

        //    Vector3 newPosition = a - (forward * 3);
        //    newPosition.Y += 0.5f;

        //    Globals.Camera.Position = newPosition;
        //    Globals.Camera.Target = a;
        //    Globals.Camera.TargetDirection = forward;

        //}

        //public void DoMeleeCamera()
        //{
        //    Globals.CameraManager.SetStaticCamera();
        //    // focus on point midway between two characters.
        //    Vector3 a = CurrentActor.CameraFocusPoint;
        //    Matrix model = CurrentActor.World;

        //    Vector3 forward = model.Forward;

        //    if (CurrentActor.Target != null)
        //    {
        //        a += CurrentActor.Target.CameraFocusPoint;
        //        a /= 2.0f;
        //        // view side on
        //        forward = model.Right;
        //    }
        //    Vector3 newPosition = a - (forward * 3);
        //    newPosition.Y = 4;

        //    Globals.Camera.Position = newPosition;
        //    Globals.Camera.Target = a;
        //    Globals.Camera.TargetDirection = forward;
        //}



        public void Update()
        {
            if (AllPartyDead())
            {
                Application.LoadLevel("GameOverLose");
            }
            else if (AllOpponentsDead())
            {
                Application.LoadLevel("GameOverWin");
            }



            //if (CurrentActor != null)
            //{
            //    if (CurrentActor.Attacking)
            //    {
            //        if (CurrentActor.FiringProjectile)
            //        {
            //            DoProjectileCamera();
            //        }
            //        else
            //        {
            //            DoMeleeCamera();
            //        }
            //    }
            //    else if (CurrentActor.FiringProjectile)
            //    {
            //        DoProjectileCamera();
            //    }
            //    else
            //    {
            //        Globals.CameraManager.SetChaseCamera();

            //        if (CurrentControlState == ControlState.UsingGrid)
            //        {
            //            Globals.Camera.Target = ArenaScreen.MovementGrid.CurrentV3;
            //            Matrix model = CurrentActor.World;
            //            Globals.Camera.Up = Vector3.Up;
            //            Globals.Camera.TargetDirection = model.Forward;
            //            Globals.Camera.DesiredPositionOffset = new Vector3(0, 2f, 4.0f);
            //        }
            //        else
            //        {
            //            Globals.Camera.Target = CurrentActor.CameraFocusPoint;
            //            Matrix model = CurrentActor.World;
            //            Globals.Camera.Up = model.Up;
            //            Globals.Camera.TargetDirection = model.Forward;
            //            Globals.Camera.DesiredPositionOffset = new Vector3(0, 2f, 4.0f);
            //        }
            //    }

            //}


            if (GladiusGlobals.ArenaComponentsInitialised)
            {

                if (CurrentActor == null || CurrentActor.TurnComplete)
                {
                    EndTurn();
                    StartTurn();
                }
            }
        }

        public void EndTurn()
        {
            //Debug.Log("EndTurn");

            //ArenaScreen.MovementGrid.CurrentActor = CurrentActor;
            if (CurrentActor != null && CurrentActor.TurnComplete)
            {
                CurrentActor.EndTurn();
                if (CurrentActor.PlayerControlled)
                {
                    //ArenaScreen.SetPlayerChoiceBarVisible(false);
                }
            }

            if (m_turns.Count > 0)
            {
                System.Diagnostics.Debug.Assert(m_turns.Count > 0);
                //Debug.Log("TurnCount : " + m_turns.Count);

                EventManager.ChangeActor(this, CurrentActor, m_turns[0]);
                CurrentActor = m_turns[0];
                m_turns.RemoveAt(0);
            }
        }

        public void StartTurn()
        {
            //Globals.Camera.CurrentBehavior = Dhpoware.Camera.Behavior.Orbit;
            //GladiusGlobals.Camera.Target = CurrentActor.Position;
            //Debug.Log("StartTurn");
            if (CurrentActor != null)
            {
                // tell all the actors that this actor is starting their turn to allow for status updates..
                foreach (BaseActor actor in m_allActors)
                {
                    actor.ActorTurnStarted(CurrentActor);
                }

                CurrentActor.StartTurn();
				GladiusGlobals.CameraManager.TargetObject = CurrentActor.gameObject;
                if (CurrentActor.PlayerControlled)
                {
                    //ArenaScreen.SetPlayerChoiceBarVisible(true);
                    WaitingOnPlayerControl = true;
                }
            }
        }


        public void QueueActor(BaseActor actor)
        {
            //Debug.Log("QueueActor : " + actor.name);
            // to do  - figure out time of last turn, and use initiative values etc
            // to possibly insert this ahead of others.
            actor.TurnManager = this;
            m_turns.Add(actor);
            if (!m_allActors.Contains(actor))
            {
                m_allActors.Add(actor);
            }
        }

        public void RemoveActor(BaseActor actor)
        {
            m_turns.RemoveAll(v => v == actor);
        }


        BaseActor m_currentActor;
        public BaseActor CurrentActor
        {
            get
            {
                return m_currentActor;
            }
            set
            {
                m_currentActor = value;
            }
        }

        bool m_waitForControlResult;
        public bool WaitingOnPlayerControl
        {
            get
            {
                return m_waitForControlResult;
            }

            set
            {
                m_waitForControlResult = value;
            }
        }

        public Arena Arena
        {
            get;
            set;
        }

        public bool AllPartyDead()
        {
            foreach (BaseActor ba in m_allActors)
            {
                // someone still alive.
                if (ba.TeamName == GladiusGlobals.PlayerTeam && !ba.Dead)
                {
                    return false;
                }
            }
            return true;
        }

        public bool AllOpponentsDead()
        {
            foreach (BaseActor ba in m_allActors)
            {
                // someone still alive.
                if (ba.TeamName != GladiusGlobals.PlayerTeam && !ba.Dead)
                {
                    return false;
                }
            }
            return true;
        }

        public List<BaseActor> AllActors
        {
            get { return m_allActors; }
        }


        //public ControlState CurrentControlState
        //{
        //    get
        //    {
        //        return m_controlState;
        //    }
        //    set
        //    {
        //        m_controlState = value;
        //        if (CurrentControlState == ControlState.UsingGrid)
        //        {
        //            if (CurrentActor.CurrentAttackSkill.AttackType == AttackType.EndTurn)
        //            {
        //                CurrentActor.ConfirmAttackSkill();
        //            }
        //        }

        //    }
        //}


        ControlState m_controlState = ControlState.None;

        List<BaseActor> m_turns = new List<BaseActor>();
        List<BaseActor> m_allActors = new List<BaseActor>();
    }

}

