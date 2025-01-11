using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Coralite.Content.Dusts
{
    public class PixelPoint : ModDust
    {
        public override string Texture => AssetDirectory.Dusts + Name;

        public override void OnSpawn(Dust dust)
        {
            base.OnSpawn(dust);
        }

        public override bool Update(Dust dust)
        {
            Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.25f);

            dust.color *= 0.8f;
            dust.velocity *= 0.95f;

            dust.position += dust.velocity;

            if (dust.color.A < 10)
            {
                dust.active = false;
            }
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, Texture2D.Frame(2, 1), dust.color, 0, Texture2D.Size() / 2, dust.scale, 0, 0);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, Texture2D.Frame(2, 1, 1), dust.color * 2f, 0, Texture2D.Size() / 2, dust.scale, 0, 0);
            return false;
        }
    }

    public class PixelLine : Particle
    {
        public override string Texture => AssetDirectory.Dusts + "PixelPoint";

        public float TrailCount = 8;
        public bool useStartLimit=true;
        public float fadeFactor = 0.85f;
        public Vector2 basePos;

        public override void SetProperty()
        {
            Rotation = Velocity.ToRotation()+MathHelper.Pi;

            basePos = Position;
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            Lighting.AddLight(Position, Color.ToVector3() * 0.25f);

            Color *= fadeFactor;
            //Velocity *= 0.95f;

            Opacity++;
            if (Color.A < 10)
            {
                active = false;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;
            var frame = tex.Frame(2, 1, 1);

            Vector2 pos = Position - Main.screenPosition;
            Vector2 dir = Velocity * TrailCount;
            if (dir.Length() < frame.Width)
                dir = Velocity.SafeNormalize(Vector2.UnitX * frame.Width);

            Vector2 endPos = (Opacity < TrailCount && useStartLimit) ? (basePos - Main.screenPosition) : (pos - dir);

            Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, (int)((pos- endPos).Length() * Scale), (int)(frame.Height * Scale));
            Vector2 origin = new Vector2(0, frame.Height / 2);

            Main.spriteBatch.Draw(tex, rect, frame, Color, Rotation, origin, 0, 0);
            Main.spriteBatch.Draw(tex, rect, frame, Color * 2f, Rotation, origin, 0, 0);
            return false;
        }
    }
}
