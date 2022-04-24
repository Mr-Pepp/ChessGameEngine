using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    class Moves
    {
        public static List<int[]> moveHistory = new List<int[]>();

        const ulong file_A = 9259542123273814144L;
        const ulong file_B = 4629771061636907072L;

        const ulong file_H = 72340172838076673L;
        const ulong file_G = 144680345676153346L;

        const ulong file_AB = file_A | file_B;
        const ulong file_GH = file_G | file_H;

        const ulong rank_1 = 255L;
        const ulong rank_2 = 65280L;

        const ulong rank_8 = 18374686479671623680L;
        const ulong rank_7 = 71776119061217280L;

        const ulong rank_4 = 4278190080L;
        const ulong rank_5 = 1095216660480L;

        const ulong centre = 103481868288L;

        const ulong kingSide = 1085102592571150095L;
        const ulong queenSide = 17361641481138401520L;

        //Board Corners
        // Top Left
        public const ulong TLCorner = file_A & rank_8;
        // Top Right
        public const ulong TRCorner = file_H & rank_8;
        // Bottom Right
        public const ulong BRCorner = file_H & rank_1;
        // Bottom Left
        public const ulong BLCorner = file_A & rank_1;

        //Magic numbers for castling
        public const ulong ksRookCorners = 72057594037927937L;
        public const ulong qsRookCorners = 9223372036854775936L;
        public const ulong rookCorners = ksRookCorners | qsRookCorners;
        // In between Rook + King (Does not include king and rook)
        //White
        const ulong qsKingRook_white = 112L;
        // Attacking for QS castles as can castle if the knight square is under attacked
        const ulong qsKingRookAttacking_white = 48L;
        const ulong ksKingRook_white = 6L;
        //Black
        const ulong qsKingRook_black = 8070450532247928832L;
        // Attacking for QS castles as can castle if the knight square is under attacked
        const ulong qsKingRookAttacking_black = 3458764513820540928L;
        const ulong ksKingRook_black = 432345564227567616L;
        //End up bitboard
        //White
        public const ulong qsCastleKing_white = 32L;
        public const ulong ksCastleKing_white = 2L;
        //Rook
        public const ulong qsCastleRook_white = qsCastleKing_white >> 1;
        public const ulong ksCastleRook_white = ksCastleKing_white << 1;
        //Black
        public const ulong qsCastleKing_black = 2305843009213693952L;
        public const ulong ksCastleKing_black = 144115188075855872L;
        //Rook
        public const ulong qsCastleRook_black = qsCastleKing_black >> 1;
        public const ulong ksCastleRook_black = ksCastleKing_black << 1;
        //Default King location
        public const ulong defaultKing_white = 8L;
        public const ulong defaultKing_black = 576460752303423488L;

        /* --No use for this algorithm because of move generation
        //For when player selects a piece manually
        public static List<int> CheckLegalMoves(int piece, int squares)
        {
            ulong pieceLocation = Board.BinaryStringToBitboard(squares);
            List<int> legalSquares = new List<int>();
            ulong legalULong = 0L;
            //Check if piece is white
            if (((piece & Piece.White) == Piece.White) & whiteTurn)
            {
                // SEE FOR CHECK (MAKE IT INTO A FUNCTION IN FUTURE)
                // [Knights, Bishops, Rooks, Queens, Pawns]
                ulong[] checks = KingHits(Board.wK, whiteTurn, whitePieces, blackPieces);
                //Useful for double checks (meaning you have to move the king)
                int checksCounter = 0;
                ulong combinedChecks = checks[0] | checks[1] | checks[2] | checks[3] | checks[4];
                foreach (ulong e in checks)
                {
                    if (e != 0L)
                    {
                        checksCounter++;
                    }
                }
                if (checksCounter > 0) // Is in check
                {
                    if (checksCounter > 1) // Double check (only king can move)
                    {
                        //Only king can move
                        if (Piece.King == (piece & 0b00111))
                        {
                            //King legal moves bitboard
                            legalULong = LegalMoves_King(pieceLocation & Board.wK, whitePieces, blackPieces, whiteTurn, true);
                        }
                        else
                        {
                            //Animation to show that king is in check (flashing red, sound)
                        }
                    }
                    else //Single check (can get blocked or captured)
                    {
                        //If no sliding piece then can't block with a piece
                        if ((checks[1] | checks[2] | checks[3]) != 0L)
                        {
                            //one solution is make the king go to the piece
                            //Draw to king
                            //loop from piece to king
                            //Check whether the piece is smaller or greater than the king bitboard
                            // depending on that loop by shifting by i each turn
                            //Based on the i result (from PIECE to KING),
                            //if divisible by 8 then that means it's a above / below the piece
                            //blockMask = 
                        }
                        //Only king can move
                        if (Piece.King == (piece & 0b00111))
                        {
                            //King legal moves bitboard
                            legalULong = LegalMoves_King(pieceLocation & Board.wK, whitePieces, blackPieces, whiteTurn, true);
                        }
                        else
                        {
                            //Animation to show that king is in check (flashing red, sound)
                        }
                    }
                }
                // NO CHECK
                else // Not in check, can play normally
                {
                    switch (piece & 0b00111)
                    {
                        case Piece.King:
                            //King legal moves bitboard
                            legalULong = LegalMoves_King(pieceLocation & Board.wK, whitePieces, blackPieces, whiteTurn, true);
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
            }
            else if (!whiteTurn) //Black piece
            {
                // SEE FOR CHECK (MAKE IT INTO A FUNCTION IN FUTURE)
                // [Knights, Bishops, Rooks, Queens, Pawns]
                ulong[] checks = KingHits(Board.bK, whiteTurn, blackPieces, whitePieces);
                //Useful for double checks (meaning you have to move the king)
                int checksCounter = 0;
                ulong combinedChecks = checks[0] | checks[1] | checks[2] | checks[3] | checks[4];
                foreach (ulong e in checks)
                {
                    if (e != 0L)
                    {
                        checksCounter++;
                    }
                }
                if (checksCounter > 0) // Is in check
                {
                    if (checksCounter > 1) // Double check (only king can move)
                    {
                        //Only king can move
                        if (Piece.King == (piece & 0b00111))
                        {
                            //King legal moves bitboard
                            legalULong = LegalMoves_King(pieceLocation & Board.bK, blackPieces, whitePieces, whiteTurn, true);
                        }
                        else
                        {
                            //Animation to show that king is in check (flashing red, sound)
                        }
                    }
                    else //Single check (can get blocked or captured)
                    {
                        //Only king can move
                        if (Piece.King == (piece & 0b00111))
                        {
                            //King legal moves bitboard
                            legalULong = LegalMoves_King(pieceLocation & Board.bK, blackPieces, whitePieces, whiteTurn, true);
                        }
                        else
                        {
                            //Animation to show that king is in check (flashing red, sound)
                        }
                    }
                }
                // NO CHECK
                else // Not in check, can play normally
                {
                    switch (piece & 0b00111)
                    {
                        case Piece.King:
                            //King legal moves bitboard
                            legalULong = LegalMoves_King(pieceLocation & Board.bK, blackPieces, whitePieces, whiteTurn, true);
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
            }
            //Append to list the legal squares
            for (int i = 0; i < 64; i++)
            {
                if (((legalULong >> i) & 1L) == 1L) { legalSquares.Add(63 - i); }
            }
            return legalSquares;
        }*/


        //Pawns for bitboard move generation
        public static ulong LegalMoves_WPawn(ulong wP, ulong enemyPieces, ulong emptySquares, ulong black_enPassantMask) //White Pawns
        {
            ulong legalMoves = 0L;
            enemyPieces |= black_enPassantMask;

            //Captures top left
            legalMoves |= ((wP << 9) & enemyPieces & ~file_H);

            //Captures top right
            legalMoves |= ((wP << 7) & enemyPieces & ~file_A);

            //Pushing forward
            legalMoves |= ((wP << 8) & emptySquares);

            //Opening two square push forward
            legalMoves |= (wP << 16 & rank_4 & emptySquares & emptySquares << 8);

            return legalMoves;
        }

        public static ulong LegalMoves_BPawn(ulong bP, ulong enemyPieces, ulong emptySquares, ulong white_enPassantMask) //Black Pawns
        {
            ulong legalMoves = 0L;
            enemyPieces |= white_enPassantMask;

            //Captures bottom left
            legalMoves |= ((bP >> 7) & enemyPieces & ~file_H);

            //Captures bottom right
            legalMoves |= ((bP >> 9) & enemyPieces & ~file_A);

            //Pushing down
            legalMoves |= ((bP >> 8) & emptySquares);

            //Opening two square push down
            legalMoves |= (bP >> 16 & rank_5 & emptySquares & emptySquares >> 8);

            return legalMoves;
        }

        public static ulong LegalMoves_Knight(ulong N, ulong friendlyPieces) //Knights
        {
            ulong legalMoves = 0L;

            //Up: 1, Left: 2
            legalMoves |= (N << 10 & ~file_GH & ~friendlyPieces);

            //Up: 2, Left: 1
            legalMoves |= (N << 17 & ~file_H & ~friendlyPieces);

            //Up: 2, Left: 1
            legalMoves |= (N << 15 & ~file_A & ~friendlyPieces);

            //Up: 1, Right: 2
            legalMoves |= (N << 6 & ~file_AB & ~friendlyPieces);

            //Down: 1, Right: 2
            legalMoves |= (N >> 10 & ~file_AB & ~friendlyPieces);

            //Down: 2, Right: 1
            legalMoves |= (N >> 17 & ~file_A & ~friendlyPieces);

            //Down: 2, Left: 1
            legalMoves |= (N >> 15 & ~file_H & ~friendlyPieces);

            //Down: 1, Left: 2
            legalMoves |= (N >> 6 & ~file_GH & ~friendlyPieces);

            return legalMoves;
        }

        public static ulong LegalMoves_Rook(ulong R, ulong friendlyPieces, ulong enemyPieces) //Rooks
        {

            ulong legalMoves = 0L;
            //Replace bitboard since it gets removed
            ulong fixedR = R;


            //Have to run loop until there is a piece to the right of the rook

            //Moving Horizontally Right
            for (int i = 1; i < 8; i++)
            {
                //If outside of bounds on file A, remove the appropriate rook bit(s)
                R &= ~(file_A << i);
                //If friendly piece in the way, remove the appropriate rook bit(s)
                R &= ~(friendlyPieces << i);

                //No more bits in bitboard
                if (R == 0L)
                {
                    break;
                }

                legalMoves |= (R >> i);

                //Does this operation after because we still want to capture the enemy piece
                R &= ~(enemyPieces << i);
            }

            //instantiate original bitboard
            R = fixedR;
            //Moving Horizontally Left
            for (int i = 1; i < 8; i++)
            {

                R &= ~(file_H >> i);
                R &= ~(friendlyPieces >> i);
                if (R == 0L)
                {
                    break;
                }
                legalMoves |= (R << i & ~friendlyPieces);
                R &= ~(enemyPieces >> i);
            }

            //instantiate original bitboard
            R = fixedR;
            //Moving Vertically Down
            for (int i = 1; i < 8; i++)
            {

                //Can't go further down
                R >>= (i * 8);
                R <<= (i * 8);

                //Friendly piece in the way
                R &= ~(friendlyPieces << (i * 8));

                //No more bits
                if (R == 0L)
                {
                    break;
                }

                //Add to legal moves
                legalMoves |= (R >> (i * 8) & ~friendlyPieces);

                //If there is an enemy piece there (after legal moves because we want to be able to capture the enemy piece)
                R &= ~(enemyPieces << (i * 8));
            }

            //instantiate original bitboard
            R = fixedR;
            //Moving Vertically Up
            for (int i = 1; i < 8; i++)
            {
                //Can't go further up
                R <<= (i * 8);
                R >>= (i * 8);
                //Friendly piece in the way
                R &= ~(friendlyPieces >> (i * 8));
                //No more bits
                if (R == 0L)
                {
                    break;
                }
                //Add to legal moves
                legalMoves |= (R << (i * 8) & ~friendlyPieces);
                //If there is an enemy piece there (after legal moves because we want to be able to capture the enemy piece)
                R &= ~(enemyPieces >> (i * 8));
            }

            return legalMoves;
        }

        public static ulong LegalMoves_Bishop(ulong B, ulong friendlyPieces, ulong enemyPieces) //Bishops
        {
            ulong legalMoves = 0L;
            //Replace bitboard since it gets removed
            ulong fixedB = B;

            //Moving Diagonally Top-Left
            for (int i = 1; i < 8; i++)
            {
                /*
                //If out side the board top
                if (B << (i * 8) == 0L)
                {
                    break;
                }
                //If outside the board, left
                else if ((B << i & file_H) != 0L)
                {
                    break;
                }
                else if ((B << (i * 9) & friendlyPieces) != 0L)
                {
                    break;
                }
                else if ((B << (i * 9) & enemyPieces) != 0L)
                {
                    legalMoves = legalMoves | B << (i * 9);
                    break;
                }*/

                //Outside the board (Top)
                B <<= (i * 8);
                B >>= (i * 8);
                //Outside the board (Left)
                B &= ~(file_H >> i);
                //Friendly piece in the way
                B &= ~(friendlyPieces >> (i * 9));
                //No bits in bitboard
                if (B == 0L)
                {
                    break;
                }
                legalMoves |= (B << (i * 9) & ~friendlyPieces);
                //Enemy piece in the way
                B &= ~(enemyPieces >> (i * 9));
            }

            //Restore bitboard
            B = fixedB;
            //Moving Diagonally Top-Right
            for (int i = 1; i < 8; i++)
            {
                //Outside the board (Top)
                B <<= (i * 8);
                B >>= (i * 8);
                //Outside the board (Right)
                //Shifting to left makes the file_A bitboard go up a file which means that it can miss the piece
                //So we have to shift the file_A bitboard down by 1 after the bitwise operation
                B &= ~((file_A << i) >> 8);
                //Friendly piece in the way
                B &= ~(friendlyPieces >> (i * 7));
                //No bits in bitboard
                if (B == 0L)
                {
                    break;
                }
                //Append legal moves
                legalMoves |= (B << (i * 7) & ~friendlyPieces);
                //Enemy piece in the way
                B &= ~(enemyPieces >> (i * 7));
            }

            //Restore bitboard
            B = fixedB;
            //Moving Diagonally Bottom-Right
            for (int i = 1; i < 8; i++)
            {
                //Outside the board (Bottom)
                B >>= (i * 8);
                B <<= (i * 8);
                //Outside the board (Right)
                B &= ~(file_A << i);
                //Friendly piece in the way
                B &= ~(friendlyPieces << (i * 9));
                //No bits in bitboard
                if (B == 0L)
                {
                    break;
                }
                //Append legal moves
                legalMoves |= (B >> (i * 9) & ~friendlyPieces);
                //Enemy piece in the way
                B &= ~(enemyPieces << (i * 9));
            }

            //Restore bitboard
            B = fixedB;
            //Moving Diagonally Bottom-Left
            for (int i = 1; i < 8; i++)
            {
                //Outside the board (Bottom)
                B >>= (i * 8);
                B <<= (i * 8);
                //Outside the board (Left)
                //Shifting to left makes the file_H bitboard go down a file which means that it can miss the piece
                //So we have to shift the file_H bitboard up by 1 after the bitwise operation
                B &= ~((file_H >> i) << 8);
                //Friendly piece in the way
                B &= ~(friendlyPieces << (i * 7));
                //No bits in bitboard
                if (B == 0L)
                {
                    break;
                }
                //Append legal moves
                legalMoves |= (B >> (i * 7) & ~friendlyPieces);
                //Enemy piece in the way
                B &= ~(enemyPieces << (i * 7));
            }

            return legalMoves;
        }

        public static ulong LegalMoves_Queen(ulong Q, ulong friendlyPieces, ulong enemyPieces) //Queen
        {
            ulong legalMoves = 0L;

            //Considering that the queen is both rook and bishop combined
            return legalMoves | LegalMoves_Bishop(Q, friendlyPieces, enemyPieces) | LegalMoves_Rook(Q, friendlyPieces, enemyPieces);
        }

        public static ulong LegalMoves_King(ulong K, ulong friendlyPieces, ulong enemyPieces, bool whiteTurn, bool legal, Position position, bool inCheck = false,
            int? whiteCastles = null, int? blackCastles = null) // King || Need to consider attacked squares
        {
            ulong legalMoves = 0L;
            ulong attackedSquares = 0L;

            //Check if legal since it may be generating for attackedSquares, so it avoids an overlap
            if (legal)
            {
                //Generate attacked squares
                if (whiteTurn) //White moving
                {
                    //Generate black moves to show the squares they are attacking
                    attackedSquares = AttackedSquares(position.bK, position.bQ, position.bR, position.bB, position.bN, position.bP, K, friendlyPieces, enemyPieces, whiteTurn, position);
                }
                else // Black moving
                {
                    //Generate white moves to show the squares they are attacking
                    attackedSquares = AttackedSquares(position.wK, position.wQ, position.wR, position.wB, position.wN, position.wP, K, friendlyPieces, enemyPieces, whiteTurn, position);
                }

                //Castling (king and rook must be in default position)
                if (!inCheck) //Can't castle whilst in check
                {
                    // Check side
                    if (whiteTurn) // White to play
                    {
                        // King and rook must be in default positions
                        if ((K & defaultKing_white) != 0 && (position.wR & (BLCorner | BRCorner)) != 0)
                        {
                            if ((whiteCastles & 0b01) == 1) // King side castles 0b01
                            {

                                //Nothing blocking or attacking squares
                                if (((friendlyPieces | enemyPieces | attackedSquares) & ksKingRook_white) == 0)
                                {
                                    //Can castle KS
                                    legalMoves |= ksCastleKing_white;
                                }
                            }
                            if (whiteCastles >> 1 == 1) // Queen side castles 0b10
                            {
                                //Nothing blocking or attacking squares
                                if (((friendlyPieces | enemyPieces | (attackedSquares & qsKingRookAttacking_white)) & qsKingRook_white) == 0)
                                {
                                    //Can castle QS
                                    legalMoves |= qsCastleKing_white;
                                }
                            }
                        }
                    }

                    else // Black to play
                    {
                        // King and rook must be in default positions
                        if ((K & defaultKing_black) != 0 && (position.bR & (TLCorner | TRCorner)) != 0)
                        {
                            if ((blackCastles & 0b01) == 1) // King side castles 0b01
                            {
                                //Nothing blocking or attacking squares
                                if (((friendlyPieces | enemyPieces | attackedSquares) & ksKingRook_black) == 0)
                                {
                                    //Can castle KS
                                    legalMoves |= ksCastleKing_black;
                                }
                            }
                            if (blackCastles >> 1 == 1) // Queen side castles 0b10
                            {
                                //Nothing blocking or attacking squares
                                if (((friendlyPieces | enemyPieces | (attackedSquares & qsKingRookAttacking_black)) & qsKingRook_black) == 0)
                                {
                                    //Can castle QS
                                    legalMoves |= qsCastleKing_black;
                                }
                            }
                        }
                    }
                }
            }

            //King move all around but only by one square
            legalMoves |= (K << 1 & ~friendlyPieces | K << 9 & ~friendlyPieces | K >> 7 & ~friendlyPieces) & ~file_H | K << 8 & ~friendlyPieces | (K << 7 & ~friendlyPieces |
                K >> 1 & ~friendlyPieces | K >> 9 & ~friendlyPieces) & ~file_A | K >> 8 & ~friendlyPieces;
            //Filter illegal moves
            legalMoves &= ~attackedSquares;

            return legalMoves;
        }




        //Generating each move from king and checking for same piece (Very efficient)

        //Generate from king and each hit we check what piece it is
        //This is played after an assumed move

        //Check for checks
        // [Knights, Bishops, Rooks, Queens, Pawns]
        public static ulong[] KingHits(ulong k, bool whiteTurn, ulong friendlyPieces, ulong enemyPieces, Position position)
        {
            //Initialise attacking piece bitboards
            ulong aN;
            ulong aR;
            ulong aB;
            ulong aQ;
            ulong aP;


            //LegalMoves_Knight(k, friendlyPieces) & (enemyPieces & (Board.wN | Board.bN) -- Method for not knowing the colour
            if (whiteTurn) //White to play (if check then black just checked white)
            {
                //Check from king square
                //We have to reverse enemyPieces and friendlyPieces because we are calculating the moves from the enemy's perspective
                aN = LegalMoves_Knight(k, friendlyPieces) & position.bN;
                aB = LegalMoves_Bishop(k, friendlyPieces, enemyPieces) & position.bB;
                aR = LegalMoves_Rook(k, friendlyPieces, enemyPieces) & position.bR;
                aQ = LegalMoves_Queen(k, friendlyPieces, enemyPieces) & position.bQ;

                //Locates pawn attacking from top left
                aP = ((k << 9) & ~file_H) & position.bP;
                //Locates pawn attacking from top right
                aP |= ((k << 7) & ~file_A) & position.bP;
            }

            else //Black to play (if check then white just checked black)
            {
                //Check from king square
                //We have to reverse enemyPieces and friendlyPieces because we are calculating the moves from the enemy's perspective
                aN = LegalMoves_Knight(k, friendlyPieces) & position.wN;
                aB = LegalMoves_Bishop(k, friendlyPieces, enemyPieces) & position.wB;
                aR = LegalMoves_Rook(k, friendlyPieces, enemyPieces) & position.wR;
                aQ = LegalMoves_Queen(k, friendlyPieces, enemyPieces) & position.wQ;

                //Locates pawn attacking from bottom left
                aP = ((k >> 7) & ~file_H) & position.wP;
                //Locates pawn attacking from bottom right
                aP |= ((k >> 9) & ~file_A) & position.wP;
            }

            // [Knights, Bishops, Rooks, Queens, Pawns]
            return new ulong[] { aN, aB, aR, aQ, aP };
        }



        /*
        void InCheck(ulong k, bool whiteTurn, ulong friendlyPieces, ulong enemyPieces, int piece, ulong pieceLocation)
        {
            // [Knights, Bishops, Rooks, Queens, Pawns]
            ulong[] checks = KingHits(Board.wK, whiteTurn, whitePieces, blackPieces);
            //Useful for double checks (meaning you have to move the king)
            int checksCounter = 0;
            ulong combinedChecks = checks[0] | checks[1] | checks[2] | checks[3] | checks[4];
            foreach (ulong e in checks)
            {
                if (e != 0L)
                {
                    checksCounter++;
                }
            }
            if ((checksCounter > 0) & whiteTurn) // Is in check
            {
                if (checksCounter > 1) // Double check (only king can move)
                {
                    //Only king can move
                    if (Piece.King == (piece & 0b00111))
                    {
                        //King legal moves bitboard
                        legalULong = LegalMoves_King(pieceLocation & Board.wK, whitePieces, blackPieces, whiteTurn, true);
                    }
                    else
                    {
                        //Animation to show that king is in check (flashing red, sound)
                    }
                }
                else
                {
                }
            }
        }
        */


        //Psuedo legal moves
        // Must check that after a move the board is still valid (King not in check)

        // Generate a bitboard of attacked squares (Must switch each king, each turn)

        public static ulong AttackedSquares(ulong eK, ulong eQ, ulong eR, ulong eB, ulong eN, ulong eP, ulong fK, ulong friendlyPieces, ulong enemyPieces, bool whiteTurn, Position position) // King || Need to consider attacked squares
        {
            //Important to note that all the bitboards for pieces provided are enemy bitboards, hence e at beginning
            //fK = Friendly king
            //eK = Enemy King

            ulong attackedSquares = 0L;
            ulong pawnAttackingSquares = 0L;

            //Friendly pieces no king for valid move generation
            friendlyPieces &= ~fK;

            //Pawn attack generation
            if (whiteTurn) //Still white turn, pawns go down because its for black pawns that attack
            {
                //attacks bottom left
                pawnAttackingSquares |= ((eP >> 7) & ~file_H);
                //attacks bottom right
                pawnAttackingSquares |= ((eP >> 9) & ~file_A);
            }
            else //Black turn pawns go up
            {
                //attacks top left
                pawnAttackingSquares |= ((eP << 9) & ~file_H);
                //attacks top right
                pawnAttackingSquares |= ((eP << 7) & ~file_A);
            }


            //Enemy pieces for attacked squares
            //Generate all moves for enemy pieces


            //Since we are generating moves for the enemy, we would assume that we would have to give enemyPieces bitboard for the friendlyPieces arguement, etc, but it would be invalid
            //because we have to show that our piece protects our own piece by attacking the square our own piece is on so the enemy king cant capture it.
            attackedSquares = attackedSquares | pawnAttackingSquares | LegalMoves_King(eK, friendlyPieces, enemyPieces, whiteTurn, false, position) | LegalMoves_Knight(eN, friendlyPieces) |
                LegalMoves_Rook(eR, friendlyPieces, enemyPieces) | LegalMoves_Bishop(eB, friendlyPieces, enemyPieces) | LegalMoves_Queen(eQ, friendlyPieces, enemyPieces);

            return attackedSquares;
        }


        //used for debugging and returning the squares for bitboard
        public static List<int> DebugSquares()
        {
            List<int> debugSquares = new List<int>();

            //Generate squares that are attacked by black pieces sicne since it's black's turn in this debugging example
            ulong squares = AttackedSquares(Board.position.bK, Board.position.bQ, Board.position.bR, Board.position.bB,
                Board.position.bN, Board.position.bP, Board.position.wK, Board.position.whitePieces, Board.position.blackPieces,
                true, Board.position);
            //ulong squares = PinnedPieces(Board.position.bK, Board.position.blackPieces, Board.position.whitePieces,
            //    Board.position.wR, Board.position.wB, Board.position.wQ);

            //Append to list the legal squares
            for (int i = 0; i < 64; i++)
            {

                if (((squares >> i) & 1L) == 1L)
                {
                    debugSquares.Add(63 - i);
                }

            }

            return debugSquares;
        }

        public enum Flag
        {
            /*
            * Flags (0b111):
            * 000 = Normal Move
            * 001 = Captures
            * 010 = Evasion (From check)
            * 011 = Promotion
            * 100 = Castles Queen Side
            * 101 = Castles King Side
            * 110 = En Passant
            * 111 = Double Push    
            */
            Normal_Move = 0b000, Captures = 0b001, Evasion = 0b010, Promotion = 0b011, Castles_QS = 0b100, Castles_KS = 0b101,
            En_Passant = 0b110, Double_Push = 0b111,
        }


        //Pinned Piece Solution:
        //Calculate all sliding piece moves (for each sliding piece)
        //Calculating sliding piece moves from the king (for each sliding piece)
        //If bitboards overlap with the friendly pieces then can't move that piece

        //Will generate pinned piece moves
        public static List<int> GenerateAllMoves(Position position, bool genKingMoves, ulong allowMask) // 0000 0000 0000 0000  to store: flag | to | from
        {
            List<int> legalSquares = new List<int>();

            ulong pieceLocation;
            int correctFrom;


            //Used for getting all the ulong bitboard move locations
            ulong legalULong;

            //Generate a pinned pieces mask to identify pinned pieces
            ulong pins;

            ulong horizontalPins;
            ulong verticalPins;

            ulong BLTRPins;
            ulong BRTLPins;

            //Used when checking each bit of the generated moves
            ulong legalULongBit;

            //Double push verify
            ulong doublePushVerifyMask;

            //En Passant verify
            ulong enPassantVerifyMask;

            //Castling Queen Side Verify
            ulong qsCastleVerifyMask;

            //Castling King Side Verify
            ulong ksCastleVerifyMask;

            //Captures
            ulong captureVerifyMask;

            //Promotion masks
            ulong promotionVerifyMask;

            //normal move flag
            ulong normalMoveVerifyMask;

            /*
            * Flags (0b111):
            * 000 = Normal Move
            * 001 = Captures
            * 010 = Evasion (From check)
            * 011 = Promotion
            * 100 = Castles Queen Side
            * 101 = Castles King Side
            * 110 = En Passant
            * 111 = Double Push  
            */

            int flag;

            if (position.whiteTurn) //White to play
            {
                //Generate for en passant pins
                ulong enemyPieces = position.blackPieces & ~(position.black_enPassantMask >> 8);

                //Generate mask horizontal and vertical pins
                horizontalPins = Pins_Horizontal(position.wK, position.whitePieces, enemyPieces, position.bR, position.bQ);
                verticalPins = Pins_Vertical(position.wK, position.whitePieces, position.blackPieces, position.bR, position.bQ);

                //Generate mask diagonal pins
                BLTRPins = Pins_BLTR(position.wK, position.whitePieces, position.blackPieces, position.bB, position.bQ);
                BRTLPins = Pins_BRTL(position.wK, position.whitePieces, position.blackPieces, position.bB, position.bQ);
                //Generate overall pin mask
                pins = horizontalPins | verticalPins | BRTLPins | BLTRPins;
            }

            else //Black to play
            {
                //Generate for en passant pins
                ulong enemyPieces = position.whitePieces & ~(position.white_enPassantMask << 8);

                //Generate mask horizontal and vertical pins
                horizontalPins = Pins_Horizontal(position.bK, position.blackPieces, enemyPieces, position.wR, position.wQ);
                verticalPins = Pins_Vertical(position.bK, position.blackPieces, position.whitePieces, position.wR, position.wQ);

                //Generate mask diagonal pins
                BLTRPins = Pins_BLTR(position.bK, position.blackPieces, position.whitePieces, position.wB, position.wQ);
                BRTLPins = Pins_BRTL(position.bK, position.blackPieces, position.whitePieces, position.wB, position.wQ);

                //Generate overall pin mask
                pins = horizontalPins | verticalPins | BRTLPins | BLTRPins;
            }


            //read bitboards as index
            //the bitwise operations will reverse the board so initially the board will be on blacks side
            void MakeMoves(int correctForm)
            {
                int i = 63 - correctForm;

                //Default to a normal move each loop
                flag = (int)Flag.Normal_Move;

                //Assign squares; //Assing to piece information // Piece Information: Colour | Piece | Location
                //**Implement flags and other piece information

                //Format legalULong
                legalULong = 0L;

                //Format double push location
                doublePushVerifyMask = 0L;

                //Format en passant verify mask
                enPassantVerifyMask = 0L;

                //Format castling verify masks
                qsCastleVerifyMask = 0L;
                ksCastleVerifyMask = 0L;

                // Format promotion verify mask;
                promotionVerifyMask = 0L;

                //Bool if there is a pin. (Used for knights)
                bool pin = false;

                //initiate friendlyPieces
                ulong friendlyPieces;

                //Set pinnedBlock to 0 initially, this is used to block calculations in the wrong directions by placing a friendly piece
                //in the way
                ulong pinnedBlock = 0L;

                //Since the index is reversed for bitboards
                correctFrom = 63 - i;

                //Convert the index to bitboard
                pieceLocation = Board.BinaryStringToBitboard(correctFrom);

                //pieceLocation offset for pinned pieces
                if (((pins >> i) & 1L) == 1L) //There is a pinned piece
                {
                    pin = true;


                    //Diagonal Pins first as they are more common
                    if (((BLTRPins >> i) & 1L) == 1L) //BLTR = Bottom Left, Top Right
                    {
                        //Pinned blocked
                        //Place the "friendly piece" mask everywhere but the pin location
                        pinnedBlock = pieceLocation << 1 | pieceLocation << 9 | pieceLocation << 8 |
                            pieceLocation >> 1 | pieceLocation >> 9 | pieceLocation >> 8;
                    }

                    else if (((BRTLPins >> i) & 1L) == 1L) //BRTL = Bottom Right, Top Left
                    {
                        //Pinned blocked
                        //Place the "friendly piece" mask everywhere but the pin location
                        pinnedBlock = pieceLocation << 1 | pieceLocation << 8 | pieceLocation << 7 |
                            pieceLocation >> 1 | pieceLocation >> 8 | pieceLocation >> 7;
                    }

                    //Horizontal Pin
                    else if (((horizontalPins >> i) & 1L) == 1L)
                    {
                        //Pinned blocked
                        //Place the "friendly piece" mask everywhere but the pin location
                        pinnedBlock = pieceLocation << 9 | pieceLocation << 8 | pieceLocation << 7 |
                            pieceLocation >> 9 | pieceLocation >> 8 | pieceLocation >> 7;

                        //Check for en passant pin
                        if ((pinnedBlock & (position.white_enPassantMask | position.black_enPassantMask)) != 0)
                        {
                            // There is an en passant pin, therefore block
                            pinnedBlock = (position.white_enPassantMask | position.black_enPassantMask);
                        }
                    }
                    else //Vertical pin
                    {
                        //Pinned blocked
                        //Place the "friendly piece" mask everywhere but the pin location
                        pinnedBlock = pieceLocation << 1 | pieceLocation << 9 | pieceLocation << 7 |
                            pieceLocation >> 1 | pieceLocation >> 9 | pieceLocation >> 7;
                    }
                }

                if (position.whiteTurn) // White to play
                {
                    captureVerifyMask = position.blackPieces;

                    //Assign friendlyPieces including pinnedPiece block
                    friendlyPieces = position.whitePieces | pinnedBlock;

                    if (((position.wP >> i) & 1L) == 1L) //Pawns first as they are most likely to appear since they are more common
                    {
                        //White pawn legal moves bitboard
                        legalULong = LegalMoves_WPawn(pieceLocation, position.blackPieces & ~pinnedBlock, position.emptySquares & ~pinnedBlock,
                            position.black_enPassantMask & ~pinnedBlock);

                        // Promotion veriy mask
                        promotionVerifyMask = legalULong & rank_8;

                        // En passant possibility
                        enPassantVerifyMask = legalULong & position.black_enPassantMask;

                        //Opening two square push forward, getting the pawn's double push location for en passant
                        doublePushVerifyMask = legalULong & rank_4;

                    }
                    else if (((position.wK >> i) & 1L) == 1L & genKingMoves)
                    {
                        //King legal moves bitboard
                        legalULong = LegalMoves_King(pieceLocation, friendlyPieces, position.blackPieces, position.whiteTurn, true, position, false,
                            position.whiteCastles, position.blackCastles);

                        // Add qs and ks castle mask for the flag
                        if ((defaultKing_white == position.wK) && (legalULong & qsCastleKing_white) != 0) // King has moved by two to left; QS castles
                        {
                            // Set mask for flag
                            qsCastleVerifyMask = qsCastleKing_white;
                        }
                        if ((defaultKing_white == position.wK) && (legalULong & ksCastleKing_white) != 0) // King has moved by two to right; KS castles
                        {
                            // Set mask for flag
                            ksCastleVerifyMask = ksCastleKing_white;
                        }
                    }
                    else if (((position.wQ >> i) & 1L) == 1L)
                    {
                        //Queen legal moves bitboard
                        legalULong = LegalMoves_Queen(pieceLocation, friendlyPieces, position.blackPieces);
                    }
                    else if (((position.wR >> i) & 1L) == 1L)
                    {
                        //Rook legal moves bitboard
                        legalULong = LegalMoves_Rook(pieceLocation, friendlyPieces, position.blackPieces);
                    }
                    else if (((position.wB >> i) & 1L) == 1L)
                    {
                        //Bishop legal moves bitboard
                        legalULong = LegalMoves_Bishop(pieceLocation, friendlyPieces, position.blackPieces);
                    }
                    else if (((position.wN >> i) & 1L) == 1L & !pin)
                    {
                        //Knight legal moves bitboard
                        legalULong = LegalMoves_Knight(pieceLocation, friendlyPieces);
                    }
                }

                else // Black to play
                {
                    //System.Diagnostics.Debug.WriteLine(pin);

                    captureVerifyMask = position.whitePieces;

                    //Assign friendlyPieces including pinnedPiece block
                    friendlyPieces = position.blackPieces | pinnedBlock;

                    if (((position.bP >> i) & 1L) == 1L) //Pawns first as they are most likely to appear since they are more common
                    {
                        //Black pawn legal moves
                        legalULong = LegalMoves_BPawn(pieceLocation, position.whitePieces & ~pinnedBlock, position.emptySquares & ~pinnedBlock,
                            position.white_enPassantMask & ~pinnedBlock);


                        // Promotion verify mask
                        promotionVerifyMask = legalULong & rank_1;

                        // En passant possibility
                        enPassantVerifyMask = legalULong & position.white_enPassantMask;

                        //Opening two square push forward, getting the pawn's double push location for en passant
                        doublePushVerifyMask = legalULong & rank_5;
                    }
                    else if (((position.bK >> i) & 1L) == 1L & genKingMoves)
                    {
                        //King legal moves bitboard
                        legalULong = LegalMoves_King(pieceLocation, friendlyPieces, position.whitePieces, position.whiteTurn, true, position, false,
                            position.whiteCastles, position.blackCastles);

                        // Add qs and ks castle mask for the flag
                        if ((defaultKing_black == position.bK) && (legalULong & qsCastleKing_black) != 0) // King has moved by two to left; QS castles
                        {
                            // Set mask for flag
                            qsCastleVerifyMask = qsCastleKing_black;
                        }
                        if ((defaultKing_black == position.bK) && (legalULong & ksCastleKing_black) != 0) // King has moved by two to right; KS castles
                        {
                            // Set mask for flag
                            ksCastleVerifyMask = ksCastleKing_black;
                        }
                    }
                    else if (((position.bQ >> i) & 1L) == 1L)
                    {
                        //Queen legal moves bitboard
                        legalULong = LegalMoves_Queen(pieceLocation, friendlyPieces, position.whitePieces);
                    }
                    else if (((position.bR >> i) & 1L) == 1L)
                    {
                        //Rook legal moves bitboard
                        legalULong = LegalMoves_Rook(pieceLocation, friendlyPieces, position.whitePieces);
                    }
                    else if (((position.bB >> i) & 1L) == 1L)
                    {
                        //Bishop legal moves bitboard
                        legalULong = LegalMoves_Bishop(pieceLocation, friendlyPieces, position.whitePieces);
                    }
                    else if (((position.bN >> i) & 1L) == 1L & !pin)
                    {
                        //Knight legal moves bitboard
                        legalULong = LegalMoves_Knight(pieceLocation, friendlyPieces);
                    }
                }


                //Set the normal move flag
                normalMoveVerifyMask = ~(captureVerifyMask | doublePushVerifyMask | enPassantVerifyMask |
                    ksCastleVerifyMask | qsCastleVerifyMask | promotionVerifyMask);


                //Filter blocked moves (For checks; blocking moves and capturing piece)
                legalULong &= allowMask;

                //Append to list the legal squares
                if (legalULong != 0L) // There are moves to append
                {
                    for (int y = 0; y < 64; y++)
                    {
                        // Assign bit
                        legalULongBit = legalULong >> y & 1L;

                        //flag | to | from
                        if (legalULongBit == 1L) // There is a bit
                        {
                            // Set appropriate flags for the move

                            // If the mvoe is considered a normal move
                            if (legalULongBit == (normalMoveVerifyMask >> y & 1L))
                            {
                                // Set normal move flag
                                flag = (int)Flag.Normal_Move;
                            }

                            else if (legalULongBit == (promotionVerifyMask >> y & 1L))
                            {
                                // Set promotion flag
                                flag = (int)Flag.Promotion;
                            }

                            //Check for capture (After promotion because can capture to promote)
                            else if (legalULongBit == (captureVerifyMask >> y & 1L))
                            {
                                // Set capture flag
                                flag = (int)Flag.Captures;
                            }

                            else if (legalULongBit == (enPassantVerifyMask >> y & 1L))
                            {
                                // Set En passant flag
                                flag = (int)Flag.En_Passant;
                            }

                            //Check for double push
                            else if (legalULongBit == (doublePushVerifyMask >> y & 1L)) // There was a double push
                            {
                                // Set double push flag
                                flag = (int)Flag.Double_Push;

                            }

                            // Check for king side castle
                            else if (legalULongBit == (ksCastleVerifyMask >> y & 1L))
                            {
                                // Set kingside castle flag
                                flag = (int)Flag.Castles_KS;
                            }

                            // Check for queen side castle
                            else if (legalULongBit == (qsCastleVerifyMask >> y & 1L))
                            {
                                // Set queenside castle flag
                                flag = (int)Flag.Castles_QS;
                            }

                            legalSquares.Add(flag << 12 | (63 - y) << 6 | correctFrom);
                        }

                    }
                }
            }


            // Make moves absed ont he index positions in the position piece lists
            if (position.whiteTurn) // White to play
            {
                foreach(int index in position.whiteKing)
                {
                    MakeMoves(index);
                }
                foreach (int index in position.whitePawn)
                {
                    MakeMoves(index);
                }
                foreach (int index in position.whiteQueen)
                {
                    MakeMoves(index);
                }
                foreach (int index in position.whiteRook)
                {
                    MakeMoves(index);
                }
                foreach (int index in position.whiteBishop)
                {
                    MakeMoves(index);
                }
                foreach (int index in position.whiteKnight)
                {
                    MakeMoves(index);
                }
            }

            else // Black to play
            {
                foreach (int index in position.blackKing)
                {
                    MakeMoves(index);
                }
                foreach (int index in position.blackPawn)
                {
                    MakeMoves(index);
                }
                foreach (int index in position.blackQueen)
                {
                    MakeMoves(index);
                }
                foreach (int index in position.blackRook)
                {
                    MakeMoves(index);
                }
                foreach (int index in position.blackBishop)
                {
                    MakeMoves(index);
                }
                foreach (int index in position.blackKnight)
                {
                    MakeMoves(index);
                }
            }



            return legalSquares;
        }


        public static List<int> GenerateGameMoves(Position position)
        {
            ulong[] checkArray;
            ulong legalULong;
            ulong pieceLocation;

            position.InitBitboards(); // Initiate bitboards

            List<int> legalSquares = new List<int>();

            //Check for checks
            if (position.whiteTurn) //White to move
            {
                //System.Diagnostics.Debug.WriteLine(whitePieces);

                // [Knights, Bishops, Rooks, Queens, Pawns]
                checkArray = KingHits(position.wK, position.whiteTurn, position.whitePieces, position.blackPieces, position);
            }
            else //Black to move
            {
                //System.Diagnostics.Debug.WriteLine(blackPieces);

                // [Knights, Bishops, Rooks, Queens, Pawns]
                checkArray = KingHits(position.bK, position.whiteTurn, position.blackPieces, position.whitePieces, position);
            }

            int checks = 0;

            foreach (ulong e in checkArray)
            {
                if (e != 0L) //There is a check
                {
                    checks++;
                }
            }

            if (checks > 0) //There is a check
            {
                if (checks == 1) //Single check, therefore king can move, checker can be captured, or the check can be blocked
                {

                    ulong allowedMask; // Used for filtering moves when generating moves (on blocked or king captures)
                    ulong kingLocation;

                    //Combine all the check locations for capturing the checker
                    ulong checkerMask = checkArray[0] | checkArray[1] | checkArray[2] | checkArray[3] | checkArray[4];

                    if (position.whiteTurn) // White to play
                    {
                        kingLocation = position.wK;

                        //Only king moves
                        legalULong = LegalMoves_King(kingLocation, position.whitePieces, position.blackPieces, position.whiteTurn, true, position, true);

                    }
                    else // Black to play
                    {
                        kingLocation = position.bK;

                        //Only king moves
                        legalULong = LegalMoves_King(kingLocation, position.blackPieces, position.whitePieces, position.whiteTurn, true, position, true);

                    }

                    //Mask for capturing the checker, to allow for the capture of the checker
                    allowedMask = checkerMask;

                    //Blocking the check
                    //If a sliding piece checked (Queen, Rook, Bishop), can block
                    //checkArray = [Knights, Bishops, Rooks, Queens, Pawns]

                    if ((checkArray[1] | checkArray[2] | checkArray[3]) != 0) //sliding piece
                    {
                        //slider: 0 = Bishop,   1 = Rook,   2 = Queen 
                        int sliderPiece;

                        // [Knights, Bishops, Rooks, Queens, Pawns]
                        //The checker is a sliding piece if conditions are met
                        if (checkArray[1] != 0) // Check by Bishop
                        {
                            sliderPiece = 0;
                        }
                        else if (checkArray[2] != 0) // Check by Rook
                        {
                            sliderPiece = 1;
                        }
                        else // Check by Queen
                        {
                            sliderPiece = 2;
                        }

                        //Creates a mask for squares in-between the check
                        allowedMask |= BetweenPieceRay(sliderPiece, kingLocation, checkerMask, position.emptySquares);

                    }


                    legalSquares = UlongTranslator(kingLocation, legalULong, legalSquares);

                    foreach (int e in GenerateAllMoves(position, false, allowedMask)) //loop through blocks and capturing checker moves to then append to the move list
                    {
                        legalSquares.Add(e); //Add to legal squares list
                    }

                    if (legalSquares.Count == 0) // Checkmate since no moves + in check
                    {
                        legalSquares.Add(1 << 15); //Adds a checkmate flag
                    }

                    return legalSquares;
                }

                else //Double check, therefore the king can only move
                {
                    if (position.whiteTurn) // White to move
                    {
                        pieceLocation = position.wK;

                        //Only king moves
                        legalULong = LegalMoves_King(pieceLocation, position.whitePieces, position.blackPieces, position.whiteTurn, true, position);
                    }
                    else // Black to move
                    {
                        pieceLocation = position.bK;

                        //Only king moves
                        legalULong = LegalMoves_King(pieceLocation, position.blackPieces, position.whitePieces, position.whiteTurn, true, position);
                    }

                    if (legalULong != 0L) // There are king moves available
                    {
                        //Add moves to the list
                        //Double check, therefore only king can move

                        legalSquares = UlongTranslator(pieceLocation, legalULong, legalSquares);

                        return legalSquares;

                    }

                    else // CHECKMATE since can't move king
                    {
                        legalSquares.Add(1 << 15);

                        // Return checkmate
                        return legalSquares;
                    }
                }
            }

            else //No check
            {
                //Generate moves normally
                legalSquares = GenerateAllMoves(position, true, ~(ulong)0L);
                if (legalSquares.Count == 0) // Stalemate
                {
                    legalSquares.Add(1 << 16);
                }

                return legalSquares;
            }
        }

        public static int GetBitboardIndex(ulong pieceLocation) // Used for single piece bitboard
        {
            int pieceIndex = 0;
            //Get the index of the bitboard piece
            for (int i = 0; i < 64; i++)
            {
                //Can set a condition equal to 1 without an AND bitwise operation because there is only one bit
                if ((pieceLocation >> i) == 1L) //There is a bit on the index
                {
                    pieceIndex = 63 - i;
                    break;
                }
            }
            return pieceIndex;
        }

        public static List<int> UlongTranslator(ulong pieceLocation, ulong legalULong, List<int> legalSquares) //Used for single pieces because of break
        {
            for (int i = 0; i < 64; i++) // Locate the index of the square that the bitboard is on
            {
                if (((pieceLocation >> i) & 1L) == 1L) // If the king bit is located
                {
                    int correctFrom = 63 - i; // Reverses bitboard to read

                    for (int y = 0; y < 64; y++)
                    {

                        if (((legalULong >> y) & 1L) == 1L)
                        {
                            // Flag | To | From
                            legalSquares.Add((63 - y) << 6 | correctFrom);
                        }

                    }

                    break; // Used for single pieces because of break
                }
            }

            return legalSquares;
        }

        public static ulong BetweenPieceRay(int slider, ulong fromPiece, ulong toPiece, ulong emptySquares)
        {
            //fromPiece is king
            //toPiece is checker

            ulong inBetweenMask = 0L;
            ulong fixedFromPiece = fromPiece;

            // Adding the toPiece to emptySquares, so it does not remove the bit from fromPieces, breaking the loop
            emptySquares |= toPiece;

            if (fromPiece < toPiece) // Left, TL, Up, TR
            {
                //slider: 0 = Bishop, 1 = Rook, 2 = Queen
                if (slider == 1 | slider == 2) // Rook or Queen
                {

                    // Ray Left
                    for (int i = 1; i < 8; i++)
                    {
                        //Move by one
                        //fromPiece out of the board & If there is a piece in the way
                        fixedFromPiece = (fixedFromPiece << 1) & ~(file_H) & emptySquares;

                        if (fixedFromPiece == 0L)
                        {
                            break;
                        }

                        else if (fixedFromPiece == toPiece) //If it has found the checker
                        {
                            return inBetweenMask;
                        }
                        //Add to inBetweenMask
                        inBetweenMask |= fixedFromPiece;
                    }

                    //If not found then format inBetweenMask and fixedFromPiece
                    inBetweenMask = 0L;
                    fixedFromPiece = fromPiece;

                    // Ray Up
                    for (int i = 1; i < 8; i++)
                    {
                        //Move by one
                        //fromPiece out of the board & If there is a piece in the way
                        fixedFromPiece = (fixedFromPiece << 8) & emptySquares;

                        if (fixedFromPiece == 0L)
                        {
                            break;
                        }

                        else if (fixedFromPiece == toPiece) //If it has found the checker
                        {
                            return inBetweenMask;
                        }
                        //Add to inBetweenMask
                        inBetweenMask |= fixedFromPiece;
                    }

                }


                if (slider == 0 | slider == 2) // Bishop or Queen
                {


                    //format inBetweenMask and fixedFromPiece for Queen
                    inBetweenMask = 0L;
                    fixedFromPiece = fromPiece;

                    // Ray TL

                    //Moving Diagonally Top-Left
                    for (int i = 1; i < 8; i++)
                    {

                        //Shifting Top Right
                        //Move by one
                        //fromPiece out of the board & If there is a piece in the way to save time, since if it hits then
                        //it's impossible to locate checker
                        fixedFromPiece = (fixedFromPiece << 9) & emptySquares & ~file_H;

                        //Null bitboard
                        if (fixedFromPiece == 0L)
                        {
                            break;
                        }

                        else if (fixedFromPiece == toPiece) //If it has found the checker
                        {
                            return inBetweenMask;
                        }

                        //Add to inBetweenMask
                        inBetweenMask |= fixedFromPiece;
                    }

                    //If not found then format inBetweenMask and fixedFromPiece
                    inBetweenMask = 0L;
                    fixedFromPiece = fromPiece;

                    // Ray TR

                    //Moving Diagonally Top-Right
                    for (int i = 1; i < 8; i++)
                    {

                        //Shifting Top Right
                        //Move by one
                        //fromPiece out of the board & If there is a piece in the way to save time, since if it hits then
                        //it's impossible to locate checker
                        fixedFromPiece = (fixedFromPiece << 7) & emptySquares & ~file_A;


                        //Null bitboard
                        if (fixedFromPiece == 0L)
                        {
                            break;
                        }

                        else if (fixedFromPiece == toPiece) //If it has found the checker
                        {
                            return inBetweenMask;
                        }

                        //Add to inBetweenMask
                        inBetweenMask |= fixedFromPiece;
                    }

                }


            }
            else // Right, BR, Down, BL
            {
                //slider: 0 = Bishop, 1 = Rook, 2 = Queen
                if (slider == 1 | slider == 2) // Rook or Queen
                {
                    // Ray Right
                    for (int i = 1; i < 8; i++)
                    {
                        //Move by one
                        //fromPiece out of the board & If there is a piece in the way
                        fixedFromPiece = (fixedFromPiece >> 1) & ~(file_A) & emptySquares;

                        if (fixedFromPiece == 0L)
                        {
                            break;
                        }

                        else if (fixedFromPiece == toPiece) //If it has found the checker
                        {
                            return inBetweenMask;
                        }
                        //Add to inBetweenMask
                        inBetweenMask |= fixedFromPiece;
                    }

                    //If not found then format inBetweenMask and fixedFromPiece
                    inBetweenMask = 0L;
                    fixedFromPiece = fromPiece;

                    // Ray Down
                    for (int i = 1; i < 8; i++)
                    {
                        //Move by one
                        //fromPiece out of the board & If there is a piece in the way
                        fixedFromPiece = (fixedFromPiece >> 8) & emptySquares;

                        if (fixedFromPiece == 0L)
                        {
                            break;
                        }

                        else if (fixedFromPiece == toPiece) //If it has found the checker
                        {
                            return inBetweenMask;
                        }
                        //Add to inBetweenMask
                        inBetweenMask |= fixedFromPiece;
                    }
                }

                if (slider == 0 | slider == 2) // Bishop or Queen
                {


                    //format inBetweenMask and fixedFromPiece for Queen
                    inBetweenMask = 0L;
                    fixedFromPiece = fromPiece;

                    // Ray BR

                    //Moving Diagonally Bottom-Right
                    for (int i = 1; i < 8; i++)
                    {

                        //Shifting Bottom Right
                        //Move by one
                        //fromPiece out of the board & If there is a piece in the way to save time, since if it hits then
                        //it's impossible to locate checker
                        fixedFromPiece = (fixedFromPiece >> 9) & emptySquares & ~file_A;

                        //Null bitboard
                        if (fixedFromPiece == 0L)
                        {
                            break;
                        }

                        else if (fixedFromPiece == toPiece) //If it has found the checker
                        {
                            return inBetweenMask;
                        }

                        //Add to inBetweenMask
                        inBetweenMask |= fixedFromPiece;
                    }

                    //If not found then format inBetweenMask and fixedFromPiece
                    inBetweenMask = 0L;
                    fixedFromPiece = fromPiece;

                    // Ray BL

                    //Moving Diagonally Bottom-Left
                    for (int i = 1; i < 8; i++)
                    {

                        //Shifting Bottom Left
                        //Move by one
                        //fromPiece out of the board & If there is a piece in the way to save time, since if it hits then
                        //it's impossible to locate checker
                        fixedFromPiece = (fixedFromPiece >> 7) & emptySquares & ~file_H;


                        //Null bitboard
                        if (fixedFromPiece == 0L)
                        {
                            break;
                        }

                        else if (fixedFromPiece == toPiece) //If it has found the checker
                        {
                            return inBetweenMask;
                        }

                        //Add to inBetweenMask
                        inBetweenMask |= fixedFromPiece;
                    }

                }

            }

            return 0L;
        }


        //Pinned pieces bitboard

        //Horizontal pinned pieces
        public static ulong Pins_Horizontal(ulong fK, ulong friendlyPieces, ulong enemyPieces, ulong eR, ulong eQ)
        {
            //Horizontal Movement
            ulong rookBlockMask_Horizontal = eR << 8 | eR >> 8;
            ulong queenBlockMask_Horizontal = eQ << 8 | eQ >> 8;
            ulong kingBlockMask_Horizontal = fK << 8 | fK >> 8;

            ulong kingRook_Horizontal = LegalMoves_Rook(fK, enemyPieces | kingBlockMask_Horizontal, friendlyPieces);
            ulong enemyQueen_Horizontal = LegalMoves_Rook(eQ, enemyPieces | queenBlockMask_Horizontal, friendlyPieces);
            ulong enemyRook_Horizontal = LegalMoves_Rook(eR, enemyPieces | rookBlockMask_Horizontal, friendlyPieces);

            //Return all pin locations 
            return kingRook_Horizontal & (enemyQueen_Horizontal | enemyRook_Horizontal);
        }

        //Vertical pinned pieces
        public static ulong Pins_Vertical(ulong fK, ulong friendlyPieces, ulong enemyPieces, ulong eR, ulong eQ)
        {
            //Vertical Movement
            ulong rookBlockMask_Vertical = eR << 1 | eR >> 1;
            ulong queenBlockMask_Vertical = eQ << 1 | eQ >> 1;
            ulong kingBlockMask_Vertical = fK << 1 | fK >> 1;

            ulong kingRook_Vertical = LegalMoves_Rook(fK, enemyPieces | kingBlockMask_Vertical, friendlyPieces);
            ulong enemyQueen_Vertical = LegalMoves_Rook(eQ, enemyPieces | queenBlockMask_Vertical, friendlyPieces);
            ulong enemyRook_Vertical = LegalMoves_Rook(eR, enemyPieces | rookBlockMask_Vertical, friendlyPieces);

            return kingRook_Vertical & (enemyQueen_Vertical | enemyRook_Vertical);
        }

        //Diagonal pins
        //Bottom Left - Top Right pin ray
        public static ulong Pins_BLTR(ulong fK, ulong friendlyPieces, ulong enemyPieces, ulong eB, ulong eQ)
        {

            //Diagonal (Top right, bottom left ray)
            ulong bishopBlockMask_BRTL = eB << 9 | eB >> 9;
            //Diagonal for Queen
            ulong queenBlockMask_BRTL = eQ << 9 | eQ >> 9;
            ulong kingBlockMask_BRTL = fK << 9 | fK >> 9;

            //Generate a move ray based on one diagonal
            ulong enemyBishop_BLTR_Ray = LegalMoves_Bishop(eB, enemyPieces | bishopBlockMask_BRTL, friendlyPieces);
            ulong enemyQueen_BLTR_Ray = LegalMoves_Bishop(eQ, enemyPieces | queenBlockMask_BRTL, friendlyPieces);
            ulong kingBishop_BLTR_Ray = LegalMoves_Bishop(fK, enemyPieces | kingBlockMask_BRTL, friendlyPieces);

            //Pins overlap mask
            return kingBishop_BLTR_Ray & (enemyBishop_BLTR_Ray | enemyQueen_BLTR_Ray);
        }

        //Bottom Right - Top Left pin ray
        public static ulong Pins_BRTL(ulong fK, ulong friendlyPieces, ulong enemyPieces, ulong eB, ulong eQ)
        {
            //Diagonal (Top left, bottom right ray)
            ulong bishopBlockMask_BRTL = eB << 7 | eB >> 7;
            //Diagonal for Queen
            ulong queenBlockMask_BRTL = eQ << 7 | eQ >> 7;
            ulong kingBlockMask_BRTL = fK << 7 | fK >> 7;

            //Generate a move ray based on one diagonal
            ulong enemyBishop_BRTL_Ray = LegalMoves_Bishop(eB, enemyPieces | bishopBlockMask_BRTL, friendlyPieces);
            ulong enemyQueen_BRTL_Ray = LegalMoves_Bishop(eQ, enemyPieces | queenBlockMask_BRTL, friendlyPieces);
            ulong kingBishop_BRTL_Ray = LegalMoves_Bishop(fK, enemyPieces | kingBlockMask_BRTL, friendlyPieces);

            return kingBishop_BRTL_Ray & (enemyBishop_BRTL_Ray | enemyQueen_BRTL_Ray);
        }


        //All pinned pieces bitboard
        public static ulong PinnedPieces(ulong fK, ulong friendlyPieces, ulong enemyPieces, ulong eR, ulong eB, ulong eQ)
        {
            //Generate sliding pieces moves from friendly king
            //Sliding pieces from king (Queen, Rook, Bishop) where they attack friendly pieces to check for overlap in bitboards

            //Reverse enemy and friendly pieces to allow for an overlap with friendly pieces
            //Don't need to generate queen moves since it's only Rook & Bishop combined, so we can separate them

            //Need to separate into both vertical and horizontal to create a single ray
            //Horizontal Movement
            ulong rookBlockMask_Horizontal = eR << 1 | eR >> 1;
            ulong queenBlockMask_Horizontal = eQ << 1 | eQ >> 1;
            ulong kingBlockMask_Horizontal = fK << 1 | fK >> 1;

            ulong kingRook_Horizontal = LegalMoves_Rook(fK, enemyPieces | kingBlockMask_Horizontal, friendlyPieces);
            ulong enemyQueen_Horizontal = LegalMoves_Rook(eQ, enemyPieces | queenBlockMask_Horizontal, friendlyPieces);
            ulong enemyRook_Horizontal = LegalMoves_Rook(eR, enemyPieces | rookBlockMask_Horizontal, friendlyPieces);

            //Vertical Movement
            ulong rookBlockMask_Vertical = eR << 8 | eR >> 8;
            ulong queenBlockMask_Vertical = eQ << 8 | eQ >> 8;
            ulong kingBlockMask_Vertical = fK << 8 | fK >> 8;

            ulong kingRook_Vertical = LegalMoves_Rook(fK, enemyPieces | kingBlockMask_Vertical, friendlyPieces);
            ulong enemyQueen_Vertical = LegalMoves_Rook(eQ, enemyPieces | queenBlockMask_Vertical, friendlyPieces);
            ulong enemyRook_Vertical = LegalMoves_Rook(eR, enemyPieces | rookBlockMask_Vertical, friendlyPieces);

            //Check for pinned pieces
            ulong queenPins_Horizontal = kingRook_Horizontal & enemyQueen_Horizontal;
            ulong queenPins_Vertical = kingRook_Vertical & enemyQueen_Vertical;

            ulong rookPins_Horizontal = kingRook_Horizontal & enemyRook_Horizontal;
            ulong rookPins_Vertical = kingRook_Vertical & enemyRook_Vertical;

            //Generate enemy sliding piece moves (enemy pieces = black)

            //Need to separate the diagonal rays because they can overlap without the piece being actually pinned
            //To separate we block one of the diagonals by placing "friendly pieces" there
            //Diagonal (Top right, bottom left ray)
            ulong bishopBlockMask_TLBR = eB << 9 | eB >> 9;
            //Diagonal for Queen
            ulong queenBlockMask_TLBR = eQ << 9 | eQ >> 9;
            ulong kingBlockMask_TLBR = fK << 9 | fK >> 9;

            //Generate a move ray based on one diagonal
            ulong enemyBishop_TRBL_Ray = LegalMoves_Bishop(eB, enemyPieces | bishopBlockMask_TLBR, friendlyPieces);
            ulong enemyQueen_TRBL_Ray = LegalMoves_Bishop(eQ, enemyPieces | queenBlockMask_TLBR, friendlyPieces);
            ulong kingBishop_TRBL_Ray = LegalMoves_Bishop(fK, enemyPieces | kingBlockMask_TLBR, friendlyPieces);

            //Queen pin mask (Looking for overlaps)
            ulong queenPins_TRBL_Ray = kingBishop_TRBL_Ray & enemyQueen_TRBL_Ray;
            //Bishop pin mask (Looking for overlaps)
            ulong bishopPins_TRBL_Ray = kingBishop_TRBL_Ray & enemyBishop_TRBL_Ray;

            //Diagonal (Top left, bottom right ray)
            ulong bishopBlockMask_TRBL = eB << 7 | eB >> 7;
            //Diagonal for Queen
            ulong queenBlockMask_TRBL = eQ << 7 | eQ >> 7;
            ulong kingBlockMask_TRBL = fK << 7 | fK >> 7;

            //Generate a move ray based on one diagonal
            ulong enemyBishop_TLBR_Ray = LegalMoves_Bishop(eB, enemyPieces | bishopBlockMask_TRBL, friendlyPieces);
            ulong enemyQueen_TLBR_Ray = LegalMoves_Bishop(eQ, enemyPieces | queenBlockMask_TRBL, friendlyPieces);
            ulong kingBishop_TLBR_Ray = LegalMoves_Bishop(fK, enemyPieces | kingBlockMask_TRBL, friendlyPieces);

            //Queen pin mask (Looking for overlaps)
            ulong queenPins_TLBR_Ray = kingBishop_TLBR_Ray & enemyQueen_TLBR_Ray;
            //Bishop pin mask (Looking for overlaps)
            ulong bishopPins_TLBR_Ray = kingBishop_TLBR_Ray & enemyBishop_TLBR_Ray;

            //Combine the pins
            ulong queenPins = queenPins_TLBR_Ray | queenPins_TRBL_Ray | queenPins_Horizontal | queenPins_Vertical;
            ulong bishopPins = bishopPins_TLBR_Ray | bishopPins_TRBL_Ray;
            ulong rookPins = rookPins_Horizontal | rookPins_Vertical;

            //Return pinned pieces mask
            return rookPins | bishopPins | queenPins;

        }
    }
}