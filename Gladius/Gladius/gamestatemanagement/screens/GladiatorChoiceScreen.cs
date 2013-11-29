using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.gamestatemanagement.screenmanager;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Gladius.events;
using Gladius.modes.shared;
using Gladius.control;
using Gladius.actors;

namespace Gladius.gamestatemanagement.screens
{
    public class GladiatorChoiceScreen : GameScreen
    {
        public override void LoadContent()
        {
            base.LoadContent();
            m_screenBackground = ContentManager.Load<Texture2D>("UI/backgrounds/GladiatorChoiceBackground");
            m_gladiatorSchool = new GladiatorSchool();
            m_gladiatorSchool.Load("Content/CharacterData/CharacterData.xml");
            m_smallFont = ContentManager.Load<SpriteFont>("UI/Fonts/DebugFont8");
            BuildCharacterData();

            RegisterListeners();
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                 
            spriteBatch.Begin();
            spriteBatch.Draw(m_screenBackground, viewport.Bounds, Color.White);
            DrawGladiatorGrid(spriteBatch);
            spriteBatch.End();
        }


        public void RegisterListeners()
        {
            EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
        }


        public void UnregisterListeners()
        {
            //EventManager.ActionPressed -= new event ActionButtonPressed();
            EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);

        }


        void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
        {
            HandleAction(e);
        }


        private void HandleAction(ActionButtonPressedArgs e)
        {
            switch (e.ActionButton)
            {

                case (ActionButton.ActionLeft):
                    {
                        m_cursorPoint.X--;
                        break;
                    }
                case (ActionButton.ActionRight):
                    {
                        m_cursorPoint.X++;
                        break;
                    }
                case (ActionButton.ActionUp):
                    {
                        m_cursorPoint.Y--;
                        break;
                    }
                case (ActionButton.ActionDown):
                    {
                        m_cursorPoint.Y++;
                        break;
                    }
                case (ActionButton.ActionButton1):
                    {
                        break;
                    }
                // cancel
                case (ActionButton.ActionButton2):
                    {
                        break;
                    }

            }

            m_cursorPoint.X = MathHelper.Clamp(m_cursorPoint.X, 0, m_numGladiatorsX-1);
            m_cursorPoint.Y = MathHelper.Clamp(m_cursorPoint.Y, 0, m_numGladiatorsY-1);

        }

        public void DrawGladiatorGrid(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ContentManager.GetColourTexture(Color.White), m_gladiatorsRectangle, Color.White);
            Rectangle dims = new Rectangle(0, 0, m_gladiatorsRectangle.Width / m_numGladiatorsX, m_gladiatorsRectangle.Height / m_numGladiatorsY);
            for (int i = 0; i < m_numGladiatorsX; i++)
            {
                for (int j = 0; j < m_numGladiatorsY; ++j)
                {
                    Color borderColour = Color.White;
                    Point p = new Point(i, j);
                    if (p == m_cursorPoint)
                    {
                        borderColour = Color.Black;
                    }
                    CharacterData currentCharacter = m_characterData[p.X, p.Y];

                    Rectangle r = new Rectangle(p.X * dims.Width, p.Y * dims.Height, dims.Width, dims.Height);
                    r.Location += m_gladiatorsRectangle.Location;
                    spriteBatch.Draw(ContentManager.GetColourTexture(borderColour), r, Color.White);
                    r = Globals.InsetRectangle(r, 2);
                    //spriteBatch.Draw(ContentManager.GetColourTexture(Color.Wheat), r, Color.White);
                    Texture2D tex = ThumbnailTextureForChar(currentCharacter);
                    if(tex == null)
                    {
                        tex = ContentManager.GetColourTexture(Color.Wheat);
                    }
                    spriteBatch.Draw(tex, r, Color.White);

                    String data = "None";
                    if (currentCharacter != null)
                    {
                        data = currentCharacter.Name;
                    }
                    spriteBatch.DrawString(m_smallFont, data, new Vector2(r.X + 2, r.Y + (r.Height / 2)), Color.Black);
                }
            }
        }

        private void BuildCharacterData()
        {
            int counter = 0;
            List<CharacterData> gladiators = m_gladiatorSchool.Gladiators;
            for (int i = 0; i < m_numGladiatorsX; ++i)
            {
                for (int j = 0; j < m_numGladiatorsY; ++j)
                {
                    if (counter < gladiators.Count)
                    {
                        m_characterData[i, j] = gladiators[counter++];
                    }
                    else
                    {
                        m_characterData[i, j] = null;
                    }
                }
            }
        }

        private Texture2D ThumbnailTextureForChar(CharacterData characterData)
        {
            String path = ThumbnailNameForChar(characterData);
            if(path != null)
            {
            return ContentManager.Load<Texture2D>(path);
            }
            return null;

        }

        private String ThumbnailNameForChar(CharacterData characterData)
        {
            if (characterData != null)
            {
                String directory = "UI/characters/small/";
                switch (characterData.ActorClass)
                {
                    case ActorClass.Urlan: return directory + "barbarian-male";
                    case ActorClass.Ursula: return directory + "channeller";
                    case ActorClass.Amazon: return directory + "channeller";
                    case ActorClass.Barbarian: return directory + "barbarian-male";
                }
            }
            return null;
        }



        private Point m_cursorPoint = new Point();
        
        private const int thumbnailDim = 64;        
        public const int m_numGladiatorsX = 8;
        public const int m_numGladiatorsY = 4;

        private Rectangle m_gladiatorsRectangle = new Rectangle(16, 16, thumbnailDim * m_numGladiatorsX,thumbnailDim * m_numGladiatorsY );
        

        public CharacterData[,] m_characterData = new CharacterData[m_numGladiatorsX, m_numGladiatorsY];

        private SpriteFont m_smallFont;
        private GladiatorSchool m_gladiatorSchool;
        private Texture2D m_screenBackground;

    }

    enum GCSFocus
    {
        Gladiators,
        Arena
    }

}
