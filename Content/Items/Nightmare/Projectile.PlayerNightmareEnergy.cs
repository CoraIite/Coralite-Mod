using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Items.Nightmare
{
    public class PlayerNightmareEnergy : BaseHeldProj, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Blank;
        public ref float Timer => ref Projectile.localAI[0];
        public ref float Scale => ref Projectile.localAI[1];
        public ref float Rot => ref Projectile.localAI[2];

        public static Asset<Texture2D> ringTex;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            ringTex = ModContent.Request<Texture2D>(AssetDirectory.Rediancie + "RedShield");
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            ringTex = null;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 2;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void OnSpawn(IEntitySource source)
        {
            Scale = 0.3f;
        }

        public override void AI()
        {
            if (Owner.dead || !Owner.active)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Center = Owner.Center;

            if (Item.ModItem is INightmareWeapon)
            {
                Projectile.timeLeft = 2;
            }

            Timer++;
            Projectile.rotation += 0.03f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int howMany = 0;
            bool nightmareHeart = false;

            Texture2D lightTex = BaseNightmareSparkle.MainLight.Value;

            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                howMany = cp.nightmareEnergy;
                nightmareHeart = cp.HasEffect(nameof(NightmareHeart));
            }

            Vector2 pos = Projectile.Center - Main.screenPosition;
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            float rot = Timer * 0.02f;
            Vector2 origin = lightTex.Size() / 2;

            Color c1 = NightmarePlantera.nightmareRed * 0.75f;
            c1.A = 0;
            Color c2 = NightmarePlantera.nightPurple;
            c2.A = 0;
            Color c3 = Color.White * 0.35f;
            c3.A = 0;

            for (int i = 0; i < howMany; i++)
            {
                float rot1 = Projectile.rotation + (i * MathHelper.TwoPi / howMany);
                Vector2 dir = rot1.ToRotationVector2();
                Vector2 position = pos + (dir * (40 + MathF.Sin(Main.GlobalTimeWrappedHourly * 2 + i * MathHelper.PiOver2) * 6 * factor));
                float factor2 = 0.15f + (factor * 0.02f);
                Vector2 scale = new Vector2(1.35f, 1f) * factor2;

                Main.spriteBatch.Draw(lightTex, position, null, c1, rot1 + MathHelper.PiOver2, origin, scale, 0, 0);

                //Helpers.Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, position, Color.White * 0.7f, NightmarePlantera.nightmareRed,
                //   factor2, 0f, 0.5f, 0.5f, 1f, Projectile.rotation + rot, new Vector2(0.9f, 0.9f), Vector2.One * 1.2f);
                if (nightmareHeart)
                {
                    scale = new Vector2(1f, 0.5f) * factor2 * 0.7f;
                    Main.spriteBatch.Draw(lightTex, position, null, c2, rot1, origin, scale, 0, 0);
                    c3 = c2;
                    //Main.spriteBatch.Draw(lightTex, position, null, c3*0.5f, rot1 + MathHelper.PiOver4, origin, factor2, 0, 0);
                    //Helpers.Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, position, Color.White * 0.7f, NightmarePlantera.nightPurple,
                    //       factor2, 0f, 0.5f, 0.5f, 1f, Projectile.rotation + rot + MathHelper.PiOver4, new Vector2(0.4f, 0.4f), Vector2.One);
                }

                Main.spriteBatch.Draw(lightTex, position, null, c3, rot1 + MathHelper.PiOver2, origin, scale, 0, 0);

            }
            return false;
        }

        public static void Spawn(Player player, Item item)
        {
            if (Main.projectile.Any(p => p.active && p.owner == player.whoAmI && p.type == ModContent.ProjectileType<PlayerNightmareEnergy>()))
                return;

            Projectile.NewProjectile(new EntitySource_ItemUse(player, item), player.Center, Vector2.Zero, ModContent.ProjectileType<PlayerNightmareEnergy>(), 1, 0, player.whoAmI);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            float scale;
            float halfScaleRange = 0.1f;
            float half = 0.4f;

            if (!Main.gamePaused && Main.instance.IsActive)
                Scale += 0.003f;

            if (Scale < 0.5f)
            {
                scale = Scale;
            }
            else
            {
                Scale = 0.3f;
                scale = Scale;
            }

            if (!Main.gamePaused && Main.instance.IsActive)
                Rot += 0.01f;

            if (Rot > (float)Math.PI * 2f)
                Rot -= (float)Math.PI * 2f;

            if (Rot < (float)Math.PI * -2f)
                Rot += (float)Math.PI * 2f;

            for (int j = 0; j < 3; j++)
            {
                float num4 = scale + (halfScaleRange * j);
                if (num4 > 0.5f)
                    num4 -= halfScaleRange * 2f;

                float num5 = MathHelper.Lerp(0.8f, 0f, Math.Abs(num4 - half) * 10f);
                Color c = NightmarePlantera.nightmareRed;
                c.A = (byte)(num5 * 255 / 2f);
                spriteBatch.Draw(ringTex.Value, Owner.Center - Main.screenPosition, null, c, Rot + (MathHelper.Pi / 3f * j), ringTex.Size() / 2, num4, SpriteEffects.None, 0f);
            }
        }
    }
}
