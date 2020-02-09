namespace SudokuSpice
{
    public interface IColumnRestrict
    {
        public BitVector GetPossibleColumnValues(int column);
    }
}
