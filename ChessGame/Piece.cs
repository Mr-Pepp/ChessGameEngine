using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessGame
{
    class Piece
    {
        //reference pieces
        //In binary, using an OR operator to be able to call either
        public const int None = 0b00000;   // 00000 b

        public const int King = 0b00001;   // 00001 b
        public const int Pawn = 0b00010;   // 00010 b
        public const int Knight = 0b00011; // 00011 b
        public const int Bishop = 0b00100; // 00100 b
        public const int Rook = 0b00101;   // 00101 b
        public const int Queen = 0b00110;  // 00110 b

        public const int White = 0b01000;  // 01000 b
        public const int Black = 0b10000;  // 10000 b


        public static Texture2D whiteKing;
        public static Texture2D whitePawn;
        public static Texture2D whiteKnight;
        public static Texture2D whiteBishop;
        public static Texture2D whiteRook;
        public static Texture2D whiteQueen;

        public static Texture2D blackKing;
        public static Texture2D blackPawn;
        public static Texture2D blackKnight;
        public static Texture2D blackBishop;
        public static Texture2D blackRook;
        public static Texture2D blackQueen;

        //dictionary for reference 
        static public Dictionary<int, Texture2D> pieceTextureDic;

        //Piece info
        public static int[] diagonalOffsets = { -9, -7, 7, 9 }; //top left, top right, bottom left, bottom right
        public static int[] directOffsets = { -1, 1, -8, 8 }; //left, right, top, bottom
        //public int[] maxPieceRange;

        public static int[] knightMoves = new int[] //First variable is to declare whether the moves are continous
        {
            (diagonalOffsets[0] + directOffsets[0]), (diagonalOffsets[0] + directOffsets[2]), (diagonalOffsets[1] + directOffsets[2]), (diagonalOffsets[1] + directOffsets[1]),
            (diagonalOffsets[3] + directOffsets[1]), (diagonalOffsets[3] + directOffsets[3]), (diagonalOffsets[2] + directOffsets[3]), (diagonalOffsets[2] + directOffsets[0])
        };

        public Piece() { }


        //legal moves
        public static List<int> checkLegalMoves(int piece, int currentSquare, Square[] squares)
        {
            //Squares that are legal for the pieces to step on
            List<int> legalSquares = new List<int>();

            int checkSquare = 0;

            //Gets just the piece without colour
            switch (piece & 0b00111)
            {
                case King:
                    //check how many moves allowed 
                    break;

                case Pawn:

                    break;

                case Knight:
                    //check squares that the piece can jump to

                    foreach (int offset in knightMoves)
                    {
                        //Square index to be checked
                        checkSquare = currentSquare + offset;

                        //If checkSquare is within the board
                        if (checkSquare > 0 && checkSquare < (squares.Length - 1))
                        {
                            //white piece
                            if ((piece & 0b11000) == White)
                            {
                                //Make sure the target square is not a friendly piece
                                if ((squares[checkSquare].piece & 0b11000) != White)
                                {
                                    //Append the square that is legal
                                    legalSquares.Add(checkSquare);
                                }
                            }

                            //black piece
                            else
                            {
                                //See whether the target square is a friendly piece
                                if ((squares[checkSquare].piece & 0b11000) != Black)
                                {
                                    //Append the square that is legal
                                    legalSquares.Add(checkSquare);
                                }
                            }
                        }
                    
                    }
                    break;

                case Bishop:

                    break;

                case Rook:

                    break;

                case Queen:

                    break;

            }

            return legalSquares;
        }
        

        private void directMoves(int maxMoveAmount, List<int> legalMoves)
        {

        }

    }
}
