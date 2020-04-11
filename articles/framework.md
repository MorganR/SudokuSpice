# Framework Overview

## Concepts

SudokuSpice uses three main interfaces:

1.  The [`IPuzzle`](xref:SudokuSpice.IPuzzle)

	Puzzles store the underlying Sudoku data. You likely only need the standard
	[`Puzzle`](xref:SudokuSpice.Puzzle) implementation, but the interface is provided in case
	you need to do something a little different, like work with a puzzle with jagged regions
	instead of the normal square box regions.

2.  The [`ISudokuRule`](xref:SudokuSpice.Rules.ISudokuRule)
	
	Rules define - you guessed it - the rules for a puzzle. For example, standard Sudokus use the
	[`RowUniquenessRule`](xref:SudokuSpice.Rules.RowUniquenessRule), the
	[`ColumnUniquenessRule`](xref:SudokuSpice.Rules.ColumnUniquenessRule), and the
	[`BoxUniquenessRule`](xref:SudokuSpice.Rules.BoxUniquenessRule). For convenience and
	efficiency, these come prepackaged in the
	[`StandardRules`](xref:SudokuSpice.Rules.StandardRules) class. Rules do not directly modify the
	[`IPuzzle`](xref:SudokuSpice.IPuzzle) or [`PossibleValues`](xref:SudokuSpice.Data.PossibleValues)
	themselves. They should use an [`IReadOnlyPuzzle`](xref:SudokuSpice.IReadOnlyPuzzle) and just
	enough internal state to efficiently provide the possible values of any given square according to
	*only* that rule.
	
	Rules are enforced by an [`ISudokuRuleKeeper`](xref:SudokuSpice.Rules.ISudokuRuleKeeper). The
	[`DynamicRuleKeeper`](xref:SudokuSpice.Rules.DynamicRuleKeeper) provides a general implementation
	that works with any number of rules. Custom implementations can provide even more efficiency, but
	are generally messier and more complex than simply creating custom rules.
	[`StandardRuleKeeper`](xref:SudokuSpice.Rules.StandardRuleKeeper) is an example of this. Check out
	the [benchmarks](performance.md) for performance comparisons. The rule keeper actually
	updates the [`PossibleValues`](xref:SudokuSpice.Data.PossibleValues) based on all the rules while
	ensuring that no rules are broken by a given update.

3.  The [`ISudokuHeuristic`](xref:SudokuSpice.Heuristics.ISudokuHeuristic)

	Heuristics are logical tricks that can be used to reduce the number of possible values for any
	given square. These are only used in solving when the framework would otherwise have to guess
	(i.e. all unset squares have at least two possible values), so they can be relatively expensive
	and still improve solving times.
	
	Heuristics depend on an [`IReadOnlyPuzzle`](xref:SudokuSpice.IReadOnlyPuzzle) and directly modify
	the [`PossibleValues`](xref:SudokuSpice.Data.PossibleValues). They can alo optionally depend on one or
	more rules, as is demonstrated by the
	[`UniqueInRowHeuristic`](xref:SudokuSpice.Heuristics.UniqueInRowHeuristic). Heuristics can either
	be *perfect* heuristics, i.e. they reduce squares to only one possible value (like the `UniqueIn*`
	heuristics), or they can be *loose* heuristics, i.e. they eliminate possible values from squares,
	but don't necessarily reduce them down to a single possible value.

	Due to heuristics' complexity and flexibility, no generic class is provided to enforce multiple
	heuristics. To enforce multiple heuristics, define a custom heuristic that implements your desired
	heuristics. This pattern is demonstrated by the
	[`StandardHeuristic`](xref:SudokuSpice.Heuristics.StandardHeuristic).

For more information on extending SudokuSpice, see the [custom rule example](custom-rules.md).

## Namespaces

*   **<xref:SudokuSpice>:** Contains the main public classes for solving and generating puzzles.
*   **<xref:SudokuSpice.Data>:** Contains data classes used internally, and made public for
	users who wish to extend the framework.
*   **<xref:SudokuSpice.Heuristics>:** Contains standard heuristics and interfaces for creating
	custom heuristics.
*   **<xref:SudokuSpice.Rules>:** Contains rules, rule keepers, and interfaces for creating custom
	rules.
