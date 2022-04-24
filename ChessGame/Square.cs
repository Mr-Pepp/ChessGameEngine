using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChessGame
{
    class Square
    {
        readonly private Texture2D _texture;
        private Texture2D pieceTexture;

        //miscs
        public static Texture2D dotTexture;

        public Rectangle rect;

        private Color _colour;
        readonly private int _pxSize;
        public Vector2 position;

        public int piece = Piece.None; // initial

        // miscs
        public bool dot = false;
        public bool targetSquare = false;
        

        public Square() {}

        public Square(Texture2D texture, Color colour, int pxSize)
        {

            _texture = texture;
            _colour = colour;
            _pxSize = pxSize;

        }

        //load content
        public void LoadContent()
        {
            //create new rectangle from information gathered. Only runs once?
            //this could be done within the board itself. however real time scaling will be a thing in the future
            rect = new Rectangle((int)position.X, (int)position.Y, _pxSize, _pxSize);
        }

        public void Draw(SpriteBatch s)
        {
            //Draw the square out
            s.Draw(_texture, rect, null, _colour);

            if (targetSquare)
            {
                s.Draw(_texture, rect, null, new Color(15, 45, 15, 150));
            }

            if (piece != 0b00000) //piece exists
            {
                //Draw the piece out
                s.Draw(pieceTexture, rect, null, Color.White);
            }

            if (dot)
            {
                s.Draw(dotTexture, rect, null, new Color(15, 45, 15, 90));
            }
            

        }

        public void AssignPiece()
        {
            if(!(piece == Piece.None))
            {
                pieceTexture = Piece.pieceTextureDic[piece];
            }

            //check and store the next legal moves that the piece can make
            //Issue is that if another piece is moved then more squares are available to the piece now. 

        }
    }
}
