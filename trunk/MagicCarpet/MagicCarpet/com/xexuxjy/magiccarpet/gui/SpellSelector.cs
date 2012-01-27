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
        public SpellSelector(int x, int y,int width) : base(x,y,width)
        {
            m_enabled = true;
            m_hasGuiControl = true;
            m_textureUpdateNeeded = true;
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            m_texture = new Texture2D(Game.GraphicsDevice, m_width, m_width, false, SurfaceFormat.Color);
            m_colorData = new Color[m_width * m_width];
        }
        


        public override void Update(GameTime gameTime)
        {
            if (m_enabled && m_hasGuiControl && m_textureUpdateNeeded)
            {
                DrawFilledCircle(m_colorData, m_width, m_width / 2, m_width / 2, 50, Color.White, Color.Transparent, true);
                DrawFilledCircle(m_colorData, m_width, m_width / 2, m_width / 2, 20, Color.Green, Color.Transparent, false);
                DrawFilledRectangle(m_colorData, m_width, 0, 0, 100, 200, Color.Plum, Color.Transparent, false);
                m_texture.SetData<Color>(m_colorData);
                m_textureUpdateNeeded = false;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_texture, m_rectangle, Color.White);
            m_spriteBatch.End();
        }
            
        public override void HandleInput(InputState inputState)
        {
            GamePadState gamePadState = inputState.CurrentControllingPadState();

            m_selectorDirection = gamePadState.ThumbSticks.Left;
            m_selectorDirection.Normalize();
            
            bool spell1Selected =inputState.IsNewButtonPress(Buttons.A);
            bool spell2Selected =inputState.IsNewButtonPress(Buttons.B);

            if (spell1Selected)
            {
                m_selectedSpell1 = GetSelectedSpell();
            }

            if (spell2Selected)
            {
                m_selectedSpell2 = GetSelectedSpell();
            }


            if (spell1Selected || spell2Selected)
            {
                // close selector
            }


        }


        public SpellType GetSelectedSpell()
        {
            float selectorAngle = (float)Math.Atan2(m_selectorDirection.Y, m_selectorDirection.X);
            // simple 4 way selector for now
            int numSegments = m_spellTypes.Length;
            float segmentSpan = MathUtil.SIMD_2_PI / numSegments;
            float halfSegmentSpan = segmentSpan / 2f;

            // rotate selector and adjust
            selectorAngle -= halfSegmentSpan;
            if(selectorAngle < 0)
            {
                selectorAngle += MathUtil.SIMD_2_PI;
            }

            float lastAngle = 0;
            int index = 0;
            for(int i=0;i<numSegments;++i)
            {
                float newAngle = segmentSpan * i;
                if(selectorAngle >= lastAngle && selectorAngle < newAngle)
                {
                    index = i;
                    break;
                }
                lastAngle = newAngle;
            }

            return m_spellTypes[index];
        }



        public SpellType SelectedSpell1
        {
            get { return m_selectedSpell1; }
        }

        public SpellType SelectedSpell2
        {
            get { return m_selectedSpell2; }
        }


        private Magician m_magician;
        private SpellType[] m_spellTypes = new SpellType[] { SpellType.Castle, SpellType.Convert, SpellType.Fireball, SpellType.Lower, SpellType.Raise };
        private SpellType m_selectedSpell1;
        private SpellType m_selectedSpell2;

        private bool m_textureUpdateNeeded;

        private Vector2 m_selectorDirection;
    }
}
