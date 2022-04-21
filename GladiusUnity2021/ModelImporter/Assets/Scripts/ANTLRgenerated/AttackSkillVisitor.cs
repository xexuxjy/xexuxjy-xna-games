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
using Antlr4.Runtime.Tree;
using IToken = Antlr4.Runtime.IToken;

/// <summary>
/// This interface defines a complete generic visitor for a parse tree produced
/// by <see cref="AttackSkillParser"/>.
/// </summary>
/// <typeparam name="Result">The return type of the visit operation.</typeparam>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.CLSCompliant(false)]
public interface IAttackSkillVisitor<Result> : IParseTreeVisitor<Result> {
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRoot([NotNull] AttackSkillParser.RootContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.numEntries"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitNumEntries([NotNull] AttackSkillParser.NumEntriesContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.create"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCreate([NotNull] AttackSkillParser.CreateContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.skillProperty"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSkillProperty([NotNull] AttackSkillParser.SkillPropertyContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.animBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAnimBlock([NotNull] AttackSkillParser.AnimBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.fxBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFxBlock([NotNull] AttackSkillParser.FxBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.effectBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEffectBlock([NotNull] AttackSkillParser.EffectBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.statusBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatusBlock([NotNull] AttackSkillParser.StatusBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.projectileBlock"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProjectileBlock([NotNull] AttackSkillParser.ProjectileBlockContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.createDef"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCreateDef([NotNull] AttackSkillParser.CreateDefContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.useClass"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUseClass([NotNull] AttackSkillParser.UseClassContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.displayNameId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDisplayNameId([NotNull] AttackSkillParser.DisplayNameIdContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.descriptionId"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDescriptionId([NotNull] AttackSkillParser.DescriptionIdContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.level"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLevel([NotNull] AttackSkillParser.LevelContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.jobPointCost"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitJobPointCost([NotNull] AttackSkillParser.JobPointCostContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.prereq"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitPrereq([NotNull] AttackSkillParser.PrereqContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.attribute"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAttribute([NotNull] AttackSkillParser.AttributeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.costs"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCosts([NotNull] AttackSkillParser.CostsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.shiftData"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitShiftData([NotNull] AttackSkillParser.ShiftDataContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.affCost"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAffCost([NotNull] AttackSkillParser.AffCostContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.effect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEffect([NotNull] AttackSkillParser.EffectContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.effectFX"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEffectFX([NotNull] AttackSkillParser.EffectFXContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.splitEffect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSplitEffect([NotNull] AttackSkillParser.SplitEffectContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.splitEffectCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSplitEffectCondition([NotNull] AttackSkillParser.SplitEffectConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.effectSkillCond"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEffectSkillCond([NotNull] AttackSkillParser.EffectSkillCondContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.combatMods"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitCombatMods([NotNull] AttackSkillParser.CombatModsContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.moveToAttackMod"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMoveToAttackMod([NotNull] AttackSkillParser.MoveToAttackModContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.affinity"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAffinity([NotNull] AttackSkillParser.AffinityContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.range"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitRange([NotNull] AttackSkillParser.RangeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.excludeRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitExcludeRange([NotNull] AttackSkillParser.ExcludeRangeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.meter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMeter([NotNull] AttackSkillParser.MeterContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.anim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAnim([NotNull] AttackSkillParser.AnimContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.loopAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLoopAnim([NotNull] AttackSkillParser.LoopAnimContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.moveAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMoveAnim([NotNull] AttackSkillParser.MoveAnimContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.defendAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDefendAnim([NotNull] AttackSkillParser.DefendAnimContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.lowAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitLowAnim([NotNull] AttackSkillParser.LowAnimContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.chargeAnim"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitChargeAnim([NotNull] AttackSkillParser.ChargeAnimContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.animTime"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAnimTime([NotNull] AttackSkillParser.AnimTimeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.animStartFrame"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAnimStartFrame([NotNull] AttackSkillParser.AnimStartFrameContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.fx"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFx([NotNull] AttackSkillParser.FxContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.fxSwing"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFxSwing([NotNull] AttackSkillParser.FxSwingContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.fxCTAG"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFxCTAG([NotNull] AttackSkillParser.FxCTAGContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.fxProjectile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFxProjectile([NotNull] AttackSkillParser.FxProjectileContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.projectile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProjectile([NotNull] AttackSkillParser.ProjectileContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.projectileSequence"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProjectileSequence([NotNull] AttackSkillParser.ProjectileSequenceContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.projectileRotation"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProjectileRotation([NotNull] AttackSkillParser.ProjectileRotationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.projectileAttr"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProjectileAttr([NotNull] AttackSkillParser.ProjectileAttrContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.status"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatus([NotNull] AttackSkillParser.StatusContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.statusDuration"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatusDuration([NotNull] AttackSkillParser.StatusDurationContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.statusTarget"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatusTarget([NotNull] AttackSkillParser.StatusTargetContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.statusSituationAffinityCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatusSituationAffinityCondition([NotNull] AttackSkillParser.StatusSituationAffinityConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.statusChance"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatusChance([NotNull] AttackSkillParser.StatusChanceContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.effectStatusCond"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEffectStatusCond([NotNull] AttackSkillParser.EffectStatusCondContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.statusInterval"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatusInterval([NotNull] AttackSkillParser.StatusIntervalContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.statusCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatusCondition([NotNull] AttackSkillParser.StatusConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.skillStatusSituationUnitCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSkillStatusSituationUnitCondition([NotNull] AttackSkillParser.SkillStatusSituationUnitConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.skillStatusSituationStatusCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSkillStatusSituationStatusCondition([NotNull] AttackSkillParser.SkillStatusSituationStatusConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.animSpeed"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitAnimSpeed([NotNull] AttackSkillParser.AnimSpeedContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.targetCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitTargetCondition([NotNull] AttackSkillParser.TargetConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.effectRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEffectRange([NotNull] AttackSkillParser.EffectRangeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.effectCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitEffectCondition([NotNull] AttackSkillParser.EffectConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.subSkill"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSubSkill([NotNull] AttackSkillParser.SubSkillContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.proxy"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitProxy([NotNull] AttackSkillParser.ProxyContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.weaponReq"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitWeaponReq([NotNull] AttackSkillParser.WeaponReqContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.comboButton"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitComboButton([NotNull] AttackSkillParser.ComboButtonContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.fxProjectileImpact"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFxProjectileImpact([NotNull] AttackSkillParser.FxProjectileImpactContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.sound"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSound([NotNull] AttackSkillParser.SoundContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.skillFree"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSkillFree([NotNull] AttackSkillParser.SkillFreeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.multiHitData"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMultiHitData([NotNull] AttackSkillParser.MultiHitDataContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.usabilityCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitUsabilityCondition([NotNull] AttackSkillParser.UsabilityConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.summonData"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitSummonData([NotNull] AttackSkillParser.SummonDataContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.moveRange"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMoveRange([NotNull] AttackSkillParser.MoveRangeContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.moveRangeCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitMoveRangeCondition([NotNull] AttackSkillParser.MoveRangeConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.replaces"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitReplaces([NotNull] AttackSkillParser.ReplacesContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.statusSituationSkillCondition"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatusSituationSkillCondition([NotNull] AttackSkillParser.StatusSituationSkillConditionContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.fxMove"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitFxMove([NotNull] AttackSkillParser.FxMoveContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.distanceDelay"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitDistanceDelay([NotNull] AttackSkillParser.DistanceDelayContext context);
	/// <summary>
	/// Visit a parse tree produced by <see cref="AttackSkillParser.statusUseLimit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	/// <return>The visitor result.</return>
	Result VisitStatusUseLimit([NotNull] AttackSkillParser.StatusUseLimitContext context);
}
