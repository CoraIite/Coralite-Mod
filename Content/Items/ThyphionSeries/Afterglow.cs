using Coralite.Content.ModPlayers;
using Coralite.Content.RecipeGroups;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Afterglow : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(13, 2f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 27, 7f);

            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 0, 10);

            Item.noUseGraphic = true;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<AfterglowHeldProj>(), damage, knockback, player.whoAmI, rot, 0);
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(WoodenBowGroup.GroupName)
                .AddIngredient(ItemID.Torch, 49)
                .AddIngredient(ItemID.Ruby)
                .AddTile(TileID.WorkBenches)
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
                        newVelocity.X = dashDirection * 7;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 80;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.velocity = newVelocity;
            Player.direction = (int)dashDirection;

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<AfterglowHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<AfterglowHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f + dashDirection * 1, 1, 20);
            }

            return true;
        }
    }

    public class AfterglowHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "Afterglow";

        public ref float CurrentArrowType => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public enum ArrowType
        {
            arrow,
            fire,
            iceFire
        }

        public override int GetItemType()
            => ItemType<Afterglow>();

        public override Vector2 GetOffset()
            => new(10, 0);

        public override void DashAttackAI()
        {
            do
            {
                switch ((ArrowType)CurrentArrowType)
                {
                    default:
                    case ArrowType.arrow:
                        break;
                    case ArrowType.fire:
                        Lighting.AddLight(Projectile.Center, TorchID.Torch);
                        break;
                    case ArrowType.iceFire:
                        Lighting.AddLight(Projectile.Center, TorchID.Ice);
                        break;
                }

                if (Timer < DashTime + 2)
                {
                    if (CurrentArrowType == (int)ArrowType.arrow)
                    {
                        if (Timer < DashTime * 0.75f)
                        {
                            Point16 pos = Owner.Bottom.ToTileCoordinates16();
                            for (int i = 0; i < 2; i++)
                            {
                                Tile t = Framing.GetTileSafely(pos + new Point16(0, i));

                                if (t.HasTile && (Main.tileSolid[t.TileType] || TileID.Sets.Platforms[t.TileType]))
                                {
                                    Helper.PlayPitched(CoraliteSoundID.FireBallExplosion_Item74, Projectile.Center, pitch: 0.4f);
                                    CurrentArrowType = (int)ArrowType.fire;
                                    if (t.TileType is TileID.IceBlock or TileID.IceBrick)
                                        CurrentArrowType = (int)ArrowType.iceFire;

                                    break;
                                }
                            }
                        }
                    }
                    else if (CurrentArrowType == (int)ArrowType.fire)
                    {
                        if (Timer < DashTime * 0.75f)
                        {
                            Point16 pos = Owner.Bottom.ToTileCoordinates16();
                            for (int i = 0; i < 2; i++)
                            {
                                Tile t = Framing.GetTileSafely(pos + new Point16(0, i));

                                if (t.HasSolidTile() && (TileID.Sets.Ices[t.TileType] || TileID.Sets.IcesSnow[t.TileType]))
                                {
                                    Helper.PlayPitched(CoraliteSoundID.IceMagic_Item28, Projectile.Center, pitch: 0.2f);
                                    CurrentArrowType = (int)ArrowType.iceFire;

                                    break;
                                }
                            }
                        }

                        Vector2 dir = Rotation.ToRotationVector2();
                        Vector2 center = Projectile.Center + dir * 20;

                        for (int i = 0; i < 2; i++)
                        {
                            Dust d = Dust.NewDustPerfect(center + Main.rand.NextVector2Circular(4, 4), DustID.Torch
                                , Helper.NextVec2Dir(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
                            d.noGravity = true;
                        }
                    }
                    else
                    {
                        Vector2 dir = Rotation.ToRotationVector2();
                        Vector2 center = Projectile.Center + dir * 20;

                        for (int i = 0; i < 2; i++)
                        {
                            Dust d = Dust.NewDustPerfect(center + Main.rand.NextVector2Circular(4, 4), DustID.IceTorch
                                , Helper.NextVec2Dir(0.5f, 2f), Scale: Main.rand.NextFloat(1f, 1.5f));
                            d.noGravity = true;
                        }
                    }

                    Owner.itemTime = Owner.itemAnimation = 2;

                    Rotation = Helper.Lerp(RecordAngle, OwnerDirection > 0 ? 0 : 3.141f, Timer / DashTime);
                    break;
                }

                if (Owner.controlUseItem)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                        Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.35f);

                        if (Main.rand.NextBool(10))
                        {
                            Vector2 dir = Rotation.ToRotationVector2();
                            Vector2 center = Projectile.Center + dir * 20;

                            if (CurrentArrowType == (int)ArrowType.fire)
                            {
                                Dust d = Dust.NewDustPerfect(center + Main.rand.NextVector2Circular(4, 4), DustID.Torch
                                    , (-Vector2.UnitY).RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.noGravity = true;
                            }
                            else if (CurrentArrowType == (int)ArrowType.iceFire)
                            {
                                Dust d = Dust.NewDustPerfect(center + Main.rand.NextVector2Circular(4, 4), DustID.IceTorch
                                    , (-Vector2.UnitY).RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(2f, 4f), Scale: Main.rand.NextFloat(1f, 1.5f));
                                d.noGravity = true;
                            }
                        }
                    }

                    Projectile.timeLeft = 2;
                    LockOwnerItemTime();
                }
                else
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.One) * 12f
                            , GetArrowType(), Owner.GetWeaponDamage(Owner.HeldItem), Projectile.knockBack, Projectile.owner);
                        SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
                    }

                    Projectile.Kill();
                }

            } while (false);

            Projectile.rotation = Rotation;
            Timer++;
        }

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

        public int GetArrowType()
        {
            return (ArrowType)CurrentArrowType switch
            {
                ArrowType.fire => ProjectileID.FireArrow,
                ArrowType.iceFire => ProjectileID.FrostburnArrow,
                _ => ProjectileID.WoodenArrowFriendly
            };
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1, OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (Special == 0)
                return false;

            int type = GetArrowType();
            Main.instance.LoadProjectile(type);
            Texture2D arrowTex = TextureAssets.Projectile[type].Value;
            Vector2 dir = Rotation.ToRotationVector2();
            Main.spriteBatch.Draw(arrowTex, center, null, lightColor, Projectile.rotation + 1.57f
                , new Vector2(arrowTex.Width / 2, arrowTex.Height * 5 / 6), 1, 0, 0f);

            return false;
        }
    }
}
