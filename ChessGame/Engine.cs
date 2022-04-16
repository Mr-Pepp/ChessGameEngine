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


        public static int MiniMax(int depth, bool maximizingPlayer)
        {
            if (depth == 0)
            {
                return Evaluation();
            }

            //Generate moves
            List<int> moves = Moves.GenerateGameMoves(Board.position);

            if (moves.Count == 1) // potential checkmate or stalemate returned
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


            if (maximizingPlayer)
            {
                int maxEval = negInfinity;

                foreach (int move in moves)
                {
                    //Create move
                    Board.MoveInfo actMove = Board.MoveFormat(move);
                    //Make move
                    Board.MakeMoveOnBoard(actMove);

                    int eval = MiniMax(depth - 1, false);

                    //Undo move
                    Board.UndoMove(actMove);
                    maxEval = Math.Max(maxEval, eval);
                }
                return maxEval;
            }


            else
            {
                int minEval = posInfinity;

                foreach (int move in moves)
                {
                    //Create move
                    Board.MoveInfo actMove = Board.MoveFormat(move);
                    //Make move
                    Board.MakeMoveOnBoard(actMove);
                    int eval = MiniMax(depth -1, true);

                    //Undo move
                    Board.UndoMove(actMove);
                    minEval = Math.Min(minEval, eval);
                }
                return minEval; 
            }

        }



        //NegaMax search
        public static int NegaMax (int depth)
        {
            if (depth == 0)
            {
                return Evaluation();
            }

            List<int> moves = Moves.GenerateGameMoves(Board.position);

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
                
                //Make into valid move format
                Board.MoveInfo makeMove = Board.MoveFormat(move);


                // Make move
                Board.MakeMoveOnBoard(makeMove);
                // Negative sign because it will alternate black and white turns
                // Current evaluation
                int evaluation  = -NegaMax(depth - 1);
                // Get which evaluation is greater
                max = Math.Max(evaluation, max);
                // Undo move
                Board.UndoMove(Board.moveHistory.Pop());
            }

            //return 
            return max;
        }


        public static int Evaluation()
        {
            int whiteMaterial = 0;
            int blackMaterial = 0;

            //MaterialCountBlack 

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
