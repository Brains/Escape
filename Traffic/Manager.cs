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
        private Road road;

        //------------------------------------------------------------------
        public Manager (Game game) : base (game)
        {
            road = new Road (Game);
        }

        //------------------------------------------------------------------
        public override void Initialize ()
        {
            road.Initialize ();

            base.Initialize ();
        }

        //------------------------------------------------------------------
        protected override void LoadContent ( )
        {
            road.LoadContent ();

            base.LoadContent ();
        }

        //------------------------------------------------------------------
        protected override void UnloadContent ()
        {
            road.UnloadContent ();

            base.UnloadContent ();
        }

        //------------------------------------------------------------------
        public override void Update (GameTime gameTime)
        {
            road.Update ();

            base.Update (gameTime);
        }

        //------------------------------------------------------------------
        public override void Draw (GameTime gameTime)
        {
            road.Draw ();

            base.Draw (gameTime);
        }
    }
}