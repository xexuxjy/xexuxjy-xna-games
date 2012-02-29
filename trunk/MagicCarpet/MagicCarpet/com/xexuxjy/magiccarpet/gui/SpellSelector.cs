using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.spells;
using Microsoft.Xna.Framework;
using GameStateManagement;
using Microsoft.Xna.Framework.Input;
using com.xexuxjy.magiccarpet.gameobjects;
using BulletXNA;
using Microsoft.Xna.Framework.Graphics;

namespace com.xexuxjy.magiccarpet.gui
{
    public class SpellSelector : GuiComponent
    {
        public SpellSelector(Point topLeft,int width) : base(topLeft,width)
        {
            m_enabled = true;
            m_hasGuiControl = true;
            m_textureUpdateNeeded = true;
            Visible = false;
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            //m_renderTarget = new RenderTarget2D(Globals.Game.GraphicsDevice, m_width, m_width, false, SurfaceFormat.Color, DepthFormat.Depth16);
            //m_texture = m_renderTarget;
            m_colorData = new Color[m_width * m_width];

            int radius = m_width / 2;

            m_arrowTexture = new Texture2D(Globals.Game.GraphicsDevice, radius, 30);
            Color[] temp = new Color[m_arrowTexture.Width * m_arrowTexture.Height];

            Color color = Color.White;
            color.A = 40;
            for (int i = 0; i < temp.Length; ++i)
            {
                temp[i] = color;
            }
            m_arrowTexture.SetData<Color>(temp);

        }


