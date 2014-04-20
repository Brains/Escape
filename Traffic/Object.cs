using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    public class Object
    {
        // Nodes
        public Object Root { get; /*private*/ set; }
        public List<Object> Components { get; private set; }

        // Drawing
        public Drawable Drawable { get; private set; }

        // Properties
        public Vector2 Position
        {
            get
            {
                if (Fixed) return LocalPosition;
                if (Root == null) return LocalPosition;
                    
                return LocalPosition + Root.Position;
            }
        }

        public Vector2 LocalPosition { get; set; }
        public bool Active { get; set; }
        public bool Fixed { get; set; }
        public bool Deleted { get; set; }

        //------------------------------------------------------------------
        public Object (Object root)
        {
            Root = root;
            Components = new List <Object> ();
        }

        //------------------------------------------------------------------
        public virtual void Setup (Game game)
        {
            foreach (var component in Components)
                component.Setup (game);

            Active = true;
        }

        //------------------------------------------------------------------
        public void CreateDrawable (Game game, string name)
        {
            Drawable = new Drawable (this, game, name);
        }

        //------------------------------------------------------------------
        public virtual void Update (float elapsed)
        {
            foreach (var component in Components)
                if (component.Active)
                    component.Update (elapsed);

            EraseDeleted();
        }

        //-----------------------------------------------------------------
        public virtual void Draw (SpriteBatch batch)
        {
            // Draw itself
            if (Drawable != null)
                Drawable.Draw (batch);

            // Draw each Component
            foreach (var component in Components)
                component.Draw (batch);
        }

        //------------------------------------------------------------------
        protected virtual void Add (Object item)
        {
            Components.Add (item);
        }

        //-----------------------------------------------------------------
        protected void Remove (Object item)
        {
            Components.Remove (item);
        }

        //------------------------------------------------------------------
        protected void Delete ()
        {
            Deleted = true;
        }

        //-----------------------------------------------------------------
        private void SetRoot (Object root)
        {
            // Remove from Current root
            // Add to New root
            // this.Root change to New root

            // Must works in Update loop

            throw new NotImplementedException();
        }

        //-----------------------------------------------------------------
        private void EraseDeleted ( )
        {
            Components.RemoveAll (component => component.Deleted);
        }

        //------------------------------------------------------------------
        public void Move (Vector2 shift)
        {
            LocalPosition += shift;
        }
    }
}