//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from AttackSkill.g4 by ANTLR 4.9.2

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
/// <see cref="AttackSkillParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.CLSCompliant(false)]
public interface IAttackSkillListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRoot([NotNull] AttackSkillParser.RootContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRoot([NotNull] AttackSkillParser.RootContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.numEntries"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNumEntries([NotNull] AttackSkillParser.NumEntriesContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.numEntries"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNumEntries([NotNull] AttackSkillParser.NumEntriesContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.create"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCreate([NotNull] AttackSkillParser.CreateContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.create"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCreate([NotNull] AttackSkillParser.CreateContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.skillProperty"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSkillProperty([NotNull] AttackSkillParser.SkillPropertyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.skillProperty"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSkillProperty([NotNull] AttackSkillParser.SkillPropertyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.animBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnimBlock([NotNull] AttackSkillParser.AnimBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.animBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnimBlock([NotNull] AttackSkillParser.AnimBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.fxBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFxBlock([NotNull] AttackSkillParser.FxBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.fxBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFxBlock([NotNull] AttackSkillParser.FxBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.effectBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEffectBlock([NotNull] AttackSkillParser.EffectBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.effectBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEffectBlock([NotNull] AttackSkillParser.EffectBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.statusBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatusBlock([NotNull] AttackSkillParser.StatusBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.statusBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatusBlock([NotNull] AttackSkillParser.StatusBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.projectileBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProjectileBlock([NotNull] AttackSkillParser.ProjectileBlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.projectileBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProjectileBlock([NotNull] AttackSkillParser.ProjectileBlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.createDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCreateDef([NotNull] AttackSkillParser.CreateDefContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.createDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCreateDef([NotNull] AttackSkillParser.CreateDefContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.useClass"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUseClass([NotNull] AttackSkillParser.UseClassContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.useClass"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUseClass([NotNull] AttackSkillParser.UseClassContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.displayNameId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDisplayNameId([NotNull] AttackSkillParser.DisplayNameIdContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.displayNameId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDisplayNameId([NotNull] AttackSkillParser.DisplayNameIdContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.descriptionId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDescriptionId([NotNull] AttackSkillParser.DescriptionIdContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.descriptionId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDescriptionId([NotNull] AttackSkillParser.DescriptionIdContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.level"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLevel([NotNull] AttackSkillParser.LevelContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.level"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLevel([NotNull] AttackSkillParser.LevelContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.jobPointCost"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterJobPointCost([NotNull] AttackSkillParser.JobPointCostContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.jobPointCost"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitJobPointCost([NotNull] AttackSkillParser.JobPointCostContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.prereq"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrereq([NotNull] AttackSkillParser.PrereqContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.prereq"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrereq([NotNull] AttackSkillParser.PrereqContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAttribute([NotNull] AttackSkillParser.AttributeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAttribute([NotNull] AttackSkillParser.AttributeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.costs"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCosts([NotNull] AttackSkillParser.CostsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.costs"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCosts([NotNull] AttackSkillParser.CostsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.shiftData"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterShiftData([NotNull] AttackSkillParser.ShiftDataContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.shiftData"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitShiftData([NotNull] AttackSkillParser.ShiftDataContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.affCost"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAffCost([NotNull] AttackSkillParser.AffCostContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.affCost"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAffCost([NotNull] AttackSkillParser.AffCostContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.effect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEffect([NotNull] AttackSkillParser.EffectContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.effect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEffect([NotNull] AttackSkillParser.EffectContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.effectFX"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEffectFX([NotNull] AttackSkillParser.EffectFXContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.effectFX"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEffectFX([NotNull] AttackSkillParser.EffectFXContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.splitEffect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSplitEffect([NotNull] AttackSkillParser.SplitEffectContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.splitEffect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSplitEffect([NotNull] AttackSkillParser.SplitEffectContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.splitEffectCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSplitEffectCondition([NotNull] AttackSkillParser.SplitEffectConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.splitEffectCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSplitEffectCondition([NotNull] AttackSkillParser.SplitEffectConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.effectSkillCond"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEffectSkillCond([NotNull] AttackSkillParser.EffectSkillCondContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.effectSkillCond"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEffectSkillCond([NotNull] AttackSkillParser.EffectSkillCondContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.combatMods"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCombatMods([NotNull] AttackSkillParser.CombatModsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.combatMods"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCombatMods([NotNull] AttackSkillParser.CombatModsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.moveToAttackMod"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMoveToAttackMod([NotNull] AttackSkillParser.MoveToAttackModContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.moveToAttackMod"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMoveToAttackMod([NotNull] AttackSkillParser.MoveToAttackModContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.affinity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAffinity([NotNull] AttackSkillParser.AffinityContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.affinity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAffinity([NotNull] AttackSkillParser.AffinityContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.range"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRange([NotNull] AttackSkillParser.RangeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.range"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRange([NotNull] AttackSkillParser.RangeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.excludeRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExcludeRange([NotNull] AttackSkillParser.ExcludeRangeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.excludeRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExcludeRange([NotNull] AttackSkillParser.ExcludeRangeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.meter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMeter([NotNull] AttackSkillParser.MeterContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.meter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMeter([NotNull] AttackSkillParser.MeterContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.anim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnim([NotNull] AttackSkillParser.AnimContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.anim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnim([NotNull] AttackSkillParser.AnimContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.loopAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLoopAnim([NotNull] AttackSkillParser.LoopAnimContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.loopAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLoopAnim([NotNull] AttackSkillParser.LoopAnimContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.moveAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMoveAnim([NotNull] AttackSkillParser.MoveAnimContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.moveAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMoveAnim([NotNull] AttackSkillParser.MoveAnimContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.defendAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDefendAnim([NotNull] AttackSkillParser.DefendAnimContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.defendAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDefendAnim([NotNull] AttackSkillParser.DefendAnimContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.lowAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLowAnim([NotNull] AttackSkillParser.LowAnimContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.lowAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLowAnim([NotNull] AttackSkillParser.LowAnimContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.chargeAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterChargeAnim([NotNull] AttackSkillParser.ChargeAnimContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.chargeAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitChargeAnim([NotNull] AttackSkillParser.ChargeAnimContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.animTime"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnimTime([NotNull] AttackSkillParser.AnimTimeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.animTime"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnimTime([NotNull] AttackSkillParser.AnimTimeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.animStartFrame"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnimStartFrame([NotNull] AttackSkillParser.AnimStartFrameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.animStartFrame"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnimStartFrame([NotNull] AttackSkillParser.AnimStartFrameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.fx"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFx([NotNull] AttackSkillParser.FxContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.fx"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFx([NotNull] AttackSkillParser.FxContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.fxSwing"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFxSwing([NotNull] AttackSkillParser.FxSwingContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.fxSwing"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFxSwing([NotNull] AttackSkillParser.FxSwingContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.fxCTAG"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFxCTAG([NotNull] AttackSkillParser.FxCTAGContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.fxCTAG"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFxCTAG([NotNull] AttackSkillParser.FxCTAGContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.fxProjectile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFxProjectile([NotNull] AttackSkillParser.FxProjectileContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.fxProjectile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFxProjectile([NotNull] AttackSkillParser.FxProjectileContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.projectile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProjectile([NotNull] AttackSkillParser.ProjectileContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.projectile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProjectile([NotNull] AttackSkillParser.ProjectileContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.projectileSequence"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProjectileSequence([NotNull] AttackSkillParser.ProjectileSequenceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.projectileSequence"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProjectileSequence([NotNull] AttackSkillParser.ProjectileSequenceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.projectileRotation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProjectileRotation([NotNull] AttackSkillParser.ProjectileRotationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.projectileRotation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProjectileRotation([NotNull] AttackSkillParser.ProjectileRotationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.projectileAttr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProjectileAttr([NotNull] AttackSkillParser.ProjectileAttrContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.projectileAttr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProjectileAttr([NotNull] AttackSkillParser.ProjectileAttrContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.status"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatus([NotNull] AttackSkillParser.StatusContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.status"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatus([NotNull] AttackSkillParser.StatusContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.statusDuration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatusDuration([NotNull] AttackSkillParser.StatusDurationContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.statusDuration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatusDuration([NotNull] AttackSkillParser.StatusDurationContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.statusTarget"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatusTarget([NotNull] AttackSkillParser.StatusTargetContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.statusTarget"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatusTarget([NotNull] AttackSkillParser.StatusTargetContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.statusSituationAffinityCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatusSituationAffinityCondition([NotNull] AttackSkillParser.StatusSituationAffinityConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.statusSituationAffinityCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatusSituationAffinityCondition([NotNull] AttackSkillParser.StatusSituationAffinityConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.statusChance"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatusChance([NotNull] AttackSkillParser.StatusChanceContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.statusChance"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatusChance([NotNull] AttackSkillParser.StatusChanceContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.effectStatusCond"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEffectStatusCond([NotNull] AttackSkillParser.EffectStatusCondContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.effectStatusCond"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEffectStatusCond([NotNull] AttackSkillParser.EffectStatusCondContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.statusInterval"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatusInterval([NotNull] AttackSkillParser.StatusIntervalContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.statusInterval"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatusInterval([NotNull] AttackSkillParser.StatusIntervalContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.statusCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatusCondition([NotNull] AttackSkillParser.StatusConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.statusCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatusCondition([NotNull] AttackSkillParser.StatusConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.skillStatusSituationUnitCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSkillStatusSituationUnitCondition([NotNull] AttackSkillParser.SkillStatusSituationUnitConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.skillStatusSituationUnitCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSkillStatusSituationUnitCondition([NotNull] AttackSkillParser.SkillStatusSituationUnitConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.skillStatusSituationStatusCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSkillStatusSituationStatusCondition([NotNull] AttackSkillParser.SkillStatusSituationStatusConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.skillStatusSituationStatusCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSkillStatusSituationStatusCondition([NotNull] AttackSkillParser.SkillStatusSituationStatusConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.animSpeed"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAnimSpeed([NotNull] AttackSkillParser.AnimSpeedContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.animSpeed"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAnimSpeed([NotNull] AttackSkillParser.AnimSpeedContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.targetCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTargetCondition([NotNull] AttackSkillParser.TargetConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.targetCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTargetCondition([NotNull] AttackSkillParser.TargetConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.effectRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEffectRange([NotNull] AttackSkillParser.EffectRangeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.effectRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEffectRange([NotNull] AttackSkillParser.EffectRangeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.effectCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEffectCondition([NotNull] AttackSkillParser.EffectConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.effectCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEffectCondition([NotNull] AttackSkillParser.EffectConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.subSkill"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSubSkill([NotNull] AttackSkillParser.SubSkillContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.subSkill"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSubSkill([NotNull] AttackSkillParser.SubSkillContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.proxy"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterProxy([NotNull] AttackSkillParser.ProxyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.proxy"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitProxy([NotNull] AttackSkillParser.ProxyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.weaponReq"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterWeaponReq([NotNull] AttackSkillParser.WeaponReqContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.weaponReq"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitWeaponReq([NotNull] AttackSkillParser.WeaponReqContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.comboButton"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterComboButton([NotNull] AttackSkillParser.ComboButtonContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.comboButton"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitComboButton([NotNull] AttackSkillParser.ComboButtonContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.fxProjectileImpact"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFxProjectileImpact([NotNull] AttackSkillParser.FxProjectileImpactContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.fxProjectileImpact"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFxProjectileImpact([NotNull] AttackSkillParser.FxProjectileImpactContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.sound"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSound([NotNull] AttackSkillParser.SoundContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.sound"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSound([NotNull] AttackSkillParser.SoundContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.skillFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSkillFree([NotNull] AttackSkillParser.SkillFreeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.skillFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSkillFree([NotNull] AttackSkillParser.SkillFreeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.multiHitData"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMultiHitData([NotNull] AttackSkillParser.MultiHitDataContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.multiHitData"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMultiHitData([NotNull] AttackSkillParser.MultiHitDataContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.usabilityCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterUsabilityCondition([NotNull] AttackSkillParser.UsabilityConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.usabilityCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitUsabilityCondition([NotNull] AttackSkillParser.UsabilityConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.summonData"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSummonData([NotNull] AttackSkillParser.SummonDataContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.summonData"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSummonData([NotNull] AttackSkillParser.SummonDataContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.moveRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMoveRange([NotNull] AttackSkillParser.MoveRangeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.moveRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMoveRange([NotNull] AttackSkillParser.MoveRangeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.moveRangeCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMoveRangeCondition([NotNull] AttackSkillParser.MoveRangeConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.moveRangeCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMoveRangeCondition([NotNull] AttackSkillParser.MoveRangeConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.replaces"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterReplaces([NotNull] AttackSkillParser.ReplacesContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.replaces"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitReplaces([NotNull] AttackSkillParser.ReplacesContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.statusSituationSkillCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatusSituationSkillCondition([NotNull] AttackSkillParser.StatusSituationSkillConditionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.statusSituationSkillCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatusSituationSkillCondition([NotNull] AttackSkillParser.StatusSituationSkillConditionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.fxMove"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFxMove([NotNull] AttackSkillParser.FxMoveContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.fxMove"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFxMove([NotNull] AttackSkillParser.FxMoveContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.distanceDelay"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDistanceDelay([NotNull] AttackSkillParser.DistanceDelayContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.distanceDelay"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDistanceDelay([NotNull] AttackSkillParser.DistanceDelayContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="AttackSkillParser.statusUseLimit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatusUseLimit([NotNull] AttackSkillParser.StatusUseLimitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="AttackSkillParser.statusUseLimit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatusUseLimit([NotNull] AttackSkillParser.StatusUseLimitContext context);
}
