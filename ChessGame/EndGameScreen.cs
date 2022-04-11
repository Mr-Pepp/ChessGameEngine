using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChessGame
{
    class EndGameScreen
    {
        private Texture2D _texture;
        private Color _background;
        private Color _line;
        private int _pxSize;
        public Vector2 position;

        private string stateMsg;
        private Vector2 stringSize;

        private Rectangle backgroundRect;
        private Rectangle lineRect;

        private int xPosition;
        private int xSize;
        private int ySize_background;

        private int yPosition_background;
        private int yPosition_line;

        private Vector2 stringVector;

        //1 * (45f / _pxSize)
        private float scale;

        public EndGameScreen() { }

        public EndGameScreen(Texture2D texture, Color background, Color line, int pxSize)
        {

            _texture = texture;
            _background = background;
            _line = line;
            _pxSize = pxSize;

            //Set initial values
            //OnLoad();
        }

        public void OnLoad() // Runs on load
        {
            //Set values
            //Scale
            scale = 1 * (45f / _pxSize);

            xPosition = (int)position.X + (int)(_pxSize * 1.5);
            yPosition_background = (int)position.Y + (int)(_pxSize * 2.5);
            yPosition_line = (int)position.Y + (int)(_pxSize * 3.95);

            xSize = _pxSize * 5;
            ySize_background = _pxSize * 3;

            //Background box rectangle
            backgroundRect = new Rectangle(xPosition, yPosition_background, xSize, ySize_background);
            //Middle Line rectangles
            lineRect = new Rectangle(xPosition, yPosition_line, xSize, (int)(_pxSize * 0.1));

            stringVector = new Vector2(xPosition, yPosition_background);
        }

        public void Draw(SpriteBatch s)
        {
            if (GameState.state == 1 | GameState.state == 2) // If there is a check then show this
            {

                //Draw background box
                s.Draw(_texture, backgroundRect, null, _background);
                
                //Draw Line in the middle
                s.Draw(_texture, lineRect, null, _line);

                if (GameState.state == 1) // Checkmate
                {
                    stateMsg = "CHECKMATE";
                }
                else // Stalemate
                {
                    stateMsg = "STALEMATE";
                }

                stringSize = Game1.text.MeasureString(stateMsg) * scale;


                //Draw the text and allign it on screen
                s.DrawString(Game1.text, stateMsg, new Vector2(stringVector.X + (xSize - stringSize.X) / 2, 
                    stringVector.Y + (ySize_background - stringSize.Y) / 4),
                    Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }
    }
}
