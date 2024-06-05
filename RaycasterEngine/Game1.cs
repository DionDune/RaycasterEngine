using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.ComponentModel;

namespace RaycasterEngine
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Random random;
        public static Texture2D TextureWhite;

        Settings settings;
        Grid Grid;
        Screen Screen;
        Camera Camera;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1800;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            random = new Random();
            settings = new Settings();
            Grid = new Grid(settings.gridDimentions);
            Screen = new Screen(new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight), _spriteBatch);
            Camera = new Camera(settings, new Vector2(50, 50));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Procedurally Creating and Assigning a 1x1 white texture to Color_White
            TextureWhite = new Texture2D(GraphicsDevice, 1, 1);
            TextureWhite.SetData(new Color[1] { Color.White });
        }




        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Camera.WorldPosition += new Vector2(settings.cameraMovementSpeed * (float)Math.Cos((Camera.Direction + 45) * (Math.PI / 180)),
                                                    settings.cameraMovementSpeed * (float)Math.Sin((Camera.Direction + 45) * (Math.PI / 180)));
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Camera.WorldPosition -= new Vector2(settings.cameraMovementSpeed * (float)Math.Cos((Camera.Direction + 45) * (Math.PI / 180)),
                                                    settings.cameraMovementSpeed * (float)Math.Sin((Camera.Direction + 45) * (Math.PI / 180)));
            }

            if (Keyboard.GetState().IsKeyDown(Keys.E))
                Camera.Direction += settings.cameraRotationSpeed;
            else if (Keyboard.GetState().IsKeyDown(Keys.Q))
                Camera.Direction -= settings.cameraRotationSpeed;


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();


            Camera.CastRays(_spriteBatch, settings, Screen, Grid);


            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}