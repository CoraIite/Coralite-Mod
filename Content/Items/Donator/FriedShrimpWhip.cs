using Coralite.Content.Dusts;
using Coralite.Content.Items.Gels;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.Items.Donator
{
    public class FriedShrimpWhip : ModItem
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<FriedShrimpProj>(), 18, 1.5f, 4f,50);
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ModContent.RarityType<FriedShrimpRarity>();
        }

        public override bool AllowPrefix(int pre) => true;
        public override bool MeleePrefix() => true;
    }

    public class FriedShrimpBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(3))
            {
               int index= Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<FriedDust>(), Scale : Main.rand.NextFloat(0.75f, 1.2f));
                Dust d = Main.dust[index];
                d.velocity = new Vector2(0, -Main.rand.NextFloat(3f)).RotateByRandom(-0.1f,0.1f);
                //d.noGravity = true;
                d.fadeIn = 55;
            }

            if ((int)Main.timeForVisualEffects%10==0&&Main.rand.NextBool(5))
            {
                Dust.NewDustPerfect(npc.Center + Main.rand.NextVector2Circular(16, 16)
                , ModContent.DustType<Spiral>(), new Vector2(Main.rand.NextFloat(0.3f, 1f) * Main.rand.NextFromList(-1, 1), 0), newColor: new Color(200, 50, 50), Scale: Main.rand.NextFloat(0.6f, 0.8f));

            }
        }
    }

    public class FriedShrimpRarity : ModRarity
    {
        public override Color RarityColor => Color.Lerp(new Color(252, 213, 108), new Color(231, 150, 48)
            , MathF.Sin(Main.GlobalTimeWrappedHourly) / 2 + 0.5f);
    }

    public class FriedShrimpProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Donator + Name;

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.Segments = 16;
            Projectile.WhipSettings.RangeMultiplier = 1;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // Without PiOver2, the rotation would be off by 90 degrees counterclockwise.

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            Projectile.spriteDirection = Projectile.velocity.X >= 0f ? 1 : -1;

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

            float swingProgress = Timer / swingTime;
            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f 
                && Main.rand.NextBool(4))
            {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                points.Clear();
                Projectile.FillWhipControlPoints(Projectile, points);
                int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
                Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));

                // After choosing a randomized dust and a whip segment to spawn from, dust is spawned.
                Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, ModContent.DustType<FriedDust>(), 0f, 0f, 150, new Color(78, 136, 255, 80)
                    ,Scale:Main.rand.NextFloat(0.75f,1f));
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<FriedShrimpBuff>(), 240);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.75f);

            Dust.NewDustPerfect(target.Center + Main.rand.NextVector2Circular(16, 16)
                , ModContent.DustType<Spiral>(), new Vector2(Main.rand.NextFloat(0.3f, 1f)*Main.rand.NextFromList(-1,1), 0) , newColor:new Color(200,50,50),Scale: Main.rand.NextFloat(0.6f, 0.8f));
        }

        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.Orange);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            //DrawLine(list);


            Main.instance.LoadProjectile(Type);
            Texture2D texture = Projectile.GetTexture();

            Vector2 pos = list[1];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new Rectangle(0, 0, 30, 24); // 鞭子把手的大小
                SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                float scale = 1;

                if (i == list.Count - 2)
                {
                    frame.Y = 114; // 顶端到最下面的鞭子头部的距离
                    frame.Height = 14; // 鞭子头部的长度

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    //scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 0)
                {
                    frame.Y = 24 + ((i - 1) / 2) * 10;
                    frame.Height = 16;

                    if (i % 2 == 0)
                    {
                        flip = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    }
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
}
