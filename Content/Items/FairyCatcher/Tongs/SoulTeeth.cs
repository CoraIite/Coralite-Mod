using Coralite.Content.DamageClasses;
using Coralite.Core;
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

    [VaultLoaden(AssetDirectory.FairyCatcherTong)]
    public class SoulTeethProj : BaseTongsProj
    {
        public static ATex SoulTeethChain { get; private set; }
        public static ATex SoulTeethHandle { get; private set; }

        public override Vector2 TongPosOffset => new Vector2(22, 0);
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

                for (int i = 0; i < 4; i++)
                {
                    int dir2 = (i % 2) == 0 ? -1 : 1;

                    Vector2 pos = Projectile.Center
                        + dir * (i * 15 - 50)
                        + dir.RotatedBy(MathHelper.PiOver2 * -dir2) * (35 + i * 10);


                    Projectile.NewProjectileFromThis<SoulTeethBite>(pos, dir * (5 + i * 1f)
                        , (int)(Projectile.damage * 0.25f), Projectile.knockBack, dir2 * 0.1f);
                }
            }
        }

        public override void Flying()
        {
            base.Flying();

            Projectile.SpawnTrailDust(DustID.Corruption, Main.rand.NextFloat(0.1f, 0.2f), Scale: Main.rand.NextFloat(1, 1.5f));
        }

        public override void PreDrawTong(Vector2 pos, Color lightColor)
        {
            SoulTeethHandle.Value.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, 1, 1, 2), pos - Main.screenPosition
                , lightColor, HandleRot, effect: Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        }

        public override void DrawHandle(Texture2D handleTex, Vector2 pos, Color lightColor)
        {
            handleTex.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, 0, 1, 2), pos - Main.screenPosition
                , lightColor, HandleRot, effect: Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        }


    }

    public class SoulTeethBite : ModProjectile
    {
        public override string Texture => AssetDirectory.FairyCatcherTong + Name;

        private ref float Alpha => ref Projectile.localAI[0];
        //private ref float StartRot => ref Projectile.ai[0];
        private ref float TurnRotPerFrame => ref Projectile.ai[0];
        private ref float HitCount => ref Projectile.ai[1];
        private ref float Timer => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.width = Projectile.height = 45;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool? CanDamage()
        {
            if (Timer > 13)
                return false;

            return base.CanDamage();
        }

        public override void AI()
        {
            //转动
            Projectile.velocity = Projectile.velocity.RotatedBy(TurnRotPerFrame);
            Projectile.rotation = Projectile.velocity.ToRotation();

            const int time = 10;

            if (Timer < 6)
            {
                Alpha += 1 / 6f;
            }
            else if (Timer > time)
            {
                Alpha -= 1 / 6f;
                if (Alpha < 0)
                    Projectile.Kill();
            }

            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(Projectile.width * 0.4f, DustID.ShadowbeamStaff, Main.rand.NextFloat(0.4f, -0.4f)
                    , 50);

            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            HitCount++;
            if (HitCount > 2 && Timer < 13)
                Timer = 13;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            Texture2D tex = Projectile.GetTexture();
            Vector2 pos = Projectile.Center - Main.screenPosition;

            float dir = MathF.Sign(TurnRotPerFrame);
            float rot = Projectile.rotation - dir * 0.3f;
            float scale = Projectile.scale * 0.7f;
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Color c1 = new Color(123, 89, 234) * Alpha * 0.7f;
            Color c2 = new Color(169, 139, 231) * Alpha * 0.8f;
            c2.A = 0;

            tex.QuickCenteredDraw(spriteBatch, pos, c1 * 0.4f, rot, scale * 1.8f, effect);
            tex.QuickCenteredDraw(spriteBatch, pos, c1, rot, scale * 1.4f, effect);

            tex.QuickCenteredDraw(spriteBatch, pos, c1, rot, scale, effect);
            //tex.QuickCenteredDraw(spriteBatch, pos, c2, rot, Projectile.scale, effect);

            tex.QuickCenteredDraw(spriteBatch, pos, c2, rot, scale, effect);

            return false;
        }
    }
}
