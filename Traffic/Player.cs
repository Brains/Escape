using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics;
using Tools.Markers;
using Tools;

namespace Traffic
{
    class Player : Car
    {
        //------------------------------------------------------------------
        public Player (Lane lane, int insertPoint) : base (lane, insertPoint)
        {
            InitialColor = Color.SkyBlue;
            TextureName = "Player";

            MaximumSpeed = 500;
            MinimumSpeed = 50;
            Acceleration = 0.02f;
        }

        #region Update

        //------------------------------------------------------------------
        public override void Update ()
        {
            UpdateInput ();

//            UpdateSensor ();
            

            base.Update ();
            
            new Text (Lane.ToString (), Position, Color.Red, true);
        }

        //------------------------------------------------------------------
        public void UpdateInput ()
        {
            if (KeyboardInput.IsKeyPressed (Keys.Right)) ChangeLane (Lane.Right);
            if (KeyboardInput.IsKeyPressed (Keys.Left)) ChangeLane (Lane.Left);
            if (KeyboardInput.IsKeyDown (Keys.Down)) Brake ();
            if (KeyboardInput.IsKeyDown (Keys.Up)) Accelerate (); 
        }

        #endregion



        //------------------------------------------------------------------

        //------------------------------------------------------------------

        //------------------------------------------------------------------
    }
}