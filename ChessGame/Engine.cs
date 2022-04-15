using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    class Engine
    {

        //Material count
        const int pawnValue = 100;
        const int bishopValue = 300;
        const int knightValue = 280; // Knights are considered a bit worse than bishops
        const int queenValue = 900;
        const int rookValue = 500;

        const int posInfinity = int.MaxValue;
        const int negInfinity = int.MinValue;
        

        public Engine()
        {

        }


        //Random Moves - - Will return the move in int (Flag | To | From)
        public static int RandomMove(Position position)
        {
            List<int> moves = Moves.GenerateGameMoves(position);

            return moves[new Random().Next(moves.Count)];

        }

        //NegaMax search
        int NegaMax (int depth, Position position)
        {
            if (depth == 0)
            {
                return Evaluation();
            }

            List<int> moves = Moves.GenerateGameMoves(position);

            if (moves.Count == 1)
            {
                if ((moves[0] >> 15) == 0b01) // Checkmate
                {
                    // Worse scenario
                    return negInfinity;
                }
                else if ((moves[0] >> 16) == 1) // Stalemate
                {
                    // Draw
                    return 0;
                }
            }

            //Initiate at worse
            int max = negInfinity;

            foreach (int move in moves)
            {
                // Make move

                // Negative sign because it will alternate black and white turns
                // Current evaluation
                //int evaluation  = -NegaMax(depth - 1);
                // Get which evaluation is greater
                //max = Math.Max(evaluation, max);
                // Undo move

            }

            //return 
            return max;
        }


        public static int Evaluation()
        {
            int whiteMaterial = 0;
            int blackMaterial = 0;

            int evaluation = whiteMaterial - blackMaterial;

            return evaluation;
        }

        //Count the material
        static int MaterialCountWhite(ulong wQ, ulong wR, ulong wB, ulong wN, ulong wP)
        {
            int materialCount = 0;

            materialCount += BitboardCounter(wQ);
            materialCount += BitboardCounter(wR);
            materialCount += BitboardCounter(wB);
            materialCount += BitboardCounter(wN);
            materialCount += BitboardCounter(wP);

            return materialCount;
        }

        //Count the material
        static int MaterialCountBlack(ulong bQ, ulong bR, ulong bB, ulong bN, ulong bP)
        {
            int materialCount = 0;

            materialCount += BitboardCounter(bQ);
            materialCount += BitboardCounter(bR);
            materialCount += BitboardCounter(bB);
            materialCount += BitboardCounter(bN);
            materialCount += BitboardCounter(bP);

            return materialCount;
        }

        //Very inneficient
        static int BitboardCounter(ulong bitboard)
        {
            int count = 0;
            while (bitboard != 0)
            {
                bitboard = bitboard & (bitboard - 1);
                count++;
            }

            return count;
        }
    }
}
