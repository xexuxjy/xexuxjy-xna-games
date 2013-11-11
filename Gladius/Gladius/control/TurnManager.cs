using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Gladius.events;
using Gladius.gamestatemanagement.screens;
using Gladius.modes.arena;
using Gladius.combat;

namespace Gladius.control
{
    public class TurnManager : GameComponent
    {
        public TurnManager(Game game,ArenaScreen arenaScreen)
            : base(game)
        {
            m_arenaScreen = arenaScreen;
        }

        public void DoProjectileCamera()
        {
            Globals.CameraManager.SetStaticCamera();
            // focus on point midway between two characters.
            Vector3 a = CurrentActor.GetProjectile().Position;

            Matrix model = CurrentActor.GetProjectile().World;

            Vector3 forward = model.Forward;

            Vector3 newPosition = a - (forward * 3);
            newPosition.Y += 2;

            Globals.Camera.Position = newPosition;
            Globals.Camera.Target = a;
            Globals.Camera.TargetDirection = forward;

        }

        public void DoMeleeCamera()
        {
            Globals.CameraManager.SetStaticCamera();
            // focus on point midway between two characters.
            Vector3 a = CurrentActor.CameraFocusPoint;
            Matrix model = CurrentActor.World;

            Vector3 forward = model.Forward;

            if(CurrentActor.Target != null)
            {
                a += CurrentActor.Target.CameraFocusPoint;
                a /= 2.0f;
                // view side on
                forward = model.Right;
            }
            Vector3 newPosition = a - (forward * 3);
            newPosition.Y = 4;

            Globals.Camera.Position = newPosition;
            Globals.Camera.Target = a;
            Globals.Camera.TargetDirection = forward;
        }



        public override void Update(GameTime gameTime)
        {
            if (AllPartyDead())
            {
                m_arenaScreen.BattleOverDefeat();
            }
            else if (AllOpponentsDead())
            {
                m_arenaScreen.BattleOverVictory();
            }



            if (CurrentActor != null)
            {
                if (CurrentActor.Attacking)
                {
                    if (CurrentActor.FiringProjectile)
                    {
                        DoProjectileCamera();
                    }
                    else
                    {
                        DoMeleeCamera();
                    }
                }
                else
                {
                    Globals.CameraManager.SetChaseCamera();

                    if (CurrentControlState == ControlState.UsingGrid)
                    {
                        Globals.Camera.Target = ArenaScreen.MovementGrid.CurrentV3;
                        Matrix model = CurrentActor.World;
                        Globals.Camera.Up = Vector3.Up;
                        Globals.Camera.TargetDirection = model.Forward;
                        Globals.Camera.DesiredPositionOffset = new Vector3(0, 2f, 4.0f);
                    }
                    else
                    {
                        Globals.Camera.Target = CurrentActor.CameraFocusPoint;
                        Matrix model = CurrentActor.World;
                        Globals.Camera.Up = model.Up;
                        Globals.Camera.TargetDirection = model.Forward;
                        Globals.Camera.DesiredPositionOffset = new Vector3(0, 2f, 4.0f);
                    }
                }
            
            }




            if (CurrentActor == null || CurrentActor.TurnComplete)
            {
                EndTurn();
                StartTurn();
            }
        }

        public void EndTurn()
        {
            ArenaScreen.MovementGrid.CurrentActor = CurrentActor;
            if (CurrentActor != null && CurrentActor.TurnComplete)
            {
                CurrentActor.EndTurn();
                if (CurrentActor.PlayerControlled)
                {
                    ArenaScreen.SetPlayerChoiceBarVisible(false);
                }
            }

            Debug.Assert(m_turns.Count > 0);
            EventManager.ChangeActor(this,CurrentActor, m_turns[0]);
            CurrentActor = m_turns[0];
            m_turns.RemoveAt(0);
        }

        public void StartTurn()
        {
            //Globals.Camera.CurrentBehavior = Dhpoware.Camera.Behavior.Orbit;
            Globals.Camera.Target = CurrentActor.Position;

            CurrentActor.StartTurn();

            if (CurrentActor.PlayerControlled)
            {
                ArenaScreen.SetPlayerChoiceBarVisible(true);
                WaitingOnPlayerControl = true;
            }

        }


        public void QueueActor(BaseActor actor)
        {
            // to do  - figure out time of last turn, and use initiative values etc
            // to possibly insert this ahead of others.
            actor.TurnManager = this;
            m_turns.Add(actor);
            if (!m_allActors.Contains(actor))
            {
                m_allActors.Add(actor);
            }
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

        public ArenaScreen ArenaScreen
        {
            get;
            set;
        }

        public bool AllPartyDead()
        {
            foreach (BaseActor ba in m_allActors)
            {
                // someone still alive.
                if (ba.Team == Globals.PlayerTeam && !ba.Dead)
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
                if (ba.Team != Globals.PlayerTeam && !ba.Dead)
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


        public ControlState CurrentControlState
        {
            get
            {
                return m_controlState;
            }
            set
            {
                m_controlState = value;
                if (CurrentControlState == ControlState.UsingGrid)
                {
                    if (CurrentActor.CurrentAttackSkill.AttackType == AttackType.EndTurn)
                    {
                        CurrentActor.ConfirmAttackSkill();
                    }
                }

            }
        }


        public enum ControlState
        {
            None,
            ChoosingSkill,
            UsingGrid
        }

        ControlState m_controlState = ControlState.None;

        private ArenaScreen m_arenaScreen;
        List<BaseActor> m_turns = new List<BaseActor>();
        List<BaseActor> m_allActors = new List<BaseActor>();
    }

}
