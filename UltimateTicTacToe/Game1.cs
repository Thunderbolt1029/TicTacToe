using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace UltimateTicTacToe
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        State PlayerToPlay, Winner;
        bool Won;
        int WinType;

        Keys[] CurrentHeldButtons = new Keys[1], PreviousHeldButtons = new Keys[1];
        Keys[] ClickedButtons => CurrentHeldButtons.Except(PreviousHeldButtons).ToArray();

        Board[,] boards = new Board[3, 3];

        InputStage inputStage = InputStage.ChooseBoard;
        Board SelectedBoard;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1070;
            graphics.PreferredBackBufferHeight = 890;
        }

        protected override void Initialize()
        {
            Reset();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Globals.ColorTexture = new Texture2D(GraphicsDevice, 1, 1);
            Globals.ColorTexture.SetData(new[] { Color.White });

            Globals.text = Content.Load<SpriteFont>("text");
        }

        protected override void Update(GameTime gameTime)
        {
            PreviousHeldButtons = CurrentHeldButtons;
            CurrentHeldButtons = Keyboard.GetState().GetPressedKeys();

            if (ClickedButtons.Contains(Keys.Escape))
                Exit();
            if (Won)
                if (ClickedButtons.Contains(Keys.Space))
                    Reset();
                else
                    return;

            Point PosToPlay = new Point(-1);
            if (ClickedButtons.Contains(Keys.NumPad1)) PosToPlay = KeyToPos(1);
            else if (ClickedButtons.Contains(Keys.NumPad2)) PosToPlay = KeyToPos(2);
            else if (ClickedButtons.Contains(Keys.NumPad3)) PosToPlay = KeyToPos(3);
            else if (ClickedButtons.Contains(Keys.NumPad4)) PosToPlay = KeyToPos(4);
            else if (ClickedButtons.Contains(Keys.NumPad5)) PosToPlay = KeyToPos(5);
            else if (ClickedButtons.Contains(Keys.NumPad6)) PosToPlay = KeyToPos(6);
            else if (ClickedButtons.Contains(Keys.NumPad7)) PosToPlay = KeyToPos(7);
            else if (ClickedButtons.Contains(Keys.NumPad8)) PosToPlay = KeyToPos(8);
            else if (ClickedButtons.Contains(Keys.NumPad9)) PosToPlay = KeyToPos(9);

            if (PosToPlay != new Point(-1))
            {
                switch (inputStage)
                {
                    case InputStage.ChooseBoard:
                        if (SelectedBoard != null)
                            SelectedBoard.Selected = false;
                        SelectedBoard = boards[PosToPlay.X, PosToPlay.Y];
                        if (SelectedBoard.Full)
                            break;

                        SelectedBoard.Selected = true;
                        inputStage = InputStage.PlayOnBoard;
                        break;

                    case InputStage.PlayOnBoard:
                        if (SelectedBoard.Play(PlayerToPlay, PosToPlay))
                        {
                            PlayerToPlay = PlayerToPlay == State.Cross ? State.Nought : State.Cross;
                            Won = CheckForWin(out Winner);

                            SelectedBoard.Selected = false;
                            SelectedBoard = boards[PosToPlay.X, PosToPlay.Y];
                            if (SelectedBoard.Full)
                                inputStage = InputStage.ChooseBoard;
                            else
                                SelectedBoard.Selected = true;
                        }
                        break;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);
            spriteBatch.Begin();

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    boards[x, y].Draw(spriteBatch);

            for (int x = 1; x < 3; x++)
                spriteBatch.Draw(Globals.ColorTexture, new Rectangle(10 + (Board.BoardDimensions.X + Board.BoardPadding.X) * x - Board.BoardPadding.X / 2, 10, 1, Board.BoardDimensions.Y * 3 + Board.BoardPadding.Y * 2 + Board.BoardPadding.Y / 2 - 20), Won ? Color.Gray : Color.Black);
            for (int y = 1; y < 3; y++)
                spriteBatch.Draw(Globals.ColorTexture, new Rectangle(10, 10 + (Board.BoardDimensions.Y + Board.BoardPadding.Y) * y - Board.BoardPadding.Y / 2, Board.BoardDimensions.X * 3 + Board.BoardPadding.X * 2 + Board.BoardPadding.X / 2 - 20, 1), Won ? Color.Gray : Color.Black);
            
            if (Won)
            {
                spriteBatch.DrawString(Globals.text, "Player Won:", new Vector2(945, 435) - Globals.text.MeasureString("Player Won:") / 2, Color.Black);
                spriteBatch.DrawString(Globals.text, Winner == State.Cross ? "x" : "o", new Vector2(945, 455) - Globals.text.MeasureString(Winner == State.Cross ? "x" : "o") / 2, Color.Black);
            }
            else
            {
                spriteBatch.DrawString(Globals.text, "Next to play:", new Vector2(945, 435) - Globals.text.MeasureString("Next to play:") / 2, Color.Black);
                spriteBatch.DrawString(Globals.text, PlayerToPlay == State.Cross ? "x" : "o", new Vector2(945, 455) - Globals.text.MeasureString(PlayerToPlay == State.Cross ? "x" : "o") / 2, Color.Black);
            }
            spriteBatch.DrawString(Globals.text, "Press space to reset", new Vector2(945, 485) - Globals.text.MeasureString("Press space to reset") / 2, Color.Black);
            
            spriteBatch.End();
            base.Draw(gameTime);
        }

        void Reset()
        {
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    boards[x, y] = new Board(x, y);

            PlayerToPlay = State.Cross;
            inputStage = 0;

            Won = false;
        }

        Point KeyToPos(int Key)
        {
            switch (Key)
            {
                case 1:
                    return new Point(0, 2);
                case 2:
                    return new Point(1, 2);
                case 3:
                    return new Point(2, 2);
                case 4:
                    return new Point(0, 1);
                case 5:
                    return new Point(1, 1);
                case 6:
                    return new Point(2, 1);
                case 7:
                    return new Point(0, 0);
                case 8:
                    return new Point(1, 0);
                case 9:
                    return new Point(2, 0);
            }

            return new Point(-1);
        }

        bool CheckForWin(out State Winner)
        {
            Winner = State.Empty;

            bool[,] Crosses = new bool[3, 3];
            bool[,] Noughts = new bool[3, 3];
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                {
                    Crosses[x, y] = boards[x, y].Winner == State.Cross;
                    Noughts[x, y] = boards[x, y].Winner == State.Nought;
                }


            for (int x = 0; x < 3; x++)
                if (Crosses[x, 0] && Crosses[x, 1] && Crosses[x, 2])
                {
                    Winner = State.Cross;
                    WinType = x;
                    return true;
                }
            for (int y = 0; y < 3; y++)
                if (Crosses[0, y] && Crosses[1, y] && Crosses[2, y])
                {
                    Winner = State.Cross;
                    WinType = 3 + y;
                    return true;
                }
            if (Crosses[0, 0] && Crosses[1, 1] && Crosses[2, 2] || Crosses[0, 2] && Crosses[1, 1] && Crosses[2, 0])
            {
                Winner = State.Cross;
                WinType = 6 + (Crosses[0, 2] && Crosses[1, 1] && Crosses[2, 0] ? 1 : 0);
                return true;
            }


            for (int x = 0; x < 3; x++)
                if (Noughts[x, 0] && Noughts[x, 1] && Noughts[x, 2])
                {
                    Winner = State.Nought;
                    WinType = x;
                    return true;
                }
            for (int y = 0; y < 3; y++)
                if (Noughts[0, y] && Noughts[1, y] && Noughts[2, y])
                {
                    Winner = State.Nought;
                    WinType = 3 + y;
                    return true;
                }
            if (Noughts[0, 0] && Noughts[1, 1] && Noughts[2, 2] || Noughts[0, 2] && Noughts[1, 1] && Noughts[2, 0])
            {
                Winner = State.Nought;
                WinType = 6 + (Noughts[0, 2] && Noughts[1, 1] && Noughts[2, 0] ? 1 : 0);
                return true;
            }

            return false;
        }
    }
}
