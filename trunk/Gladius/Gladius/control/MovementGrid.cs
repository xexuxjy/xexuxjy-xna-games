using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Gladius.renderer;
using Dhpoware;
using Gladius.actors;

namespace Gladius.control
{
    public class MovementGrid : GameComponent
    {
        public MovementGrid(Game game,Arena arena)
            : base(game)
        {
            m_arena = arena;
        }
        
        
        public void LoadContent()
        {
            m_simpleCursor = Game.Content.Load<Texture2D>("UI/SimpleCursor");
            m_selectCursor = Game.Content.Load<Texture2D>("UI/SelectCursor");
            m_attackCursor = Game.Content.Load<Texture2D>("UI/AttackCursor");
            m_blockedCursor = Game.Content.Load<Texture2D>("UI/BlockedCursor");
            m_forwardMoveCursor = Game.Content.Load<Texture2D>("UI/ForwardMoveCursor");
            m_turnMoveCursor = Game.Content.Load<Texture2D>("UI/TurnMoveCursor");


            m_simpleQuad = new SimpleQuad(Game.GraphicsDevice);
        }

        public void Draw(GraphicsDevice device,ICamera camera)
        {
            m_simpleQuad.Draw(device, m_simpleCursor, CurrentV3, Vector3.Up, Vector3.One, camera);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateMovement();
        }

        public void UpdateMovement()
        {
            Vector3 v = new Vector3();

            Vector3 fwd = Globals.Camera.ViewDirection;
            Vector3 right = Vector3.Cross(fwd, Vector3.Up);

            Point p = CurrentPosition;
            if (Globals.UserControl.CursorLeftPressed())
            {
                v = -right;
            }
            if (Globals.UserControl.CursorRightPressed())
            {
                v = right;
            }
            if (Globals.UserControl.CursorUpPressed())
            {
                v = fwd;
            }
            if (Globals.UserControl.CursorDownPressed())
            {
                v = -fwd;
            }

            if (v.LengthSquared() > 0)
            {
                v.Y = 0;
                v.Normalize();

                //v = v * vd;
                //v = result;

                if (Math.Abs(v.X) > Math.Abs(v.Z))
                {
                    if (v.X < 0)
                    {
                        p.X--;
                    }
                    if (v.X > 0)
                    {
                        p.X++;
                    }
                }
                else
                {
                    if (v.Z < 0)
                    {
                        p.Y--;
                    }
                    if (v.Z > 0)
                    {
                        p.Y++;
                    }
                }
                if (m_arena.InLevel(p))
                {
                    CurrentPosition = p;
                }
            }
        }

        public Point CurrentPosition
        {
            get
            {
                return m_currentPosition;
            }
            set
            {
                m_currentPosition = value;
            }
        }

        public Vector3 CurrentV3
        {

            get
            {
                float height = m_arena.GetHeightAtLocation(CurrentPosition);
                height += m_hover;
                return new Vector3(CurrentPosition.X, height, CurrentPosition.Y);
            }
        }


        public void BuildForPlayer(BaseActor actor)
        {

            // build a grid of 'x' values centered around player.




        }


        
        public const float m_hover = 0.01f;
        public Arena m_arena;
        public Point m_currentPosition;
        public SimpleQuad m_simpleQuad;
        
        public Texture2D m_simpleCursor;
        public Texture2D m_selectCursor;
        public Texture2D m_attackCursor;
        public Texture2D m_blockedCursor;
        public Texture2D m_forwardMoveCursor;
        public Texture2D m_turnMoveCursor;


    }
}
