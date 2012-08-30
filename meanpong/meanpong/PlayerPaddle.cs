using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeanPong
{
    public class PlayerPaddle : IPaddle
    {
        private int _height;
        private int _width;
        private int[] _yRange;
        private float _yVelocity;
        public int Height { get { return _height; } }
        public int Width { get { return _width; } }
        public Vector2 Position;
        public Vector2 Velocity;
        public float AbsYVelocity { get { return _yVelocity; } }
        public int[] YRange { get { return _yRange; } }

        public Texture2D Texture;

        public PlayerPaddle(Court court)
        {
            _height = court.Height / 8;
            _width = court.Width / 64;
            Position.X = court.Width / 64 * 3.0f;
            _yRange = new int [] {0, court.Height - _height};
            _yVelocity = court.Width / 1024.0f * 0.4f;
        }

        public void StartingPosition()
        {
            Position.Y = _yRange[1] / 2;
        }

        public Rectangle GetRect()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, _width, _height);
        }
    }
}
