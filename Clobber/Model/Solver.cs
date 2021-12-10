using System;

namespace Clobber.Model
{
    public abstract class Solver<TNode> where TNode : IState
    {
        protected int EstimateNode(TNode position, uint depth, int alpha, int beta, bool maximizing)
        {
            if (depth == 0)
            {
                return position.Heuristics;
            }

            if (maximizing)
            {
                int maxEval = Int32.MinValue;
                foreach (TNode child in position.Children)
                {
                    int eval = EstimateNode(child, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return maxEval;
            }
            else
            {
                int minEval = Int32.MaxValue;
                foreach (TNode child in position.Children)
                {
                    int eval = EstimateNode(child, depth - 1, alpha, beta, true);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return minEval;
            }
        }
    }
}