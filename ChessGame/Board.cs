using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace ChessGame
{
    class Board
    {
        public static Square[] squares = new Square[64]; //1D array for FEN
        private OutsideBoard outsideBoard;

        //pieceInfo //Piece information Piece
        //Stores the information of all the pieces on the board
        // 
        // Piece Information: Colour | Piece | Location
        public static List<int> piecesInfo = new List<int>() { };


        Dictionary<string, Color> _colorDic;

        private int _squareSize;
        private Vector2 _initPos;
        private Color colour;


        //for selecting piece
        private bool pieceSelected = false;
        private int tempPiece;
        private int square;
        private Texture2D mousePiece;
        private bool mousePressed;
        private Rectangle mousePieceRect;


        private string defaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"; //default position
        //private string FEN = "r5nr/1pp2pp1/3q4/2b1P2p/1NK2Pk1/2BP1BR1/PP1Q1P1p/8 w - - 0 1"; //debug position
        private string FEN = "5k2/8/8/8/8/8/8/3BB3 w - - 0 1";


        //For bitboards IMPROVEMENT _________________________ *****

        //bitboards --- w = White, b = Black
        public static ulong wK = 0L, wQ = 0L, wR = 0L, wN = 0L, wB = 0L, wP = 0L, bK = 0L, bQ = 0L, bR = 0L, bN = 0L, bB = 0L, bP = 0L;


        public Board(int squareSize, Vector2 initPos, Dictionary<string, Color> colorDic )
        {
            _initPos = initPos;
            _squareSize = squareSize;
            _colorDic = colorDic;
        }

        //Loaded right away, ran once
        public void LoadContent(SpriteBatch _spriteBatch, GraphicsDevice _graphicsDevice)
        {
            // create a white texture pixel
            Texture2D blankTexture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1); 
            blankTexture.SetData(new Color[] { Color.White });


            // Create board
            for (int iy = 1; iy <= 8; iy++) 
            {
                for (int ix = 1; ix <= 8; ix++)
                {


                    if (((ix + iy - 2) % 2) == 0) // If even square then change colour
                    {
                        colour = _colorDic["Light Square"];
                    }
                    else
                    {
                        colour = _colorDic["Dark Square"];
                    }

                    squares[SquareID(ix, iy)] = new Square(blankTexture, colour, _squareSize)
                    {
                        position = new Vector2(_initPos.X + (_squareSize * (ix - 1)), _initPos.Y + (_squareSize * (iy - 1))),
                    };
                    //run load content of the square. change if real time scaling becomes a thing
                    squares[SquareID(ix, iy)].LoadContent();
                }
            }

            //Create a visual board on the outside
            outsideBoard = new OutsideBoard(blankTexture, _colorDic["Outside Board"], _colorDic["Inner Outside Board"], _squareSize)
            {
                position = _initPos,
            };

            //run load FEN position
            LoadFEN(FEN);

        }


        //Update, looped
        public void Update()
        {
            //'R' is pressed
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                //load starting pos
                LoadFEN(defaultFEN);

                pieceSelected = false;
                Moves.whiteTurn = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
                System.Diagnostics.Debug.WriteLine(Moves.GenerateAllMoves().Count);
      
            }

            //'D' is pressed -- for debugging purposes
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {

                //Clear board from dots and target squares
                for (int i = 0; i < 64; i++)
                {
                    squares[i].targetSquare = false;
                    squares[i].dot = false;
                }


                foreach (int e in Moves.DebugSquares())
                {
                    squares[e].targetSquare = true;
                }

                
            }

            PieceSelection();
        } 


        //Draw, looped
        public void Draw(SpriteBatch _spriteBatch)
        {
            outsideBoard.Draw(_spriteBatch);

            for (int i = 0; i <= 63; i++)
            {
                //System.Diagnostics.Debug.WriteLine(squares[i].position.X + ", " + squares[i].position.Y);
                squares[i].Draw(_spriteBatch);
            }

            if (pieceSelected)
            {
                //draw piece at mouse point
                _spriteBatch.Draw(mousePiece, mousePieceRect, Color.White);
            }

        }


        private int SquareID(int x, int y) //Get square number from top left considering column (x) and row (y)
        {
            return ((y - 1) * 8 + x - 1);
        }

        //Allows the piece to be selected
        private void PieceSelection()
        {
            mousePressed = Mouse.GetState().LeftButton == ButtonState.Pressed;

            //future perhaps loop squares with a piece? position identifier?
            //if mouse interacts with the piece rectangle
            if (!pieceSelected && mousePressed)
            {
                for (int iy = 1; iy <= 8; iy++)
                {
                    for (int ix = 1; ix <= 8; ix++)
                    {
                        square = SquareID(iy, ix);
                        if (squares[square].rect.Contains(Game1.mousePoint) && squares[square].piece != 0)
                        {
                            //store piece
                            tempPiece = squares[square].piece;
                            //remove piece
                            squares[square].piece = Piece.None;

                            //change piece in hand
                            mousePiece = Piece.pieceTextureDic[tempPiece];

                            //set piece selected as true
                            pieceSelected = true;

                            goto loopEnd;
                        }
                    }
                }
            }


            goto loopEnd;

            //needs changing. Used since we are breaking from a nested loop
            loopEnd:

            //holding the piece
            if (pieceSelected && mousePressed)
            {
                //place piece at mouse point ---- !! Future make this run only once !!
                mousePieceRect = new Rectangle(Game1.mousePoint.X - (_squareSize / 2), Game1.mousePoint.Y - (_squareSize / 2), _squareSize, _squareSize);

                //Need to find a way to do this with bitboards
                foreach(int e in Moves.CheckLegalMoves(tempPiece, square))
                {
                    //Show available moves
                    if (squares[e].piece == 0)
                    {
                        squares[e].dot = true;
                    }

                    else // there is a piece
                    {
                        //colour square to indicate the ability to take the piece

                        squares[e].targetSquare = true;
                    }
                }
                
                //if right button is pressed when holding piece
                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    //piece is not selected
                    pieceSelected = false;

                    //place back piece
                    squares[square].piece = tempPiece;
                    //assign texture and moves
                    squares[square].AssignPiece();

                    //Need to find a way for this with bitboards
                    foreach (int e in Moves.CheckLegalMoves(tempPiece, square))
                    {
                        //Remove
                        squares[e].dot = false;
                        squares[e].targetSquare = false;
                    }

                }
            }

            //holding released
            else if (pieceSelected)
            {
                //return stored piece (use for illegal moves)
                //squares[square].piece = tempPiece;

                //set mouse piece as null, although this is not needed as it is not executed
                mousePiece = null;

                //set piece selected back to false
                pieceSelected = false;

                //temp square to place back the piece in case
                int tempSquare = square;

                foreach (int e in Moves.CheckLegalMoves(tempPiece, tempSquare))
                {
                    //Remove
                    squares[e].dot = false;
                    squares[e].targetSquare = false;
                }

                //place Piece at current square
                for (int iy = 1; iy <= 8; iy++)
                {
                    for (int ix = 1; ix <= 8; ix++)
                    {
                        square = SquareID(iy, ix);
                        //add rules to if
                        //square is at current mouse position
                        if (squares[square].rect.Contains(Game1.mousePoint) && Moves.CheckLegalMoves(tempPiece, tempSquare).Contains(square))
                        {
                            //Future add an if statement to determine white / black move and then do the checks.


                            //Filter piece to update the correct bitboard

                            //First update the targetted square piece (won't do anything if the piece is Piece.None)
                            switch (squares[square].piece)
                            {
                                //White Pieces
                                case Piece.White | Piece.King:
                                    wK = wK & ~BinaryStringToBitboard(square);
                                    break;

                                case (Piece.White | Piece.Queen):
                                    wQ = wQ & ~BinaryStringToBitboard(square);
                                    break;

                                case Piece.White | Piece.Rook:
                                    wR = wR & ~BinaryStringToBitboard(square);
                                    break;

                                case (Piece.White | Piece.Bishop):
                                    wB = wB & ~BinaryStringToBitboard(square);
                                    break;

                                case Piece.White | Piece.Knight:
                                    wN = wN & ~BinaryStringToBitboard(square);
                                    break;

                                case (Piece.White | Piece.Pawn):
                                    wP = wP & ~BinaryStringToBitboard(square);
                                    break;

                                //Black Pieces
                                case Piece.Black | Piece.King:
                                    bK = bK & ~BinaryStringToBitboard(square);
                                    break;

                                case (Piece.Black | Piece.Queen):
                                    bQ = bQ & ~BinaryStringToBitboard(square);
                                    break;

                                case Piece.Black | Piece.Rook:
                                    bR = bR & ~BinaryStringToBitboard(square);
                                    break;

                                case (Piece.Black | Piece.Bishop):
                                    bB = bB & ~BinaryStringToBitboard(square);
                                    break;

                                case Piece.Black | Piece.Knight:
                                    bN = bN & ~BinaryStringToBitboard(square);
                                    break;

                                case (Piece.Black | Piece.Pawn):
                                    bP = bP & ~BinaryStringToBitboard(square);
                                    break;
                            }

                            //Update for the piece taking the square
                            switch (tempPiece)
                            {
                                //White Pieces
                                case Piece.White | Piece.King:
                                    wK = wK | BinaryStringToBitboard(square);
                                    wK = wK & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                case (Piece.White | Piece.Queen):
                                    wQ = wQ | BinaryStringToBitboard(square);
                                    wQ = wQ & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                case Piece.White | Piece.Rook:
                                    wR = wR | BinaryStringToBitboard(square);
                                    wR = wR & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                case (Piece.White | Piece.Bishop):
                                    wB = wB | BinaryStringToBitboard(square);
                                    wB = wB & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                case Piece.White | Piece.Knight:
                                    wN = wN | BinaryStringToBitboard(square);
                                    wN = wN & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                case (Piece.White | Piece.Pawn):
                                    wP = wP | BinaryStringToBitboard(square);
                                    wP = wP & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                //Black Pieces
                                case Piece.Black | Piece.King:
                                    bK = bK | BinaryStringToBitboard(square);
                                    bK = bK & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                case (Piece.Black | Piece.Queen):
                                    bQ = bQ | BinaryStringToBitboard(square);
                                    bQ = bQ & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                case Piece.Black | Piece.Rook:
                                    bR = bR | BinaryStringToBitboard(square);
                                    bR = bR & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                case (Piece.Black | Piece.Bishop):
                                    bB = bB | BinaryStringToBitboard(square);
                                    bB = bB & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                case Piece.Black | Piece.Knight:
                                    bN = bN | BinaryStringToBitboard(square);
                                    bN = bN & ~BinaryStringToBitboard(tempSquare);
                                    break;

                                case (Piece.Black | Piece.Pawn):
                                    bP = bP | BinaryStringToBitboard(square);
                                    bP = bP & ~BinaryStringToBitboard(tempSquare);
                                    break;
                            }

                            // ADD MOVEHISTORY:  Piece, From, To, Captured
                            Moves.moveHistory.Add(new int[] { tempPiece, tempSquare, square, squares[square].piece });

                            //System.Diagnostics.Debug.WriteLine(Moves.moveHistory[0][0].ToString() + Moves.moveHistory[0][1].ToString() + Moves.moveHistory[0][2].ToString() + Moves.moveHistory[0][3].ToString());


                            Moves.whiteTurn = !Moves.whiteTurn;
                            
                            
                            // REVERSE BOARD (In future... to develop)
                            //BitboardOutput(Moves.whiteTurn);


                            //Update the square
                            squares[square].piece = tempPiece;
                            //assign texture and moves
                            squares[square].AssignPiece();

                            //Initialise bitboards in the Moves class
                            Moves.InitBitboards();


                            //end loop
                            goto loopEnd2;
                        }
                    }
                }

                //If can't place, place back at normal position
                squares[tempSquare].piece = tempPiece;
                squares[tempSquare].AssignPiece();

            }
            loopEnd2:
            { } //pass
        }


        private void LoadFEN(string FEN) //Makes FEN readable for computer
        {
            //default all bitboards
            wK = 0L; wQ = 0L; wR = 0L; wN = 0L; wB = 0L; wP = 0L; bK = 0L; bQ = 0L; bR = 0L; bN = 0L; bB = 0L; bP = 0L;



            // With bitboards, the FEN is first converted to a bitboard then the squares are assigned the pieces

            //example FEN 
            //  4kr2/p2p1r1p/2b1q3/8/2P5/1P6/P7/2RQR1K1 w - - 0 1

            /*
            Dictionary<char, int> pieceDic = new Dictionary<char, int>()
            {
                //White pieces
                { 'K', Piece.White | Piece.King }, //Use of the OR operator for reference
                { 'P', Piece.White | Piece.Pawn },
                { 'N', Piece.White | Piece.Knight },
                { 'B', Piece.White | Piece.Bishop },
                { 'R', Piece.White | Piece.Rook },
                { 'Q', Piece.White | Piece.Queen },

                //Black pieces
                { 'k', Piece.Black | Piece.King },
                { 'p', Piece.Black | Piece.Pawn },
                { 'n', Piece.Black | Piece.Knight },
                { 'b', Piece.Black | Piece.Bishop },
                { 'r', Piece.Black | Piece.Rook },
                { 'q', Piece.Black | Piece.Queen },
            };
            */

            string[] fenList = FEN.Split("/");

            int i = 0;

            

            //Checks how to deal with each symbol and does it
            foreach (string e in fenList)
            {
                //default all bitboards

                foreach (char c in e)
                {
                    //if integer then add to index by integer
                    if (int.TryParse(c.ToString().AsSpan(), out int n))
                    {
                        i += n;
                    }

                    //else if string
                    else
                    {
                        //populate bitboard
                        switch(c)
                        {
                            case 'K': //White King
                                wK += BinaryStringToBitboard(i);
                                break;
                            case 'Q': //White Queen
                                wQ += BinaryStringToBitboard(i);
                                break;
                            case 'R': //White Rook
                                wR += BinaryStringToBitboard(i);
                                break;
                            case 'B': //White Bishop
                                wB += BinaryStringToBitboard(i);
                                break;
                            case 'N': //White Knight
                                wN += BinaryStringToBitboard(i);
                                break;
                            case 'P': //White Pawn
                                wP += BinaryStringToBitboard(i);
                                break;

                            case 'k': //Black King
                                bK += BinaryStringToBitboard(i);
                                break;
                            case 'q': //Black Queen
                                bQ += BinaryStringToBitboard(i);
                                break;
                            case 'r': //Black Rook
                                bR += BinaryStringToBitboard(i);
                                break;
                            case 'b': //Black Bishop
                                bB += BinaryStringToBitboard(i);
                                break;
                            case 'n': //Black Knight
                                bN += BinaryStringToBitboard(i);
                                break;
                            case 'p': //Black Pawn
                                bP += BinaryStringToBitboard(i);
                                break;
                        }

                        i++;
                    }

                    //if finished all squares then break... still need to assign remaining FEN values tho
                    if (i >= 64)
                    {
                        break;
                    }
                }
            }

            /*
            //Checks how to deal with each symbol and does it
            foreach (string e in fenList)
            {
                foreach (char c in e)
                {
                    //if integer then add to index by integer
                    if (int.TryParse(c.ToString().AsSpan(), out int n))
                    {

                        //overwrite current pieces on said squares
                        for (int j = 0; j <= n-1; j++)
                        {
                            squares[i + j].piece = Piece.None;
                        }

                        //squares[i].piece = Piece.None;

                        i += n;
                    }

                    //else if string
                    else
                    {
                        squares[i].piece = pieceDic[c];
                        //assign the texture and legal moves that the piece has
                        squares[i].AssignPiece();

                        i++;
                    }

                    //if finished then break to assign the remaining values
                    if (i >= 64)
                    {
                        break;
                    }

                }
            }*/

            //Leave at true for now...
            BitboardOutput(true);

        }

        //Make bitboard into the game board
        void BitboardOutput(bool asWhite)
        {
            int sideIndex = 64;

            Moves.InitBitboards();

            //Init PiecesInfo
            piecesInfo = new List<int>();


            //wK = Moves.LegalMoves_WPawn(wP);


            //read bitboards as index
            //the bitwise operations will reverse the board so initially the board will be on blacks side
            for (int i = 0; i < 64; i++)
            {
                //For switching the board side
                if (asWhite)
                {
                    sideIndex -= 1;
                }
                else { sideIndex = i; }



                //Assign squares; //Assing to piece information // Piece Information: Colour | Piece | Location
                //**Implement flags and other piece information
                if (((wK >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.White | Piece.King;
                    // Piece Information: Colour | Piece | Location
                    piecesInfo.Add((Piece.White | Piece.King) << 6 | (63 - i));    
                }
                else if (((wQ >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.White | Piece.Queen;
                    // Piece Information: Colour | Piece | Location 
                    piecesInfo.Add((Piece.White | Piece.Queen) << 6 | (63 - i));
                }
                else if (((wR >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.White | Piece.Rook;
                    // Piece Information: Colour | Piece | Location
                    piecesInfo.Add((Piece.White | Piece.Rook) << 6 | (63 - i));
                }
                else if (((wB >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.White | Piece.Bishop;
                    // Piece Information: Colour | Piece | Location
                    piecesInfo.Add((Piece.White | Piece.Bishop) << 6 | (63 - i));

                    System.Diagnostics.Debug.WriteLine((63 - i));
                }
                else if (((wN >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.White | Piece.Knight;
                    // Piece Information: Colour | Piece | Location
                    piecesInfo.Add((Piece.White | Piece.Knight) << 6 | (63 - i));
                }
                else if (((wP >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.White | Piece.Pawn;
                    // Piece Information: Colour | Piece | Location
                    piecesInfo.Add((Piece.White | Piece.Pawn) << 6 | (63 - i));
                }

                else if (((bK >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.Black | Piece.King;
                    // Piece Information: Colour | Piece | Location 
                    piecesInfo.Add((Piece.Black | Piece.King) << 6 | (63 - i));
                }
                else if (((bQ >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.Black | Piece.Queen;
                    // Piece Information: Colour | Piece | Location
                    piecesInfo.Add((Piece.Black | Piece.Queen) << 6 | (63 - i));
                }
                else if (((bR >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.Black | Piece.Rook;
                    // Piece Information: Colour | Piece | Location
                    piecesInfo.Add((Piece.Black | Piece.Rook) << 6 | (63 - i));
                }
                else if (((bB >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.Black | Piece.Bishop;
                    // Piece Information: Colour | Piece | Location
                    piecesInfo.Add((Piece.Black | Piece.Bishop) << 6 | (63 - i));
                }
                else if (((bN >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.Black | Piece.Knight;
                    // Piece Information: Colour | Piece | Location
                    piecesInfo.Add((Piece.Black | Piece.Knight) << 6 | (63 - i));
                }
                else if (((bP >> i) & 1L) == 1L) { squares[sideIndex].piece = Piece.Black | Piece.Pawn;
                    // Piece Information: Colour | Piece | Location
                    piecesInfo.Add((Piece.Black | Piece.Pawn) << 6 | (63 - i));
                }
                
                else { squares[sideIndex].piece = Piece.None; }

                

                squares[sideIndex].AssignPiece();
            }

            
        }

        public static ulong BinaryStringToBitboard( int index )
        {
            string binary = "0000000000000000000000000000000000000000000000000000000000000000"; //64 bit

            //insert 1 in index
            binary = binary.Remove(index, 1).Insert(index, "1");

            //return as type long
            return Convert.ToUInt64(binary, 2);
        }
    }
}
