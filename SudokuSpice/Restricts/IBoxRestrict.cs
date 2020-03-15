namespace SudokuSpice
{
    public interface IBoxRestrict : ISudokuRestrict
    {
        public BitVector GetPossibleBoxValues(int box);
    }
}
