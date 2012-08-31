using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MeanPong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MeanPong : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Court _court = new Court();
        private PlayerPaddle _player = null;
        private AIPaddle _ai = null;
        private Ball _ball = null;
        private enum GameState { Splash, Help, NotReady, Playing, PlayerPoint, AIPoint, PlayerWon, PlayerLost, Credits, Closing };
        private GameState _gameState = GameState.Splash;
        private Texture2D _splash;
        private int _playerScore = 7;
        private int _aiScore = 7;
        private Color _meanGreen;
        private SpriteFont _font;
        private SpriteFont _gameEndHeadingFont;
        private SpriteFont _creditsFont;
        private SoundEffect _paddleCollidePlayer;
        private SoundEffect _paddleCollideAi;
        private SoundEffect _wallCollide;
        private SoundEffect _aiScoreSound;
        private SoundEffect _playerScoreSound;
        private float _creditsBaseY;

        private List<string> _creditLines = new List<string>
        {
            "Programming by jamesb43.",
            "Also blame jamesb43 for bad voice acting.",
            "Thanks to DrPetter for sfxr (http://www.drpetter.se/project_sfxr.html)",
            "I used sfxr to make the ball/paddle/wall collision sounds",
            "Thanks to Yannick Lemieux via SoundBible.com",
            "I used Yannicks Lemieux's small crowd applause for a player point.",
            "Thanks to freesounds.org.",
            "Thanks to the /r/gamedev community for being awesome. :)",
            "And a special thanks to the /r/gamedev redditor, whom I couldn't track down",
            "despite considerable effort.",
            "(S)he commented on a typical \"How do I get started?\" post in /r/gamedev,",
            "with \"build a pong clone!\", which is why I did this.",
            "Best advice ever!",
            "And most of all, thanks to my dad...",
            "who wrote assembly for the PDP-11...",
            "and yet still looks up movie listings in the local newspaper! :)"
        };

        private float _drawCreditsY;
        private float _creditsYSpacing;
        private List<Vector2> _creditLineSizes;

        // I picked an arbitrary date in past at random since any will do. :)
        private DateTime _aiScoreSoundStart = new DateTime(2000, 1, 1, 0, 0, 0);
        private DateTime _playerScoreSoundStart = new DateTime(2000, 1, 1, 0, 0, 0);
        private DateTime _prevAIScoreSoundStart = new DateTime(2000, 1, 1, 0, 0, 0);
        private DateTime _prevPlayerScoreSoundStart = new DateTime(2000, 1, 1, 0, 0, 0);
        private DateTime? _gameEndedAt;
        
        private TimeSpan _gameEndDuration = new TimeSpan(0, 0, 2);

        public MeanPong()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferHeight = _court.Height;
            _graphics.PreferredBackBufferWidth = _court.Width;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _player = new PlayerPaddle(_court);
            _player.Texture = Content.Load<Texture2D>("paddle");
            _player.StartingPosition();

            _ai = new AIPaddle(_court);
            _ai.Texture = Content.Load<Texture2D>("paddle");
            _ai.StartingPosition();

            _ball = new Ball(_court);
            _ball.Texture = Content.Load<Texture2D>("ball");
            _ball.StartingPosition(_court);

            _splash = Content.Load<Texture2D>("splash");
            _font = Content.Load<SpriteFont>("ScoreFont");
            _gameEndHeadingFont = Content.Load<SpriteFont>("GameEndHeading");

            _paddleCollidePlayer = Content.Load<SoundEffect>("paddle_collide_player");
            _paddleCollideAi = Content.Load<SoundEffect>("paddle_collide_ai");
            _wallCollide = Content.Load<SoundEffect>("wall_collide");
            _aiScoreSound = Content.Load<SoundEffect>("disappointed_crowd_idiot");
            _playerScoreSound = Content.Load<SoundEffect>("pleased_crowd_about_time");

            _meanGreen = new Color(45, 198, 13);
            _creditsFont = Content.Load<SpriteFont>("CreditsFont");
            _creditsYSpacing = _creditsFont.MeasureString("X").Y * 0.2f ;
            _creditsBaseY = _court.Height / 2.0f - _creditsYSpacing / 2.0f;
            _creditLineSizes = _creditLines.ConvertAll(x => _creditsFont.MeasureString(x));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                _gameState = GameState.Closing;
                this.Exit();
            }

            KeyboardState kbs = Keyboard.GetState();

            switch (_gameState)
            {
                case GameState.Splash:
                    if (kbs.IsKeyDown(Keys.Enter))
                    {
                        _gameState = GameState.Playing; // temporary
                    }
                    break;

                case GameState.Playing:
                    UpdatePlaying(kbs, gameTime);
                    break;

                case GameState.PlayerPoint:
                    UpdatePlayerPoint(kbs, gameTime);
                    break;

                case GameState.AIPoint:
                    UpdateAIPoint(kbs, gameTime);
                    break;

                case GameState.PlayerWon:
                    UpdatePlayerWon();
                    break;

                case GameState.PlayerLost:
                    UpdatePlayerLost();
                    break;

                case GameState.Credits:
                    UpdateCredits(kbs, gameTime);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            switch (_gameState)
            {
                case GameState.Splash:
                    _spriteBatch.Draw(_splash, new Rectangle(0, 0, 1024, 768), Color.White);
                    break;

                case GameState.Playing:
                    _spriteBatch.Draw(_player.Texture, _player.GetRect(), Color.White);
                    _spriteBatch.Draw(_ai.Texture, _ai.GetRect(), Color.White);
                    _spriteBatch.Draw(_ball.Texture, _ball.GetRect(), Color.White);
                    DrawScore();
                    break;

                case GameState.AIPoint:
                case GameState.PlayerPoint:
                    _spriteBatch.Draw(_player.Texture, _player.GetRect(), Color.White);
                    _spriteBatch.Draw(_ai.Texture, _ai.GetRect(), Color.White);
                    _spriteBatch.Draw(_ball.Texture, _ball.GetRect(), Color.White);
                    DrawScore();
                    break;

                case GameState.PlayerWon:
                    DrawPlayerWon();
                    break;

                case GameState.PlayerLost:
                    DrawPlayerLost();
                    break;

                case GameState.Credits:
                    DrawCredits();
                    break;

            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdatePlaying(KeyboardState kbs, GameTime gameTime)
        {
            UpdatePlayerInput(kbs, gameTime);
            UpdateBall();
            UpdateAI(gameTime);

            Rectangle playerRect = _player.GetRect();
            Rectangle aiRect = _ai.GetRect();
            Rectangle ballRect = _ball.GetRect();

            if (ballRect.Intersects(playerRect) && _ball.Velocity.X < 0.0f)
            {
                _ball.Velocity.X = -_ball.Velocity.X;
                if (_player.Velocity.Y == 0.0f)
                {
                    _ball.Velocity.Y = -_ball.Velocity.Y;
                }
                else
                {
                    _ball.Velocity.Y += _player.Velocity.Y * 0.5f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }

                _paddleCollidePlayer.Play();
            }

            if (ballRect.Intersects(aiRect) && _ball.Velocity.X > 0.0f)
            {
                _ball.Velocity.X = -_ball.Velocity.X;
                if (_ai.Velocity.Y == 0.0f)
                {
                    _ball.Velocity.Y = -_ball.Velocity.Y;
                }
                else
                {
                    _ball.Velocity.Y += _ai.Velocity.Y * 0.5f * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }

                _paddleCollideAi.Play();
            }

            if (_ball.Position.Y < _ball.YRange[0] || _ball.Position.Y > _ball.YRange[1])
            {
                if (_ball.Position.Y < _ball.YRange[0]) _ball.Position.Y = _ball.YRange[0];
                if (_ball.Position.Y > _ball.YRange[1]) _ball.Position.Y = _ball.YRange[1];

                _ball.Velocity.Y = -_ball.Velocity.Y;

                _wallCollide.Play();
            }

            if (_ball.Position.X > _court.Width)
            {
                _gameState = GameState.PlayerPoint;
            }

            if (_ball.Position.X < 0.0f)
            {
                _gameState = GameState.AIPoint;
            }
        }

        private void UpdateAI(GameTime gameTime)
        {
            _ai.Think(_ball);
            _ai.Position.Y += _ai.Velocity.Y * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_ai.Position.Y < _ai.YRange[0])
            {
                _ai.Position.Y = _ai.YRange[0];
            }

            if (_ai.Position.Y > _ai.YRange[1])
            {
                _ai.Position.Y = _ai.YRange[1];
            }
        }

        private void UpdatePlayerInput(KeyboardState kbs, GameTime gameTime)
        {
            if (kbs.IsKeyDown(Keys.A))
            {
                _player.Position.Y -= _player.AbsYVelocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                _player.Velocity.Y = -_player.AbsYVelocity;

                if (_player.Position.Y < _player.YRange[0])
                {
                    _player.Position.Y = _player.YRange[0];
                }
            }
            else if (kbs.IsKeyDown(Keys.D))
            {
                _player.Position.Y += _player.AbsYVelocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                _player.Velocity.Y = _player.AbsYVelocity;

                if (_player.Position.Y > _player.YRange[1])
                {
                    _player.Position.Y = _player.YRange[1];
                }
            }
            else
            {
                _player.Velocity.Y = 0.0f;
            }
        }

        private void UpdatePlayerPoint(KeyboardState kbs, GameTime gameTime)
        {
            UpdatePlayerInput(kbs, gameTime);
            UpdateAI(gameTime);
            UpdateBall();

            DateTime now = DateTime.Now;

            if (_playerScoreSoundStart == _prevPlayerScoreSoundStart)
            {
                _playerScore++;
                _playerScoreSound.Play();
                _playerScoreSoundStart = now;
            }
            else if (now < _playerScoreSoundStart + _playerScoreSound.Duration)
            {
                // Do nothing
            }
            else
            {
                _prevPlayerScoreSoundStart = _playerScoreSoundStart;

                if (_playerScore < 8)
                {
                    NewPoint();
                }
                else
                {
                    _gameState = GameState.PlayerWon;
                }
            }
        }

        private void UpdateAIPoint(KeyboardState kbs, GameTime gameTime)
        {
            UpdatePlayerInput(kbs, gameTime);
            UpdateAI(gameTime);
            UpdateBall();

            DateTime now = DateTime.Now;

            if (_aiScoreSoundStart == _prevAIScoreSoundStart)
            {
                _aiScore++;
                _aiScoreSound.Play();
                _aiScoreSoundStart = now;
            }
            else if (now < _aiScoreSoundStart + _aiScoreSound.Duration)
            {
                // Do nothing
            }
            else
            {
                _prevAIScoreSoundStart = _aiScoreSoundStart;

                if (_aiScore < 8)
                {
                    NewPoint();
                }
                else
                {
                    _gameState = GameState.PlayerLost;
                }
            }
        }

        private void UpdatePlayerWon()
        {
            GameOver();
        }

        private void UpdatePlayerLost()
        {
            GameOver();
        }

        private void GameOver()
        {
            if (!_gameEndedAt.HasValue)
            {
                Debug.WriteLine("GameOver: gameEndedAt has no value");
                _gameEndedAt = DateTime.Now;
            }
            else if (_gameEndedAt.Value + _gameEndDuration > DateTime.Now)
            {
                Debug.WriteLine("GameOver: gameEndedAt + gameEndDuration > DateTime.Now");
                ; // Do nothing
            }
            else
            {
                Debug.WriteLine("GameOver: going to credits state");
                _gameState = GameState.Credits;
            }
        }

        private void UpdateCredits(KeyboardState kbs, GameTime gameTime)
        {
            _creditsBaseY -= 1.2f;
        }

        private void DrawPlayerWon()
        {
            string msg = "I'M SOOO IMPRESSED";
            DrawGameEndMessage(msg);
        }

        private void DrawPlayerLost()
        {
            string msg = "YOU SUCK!";
            DrawGameEndMessage(msg);
        }

        private void DrawGameEndMessage(string msg)
        {
            Vector2 msgDimensions = _gameEndHeadingFont.MeasureString(msg);
            _spriteBatch.DrawString(_gameEndHeadingFont, msg, new Vector2(_court.Width / 2 - msgDimensions.X / 2, _court.Height / 2 - msgDimensions.Y / 2), _meanGreen);
        }

        private void DrawCredits()
        {
            int i = 0;

            foreach (var line in _creditLines)
            {
                _spriteBatch.DrawString(_creditsFont, line, new Vector2(_court.Width / 2 - _creditLineSizes[i].X / 2, 
                    _creditsBaseY + ((_creditLineSizes[i].Y + i) + i * _creditsYSpacing * 2.0f)), Color.White);
                ++i;
            }
        }

        private void UpdateBall()
        {
            _ball.Position.X += _ball.Velocity.X;
            _ball.Position.Y += _ball.Velocity.Y;
        }

        private void NewPoint()
        {
            _gameState = GameState.Playing;
            _ball.StartingPosition(_court);
            _player.StartingPosition();
            _ai.StartingPosition();
        }

        private void DrawScore()
        {
            const float offset = 20.0f;
            _spriteBatch.DrawString(_font, _playerScore.ToString(), new Vector2(offset, offset), Color.White);
            _spriteBatch.DrawString(_font, _aiScore.ToString(), 
                new Vector2(_court.Width - _font.MeasureString(_aiScore.ToString()).X - offset, offset), Color.White);
        }
    }  
}
