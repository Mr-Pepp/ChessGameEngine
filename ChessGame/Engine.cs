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

        // Used for returning the best move and evaluation of move at once
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

        //NegaMax search
        public static MaxMove NegaMax(int depth, int alpha, int beta, int negateSide)
        {
            if (depth == 0)
            {
                // Return evaluation of current position with a null move
                return new MaxMove(negateSide * Evaluation(), new Board.MoveInfo());
            }

            // Generate all moves and store in the moves list
            List<int> moves = Moves.GenerateGameMoves(Board.position);

            // If there is only one move, there could be a checkmate or stalemate returned
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

            // Loop through each move in moves
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

                // If the evaluation is greater than the maxMove 
                if (evalMove.max > maxMove.max)
                {
                    // Set the new max move
                    maxMove = evalMove;
                }

                else if (evalMove.max < -9999) // Getting mated
                {
                    maxMove.move = evalMove.move; // Return best move
                }

                // Set alpha as the value that is greater from Alpha and evalMove
                alpha = Math.Max(alpha, evalMove.max);
                
                // If alpha is greater than or equal to beta
                if (alpha >= beta)
                {
                    // Return; prune line
                    return maxMove;
                }
            }

            //return best move and score
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

            // For each piece, evaluate their positioning based on the piece-square tables
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

            // For each piece, evaluate their positioning based on the piece-square tables
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
