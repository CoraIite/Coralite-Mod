using Coralite.Content.Bosses.BabyIceDragon;
using Coralite.Content.Dusts;
using Coralite.Content.GlobalItems;
using Coralite.Content.Items.Icicle;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Glaciate : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public bool powerfulAttack;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(78, 5f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 24, 13f);

            Item.rare = ItemRarityID.Pink;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 4);

            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useTurn = false;
            Item.UseSound = CoraliteSoundID.Bow_Item5;
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddDash(this);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                type = ProjectileType<IcicleArrow>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();

            int sp = 0;

            if (powerfulAttack)
            {
                powerfulAttack = false;
                sp = 2;
                Projectile.NewProjectile(source, player.Center, velocity, ProjectileType<GlaciateIceCube>(), damage * 2, knockback, player.whoAmI);
            }
            else
                Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                , player.Center, Vector2.Zero, ProjectileType<GlaciateHeldProj>(), damage, knockback, player.whoAmI, rot, sp);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleBow>()
                .AddIngredient(ItemID.IceBow)
                .AddIngredient(ItemID.BlizzardStaff)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            float dashDirection;

            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * 12;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 90;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 22;

            Player.velocity = newVelocity;
            Player.AddImmuneTime(ImmunityCooldownID.General, 24);
            Player.immune = true;

            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 10));

            if (Player.whoAmI == Main.myPlayer)
            {
                //Helper.PlayPitched("Misc/HallowDash", 0.4f, -0.2f, Player.Center);
                Helper.PlayPitched(CoraliteSoundID.IceMagic_Item28, Player.Center, pitch: -0.5f);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<GlaciateHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, newVelocity, ProjectileType<GlaciateHeldProj>(),
                    Player.GetDamageWithAmmo(Item), Player.HeldItem.knockBack, Player.whoAmI, newVelocity.ToRotation(), 1, 22);
            }

            return true;
        }
    }

    public class GlaciateHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public ref float DashState => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public float handOffset;
        public int SPTimer;

        public float rotAngle;

        public override int GetItemType()
            => ItemType<Glaciate>();

        public override void InitializeDashBow()
        {
            RecordAngle = Rotation;

            rotAngle = Special switch
            {
                1 => 0,
                _ => 0.5f,
            };
        }

        public override void DashAttackAI()
        {
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3());

            if (Timer < DashTime + 1)
            {
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    Owner.itemTime = Owner.itemAnimation = 2;
                    Rotation += MathHelper.TwoPi / DashTime;
                }

                if (Main.rand.NextBool())
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(40, 40), DustType<PixelPoint>()
                        , -RecordAngle.ToRotationVector2() * Main.rand.NextFloat(1, 3), newColor: Main.rand.NextFromList(Color.LightCyan, Coralite.IcicleCyan), Scale: 2);
                }
                else
                {
                    var particle = PRTLoader.NewParticle<PixelLine>(Projectile.Center + Main.rand.NextVector2Circular(40, 40)
                         , -Owner.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 7), Main.rand.NextFromList(Color.LightCyan, Coralite.IcicleCyan));
                    particle.TrailCount = Main.rand.Next(10, 24);
                }

                rotAngle += 0.4f / (DashTime + 1);

                Projectile.timeLeft = 22;
                Owner.itemTime = Owner.itemAnimation = 22;
            }
            else
            {
                if (!DownLeft && SPTimer == 0)
                {
                    if (Projectile.IsOwnedByLocalPlayer())
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        Rotation = Rotation.AngleLerp(ToMouseA, 0.15f);
                    }

                    Projectile.timeLeft = 22;
                    Owner.itemTime = Owner.itemAnimation = 22;
                }
                else
                {
                    if (SPTimer == 0 && Projectile.IsOwnedByLocalPlayer())
                    {
                        handOffset = -20;

                        Helper.PlayPitched(CoraliteSoundID.FireStaffSummon_Item77, Projectile.Center);

                        //射冰晶波
                        Projectile.NewProjectileFromThis<GlaciateWave>(Projectile.Center, UnitToMouseV * 24, (int)(Owner.GetDamageWithAmmo(Item) * 2f)
                            , Projectile.knockBack);

                        if (Projectile.IsOwnedByLocalPlayer())
                        {
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Projectile.rotation.ToRotationVector2(), 8, 12, 10, 500));
                        }

                        Rotation = ToMouseA;
                    }

                    if (Projectile.IsOwnedByLocalPlayer())
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        if (SPTimer < 8)
                            Rotation -= Owner.direction * 0.05f;
                        else
                            Rotation = Rotation.AngleLerp(ToMouseA, 0.15f);
                    }

                    handOffset = Helper.Lerp(handOffset, 0, 0.1f);
                    rotAngle = Helper.Lerp(rotAngle, 0, 0.2f);
                    SPTimer++;

                    if (SPTimer > 22)
                        Projectile.Kill();
                }
            }

            Timer++;
            Projectile.rotation = Rotation;
        }

        public override void NormalShootAI()
        {
            if (Special == 2)
            {
                Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3());

                if (Projectile.timeLeft == Owner.itemTimeMax - 4)
                    handOffset = -20;

                handOffset = Helper.Lerp(handOffset, 0, 0.1f);
            }

            if (Projectile.timeLeft > Owner.itemAnimationMax * 0.6f)
                rotAngle = Helper.Lerp(rotAngle, 0, 0.15f);
            else
                rotAngle = Helper.Lerp(rotAngle, 0.3f, 0.05f);

            base.NormalShootAI();
        }

        public override Vector2 GetOffset()
            => new Vector2(22 + handOffset, -10);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            Rectangle framebox = mainTex.Frame(2, 1, 1, 0);
            float exAngle = rotAngle * DirSign;

            //绘制弦
            Vector2 dir1 = (Rotation + exAngle).ToRotationVector2();
            Vector2 n1 = (Rotation + exAngle + DirSign * 1.57f).ToRotationVector2();
            Vector2 dir2 = (Rotation - exAngle).ToRotationVector2();
            Vector2 n2 = (Rotation - exAngle + DirSign * 1.57f).ToRotationVector2();

            Vector2 TopPos = center - dir2 * 28 - n2 * 25;
            Vector2 bottomPos = center - dir1 * 12 + n1 * 47;

            Texture2D lineTex = TextureAssets.FishingLine.Value;
            Rectangle dest = new Rectangle((int)TopPos.X, (int)TopPos.Y, lineTex.Width, (int)Vector2.Distance(TopPos, bottomPos));
            Main.spriteBatch.Draw(lineTex, dest, null, Color.Cyan, (bottomPos - TopPos).ToRotation() - 1.57f, new Vector2(lineTex.Width / 2, 0), 0, 0);

            //绘制下颚
            Vector2 origin = framebox.Size() / 2;
            Main.spriteBatch.Draw(mainTex, center, framebox, lightColor, Projectile.rotation + exAngle, origin, 1, base.DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);
            //绘制上鄂
            framebox = mainTex.Frame(2, 1, 0, 0);
            Main.spriteBatch.Draw(mainTex, center, framebox, lightColor, Projectile.rotation - exAngle, origin, 1, base.DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (Special == 0)
                return false;

            Vector2 pos = center + dir2 * 16 - n2 * 2;

            Helper.DrawPrettyStarSparkle(1, 0, pos, Color.White, Coralite.IcicleCyan * 0.75f, 0.5f, 0, 0.5f, 0.5f, 0, 0.785f
                , new Vector2(2.5f), new Vector2(0.75f));

            return false;
        }
    }

    public class GlaciateWave : ModProjectile, IPostDrawAdditive, IDrawNonPremultiplied
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        private PRTGroup group;
        private Vector2 scale = new Vector2(1.5f, 0.2f);

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Alpha => ref Projectile.localAI[0];

        public Vector2 oldPoss;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.timeLeft = 12000;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.coldDamage = true;
        }

        public override bool? CanDamage()
        {
            if (State == 2)
            {
                return false;
            }

            return null;
        }

        public override void AI()
        {
            if (!VaultUtils.isServer)
                group ??= new PRTGroup();

            switch (State)
            {
                default:
                case 0://刚射出，减速
                    {
                        float factor = Timer / 40;
                        scale = Vector2.Lerp(new Vector2(1.5f, 0.2f), Vector2.One, Coralite.Instance.BezierEaseSmoother.Smoother(factor));
                        Alpha = Coralite.Instance.SqrtSmoother.Smoother(factor);

                        if (Timer > 40)
                        {
                            scale = Vector2.One;
                            Projectile.Resize(160, 160);
                            State = 1;
                            Timer = 0;
                            Alpha = 1;
                        }

                        if (Timer > 20)
                            SpawnParticle();
                        else
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(30, 30)
                                     , DustID.ApprenticeStorm, -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 4), Scale: Main.rand.NextFloat(1, 1.5f));

                                d.noGravity = true;
                            }
                        }
                        Timer++;
                    }
                    break;
                case 1://缓慢飞行
                    {
                        SpawnParticle();
                        Timer++;
                        if (Timer > 60 * 8)
                            TurnToFade();
                    }
                    break;
                case 2://消失
                    {
                        Alpha = 1 - Timer / 20f;
                        Timer++;

                        Projectile.velocity *= 0.8f;
                        if (Timer > 20)
                            Projectile.Kill();
                    }
                    break;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            group?.Update();
            foreach (var p in group)
                p.Position += Projectile.Center - oldPoss;

            oldPoss = Projectile.Center;
        }

        public void SpawnParticle()
        {
            if (group == null || Timer < 5)
                return;

            float r = 1.3f * scale.Y;
            float l = 65 + 45 * (scale.X - 1) / 0.5f;

            if (Timer % 2 == 0)
            {
                group.NewParticle<SnowFlower>(Projectile.Center + (Projectile.rotation + Main.rand.NextFloat(-r, r)).ToRotationVector2() * l, -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 3)
                    , Main.rand.NextFromList(Color.White, Coralite.IcicleCyan), Main.rand.NextFloat(0.15f, 0.3f));

            }

            group.NewParticle<Fog>(Projectile.Center + (Projectile.rotation + Main.rand.NextFloat(-r, r)).ToRotationVector2() * l, -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3, 5)
                , Main.rand.NextFromList(Color.White, Coralite.IcicleCyan), Main.rand.NextFloat(0.6f, 1f));

            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation + Main.rand.NextFloat(-r, r)).ToRotationVector2() * l
                     , DustID.ApprenticeStorm, -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2, 4));

                d.noGravity = true;
            }
        }

        public void TurnToFade()
        {
            State = 2;
            Timer = 0;
            Projectile.tileCollide = false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            TurnToFade();
            Projectile.velocity = oldVelocity * 0.6f;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 2;
            if (Projectile.localAI[2] == 0)
            {
                Projectile.localAI[2] = 1;
                if (Main.player[Projectile.owner].HeldItem.ModItem is Glaciate glaciate)
                    glaciate.powerfulAttack = true;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 32;
            height = 32;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTexture();

            var origin = mainTex.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 scale = this.scale * 0.35f;
            Color c = Color.White;
            c.A = (byte)(50 * Alpha);

            float factor = 0.05f * MathF.Sin((float)Main.timeForVisualEffects * 0.1f);

            float y = scale.Y;
            scale.Y = y * (1 + factor);
            spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale, 0, 0);
            scale.Y = y * (1 - factor);
            spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale, SpriteEffects.FlipVertically, 0);

            group?.Draw(spriteBatch);
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = Projectile.GetTexture();

            var origin = mainTex.Size() / 2;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Vector2 scale = this.scale * 0.35f;
            Color c = Color.DarkBlue;
            c.A = (byte)(150 * Alpha);

            spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale, 0, 0);

            c = Color.Cyan;
            c.A = (byte)(80 * Alpha);

            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(mainTex, pos + ((float)(Main.timeForVisualEffects * 0.05f) + i * MathHelper.TwoPi / 3).ToRotationVector2() * 4, null
                    , c, Projectile.rotation, origin, new Vector2(scale.X, scale.Y * 0.8f), i % 2 == 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0);
            }

            c = Color.LightBlue;
            c.A = (byte)(150 * Alpha);

            float factor = 0.05f * MathF.Sin((float)Main.timeForVisualEffects * 0.1f);

            float y = scale.Y;
            scale.Y = y * (1 + factor);
            spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale, 0, 0);
            scale.Y = y * (1 - factor);
            spriteBatch.Draw(mainTex, pos, null, c, Projectile.rotation, origin, scale, SpriteEffects.FlipVertically, 0);
        }
    }

    public class GlaciateIceCube : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleSpurt";

        public ref float OwnerDirection => ref Projectile.ai[0];
        public ref float MaxTime => ref Projectile.ai[1];

        public ref float Alpha => ref Projectile.localAI[1];
        public bool fadeIn = true;
        public bool canDamage = true;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 30;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.coldDamage = true;
            Projectile.timeLeft = 60 * 20;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (fadeIn)
            {
                if (Alpha == 0f)
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
                }

                Alpha += 0.15f;
                if (Alpha > 1)
                {
                    Alpha = 1;
                    fadeIn = false;
                }
            }

            if (canDamage)
            {
                Vector2 targetDir = (Projectile.rotation - 1.57f).ToRotationVector2();
                for (int i = 0; i < 2; i++)
                {
                    if (Framing.GetTileSafely(Projectile.Center + (targetDir * i * 16)).HasSolidTile())
                    {
                        Projectile.timeLeft = 10;
                        canDamage = false;
                        Projectile.netUpdate = true;
                        break;
                    }
                }

                HitWave();

                Color lightColor = Lighting.GetColor((Projectile.Center / 16).ToPoint());
                PRTLoader.NewParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 0.1f, CoraliteContent.ParticleType<Fog>(), lightColor * Alpha, Main.rand.NextFloat(0.6f, 0.8f));
                if (Projectile.timeLeft % 2 == 0)
                    PRTLoader.NewParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 0.3f, CoraliteContent.ParticleType<SnowFlower>(), lightColor * Alpha, Main.rand.NextFloat(0.2f, 0.4f));

                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8)
                    , DustID.ApprenticeStorm, -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.8f));

                d.noGravity = true;
            }

            Lighting.AddLight(Projectile.Center, Coralite.IcicleCyan.ToVector3());

            if (Projectile.timeLeft < 10)
            {
                Projectile.velocity *= 0.65f;
                if (Alpha > 0f)
                {
                    if (!fadeIn)
                        Alpha -= 0.1f;
                    Projectile.timeLeft += 1;
                }
            }
        }

        public void HitWave()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];

                if (p.active && p.friendly && p.type == ProjectileType<GlaciateWave>() && p.Colliding(p.getRect(), Projectile.getRect()))
                {
                    if (p.ai[0] < 2)
                        (p.ModProjectile as GlaciateWave).TurnToFade();

                    //生成冰晶柱
                    SpawnIceThorns(Vector2.Lerp(Projectile.Center, p.Center + p.velocity * 5, 0.5f), -Projectile.velocity.SafeNormalize(Vector2.Zero)
                        , p.velocity.SafeNormalize(Vector2.Zero));
                    Projectile.Kill();
                }
            }
        }

        public void SpawnIceThorns(Vector2 pos, Vector2 reverseSelfDir, Vector2 targetDir)
        {
            if (!Projectile.IsOwnedByLocalPlayer())
                return;

            PunchCameraModifier modifier = new(Projectile.Center, new Vector2(0f, 1f), 10f, 6f, 10, 1000f, "BabyIceDragon");
            Main.instance.CameraModifiers.Add(modifier);

            TryMakingSpike(pos, reverseSelfDir);
            TryMakingSpike(pos, targetDir);
        }

        private void TryMakingSpike(Vector2 pos, Vector2 dir)
        {
            int damage = (int)(Projectile.damage * 6f);

            int p = Projectile.NewProjectileFromThis(pos + dir * 30, dir, ProjectileType<IceThorn>(),
                 damage, 0f, 0f, 1.3f);
            Main.projectile[p].DamageType = DamageClass.Ranged;

            for (int i = -1; i < 2; i += 2)
            {
                Vector2 dir2 = dir.RotatedBy(i * 0.8f);
                p = Projectile.NewProjectileFromThis(pos + dir2 * 30, dir2, ProjectileType<IceThorn>(),
                    damage, 0f, 0f, 0.6f);
                Main.projectile[p].DamageType = DamageClass.Ranged;
            }
        }

        public override bool? CanDamage() => canDamage;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
            if (Projectile.damage > 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 center = Projectile.Center - new Vector2(0, Main.rand.Next(140, 220)).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
                Vector2 velocity = (Projectile.Center + Main.rand.NextVector2Circular(8, 8) - center).SafeNormalize(Vector2.UnitY) * 12;
                Projectile.NewProjectileFromThis(center, velocity, ProjectileID.Blizzard,
                     (int)(Projectile.damage * 0.9f), Projectile.knockBack);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            SpriteEffects effects = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Projectile.DrawShadowTrailsSacleStep(lightColor * Alpha, 0.5f, 0.5f / 12, 0, 12, 1, 0.3f / 12, null, 0);

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor * Alpha, Projectile.rotation, mainTex.Size() / 2, 1.2f, effects, 0f);
            return false;
        }
    }
}
