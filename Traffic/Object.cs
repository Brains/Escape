using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    public class Object
    {
        public Object Root { get; set; }
        public List<Object> Components { get; private set; }

        public Vector2 LocalPosition { get; set; }

        public bool Deleted { get; set; }
        public bool Anchored { get; set; }
        public bool Active { get; set; }
        public bool Visible { get; set; }

        //------------------------------------------------------------------
        public Vector2 Position
        {
            get
            {
                if (Anchored) return LocalPosition;
                if (Root == null) return LocalPosition;
                    
                return LocalPosition + Root.Position;
            }
        }

        //------------------------------------------------------------------
        public Object (Object root)
        {
            Root = root;

            Components = new List <Object> ();

            Active = true;
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
        protected void Delete ()
        {
            Deleted = true;
        }

        //------------------------------------------------------------------
        protected void Remove (Object item)
        {
            Components.Remove (item);
        }

        //------------------------------------------------------------------
        public void Move (float shift)
        {
            LocalPosition += new Vector2 (0, shift * 0.3f);
        }

    }
}