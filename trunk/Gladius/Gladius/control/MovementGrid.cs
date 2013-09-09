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
using System.Diagnostics;

namespace Gladius.control
{
    public class MovementGrid 
    {
        public MovementGrid(TurnManager turnManager,Arena arena)
        {
            TurnManager = turnManager;
            m_arena = arena;
        }

        public void Initialize()
        {
            //SetupEvents();
        }

        public virtual void Cleanup()
        {
            TeardownEvents();
        }


        public void LoadContent(ContentManager content,GraphicsDevice device)
        {
            m_simpleCursor = content.Load<Texture2D>("UI/SimpleCursor");
            m_selectCursor = content.Load<Texture2D>("UI/SelectCursor");
            m_attackCursor = content.Load<Texture2D>("UI/AttackCursor");
            m_blockedCursor = content.Load<Texture2D>("UI/BlockedCursor");
            m_forwardMoveCursor = content.Load<Texture2D>("UI/ForwardMoveCursor");
            m_turnMoveCursor = content.Load<Texture2D>("UI/TurnMoveCursor");


            m_simpleQuad = new SimpleQuad(device);
        }

        public void Draw(GameTime gameTime,GraphicsDevice device)
        {
            Draw(device, Globals.Camera);
        }

        public void Draw(GraphicsDevice device, ICamera camera)
        {
            device.BlendState = BlendState.AlphaBlend;
            //device.DepthStencilState = DepthStencilState.None;

            switch (GridMode)
            {
                case GridMode.Select:
                    DrawIfValid(device, camera, CurrentPosition, SelectedActor);
                    break;
                case GridMode.Move:
                    if (SelectedActor != null)
                    {
                        DrawIfValid(device, camera, SelectedActor.CurrentPosition, SelectedActor, m_selectCursor);
                        foreach (Point p in SelectedActor.WayPointList)
                        {
                            DrawIfValid(device, camera, p, SelectedActor);
                        }
                        DrawIfValid(device, camera, CurrentPosition, SelectedActor);
                    }
                    break;
                default:
                    // calculate type / size of grid to display based on player,skill, etc
                    int width = ((CurrentCursorSize - 1) / 2);

                    for (int i = -width; i <= width; ++i)
                    {
                        for (int j = -width; j <= width; ++j)
                        {
                            Point p = new Point(CurrentPosition.X + i, CurrentPosition.Y + j);
                            DrawIfValid(device, camera, p, SelectedActor);
                        }
                    }
                    break;
            }

        }

        public void DrawMovementCross(GraphicsDevice device, ICamera camera,BaseActor actor)
        {
            //Vector3 topLeft = new Vector3(CurrentCursorSize - 1, 0, CurrentCursorSize - 1);
            //topLeft /= -2f;
            DrawIfValid(device, camera, new Point(CurrentPosition.X - 1, CurrentPosition.Y),actor);
            DrawIfValid(device, camera, new Point(CurrentPosition.X + 1, CurrentPosition.Y), actor);
            DrawIfValid(device, camera, new Point(CurrentPosition.X, CurrentPosition.Y - 1), actor);
            DrawIfValid(device, camera, new Point(CurrentPosition.X, CurrentPosition.Y + 1), actor);
        }

        public void DrawIfValid(GraphicsDevice device, ICamera camera, Point p,BaseActor actor,Texture2D cursor=null)
        {
            if (cursor == null)
            {
                cursor = CursorForSquare(p,actor);
            }
            if (cursor != null)
            {
                Vector3 v3 = V3ForSquare(p);
                m_simpleQuad.Draw(device, cursor, v3, Vector3.Up, Vector3.One, camera);
            }
        }

        public Texture2D CursorForSquare(Point p,BaseActor actor)
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
                            BaseActor target = m_arena.GetActorAtPosition(p);
                            if (Globals.CombatEngine.IsValidTarget(SelectedActor, target))
                            {
                                return m_attackCursor;
                            }
                            else
                            {
                                return m_blockedCursor;
                            }
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

        public void DrawMovementPath(GraphicsDevice device, ICamera camera,BaseActor actor,List<Point> points)
        {
            foreach (Point p in points)
            {
                DrawIfValid(device, camera, p,actor);
            }
        }


        public void Update(GameTime gameTime)
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

                    if (SelectedActor != null)
                    {
                        GridMode = GridMode.Move;
                        SquareType st = m_arena.GetSquareTypeAtLocation(CurrentPosition);
                        if (st == SquareType.Empty)
                        {
                            // try and find a path.
                            SelectedActor.WayPointList.Clear();
                            if (m_arena.FindPath(SelectedActor.CurrentPosition, CurrentPosition, SelectedActor.WayPointList))
                            {

                            }
                        }
                        else if(st == SquareType.Mobile)
                        {
                            // still need to pathfind to actor?
                            SelectedActor.WayPointList.Clear();
                            BaseActor target = m_arena.GetActorAtPosition(CurrentPosition);
                            Point adjustedPoint = FindClearPointNearTarget(SelectedActor, target);

                            if (m_arena.FindPath(SelectedActor.CurrentPosition, adjustedPoint, SelectedActor.WayPointList))
                            {
                                if (Globals.CombatEngine.IsValidTarget(SelectedActor, target))
                                {
                                    SelectedActor.SetTarget(target);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ActionComplete()
        {
            TurnManager.WaitingOnPlayerControl = false;
        }

        public TurnManager TurnManager
        {
            get;
            set;
        }

        public Point FindClearPointNearTarget(BaseActor from, BaseActor to)
        {
            Debug.Assert(from != null && to != null);
            Point fp = from.CurrentPosition;
            Point tp = to.CurrentPosition;
            Vector3 diff = new Vector3(tp.X,0,tp.Y) - new Vector3(fp.X,0,fp.Y);
            
            // find the ordinate square thats closest.
            if(Math.Abs(diff.X) > Math.Abs(diff.Z))
            {
                diff.Z = 0;
            }
            else
            {
                diff.X = 0;
            }
            diff.Normalize();
            // come one square closer.
            return new Point((int)(tp.X - diff.X), (int)(tp.Y - diff.Z));

        }

        public bool CursorOnTarget(BaseActor source)
        {
            BaseActor ba = m_arena.GetActorAtPosition(CurrentPosition);
            return (ba != null && Globals.CombatEngine.IsValidTarget(source,ba));
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
                switch (GridMode)
                {
                    case (GridMode.Select):
                        {
                            BaseActor ba = m_arena.GetActorAtPosition(CurrentPosition);
                            EventManager.ChangeActor(this, SelectedActor, ba);
                            ba.UnitActive = true;
                            break;
                        }
                    case (GridMode.Move):
                        {
                            if (SelectedActor != null)
                            {
                                SelectedActor.ConfirmMove();
                                if (CursorOnTarget(SelectedActor))
                                {
                                    SelectedActor.AttackRequested = true;
                                }
                            }
                            EventManager.ChangeActor(this, SelectedActor, null);
                            break;
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
        Inactive,
        Select,
        Move
    }

}
