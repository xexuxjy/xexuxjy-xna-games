using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Text;
using System.IO;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

//namespace Gladius
//{

public static class GladiusGlobals
{

    public static Vector3 OffScreenItemCamPosition1 = new Vector3(1000, 0, 1000);
    public static Vector3 OffScreenItemCamPosition2 = new Vector3(2000, 0, 2000);

    public static Quaternion CharacterLocalRotation = Quaternion.identity;// Quaternion.Euler(270.0f, 0, 0);

    public static Point InvalidPoint = new Point(-1000, -1000);
    public static Point[] CardinalPoints = new Point[] { new Point(0, 0), new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };
    public static Point[] SurroundingPoints = new Point[] { new Point(0,0),new Point(-1, -1), new Point(0, -1), new Point(1, -1), new Point(0, -1),
            new Point(0,1),new Point(-1,1),new Point(0,1),new Point(1,1)};


    public const int BaseMovePoints = 10;
    public const int BaseMoveToAttackPoints = 2;

    public const int CUBE = 2;

    public static String GetStringVal(this ITerminalNode node)
    {
        return node.GetText().Replace("\"", "");
    }

    public static int GetIntVal(this ITerminalNode node)
    {
        int result = 0;
        if (!int.TryParse(node.GetText(), out result))
        {
            String text = node.GetText();
            int ibreak = 0;
        }
        return result;
    }

    public static float GetFloatVal(this ITerminalNode node)
    {
        return float.Parse(node.GetText());
    }



    public static string StripTextureExtension(string textureName)
    {
        textureName = textureName.Replace(".tga", "");
        textureName = textureName.Replace(".png", "");
        return textureName;
    }


    // Gladius Model Axis.
    public static Vector3 GMUp = Vector3.up;
    public static Vector3 GMRight = Vector3.right;
    public static Vector3 GMForward = Vector3.forward;

    public static void WriteToSB(IEnumerable<string> collection, StringBuilder sb, char separator = ',')
    {
        foreach(string value in collection)
        {
            sb.Append(value);
            sb.Append(separator);
        }
    }


    public static TextAsset LoadTextAsset(String filename)
    {
        filename = GetFileName(filename);
        TextAsset ta = Resources.Load<TextAsset>(filename);
        if(ta == null)
        {
            UnityEngine.Debug.LogWarning("Can't read textasset : " + filename);
        }

        return ta;
    }

    public static TextAsset[] LoadAllTextAsset(String filename)
    {
        filename = GetFileName(filename);
        return Resources.LoadAll<TextAsset>(filename);
    }


    public static Material LoadMaterial(String filename)
    {
        filename = GetFileName(filename);
        return Resources.Load<Material>(filename);
    }

    public static Sprite LoadSprite(String filename)
    {
        filename = GetFileName(filename);
        return Resources.Load<Sprite>(filename);
    }

    public static Sprite[] LoadAllSprite(String filename)
    {
        filename = GetFileName(filename);
        return Resources.LoadAll<Sprite>(filename);
    }


    public static string GetLastPart(this string theString, char theSeparator)
    {
        string[] aSplit = theString.Split(theSeparator);
        return aSplit[aSplit.Length - 1];
    }

    public static List<T> ToDynList<T>(this IEnumerable<T> theList)
    {
        return new List<T>(theList);
    }

    public static void SetLayerRecursively(this GameObject theGameObject, int theLayer)
    {
        theGameObject.layer = theLayer;
        foreach (Transform aTransform in theGameObject.transform)
        {
            GameObject aGameObject = aTransform.gameObject;
            aGameObject.SetLayerRecursively(theLayer);
        }
    }
    public static void SetLayerRecursively(this GameObject theGameObject, string theLayer)
    {
        theGameObject.SetLayerRecursively(LayerMask.NameToLayer(theLayer));
    }

