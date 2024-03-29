﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice.RuleBased
{
    /// <summary>
    /// Tracks per-square possible values for a puzzle.
    /// </summary>
    public class PossibleValues
    {
        private readonly BitVector[][] _possibleValues;

        /// <summary>
        /// Returns a <see cref="BitVector"/> for the complete set of possible values for any given
        /// square in the puzzle represented by this <c>PossibleValues</c> object.
        /// </summary>
        public BitVector AllPossible { get; }

        /// <summary>
        /// Constructs a <c>PossibleValues</c> object to track possible values for a size-x-size
        /// puzzle. All coordinates are initialized to <paramref name="uniquePossibleValues"/>
        /// possible values.
        /// </summary>
        /// <param name="size">
        /// The size of the puzzle for which we want to track possible values.
        /// </param>
        /// <param name="uniquePossibleValues">
        /// The full set of possible values for any given square in this puzzle.
        /// </param>
        public PossibleValues(int size, BitVector uniquePossibleValues)
        {
            AllPossible = uniquePossibleValues;
            _possibleValues = new BitVector[size][];
            for (int row = 0; row < size; ++row)
            {
                _possibleValues[row] = new BitVector[size];
                _possibleValues[row].AsSpan().Fill(uniquePossibleValues);
            }
        }

        /// <summary>
        /// Copy-constructor to provide a deep copy.
        /// </summary>
        /// <param name="existing">
        /// The existing <c>PossibleValues</c> object that you want to copy.
        /// </param>
        public PossibleValues(PossibleValues existing)
        {
            int size = existing._possibleValues.Length;
            _possibleValues = new BitVector[size][];
            for (int row = 0; row < size; ++row)
            {
                _possibleValues[row] = existing._possibleValues[row].AsSpan().ToArray();
            }
            AllPossible = existing.AllPossible;
        }

        /// <summary>
        /// Provides read and write access to the possible values for the given square.
        /// </summary>
        /// <param name="c">The <see cref="Coordinate"/> of the square.</param>
        /// <returns>The possible values for that square as a <see cref="BitVector"/>.</returns>
        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "This makes sense with Coordinate, which removes any ambiguity between first and second arguments")]
        public BitVector this[in Coordinate c]
        {
            get => _possibleValues[c.Row][c.Column];
            set => _possibleValues[c.Row][c.Column] = value;
        }

        /// <summary>
        /// Updates the possible values for the square at the given <see cref="Coordinate"/> to be
        /// the intersect of their current value and the given <c>possibleValues</c>.
        /// </summary>
        /// <param name="c">The <see cref="Coordinate"/> of the square to update.</param>
        /// <param name="possibleValues">The possible values to intersect with.</param>
        public void Intersect(in Coordinate c, BitVector possibleValues)
        {
            _possibleValues[c.Row][c.Column] =
                BitVector.FindIntersect(_possibleValues[c.Row][c.Column], possibleValues);
        }

        /// <summary>
        /// Resets the possible values for the given <see cref="Coordinate"/> to the full set of
        /// possible values for this puzzle.
        /// </summary>
        /// <param name="c">The <c>Coordinate</c> of the square to reset.</param>
        public void Reset(in Coordinate c) => _possibleValues[c.Row][c.Column] = AllPossible;

        /// <summary>
        /// Resets the possible values to <see cref="AllPossible"/> at the given coordinates.
        /// </summary>
        public void Reset(ReadOnlySpan<Coordinate> coordinates)
        {
            foreach (Coordinate c in coordinates)
            {
                _possibleValues[c.Row][c.Column] = AllPossible;
            }
        }
    }
}