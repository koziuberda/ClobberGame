using System;
using System.Diagnostics;

namespace Clobber.Model
{
    public class ClobberSolver : Solver<State>
    {
        private int _moveCounter;

        private Level _difficulty;
        private uint SearchDepth { get; set; }
        public ClobberSolver(Level difficulty)
        {
            _difficulty = difficulty;
            _moveCounter = 0;
            SearchDepth = difficulty switch
            {
                Level.Easy => 3,
                Level.Medium => 4,
                Level.Hard => 4,
                _ => SearchDepth
            };
        }

        public State BestMove(State position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position), "Position to solve is zero");
            }

            if (_difficulty == Level.Hard)
            {
                if (_moveCounter >= 2)
                {
                    SearchDepth++;
                }
            }

            var maximizing = (position.PlayerPiece == Piece.White);
            
            var children = position.Children;
            
            Debug.Assert(children.Count != 0, "Why to solve terminate?");
            
            var bestEval = maximizing ? Int32.MinValue : Int32.MaxValue;
            var bestNode = default(State);

            foreach (State state in children)
            {
                var eval = EstimateNode(state, SearchDepth - 1, 
                    Int32.MinValue, Int32.MaxValue, !maximizing);
                if (maximizing)
                {
                    if (eval >= bestEval)
                    {
                        bestEval = eval;
                        bestNode = state;
                    }
                }
                else
                {
                    if (eval <= bestEval)
                    {
                        bestEval = eval;
                        bestNode = state;
                    }
                }
            }

            _moveCounter++;

            return bestNode;
        }
    }
}