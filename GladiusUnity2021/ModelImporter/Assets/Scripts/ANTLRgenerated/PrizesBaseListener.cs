//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.9.2
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from Prizes.g4 by ANTLR 4.9.2

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419


using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="IPrizesListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.9.2")]
[System.Diagnostics.DebuggerNonUserCode]
[System.CLSCompliant(false)]
public partial class PrizesBaseListener : IPrizesListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrizesParser.root"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRoot([NotNull] PrizesParser.RootContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrizesParser.root"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRoot([NotNull] PrizesParser.RootContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrizesParser.prize"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrize([NotNull] PrizesParser.PrizeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrizesParser.prize"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrize([NotNull] PrizesParser.PrizeContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrizesParser.prizeCash"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrizeCash([NotNull] PrizesParser.PrizeCashContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrizesParser.prizeCash"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrizeCash([NotNull] PrizesParser.PrizeCashContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrizesParser.prizeExp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrizeExp([NotNull] PrizesParser.PrizeExpContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrizesParser.prizeExp"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrizeExp([NotNull] PrizesParser.PrizeExpContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrizesParser.prizeItem"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrizeItem([NotNull] PrizesParser.PrizeItemContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrizesParser.prizeItem"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrizeItem([NotNull] PrizesParser.PrizeItemContext context) { }
	/// <summary>
	/// Enter a parse tree produced by <see cref="PrizesParser.prizeBadge"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPrizeBadge([NotNull] PrizesParser.PrizeBadgeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="PrizesParser.prizeBadge"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPrizeBadge([NotNull] PrizesParser.PrizeBadgeContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