    static GladiusGlobals()
    {
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


    public static void AdjustV4(ref Vector4 input)
    {
        input = new Vector4(input.x, input.z, input.y,0);
    }



    public static void AdjustQuaternion(ref Quaternion q)
    {
        q = new Quaternion(q.x, q.z, q.y, q.w);
    }


    public static float XZDistance2(Vector3 a, Vector3 b)
    {
        Vector3 diff = b - a;
        diff.y = 0;
        return diff.sqrMagnitude;
    }

    public static bool XZReached(Vector3 a, Vector3 b)
    {
        return XZDistance2(a, b) < 0.05f;

    }

    // pretend tunelling check...
    public static bool XZReachedTunnel(Vector3 start, Vector3 end,Vector3 target)
    {
        float startToTarget = XZDistance2(start, target);
        float endToTarget = XZDistance2(end, target);
        //return (startToTarget <= endToTarget);
        return (endToTarget <= startToTarget);
    }



    public static String DataRoot = "gladiusdata/";
    public static String PropDataRoot = DataRoot + "propfiles/";
    public static String SchoolsPath = DataRoot + "schools/";
    public static String TownsPath = DataRoot + "towns/";
    public static String EncountersPath = DataRoot + "encounters/";
    public static String LeaguesPath = DataRoot + "leagues/";
    public static String ModelsRoot = "XBoxModelPrefabs/";
    public static String ItemsModelsRoot = ModelsRoot + "items/";
    public static String ArenaModelsRoot = ModelsRoot + "Arenas/";
    public static String MaterialsRoot = "Materials/";


    public static Color[] TeamColorMap = new Color[] { new Color(230f/255f, 1655/255f, 145f/255f, 1f), new Color(86f/255f, 71f / 255f, 198f / 255f, 1f), new Color(124f / 255f, 225f / 255f, 122f / 255f, 1f), new Color(81f / 255f, 152f / 255f, 240f / 255f, 1f) };

    public static Color DefaultTeamColor(int index)
    {
        if(index >=0 && index < TeamColorMap.Length)
        {
            return TeamColorMap[index];
        }
        return Color.black;
    }


    public static System.Random Random = new System.Random();

    public static int Random100()
    {
        return Random.Next(1, 100);
    }

    public static string GetFileName(String inputString)
    {
        if (inputString != null)
        {
            inputString = inputString.ToLower();
        }
        if (inputString.Contains("\\"))
        {
            inputString = inputString.Replace("\\", "/");
        }
        return inputString;
    }


    public static void SetSize(this RectTransform self, Vector2 size)
    {
        Vector2 oldSize = self.rect.size;
        Vector2 deltaSize = size - oldSize;

        self.offsetMin = self.offsetMin - new Vector2(
            deltaSize.x * self.pivot.x,
            deltaSize.y * self.pivot.y);
        self.offsetMax = self.offsetMax + new Vector2(
            deltaSize.x * (1f - self.pivot.x),
            deltaSize.y * (1f - self.pivot.y));
    }

    public static void SetWidth(this RectTransform self, float size)
    {
        self.SetSize(new Vector2(size, self.rect.size.y));
    }

    public static void SetHeight(this RectTransform self, float size)
    {
        self.SetSize(new Vector2(self.rect.size.x, size));
    }

    public static String ReadTextAsset(String filename)
    {
        String result = null;
        TextAsset textAsset = (TextAsset)Resources.Load(GladiusGlobals.GetFileName(filename));
        if (textAsset != null)
        {
            result = textAsset.text;
            if (String.IsNullOrEmpty(result))
            {
                result = System.Text.Encoding.Default.GetString(textAsset.bytes);
            }
        }
        if (textAsset == null)
        {
            //UnityEngine.Debug.LogWarning("Can't read textasset : " + filename);
        }

        return result;
    }

    
    public static Transform FuzzyFind(string name)
    {
        GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i].name.Contains(name))
            {
                return gos[i].transform;
            }
        }
        return null;
    }

    public static Transform FuzzyFindChild(this Transform t,string name)
    {
        if (t.name.Contains(name))
            return t;
        foreach (Transform child in t)
        {
            Transform ct = child.FuzzyFindChild(name);
            if (ct != null)
                return ct;
        }
        return null;
    }

    public static Vector3 Div(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
    }

    public static Vector3 ReadV3(String[] values, int startPos)
    {
        Vector3 result = new Vector3();
        //if (startPos+3 < values.Length)
        {
            float.TryParse(values[startPos], out result.x);
            float.TryParse(values[startPos + 1], out result.y);
            float.TryParse(values[startPos + 2], out result.z);
        }
        result = AdjustV3(result);
        return result;
    }

    public static Quaternion ReadQuaternion(String[] values, int startPos)
    {
        Quaternion result = new Quaternion();
        //if (startPos+4 < values.Length)
        {
            float.TryParse(values[startPos], out result.x);
            float.TryParse(values[startPos + 1], out result.y);
            float.TryParse(values[startPos + 2], out result.z);
            float.TryParse(values[startPos + 3], out result.w);
        }
        AdjustQuaternion(ref result);
        return result;
    }



    public static bool SafeGetValue(this XmlNode node, String path, ref string value)
    {
        bool success = false;
        XmlNode childNode = node.SelectSingleNode(path);
        if (childNode != null)
        {
            value = childNode.InnerText;
            success = true;
        }
        return success;
    }

    public static bool SafeGetValue(this XmlNode node, String path, ref int value)
    {
        bool success = false;
        XmlNode childNode = node.SelectSingleNode(path);
        if (childNode != null)
        {
            success = int.TryParse(childNode.InnerText, out value);
        }
        return success;
    }

    public static bool SafeGetValue(this XmlNode node, String path, ref float value)
    {
        bool success = false;
        XmlNode childNode = node.SelectSingleNode(path);
        if (childNode != null)
        {
            success = float.TryParse(childNode.InnerText, out value);
        }
        return success;
    }


    public static bool SafeGetValue(this XmlNode node, String path, ref Vector3 result)
    {
        float x = 0.0f;
        float y = 0.0f;
        float z = 0.0f;
        bool success = false;
        XmlNode childNode = node.SelectSingleNode(path);
        if (childNode != null)
        {
            if (childNode.SafeGetValue("@x", ref x) &&
                childNode.SafeGetValue("@y", ref y) &&
                childNode.SafeGetValue("@z", ref z))
            {
                result = new Vector3(x, y, z);
                success = true;
            }
        }
        return success;
    }


    public static Vector3 FromStreamVector3(BinaryReader reader)
    {
        Vector3 v = new Vector3();
        v.x = reader.ReadSingle();
        v.y = reader.ReadSingle();
        v.z = reader.ReadSingle();
        v = AdjustV3(v);
        return v;
    }



    public static String AdjustModelName(String modelName)
    {
        if (!modelName.StartsWith(GladiusGlobals.ModelsRoot))
        {
            modelName = GladiusGlobals.ModelsRoot + "/" + modelName;
        }

        if (!modelName.EndsWith(".mdl"))
        {
            modelName += ".mdl";
        }
        modelName = modelName.Replace("//", "/");

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


    public static float Max(this Vector3 v)
    {
        return v.x < v.y ? (v.y < v.z ? v.z : v.y) : (v.x < v.z ? v.z : v.x);
    }

    public static float Min(this Vector3 v)
    {
        return v.x < v.y ? (v.x < v.z ? v.x : v.z) : (v.y < v.z ? v.y : v.z);
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

    public static Vector3 Mult(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }


    // yucky way of passing data between scenes.
    public static List<CharacterData> ChosenCharacterList = new List<CharacterData>();




    public const int SkyBoxDrawOrder = 0;
    public const int ArenaDrawOrder = 1;
    public const int MoveGridDrawOrder = 2;
    public const int CharacterDrawOrder = 3;


    //public const String PlayerTeam = "Player";
    //public const String EnemyTeam1 = "Enemy1";
    //public const String EnemyTeam2 = "Enemy2";
    //public const String EnemyTeam3 = "Enemy3";

    static char[] trimChars = new char[] { '"', '\r', ' ', '\t' };

    //public static object Camera { get; internal set; }

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

    /// <summary>
    /// Performs a depth-first search of the transforms associated to the given transform, in search
    /// of a descendant with the given name.  Avoid using this method on a frame-by-frame basis, as
    /// it is recursive and quite capable of being slow!
    /// </summary>
    /// <param name="searchTransform">Transform to search within</param>
    /// <param name="descendantName">Name of the descendant to find</param>
    /// <returns>Descendant transform if found, otherwise null.</returns>
    /// 
    public static void DestroyChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
        transform.DetachChildren();
    }

    public static Transform FindDescendentTransform(this Transform searchTransform, string descendantName)
    {
        Transform result = null;

        int childCount = searchTransform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform childTransform = searchTransform.GetChild(i);

            // Not it, but has children? Search the children.
            if (childTransform.name != descendantName
               && childTransform.childCount > 0)
            {
                Transform grandchildTransform = FindDescendentTransform(childTransform, descendantName);
                if (grandchildTransform == null)
                    continue;

                result = grandchildTransform;
                break;
            }
            // Not it, but has no children?  Go on to the next sibling.
            else if (childTransform.name != descendantName
                    && childTransform.childCount == 0)
            {
                continue;
            }

            // Found it.
            result = childTransform;
            break;
        }

        return result;
    }

    public static void SetSingleChildEnabled(Transform t, bool active)
    {
        if (t != null && t.childCount == 1)
        {
            t.GetChild(0).gameObject.SetActive(active);
        }
    }

    public static void ToggleSingleChildVisible(Transform t)
    {
        if (t != null && t.childCount == 1)
        {
            MeshRenderer[] mra = t.GetChild(0).gameObject.GetComponentsInChildren<MeshRenderer>();
            if(mra != null)
            {
                foreach(MeshRenderer mr in mra)
                {
                    mr.enabled = !mr.enabled;
                }
            }

        }
    }


    public static void SetSingleChildVisible(Transform t, bool active)
    {
        if (t != null && t.childCount == 1)
        {
            t.GetChild(0).gameObject.SetActive(active);
        }
    }

    public static String CreateSpriteText(string sheetName, string spriteName, int numInstances)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < numInstances; ++i)
        {
            sb.AppendFormat("<sprite=\"{0}\" name=\"{1}\">", sheetName, spriteName);
        }
        return sb.ToString();

    }


    public static GameObject InstantiateModel(String modelName)
    {
        modelName = GladiusGlobals.AdjustModelName(modelName);

        UnityEngine.Object resourceToInstantiate = Resources.Load(modelName);
        if (resourceToInstantiate != null)
        {
            return UnityEngine.Object.Instantiate(resourceToInstantiate) as UnityEngine.GameObject;

        }
        UnityEngine.Debug.LogWarningFormat("Couldn't create [{0}]", modelName);
        return null;
    }


    public static bool IsAffinity(DamageType dt)
    {
        return dt == DamageType.Air || dt == DamageType.Dark || dt == DamageType.Earth || dt == DamageType.Fire || dt == DamageType.Light || dt == DamageType.Water;
    }


}

