using System;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet.spells;

namespace com.xexuxjy.magiccarpet.gui
{
    public class PlayerStats : GuiComponent
    {
        public PlayerStats(Point topLeft, int width)
            : base(topLeft, width)
        {
            Globals.TrackedObjectChanged += new Globals.TrackedObjectChangedEventHandler(Globals_TrackedObjectChanged);

        }

        void Globals_TrackedObjectChanged(object sender, GameObject oldObject, GameObject newObject)
        {
            m_trackedObject = newObject;
        }


        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(Globals.GraphicsDevice);
            m_spriteFont = Globals.MCContentManager.DebugFont;
            m_renderTarget = new RenderTarget2D(Globals.Game.GraphicsDevice, m_width, m_width, false, SurfaceFormat.Color, DepthFormat.Depth16);
            m_texture = m_renderTarget;
            m_textureUpdateNeeded = true;
            m_spellSpriteAtlas = Globals.MCContentManager.GetTexture("SpellAtlas");
        }

        public override void CheckAndUpdateTexture()
        {
            if (m_textureUpdateNeeded)
            {
                Game.GraphicsDevice.SetRenderTarget(m_renderTarget);
                Globals.GraphicsDevice.Clear(Color.CornflowerBlue);
                m_spriteBatch.Begin();
                int statBarHeight = 33;
                int statBarWidth = 173;

                m_spriteBatch.Draw(Globals.MCContentManager.GetTexture("PlayerStatsFrame"), new Rectangle(0,0,m_width,m_width), Color.White);


                Rectangle healthBarRectangle= new Rectangle(7, 26,statBarWidth,statBarHeight);
                m_spriteBatch.Draw(Globals.MCContentManager.GetTexture(Color.Red), healthBarRectangle, Color.White);

                Rectangle manaBarRectangle = new Rectangle(7, 84, statBarWidth, statBarHeight);
                m_spriteBatch.Draw(Globals.MCContentManager.GetTexture(Color.Blue), manaBarRectangle, Color.White);


                //DrawStat(GameObjectAttributeType.Health, new Point(0, 0), statBarWidth, statBarHeight, Color.Red);
                //DrawStat(GameObjectAttributeType.Mana, new Point(0, statBarHeight), statBarWidth, statBarHeight, Color.Blue);
                // if a magician then draw the spell boxes as well.
                Magician magician = m_trackedObject as Magician;
                if (magician != null)
                {
                    int spellWidth = 64;
                    Point topLeft = new Point(7, 130);
                    DrawCoolDown(magician.SelectedSpell1, magician, topLeft, spellWidth);
                    topLeft = new Point(118, 130);
                    DrawCoolDown(magician.SelectedSpell2, magician, topLeft, spellWidth);

                    // Draw the active spells.
                    magician.SpellComponent.GetActiveSpells();


                }
                m_spriteBatch.End();
                Game.GraphicsDevice.SetRenderTarget(null);
           }
        }

        public void DrawCoolDown(SpellType spellType,Magician magician,Point topLeft,int spellWidth)
        {
            SpellTemplate template = magician.SpellComponent.GetSpellTemplate(spellType);

            SpellTemplate.SpellTemplateState templateState = template.State;
            float drawPercentage = 0f;

            if (templateState == SpellTemplate.SpellTemplateState.Cooldown)
            {
                drawPercentage = template.CoolDownPercentage();
            }
            DrawSpell(template, topLeft, spellWidth, spellWidth, Color.Yellow, drawPercentage);

        }


        public void DrawStat(GameObjectAttributeType type, Point topLeft, int width,int height,Color color)
        {
            String statText = type.ToString();
            Vector2 textDimensions = m_spriteFont.MeasureString(statText);
            m_spriteBatch.DrawString(m_spriteFont,statText,new Vector2(topLeft.X,topLeft.Y),Color.White);

            int rectangleWidth = width - (int)textDimensions.X;

            Rectangle afterText = new Rectangle(topLeft.X + (int)textDimensions.X, topLeft.Y, rectangleWidth, height);

            m_spriteBatch.Draw(Globals.MCContentManager.GetTexture(color),afterText,Color.White);

        }


        public void DrawSpell(SpellTemplate template, Point topLeft, int width, int height, Color color, float drawPercentage)
        {
            if (template != null)
            {

                Rectangle? sourceRectangle = Globals.MCContentManager.SpritePositionForSpellType(template.SpellType);

                if (sourceRectangle.HasValue)
                {
                    m_spriteBatch.Draw(m_spellSpriteAtlas, new Vector2(topLeft.X,topLeft.Y), sourceRectangle, color);
                }

                if (drawPercentage > 0f)
                {
                    Rectangle shadingRect = new Rectangle(topLeft.X, topLeft.Y, width, (int)(height * drawPercentage));
                    Color coolDownColor = Color.Gray;
                    coolDownColor.A = 128;
                    m_spriteBatch.Draw(Globals.MCContentManager.GetTexture(coolDownColor), shadingRect, Color.White);
                }
            }
        }



        private Texture2D m_spellSpriteAtlas;
        private GameObject m_trackedObject;
        private RenderTarget2D m_renderTarget;
        public SpriteFont m_spriteFont;
    }
}
