using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessGame
{
    class Square
    {
        private Texture2D _texture;
        private Texture2D pieceTexture;

        public Rectangle rect;


        private Color _colour;
        private int _pxSize;
        public Vector2 position;

        public int piece = Piece.None; // initial

        //list so i can append, but slower than an array.
        private List<int> legalMoves = new List<int>();

        

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

            if (piece != 0b00000) //piece exists
            {
                //Draw the piece out
                s.Draw(pieceTexture, rect, null, Color.White);
            }
        }

        public void AssignPiece()
        {
            //assign the correct texture
            pieceTexture = Piece.pieceTextureDic[piece];

            //check and store the next legal moves that the piece can make
            //Issue is that if another piece is moved then more squares are available to the piece now.

        }
    }
}
