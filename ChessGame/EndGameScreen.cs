using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChessGame
{
    class EndGameScreen
    {
        readonly private Texture2D _texture;
        private Color _backgroundColour;
        private Color _lineColour;
        readonly private int _pxSize;
        public Vector2 position;

        private string stateMsg;
        private Vector2 stringSize;

        private Rectangle backgroundRect;
        private Rectangle lineRect;

        private int xPosition;
        private int xSize;
        private int ySize_background;
        private int size_icon;
        private int size_icon_restart;

        private int yPosition_background;
        private int yPosition_line;
        private int yPosition_Bottom;

        private Vector2 stringVector;

        public Rectangle restartRect;
        public Rectangle exitRect;

        public static Texture2D exitText;
        public static Texture2D restartText;

        //1 * (45f / _pxSize)
        private float scale;

        public EndGameScreen() { }

        public EndGameScreen(Texture2D texture, Color backgroundColour, Color lineColour, int pxSize)
        {

            _texture = texture;
            _backgroundColour = backgroundColour;
            _lineColour = lineColour;
            _pxSize = pxSize;

        }

        public void OnLoad() // Runs on load
        {
            //Set values
            //Scale
            scale = 1 * (45f / _pxSize);

            xPosition = (int)position.X + (int)(_pxSize * 1.5);
            yPosition_background = (int)position.Y + (int)(_pxSize * 2.5);
            yPosition_line = (int)position.Y + (int)(_pxSize * 3.95);

            yPosition_Bottom = (int)position.Y + (int)(_pxSize * 4.05);

            xSize = _pxSize * 5;
            ySize_background = _pxSize * 3;

            size_icon = ySize_background / 2;
            size_icon_restart = (int)(size_icon * 0.9f);

            //Background box rectangle
            backgroundRect = new Rectangle(xPosition, yPosition_background, xSize, ySize_background);
            //Middle Line rectangles
            lineRect = new Rectangle(xPosition, yPosition_line, xSize, (int)(_pxSize * 0.1));

            stringVector = new Vector2(xPosition, yPosition_background);

            //Exit: Bottom Left
            exitRect = new Rectangle(xPosition + (xSize / 4) - (size_icon / 2), yPosition_Bottom, size_icon, size_icon);

            //Restart: Bottom Right
            restartRect = new Rectangle(xPosition + (int)(xSize * 0.47) + (size_icon_restart / 2),
                yPosition_Bottom + (int)(size_icon * 0.05f), size_icon_restart, size_icon_restart);
        }

        public void Draw(SpriteBatch s)
        {
            //Draw background box
            s.Draw(_texture, backgroundRect, null, _backgroundColour);

            //Draw Line in the middle
            s.Draw(_texture, lineRect, null, _lineColour);

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

            //Draw the exit bottom left
            s.Draw(exitText, exitRect, null, Color.Black);

            //Draw the restart bottom right
            s.Draw(restartText, restartRect, null, Color.Black);
        }
    }
}
