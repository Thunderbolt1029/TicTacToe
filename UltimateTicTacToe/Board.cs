using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimateTicTacToe
{
    internal class Board
    {
        public static Point BoardDimensions = new Point(250, 250);
        public static Point BoardPadding = new Point(40, 40);

        Tile[,] tiles = new Tile[3, 3];

        Rectangle BoardContainer;

        public bool Won { get; private set; }
        State winner;
        public State Winner => winner;
        int WinType;

        public bool Selected;
        public bool Full
        {
            get
            {
                bool full = true;
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                        if (tiles[x, y].OutString == " ")
                        {
                            full = false;
                            break;
                        }
                    if (!full)
                        break;
                }

                return full || Won;
            }
        }

        public Board(int BoardX, int BoardY)
        {
            BoardContainer = new Rectangle(10 + (BoardDimensions.X + BoardPadding.X) * BoardX, 10 + (BoardDimensions.Y + BoardPadding.Y) * BoardY, BoardDimensions.X, BoardDimensions.Y);

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    tiles[x, y] = new Tile();
        }

        public bool Play(State PlayerToPlay, Point PosToPlay)
        {
            if (tiles[PosToPlay.X, PosToPlay.Y].Play(PlayerToPlay))
            {
                Won = CheckForWin(out winner);
                return true;
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Selected)
            {
                spriteBatch.Draw(Globals.ColorTexture, new Rectangle(BoardContainer.Left, BoardContainer.Top, BoardContainer.Width, 1), Color.Black);
                spriteBatch.Draw(Globals.ColorTexture, new Rectangle(BoardContainer.Left, BoardContainer.Top, 1, BoardContainer.Height), Color.Black);
                spriteBatch.Draw(Globals.ColorTexture, new Rectangle(BoardContainer.Left, BoardContainer.Bottom, BoardContainer.Width, 1), Color.Black);
                spriteBatch.Draw(Globals.ColorTexture, new Rectangle(BoardContainer.Right, BoardContainer.Top, 1, BoardContainer.Height), Color.Black);
            }

            // Draw board lines
            for (int x = 1; x < 3; x++)
                spriteBatch.Draw(Globals.ColorTexture, new Rectangle(BoardContainer.X + x * Tile.TileDimensions.X, BoardContainer.Y + 10, 1, BoardContainer.Height - 20), Won ? Color.White : Color.Black);
            for (int y = 1; y < 3; y++)
                spriteBatch.Draw(Globals.ColorTexture, new Rectangle(BoardContainer.X + 10, BoardContainer.Y + y * Tile.TileDimensions.Y, BoardContainer.Width - 20, 1), Won ? Color.White : Color.Black);

            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    spriteBatch.DrawString(Globals.text, tiles[x, y].OutString, new Vector2(BoardContainer.Left + (float)Tile.TileDimensions.X / 2 + Tile.TileDimensions.X * x, BoardContainer.Top + (float)Tile.TileDimensions.Y / 2 + Tile.TileDimensions.Y * y) - Globals.text.MeasureString(tiles[x, y].OutString) / 2, Won ? Color.White : Color.Black);

            if (Won)
            {
                DrawWinLine(spriteBatch);
                spriteBatch.DrawString(Globals.text, winner == State.Cross ? "x" : "o", new Vector2(BoardContainer.Left + (float)Tile.TileDimensions.X / 2 + Tile.TileDimensions.X, BoardContainer.Top + (float)Tile.TileDimensions.Y / 2 + Tile.TileDimensions.Y) - Globals.text.MeasureString(winner == State.Cross ? "x" : "x") * 2, Color.Black, 0f, new Vector2(), 3, SpriteEffects.None, 0f);
            }
        }

        bool CheckForWin(out State Winner)
        {
            Winner = State.Empty;

            bool[,] Crosses = new bool[3, 3];
            bool[,] Noughts = new bool[3, 3];
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                {
                    Crosses[x, y] = tiles[x, y].OutString == "x";
                    Noughts[x, y] = tiles[x, y].OutString == "o";
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

        void DrawWinLine(SpriteBatch spriteBatch)
        {
            int HVD = WinType / 3;
            int i = WinType % 3;

            switch (HVD)
            {
                case 0:
                    spriteBatch.Draw(Globals.ColorTexture, new Rectangle(BoardContainer.Left + Tile.TileDimensions.X / 2 + Tile.TileDimensions.X * i, BoardContainer.Top + 30, 1, BoardContainer.Height - 60), Won ? Color.White : Color.Black);
                    break;

                case 1:
                    spriteBatch.Draw(Globals.ColorTexture, new Rectangle(BoardContainer.Left + 30, BoardContainer.Top + Tile.TileDimensions.Y / 2 + Tile.TileDimensions.Y * i, BoardContainer.Width - 60, 1), Won ? Color.White : Color.Black);
                    break;

                case 2:
                    // diagonal
                    for (int j = 50; j < BoardContainer.Width - 50; j++)
                        spriteBatch.Draw(Globals.ColorTexture, new Rectangle(BoardContainer.Left + j, BoardContainer.Top + (i == 0 ? j : BoardContainer.Width - j), 1, 1), Won ? Color.White : Color.Black);
                    break;
            }
        }
    }
}
