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

        
        //For testing the legal move generation amount
        public static int GenerationTest(int depth)
        {
            if (depth == 0)
            {
                // Reached end move; return the position as reached
                return 1;
            }

            List<int> moves = Moves.GenerateGameMoves(Board.position);
            int positions = 0;

            foreach(int move in moves)
            {
                //Create move
                Board.MoveInfo actMove = Board.MoveFormat(move);

                Board.MakeMoveOnBoard(actMove);
                positions += GenerationTest(depth - 1);
                Board.UndoMove(actMove);
            }

            return positions;
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
            int evaluation = WhiteMaterial() - BlackMaterial();

            return evaluation;
        }


        //Counting white material
        public static int WhiteMaterial()
        {
            Position position = Board.position;
            return (position.wQCount * queenValue) + (position.wRCount * rookValue) +
                (position.wBCount * bishopValue) + (position.wNCount * knightValue) + (position.wPCount * pawnValue);
        }

        //Counting black material
        public static int BlackMaterial()
        {
            Position position = Board.position;
            return (position.bQCount * queenValue) + (position.bRCount * rookValue) + 
                (position.bBCount * bishopValue) + (position.bNCount * knightValue) + (position.bPCount * pawnValue);
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
