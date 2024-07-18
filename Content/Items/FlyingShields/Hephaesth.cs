using Coralite.Content.Items.RedJades;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    public class Hephaesth : BaseFlyingShieldItem<HephaesthGuard>, IInventoryCraftStation
    {
        public Hephaesth() : base(Item.sellPrice(0, 40), ItemRarityID.Red, AssetDirectory.FlyingShieldItems)
        { }

        /// <summary>
        /// 燃料
        /// </summary>
        public int Fuel;
        /// <summary>
        /// 爆燃状态持续时间
        /// </summary>
        public int BurstTime;

        public bool GetFuel(int fuel)
        {
            Fuel += fuel;
            Fuel = Math.Clamp(Fuel, 0, 30 * 30);
            if (Fuel > 30 * 25)
            {
                BurstTime = Fuel;
                Fuel = 0;
                return true;
            }

            return false;
        }

        public override void SetDefaults2()
        {
            Item.useTime = Item.useAnimation = 20;
            Item.shoot = ModContent.ProjectileType<HephaesthProj>();
            Item.knockBack = 2;
            Item.shootSpeed = 22 / 2;
            Item.damage = 225;
            Item.UseSound = CoraliteSoundID.Zenith_Item169;
        }

        public override void HoldItem(Player player)
        {
            if (BurstTime > 0)
            {
                BurstTime--;
                return;
            }
        }

        public override bool CanUseItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.MaxFlyingShield += 2;
            }

            return base.CanUseItem(player);
        }

        public override void UpdateInventory(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.inventoryCraftStations.Add(this);
        }

        public void AdjTiles(Player player)
        {
            //player.adjTile[TileID.Furnaces] = true;
            //player.adjTile[TileID.Hellforge] = true;
            //player.adjTile[TileID.AdamantiteForge] = true;
            //player.adjTile[TileID.LunarCraftingStation] = true;
            TileLoader.AdjTiles(player, ModContent.TileType<AncientFurnaceTile>());
            player.adjTile[ModContent.TileType<AncientFurnaceTile>()] = true;
        }

        public override void LeftShoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            if (BurstTime < 1)
                GetFuel(30);
            else
                damage = (int)(1.5f * damage);

            Projectile.NewProjectile(source, player.Center + new Vector2(0, -16), velocity, type
                , damage, knockback, player.whoAmI, ai2: BurstTime > 0 ? 1 : 0);
        }

        public override void RightShoot(Player player, IEntitySource source, int damage)
        {
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<HephaesthGuard>()
                , (int)(damage * 0.9f), 6, player.whoAmI, ai2: BurstTime > 0 ? 1 : 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientFurnace>()
                .AddIngredient<TrashCanLid>()
                .AddIngredient<RedJadeShield>()
                .AddIngredient<GoldenSamurai>()
                .AddIngredient<GemrainAegis>()
                .AddIngredient<MechRioter>()
                .AddIngredient<TortoiseshellFortress>()
                .AddIngredient<Fishronguard>()
                .AddIngredient<ShanHai>()
                .AddIngredient<Solleonis>()
                .AddIngredient<ConquerorOfTheSeas>()
                .AddIngredient<Noctiflair>()
                .Register();
        }
    }

    public class HephaesthProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "HephaesthProj";

        public bool Burning => Projectile.ai[2] == 1;

        private Color trailColor = Color.Black;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 54;
            Projectile.extraUpdates = 1;
        }

        public override void SetOtherValues()
        {
            flyingTime = 26 * 2;
            backTime = 18 * 2;
            backSpeed = 24 / 2;
            trailCachesLength = 8 * 2;
            trailWidth = 20 / 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State == (int)FlyingShieldStates.Shooting)
            {
                if (Timer > flyingTime / 2)
                {
                    Projectile.NewProjectileFromThis<HephaesthBurst>(target.Center + Main.rand.NextVector2Circular(target.width / 2, target.height / 2),
                        Vector2.Zero, (int)(Projectile.damage * 1.2f), Projectile.knockBack, ai2: Projectile.ai[2]);
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnShootDusts()
        {
            if (Timer == flyingTime / 2)
                SpawnShield(12);

            if (Burning && Timer == 35 * flyingTime / 100)
                SpawnShield(10);

            float factor = 1 - Timer / flyingTime;
            trailColor = ColorFunc(factor);
            if (Main.rand.NextBool())
                Projectile.SpawnTrailDust((float)(Projectile.width / 3), DustID.RainbowMk2, Main.rand.NextFloat(0.2f, 0.4f), newColor: trailColor);
        }

        public void SpawnShield(float speed)
        {
            for (int i = 0; i < 7; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center
                    , DustID.PortalBoltTrail, Helper.NextVec2Dir(2f, 9f), newColor: trailColor, Scale: Main.rand.NextFloat(1, 1.5f));
                dust.noGravity = true;
            }
            Vector2 dir = Projectile.rotation.ToRotationVector2().RotateByRandom(-0.3f, 0.3f);
            int type = -Main.rand.Next(1, 17);
            if (CoraliteWorld.chaosWorld)
                type = Main.rand.Next(ItemLoader.ItemCount);

            int index = Projectile.NewProjectileFromThis<HephaesthSmeltingResults>(Projectile.Center, dir * speed,
                  (int)(Projectile.damage * 1.2f), Projectile.knockBack, ai2: type);//Main.rand.Next(1, ItemLoader.ItemCount));
            if (Burning)
            {
                (Main.projectile[index].ModProjectile as HephaesthSmeltingResults).DefaultColor = new Color(89, 219, 255);
            }
        }

        public override Color GetColor(float factor)
        {
            if (State == (int)FlyingShieldStates.Shooting)
            {
                return trailColor * factor;
            }
            return new Color(86, 80, 112) * factor;
        }

        public Color ColorFunc(float factor)
        {
            factor *= 6;

            if (Burning)
            {
                if (factor < 1)
                    return Color.Lerp(new Color(86, 80, 112), new Color(52, 29, 114), factor);
                else if (factor < 2)
                    return Color.Lerp(new Color(52, 29, 114), new Color(83, 129, 255), factor - 1);
                else if (factor < 3)
                    return Color.Lerp(new Color(83, 129, 255), new Color(89, 219, 255), factor - 2);
                else
                    return new Color(89, 219, 255);
            }

            if (factor < 1)
                return Color.Lerp(new Color(86, 80, 112), new Color(155, 26, 10), factor);
            else if (factor < 2)
                return Color.Lerp(new Color(155, 26, 10), new Color(255, 174, 33), factor - 1);
            else if (factor < 3)
                return Color.Lerp(new Color(255, 174, 33), new Color(255, 237, 134), factor - 2);
            else
                return new Color(255, 237, 134);
        }

        public override void DrawSelf(Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frame = mainTex.Frame(4, 2, 0, Burning ? 1 : 0);
            var pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(mainTex, pos, frame, lightColor, Projectile.rotation - 1.57f + extraRotation, frame.Size() / 2, Projectile.scale, 0, 0);
        }
    }

    public class HephaesthGuard : BaseFlyingShieldGuard
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + "HephaesthProj";

        public bool Burning => Projectile.ai[2] == 1;

        int turnToBuring;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 58;
            Projectile.height = 64;
            Projectile.scale = 1.3f;
            Projectile.localNPCHitCooldown = 24;
        }

        public override void SetOtherValues()
        {
            damageReduce = 0.4f;
            distanceAdder = 2.6f;
            strongGuard = 0.3f;
            scalePercent = 1.4f;
        }

        public override void OnHoldShield()
        {
            if (Burning && turnToBuring == 0 && Owner.HeldItem.ModItem is Hephaesth hephaesth)
            {
                if (hephaesth.BurstTime == 0)
                {
                    turnToBuring = 30;
                }
            }

            if (turnToBuring > 0)
            {
                if (turnToBuring == 15)
                {
                    if (Projectile.ai[2] == 1)
                        Projectile.ai[2] = 0;
                    else
                        Projectile.ai[2] = 1;

                    SoundEngine.PlaySound(CoraliteSoundID.FireBallExplosion_Item74, Projectile.Center);
                }

                int type = Burning ? DustID.BlueTorch : DustID.Torch;
                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustPerfect(Projectile.Center, type
                         , (Projectile.rotation + i * MathHelper.Pi + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2() * Main.rand.NextFloat(2f, 8)
                         , Scale: Main.rand.NextFloat(1.5f, 2.5f));
                    d.noGravity = true;
                }

                turnToBuring--;
            }

            if (Burning && Main.rand.NextBool(3))
            {
                int type = DustID.BlueTorch;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 6), type
                     , -Vector2.UnitY.RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(2f, 8f)
                     , Scale: Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
            }
        }

        public override void OnGuard()
        {
            base.OnGuard();
            if (Owner.HeldItem.ModItem is Hephaesth hephaesth)
            {
                if (!Burning && hephaesth.GetFuel(30 * 2))
                {
                    //特效
                    turnToBuring = 30;
                }
            }

            Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
            for (int i = -2; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, dir * i * 0.8f, 100, newColor: Burning ? new Color(89, 219, 255) : new Color(255, 237, 134),
                     Scale: Main.rand.NextFloat(1.3f, 1.5f));
                d.noGravity = true;
            }

            //Helper.SpawnDirDustJet(Projectile.Center, () => (Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f)).ToRotationVector2(),
            //    2, 5, i => 2 + i * 0.8f, DustID.RainbowMk2, 100, Burning ? new Color(89, 219, 255) : new Color(255, 237, 134),
            //    1.2f);

            //生成弹幕
            Projectile.NewProjectileFromThis<HephaesthFire>(Projectile.Center, (Projectile.rotation + Main.rand.NextFloat(-0.08f, 0.08f)).ToRotationVector2() * Main.rand.NextFloat(14, 18),
                Projectile.damage, Projectile.knockBack, ai2: Projectile.ai[2]);

            if (Main.rand.NextBool())
                Projectile.NewProjectileFromThis<HephaesthFire>(Projectile.Center, (Projectile.rotation + Main.rand.NextFloat(-0.08f, 0.08f)).ToRotationVector2() * Main.rand.NextFloat(14, 18),
                            Projectile.damage, Projectile.knockBack, ai2: Projectile.ai[2]);

            if (Main.rand.NextBool(4))
                Projectile.NewProjectileFromThis<HephaesthFire>(Projectile.Center, (Projectile.rotation + Main.rand.NextFloat(-0.08f, 0.08f)).ToRotationVector2() * Main.rand.NextFloat(14, 18),
                            Projectile.damage, Projectile.knockBack, ai2: Projectile.ai[2]);
        }

        public override void OnStrongGuard()
        {
            SoundStyle st = CoraliteSoundID.NoUse_SuperMagicShoot_Item68;
            st.Pitch = -0.5f;
            SoundEngine.PlaySound(st, Projectile.Center);
            if (Owner.HeldItem.ModItem is Hephaesth hephaesth)
            {
                if (!Burning && hephaesth.GetFuel(30 * 8))
                {
                    //特效
                    turnToBuring = 30;
                }
            }
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

            Vector2 pos1 = pos;
            Vector2 pos2 = pos;

            if (turnToBuring > 0)
            {
                float factor = MathF.Sin(MathHelper.Pi * turnToBuring / 30f);
                pos1 += (Projectile.rotation - Owner.direction * 0.7f * factor).ToRotationVector2() * factor * 12;
                pos2 += (Projectile.rotation + Owner.direction * 0.7f * factor).ToRotationVector2() * factor * 12;
            }

            if (Burning)
            {
                frameBox = mainTex.Frame(4, 2, 1, 1);
                Vector2 origin1 = frameBox.Size() / 2;
                //绘制基底
                Main.spriteBatch.Draw(mainTex, pos - dir * 10, frameBox, c2, rotation, origin1, scale, effect, 0);
                Main.spriteBatch.Draw(mainTex, pos - dir * 5, frameBox, c, rotation, origin1, scale, effect, 0);
                Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.White, rotation, origin1, scale, effect, 0);

                //绘制上部
                frameBox = mainTex.Frame(4, 2, 2, 1);
                Main.spriteBatch.Draw(mainTex, pos2 + dir * 6, frameBox, c2, rotation, origin1, scale, effect, 0);
                Main.spriteBatch.Draw(mainTex, pos2 + dir * 11, frameBox, c, rotation, origin1, scale, effect, 0);
                Main.spriteBatch.Draw(mainTex, pos2 + dir * 16, frameBox, lightColor, rotation, origin1, scale, effect, 0);

                //绘制下部
                frameBox = mainTex.Frame(4, 2, 3, 1);
                Main.spriteBatch.Draw(mainTex, pos1 - dir * 2, frameBox, c2, rotation, origin1, scale, effect, 0);
                Main.spriteBatch.Draw(mainTex, pos1 + dir * 3, frameBox, c, rotation, origin1, scale, effect, 0);
                Main.spriteBatch.Draw(mainTex, pos1 + dir * 8, frameBox, lightColor, rotation, origin1, scale, effect, 0);

                return;
            }

            frameBox = mainTex.Frame(4, 2, 1, 0);
            Vector2 origin2 = frameBox.Size() / 2;
            //绘制基底
            Main.spriteBatch.Draw(mainTex, pos - dir * 10, frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos - dir * 5, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, Color.White, rotation, origin2, scale, effect, 0);

            //绘制上部
            frameBox = mainTex.Frame(4, 2, 2, 0);
            Main.spriteBatch.Draw(mainTex, pos2 + dir * 6, frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos2 + dir * 11, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos2 + dir * 16, frameBox, lightColor, rotation, origin2, scale, effect, 0);

            //绘制下部
            frameBox = mainTex.Frame(4, 2, 3, 0);
            Main.spriteBatch.Draw(mainTex, pos1 - dir * 2, frameBox, c2, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos1 + dir * 3, frameBox, c, rotation, origin2, scale, effect, 0);
            Main.spriteBatch.Draw(mainTex, pos1 + dir * 8, frameBox, lightColor, rotation, origin2, scale, effect, 0);
        }

        public override float GetWidth()
        {
            return Projectile.width * 0.45f / Projectile.scale;
        }
    }

    public class HephaesthFire : ModProjectile, IDrawAdditive, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Blank;

        ref float State => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];
        ref float FlyingTime => ref Projectile.localAI[2];
        public bool Burning => Projectile.ai[2] == 1;

        private ParticleGroup fireParticles;
        private Trail trail;
        private readonly int trailPoint = 24;

        private int chaseTime;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 32;
            Projectile.tileCollide = true;
            Projectile.penetrate = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override bool? CanDamage()
        {
            if (State > 1)
            {
                return false;
            }
            return base.CanDamage();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.tileCollide = false;
            Projectile.velocity *= 0;
            State = 2;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            chaseTime = 0;
            if (Projectile.penetrate < 3)
            {
                State = 2;
                Projectile.velocity *= 0;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.InitOldPosCache(trailPoint);
            Projectile.localAI[1] = Main.rand.NextFloat(-0.01f, 0.01f);
            if (Burning)
                FlyingTime = 20 * 5;
            else
                FlyingTime = 20;
        }

        public override void AI()
        {
            fireParticles ??= new ParticleGroup();
            trail ??= new Trail(Main.instance.GraphicsDevice, trailPoint, new NoTip(), factor =>
            {
                if (factor < 0.8f)
                    return Helper.Lerp(6, 12, factor / 0.8f);

                return Helper.Lerp(12, 0, (factor - 0.8f) / 0.2f);
            }, Burning ? ColorFunc2 : ColorFunc1);

            switch (State)
            {
                default:
                case 0://下落
                    {
                        Lighting.AddLight(Projectile.Center, new Vector3(0.5f));
                        if (Burning && Helper.TryFindClosestEnemy(Projectile.Center, 1000, n => n.CanBeChasedBy() && Projectile.localNPCImmunity.IndexInRange(n.whoAmI) && Projectile.localNPCImmunity[n.whoAmI] == 0, out NPC target))
                        {
                            float selfAngle = Projectile.velocity.ToRotation();
                            float targetAngle = (target.Center - Projectile.Center).ToRotation();

                            Projectile.velocity = selfAngle.AngleLerp(targetAngle, 0.2f + 0.8f * Math.Clamp((chaseTime - 20) / 20, 0, 1f)).ToRotationVector2() * Projectile.velocity.Length();
                        }

                        SpawnDusts(1 - 0.3f * Timer / FlyingTime);

                        Timer++;
                        chaseTime++;

                        if (Timer > FlyingTime)
                        {
                            State = 1;
                            Timer = 0;
                        }
                    }
                    break;
                case 1:
                    {
                        Projectile.localAI[0] += Projectile.localAI[1];
                        Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.localAI[0]);
                        Projectile.velocity *= 0.97f;

                        SpawnDusts(0.7f);

                        Lighting.AddLight(Projectile.Center, new Vector3(0.5f));
                        Timer++;
                        if (Timer > 15)
                        {
                            Projectile.velocity *= 0;
                            State++;
                        }
                    }
                    break;
                case 2://他紫砂了
                    {
                        if (!fireParticles.Any())
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    break;
            }

            Projectile.UpdateOldPosCache();
            trail.Positions = Projectile.oldPos;
            fireParticles.UpdateParticles();
        }

        public static Color ColorFunc1(Vector2 factor)
        {
            if (factor.X < 0.7f)
            {
                return Color.Lerp(new Color(0, 0, 0, 0), new Color(255, 108, 31), factor.X / 0.7f);
            }

            return Color.Lerp(new Color(255, 108, 31), new Color(255, 174, 33), (factor.X - 0.7f) / 0.3f);
        }

        public static Color ColorFunc2(Vector2 factor)
        {
            if (factor.X < 0.7f)
            {
                return Color.Lerp(new Color(0, 0, 0, 0), new Color(83, 129, 255), factor.X / 0.7f);
            }

            return Color.Lerp(new Color(83, 129, 255), new Color(89, 219, 255), (factor.X - 0.7f) / 0.3f);
        }

        public void SpawnDusts(float factor)
        {
            Color c;
            int type;
            if (Burning)
            {
                type = DustID.BlueTorch;
                c = Main.rand.Next(3) switch
                {
                    0 => new Color(89, 219, 255),
                    1 => new Color(83, 129, 255),
                    _ => new Color(77, 69, 181),
                };
            }
            else
            {
                type = DustID.Torch;
                c = Main.rand.Next(3) switch
                {
                    0 => new Color(255, 174, 33),
                    1 => new Color(255, 108, 31),
                    _ => new Color(222, 53, 10),
                };
            }

            Projectile.SpawnTrailDust(type, Main.rand.NextFloat(0.2f, 0.6f), Scale: Main.rand.NextFloat(1f, 2f));

            float angle = Projectile.velocity.AngleFrom(Projectile.oldVelocity);
            float rate = MathHelper.Clamp(0.4f - Math.Abs(angle) / 5, 0, 0.4f);
            fireParticles.NewParticle(Projectile.Center + Main.rand.NextVector2Circular(8, 8),
                (Projectile.velocity * factor * Main.rand.NextFloat(rate * 0.7f, rate * 1.3f + 0.001f)).RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f)),
                CoraliteContent.ParticleType<FireParticle>(), c, Main.rand.NextFloat(0.4f, 0.8f));

        }

        public override bool PreDraw(ref Color lightColor)
        {
            //var a= Main.graphics.GraphicsDevice.BlendState;
            // fireParticles?.DrawParticles(Main.spriteBatch);
            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            fireParticles?.DrawParticles(Main.spriteBatch);
        }

        public void DrawPrimitives()
        {
            if (State == 2 || trail == null)
                return;

            Effect effect = Filters.Scene["Flow2"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 5);
            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["uTextImage"].SetValue(ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "ExtraLaserFlow").Value);

            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;

            trail.Render(effect);

            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }

    public class HephaesthSmeltingResults : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.Blank;
        public override string TrailTexture => AssetDirectory.OtherProjectiles + "SpurtTrail";

        ref float ShieldType => ref Projectile.ai[2];

        public Color DefaultColor = Color.Gold;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 24;
            Projectile.extraUpdates = 2;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = DamageClass.Generic;
        }

        public override void OnShootDusts()
        {
            SpawnDust();
        }

        public override void OnBackDusts()
        {
            SpawnDust();
        }

        public void SpawnDust()
        {
            if (Main.rand.NextBool(10))
            {
                float a = 1;
                Color c = GetColor(ref a);
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.width)
                    , DustID.PortalBoltTrail, Vector2.Zero, newColor: c);
                dust.noGravity = true;
                dust.velocity = Projectile.velocity.RotateByRandom(-0.5f, 0.5f) * Main.rand.NextFloat(0.3f, 0.7f);
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            trailWidth = 24 / 2;
            shootSpeed = Projectile.velocity.Length();

            flyingTime = 12 * 3;
            backTime = 14 * 3;
            backSpeed = 48 / 3;
            trailCachesLength = 10;
            canChase = true;
            maxJump += 2;

            UpdateShieldAccessory(accessory => accessory.OnInitialize(this));
            UpdateShieldAccessory(accessory => accessory.PostInitialize(this));

            Timer = flyingTime;

            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * shootSpeed;
            Projectile.oldPos = new Vector2[trailCachesLength];
            Projectile.oldRot = new float[trailCachesLength];
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < trailCachesLength; i++)
            {
                Projectile.oldPos[i] = Projectile.Center;
                Projectile.oldRot[i] = Projectile.rotation;
            }
            State = (int)FlyingShieldStates.Shooting;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            State = (int)FlyingShieldStates.JustHited;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State != (int)FlyingShieldStates.Backing)
                State = (int)FlyingShieldStates.JustHited;

            //for (int i = 0; i < 3; i++)
            //{
            //    //Dust.NewDustPerfect(Projectile.Center,DustID.)
            //}
        }

        public override void OnJustHited()
        {
            jumpCount++;
            if (jumpCount > maxJump)
                TurnToBack();
            else
                JumpInNpcs();
        }

        public override void TurnToBack()
        {
            State = (int)FlyingShieldStates.Backing;
            Timer = 0;
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            Projectile.tileCollide = false;
        }

        public override void Chasing()
        {
            if (canChase)
                if (Helper.TryFindClosestEnemy(Projectile.Center, Timer * shootSpeed + Projectile.width * 4, n => n.CanBeChasedBy() && Projectile.localNPCImmunity.IndexInRange(n.whoAmI) && Projectile.localNPCImmunity[n.whoAmI] == 0, out NPC target))
                {
                    float selfAngle = Projectile.velocity.ToRotation();
                    float targetAngle = (target.Center - Projectile.Center).ToRotation();

                    Projectile.velocity = selfAngle.AngleLerp(targetAngle, 0.2f * (1 - Timer / flyingTime)).ToRotationVector2() * shootSpeed;
                }
        }

        public override void OnBacking()
        {
            base.OnBacking();
            if (Timer > 240)
                Projectile.Kill();
        }

        public static int GetTextureType(int index)
        {
            return index switch
            {
                -1 => ModContent.ItemType<TrashCanLid>(),
                -2 => ModContent.ItemType<RedJadeShield>(),
                -3 => ModContent.ItemType<GoldenSamurai>(),
                -4 => ModContent.ItemType<GlassShield>(),
                -5 => ModContent.ItemType<GlazeBulwark>(),
                -6 => ModContent.ItemType<GemrainAegis>(),
                -7 => ModContent.ItemType<MechRioter>(),
                -8 => ModContent.ItemType<TortoiseshellFortress>(),
                -9 => ModContent.ItemType<Fishronguard>(),
                -10 => ModContent.ItemType<ShanHai>(),
                -11 => ModContent.ItemType<Leonids>(),
                -12 => ModContent.ItemType<Solleonis>(),
                -13 => ModContent.ItemType<ConquerorOfTheSeas>(),
                -14 => ModContent.ItemType<SilverAngel>(),
                -15 => ModContent.ItemType<RoyalAngel>(),
                -16 => ModContent.ItemType<Noctiflair>(),
                _ => index,
            };
        }

        public Color GetColor(ref float factor)
        {
            Color c;
            if (State == (int)FlyingShieldStates.Shooting)
                factor *= Math.Clamp((flyingTime - Timer) / 12, 0, 1);
            c = (int)ShieldType switch
            {
                -1 => Color.DarkGray,
                -2 => Coralite.Instance.RedJadeRed,
                -3 => new Color(238, 202, 158),
                -4 => Color.Gray,
                -5 => Color.Gray,
                -6 => GemrainColor(factor),
                -7 => Color.DarkGray,
                -8 => new Color(114, 84, 66),
                -9 => new Color(39, 100, 104),
                -10 => new Color(28, 90, 144),
                -11 => new Color(32, 180, 186),
                -12 => new Color(255, 174, 33),
                -13 => new Color(122, 122, 156),
                -14 => new Color(77, 69, 181),
                -15 => Color.Gold,
                -16 => new Color(83, 129, 155),
                _ => DefaultColor,
            };
            return c;
        }

        private static Color GemrainColor(float factor)
        {
            float x = Main.GlobalTimeWrappedHourly * 0.5f + factor;
            float f = MathF.Truncate(x);
            x -= f;
            return Main.hslToRgb(x, 1, 0.8f);
        }

        public override void DrawSelf(Color lightColor)
        {
            int textureType = GetTextureType((int)ShieldType);
            Main.instance.LoadItem(textureType);
            Texture2D mainTex = TextureAssets.Item[textureType].Value;
            var pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(mainTex, pos, null, Color.White, Projectile.rotation - 1.57f + extraRotation, mainTex.Size() / 2, Projectile.scale, 0, 0);
            if (State == (int)FlyingShieldStates.Shooting)
            {
                float a = 1;
                Color c = GetColor(ref a);
                Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, Color.White * 0.5f, c * 0.7f,
                    Timer / flyingTime, 0f, 0.3f, 0.8f, 1, MathHelper.PiOver4
                    , new Vector2(3.5f, 1.5f), Vector2.One);
                Helper.DrawPrettyStarSparkle(Projectile.Opacity, 0, pos, Color.White, c,
                    Timer / flyingTime, 0.4f, 0.6f, 0.9f, 1f, MathHelper.PiOver4
                    , new Vector2(1.5f, 3.5f), Vector2.One);
            }
        }

        public override void DrawTrails(Color lightColor)
        {
            Texture2D Texture = ModContent.Request<Texture2D>(TrailTexture).Value;

            List<CustomVertexInfo> bars = new List<CustomVertexInfo>();
            List<CustomVertexInfo> bars2 = new List<CustomVertexInfo>();
            for (int i = 0; i < trailCachesLength; i++)
            {
                float factor = (float)i / trailCachesLength;
                Vector2 Center = Projectile.oldPos[i] - Main.screenPosition;
                Vector2 normal = (Projectile.oldRot[i] + MathHelper.PiOver2).ToRotationVector2();
                Vector2 Top = Center + normal * trailWidth;
                Vector2 Bottom = Center - normal * trailWidth;

                Vector2 Top2 = Center + normal * 6;
                Vector2 Bottom2 = Center - normal * 6;

                var Color = GetColor(ref factor);//.MultiplyRGB(lightColor);
                Color.A = (byte)(150 * factor);

                var c2 = Color.White;
                c2.A = (byte)(150 * factor);
                bars.Add(new(Top, Color, new Vector3(factor, 0, 0)));
                bars.Add(new(Bottom, Color, new Vector3(factor, 1, 0)));
                bars2.Add(new(Top2, c2, new Vector3(factor, 0, 0)));
                bars2.Add(new(Bottom2, c2, new Vector3(factor, 1, 0)));
            }

            Main.graphics.GraphicsDevice.Textures[0] = Texture;
            BlendState b = Main.graphics.GraphicsDevice.BlendState;
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
            Main.graphics.GraphicsDevice.BlendState = b;
        }
    }

    /// <summary>
    /// ai0传入贴图类型，ai1传入是否燃烧
    /// </summary>
    //public class HephaesthWave : ModProjectile
    //{
    //    public override string Texture => AssetDirectory.Accessories + "HallowedShield";

    //    ref float TextureType => ref Projectile.ai[0];
    //    bool Burning =>  Projectile.ai[1]==1;
    //    ref float Timer => ref Projectile.ai[2];
    //    ref float Timer2 => ref Projectile.localAI[2];

    //    Vector2 top;
    //    Vector2 bottom;
    //    float scale;

    //    public override void SetDefaults()
    //    {
    //        Projectile.DamageType = DamageClass.Melee;
    //        Projectile.tileCollide = true;
    //        Projectile.friendly = true;
    //        Projectile.penetrate = -1;
    //        Projectile.width = Projectile.height = 16;
    //        Projectile.scale = 1.25f;
    //        Projectile.usesLocalNPCImmunity = true;
    //        Projectile.localNPCHitCooldown = 10;
    //    }

    //    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    //    {
    //        float a = 0;
    //        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
    //            top, bottom, 40 * Projectile.scale * scale, ref a);
    //    }

    //    public override void AI()
    //    {
    //        const int MaxTime = 25;

    //        float factor = Timer / MaxTime;
    //        float sqrtFactor = Coralite.Instance.SqrtSmoother.Smoother(factor);

    //        scale = Helper.Lerp(0.8f, 1.2f, sqrtFactor);

    //        Projectile.velocity *= 0.9f;
    //        Projectile.rotation = Projectile.velocity.ToRotation();

    //        float num8 = Projectile.rotation + Main.rand.NextFloatDirection() * MathHelper.PiOver2 * 0.7f;
    //        Vector2 vector2 = Projectile.Center + num8.ToRotationVector2() * 74f * Projectile.scale * scale;
    //        Vector2 vector3 = num8.ToRotationVector2();
    //        if (Main.rand.NextBool())
    //        {
    //            Dust dust2 = Dust.NewDustPerfect(Projectile.Center + num8.ToRotationVector2() * (Main.rand.NextFloat() * 80f * Projectile.scale + 20f * Projectile.scale)
    //                , 278, vector3 * 1f, 100, Color.Lerp(Burning ? new Color(89, 219, 255) : new Color(255, 237, 134), Color.White, Main.rand.NextFloat() * 0.3f), 0.4f);
    //            dust2.fadeIn = 0.4f + Main.rand.NextFloat() * 0.15f;
    //            dust2.noGravity = true;
    //        }

    //        if (Main.rand.NextFloat() * 1.5f < Projectile.Opacity)
    //            Dust.NewDustPerfect(vector2, 43, vector3 * 1f, 100, Color.White * Projectile.Opacity, 1.2f * Projectile.Opacity);
    //        Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
    //        Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 32;

    //        top = pos + dir * 70 * Projectile.scale * scale;
    //        bottom = pos - dir * 70 * Projectile.scale * scale;
    //        Timer++;
    //        Timer2 += 0.085f;
    //        if (Timer > MaxTime)
    //            Projectile.Kill();
    //    }

    //    public static int GetTextureType(int index)
    //    {
    //        return index switch
    //        {
    //            -1 => ModContent.ItemType<TrashCanLid>(),
    //            -2 => ModContent.ItemType<RedJadeShield>(),
    //            -3 => ModContent.ItemType<GoldenSamurai>(),
    //            -4 => ModContent.ItemType<GlassShield>(),
    //            -5 => ModContent.ItemType<GlazeBulwark>(),
    //            -6 => ModContent.ItemType<GemrainAegis>(),
    //            -7 => ModContent.ItemType<MechRioter>(),
    //            -8 => ModContent.ItemType<TortoiseshellFortress>(),
    //            -9 => ModContent.ItemType<Fishronguard>(),
    //            -10 => ModContent.ItemType<ShanHai>(),
    //            -11 => ModContent.ItemType<Leonids>(),
    //            -12 => ModContent.ItemType<Solleonis>(),
    //            -13 => ModContent.ItemType<ConquerorOfTheSeas>(),
    //            -14 => ModContent.ItemType<SilverAngel>(),
    //            -15 => ModContent.ItemType<RoyalAngel>(),
    //            -16 => ModContent.ItemType<Noctiflair>(),
    //            _ => index,
    //        };
    //    }

    //    public override bool OnTileCollide(Vector2 oldVelocity) => false;

    //    public override bool PreDraw(ref Color lightColor)
    //    {
    //        int textureType = GetTextureType((int)TextureType);
    //        Main.instance.LoadItem(textureType);
    //        Texture2D mainTex = TextureAssets.Item[textureType].Value;
    //        Texture2D extraTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "TerraBladeSlash").Value;

    //        var frameBox = extraTex.Frame(1, 4, 0, 0);

    //        var mainOrigin = mainTex.Size() / 2;
    //        var extraOrigin = frameBox.Size() / 2;

    //        var pos = Projectile.Center - Main.screenPosition;

    //        SpriteEffects effects = Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
    //        float num = Projectile.scale * scale;
    //        float num2 = Timer / 25;
    //        float num3 = Utils.Remap(num2, 0f, 0.6f, 0f, 1f) * Utils.Remap(num2, 0.6f, 1f, 1f, 0f);
    //        float num4 = 0.975f;
    //        float fromValue = Lighting.GetColor(Projectile.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
    //        fromValue = Utils.Remap(fromValue, 0.2f, 1f, 0f, 1f);
    //        Color color;
    //        Color color2;
    //        Color color3;

    //        if (Burning)
    //        {
    //            color = new Color(52, 29, 114);
    //            color2 = new Color(83, 129, 255);
    //            color3 = new Color(89, 219, 255);
    //        }
    //        else
    //        {
    //            color2 = new Color(155, 26, 10);
    //            color = new Color(255, 174, 33);
    //            color3 = new Color(255, 237, 134);
    //        }

    //        float rot = Projectile.rotation;

    //        Main.spriteBatch.Draw(extraTex, pos, frameBox, color * fromValue * num3, rot
    //            + Timer2 * MathHelper.PiOver4 * -1f * (1f - num2), extraOrigin, num, 0, 0f);
    //        Color color4 = Color.White * num3 * 0.5f;
    //        color4.A = (byte)(color4.A * (1f - fromValue));
    //        Color color5 = color4 * fromValue * 0.5f;
    //        color5.G = (byte)(color5.G * fromValue);
    //        color5.B = (byte)(color5.R * (0.25f + fromValue * 0.75f));
    //        Main.spriteBatch.Draw(extraTex, pos, frameBox, color5 * 0.15f, rot + Timer2 * 0.04f, extraOrigin, num, effects, 0f);
    //        Main.spriteBatch.Draw(extraTex, pos, frameBox, color3 * fromValue * num3 * 0.3f, rot, extraOrigin, num, effects, 0f);
    //        Main.spriteBatch.Draw(extraTex, pos, frameBox, color2 * fromValue * num3 * 0.5f, rot, extraOrigin, num * num4, effects, 0f);
    //        Main.spriteBatch.Draw(extraTex, pos, extraTex.Frame(1, 4, 0, 3), Color.White * 0.6f * num3, rot + Timer2 * 0.02f, extraOrigin, num, effects, 0f);
    //        Main.spriteBatch.Draw(extraTex, pos, extraTex.Frame(1, 4, 0, 3), Color.White * 0.5f * num3, rot + Timer2 * -0.1f, extraOrigin, num * 0.8f, effects, 0f);
    //        Main.spriteBatch.Draw(extraTex, pos, extraTex.Frame(1, 4, 0, 3), Color.White * 0.4f * num3, rot + Timer2 * -0.2f, extraOrigin, num * 0.6f, effects, 0f);
    //        //绘制盾牌
    //        Color shieldColor = Color.White * num3;
    //        for (int i = 0; i < 3; i++)
    //        {
    //            Main.spriteBatch.Draw(mainTex, pos + Projectile.rotation.ToRotationVector2() * (extraTex.Width * (0.25f + i * 0.1f) - 6f) * num, null
    //                , shieldColor * (0.4f + i * 0.2f), Projectile.rotation, mainOrigin, new Vector2(0.8f - num2 * 0.6f, 1) * Projectile.scale * scale * (1f + i * 0.5f), effects, 0f);
    //        }

    //        //绘制闪光
    //        for (float num5 = 0f; num5 < 8f; num5 += 1f)
    //        {
    //            float num6 = Projectile.rotation + Timer2 * num5 * (MathHelper.Pi * -2.1f) * 0.025f + Utils.Remap(num2, 0f, 1f, 0f, MathHelper.PiOver4 * Timer2);
    //            Vector2 drawpos = pos + num6.ToRotationVector2() * (extraTex.Width * 0.5f - 6f) * num;
    //            float num7 = num5 / 9f;
    //            Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None
    //                , drawpos, new Color(255, 255, 255, 0) * num3 * num7, color3, num2, 0f, 0.5f, 0.5f, 1f, num6, new Vector2(0f, Utils.Remap(num2, 0f, 1f, 3f, 0f)) * num, Vector2.One * num);
    //        }

    //        //绘制星星
    //        for (int i = 0; i < 3; i++)
    //        {
    //            Vector2 drawpos2 = pos + (Projectile.rotation + Timer2 * (-0.4f + i * 0.4f)).ToRotationVector2() * (extraTex.Width * 0.5f - 4f) * num;

    //            Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None
    //                , drawpos2, new Color(255, 255, 255, 0) * num3 * 0.5f, color3, num2
    //                , 0f, 0.5f, 0.5f, 1f, 0f
    //                , new Vector2(2f, Utils.Remap(num2, 0f, 1f, 3f + i * 0.5f, 1f)) * num
    //                , Vector2.One * num);
    //        }
    //        return false;
    //    }
    //}

    public class HephaesthBurst : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "Star2";

        ref float State => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];
        public bool Burning => Projectile.ai[2] == 1;

        float targetRotation;
        float baseRotation;
        float alpha;
        Color drawColor;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            if (State < 2)
            {
                return false;
            }

            return null;
        }

        public override void OnSpawn(IEntitySource source)
        {
            targetRotation = Main.rand.Next(3) * MathHelper.PiOver2 + MathHelper.PiOver4;

            Projectile.rotation = baseRotation = targetRotation + Main.rand.NextFloat(-3.141f, 3.141f);
            Projectile.scale = 0.3f;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://星星旋转放大
                    {
                        const int RollingTime = 9;
                        float factor = Timer / RollingTime;
                        factor = Coralite.Instance.SqrtSmoother.Smoother(factor);

                        if (alpha < 0.9f)
                        {
                            alpha += 1 / 4f;
                        }

                        if (Burning)
                            drawColor = Color.Lerp(new Color(52, 29, 114), new Color(83, 129, 255), factor);
                        else
                            drawColor = Color.Lerp(new Color(86, 80, 112), new Color(255, 174, 33), factor);
                        Projectile.rotation = Helper.Lerp(baseRotation, targetRotation, factor);
                        Projectile.scale = Helper.Lerp(0.3f, 1f, factor);
                        Timer++;
                        if (Timer > RollingTime)
                        {
                            Timer = 0;
                            State++;
                        }
                    }
                    break;
                case 1://星星缩小
                    {
                        const int ScaleTime = 7;
                        float factor = Timer / ScaleTime;

                        Projectile.scale = Helper.Lerp(1f, 0f, factor);
                        Timer++;
                        if (Timer > ScaleTime)
                        {
                            Timer = 0;
                            State++;
                            Projectile.rotation = Main.rand.NextFloat(6.282f);
                            //生成粒子
                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 dir = (i * MathHelper.PiOver2 + MathHelper.PiOver4).ToRotationVector2();
                                for (int k = 0; k < 7; k++)
                                {
                                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.RainbowMk2, dir * (3 + k), 30, drawColor, 2f - k * 0.17f);
                                    d.noGravity = true;
                                }
                            }

                            for (int i = 0; i < 4; i++)
                            {
                                Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Helper.NextVec2Dir() * Main.rand.NextFloat(0.5f, 3f), 100, Color.White, Main.rand.NextFloat(1.3f, 1.6f));
                                //Dust.NewDustPerfect(Projectile.Center, DustID.Titanium, Helper.NextVec2Dir() * Main.rand.NextFloat(0.5f,5f),100, Color.White,Main.rand.NextFloat(1f,1.3f));
                            }
                        }
                    }
                    break;
                case 2://爆！！！！！！！！！！！！！！！！
                    {
                        const int BurstTime = 20;

                        //if (Projectile.scale < 1.2f)
                        //    Projectile.scale += 0.1f;
                        //float factor = Timer / BurstTime;
                        //factor = Coralite.Instance.SqrtSmoother.Smoother(factor);

                        //alpha = Helper.Lerp(1, 0, factor);
                        Timer++;
                        if (Timer > BurstTime)
                        {
                            Projectile.Kill();
                        }
                    }
                    break;
            }
        }

        public override void OnKill(int timeLeft)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            var pos = Projectile.Center - Main.screenPosition;
            Color c = drawColor;
            c.A = (byte)(255 * alpha);
            if (State < 2)
            {
                Texture2D mainTex = Projectile.GetTexture();

                float scale = Projectile.scale * 0.15f;

                Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation,
                    mainTex.Size() / 2, scale, 0, 0);
                c = Color.White * 0.5f;
                c.A = (byte)(255 * alpha);
                Main.spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation + MathHelper.Pi,
                    mainTex.Size() / 2, scale * 0.5f, 0, 0);
            }
        }
    }
}
