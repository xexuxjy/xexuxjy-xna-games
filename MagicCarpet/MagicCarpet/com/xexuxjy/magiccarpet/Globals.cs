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
using com.xexuxjy.utils.debug;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.utils.profile;
using com.xexuxjy.utils.console;
using com.xexuxjy.magiccarpet.manager;

namespace com.xexuxjy.magiccarpet
{
    public static class Globals
    {
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

        public static float BalloonLoadTime = 2.0f;
        public static float BalloonUnLoadTime = 2.0f;


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


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public const int WorldWidth = 64;//1024;
        public const int WorldHeight = 10;

        public static Vector3 worldMinPos = new Vector3(-WorldWidth/2, -WorldHeight, -WorldWidth/2);
        public static Vector3 worldMaxPos = -worldMinPos;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Vector3 gravity = new Vector3(0f, -9.8f, 0f);


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





    }
}
