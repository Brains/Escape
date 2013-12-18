using System;
using System.Reflection;
using Fluid;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools.Markers;
using Traffic.Actions;
using Traffic.Cars;

namespace Traffic
{
    public class Manager : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private readonly Director director;

        //------------------------------------------------------------------
        public Road Road { get; private set; }
        public Computation Fluid { get; private set; }

        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            Road = new Road (Game);
            director = new Director (this);
            Fluid = new Computation (Game);
            Road.Fluid = Fluid;
        }

        //------------------------------------------------------------------
        public override void Initialize ()
        {
            spriteBatch = new SpriteBatch (Game.GraphicsDevice);
            
            Road.Setup ();
            director.Setup ();
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gameTime)
        {
            float elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

            elapsed *= Settings.TimeScale;

            Road.Update (elapsed);
            director.Update (elapsed);

            Tools.Markers.Manager.Clear = !Settings.NoMarkersClear;
        }

        //------------------------------------------------------------------
        public override void Draw (GameTime gameTime)
        {
            Road.GenerateFluidObstacles (spriteBatch);
            Fluid.SetScene (Road.Obstacles);
            Fluid.Update();
            
//            Road.DrawRoad (spriteBatch);
            Fluid.Render();
            Road.Draw (spriteBatch);

        }
    }
}
