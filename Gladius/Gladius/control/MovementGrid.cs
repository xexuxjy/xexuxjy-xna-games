using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Gladius.renderer;
using Gladius.actors;
using Gladius.events;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Gladius.gamestatemanagement.screens;
using Gladius.combat;
using Gladius.modes.arena;
using Gladius.util;

namespace Gladius.control
{
    public class MovementGrid : GameScreenComponent
    {
        public MovementGrid(Arena arena,ArenaScreen arenaScreen) : base(arenaScreen)
        {
            m_arena = arena;
            m_arenaScreen = arenaScreen;
            DrawOrder = Globals.MoveGridDrawOrder;
        }

        public override void LoadContent()
        {
            m_defaultTile = ContentManager.Load<Texture2D>("UI/cursors/DefaultCursor");
            m_selectCursor = ContentManager.Load<Texture2D>("UI/cursors/SelectCursor");
            m_targetCursor = ContentManager.Load<Texture2D>("UI/cursors/TargetCursor");
            m_targetAndSelectCursor = ContentManager.Load<Texture2D>("UI/cursors/TargetSelectCursor");

            m_startMoveCursor = ContentManager.Load<Texture2D>("UI/cursors/StartMove");
            m_interMoveCursor = ContentManager.Load<Texture2D>("UI/cursors/InterMove");
            m_turnMoveCursor = ContentManager.Load<Texture2D>("UI/cursors/CornerTurn");
            m_endMoveCursor = ContentManager.Load<Texture2D>("UI/cursors/EndMove");

            m_simpleQuad = new SimpleQuad(ContentManager);

            // always want to know about actor changes. unlike actionevents.
            EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);
        }

        public ArenaScreen ArenaScreen
        {
            get { return m_gameScreen as ArenaScreen; }
        }

        public override void Draw(GameTime gameTime)
        {
            ICamera camera = Globals.Camera;
            //return;
            if (Visible && CurrentActor != null)
            {
                //device.BlendState = BlendState.AlphaBlend;
                //device.DepthStencilState = DepthStencilState.None;

                //DrawCenteredGrid(SelectedActor,SelectedActor.CurrentMovePoints,Game.GraphicsDevice,camera);

                // draw at current actor.
                DrawIfValid(camera, CurrentActor.CurrentPosition, CurrentActor, m_selectCursor);

                if (m_arenaScreen.TurnManager.CurrentControlState == Gladius.control.TurnManager.ControlState.UsingGrid)
                {

                    if (CurrentActor.CurrentAttackSkill != null)
                    {
                        if (CurrentActor.CurrentAttackSkill.HasMovementPath())
                        {
                            DrawMovementPath(camera, CurrentActor, CurrentActor.WayPointList);
                        }


                        // if the current position is on a valid target(which wouldn't be in the movement list
                        // then draw a target/select icon.
                        if (CursorOnTarget(CurrentActor))
                        {

                            DrawIfValid(camera, CurrentPosition, CurrentActor);
                        }
                    }

                    // draw target markers under all players of different team.
                    foreach (BaseActor actor in m_arenaScreen.TurnManager.AllActors)
                    {
                        if (actor.Team != CurrentActor.Team)
                        {
                            DrawIfValid(camera, actor.CurrentPosition, actor, m_targetCursor);
                        }
                    }
                }
            }
            //restore the state
            Game.GraphicsDevice.BlendState = BlendState.Opaque;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

        }

        public void DrawAttackSkillCursor(BaseActor actor, Point centerPoint, ICamera camera)
        {
            int distance = Globals.PathDistance(actor.CurrentPosition, centerPoint);
            if (distance >= actor.CurrentAttackSkill.MinRange && distance <= actor.CurrentAttackSkill.MaxRange)
            {
                DrawCenteredGrid(actor, actor.CurrentAttackSkill.Radius, camera);
            }
        }


        public void DrawCenteredGrid(BaseActor actor, int size, ICamera camera)
        {
            int width = size;//((size - 1) / 2);

            for (int i = -width; i <= width; ++i)
            {
                for (int j = -width; j <= width; ++j)
                {
                    Point p = new Point(actor.CurrentPosition.X + i, actor.CurrentPosition.Y + j);
                    DrawIfValid(camera, p, actor, m_defaultTile);
                }
            }
        }


        //public bool DrawingMovePath
        //{
        //    get
        //    {
        //        return (Visible && CurrentActor != null && CurrentActor.CurrentAttackSkill != null && CurrentActor.CurrentAttackSkill.MaxRange >1);
        //    }
        //}

