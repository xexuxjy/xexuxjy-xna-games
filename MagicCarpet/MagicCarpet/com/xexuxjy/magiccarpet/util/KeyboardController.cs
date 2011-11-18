using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.spells;
using GameStateManagement;

namespace com.xexuxjy.magiccarpet.util
{
    public class KeyboardController 
    {
        public KeyboardController()
        {

        }

        //----------------------------------------------------------------------------------------------

        public void HandleInput(InputState inputState)
        {
            KeyboardState currentState = Keyboard.GetState();
            GenerateKeyEvents(ref inputState.LastKeyboardStates[0], ref inputState.CurrentKeyboardStates[0]);
            m_lastKeyboardState = currentState;
        }

        //----------------------------------------------------------------------------------------------

        public virtual void KeyboardCallback(Keys key, bool released, ref KeyboardState newState, ref KeyboardState oldState)
        {
            // forward commands onto the game console
            if (Globals.SimpleConsole.Enabled && released)
            {
                Globals.SimpleConsole.KeyboardCallback(key,released,ref newState,ref oldState);
            }


            switch (key)
            {
                case Keys.Escape:
                    {
                        Globals.GameObjectManager.Game.Exit();
                        break;
                    }
                case Keys.OemTilde:
                case Keys.OemPipe:
                case Keys.Tab:
                    {
                        if (Globals.SimpleConsole.Enabled)
                        {
                            Globals.DisableDebugConsole();
                        }
                        else
                        {
                            Globals.EnableDebugConsole();
                        }
                        break;
                    }
                case Keys.OemComma :
                    {
                        if (Globals.DebugObjectManager.Enabled)
                        {
                            Globals.DebugObjectManager.PreviousObject();
                        }
                        break;
                    }
                case Keys.OemPeriod:
                    {
                        if (Globals.DebugObjectManager.Enabled)
                        {
                            Globals.DebugObjectManager.NextObject();
                        }
                        break;
                    }
                case Keys.NumPad1:
                case Keys.D1:
                    {
                        Globals.Player.SelectedSpell1 = SpellType.Convert;
                        break;
                    }
                case Keys.NumPad2:
                case Keys.D2:
                    {
                        Globals.Player.SelectedSpell1 = SpellType.Fireball;
                        break;
                    }
                case Keys.NumPad3:
                case Keys.D3:

                    {
                        Globals.Player.SelectedSpell1 = SpellType.Lower;
                        break;
                    }
                case Keys.NumPad4:
                case Keys.D4:

                    {
                        Globals.Player.SelectedSpell1 = SpellType.Raise;
                        break;
                    }
                case Keys.NumPad5:
                case Keys.D5:

                    {
                        Globals.Player.SelectedSpell1 = SpellType.Castle;
                        break;
                    }



                //case Keys.W: StepForward((Globals.STEPSIZETRANSLATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                //case Keys.A: StepLeft((Globals.STEPSIZETRANSLATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                //case Keys.S: StepBackward((Globals.STEPSIZETRANSLATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                //case Keys.D: StepRight((Globals.STEPSIZETRANSLATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                //case Keys.Q: StepUp((Globals.STEPSIZETRANSLATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                //case Keys.Z: StepDown((Globals.STEPSIZETRANSLATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                //case Keys.Left: YawLeft((Globals.STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                //case Keys.Right: YawRight((Globals.STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                //case Keys.Up: PitchUp((Globals.STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                //case Keys.Down: PitchDown((Globals.STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                //case Keys.PageUp: ZoomIn(0.4f); break;
                //case Keys.PageDown: ZoomOut(0.4f); break;
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
        private void GenerateKeyEvents(ref KeyboardState old, ref KeyboardState current)
        {
            foreach (Keys key in keysEnumValues)
            {
                bool released = WasReleased(ref old, ref current, key);
                if (released || IsHeldKey(ref current, key))
                {
                    KeyboardCallback(key, released, ref current, ref old);
                }
            }
        }

        //----------------------------------------------------------------------------------------------
        // This is a way of generating 'pressed' events for keys that we want to hold down
        private bool IsHeldKey(ref KeyboardState current, Keys key)
        {
            return (current.IsKeyDown(key) && ((key == Keys.Left || key == Keys.Right || key == Keys.Up ||
                key == Keys.Down || key == Keys.PageUp || key == Keys.PageDown || key == Keys.A ||
                key == Keys.W || key == Keys.S || key == Keys.D || key == Keys.Q || key == Keys.Z)));
        }
        //----------------------------------------------------------------------------------------------

        private bool WasReleased(ref KeyboardState old, ref KeyboardState current, Keys key)
        {
            // figure out if the key was released between states.
            return old.IsKeyDown(key) && !current.IsKeyDown(key);
        }

        //----------------------------------------------------------------------------------------------

        public void PlaceManaBall(Vector3 position)
        {
            Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.manaball, position);
        }



        protected KeyboardState m_lastKeyboardState;
        
    }
}
