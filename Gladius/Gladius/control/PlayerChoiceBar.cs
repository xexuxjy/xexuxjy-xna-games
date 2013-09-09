using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Gladius.events;
using Microsoft.Xna.Framework.Graphics;
using Gladius.gamestatemanagement.screens;
using Microsoft.Xna.Framework.Content;
using Gladius.actors;
using Gladius.combat;

namespace Gladius.control
{
    public class PlayerChoiceBar : IUIElement
    {
        const int numSkillSlots = 4;

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
            m_spriteFont = manager.Load<SpriteFont>("UI/DebugFont8");
            m_attackSkills = new List<List<AttackSkill>>();
            for (int i = 0; i < numSkillSlots; ++i)
            {
                m_attackSkills.Add( new List<AttackSkill>());
            }
            m_currentAttackSkillLine = new List<AttackSkill>(numSkillSlots);
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

            DrawSkillBar2(spriteBatch, barRect, m_currentAttackSkillLine, null,null);
        }

        public void RegisterListeners()
        {
            EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
            EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);
        }




        void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
        {
            CurrentActor = e.New;
        }

        public void BuildDataForActor()
        {
            foreach (List<AttackSkill> list in m_attackSkills)
            {
                list.Clear();
            }
            m_currentAttackSkillLine.Clear();


            foreach (AttackSkill attackSkill in CurrentActor.AttackSkills)
            {
                m_attackSkills[attackSkill.SkillRow].Add(attackSkill);
            }

            for (int i = 0; i < numSkillSlots; ++i)
            {
                m_currentAttackSkillLine.Add(m_attackSkills[i][0]);
            }
        }


        void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
        {
            switch (e.ActionButton)
            {
                case (ActionButton.ActionLeft):
                    {
                        CursorLeft();
                        break;
                    }
                case (ActionButton.ActionRight):
                    {
                        CursorRight();
                        break;
                    }
                case (ActionButton.ActionUp):
                    {
                        CursorUp();
                        break;
                    }
                case (ActionButton.ActionDown):
                    {
                        CursorDown();
                        break;
                    }
            }
        }

        public void CursorLeft()
        {
            m_actionCursor.X--;
            if (m_actionCursor.X < 0)
            {
                m_actionCursor.X += m_attackSkills.Count;
            }
            m_actionCursor.Y = 0;
        }

        public void CursorRight()
        {
            m_actionCursor.X++;
            if (m_actionCursor.X >= m_attackSkills.Count)
            {
                m_actionCursor.X -= m_attackSkills.Count;
            }
            m_actionCursor.Y = 0;
        }

        public void CursorUp()
        {
            m_actionCursor.Y++;
            if (m_actionCursor.Y >= m_attackSkills[m_actionCursor.X].Count)
            {
                m_actionCursor.Y -= m_attackSkills[m_actionCursor.X].Count;
            }
            m_currentAttackSkillLine[m_actionCursor.X] = m_attackSkills[m_actionCursor.X][m_actionCursor.Y];

        }

        public void CursorDown()
        {
            m_actionCursor.Y--;
            if (m_actionCursor.Y < 0 )
            {
                m_actionCursor.Y += m_attackSkills[m_actionCursor.X].Count;
            }
            m_currentAttackSkillLine[m_actionCursor.X] = m_attackSkills[m_actionCursor.X][m_actionCursor.Y];
        }


        public void UnregisterListeners()
        {
            //EventManager.ActionPressed -= new event ActionButtonPressed();

        }

        private void DrawSkillBar1(SpriteBatch spriteBatch, Rectangle rect, String bigIconName, String smallIconName, float bar1Value, float bar1MaxValue, float bar2Value, float bar2MaxValue)
        {
            // ideally read in values from svg. but.

            float bigCircleRadius = 32;
            float bigCircleYOffset = 16;

            float smallCircleRadius = 16;
            float smallircleYOffset = 0;



            Rectangle skillRect1Dims = new Rectangle(33, 15, 107, 16);
            Rectangle skillRect2Dims = new Rectangle(33, 38, 107 , 16);

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

        private void DrawSkillBar2(SpriteBatch spriteBatch, Rectangle rect, List<AttackSkill> skills,StringBuilder bar1Text,StringBuilder bar2Text)
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

            for (int i = 0; i < skills.Count(); ++i)
            {
                Point dims;
                Point uv;

                SkillIconState iconState = (i == m_actionCursor.X) ? SkillIconState.Selected : SkillIconState.Available;

                GetUVForIcon(m_currentAttackSkillLine[i].SkillIcon,iconState,out uv,out dims);
                Rectangle dest = new Rectangle(rect.X + (i * (circleRadius+xpad)), rect.Y + 2, circleRadius, circleRadius);
                Rectangle src = new Rectangle(uv.X,uv.Y,dims.X,dims.Y);

                spriteBatch.Draw(m_skillsBitmap, dest, src, Color.White);
            }
            Vector2 pos = new Vector2(rect.X,rect.Y);
            pos += new Vector2(skillRect1Dims.X,skillRect1Dims.Y);
            spriteBatch.DrawString(m_spriteFont, skills[m_actionCursor.X].Name, pos, Color.Black);

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

        private BaseActor m_currentActor;
        public BaseActor CurrentActor
        {
            get
            {
                return m_currentActor;
            }
            set
            {
                m_currentActor = value;
                BuildDataForActor();
            }
        }


        public enum SkillIconState
        {
            Available,
            Selected,
            Unavailable
        }

        String m_mode;
        String m_affinityIcon;

        List<List<AttackSkill>> m_attackSkills;
        List<AttackSkill> m_currentAttackSkillLine;

        Point m_actionCursor = new Point();        

        
        Texture2D m_skillBar1Bitmap;
        Texture2D m_skillBar2Bitmap;
        Texture2D m_skillsBitmap;

        Point m_topLeft = new Point(20, 400);
        SpriteFont m_spriteFont;

    }
}
