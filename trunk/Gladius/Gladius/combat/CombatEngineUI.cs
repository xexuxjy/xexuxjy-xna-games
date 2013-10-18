using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.gamestatemanagement.screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StringLeakTest;
using Gladius.actors;

namespace Gladius.combat
{
    public class CombatEngineUI : BaseUIElement
    {
        public void DrawFloatingText(Vector3 initialPosition, Color textColor, StringBuilder text, float age)
        {
            FloatingText ft = GetFloatingText();
            ft.Initialise(initialPosition, text, textColor, age);
            m_activeFloatingText.Add(ft);
        }

        public override void Update(GameTime gameTime)
        {
            Matrix cameraView = Matrix.Invert(Globals.Camera.View);
            //Matrix cameraView = Globals.Camera.View;
            int active = m_activeFloatingText.Count - 1;
            for (int i = active; i >= 0; )
            {
                FloatingText ft = m_activeFloatingText[i];
                ft.Update(gameTime);
                if (!ft.Complete)
                {
                    ft.CameraPosition = Vector3.TransformNormal(ft.WorldPosition, cameraView);
                    i--;
                }
                else
                {
                    FreeFloatingText(m_activeFloatingText[i]);
                    m_activeFloatingText.RemoveAt(i);
                    // don't decrement.
                }
            }

            // order them by camera distance?
            m_activeFloatingText.Sort();

        }

        public override void DrawElement(GameTime gameTime, GraphicsDevice graphicsDevice,Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            Vector2 viewPortAdjust = new Vector2(graphicsDevice.Viewport.X,graphicsDevice.Viewport.Y);

            foreach (FloatingText ft in m_activeFloatingText)
            {
                Vector3 result = graphicsDevice.Viewport.Project(ft.WorldPosition,Globals.Camera.Projection,Globals.Camera.View,Matrix.Identity);
                
                // swap up/down
                //result.Y = graphicsDevice.Viewport.Height - result.Y;
                // measure the text so it's centered - can only do at this point?
                Vector2 textDims = m_spriteFont.MeasureString(ft.StringData);
                if (result.Z > 1)
                {
                    result = -result;
                }

                Vector2 pos = new Vector2(result.X, result.Y);

                pos -= viewPortAdjust;

                pos.X -= (textDims.X / 2f);
                spriteBatch.DrawString(m_spriteFont, ft.StringData, pos, ft.TextColor);
            }
        }

        private FloatingText GetFloatingText()
        {
            if (m_floatingTextPool.Count == 0)
            {
                m_floatingTextPool.Push(new FloatingText());
            }
            return m_floatingTextPool.Pop();
        }

        private void FreeFloatingText(FloatingText ft)
        {
            m_floatingTextPool.Push(ft);
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager manager, GraphicsDevice device)
        {
            base.LoadContent(manager, device);
            m_spriteFont = manager.Load<SpriteFont>("UI/fonts/ShopFont");
        }

        public void DrawNameAndHealth(BaseActor actor, GraphicsDevice device, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            Vector3 worldPoint = actor.Position;
            worldPoint.Y += (actor.ModelHeight*2);
            Vector3 result = device.Viewport.Project(worldPoint, Globals.Camera.Projection, Globals.Camera.View, Matrix.Identity);

            // swap up/down
            //result.Y = graphicsDevice.Viewport.Height - result.Y;
            // measure the text so it's centered - can only do at this point?
            Vector2 textDims = m_spriteFont.MeasureString(actor.Name);
            if (result.Z > 1)
            {
                result = -result;
            }

            Vector2 pos = new Vector2(result.X, result.Y);
            pos.X -= (textDims.X / 2f);
            // Shadow text.
            spriteBatch.DrawString(m_spriteFont, actor.Name, pos, Color.Black);
            spriteBatch.DrawString(m_spriteFont, actor.Name, pos + new Vector2(1), Color.White);
            pos.Y += textDims.Y + 2;
            int barHeight = 16;
            DrawHealthBar(spriteBatch, actor.Health, actor.MaxHealth, Color.Green, Color.Red, pos,(int)textDims.X,barHeight);

            //Rectangle healthBarDims = new Rectangle((int)pos.X,(int)pos.Y, 50, 10);
            //spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(Color.White), healthBarDims, Color.White);

        }

        private void DrawHealthBar(SpriteBatch spriteBatch,float value, float maxValue, Color colour1, Color colour2,Vector2 topLeft,int width,int height)
        {
            Color borderColour = Color.Black;
            int inset = 2;
            Rectangle rect = new Rectangle((int)topLeft.X, (int)topLeft.Y, width, height);
            Rectangle insetRect = Globals.InsetRectangle(rect, inset);
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(borderColour), rect,Color.White);
            float scale = maxValue > 0 ? (value / maxValue) : 1;
            
            // draw bad health below 30%
            Color drawColour = scale > 0.3f? colour1:colour2;
            insetRect.Width = (int)((float)insetRect.Width*scale);
            spriteBatch.Draw(Globals.GlobalContentManager.GetColourTexture(drawColour), insetRect,Color.White);

        }






        private List<FloatingText> m_activeFloatingText = new List<FloatingText>();
        private Stack<FloatingText> m_floatingTextPool = new Stack<FloatingText>();
        private SpriteFont m_spriteFont;
    }

    public class FloatingText : IComparable
    {
        public Vector3 WorldPosition
        {
            get;set;
        }

        public Vector3 CameraPosition
        {
            get;
            set;
        }

        public Color TextColor
        {
            get;set;
        }
        public float Age
        {
            get;set;
        }
        public float MaxAge
        {
            get;set;
        }
        public StringBuilder StringData
        {
            get;set;
        }

        public bool Complete
        {
            get { return (Age > MaxAge); }
        }

        public FloatingText()
        {
            StringData = new StringBuilder();
        }
        
        public void Initialise(Vector3 worldPosition, StringBuilder textToCopy, Color color,float maxAge)
        {
            Age = 0f;
            WorldPosition = worldPosition;
            TextColor = color;
            MaxAge = maxAge;
            StringData.Clear();
            StringData.Append(textToCopy);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float update = 1f * elapsed;
            Age += elapsed;

            Vector3 delta = new Vector3(0,update,0);
            WorldPosition += delta;
            // to do? fade text as it reaches end
        }


   
#region IComparable Members

public int  CompareTo(object obj)
{
 	return (int)(this.CameraPosition.Z - ((FloatingText)obj).CameraPosition.Z);
}

#endregion
}

}
