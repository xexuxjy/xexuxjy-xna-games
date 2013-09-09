using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using Gladius.events;

namespace Gladius.control
{
    public class UserControl : GameComponent
    {
        public UserControl(Game game,InputState inputState) : base(game)
        {
            m_inputState = inputState;
        }

        // FIXME MAN - update these to be camera facing relative.

        public bool CursorLeftPressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.J, m_controllingPlayer, out pi);
        }

        public bool CursorRightPressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.L, m_controllingPlayer, out pi);

        }

        public bool CursorUpPressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.I, m_controllingPlayer, out pi);

        }

        public bool CursorDownPressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.K, m_controllingPlayer, out pi);

        }

        public bool Action1Pressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.X, m_controllingPlayer, out pi);

        }

        public bool Action2Pressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.C, m_controllingPlayer, out pi);

        }

        public bool Action3Pressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.V, m_controllingPlayer, out pi);

        }

        public bool Action4Pressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.B, m_controllingPlayer, out pi);

        }

        public override void Update(GameTime gameTime)
        {
            if (Action1Pressed())
            {
                EventManager.PerformAction(this, ActionButton.ActionButton1);
            }
            if (Action2Pressed())
            {
                EventManager.PerformAction(this, ActionButton.ActionButton2);
            }
            if (Action3Pressed())
            {
                EventManager.PerformAction(this, ActionButton.ActionButton3);
            }
            if (Action4Pressed())
            {
                EventManager.PerformAction(this, ActionButton.ActionButton4);
            }
            if (CursorLeftPressed())
            {
                EventManager.PerformAction(this, ActionButton.ActionLeft);
            }
            if (CursorRightPressed())
            {
                EventManager.PerformAction(this, ActionButton.ActionRight);
            }
            if (CursorUpPressed())
            {
                EventManager.PerformAction(this, ActionButton.ActionUp);
            }
            if (CursorDownPressed())
            {
                EventManager.PerformAction(this, ActionButton.ActionDown);
            }

        }


        private PlayerIndex m_controllingPlayer = PlayerIndex.One;
        private InputState m_inputState;
    }

    public enum ActionButton
    {
        ActionUp,
        ActionDown,
        ActionLeft,
        ActionRight,
        ActionButton1,
        ActionButton2,
        ActionButton3,
        ActionButton4
    }

}
