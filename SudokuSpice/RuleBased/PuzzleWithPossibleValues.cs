﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice.RuleBased
{
    /// <summary>Stores a puzzle's data and associated possible values.</summary>.
    public class PuzzleWithPossibleValues : IPuzzleWithPossibleValues<PuzzleWithPossibleValues>
    {
        private readonly Puzzle _puzzle;
        private readonly PossibleValues _possibleValues;

        /// <inheritdoc cref="IReadOnlyPuzzle"/>
        public int Size => _puzzle.Size;
        /// <inheritdoc cref="IReadOnlyPuzzle"/>
        public int NumSquares => _puzzle.NumSquares;
        /// <inheritdoc cref="IReadOnlyPuzzle"/>
        public int NumEmptySquares => _puzzle.NumEmptySquares;
        /// <inheritdoc cref="IReadOnlyPuzzle"/>
        public int NumSetSquares => _puzzle.NumSetSquares;
        /// <inheritdoc cref="IReadOnlyPuzzle"/>
        public BitVector UniquePossibleValues => _possibleValues.AllPossible;
        /// <inheritdoc/>
        public ReadOnlySpan<int> AllPossibleValuesSpan => _puzzle.AllPossibleValuesSpan;
        /// <inheritdoc/>
        public IReadOnlyDictionary<int, int> CountPerUniqueValue => _puzzle.CountPerUniqueValue;

        /// <summary>
        /// Constructs a new puzzle of the given side length.
        /// </summary>
        /// <param name="size">
        /// The side-length for this Sudoku puzzle. Must be in the inclusive range [1, 31].
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown if size is not the square of a whole number, or is outside the range [1, 31].
        /// </exception>
        public PuzzleWithPossibleValues(int size)
        {
            if (size < 1 || size >= BitVector.NumBits)
            {
                throw new ArgumentException($"Puzzle size must be in the range [1, {BitVector.NumBits - 1}].");
            }
            _puzzle = new Puzzle(size);
            var possibleValues = BitVector.CreateWithSize(size + 1);
            possibleValues.UnsetBit(0);
            _possibleValues = new PossibleValues(size, possibleValues);
        }

        /// <summary>
        /// Constructs a new puzzle backed by the given array.
        ///
        /// The puzzle is backed directly by this array (i.e. modifying the array
        /// modifies the puzzle, and vice-versa). If this is not what you want, see
        /// <see cref="CopyFrom(int?[,])"/> and <see cref="CopyFrom(int?[][])"/>. Note that all
        /// future modifications should be done through this puzzle object, else this will be in an
        /// incorrect state.
        /// </summary>
        /// <param name="puzzleMatrix">
        /// The data for this Sudoku puzzle. Preset squares should be set, and unset squares should
        /// be null. The puzzle maintains a reference to this array.
        /// </param>
        public PuzzleWithPossibleValues(int?[][] puzzleMatrix)
        {
            _puzzle = new Puzzle(puzzleMatrix);
            int size = _puzzle.Size;
            if (size < 1 || size >= BitVector.NumBits)
            {
                throw new ArgumentException($"Puzzle size must be in the range [1, {BitVector.NumBits - 1}].");
            }
            var possibleValues = BitVector.CreateWithSize(size + 1);
            possibleValues.UnsetBit(0);
            _possibleValues = new PossibleValues(size, possibleValues);
        }

        /// <summary>
        /// Constructs a puzzle backed by the given <see cref="Puzzle"/> object, but now with the
        /// ability to track possible values.
        /// </summary>
        /// <param name="puzzle">
        /// The puzzle data to use. The puzzle maintains a reference to this object.
        /// </param>
        public PuzzleWithPossibleValues(Puzzle puzzle)
        {
            _puzzle = puzzle;
            var possibleValues = new BitVector();
            foreach (var possibleValue in puzzle.AllPossibleValuesSpan)
            {
                if (possibleValue >= BitVector.NumBits)
                {
                    throw new ArgumentException(
                        $"Puzzle must have possible values in the range [0, {BitVector.NumBits - 1}]. Received value {possibleValue}.");
                }
                possibleValues.SetBit(possibleValue);
            }
            _possibleValues = new PossibleValues(Size, possibleValues);
        }

        /// <summary>
        /// A deep copy constructor for an existing puzzle.
        /// </summary>
        public PuzzleWithPossibleValues(PuzzleWithPossibleValues existing)
        {
            _puzzle = existing._puzzle.DeepCopy();
            _possibleValues = new PossibleValues(existing._possibleValues);
        }

        /// <summary>Creates a new puzzle with a copy of the given matrix.</summary>
        [SuppressMessage("Performance", "CA1814:Prefer jagged arrays over multidimensional", Justification = "Provided to ease migration.")]
        public static PuzzleWithPossibleValues CopyFrom(int?[,] matrix)
        {
            return new PuzzleWithPossibleValues(matrix.CopyToJagged2D());
        }

        /// <summary>Creates a new puzzle with a copy of the given matrix.</summary>
        public static PuzzleWithPossibleValues CopyFrom(int?[][] matrix)
        {
            return new PuzzleWithPossibleValues(matrix.Copy2D());
        }

        /// <inheritdoc/>
        public PuzzleWithPossibleValues DeepCopy() => new PuzzleWithPossibleValues(this);

        /// <inheritdoc/>
        public int? this[int row, int col]
        {
            get => _puzzle[row, col];
            set {
                _puzzle[row, col] = value;
                if (!value.HasValue) {
                    _possibleValues.Reset(new Coordinate(row, col));
                }
            }
        }

        /// <inheritdoc cref="IPuzzle{T}"/>
        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "This makes sense with Coordinate, which removes any ambiguity between first and second arguments")]
        public int? this[in Coordinate c]
        {
            get => _puzzle[in c];
            set => _puzzle[in c] = value;
        }

        /// <summary>Gets a span of coordinates for all the unset squares.</summary>
        public ReadOnlySpan<Coordinate> GetUnsetCoords() => _puzzle.GetUnsetCoords();

        /// <summary>
        /// Returns the puzzle in a pretty string format, with boxes and rows separated by pipes
        /// and dashes.
        /// </summary>
        public override string ToString() => Puzzles.ToString(_puzzle);

        /// <inheritdoc/>
        public void IntersectPossibleValues(in Coordinate c, BitVector possibleValues) =>
            _possibleValues.Intersect(in c, possibleValues);
        /// <inheritdoc/>
        public void ResetPossibleValues(in Coordinate c) => _possibleValues.Reset(in c);
        /// <inheritdoc/>
        public BitVector GetPossibleValues(in Coordinate c) => _possibleValues[in c];
        /// <inheritdoc/>
        public void SetPossibleValues(in Coordinate c, BitVector possibleValues) => _possibleValues[in c] = possibleValues;
    }
}