//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from GladiusLeague.g4 by ANTLR 4.9.2

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
/// <see cref="GladiusLeagueParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.CLSCompliant(false)]
public interface IGladiusLeagueListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRoot([NotNull] GladiusLeagueParser.RootContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.root"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRoot([NotNull] GladiusLeagueParser.RootContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.officeName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOfficeName([NotNull] GladiusLeagueParser.OfficeNameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.officeName"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOfficeName([NotNull] GladiusLeagueParser.OfficeNameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.officeDesc"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOfficeDesc([NotNull] GladiusLeagueParser.OfficeDescContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.officeDesc"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOfficeDesc([NotNull] GladiusLeagueParser.OfficeDescContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.tagLine1"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTagLine1([NotNull] GladiusLeagueParser.TagLine1Context context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.tagLine1"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTagLine1([NotNull] GladiusLeagueParser.TagLine1Context context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.tagLine2"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTagLine2([NotNull] GladiusLeagueParser.TagLine2Context context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.tagLine2"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTagLine2([NotNull] GladiusLeagueParser.TagLine2Context context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.leaguePtsNeeded"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLeaguePtsNeeded([NotNull] GladiusLeagueParser.LeaguePtsNeededContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.leaguePtsNeeded"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLeaguePtsNeeded([NotNull] GladiusLeagueParser.LeaguePtsNeededContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.officer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOfficer([NotNull] GladiusLeagueParser.OfficerContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.officer"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOfficer([NotNull] GladiusLeagueParser.OfficerContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.recruit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRecruit([NotNull] GladiusLeagueParser.RecruitContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.recruit"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRecruit([NotNull] GladiusLeagueParser.RecruitContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.school"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSchool([NotNull] GladiusLeagueParser.SchoolContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.school"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSchool([NotNull] GladiusLeagueParser.SchoolContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.league"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLeague([NotNull] GladiusLeagueParser.LeagueContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.league"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLeague([NotNull] GladiusLeagueParser.LeagueContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.leagueDesc"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLeagueDesc([NotNull] GladiusLeagueParser.LeagueDescContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.leagueDesc"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLeagueDesc([NotNull] GladiusLeagueParser.LeagueDescContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.leaguePts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLeaguePts([NotNull] GladiusLeagueParser.LeaguePtsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.leaguePts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLeaguePts([NotNull] GladiusLeagueParser.LeaguePtsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.encptsNeeded"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEncptsNeeded([NotNull] GladiusLeagueParser.EncptsNeededContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.encptsNeeded"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEncptsNeeded([NotNull] GladiusLeagueParser.EncptsNeededContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.onHover"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOnHover([NotNull] GladiusLeagueParser.OnHoverContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.onHover"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOnHover([NotNull] GladiusLeagueParser.OnHoverContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.onSelect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOnSelect([NotNull] GladiusLeagueParser.OnSelectContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.onSelect"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOnSelect([NotNull] GladiusLeagueParser.OnSelectContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.onWin"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterOnWin([NotNull] GladiusLeagueParser.OnWinContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.onWin"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitOnWin([NotNull] GladiusLeagueParser.OnWinContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.designNotes"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDesignNotes([NotNull] GladiusLeagueParser.DesignNotesContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.designNotes"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDesignNotes([NotNull] GladiusLeagueParser.DesignNotesContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.minPop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMinPop([NotNull] GladiusLeagueParser.MinPopContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.minPop"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMinPop([NotNull] GladiusLeagueParser.MinPopContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.minLevel"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMinLevel([NotNull] GladiusLeagueParser.MinLevelContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.minLevel"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMinLevel([NotNull] GladiusLeagueParser.MinLevelContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.maxLevel"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterMaxLevel([NotNull] GladiusLeagueParser.MaxLevelContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.maxLevel"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitMaxLevel([NotNull] GladiusLeagueParser.MaxLevelContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.hero"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterHero([NotNull] GladiusLeagueParser.HeroContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.hero"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitHero([NotNull] GladiusLeagueParser.HeroContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.tier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTier([NotNull] GladiusLeagueParser.TierContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.tier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTier([NotNull] GladiusLeagueParser.TierContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.badge"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBadge([NotNull] GladiusLeagueParser.BadgeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.badge"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBadge([NotNull] GladiusLeagueParser.BadgeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.prizeCompletion"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrizeCompletion([NotNull] GladiusLeagueParser.PrizeCompletionContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.prizeCompletion"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrizeCompletion([NotNull] GladiusLeagueParser.PrizeCompletionContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.prizeMastery"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrizeMastery([NotNull] GladiusLeagueParser.PrizeMasteryContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.prizeMastery"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrizeMastery([NotNull] GladiusLeagueParser.PrizeMasteryContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.leagueImage"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLeagueImage([NotNull] GladiusLeagueParser.LeagueImageContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.leagueImage"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLeagueImage([NotNull] GladiusLeagueParser.LeagueImageContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.encounter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEncounter([NotNull] GladiusLeagueParser.EncounterContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.encounter"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEncounter([NotNull] GladiusLeagueParser.EncounterContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.encDesc"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEncDesc([NotNull] GladiusLeagueParser.EncDescContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.encDesc"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEncDesc([NotNull] GladiusLeagueParser.EncDescContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.encFile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEncFile([NotNull] GladiusLeagueParser.EncFileContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.encFile"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEncFile([NotNull] GladiusLeagueParser.EncFileContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.encpts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEncpts([NotNull] GladiusLeagueParser.EncptsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.encpts"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEncpts([NotNull] GladiusLeagueParser.EncptsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.teams"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterTeams([NotNull] GladiusLeagueParser.TeamsContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.teams"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitTeams([NotNull] GladiusLeagueParser.TeamsContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.frequency"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFrequency([NotNull] GladiusLeagueParser.FrequencyContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.frequency"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFrequency([NotNull] GladiusLeagueParser.FrequencyContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType([NotNull] GladiusLeagueParser.TypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType([NotNull] GladiusLeagueParser.TypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.entryFee"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterEntryFee([NotNull] GladiusLeagueParser.EntryFeeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.entryFee"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitEntryFee([NotNull] GladiusLeagueParser.EntryFeeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="GladiusLeagueParser.prizeTier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrizeTier([NotNull] GladiusLeagueParser.PrizeTierContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="GladiusLeagueParser.prizeTier"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrizeTier([NotNull] GladiusLeagueParser.PrizeTierContext context);
}
