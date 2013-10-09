using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using GameStateManagement;
using Gladius.events;
using Gladius.gamestatemanagement.screens;
using Gladius.modes.arena;

namespace Gladius.control
{
    public class TurnManager : GameComponent
    {
        public TurnManager(Game game,ArenaScreen arenaScreen)
            : base(game)
        {
            m_arenaScreen = arenaScreen;
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

                    Globals.Camera.Target = a;
                    Globals.Camera.TargetDirection = forward;
                }
                else
                {
                    Globals.CameraManager.SetChaseCamera();

                    if (ArenaScreen.MovementGrid.DrawingMovePath)
                    {
                        Globals.Camera.Target = ArenaScreen.MovementGrid.CurrentV3;
                        Matrix model = CurrentActor.World;
                        Globals.Camera.Up = Vector3.Up;
                        Globals.Camera.TargetDirection = model.Forward;
                        Globals.Camera.DesiredPositionOffset = new Vector3(0, 2f, 8.0f);
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

            if (WaitingOnPlayerControl)
            {
                //
            }



        }

        public void EndTurn()
        {
            ArenaScreen.MovementGrid.SelectedActor = CurrentActor;
            ArenaScreen.SetMovementGridVisible(false);
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
            //Globals.MovementGrid.SelectedActor = CurrentActor;

            ArenaScreen.SetMovementGridVisible(true);

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
                //m_movementGrid.GridMode = value?GridMode.Select:GridMode.Inactive;
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

        private ArenaScreen m_arenaScreen;
        List<BaseActor> m_turns = new List<BaseActor>();
        List<BaseActor> m_allActors = new List<BaseActor>();
    }

}
