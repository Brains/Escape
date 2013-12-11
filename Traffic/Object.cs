using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    public class Object
    {
        // Nodes
        public Object Root { get; set; }
        public List<Object> Components { get; private set; }

        // Properties
        public Vector2 LocalPosition { get; set; }

        public bool Active { get; set; }
        // ToDo: Can replace Anchored?
        public bool Anchored { get; set; }
        // ToDo: Delete Deleted?
        public bool Deleted { get; set; }

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
        // ToDo: Delete? Lane.Setup?
        public virtual void SetupDelete ()
        {
            Components.ForEach (item => item.Setup ());
        }

        //------------------------------------------------------------------
        public virtual void Update (float elapsed)
        {
            foreach (var component in Components)
                if (component.Active)
                    component.Update (elapsed);
        }

        //-----------------------------------------------------------------
        protected virtual void Add (Object item)
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
            LocalPosition += new Vector2 (0, shift);
        }

    }
}