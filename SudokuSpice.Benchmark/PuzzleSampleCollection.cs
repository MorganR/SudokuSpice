using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSpice.Benchmark
{
    public class PuzzleSampleCollection
    {
        private readonly Random _random = new Random();
        private readonly string _name;
        private readonly IReadOnlyList<PuzzleSample> _samples;
        private int _idx = 0;

        internal PuzzleSampleCollection(IReadOnlyList<SudokuSample> sudokus, int minNumSquaresToGuess, int maxNumSquaresToGuess)
        {
            if (maxNumSquaresToGuess - minNumSquaresToGuess == 1)
            {
                _name = $"Guesses: {minNumSquaresToGuess}";
            } else if (maxNumSquaresToGuess == int.MaxValue)
            {
                _name = $"Guesses: {minNumSquaresToGuess}+";
            } else
            {
                _name = $"Guesses: {minNumSquaresToGuess}-{maxNumSquaresToGuess - 1}";
            }

            _samples = sudokus.Where(s => s.NumSquaresToGuess >= minNumSquaresToGuess && s.NumSquaresToGuess < maxNumSquaresToGuess)
                .Select(
                    sample => new PuzzleSample(
                        _name,
                        CsvUtils.PuzzleStringToMatrix(sample.Puzzle)))
                .ToList();
            if (_samples.Count == 0)
            {
                throw new ArgumentException($"No puzzles satisfied puzzle sample collection: {_name}");
            }
        }

        public PuzzleSample Next()
        {
            int idx = _idx;
            if (++_idx == _samples.Count)
            {
                _idx = 0;
            }
            return _samples[idx];
        }

        public PuzzleSample Random() => _samples[_random.Next(0, _samples.Count)];

        public override string ToString() => _name;
    }
}
