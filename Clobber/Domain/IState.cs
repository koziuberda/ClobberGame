using System.Collections.Generic;

namespace Clobber.Model
{
    public interface IState
    {
        int Heuristics { get; }
        List<IState> Children { get; }
    }
}