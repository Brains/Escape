using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Physics;
using Tools;
using Tools.Markers;
using Tools.Processes;
using Tools.Timers;

namespace Traffic
{
    internal class Player : Car
    {
        //------------------------------------------------------------------
        public Player (Lane lane, int insertPoint) : base (lane, insertPoint)
        {
            Driver = new Drivers.Player (this);
            InitialColor = Color.White;
            TextureName = "Player";
            Lives = 99;
        }

        #region Update

        //------------------------------------------------------------------
        public override void Update (float elapsed)
        {
            UpdateInput ();

            base.Update (elapsed);
        }

        //------------------------------------------------------------------
        public void UpdateInput ()
        {
            if (KeyboardInput.IsKeyPressed (Keys.Right)) ChangeLane (Lane.Right);
            if (KeyboardInput.IsKeyPressed (Keys.Left)) ChangeLane (Lane.Left);
            if (KeyboardInput.IsKeyDown (Keys.Down)) Brake ();
            if (KeyboardInput.IsKeyDown (Keys.Up)) ForceAccelerate ();
        }

        //------------------------------------------------------------------
        protected void ForceAccelerate ()
        {
            Velocity += Acceleration;
        }

        #endregion
    }
}