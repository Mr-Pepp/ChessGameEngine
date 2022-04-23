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

        //EndGameScreen
        private EndGameScreen endGameScreen;

        //Promotion Screen
        public static PromotionScreen promotionScreen;

        //Side options
        private SideOptions sideOptions;

        //For undoing moves
        public static Stack<MoveInfo> moveHistory = new Stack<MoveInfo>();

        //pieceInfo //Piece information Piece
        //Stores the information of all the pieces on the board
        // 
        // Piece Information: Colour | Piece | Location
        //public static List<int> piecesInfo = new List<int>() { }; 
        //Use a 64 bit array (instead)
        //public static int[] piecesInfo = new int[64]; //Creates a 64 length array

        Dictionary<string, Color> _colorDic;

        private int _squareSize;
        private Vector2 _initPos;
        private Color colour;

        // Promotion:
        //Used for promotion to know the piece that is promoted to (Does not include colour of the piece)
        private int promotionPiece;
        // Promotion piece colour
        private int promotionColour;
        //Promotion square
        private Square promotionSquare;

        //for selecting piece
        private bool pieceSelected = false;
        private int tempPiece;
        public static int square;
        private Texture2D mousePiece;
        private bool mousePressed;
        private Rectangle mousePieceRect;

        public static Position position;

        //Public Blank texture to pass
        public Texture2D blankTexture;

        // To square bitboar to pass into promotion function
        public static ulong _toSquareBitboard;
        public static int _toSquare;


        private string defaultFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"; //default position
        //private string FEN = "r5nr/1pp2pp1/3q4/2b1P2p/1NK2Pk1/2BP1BR1/PP1Q1P1p/8 w - - 0 1"; //debug position
        //private string FEN = "8/6bb/8/8/R1p3k1/4P3/P2P4/K7 b - - 0 1";
        private string customFEN = "8/8/3K4/8/8/8/8/4kr2 w - - 0 1";
        

        //For when the piece is selected
        //Make a list of moves that are only appropriate 
        List<int> fromMoves;
        //For storing the generated moves in current position
        public static List<int> moves;


        public Board(int squareSize, Vector2 initPos, Dictionary<string, Color> colorDic)
        {
            _initPos = initPos;
            _squareSize = squareSize;
            _colorDic = colorDic;
        }

        //Loaded right away, ran once
        public void LoadContent(SpriteBatch _spriteBatch, GraphicsDevice _graphicsDevice)
        {
            // create a white texture pixel
            blankTexture = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
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

            promotionScreen = new PromotionScreen(blankTexture, _colorDic["Promotion Background"], _squareSize);

            endGameScreen = new EndGameScreen(blankTexture, _colorDic["Endgame Background"], _colorDic["Endgame Line"], _squareSize)
            {
                position = _initPos,
            };

            sideOptions = new SideOptions(_squareSize, _initPos);

            // Class on load
            endGameScreen.OnLoad();
            sideOptions.OnLoad();

            //run load FEN position
            LoadFEN(defaultFEN);

        }

        public static int moveAmount = 0;


        //Update, looped
        public void Update()
        {
            

            if (GameState.state == 0) // If not check or stalemate
            {
                // Restart game for 'R'
                if (Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    RestartGame();
                }

                else if (Keyboard.GetState().IsKeyDown(Keys.T))
                {
                    // Undo move

                    if (moveHistory.Count != 0) // Greater than 0
                    {
                      UndoMove(moveHistory.Pop());
                    }

                }

                // Force Engine moving for 'E'
                else if (Keyboard.GetState().IsKeyDown(Keys.E) && GameState.playerMove == true)
                {
                    System.Threading.Thread.Sleep(500);
                    GameState.playerMove = false;
                }

                // Load custom position (Hardcoded)
                else if (Keyboard.GetState().IsKeyDown(Keys.L))
                {
                    // Restart game values
                    RestartGame();
                    LoadFEN(customFEN);
                }

                if (GameState.playerMove) // Player move
                {

                    PieceSelection();

                    mousePressed = Mouse.GetState().LeftButton == ButtonState.Pressed;
                    
                    if (!pieceSelected && mousePressed)
                    {
                        SelectSideOptions();
                    }
                }

                else // Computer move
                {
                    GameState.engineCalculating = true;

                    System.Diagnostics.Debug.WriteLine("Starting Calculating");
                    DateTime time = DateTime.Now;
                    
                    //System.Diagnostics.Debug.WriteLine(Engine.MiniMax(3, position.whiteTurn));
                    
                    Engine.maxMove engineMove;

                    if (position.whiteTurn) // White to play
                    {
                        // Negate side to switch the evaluation score
                        // Set alpha as lowest, set beta as highest
                        engineMove = Engine.NegaMax(3, -9999, 9999, -1);
                    }
                    else // Black to play
                    {
                        // Negate side to switch the evaluation score
                        // Set alpha as lowest, set beta as highest
                        engineMove = Engine.NegaMax(3, -9999, 9999, 1);
                    }
                    

                    System.Diagnostics.Debug.WriteLine(engineMove.max);

                    System.Diagnostics.Debug.WriteLine(DateTime.Now - time);

                    System.Diagnostics.Debug.WriteLine("After: " + position.whiteTurn);

                    GameState.engineCalculating = false;
                    
                    
                    MakeMoveOnBoard(engineMove.move);

                    System.Diagnostics.Debug.WriteLine("After MOVE: " + position.whiteTurn);

                    if ((engineMove.move.flag & 0b11000) != 0) // Checkmate or stalemate
                    {
                        if (engineMove.move.flag >> 4 == 1) // Stalemate
                        {
                            GameState.state = 2;
                        }
                        else // Checkmate
                        {
                            GameState.state = 1;
                        }
                    }


                    /*
                    // Random moves
                    int RandMove = Engine.RandomMove(position);
                    
                    if (RandMove >> 15 == 0) // Not checkmate or stalemate
                    {
                        MoveInfo move = MoveFormat(RandMove);

                        moveAmount++;
                        System.Diagnostics.Debug.WriteLine(moveAmount);
                        
                        MakeMoveOnBoard(move);
                    }

                    else
                    {
                        if (((RandMove >> 15) & 1) == 1) // Checkmate
                        {
                            GameState.state = 1;
                        }
                        else // Stalemate
                        {
                            GameState.state = 2;
                        }
                    }*/
                }
            }
            else if (GameState.state == 1 | GameState.state == 2) // Checkmate or stalemate (endgame screen)
            {
                //Pressed left mouse button
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (endGameScreen.restartRect.Contains(Game1.mousePoint)) // Restart game
                    {
                        RestartGame();
                        // Set gamestate back to playing
                        GameState.state = 0;
                    }
                    else if (endGameScreen.exitRect.Contains(Game1.mousePoint))
                    {
                        Game1.self.Exit();
                    }
                }
            }
            else // Promotion
            {
                Promotion(_toSquareBitboard, _toSquare);
            }

        }


        //Draw, looped
        public void Draw(SpriteBatch _spriteBatch)
        {
            outsideBoard.Draw(_spriteBatch);

            sideOptions.Draw(_spriteBatch);

            for (int i = 0; i <= 63; i++)
            {
                squares[i].Draw(_spriteBatch);
            }

            if (pieceSelected)
            {
                //draw piece at mouse point
                _spriteBatch.Draw(mousePiece, mousePieceRect, Color.White);
            }

            //Will draw if there is a check / stalemate
            if (GameState.state == 1 | GameState.state == 2)
            {
                endGameScreen.Draw(_spriteBatch);
            }
            
            else if (GameState.state == 3) // Promotion
            {
                promotionScreen.Draw(_spriteBatch);
            }

        }


        private int SquareID(int x, int y) //Get square number from top left considering column (x) and row (y)
        {
            return ((y - 1) * 8 + x - 1);
        }


        // To select the options on the side
        private void SelectSideOptions ()
        {
            // Check if mouse is selecting one of the rectangles

            // Selected white side
            if (sideOptions.whiteSideRect.Contains(Game1.mousePoint) && !position.whiteTurn)
            {
                // Make engine move (will automatically switch sides)
                GameState.playerMove = false;
            }
            // Selected black side
            else if (sideOptions.blackSideRect.Contains(Game1.mousePoint) && position.whiteTurn)
            {
                // Make engine move (will automatically switch sides)
                GameState.playerMove = false;
            }
            // Selected undo move
            else if (sideOptions.backRect.Contains(Game1.mousePoint))
            {
                // Undo move
                if (moveHistory.Count > 0) // Stack is not empty
                {
                    UndoMove(moveHistory.Pop());
                }

                // Sleep to not move quick
                System.Threading.Thread.Sleep(250);
            }
            // Selected restart game
            else if (sideOptions.restartRect.Contains(Game1.mousePoint))
            {
                // Restart game to default
                RestartGame();
            }

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


                            //Generate all the moves and add it to the moves list
                            //Then export all the (to) moves into another list based on the (from) matching the current square

                            //Fetch all the moves from the current square
                            fromMoves = new List<int>();


                            //Go through the generated moves and select the appropriate move
                            foreach (int e in moves)
                            {
                                /*
                                 * Flags (0b111):
                                 * 000 = Normal Move
                                 * 001 = Captures
                                 * 010 = Evasive (From check)
                                 * 011 = Promotion
                                 * 100 = Castles Queen Side
                                 * 101 = Castles King Side
                                 * 110 = En Passant
                                 * 
                                 * 
                                 */

                                //If the from destination is correct then store it in the list that allow the piece to go to the right square
                                if ((e & 0b111111) == square) //flag | to | from
                                {
                                    //System.Diagnostics.Debug.WriteLine(e & 0b111111);
                                    fromMoves.Add(e); //Will only contain the moves from the selected square
                                }
                            }

                            goto loopEnd;
                        }
                    }
                }


                goto loopEnd;


            }

        //Used since we are breaking from a nested loop
        loopEnd:

            //To store the "to" value from the element of fromMoves
            int to;
            //To store the "flag" value from the element of fromMoves
            int flag = 0; // If "to" square is valid then the flag will be assigned

            //To check if the square that is being moves to is correct
            bool validSquare = false;


            //holding the piece
            if (pieceSelected && mousePressed)
            {
                //place piece at mouse point ---- !! Future make this run only once !!
                mousePieceRect = new Rectangle(Game1.mousePoint.X - (_squareSize / 2), Game1.mousePoint.Y - (_squareSize / 2), _squareSize, _squareSize);

                //Need to find a way to do this with bitboards
                foreach (int e in fromMoves)
                {
                    to = e >> 6 & 0b111111;
                    //System.Diagnostics.Debug.WriteLine(to);

                    //Show available moves
                    if (squares[to].piece == 0)
                    {
                        squares[to].dot = true;
                    }

                    else // there is a piece
                    {
                        //colour square to indicate the ability to take the piece

                        squares[to].targetSquare = true;
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
                    foreach (int e in fromMoves)
                    {
                        to = e >> 6 & 0b111111;

                        //Remove
                        squares[to].dot = false;
                        squares[to].targetSquare = false;
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

                
                foreach (int e in fromMoves)
                {
                    to = e >> 6 & 0b111111;

                    //Remove
                    squares[to].dot = false;
                    squares[to].targetSquare = false;
                }

                //place Piece at current square
                for (int iy = 1; iy <= 8; iy++)
                {
                    for (int ix = 1; ix <= 8; ix++)
                    {
                        square = SquareID(iy, ix);
                        //Assigning before to avoid bugs
                        validSquare = false; 

                        //Loop through the fromMoves to see if it can be placed on the right sqauare and fetch information from
                        foreach (int e in fromMoves)
                        {
                            to = (e >> 6 & 0b111111);
                            if (to == square) // The square is valid with the "to" value
                            {
                                validSquare = true;
                                //Extract the flag of the piece placement
                                // Flags are 3 bit
                                flag = (e >> 12) & 0b111;
                                break;
                            }
                        }


                        //add rules to if
                        //square is at current mouse position
                        //Flag | To | From
                        if (squares[square].rect.Contains(Game1.mousePoint) && validSquare)
                        {

                            //The main issue is that I would have to update the new board and remove the old piece from it.
                            //This is a pain because
                            /*
                             * I have no clue how to do this efficiently
                             * From a list you have to do a bunch of searches so it makes sense to use a 64 array?.. So that's what I'm using...
                             * 
                             */
                            /*
                            //Remove piece from pieceInfo and add the new piece location
                            // Consider --- Pawn Promotion, Captures
                            foreach (int e in piecesInfo)//Loops through the pieceInfo list
                            {
                                if ((e & 0b111111) == tempSquare) //Remove previous square
                                {
                                    //piecesInfo.Remove(e);
                                }

                                else if ((e & 0b111111) == square) //Remove current square piece
                                {
                                    //piecesInfo.Remove(e);
                                }
                            }*/


                            //To store the bitboard of the "to" square
                            ulong toSquareBitboard = BinaryStringToBitboard(square);
                            //To store the bitboard of the "from" square
                            ulong fromSquareBitboard = BinaryStringToBitboard(tempSquare);

                            MoveInfo move = new MoveInfo();

                            move.fromSquare = tempSquare;
                            move.toSquare = square;
                            move.flag = flag;// Can be castling.. etc

                            move.capturedPiece = squares[square].piece;

                            // For updating the bitboards in position
                            move.toSquareBitboard = toSquareBitboard;
                            move.fromSquareBitboard = fromSquareBitboard;

                            move.piece = tempPiece; // The piece that is moving 

                            MakeMoveOnBoard(move);

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


        private void Promotion(ulong toSquareBitboard, int toSquare)
        {
            //If mouse pressed
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                //Assing promotion square
                promotionSquare = squares[square];

                //Check if pressed on a promotion piece
                // If pressed on the bishop
                if (promotionScreen.bishopRect.Contains(Game1.mousePoint))
                {
                    promotionPiece = Piece.Bishop;

                    //Update bishop bitboard and set the correct piece colour
                    if (position.whiteTurn) // White to move
                    {
                        //Set piece colour
                        promotionColour = Piece.White;
                        position.wBCount++;

                        //Update the piece's bitboard
                        position.wB = position.wB | toSquareBitboard;
                        position.whiteBishop.Add(toSquare);
                    }
                    else // Black to move
                    {
                        //Set piece colour
                        promotionColour = Piece.Black;
                        position.bBCount++;

                        //Update the piece's bitboard
                        position.bB = position.bB | toSquareBitboard;
                        position.blackBishop.Add(toSquare);
                    }

                    //Set state back
                    GameState.state = 0;
                }

                //If pressed on the rook
                else if (promotionScreen.rookRect.Contains(Game1.mousePoint))
                {
                    promotionPiece = Piece.Rook;

                    //Update rook bitboard and set the correct piece colour
                    if (position.whiteTurn) // White to move
                    {
                        //Set piece colour
                        promotionColour = Piece.White;
                        position.wRCount++;

                        //Update the piece's bitboard
                        position.wR = position.wR | toSquareBitboard;
                        position.whiteRook.Add(toSquare);
                    }
                    else // Black to move
                    {
                        //Set piece colour
                        promotionColour = Piece.Black;
                        position.bRCount++;

                        //Update the piece's bitboard
                        position.bR = position.bR | toSquareBitboard;
                        position.blackRook.Add(toSquare);
                    }


                    //Set state back
                    GameState.state = 0;
                }

                //If pressed on the queen
                else if (promotionScreen.queenRect.Contains(Game1.mousePoint))
                {
                    promotionPiece = Piece.Queen;

                    //Update rook bitboard and set the correct piece colour
                    if (position.whiteTurn) // White to move
                    {
                        //Set piece colour
                        promotionColour = Piece.White;
                        position.wQCount++;

                        //Update the piece's bitboard
                        position.wQ = position.wQ | toSquareBitboard;
                        position.whiteQueen.Add(toSquare);
                    }
                    else // Black to move
                    {
                        //Set piece colour
                        promotionColour = Piece.Black;
                        position.bQCount++;

                        //Update the piece's bitboard
                        position.bQ = position.bQ | toSquareBitboard;
                        position.blackQueen.Add(toSquare);
                    }

                    //Set state back
                    GameState.state = 0;
                }

                //If pressed on the knight
                else if (promotionScreen.knightRect.Contains(Game1.mousePoint))
                {
                    promotionPiece = Piece.Knight;

                    //Update rook bitboard and set the correct piece colour
                    if (position.whiteTurn) // White to move
                    {
                        //Set piece colour
                        promotionColour = Piece.White;
                        position.wNCount++;

                        //Update the piece's bitboard
                        position.wN = position.wN | toSquareBitboard;
                        position.whiteKnight.Add(toSquare);
                    }
                    else // Black to move
                    {
                        //Set piece colour
                        promotionColour = Piece.Black;
                        position.bNCount++;

                        //Update the piece's bitboard
                        position.bN = position.bN | toSquareBitboard;
                        position.blackKnight.Add(toSquare);
                    }

                    //Set state back
                    GameState.state = 0;
                }

                // State set back to 0, therefore a promotion piece was selected
                if (GameState.state == 0)
                {
                    // Assign the correct piece
                    promotionSquare.piece = promotionColour | promotionPiece;
                    // Assign promotion piece
                    promotionSquare.AssignPiece();

                    //End of turn
                    EndOfTurn();

                }
            }
        }

        public static void EndOfTurn()
        {
            // End of all moves this turn, therefore
            // End of turn; other colour turn
            position.whiteTurn = !position.whiteTurn;

            //Generate moves once a new position is established and the player is moving
            if (!GameState.engineCalculating)
            {
                moves = Moves.GenerateGameMoves(position);
                GameState.playerMove = !GameState.playerMove;

                // (Stalemate | Checkmate) (0b1100 0000 0000 0000) | flag | to | from

                if (moves.Count == 1) // Only one move, therefore could have returned gamestate
                {
                    if (moves[0] >> 15 == 1L) // Checkmate
                    {
                        // Checkmate State
                        GameState.state = 1;
                    }

                    else if (moves[0] >> 16 == 1L) // Stalemate
                    {
                        // Stalemate State
                        GameState.state = 2;
                    }
                }
            }
            
            
        }

        //Structure for making a move
        public struct MoveInfo
        {
            public int fromSquare;
            public int toSquare;
            public int flag; // Can be castling.. etc

            public int capturedPiece;

            // For updating the bitboards in position
            public ulong toSquareBitboard;
            public ulong fromSquareBitboard;

            public int piece; // The piece that is moving

            // Flag Move information (En passant)
            public ulong white_enPassantMask;
            public ulong black_enPassantMask;
            // Castling rights
            public int whiteCastles;
            public int blackCastles;
        }


        // move (flag | toSquare | fromSquare), for the engine to make moves
        public static MoveInfo MoveFormat(int move)
        {
            MoveInfo forMove = new MoveInfo();

            forMove.fromSquare = move & 0b111111;
            forMove.toSquare = move >> 6 & 0b111111;
            forMove.flag = move >> 12 & 0b111;// Can be castling.. etc (0b11000) says whether it's checkmate

            forMove.capturedPiece = squares[forMove.toSquare].piece;

            // For updating the bitboards in position
            forMove.toSquareBitboard = BinaryStringToBitboard(forMove.toSquare);
            forMove.fromSquareBitboard = BinaryStringToBitboard(forMove.fromSquare);

            forMove.piece = squares[forMove.fromSquare].piece; // The piece that is moving 

            //Positional updates
            forMove.white_enPassantMask = position.white_enPassantMask;
            forMove.black_enPassantMask = position.black_enPassantMask;

            forMove.whiteCastles = position.whiteCastles;
            forMove.blackCastles = position.blackCastles;

            return forMove;
        }



        // Undoing a move
        // All previous saves... Moves.whiteCastles, Moves.blackCastles,
        //Moves.white_enPassantMask, Moves.black_enPassantMask, Moves.whiteTurn
        public static void UndoMove(MoveInfo move)
        {
            int fromSquare = move.fromSquare;
            int toSquare = move.toSquare;
            int flag = move.flag;

            int capturedPiece = move.capturedPiece;

            ulong toSquareBitboard = move.toSquareBitboard;
            ulong fromSquareBitboard = move.fromSquareBitboard;

            // The piece that is moving
            int piece = move.piece;

            //Update castling bitboard because of potential king move / rook move
            position.whiteCastles = move.whiteCastles;
            position.blackCastles = move.blackCastles;


            if (flag == (int)Moves.Flag.Promotion) // Promoting a pawn (early to get the piece there)
            {
                int promotedPiece = squares[toSquare].piece & 0b111;

                
                if (GameState.playerMove)
                {
                    // Update the bitboards and counter
                    if (position.whiteTurn) // Black just promoted
                    {
                        // Add pawn to counter
                        position.bPCount++;

                        switch (promotedPiece)
                        {
                            case Piece.Queen:
                                position.bQ = position.bQ & ~toSquareBitboard;
                                position.bQCount--;
                                position.blackQueen.Remove(toSquare);
                                break;
                            case Piece.Knight:
                                position.bN = position.bN & ~toSquareBitboard;
                                position.bNCount--;
                                position.blackKnight.Remove(toSquare);
                                break;
                            case Piece.Rook:
                                position.bR = position.bR & ~toSquareBitboard;
                                position.bRCount--;
                                position.blackRook.Remove(toSquare);
                                break;
                            case Piece.Bishop:
                                position.bB = position.bB & ~toSquareBitboard;
                                position.bBCount--;
                                position.blackBishop.Remove(toSquare);
                                break;
                        }
                    }

                    else // White just promoted
                    {
                        // Add pawn to counter
                        position.wPCount++;

                        switch (promotedPiece)
                        {
                            case Piece.Queen:
                                position.wQ = position.wQ & ~toSquareBitboard;
                                position.wQCount--;
                                position.whiteQueen.Remove(toSquare);
                                break;
                            case Piece.Knight:
                                position.wN = position.wN & ~toSquareBitboard;
                                position.wNCount--;
                                position.whiteKnight.Remove(toSquare);
                                break;
                            case Piece.Rook:
                                position.wR = position.wR & ~toSquareBitboard;
                                position.wRCount--;
                                position.whiteRook.Remove(toSquare);
                                break;
                            case Piece.Bishop:
                                position.wB = position.wB & ~toSquareBitboard;
                                position.wBCount--;
                                position.whiteBishop.Remove(toSquare);
                                break;
                        }
                    }
                }

                else // Otherwise computer move; promoted to queen
                {
                    if (position.whiteTurn) // black just promoted
                    {
                        position.bPCount++;

                        position.bQ = position.bQ & ~toSquareBitboard;
                        position.bQCount--;
                        position.blackQueen.Remove(toSquare);
                    }
                    else // white just promoted
                    {
                        position.wPCount++;

                        position.wQ = position.wQ & ~toSquareBitboard;
                        position.wQCount--;
                        position.whiteQueen.Remove(toSquare);
                    }
                }
                
            }


            // Reverse

            //Piece bitboard update
            //Filter piece to update the correct bitboard

            //Update for the piece taking the square
            switch (piece)
            {
                //White Pieces
                case Piece.White | Piece.King:
                    position.wK = position.wK | fromSquareBitboard;
                    position.wK = position.wK & ~toSquareBitboard;

                    //Replace in mailbox
                    position.whiteKing.Remove(toSquare);
                    position.whiteKing.Add(fromSquare);
                    break;

                case (Piece.White | Piece.Queen):
                    position.wQ = position.wQ | fromSquareBitboard;
                    position.wQ = position.wQ & ~toSquareBitboard;

                    //Replace in mailbox
                    position.whiteQueen.Remove(toSquare);
                    position.whiteQueen.Add(fromSquare);
                    break;

                case Piece.White | Piece.Rook:
                    position.wR = position.wR | fromSquareBitboard;
                    position.wR = position.wR & ~toSquareBitboard;

                    //Replace in mailbox
                    position.whiteRook.Remove(toSquare);
                    position.whiteRook.Add(fromSquare);
                    break;

                case (Piece.White | Piece.Bishop):
                    position.wB = position.wB | fromSquareBitboard;
                    position.wB = position.wB & ~toSquareBitboard;

                    //Replace in mailbox
                    position.whiteBishop.Remove(toSquare);
                    position.whiteBishop.Add(fromSquare);
                    break;

                case Piece.White | Piece.Knight:
                    position.wN = position.wN | fromSquareBitboard;
                    position.wN = position.wN & ~toSquareBitboard;

                    //Replace in mailbox
                    position.whiteKnight.Remove(toSquare);
                    position.whiteKnight.Add(fromSquare);
                    break;

                case (Piece.White | Piece.Pawn):
                    position.wP = position.wP | fromSquareBitboard;
                    position.wP = position.wP & ~toSquareBitboard;

                    //Replace in mailbox
                    position.whitePawn.Remove(toSquare);
                    position.whitePawn.Add(fromSquare);
                    break;


                //Black Pieces
                case Piece.Black | Piece.King:
                    position.bK = position.bK | fromSquareBitboard;
                    position.bK = position.bK & ~toSquareBitboard;

                    //Replace in mailbox
                    position.blackKing.Remove(toSquare);
                    position.blackKing.Add(fromSquare);
                    break;

                case (Piece.Black | Piece.Queen):
                    position.bQ = position.bQ | fromSquareBitboard;
                    position.bQ = position.bQ & ~toSquareBitboard;

                    //Replace in mailbox
                    position.blackQueen.Remove(toSquare);
                    position.blackQueen.Add(fromSquare);
                    break;

                case Piece.Black | Piece.Rook:
                    position.bR = position.bR | fromSquareBitboard;
                    position.bR = position.bR & ~toSquareBitboard;

                    //Replace in mailbox
                    position.blackRook.Remove(toSquare);
                    position.blackRook.Add(fromSquare);
                    break;

                case (Piece.Black | Piece.Bishop):
                    position.bB = position.bB | fromSquareBitboard;
                    position.bB = position.bB & ~toSquareBitboard;

                    //Replace in mailbox
                    position.blackBishop.Remove(toSquare);
                    position.blackBishop.Add(fromSquare);
                    break;

                case Piece.Black | Piece.Knight:
                    position.bN = position.bN | fromSquareBitboard;
                    position.bN = position.bN & ~toSquareBitboard;

                    //Replace in mailbox
                    position.blackKnight.Remove(toSquare);
                    position.blackKnight.Add(fromSquare);
                    break;

                case (Piece.Black | Piece.Pawn):
                    position.bP = position.bP | fromSquareBitboard;
                    position.bP = position.bP & ~toSquareBitboard;

                    //Replace in mailbox
                    position.blackPawn.Remove(toSquare);
                    position.blackPawn.Add(fromSquare);
                    break;
            }

            if (capturedPiece != Piece.None)
            {
                switch (capturedPiece)
                {
                    //White Pieces
                    case (Piece.White | Piece.Queen):
                        position.wQ = position.wQ | toSquareBitboard;
                        position.wQCount++; // Add to piece count

                        // Add to mailbox
                        position.whiteQueen.Add(toSquare);
                        break;

                    case Piece.White | Piece.Rook:
                        position.wR = position.wR | toSquareBitboard;
                        position.wRCount++;

                        // Add to mailbox
                        position.whiteRook.Add(toSquare);
                        break;

                    case (Piece.White | Piece.Bishop):
                        position.wB = position.wB | toSquareBitboard;
                        position.wBCount++;

                        // Add to mailbox
                        position.whiteBishop.Add(toSquare);
                        break;

                    case Piece.White | Piece.Knight:
                        position.wN = position.wN | toSquareBitboard;
                        position.wNCount++;

                        // Add to mailbox
                        position.whiteKnight.Add(toSquare);
                        break;

                    case (Piece.White | Piece.Pawn):
                        position.wP = position.wP | toSquareBitboard;
                        position.wPCount++;

                        // Add to mailbox
                        position.whitePawn.Add(toSquare);
                        break;

                    //Black Pieces
                    case (Piece.Black | Piece.Queen):
                        position.bQ = position.bQ | toSquareBitboard;
                        position.bQCount++;

                        // Add to mailbox
                        position.blackQueen.Add(toSquare);
                        break;

                    case Piece.Black | Piece.Rook:
                        position.bR = position.bR | toSquareBitboard;
                        position.bRCount++;

                        // Add to mailbox
                        position.blackRook.Add(toSquare);

                        break;

                    case (Piece.Black | Piece.Bishop):
                        position.bB = position.bB | toSquareBitboard;
                        position.bBCount++;

                        // Add to mailbox
                        position.blackBishop.Add(toSquare);
                        break;

                    case Piece.Black | Piece.Knight:
                        position.bN = position.bN | toSquareBitboard;
                        position.bNCount++;

                        // Add to mailbox
                        position.blackKnight.Add(toSquare);
                        break;

                    case (Piece.Black | Piece.Pawn):
                        position.bP = position.bP | toSquareBitboard;
                        position.bPCount++;

                        // Add to mailbox
                        position.blackPawn.Add(toSquare);
                        break;
                }
            }

            // Set back the previous en passant masks
            position.black_enPassantMask = move.black_enPassantMask;
            position.white_enPassantMask = move.white_enPassantMask;


            squares[fromSquare].piece = piece;
            squares[fromSquare].AssignPiece();

            // Consider flags.. (en passant)
            if (flag == (int)Moves.Flag.En_Passant)
            {
                // Remove current piece
                squares[toSquare].piece = Piece.None;

                // Place back the piece
                if (position.whiteTurn) // Black just made the move
                {
                    // Set bitboard
                    position.wP = position.wP | move.white_enPassantMask << 8;
                    

                    position.wPCount++;

                    toSquare -= 8;
                    position.whitePawn.Add(toSquare);
                    //Captured piece
                    capturedPiece = Piece.White | Piece.Pawn;
                }
                else // White just made the move
                {
                    // Set bitboard
                    position.bP = position.bP | move.black_enPassantMask >> 8;

                    position.bPCount++;

                    toSquare += 8;
                    position.blackPawn.Add(toSquare);
                    //Captured piece
                    capturedPiece = Piece.Black | Piece.Pawn;
                }
            }

            else if (flag == (int)Moves.Flag.Castles_QS) // Castling queen side
            {
                // Place back the piece
                if (position.whiteTurn) // Black just made the move
                {
                    // Moving rook back
                    // On board
                    squares[3].piece = Piece.None;
                    position.blackRook.Remove(3);
                    squares[0].piece = Piece.Black | Piece.Rook;
                    position.blackRook.Add(0);

                    //Update bitboard
                    position.bR = (position.bR | Moves.TLCorner) & ~Moves.qsCastleRook_black;

                    // Castling rights
                    //position.blackCastles = position.blackCastles | 0b10;
                }
                else // White just made the move
                {
                    // Moving rook back
                    // On board
                    squares[59].piece = Piece.None;
                    position.whiteRook.Remove(59);
                    squares[56].piece = Piece.White | Piece.Rook;
                    position.whiteRook.Add(56);

                    //Update bitboard
                    position.wR = (position.wR | Moves.BLCorner) & ~Moves.qsCastleRook_white;

                    // Castling rights
                    //position.whiteCastles = position.whiteCastles | 0b10;

                }
            }
            else if (flag == (int)Moves.Flag.Castles_KS) // Castling king side
            {
                // Place back the piece
                if (position.whiteTurn) // Black just made the move
                {
                    // Moving rook back
                    // On board
                    squares[5].piece = Piece.None;
                    position.blackRook.Remove(5);
                    squares[7].piece = Piece.Black | Piece.Rook;
                    position.blackRook.Add(7);

                    //Update bitboard
                    position.bR = (position.bR | Moves.TRCorner) & ~Moves.ksCastleRook_black;

                    //Castling righs
                    //position.blackCastles = position.blackCastles | 0b01;

                }
                else // White just made the move
                {
                    // Moving rook back
                    // On board
                    squares[61].piece = Piece.None;
                    position.whiteRook.Remove(61);
                    squares[63].piece = Piece.White | Piece.Rook;
                    position.whiteRook.Add(63);

                    //Update bitboard
                    position.wR = (position.wR | Moves.BRCorner) & ~Moves.ksCastleRook_white;

                    //Castling righs
                    //position.whiteCastles = position.whiteCastles | 0b01;
                }
            }

            squares[toSquare].piece = capturedPiece;
            squares[toSquare].AssignPiece();

            //Switch colours
            position.whiteTurn = !position.whiteTurn;

            if (!GameState.engineCalculating)
            {
                // Generate moves again
                moves = Moves.GenerateGameMoves(position);
            }
            
        }

        // Used for restarting the game
        // Used for restarting the game
        private void RestartGame()
        {
            //Format all
            position.white_enPassantMask = 0;
            position.black_enPassantMask = 0;
            position.whiteCastles = 0b11;
            position.blackCastles = 0b11;

            //load starting pos
            LoadFEN(defaultFEN);

            //Set to player move
            GameState.playerMove = true;

            pieceSelected = false;
            position.whiteTurn = true;
        }

        // Making a move on board **
        public static void MakeMoveOnBoard(MoveInfo move)
        {

            //Positional updates
            move.white_enPassantMask = position.white_enPassantMask;
            move.black_enPassantMask = position.black_enPassantMask;

            move.whiteCastles = position.whiteCastles;
            move.blackCastles = position.blackCastles;

            //Add move to history stack
            if (!GameState.engineCalculating)
            {
                moveHistory.Push(move);
            }
            

            int fromSquare = move.fromSquare;
            int toSquare = move.toSquare;
            int flag = move.flag; // Can be castling.. etc

            int capturedPiece = move.capturedPiece;

            // For updating the bitboards in position
            ulong toSquareBitboard = move.toSquareBitboard;
            ulong fromSquareBitboard = move.fromSquareBitboard;

            // Set the piece that is moving
            int piece = move.piece; 

            //Set global for promotion
            _toSquareBitboard = toSquareBitboard;
            _toSquare = toSquare;

            PieceBitboardUpdate(toSquareBitboard, toSquare, fromSquareBitboard, piece, capturedPiece, fromSquare, flag);

            //Update the square
            squares[toSquare].piece = piece;
            //assign texture and moves
            squares[toSquare].AssignPiece();

            //Remove previous piece
            if (!GameState.playerMove) // Computer moving
            {
                //Remove piece from board
                squares[fromSquare].piece = Piece.None;
            }

            // Format both En Passant masks **
            position.white_enPassantMask = 0L;
            position.black_enPassantMask = 0L;

            /*
                 * Flags (0b111):
                 * 000 = Normal Move
                 * 001 = Captures
                 * 010 = Evasive (From check)
                 * 011 = Promotion
                 * 100 = Castles Queen Side
                 * 101 = Castles King Side
                 * 110 = En Passant
                 * 111 = Double Push
                 */

            //Check flag
            if (flag == (int)Moves.Flag.Promotion) // Pawn promotion
            {
                //Check which side to takeaway pawn from counter and update bitboard
                if (position.whiteTurn) // White to move
                {
                    position.wPCount--;
                    // Remove pawn from bitboard
                    position.wP = position.wP & ~toSquareBitboard;
                    position.whitePawn.Remove(toSquare);

                }
                else // Black to move
                {
                    position.bPCount--;
                    // Remove pawn from bitboard
                    position.bP = position.bP & ~toSquareBitboard;
                    position.blackPawn.Remove(toSquare);
                }


                if (GameState.playerMove) // Player move show screen
                {
                    //Set state for promotion
                    GameState.state = 3;
                    //Update class with square position
                    promotionScreen.Update(position, squares[toSquare].position);
                }
                else // Computer move (Always select queen)
                {
                    // Pawn to queen
                    if (position.whiteTurn)
                    {
                        squares[toSquare].piece = Piece.White | Piece.Queen;
                        position.wQCount++; // Add to piece counter

                        position.whiteQueen.Add(toSquare);

                        // Update bitboards
                        position.wQ = position.wQ | toSquareBitboard;
                    }
                    else
                    {
                        squares[toSquare].piece = Piece.Black | Piece.Queen;
                        position.bQCount++; // Add to piece counter

                        position.blackQueen.Add(toSquare);

                        // Update bitboards
                        position.bQ = position.bQ | toSquareBitboard;
                    }

                    squares[toSquare].AssignPiece();

                    // End of turn
                    EndOfTurn();
                }
            }
            else if (flag == (int)Moves.Flag.Double_Push) // Double push by pawn
            {
                // Mark en passant location
                if (position.whiteTurn) // White to play
                {
                    // Set en passant location
                    position.white_enPassantMask = BinaryStringToBitboard(toSquare + 8);
                }
                else // Black to play
                {
                    // Set en passant location
                    position.black_enPassantMask = BinaryStringToBitboard(toSquare - 8);
                }

                EndOfTurn();
            }
            else if (flag == (int)Moves.Flag.Castles_KS) // Kingside castle
            {
                // Adjust rook
                if (position.whiteTurn) // White to play
                {
                    // Switch rooks
                    squares[63].piece = Piece.None;
                    position.whiteRook.Remove(63);
                    squares[61].piece = Piece.White | Piece.Rook;
                    position.whiteRook.Add(61);

                    // Assign piece
                    squares[61].AssignPiece();

                    //Update bitboards
                    position.wR = (position.wR & ~Moves.BRCorner) | Moves.ksCastleRook_white;
                }
                else // Black to play
                {
                    // Switch rooks
                    squares[7].piece = Piece.None;
                    position.blackRook.Remove(7);
                    squares[5].piece = Piece.Black | Piece.Rook;
                    position.blackRook.Add(5);

                    // Assign piece
                    squares[5].AssignPiece();

                    //Update bitboards
                    position.bR = (position.bR & ~Moves.TRCorner) | Moves.ksCastleRook_black;
                }
                // End of turn
                EndOfTurn();
            }
            else if (flag == (int)Moves.Flag.Castles_QS) // Queenside castle
            {
                // Adjust rook
                if (position.whiteTurn) // White to play
                {
                    // Switch rooks
                    squares[56].piece = Piece.None;
                    position.whiteRook.Remove(56);
                    squares[59].piece = Piece.White | Piece.Rook;
                    position.whiteRook.Add(59);

                    // Assign piece
                    squares[59].AssignPiece();

                    //Update bitboards
                    position.wR = (position.wR & ~Moves.BLCorner) | Moves.qsCastleRook_white;
                }
                else // Black to play
                {
                    // Switch rooks
                    squares[0].piece = Piece.None;
                    position.blackRook.Remove(0);
                    squares[3].piece = Piece.Black | Piece.Rook;
                    position.blackRook.Add(3);

                    // Assign piece
                    squares[3].AssignPiece();

                    //Update bitboards
                    position.bR = (position.bR & ~Moves.TLCorner) | Moves.qsCastleRook_black;
                }

                // End of turn
                EndOfTurn();
            }
            else if (flag == (int)Moves.Flag.En_Passant) // En passant move
            {
                //Check which side
                if (position.whiteTurn) // White turn 
                {
                    // Remove pawn below
                    squares[toSquare + 8].piece = Piece.None;
                    position.blackPawn.Remove(toSquare + 8);
                    squares[toSquare + 8].AssignPiece();

                    //Update bitboard
                    position.bP = position.bP & ~(toSquareBitboard >> 8);
                    position.bPCount--; // takeaway from counter
                }
                else // Black turn
                {
                    // Remove pawn above
                    squares[toSquare - 8].piece = Piece.None;
                    position.whitePawn.Remove(toSquare - 8);
                    squares[toSquare - 8].AssignPiece();

                    //Update bitboard
                    position.wP = position.wP & ~(toSquareBitboard << 8);
                    position.wPCount--; // takeaway from counter
                }

                //End of turn
                EndOfTurn();
            }
            else
            {
                //En passant not moved
                //Set En passant = 0

                //End of turn
                EndOfTurn();
            }
        }



        static void PieceBitboardUpdate(ulong toSquareBitboard, int toSquare, ulong fromSquareBitboard, int pieceMoving,
            int capturedPiece, int fromSquare, int flag)
        {
            //Castling (Captures of a rook)
            if (position.whiteTurn) // White to move
            {
                if (toSquareBitboard == (position.bR & Moves.TRCorner)) // Kingside rook captures
                {
                    position.blackCastles = position.blackCastles & 0b10;
                }
                else if (toSquareBitboard == (position.bR & Moves.TLCorner)) // Queenside rook captures
                {
                    position.blackCastles = position.blackCastles & 0b01;
                }
            }
            else // Black to move
            {
                if (toSquareBitboard == (position.wR & Moves.BRCorner)) // Kingside rook captures
                {
                    position.whiteCastles = position.whiteCastles & 0b10;
                }
                else if (toSquareBitboard == (position.wR & Moves.BLCorner)) // Queenside rook captures
                {
                    position.whiteCastles = position.whiteCastles & 0b01;
                }
            }

            //Filter piece to update the correct bitboard

            //First update the targetted square piece (won't do anything if the piece is Piece.None)
            switch (capturedPiece)
            {
                //White Pieces
                case (Piece.White | Piece.Queen):
                    position.wQ = position.wQ & ~toSquareBitboard;
                    position.wQCount--;
                    position.whiteQueen.Remove(toSquare);
                    break;

                case Piece.White | Piece.Rook:
                    position.wR = position.wR & ~toSquareBitboard;
                    position.wRCount--;
                    position.whiteRook.Remove(toSquare);
                    break;

                case (Piece.White | Piece.Bishop):
                    position.wB = position.wB & ~toSquareBitboard;
                    position.wBCount--;
                    position.whiteBishop.Remove(toSquare);
                    break;

                case Piece.White | Piece.Knight:
                    position.wN = position.wN & ~toSquareBitboard;
                    position.wNCount--;
                    position.whiteKnight.Remove(toSquare);
                    break;

                case (Piece.White | Piece.Pawn):
                    position.wP = position.wP & ~toSquareBitboard;
                    position.wPCount--;
                    position.whitePawn.Remove(toSquare);
                    break;

                //Black Pieces
                case (Piece.Black | Piece.Queen):
                    position.bQ = position.bQ & ~toSquareBitboard;
                    position.bQCount--;
                    position.blackQueen.Remove(toSquare);
                    break;

                case Piece.Black | Piece.Rook:
                    position.bR = position.bR & ~toSquareBitboard;
                    position.bRCount--;
                    position.blackRook.Remove(toSquare);
                    break;

                case (Piece.Black | Piece.Bishop):
                    position.bB = position.bB & ~toSquareBitboard;
                    position.bBCount--;
                    position.blackBishop.Remove(toSquare);
                    break;

                case Piece.Black | Piece.Knight:
                    position.bN = position.bN & ~toSquareBitboard;
                    position.bNCount--;
                    position.blackKnight.Remove(toSquare);
                    break;

                case (Piece.Black | Piece.Pawn):
                    position.bP = position.bP & ~toSquareBitboard;
                    position.bPCount--;
                    position.blackPawn.Remove(toSquare);
                    break;
            }

            //Update for the piece taking the square
            switch (pieceMoving)
            {
                //White Pieces
                case Piece.White | Piece.King:
                    position.wK = position.wK | toSquareBitboard;
                    position.wK = position.wK & ~fromSquareBitboard;

                    // update mailbox
                    position.whiteKing.Remove(fromSquare); 
                    position.whiteKing.Add(toSquare);

                    //King moved, cant castle
                    position.whiteCastles = 0;
                    break;

                case (Piece.White | Piece.Queen):
                    position.wQ = position.wQ | toSquareBitboard;
                    position.wQ = position.wQ & ~fromSquareBitboard;

                    // update mailbox
                    position.whiteQueen.Remove(fromSquare);
                    position.whiteQueen.Add(toSquare);
                    break;

                case Piece.White | Piece.Rook:
                    position.wR = position.wR | toSquareBitboard;
                    position.wR = position.wR & ~fromSquareBitboard;

                    // update mailbox
                    position.whiteRook.Remove(fromSquare);
                    position.whiteRook.Add(toSquare);

                    // Can't castle with that rook

                    if ((Moves.qsRookCorners & fromSquareBitboard) != 0)// Rook moved on queen side
                    {
                        position.whiteCastles = position.whiteCastles & 0b01; // Can't castle queen side
                    }
                    else if ((Moves.ksRookCorners & fromSquareBitboard) != 0) // Rook moved on king side
                    {
                        position.whiteCastles = position.whiteCastles & 0b10; // Can't castle king side
                    }
                    break;

                case (Piece.White | Piece.Bishop):
                    position.wB = position.wB | toSquareBitboard;
                    position.wB = position.wB & ~fromSquareBitboard;

                    // update mailbox
                    position.whiteBishop.Remove(fromSquare);
                    position.whiteBishop.Add(toSquare);
                    break;

                case Piece.White | Piece.Knight:
                    position.wN = position.wN | toSquareBitboard;
                    position.wN = position.wN & ~fromSquareBitboard;

                    // update mailbox
                    position.whiteKnight.Remove(fromSquare);
                    position.whiteKnight.Add(toSquare);
                    break;

                case (Piece.White | Piece.Pawn):
                    position.wP = position.wP | toSquareBitboard;
                    position.wP = position.wP & ~fromSquareBitboard;

                    // update mailbox
                    position.whitePawn.Remove(fromSquare);
                    position.whitePawn.Add(toSquare);
                    break;

                //Black Pieces
                case Piece.Black | Piece.King:
                    position.bK = position.bK | toSquareBitboard;
                    position.bK = position.bK & ~fromSquareBitboard;

                    // update mailbox
                    position.blackKing.Remove(fromSquare);
                    position.blackKing.Add(toSquare);

                    //King moved, can't castle
                    position.blackCastles = 0;

                    break;

                case (Piece.Black | Piece.Queen):
                    position.bQ = position.bQ | toSquareBitboard;
                    position.bQ = position.bQ & ~fromSquareBitboard;

                    // update mailbox
                    position.blackQueen.Remove(fromSquare);
                    position.blackQueen.Add(toSquare);
                    break;

                case Piece.Black | Piece.Rook:

                    position.bR = position.bR | toSquareBitboard;
                    position.bR = position.bR & ~fromSquareBitboard;

                    // update mailbox
                    position.blackRook.Remove(fromSquare);
                    position.blackRook.Add(toSquare);

                    // Can't castle with that rook

                    if ((Moves.qsRookCorners & fromSquareBitboard) != 0)// Rook moved on queen side
                    {
                        position.blackCastles = position.blackCastles & 0b01; // Can't castle queen side
                    }
                    else if ((Moves.ksRookCorners & fromSquareBitboard) != 0) // Rook moved on king side
                    {
                        position.blackCastles = position.blackCastles & 0b10; // Can't castle king side
                    }
                    break;

                case (Piece.Black | Piece.Bishop):
                    position.bB = position.bB | toSquareBitboard;
                    position.bB = position.bB & ~fromSquareBitboard;

                    // update mailbox
                    position.blackBishop.Remove(fromSquare);
                    position.blackBishop.Add(toSquare);
                    break;

                case Piece.Black | Piece.Knight:
                    position.bN = position.bN | toSquareBitboard;
                    position.bN = position.bN & ~fromSquareBitboard;

                    // update mailbox
                    position.blackKnight.Remove(fromSquare);
                    position.blackKnight.Add(toSquare);
                    break;

                case (Piece.Black | Piece.Pawn):
                    position.bP = position.bP | toSquareBitboard;
                    position.bP = position.bP & ~fromSquareBitboard;

                    // update mailbox
                    position.blackPawn.Remove(fromSquare);
                    position.blackPawn.Add(toSquare);
                    break;
            }
        }

        private void LoadFEN(string FEN) //Makes FEN readable for computer
        {
            //Default all bitboards
            ulong wK = 0L, wQ = 0L, wR = 0L, wN = 0L, wB = 0L, wP = 0L, bK = 0L, bQ = 0L, bR = 0L, bN = 0L, bB = 0L, bP = 0L;


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
                        switch (c)
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
            BitboardOutput(true, wK, wQ, wR, wB, wN, wP, bK, bQ, bR, bB, bN, bP);

        }

        //Make bitboard into the game board
        void BitboardOutput(bool asWhite, ulong wK, ulong wQ, ulong wR, ulong wB, ulong wN, ulong wP, ulong bK, 
            ulong bQ, ulong bR, ulong bB, ulong bN, ulong bP)
        {
            int sideIndex = 64;

            //To assign to the position class
            int wKCount = 0, wQCount = 0, wRCount = 0, wNCount = 0, wBCount = 0, wPCount = 0,
                bKCount = 0, bQCount = 0, bRCount = 0, bNCount = 0, bBCount = 0, bPCount = 0;

            // Mailbox approach for faster evaluation and generation
            // Each will include a list of where they are (0 - 63)

            // White pieces
            List<int> whiteKing = new List<int>();
            List<int> whiteQueen = new List<int>();
            List<int> whiteRook = new List<int>();
            List<int> whiteBishop = new List<int>();
            List<int> whiteKnight = new List<int>();
            List<int> whitePawn = new List<int>();

            // Black pieces
            List<int> blackKing = new List<int>();
            List<int> blackQueen = new List<int>();
            List<int> blackRook = new List<int>();
            List<int> blackBishop = new List<int>();
            List<int> blackKnight = new List<int>();
            List<int> blackPawn = new List<int>();


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
                if (((wK >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.White | Piece.King; // Place piece on the square
                    wKCount++; // Add piece to the counter
                    whiteKing.Add(63 - i); // Add to list
                }
                else if (((wQ >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.White | Piece.Queen;
                    wQCount++;
                    whiteQueen.Add(63 - i); // Add to list
                }
                else if (((wR >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.White | Piece.Rook;
                    wRCount++;
                    whiteRook.Add(63 - i); // Add to list
                }
                else if (((wB >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.White | Piece.Bishop;
                    wBCount++;
                    whiteBishop.Add(63 - i); // Add to list
                }
                else if (((wN >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.White | Piece.Knight;
                    wNCount++;
                    whiteKnight.Add(63 - i); // Add to list
                }
                else if (((wP >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.White | Piece.Pawn;
                    wPCount++;
                    whitePawn.Add(63 - i); // Add to list
                }

                else if (((bK >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.Black | Piece.King;
                    bKCount++;
                    blackKing.Add(63 - i); // Add to list
                }
                else if (((bQ >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.Black | Piece.Queen;
                    bQCount++;
                    blackQueen.Add(63 - i); // Add to list
                }
                else if (((bR >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.Black | Piece.Rook;
                    bRCount++;
                    blackRook.Add(63 - i); // Add to list
                }
                else if (((bB >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.Black | Piece.Bishop;
                    bBCount++;
                    blackBishop.Add(63 - i); // Add to list
                }
                else if (((bN >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.Black | Piece.Knight;
                    bNCount++;
                    blackKnight.Add(63 - i); // Add to list
                }
                else if (((bP >> i) & 1L) == 1L)
                {
                    squares[sideIndex].piece = Piece.Black | Piece.Pawn;
                    bPCount++;
                    blackPawn.Add(63 - i); // Add to list
                }

                else { squares[sideIndex].piece = Piece.None; }

                squares[sideIndex].AssignPiece();

            }

            //Assumes default position with castling rights, no en passant and white to play
            position = new Position(wK, wQ, wR, wB, wN, wP, bK, bQ, bR, bB, bN, bP, 0b11, 0b11, 0L, 0L, true);

            // Assign the counts to the position
            position.wKCount = wKCount;
            position.wQCount = wQCount;
            position.wRCount = wRCount;
            position.wBCount = wBCount;
            position.wNCount = wNCount;
            position.wPCount = wPCount;
            position.bKCount = bKCount;
            position.bQCount = bQCount;
            position.bRCount = bRCount;
            position.bBCount = bBCount;
            position.bNCount = bNCount;
            position.bPCount = bPCount;

            // Assign the lists to the position
            // White pieces
            position.whiteKing = whiteKing;
            position.whiteQueen = whiteQueen;
            position.whiteBishop = whiteBishop;
            position.whiteRook = whiteRook;
            position.whiteKnight = whiteKnight;
            position.whitePawn = whitePawn;

            // Black pieces
            position.blackKing = blackKing;
            position.blackQueen = blackQueen;
            position.blackRook = blackRook;
            position.blackBishop = blackBishop;
            position.blackKnight = blackKnight;
            position.blackPawn = blackPawn;


            //Generate legal moves once a new position is established
            moves = Moves.GenerateGameMoves(position);

            // Format the move history
            moveHistory = new Stack<MoveInfo>();
        }

        public static ulong BinaryStringToBitboard(int index)
        {
            string binary = "0000000000000000000000000000000000000000000000000000000000000000"; //64 bit

            //insert 1 in index
            binary = binary.Remove(index, 1).Insert(index, "1");

            //return as type long
            return Convert.ToUInt64(binary, 2);
        }
    }
}