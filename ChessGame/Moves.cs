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

        static ulong blockMask;
        static ulong captureMask;

        static ulong moves;

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
        }


        //Pawns for bitboard move generation
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
            //Replace bitboard since it gets removed
            ulong fixedR = R;


            //Have to run loop until there is a piece to the right of the rook

            //Moving Horizontally Right
            for (int i = 1; i < 8; i++)
            {
                /* Inefficient method
                //If outside the board
                if ((R >> i & file_A) != 0L)
                {
                    //remove bit from calculating further
                    R = R & ~(file_A << i);
                    //We do this by reversing the bitwise operation we set on the condition to locate the appropriate bit(s)
                }

                //Until there is a friendly piece
                else if ((R >> i & friendlyPieces) != 0L)
                {
                    //remove bit from calculating further
                    R = R & ~(friendlyPieces << i);
                }

                else if((R >> i & enemyPieces) != 0L)
                {
                    legalMoves = legalMoves | R >> i;
                    //remove bit from calculating
                    R = R & ~(enemyPieces << i);
                }*/

                //If outside of bounds on file A, remove the appropriate rook bit(s)
                R = R & ~(file_A << i);
                //If friendly piece in the way, remove the appropriate rook bit(s)
                R = R & ~(friendlyPieces << i);

                //No more bits in bitboard
                if (R == 0L)
                {
                    break;
                }

                legalMoves = legalMoves | (R >> i);

                //Does this operation after because we still want to capture the enemy piece
                R = R & ~(enemyPieces << i);
            }

            //instantiate original bitboard
            R = fixedR;
            //Moving Horizontally Left
            for (int i = 1; i < 8; i++)
            {

                R = R & ~(file_H >> i);
                R = R & ~(friendlyPieces >> i);
                if (R == 0L)
                {
                    break;
                }
                legalMoves = legalMoves | (R << i & ~friendlyPieces);
                R = R & ~(enemyPieces >> i);
            }

            //instantiate original bitboard
            R = fixedR;
            //Moving Vertically Down
            for (int i = 1; i < 8; i++)
            {

                //Can't go further down
                R = R >> (i * 8);
                R = R << (i * 8);
                //Friendly piece in the way
                R = R & ~(friendlyPieces << (i * 8));
                //No more bits
                if (R == 0L)
                {
                    break;
                }
                //Add to legal moves
                legalMoves = legalMoves | (R >> (i * 8) & ~friendlyPieces);
                //If there is an enemy piece there (after legal moves because we want to be able to capture the enemy piece)
                R = R & ~(enemyPieces << (i * 8));
            }

            //instantiate original bitboard
            R = fixedR;
            //Moving Vertically Up
            for (int i = 1; i < 8; i++)
            {
                //Can't go further down
                R = R << (i * 8);
                R = R >> (i * 8);
                //Friendly piece in the way
                R = R & ~(friendlyPieces >> (i * 8));
                //No more bits
                if (R == 0L)
                {
                    break;
                }
                //Add to legal moves
                legalMoves = legalMoves | (R << (i * 8) & ~friendlyPieces);
                //If there is an enemy piece there (after legal moves because we want to be able to capture the enemy piece)
                R = R & ~(enemyPieces >> (i * 8));
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
                B = B << (i * 8);
                B = B >> (i * 8);
                //Outside the board (Left)
                B = B & ~(file_H >> i);
                //Friendly piece in the way
                B = B & ~(friendlyPieces >> (i * 9));
                //No bits in bitboard
                if (B == 0L)
                {
                    break;
                }
                legalMoves = legalMoves | (B << (i * 9) & ~friendlyPieces);
                //Enemy piece in the way
                B = B & ~(enemyPieces >> (i * 9));
            }

            //Restore bitboard
            B = fixedB;
            //Moving Diagonally Top-Right
            for (int i = 1; i < 8; i++)
            {
                //Outside the board (Top)
                B = B << (i * 8);
                B = B >> (i * 8);
                //Outside the board (Right)
                //Shifting to left makes the file_A bitboard go up a file which means that it can miss the piece
                //So we have to shift the file_A bitboard down by 1 after the bitwise operation
                B = B & ~((file_A << i) >> 8);
                //Friendly piece in the way
                B = B & ~(friendlyPieces >> (i * 7));
                //No bits in bitboard
                if (B == 0L)
                {
                    break;
                }
                //Append legal moves
                legalMoves = legalMoves | (B << (i * 7) & ~friendlyPieces);
                //Enemy piece in the way
                B = B & ~(enemyPieces >> (i * 7));
            }

            //Restore bitboard
            B = fixedB;
            //Moving Diagonally Bottom-Right
            for (int i = 1; i < 8; i++)
            {
                //Outside the board (Bottom)
                B = B >> (i * 8);
                B = B << (i * 8);
                //Outside the board (Right)
                B = B & ~(file_A << i);
                //Friendly piece in the way
                B = B & ~(friendlyPieces << (i * 9));
                //No bits in bitboard
                if (B == 0L)
                {
                    break;
                }
                //Append legal moves
                legalMoves = legalMoves | (B >> (i * 9) & ~friendlyPieces);
                //Enemy piece in the way
                B = B & ~(enemyPieces << (i * 9));
            }

            //Restore bitboard
            B = fixedB;
            //Moving Diagonally Bottom-Left
            for (int i = 1; i < 8; i++)
            {
                //Outside the board (Bottom)
                B = B >> (i * 8);
                B = B << (i * 8);
                //Outside the board (Left)
                //Shifting to left makes the file_H bitboard go down a file which means that it can miss the piece
                //So we have to shift the file_H bitboard up by 1 after the bitwise operation
                B = B & ~((file_H >> i) << 8);
                //Friendly piece in the way
                B = B & ~(friendlyPieces << (i * 7));
                //No bits in bitboard
                if (B == 0L)
                {
                    break;
                }
                //Append legal moves
                legalMoves = legalMoves | (B >> (i * 7) & ~friendlyPieces);
                //Enemy piece in the way
                B = B & ~(enemyPieces << (i * 7));
            }

            return legalMoves;
        }

        public static ulong LegalMoves_Queen(ulong Q, ulong friendlyPieces, ulong enemyPieces) //Queen
        {
            ulong legalMoves = 0L;

            //Considering that the queen is both rook and bishop combined
            return legalMoves | LegalMoves_Bishop(Q, friendlyPieces, enemyPieces) | LegalMoves_Rook(Q, friendlyPieces, enemyPieces);
        }

        public static ulong LegalMoves_King(ulong K, ulong friendlyPieces, ulong enemyPieces, bool whiteTurn, bool legal) // King || Need to consider attacked squares
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
                    attackedSquares = AttackedSquares(Board.bK, Board.bQ, Board.bR, Board.bB, Board.bN, Board.bP, K, friendlyPieces, enemyPieces, whiteTurn);
                }
                else // Black moving
                {
                    //Generate white moves to show the squares they are attacking
                    attackedSquares = AttackedSquares(Board.wK, Board.wQ, Board.wR, Board.wB, Board.wN, Board.wP, K, friendlyPieces, enemyPieces, whiteTurn);
                }
            }

            //King move all around but only by one square
            legalMoves = legalMoves | (K << 1 & ~friendlyPieces | K << 9 & ~friendlyPieces | K >> 7 & ~friendlyPieces) & ~file_H | K << 8 & ~friendlyPieces | (K << 7 & ~friendlyPieces |
                K >> 1 & ~friendlyPieces | K >> 9 & ~friendlyPieces) & ~file_A | K >> 8 & ~friendlyPieces;
            //Filter illegal moves
            legalMoves = legalMoves & ~attackedSquares;

            return legalMoves;
        }




        //Generating each move from king and checking for same piece (Very efficient)

        //Generate from king and each hit we check what piece it is
        //This is played after an assumed move

        //Check for checks
        // [Knights, Bishops, Rooks, Queens, Pawns]
        public static ulong[] KingHits(ulong k, bool whiteTurn, ulong friendlyPieces, ulong enemyPieces)
        {
            //Initialise attacking piece bitboards
            ulong aN = 0L;
            ulong aR = 0L;
            ulong aB = 0L;
            ulong aQ = 0L;
            ulong aP = 0L;


            //LegalMoves_Knight(k, friendlyPieces) & (enemyPieces & (Board.wN | Board.bN) -- Method for not knowing the colour
            if (whiteTurn) //White to play (if check then black just checked white)
            {
                //Check from king square
                //We have to reverse enemyPieces and friendlyPieces because we are calculating the moves from the enemy's perspective
                aN = LegalMoves_Knight(k, friendlyPieces) & Board.bN;
                aB = LegalMoves_Bishop(k, friendlyPieces, enemyPieces) & Board.bB;
                aR = LegalMoves_Rook(k, friendlyPieces, enemyPieces) & Board.bR;
                aQ = LegalMoves_Queen(k, friendlyPieces, enemyPieces) & Board.bQ;

                //Locates pawn attacking from top left
                aP = ((k << 9) & ~file_H) & Board.bP;
                //Locates pawn attacking from top right
                aP = aP | ((k << 7) & ~file_A) & Board.bP;
            }

            else //Black to play (if check then white just checked black)
            {
                //Check from king square
                //We have to reverse enemyPieces and friendlyPieces because we are calculating the moves from the enemy's perspective
                aN = LegalMoves_Knight(k, friendlyPieces) & Board.wN;
                aB = LegalMoves_Bishop(k, friendlyPieces, enemyPieces) & Board.wB;
                aR = LegalMoves_Rook(k, friendlyPieces, enemyPieces) & Board.wR;
                aQ = LegalMoves_Queen(k, friendlyPieces, enemyPieces) & Board.wQ;

                //Locates pawn attacking from bottom left
                aP = ((k >> 7) & ~file_H) & Board.wP;
                //Locates pawn attacking from bottom right
                aP = aP | ((k >> 9) & ~file_A) & Board.wP;
            }

            // [Knights, Bishops, Rooks, Queens, Pawns]
            return new ulong[] { aN, aR, aB, aQ, aP };
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

        public static ulong AttackedSquares(ulong eK, ulong eQ, ulong eR, ulong eB, ulong eN, ulong eP, ulong fK, ulong friendlyPieces, ulong enemyPieces, bool whiteTurn) // King || Need to consider attacked squares
        {
            //Important to note that all the bitboards for pieces provided are enemy bitboards, hence e at beginning
            //fK = Friendly king
            //eK = Enemy King

            ulong attackedSquares = 0L;
            ulong pawnAttackingSquares = 0L;

            //Friendly pieces no king for valid move generation
            friendlyPieces = friendlyPieces & ~fK;

            //Pawn attack generation
            if (whiteTurn) //Still white turn, pawns go down because its for black pawns that attack
            {
                //attacks bottom left
                pawnAttackingSquares = pawnAttackingSquares | ((eP >> 7) & ~file_H);
                //attacks bottom right
                pawnAttackingSquares = pawnAttackingSquares | ((eP >> 9) & ~file_A);
            }
            else //Black turn pawns go up
            {
                //attacks top left
                pawnAttackingSquares = pawnAttackingSquares | ((eP << 9) & ~file_H);
                //attacks top right
                pawnAttackingSquares = pawnAttackingSquares | ((eP << 7) & ~file_A);
            }


            //Enemy pieces for attacked squares
            //Generate all moves for enemy pieces


            //Since we are generating moves for the enemy, we would assume that we would have to give enemyPieces bitboard for the friendlyPieces arguement, etc, but it would be invalid
            //because we have to show that our piece protects our own piece by attacking the square our own piece is on so the enemy king cant capture it.
            attackedSquares = attackedSquares | pawnAttackingSquares | LegalMoves_King(eK, friendlyPieces, enemyPieces, whiteTurn, false) | LegalMoves_Knight(eN, friendlyPieces) |
                LegalMoves_Rook(eR, friendlyPieces, enemyPieces) | LegalMoves_Bishop(eB, friendlyPieces, enemyPieces) | LegalMoves_Queen(eQ, friendlyPieces, enemyPieces);

            return attackedSquares;
        }


        //used for debugging and returning the squares for bitboard
        public static List<int> DebugSquares()
        {
            List<int> debugSquares = new List<int>();

            //Generate squares that are attacked by black pieces sicne since it's black's turn in this debugging example
            ulong squares = AttackedSquares(Board.wK, Board.wQ, Board.wR, Board.wB, Board.wN, Board.wP, Board.bK, blackPieces, whitePieces, false);

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


        public struct MakeMove
        {
            uint move;

            //https://chess.stackexchange.com/questions/18017/whats-the-right-approach-of-storing-moves-generated-using-bitboards
             public MakeMove(uint from, uint to, uint flag)
            {
                //0000 0000 0000 0000  to store moveType | to | from
                //From: 0000 0000 0011 1111
                from = 0b111111 & from;
                //To: 0000 1111 1100 0000
                to = (0b111111 & to) << 6;
                //moveType: 0011 0000 0000 0000
                flag = (0b11 & flag) << 12;

                this.move = flag | to | from;
            }
        }

        enum Flag
        {
            QUIET, CAPTURE, EVASION, ENPASSANT, CASTLING
        }


        //Pinned Piece Solution:
        //Calculate all sliding piece moves (for each sliding piece)
        //Calculating sliding piece moves from the king (for each sliding piece)
        //If bitboards overlap with the friendly pieces then can't move that piece

        //Will not generate pinned piece moves
        public static List<int> GenerateAllMoves(ulong wK, ulong wQ, ulong wR, ulong wB, ulong wN, ulong wP,
            ulong bK, ulong bQ, ulong bR, ulong bB, ulong bN, ulong bP) // 0000 0000 0000 0000  to store: flag | to | from
        {
            List<int> legalSquares = new List<int>();
            InitBitboards(); // Initiate bitboards

            ulong pieceLocation = 0;
            int correctFrom;

            //Used for getting all the ulong bitboard move locations
            ulong legalULong = 0L;

            //System.Diagnostics.Debug.WriteLine(pieces[1] & 0b11111 << 6);

            //read bitboards as index
            //the bitwise operations will reverse the board so initially the board will be on blacks side
            for (int i = 63; i > 0; i--)
            {
                //System.Diagnostics.Debug.WriteLine(i);

                //Assign squares; //Assing to piece information // Piece Information: Colour | Piece | Location
                //**Implement flags and other piece information


                //First continue if no pieces because that is most likely as there are more empty squares than any other
                if ((blackPieces | whitePieces) == 0L) { continue; } //pass

                else //There is a piece to generate moves for
                {
                    //Since the index is reversed for bitboards
                    correctFrom = 63 - i;
                    //Convert the index to bitboard
                    pieceLocation = Board.BinaryStringToBitboard(correctFrom);


                    if (whiteTurn) // White to play
                    {
                        //System.Diagnostics.Debug.WriteLine(i);

                        if (((wP >> i) & 1L) == 1L) //Pawns first as most likely since they are more common
                        {
                            //White pawn legal moves bitboard
                            legalULong = LegalMoves_WPawn(pieceLocation);
                        }
                        else if (((wK >> i) & 1L) == 1L)
                        {
                            //King legal moves bitboard
                            legalULong = LegalMoves_King(pieceLocation, whitePieces, blackPieces, whiteTurn, true);
                        }
                        else if (((wQ >> i) & 1L) == 1L)
                        {
                            //Queen legal moves bitboard
                            legalULong = LegalMoves_Queen(pieceLocation, whitePieces, blackPieces);
                        }
                        else if (((wR >> i) & 1L) == 1L)
                        {
                            //Rook legal moves bitboard
                            legalULong = LegalMoves_Rook(pieceLocation, whitePieces, blackPieces);
                        }
                        else if (((wB >> i) & 1L) == 1L)
                        {
                            //Bishop legal moves bitboard
                            legalULong = LegalMoves_Bishop(pieceLocation, whitePieces, blackPieces);
                            System.Diagnostics.Debug.WriteLine((63 - i));
                        }
                        else if (((wN >> i) & 1L) == 1L)
                        {
                            //Knight legal moves bitboard
                            legalULong = LegalMoves_Knight(pieceLocation, whitePieces);
                        }
                    }

                    else // Black to play
                    {
                        if (((bP >> i) & 1L) == 1L) //Pawns first as most likely since they are more common
                        {
                            //Black pawn legal moves
                            legalULong = LegalMoves_BPawn(pieceLocation);
                        }
                        else if (((bK >> i) & 1L) == 1L)
                        {
                            //King legal moves bitboard
                            legalULong = LegalMoves_King(pieceLocation, blackPieces, whitePieces, whiteTurn, true);
                        }
                        else if (((bQ >> i) & 1L) == 1L)
                        {
                            //Queen legal moves bitboard
                            legalULong = LegalMoves_Queen(pieceLocation, blackPieces, whitePieces);
                        }
                        else if (((bR >> i) & 1L) == 1L)
                        {
                            //Rook legal moves bitboard
                            legalULong = LegalMoves_Rook(pieceLocation, blackPieces, whitePieces);
                        }
                        else if (((bB >> i) & 1L) == 1L)
                        {
                            //Bishop legal moves bitboard
                            legalULong = LegalMoves_Bishop(pieceLocation, blackPieces, whitePieces);
                        }
                        else if (((bN >> i) & 1L) == 1L)
                        {
                            //Knight legal moves bitboard
                            legalULong = LegalMoves_Knight(pieceLocation, blackPieces);
                        }
                    }
                    //System.Diagnostics.Debug.WriteLine(legalULong.ToString() + i);
                    //System.Diagnostics.Debug.WriteLine(legalULong);
                    //Append to list the legal squares
                    if (legalULong != 0L)
                    {
                        for (int y = 0; y < 64; y++)
                        {
                            //flag | to | from
                            if (((legalULong >> y) & 1L) == 1L)
                            {
                                legalSquares.Add((63 - y) << 6 | correctFrom);

                            }

                        }
                    }
                }
                
                
            }


            return legalSquares;
        } 

        /*
        int[] GetValidMoves()
        {
            int[] moves;

            if (whiteTurn) // White to move
            {
                //First check whether youre in check
                // [Knights, Bishops, Rooks, Queens, Pawns]
                ulong[] checks = KingHits(Board.wK, whiteTurn, whitePieces, blackPieces);

                if (checks.Length > 0)
                {
                    //Generate legal king moves

                    if (checks.Length == 1) // Only one check, king can move, piece can block, can be captured
                    {
                        //Capture Checker: use same method as for king to see if there are any pieces that can capture it


                    }
                }
                else // Not in check, generate all moves
                {

                }
            }
            else // Black to Move
            {

            }

            return moves;
        }*/




        //Generate the moves & store them
        struct MoveGeneration
        {
            public List<int[]> moves;

            void GenerateMoves()
            {
                
                
                



            }
        }

    }
}