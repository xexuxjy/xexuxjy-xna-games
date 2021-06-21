using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

//namespace Gladius
//{

public static class GladiusGlobals
{
    public static Vector3 DefaultModelScale = new Vector3(1, 1, 1);
    public static Vector3 DefaultPropModelScale = new Vector3(1, 1, 1);
    public static Vector3 DefaultCharacterModelScale = new Vector3(1, 1, 1);

    public static Vector3 DefaultOffScreenItemCamPosition = new Vector3(1000, 0, 1000);


    public static String DataRoot = "GladiusData/";
    public static String SchoolsPath = DataRoot + "Schools/";
    public static String TownsPath = DataRoot + "Towns/";
    public static String EncountersPath = DataRoot + "Encounters/";
    public static String LeaguesPath = DataRoot + "Leagues/";
    //public static String ModelsRoot = "GladiusModels/";
    public static String ModelsRoot = "XBoxModelPrefabs/";
    public static String CharacterModelsRoot = ModelsRoot + "Characters/";
    public static String WeaponModelsRoot = ModelsRoot + "Weapons/";
    public static String ArenaModelsRoot = ModelsRoot + "Arenas/";
    public static String PropModelsRoot = ModelsRoot + "Props/";
    public static String MaterialsRoot = "Materials/";
    public static String UIRoot = "GladiusUI/";
    public static String UIElements = UIRoot + "UIElements/";
    public static String ArenaUIRoot = UIRoot+"Arena/";

    public const int MaxLevel = 20;


    public static System.Random Random = new System.Random();

    public static int Random100()
    {
        return Random.Next(1, 100);
    }


    public static Transform FuzzyFindChild(string name, Transform t)
    {
        if (t.name.Contains(name))
            return t;
        foreach (Transform child in t)
        {
            Transform ct = FuzzyFindChild(name, child);
            if (ct != null)
                return ct;
        }
        return null;
    }

    public static void AdjustV3(ref Vector3 input)
    {
        input = new Vector3(input.x, input.z, input.y);
    }

    public static Vector3 AdjustV3(Vector3 input)
    {
        input = new Vector3(input.x, input.z, input.y);
        return input;
    }

    public static void AdjustQuaternion(ref Quaternion q)
    {
        q = new Quaternion(q.x, q.z, q.y, q.w);
    }



    public static String AdjustModelName(String modelName)
    {
        if (!modelName.EndsWith(".mdl"))
        {
            modelName += ".mdl";
        }
        return modelName;
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


    public static void OffsetRect(ref Rect src, ref Rect dst)
    {
        dst.x += src.x;
        dst.y += src.y;
    }



    public static float Max(Vector3 v)
    {
        return v.x < v.y ? (v.y < v.z ? v.z : v.y) : (v.x < v.z ? v.z : v.x);
    }

    public static float Min(Vector3 v)
    {
        return v.x < v.y ? (v.x < v.z ? v.x : v.z) : (v.y < v.z ? v.y : v.z);
    }



    public static int Clamp(int v, int min, int max)
    {
        return v < min ? min : v > max ? max : v;
    }

    public static Vector3 ReadVector3(String path, XmlDocument doc)
    {
        Vector3 result = new Vector3();
        result.x = float.Parse(doc.SelectSingleNode(path + "/@x").InnerText);
        result.y = float.Parse(doc.SelectSingleNode(path + "/@y").InnerText);
        result.z = float.Parse(doc.SelectSingleNode(path + "/@z").InnerText);
        return result;
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


    public static Vector2 CardinalNormalize(Vector2 a)
    {
        if (Math.Abs(a.x) > Math.Abs(a.y))
        {
            if (a.x < 0) return new Vector2(-1, 0);
            else return new Vector2(1, 0);
        }
        else
        {
            if (a.y < 0) return new Vector2(0, -1);
            else return new Vector2(0, 1);

        }
    }

    public static Vector3 Mult(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public static string StripTextureExtension(string textureName)
    {
        textureName = textureName.Replace(".tga", "");
        textureName = textureName.Replace(".png", "");
        return textureName;
    }



    public const String PlayerTeam = "Player";
    public const String EnemyTeam1 = "Enemy1";
    public const String EnemyTeam2 = "Enemy2";
    public const String EnemyTeam3 = "Enemy3";

    static char[] trimChars = new char[] { '"', '\r', ' ', '\t' };
    public static String[] SplitAndTidyString(String input, char[] splitChars, bool removeComments = true, bool removeEmpty = true)
    {
        // strip comments
        if (removeComments && input.Contains("//"))
        {
            input = input.Substring(0, input.IndexOf("//"));
        }
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
            if (!removeEmpty || !String.IsNullOrEmpty(lineTokens[i]))
            {
                validTokens.Add(lineTokens[i]);
            }
        }
        return validTokens.ToArray();
    }

    public static string SceneForZone(OverlandZone zone)
    {
        switch (zone)
        {
            case OverlandZone.Imperia:
                return "ImperiaWorldMap";
            case OverlandZone.Nordagh:
                return "NordaghWorldMap";
            case OverlandZone.Steppes:
                return "SteppesWorldMap";
            case OverlandZone.Expanse:
                return "ExpanseWorldMap";
        }
        return null;
    }


}


public class DebugUtils {[Conditional("DEBUG")] public static void Assert(bool condition) { if (!condition) throw new Exception(); } }

//}

/// <summary>
/// Camera frustum extensions
/// </summary>
/// https://www.snip2code.com/Snippet/465692/Helpful-unity-frustum-retulated-camera-e
public static class CameraFrustumExtensions
{
    /// <summary>
    /// Returns the frustum's height at the given distance from the camera.
    /// This returns the entire height from top to bottom, so if your camera is at 0,0
    /// the frustums top is at: Frustrumheight/2 and bottom is at: -Frustrumheight/2
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static float FrustumHeightAtDistace(this Camera camera, float distance)
    {
        return 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    /// <summary>
    /// Returns the distance from camera a given frustrum height can be found
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="frustumHeight"></param>
    /// <returns></returns>
    public static float DistanceAtFrustumHeight(this Camera camera, float frustumHeight)
    {
        return frustumHeight * 0.5f / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
    }

    /// <summary>
    /// Returns the frustum's width at a given distance from the camera.
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="frustumHeight"></param>
    /// <returns></returns>
    public static float FrustumWidthAtFrustumHeight(this Camera camera, float frustumHeight)
    {
        return frustumHeight * camera.aspect;
    }

}