using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.control;
using Dhpoware;
using Gladius.combat;
using Gladius.util;
using Gladius.actors;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Gladius
{
    public class Globals
    {
        public const float MovementStepTime = 2f;

        public static UserControl UserControl;
        public static CameraComponent Camera;
        public static CombatEngine CombatEngine;
        public static EventLogger EventLogger;
        public static MovementGrid MovementGrid;
        public static AttackBar AttackBar;
        public static PlayerChoiceBar PlayerChoiceBar;
        public static SoundManager SoundManager;


        public static AttackSkillDictionary AttackSkillDictionary;


        public static ThreadSafeContentManager GlobalContentManager;


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

    }
}
