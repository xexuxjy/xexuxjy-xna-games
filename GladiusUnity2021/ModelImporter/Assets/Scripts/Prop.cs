using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Antlr4.Runtime.Misc;

public class Prop
{
    public String Name;
    public String Model;
    public bool Lighting;
    public bool Shadow;
    public float Height;
    public Vector3 Position;
    public Vector3 Rotation;
    public String Script;
    public GameObject GameObject;
    public Bounds Bounds;
    public Point[] ArenaPoints;
    public List<string> Attributes = new List<string>();
}

public static class PropLoader
{

    public static List<Prop> Load(String data)
    {
        try
        {
            //TextAsset textAsset = (TextAsset)Resources.Load(GladiusGlobals.DataRoot + "ItemData");
            //var lexer = new GladiusItemLexer(new Antlr4.Runtime.AntlrInputStream(textAsset.text));
            var lexer = new GladiusPropsLexer(new Antlr4.Runtime.AntlrInputStream(data));
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            var parser = new GladiusPropsParser(commonTokenStream);
            IParseTree parseTree = parser.root();
            MyGladiusPropsParser listener = new MyGladiusPropsParser();
            ParseTreeWalker.Default.Walk(listener, parseTree);
            return listener.AllProps;
        }
        catch (Exception e)
        {
            int ibreak = 0;
        }
        return null;
    }


}


public class MyGladiusPropsParser : GladiusPropsBaseListener
{
    public override void EnterRoot([NotNull] GladiusPropsParser.RootContext context)
    {
        base.EnterRoot(context);
    }
    public override void EnterPropname([NotNull] GladiusPropsParser.PropnameContext context)
    {
        base.EnterPropname(context);
        currentProp = new Prop();
        AllProps.Add(currentProp);

        currentProp.Name = context.QUOTEDSTRING().GetStringVal();
    }

    public override void EnterModels([NotNull] GladiusPropsParser.ModelsContext context)
    {
        base.EnterModels(context);
        if (context.QUOTEDSTRING() != null)
        {
            currentProp.Model = context.QUOTEDSTRING().GetStringVal();
        }
    }

    public override void EnterLighting([NotNull] GladiusPropsParser.LightingContext context)
    {
        base.EnterLighting(context);
     //   currentProp.Lighting = context.onoff().GetText() == "ON";
    }

    public override void EnterShadow([NotNull] GladiusPropsParser.ShadowContext context)
    {
        base.EnterShadow(context);
       // currentProp.Shadow = context.onoff().GetText() == "ON";
    }

    public override void EnterPos([NotNull] GladiusPropsParser.PosContext context)
    {
        base.EnterPos(context);
        float x = context.FLOAT()[0].GetFloatVal();
        float y = context.FLOAT()[1].GetFloatVal();
        float z = context.FLOAT()[2].GetFloatVal();

        currentProp.Position = new Vector3(x, y, z);

    }

    public override void EnterRot([NotNull] GladiusPropsParser.RotContext context)
    {
        base.EnterRot(context);
        float x = context.FLOAT()[0].GetFloatVal();
        float y = context.FLOAT()[1].GetFloatVal();
        float z = context.FLOAT()[2].GetFloatVal();

        currentProp.Rotation = new Vector3(x, y, z);
    }

    public override void EnterAttribute([NotNull] GladiusPropsParser.AttributeContext context)
    {
        base.EnterAttribute(context);
        currentProp.Attributes.Add(context.QUOTEDSTRING().GetStringVal());
    }

    Prop currentProp = null;
    public List<Prop> AllProps = new List<Prop>();
}
