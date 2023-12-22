using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace UltimateTicTacToe
{
    internal class Tile
    {
        public static Point TileDimensions => new Point(Board.BoardDimensions.X / 3, Board.BoardDimensions.Y / 3);

        State state;
        public string OutString 
        { 
            get 
            {
                switch (state)
                {
                    case State.Cross:
                        return "x";
                    case State.Nought:
                        return "o";
                }

                return " ";
            } 
        }

        public bool Play(State newState)
        {
            if (state != State.Empty || newState == State.Empty) return false;

            state = newState;

            return true;
        }
    }
}
