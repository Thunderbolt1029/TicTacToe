using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace TicTacToe
{
	public class Game1 : Game
	{
		const bool AI_ENABLED = false;
		const State AI_PLAYER = State.Nought;

		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		Texture2D ColorTexture;
		SpriteFont text;

		Rectangle BoardContainer;
		Point CellDimensions;

		State PlayerToPlay, Winner;
        bool Won;
		int WinType;

        Keys[] CurrentHeldButtons = new Keys[1], PreviousHeldButtons = new Keys[1];
		Keys[] ClickedButtons => CurrentHeldButtons.Except(PreviousHeldButtons).ToArray();

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			graphics.PreferredBackBufferWidth = 700;
			graphics.PreferredBackBufferHeight = 520;
		}

		protected override void Initialize()
		{
			BoardContainer = new Rectangle(10, 10, 500, 500);
			CellDimensions = new Point(BoardContainer.Width / 3, BoardContainer.Height / 3);

			Reset();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			ColorTexture = new Texture2D(GraphicsDevice, 1, 1);
			ColorTexture.SetData(new[] { Color.White });

			text = Content.Load<SpriteFont>("text");
		}

		protected override void Update(GameTime gameTime)
		{
            base.Update(gameTime);

            PreviousHeldButtons = CurrentHeldButtons;
			CurrentHeldButtons = Keyboard.GetState().GetPressedKeys();

			if (ClickedButtons.Contains(Keys.Escape))
				Exit();
            else if (ClickedButtons.Contains(Keys.Space))
			{
                Reset();
                return;
            }

            if (Won)
				return;


            if (AI_ENABLED && PlayerToPlay == AI_PLAYER)
            {
                Point ThePlay = AI.MiniMax(Tile.tiles, PlayerToPlay);
                Tile.tiles[ThePlay.X, ThePlay.Y].Play(PlayerToPlay);

                PlayerToPlay = PlayerToPlay == State.Cross ? State.Nought : State.Cross;

                Won = CheckForWin(out Winner);

				return;
            }


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

			if (PosToPlay != new Point(-1) && Tile.tiles[PosToPlay.X, PosToPlay.Y].Play(PlayerToPlay))
			{
				PlayerToPlay = PlayerToPlay == State.Cross ? State.Nought : State.Cross;

				Won = CheckForWin(out Winner);

				return;
			}

		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.LightGray);
			spriteBatch.Begin();

			// Draw board lines
			for (int x = 1; x < 3; x++)
				spriteBatch.Draw(ColorTexture, new Rectangle(BoardContainer.X + x * CellDimensions.X, BoardContainer.Y, 1, BoardContainer.Height), Color.Black);
			for (int y = 1; y < 3; y++)
				spriteBatch.Draw(ColorTexture, new Rectangle(BoardContainer.Y, BoardContainer.Y + y * CellDimensions.Y, BoardContainer.Width, 1), Color.Black);

			for (int x = 0; x < 3; x++)
				for (int y = 0; y < 3; y++)
					spriteBatch.DrawString(text, Tile.tiles[x, y].OutString, new Vector2(BoardContainer.Left + CellDimensions.X / 2 + CellDimensions.X * x, BoardContainer.Top + CellDimensions.Y / 2 + CellDimensions.Y * y) - text.MeasureString(Tile.tiles[x, y].OutString) / 2, Color.Black);

			if (Won)
				if (Winner == State.Empty)
                    spriteBatch.DrawString(text, "Draw", new Vector2(600, 260) - text.MeasureString("Draw") / 2, Color.Black);
				else
				{
					spriteBatch.DrawString(text, "Player Won:", new Vector2(600, 250) - text.MeasureString("Player Won:") / 2, Color.Black);
					spriteBatch.DrawString(text, Winner == State.Cross ? "X" : "O", new Vector2(600, 270) - text.MeasureString(Winner == State.Cross ? "X" : "O") / 2, Color.Black);

					DrawWinLine();
				}
            else
			{
				spriteBatch.DrawString(text, "Next to play:", new Vector2(600, 250) - text.MeasureString("Next to play:") / 2, Color.Black);
				spriteBatch.DrawString(text, PlayerToPlay == State.Cross ? "X" : "O", new Vector2(600, 270) - text.MeasureString(PlayerToPlay == State.Cross ? "X" : "O") / 2, Color.Black);
			}
            spriteBatch.DrawString(text, "Press space to reset", new Vector2(600, 300) - text.MeasureString("Press space to reset") / 2, Color.Black);


            spriteBatch.End();
			base.Draw(gameTime);
		}

		void Reset()
		{
			for (int x = 0; x < 3; x++)
				for (int y = 0; y < 3; y++)
					Tile.tiles[x, y] = new Tile(x, y);

			PlayerToPlay = State.Cross;

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

            bool Draw = true;
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    if (Tile.tiles[x, y].OutString == " ")
                        Draw = false;
			if (Draw == true) return true;


            bool[,] Crosses = new bool[3, 3];
			bool[,] Noughts = new bool[3, 3];
            for (int x = 0; x < 3; x++)
				for (int y = 0; y < 3; y++)
				{
					Crosses[x, y] = Tile.tiles[x, y].OutString == "X";
					Noughts[x, y] = Tile.tiles[x, y].OutString == "O";
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

		void DrawWinLine()
		{
			int HVD = WinType / 3;
			int i = WinType % 3;

			switch (HVD)
			{
				case 0:
					spriteBatch.Draw(ColorTexture, new Rectangle(BoardContainer.Left + CellDimensions.X / 2 + CellDimensions.X * i, BoardContainer.Top + 30, 1, BoardContainer.Height - 60), Color.Black);
					break;

				case 1:
                    spriteBatch.Draw(ColorTexture, new Rectangle(BoardContainer.Left + 30, BoardContainer.Top + CellDimensions.Y / 2 + CellDimensions.Y * i, BoardContainer.Width - 60, 1), Color.Black);
                    break;

				case 2:
					// diagonal
					for (int j = 50; j < BoardContainer.Width - 50; j++)
						spriteBatch.Draw(ColorTexture, new Rectangle(BoardContainer.Left + j, BoardContainer.Top + (i == 0 ? j : BoardContainer.Width - j), 1, 1), Color.Black);
					break;
			}
		}
	}
}
