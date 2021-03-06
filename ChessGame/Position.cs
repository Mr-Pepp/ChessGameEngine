using System.Collections.Generic;

namespace ChessGame
{
    class Position
    {
        public ulong wK;
        public ulong wQ;
        public ulong wR;
        public ulong wB;
        public ulong wN;
        public ulong wP;

        public ulong bK;
        public ulong bQ;
        public ulong bR;
        public ulong bB;
        public ulong bN;
        public ulong bP;

        public int whiteCastles;
        public int blackCastles;

        // When a pawn moves by two, we want to mark the bitboard so that it can be captured as if it moved by one (on next turn)
        public ulong white_enPassantMask;
        public ulong black_enPassantMask;

        public bool whiteTurn;

        public ulong whitePieces;
        public ulong blackPieces;

        public ulong emptySquares;

        // Counting the pieces on board
        public int wKCount = 0, wQCount = 0, wRCount = 0, wNCount = 0, wBCount = 0, wPCount = 0,
            bKCount = 0, bQCount = 0, bRCount = 0, bNCount = 0, bBCount = 0, bPCount = 0;



        // Mailbox approach for faster evaluation and generation
        // Each will include a list of where they are (0 - 63)

        // White pieces
        public List<int> whiteKing;
        public List<int> whiteQueen;
        public List<int> whiteRook;
        public List<int> whiteBishop;
        public List<int> whiteKnight;
        public List<int> whitePawn;
        
        // Black pieces
        public List<int> blackKing;
        public List<int> blackQueen;
        public List<int> blackRook;
        public List<int> blackBishop;
        public List<int> blackKnight;
        public List<int> blackPawn;




        // All information required:
        // All white pieces bitboard locations
        // All black pieces bitboard locations
        // White castle information
        // Black castle information
        // White En Passant bitboard
        // Black En Passant bitboard
        // Who's turn it is
        public Position(ulong _wK, ulong _wQ, ulong _wR, ulong _wB, ulong _wN, ulong _wP,
            ulong _bK, ulong _bQ, ulong _bR, ulong _bB, ulong _bN, ulong _bP, int _whiteCastles, int _blackCastles,
            ulong _white_enPassantMask, ulong _black_enPassantMask, bool _whiteTurn)
        {

            wK = _wK;
            wQ = _wQ;
            wR = _wR;
            wB = _wB;
            wN = _wN;
            wP = _wP;
            bK = _bK;
            bQ = _bQ;
            bR = _bR;
            bB = _bB;
            bN = _bN;
            bP = _bP;

            whiteCastles = _whiteCastles;
            blackCastles = _blackCastles;
            white_enPassantMask = _white_enPassantMask;
            black_enPassantMask = _black_enPassantMask;
            whiteTurn = _whiteTurn;

            InitBitboards();
        }

        public void InitBitboards()
        {
            whitePieces = wK | wQ | wR | wB | wN | wP;
            blackPieces = bK | bQ | bR | bB | bN | bP;
            emptySquares = ~(whitePieces | blackPieces);
        }
    }
}
