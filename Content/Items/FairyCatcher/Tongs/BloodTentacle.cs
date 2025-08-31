using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Tongs
{
    public class BloodTentacle : BaseTongsItem
    {
        public override string Texture => AssetDirectory.FairyCatcherTong + Name;

        public override int CatchPower => 15;

        public int hitCount;

        public override void SetOtherDefaults()
        {
            Item.shoot = ModContent.ProjectileType<BloodTentacleProj>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 20;
            Item.shootSpeed = 12;
            Item.SetWeaponValues(24, 3);
            Item.SetShopValues(ItemRarityColor.Green2, Item.sellPrice(0, 0, 50));
            Item.autoReuse = true;
        }
    }

    [VaultLoaden(AssetDirectory.FairyCatcherTong)]
    public class BloodTentacleProj : BaseTongsProj
    {
        public static ATex BloodTentacleChain { get; private set; }
        public static ATex BloodTentacleHandle { get; private set; }

        public override Vector2 TongPosOffset => new Vector2(26, 6);
        public override Vector2 HandelOffset => new Vector2(16, -6);

        public override int ItemType => ModContent.ItemType<BloodTentacle>();

        public override int MaxFlyLength => 16 * 12;

        public override bool DrawHnadleOnTop => true;

        public override Texture2D GetHandleTex() => BloodTentacleHandle.Value;
        public override Texture2D GetLineTex() => BloodTentacleChain.Value;

        public override Vector2 LineDrawStartPosOffset()
            => -HandleRot.ToRotationVector2() * 10;

        public override void OnHitNPCFlying(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Owner.HeldItem.ModItem is BloodTentacle bt)
            {
                bt.hitCount++;
                if (bt.hitCount > 1)
                    bt.hitCount = 0;
                float dir = (target.Center - Owner.Center).ToRotation() + Main.rand.NextFloat(-0.1f, 0.1f);
                Projectile.NewProjectileFromThis<BloodTentacleSPProj>(Owner.Center, Vector2.Zero
                    , (int)(Projectile.damage * 0.4f), Projectile.knockBack, dir, bt.hitCount==0?-1:1);
            }
        }

        public override void Flying()
        {
            base.Flying();

            Projectile.SpawnTrailDust(DustID.Blood, Main.rand.NextFloat(0.1f, 0.2f),Scale:Main.rand.NextFloat(1,1.5f));
        }
    }

    public class BloodTentacleSPProj : ModProjectile
    {
        public override string Texture => AssetDirectory.FairyCatcherTong + "BloodTentacleProj";

        public Player Owner => Main.player[Projectile.owner];

        public ref float TargetDir => ref Projectile.ai[0]; 
        public ref float RandDir => ref Projectile.ai[1]; 
        public ref float Timer => ref Projectile.ai[2]; 
        public ref float HandleLength => ref Projectile.localAI[2]; 

        public override void SetDefaults()
        {
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.width = Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            //甩出，
            if (Timer < 15)
            {
                //画弧线飞出
                if (Timer == 0)
                {
                    Projectile.tileCollide = false;
                    Projectile.rotation = TargetDir + RandDir * 2f;
                    Projectile.velocity = (TargetDir + RandDir * 0.9f).ToRotationVector2() * 13;
                }

                if (Timer == 8)
                    Projectile.tileCollide = true;

                Projectile.rotation += -RandDir * 2f / 12;
                HandleLength = Helper.Lerp(80, 40, Timer / 15);
                Projectile.velocity = Projectile.velocity.RotatedBy(-RandDir * 1.5f / 15);
            }
            else if (Timer < 25)
            {
                //伸直
                Projectile.velocity = Projectile.velocity.RotatedBy(-RandDir * 0.5f / 10);
                Projectile.velocity *= 0.94f;
                Projectile.rotation += -RandDir * 1f / 10;
                Vector2 toOwner = Owner.Center - Projectile.Center;

                HandleLength = Helper.Lerp(HandleLength, toOwner.Length() / 2, 0.2f);
            }
            else
            {
                Vector2 toOwner = Owner.Center - Projectile.Center;

                HandleLength = Helper.Lerp(HandleLength, toOwner.Length() / 2, 0.2f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toOwner.SafeNormalize(Vector2.Zero) * (Timer / 2)
                    , MathHelper.Clamp(Timer / 100, 0, 1));
                Projectile.rotation = Projectile.rotation.AngleLerp((-toOwner).ToRotation() + RandDir * 0.8f, 0.05f);

                if (Vector2.Distance(Owner.Center, Projectile.Center) < Projectile.velocity.Length() * 1.5f)
                    Projectile.Kill();
            }

            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(DustID.Blood, Main.rand.NextFloat(0.1f, 0.3f), Scale: Main.rand.NextFloat(1, 1.5f));

            Timer++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Timer < 15)
                Timer = 15;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Timer < 15)
                Timer = 15;

            Projectile.damage = (int)(Projectile.damage * 0.9f);

            if (VisualEffectSystem.HitEffect_Dusts)
            {
                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, DustID.Blood
                        , Helper.NextVec2Dir(0.5f, 2f), Scale: Main.rand.NextFloat(1, 2f));
                }
            }
        }

        public virtual Color GetStringColor(Vector2 pos)
        {
            return Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), Color.White);
        }

        public virtual void DrawLine(Texture2D lineTex, Vector2 startPos, Vector2 endPos)
        {
            List<ColoredVertex> bars = new();

            float halfLineWidth = lineTex.Height / 2;

            Vector2 recordPos = startPos;
            float recordUV = 0;

            int lineLength = (int)(startPos - endPos).Length();   //链条长度
            int pointCount = lineLength / 8 + 3;
            Vector2 controlPos = endPos - Projectile.rotation.ToRotationVector2() * HandleLength;

            //贝塞尔曲线
            for (int i = 0; i < pointCount; i++)
            {
                float factor = (float)i / pointCount;

                Vector2 P1 = Vector2.Lerp(startPos, controlPos, factor);
                Vector2 P2 = Vector2.Lerp(controlPos, endPos, factor);

                Vector2 Center = Vector2.Lerp(P1, P2, factor);
                var Color = GetStringColor(Center + Main.screenPosition);

                Vector2 normal = (P2 - P1).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
                Vector2 Top = Center + normal * halfLineWidth;
                Vector2 Bottom = Center - normal * halfLineWidth;

                recordUV += (Center - recordPos).Length() / lineTex.Width;

                bars.Add(new(Top, Color, new Vector3(recordUV, 0, 1)));
                bars.Add(new(Bottom, Color, new Vector3(recordUV, 1, 1)));

                recordPos = Center;
            }

            var state = Main.graphics.GraphicsDevice.SamplerStates[0];
            Main.graphics.GraphicsDevice.Textures[0] = lineTex;
            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.SamplerStates[0] = state;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawLine(BloodTentacleProj.BloodTentacleChain.Value, Owner.Center - Main.screenPosition
                , Projectile.Center - Main.screenPosition);

            Projectile.GetTexture().QuickCenteredDraw(Main.spriteBatch, Projectile.Center - Main.screenPosition
                , lightColor, Projectile.rotation, Projectile.scale, Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);

            return false;
        }
    }
}