        public override void CheckAndUpdateTexture()
        {
            //if (m_enabled && m_hasGuiControl && m_textureUpdateNeeded)
            if(m_textureUpdateNeeded)
            {
                DrawFilledCircle(m_colorData, m_width, m_width / 2, m_width / 2, 50, Color.White, Color.Transparent, true);

                float arcWidth = MathUtil.SIMD_2_PI / m_spellTypes.Length;
                float startAngle = m_currentSelected * arcWidth ;

                DrawFilledArc(m_colorData, m_width, m_width / 2, m_width / 2, 50, startAngle,arcWidth,Color.Blue, Color.Transparent, true);

                m_texture.SetData<Color>(m_colorData);
                m_textureUpdateNeeded = false;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            Texture2D selector = Globals.MCContentManager.GetTexture("SpellSelector");
            Texture2D spellAtlas = Globals.MCContentManager.GetTexture("SpellAtlas");

            m_spriteBatch.Draw(selector,m_rectangle,Color.White);


            int spellWidth = 32;
            int radius = m_width / 2;

            int numSegments = m_spellTypes.Length;
            float segmentSpan = MathUtil.SIMD_2_PI / numSegments;

            Vector2 center = new Vector2(m_rectangle.X + radius, m_rectangle.Y + radius);
            Vector2 spellOffset = new Vector2(spellWidth, spellWidth);

            int spellAdjustedRadius = radius - spellWidth;
            spellAdjustedRadius -= 10;


            for (int i = 0; i < m_spellTypes.Length; ++i)
            {
                int x = (int)(Math.Sin(segmentSpan * i) * spellAdjustedRadius);
                int y = (int)(Math.Cos(segmentSpan * i) * spellAdjustedRadius);


                Rectangle? sourceRectangle = Globals.MCContentManager.SpritePositionForSpellType(m_spellTypes[i]);

                if (sourceRectangle.HasValue)
                {
                    Vector2 offset = new Vector2(x,y);
                    Vector2 position = center + offset;
                    position -= spellOffset;
                    m_spriteBatch.Draw(spellAtlas, position, sourceRectangle, Color.White);
                }

//int xPos = (int)player.Position.X;
// int yPos = (int)player.Position.Y;
// Vector2 cannonOrigin = new Vector2(11, 50);
 
// spriteBatch.Draw(cannonTexture, new Vector2(xPos + 20, yPos - 10), null, player.Color, player.Angle, cannonOrigin, playerScaling, SpriteEffects.None, 1);

                float arcWidth = MathUtil.SIMD_2_PI / m_spellTypes.Length;
                float selectorAngle = m_currentSelected * arcWidth;

                // invert Y here?
                selectorAngle = GetSelectorAngle();
                //selectorAngle += MathUtil.SIMD_HALF_PI/2f;


                m_spriteBatch.Draw(m_arrowTexture, center, null, Color.White, selectorAngle, new Vector2(0, m_arrowTexture.Height / 2), 1f, SpriteEffects.None, 1);
            }
            m_spriteBatch.End();

        
        }



            
        public override void HandleInput(InputState inputState)
        {
            GamePadState gamePadState = inputState.CurrentControllingPadState();

            m_selectorDirection = gamePadState.ThumbSticks.Right;
            m_selectorDirection.Normalize();
            
            bool spell1Selected =inputState.IsNewButtonPress(Buttons.LeftTrigger);
            bool spell2Selected =inputState.IsNewButtonPress(Buttons.RightTrigger);

            if (spell1Selected)
            {
                m_selectedSpell1 = GetSelectedSpell();
                // bit ugly
                Globals.Player.SelectedSpell1 = m_selectedSpell1;
            }

            if (spell2Selected)
            {
                m_selectedSpell2 = GetSelectedSpell();
                // bit ugly
                Globals.Player.SelectedSpell2 = m_selectedSpell2;
            }


            if (spell1Selected || spell2Selected)
            {
                // close selector
                HasGuiControl = false;
                Visible = false;
            }


        }

        public float GetSelectorAngle()
        {
            // invert Y here?
            return (float)Math.Atan2(-m_selectorDirection.Y, m_selectorDirection.X);
        }


        public SpellType GetSelectedSpell()
        {
            float selectorAngle = GetSelectorAngle();
            int numSegments = m_spellTypes.Length;
            float segmentSpan = MathUtil.SIMD_2_PI / numSegments;
            float halfSegmentSpan = segmentSpan / 2f;

            float low  = 0;
            float high = segmentSpan;
            int index = -1;

            for (int i = 0; i < numSegments; ++i)
            {
                if (selectorAngle >= low && selectorAngle <= high)
                {
                    index = i;
                    break;
                }
                low = high;
                high += segmentSpan;
            }


            //// rotate selector and adjust
            //selectorAngle -= halfSegmentSpan;
            //if(selectorAngle < 0)
            //{
            //    selectorAngle += MathUtil.SIMD_2_PI;
            //}

            //float lastAngle = 0;
            //int index = 0;
            //for(int i=0;i<numSegments;++i)
            //{
            //    float newAngle = segmentSpan * i;
            //    if(selectorAngle >= lastAngle && selectorAngle < newAngle)
            //    {
            //        index = i;
            //        break;
            //    }
            //    lastAngle = newAngle;
            //}
            if (index > -1)
            {
                return m_spellTypes[index];
            }
            else
            {
                return SpellType.None;
            }
        }


        public override void Update(GameTime gameTime)
        {
            m_elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (m_elapsed > m_updateFrequency)
            {
                m_elapsed = 0f;
                m_currentSelected++;
                m_currentSelected %= m_spellTypes.Length;
                m_textureUpdateNeeded = true;
            }
        }



        public SpellType SelectedSpell1
        {
            get { return m_selectedSpell1; }
        }

        public SpellType SelectedSpell2
        {
            get { return m_selectedSpell2; }
        }



        private float m_updateFrequency = 2f;
        private float m_elapsed;
        private int m_currentSelected = 0;

        private Magician m_magician;
        private SpellType[] m_spellTypes = new SpellType[] { SpellType.Castle, SpellType.Convert, SpellType.Fireball, SpellType.Lower, SpellType.Raise };
        private SpellType m_selectedSpell1;
        private SpellType m_selectedSpell2;
        private RenderTarget2D m_renderTarget;
        private Texture2D m_arrowTexture;

        private Vector2 m_selectorDirection;
    }
}
