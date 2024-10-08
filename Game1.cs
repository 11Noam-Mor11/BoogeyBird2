﻿using Microsoft.Xna.Framework;
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
        private Texture2D _rectangleTexture; // Added variable to hold the rectangle texture
        private Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(20, 20, 160 * 5, 100 * 5);

        private bool pipeInvincible = false;
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
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _birdTexture = Content.Load<Texture2D>("bird");
            _pipeTexture = Content.Load<Texture2D>("pipe");

            // Create a 1x1 texture for the rectangle
            _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _rectangleTexture.SetData(new[] { Microsoft.Xna.Framework.Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) pipeInvincible = true;
            //else pipeInvincible = false;

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

            _pipes.RemoveAll(p => p.position.X < -Pipe.PipeWidth); //remove pipes at end of screen

            if (CheckLoseCondition(_pipes, _birdTexture, _birdPosition, pipeInvincible)) Exit(); // Game over

            base.Update(gameTime);
        }



        public static bool CheckLoseCondition(List<Pipe> pipes, Texture2D birdTexture, Vector2 birdPosition, bool PI)
        {
            return CheckPipeCol(pipes, birdTexture, birdPosition, PI);
        }

        public static bool CheckPipeCol(List<Pipe> pipes, Texture2D birdTexture, Vector2 birdPosition, bool PI)
        {
            if (PI) return false; //pipe invincibility
            foreach (var pipe in pipes)
            {
                if (birdPosition.X + birdTexture.Width > pipe.position.X && birdPosition.X < pipe.position.X + Pipe.PipeWidth &&
                    birdPosition.Y + birdTexture.Height > pipe.position.Y && birdPosition.Y < pipe.position.Y + Pipe.PipeHeight)
                {
                    return true; //Pipe has collided with player
                }
            }
            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.Draw(_birdTexture, _birdPosition, Microsoft.Xna.Framework.Color.White);
            foreach (var pipe in _pipes)
            {
                pipe.Draw(_spriteBatch);
            }

            // Draw the rectangle
            _spriteBatch.Draw(_rectangleTexture, rectangle, Microsoft.Xna.Framework.Color.Black);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
