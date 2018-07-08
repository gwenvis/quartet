using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Core
{
    public abstract class Entity
    {
        public Vector2 Position { get; set; }
        public Texture2D Sprite { get; set; }
        public Vector2 Size { get; set; }
        public float Rotation { get; set; }
        public int LayerDepth { get; set; }

        public virtual void Update(float dt) {}

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(Sprite, Position, null, Color.White, Rotation, new Vector2(), Size, SpriteEffects.None, LayerDepth);
        }

        public void SetSize(float size)
        {
            this.Size = new Vector2(size);
        }

        public void SetSize(Vector2 size)
        {
            this.Size = size;
        }

        /// <summary>
        /// Move towards
        /// </summary>
        public void Move(Vector2 vector)
        {
            Position += vector;
        }

        /// <summary>
        /// Rotate with certain amounht
        /// </summary>
        public void Rotate(float amount)
        {
            Rotation += amount;
        }
    }
}