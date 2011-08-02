using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;
using Microsoft.Xna.Framework.Input;

namespace com.xexuxjy.magiccarpet.util
{
    public class KeyboardController : GameComponent
    {
        public KeyboardController(Game game)
            : base(game)
        {

        }

        //----------------------------------------------------------------------------------------------

        public override void Update(GameTime gameTime)
        {
            KeyboardState currentState = Keyboard.GetState();
            GenerateKeyEvents(ref m_lastKeyboardState, ref currentState, gameTime);
            m_lastKeyboardState = currentState;
        }

        //----------------------------------------------------------------------------------------------

        public virtual void KeyboardCallback(Keys key, GameTime gameTime, bool released, ref KeyboardState newState, ref KeyboardState oldState)
        {
            if (key == Keys.Q || key == Keys.Escape)
            {
                Game.Exit();
            }
        }


        //----------------------------------------------------------------------------------------------
        // workaround from justastro at : http://forums.create.msdn.com/forums/p/1610/157478.aspx

        public static Enum[] GetEnumValues(Type enumType)
        {

            if (enumType.BaseType == typeof(Enum))
            {
                FieldInfo[] info = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
                Enum[] values = new Enum[info.Length];
                for (int i = 0; i < values.Length; ++i)
                {
                    values[i] = (Enum)info[i].GetValue(null);
                }
                return values;
            }
            else
            {
                throw new Exception("Given type is not an Enum type");
            }
        }

        static Enum[] keysEnumValues = GetEnumValues(typeof(Keys));
        private void GenerateKeyEvents(ref KeyboardState old, ref KeyboardState current, GameTime gameTime)
        {
            foreach (Keys key in keysEnumValues)
            {
                bool released = WasReleased(ref old, ref current, key);
                if (released || IsHeldKey(ref current, key))
                {
                    KeyboardCallback(key, gameTime, released, ref current, ref old);
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        // This is a way of generating 'pressed' events for keys that we want to hold down
        private bool IsHeldKey(ref KeyboardState current, Keys key)
        {
            return (current.IsKeyDown(key) && ((key == Keys.Left || key == Keys.Right || key == Keys.Up ||
                key == Keys.Down || key == Keys.PageUp || key == Keys.PageDown)));
        }
        //----------------------------------------------------------------------------------------------

        private bool WasReleased(ref KeyboardState old, ref KeyboardState current, Keys key)
        {
            // figure out if the key was released between states.
            return old.IsKeyDown(key) && !current.IsKeyDown(key);
        }


        protected KeyboardState m_lastKeyboardState;

    }
}
