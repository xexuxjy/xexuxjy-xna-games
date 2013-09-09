using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Gladius.control
{
    public class TurnManager : DrawableGameComponent
    {
        public TurnManager(Game game,Arena arena)
            : base(game)
        {
            m_arena = arena;
            m_movementGrid = new MovementGrid(this,m_arena);
            //movementGrid.CurrentPosition = ba1.CurrentPosition;
            //m_screenComponents.Components.Add(movementGrid);
            Globals.MovementGrid = m_movementGrid;

        }

        public override void Initialize()
        {
            base.Initialize();
            m_movementGrid.Initialize();
        }

        protected override void LoadContent()
        {
            m_movementGrid.LoadContent(Game.Content, Game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            m_movementGrid.Draw(gameTime, Game.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            m_movementGrid.Update(gameTime);

            if (CurrentActor == null || CurrentActor.TurnComplete)
            {
                if (CurrentActor != null && CurrentActor.TurnComplete)
                {
                    CurrentActor.EndTurn();
                }
                
                Debug.Assert(m_turns.Count > 0);
                CurrentActor = m_turns[0];
                m_turns.RemoveAt(0);
                StartTurn();
            }

            if (WaitingOnPlayerControl)
            {
                //
            }



        }

        public void StartTurn()
        {
            Globals.Camera.LookAt(CurrentActor.Position);

            CurrentActor.StartTurn();
            if (CurrentActor.PlayerControlled)
            {
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
                m_movementGrid.GridMode = value?GridMode.Select:GridMode.Inactive;
            }
        }


        MovementGrid m_movementGrid;
        PlayerChoiceBar m_playerChoiceBar;
        
        Arena m_arena;
        List<BaseActor> m_turns = new List<BaseActor>();
    }

}
