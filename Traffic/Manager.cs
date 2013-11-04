using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    public class Manager : DrawableGameComponent
    {
        private readonly Road road;
        private SpriteBatch spriteBatch;

        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            road = new Road (Game);
        }

        //------------------------------------------------------------------
        public override void Initialize ()
        {
<<<<<<< HEAD
            spriteBatch = new SpriteBatch (Game.GraphicsDevice);
            
            road.Setup ();
=======
            road.Create ();

            base.Initialize ();
>>>>>>> parent of 62fdd8c... Object (Composite): Start
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gameTime)
        {
            float elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

            road.Update (elapsed);
        }

        //------------------------------------------------------------------
        public override void Draw (GameTime gameTime)
        {
            spriteBatch.Begin ();

            road.Draw (spriteBatch);

            spriteBatch.End ();

        }
    }
}
