using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using com.xexuxjy.magiccarpet.terrain;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.collision;
using BulletXNADemos.Demos;
using Dhpoware;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet.util.profile;
using com.xexuxjy.magiccarpet.util.console;
using com.xexuxjy.magiccarpet.manager;
using GameStateManagement;
using com.xexuxjy.magiccarpet.util.debug;
using System.IO;

namespace com.xexuxjy.magiccarpet
{
    public static class Globals
    {

        public static GameObjectType s_attackableObjects = GameObjectType.magician | GameObjectType.castle | GameObjectType.balloon | GameObjectType.player | GameObjectType.monster;


        // jut test values for now.
        public static float containmentMaxHeight = 100f;
        public static float containmentMinHeight = -100f;

        public static bool ProfilingEnabled;
        public static bool DebugTextEnabled = true;
        public static bool BulletCollisionEnabled;
        public static bool TerrainCollisionEnabled = true;


        public static Keys InsertWayPoint = Keys.Insert;
        public static Keys DeleteWayPoint = Keys.Delete;
        public static Keys LowerLandModifier = Keys.LeftControl;
        public static Keys Solid = Keys.F10;
        public static Keys WireFrame = Keys.F11;
        public static Keys Points = Keys.F12;
        public static Keys DebugObjectForward = Keys.P;
        public static Keys DebugObjectBackwards = Keys.O;
        public static Keys Quit = Keys.Escape;

        public static Keys Forward = Keys.W;
        public static Keys Backwards = Keys.S;
        public static Keys Left = Keys.A;
        public static Keys Right = Keys.D;
        public static Keys Up = Keys.Q;
        public static Keys Down = Keys.Z;
        public static Keys A = Keys.P;

        public static String DebugXMLNodeText = "";
        public static String DebugXMLNodeTag = "foo";

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Object constants 
        // Todo - load these in as config

        public static string s_initialScript = "level2";
        public static int s_initialRandomSeed = 1;

        public static float s_balloonLoadTime = 2.0f;
        public static float s_balloonUnLoadTime = 2.0f;
        public static float s_balloonSearchRadiusManaball = 50f;
        public static float s_balloonSearchRadiusCastle = 50f;
        public static float s_balloonTravelSpeed = 5f;
        public static float s_balloonFleeSpeed = 10f;
        public static float s_balloonMaxCapacity = 100f;
        public static float s_balloonHoverHeight = 5f;
        public static float s_balloonFleeHealthPercentage = 0.25f;

        public static float s_monsterSearchRadius = 50f;
        public static float s_monsterTravelSpeed = 0.1f;
        public static float s_monsterFleeSpeed = 5.0f;
        public static float s_monsterMeleeRange = 0.5f;
        public static float s_monsterMeleeDamage = 5f;
        public static float s_monsterRangedRange = 10f;
        public static float s_monsterRangedDamage = 5f;
        public static float s_monsterFleeHealthPercentage = 0.25f;
        public static float s_monsterMaxFollowRange = 50;


        public static int s_castleSize1 = 2;
        public static int s_castleSize2 = 4;
        public static int s_castleSize3 = 8;
        public static float s_castleTurretSearchRadius = 2;
        public static float s_castleTurretSearchFrequency = 2;
        public static float s_castleTurretAttackFrequency = 2;
        public static float s_castleTurretAttackDamage = 2;

        public static float s_magicianTravelSpeed = 5f;
        public static float s_magicianFleeSpeed = 10f;
        public static float s_magicianSearchRadiusManaball = 50f;
        public static float s_magicianSearchRadiusMonster = 50f;
        public static float s_magicianMeleeRange = 0.5f;
        public static float s_magicianMeleeDamage = 5f;
        public static float s_magicianRangedRange = 10f;
        public static float s_magicianRangedDamage = 5f;
        public static float s_magicianMaxFollowRange = 50;



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Color player1Colour = Color.Red;
        public static Color player2Colour = Color.Green;
        public static Color player3Colour = Color.Blue;
        public static Color player4Colour = Color.Yellow;
        public static Color unassignedPlayerColour = Color.Pink;




        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Debug Text Positions
        public static Vector3 DebugTextCamera = new Vector3(10, 10, 0);
        public static Vector3 DebugTextFPS = new Vector3(10, 20, 0);
        public static Vector3 DebugTextPlayer = new Vector3(10, 30, 0);
        public static Vector3 DebugTextCollisionManager = new Vector3(10, 40, 0);


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public const int WorldWidth = 512;//1024;
        public const int WorldHeight = 10;

