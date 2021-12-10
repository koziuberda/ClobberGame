using System;
using System.Collections.Generic;

namespace Clobber.Model
{
    public class State : IState
    {
        public int Heuristics => GetHeuristic();
        public List<IState> Children => _children ??= GetChildren();
        public Board CurrentBoard { get; set; }
        public Piece PlayerPiece { get; set; }

        public State(Board currentBoard, Piece playerPiece)
        {
            CurrentBoard = currentBoard ?? throw new ArgumentNullException(nameof(currentBoard));
            
            if (playerPiece == Piece.Empty)
            { 
                throw new ArgumentException("Player must be specified", nameof(playerPiece));
            }
            PlayerPiece = playerPiece; 
        }

        public bool IsTerminate()
        {
            return (Children.Count == 0);
        }

        public State MakeMove(int row, int column, int newRow, int newColumn)
        {
            Piece nextPlayer = Piece.Empty;
            if (PlayerPiece == Piece.White)
            {
                nextPlayer = Piece.Black;
            }
            else if (PlayerPiece == Piece.Black)
            {
                nextPlayer = Piece.White;
            }

            var nextBoard = CurrentBoard.Clone();
            nextBoard.Cells[newRow, newColumn] = nextBoard.Cells[row, column];
            nextBoard.Cells[row, column] = Piece.Empty;

            return new State(nextBoard, nextPlayer);
        }

        private List<IState> _children;

        private List<IState> GetChildren()
        {
            var boards = new List<Board>();

            for (int i = 0; i < CurrentBoard.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < CurrentBoard.Cells.GetLength(1); j++)
                {
                    if (CurrentBoard.Cells[i, j] == PlayerPiece)
                    {
                        boards.AddRange(CurrentBoard.GeneratePossibleMoves(i, j));
                    }
                }
            }

            var states = new List<IState>(boards.Capacity);
            foreach (var board in boards)
            {
                Piece nextPlayer = Piece.Empty;
                if (PlayerPiece == Piece.White)
                {
                    nextPlayer = Piece.Black;
                }
                else if (PlayerPiece == Piece.Black)
                {
                    nextPlayer = Piece.White;
                }
                
                State state = new State(board, nextPlayer);
                states.Add(state);
            }

            return states;
        }
        
        private int GetHeuristic()
        {
            int whiteS = 0;
            int blackS = 0;

            for (int i = 0; i < CurrentBoard.Cells.GetLength(0); i++)
            {
                for (int j = 0; j < CurrentBoard.Cells.GetLength(1); j++)
                {
                    if (CurrentBoard.Cells[i, j] == Piece.Black)
                    {
                        blackS += CurrentBoard.GetCoordinatesOfPossibleMoves(i, j).Count;
                    }
                    else if (CurrentBoard.Cells[i, j] == Piece.White)
                    {
                        whiteS += CurrentBoard.GetCoordinatesOfPossibleMoves(i, j).Count;
                    }
                }
            }

            if (blackS == 0)
            {
                return int.MaxValue;
            }

            if (whiteS == 0)
            {
                return Int32.MinValue;
            }

            return (whiteS - blackS);
        }

    }
}