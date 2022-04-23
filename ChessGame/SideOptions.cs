using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChessGame
{
    class SideOptions
    {
        // for textures
        public static Texture2D whiteBtnTexture;
        public static Texture2D blackBtnTexture;

        public static Texture2D backBtnTexture;
        public static Texture2D restartBtnTexture;

        // Screen size
        private int _pxSize;
        public Vector2 _position;

        public Rectangle whiteSideRect;
        public Rectangle blackSideRect;

        public Rectangle backRect;
        public Rectangle restartRect;

        int xPosition;
        
        int yPosition_white;
        int yPosition_black;
        int yPosition_back;
        int yPosition_restart;


        public SideOptions() { }

        public SideOptions(int pxSize, Vector2 position)
        {

            _pxSize = pxSize;
            _position = position;

        }

        public void OnLoad() // Runs on load
        {
            //Set position values
            xPosition = (int)_position.X + (int)(_pxSize * 8.75);

            yPosition_white = (int)_position.Y + (int)(_pxSize);
            yPosition_black = (int)_position.Y + (int)(_pxSize * 2);
            yPosition_back = (int)_position.Y + (int)(_pxSize * 5);
            yPosition_restart = (int)_position.Y + (int)(_pxSize * 7);

            //Select White colour
            whiteSideRect = new Rectangle(xPosition, yPosition_white, _pxSize, _pxSize);
            //Select Black colour
            blackSideRect = new Rectangle(xPosition, yPosition_black, _pxSize, _pxSize);

            //Back (Undo move)
            backRect = new Rectangle(xPosition, yPosition_back, _pxSize, _pxSize);

            //Restart
            restartRect = new Rectangle(xPosition, yPosition_restart, _pxSize, _pxSize);
        }


        public void Draw(SpriteBatch s)
        {

            //Draw "Select white side"
            s.Draw(whiteBtnTexture, whiteSideRect, null, Color.White);

            //Draw "Select black side"
            s.Draw(blackBtnTexture, blackSideRect, null, Color.White);


            // Draw Undo move
            s.Draw(backBtnTexture, backRect, null, Color.White);

            // Draw Restart game
            s.Draw(restartBtnTexture, restartRect, null, Color.White);

        }

    }
}
