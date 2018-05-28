using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kwartet.Desktop.Core
{
    public abstract class Entity
    {
        public Vector2 position;
        public Texture2D sprite;

        public virtual void Update(float dt) {}
        public virtual void Draw(SpriteBatch sb) {}
    }
}