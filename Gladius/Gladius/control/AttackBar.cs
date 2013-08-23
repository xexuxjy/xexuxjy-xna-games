using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gladius.events;
using Gladius.combat;

namespace Gladius.control
{
    public class AttackBar : DrawableGameComponent
    {
        public AttackBar(Game game)
            : base(game)
        {

        }

        public Rectangle Rectangle
        {
            get;
            set;
        }

        public void InitializeCombatBar(int numAttacks,float critPointPercentage,float missPointPercentage,float totalTime)
        {
            m_attackBarParts.Clear();
            m_totalTime = totalTime;
            m_updateTime = 0f;

            float partWidth = Rectangle.Width / numAttacks;
            float partTime = totalTime / numAttacks;
            AttackBarPart part = null;
            AttackBarPart previousPart = null;
            float widthCounter = 0;
            float timeCounter = 0;
            

            for (int i = 0; i < numAttacks; ++i)
            {
                part = new AttackBarPart();
                if (i == 0)
                {
                    CurrentPart = part;
                }
                if (previousPart != null)
                {
                    previousPart.nextPart = part;
                }

                part.startPoint = widthCounter;
                part.critPoint = part.startPoint + (partWidth * critPointPercentage);
                part.critPointPercentage = critPointPercentage;
                part.missPointPercentage = missPointPercentage;
                part.critPointWidth = 3;
                part.endPoint = widthCounter + partWidth;
                part.startTime = timeCounter;
                part.buttonRequired = ActionButton.ActionButton2;
                timeCounter += partTime;
                part.endTime = timeCounter;                
                
                m_attackBarParts.Add(part);

                widthCounter = part.endPoint;
                
            }

            RegisterListeners();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            //m_coverageTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            //Color[] colorData = new Color[1];
            //m_coverageTexture.GetData<Color>(colorData);
            //colorData[0] = new Color(Color.White.ToVector3());
            //colorData[0].A = 50;
            //m_coverageTexture.SetData(colorData);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            m_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            m_spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.White), Rectangle, Color.White);
            
            Rectangle insetRectangle = InsetRectangle(Rectangle,1);
            m_spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.Black), insetRectangle, Color.White);
            insetRectangle = InsetRectangle(insetRectangle, 1);
            
            for (int i = 0; i < m_attackBarParts.Count; ++i)
            {
                m_attackBarParts[i].Draw(m_spriteBatch, insetRectangle);
            }

            float coverage = (float)( m_updateTime / m_totalTime);
            Rectangle coverageRectangle = insetRectangle;
            coverageRectangle.Width = (int)(((float)coverageRectangle.Width) * coverage);

            Color coverageColor = Color.DarkGray;
            //coverageColor.A = 50;
            coverageColor *= 0.35f;
            m_spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.White), coverageRectangle, coverageColor);

            m_spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            if (m_totalTime > 0)
            {
                m_updateTime += gameTime.ElapsedGameTime.TotalSeconds;
                // move onto the next one.
                if (m_updateTime > CurrentPart.endPoint)
                {
                    CurrentPart = CurrentPart.nextPart;
                }



                if (m_updateTime > m_totalTime)
                {
                    m_updateTime -= m_totalTime;
                }

                if (Complete)
                {
                    UnregisterListeners();
                }
            }
        }


        public AttackBarPart CurrentPart
        {
            get
            {
                return m_currentPart;
            }
            set
            {
                m_currentPart = value;
            }
        }

        public bool Complete
        {
            get
            {
                return m_totalTime > 0 && m_updateTime > m_totalTime;
            }
        }

        public bool Started
        {
            get
            {
                return m_updateTime > 0f;
            }
        }

        public void GetCombatResult(List<AttackResultType> results)
        {
            results.Clear();
            for (int i = 0; i < m_attackBarParts.Count; ++i)
            {
                results.Add(m_attackBarParts[i].BarResult);
            }

        }

        public static Rectangle InsetRectangle(Rectangle orig, int inset)
        {
            Rectangle insetRectangle = orig;
            insetRectangle.Width -= (inset * 2);
            insetRectangle.Height -= (inset * 2);
            insetRectangle.X += inset;
            insetRectangle.Y += inset;
            return insetRectangle;
        }

        public void RegisterListeners()
        {
            EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);

        }

        void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
        {
            if (Started && !Complete && CurrentPart != null)
            {
                if (e.ActionButton == CurrentPart.buttonRequired)
                {
                    // pressed the right key. but at what point?
                    float currentAttackPercentage = (float)((m_updateTime - CurrentPart.startTime) / (CurrentPart.endTime));
                    if (currentAttackPercentage < CurrentPart.critPointPercentage)
                    {
                        CurrentPart.BarResult = AttackResultType.Hit;
                    }
                    else if (currentAttackPercentage >= CurrentPart.critPointPercentage && currentAttackPercentage <= CurrentPart.missPointPercentage)
                    {
                        CurrentPart.BarResult = AttackResultType.Critical;
                    }
                    else
                    {
                        CurrentPart.BarResult = AttackResultType.Miss;
                    }
                }
                else
                {
                    // pressed the wrong key.
                    CurrentPart.BarResult = AttackResultType.Miss;
                }
            }

        }


        public void UnregisterListeners()
        {
            EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);
        }


        private AttackBarPart m_currentPart;
        public float m_totalTime;
        private double m_updateTime;
        private List<AttackBarPart> m_attackBarParts = new List<AttackBarPart>();
        private SpriteBatch m_spriteBatch;
        private Texture2D m_coverageTexture;
    }


    public class AttackBarPart
    {
        public float startPoint;
        public float critPoint;
        public float critPointWidth;
        public float endPoint;
        public double startTime;
        public double endTime;
        public float critPointPercentage;
        public float missPointPercentage;
        public ActionButton buttonRequired;
        public AttackBarPart nextPart;
        private AttackResultType barResult = AttackResultType.None;


        public AttackResultType BarResult
        {
            get
            {
                return barResult;
            }
            set
            {
                // only allowed to set it once...
                if (barResult == AttackResultType.None)
                {
                    barResult = value;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch,Rectangle baseRectangle)
        {
            int height = baseRectangle.Height;
            int ypos = baseRectangle.Y;
            int start = baseRectangle.X + (int)startPoint;

            int width = (int)(critPoint - startPoint);
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.Yellow), new Rectangle(start, ypos, width, height), Color.White);
            start += width;
            width = (int)critPointWidth;
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.Red), new Rectangle(start, ypos, width, height), Color.White);
            start += width;
            width = (int)(endPoint-start);
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.Blue), new Rectangle(start, ypos, width, height), Color.White);


        }
    }

    
}
