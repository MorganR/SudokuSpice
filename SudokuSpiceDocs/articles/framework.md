# Framework Overview

## Solvers

*SudokuSpice* provides two different solvers: the
[`ConstraintBased.PuzzleSolver`](xref:SudokuSpice.ConstraintBased.PuzzleSolver`1) or the original
[`RuleBased.PuzzleSolver`](xref:SudokuSpice.RuleBased.PuzzleSolver`1).

Generally speaking, the original solver is the fastest of the two when solving standard Sudoku
puzzles. However, the constraint-based solver can be faster in some cases, especially when
implementing custom rules or constraints.

## Important concepts

SudokuSpice uses four main interfaces:

1.  The [`IPuzzle`](xref:SudokuSpice.IPuzzle)

	Puzzles store the underlying Sudoku data. You likely only need the standard implementations,
	[`Puzzle`](xref:SudokuSpice.Puzzle) for constraint-based tools, and [`PuzzleWithPossibleValues`]
	for rule-based tools. However, the interface is provided in case you need to do something a
	little different, like work with a puzzle with jagged regions instead of the normal square box
	regions.

2.  The [`IRule`](xref:SudokuSpice.RuleBased.Rules.IRule)
	
	Rules define - you guessed it - the rules for a puzzle. For example, standard Sudokus use the
	[`RowUniquenessRule`](xref:SudokuSpice.RuleBased.Rules.RowUniquenessRule), the
	[`ColumnUniquenessRule`](xref:SudokuSpice.RuleBased.Rules.ColumnUniquenessRule), and the
	[`BoxUniquenessRule`](xref:SudokuSpice.RuleBased.Rules.BoxUniquenessRule). For convenience and
	efficiency, these come prepackaged in the
	[`StandardRules`](xref:SudokuSpice.RuleBased.Rules.StandardRules) class. Rules do not directly modify the
	[`IPuzzle`](xref:SudokuSpice.IPuzzle) or its possible values themselves. They should use an
	[`IReadOnlyPuzzle`](xref:SudokuSpice.IReadOnlyPuzzle) and just enough internal state to
	efficiently provide the possible values of any given square according to *only* that rule.
	
	Rules are enforced by an [`IRuleKeeper`](xref:SudokuSpice.RuleBased.IRuleKeeper). The
	[`DynamicRuleKeeper`](xref:SudokuSpice.RuleBased.DynamicRuleKeeper) provides a general implementation
	that works with any number of rules. Custom implementations can provide even more efficiency, but
	are generally messier and more complex than simply creating custom rules.
	[`StandardRuleKeeper`](xref:SudokuSpice.RuleBased.StandardRuleKeeper) is an example of this. Check out
	the [benchmarks](performance.md) for performance comparisons. The rule keeper actually
	updates the possible values based on all the rules while ensuring that no rules are broken by
	any given update.

3.  The [`IHeuristic`](xref:SudokuSpice.RuleBased.Heuristics.IHeuristic)

	Heuristics are logical tricks that can be used to reduce the number of possible values for any
	given square. These are only used in solving when the framework would otherwise have to guess
	(i.e. all unset squares have at least two possible values), so they can be relatively expensive
	and still improve solving times.
	
	Heuristics depend on an
	[`IReadOnlyPuzzleWithMutablePossibleValues`](xref:SudokuSpice.RuleBased.IReadOnlyPuzzleWithMutablePossibleValues).
	They directly modify the possible values during an update. They can alo optionally depend on one or
	more rules, as is demonstrated by the
	[`UniqueInRowHeuristic`](xref:SudokuSpice.RuleBased.Heuristics.UniqueInRowHeuristic). Heuristics
	can either be *perfect* heuristics, i.e. they reduce squares to only one possible value (like
	the `UniqueIn*` heuristics), or they can be *loose* heuristics, i.e. they eliminate possible
	values from squares, but don't necessarily reduce them down to a single possible value.

	Due to heuristics' complexity and flexibility, no generic class is provided to enforce multiple
	heuristics. To enforce multiple heuristics, define a custom heuristic that implements your desired
	heuristics. This pattern is demonstrated by the
	[`StandardHeuristic`](xref:SudokuSpice.RuleBased.Heuristics.StandardHeuristic).

