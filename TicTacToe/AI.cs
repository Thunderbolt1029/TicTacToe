using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace TicTacToe
{
    internal static class AI
    {
        static Random RNG = new Random();

        // Cross is maximising player, Nought is minimising

        public static Point MiniMax(Tile[,] BoardState, State player)
        {
            if (player == State.Empty) throw new ArgumentException("Argument 'player' cannot be State.Empty");

            int[,] Scores = new int[3,3];
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                {
                    Scores[x, y] = player == State.Cross ? int.MinValue : int.MaxValue;

                    Tile[,] NewBoardState = CreateCopy(BoardState);
                    if (!NewBoardState[x, y].Play(player)) continue;

                    Scores[x, y] = EvaluateBoardScore(NewBoardState, SwitchPlayer(player), 10);
                }

            List<Point> ThePlays = new List<Point>();
            int Max = int.MinValue, Min = int.MaxValue;
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    if (player == State.Cross)
                    {
                        if (Scores[x, y] > Max)
                        {
                            Max = Scores[x, y];
                            ThePlays.Clear();
                            ThePlays.Add(new Point(x, y));
                        }
                        else if (Scores[x, y] == Max)
                        {
                            Max = Scores[x, y];
                            ThePlays.Add(new Point(x, y));
                        }
                    }
                    else
                    {
                        if (Scores[x, y] < Min)
                        {
                            Min = Scores[x, y];
                            ThePlays.Clear();
                            ThePlays.Add(new Point(x, y));
                        }
                        else if (Scores[x, y] == Min && RNG.Next(2) == 0)
                        {
                            Min = Scores[x, y];
                            ThePlays.Add(new Point(x, y));
                        }
                    }

            return ThePlays[RNG.Next(ThePlays.Count)];
        }

        /*
        function minimax(node, depth, maximizingPlayer) is
            if depth = 0 or node is a terminal node then
                return the heuristic value of node
            if maximizingPlayer then
                value := −∞
                for each child of node do
                    value := max(value, minimax(child, depth − 1, FALSE))
                return value
            else (* minimizing player *)
                value := +∞
                for each child of node do
                    value := min(value, minimax(child, depth − 1, TRUE))
                return value

        (* Initial call *)
        minimax(origin, depth, TRUE)
        */
        static int EvaluateBoardScore(Tile[,] BoardState, State player, int depth)
        {
            if (player == State.Empty) throw new ArgumentException("Argument 'player' cannot be State.Empty");

            if (depth == 0)
                throw new Exception("I didn't account for this");

            if (CheckForWin(BoardState, out State winner))
                switch (winner)
                {
                    case State.Empty:
                        return 0;
                    case State.Cross:
                        return 10;
                    case State.Nought:
                        return -10;
                }

            if (player == State.Cross)
            {
                int Score = int.MinValue;

                for (int x = 0; x < 3; x++)
                    for (int y = 0; y < 3; y++)
                    {
                        Tile[,] NewBoardState = CreateCopy(BoardState);
                        if (!NewBoardState[x, y].Play(player)) continue;

                        Score = Math.Max(Score, EvaluateBoardScore(NewBoardState, State.Nought, depth - 1));
                    }
                return Score;
            }
            else
            {
                int Score = int.MaxValue;

                for (int x = 0; x < 3; x++)
                    for (int y = 0; y < 3; y++)
                    {
                        Tile[,] NewBoardState = CreateCopy(BoardState);
                        if (!NewBoardState[x, y].Play(player)) continue;

                        Score = Math.Min(Score, EvaluateBoardScore(NewBoardState, State.Cross, depth - 1));
                    }
                return Score;
            }
        }

        static State SwitchPlayer(State player)
        {
            switch (player)
            {
                case State.Empty:
                    return State.Empty;
                case State.Cross:
                    return State.Nought;
                case State.Nought:
                    return State.Cross;
            }

            return State.Empty;
        }

        static bool CheckForWin(Tile[,] BoardState, out State Winner)
        {
            Winner = State.Empty;

            bool[,] Crosses = new bool[3, 3];
            bool[,] Noughts = new bool[3, 3];
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                {
                    Crosses[x, y] = BoardState[x, y].OutString == "X";
                    Noughts[x, y] = BoardState[x, y].OutString == "O";
                }


            for (int x = 0; x < 3; x++)
                if (Crosses[x, 0] && Crosses[x, 1] && Crosses[x, 2])
                {
                    Winner = State.Cross;
                    return true;
                }
            for (int y = 0; y < 3; y++)
                if (Crosses[0, y] && Crosses[1, y] && Crosses[2, y])
                {
                    Winner = State.Cross;
                    return true;
                }
            if (Crosses[0, 0] && Crosses[1, 1] && Crosses[2, 2] || Crosses[0, 2] && Crosses[1, 1] && Crosses[2, 0])
            {
                Winner = State.Cross;
                return true;
            }


            for (int x = 0; x < 3; x++)
                if (Noughts[x, 0] && Noughts[x, 1] && Noughts[x, 2])
                {
                    Winner = State.Nought;
                    return true;
                }
            for (int y = 0; y < 3; y++)
                if (Noughts[0, y] && Noughts[1, y] && Noughts[2, y])
                {
                    Winner = State.Nought;
                    return true;
                }
            if (Noughts[0, 0] && Noughts[1, 1] && Noughts[2, 2] || Noughts[0, 2] && Noughts[1, 1] && Noughts[2, 0])
            {
                Winner = State.Nought;
                return true;
            }

            bool Draw = true;
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    if (BoardState[x, y].OutString == " ")
                        Draw = false;
            if (Draw == true)
            {
                Winner = State.Empty;
                return true;
            }

            return false;
        }

        static Tile[,] CreateCopy(this Tile[,] arr)
        {
            Tile[,] newArr = new Tile[3, 3];
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                    newArr[x, y] = arr[x, y];
            return newArr;
        }
    }
}
