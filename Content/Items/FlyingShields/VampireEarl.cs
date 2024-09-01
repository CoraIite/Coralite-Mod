using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class VampireEarl : BaseFlyingShieldItem<VampireEarlGuard>
    {
        public VampireEarl() : base(Item.sellPrice(0, 15), ItemRarityID.Red, AssetDirectory.FlyingShieldItems)
        { }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 14;
            Item.shoot = ModContent.ProjectileType<VampireEarlProj>();
            Item.knockBack = 6.5f;
            Item.shootSpeed = 20 / 2;
            Item.damage = 115;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VampiresFang>()
                .AddIngredient(ItemID.LunarBar, 4)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }

    public class VampireEarlProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "VampireEarl";

        private int shootDelay;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 60;
            Projectile.extraUpdates = 1;
        }

        public override void SetOtherValues()
        {
            flyingTime = 22 * 2;
            backTime = 22 * 2;
            backSpeed = 30 / 2;
            trailCachesLength = 16;
            trailWidth = 30 / 2;

            if (Main.bloodMoon)
            {
                flyingTime += 10;
                backTime -= 6;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            shootDelay = flyingTime / Main.rand.Next(2, 5);
            if (Main.bloodMoon)
                shootDelay = flyingTime / Main.rand.Next(3, 6);
        }

        public override void OnShootDusts()
        {
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() / 2);
            Projectile.SpawnTrailDust(20f, DustID.CrimsonTorch, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(1, 1.5f));

            if (Timer != 0 && Timer % shootDelay == 0)
            {
                //射蝙蝠
                if (Helper.TryFindClosestEnemy(Projectile.Center, 600, n => n.CanBeChasedBy(), out _))
                    Projectile.NewProjectileFromThis<VampireEarlBat>(Projectile.Center
                    , Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(3.141f + Main.rand.NextFloat(-0.8f, 0.8f)) * Main.rand.NextFloat(3f, 12f),
                    (int)(Projectile.damage * 0.8f), Projectile.knockBack);
            }
        }

        public override void OnBackDusts()
        {
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() / 2);
            Projectile.SpawnTrailDust(20f, DustID.CrimsonTorch, -Main.rand.NextFloat(0.1f, 0.4f), Scale: Main.rand.NextFloat(1f, 1.5f));
        }

        public override Color GetColor(float factor)
        {
            return Color.Red * factor;
        }

        public override void DrawSelf(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            var origin = mainTex.Size() / 2;

            Color c = Color.Red;
            c.A = 0;

            for (int i = trailCachesLength - 1; i > 10; i--)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                c * 0.5f * ((trailCachesLength - i) * 1f / (trailCachesLength - 10)), Projectile.oldRot[i] - 1.57f + extraRotation, origin, Projectile.scale * (i * 1f / trailCachesLength), 0, 0);

            Main.spriteBatch.Draw(mainTex, pos, null, c * 0.3f, Projectile.rotation - 1.57f + extraRotation, origin, Projectile.scale * 1.15f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, Projectile.rotation - 1.57f + extraRotation, origin, Projectile.scale, 0, 0);
        }
    }

    public class VampireEarlGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 70;
            Projectile.height = 62;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.35f;
            scalePercent = 1.4f;
            distanceAdder = 2;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.Fleshy_NPCHit1, Projectile.Center);

            if (!Owner.moonLeech)
            {
                float num = Projectile.damage * 0.025f;
                if (num > 5)
                    num = 5;
                if ((int)num != 0 && !(Owner.lifeSteal <= 0f))
                {
                    Owner.lifeSteal -= num * 1.5f;
                    int num2 = Projectile.owner;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, 305, 0, 0f, Projectile.owner, num2, num);
                }
            }
            //int num4 = Projectile.NewProjectileFromThis(Owner.Center, Vector2.Zero, 608, Projectile.damage, 15f);
            //Main.projectile[num4].netUpdate = true;
            //Main.projectile[num4].Kill();
        }

        public override float GetWidth()
        {
            return Projectile.width * 0.4f / Projectile.scale;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.6f;
            c.A = lightColor.A;

            frameBox = mainTex.Frame(3, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - (dir * 4), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 10), frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上上部
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 12), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 17), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }

    public class VampireEarlBat : ModProjectile
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public ref float Target => ref Projectile.ai[0];
        public ref float Backing => ref Projectile.ai[1];
        public Player Owner => Main.player[Projectile.owner];

        private bool init = true;
        private int[] oldFrame;
        private int[] oldDirection;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.scale = 0.75f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
            if (Backing > 5)
                return false;
            return base.CanDamage();
        }

        public override void AI()
        {
            const int trailCacheLength = 6;
            if (init)
            {
                init = false;
                Projectile.InitOldPosCache(trailCacheLength);
                Projectile.InitOldRotCache(trailCacheLength);
                oldFrame = new int[trailCacheLength];
                oldDirection = new int[trailCacheLength];

                for (int i = 0; i < trailCacheLength; i++)
                    oldFrame[i] = Projectile.frame;
                for (int i = 0; i < trailCacheLength; i++)
                    oldDirection[i] = Projectile.spriteDirection;
                Target = -1;
                Projectile.scale = Main.rand.NextFloat(0.65f, 0.8f);
            }

            float targetRot = Projectile.velocity.Length() * 0.01f * Projectile.spriteDirection;
            Projectile.rotation = Projectile.rotation.AngleLerp(targetRot, 0.1f);

            Projectile.UpdateFrameNormally(6, 3);
            Projectile.UpdateOldPosCache();
            Projectile.UpdateOldRotCache();

            for (int i = 0; i < oldFrame.Length - 1; i++)
                oldFrame[i] = oldFrame[i + 1];
            oldFrame[^1] = Projectile.frame;
            for (int i = 0; i < oldDirection.Length - 1; i++)
                oldDirection[i] = oldDirection[i + 1];
            oldDirection[^1] = Projectile.spriteDirection;

            if (Backing > 5)
            {
                Projectile.alpha -= 3;
                Projectile.tileCollide = false;
                Projectile.direction = Projectile.Center.X > Owner.Center.X ? -1 : 1;
                Projectile.spriteDirection = Projectile.direction;
                int directionY = Owner.Center.Y > Projectile.Center.Y ? 1 : -1;

                float xLength = Math.Abs(Owner.Center.X - Projectile.Center.X);
                if (xLength > 20)
                    Helper.Movement_SimpleOneLine(ref Projectile.velocity.X, Projectile.direction, 30f, 0.6f, 2f, 0.95f);
                else
                    Projectile.velocity.X *= 0.96f;
                //控制Y方向的移动
                float yLength = Math.Abs(Owner.Center.Y - Projectile.Center.Y);
                if (yLength > 20)
                    Helper.Movement_SimpleOneLine(ref Projectile.velocity.Y, directionY, 25f, 0.6f, 2f, 0.95f);
                else
                    Projectile.velocity.Y *= 0.96f;

                if (Vector2.Distance(Projectile.Center, Owner.Center) < 30 || Projectile.alpha < 2)
                    Projectile.Kill();
                return;
            }

            if (!Target.GetNPCOwner(out NPC target))
            {
                if (Main.rand.NextBool(6))
                {
                    if (Helper.TryFindClosestEnemy(Projectile.Center, 700, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC npc))
                        Target = npc.whoAmI;
                    else
                        Backing++;
                }

                Projectile.direction = Math.Sign(Projectile.velocity.X);
                return;
            }

            if (Math.Abs(target.Center.X - Projectile.Center.X) < 16)
                return;
            {
                Projectile.direction = Projectile.Center.X > target.Center.X ? -1 : 1;
                Projectile.spriteDirection = Projectile.direction;
                int directionY = target.Center.Y > Projectile.Center.Y ? 1 : -1;

                float xLength = Math.Abs(target.Center.X - Projectile.Center.X);
                if (xLength > 20)
                    Helper.Movement_SimpleOneLine(ref Projectile.velocity.X, Projectile.direction, 15f, 0.45f, 0.9f, 0.92f);
                else
                    Projectile.velocity.X *= 0.96f;
                //控制Y方向的移动
                float yLength = Math.Abs(target.Center.Y - Projectile.Center.Y);
                if (yLength > 20)
                    Helper.Movement_SimpleOneLine(ref Projectile.velocity.Y, directionY, 15f, 0.45f, 0.9f, 0.92f);
                else
                    Projectile.velocity.Y *= 0.96f;

                if (Main.rand.NextBool(6))
                {
                    Projectile.SpawnTrailDust(10f, DustID.CrimsonTorch, -Main.rand.NextFloat(0.1f, 0.3f), 100, Scale: Main.rand.NextFloat(1f, 1.5f));
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!Owner.moonLeech && !target.immortal && Main.rand.NextBool(2, 10))
            {
                float num = damageDone * 0.025f;
                if ((int)num != 0 && !(Owner.lifeSteal <= 0f))
                {
                    Owner.lifeSteal -= num * 1.5f;
                    int num2 = Projectile.owner;
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center, Vector2.Zero, 305, 0, 0f, Projectile.owner, num2, num);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(30, 30);

                for (int j = 0; j < 4; j++)
                {
                    Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(2, 2), DustID.VampireHeal,
                         Helper.NextVec2Dir(0.1f, 0.5f), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }
            }

            Projectile.alpha = 125;
            Backing = 6;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);

            if (oldVelX > newVelX)
                Projectile.velocity.X = -Math.Sign(oldVelocity.X) * Math.Clamp(oldVelX * 1.5f, 0, 8);
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -Math.Sign(oldVelocity.Y) * Math.Clamp(oldVelY * 1.5f, 0, 8);

            Projectile.position -= oldVelocity;
            Backing++;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor *= Projectile.alpha / 255f;

            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(1, 4, 0, Projectile.frame);
            var origin = frameBox.Size() / 2;
            var pos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation;

            SpriteEffects effects = SpriteEffects.None;

            if (Projectile.spriteDirection < 0)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            const int trailCacheLength = 6;
            Color shadowColor = Color.Red;
            shadowColor.A = 50;
            shadowColor *= Projectile.alpha / 255f;
            if (oldFrame != null && oldDirection != null)
            {
                for (int i = 0; i < trailCacheLength; i++)
                {
                    Vector2 oldPos = Projectile.oldPos[i] - Main.screenPosition;
                    float oldrot = Projectile.oldRot[i];
                    var frameOld = mainTex.Frame(1, 4, 0, oldFrame[i]);
                    float factor = (float)i / trailCacheLength;

                    SpriteEffects oldEffect = oldDirection[i] > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
                    Main.spriteBatch.Draw(mainTex, oldPos, frameOld, shadowColor * factor, oldrot, origin
                         , Projectile.scale * (1 - ((1 - factor) * 0.3f)), oldEffect, 0);
                }
            }

            Main.spriteBatch.Draw(mainTex, pos, frameBox, shadowColor, rot, origin, Projectile.scale * 1.2f, effects, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rot, origin, Projectile.scale, effects, 0);

            return false;
        }
    }
}
