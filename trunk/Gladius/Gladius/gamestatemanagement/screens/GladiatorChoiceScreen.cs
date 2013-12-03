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
using Gladius.util;

namespace Gladius.gamestatemanagement.screens
{
    public class GladiatorChoiceScreen : GameScreen, ICharacterViewPortCallback
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
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);

            spriteBatch.Begin();
            spriteBatch.Draw(m_screenBackground, viewport.Bounds, Color.White);

            m_selectedViewPort.Draw(spriteBatch, m_selectedCharactersRectangle);
            m_availableViewPort.Draw(spriteBatch, m_availableCharactersRectangle);
            DrawSelectedCharacterInfo(spriteBatch);

            m_nextButton.Draw(spriteBatch, m_nextButtonRectangle);

            spriteBatch.End();
        }


        public override void RegisterListeners()
        {
            EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
        }


        public override void UnregisterListeners()
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
                        CurrentControl.ScrollLeft();
                        break;
                    }
                case (ActionButton.ActionRight):
                    {
                        CurrentControl.ScrollRight();
                        break;
                    }
                case (ActionButton.ActionUp):
                    {
                        CurrentControl.ScrollUp();
                        break;
                    }
                case (ActionButton.ActionDown):
                    {
                        CurrentControl.ScrollDown();
                        break;
                    }
                case (ActionButton.ActionButton1):
                    {
                        if (CurrentControl == m_availableViewPort)
                        {
                            // add character to selection
                            AddCharacterToSelected(SelectedCharacter);
                        }

                        break;
                    }
                // cancel
                case (ActionButton.ActionButton2):
                    {
                        if (CurrentControl == m_availableViewPort)
                        {
                            // add character to selection
                            RemoveCharacterFromSelected(SelectedCharacter);
                        }
                        break;
                    }

            }
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

        public void DrawSelectedCharacterInfo(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ContentManager.GetColourTexture(Color.White), m_currentCharacterRectangle, Color.White);
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

            Rectangle standardRect = new Rectangle(0, 0, m_numGladiatorsX, m_numGladiatorsY);

            int totalHeight = (m_availableCharacters.Count / m_numGladiatorsY) + (((m_availableCharacters.Count % m_numGladiatorsY) == 0) ? 0 : 1);

            m_availableViewPort = new CharacterViewPort(new Rectangle(0, 0, m_numGladiatorsX, totalHeight), standardRect, this, m_availableCharacters);
            m_availableViewPort.Text = "Orin's School";
            m_selectedViewPort = new CharacterViewPort(standardRect, standardRect, this, m_selectedCharacters);
            m_selectedViewPort.Text = "Available";

            m_selectedViewPort.SpriteFont = m_headerFont;
            m_availableViewPort.SpriteFont = m_headerFont;

            m_nextButton = new Button();
            m_nextButton.Text = "Next";
            m_nextButton.SpriteFont = m_headerFont;
            m_nextButton.ContentManager = ContentManager;
            

            CurrentControl = m_availableViewPort;
            m_selectedViewPort.Next = m_availableViewPort;
            m_availableViewPort.Prev = m_selectedViewPort;
            m_availableViewPort.Next = m_nextButton;
            m_nextButton.Prev = m_availableViewPort;

        }


        public TextureRegion RegionForChar(CharacterData characterData)
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

        public void UIControlChanged(UIControl oldControl, UIControl newControl, bool up)
        {
            CurrentControl = newControl;
            CharacterViewPort oldvp = oldControl as CharacterViewPort;
            CharacterViewPort newvp = newControl as CharacterViewPort;
            if (oldvp != null && newvp != null)
            {
                Point p = oldvp.m_cursorPoint;
                p.Y = up ? newvp.m_total.Height - 1 : 0;
                newvp.m_cursorPoint = p;
            }
        }

        public Texture2D AtlasTexture
        {
            get { return m_atlasTexture; }
        }

        public UIControl CurrentControl
        {
            get
            {
                return m_currentControl;
            }
            set
            {
                if (m_currentControl != null)
                {
                    m_currentControl.HasFocus = false;
                }
                m_currentControl = value;
                m_currentControl.HasFocus = true;
            }
        }


        UIControl m_currentControl;
        CharacterViewPort m_availableViewPort;
        CharacterViewPort m_selectedViewPort;
        Button m_nextButton;

        private Rectangle m_selectedCharactersRectangle = new Rectangle(16, 16, 400, 300);
        private Rectangle m_availableCharactersRectangle = new Rectangle(16, 350, 400, 300);
        private Rectangle m_currentCharacterRectangle = new Rectangle(550, 16, 300, 500);
        private Rectangle m_nextButtonRectangle = new Rectangle(550, 550, 80, 40);

        public static Rectangle ThumbnailDims = new Rectangle(0, 0, 96, 96);

        public List<CharacterData> m_availableCharacters = new List<CharacterData>();
        public List<CharacterData> m_selectedCharacters = new List<CharacterData>();

        private SpriteFont m_smallFont;
        private SpriteFont m_headerFont;

        private GladiatorSchool m_gladiatorSchool;
        private BattleData m_currentBattle;

        private Texture2D m_screenBackground;
        public Texture2D m_atlasTexture;
        private TextureAtlas m_textureAtlas;

        public const int m_numGladiatorsX = 5;
        public const int m_numGladiatorsY = 2;
    }

    public interface IUIControlCallback
    {
        void UIControlChanged(UIControl oldvp, UIControl newvp, bool up);

    }

    public interface ICharacterViewPortCallback : IUIControlCallback
    {
        UIControl CurrentControl { get; }
        ThreadSafeContentManager ContentManager { get; }
        Texture2D AtlasTexture {get;}
        TextureRegion RegionForChar(CharacterData characterData);
    }


    public abstract class UIControl
    {
        public UIControl Prev
        {
            get;
            set;
        }
        public UIControl Next
        {
            get;
            set;
        }

        public String Text
        {
            get;
            set;
        }

        public SpriteFont SpriteFont
        {
            get;
            set;
        }

        public Rectangle VisibleArea
        {
            get;
            set;
        }

        public bool HasFocus
        {
            get;
            set;
        }

        public ThreadSafeContentManager ContentManager
        {
            get;
            set;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Rectangle toDraw) { }

        public virtual void ScrollUp() { }
        public virtual void ScrollDown() { }
        public virtual void ScrollLeft() { }
        public virtual void ScrollRight() { }
    }


    public class Button : UIControl
    {
        public override void Draw(SpriteBatch spriteBatch, Rectangle toDraw)
        {
            if (HasFocus)
            {
                spriteBatch.Draw(ContentManager.GetColourTexture(Color.White), toDraw, Color.Black);
                Rectangle inset = Globals.InsetRectangle(toDraw, 2);
                spriteBatch.Draw(ContentManager.GetColourTexture(Color.White), inset, Color.White);
            }
            else
            {
                spriteBatch.Draw(ContentManager.GetColourTexture(Color.White), toDraw, Color.White);
            }
            Vector2 centerOfRect = new Vector2(toDraw.Width,toDraw.Height) / 2f;
            centerOfRect += new Vector2(toDraw.X,toDraw.Y);
            GraphicsHelper.DrawCenteredText(spriteBatch, SpriteFont, Text, centerOfRect, Color.Black);
        }
        
    
    }



    public class CharacterViewPort : UIControl
    {
        public Rectangle m_total;
        private Rectangle m_currentlyVisible;
        private ICharacterViewPortCallback m_callback;
        public Point m_cursorPoint = new Point();
        private List<CharacterData> m_characterList = new List<CharacterData>();

        public CharacterViewPort(Rectangle total, Rectangle visible, ICharacterViewPortCallback callback, List<CharacterData> characterList)
        {
            m_total = total;
            VisibleArea = visible;
            m_currentlyVisible = VisibleArea;
            m_callback = callback;
            m_characterList = characterList;
        }

        public Rectangle CurrentlyVisible
        {
            get
            {
                return m_currentlyVisible;
            }
        }

        public void AdjustWindow()
        {
            m_cursorPoint.X = MathHelper.Clamp(m_cursorPoint.X, 0, m_total.Width-1);
            m_cursorPoint.Y = MathHelper.Clamp(m_cursorPoint.Y, 0, m_total.Height- 1);

            m_currentlyVisible.X = MathHelper.Clamp(m_currentlyVisible.X, 0, m_total.X);
            m_currentlyVisible.Y = MathHelper.Clamp(m_currentlyVisible.Y, 0, m_total.Y);

        }

        public override void ScrollLeft()
        {
            m_cursorPoint.X--;
            AdjustWindow();
        }

        public override void ScrollRight()
        {
            m_cursorPoint.X++;
            AdjustWindow();
        }

        public override void ScrollUp()
        {
            m_cursorPoint.Y--;

            if (m_cursorPoint.Y < 0 && Prev != null)
            {
                m_callback.UIControlChanged(this, Prev,true);
            }
            AdjustWindow();
        }

        public override void ScrollDown()
        {
            m_cursorPoint.Y++;

            if (m_cursorPoint.Y > m_total.Height -1 && Next != null)
            {
                m_callback.UIControlChanged(this, Next, false);

            }
            AdjustWindow();
        }

        public CharacterData SelectedCharacter
        {
            get
            {
                int index = ((m_cursorPoint.Y) * VisibleArea.Width) + m_cursorPoint.X;
                if (index < m_characterList.Count)
                {
                    return m_characterList[index];
                }
                return null;
            }
        }


        public override void Draw(SpriteBatch spriteBatch, Rectangle toDraw)
        {
            Rectangle adjustedRect = toDraw;

            Vector2 textDims = SpriteFont.MeasureString(Text);
            GraphicsHelper.DrawShadowedText(spriteBatch, SpriteFont, Text, adjustedRect.Location);

            adjustedRect.Y += (int)textDims.Y + 3;

            Rectangle dims = GladiatorChoiceScreen.ThumbnailDims;
            for (int i = 0; i < VisibleArea.Width; i++)
            {
                for (int j = 0; j < VisibleArea.Height; ++j)
                {
                    Point p = new Point(i, j);
                    Rectangle r = new Rectangle(p.X * dims.Width, p.Y * dims.Height, dims.Width, dims.Height);
                    r.Location += adjustedRect.Location;
                    if (m_callback.CurrentControl == this)
                    {
                        Color borderColour = Color.White;
                        if (p == m_cursorPoint)
                        {
                            borderColour = Color.Black;
                        }
                        spriteBatch.Draw(m_callback.ContentManager.GetColourTexture(borderColour), r, Color.White);
                    }
                    r = Globals.InsetRectangle(r, 2);

                    int index = (j * VisibleArea.Width) + i;
                    CharacterData currentCharacter = null;
                    if (index < m_characterList.Count)
                    {
                        currentCharacter = m_characterList[index];
                    }

                    TextureRegion region = m_callback.RegionForChar(currentCharacter);
                    spriteBatch.Draw(m_callback.AtlasTexture, r, region.Bounds, Color.White);

                    // draw level indicator.
                    if (currentCharacter != null)
                    {
                        Vector2 levelTextPos = new Vector2(r.X, r.Y) + (new Vector2(dims.Width) * 0.7f);
                        GraphicsHelper.DrawShadowedText(spriteBatch, SpriteFont, "" + currentCharacter.Level, levelTextPos);
                    }
                }
            }
        }
    }
}
