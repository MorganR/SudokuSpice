using System.Diagnostics.CodeAnalysis;

namespace SudokuSpice
{
    /// <summary>
    /// Tracks possible values for a puzzle.
    /// </summary>
    public class PossibleValues
    {
        private readonly Puzzle _puzzle;
        [SuppressMessage("Performance", "CA1814:Prefer jagged arrays over multidimensional", Justification = "This is a square, so no space is wasted")]
        private readonly BitVector[,] _possibleValues;
        private readonly BitVector _allPossible;

        [SuppressMessage("Performance", "CA1814:Prefer jagged arrays over multidimensional", Justification = "This is a square, so no space is wasted")]
        public PossibleValues(Puzzle puzzle)
        {
            _puzzle = puzzle;
            _possibleValues = new BitVector[puzzle.Size, puzzle.Size];
            _allPossible = BitVector.CreateWithSize(puzzle.Size);
            foreach (var c in _puzzle.GetUnsetCoords())
            {
                _possibleValues[c.Row, c.Column] = _allPossible;
            }
        }

        [SuppressMessage("Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "It is less error-prone to index this by Coordinate.")]
        public BitVector this[in Coordinate c]
        {
            get => _possibleValues[c.Row, c.Column];
            set => _possibleValues[c.Row, c.Column] = value;
        }

        public void Intersect(in Coordinate c, BitVector possibleValues)
        {
            _possibleValues[c.Row, c.Column] =
                BitVector.FindIntersect(_possibleValues[c.Row, c.Column], possibleValues);
        }

        public void Reset(in Coordinate c)
        {
            _possibleValues[c.Row, c.Column] = _allPossible;
        }
    }
}
