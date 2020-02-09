namespace SudokuSpice
{
    public interface IBoxRestrict
    {
        public BitVector GetPossibleBoxValues(int box);
    }
}
