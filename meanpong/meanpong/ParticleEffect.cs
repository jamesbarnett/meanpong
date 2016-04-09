using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MeanPong
{
    public struct ParticleData
    {
        public float BirthTime;
        public float MaxAge;
        public Vector2 OriginalPosition;
        public Vector2 Acceleration;
        public Vector2 Direction;
        public Vector2 Position;
        public Vector2 Velocity;
        public float A;
        public float B;
        public float C;
        public float Angle;
        public float Scaling;
        public Color ModColor;
        public bool PlayerCollision;

        public static ParticleData UpdateParticle(ParticleData particle, GameTime gameTime)
        {
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            float timeAlive = now - particle.BirthTime;
            float relAge = timeAlive / particle.MaxAge;
        
            // TODO: Need to figure out here how to manipulate velocity to get desired shape and speed, and later on, color
            return UpdateColor(UpdateVelocity(particle, relAge), relAge);
        }

        private static ParticleData UpdateVelocity(ParticleData particle, float relAge)
        {
            if (relAge < 0.25f)
            {
                if (particle.Velocity.Y < 0.0f)
                {
                    particle.Velocity.Y += -0.5f;
                }
                else
                {
                    particle.Velocity.Y += 0.5f;
                }

                if (particle.Velocity.X < 0.0f)
                {
                    particle.Velocity.X += -0.05f;
                }
                else
                {
                    particle.Velocity.X += 0.05f;
                }
            }
            else if (relAge >= 0.25f && relAge < 0.75f)
            {
                if (particle.Velocity.Y < 0.0f)
                {
                    particle.Velocity.Y += -0.25f;
                }
                else
                {
                    particle.Velocity.Y += 0.25f;
                }

                if (particle.Velocity.X < 0.0f)
                {
                    particle.Velocity.X += 0.25f;
                }
                else
                {
                    particle.Velocity.X += -0.25f;
                }
            }
            else if (relAge >= 0.75f)
            {
                if (particle.PlayerCollision)
                {
                    particle.Velocity.X += -0.5f;
                }
                else
                {
                    particle.Velocity.X += 0.5f;
                }
            }

            particle.Position += particle.Velocity;

            return particle;
        }

        private static ParticleData UpdateColor(ParticleData particle, float relAge)
        {
            if (relAge < 0.25f)
            {
                particle.ModColor = Color.White;
            }
            else if (relAge >= 0.25f && relAge < 0.75f)
            {
                particle.ModColor.G -= 6;
                particle.ModColor.B -= 9;
            }
            
            return particle;
        }
    }

    public class ParticleEffect
    {
        public Texture2D Texture;
        private Random _rng = new Random();
        public List<ParticleData> ActiveParticles = new List<ParticleData>();
        public Queue<ParticleData> ParticleQueue = new Queue<ParticleData>();

        public void AddExplosion(Vector2 explosionPos, int numberOfParticles, float maxAge, bool playerCollision, GameTime gameTime)
        {
            for (int i = 0; i < numberOfParticles; i++)
            {
                AddExplosionParticle(explosionPos, maxAge, playerCollision, gameTime);
            }

            var p = ParticleQueue.Dequeue();
            p.BirthTime = (float)gameTime.TotalGameTime.Milliseconds;
            ActiveParticles.Add(p);
        }

        private void AddExplosionParticle(Vector2 explosionPos, float maxAge, bool playerCollision, GameTime gameTime)
        {
            var particle = new ParticleData();

            particle.OriginalPosition = explosionPos;
            particle.Position = particle.OriginalPosition;
            //particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            particle.MaxAge = maxAge;
            particle.Scaling = 1.0f;
            particle.ModColor = Color.White;
            particle.Angle = 0.0f;

            // generate random angle in a range (or set of ranges) to be determined later
            float baseAngle = (float)_rng.NextDouble() * 20.0f + 32.5f;
            Vector2 baseVelocity = new Vector2(10.0f, 0.0f);
            bool upDirection = _rng.Next() % 2 == 0;

            if (upDirection && playerCollision)
            {
                particle.Velocity = Vector2.Transform(baseVelocity, Matrix.CreateRotationZ(MathHelper.ToRadians(baseAngle)));
            }
            else if (playerCollision)
            {
                particle.Velocity = Vector2.Transform(baseVelocity, Matrix.CreateRotationZ(MathHelper.ToRadians(-baseAngle)));
            }
            else if (!upDirection && !playerCollision)
            {
                particle.Velocity = Vector2.Transform(baseVelocity, Matrix.CreateRotationZ(MathHelper.ToRadians(180.0f - baseAngle)));
            }
            else
            {
                particle.Velocity = Vector2.Transform(baseVelocity, Matrix.CreateRotationZ(MathHelper.ToRadians(180.0f + baseAngle)));
            }

            particle.PlayerCollision = playerCollision;

            ParticleQueue.Enqueue(particle);
        }

        private Vector2 OriginalPosition(Vector2 epicenter, bool playerCollision)
        {
            Vector2 magnitude = playerCollision ? new Vector2(10.0f, 0.0f) : new Vector2(-10.0f, 0.0f);

            float angle = MathHelper.ToRadians((float)_rng.NextDouble() * 20.0f - 10.0f);

            return Vector2.Transform(Vector2.Transform(magnitude, Matrix.CreateRotationZ(angle)), 
                Matrix.CreateTranslation(epicenter.X, epicenter.Y, 0.0f));
        }
    }
}

        
    

