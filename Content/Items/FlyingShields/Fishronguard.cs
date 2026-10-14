using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class Fishronguard : BaseFlyingShieldItem<FishronguardGuard>
    {
        public Fishronguard() : base(Item.sellPrice(0, 5), ItemRarityID.Yellow, AssetDirectory.FlyingShieldItems)
        { }

        private int ShootDir = 1;

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<FishronguardProj>();
            Item.knockBack = 4.5f;
            Item.shootSpeed = 28;
            Item.damage = 110;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<DukeFishronSkin>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public override void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center + new Vector2(0, -16), velocity, ModContent.ProjectileType<FishronguardShooter>(), damage, knockback, player.whoAmI, ShootDir);

            if (ShootDir == 1)
                ShootDir = -1;
            else
                ShootDir = 1;
        }
    }

    public class FishronguardShooter : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Dir => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
        }

        public override bool? CanDamage() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Player p = Main.player[Projectile.owner];
            Projectile.Center = p.Center;

            if (Projectile.ai[1] == 0)
            {
                Projectile.ai[1] = 1;
                Projectile.netUpdate = true;
                Projectile.timeLeft = p.itemTimeMax;
            }

            Projectile.ai[2]++;

            const float perAngle = 0.17f;
            float baseDir = Dir * (perAngle * 1.5f);

            if (p.itemTime == (int)(p.itemTimeMax - 1))
            {
                Projectile.NewProjectileFromThis<FishronguardProj>(Projectile.Center, Projectile.velocity.RotatedBy(baseDir), Projectile.damage, Projectile.knockBack, ai2: Dir);
            }
            if (p.itemTime == (int)(p.itemTimeMax * 0.9f))
            {
                Projectile.NewProjectileFromThis<FishronguardProj>(Projectile.Center, Projectile.velocity.RotatedBy(baseDir - Dir * perAngle), Projectile.damage, Projectile.knockBack, ai2: Dir * 2);
            }
            if (p.itemTime == (int)(p.itemTimeMax * 0.8f))
            {
                Projectile.NewProjectileFromThis<FishronguardProj>(Projectile.Center, Projectile.velocity.RotatedBy(baseDir - Dir * perAngle * 2), Projectile.damage, Projectile.knockBack, ai2: Dir * 3);
            }
            if (p.itemTime == (int)(p.itemTimeMax * 0.7f))
            {
                Projectile.NewProjectileFromThis<FishronguardProj>(Projectile.Center, Projectile.velocity.RotatedBy(baseDir - Dir * perAngle * 3), Projectile.damage, Projectile.knockBack, ai2: Dir * 4);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }

    public class FishronguardProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "Fishronguard";

        private const int AimTime = 14;
        private static int DashTime = 18;

        public ref float Rot => ref Projectile.ai[2];
        public ref float TargetIndex => ref Projectile.localAI[0];

        public override ATex GetTrailTex()
            => CoraliteAssets.Trail.CircleSPA;

        public float alpha = 1;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
            Projectile.tileCollide = false;
            TargetIndex = -1;
        }

        public override void Shooting()
        {
            JustTileCollide = false;

            if (firstShoot)
            {
                Projectile.tileCollide = true;

                Projectile.rotation = Projectile.velocity.ToRotation();
                Timer = Math.Max(0, Timer - 1);
                alpha = Helper.SqrtEase(MathHelper.Clamp(Timer / flyingTime, 0, 1));

                if (Timer == 0)
                {
                    Projectile.velocity = Vector2.Zero;
                    if (!Projectile.IsOwnedByLocalPlayer())
                        return;

                    firstShoot = false;
                    if (Helper.TryFindClosestEnemy(Projectile.Center, flyingTime * shootSpeed * 3,
                        n => n.CanBeChasedBy(), out NPC target))
                    {
                        TargetIndex = target.whoAmI;
                        State = (int)FlyingShieldStates.Shooting;
                        Timer = 0;
                        TeleportTo(target.Center);
                    }
                    else
                        TurnToBack();
                }

                return;
            }

            if (!TargetIndex.GetNPCOwner(out NPC dashTarget))
            {
                ReturnOrWaitForOwner();
                return;
            }

            Projectile.tileCollide = false;

            int aimTime = AimTime * Projectile.MaxUpdates;
            int dashTime = DashTime * Projectile.MaxUpdates;
            Timer++;
            if (Timer <= aimTime)
            {
                //停在瞬移落点，使用平方缓动逐渐显形。
                TeleportTo(dashTarget.Center);
                Projectile.rotation = (dashTarget.Center - Projectile.Center).ToRotation();

                if (Timer == aimTime)
                {
                    Projectile.velocity = (dashTarget.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * shootSpeed ;
                    Projectile.netUpdate = true;
                    Projectile.AttachToTarget(dashTarget);
                }
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();

                alpha = MathF.Sin(MathHelper.Pi * (Timer - aimTime) / dashTime);

                if (Timer >= aimTime + dashTime)
                    ReturnOrWaitForOwner();
            }
        }

        private void TeleportTo(Vector2 center)
        {
            int direction = MathF.Sign(Rot);
            Vector2 offset = new Vector2(direction * shootSpeed * DashTime * Projectile.MaxUpdates / 2, 0).RotatedBy(
                direction * (-0.24f * 1.5f + (MathF.Abs(Rot) - 1) * 0.24f));
            Projectile.Center = center + offset;
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = (-offset).ToRotation();
            Projectile.InitOldPosCache(trailCachesLength);
            Projectile.InitOldRotCache(trailCachesLength);
            Projectile.netUpdate = true;
        }

        private void ReturnOrWaitForOwner()
        {
            Projectile.velocity = Vector2.Zero;
            if (TargetIndex.GetNPCOwner(out NPC target))
                Projectile.DeattachToTarget(target);
            if (Projectile.IsOwnedByLocalPlayer())
                TurnToBack();
        }

        public override void TurnToBack()
        {
            base.TurnToBack();
            TargetIndex = -1;
            alpha = 0;
            TeleportTo(Owner.Center);
            Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * backSpeed;
        }

        public override void OnBacking()
        {
            alpha = Math.Min(alpha + 0.1f / Projectile.MaxUpdates, 1);
            base.OnBacking();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += maxPuncture * 0.06f;
            modifiers.SourceDamage += maxJump * 0.1f;
            if (canChase)
                modifiers.SourceDamage += 0.15f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //命中不打断飞行计时，仍然触发飞盾饰品的命中效果。
            UpdateShieldAccessory(accessory => accessory.OnHitNPC(this, target, hit, damageDone));
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            JustTileCollide = true;
            Projectile.TileReflect(oldVelocity, 1);
            UpdateShieldAccessory(accessory => accessory.OnTileCollide(this));

            return false;
        }

        public override void NetHeldSend(BinaryWriter writer)
        {
            base.NetHeldSend(writer);
            writer.Write((int)TargetIndex);
            writer.Write(alpha);
        }

        public override void NetHeldReceive(BinaryReader reader)
        {
            base.NetHeldReceive(reader);
            TargetIndex = reader.ReadInt32();
            alpha = reader.ReadSingle();
        }

        public override void OnShootDusts()
        {
            if (VaultUtils.isServer)
                return;

            if (firstShoot)
            {
                for (int i = 0; i < 2; i++)
                    Projectile.SpawnTrailDust(32f, Main.rand.NextBool(3) ? DustID.BlueTorch : DustID.Water, Main.rand.NextFloat(0.1f, 0.6f),
                        Scale: Main.rand.NextFloat(1f, 1.4f));
            }
            else if (Timer > AimTime * Projectile.MaxUpdates)
            {
                //if (Main.rand.NextBool())
                Projectile.SpawnTrailDust(Main.rand.NextBool(3) ? DustID.BlueTorch : DustID.Water, Main.rand.NextFloat(0.3f, 0.5f), Scale: Main.rand.NextFloat(1, 1.5f));

                if (Timer % 2 == 0)
                {
                    Color c = Color.Lerp(new Color(11, 32, 102, 200), new Color(33, 122, 166, 200), MathF.Sin((Timer - AimTime * Projectile.MaxUpdates) / (DashTime * Projectile.MaxUpdates) * MathHelper.Pi));

                    var p = PRTLoader.NewParticle<FishronguardParticle>(Projectile.Center
                        , Projectile.velocity / 5, c * alpha, 0.75f);
                    p.scale = new Vector2(0.9f, 0.75f);
                }
            }
        }

        public override void OnBackDusts()
        {
            for (int i = 0; i < 2; i++)
                Projectile.SpawnTrailDust(32f, Main.rand.NextBool(3) ? DustID.BlueTorch : DustID.Water, Main.rand.NextFloat(0.1f, 0.6f),
                    Scale: Main.rand.NextFloat(1f, 1.4f));
        }

        public override void SetOtherValues()
        {
            ShieldSlot = 1 / 4f;

            flyingTime = 14;
            backTime = 12;
            backSpeed = 17;
            trailCachesLength = 7;
            trailWidth = 24 / 2;
        }

        public override Color GetColor(float factor)
        {
            return new Color(109, 170, 174, 180) * factor * alpha;
        }

        public override void DrawSelf(Color lightColor)
        {
            base.DrawSelf(lightColor * alpha);

            if (State == (int)FlyingShieldStates.Shooting && !firstShoot)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                for (int k = -1; k < 2; k += 2)
                {
                    Texture2D Texture = GetTrailTex().Value;

                    CoraliteSystem.InitBars();

                    for (int i = 0; i < trailCachesLength; i++)
                    {
                        float factor = (float)i / trailCachesLength;
                        Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();

                        Vector2 Center = Projectile.oldPos[i] - Main.screenPosition + normal * k * 8 * Projectile.scale - Projectile.oldRot[i].ToRotationVector2() * 3 * Projectile.scale;
                        Vector2 Top = Center + (normal * 5 * factor);
                        Vector2 Bottom = Center - (normal * 5 * factor);

                        var c = Color.Yellow with { A = 0 } * alpha;
                        CoraliteSystem.Vertexes.Add(new(Top, c, new Vector3(factor, 0, 1)));
                        CoraliteSystem.Vertexes.Add(new(Bottom, c, new Vector3(factor, 1, 1)));
                    }

                    Main.graphics.GraphicsDevice.Textures[0] = Texture;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, CoraliteSystem.Vertexes.ToArray(), 0, CoraliteSystem.Vertexes.Count - 2);
                }
            }
        }
    }

    public class FishronguardParticle : Particle
    {
        public override string Texture => AssetDirectory.Particles + "LightShotSPA";

        public Vector2 scale = Vector2.One;

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            Opacity++;

            Color *= 0.8f;
            if (Color.A < 10 || Opacity > 20)
                active = false;

            if (Opacity < 3)
                scale.Y *= 1.05f;
            else
                scale.Y *= 0.93f;

            scale.X *= 1.01f;
            Scale *= 0.98f;

            Rotation = Velocity.ToRotation();

            Position += Velocity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = TexValue;

            Vector2 position = Position - Main.screenPosition;
            Vector2 origin = tex.Size() / 2;
            spriteBatch.Draw(tex, position, null, Color, Rotation, origin, Scale * scale, 0, 0);

            return false;
        }
    }

    public class FishronguardGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 50;
            Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.2f;
            distanceAdder = 3;
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 3;
            SoundEngine.PlaySound(CoraliteSoundID.DukeFishron_NPCHit14, Projectile.Center);

            for (int i = 0; i < 4; i++)
            {

            }
        }

        public override float GetWidth()
        {
            return Projectile.width / 2;
        }

        public override void DrawSelf(Texture2D mainTex, Vector2 pos, float rotation, Color lightColor, Vector2 scale, SpriteEffects effect)
        {
            Rectangle frameBox;
            Vector2 rotDir = Projectile.rotation.ToRotationVector2();
            Vector2 dir = rotDir * (DistanceToOwner / (Projectile.width * scalePercent));
            Color c = lightColor * 0.7f;
            c.A = lightColor.A;
            Color c2 = lightColor * 0.5f;
            c2.A = lightColor.A;

            frameBox = mainTex.Frame(3, 1, 0, 0);
            Vector2 origin2 = frameBox.Size() / 2;

            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - (dir * 5), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(3, 1, 1, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 8), frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 12), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 17), frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制上上部
            frameBox = mainTex.Frame(3, 1, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 22), frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 30), frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos + (dir * 38), frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }
    }
}
