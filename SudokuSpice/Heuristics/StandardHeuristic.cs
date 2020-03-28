﻿using SudokuSpice.Data;
using System.Collections.Generic;

namespace SudokuSpice.Heuristics
{
    public class StandardHeuristic : ISudokuHeuristic
    {
        private readonly UniqueInRowHeuristic _rowHeuristic;
        private readonly UniqueInColumnHeuristic _columnHeuristic;
        private readonly UniqueInBoxHeuristic _boxHeuristic;
        private readonly Stack<int> _numHeuristicsRan; 

        public StandardHeuristic(Puzzle puzzle, PossibleValues possibleValues,
            IRowRestrict rowRestrict, IColumnRestrict columnRestrict, IBoxRestrict boxRestrict)
        {
            _rowHeuristic = new UniqueInRowHeuristic(puzzle, possibleValues, rowRestrict);
            _columnHeuristic = new UniqueInColumnHeuristic(puzzle, possibleValues, columnRestrict);
            _boxHeuristic = new UniqueInBoxHeuristic(puzzle, possibleValues, boxRestrict);
            _numHeuristicsRan = new Stack<int>(puzzle.NumEmptySquares);
        }

        private StandardHeuristic(
            StandardHeuristic existing,
            Puzzle puzzle,
            PossibleValues possibleValues,
            IReadOnlyList<ISudokuRestrict> restricts)
        {
            _rowHeuristic = (UniqueInRowHeuristic)existing._rowHeuristic.CopyWithNewReferences(
                puzzle, possibleValues, restricts);
            _columnHeuristic = (UniqueInColumnHeuristic)existing._columnHeuristic
                .CopyWithNewReferences(puzzle, possibleValues, restricts);
            _boxHeuristic = (UniqueInBoxHeuristic)existing._boxHeuristic.CopyWithNewReferences(
                puzzle, possibleValues, restricts);
            _numHeuristicsRan = new Stack<int>(existing._numHeuristicsRan);
        }

        public ISudokuHeuristic CopyWithNewReferences(
            Puzzle puzzle,
            PossibleValues possibleValues,
            IReadOnlyList<ISudokuRestrict> restricts)
        {
            return new StandardHeuristic(this, puzzle, possibleValues, restricts);
        }

        public void UndoLastUpdate()
        {
            int numToUndo = _numHeuristicsRan.Pop();
            if (numToUndo > 2)
            {
                _boxHeuristic.UndoLastUpdate();
            }
            if (numToUndo > 1)
            {
                _columnHeuristic.UndoLastUpdate();
            }
            _rowHeuristic.UndoLastUpdate();
        }

        public bool UpdateAll()
        {
            //// Note: we can't simply do _h1.UpdateAll() | _h2.UpdateAll() | _h3.UpdateAll because
            //// we need to have a definite ordering of which heuristic ran first. This is
            //// necessary in order to accurately undo the heuristics.
            int numRan = 1;
            if (_rowHeuristic.UpdateAll())
            {
                _numHeuristicsRan.Push(numRan);
                return true;
            }
            numRan++;
            if (_columnHeuristic.UpdateAll())
            {
                _numHeuristicsRan.Push(numRan);
                return true;
            }
            numRan++;
            _numHeuristicsRan.Push(numRan);
            return _boxHeuristic.UpdateAll();
        }
    }
}
