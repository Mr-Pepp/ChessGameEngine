using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessGame
{
    class PromotionScreen
    {
        private Texture2D _texture;
        private Color _backgroundColour;
        private int _pxSize;
        private Vector2 _squarePosition;
        private int promotionPosition;

        public int xBackgroundPosition;
        public int yBackgroundPosition;

        private Rectangle backgroundRect;

        public Rectangle bishopRect;
        public Rectangle rookRect;
        public Rectangle queenRect;
        public Rectangle knightRect;

        private int pieceColour;

        public PromotionScreen() { }

        public PromotionScreen(Texture2D texture, Color backgroundColour, int pxSize)
        {

            _texture = texture;
            _backgroundColour = backgroundColour;
            _pxSize = pxSize;
            
        }

        public void Update(Vector2 squarePosition)
        {
            _squarePosition = squarePosition;

            
            

            //If white turn
            if (Moves.whiteTurn)
            {
                // Locate above the board
                promotionPosition = (int)squarePosition.Y - _pxSize;

                //Set piece colour
                pieceColour = Piece.White;
            }
            else //If black turn
            {
                // Locate below the board
                promotionPosition = (int)squarePosition.Y + _pxSize;

                //Set piece colour
                pieceColour = Piece.Black;
            }

            xBackgroundPosition = (int)squarePosition.X - _pxSize * 2;
            yBackgroundPosition = promotionPosition;

            //Can promote to Bishop, Rook, Queen, Knight
            //From square position (top left)
            backgroundRect = new Rectangle(xBackgroundPosition, yBackgroundPosition, _pxSize * 4, _pxSize);

            //Set piece rectangles

            //New Bishop rectangle
            bishopRect = new Rectangle(xBackgroundPosition, yBackgroundPosition, _pxSize, _pxSize);
            //New Rook rectangle
            rookRect = new Rectangle(xBackgroundPosition + _pxSize, yBackgroundPosition, _pxSize, _pxSize);
            //New Queen rectangle
            queenRect = new Rectangle(xBackgroundPosition + _pxSize * 2, yBackgroundPosition, _pxSize, _pxSize);
            //New Knight rectangle
            knightRect = new Rectangle(xBackgroundPosition + +_pxSize * 3, yBackgroundPosition, _pxSize, _pxSize);

        }

        public void Draw(SpriteBatch s)
        {
            //Draw background box
            s.Draw(_texture, backgroundRect, null, _backgroundColour);

            //Draw pieces

            //Bishop
            s.Draw(Piece.pieceTextureDic[pieceColour | Piece.Bishop], bishopRect, null, Color.White);

            //Rook
            s.Draw(Piece.pieceTextureDic[pieceColour | Piece.Rook], rookRect, null, Color.White);

            //Queen
            s.Draw(Piece.pieceTextureDic[pieceColour | Piece.Queen], queenRect, null, Color.White);

            //Knight
            s.Draw(Piece.pieceTextureDic[pieceColour | Piece.Knight], knightRect, null, Color.White);

        }
    }
}
