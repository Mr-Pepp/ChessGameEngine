using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    class Moves
    {

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
            if ((piece & Piece.White) == Piece.White)
            {
                switch (piece & 0b00111)
                {
                    case Piece.King:
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

                        break;

                    case Piece.Rook:

                        break;

                    case Piece.Queen:

                        break;

                }
            }
            else //Black piece
            {
                switch (piece & 0b00111)
                {
                    case Piece.King:
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

                        break;

                    case Piece.Rook:

                        break;

                    case Piece.Queen:

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
    }
}