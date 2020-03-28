using SudokuSpice.Data;

namespace SudokuSpice
{
    public interface IColumnRestrict : ISudokuRestrict
    {
        public BitVector GetPossibleColumnValues(int column);
    }
}
