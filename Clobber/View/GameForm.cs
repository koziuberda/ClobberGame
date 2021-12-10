using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Clobber.Model;
using Clobber.Presenter;

namespace Clobber.View
{
    public partial class GameForm : Form, IGameView
    {
        public ClobberPresenter Presenter { private get; set; }
        
        public State GameState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                RenderGameBoard(_currentState);
            }
        }
        
        public string Winner
        {
            set
            {
                MessageBox.Show(
                    $"The {value} won!",
                    "The end",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        
        private Button[,] Cells { get; set; }
        private Piece Player { get; set; }

        private const int CellSize = 100;

        private Color colorOfPossibleMoves = Color.Lime;
        private Color colorOfPressedButton = Color.Red;

        private Color previousColor;
        private Button previousButton;
        
        private bool isMoving;
        private List<Button> possibleMoves;

        private State _currentState;
        
        private Image _whitePiece;
        private Image _blackPiece;
        public GameForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            possibleMoves = new List<Button>();
            isMoving = false;
            _whitePiece = new Bitmap(new Bitmap(@"C:\Users\koziu\RiderProjects\ClobberGame\Clobber\Resources\w.png"), new Size(CellSize - 10, CellSize - 10));
            _blackPiece = new Bitmap(new Bitmap(@"C:\Users\koziu\RiderProjects\ClobberGame\Clobber\Resources\b.png"), new Size(CellSize - 10, CellSize - 10));
            InitializeComponent();
            PlayerSetup();
        }

        private void PlayerSetup()
        {
            Controls.Clear();
            Width = 400;
            Height = 400;
            
            var btnWhite = new Button();
            var x = (this.Width / 2) - (btnWhite.Width / 2);
            var y = (this.Height / 2) - (btnWhite.Height / 2) - CellSize;
            btnWhite.Size = new Size(CellSize, CellSize);
            btnWhite.Location = new Point(x, y);
            btnWhite.Text = "White";
            
            btnWhite.Click += new EventHandler((sender, e) =>
            {
                Player = Piece.White;
                DifficultySetup();
            });
            
            var btnBlack = new Button();
            btnBlack.Size = new Size(CellSize, CellSize);
            btnBlack.Location = new Point(x, y+CellSize);
            btnBlack.Text = "Black";
            
            btnBlack.Click += new EventHandler((sender, e) =>
            {
                Player = Piece.Black;
                DifficultySetup();
            });
            

            Controls.Add(btnWhite);
            Controls.Add(btnBlack);
            
        }

        private void DifficultySetup()
        {
            Controls.Clear();
            Width = 400;
            Height = 400;
            
            var btnEasy = new Button();
            var x = (this.Width / 2) - (btnEasy.Width / 2);
            var y = (this.Height / 2) - (btnEasy.Height / 2) - 2*CellSize;
            btnEasy.Size = new Size(CellSize, CellSize);
            btnEasy.Location = new Point(x, y);
            btnEasy.Text = "Easy";
            
            btnEasy.Click += new EventHandler((sender, e) =>
            {
                Presenter.StartGame(Player, Level.Easy);
            });
            
            var btnMedium = new Button();
            btnMedium.Size = new Size(CellSize, CellSize);
            btnMedium.Location = new Point(x, btnEasy.Location.Y + CellSize);
            btnMedium.Text = "Medium";
            
            btnMedium.Click += new EventHandler((sender, e) =>
            {
                Presenter.StartGame(Player, Level.Medium);
            });
            
            var btnHard = new Button();
            btnHard.Size = new Size(CellSize, CellSize);
            btnHard.Location = new Point(x, btnMedium.Location.Y + CellSize);
            btnHard.Text = "Hard";
            
            btnHard.Click += new EventHandler((sender, e) =>
            {
                Presenter.StartGame(Player, Level.Hard);
            });
            

            Controls.Add(btnEasy);
            Controls.Add(btnMedium);
            Controls.Add(btnHard);

        }


        private void RenderGameBoard(State gameState)
        {
            Controls.Clear();
            isMoving = false;
            var board = gameState.CurrentBoard.Clone();

            Height = (board.Rows + 1) * CellSize;
            Width = (board.Columns + 1) * CellSize;

            Cells = new Button[board.Rows, board.Columns];
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    var button = new Button();
                    button.Location = new Point(j * CellSize, i * CellSize);
                    button.Size = new Size(CellSize, CellSize);
                    button.Click += new EventHandler(OnFigurePress);

                    button.Image = board.Cells[i, j] switch
                    {
                        Piece.Black => _blackPiece,
                        Piece.White => _whitePiece,
                        _ => button.Image
                    };

                    button.BackColor = OriginalColor(i, j);
                    
                    Cells[i, j] = button;
                    Controls.Add(button);
                }
            }
        }

        private Color OriginalColor(int row, int column)
        {
            var color = Color.White;
            if (row % 2 == 0)
            {
                if (column % 2 != 0)
                {
                    color = Color.Gray;
                }
            }
            else
            {
                if (column % 2 == 0)
                {
                    color = Color.Gray;
                }
            }

            return color;
        }

        private Tuple<int, int> GetCellCoordinates(Button button)
        {
            int row = button.Location.Y / CellSize;
            int column = button.Location.X / CellSize;

            return Tuple.Create(row, column);
        }
        private void OnFigurePress(object sender, EventArgs e)
        {
            var pressed = sender as Button;

            if (pressed == null)
            {
                throw new ArgumentNullException();
            }

            if (isMoving)
            {
                if (possibleMoves.Contains(pressed))
                {
                    var from = GetCellCoordinates(previousButton);
                    var to = GetCellCoordinates(pressed);
                    Presenter.MakeMove(from, to);
                    
                    isMoving = false;
                }
                else
                {
                    var cords = GetCellCoordinates(pressed);
                    if (GameState.CurrentBoard.Cells[cords.Item1, cords.Item2] != GameState.PlayerPiece)
                    {
                        return;
                    }
                    foreach (var button in possibleMoves)
                    {
                        var oldCoordinates = GetCellCoordinates(button);
                        button.BackColor = OriginalColor(oldCoordinates.Item1, oldCoordinates.Item2);
                    }
                   
                    previousButton.BackColor = previousColor;
                    
                    previousColor = pressed.BackColor;
                    previousButton = pressed;
                    pressed.BackColor = colorOfPressedButton;
                    
                    possibleMoves = ShowPossibleMoves(pressed);

                }
            }
            else
            {
                var cords = GetCellCoordinates(pressed);
                if (GameState.CurrentBoard.Cells[cords.Item1, cords.Item2] != GameState.PlayerPiece)
                {
                    return;
                }
                isMoving = true;
                if (previousButton != null)
                { 
                    previousButton.BackColor = previousColor;
                }
                
                previousColor = pressed.BackColor;
                pressed.BackColor =colorOfPressedButton;
                previousButton = pressed;
                
                possibleMoves = ShowPossibleMoves(pressed);
                
            }
        }

        private List<Button> ShowPossibleMoves(Button pressed)
        {
            var listOfButtonMoves = new List<Button>();
            var c = GetCellCoordinates(pressed);
            var moves = GameState.CurrentBoard.GetCoordinatesOfPossibleMoves(c.Item1, c.Item2);
            
            foreach (var tupleMove in moves)
            {
                var buttonMove = Cells[tupleMove.Item1, tupleMove.Item2];
                buttonMove.BackColor = colorOfPossibleMoves;
                listOfButtonMoves.Add(buttonMove);
            }

            return listOfButtonMoves;
        }
    }
}