public static class GladiusJournal
{
    public static void Load()
    {
        TextAsset textAsset = (TextAsset)Resources.Load(GladiusGlobals.DataRoot + "Journal");
        String data = textAsset.text;

        var lexer = new GladiusJournalLexer(new Antlr4.Runtime.AntlrInputStream(data));
        CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
        var parser = new GladiusJournalParser(commonTokenStream);
        IParseTree parseTree = parser.root();
        GladiusJournalDataParser listener = new GladiusJournalDataParser();
        ParseTreeWalker.Default.Walk(listener, parseTree);
    }

    public static JournalEntry Get(string key)
    {
        JournalEntry result;
        EntryDictionary.TryGetValue(key, out result);
        return result;
    }

    public static Dictionary<string, JournalEntry> EntryDictionary = new Dictionary<string, JournalEntry>();
}

public class JournalEntry
{
    public string name;
    public string conversationFile;
    public string entryType;
    public int minDays;
    public int minLevel;
    public int ursulaChoice;
    public int valensChoice;
    public int journalTitle;
    public int journalText;
    public string location;
    public bool yank;
    public bool oneTime;
    public bool relativeDay;
    public int lastDay;
    public int type;

    public string encounter;

    public string success;
    public string failure;

    public List<string> shopFiles = new List<string>();
    public List<string> prizes = new List<string>();
    public List<string> items = new List<string>();

}


