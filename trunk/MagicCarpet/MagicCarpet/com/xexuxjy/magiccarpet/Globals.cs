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

        public static String textureBase;
        public static String deepWaterTextureId;
        public static String shallowWaterTextureId;
        public static String sandTextureId;
        public static String grassTextureId;
        public static String screeTextureId;
        public static String iceTextureId;


        public static String modelBase;
        public static String magicianModel;
        public static String manaballModel;
        public static String balloonModel;

        public static String castle1Model;
        public static String castle2Model;
        public static String castle3Model;
        public static String castle4Model;
        public static String castle5Model;

        public static String spellBase;
        public static String fireballModel;
        public static String movelandModel;
        public static String convertModel;

        public static String skydomeModel;

        public static String monster1Model;
        public static String monster2Model;
        public static String monster3Model;

        public static String planeMeshModel;


        public static String effectBase;
        public static String terrainEffect;

        public static String fontBase;
        public static String debugFont;

        public static int DebugDrawOrder = 5;
        public static int TextDrawOrder = 4;
        public static int CursorDrawOrder = 3;
        public static int ObjectDrawOrder = 2;
        public static int TerrainDrawOrder = 1;
        public static int DefaultDrawOrder = 0;


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


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Vector3 worldMinPos = new Vector3(-2048f, -100f, -2048f);
        public static Vector3 worldMaxPos = new Vector3(2048f, 100f, 2048f);

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Vector3 gravity = new Vector3(0f, -9.8f, 0f);


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ICamera Camera;
        public static Terrain Terrain;
        public static XNA_ShapeDrawer DebugDraw;
        public static CollisionManager CollisionManager;
        public static GameObjectManager GameObjectManager;

        public const float STEPSIZEROTATE = (float)Math.PI/ 3f; // 60 deg a second
        public const float STEPSIZETRANSLATE = 20f; 

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // hacky for now to preserve last point.

        public static bool cameraHasGroundContact;
        public static Vector3 cameraGroundContactPoint;
        public static Vector3 cameraGroundContactNormal;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void Initialize()
        {
            String contentBase = "";

            textureBase = contentBase+"Textures/";
            deepWaterTextureId = textureBase + "Terrain/deepWater";
            shallowWaterTextureId = textureBase + "Terrain/shallowWater";
            sandTextureId = textureBase + "Terrain/sand";
            grassTextureId = textureBase + "Terrain/grass";
            screeTextureId = textureBase + "Terrain/scree";
            iceTextureId = textureBase + "Terrain/ice";

            modelBase = contentBase+"Models/";
            magicianModel = modelBase + "Magician/magician";
            manaballModel = modelBase + "SimpleShapes/unitsphere";//"Manaball/Manaball";
            balloonModel = modelBase + "Balloon/Balloon";

            castle1Model = modelBase + "Castle/CastleSize1";
            castle2Model = modelBase + "Castle/CastleSize2";
            castle3Model = modelBase + "Castle/CastleSize3";
            castle4Model = modelBase + "Castle/castle4";
            castle5Model = modelBase + "Castle/castle5";

            monster1Model = modelBase + "Monster/monster1";
            monster2Model = modelBase + "Monster/monster2";
            monster3Model = modelBase + "Monster/monster3";

            fireballModel = manaballModel;

            skydomeModel = modelBase + "Skydome/skydome";

            planeMeshModel = modelBase + "PlaneMesh/planeMesh";

            effectBase = contentBase+"Effects/";
            terrainEffect = effectBase + "Terrain/terrain1";

            fontBase = contentBase+"Fonts/";
            debugFont = fontBase + "DebugFont";

            spellBase = modelBase + "Spells/";
            fireballModel = spellBase + "Fireball";
            movelandModel = spellBase + "Moveland";
            convertModel = spellBase + "Convert";
        }

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
