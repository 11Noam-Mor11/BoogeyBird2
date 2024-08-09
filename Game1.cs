using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace BoogeyMan
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _birdTexture;
        private Vector2 _birdPosition;
        private float _birdVelocity;
        private const float Gravity = 0.5f;
        private const float FlapStrength = -10f;

        private List<Pipe> _pipes;
        private Texture2D _pipeTexture;
        private Random _random;
        private float _pipeSpawnTimer;
        private const float PipeSpawnInterval = 1.5f; // in seconds
        private const int PipeGap = 150;

        private int pipeSpeed = 5;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _birdPosition = new Vector2(100, 100);
            _pipes = new List<Pipe>();
            _random = new Random();

            int w = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int h = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.PreferredBackBufferWidth = w;
            _graphics.PreferredBackBufferHeight = h;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            base.Initialize(); 
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _birdTexture = Content.Load<Texture2D>("bird");
            _pipeTexture = Content.Load<Texture2D>("pipe");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _birdVelocity = FlapStrength;
            }

            _birdVelocity += Gravity;
            _birdPosition.Y += _birdVelocity;

            _pipeSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_pipeSpawnTimer >= PipeSpawnInterval)
            {
                _pipeSpawnTimer = 0f;
                int pipeHeight = _random.Next(50, _graphics.PreferredBackBufferHeight - PipeGap - 50);
                _pipes.Add(new Pipe(_pipeTexture, new Vector2(_graphics.PreferredBackBufferWidth, pipeHeight), true, pipeSpeed));
                _pipes.Add(new Pipe(_pipeTexture, new Vector2(_graphics.PreferredBackBufferWidth, pipeHeight + PipeGap), false, pipeSpeed));
            }

            foreach (var pipe in _pipes)
            { 
                pipe.Update();
            }

            _pipes.RemoveAll(p => p.position.X < -Pipe.PipeWidth);

            foreach (var pipe in _pipes)
            {
                if (_birdPosition.X + _birdTexture.Width > pipe.position.X && _birdPosition.X < pipe.position.X + Pipe.PipeWidth &&
                    _birdPosition.Y + _birdTexture.Height > pipe.position.Y && _birdPosition.Y < pipe.position.Y + Pipe.PipeHeight)
                {
                    Exit(); // Game over
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_birdTexture, _birdPosition, Color.White);
            foreach (var pipe in _pipes)
            {
                pipe.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
