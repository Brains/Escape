using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    public class Object
    {
        public List <Object> Components { get; private set; }
        public Vector2 Position { get; set; }
        public Object Root { get; set; }
        public bool Deleted { get; set; }
        public bool Anchored { get; set; }
        public bool Visible { get; set; }

        //------------------------------------------------------------------
        public Vector2 GlobalPosition
        {
            get
            {
                if (!Anchored && Root != null)
                    return Position + Root.GlobalPosition;
                return Position;
            }
        }

        //------------------------------------------------------------------
        public Object (Object root)
        {
            Root = root;

            Components = new List <Object> ();
        }

        //------------------------------------------------------------------
        public virtual void Setup ()
        {
            Components.ForEach (item => item.Setup ());
        }

        //------------------------------------------------------------------
        public virtual void Update (float elapsed)
        {
            Components.ForEach (item => item.Update (elapsed));
        }

        //------------------------------------------------------------------
        public virtual void Draw (SpriteBatch spriteBatch)
        {
            Components.ForEach (item => item.Draw (spriteBatch));
        }

        //-----------------------------------------------------------------
        protected void Add (Object item)
        {
            Components.Add (item);
        }

        //-----------------------------------------------------------------
        protected void Remove (Object item)
        {
            Components.Remove (item);
        }

        //------------------------------------------------------------------
        public void Move (float shift)
        {
            const float factor = 50; // Just for comfortable values

            Position += new Vector2 (0, shift / factor);
        }
    }
}