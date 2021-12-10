using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Clobber.Model;
using Clobber.View;

namespace Clobber.Presenter
{
    public class ClobberPresenter
    {
        private readonly IGameView _view;
        private readonly BackgroundWorker _backgroundWorker;
        private ClobberSolver _solver;
        private State _currentState;

        public ClobberPresenter(IGameView view)
        {
            _view = view;
            view.Presenter = this;
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += new DoWorkEventHandler(BackgroundWorker_DoWork);
            _backgroundWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(BackgroundWorker_RunWorkerCompleted);
        }
        
        public void StartGame(Piece player, Level difficulty)
        {
            _currentState = new State(new Board(), player);
            _solver = new ClobberSolver(difficulty);
            _view.GameState = _currentState;
        }

        public void MakeMove(Tuple<int, int> from, Tuple<int, int> to)
        {
            var nextState = _currentState.MakeMove(from.Item1, from.Item2, to.Item1, to.Item2);
            bool terminate = nextState.IsTerminate();
            _currentState = nextState;
            _view.GameState = _currentState;
            
            if (terminate)
            {
                Final();
                return;
            }
            
            _backgroundWorker.RunWorkerAsync();
        }
        
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            AIMove();
            Thread.Sleep(100);
        }
        
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _view.GameState = _currentState;
            if (_currentState.IsTerminate())
            {
                Final();
            }
        }


        private void AIMove()
        {
            Debug.Assert(_currentState != null);
            var move = _solver.BestMove(_currentState);
            _currentState = move;
        }

        private void Final()
        {
            if (_currentState.PlayerPiece == Piece.White)
            {
                _view.Winner = "blacks";
            }
            else if (_currentState.PlayerPiece == Piece.Black)
            {
                _view.Winner = "whites";
            }
        }
    }
}