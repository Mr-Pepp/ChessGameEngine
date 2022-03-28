using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    class Moves
    {

        public static bool whiteTurn = true;
        public static bool enPassant = false;

        public static List<int[]> moveHistory = new List<int[]>();


        static ulong file_A = 9259542123273814144L;
        static ulong file_B = 4629771061636907072L;

        static ulong file_H = 72340172838076673L;
        static ulong file_G = 144680345676153346L;

        static ulong file_AB = file_A | file_B;
        static ulong file_GH = file_G | file_H;

        static ulong rank_1 = 255L;
        static ulong rank_2 = 65280L;

        static ulong rank_8 = 18374686479671623680L;
        static ulong rank_7 = 71776119061217280L;

        static ulong rank_4 = 4278190080L;
        static ulong rank_5 = 1095216660480L;

        static ulong centre = 103481868288L;

        static ulong kingSide = 1085102592571150095L;
        static ulong queenSide = 17361641481138401520L;

        static ulong whitePieces;
        static ulong blackPieces;

        static ulong emptySquares;

        

        public static void InitBitboards()
        {
            whitePieces = Board.wK | Board.wQ | Board.wR | Board.wB | Board.wN | Board.wP;
            blackPieces = Board.bK | Board.bQ | Board.bR | Board.bB | Board.bN | Board.bP;
            emptySquares = ~(whitePieces | blackPieces); 
        }

        //For when player selects a piece manually
        public static List<int> CheckLegalMoves(int piece, int squares)
        {
            ulong pieceLocation = Board.BinaryStringToBitboard(squares);

            List<int> legalSquares = new List<int>();
            ulong legalULong = 0L;

            //Check if piece is white
            if (((piece & Piece.White) == Piece.White) & whiteTurn)
            {
                switch (piece & 0b00111)
                {
                    case Piece.King:
                        //King legal moves bitboard
                        legalULong = LegalMoves_King(pieceLocation & Board.wK, whitePieces);
                        break;

                    case Piece.Pawn:
                        //White pawn legal moves bitboard
                        legalULong = LegalMoves_WPawn(pieceLocation & Board.wP);
                        break;

                    case Piece.Knight:
                        //Knight legal moves bitboard
                        legalULong = LegalMoves_Knight(pieceLocation & Board.wN, whitePieces);
                        break;

                    case Piece.Bishop:
                        //Bishop legal moves bitboard
                        legalULong = LegalMoves_Bishop(pieceLocation & Board.wB, whitePieces, blackPieces);
                        break;

                    case Piece.Rook:
                        //Rook legal moves bitboard
                        legalULong = LegalMoves_Rook(pieceLocation & Board.wR, whitePieces, blackPieces);
                        break;

                    case Piece.Queen:
                        //Queen legal moves bitboard
                        legalULong = LegalMoves_Queen(pieceLocation & Board.wQ, whitePieces, blackPieces);
                        break;

                }
            }
            else if (!whiteTurn) //Black piece
            {
                switch (piece & 0b00111)
                {
                    case Piece.King:
                        //King legal moves bitboard
                        legalULong = LegalMoves_King(pieceLocation & Board.bK, blackPieces);
                        break;

                    case Piece.Pawn:
                        //Black pawn legal moves
                        legalULong = LegalMoves_BPawn(pieceLocation & Board.bP);
                        break;

                    case Piece.Knight:
                        //Knight legal moves bitboard
                        legalULong = LegalMoves_Knight(pieceLocation & Board.bN, blackPieces);
                        break;

                    case Piece.Bishop:
                        //Bishop legal moves bitboard
                        legalULong = LegalMoves_Bishop(pieceLocation & Board.bB, blackPieces, whitePieces);
                        break;

                    case Piece.Rook:
                        //Rook legal moves bitboard
                        legalULong = LegalMoves_Rook(pieceLocation & Board.bR, blackPieces, whitePieces);
                        break;

                    case Piece.Queen:
                        //Queen legal moves bitboard
                        legalULong = LegalMoves_Queen(pieceLocation & Board.bQ, blackPieces, whitePieces);
                        break;
                }
            }

            //Append to list the legal squares
            for (int i = 0; i < 64; i++)
            {

                if (((legalULong >> i) & 1L) == 1L) { legalSquares.Add(63 - i); }
                
            }

            return legalSquares;
        }

        public static ulong LegalMoves_WPawn(ulong wP) //White Pawns
        {
            ulong legalMoves = 0L;

            //Captures top left
            legalMoves = legalMoves | ((wP << 9) & blackPieces & ~file_H);

            //Captures top right
            legalMoves = legalMoves | ((wP << 7) & blackPieces & ~file_A);

            //Pushing forward
            legalMoves = legalMoves | ((wP << 8) & emptySquares);

            //Opening two square push forward
            legalMoves = legalMoves | (wP << 16 & rank_4 & emptySquares & emptySquares << 8);

            return legalMoves;
        }

        public static ulong LegalMoves_BPawn(ulong bP) //Black Pawns
        {
            ulong legalMoves = 0L;

            //Captures bottom left
            legalMoves = legalMoves | ((bP >> 7) & whitePieces & ~file_H);

            //Captures bottom right
            legalMoves = legalMoves | ((bP >> 9) & whitePieces & ~file_A);

            //Pushing down
            legalMoves = legalMoves | ((bP >> 8) & emptySquares);

            //Opening two square push down
            legalMoves = legalMoves | (bP >> 16 & rank_5 & emptySquares & emptySquares >> 8);

            return legalMoves;
        }

        public static ulong LegalMoves_Knight(ulong N, ulong friendlyPieces) //Knights
        {
            ulong legalMoves = 0L;

            //Up: 1, Left: 2
            legalMoves = legalMoves | (N << 10 & ~file_GH & ~friendlyPieces);

            //Up: 2, Left: 1
            legalMoves = legalMoves | (N << 17 & ~file_H & ~friendlyPieces);

            //Up: 2, Left: 1
            legalMoves = legalMoves | (N << 15 & ~file_A & ~friendlyPieces);

            //Up: 1, Right: 2
            legalMoves = legalMoves | (N << 6 & ~file_AB & ~friendlyPieces);

            //Down: 1, Right: 2
            legalMoves = legalMoves | (N >> 10 & ~file_AB & ~friendlyPieces);

            //Down: 2, Right: 1
            legalMoves = legalMoves | (N >> 17 & ~file_A & ~friendlyPieces);

            //Down: 2, Left: 1
            legalMoves = legalMoves | (N >> 15 & ~file_H & ~friendlyPieces);

            //Down: 1, Left: 2
            legalMoves = legalMoves | (N >> 6 & ~file_GH & ~friendlyPieces);

            return legalMoves;
        }

        public static ulong LegalMoves_Rook(ulong R, ulong friendlyPieces, ulong enemyPieces) //Rooks
        {
            ulong legalMoves = 0L;

            //Have to run loop until there is a piece to the right of the rook

            //Until there is a white piece
            //Condition that must be hit is that 5+5

            //Moving Horizontally Right
            for (int i = 1; i < 8; i++)
            {
                //If outside the board
                if ((R >> i) == (R >> i & file_A))
                {
                    break;
                }

                else if(R >> i == (R >> i & friendlyPieces))
                {
                    break;
                }

                else if(R >> i == (R >> i & enemyPieces))
                {
                    legalMoves = legalMoves | R >> i;
                    break;
                }

                legalMoves = legalMoves | (R >> i & ~friendlyPieces);
            }

            //Moving Horizontally Left
            for (int i = 1; i < 8; i++)
            {

                //If outside the board
                if ((R << i) == (R << i & file_H))
                {
                    break;
                }

                if (R << i == (R << i & friendlyPieces))
                {
                    break;
                }
                else if (R << i == (R << i & enemyPieces))
                {
                    legalMoves = legalMoves | R << i;
                    break;
                }
                legalMoves = legalMoves | (R << i & ~friendlyPieces);
            }

            //Moving Vertically Down
            for (int i = 1; i < 8; i++)
            {
                //Waste less time because it cant go further down
                if (R >> (i * 8) == 0L)
                {
                    break;
                }
                else if (R >> (i * 8) == (R >> (i * 8) & friendlyPieces))
                {
                    break;
                }
                else if (R >> (i * 8) == (R >> (i * 8) & enemyPieces))
                {
                    legalMoves = legalMoves | R >> (i * 8);
                    break;
                }
                legalMoves = legalMoves | (R >> (i * 8) & ~friendlyPieces);
            }

            //Moving Vertically Up
            for (int i = 1; i < 8; i++)
            {
                if (R << (i * 8) == (R << (i * 8) & friendlyPieces))
                {
                    break;
                }
                else if (R << (i * 8) == (R << (i * 8) & enemyPieces))
                {
                    legalMoves = legalMoves | R << (i * 8);
                    break;
                }
                legalMoves = legalMoves | (R << (i * 8) & ~friendlyPieces);
            }

            return legalMoves;
        }

        public static ulong LegalMoves_Bishop(ulong B, ulong friendlyPieces, ulong enemyPieces) //Bishops
        {
            ulong legalMoves = 0L;

            //Have to run loop until there is a piece to the right of the bishop


            //Moving Diagonally Top-Left
            for (int i = 1; i < 8; i++)
            {
                //If out side the board top
                if (B << (i * 8) == 0L)
                {
                    break;
                }

                //If outside the board, left
                else if ((B << i) == (B << i & file_H))
                {
                    break;
                }

                else if (B << (i * 9) == (B << (i * 9) & friendlyPieces))
                {
                    break;
                }

                else if (B << (i * 9) == (B << (i * 9) & enemyPieces))
                {
                    legalMoves = legalMoves | B << (i * 9);
                    break;
                }

                legalMoves = legalMoves | (B << (i * 9) & ~friendlyPieces);
            }

            //Moving Diagonally Top-Right
            for (int i = 1; i < 8; i++)
            {
                //If out side the board top
                if (B << (i * 8) == 0L)
                {
                    break;
                }

                //If outside the board, right
                else if ((B >> i) == (B << i & file_A))
                {
                    break;
                }

                else if (B << (i * 7) == (B << (i * 7) & friendlyPieces))
                {
                    break;
                }

                else if (B << (i * 7) == (B << (i * 7) & enemyPieces))
                {
                    legalMoves = legalMoves | B << (i * 7);
                    break;
                }

                legalMoves = legalMoves | (B << (i * 7) & ~friendlyPieces);
            }

            //Moving Diagonally Bottom-Right
            for (int i = 1; i < 8; i++)
            {
                //If out side the board down
                if (B >> (i * 8) == 0L)
                {
                    break;
                }

                //If outside the board, right
                else if ((B >> i) == (B >> i & file_A))
                {
                    break;
                }

                else if (B >> (i * 9) == (B >> (i * 9) & friendlyPieces))
                {
                    break;
                }

                else if (B >> (i * 9) == (B >> (i * 9) & enemyPieces))
                {
                    legalMoves = legalMoves | B >> (i * 9);
                    break;
                }

                legalMoves = legalMoves | (B >> (i * 9) & ~friendlyPieces);
            }

            //Moving Diagonally Bottom-Left
            for (int i = 1; i < 8; i++)
            {
                //If out side the board down
                if (B >> (i * 8) == 0L)
                {
                    break;
                }

                //If outside the board, left
                else if ((B << i) == (B << i & file_H))
                {
                    break;
                }

                else if (B >> (i * 7) == (B >> (i * 7) & friendlyPieces))
                {
                    break;
                }

                else if (B >> (i * 7) == (B >> (i * 7) & enemyPieces))
                {
                    legalMoves = legalMoves | B >> (i * 7);
                    break;
                }

                legalMoves = legalMoves | (B >> (i * 7) & ~friendlyPieces);
            }

            return legalMoves;
        }


        public static ulong LegalMoves_Queen(ulong Q, ulong friendlyPieces, ulong enemyPieces) //Queen
        {
            ulong legalMoves = 0L;

            //Considering that the queen is both rook and bishop combined
            return legalMoves | LegalMoves_Bishop(Q, friendlyPieces, enemyPieces) | LegalMoves_Rook(Q, friendlyPieces, enemyPieces) ;
        }

        public static ulong LegalMoves_King(ulong K, ulong friendlyPieces) // King || Need to consider attacked squares
        {
            ulong legalMoves = 0L;

            //King move all around but only by one square
            legalMoves = legalMoves | K << 1 & ~friendlyPieces | K << 9 & ~friendlyPieces | K << 8 & ~friendlyPieces | K << 7 & ~friendlyPieces |
                K >> 1 & ~friendlyPieces | K >> 9 & ~friendlyPieces | K >> 8 & ~friendlyPieces | K >> 7 & ~friendlyPieces;

            return legalMoves;
        }
    }
}