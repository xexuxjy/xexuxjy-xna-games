using System.Collections.Generic;
using System;
using System.Xml;
using System.IO;
using System.Diagnostics;
using UnityEngine;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Misc;

public static class ItemManager
{
    public static void Load()
    {
        try
        {
            String data = GladiusGlobals.ReadTextAsset(GladiusGlobals.DataRoot + "ItemData");
            var lexer = new GladiusItemLexer(new Antlr4.Runtime.AntlrInputStream(data));
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            var parser = new GladiusItemParser(commonTokenStream);
            IParseTree parseTree = parser.root();
            MyGladiusItemParser listener = new MyGladiusItemParser();
            ParseTreeWalker.Default.Walk(listener, parseTree);
            foreach (Item i in listener.Items)
            {
                Items[i.Name] = i;
            }
            
        }
        catch (Exception e)
        {
            int ibreak = 0;
        }
    }


    public static Dictionary<String, Item> Items = new Dictionary<string, Item>();
}

public class Item
{

    public String Name;

    public ItemLocation ItemLocation;

    public String ItemSubType;

    public String ClassInfo;

    public DamageType AffinityType = DamageType.None;
    public int AffinityValue;
    public int DescriptionId;

    public int DisplayNameId;


    public int Cost;

    public int MinLevel;

    public String Rarity;

    public String Region;

    public String MeshName;
    public String MeshName2;

    public String MaterialName;

    public int GetStatValue(string statName)
    {
        foreach (ItemStat itemStat in ItemStats)
        {
            if (itemStat.Stat == statName)
            {
                return itemStat.Value;
            }
        }
        return 0;

    }

    public List<string> Attributes = new List<string>();
    public List<ItemStat> ItemStats = new List<ItemStat>();
    public List<string> ItemSkills = new List<string>();
}

public class ItemStat
{
    public string Stat;
    public int Value;

    public ItemStat(string name, int value)
    {
        Stat = name;
        if (Stat == "accuracy")
        {
            Stat = "ACC";
        }
        else if (Stat == "defense")
        {
            Stat = "DEF";
        }

        Value = value;
    }
}


public class MyGladiusItemParser : GladiusItemBaseListener
{
    public List<Item> Items = new List<Item>();
    Item currentItem;

    public override void EnterItemCreate([NotNull] GladiusItemParser.ItemCreateContext context)
    {
        base.EnterItemCreate(context);
        currentItem = new Item();
        Items.Add(currentItem);
        currentItem.Name = context.STRING(0).GetStringVal();
        currentItem.ItemLocation = (ItemLocation)Enum.Parse(typeof(ItemLocation),context.STRING(1).GetStringVal());
        currentItem.ItemSubType = context.STRING(2).GetStringVal();
        currentItem.ClassInfo = context.STRING(3).GetStringVal();
        // missing a 'rating' value on the end here?
    }

    public override void EnterItemDescriptionId([NotNull] GladiusItemParser.ItemDescriptionIdContext context)
    {
        base.EnterItemDescriptionId(context);
        currentItem.DescriptionId = context.INT().GetIntVal();
    }

    public override void EnterItemDisplayNameId([NotNull] GladiusItemParser.ItemDisplayNameIdContext context)
    {
        base.EnterItemDisplayNameId(context);
        currentItem.DisplayNameId = context.INT().GetIntVal();
    }

    public override void EnterItemCost([NotNull] GladiusItemParser.ItemCostContext context)
    {
        base.EnterItemCost(context);
        currentItem.Cost = context.INT().GetIntVal();
    }

    public override void EnterItemMinLevel([NotNull] GladiusItemParser.ItemMinLevelContext context)
    {
        base.EnterItemMinLevel(context);
        currentItem.MinLevel = context.INT().GetIntVal();
    }

    public override void EnterItemRarity([NotNull] GladiusItemParser.ItemRarityContext context)
    {
        base.EnterItemRarity(context);
        currentItem.Rarity = context.STRING().GetStringVal();
    }

    public override void EnterItemRegion([NotNull] GladiusItemParser.ItemRegionContext context)
    {
        base.EnterItemRegion(context);
        currentItem.Region = context.STRING().GetStringVal();
    }

    public override void EnterItemMesh([NotNull] GladiusItemParser.ItemMeshContext context)
    {
        base.EnterItemMesh(context);
        string meshName = context.STRING().GetStringVal();
        if (String.IsNullOrEmpty(currentItem.MeshName))
        {
            currentItem.MeshName = meshName.Replace('\\', '/');
            currentItem.MeshName = GladiusGlobals.AdjustModelName(currentItem.MeshName);
        }
        else
        {
            currentItem.MeshName2 = meshName.Replace('\\', '/');
            currentItem.MeshName2 = GladiusGlobals.AdjustModelName(currentItem.MeshName);

        }
    }

    //public override void EnterItemMesh2([NotNull] GladiusItemParser.ItemMesh2Context context)
    //{
    //    base.EnterItemMesh2(context);
    //    string meshName = context.STRING().GetStringVal();
    //    currentItem.MeshName2 = meshName.Replace('\\', '/');
    //    currentItem.MeshName2 = GladiusGlobals.AdjustModelName(currentItem.MeshName2);
    //}

    public override void EnterItemMaterial([NotNull] GladiusItemParser.ItemMaterialContext context)
    {
        base.EnterItemMaterial(context);
        currentItem.MaterialName = context.STRING().GetStringVal();
    }

    public override void EnterItemAffinity([NotNull] GladiusItemParser.ItemAffinityContext context)
    {
        base.EnterItemAffinity(context);
        currentItem.AffinityType = (DamageType)Enum.Parse(typeof(DamageType),context.STRING().GetStringVal());
        currentItem.AffinityValue = context.INT().GetIntVal();
    }

    public override void EnterItemStatMod([NotNull] GladiusItemParser.ItemStatModContext context)
    {
        base.EnterItemStatMod(context);
        String name = context.STRING().GetStringVal();
        int value = context.INT().GetIntVal();
        ItemStat itemStat = new ItemStat(name, value);
        currentItem.ItemStats.Add(itemStat);

    }

    public override void EnterItemSkill([NotNull] GladiusItemParser.ItemSkillContext context)
    {
        String name = context.STRING().GetStringVal();
        currentItem.ItemSkills.Add(name);
    }

}


