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
        public TurnManager(Game game)
            : base(game)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (CurrentActor != null)
            {
                Globals.Camera.Target = CurrentActor.CameraFocusPoint;
                Matrix model = Matrix.CreateFromQuaternion(CurrentActor.Rotation);
                Globals.Camera.Up = model.Up;
                Globals.Camera.TargetDirection = model.Forward;
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
            Globals.MovementGrid.SelectedActor = CurrentActor;
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


        List<BaseActor> m_turns = new List<BaseActor>();
    }

}
