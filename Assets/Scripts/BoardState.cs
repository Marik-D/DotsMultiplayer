using System;

namespace DefaultNamespace
{
    public enum Player
    {
        Red,
        Blue
    }

    public struct CellState
    {
        public Player Player;
        public bool IsPlaced;
        public bool IsCaptured;
    }
    
    public class BoardState
    {
        public readonly int cols;
        
        public readonly int rows;

        private CellState[,] _state;

        public Player CurrentMove = Player.Red;

        public BoardState(int rows, int cols)
        {
            this.cols = cols;
            this.rows = rows;
            this._state = new CellState[rows, cols];
        }

        public bool IsValidCell(int row, int col) => row >= 0 && row < rows && col >= 0 && col < cols;

        public CellState Get(int row, int col)
        {
            if (!IsValidCell(row, col))
            {
                throw new ArgumentOutOfRangeException();
            }

            return _state[row, col];
        }

        public bool CanPlace(int row, int col) => IsValidCell(row, col) && !Get(row, col).IsPlaced;

        public void Place(int row, int col)
        {
            if (!CanPlace(row, col))
            {
                throw new ArgumentException();
            }

            _state[row, col].IsPlaced = true;
            _state[row, col].Player = CurrentMove;
            CurrentMove = CurrentMove == Player.Red ? Player.Blue : Player.Red;
        }
    }
}