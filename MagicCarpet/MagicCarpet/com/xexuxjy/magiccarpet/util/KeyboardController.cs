using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.spells;
using GameStateManagement;
using com.xexuxjy.magiccarpet.control;

namespace com.xexuxjy.magiccarpet.util
{
    public class KeyboardController 
    {
        public KeyboardController(PlayerController playerController)
        {
            m_playerController = playerController;

        }

        //----------------------------------------------------------------------------------------------

        public void HandleInput(InputState inputState,GameTime gameTime)
        {
            GenerateKeyEvents(ref inputState.LastKeyboardStates[0], ref inputState.CurrentKeyboardStates[0],gameTime);
        }

        //----------------------------------------------------------------------------------------------

        public virtual void KeyboardCallback(Keys key, bool released, ref KeyboardState newState, ref KeyboardState oldState,GameTime gameTime)
        {
            // forward commands onto the game console
            if (Globals.SimpleConsole.Enabled && released)
            {
                Globals.SimpleConsole.KeyboardCallback(key,released,ref newState,ref oldState);

                // eat the event if its not a console close
                if (key == Keys.Tab)
                {
                    Globals.DisableDebugConsole();
                }
                
                return;
            }

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (key)
            {
                case Keys.Escape:
                    {
                        // FIXME - provide a nice cleanup path for all objects....
                        Globals.CollisionManager.Cleanup();
                        Globals.Game.Exit();
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
                        if (Globals.Player != null)
                        {
                            Globals.Player.SelectedSpell1 = SpellType.Convert;
                        }
                        break;
                    }
                case Keys.NumPad2:
                case Keys.D2:
                    {
                        if (Globals.Player != null)
                        {
                            Globals.Player.SelectedSpell1 = SpellType.Fireball;
                        }
                        break;
                    }
                case Keys.NumPad3:
                case Keys.D3:

                    {
                        if (Globals.Player != null)
                        {

                            Globals.Player.SelectedSpell1 = SpellType.Lower;
                        }
                        break;
                    }
                case Keys.NumPad4:
                case Keys.D4:

                    {
                        if (Globals.Player != null)
                        {

                            Globals.Player.SelectedSpell1 = SpellType.Raise;
                        }
                        break;
                    }
                case Keys.NumPad5:
                case Keys.D5:

                    {
                        if (Globals.Player != null)
                        {

                            Globals.Player.SelectedSpell1 = SpellType.Castle;
                        }
                        break;
                    }
                case Keys.OemPlus:
                case Keys.PageUp:
                    {
                        if (Globals.MiniMap != null)
                        {
                            Globals.MiniMap.ZoomIn();
                        }
                        break;
                    }
                case Keys.OemMinus:
                case Keys.PageDown:
                    {
                        if (Globals.MiniMap != null)
                        {
                            Globals.MiniMap.ZoomOut();
                        }
                        break;
                    }


                case Keys.W: m_playerController.StepForward(Globals.STEPSIZETRANSLATE * elapsedTime); break;
                case Keys.A: m_playerController.StepLeft(Globals.STEPSIZETRANSLATE * elapsedTime); break;
                case Keys.S: m_playerController.StepBackward(Globals.STEPSIZETRANSLATE * elapsedTime); break;
                case Keys.D: m_playerController.StepRight(Globals.STEPSIZETRANSLATE * elapsedTime); break;
                case Keys.Q: m_playerController.StepUp(Globals.STEPSIZETRANSLATE * elapsedTime); break;
                case Keys.Z: m_playerController.StepDown(Globals.STEPSIZETRANSLATE * elapsedTime); break;
                case Keys.Left: m_playerController.UpdateYaw(-Globals.STEPSIZEROTATE * elapsedTime); break;
                case Keys.Right: m_playerController.UpdateYaw(Globals.STEPSIZEROTATE * elapsedTime); break;
                case Keys.Up: m_playerController.UpdatePitch(-Globals.STEPSIZEROTATE * elapsedTime); break;
                case Keys.Down: m_playerController.UpdatePitch(Globals.STEPSIZEROTATE * elapsedTime); break;
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
        private void GenerateKeyEvents(ref KeyboardState old, ref KeyboardState current,GameTime gameTime)
        {
            foreach (Keys key in keysEnumValues)
            {
                bool released = WasReleased(ref old, ref current, key);
                if (released || IsHeldKey(ref current, key))
                {
                    KeyboardCallback(key, released, ref current, ref old,gameTime);
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
            Globals.GameObjectManager.CreateAndInitialiseGameObject("manaball", position);
        }

        private PlayerController m_playerController;
    }
}
