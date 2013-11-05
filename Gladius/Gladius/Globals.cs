using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.control;
using Gladius.combat;
using Gladius.util;
using Gladius.actors;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Gladius.renderer;

namespace Gladius
{
    public class Globals
    {
        public const float MovementStepTime = 2f;

        public static UserControl UserControl;
        public static ICamera Camera;
        public static CameraManager CameraManager;

        public static EventLogger EventLogger;
        //public static MovementGrid MovementGrid;
        //public static AttackBar AttackBar;
        //public static PlayerChoiceBar PlayerChoiceBar;
        //public static CombatEngine CombatEngine;

        public static SoundManager SoundManager;

        public static BoundingFrustum BoundingFrustum;


        public static AttackSkillDictionary AttackSkillDictionary;


        //public static ThreadSafeContentManager GlobalContentManager;

        public static Random Random = new Random();


        public static bool NextToTarget(BaseActor from, BaseActor to)
        {
            Debug.Assert(from != null && to != null);
            if (from != null && to!= null)
            {
                Point fp = from.CurrentPosition;
                Point tp = to.CurrentPosition;
                Vector3 diff = new Vector3(tp.X, 0, tp.Y) - new Vector3(fp.X, 0, fp.Y);
                return diff.LengthSquared() == 1f;
            }
            return false;
        }



        public static void DrawCameraDebugText(SpriteBatch spriteBatch,SpriteFont spriteFont,int fps)
        {
            string text = null;
            StringBuilder buffer = new StringBuilder();
            Vector2 fontPos = new Vector2(1.0f, 1.0f);
            buffer.AppendFormat("FPS: {0} \n", fps);
            buffer.Append("Camera:\n");
            buffer.AppendFormat("  Position: x:{0} y:{1} z:{2}\n",
                Globals.Camera.Position.X.ToString("#0.00"),
                Globals.Camera.Position.Y.ToString("#0.00"),
                Globals.Camera.Position.Z.ToString("#0.00"));
            buffer.AppendFormat("  Velocity: x:{0} y:{1} z:{2}\n",
                Globals.Camera.Velocity.X.ToString("#0.00"),
                Globals.Camera.Velocity.Y.ToString("#0.00"),
                Globals.Camera.Velocity.Z.ToString("#0.00"));

            buffer.AppendFormat("  Forward: x:{0} y:{1} z:{2}\n",
                Globals.Camera.Forward.X.ToString("#0.00"),
                Globals.Camera.Forward.Y.ToString("#0.00"),
                Globals.Camera.Forward.Z.ToString("#0.00"));

            //if(Globals.MovementGrid != null)
            //{
            //    buffer.AppendFormat("Cursor Pos : [{0},{1}]", Globals.MovementGrid.CurrentPosition.X, Globals.MovementGrid.CurrentPosition.Y);
            //}

            text = buffer.ToString();

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.DrawString(spriteFont, text, fontPos, Color.Yellow);
            spriteBatch.End();
        }

        public static void RemapModel(Model model, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
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

        public static Vector2 CenterText(SpriteFont font, String text, Vector2 pos)
        {
            Vector2 dims = font.MeasureString(text);
            dims /= 2f;
            return pos - dims;
        }

        public static int PointDist2(Point start, Point end)
        {
            Point diff = end -start;
            return (diff.X * diff.X) + (diff.Y * diff.Y);
        }

        public static int PathDistance(Point start, Point end)
        {
            int xdiff = Math.Abs(start.X - end.X);
            int ydiff = Math.Abs(start.Y - end.Y);
            return xdiff + ydiff;
        }

        public const int ArenaDrawOrder = 1;
        public const int MoveGridDrawOrder = 2;
        public const int CharacterDrawOrder = 3;



        public const String PlayerTeam = "Player";
        public const String EnemyTeam1 = "Enemy1";
        public const String EnemyTeam2 = "Enemy2";
        public const String EnemyTeam3 = "Enemy3";

    }
}
