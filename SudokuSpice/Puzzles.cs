using System.Globalization;
using System.Text;

namespace SudokuSpice
{
    internal static class Puzzles
    {
        internal static string ToString(IReadOnlyPuzzle puzzle)
        {
            int size = puzzle.Size;
            bool showBoxes = Boxes.TryIntSquareRoot(size, out int boxSize);
            if (showBoxes)
            {
                return _ToStringWithBoxes(puzzle, boxSize);
            }
            int maxDigit = puzzle.AllPossibleValuesSpan.FindMax();
            var shape = new PuzzleShape {
                Size = size,
                SquareWidth = maxDigit.ToString(NumberFormatInfo.InvariantInfo).Length
            };
            var normalChars = _GetCharsForNormalRow();
            var strBuild = new StringBuilder();
            for (int row = 0; row < size; row++)
            {
                _AppendDividerRow(shape, _GetCharsForDivider(row, shape), strBuild);
                strBuild.Append(normalChars.RowStart);
                for (int col = 0; col < size; col++)
                {
                    _AppendSquareContents(puzzle[row, col], shape, strBuild);
                    if (col == size - 1)
                    {
                        strBuild.Append(normalChars.RowEnd);
                    } else
                    {
                        strBuild.Append(normalChars.BetweenColumns);
                    }
                }
                strBuild.Append('\n');
            }
            _AppendDividerRow(shape, _GetCharsForDivider(size, shape), strBuild);
            return strBuild.ToString();
        }

        private static string _ToStringWithBoxes(IReadOnlyPuzzle puzzle, int boxSize)
        {
            int size = puzzle.Size;
            int maxDigit = puzzle.AllPossibleValuesSpan.FindMax();
            var strBuild = new StringBuilder();
            var shape = new BoxPuzzleShape {
                Size = size,
                BoxSize = boxSize,
                SquareWidth = maxDigit.ToString(NumberFormatInfo.InvariantInfo).Length
            };
            var normalChars = _GetCharsForBoxRow();
            for (int row = 0; row < size; row++)
            {
                _AppendBoxDividerRow(shape, _GetCharsForDivider(row, shape), strBuild);
                strBuild.Append(normalChars.RowStart);
                for (int col = 0; col < size; col++)
                {
                    _AppendSquareContents(puzzle[row, col], shape, strBuild);
                    if (col == size - 1)
                    {
                        strBuild.Append(normalChars.RowEnd);
                    } else if (col % boxSize == boxSize - 1)
                    {
                        strBuild.Append(normalChars.BetweenBoxes);
                    } else
                    {
                        strBuild.Append(normalChars.BetweenColumns);
                    }
                }
                strBuild.Append('\n');
            }
            _AppendBoxDividerRow(shape, _GetCharsForDivider(size, shape), strBuild);
            return strBuild.ToString();
        }

        private record PuzzleShape
        {
            public int Size { get; init; }
        public int SquareWidth { get; init; }
    }

    private record BoxPuzzleShape : PuzzleShape
        {
            public int BoxSize { get; init; }
}

private record PuzzleChars
{
            public char RowStart { get; init; }
public char RowEnd { get; init; }
public char BetweenColumns { get; init; }
public char BetweenRows { get; init; }
        }

        private record BoxPuzzleChars : PuzzleChars
{
            public char BetweenBoxes { get; init; }
        }
        
        private static PuzzleChars _GetCharsForNormalRow()
{
    return new PuzzleChars {
        RowStart = '║',
        RowEnd = '║',
        BetweenColumns = '│',
        BetweenRows = '─'
    };
}

private static PuzzleChars _GetCharsForDivider(int divider, PuzzleShape shape)
{
    if (divider == 0)
    {
        return new PuzzleChars {
            RowStart = '╔',
            RowEnd = '╗',
            BetweenColumns = '╤',
            BetweenRows = '═'
        };
    } else if (divider == shape.Size)
    {
        return new PuzzleChars {
            RowStart = '╚',
            RowEnd = '╝',
            BetweenColumns = '╧',
            BetweenRows = '═'
        };
    } else
    {
        return new PuzzleChars {
            RowStart = '╟',
            RowEnd = '╢',
            BetweenColumns = '┼',
            BetweenRows = '─'
        };
    }
}

private static BoxPuzzleChars _GetCharsForBoxRow()
{
    return new BoxPuzzleChars {
        RowStart = '║',
        RowEnd = '║',
        BetweenBoxes = '║',
        BetweenColumns = '│',
        BetweenRows = '─'
    };
}

private static BoxPuzzleChars _GetCharsForDivider(int divider, BoxPuzzleShape shape)
{
    if (divider == 0)
    {
        return new BoxPuzzleChars {
            RowStart = '╔',
            RowEnd = '╗',
            BetweenBoxes = '╦',
            BetweenColumns = '╤',
            BetweenRows = '═'
        };
    } else if (divider == shape.Size)
    {
        return new BoxPuzzleChars {
            RowStart = '╚',
            RowEnd = '╝',
            BetweenBoxes = '╩',
            BetweenColumns = '╧',
            BetweenRows = '═'
        };
    } else if (divider % shape.BoxSize == 0)
    {
        return new BoxPuzzleChars {
            RowStart = '╠',
            RowEnd = '╣',
            BetweenBoxes = '╬',
            BetweenColumns = '╪',
            BetweenRows = '═'
        };
    } else
    {
        return new BoxPuzzleChars {
            RowStart = '╟',
            RowEnd = '╢',
            BetweenBoxes = '╫',
            BetweenColumns = '┼',
            BetweenRows = '─'
        };
    }
}

private static void _AppendSquareContents(int? square, PuzzleShape shape, StringBuilder strBuild)
{
    string? numberString =
        square.HasValue ?
        square.Value.ToString(NumberFormatInfo.InvariantInfo) : " ";
    for (int remainingWidth = shape.SquareWidth - numberString.Length; remainingWidth > 0; --remainingWidth)
    {
        strBuild.Append(' ');
    }
    strBuild.Append(numberString);
}

private static void _AppendBoxDividerRow(BoxPuzzleShape shape, BoxPuzzleChars rowData, StringBuilder strBuild)
{
    strBuild.Append(rowData.RowStart);
    for (int col = 0; col < shape.Size; ++col)
    {
        for (int i = 0; i < shape.SquareWidth; ++i)
        {
            strBuild.Append(rowData.BetweenRows);
        }
        if (col != shape.Size - 1)
        {
            if ((col + 1) % shape.BoxSize == 0)
            {
                strBuild.Append(rowData.BetweenBoxes);
            } else
            {
                strBuild.Append(rowData.BetweenColumns);
            }
        }
    }
    strBuild.Append(rowData.RowEnd);
    strBuild.Append('\n');
}

private static void _AppendDividerRow(PuzzleShape shape, PuzzleChars rowData, StringBuilder strBuild)
{
    strBuild.Append(rowData.RowStart);
    for (int col = 0; col < shape.Size; ++col)
    {
        for (int i = 0; i < shape.SquareWidth; ++i)
        {
            strBuild.Append(rowData.BetweenRows);
        }
        if (col != shape.Size - 1)
        {
            strBuild.Append(rowData.BetweenColumns);
        }
    }
    strBuild.Append(rowData.RowEnd);
    strBuild.Append('\n');
}
    }
}