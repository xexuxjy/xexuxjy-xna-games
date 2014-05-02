using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Globalization;

public class TextureAtlas
{
    public TextureAtlas(String resourceName)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(resourceName);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        XmlElement atlasElement = doc.SelectSingleNode("/TextureAtlas") as XmlElement;
        int width = int.Parse(atlasElement.Attributes["width"].Value);
        int height = int.Parse(atlasElement.Attributes["height"].Value);
        //bool swapTopBottom = true   ;// atlasElement.HasAttribute("SwapTB");
        Regions = new Dictionary<string, TextureRegion>();

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
            textureRegion.BoundsUV = new Rect(((float)x)/width,((float)y)/height,((float)x+w)/width,((float)y+h)/height);
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