4.  The [`IConstraint`](xref:SudokuSpice.ConstraintBased.Constraints.IConstraint)

    Constraints represent rules that the puzzle needs to satisfy. For example, the
    [`RowUniquenessConstraint`](xref:SudokuSpice.ConstraintBased.Constraints.RowUniquenessConstraint)
    enforces the constraint that "each row must contain *all* possible values."

    Constraints are implemented using an
    [exact-cover matrix](https://en.wikipedia.org/wiki/Exact_cover). The exact-cover matrix combines
    two concepts into a single matrix. Each row represents a possible value for a single square, for
    example "Row: 1, Column: 0, Value: 2". We'll represent this in the short-form notation: `R1C0V2`.
    Each column represents a single constraint that must be satisfied, for example, "Row 1 contains a
    2." We'll represent columns in the short-form notation: "R1V2". These rows and columns can be
    combined into a single matrix containing 1s and 0s, where a 1 is placed in each column (i.e.
    constraint) that a given row (i.e. possible square value) satisfies. For a standard Sudoku puzzle,
    this looks something like the following:

    |        | R0V1 | R0V2 | ... | R1V1 | R1V2 | ... | C0V1 | C0V2 | ... | B0V1 | V0V2 | ... | B8V8 | B8V9 |
    |--------|------|------|-----|------|------|-----|------|------|-----|------|------|-----|------|------|
    | R0C0V1 | 1    | 0    | ... | 0    | 0    | ... | 1    | 0    | ... | 1    | 0    | ... | 0    | 0    |
    | R0C0V2 | 0    | 1    | ... | 0    | 0    | ... | 0    | 1    | ... | 0    | 1    | ... | 0    | 0    |
    | ...    | ...  | ...  | ... | ...  | ...  | ... | ...  | ...  | ... | ...  | ...  | ... | ...  | ...  |
    | R0C1V1 | 1    | 0    | ... | 0    | 0    | ... | 0    | 0    | ... | 1    | 0    | ... | 0    | 0    |
    | R0C1V2 | 0    | 1    | ... | 0    | 0    | ... | 0    | 0    | ... | 0    | 1    | ... | 0    | 0    |
    | ...    | ...  | ...  | ... | ...  | ...  | ... | ...  | ...  | ... | ...  | ...  | ... | ...  | ...  |
    | R1C0V1 | 0    | 0    | ... | 1    | 0    | ... | 1    | 0    | ... | 1    | 0    | ... | 0    | 0    |
    | ...    | ...  | ...  | ... | ...  | ...  | ... | ...  | ...  | ... | ...  | ...  | ... | ...  | ...  |
    | R8C8V8 | 0    | 0    | ... | 0    | 0    | ... | 0    | 0    | ... | 0    | 0    | ... | 1    | 0    |
    | R8C8V9 | 0    | 0    | ... | 0    | 0    | ... | 0    | 0    | ... | 0    | 0    | ... | 0    | 1    |

    SudokuSpice's implementation represents this matrix as a 2D-doubly linked list. Row headers (i.e.
    the `RxCxVx` cells in the first column) are represented by
    [`PossibleSquareValue`s](xref:SudokuSpice.ConstraintBased.PossibleSquareValue). Column headers
    (i.e. the cells in the first row) are represented by
    [`ConstraintHeader`s](xref:SudokuSpice.ConstraintBased.ConstraintHeader). Rows and columns are
    connected by links, which represent the 1s in the matrix. Each link is connected up and down to
    the other '1s' that satisfy that constraint header, and connected left and right to the other
    '1s' that are present for that possible square value.

    The constraint-based solver uses constraints instead of rules. It does not provide a separate
    heuristics concept because the exact-cover matrix provides the `UniqueIn*` heuristics by default.
    Adding additional layers of heuristics would add complexity with minimal, if any, performance
    improvement.

For more information on extending SudokuSpice, see:

*  [Custom rule example](custom-rules.md).
*  [Custom constraint example](custom-constraints.md).

## Namespaces

*   **<xref:SudokuSpice>:** Contains base classes for solving and generating puzzles.
*   **<xref:SudokuSpice.RuleBased>:** Contains classes for rule-based puzzle-solving and
	-generating.
*   **<xref:SudokuSpice.RuleBased.Heuristics>:** Contains standard heuristics and interfaces for
	creating custom heuristics.
*   **<xref:SudokuSpice.RuleBased.Rules>:** Contains rules and interfaces for creating custom
	rules.
*   **<xref:SudokuSpice.ConstraintBased>:** Contains classes for constraint-based puzzle-solving
	and -generating.
*   **<xref:SudokuSpice.ConstraintBased.Constraints>:** Contains standard constraints and the
    [`IConstraint`](xref:SudokuSpice.ConstraintBased.Constraints.IConstraint) interface.
