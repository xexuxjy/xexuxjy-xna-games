using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Gladius.renderer;
using Dhpoware;
using Gladius.actors;
using Gladius.events;
using Microsoft.Xna.Framework.Content;

namespace Gladius.control
{
    public class MovementGrid :DrawableGameComponent
    {
        public MovementGrid(Game game, Arena arena)
            : base(game)
        {
            m_arena = arena;
        }

        public override void Initialize()
        {
            base.Initialize();
            SetupEvents();
        }

        public virtual void Cleanup()
        {
            TeardownEvents();
        }


        protected override void LoadContent()
        {
            base.LoadContent();
            m_simpleCursor = Game.Content.Load<Texture2D>("UI/SimpleCursor");
            m_selectCursor = Game.Content.Load<Texture2D>("UI/SelectCursor");
            m_attackCursor = Game.Content.Load<Texture2D>("UI/AttackCursor");
            m_blockedCursor = Game.Content.Load<Texture2D>("UI/BlockedCursor");
            m_forwardMoveCursor = Game.Content.Load<Texture2D>("UI/ForwardMoveCursor");
            m_turnMoveCursor = Game.Content.Load<Texture2D>("UI/TurnMoveCursor");


            m_simpleQuad = new SimpleQuad(Game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            Draw(Game.GraphicsDevice, Globals.Camera);
        }

        public void Draw(GraphicsDevice device, ICamera camera)
        {
            device.BlendState = BlendState.AlphaBlend;
            //device.DepthStencilState = DepthStencilState.None;

            if (GridMode == GridMode.Select)
            {
                DrawIfValid(device, camera, CurrentPosition);
            }
            else if (GridMode == GridMode.Move)
            {
                if (SelectedActor != null)
                {
                    foreach (Point p in SelectedActor.WayPointList)
                    {
                        DrawIfValid(device, camera, p);
                    }

                }
            }
            else
            {
                // calculate type / size of grid to display based on player,skill, etc
                int width = ((CurrentCursorSize - 1) / 2);

                for (int i = -width; i <= width; ++i)
                {
                    for (int j = -width; j <= width; ++j)
                    {
                        Point p = new Point(CurrentPosition.X + i, CurrentPosition.Y + j);
                        DrawIfValid(device, camera, p);
                    }
                }
            }
        }

        public void DrawMovementCross(GraphicsDevice device, ICamera camera)
        {
            //Vector3 topLeft = new Vector3(CurrentCursorSize - 1, 0, CurrentCursorSize - 1);
            //topLeft /= -2f;
            DrawIfValid(device, camera, new Point(CurrentPosition.X - 1, CurrentPosition.Y));
            DrawIfValid(device, camera, new Point(CurrentPosition.X + 1, CurrentPosition.Y));
            DrawIfValid(device, camera, new Point(CurrentPosition.X, CurrentPosition.Y - 1));
            DrawIfValid(device, camera, new Point(CurrentPosition.X, CurrentPosition.Y + 1));
        }

        public void DrawIfValid(GraphicsDevice device, ICamera camera, Point p)
        {
            Texture2D cursor = CursorForSquare(p);
            if (cursor != null)
            {
                Vector3 v3 = V3ForSquare(p);
                m_simpleQuad.Draw(device, cursor, v3, Vector3.Up, Vector3.One, camera);
            }
        }

        public Texture2D CursorForSquare(Point p)
        {
            if (m_arena.InLevel(p))
            {
                SquareType type = m_arena.GetSquareTypeAtLocation(p);
                switch (type)
                {
                    case (SquareType.Empty):
                        {
                            return m_forwardMoveCursor;
                        }
                    case (SquareType.Mobile):
                        {
                            return m_blockedCursor;
                        }
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void DrawMovementPath(GraphicsDevice device, ICamera camera,List<Point> points)
        {
            foreach (Point p in points)
            {
                DrawIfValid(device, camera, p);
            }
        }


        public override void Update(GameTime gameTime)
        {
            UpdateMovement();
            UpdateActions();
        }

        public void UpdateActions()
        {
            if (Globals.UserControl.Action1Pressed())
            {
                if (SelectedActor == null)
                {
                    SelectedActor = m_arena.GetActorAtPosition(CurrentPosition);
                }
            }
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
                return V3ForSquare(CurrentPosition);
            }
        }

        public Vector3 V3ForSquare(Point p)
        {
            Vector3 result = m_arena.ArenaToWorld(p);
            result.Y += m_hover;
            return result;
        }

        public void BuildForPlayer(BaseActor actor)
        {

            // build a grid of 'x' values centered around player.




        }

        public BaseActor SelectedActor
        {
            get;
            set;
        }


        public void SetupEvents()
        {
            EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
            EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);    
        }


        
        
        void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
        {
            SelectedActor = e.New;
            if(e.Original == e.New)
            {
                GridMode = GridMode.Move;
            }
            else
            {
                GridMode = GridMode.Select;
            }
        }

        void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
        {
            if (e.ActionButton == ActionButton.ActionButton1)
            {
                BaseActor ba = m_arena.GetActorAtPosition(CurrentPosition);
                EventManager.ChangeActor(this, SelectedActor, ba);

                if (SelectedActor != null)
                {
                    SquareType st = m_arena.GetSquareTypeAtLocation(CurrentPosition);
                    if (st == SquareType.Empty)
                    {
                        // try and find a path.
                        SelectedActor.WayPointList.Clear();
                        if (m_arena.FindPath(SelectedActor.CurrentPoint, CurrentPosition, SelectedActor.WayPointList))
                        {

                        }
                    }
                }

            }


        }

        public void TeardownEvents()
        {
            //EventManager.ActionPressed -= new event ActionButtonPressed();

        }

        public Vector3 m_cursorMovement = Vector3.Zero;
        public GridMode GridMode = GridMode.Select;
        public int CurrentCursorSize = 5;
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


    public enum GridMode
    {
        Select,
        Move,
        PerformAction
    }

}
