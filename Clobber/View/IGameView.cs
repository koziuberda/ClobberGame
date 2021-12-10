using System;
using System.Windows.Forms;
using Clobber.Model;
using Clobber.Presenter;

namespace Clobber.View
{
    public interface IGameView : IView
    {
        string Winner { set; }
        State GameState { get; set; }
        ClobberPresenter Presenter { set; }
    }
}