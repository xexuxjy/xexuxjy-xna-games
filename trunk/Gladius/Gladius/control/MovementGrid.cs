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

namespace Gladius.control
{
    public class MovementGrid : BaseUIElement
    {
        public MovementGrid(Arena arena)
        {
            m_arena = arena;
        }

        public override void LoadContent(ContentManager content, GraphicsDevice device)
        {
            m_simpleCursor = content.Load<Texture2D>("UI/SimpleCursor");
            m_selectCursor = content.Load<Texture2D>("UI/SelectCursor");
            m_attackCursor = content.Load<Texture2D>("UI/AttackCursor");
            m_blockedCursor = content.Load<Texture2D>("UI/BlockedCursor");
            m_forwardMoveCursor = content.Load<Texture2D>("UI/ForwardMoveCursor");
            m_turnMoveCursor = content.Load<Texture2D>("UI/TurnMoveCursor");
            m_destinationCursor = content.Load<Texture2D>("UI/BlockedCursor");

            m_simpleQuad = new SimpleQuad(device);

            // always want to know about actor changes. unlike actionevents.
            EventManager.BaseActorChanged += new EventManager.BaseActorSelectionChanged(EventManager_BaseActorChanged);
        }


        public override void DrawElement(GameTime gameTime, GraphicsDevice device, ICamera camera)
        {
            if (Visible && SelectedActor != null)
            {
                device.BlendState = BlendState.AlphaBlend;
                //device.DepthStencilState = DepthStencilState.None;


                if (SelectedActor.CurrentAttackSkill == null)
                {
                    // draw normal selection cursor
                    DrawIfValid(device, camera, SelectedActor.CurrentPosition, SelectedActor, m_selectCursor);
                }
                else
                {
                    switch (SelectedActor.CurrentAttackSkill.AttackType)
                    {
                        case AttackType.Move:
                            DrawIfValid(device, camera, SelectedActor.CurrentPosition, SelectedActor, m_selectCursor);
                            DrawMovementPath(device, camera, SelectedActor, SelectedActor.WayPointList);
                            //DrawIfValid(device, camera, CurrentPosition, SelectedActor);
                            break;
                        case (AttackType.AOE):
                            break;
                        case (AttackType.Single):
                            DrawIfValid(device, camera, CurrentPosition, SelectedActor);
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
            }
        }

        public bool DrawingMovePath
        {
            get
            {
                return (Visible && SelectedActor != null && SelectedActor.CurrentAttackSkill != null && SelectedActor.CurrentAttackSkill.AttackType == AttackType.Move);
            }
        }

        public void DrawMovementCross(GraphicsDevice device, ICamera camera, BaseActor actor)
        {
            //Vector3 topLeft = new Vector3(CurrentCursorSize - 1, 0, CurrentCursorSize - 1);
            //topLeft /= -2f;
            DrawIfValid(device, camera, new Point(CurrentPosition.X - 1, CurrentPosition.Y), actor);
            DrawIfValid(device, camera, new Point(CurrentPosition.X + 1, CurrentPosition.Y), actor);
            DrawIfValid(device, camera, new Point(CurrentPosition.X, CurrentPosition.Y - 1), actor);
            DrawIfValid(device, camera, new Point(CurrentPosition.X, CurrentPosition.Y + 1), actor);
        }

        public void DrawIfValid(GraphicsDevice device, ICamera camera, Point p, BaseActor actor, Texture2D cursor = null)
        {
            if (cursor == null)
            {
                cursor = CursorForSquare(p, actor);
            }
            if (cursor != null)
            {
                Vector3 v3 = V3ForSquare(p);
                Matrix m = Matrix.CreateTranslation(v3);


                m_simpleQuad.Draw(device, cursor, m, Vector3.Up, Vector3.One, camera);
            }
        }


        public void DrawIfValid(GraphicsDevice device, ICamera camera, Point p, Point nextPoint, BaseActor actor, Texture2D cursor = null)
        {
            if (cursor == null)
            {
                cursor = CursorForSquare(p, actor);
            }
            if (cursor != null)
            {
                Vector3 v3 = V3ForSquare(p);
                Matrix rot = Matrix.Identity;

                if (nextPoint.X != 0 || nextPoint.Y != 0)
                {
                    Vector3 diff = new Vector3(p.X - nextPoint.X, 0, p.Y - nextPoint.Y);
                    if (diff.X == 1)
                    {
                        Matrix.CreateRotationY((float)Math.PI / 2f, out rot);
                    }
                    if (diff.X == -1)
                    {
                        Matrix.CreateRotationY((float)(3 * Math.PI) / 2f, out rot);
                    }
                    if (diff.Z == -1)
                    {
                        Matrix.CreateRotationY((float)Math.PI, out rot);
                    }
                    if (diff.Z == 1)
                    {
                        Matrix.CreateRotationY(0, out rot);
                    }

                }

                Matrix m = rot * Matrix.CreateTranslation(v3);

                m_simpleQuad.Draw(device, cursor, m, Vector3.Up, Vector3.One, camera);
            }
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

        public void DrawMovementPath(GraphicsDevice device, ICamera camera, BaseActor actor, List<Point> points)
        {
            int numPoints = points.Count;
            for (int i = 0; i < numPoints; ++i)
            {
                if (i < (numPoints - 1))
                {
                    DrawIfValid(device, camera, points[i], points[i + 1], actor);
                }
                else
                {
                    // last point really needs target icon.
                    DrawIfValid(device, camera, points[i], actor, m_destinationCursor);
                }
            }
        }

        public Point FindClearPointNearTarget(BaseActor from, BaseActor to)
        {
            Debug.Assert(from != null && to != null);
            Point fp = from.CurrentPosition;
            Point tp = to.CurrentPosition;
            Vector3 diff = new Vector3(tp.X, 0, tp.Y) - new Vector3(fp.X, 0, fp.Y);

            // we're next to the target already.
            if (diff.LengthSquared() == 1)
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
            return (ba != null && Globals.CombatEngine.IsValidTarget(source, ba));
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


        public override void RegisterListeners()
        {
            if (RegisterCount == 0)
            {
                EventManager.ActionPressed += new EventManager.ActionButtonPressed(EventManager_ActionPressed);
                RegisterCount++;
            }
            Debug.Assert(RegisterCount == 1);
        }




        void EventManager_BaseActorChanged(object sender, BaseActorChangedArgs e)
        {
            SelectedActor = e.New;
            CurrentPosition = SelectedActor.CurrentPosition;
        }

        void EventManager_ActionPressed(object sender, ActionButtonPressedArgs e)
        {
            if (SelectedActor.CurrentAttackSkill == null)
            {
                return;
            }

            switch (e.ActionButton)
            {
                case (ActionButton.ActionButton1):
                    switch (SelectedActor.CurrentAttackSkill.AttackType)
                    {
                        case (AttackType.Single):
                        case (AttackType.AOE):
                            {
                                if (CursorOnTarget(SelectedActor))
                                {
                                    BaseActor target = m_arena.GetActorAtPosition(CurrentPosition);
                                    if (Globals.CombatEngine.IsValidTarget(SelectedActor, target))
                                    {
                                        SelectedActor.Target = target;
                                        SelectedActor.AttackRequested = true;
                                    }
                                }
                                break;
                            }
                        case (AttackType.Move):
                            {
                                SelectedActor.ConfirmMove();
                                break;
                            }
                    }
                    break;
                case (ActionButton.ActionButton2):
                    {
                        // check dependency issues on this.
                        ArenaScreen.PlayerChoiceBar.CancelAction();
                        ArenaScreen.SetMovementGridVisible(false);
                        break;
                    }
                case (ActionButton.ActionLeft):
                case (ActionButton.ActionRight):
                case (ActionButton.ActionUp):
                case (ActionButton.ActionDown):
                    {
                        Point p = ApplyMove(e.ActionButton);
                        if (m_arena.InLevel(p))
                        {
                            CurrentPosition = p;
                            SquareType st = m_arena.GetSquareTypeAtLocation(CurrentPosition);
                            BaseActor target = m_arena.GetActorAtPosition(CurrentPosition);

                            switch (SelectedActor.CurrentAttackSkill.AttackType)
                            {
                                case (AttackType.Move):
                                    {
                                        // try and find a path.
                                        SelectedActor.WayPointList.Clear();
                                        m_arena.FindPath(SelectedActor.CurrentPosition, CurrentPosition, SelectedActor.WayPointList);

                                        break;
                                    }

                                case (AttackType.Single):
                                    if(!Globals.CombatEngine.IsAttackNextTo(SelectedActor,target))
                                    {
                                        SelectedActor.WayPointList.Clear();
                                        Point adjustedPoint = CurrentPosition;
                                        if (target != null)
                                        {
                                            adjustedPoint = FindClearPointNearTarget(SelectedActor, target);
                                        }

                                        m_arena.FindPath(SelectedActor.CurrentPosition, adjustedPoint, SelectedActor.WayPointList);

                                    }
                                    break;
                            }
                        }
                        break;
                    }
            }

        }


        public Point ApplyMove(ActionButton button)
        {
            Vector3 fwd = Globals.Camera.Forward;
            Vector3 right = Vector3.Cross(fwd, Vector3.Up);
            Vector3 v = Vector3.Zero;

            Point p = CurrentPosition;
            if (button == ActionButton.ActionLeft)
            {
                v = -right;
            }
            else if (button == ActionButton.ActionRight)
            {
                v = right;
            }
            else if (button == ActionButton.ActionUp)
            {
                v = fwd;
            }
            else if (button == ActionButton.ActionDown)
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

            }
            return p;
        }



        public override void UnregisterListeners()
        {
            if (RegisterCount == 1)
            {
                EventManager.ActionPressed -= new EventManager.ActionButtonPressed(EventManager_ActionPressed);
                RegisterCount--;

            }
            Debug.Assert(RegisterCount == 0);
        }

        public Vector3 m_cursorMovement = Vector3.Zero;
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
        public Texture2D m_destinationCursor;

    }


    //public enum GridMode
    //{
    //    Inactive,
    //    Select,
    //    Move
    //}

}
