using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    public class Manager : DrawableGameComponent
    {
        private Texture2D car;
        private SpriteBatch spriteBatch;

        //------------------------------------------------------------------
        public Manager (Game game) : base(game)
        {
            game.Components.Add (this);
        }

        //------------------------------------------------------------------
        public override void Initialize ()
        {
            spriteBatch = new SpriteBatch (GraphicsDevice);


            base.Initialize ();
        }

        //------------------------------------------------------------------
        protected override void LoadContent ()
        {
            ContentManager content = (ContentManager) Game.Services.GetService (typeof (ContentManager));
            car = content.Load <Texture2D> ("Images/Cars/Car");
            
            base.LoadContent ();
        }

        //------------------------------------------------------------------
        public override void Draw (GameTime gameTime)
        {
            spriteBatch.Begin ();
            spriteBatch.Draw (car, new Vector2 (200, 200), Color.White);
            spriteBatch.End ();
            
            base.Draw (gameTime);
        }
    }
}