public class GladiusJournalDataParser : GladiusJournalBaseListener
{
    public String GetStringVal(ITerminalNode node)
    {
        return node.GetText().Replace("\"", "");
    }

    public int GetIntVal(ITerminalNode node)
    {
        int result = 0;
        if (!int.TryParse(node.GetText(), out result))
        {
            String text = node.GetText();
            int ibreak = 0;
        }
        return result;
    }

    public float GetFloatVal(ITerminalNode node)
    {
        return float.Parse(node.GetText());
    }


    public override void EnterRumour([NotNull] GladiusJournalParser.RumourContext context)
    {
        base.EnterRumour(context);
        CurrentEntry = new JournalEntry();
        CurrentEntry.name = GetStringVal(context.STRING());
        CurrentEntry.entryType = "rumour";
        GladiusJournal.EntryDictionary[CurrentEntry.name] = CurrentEntry;
    }

    public override void EnterGossip([NotNull] GladiusJournalParser.GossipContext context)
    {
        base.EnterGossip(context);
        CurrentEntry = new JournalEntry();
        CurrentEntry.name = GetStringVal(context.STRING());
        CurrentEntry.entryType = "gossip";
        GladiusJournal.EntryDictionary[CurrentEntry.name] = CurrentEntry;
    }

    public override void EnterQuest([NotNull] GladiusJournalParser.QuestContext context)
    {
        base.EnterQuest(context);
        CurrentEntry = new JournalEntry();
        CurrentEntry.name = GetStringVal(context.STRING());
        CurrentEntry.entryType = "quest";
        GladiusJournal.EntryDictionary[CurrentEntry.name] = CurrentEntry;
    }

