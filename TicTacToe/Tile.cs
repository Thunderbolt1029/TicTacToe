using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TicTacToe
{
    internal struct Tile
    {
        public static Tile[,] tiles = new Tile[3, 3];

        int x, y;
        State state;
        public string OutString 
        { 
            get 
            {
                switch (state)
                {
                    case State.Cross:
                        return "X";
                    case State.Nought:
                        return "O";
                }

                return " ";
            } 
        }

        public Tile(int x, int y) 
        { 
            this.x = x;
            this.y = y;

            state = State.Empty;
        }

        public bool Play(State newState)
        {
            if (state != State.Empty || newState == State.Empty) return false;

            state = newState;

            return true;
        }
    }
}
