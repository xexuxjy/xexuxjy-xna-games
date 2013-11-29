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
using Gladius.renderer;
using ThirdPartyNinjas.XnaUtility;
using Gladius.actors;

namespace Gladius.gamestatemanagement.screens
{
    public class GladiatorChoiceScreen : GameScreen
    {
        public override void LoadContent()
        {
            base.LoadContent();
            m_screenBackground = ContentManager.Load<Texture2D>("UI/backgrounds/PlainBackground");
            m_gladiatorSchool = new GladiatorSchool();
            m_gladiatorSchool.Load("Content/CharacterData/CharacterData.xml");
            m_smallFont = ContentManager.Load<SpriteFont>("UI/Fonts/DebugFont8");
            m_headerFont = ContentManager.Load<SpriteFont>("UI/Fonts/UIFontLarge");
            m_atlasTexture = ContentManager.Load<Texture2D>("UI/Characters/thumbnail/atlas_texture");
            m_textureAtlas = ContentManager.Load<TextureAtlas>("UI/Characters/thumbnail/atlas");

            BuildCharacterData();
            FocusMode = GCSFocus.Selected;

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
            DrawGladiatorGrid(m_selectedCharacters, spriteBatch, m_selectedCharactersRectangle, "Orins School", m_headerFont, FocusMode == GCSFocus.Selected);
            DrawGladiatorGrid(m_availableCharacters, spriteBatch, m_availableCharactersRectangle, "Available Gladiators", m_headerFont, FocusMode == GCSFocus.Available);


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
                        m_curentViewPort.ScrollUp();
                        break;
                    }
                case (ActionButton.ActionDown):
                    {
                        m_cursorPoint.Y++;
                        m_curentViewPort.ScrollDown();
                        break;
                    }
                case (ActionButton.ActionButton1):
                    {
                        if (FocusMode == GCSFocus.Available)
                        {
                            // add character to selection
                            AddCharacterToSelected(CharacterDataForPoint(m_availableCharacters, m_cursorPoint));
                        }

                        break;
                    }
                // cancel
                case (ActionButton.ActionButton2):
                    {
                        if (FocusMode == GCSFocus.Available)
                        {
                            // add character to selection
                            RemoveCharacterFromSelected(CharacterDataForPoint(m_selectedCharacters, m_cursorPoint));
                        }
                        break;
                    }

            }

            m_cursorPoint.X = MathHelper.Clamp(m_cursorPoint.X, 0, m_numGladiatorsX - 1);

            int totalYRange = m_numGladiatorsY + m_availableCharacters.Count() % m_numGladiatorsX;

            m_cursorPoint.Y = MathHelper.Clamp(m_cursorPoint.Y, 0, totalYRange);