    public override void EnterConversation([NotNull] GladiusJournalParser.ConversationContext context)
    {
        base.EnterConversation(context);
        CurrentEntry.conversationFile = GetStringVal(context.STRING());
    }

    public override void EnterMinDays([NotNull] GladiusJournalParser.MinDaysContext context)
    {
        base.EnterMinDays(context);
        CurrentEntry.minDays = GetIntVal(context.INT());
    }

    public override void EnterUrsulaChoice([NotNull] GladiusJournalParser.UrsulaChoiceContext context)
    {
        base.EnterUrsulaChoice(context);
        CurrentEntry.ursulaChoice = GetIntVal(context.INT());
    }

    public override void EnterValensChoice([NotNull] GladiusJournalParser.ValensChoiceContext context)
    {
        base.EnterValensChoice(context);
        CurrentEntry.valensChoice = GetIntVal(context.INT());
    }

    public override void EnterJournalTitle([NotNull] GladiusJournalParser.JournalTitleContext context)
    {
        base.EnterJournalTitle(context);
        CurrentEntry.journalTitle = GetIntVal(context.INT());
    }

    public override void EnterJournalText([NotNull] GladiusJournalParser.JournalTextContext context)
    {
        base.EnterJournalText(context);
        CurrentEntry.journalText = GetIntVal(context.INT());
    }

    public override void EnterShop([NotNull] GladiusJournalParser.ShopContext context)
    {
        base.EnterShop(context);
        CurrentEntry.shopFiles.Add(GetStringVal(context.STRING()));
    }

    public override void EnterFailure([NotNull] GladiusJournalParser.FailureContext context)
    {
        base.EnterFailure(context);
        CurrentEntry.failure = GetStringVal(context.STRING());
    }

    public override void EnterPrize([NotNull] GladiusJournalParser.PrizeContext context)
    {
        base.EnterPrize(context);
        CurrentEntry.prizes.Add(GetStringVal(context.STRING()));
    }

    public override void EnterItem([NotNull] GladiusJournalParser.ItemContext context)
    {
        base.EnterItem(context);
        CurrentEntry.items.Add(GetStringVal(context.STRING()));
    }

    public override void EnterLastDay([NotNull] GladiusJournalParser.LastDayContext context)
    {
        base.EnterLastDay(context);
        CurrentEntry.lastDay = GetIntVal(context.INT());
    }

    public override void EnterMinLevel([NotNull] GladiusJournalParser.MinLevelContext context)
    {
        base.EnterMinLevel(context);
        CurrentEntry.minLevel = GetIntVal(context.INT());
    }

    public override void EnterEncounter([NotNull] GladiusJournalParser.EncounterContext context)
    {
        base.EnterEncounter(context);
        CurrentEntry.encounter = GetStringVal(context.STRING());
    }

