using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;

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

        private InputState m_inputState;
    }
}
