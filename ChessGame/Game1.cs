using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ChessGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        RenderTarget2D renderTarget;

        //Reference later for functions like exitting the game
        public static Game1 self; 

        //Create the font
        public static SpriteFont text; // Public to use at anytime

        //public float scale = 0.44444f;

        private Board board;

        private Dictionary<string, Color> colorDic = new Dictionary<string, Color>()
        {
            { "Light Square", new Color(239, 231, 232) },
            { "Dark Square", new Color(87, 63, 63) },
            { "Outside Board", new Color(99, 66, 53) },
            { "Inner Outside Board", new Color(194, 141, 103) },
            { "Background", new Color(74, 44, 42) },
            { "Target Colour", new Color(254, 172, 238) }, // currently not referenced. [new] in Square.cs
            { "Endgame Background", new Color(93, 119, 123, 215) },
            { "Endgame Line", new Color(65, 86, 86) }
        };

        private int squareSize;
        private Vector2 initPos;

        //texture location dir || Change as more textures come 
        private string dir = "Set 1/";

        //"Other" texture directory
        private string otherDir = "Other/";

        //mouse point
        public static Point mousePoint;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            self = this; //Reference to exit
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferWidth = 900;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            renderTarget = new RenderTarget2D(GraphicsDevice, 1920, 1080);

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

            //EndGameScreen Textures
            EndGameScreen.exitText = Content.Load<Texture2D>("EndGame Screen/exit");
            EndGameScreen.restartText = Content.Load<Texture2D>("EndGame Screen/restart");

            //update Piece dictionary. No idea how to update with the whole dictionary in Piece
            Piece.pieceTextureDic =
                new Dictionary<int, Texture2D>()
            {
                //White pieces
                { Piece.White | Piece.King, Piece.whiteKing }, //Could also use Piece.cs for reference; "Piece.White | Piece.King"
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

            //Set other parameters
            initPos = new Vector2(90, 90);
            squareSize = 90;

            board = new Board(squareSize, initPos, colorDic);

            board.LoadContent(_spriteBatch, GraphicsDevice);


            // TODO: use this.Content to load your game content here
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
            //scale = 1f / (1080f / _graphics.GraphicsDevice.Viewport.Height);

            //GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(colorDic["Background"]);


            // TODO: Add your drawing code here


            //Draw the board
            _spriteBatch.Begin();
            board.Draw(_spriteBatch);
            _spriteBatch.End();

            /* DEAL WITH LATER , CURRENTLY DOSE NOT SCALE RECTANGLES, LEARN MORE
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(colorDic["Background"]);

            
            //render resolution
            _spriteBatch.Begin();
            _spriteBatch.Draw(renderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            _spriteBatch.End();
            */


            base.Draw(gameTime);
        }
    }
}
