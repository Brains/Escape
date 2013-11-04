using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    internal class Object
    {
        private Vector2 position;
        
        protected readonly Object Root;

        public List <Object> Components { get; private set; }
        public bool Deleted { get; set; }

        //------------------------------------------------------------------
        public Vector2 Position
        {
            get
            {
                if (Root != null)
                    return position + Root.Position;
                
                return position;
            }
            set { position = value; }
        }

        //------------------------------------------------------------------
        public Object (Object root)
        {
            this.Root = root;

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
        private void Add (Object item)
        {
            Components.Add (item);
        }

        //-----------------------------------------------------------------
        private void Remove (Object item)
        {
            Components.Remove (item);
        }
    }
}