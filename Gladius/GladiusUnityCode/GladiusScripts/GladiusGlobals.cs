﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

//namespace Gladius
//{

    public static class GladiusGlobals
    {
        //public const float MovementStepTime = 2f;

        //public static MovementGrid MovementGrid;
        //public static UserControl UserControl;
        //public static Camera Camera;
        
        //public static ItemManager ItemManager = new ItemManager();
        //public static CombatEngine CombatEngine = new CombatEngine();
        //public static TurnManager TurnManager;
        //public static LocalisationData LocalisationData;
        //public static Arena Arena;

        //public static LOSTester LOSTester;

        //public static CombatEngineUI CombatEngineUI;

        //public static BattleData CurrentBattleData;

        //public static ShopManager ShopManager;
        //public static Shop CurrentShop;

        public static String DataRoot = "GladiusData/";
        public static String SchoolsPath = DataRoot+"Schools/";
        public static String TownsPath = DataRoot + "Towns/";
        public static String EncountersPath = DataRoot + "Encounters/";
        public static String LeaguesPath = DataRoot + "Leagues/";
        public static String ModelsRoot = "GladiusModels/";
        public static String CharacterModelsRoot = ModelsRoot+"Characters/";
        public static String WeaponModelsRoot = ModelsRoot + "Weapons/";
        public static String ArenaModelsRoot = ModelsRoot + "Arenas/";
        public static String UIRoot = "GladiusUI/";

        public static GameStateManager GameStateManager;
        //public static GladiatorSchool GladiatorSchool;
        //public static CameraManager CameraManager = new CameraManager();
        //public static LocalisationData LocalisationData = new LocalisationData();
        //public static AttackSkillDictionary AttackSkillDictionary;
        //public static ItemManager ItemManager = new ItemManager();
        public static EventLogger EventLogger = new EventLogger(null);



        //public static bool ArenaComponentsInitialised
        //{
        //    get
        //    {
        //        return MovementGrid != null && UserControl != null && CameraManager != null && CombatEngine != null && Arena != null;
        //    }
        //}


        //public static Crowd Crowd;

        public const int MaxLevel = 20;

        public static ColourTextureHelper ColourTextureHelper = new ColourTextureHelper();

        //public static ThreadSafeContentManager GlobalContentManager;

        public static System.Random Random = new System.Random();

        public static int Random100()
        {
            return Random.Next(1, 100);
        }




        public static Rect RectangleToRect(Rectangle r)
        {
            return new Rect(r.X, r.Y, r.Width, r.Height);
        }

        public static Transform FindChild(string name, Transform t)
        {
            if (t.name == name)
                return t;
            foreach (Transform child in t)
            {
                Transform ct = FindChild(name, child);
                if (ct != null)
                    return ct;
            }
            return null;
        }

        //recursive calls
        public static void MoveToLayer(Transform root, int layer)
        {
            root.gameObject.layer = layer;
            foreach (Transform child in root)
                MoveToLayer(child, layer);
        }

        //public static bool NextToTarget(BaseActor from, BaseActor to)
        //{
        //    Debug.Assert(from != null && to != null);
        //    if (from != null && to!= null)
        //    {
        //        Point fp = from.CurrentPosition;
        //        Point tp = to.CurrentPosition;
        //        Vector3 diff = new Vector3(tp.X, 0, tp.Y) - new Vector3(fp.X, 0, fp.Y);
        //        return diff.LengthSquared() == 1f;
        //    }
        //    return false;
        //}



        public static Rect AddRect(Rect a, Rect b)
        {
            return new Rect(a.x + b.x, a.y + b.y, a.width + b.width, a.height + b.height);
        }


        public static Rect InsetRectangle(Rect orig, int inset)
        {
            Rect insetRectangle = orig;
            insetRectangle.width -= (inset * 2);
            insetRectangle.height -= (inset * 2);
            insetRectangle.x += inset;
            insetRectangle.y += inset;
            return insetRectangle;
        }

        public static void OffsetRect(ref Rectangle src, ref Rectangle dst)
        {
            dst.X += src.X;
            dst.Y += src.Y;
        }

        public static void OffsetRect(ref Rect src, ref Rect dst)
        {
            dst.x += src.x;
            dst.y += src.y;
        }


        public static int PointDist2(Point start, Point end)
        {
            Point diff = Point.Subtract(end, start);
            return (diff.X * diff.X) + (diff.Y * diff.Y);
        }

        public static int PathDistance(Point start, Point end)
        {
            int xdiff = Math.Abs(start.X - end.X);
            int ydiff = Math.Abs(start.Y - end.Y);
            return xdiff + ydiff;
        }


        public static int Clamp(int v, int min, int max)
        {
            return v < min ? min : v > max ? max : v;
        }


        public static Quaternion QuatNormalize(Quaternion q)
        {
            float num = 1f / (float)Math.Sqrt((double)q.x * (double)q.x + (double)q.y * (double)q.y + (double)q.z * (double)q.z + (double)q.w * (double)q.w);
            Quaternion q2 = new Quaternion();
            q2.x = q.x * num;
            q2.y = q.y * num;
            q2.z = q.z * num;
            q2.w = q.w * num;
            return q2;
        }

        public static bool FuzzyEquals(Quaternion q1, Quaternion q2)
        {
            q1 = QuatNormalize(q1);
            q2 = QuatNormalize(q2);

            float diff = Math.Abs(1f - Quaternion.Dot(q1, q2));
            float closeEnough = 0.0001f;
            return diff < closeEnough;
        }

        public static Point Add(Point value1, Point value2)
        {
            return new Point(value1.X + value2.X, value1.Y + value2.Y);
        }

        public static Point Subtract(Point value1, Point value2)
        {
            return new Point(value1.X - value2.X, value1.Y - value2.Y);
        }

        public static Point Cross(Point value)
        {
            return new Point(value.Y, -value.X);
        }

        public static Point Mult(Point value, int scalar)
        {
            return new Point(value.X * scalar, value.Y * scalar);
        }

        public static Point CardinalNormalize(Point a)
        {
            if(Math.Abs(a.X) > Math.Abs(a.Y))
            {
                if(a.X < 0) return new Point(-1,0);
                else return new Point(1,0);
            }
            else
            {
                if(a.Y < 0) return new Point(0,-1);
                else return new Point(0,1);

            }
        }

        // yucky way of passing data between scenes.
        public static List<CharacterData> ChosenCharacterList = new List<CharacterData>();
        public static Dictionary<Point, CharacterData> CharacterPlacementMap = new Dictionary<Point, CharacterData>();




        public const int SkyBoxDrawOrder = 0;
        public const int ArenaDrawOrder = 1;
        public const int MoveGridDrawOrder = 2;
        public const int CharacterDrawOrder = 3;

        public static Point[] CardinalPoints = new Point[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };
        public static Point[] SurroundingPoints = new Point[] { new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(0, -1), 
            new Point(0,1),new Point(-1,1),new Point(0,1),new Point(1,1)};


        public const String PlayerTeam = "Player";
        public const String EnemyTeam1 = "Enemy1";
        public const String EnemyTeam2 = "Enemy2";
        public const String EnemyTeam3 = "Enemy3";

        static char[] trimChars = new char[] { '"', '\r',' ','\t' };
        public static String[] SplitAndTidyString(String input,char[] splitChars)
        {
            String[] lineTokens = input.Split(splitChars);
            List<string> validTokens = new List<string>();
            for (int i = 0; i < lineTokens.Length; ++i)
            {
                lineTokens[i] = lineTokens[i].Replace("\"", "").Trim(trimChars);
                int commentIndex = lineTokens[i].IndexOf("//");
                if (commentIndex >= 0)
                {
                    lineTokens[i] = lineTokens[i].Substring(0, commentIndex);
                }
                if (!String.IsNullOrEmpty(lineTokens[i]))
                {
                    validTokens.Add(lineTokens[i]);
                }
            }
            return validTokens.ToArray();
        }



    }


public class DebugUtils { [Conditional("DEBUG")] public static void Assert(bool condition) { if (!condition) throw new Exception(); } }

//}