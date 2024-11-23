using Coralite.Core;
using InnoVault.PRT;
using System;
using Terraria;

namespace Coralite.Content.Bosses.ThunderveinDragon
{
    public class ElectricParticle : BasePRT
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "ElectricParticle";

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = TexValue.Frame(7, 5, 0, Main.rand.Next(5));
            Color = Color.White;
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Opacity++;
            Position += Velocity;
            if (Opacity > 1 && Opacity % 4 == 0)
            {
                Frame.X += 80;
                if (Frame.X > 80 * 6)
                    active = false;
            }
        }
    }

    public class ElectricParticle_Purple : ElectricParticle
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "ElectricParticle_Purple";
    }

    public class ElectricParticle_Follow : ElectricParticle
    {
        public override bool ShouldUpdatePosition() => false;

        private Func<Vector2> GetParentCenter;

        public override void AI()
        {
            if (!GetCenter(out Vector2 parentCenter))
                return;

            Position = parentCenter + Velocity;
            Opacity++;
            if (Opacity > 1 && Opacity % 4 == 0)
            {
                Frame.X += 80;
                if (Frame.X > 80 * 6)
                    active = false;
            }
        }

        public bool GetCenter(out Vector2 parentCenter)
        {
            if (GetParentCenter != null)
            {
                parentCenter = GetParentCenter();
                return true;
            }

            parentCenter = Vector2.Zero;
            active = false;
            return false;
        }

        public static void Spawn(Vector2 parentCenter, Vector2 offset, Func<Vector2> GetParentCenter, float scale = 1f)
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            ElectricParticle_Follow p = PRTLoader.NewParticle<ElectricParticle_Follow>(parentCenter + offset, offset, Scale: scale);
            p.GetParentCenter = GetParentCenter;
        }
    }

    public class LightningParticle : BasePRT
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + Name;

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            Frame = TexValue.Frame(4, 4, 0, Main.rand.Next(4));
            Color = Color.White;
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override void AI()
        {
            Opacity++;
            Position += Velocity;
            if (Opacity > 1 && Opacity % 5 == 0)
            {
                Frame.X += 32;
                if (Frame.X > 32 * 3)
                    active = false;
            }
        }
    }

    public class LightningShineBall : ModDust
    {
        public override string Texture => AssetDirectory.Particles + "LightBall";

        public override void OnSpawn(Dust dust)
        {
            dust.color.A = 0;
        }

        public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.fadeIn++;
            if (dust.fadeIn > 5)
                dust.scale *= 0.9f;

            if (dust.fadeIn > 60 || dust.scale < 0.001f)
            {
                dust.active = false;
            }
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color, 0, Texture2D.Size() / 2, dust.scale, 0, 0);
            Main.spriteBatch.Draw(Texture2D.Value, dust.position - Main.screenPosition, null, dust.color, 0, Texture2D.Size() / 2, dust.scale / 2, 0, 0);
            return false;
        }
    }
}
