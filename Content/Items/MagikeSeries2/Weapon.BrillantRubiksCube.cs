using Coralite.Content.DamageClasses;
using Coralite.Content.Dusts;
using Coralite.Content.GlobalItems;
using Coralite.Content.ModPlayers;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class BrillantRubiksCube : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public static LocalizedText SingleText { get; private set; }
        public static LocalizedText AreaText { get; private set; }
        public static LocalizedText MagicText { get; private set; }

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(style switch
        {
            AttackStyle.SinglePoint => SingleText.Value,
            AttackStyle.Area => AreaText.Value,
            _ => MagicText.Value
        });

        public AttackStyle style;

        public enum AttackStyle
        {
            /// <summary>
            /// 单点生成
            /// </summary>
            SinglePoint,
            /// <summary>
            /// 区域生成
            /// </summary>
            Area,
            /// <summary>
            /// 射出几个魔法弹幕并生成
            /// </summary>
            Magic
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                SingleText = this.GetLocalization(nameof(SingleText));
                AreaText = this.GetLocalization(nameof(AreaText));
                MagicText = this.GetLocalization(nameof(MagicText));
            }
        }

        public override void Unload()
        {
            SingleText = null;
            AreaText = null;
            MagicText = null;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.DefaultToRangedWeapon(ModContent.ProjectileType<CrystallineLanceProj>(), AmmoID.Bullet, 28, 12f, true);
            Item.DamageType = MagikeDamage.Instance;
            Item.SetWeaponValues(50, 12f, 0);
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 4);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = CoraliteSoundID.MagicStaff_Item8;

            Item.useTurn = false;
            Item.noUseGraphic = true;

            Item.GetMagikeItem().MagikeMax = 7500;
            if (Item.TryGetGlobalItem(out CoraliteGlobalItem cgi))
                cgi.SpecialUse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override float UseTimeMultiplier(Player player)
        {
            switch (style)
            {
                case AttackStyle.SinglePoint:
                    return 0.3f;
                case AttackStyle.Area:
                    break;
                case AttackStyle.Magic:
                    break;
                default:
                    break;
            }

            return base.UseTimeMultiplier(player);
        }

        public override float UseAnimationMultiplier(Player player)
        {
            switch (style)
            {
                case AttackStyle.SinglePoint:
                    return 0.3f;
                case AttackStyle.Area:
                    break;
                case AttackStyle.Magic:
                    break;
                default:
                    break;
            }

            return base.UseTimeMultiplier(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.useSpecialAttack)
            {
                style++;
                if (style > AttackStyle.Magic)
                    style = AttackStyle.SinglePoint;
                return false;
            }

            Point p = Main.MouseWorld.ToTileCoordinates();
            if (player.altFunctionUse == 2)
            {
                switch (style)
                {
                    case AttackStyle.SinglePoint:
                        Tile t = Framing.GetTileSafely(p);
                        if (t.TileType == ModContent.TileType<CrystallineBarrier>())
                            WorldGen.KillTile(p.X, p.Y);
                        break;
                    case AttackStyle.Area:
                        Point p2 = Main.MouseWorld.ToTileCoordinates();
                        Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<BrillantRubiksCubeArea>(), 0, 0, player.whoAmI, ai2: 1);
                        break;
                    case AttackStyle.Magic:
                        break;
                    default:
                        break;
                }

                return false;
            }

            switch (style)
            {
                case AttackStyle.SinglePoint:
                    PlaceBarrier(Item, p);
                    break;
                case AttackStyle.Area:
                    Point p2 = Main.MouseWorld.ToTileCoordinates();
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<BrillantRubiksCubeArea>(), 0, 0,player.whoAmI, ai2: 0);
                    break;
                case AttackStyle.Magic:
                    break;
                default:
                    break;
            }

            return false;
        }

        public static void PlaceBarrier(Item item,Point p)
        {
            Tile t = Framing.GetTileSafely(p);

            if (t.HasTile)
                return;

            Rectangle tileRect = new Rectangle(p.X * 16, p.Y * 16, 16, 16);

            foreach (var player in Main.ActivePlayers)
                if (player.getRect().Intersects(tileRect))
                    return;
            foreach (var npc in Main.ActiveNPCs)
                if (npc.getRect().Intersects(tileRect))
                    return;

            if (MagikeHelper.TryCosumeMagike(item, 1))
                WorldGen.PlaceTile(p.X, p.Y, ModContent.TileType<CrystallineBarrier>());
        }
    }

    public class BrillantRubiksMagicProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public ref float TargetX => ref Projectile.ai[0];
        public ref float TargetY => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = MagikeDamage.Instance;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                    break;
                case 0:
                    Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    for (int k = 0; k < 4; k++)
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BarrierDust>(), Helper.NextVec2Dir(1f, 2f));
                    State = 1;
                    break;
                case 1://飞行
                    {
                        Projectile.rotation += 0.1f;

                        if (Main.rand.NextBool(3))
                        {
                            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BarrierDust>(), Helper.NextVec2Dir(0.5f, 1.5f));
                        }
                        if (Main.rand.NextBool(2))
                            Projectile.SpawnTrailDust(ModContent.DustType<CrystallineDustSmall>(), Main.rand.NextFloat(0.1f));

                        if (Vector2.DistanceSquared(Projectile.Center, new Vector2(TargetX, TargetY) * 16 + new Vector2(8, 8)) < Projectile.velocity.LengthSquared() + 4)
                        {
                            State = 2;

                            Tile t = Framing.GetTileSafely((int)TargetX, (int)TargetY);
                            if (t.HasTile)//有物块直接紫砂
                            {
                                Projectile.Kill();
                                return;
                            }

                            WorldGen.PlaceTile((int)TargetX, (int)TargetY, ModContent.TileType<CrystallineBarrier>(), true);

                            Projectile.velocity = Vector2.Zero;
                            Projectile.Center = new Vector2(TargetX, TargetY) * 16 + new Vector2(8, 8);
                        }
                    }
                    break;
                case 2://留在原地生成水晶
                    {
                        Timer++;

                        Tile t = Framing.GetTileSafely((int)TargetX, (int)TargetY);
                        if (!t.HasTile || t.TileType != ModContent.TileType<CrystallineBarrier>())
                        {
                            Projectile.Kill();
                            return;
                        }

                        if (Timer > 60 * 15)
                        {
                            WorldGen.KillTile((int)TargetX, (int)TargetY);
                            Projectile.Kill();
                        }
                    }
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State != 2)
            {
                Texture2D tex = Projectile.GetTextureValue();

                Rectangle box = tex.Frame(2, 1, 0, 0);
                Vector2 origin = box.Size() / 2;
                Vector2 pos = Projectile.Center - Main.screenPosition;

                Color selfC = Color.Lerp(Color.White * 0.9f, Color.White * 0.2f, MathF.Cos(Main.GlobalTimeWrappedHourly * 2) / 2 + 0.5f);
                Main.spriteBatch.Draw(tex, pos, box, Color.White * 0.5f, 0, Vector2.Zero, 1, 0, 0);

                Color c2 = selfC * 0.5f;
                c2.A = 0;

                for (int k = 0; k < 4; k++)
                {
                    Vector2 off = (Main.GlobalTimeWrappedHourly + k * MathHelper.PiOver4).ToRotationVector2() * 2;
                    Main.spriteBatch.Draw(tex, pos + off, box, c2, Projectile.rotation, origin, 1, 0, 0);
                }

                Main.spriteBatch.Draw(tex, pos, box, Color.White * 0.4f, Projectile.rotation, origin, 1, 0, 0);

                box = tex.Frame(2, 1, 1, 0);

                Main.spriteBatch.Draw(tex, pos, box, Color.White, Projectile.rotation, origin, 1, 0, 0);
            }

            return false;
        }
    }

    public class BrillantRubiksCubeArea : RectangleSelectProj
    {
        public override string Texture => AssetDirectory.Blank;

        public override int ItemType => ModContent.ItemType<BrillantRubiksCube>();

        public ref float UseType => ref Projectile.ai[2];

        public override void AI()
        {
            if (!onspawn)
            {
                Projectile.ai[0] = InMousePos.ToTileCoordinates16().X;
                Projectile.ai[1] = InMousePos.ToTileCoordinates16().Y;
                TargetPoint = BasePosition;
                onspawn = true;
            }

            Projectile.Center = Owner.Center;

            if (CheckHeldItem())
            {
                Projectile.Kill();
                return;
            }

            bool isUsing = UseType == 1 ? DownRight : DownLeft;

            if (isUsing)
            {
                Owner.itemTime = Owner.itemAnimation = 7;
                TargetPoint = InMousePos.ToTileCoordinates16();

                //限制范围
                if (Math.Abs(TargetPoint.X - BasePosition.X) > GamePlaySystem.SelectSize)
                    TargetPoint = new Point16(Math.Clamp(TargetPoint.X, BasePosition.X - GamePlaySystem.SelectSize, BasePosition.X + GamePlaySystem.SelectSize), TargetPoint.Y);
                if (Math.Abs(TargetPoint.Y - BasePosition.Y) > GamePlaySystem.SelectSize)
                    TargetPoint = new Point16(TargetPoint.X, Math.Clamp(TargetPoint.Y, BasePosition.Y - GamePlaySystem.SelectSize, BasePosition.Y + GamePlaySystem.SelectSize));
            }
            else
            {
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    Special();
                }

                Projectile.Kill();
                return;
            }
        }

        public override void Special()
        {
            Actractive();
        }

        public void Actractive()
        {
            int baseX = Math.Min(TargetPoint.X, BasePosition.X);
            int baseY = Math.Min(TargetPoint.Y, BasePosition.Y);

            int xLength = Math.Abs(TargetPoint.X - BasePosition.X) + 1;
            int yLength = Math.Abs(TargetPoint.Y - BasePosition.Y) + 1;

            if (UseType == 1)
                for (int j = baseY; j < baseY + yLength; j++)
                    for (int i = baseX; i < baseX + xLength; i++)
                    {
                        Point p = new Point(i, j);
                        Tile t = Framing.GetTileSafely(p);

                        if (t.TileType == ModContent.TileType<CrystallineBarrier>())
                            WorldGen.KillTile(p.X, p.Y);
                    }
            else
                //遍历一个矩形区域，并直接检测该位置是否有魔能仪器的物块实体
                for (int j = baseY; j < baseY + yLength; j++)
                    for (int i = baseX; i < baseX + xLength; i++)
                    {
                        Point p = new Point(i, j);
                        BrillantRubiksCube.PlaceBarrier(Item, p);
                    }
        }

        public override Color GetDrawColor() => Coralite.CrystallinePurple;
    }
}