            FocusMode = FocusModeForPoint(m_cursorPoint);
        }

        bool InSelectionWindow(Point p)
        {
            int bottomOfSelectedWindow = m_numGladiatorsY;
            return (p.Y < bottomOfSelectedWindow);
        }

        bool InAvailableWindow(Point p)
        {
            return !InSelectionWindow(p);
        }

        public GCSFocus FocusModeForPoint(Point p)
        {
            // left and right always handled by width.
            p.X = MathHelper.Clamp(p.X, 0, m_numGladiatorsX - 1);
            // up and down depend... on where we are.
            // bottom of 
            // chain next/previous windows?
            int bottomOfSelectedWindow = m_numGladiatorsY;

            if (p.Y >= bottomOfSelectedWindow)
            {
                return GCSFocus.Available;
            }

            return GCSFocus.Selected;

        }


        public void AddCharacterToSelected(CharacterData characterData)
        {
            if (!m_selectedCharacters.Contains(characterData))
            {
                m_selectedCharacters.Add(characterData);
            }
        }

        public void RemoveCharacterFromSelected(CharacterData characterData)
        {
            m_selectedCharacters.Remove(characterData);
        }

        public void DrawGladiatorGrid(List<CharacterData> list, SpriteBatch spriteBatch, Rectangle toDraw, String header, SpriteFont headerFont, bool hasFocus)
        {
            Rectangle adjustedRect = toDraw;

            Vector2 textDims = headerFont.MeasureString(header);
            GraphicsHelper.DrawShadowedText(spriteBatch, headerFont, header, adjustedRect.Location);

            adjustedRect.Y += (int)textDims.Y + 3;

            Rectangle dims = ThumbnailDims;//new Rectangle(0, 0, adjustedRect.Width / m_numGladiatorsX, adjustedRect.Height / m_numGladiatorsY);
            for (int i = 0; i < m_numGladiatorsX; i++)
            {
                for (int j = 0; j < m_numGladiatorsY; ++j)
                {
                    Point p = new Point(i, j);
                    Rectangle r = new Rectangle(p.X * dims.Width, p.Y * dims.Height, dims.Width, dims.Height);
                    r.Location += adjustedRect.Location;
                    if (hasFocus)
                    {
                        Color borderColour = Color.White;
                        if (p == m_cursorPoint)
                        {
                            borderColour = Color.Black;
                        }
                        spriteBatch.Draw(ContentManager.GetColourTexture(borderColour), r, Color.White);
                    }
                    r = Globals.InsetRectangle(r, 2);

                    p = WindowForAvailable(p);

                    CharacterData currentCharacter = CharacterDataForPoint(list, p);
                    TextureRegion region = RegionForChar(currentCharacter);
                    spriteBatch.Draw(m_atlasTexture, r, region.Bounds, Color.White);

                    if (currentCharacter != null)
                    {
                        Vector2 levelTextPos = new Vector2(r.X, r.Y) + (new Vector2(dims.Width) * 0.7f);
                        GraphicsHelper.DrawShadowedText(spriteBatch, headerFont, "" + currentCharacter.Level, levelTextPos);
                    }
                }
            }
        }


        Point WindowForAvailable(Point p)
        {
            if (InAvailableWindow(p))
            {
                int numRows = m_availableCharacters.Count() % m_numGladiatorsX;
                // update for selected
                p.Y -= m_numGladiatorsY; 

            }
            return p;
        }



        public void DrawSelectedCharacterInfo()
        {

        }





        public CharacterData CharacterDataForPoint(List<CharacterData> list, Point p)
        {
            int index = (p.Y * m_numGladiatorsX) + p.X;
            if (index < list.Count)
            {
                return list[index];
            }
            return null;
        }

        private void BuildCharacterData()
        {
            int counter = 0;
            List<CharacterData> gladiators = m_gladiatorSchool.Gladiators;
            m_availableCharacters.Clear();
            m_availableCharacters.AddRange(gladiators);
            m_selectedCharacters.Clear();

            m_availableViewPort = new ViewPort(new Rectangle(0, 0, m_numGladiatorsX, m_selectedCharacters.Count % m_numGladiatorsY),new Rectangle(0,0,m_numGladiatorsX,m_numGladiatorsY),this);
            m_selectedViewPort = new ViewPort(new Rectangle(0, 0, m_numGladiatorsX, m_numGladiatorsY), new Rectangle(0, 0, m_numGladiatorsX, m_numGladiatorsY),this);
            m_curentViewPort = m_availableViewPort;
            m_selectedViewPort.Next = m_availableViewPort;
            m_availableViewPort.Prev = m_selectedViewPort;
        }


        private TextureRegion RegionForChar(CharacterData characterData)
        {
            if (characterData != null)
            {
                switch (characterData.ActorClass)
                {
                    case ActorClass.Urlan: return m_textureAtlas.GetRegion("barbarian-male.png");
                    case ActorClass.Ursula: return m_textureAtlas.GetRegion("channeller.png");
                    case ActorClass.Amazon: return m_textureAtlas.GetRegion("channeller.png");
                    case ActorClass.Barbarian: return m_textureAtlas.GetRegion("barbarian-male.png");
                }
            }
            return m_textureAtlas.GetRegion("unavailableSlot.png");
        }


        private CharacterData SelectedCharacter
        {
            get
            {
                return null;
            }
        }

        private GCSFocus FocusMode
        {
            get;
            set;
        }

        public void ViewPortChanged(ViewPort oldvp, ViewPort newvp)
        {
            m_curentViewPort = newvp;
        }

        ViewPort m_curentViewPort;
        ViewPort m_availableViewPort;
        ViewPort m_selectedViewPort;

        private Point m_cursorPoint = new Point();

        private Rectangle m_selectedCharactersRectangle = new Rectangle(16, 16, 400, 300);
        private Rectangle m_availableCharactersRectangle = new Rectangle(16, 350, 400, 300);
        private Rectangle m_currentCharacterRectangle = new Rectangle(450, 16, 300, 600);

        public static Rectangle ThumbnailDims = new Rectangle(0, 0, 96, 96);

        public List<CharacterData> m_availableCharacters = new List<CharacterData>();
        public List<CharacterData> m_selectedCharacters = new List<CharacterData>();

        private SpriteFont m_smallFont;
        private SpriteFont m_headerFont;

        private GladiatorSchool m_gladiatorSchool;
        private BattleData m_currentBattle;

        private Texture2D m_screenBackground;
        private Texture2D m_atlasTexture;
        private TextureAtlas m_textureAtlas;

        public const int m_numGladiatorsX = 5;
        public const int m_numGladiatorsY = 2;
    }


    public class ViewPort
    {
        private Rectangle m_total;
        private Rectangle m_visibleArea;
        private Rectangle m_currentlyVisible;
        private GladiatorChoiceScreen m_callback;

        public ViewPort(Rectangle total, Rectangle visible,GladiatorChoiceScreen callback)
        {
            m_total = total;
            m_visibleArea = visible;
            m_currentlyVisible = m_visibleArea;
            m_callback = callback;
        }

        public Rectangle CurrentlyVisible
        {
            get
            {
                return m_currentlyVisible;
            }
         
        }

        public void ScrollUp()
        {
            m_currentlyVisible.Y--;

            if (m_currentlyVisible.Y < 0 && Prev != null)
            {
                m_callback.ViewPortChanged(this,Prev);
            }
            m_currentlyVisible.Y = MathHelper.Clamp(m_currentlyVisible.Y, 0, m_total.Y);
            
        }

        public void ScrollDown()
        {
            m_currentlyVisible.Y++;
            m_currentlyVisible.Y = MathHelper.Clamp(m_currentlyVisible.Y, 0, m_total.Y);
        }

        public ViewPort Next
        {
            get;
            set;
        }

        public ViewPort Prev
        {
            get;
            set;
        }


    }



    public enum GCSFocus
    {
        Available,
        Selected
    }

}