        public void DrawMovementCross(ICamera camera, BaseActor actor)
        {
            //Vector3 topLeft = new Vector3(CurrentCursorSize - 1, 0, CurrentCursorSize - 1);
            //topLeft /= -2f;
            DrawIfValid(camera, new Point(CurrentPosition.X - 1, CurrentPosition.Y), actor);
            DrawIfValid(camera, new Point(CurrentPosition.X + 1, CurrentPosition.Y), actor);
            DrawIfValid(camera, new Point(CurrentPosition.X, CurrentPosition.Y - 1), actor);
            DrawIfValid(camera, new Point(CurrentPosition.X, CurrentPosition.Y + 1), actor);
        }

        public void DrawIfValid(ICamera camera, Point p, BaseActor actor, Texture2D cursor = null)
        {
            if (cursor == null)
            {
                cursor = CursorForSquare(p, actor);
            }
            if (cursor != null)
            {
                Vector3 v3 = V3ForSquare(p);
                Matrix m = Matrix.CreateTranslation(v3);

                float alpha = (cursor == m_defaultTile) ? 0.2f : 1.0f;
                m_simpleQuad.Draw(Game.GraphicsDevice, cursor, m, Vector3.Up, Vector3.One, camera,actor.TeamColour,alpha);
            }
        }




        public void DrawIfValid(ICamera camera, Point prevPoint, Point point, Point nextPoint, BaseActor actor, bool prevExists,bool nextExists, Texture2D cursor = null)
        {
            if (cursor == null)
            {
                cursor = CursorForSquare(point, actor);
            }
            if (cursor != null)
            {
                Vector3 v3 = V3ForSquare(point);
                Vector3 v3p = V3ForSquare(prevPoint);
                Vector3 v3n = V3ForSquare(nextPoint);

                Matrix rot = Matrix.Identity;



                float rotation = 0f;

                Vector3 diffPrevious = v3 - v3p;
                diffPrevious.Y = 0;
                Vector3 diffNext = v3n - v3;
                diffNext.Y = 0;


                if (prevExists)
                {

                    Side enterSide = Side.Left;
                    Side exitSide = Side.Right;
                    if (diffPrevious.X == 1)
                    {
                        enterSide = Side.Left;
                    }
                    else if (diffPrevious.X == -1)
                    {
                        enterSide = Side.Right;
                    }
                    else if (diffPrevious.Z == 1)
                    {
                        enterSide = Side.Bottom;
                    }
                    else
                    {
                        enterSide = Side.Top;
                    }

                    if (diffNext.X == 1)
                    {
                        exitSide = Side.Right;
                    }
                    else if (diffNext.X == -1)
                    {
                        exitSide = Side.Left;
                    }
                    else if (diffNext.Z == 1)
                    {
                        exitSide = Side.Top;
                    }
                    else
                    {
                        exitSide = Side.Bottom;
                    }

                    if (nextExists)
                    {

                        if (CompareSide(enterSide, exitSide, Side.Left, Side.Right))
                        {
                            cursor = m_interMoveCursor;
                            rotation = (float)Math.PI / 2f;
                        }
                        else if (CompareSide(enterSide, exitSide, Side.Top, Side.Bottom))
                        {
                            cursor = m_interMoveCursor;
                        }
                        else if (CompareSide(enterSide, exitSide, Side.Left, Side.Top))
                        {
                            cursor = m_turnMoveCursor;
                            //rotation = (float)Math.PI / 2f;
                            rotation = ((float)(3 * Math.PI) / 2f);
                        }
                        else if (CompareSide(enterSide, exitSide, Side.Left, Side.Bottom))
                        {
                            cursor = m_turnMoveCursor;
                            rotation = ((float)Math.PI);
                        }
                        else if (CompareSide(enterSide, exitSide, Side.Right, Side.Top))
                        {
                            cursor = m_turnMoveCursor;
                            rotation =0;
                        }
                        else if (CompareSide(enterSide, exitSide, Side.Right, Side.Bottom))
                        {
                            cursor = m_turnMoveCursor;
                            rotation = (float)Math.PI/2f;
                        }
                    }
                    else
                    {
                        cursor = m_endMoveCursor;
                        switch(enterSide)
                        {
                            case(Side.Left):
                                rotation = ((float)(3 * Math.PI) / 2f);
                                break;
                            case(Side.Right):
                                rotation = ((float)(Math.PI) / 2f);
                                break;
                            case(Side.Top):
                                rotation = 0f;
                                break;
                            case(Side.Bottom):
                                rotation = ((float)Math.PI);
                                break;
                        }
                    }
                }
                else
                {
                    cursor = m_startMoveCursor;
                }

                Matrix.CreateRotationY(rotation, out rot);
                Matrix m = rot * Matrix.CreateTranslation(v3);

                // not sure why it's not getting this from the texture.
                float alpha = (cursor == m_defaultTile) ? 0.2f : 1.0f;
                m_simpleQuad.Draw(Game.GraphicsDevice, cursor, m, Vector3.Up, Vector3.One, camera,actor.TeamColour,alpha);
            }
        }

