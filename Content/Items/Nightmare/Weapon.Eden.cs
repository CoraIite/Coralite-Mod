using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class Eden : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        private int combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 27;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ProjectileType<EdenWhip>();
            Item.DamageType = DamageClass.Summon;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(170, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 10;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, source);
                if (player.altFunctionUse == 2)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.Snake_Item151, player.Center);
                    Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<EdenWhip2>(), damage, knockback, player.whoAmI);
                    return false;
                }

                SoundEngine.PlaySound(CoraliteSoundID.WhipSwing_Item152, player.Center);

                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: combo);

                if (++combo > 3)
                    combo = 0;
            }
            return false;
        }
    }

    public class EdenWhip : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

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
            Projectile.WhipSettings.Segments = 18;
            Projectile.WhipSettings.RangeMultiplier = 1.1f;
            Projectile.DamageType = DamageClass.SummonMeleeSpeed;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private ref float State => ref Projectile.ai[1];
        private bool hited = true;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2; // Without PiOver2, the rotation would be off by 90 degrees counterclockwise.

            Projectile.Center = Main.GetPlayerArmPosition(Projectile) + Projectile.velocity * Timer;
            Lighting.AddLight(Projectile.Center, NightmarePlantera.nightmareRed.ToVector3());
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
                List<Vector2> points = Projectile.WhipPointsForCollision;
                Projectile.FillWhipControlPoints(Projectile, points);
                SoundEngine.PlaySound(CoraliteSoundID.FlyingSnake_NPCDeath26, points[points.Count - 1]);
            }

            float swingProgress = Timer / swingTime;
            if (Utils.GetLerpValue(0.1f, 0.7f, swingProgress, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, swingProgress, clamped: true) > 0.5f && !Main.rand.NextBool(4))
            {
                List<Vector2> points = Projectile.WhipPointsForCollision;
                points.Clear();
                Projectile.FillWhipControlPoints(Projectile, points);
                int pointIndex = Main.rand.Next(points.Count - 10, points.Count);
                Rectangle spawnArea = Utils.CenteredRectangle(points[pointIndex], new Vector2(30f, 30f));
                int dustType = DustID.VilePowder;

                // After choosing a randomized dust and a whip segment to spawn from, dust is spawned.
                Dust dust = Dust.NewDustDirect(spawnArea.TopLeft(), spawnArea.Width, spawnArea.Height, dustType, 0f, 0f, 150, NightmarePlantera.nightPurple);
                dust.position = points[pointIndex];
                //dust.fadeIn = 0.3f;
                Vector2 spinningpoint = points[pointIndex] - points[pointIndex - 1];
                //dust.noGravity = true;
                dust.velocity *= 0.5f;
                // This math causes these dust to spawn with a velocity perpendicular to the direction of the whip segments, giving the impression of the dust flying off like sparks.
                dust.velocity += spinningpoint.RotatedBy(owner.direction * ((float)Math.PI / 2f));
                dust.velocity *= 0.5f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State == 0 && hited)
            {
                Main.player[Projectile.owner].GetModPlayer<CoralitePlayer>().GetNightmareEnergy(1);
                hited = false;
            }
            target.AddBuff(BuffType<EdenDebuff>(), 240);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            Projectile.damage = (int)(Projectile.damage * 0.75f);
        }

        public static void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);

            Vector2 pos = list[0];
            for (int i = 0; i < list.Count - 2; i++)
            {
                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates(), Color.Gray);
                Vector2 scale = new Vector2(2, (diff.Length() + 2) / frame.Height);

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);

                pos += diff;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);

            DrawLine(list);

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new Rectangle(0, 0, 34, 24); // 鞭子把手的大小
                float scale = 1;

                if (i == list.Count - 2)
                {
                    // This is the head of the whip. You need to measure the sprite to figure out these values.
                    frame.Y = 106; // 顶端到最下面的鞭子头部的距离
                    frame.Height = 40; // 鞭子头部的长度

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.8f, 1.3f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 0)
                {
                    // 根据i循环帧图
                    if (i % 5 == 0)
                    {
                        frame.Y = 28 + 26;
                    }
                    else
                    {
                        frame.Y = 28 + (i % 3 == 0 ? 0 : 26 * 2);
                    }

                    frame.Height = 22;
                }

                Vector2 origin = frame.Size() / 2; // 鞭子的中心

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                pos += diff;
            }
            return false;
        }
    }

    public class EdenWhip2 : BaseHeldProj
    {
        public override string Texture => AssetDirectory.NightmareItems + "EdenWhip";

        public ref float TimeMax => ref Projectile.ai[0];
        public ref float PerPartLength => ref Projectile.ai[1];

        public ref float FinalRotationOffset => ref Projectile.localAI[0];

        public const int CACHE_LENGTH = 18;
        private bool initialize = true;

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.timeLeft = 50;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0;
            return Collision.CanHitLine(Owner.Center, 1, 1, targetHitbox.Center.ToVector2(), 1, 1)
                && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Owner.Center, Projectile.oldPos[CACHE_LENGTH - 1], 30, ref a);
        }

        public override void AI()
        {
            if (initialize)
            {
                if (Main.myPlayer == Projectile.owner)  //初始化鞭子节点，以及其他信息
                {
                    Projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One);
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    Projectile.netUpdate = true;
                }
                TimeMax = 30;
                FinalRotationOffset = -0.17f;
                Projectile.timeLeft = (int)TimeMax;
                Projectile.oldPos = new Vector2[CACHE_LENGTH];
                Projectile.oldRot = new float[CACHE_LENGTH];
                for (int i = 0; i < CACHE_LENGTH; i++)
                {
                    Projectile.oldPos[i] = Owner.Center;
                    Projectile.oldRot[i] = FinalRotationOffset;
                }
                PerPartLength = 0.1f;
                initialize = false;
            }

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(Projectile.oldPos[CACHE_LENGTH - 1], DustID.VilePowder, newColor: NightmarePlantera.nightmareRed, Scale: Main.rand.NextFloat(1f, 1.3f));
                d.noGravity = true;
            }

            if (Projectile.timeLeft % 12 >= 6)
            {
                FinalRotationOffset -= 0.13f;  //更新额外角度
            }
            else
            {
                FinalRotationOffset += 0.13f;  //更新额外角度
            }

            if (Projectile.timeLeft > (int)TimeMax / 2)
            {
                if (PerPartLength < 28)
                    PerPartLength += 28 / (TimeMax / 3);

            }
            else if (Projectile.timeLeft == (int)TimeMax / 2)
            {
                if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                {
                    foreach (var proj in Main.projectile.Where(p => p.active && p.friendly && p.type > ProjectileID.Count && p.ModProjectile is INightmareMinion))
                    {
                        (proj.ModProjectile as INightmareMinion).GetPower(cp.nightmareEnergy);
                    }
                    cp.nightmareEnergy = 0;
                }
            }
            else
            {
                PerPartLength -= 24 / (TimeMax / 2);
            }

            for (int i = CACHE_LENGTH - 1; i > 0; i--)
                Projectile.oldRot[i] = Projectile.oldRot[i - 1];

            Projectile.oldPos[0] = Owner.Center;    //更新节点位置及旋转
            Projectile.oldRot[0] = FinalRotationOffset;

            for (int i = 1; i < CACHE_LENGTH; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i - 1] + Projectile.velocity.RotatedBy(Projectile.oldRot[i - 1]) * PerPartLength;


            Projectile.Center = Owner.Center;
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = Projectile.rotation + (OwnerDirection > 0 ? 0 : MathHelper.Pi);
            Owner.itemTime = Owner.itemAnimation = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制线条
            EdenWhip.DrawLine(Projectile.oldPos.ToList());

            Texture2D mainTex = TextureAssets.Projectile[Type].Value;

            Rectangle frame = new Rectangle(0, 0, 34, 24); // 鞭子把手的大小

            //绘制握把
            SpriteEffects effect = SpriteEffects.FlipHorizontally;
            float rot = Projectile.oldRot[0] + Projectile.rotation - MathHelper.PiOver2;
            float exRot = 0f;
            if (OwnerDirection < 0)
            {
                rot -= MathHelper.Pi;
                //exRot = MathHelper.Pi;
                effect = SpriteEffects.None;
            }
            Vector2 originCenter = Projectile.oldPos[0] - Main.screenPosition;
            Main.spriteBatch.Draw(mainTex, originCenter + Projectile.velocity * 14, frame, lightColor, rot, frame.Size() / 2, Projectile.scale, effect, 0);

            //绘制中断
            for (int i = 1; i < CACHE_LENGTH - 1; i++)
            {
                if (i % 5 == 0)
                {
                    frame.Y = 28 + 26;
                }
                else
                {
                    frame.Y = 28 + (i % 3 == 0 ? 0 : 26 * 2);
                }

                frame.Height = 22;
                Vector2 boneOrigin = frame.Size() / 2;

                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, frame, lightColor, Projectile.oldRot[i] + Projectile.rotation + -MathHelper.PiOver2, boneOrigin, Projectile.scale, effect, 0);
            }

            frame.Y = 106; // 顶端到最下面的鞭子头部的距离
            frame.Height = 40; // 鞭子头部的长度

            //绘制头部
            Main.spriteBatch.Draw(mainTex, Projectile.oldPos[CACHE_LENGTH - 2] - Main.screenPosition, frame,
                lightColor, Projectile.oldRot[CACHE_LENGTH - 1] + Projectile.rotation + exRot - MathHelper.PiOver2, frame.Size() / 2, Projectile.scale, effect, 0);

            return false;
        }
    }

    public class EdenDebuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

        public static readonly int TagDamage = 26;

        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
}
