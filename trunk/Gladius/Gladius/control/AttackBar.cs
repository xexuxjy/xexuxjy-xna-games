using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public void InitializeCombatBar(int numAttacks,float critPoint,float missPoint,float totalTime)
        {
            m_attackBarParts.Clear();
            float partWidth= Rectangle.Width / numAttacks;
            AttackBarPart part = new AttackBarPart();

            float counter = 0;
            for (int i = 0; i < numAttacks; ++i)
            {
                part.startPoint = counter;
                part.critPoint = part.startPoint + (partWidth * critPoint);
                part.critPointWidth = 3;
                part.endPoint = counter + partWidth;
                m_attackBarParts.Add(part);
                counter = part.endPoint;
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.White), Rectangle, Color.White);

            Rectangle copyRectangle = Rectangle;
            
            for (int i = 0; i < m_attackBarParts.Count; ++i)
            {
                m_attackBarParts[i].Draw(m_spriteBatch, Rectangle);
            }
            m_spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {

        }



        private List<AttackBarPart> m_attackBarParts = new List<AttackBarPart>();
        private SpriteBatch m_spriteBatch;
    }


    public class AttackBarPart
    {
        public float startPoint;
        public float critPoint;
        public float critPointWidth;
        public float endPoint;
//        public float totalLength;
        public ActionButton buttonRequired;

        public void Draw(SpriteBatch spriteBatch,Rectangle baseRectangle)
        {
            int yInset = 1;
            int height = baseRectangle.Height-(yInset*2);
            int ypos = baseRectangle.Y + yInset;
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