        public static Vector3 worldMinPos = new Vector3(-WorldWidth/2, -WorldHeight, -WorldWidth/2);
        public static Vector3 worldMaxPos = -worldMinPos;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Vector3 gravity = new Vector3(0f, -9.8f, 0f);

        public static Random s_random = null;

        public static int GUI_DRAW_ORDER = 2;
        public static int TERRAIN_DRAW_ORDER = 1;
        public static int NORMAL_DRAW_ORDER = 1;



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void EnableDebugConsole()
        {
            if (SimpleConsole != null)
            {
                SimpleConsole.Enabled = true;
                if (Camera != null)
                {
                    Camera.DisableKeyboardInput();
                }
            }
        }

        public static void DisableDebugConsole()
        {
            if (SimpleConsole != null)
            {
                SimpleConsole.Enabled = false;
                if (Camera != null)
                {
                    Camera.EnableKeyboardInput();
                }
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Magician Player;
        
        public static ICamera Camera;
        public static Terrain Terrain;
        public static IDebugDraw DebugDraw;
        public static CollisionManager CollisionManager;
        public static GameObjectManager GameObjectManager;
        public static DebugObjectManager DebugObjectManager;
        public static SimpleProfiler SimpleProfiler;
        public static GraphicsDevice GraphicsDevice;
        public static SimpleConsole SimpleConsole;
        public static MCContentManager MCContentManager;
        public static EventLogger EventLogger;
        public static ActionPool ActionPool;
        public static SpellPool SpellPool;

        public static ScreenManager ScreenManager;

        public static Game Game;

        public const float STEPSIZEROTATE = (float)Math.PI/ 3f; // 60 deg a second
        public const float STEPSIZETRANSLATE = 20f; 

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // hacky for now to preserve last point.

        public static bool cameraHasGroundContact;
        public static Vector3 cameraGroundContactPoint;
        public static Vector3 cameraGroundContactNormal;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////




        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //public static Keys formsToXNAInput(System.Windows.Forms.Keys key)
        //{
        //    if (key == System.Windows.Forms.Keys.Insert)
        //    {
        //        return Keys.Insert;
        //    }
        //    if (key == System.Windows.Forms.Keys.Delete)
        //    {
        //        return Keys.Delete;
        //    }
        //    if (key == System.Windows.Forms.Keys.LControlKey)
        //    {
        //        return Keys.LeftControl;
        //    }
        //    return Keys.Space;
        //}

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void LoadConfig()
        {
            Dictionary<String,String> configMap = new Dictionary<String,String>();

            try
            {
                List<String> commands = new List<String>();
                using (StreamReader reader = new StreamReader("../../../config/settings.txt"))
                {
                    while (reader.EndOfStream == false)
                    {
                        String dataLine = reader.ReadLine().ToLower();
                        if (dataLine.StartsWith("#") || String.IsNullOrWhiteSpace(dataLine))
                        {
                            continue;
                        }
                        int equalsIndex = dataLine.IndexOf('=');
                        String key = dataLine.Substring(0, equalsIndex);
                        String value = dataLine.Substring(equalsIndex + 1, dataLine.Length - equalsIndex - 1);
                        configMap[key] = value;
                    }
                }
            }
            catch (System.Exception ex)
            {
            }



            TryReadString(configMap, "initial.script", ref s_initialScript);
            TryReadInt(configMap, "initial.random.seed", ref s_initialRandomSeed);
            s_random = new Random(s_initialRandomSeed);


            TryReadFloat(configMap,"balloon.travel.speed",ref s_balloonTravelSpeed);
            TryReadFloat(configMap, "balloon.flee.speed", ref s_balloonFleeSpeed);

            TryReadFloat(configMap,"balloon.max.capacity",ref s_balloonMaxCapacity);

            TryReadFloat(configMap,"balloon.load.time",ref s_balloonLoadTime);
            TryReadFloat(configMap,"balloon.unload.time",ref s_balloonUnLoadTime);
            TryReadFloat(configMap,"balloon.search.radius.manaball",ref s_balloonSearchRadiusManaball);
            TryReadFloat(configMap,"balloon.search.radius.castle",ref s_balloonSearchRadiusCastle);
            TryReadFloat(configMap,"balloon.hover.height", ref s_balloonHoverHeight);
            TryReadFloat(configMap,"balloon.flee.health.percentage", ref s_balloonFleeHealthPercentage);


            TryReadInt(configMap, "castle.size1", ref s_castleSize1);
            TryReadInt(configMap, "castle.size2", ref s_castleSize2);
            TryReadInt(configMap, "castle.size3", ref s_castleSize3);
            TryReadFloat(configMap, "castle.turret.search.radius", ref s_castleTurretSearchRadius);
            TryReadFloat(configMap, "castle.turret.search.frequency", ref s_castleTurretSearchFrequency);
            TryReadFloat(configMap, "castle.turret.attack.frequency", ref s_castleTurretAttackFrequency);
            TryReadFloat(configMap, "castle.turret.attack.damage", ref s_castleTurretAttackDamage);


            TryReadFloat(configMap, "monster.flee.health.percentage", ref s_monsterFleeHealthPercentage);
            TryReadFloat(configMap, "monster.search.radius", ref s_monsterSearchRadius);
            TryReadFloat(configMap, "monster.travel.speed", ref s_monsterTravelSpeed);
            TryReadFloat(configMap, "monster.flee.speed", ref s_monsterFleeSpeed);
            TryReadFloat(configMap, "monster.melee.damage", ref s_monsterMeleeDamage);
            TryReadFloat(configMap, "monster.melee.range", ref s_monsterMeleeRange);
            TryReadFloat(configMap, "monster.ranged.damage", ref s_monsterRangedDamage);
            TryReadFloat(configMap, "monster.ranged.range", ref s_monsterRangedRange);
            TryReadFloat(configMap, "monster.max.follow.range", ref s_monsterMaxFollowRange);


            TryReadFloat(configMap, "magician.search.radius.manaball", ref s_magicianSearchRadiusManaball);
            TryReadFloat(configMap, "magician.search.radius.monster", ref s_magicianSearchRadiusMonster);
            TryReadFloat(configMap, "magician.travel.speed", ref s_magicianTravelSpeed);
            TryReadFloat(configMap, "magician.flee.speed", ref s_magicianFleeSpeed);
            TryReadFloat(configMap, "magician.melee.damage", ref s_magicianMeleeDamage);
            TryReadFloat(configMap, "magician.melee.range", ref s_magicianMeleeRange);
            TryReadFloat(configMap, "magician.ranged.damage", ref s_magicianRangedDamage);
            TryReadFloat(configMap, "magician.ranged.range", ref s_magicianRangedRange);
            TryReadFloat(configMap, "magician.max.follow.range", ref s_magicianMaxFollowRange);


           // TODO - Provide spells Data.




        }

        public static void TryReadFloat(Dictionary<String, String> map, String key, ref float floatVal)
        {
            String temp;
            if (map.TryGetValue(key, out temp))
            {
                floatVal = float.Parse(temp);
            }
        }

        public static void TryReadInt(Dictionary<String, String> map, String key, ref int intVal)
        {
            String temp;
            if (map.TryGetValue(key, out temp))
            {
                intVal = int.Parse(temp);
            }
        }

        public static void TryReadString(Dictionary<String, String> map, String key, ref string stringVal)
        {
            String temp;
            if (map.TryGetValue(key, out temp))
            {
                stringVal = temp;
            }
        }



    }
}
