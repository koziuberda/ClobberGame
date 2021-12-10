using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Clobber.Model
{
    using static Piece;

    public class Board
    {
        public Piece[,] Cells { get; private set; }

        public Board(Piece[,] state = null)
        {
            Cells = state ?? GenerateInitialBoard();
        }

        public int Rows => Cells.GetLength(0);
        public int Columns => Cells.GetLength(1);
        
        public List<Board> GeneratePossibleMoves(int row, int column)
        {
            Debug.Assert(row < Cells.GetLength(0) && row >= 0, "Invalid cell's row");
            Debug.Assert(column < Cells.GetLength(1) && column >= 0, "Invalid cell's column");
            Debug.Assert(Cells[row, column] != Empty, "There is no piece");

            if (Cells[row, column] == Empty)
            {
                return null;
            }
            
            var boards = new List<Board>();
            var coords = GetCoordinatesOfPossibleMoves(row, column);
            
            foreach (var (newRow, newColumn) in coords)
            {
                if (!IsMoveValid(row, column, newRow, newColumn))
                {
                    continue;
                }

                var clonedBoard = this.Clone();

                clonedBoard.Cells[newRow, newColumn] = clonedBoard.Cells[row, column];
                clonedBoard.Cells[row, column] = Empty;
                boards.Add(clonedBoard);
            }

            return boards;
        }

        public List<Tuple<int, int>> GetCoordinatesOfPossibleMoves(int row, int column)
        {
            var list = new List<Tuple<int, int>>
            {
                Tuple.Create(row-1,column), 
                Tuple.Create(row + 1, column),
                Tuple.Create(row, column - 1),
                Tuple.Create(row, column + 1)
            };
            var result = new List<Tuple<int, int>>();
            foreach (var move in list)
            {
                if (IsMoveValid(row, column, move.Item1, move.Item2))
                {
                    result.Add(move);
                }
            }

            return result;
        }

        public bool IsOnTheBoard(int row, int column)
        {
            if (!(row < Cells.GetLength(0) && row >= 0))
            {
                return false;
            }

            if (!(column < Cells.GetLength(1) && column >= 0))
            {
                return false;
            }

            return true;
        }

        public bool IsMoveValid(int row, int column, int newRow, int newColumn)
        {
            Debug.Assert(row < Cells.GetLength(0) && row >= 0, "Invalid cell's row");
            Debug.Assert(column < Cells.GetLength(1) && column >= 0, "Invalid cell's column");
            Debug.Assert(Cells[row, column] != Empty, "There is no piece");

            if (!IsOnTheBoard(row, column) || !IsOnTheBoard(newRow, newColumn))
            {
                return false;
            }
            
            if (Cells[newRow, newColumn] == Empty || Cells[newRow, newColumn] == Cells[row, column])
            {
                return false;
            }

            return true;
        }
        
        public int CountDiagonalNeighbours(int row, int column)
        {
            int counter = 0;
            
            Piece currentPiece = Cells[row, column];

            List<Tuple<int, int>> cells = new List<Tuple<int, int>>
            {
                Tuple.Create(row - 1, column - 1),
                Tuple.Create(row - 1, column + 1),
                Tuple.Create(row + 1, column - 1),
                Tuple.Create(row + 1, column + 1)
            };

            foreach (var (newRow, newColumn) in cells)
            {
                if (!IsOnTheBoard(newRow, newColumn))
                {
                    continue;
                }
                if (Cells[newRow, newColumn] == currentPiece)
                {
                    counter++;
                }
            }

            return counter;
        }
        
        public Board Clone()
        {
            var clonedCells = Cells.Clone() as Piece[,];
            return new Board(clonedCells);
        }

        public override string ToString()
        {
            var board = new StringBuilder();

            for (int i = 0; i < Cells.GetLength(0); i++)
            {
                for (int j = 0; j < Cells.GetLength(1); j++)
                {
                    if (Cells[i, j] == Black)
                    {
                        board.Append("B ");
                    }
                    else if (Cells[i, j] == White)
                    {
                        board.Append("W ");
                    }
                    else
                    {
                        board.Append("_ ");
                    }
                }
                board.Append('\n');
            }

            return board.ToString();
        }

        private Piece[,] GenerateInitialBoard()
        {
            var initialBoard = new Piece[,]
            {
                {White, Black, White, Black, White},
                {Black, White, Black, White, Black},
                {White, Black, White, Black, White},
                {Black, White, Black, White, Black},
                {White, Black, White, Black, White},
                {Black, White, Black, White, Black}
            };

            return initialBoard;
        }
    }
}