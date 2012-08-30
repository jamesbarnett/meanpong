using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeanPong
{
    public class AIPaddle : IPaddle
    {
        private int _height;
        private int _width;
        private int[] _yRange;
        private float _yVelocity;
        private Random _rng = new Random();
        
        public int Height { get { return _height; } }
        public int Width { get { return _width; } }
        public Vector2 Position;
        public Vector2 Velocity;
        public float AbsYVelocity { get { return _yVelocity; } }
        public int[] YRange { get { return _yRange; } }

        public Texture2D Texture;

        public AIPaddle(Court court)
        {
            _height = court.Height / 8;
            _width = court.Width / 64;
            Position.X = court.Width / 64 * 3.0f * 20.0f;
            _yRange = new int [] {0, court.Height - _height};
            _yVelocity = court.Width / 1024.0f * 0.6f;
        }

        public void StartingPosition()
        {
            Position.Y = _yRange[1] / 2;
        }

        public Rectangle GetRect()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, _width, _height);
        }

        public void Think(Ball ball)
        {
            int n = _rng.Next(10);

            switch (n)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    RealThink(ball);
                    break;

                case 9:
                    Velocity.X = 0.0f;
                    Velocity.Y = 0.0f;
                    break;

                default:
                    // do nothing
                    break;
            }
        }

        private Vector2 Midpoint()
        {
            return new Vector2(Position.X + _width / 2, Position.Y + _height / 2);
        }

        private void RealThink(Ball ball)
        {
            if (ball.Midpoint().Y > Midpoint().Y)
            {
                if (Velocity.Y < _yVelocity)
                {
                    Velocity.Y += 0.05f;
                    if (Velocity.Y > _yVelocity)
                    {
                        Velocity.Y = _yVelocity;
                    }
                }
            }
            else if (ball.Midpoint().Y < Midpoint().Y)
            {
                if (Velocity.Y > -_yVelocity)
                {
                    Velocity.Y -= 0.05f;
                    if (Velocity.Y < -_yVelocity)
                    {
                        Velocity.Y = -_yVelocity;
                    }
                }
            }
            else if (ball.Midpoint().Y == Midpoint().Y)
            {
                Velocity.Y = 0.0f;
            }
        }
    }
}
