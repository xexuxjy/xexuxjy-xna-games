//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from GladiusItem.g4 by ANTLR 4.9.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="GladiusItemParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.CLSCompliant(false)]
public interface IGladiusItemListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRoot([NotNull] GladiusItemParser.RootContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRoot([NotNull] GladiusItemParser.RootContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.numEntries"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumEntries([NotNull] GladiusItemParser.NumEntriesContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.numEntries"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumEntries([NotNull] GladiusItemParser.NumEntriesContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.item"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItem([NotNull] GladiusItemParser.ItemContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.item"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItem([NotNull] GladiusItemParser.ItemContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemCreate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemCreate([NotNull] GladiusItemParser.ItemCreateContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemCreate"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemCreate([NotNull] GladiusItemParser.ItemCreateContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemDescriptionId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemDescriptionId([NotNull] GladiusItemParser.ItemDescriptionIdContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemDescriptionId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemDescriptionId([NotNull] GladiusItemParser.ItemDescriptionIdContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemDisplayNameId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemDisplayNameId([NotNull] GladiusItemParser.ItemDisplayNameIdContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemDisplayNameId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemDisplayNameId([NotNull] GladiusItemParser.ItemDisplayNameIdContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemCost"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemCost([NotNull] GladiusItemParser.ItemCostContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemCost"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemCost([NotNull] GladiusItemParser.ItemCostContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemMinLevel"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemMinLevel([NotNull] GladiusItemParser.ItemMinLevelContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemMinLevel"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemMinLevel([NotNull] GladiusItemParser.ItemMinLevelContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemRarity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemRarity([NotNull] GladiusItemParser.ItemRarityContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemRarity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemRarity([NotNull] GladiusItemParser.ItemRarityContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemRegion"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemRegion([NotNull] GladiusItemParser.ItemRegionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemRegion"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemRegion([NotNull] GladiusItemParser.ItemRegionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemHideSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemHideSet([NotNull] GladiusItemParser.ItemHideSetContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemHideSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemHideSet([NotNull] GladiusItemParser.ItemHideSetContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemShowSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemShowSet([NotNull] GladiusItemParser.ItemShowSetContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemShowSet"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemShowSet([NotNull] GladiusItemParser.ItemShowSetContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemMesh"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemMesh([NotNull] GladiusItemParser.ItemMeshContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemMesh"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemMesh([NotNull] GladiusItemParser.ItemMeshContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemMaterial"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemMaterial([NotNull] GladiusItemParser.ItemMaterialContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemMaterial"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemMaterial([NotNull] GladiusItemParser.ItemMaterialContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemSkill"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemSkill([NotNull] GladiusItemParser.ItemSkillContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemSkill"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemSkill([NotNull] GladiusItemParser.ItemSkillContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemAffinity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemAffinity([NotNull] GladiusItemParser.ItemAffinityContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemAffinity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemAffinity([NotNull] GladiusItemParser.ItemAffinityContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemStatMod"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemStatMod([NotNull] GladiusItemParser.ItemStatModContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemStatMod"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemStatMod([NotNull] GladiusItemParser.ItemStatModContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusItemParser.itemAttribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterItemAttribute([NotNull] GladiusItemParser.ItemAttributeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusItemParser.itemAttribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitItemAttribute([NotNull] GladiusItemParser.ItemAttributeContext context);
}