    public override void EnterLocation([NotNull] GladiusJournalParser.LocationContext context)
    {
        base.EnterLocation(context);
        CurrentEntry.location = GetStringVal(context.STRING());
    }

    public override void EnterOnetime([NotNull] GladiusJournalParser.OnetimeContext context)
    {
        base.EnterOnetime(context);
        CurrentEntry.oneTime = true;
    }

    public override void EnterRelativeDay([NotNull] GladiusJournalParser.RelativeDayContext context)
    {
        base.EnterRelativeDay(context);
        CurrentEntry.relativeDay = true;
    }

    public override void EnterSuccess([NotNull] GladiusJournalParser.SuccessContext context)
    {
        base.EnterSuccess(context);
        CurrentEntry.success = GetStringVal(context.STRING());
    }

    public override void EnterType([NotNull] GladiusJournalParser.TypeContext context)
    {
        base.EnterType(context);
        CurrentEntry.type = GetIntVal(context.INT());
    }

    public override void EnterYank([NotNull] GladiusJournalParser.YankContext context)
    {
        base.EnterYank(context);
        CurrentEntry.yank = true;
    }

    public JournalEntry CurrentEntry = null;

}




public class DebugUtils {[Conditional("DEBUG")] public static void Assert(bool condition) { if (!condition) throw new Exception(); } }


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


/// <summary>
/// Utility functions to handle exceptions thrown from coroutine and iterator functions
/// http://JacksonDunstan.com/articles/3718
/// </summary>
public static class CoroutineUtils
{
    /// <summary>
    /// Start a coroutine that might throw an exception. Call the callback with the exception if it
    /// does or null if it finishes without throwing an exception.
    /// </summary>
    /// <param name="monoBehaviour">MonoBehaviour to start the coroutine on</param>
    /// <param name="enumerator">Iterator function to run as the coroutine</param>
    /// <param name="done">Callback to call when the coroutine has thrown an exception or finished.
    /// The thrown exception or null is passed as the parameter.</param>
    /// <returns>The started coroutine</returns>
    public static Coroutine StartThrowingCoroutine(
        this MonoBehaviour monoBehaviour,
        IEnumerator enumerator,
        Action<Exception> done
    )
    {
        return monoBehaviour.StartCoroutine(RunThrowingIterator(enumerator, done));
    }

    /// <summary>
    /// Run an iterator function that might throw an exception. Call the callback with the exception
    /// if it does or null if it finishes without throwing an exception.
    /// </summary>
    /// <param name="enumerator">Iterator function to run</param>
    /// <param name="done">Callback to call when the iterator has thrown an exception or finished.
    /// The thrown exception or null is passed as the parameter.</param>
    /// <returns>An enumerator that runs the given enumerator</returns>
    public static IEnumerator RunThrowingIterator(
        IEnumerator enumerator,
        Action<Exception> done
    )
    {
        while (true)
        {
            object current;
            try
            {
                if (enumerator.MoveNext() == false)
                {
                    break;
                }
                current = enumerator.Current;
            }
            catch (Exception ex)
            {
                done(ex);
                yield break;
            }
            yield return current;
        }
        done(null);
    }
}



public class FileChunk
{
    public char[] ChunkName;
    public int ChunkSize;
    public int Skip;
    public int ChunkElements;

    public const int ChunkHeaderSize = 16;

    public static FileChunk FromStream(BinaryReader binReader)
    {
        FileChunk fileChunk = null;

        if (binReader.BaseStream.Position < binReader.BaseStream.Length - ChunkHeaderSize)
        {
            fileChunk = new FileChunk();
            fileChunk.ChunkName = binReader.ReadChars(4);
            fileChunk.ChunkSize = binReader.ReadInt32();
            fileChunk.Skip = binReader.ReadInt32();
            fileChunk.ChunkElements = binReader.ReadInt32();
        }
        return fileChunk;
    }

}


public class FixedSizedQueue<T> : Queue<T>
{
    private readonly object syncObject = new object();

    public int Size { get; private set; }

    public FixedSizedQueue(int size)
    {
        Size = size;
    }

    public new void Enqueue(T obj)
    {
        base.Enqueue(obj);
        lock (syncObject)
        {
            while (base.Count > Size)
            {
                T outObj = base.Dequeue();
            }
        }
    }
}
