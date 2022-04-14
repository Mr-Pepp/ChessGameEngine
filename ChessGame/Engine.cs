using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    class Engine
    {

        public Engine()
        {

        }


        //Random Moves - - Will return the move in int (Flag | To | From)
        public static int RandomMove(ulong wK, ulong wQ, ulong wR, ulong wB, ulong wN, ulong wP,
            ulong bK, ulong bQ, ulong bR, ulong bB, ulong bN, ulong bP, int whiteCastles, int blackCastles,
            ulong white_enPassantMask, ulong black_enPassantMask, bool whiteTurn)
        {
            List<int> moves = Moves.GenerateGameMoves(wK, wQ, wR, wB, wN, wP, bK, bQ, bR, bB, bN, bP,
                whiteCastles, blackCastles, white_enPassantMask, black_enPassantMask, whiteTurn);

            return moves[new Random().Next(moves.Count)];

        }
    }
}
