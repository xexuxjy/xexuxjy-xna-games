using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.gamestatemanagement.screenmanager;
using Gladius.modes.shared;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Gladius.renderer;

namespace Gladius.gamestatemanagement.screens
{
    public class GladiatorPlacementScreen : GameScreen
    {
        public GladiatorPlacementScreen(List<CharacterData> chosenCharacters)
        {
        }

        public override void LoadContent()
        {
            m_screenBackground = ContentManager.Load<Texture2D>("UI/backgrounds/PlainBackground");
            m_smallFont = ContentManager.Load<SpriteFont>("UI/Fonts/DebugFont8");
            m_headerFont = ContentManager.Load<SpriteFont>("UI/Fonts/UIFontLarge");
        }



        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);

            spriteBatch.Begin();
            spriteBatch.Draw(m_screenBackground, viewport.Bounds, Color.White);

            GraphicsHelper.DrawCenteredText(spriteBatch, m_headerFont, "Placement!", viewportSize / 2f, Color.White);

            spriteBatch.End();

        }



        public void FillPosition(Point p,CharacterData character)
        {
            Debug.Assert(m_availablePositions.Contains(p) && character.StartPosition.HasValue == false);
            if(m_availablePositions.Contains(p) && character.StartPosition.HasValue == false)
            {
                m_characterMap[p] = character;
                m_availablePositions.Remove(p);
                m_filledPositions.Add(p);
                character.StartPosition = p;
            }
        }

        public void EmptyPosition(Point p)
        {
            Debug.Assert(m_filledPositions.Contains(p));
            if(m_filledPositions.Contains(p))
            {
                CharacterData character;
                if(m_characterMap.TryGetValue(p,out character))
                {
                    character.StartPosition = null;
                }
                m_filledPositions.Remove(p);
                m_availablePositions.Add(p);
            }
        }

        public Dictionary<Point, CharacterData> PlacementMap
        {
            get
            {
                return m_characterMap;
            }
        }

        private SpriteFont m_smallFont;
        private SpriteFont m_headerFont;

        public Dictionary<Point,CharacterData> m_characterMap = new Dictionary<Point,CharacterData>();
        public List<CharacterData> m_characters = new List<CharacterData>();
        public List<Point> m_availablePositions = new List<Point>();
        public List<Point> m_filledPositions = new List<Point>();
    }
}
