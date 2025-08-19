using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Tongs
{
    public class SoulTeeth : BaseTongsItem
    {
        public override string Texture => AssetDirectory.FairyCatcherTong + Name;

        public override int CatchPower => 15;

        public int hitCount;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<SoulTeethProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 12;
            Item.SetWeaponValues(24, 3);
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 0, 50));
            Item.autoReuse = true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.FairyCatcherTong)]
    public class SoulTeethProj : BaseTongsProj
    {
        public static ATex SoulTeethChain { get; private set; }
        public static ATex SoulTeethHandle { get; private set; }

        public override Vector2 TongPosOffset => new Vector2(26, 6);
        public override Vector2 HandelOffset => new Vector2(16, -6);

        public override int ItemType => ModContent.ItemType<SoulTeeth>();

        public override int MaxFlyLength => 16 * 12;

        public override bool DrawHnadleOnTop => true;

        public override Texture2D GetHandleTex() => SoulTeethHandle.Value;
        public override Texture2D GetLineTex() => SoulTeethChain.Value;

        public override Vector2 LineDrawStartPosOffset()
            => -HandleRot.ToRotationVector2() * 10;

        public override void OnHitNPCFlying(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.HeldItem.ModItem is SoulTeeth bt)
            {
                bt.hitCount++;
                if (bt.hitCount > 1)
                    bt.hitCount = 0;
                Vector2 dir = (target.Center - Owner.Center).SafeNormalize(Vector2.Zero);

                for (int i = 0; i < 3; i++)
                {
                    int dir2 = (i % 2) == 0 ? -1 : 1;
                    Projectile.NewProjectileFromThis<SoulTeethBite>(Owner.Center, dir * 7
                        , (int)(Projectile.damage * 0.4f), Projectile.knockBack, dir2 * 0.1f);
                }
            }
        }

        public override void Flying()
        {
            base.Flying();

            Projectile.SpawnTrailDust(DustID.Corruption, Main.rand.NextFloat(0.1f, 0.2f), Scale: Main.rand.NextFloat(1, 1.5f));
        }
    }

    public class SoulTeethBite : ModProjectile
    {
        public override string Texture => AssetDirectory.FairyCatcherTong + Name;

        private ref float Alpha => ref Projectile.localAI[0];
        //private ref float StartRot => ref Projectile.ai[0];
        private ref float TurnRotPerFrame => ref Projectile.ai[0];
        private ref float Timer => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.width = Projectile.height = 45;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            //转动
            Projectile.velocity = Projectile.velocity.RotatedBy(TurnRotPerFrame);
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Timer < 6)
            {
                Alpha += 1 / 6f;
            }

            else if (Timer > 20)
            {
                Alpha -= 1 / 6f;
                if (Timer > 20 + 6)
                    Projectile.Kill();
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();

            BlendState multiplyBlendState = EffectLoader.ReverseBlendState;
            spriteBatch.Begin(SpriteSortMode.Deferred, multiplyBlendState, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            Texture2D tex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;

            float dir = MathF.Sign(TurnRotPerFrame);
            float rot = Projectile.rotation - dir * 0.3f;
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Color c1 = Color.White*Alpha;
            Color c2 = new Color(255, 0, 255, 255)*Alpha;

            tex.QuickCenteredDraw(spriteBatch, pos, c1 * 0.4f, rot, Projectile.scale * 1.3f, effect);
            tex.QuickCenteredDraw(spriteBatch, pos, c1 * 0.4f, rot, Projectile.scale * 1.3f, effect);

            tex.QuickCenteredDraw(spriteBatch, pos, c1, rot, Projectile.scale, effect);
            //tex.QuickCenteredDraw(spriteBatch, pos, c2, rot, Projectile.scale, effect);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            tex.QuickCenteredDraw(spriteBatch, pos, c2, rot, Projectile.scale, effect);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return false;
        }
    }
}
