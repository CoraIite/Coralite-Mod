using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Gels
{
    public class GelWhip : ModItem
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<GelWhipProj>(), 24, 2, 4, 26);

            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 1);
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;
    }

    public class GelWhipProj : ModProjectile
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.WhipSettings.Segments = 12;
            Projectile.WhipSettings.RangeMultiplier = 1.5f;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;

        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        //private float ChargeTime
        //{
        //    get => Projectile.ai[1];
        //    set => Projectile.ai[1] = value;
        //}

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // Without PiOver2, the rotation would be off by 90 degrees counterclockwise.

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + (Projectile.velocity * Timer);
            // Vanilla uses Vector2.Dot(Projectile.velocity, Vector2.UnitX) here. Dot Product returns the difference between two vectors, 0 meaning they are perpendicular.
            // However, the use of UnitX basically turns it into a more complicated way of checking if the projectile's velocity is above or equal to zero on the X axis.
            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

            // remove these 3 lines if you don't want the charging mechanic
            //if (!Charge(owner))
            //{
            //    return; // timer doesn't update while charging, freezing the animation at the start.
            //}

            Timer++;

            float swingTime = owner.itemAnimationMax * Projectile.MaxUpdates;
            if (Timer >= swingTime || owner.itemAnimation <= 0)
            {
                Projectile.Kill();
                return;
            }

            owner.heldProj = Projectile.whoAmI;
            if (Timer == swingTime / 2)
            {
                // Plays a whipcrack sound at the tip of the whip.
                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(SoundID.Item153, points[points.Count - 1]);
            }

            // Spawn Dust along the whip path
            // This is the dust code used by Durendal. Consult the Terraria source code for even more examples, found in Projectile.AI_165_Whip.
            float swingProgress = Timer / swingTime;
            // This code limits dust to only spawn during the the actual swing.
            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(3))
            {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                points.Clear();
                Projectile.FillWhipControlPoints(Projectile, points);
                int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
                Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));
                int dustType = DustID.t_Slime;

                // After choosing a randomized dust and a whip segment to spawn from, dust is spawned.
                Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 150, new Color(78, 136, 255, 80));
                dust.position = points[pointIndex];
                //dust.fadeIn = 0.3f;
                Vector2 spinningpoint = points[pointIndex] - points[pointIndex - 1];
                //dust.noGravity = true;
                dust.velocity *= 0.5f;
                // This math causes these dust to spawn with a velocity perpendicular to the direction of the whip segments, giving the impression of the dust flying off like sparks.
                dust.velocity += spinningpoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
                dust.velocity *= 0.5f;
            }
        }

        // This method handles a charging mechanic.
        // If you remove this, also remove Item.channel = true from the item's SetDefaults.
        // Returns true if fully charged
        //private bool Charge(Player owner)
        //{
        //    // Like other whips, this whip updates twice per frame (Projectile.extraUpdates = 1), so 120 is equal to 1 second.
        //    if (!owner.channel || ChargeTime >= 120)
        //    {
        //        return true; // finished charging
        //    }

        //    ChargeTime++;

        //    if (ChargeTime % 12 == 0) // 1 segment per 12 ticks of charge.
        //        Projectile.WhipSettings.Segments++;

        //    // Increase range up to 2x for full charge.
        //    Projectile.WhipSettings.RangeMultiplier += 1 / 120f;

        //    // Reset the animation and item timer while charging.
        //    owner.itemAnimation = owner.itemAnimationMax;
        //    owner.itemTime = owner.itemTimeMax;

        //    return false; // still charging
        //}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GelWhipDebuff>(), 240);
            target.AddBuff(BuffID.Slimed, 320);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.75f);
        }

        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.Blue);
                Vector2 scale = new(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = Projectile.GetTexture();

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new(0, 0, 30, 38); // 鞭子把手的大小
                float scale = 1;

                if (i == list.Count - 2)
                {
                    // This is the head of the whip. You need to measure the sprite to figure out these values.
                    frame.Y = 86; // 顶端到最下面的鞭子头部的距离
                    frame.Height = 24; // 鞭子头部的长度

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 0)
                {
                    // 根据i循环帧图
                    frame.Y = 38 + (i % 3 * 16);
                    frame.Height = 16;
                }

                Vector2 origin = frame.Size() / 2; // 鞭子的中心

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }

    public class GelWhipDebuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

        public static readonly int TagDamage = 6;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
}
