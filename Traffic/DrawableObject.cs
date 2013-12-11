using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Traffic
{
    public class DrawableObject : Object
    {
        // Nodes
        public List <DrawableObject> DrawableComponents { get; private set; }

        // Properties
        public bool Visible { get; set; }
        public Color Color { get; set; }
        public float Angle { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects Flip { get; set; }
        public float Depth { get; set; }

        // Fields
        private Texture2D texture;
        private Vector2 origin; // Set in Texture property if it will be?

        //------------------------------------------------------------------
        public DrawableObject (Object root) : base (root)
        {
            DrawableComponents = new List <DrawableObject>();

            Visible = true;
            Color = Color.White; // ToDo: Otherwise will be Transparent?
            Scale = Vector2.One;
        }

        //------------------------------------------------------------------
        public override void Setup()
        {
            base.Setup();
        }

        //------------------------------------------------------------------
        protected override void Add (Object item)
        {
            AddDrawable(item);
            base.Add (item);
        }

        //------------------------------------------------------------------
        private void AddDrawable (Object item)
        {
            DrawableObject drawable = item as DrawableObject;

            if (drawable != null)
                DrawableComponents.Add (drawable);
        }

        //------------------------------------------------------------------
        public virtual void Draw (SpriteBatch spriteBatch)
        {
            // Draw Components
            foreach (var component in DrawableComponents)
                if (component.Visible)
                    component.Draw (spriteBatch);


            if (texture == null) return;

            // ToDo: Or it must be before Components drawing?
            // Draw Object
            spriteBatch.Draw (texture, Position, null, Color, Angle, origin, Scale, Flip, Depth);
        }
    }
}