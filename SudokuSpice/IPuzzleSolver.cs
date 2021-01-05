using System;
using System.Threading;

namespace SudokuSpice
{
    public interface IPuzzleSolver<TPuzzle> where TPuzzle : IPuzzle
    {
        /// <summary>
        /// Attempts to solve the given puzzle.
        /// </summary>
        /// <param name="puzzle">The puzzle to solve. This will be solved in place.</param>
        /// <param name="randomizeGuesses">
        /// If true, this will guess in a random order when forced to guess. Else, the guessing
        /// order is up to the implementation, but it should be stable.
        /// </param>
        /// <return>
        /// True if solved, else false if it couldn't be solved for some reason.
        /// </return>
        bool TrySolve(TPuzzle puzzle, bool randomizeGuesses = false);

        /// <summary>
        /// Solves the given puzzle in place.
        /// </summary>
        /// <param name="puzzle">
        /// The puzzle to solve. This will be copied instead of solved in-place.
        /// </param>
        /// <param name="randomizeGuesses">
        /// If true, this will guess in a random order when forced to guess. Else, the guessing
        /// order is up to the implementation, but it should be stable.
        /// </param>
        /// <returns>A solved copy of the given puzzle.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if this puzzle can't be solved by this solver.
        /// </exception>
        TPuzzle Solve(TPuzzle puzzle, bool randomizeGuesses = false);

        /// <summary>
        /// Finds stats for all the solutions to the given puzzle. The puzzle is left unchanged.
        /// </summary>
        /// <exception cref="OperationCanceledException">
        /// May be thrown if the given cancellation token is canceled during the operation.
        /// </exception>
        SolveStats ComputeStatsForAllSolutions(TPuzzle puzzle, CancellationToken? token = null);

        /// <summary>
        /// Determines if the given puzzle has a unique solution. The puzzle is left unchanged.
        /// </summary>
        /// <exception cref="OperationCanceledException">
        /// May be thrown if the given cancellation token is canceled during the operation.
        /// </exception>
        bool HasUniqueSolution(TPuzzle puzzle, CancellationToken? token = null);
    }
}