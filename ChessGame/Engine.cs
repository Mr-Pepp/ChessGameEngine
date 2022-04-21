using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    class Engine
    {

        //Material count
        const int pawnValue = 10;
        const int bishopValue = 30;
        const int knightValue = 28; // Knights are considered a bit worse than bishops
        const int queenValue = 90;
        const int rookValue = 50;


        //Positional value // Piece-square tables
        // White king positional
        static int[] position_whiteKing = new int[64]
        { 
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -20, -30, -30, -40, -40, -30, -30, -20,
            -10, -20, -20, -20, -20, -20, -20, -10,
            20, 20, 0, 0, 0, 0, 20, 20,
            20, 30, 10, 0, 0, 10, 30, 20
        };

        static int[] position_whiteQueen = new int[64]
        {
            -20, -10, -10, -5, -5, -10, -10, -20,
            -10, 0, 0, 0, 0, 0, 0, -10,
            -10, 0, 5, 5, 5, 5, 0, -10,
            -5, 0, 5, 5, 5, 5, 0, -5,
            0, 0, 5, 5, 5, 5, 5, -5,
            -10, 5, 5, 5, 5, 5, 0, -10,
            -10, 0, 5, 0, 0, 0, 0, -10,
            -20, -10, -10, -5, -5, -10, -10, -20
        };

        static int[] position_whiteRook = new int[64]
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            5, 15, 15, 15, 15, 15, 15, 5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            0, 0, 0, 5, 5, 0, 0, 0
        };

        static int[] position_whiteBishop = new int[64]
        {
            -20, -10, -10, -10, -10, -10, -10, -20,
            -10, 0, 0, 0, 0, 0, 0, -10,
            -10, 0, 5, 10, 10, 5, 0, -10,
            -10, 5, 5, 10, 10, 5, 5, -10,
            -10, 0, 10, 10, 10, 10, 0, -10,
            -10, 10, 10, 10, 10, 10, 10, -10,
            -10, 5, 0, 0, 0, 0, 5, -10,
            -20, -10, -10, -10, -10, -10, -10, -20
        };

        static int[] position_whiteKnight = new int[64]
        {
            -50, -40, -30, -30, -30, -30, -40, -50,
            -40, -20, 0, 0, 0, 0, -20, -40,
            -30, 0, 10, 15, 15, 10, 0, -30,
            -30, 5, 15, 20, 20, 15, 5, -30,
            -30, 0, 15, 20, 20, 15, 0, -30,
            -30, 5, 10, 15, 15, 10, 5, -30,
            -40, -20, 0, 5, 5, 0, -20, -40,
            -50, -40, -30, -30, -30, -30, -40, -50
        };

        static int[] position_whitePawn = new int[64]
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            50, 50, 50, 50, 50, 50, 50, 50,
            15, 15, 20, 30, 30, 20, 15, 15,
            5, 5, 10, 25, 25, 10, 5, 5,
            0, 0, 0, 20, 20, 0, 0, 0,
            5, -5, -10, 0, 0, -10, -5, 5,
            5, 10, 10, -20, -20, 10, 10, 5,
            0, 0, 0, 0, 0, 0, 0, 0
        };

        // Reverse white piece square tables (in Y-axis) for black evaluation
        static int[] position_blackKing = new int[]
        {
            20, 30, 10, 0, 0, 10, 30, 20,
            20, 20, 0, 0, 0, 0, 20, 20
            -10, -20, -20, -20, -20, -20, -20, -10,
            -20, -30, -30, -40, -40, -30, -30, -20,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30,
            -30, -40, -40, -50, -50, -40, -40, -30
        };

        static int[] position_blackQueen = new int[64]
        {
            -20, -10, -10, -5, -5, -10, -10, -20,
            -10, 0, 5, 0, 0, 0, 0, -10,
            -10, 5, 5, 5, 5, 5, 0, -10,
            0, 0, 5, 5, 5, 5, 5, -5,
            -5, 0, 5, 5, 5, 5, 0, -5,
            -10, 0, 5, 5, 5, 5, 0, -10,
            -10, 0, 0, 0, 0, 0, 0, -10,
            -20, -10, -10, -5, -5, -10, -10, -20
        };

        static int[] position_blackRook = new int[64]
        {
            0, 0, 0, 5, 5, 0, 0, 0,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            -5, 0, 0, 0, 0, 0, 0, -5,
            5, 15, 15, 15, 15, 15, 15, 5,
            0, 0, 0, 0, 0, 0, 0, 0
        };

        static int[] position_blackBishop = new int[64]
        {
            -20, -10, -10, -10, -10, -10, -10, -20,
            -10, 5, 0, 0, 0, 0, 5, -10,
            -10, 10, 10, 10, 10, 10, 10, -10,
            -10, 0, 10, 10, 10, 10, 0, -10,
            -10, 5, 5, 10, 10, 5, 5, -10,
            -10, 0, 5, 10, 10, 5, 0, -10,
            -10, 0, 0, 0, 0, 0, 0, -10,
            -20, -10, -10, -10, -10, -10, -10, -20
        };

        static int[] position_blackKnight = new int[64]
        {
            -50, -40, -30, -30, -30, -30, -40, -50,
            -40, -20, 0, 5, 5, 0, -20, -40,
            -30, 5, 10, 15, 15, 10, 5, -30,
            -30, 0, 15, 20, 20, 15, 0, -30,
            -30, 5, 15, 20, 20, 15, 5, -30,
            -30, 0, 10, 15, 15, 10, 0, -30,
            -40, -20, 0, 0, 0, 0, -20, -40,
            -50, -40, -30, -30, -30, -30, -40, -50
        };

        static int[] position_blackPawn = new int[64]
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            5, 10, 10, -20, -20, 10, 10, 5,
            5, -5, -10, 0, 0, -10, -5, 5,
            0, 0, 0, 20, 20, 0, 0, 0,
            5, 5, 10, 25, 25, 10, 5, 5,
            15, 15, 20, 30, 30, 20, 15, 15,
            50, 50, 50, 50, 50, 50, 50, 50,
            0, 0, 0, 0, 0, 0, 0, 0
        };




        const int posInfinity = int.MaxValue;
        const int negInfinity = int.MinValue;

        // For fetching the best engine move
        public static Board.MoveInfo engineMove;
        

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

            if (moves.Count == 1) // potential checkmate or stalemate returned
            {
                if ((moves[0] >> 15) == 0b01) // Checkmate
                {
                    return 1;
                }
                else if ((moves[0] >> 16) == 1) // Stalemate
                {
                    // Draw
                    return 1;
                }
            }

            foreach (int move in moves)
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

        public class maxMove
        {
            public int max;
            public Board.MoveInfo move;

            public maxMove(int _max, Board.MoveInfo _move)
            {
                max = _max;
                move = _move;
            }
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

                    if (maxEval < eval) // Evaluate is greater
                    {
                        maxEval = Math.Max(maxEval, eval);
                        engineMove = actMove;
                    }
                    
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
                    int eval = MiniMax(depth - 1, true);

                    if (eval > 5000)
                    {
                        System.Diagnostics.Debug.WriteLine(depth);
                        System.Diagnostics.Debug.WriteLine(actMove.piece);
                    }

                    //Undo move
                    Board.UndoMove(actMove);

                    if (minEval > eval) // Evaluate is lesser
                    {
                        minEval = Math.Max(minEval, eval);
                        engineMove = actMove;
                    }
                }
                return minEval; 
            }

        }


        /*
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
                    System.Diagnostics.Debug.WriteLine("mate possible");
                    return -9999999;
                }
                else if ((moves[0] >> 16) == 1) // Stalemate
                {
                    // Draw
                    return 0;
                }
            }

            //Initiate at worse
            int max = -9999999;

            foreach (int move in moves)
            {
                //Make into valid move format
                Board.MoveInfo actMove = Board.MoveFormat(move);

                // Make move
                Board.MakeMoveOnBoard(actMove);

                // Negative sign because it will alternate black and white turns
                // Current evaluation
                int evaluation  = -NegaMax(depth - 1);
                // Get which evaluation is greater
                if (evaluation > max)
                {
                    if (evaluation > 9999)
                    {
                        System.Diagnostics.Debug.WriteLine(evaluation);
                        System.Diagnostics.Debug.WriteLine(actMove.piece);
                    }
                    max = evaluation;
                    if (depth == 4) // Original depth
                    {
                        engineMove = actMove;
                    }
                }

                // Undo move
                Board.UndoMove(actMove);
            }

            //return 
            return max;
        }*/

        /*
        //NegaMax search
        public static maxMove NegaMax(int depth)
        {
            if (depth == 0)
            {
                // Return evaluation of current position with a null move
                return new maxMove(Evaluation(), new Board.MoveInfo());
            }

            List<int> moves = Moves.GenerateGameMoves(Board.position);

            if (moves.Count == 1)
            {
                if ((moves[0] >> 15) == 0b01) // Checkmate
                {
                    // Worse scenario
                    return new maxMove(-999999, new Board.MoveInfo());
                }
                else if ((moves[0] >> 16) == 1) // Stalemate
                {
                    // Draw
                    return new maxMove(0, new Board.MoveInfo());
                }
            }

            maxMove maxMove = new maxMove(-999999, new Board.MoveInfo());

            foreach (int move in moves)
            {
                //Make into valid move format
                Board.MoveInfo actMove = Board.MoveFormat(move);

                // Make move
                Board.MakeMoveOnBoard(actMove);

                // Negative sign because it will alternate black and white turns
                // Current evaluation
                maxMove evalMove = new maxMove(-NegaMax(depth - 1).max, actMove);

                // Undo move
                Board.UndoMove(actMove);

                if (evalMove.max > maxMove.max)
                {
                    maxMove = evalMove;
                }
            }

            //return 
            return maxMove;
        }*/


        /*
        //NegaMax search
        public static int NegaMax(int depth)
        {
            if (depth == 0)
            {
                // Return evaluation of current position with a null move
                return Evaluation();
            }

            List<int> moves = Moves.GenerateGameMoves(Board.position);

            if (moves.Count == 1)
            {
                if ((moves[0] >> 15) == 0b01) // Checkmate
                {
                    // Worse scenario
                    return -999999;
                }
                else if ((moves[0] >> 16) == 1) // Stalemate
                {
                    // Draw
                    return 0;
                }
            }

            int max = -99999;

            foreach (int move in moves)
            {
                //Make into valid move format
                Board.MoveInfo actMove = Board.MoveFormat(move);

                // Make move
                Board.MakeMoveOnBoard(actMove);

                // Negative sign because it will alternate black and white turns
                // Current evaluation
                int eval = -NegaMax(depth - 1);

                // Undo move
                Board.UndoMove(actMove);

                if (eval > max)
                {
                    max = eval;
                    engineMove = actMove;
                }
            }

            //return 
            return max;
        }
        */




        //NegaMax search
        public static maxMove NegaMax(int depth, int alpha, int beta)
        {
            if (depth == 0)
            {
                // Return evaluation of current position with a null move
                return new maxMove(Evaluation(), new Board.MoveInfo());
            }

            List<int> moves = Moves.GenerateGameMoves(Board.position);

            if (moves.Count == 1)
            {
                if ((moves[0] >> 15) == 0b01) // Checkmate
                {
                    // Worse scenario
                    return new maxMove(-999999, new Board.MoveInfo());
                }
                else if ((moves[0] >> 16) == 1) // Stalemate
                {
                    // Draw
                    return new maxMove(0, new Board.MoveInfo());
                }
            }

            // Order lists with captures first for alpha beta pruning
            moves = OrderMoves(moves);

            maxMove maxMove = new maxMove(-99999, new Board.MoveInfo());

            foreach (int move in moves)
            {
                //Make into valid move format
                Board.MoveInfo actMove = Board.MoveFormat(move);

                // Make move
                Board.MakeMoveOnBoard(actMove);

                // Negative sign because it will alternate black and white turns
                // Current evaluation
                maxMove evalMove = new maxMove(-NegaMax(depth - 1, alpha, beta).max, actMove);

                // Undo move
                Board.UndoMove(actMove);



                if (evalMove.max > maxMove.max)
                {
                    maxMove = evalMove;
                }

                else if (evalMove.max < -9999) // Getting mated
                {
                    maxMove.move = evalMove.move; // Return move
                }

                alpha = Math.Max(alpha, evalMove.max);
                
                if (alpha >= beta)
                {
                    return maxMove;
                }
            }

            //return 
            return maxMove;
        }



        static List<int> OrderMoves(List<int> moves)
        {
            List<int> captures = new List<int>(); // list with captures

            List<int> tempMoves = moves; // Will be list without captures

            // Order with captures first as most likely to be the good moves
            foreach (int move in moves.ToArray())
            {
                if (move >> 12 == (int)Moves.Flag.Captures)
                {
                    // pop from list and append to new list
                    tempMoves.Remove(move);
                    captures.Add(move);
                }
            }
            // Join list together at the end
            captures.AddRange(tempMoves);

            // retured order captured list so that captures are first and then come the other moves
            return captures;
        }




        public static int Evaluation()
        {

            int evaluation = MaterialCountWhite() + PositioningWhite() - MaterialCountBlack() - PositioningBlack();

            return evaluation;
        }


        static int PositioningWhite()
        {
            Position position = Board.position;
            int eval = 0; 

            foreach (int squareIndex in position.whiteQueen)
            {
                eval += position_whiteQueen[squareIndex];
            }
            foreach (int squareIndex in position.whiteRook)
            {
                eval += position_whiteRook[squareIndex];
            }
            foreach (int squareIndex in position.whiteBishop)
            {
                eval += position_whiteBishop[squareIndex];
            }
            foreach (int squareIndex in position.whiteKnight)
            {
                eval += position_whiteKnight[squareIndex];
            }
            foreach (int squareIndex in position.whitePawn)
            {
                eval += position_whitePawn[squareIndex];
            }

            return eval;
        }

        static int PositioningBlack()
        {
            Position position = Board.position;
            int eval = 0;

            foreach (int squareIndex in position.blackQueen)
            {
                eval += position_blackQueen[squareIndex];
            }
            foreach (int squareIndex in position.whiteRook)
            {
                eval += position_blackRook[squareIndex];
            }
            foreach (int squareIndex in position.blackBishop)
            {
                eval += position_blackBishop[squareIndex];
            }
            foreach (int squareIndex in position.blackKnight)
            {
                eval += position_blackKnight[squareIndex];
            }
            foreach (int squareIndex in position.blackPawn)
            {
                eval += position_blackPawn[squareIndex];
            }

            return eval;
        }


        //Count the material
        static int MaterialCountWhite()
        {
            Position position = Board.position;

            return position.wQCount * queenValue + position.wRCount * rookValue + position.wBCount * bishopValue + 
                position.wNCount * knightValue + position.wPCount * pawnValue;
        }

        //Count the material
        static int MaterialCountBlack()
        {
            Position position = Board.position;

            return position.bQCount * queenValue + position.bRCount * rookValue + position.bBCount * bishopValue +
                position.bNCount * knightValue + position.bPCount * pawnValue;
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
