﻿using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Content.Items.Nightmare
{
    public class PlayerNightmareEnergy : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Blank;

        public Player Owner => Main.player[Projectile.owner];

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

            if (Owner.HeldItem.ModItem is INightmareWeapon)
            {
                Projectile.timeLeft = 2;
            }

            Timer++;
            Projectile.rotation += 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int howMany = 0;
            bool nightmareHeart = false;
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                howMany = cp.nightmareEnergy;
                nightmareHeart = cp.HasEffect(nameof(NightmareHeart));
            }

            Vector2 pos = Projectile.Center - Main.screenPosition;
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            float rot = Timer * 0.02f;

            for (int i = 0; i < howMany; i++)
            {
                Vector2 dir = (Projectile.rotation + i * MathHelper.TwoPi / howMany).ToRotationVector2();
                Vector2 position = pos + dir * (40 + factor * 2);
                float factor2 = 0.5f + factor * 0.1f;
                Helpers.Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, position, Color.White * 0.7f, NightmarePlantera.nightmareRed,
                   factor2, 0f, 0.5f, 0.5f, 1f, Projectile.rotation + rot, new Vector2(0.9f, 0.9f), Vector2.One * 1.2f);
                if (nightmareHeart)
                    Helpers.Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, position, Color.White * 0.7f, NightmarePlantera.nightPurple,
                       factor2, 0f, 0.5f, 0.5f, 1f, Projectile.rotation + rot + MathHelper.PiOver4, new Vector2(0.4f, 0.4f), Vector2.One);
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
                float num4 = scale + halfScaleRange * j;
                if (num4 > 0.5f)
                    num4 -= halfScaleRange * 2f;

                float num5 = MathHelper.Lerp(0.8f, 0f, Math.Abs(num4 - half) * 10f);
                Color c = NightmarePlantera.nightmareRed;
                c.A = (byte)(num5 * 255 / 2f);
                spriteBatch.Draw(ringTex.Value, Owner.Center - Main.screenPosition, null, c, Rot + MathHelper.Pi / 3f * j, ringTex.Size() / 2, num4, SpriteEffects.None, 0f);
            }
        }
    }
}
