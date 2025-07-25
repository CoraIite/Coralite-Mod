using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using SteelSeries.GameSense;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ID;

namespace Coralite.Content.Items.FairyCatcher.Tongs
{
    public class BloodTentacle : BaseTongsItem
    {
        public override string Texture => AssetDirectory.FairyCatcherTong + Name;

        public override int CatchPower => 5;

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

    [AutoLoadTexture(Path = AssetDirectory.FairyCatcherTong)]
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
            Projectile.width = Projectile.height = 28;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            //甩出，
            if (Timer < 15)
            {
                if (Timer == 0)
                {
                    Projectile.rotation = TargetDir + RandDir * 2f;
                    Projectile.velocity = (TargetDir + RandDir * 0.4f).ToRotationVector2() * 12;
                }

                HandleLength = Helper.Lerp(30, 1, Timer / 18);
                Projectile.velocity = Projectile.velocity.RotatedBy(RandDir * 0.4f / 15);
            }
            else if (Timer < 20)
            {

            }

            Timer++;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public virtual Color GetStringColor(Vector2 pos)
        {
            return Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f), Color.White);
        }

        public virtual void DrawLine(Texture2D lineTex, Vector2 startPos, Vector2 endPos, Color color)
        {
            List<ColoredVertex> bars = new();

            float halfLineWidth = lineTex.Height / 2;

            Vector2 recordPos = startPos;
            float recordUV = 0;

            int lineLength = (int)(startPos - endPos).Length();   //链条长度
            int pointCount = lineLength / 16 + 3;
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
            return base.PreDraw(ref lightColor);
        }
    }
}
