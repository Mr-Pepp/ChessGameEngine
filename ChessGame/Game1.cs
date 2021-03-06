using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ChessGame
{
    public class Game1 : Game
    {
        readonly private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Reference later for functions like exitting the game
        public static Game1 self; 

        //Create the font
        public static SpriteFont text;

        private Board board;

        readonly private Dictionary<string, Color> colorDic = new Dictionary<string, Color>()
        {
            { "Light Square", new Color(239, 231, 232) },
            { "Dark Square", new Color(87, 63, 63) },
            { "Outside Board", new Color(99, 66, 53) },
            { "Inner Outside Board", new Color(194, 141, 103) },
            { "Background", new Color(74, 44, 42) },
            { "Target Colour", new Color(254, 172, 238) },
            { "Endgame Background", new Color(93, 119, 123, 215) },
            { "Endgame Line", new Color(65, 86, 86) },
            { "Promotion Background", new Color(255, 255, 255, 100) }
        };

        private int squareSize;
        private Vector2 initPos;

        //texture location dir || Change as more textures come 
        readonly private string dir = "Set 1/";

        //"Other" texture directory
        readonly private string otherDir = "Other/";

        //mouse point
        public static Point mousePoint;

        // Pass Game1 so that it can be called from other classes
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            self = this; //Reference to exit
        }

        protected override void Initialize()
        {
            // Initialization logic

            _graphics.PreferredBackBufferWidth = 990;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load font
            text = Content.Load<SpriteFont>("Fonts/Ubuntu32");

            //load & set textures
            Piece.whiteKing = Content.Load<Texture2D>(dir + "White_King");
            Piece.whitePawn = Content.Load<Texture2D>(dir + "White_Pawn");
            Piece.whiteKnight = Content.Load<Texture2D>(dir + "White_Knight");
            Piece.whiteBishop = Content.Load<Texture2D>(dir + "White_Bishop");
            Piece.whiteRook = Content.Load<Texture2D>(dir + "White_Rook");
            Piece.whiteQueen = Content.Load<Texture2D>(dir + "White_Queen");
            
            Piece.blackKing = Content.Load<Texture2D>(dir + "Black_King");
            Piece.blackPawn = Content.Load<Texture2D>(dir + "Black_Pawn");
            Piece.blackKnight = Content.Load<Texture2D>(dir + "Black_Knight");
            Piece.blackBishop = Content.Load<Texture2D>(dir + "Black_Bishop");
            Piece.blackRook = Content.Load<Texture2D>(dir + "Black_Rook");
            Piece.blackQueen = Content.Load<Texture2D>(dir + "Black_Queen");

            //Misc textures
            Square.dotTexture = Content.Load<Texture2D>(otherDir + "Dot");

            //Set EndGameScreen and SideOptions Textures
            EndGameScreen.exitText = Content.Load<Texture2D>("EndGame Screen/exit");
            EndGameScreen.restartText = Content.Load<Texture2D>("EndGame Screen/restart");
            
            SideOptions.backBtnTexture = Content.Load<Texture2D>("Other/undoBtn");
            SideOptions.restartBtnTexture = Content.Load<Texture2D>("Other/restartBtn");
            // Colour buttons
            SideOptions.whiteBtnTexture = Content.Load<Texture2D>("Other/whiteBtn");
            SideOptions.blackBtnTexture = Content.Load<Texture2D>("Other/blackBtn");



            //update Piece dictionary. No idea how to update with the whole dictionary in Piece
            Piece.pieceTextureDic =
                new Dictionary<int, Texture2D>()
            {
                //Using Piece.cs for reference; "Piece.White | Piece.King"
                //White pieces
                { Piece.White | Piece.King, Piece.whiteKing }, 
                { Piece.White | Piece.Pawn, Piece.whitePawn },
                { Piece.White | Piece.Knight, Piece.whiteKnight },
                { Piece.White | Piece.Bishop, Piece.whiteBishop },
                { Piece.White | Piece.Rook, Piece.whiteRook },
                { Piece.White | Piece.Queen, Piece.whiteQueen },

                //Black pieces
                { Piece.Black | Piece.King, Piece.blackKing },
                { Piece.Black | Piece.Pawn, Piece.blackPawn },
                { Piece.Black | Piece.Knight, Piece.blackKnight },
                { Piece.Black | Piece.Bishop, Piece.blackBishop },
                { Piece.Black | Piece.Rook, Piece.blackRook },
                { Piece.Black | Piece.Queen, Piece.blackQueen },
            };

            //Set other size parameters
            initPos = new Vector2(90, 90);
            squareSize = 90;

            board = new Board(squareSize, initPos, colorDic);

            board.LoadContent(_spriteBatch);
        }

        protected override void Update(GameTime gameTime)
        {
            //get mouse point, position
            mousePoint = new Point(Mouse.GetState().X, Mouse.GetState().Y);

            //if esc is pressed then exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //run board update
            board.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(colorDic["Background"]);

            //Draw the board
            _spriteBatch.Begin();
            board.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
