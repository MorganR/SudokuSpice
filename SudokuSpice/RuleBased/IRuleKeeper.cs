using SudokuSpice.RuleBased.Rules;
using System;

namespace SudokuSpice.RuleBased
{
    public interface IRuleKeeper
    {
        /// <summary>
        /// Creates a deep copy of this rule keeper, including copies of any
        /// <see cref="IRule"/>s it contains, with updated internal
        /// <see cref="IReadOnlyPuzzleWithMutablePossibleValues"/>.
        /// </summary>
        /// <param name="puzzle">
        /// The new puzzle reference. Should contain the same data as the current puzzle instance.
        /// </param>
        IRuleKeeper CopyWithNewReferences(
            IReadOnlyPuzzleWithMutablePossibleValues? puzzle);

        /// <summary>
        /// Gets the <see cref="IRule"/>s enforced by this rule keeper.
        /// </summary>
        ReadOnlySpan<IRule> GetRules();

        /// <summary>
        /// Tries to initialize this rule keeper to solve the given puzzle.
        /// 
        /// When reusing a rule keeper to solve multiple puzzles, this must be called with each new
        /// puzzle to be solved.
        /// </summary>
        /// <remarks>
        /// In general, it doesn't make sense to want to maintain the previous state if this method
        /// fails. Therefore, it is <em>not</em> guaranteed that the rule keeper's state is
        /// unchanged on failure.
        /// </remarks>
        /// <param name="puzzle">The puzzle to be solved.</param>
        /// <returns>
        /// False if the rule keeper couldn't be initialized, for example if the puzzle already
        /// violates one of the rules. Else returns true.
        /// </returns>
        bool TryInit(IReadOnlyPuzzleWithMutablePossibleValues puzzle);

        /// <summary>
        /// Updates possible values based on setting the given coordinate to the given value.
        /// 
        /// If this update fails (i.e. it leads to an unset square with no possible values), then it
        /// returns false and leaves all possible values unchanged.
        /// </summary>
        /// <param name="c">The coordinate to update.</param>
        /// <param name="value">The value to set <c>c</c> to.</param>
        /// <returns>
        /// True if the possible values have been updated and the rules are still satisfied.
        /// </returns>
        bool TrySet(in Coordinate c, int value);

        /// <summary>
        /// Undoes an update for the given value at the specified coordinate.
        /// </summary>
        /// <param name="c">The coordinate where a value is being unset.</param>
        /// <param name="value">The value being unset.</param>
        void Unset(in Coordinate c, int value);
    }
}