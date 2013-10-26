using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    class Road
    {
        List <Lane> lanes;
        SpriteBatch spriteBatch;

        //------------------------------------------------------------------
        public Road (Game game)
        {
            CreateLanes (game);

            spriteBatch = new SpriteBatch (game.GraphicsDevice);
        }

        //------------------------------------------------------------------
        private void CreateLanes (Game game)
        {
            lanes = new List<Lane> ();

            float position = 20;
            float velocity = 300;

            foreach (var index in Enumerable.Range (0, 1))
            {
                var lane = new Lane (game);
                lane.Position = position;
                lane.Velocity = velocity;

                lanes.Add (lane);
                
                position += 40;
                velocity -= 30;

            }
        }

        //------------------------------------------------------------------
        public void Initialize ()
        {
            foreach (var lane in lanes)
            {
                lane.Initialize ();
            }
        }

        //------------------------------------------------------------------
        public void LoadContent ( )
        {
            foreach (var lane in lanes)
            {
                lane.LoadContent ();
            }
        }

        //------------------------------------------------------------------
        public void UnloadContent ( )
        {
            foreach (var lane in lanes)
            {
                lane.UnloadContent ();
            }
        }

        //------------------------------------------------------------------
        public void Update ( )
        {
            foreach (var lane in lanes)
            {
                lane.Update ();
            }
        }

        //------------------------------------------------------------------
        public void Draw ()
        {
            spriteBatch.Begin ();

            foreach (var lane in lanes)
            {
                lane.Draw (spriteBatch);
            }

            spriteBatch.End ();
        }

        
    }
}