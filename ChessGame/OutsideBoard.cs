using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChessGame
{
    class OutsideBoard
    {
        readonly private Texture2D _texture;
        readonly private Color _backColour;
        readonly private Color _innerColour;
        readonly private int _pxSize;
        public Vector2 position;

        public OutsideBoard() { }

        public OutsideBoard(Texture2D texture, Color backColour, Color innerColour, int pxSize)
        {
            // Set outside board variables
            _texture = texture;
            _backColour = backColour;
            _innerColour = innerColour;
            _pxSize = pxSize;
        }

        public void Draw(SpriteBatch s)
        {
            // Draw the out side board
            s.Draw(_texture, new Rectangle((int)position.X - _pxSize / 2, (int)position.Y - _pxSize / 2, _pxSize * 9, _pxSize * 9), null, _backColour); //outside

            s.Draw(_texture, new Rectangle((int)(position.X - _pxSize * 0.1), (int)(position.Y - _pxSize * 0.1), (int)(_pxSize * 8.2), (int)(_pxSize * 8.2)), null, _innerColour); //inner

        }

    }
}
