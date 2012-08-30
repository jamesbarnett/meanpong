using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeanPong
{
    public class Ball
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Texture2D Texture;

        private int _diameter;
        private int[] _yRange;

        public int[] YRange { get { return _yRange; } }

        public Ball(Court court)
        {
            _diameter = court.Height / 48;
            _yRange = new int[]{ 0, court.Height - _diameter }; 
        }

        public void StartingPosition(Court court)
        {
            Position.X = court.Width / 2 - _diameter / 2;
            Position.Y = court.Height / 2 - _diameter / 2;
            Velocity.X = court.Width / 1024.0f * 7.0f;
            Velocity.Y = 0.0f;
        }

        public Rectangle GetRect()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, _diameter, _diameter);
        }

        public Vector2 Midpoint()
        {
            return new Vector2(Position.X + _diameter / 2, Position.Y + _diameter / 2);
        }
    }
}
