using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.control;
using Dhpoware;
using Gladius.combat;
using Gladius.util;

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

        public static ThreadSafeContentManager GlobalContentManager;
    }
}
