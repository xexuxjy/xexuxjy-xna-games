using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Globalization;

using DICT = System.Collections.Generic.Dictionary<string, object>;
using System.IO;
using System.Text;

public class TextureAtlas
{
    public TextureAtlas()
    {
        Regions = new Dictionary<string, TextureRegion>();
    }

    public void BuildXML(String resourceName)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(resourceName);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        XmlElement atlasElement = doc.SelectSingleNode("/TextureAtlas") as XmlElement;
        int width = int.Parse(atlasElement.Attributes["width"].Value);
        int height = int.Parse(atlasElement.Attributes["height"].Value);
        //bool swapTopBottom = true   ;// atlasElement.HasAttribute("SwapTB");

        XmlNodeList nodes = doc.SelectNodes("//sprite");
        foreach (XmlNode spriteNode in nodes)
        {
            XmlElement sprite = spriteNode as XmlElement;
            TextureRegion textureRegion = new TextureRegion();
            int x = int.Parse(sprite.Attributes["x"].Value, CultureInfo.InvariantCulture);
            int y = int.Parse(sprite.Attributes["y"].Value, CultureInfo.InvariantCulture);
            int w = int.Parse(sprite.Attributes["w"].Value, CultureInfo.InvariantCulture);
            int h = int.Parse(sprite.Attributes["h"].Value, CultureInfo.InvariantCulture);

            int oX = !sprite.HasAttribute("oX") ? 0 : int.Parse(sprite.Attributes["oX"].Value, CultureInfo.InvariantCulture);
            int oY = !sprite.HasAttribute("oY") ? 0 : int.Parse(sprite.Attributes["oY"].Value, CultureInfo.InvariantCulture);
            int oW = !sprite.HasAttribute("oW") ? w : int.Parse(sprite.Attributes["oW"].Value, CultureInfo.InvariantCulture);
            int oH = !sprite.HasAttribute("oH") ? h : int.Parse(sprite.Attributes["oH"].Value, CultureInfo.InvariantCulture);


            //if (swapTopBottom)
            //{
            //    y = height - y;
            //    //h = -h;
            //}


            textureRegion.Bounds = new Rect(x, y, w, h);
            textureRegion.BoundsUV = new Rect(((float)x) / width, ((float)y) / height, ((float)x + w) / width, ((float)y + h) / height);
            textureRegion.BoundsUV2 = new Rect(((float)x) / width, ((float)y) / height, ((float)w) / width, ((float)h) / height);
            textureRegion.BoundsUV3 = new Rect(textureRegion.BoundsUV2.x, 1f - textureRegion.BoundsUV2.y, textureRegion.BoundsUV2.width, textureRegion.BoundsUV2.height);
            Rect r = textureRegion.BoundsUV;
            r.y = 1f - r.y;
            r.height = 1f - r.height;
            textureRegion.BoundsUV4 = r;


            textureRegion.Rotated = sprite.HasAttribute("r") && sprite.Attributes["r"].Value.Equals("y");

            textureRegion.OriginTopLeft = new Vector2(-oX, -oY);
            textureRegion.OriginCenter = new Vector2(((oW / 2.0f) - (oX)), ((oH / 2.0f) - (oY)));
            textureRegion.OriginBottomRight = new Vector2((oW - (oX)), (oH - (oY)));

            String key = sprite.Attributes["n"].Value;
            Regions[key] = textureRegion;
            //Debug.Log(String.Format("TR {0} x{1:0.000} y{2:0.000} w{3:0.000} h{4:0.000}",key,textureRegion.BoundsUV.x,textureRegion.BoundsUV.y,textureRegion.BoundsUV.width,textureRegion.BoundsUV.height));

            //Debug.Log("Adding region : " + sprite.Attributes["n"].Value);
        }
    }

    public void BuildJSON(String resourceName,Texture2D texture)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(resourceName);
        var uvx = 1f / texture.width;
        var uvy = 1f / texture.height;


        var data = JSON.JsonDecode(textAsset.text) as DICT;
        var frames = data["frames"] as DICT;
        foreach (var key in frames.Keys)
        {

            var itemData = frames[key] as DICT;

            var isRotated = (bool)itemData["rotated"];
            if (isRotated)
            {
                Debug.LogError(string.Format("Sprite '{0}' is rotated. Rotated sprites are not yet supported", key));
                continue;
            }

            var frameRect = extractUVRect(itemData["frame"] as DICT, texture);
            var spriteSize = new Vector2(frameRect.width / uvx, frameRect.height / uvy);

            var sprite = new dfAtlas.ItemInfo()
            {
                name = key,
                border = new RectOffset(),
                deleted = false,
                region = frameRect,
                rotated = false,
                sizeInPixels = spriteSize
            };

            TextureRegion textureRegion = new TextureRegion();
            textureRegion.BoundsUV3 = frameRect;
            textureRegion.BoundsUV4 = frameRect;
            Regions[key] = textureRegion;
        }

    }


    internal TextureAtlas(Dictionary<string, TextureRegion> regions)
    {
        Regions = regions;
    }

    public bool ContainsTexture(string textureName)
    {
        return Regions.ContainsKey(textureName);
    }

    public TextureRegion GetRegion(string textureName)
    {
        TextureRegion region;

        if (Regions.TryGetValue(textureName, out region))
            return region;
        return null;
    }

    private static Rect extractUVRect(DICT data, Texture2D atlas)
    {

        var uvx = 1f / (float)atlas.width;
        var uvy = 1f / (float)atlas.height;

        var w = (float)(double)data["w"];
        var h = (float)(double)data["h"];
        var x = (float)(double)data["x"];
        var y = (float)(double)data["y"] + h;

        return new Rect(x * uvx, 1f - y * uvy, w * uvx, h * uvy);

    }

    #region Nested classes

    // JSON class modified from the source at http://techblog.procurios.nl/k/618/news/view/14605/14863/how-do-i-write-my-own-parser-(for-json).html
    // Removed serialization, changed List<object> and Dictionary data types 

    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    ///
    /// JSON uses Arrays and Objects. These correspond here to the datatypes List<object> and DICT.
    /// All numbers are parsed to doubles.
    /// </summary>
    private class JSON
    {

        public const int TOKEN_NONE = 0;
        public const int TOKEN_CURLY_OPEN = 1;
        public const int TOKEN_CURLY_CLOSE = 2;
        public const int TOKEN_SQUARED_OPEN = 3;
        public const int TOKEN_SQUARED_CLOSE = 4;
        public const int TOKEN_COLON = 5;
        public const int TOKEN_COMMA = 6;
        public const int TOKEN_STRING = 7;
        public const int TOKEN_NUMBER = 8;
        public const int TOKEN_TRUE = 9;
        public const int TOKEN_FALSE = 10;
        public const int TOKEN_NULL = 11;

        private const int BUILDER_CAPACITY = 2000;

        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An List<object>, a DICT, a double, a string, null, true, or false</returns>
        public static object JsonDecode(string json)
        {
            bool success = true;

            return JsonDecode(json, ref success);
        }

        /// <summary>
        /// Parses the string json into a value; and fills 'success' with the successfullness of the parse.
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <param name="success">Successful parse?</param>
        /// <returns>An List<object>, a DICT, a double, a string, null, true, or false</returns>
        public static object JsonDecode(string json, ref bool success)
        {
            success = true;
            if (json != null)
            {
                char[] charArray = json.ToCharArray();
                int index = 0;
                object value = ParseValue(charArray, ref index, ref success);
                return value;
            }
            else
            {
                return null;
            }
        }

        protected static DICT ParseObject(char[] json, ref int index, ref bool success)
        {

            DICT table = new DICT();
            int token;

            // {
            NextToken(json, ref index);

            bool done = false;
            while (!done)
            {
                token = LookAhead(json, index);
                if (token == JSON.TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                else if (token == JSON.TOKEN_COMMA)
                {
                    NextToken(json, ref index);
                }
                else if (token == JSON.TOKEN_CURLY_CLOSE)
                {
                    NextToken(json, ref index);
                    return table;
                }
                else
                {

                    // name
                    string name = ParseString(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    // :
                    token = NextToken(json, ref index);
                    if (token != JSON.TOKEN_COLON)
                    {
                        success = false;
                        return null;
                    }

                    // value
                    object value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    table[name] = value;
                }
            }

            return table;
        }

        protected static List<object> ParseArray(char[] json, ref int index, ref bool success)
        {
            List<object> array = new List<object>();

            // [
            NextToken(json, ref index);

            bool done = false;
            while (!done)
            {
                int token = LookAhead(json, index);
                if (token == JSON.TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                else if (token == JSON.TOKEN_COMMA)
                {
                    NextToken(json, ref index);
                }
                else if (token == JSON.TOKEN_SQUARED_CLOSE)
                {
                    NextToken(json, ref index);
                    break;
                }
                else
                {
                    object value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        return null;
                    }

                    array.Add(value);
                }
            }

            return array;
        }

        protected static object ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case JSON.TOKEN_STRING:
                    return ParseString(json, ref index, ref success);
                case JSON.TOKEN_NUMBER:
                    return ParseNumber(json, ref index, ref success);
                case JSON.TOKEN_CURLY_OPEN:
                    return ParseObject(json, ref index, ref success);
                case JSON.TOKEN_SQUARED_OPEN:
                    return ParseArray(json, ref index, ref success);
                case JSON.TOKEN_TRUE:
                    NextToken(json, ref index);
                    return true;
                case JSON.TOKEN_FALSE:
                    NextToken(json, ref index);
                    return false;
                case JSON.TOKEN_NULL:
                    NextToken(json, ref index);
                    return null;
                case JSON.TOKEN_NONE:
                    break;
            }

            success = false;
            return null;
        }

        protected static string ParseString(char[] json, ref int index, ref bool success)
        {
            StringBuilder s = new StringBuilder(BUILDER_CAPACITY);
            char c;

            EatWhitespace(json, ref index);

            // "
            c = json[index++];

            bool complete = false;
            while (!complete)
            {

                if (index == json.Length)
                {
                    break;
                }

                c = json[index++];
                if (c == '"')
                {
                    complete = true;
                    break;
                }
                else if (c == '\\')
                {

                    if (index == json.Length)
                    {
                        break;
                    }
                    c = json[index++];
                    if (c == '"')
                    {
                        s.Append('"');
                    }
                    else if (c == '\\')
                    {
                        s.Append('\\');
                    }
                    else if (c == '/')
                    {
                        s.Append('/');
                    }
                    else if (c == 'b')
                    {
                        s.Append('\b');
                    }
                    else if (c == 'f')
                    {
                        s.Append('\f');
                    }
                    else if (c == 'n')
                    {
                        s.Append('\n');
                    }
                    else if (c == 'r')
                    {
                        s.Append('\r');
                    }
                    else if (c == 't')
                    {
                        s.Append('\t');
                    }
                    else if (c == 'u')
                    {
                        int remainingLength = json.Length - index;
                        if (remainingLength >= 4)
                        {
                            // parse the 32 bit hex into an integer codepoint
                            uint codePoint;
                            if (!(success = UInt32.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)))
                            {
                                return "";
                            }
                            // convert the integer codepoint to a unicode char and add to string
                            s.Append(Char.ConvertFromUtf32((int)codePoint));
                            // skip 4 chars
                            index += 4;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                else
                {
                    s.Append(c);
                }

            }

            if (!complete)
            {
                success = false;
                return null;
            }

            return s.ToString();
        }

        protected static double ParseNumber(char[] json, ref int index, ref bool success)
        {
            EatWhitespace(json, ref index);

            int lastIndex = GetLastIndexOfNumber(json, index);
            int charLength = (lastIndex - index) + 1;

            double number;
            success = Double.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);

            index = lastIndex + 1;
            return number;
        }

        protected static int GetLastIndexOfNumber(char[] json, int index)
        {
            int lastIndex;

            for (lastIndex = index; lastIndex < json.Length; lastIndex++)
            {
                if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
                {
                    break;
                }
            }
            return lastIndex - 1;
        }

        protected static void EatWhitespace(char[] json, ref int index)
        {
            for (; index < json.Length; index++)
            {
                if (" \t\n\r".IndexOf(json[index]) == -1)
                {
                    break;
                }
            }
        }

        protected static int LookAhead(char[] json, int index)
        {
            int saveIndex = index;
            return NextToken(json, ref saveIndex);
        }

        protected static int NextToken(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);

            if (index == json.Length)
            {
                return JSON.TOKEN_NONE;
            }

            char c = json[index];
            index++;
            switch (c)
            {
                case '{':
                    return JSON.TOKEN_CURLY_OPEN;
                case '}':
                    return JSON.TOKEN_CURLY_CLOSE;
                case '[':
                    return JSON.TOKEN_SQUARED_OPEN;
                case ']':
                    return JSON.TOKEN_SQUARED_CLOSE;
                case ',':
                    return JSON.TOKEN_COMMA;
                case '"':
                    return JSON.TOKEN_STRING;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return JSON.TOKEN_NUMBER;
                case ':':
                    return JSON.TOKEN_COLON;
            }
            index--;

            int remainingLength = json.Length - index;

            // false
            if (remainingLength >= 5)
            {
                if (json[index] == 'f' &&
                    json[index + 1] == 'a' &&
                    json[index + 2] == 'l' &&
                    json[index + 3] == 's' &&
                    json[index + 4] == 'e')
                {
                    index += 5;
                    return JSON.TOKEN_FALSE;
                }
            }

            // true
            if (remainingLength >= 4)
            {
                if (json[index] == 't' &&
                    json[index + 1] == 'r' &&
                    json[index + 2] == 'u' &&
                    json[index + 3] == 'e')
                {
                    index += 4;
                    return JSON.TOKEN_TRUE;
                }
            }

            // null
            if (remainingLength >= 4)
            {
                if (json[index] == 'n' &&
                    json[index + 1] == 'u' &&
                    json[index + 2] == 'l' &&
                    json[index + 3] == 'l')
                {
                    index += 4;
                    return JSON.TOKEN_NULL;
                }
            }

            return JSON.TOKEN_NONE;
        }

    }

    #endregion


    Dictionary<string, TextureRegion> Regions { get; set; }



}



public class TextureRegion
{
    public Rect Bounds { get; set; }
    public Rect BoundsUV { get; set; }
    public Rect BoundsUV2 { get; set; }
    public Rect BoundsUV3 { get; set; }
    public Rect BoundsUV4 { get; set; }
    public Vector2 OriginTopLeft { get; set; }
    public Vector2 OriginCenter { get; set; }
    public Vector2 OriginBottomRight { get; set; }
    public bool Rotated { get; set; }
}


