using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Tools.Extensions;

namespace Traffic
{
    public class Drawable
    {
        // Properties
        public bool Visible { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public float Rotation { get; set; }
        public Color Color { get; set; }
        public SpriteEffects Flip { get; set; }
        public float Depth { get; set; }

        // Fields
        private readonly Texture2D texture;
        private readonly Vector2 origin;

        //------------------------------------------------------------------
        public Drawable (Game game, string name)
        {
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Visible = true;

            // Load Texture
            texture = game.Content.Load<Texture2D> (name);
            origin = new Vector2 (texture.Width / 2.0f, texture.Height / 2.0f);
            Color = Color.White; // ToDo: Otherwise will be Transparent?
        }

        //------------------------------------------------------------------
        public virtual void Draw (SpriteBatch spriteBatch)
        {
            if (texture == null) return;
            if (!Visible) return;

            spriteBatch.Draw (texture, Position, null, Color, Rotation, origin, Scale, Flip, Depth);
        }
    }
}