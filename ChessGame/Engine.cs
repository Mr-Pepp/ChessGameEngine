using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    class Engine
    {

        //Material count
        const int pawnValue = 20;
        const int bishopValue = 60;
        const int knightValue = 56; // Knights are considered a bit worse than bishops
        const int queenValue = 180;
        const int rookValue = 100;


        //Positional value // Piece-square tables
        // White king positional
        readonly static int[] position_whiteKing = new int[64]
        {
            -25, -20, -20, -20, -20, -20, -20, -25,
            -25, -20, -20, -20, -20, -20, -20, -25,
            -25, -20, -20, -20, -20, -20, -20, -25,
            -25, -20, -20, -20, -20, -20, -20, -25,
            -25, -20, -20, -20, -20, -20, -20, -25,
            -25, -20, -20, -20, -20, -20, -20, -25,
            15, 15, 0, 0, 0, 0, 15, 15,
           15, 20, 10, -10, 0, 10, 20, 15
        };

        readonly static int[] position_whiteQueen = new int[64]
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

        readonly static int[] position_whiteRook = new int[64]
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

        readonly static int[] position_whiteBishop = new int[64]
        {
            -15, -5, -5, -5, -5, -5, -5, -15,
            -10, 0, 0, 0, 0, 0, 0, -10,
            -10, 0, 5, 10, 10, 5, 0, -10,
            -10, 5, 5, 10, 10, 5, 5, -10,
            -10, 0, 10, 10, 10, 10, 0, -10,
            -10, 10, 10, 10, 10, 10, 10, -10,
            -10, 10, 0, 0, 0, 0, 10, -10,
            -15, -5, -5, -5, -5, -5, -5, -15
        };

        readonly static int[] position_whiteKnight = new int[64]
        {
            -20, -7, -7, -7, -7, -7, -7, -20,
            -20, -10, 5, 0, 0, 0, -10, -20,
            -20, 0, 8, 12, 12, 8, 0, -20,
            -20, 5, 10, 12, 12, 10, 5, -20,
            -20, 0, 10, 12, 12, 10, 0, -20,
            -20, 5, 6, 15, 15, 6, 5, -20,
            -20, -20, 0, 5, 5, 0, -20, -20,
            -20, -7, -7, -7, -7, -7, -7, -20,
        };

        readonly static int[] position_whitePawn = new int[64]
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            45, 45, 45, 45, 45, 45, 45, 45,
            15, 15, 20, 22, 22, 20, 15, 15,
            5, 5, 12, 15, 15, 12, 5, 5,
            0, 0, 0, 12, 14, 0, 0, 0,
            5, 2, 5, 3, 3, 5, 2, 5,
            5, 5, 5, -5, -5, 5, 5, 5,
            0, 0, 0, 0, 0, 0, 0, 0
        };

        // Reverse white piece square tables (in Y-axis) for black evaluation
        readonly static int[] position_blackKing = new int[]
        {
            15, 25, 10, -10, 0, 10, 25, 15,
            15, 15, 0, 0, 0, 0, 15, 15,
            -25, -20, -20, -20, -20, -20, -20, -25,
            -25, -20, -20, -20, -20, -20, -20, -25,
            -25, -20, -20, -20, -20, -20, -20, -25,
            -25, -20, -20, -20, -20, -20, -20, -25,
            -25, -20, -20, -20, -20, -20, -20, -25,
            -25, -20, -20, -20, -20, -20, -20, -25           
        };

        readonly static int[] position_blackQueen = new int[64]
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

        readonly static int[] position_blackRook = new int[64]
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

        readonly static int[] position_blackBishop = new int[64]
        {
            -15, -5, -5, -5, -5, -5, -5, -15,
            -10, 10, 0, 0, 0, 0, 10, -10,
            -10, 10, 10, 10, 10, 10, 10, -10,
            -10, 0, 10, 10, 10, 10, 0, -10,
            -10, 5, 5, 10, 10, 5, 5, -10,
            -10, 0, 5, 10, 10, 5, 0, -10,
            -10, 0, 0, 0, 0, 0, 0, -10,
            -15, -5, -5, -5, -5, -5, -5, -15,
        };

        readonly static int[] position_blackKnight = new int[64]
        {
            -20, -7, -7, -7, -7, -7, -7, -20,
            -20, -20, 0, 5, 5, 0, -20, -20,
            -20, 5, 6, 15, 15, 6, 5, -20,
            -20, 0, 10, 12, 12, 10, 0, -20,
            -20, 5, 10, 12, 12, 10, 5, -20,
            -20, 0, 8, 12, 12, 8, 0, -20,
            -20, -10, 5, 0, 0, 0, -10, -20,
            -20, -7, -7, -7, -7, -7, -7, -20,
        };

        readonly static int[] position_blackPawn = new int[64]
        {
            0, 0, 0, 0, 0, 0, 0, 0,
            5, 5, 5, -5, -5, 5, 5, 5,
            5, 2, 5, 3, 3, 5, 2, 5,
            0, 0, 0, 12, 14, 0, 0, 0,
            5, 5, 12, 15, 15, 12, 5, 5,
            15, 15, 20, 22, 22, 20, 15, 15,
            45, 45, 45, 45, 45, 45, 45, 45,
            0, 0, 0, 0, 0, 0, 0, 0
        };


        // square-table for king
        readonly static int[] endGame_kingPosition = new int[64]
        {
            90, 75, 62, 55, 55, 62, 75, 90,
            75, 45, 35, 30, 30, 35, 45, 75,
            62, 35, 15, 12, 12, 15, 35, 62,
            55, 30, 12, 0, 0, 12, 30, 55,
            55, 30, 12, 0, 0, 12, 30, 55,
            62, 35, 15, 12, 12, 15, 35, 62,
            75, 45, 35, 30, 30, 35, 45, 75,
            90, 75, 62, 55, 55, 62, 75, 90,
        };


        // Use for evaluation
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

        public class MaxMove
        {
            public int max;
            public Board.MoveInfo move;

            public MaxMove(int _max, Board.MoveInfo _move)
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
        public static MaxMove NegaMax(int depth, int alpha, int beta, int negateSide)
        {
            if (depth == 0)
            {
                // Return evaluation of current position with a null move
                return new MaxMove(negateSide * Evaluation(), new Board.MoveInfo());
            }

            List<int> moves = Moves.GenerateGameMoves(Board.position);

            if (moves.Count == 1)
            {
                if ((moves[0] >> 15) == 0b01) // Checkmate
                {
                    // Mark massive evluation
                    return new MaxMove(negateSide * -99999, new Board.MoveInfo());
                }
                else if ((moves[0] >> 16) == 1) // Stalemate
                {
                    // Draw
                    return new MaxMove(0, new Board.MoveInfo());
                }
            }

            // Order lists with captures first for alpha beta pruning
            moves = OrderMoves(moves);

            MaxMove maxMove = new MaxMove(-99999, new Board.MoveInfo());

            foreach (int move in moves)
            {
                //Make into valid move format
                Board.MoveInfo actMove = Board.MoveFormat(move);

                // Make move
                Board.MakeMoveOnBoard(actMove);

                // Negative sign because it will alternate black and white turns
                // Current evaluation
                MaxMove evalMove = new MaxMove(-NegaMax(depth - 1, alpha, beta, negateSide).max, actMove);

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
            Position position = Board.position;

            // Check enemy pieces (white)
            int whiteWeight = position.bQCount * 3 + position.bRCount * 2 + position.bBCount * 2 + position.bNCount * 2 + position.bPCount + 1;  // Add one to avoid diving by 0

            // Check enemy pieces (black)
            int blackWeight = position.wQCount * 3 + position.wRCount * 2 + position.wBCount * 2 + position.wNCount * 2 + position.wPCount + 1;

            int evaluation = MaterialCountWhite() + PositioningWhite(whiteWeight) - MaterialCountBlack() - PositioningBlack(blackWeight) + EndGameKingCentre(whiteWeight, blackWeight);

            return evaluation;
        }


        static int PositioningWhite(int whiteWeight)
        {
            Position position = Board.position;
            int eval = 0;

            // Make less significant with less pieces on board (endgame)
            eval += (position_whiteKing[position.whiteKing[0]] / 10) * whiteWeight;

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

        static int EndGameKingCentre(int whiteWeight, int blackWeight)
        {
            Position position = Board.position;

            // The more centre the king is the better 
            // The weight of the evaluation on centralisation of the king will increase with fewer enemy pieces on board

            return endGame_kingPosition[position.blackKing[0]] / (blackWeight) - endGame_kingPosition[position.whiteKing[0]] / (whiteWeight);

        }

        static int PositioningBlack(int blackWeight)
        {
            Position position = Board.position;
            int eval = 0;

            // Make less significant with less pieces (endgame)
            eval += (position_blackKing[position.blackKing[0]] / 10) * blackWeight;
            
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
    }
}