        public bool CompareSide(Side side1, Side side2, Side check1, Side check2)
        {
            return (side1 == check1 && side2 == check2) || (side1 == check2 && side2 == check1);
        }


        public Texture2D CursorForSquare(Point p, BaseActor actor)
        {
            if (m_arena.InLevel(p))
            {
                SquareType type = m_arena.GetSquareTypeAtLocation(p);
                switch (type)
                {
                    case (SquareType.Empty):
                        {
                            //if (AttackSkill.IsAttackSkill(actor.CurrentAttackSkill))
                            //{
                            //    return m_blockedCursor;
                            //}
                            return m_interMoveCursor;
                        }
                    case (SquareType.Mobile):
                        {
                            BaseActor target = m_arena.GetActorAtPosition(p);
                            if (m_arenaScreen.CombatEngine.IsValidTarget(CurrentActor, target, CurrentActor.CurrentAttackSkill))
                            {
                                if (m_arenaScreen.CombatEngine.IsAttackerInRange(actor, target,cursorOnly:true))
                                {
                                    return m_targetAndSelectCursor;
                                }
                                else
                                {
                                    return m_targetCursor;
                                }
                            }
                            else
                            {
                                return m_selectCursor;
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

        public void DrawMovementPath(ICamera camera, BaseActor actor, List<Point> points)
        {
            int numPoints = points.Count;
            Point prev = new Point();
            Point curr = new Point();
            Point next = new Point();
            for (int i = 0; i < numPoints; ++i)
            {
                prev = curr;
                curr = points[i];
                if (i < (numPoints - 1))
                {
                    next = points[i + 1];
                    DrawIfValid(camera, prev, curr, next, actor,(i>0),true);
                }
                else
                {
                    DrawIfValid(camera, prev,curr,next, actor,true,false,m_endMoveCursor);
                }
            }
        }

        public Point FindClearPointNearTarget(BaseActor from, BaseActor to)
        {
            Debug.Assert(from != null && to != null);
            Point fp = from.CurrentPosition;
            Point tp = to.CurrentPosition;
            Vector3 diff = new Vector3(tp.X, 0, tp.Y) - new Vector3(fp.X, 0, fp.Y);

            float len2 = diff.LengthSquared();
            // we're next to the target already.
            if( len2 <= 1f)
            {
                return fp;
            }

            // find the ordinate square thats closest.
            if (Math.Abs(diff.X) > Math.Abs(diff.Z))
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
            return (ba != null && m_arenaScreen.CombatEngine.IsValidTarget(source, ba, source.CurrentAttackSkill));
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

        public BaseActor CurrentActor
        {
            get;
            set;
        }


        //public override void RegisterListeners()
        //{
        //    if (RegisterCount == 0)
        //    {
        //        EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
        //        RegisterCount++;
        //    }
        //    Debug.Assert(RegisterCount == 1);
        //}




        void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
        {
            CurrentActor = e.New;
            CurrentPosition = CurrentActor.CurrentPosition;
        }




        //public override void UnregisterListeners()
        //{
        //    if (RegisterCount == 1)
        //    {
        //        EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);
        //        RegisterCount--;

        //    }
        //    Debug.Assert(RegisterCount == 0);
        //}

        public Vector3 m_cursorMovement = Vector3.Zero;
        public int CurrentCursorSize = 5;
        public const float m_hover = 0.01f;
        public Arena m_arena;
        public ArenaScreen m_arenaScreen;
        public Point m_currentPosition;
        public SimpleQuad m_simpleQuad;


        public Texture2D m_defaultTile;
        public Texture2D m_selectCursor;
        public Texture2D m_targetCursor;
        public Texture2D m_targetAndSelectCursor;

        public Texture2D m_startMoveCursor;
        public Texture2D m_interMoveCursor;
        public Texture2D m_turnMoveCursor;
        public Texture2D m_endMoveCursor;
        int RegisterCount = 0;
    }

    public enum Side
    {
        Left,
        Right,
        Top,
        Bottom
    }


    //public enum GridMode
    //{
    //    Inactive,
    //    Select,
    //    Move
    //}

}
