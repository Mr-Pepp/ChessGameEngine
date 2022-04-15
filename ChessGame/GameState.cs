using System;
using System.Collections.Generic;
using System.Text;

namespace ChessGame
{
    class GameState
    {

        /*
         * 0 = Currently playing
         * 1 = Checkmate
         * 2 = Stalemate
         * 3 = Promoting the pawn
         */
        public static int state = 0;

        public static bool playerMove;

    }
}
