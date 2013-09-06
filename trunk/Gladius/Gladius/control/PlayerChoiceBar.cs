using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Gladius.events;
using Microsoft.Xna.Framework.Graphics;
using Gladius.gamestatemanagement.screens;
using Microsoft.Xna.Framework.Content;

namespace Gladius.control
{
    public class PlayerChoiceBar : IUIElement
    {
        public Rectangle Rectangle
        {
            get;
            set;
        }

        public void LoadContent(ContentManager manager)
        {
            m_skillBar1Bitmap = manager.Load<Texture2D>("UI/SkillbarPart1");
            m_skillBar2Bitmap = manager.Load<Texture2D>("UI/SkillbarPart2");
            m_skillsBitmap= manager.Load<Texture2D>("UI/SkillIcons");
        }

        public void Update(GameTime gameTime)
        {

        }

        public void DrawElement(GameTime gameTime,SpriteBatch spriteBatch)
        {
            Rectangle barRect = new Rectangle(Rectangle.X,Rectangle.Y,m_skillBar1Bitmap.Width,m_skillBar1Bitmap.Height);
            DrawSkillBar1(spriteBatch, barRect, "Foo", "Bar", 30, 100, 70, 100);
            barRect.X += m_skillBar1Bitmap.Width;
            DrawSkillBar1(spriteBatch, barRect, "Foo", "Bar", 50, 100, 20, 100);

            barRect.X += m_skillBar1Bitmap.Width;
            barRect.X += 20;
            SkillIcon[] skills = new SkillIcon[]{SkillIcon.Attack,SkillIcon.Defend,SkillIcon.Move,SkillIcon.Special};
            SkillIconState[] skillStates = new SkillIconState[]{SkillIconState.Available,SkillIconState.Selected,SkillIconState.Available,SkillIconState.Unavailable};

            DrawSkillBar2(spriteBatch, barRect, skills, skillStates, 10, 100, 20, 100);
        }

        public void RegisterListeners()
        {
            EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
            EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);
        }




        void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
        {
        }

        void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
        {
        }

        public void UnregisterListeners()
        {
            //EventManager.ActionPressed -= new event ActionButtonPressed();

        }
        const int numSkillSLots = 4;
        String[] m_skillSlots = new String[numSkillSLots];

        private void DrawSkillBar1(SpriteBatch spriteBatch, Rectangle rect, String bigIconName, String smallIconName, float bar1Value, float bar1MaxValue, float bar2Value, float bar2MaxValue)
        {
            // ideally read in values from svg. but.

            float bigCircleRadius = 32;
            float bigCircleYOffset = 16;

            float smallCircleRadius = 16;
            float smallircleYOffset = 0;



            Rectangle skillRect1Dims = new Rectangle(31, 21, 108, 17);
            Rectangle skillRect2Dims = new Rectangle(50, 2, 108 , 17);

            Rectangle rect1 = new Rectangle(rect.X + skillRect1Dims.X,rect.Y + skillRect1Dims.Y + skillRect1Dims.Height,skillRect1Dims.Width,skillRect1Dims.Height);
            Rectangle rect2 = new Rectangle(rect.X + skillRect2Dims.X, rect.Y + skillRect2Dims.Y + skillRect2Dims.Height, skillRect2Dims.Width, skillRect2Dims.Height);

            spriteBatch.Draw(m_skillBar1Bitmap, rect, Color.White);

            DrawMiniBar(spriteBatch,rect1, bar1Value, bar1MaxValue, Color.Green, Color.Black);
            DrawMiniBar(spriteBatch,rect2, bar2Value, bar2MaxValue, Color.Yellow, Color.Black);

        }


        private void DrawMiniBar(SpriteBatch spriteBatch,Rectangle baseRectangle, float val, float maxVal, Color color1, Color color2)
        {
            float fillPercentage = val / maxVal;

            int height = baseRectangle.Height;
            int ypos = baseRectangle.Y;
            int start = baseRectangle.X;

            int width = (int)(fillPercentage * baseRectangle.Width);
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(color1), new Rectangle(start, ypos, width, height), Color.White);
            start += width;
            width = baseRectangle.Width - width;
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(color2), new Rectangle(start, ypos, width, height), Color.White);
        }

        private void DrawSkillBar2(SpriteBatch spriteBatch, Rectangle rect, SkillIcon[] skillIcons, SkillIconState[] skillIconStates, 
            float bar1Value, float bar1MaxValue, float bar2Value, float bar2MaxValue)
        {
            rect.Width = m_skillBar2Bitmap.Width;
            rect.Height = m_skillBar2Bitmap.Height;
            spriteBatch.Draw(m_skillBar2Bitmap, rect, Color.White);

            int circleRadius = 28;
            Rectangle skillRect1Dims = new Rectangle(161, 32, 115, 16);
            Rectangle skillRect2Dims = new Rectangle(26, 17, 250, 16);

            int xpad = 2;
            rect.X += xpad;
            rect.Y -= 1;

            for (int i = 0; i < skillIcons.Length; ++i)
            {
                Point dims;
                Point uv;
                GetUVForIcon(skillIcons[i],skillIconStates[i],out uv,out dims);
                Rectangle dest = new Rectangle(rect.X + (i * (circleRadius+xpad)), rect.Y + 2, circleRadius, circleRadius);
                Rectangle src = new Rectangle(uv.X,uv.Y,dims.X,dims.Y);

                spriteBatch.Draw(m_skillsBitmap, dest, src, Color.White);
            }

        }

        public void GetUVForIcon(SkillIcon icon, SkillIconState state, out Point uv, out Point dims)
        {
            int width = 8;
            int height  = 3;
            dims = new Point(32,32);

            uv = new Point(((int)icon) * dims.X,0);
            if(state == SkillIconState.Selected)
            {
                uv.Y = dims.Y;
            }
            if(state == SkillIconState.Unavailable)
            {
                uv.Y = dims.Y*2;
            }

        }

        public enum SkillIconState
        {
            Available,
            Selected,
            Unavailable
        }


        public enum SkillIcon
        {
            Special=0,
            Attack=1,
            Defend=2,
            Move=3
        }


        String m_mode;
        String m_affinityIcon;
        String m_health;
        String m_affinity;
        String m_skillPoints;
        String m_affinityPoints;
        Texture2D m_skillBar1Bitmap;
        Texture2D m_skillBar2Bitmap;
        Texture2D m_skillsBitmap;

        Point m_topLeft = new Point(20, 400);


    }
}
