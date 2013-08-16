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
            return m_inputState.IsNewKeyPress(Keys.J,null,out pi);
        }

        public bool CursorRightPressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.L, null, out pi);

        }

        public bool CursorUpPressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.I, null, out pi);

        }

        public bool CursorDownPressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.K, null, out pi);

        }

        public bool Action1Pressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.Z, null, out pi);

        }

        public bool Action2Pressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.X, null, out pi);

        }

        public bool Action3Pressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.C, null, out pi);

        }

        public bool Action4Pressed()
        {
            PlayerIndex pi;
            return m_inputState.IsNewKeyPress(Keys.V, null, out pi);

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

        }



        private InputState m_inputState;
    }

    public enum ActionButton
    {
        ActionButton1,
        ActionButton2,
        ActionButton3,
        ActionButton4
    }

}
