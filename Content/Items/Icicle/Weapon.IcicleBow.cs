using Coralite.Content.GlobalItems;
using Coralite.Content.Items.ThyphionSeries;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Icicle
{
    public class IcicleBow : BaseDashBowItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(26, 3f);
            Item.DefaultToRangedWeapon(ProjectileType<IcicleBowHeldProj>(), AmmoID.Arrow, 24, 9f, true);
            Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 1));

            Item.UseSound = CoraliteSoundID.Bow_Item5;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTurn = false;
            Item.noUseGraphic = true;
            Item.channel = true;
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<IcicleBowHeldProj>(), damage, knockback, player.whoAmI, rot, 0);
            Projectile.NewProjectile(source, player.Center, dir * 13, ProjectileType<IcicleArrow>(), damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<IcicleCrystal>(2)
            .AddIngredient<IcicleScale>(2)
            .AddTile(TileID.IceMachine)
            .Register();
        }

        public override bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        float dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * 10;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.immuneTime = 20;
            Player.immune = true;
            Player.velocity = newVelocity;

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28, Player.Center);
                for (int i = 0; i < 4; i++)//生成冰晶粒子
                {
                    Vector2 center = Player.Center + ((-1.57f + (i * 1.57f)).ToRotationVector2() * 64);
                    Vector2 velocity = (i * 1.57f).ToRotationVector2() * 4;
                    IceStarLight.Spawn(center, velocity, 1f, () => Player.Center, 16);
                }

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<IcicleBowHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<IcicleBowHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, (Main.MouseWorld - Player.Center).ToRotation(), 1);
            }

            return true;

        }
    }

    /// <summary>
    /// ai0用于判断玩家的手持方向,ai1用于控制是否是特殊的弹幕
    /// </summary>
    public class IcicleBowHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.IcicleItems + "IcicleBow";

        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];

        public bool fadeIn = true;

        public override int GetItemType()
            => ModContent.ItemType<IcicleBow>();

        public override void AIBefore()
        {
            if (Alpha == 0)
            {
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                }

                Projectile.rotation = Rotation;
                Alpha = 0.0001f;
                if (Projectile.ai[1] == 0)
                    Projectile.timeLeft = Owner.itemTime;
                else
                {
                    Projectile.timeLeft = Owner.itemTimeMax;
                }

                if (Owner.TryGetModPlayer(out CoralitePlayer cp))
                {
                    DashTime = cp.DashTimer;
                }
            }
        }

        public override void DashAttackAI()
        {
            if (fadeIn)
            {
                Alpha += 0.02f;
                if (Alpha > 1)
                {
                    Alpha = 1;
                    fadeIn = false;
                }
            }

            Lighting.AddLight(Owner.Center, Coralite.IcicleCyan.ToVector3() * Alpha);

            do
            {
                if (Timer < DashTime + 2)
                {
                    Rotation += 0.3141f; //1/10 Pi
                    Projectile.timeLeft = 15;

                    Owner.itemTime = Owner.itemAnimation = 2;
                    break;
                }

                if (!DownLeft && Projectile.localAI[2] == 0)
                {
                    if (Projectile.IsOwnedByLocalPlayer())
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        Rotation = Rotation.AngleLerp(ToMouseA, 0.25f);
                        Projectile.netUpdate = true;

                        if (Main.rand.NextBool(20))
                        {
                            Vector2 dir = Rotation.ToRotationVector2();
                            PRTLoader.NewParticle(Owner.Center + (dir * 16) + Main.rand.NextVector2Circular(8, 8), dir * 1.2f, CoraliteContent.ParticleType<HorizontalStar>(), Coralite.IcicleCyan, Main.rand.NextFloat(0.1f, 0.15f));
                        }
                    }
                    Projectile.timeLeft = 15;
                    Owner.itemTime = Owner.itemAnimation = 2;
                }
                else
                {
                    if (Projectile.localAI[2] == 0 && Projectile.IsOwnedByLocalPlayer())
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.One) * 9.5f
                            , ModContent.ProjectileType<IcicleStarArrow>(), (int)(Owner.GetDamageWithAmmo(Item) * 2.3f), Projectile.knockBack, Projectile.owner);
                        SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
                    }

                    Rotation = Rotation.AngleLerp(ToMouseA, 0.25f);
                    Owner.itemTime = Owner.itemAnimation = 2;
                    if (Projectile.localAI[2] > 15)
                        Projectile.Kill();

                    Projectile.localAI[2]++;
                }

            } while (false);

            Projectile.rotation = Rotation;
            Timer++;
        }

        public override Vector2 GetOffset()
            => new(12, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTextureValue();
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1.1f, DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (Alpha > 0.001f && Projectile.localAI[2] == 0)
            {
                Texture2D starTex = ModContent.Request<Texture2D>(AssetDirectory.IcicleItems + "IcicleStarArrow").Value;
                float factor = Timer % 80 / 80;
                float num3 = Utils.Remap(factor, 0f, 0.6f, 0f, 1f) * Utils.Remap(factor, 0.6f, 1f, 1f, 0f);
                Vector2 dir = Rotation.ToRotationVector2();
                Main.spriteBatch.Draw(starTex, center + (dir * 6), null, Color.White * Alpha, Projectile.rotation + 1.57f, starTex.Size() / 2, 1.4f, SpriteEffects.None, 0f);

                Helper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, center + (dir * 18), new Color(255, 255, 255, 0) * num3 * 0.5f, Coralite.IcicleCyan,
                    factor, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(1.3f, 1.3f), Vector2.One);
            }

            return false;
        }
    }

    public class IcicleArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;

            Projectile.coldDamage = true;
            Projectile.friendly = true;
            Projectile.arrow = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            Projectile.velocity.Y += 0.05f;
            if (Projectile.velocity.Y > 12)
                Projectile.velocity.Y = 12;

            if (Main.rand.NextBool())
            {
                Projectile.SpawnTrailDust(DustID.IceTorch, Main.rand.NextFloat(0.1f, 0.2f), Scale: Main.rand.NextFloat(1f, 1.4f));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.IsOwnedByLocalPlayer())
            {
                Vector2 center = Projectile.Center - new Vector2(0, Main.rand.Next(220, 280)).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));
                Vector2 velocity = (Projectile.Center + Main.rand.NextVector2Circular(24, 24) - center).SafeNormalize(Vector2.UnitY) * 12;
                int damage = (int)(Projectile.damage * 0.35f);
                if (damage > 26)
                    damage = 26;

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), center, velocity,
                    ModContent.ProjectileType<IcicleFalling>(), damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (VisualEffectSystem.HitEffect_Dusts)
                for (int i = 0; i < 2; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.1f, 0.3f));

            SoundEngine.PlaySound(CoraliteSoundID.CrushedIce_Item27, Projectile.Center);
        }

    }
}
