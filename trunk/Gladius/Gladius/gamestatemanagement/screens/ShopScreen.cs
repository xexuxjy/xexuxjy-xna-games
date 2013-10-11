using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameStateManagement;
using Microsoft.Xna.Framework.Content;
using Gladius.util;
using Gladius.modes.shared;
using Gladius.events;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace Gladius.gamestatemanagement.screens
{
    public class ShopScreen : GameScreen
    {
    //    public override void LoadContent()
    //    {
    //        if (m_content == null)
    //        {
    //            m_content = new ContentManager(ScreenManager.Game.Services, "Content");
    //        }

    //        // once the load has finished, we use ResetElapsedTime to tell the game's
    //        // timing mechanism that we have just finished a very long frame, and that
    //        // it should not try to catch up.
    //        ScreenManager.Game.ResetElapsedTime();

    //        m_screenComponents = new ScreenGameComponents(ScreenManager.Game);

    //        ItemManager itemManager = new ItemManager();
    //        itemManager.LoadItems("Content/ItemData/CompleteItemData.csv");
    //        int ibreak = 0;

    //        m_background = m_content.Load<Texture2D>("UI/backgrounds/ScrollBackground");
    //        m_spriteFont = m_content.Load<SpriteFont>("UI/fonts/ShopFont");
    //        SetupShop();
    //    }

    //    public void RegisterListeners()
    //    {
    //        EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed); 
    //    }

    //    void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
    //    {
            

    //    }



    //    public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    //    {
    //        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    //        if (m_gui != null)
    //        {
    //            m_gui.Update();
    //        }
    //    }

    //    public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
    //    {
    //        base.Draw(gameTime);
    //        m_gui.Draw();
    //    }


    //    public void UnregisterListeners()
    //    {
    //        EventManager.ActionPressed -= EventManager_ActionPressed; 
    //    }


    //    /// <summary>
    //    /// Unload graphics content used by the game.
    //    /// </summary>
    //    public override void UnloadContent()
    //    {
    //        m_content.Unload();
    //    }


    //    public void SetupShop()
    //    {
    //        //Color = Color.White;
    //        String skinMap = null;
    //        using (StreamReader sr = new StreamReader(TitleContainer.OpenStream("Content/UI/shop/ShopMap.txt")))
    //        {
    //            skinMap = sr.ReadToEnd();
    //        }
    //        m_skin = new Skin(m_background, skinMap);
    //        m_text = new Text(m_spriteFont, Color.Black);

    //        m_gui = new Gui(ScreenManager.Game, m_skin, m_text);
    //            //Widgets = new Widget[] {       }                                    

    //    }

    //    private Texture2D m_background;
    //    private Gui m_gui;
    //    private Skin m_skin;
    //    private Text m_text;
    //    private SpriteFont m_spriteFont;
    }
}
