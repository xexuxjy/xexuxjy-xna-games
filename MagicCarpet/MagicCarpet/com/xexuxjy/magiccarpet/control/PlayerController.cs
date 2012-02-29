using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.util;
using GameStateManagement;
using com.xexuxjy.magiccarpet.gui;
using Microsoft.Xna.Framework.Input;

namespace com.xexuxjy.magiccarpet.control
{
    public class PlayerController
    {

        public PlayerController(PlayerHud playerHud)
        {
            m_keyboardController = new KeyboardController();
            m_mouseController = new MouseController();

            m_playerHud = playerHud;

        }



        public virtual void HandleInput(InputState inputState)
        {
            m_mouseController.HandleInput(inputState);
            m_keyboardController.HandleInput(inputState);


            if (inputState.IsNewButtonPress(Buttons.Y))
            {
                m_playerHud.ToggleSpellSelector();
            }



        }


        PlayerHud m_playerHud;
        MouseController m_mouseController;
        KeyboardController m_keyboardController;   

    }
}
