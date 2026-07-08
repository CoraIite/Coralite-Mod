using Coralite.Content.DamageClasses;
using Coralite.Content.Dusts;
using Coralite.Content.GlobalItems;
using Coralite.Content.ModPlayers;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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

        public override void SetDefaults()
        {
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<BrillantRubiksCubeHeldProj>(), 18, 14, true);
            Item.DamageType = MagikeDamage.Instance;
            Item.SetWeaponValues(40, 12f, 0);
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 4);

            Item.useStyle = ItemUseStyleID.Rapier;

            Item.useTurn = false;
            Item.noUseGraphic = true;

            Item.GetMagikeItem().MagikeMax = 15000;
            if (Item.TryGetGlobalItem(out CoraliteGlobalItem cgi))
                cgi.SpecialUse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BrillantRubiksCubeHeldProj>()] < 1)
                {
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                        , player.Center, Vector2.Zero, ModContent.ProjectileType<BrillantRubiksCubeHeldProj>(), 0, 0, player.whoAmI);
                }
            }
        }

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
            Point p = Main.MouseWorld.ToTileCoordinates();

            if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.useSpecialAttack)
            {
                switch (style)
                {
                    case AttackStyle.SinglePoint:
                        Tile t = Framing.GetTileSafely(p);
                        if (t.TileType == ModContent.TileType<CrystallineBarrier>())
                            WorldGen.KillTile(p.X, p.Y);
                        break;
                    case AttackStyle.Area:
                        Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<BrillantRubiksCubeArea>(), 0, 0, player.whoAmI, ai2: 1);
                        break;
                    case AttackStyle.Magic:
                        MagicShoot(player, Item, source, position, ModContent.ProjectileType<BrillantRubiksMagicProj>(), damage, knockback);
                        break;
                    default:
                        break;
                }

                return false;
            }

            if (player.altFunctionUse == 2)
            {
                style++;
                if (style > AttackStyle.Magic)
                    style = AttackStyle.SinglePoint;

                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.CrystallinePurple,
                    Text = style switch
                    {
                        AttackStyle.SinglePoint => SingleText.Value,
                        AttackStyle.Area => AreaText.Value,
                        _ => MagicText.Value,
                    },
                    DurationInFrames = 90,
                    Velocity = -Vector2.UnitY
                }, player.Center);

                return false;
            }

            switch (style)
            {
                case AttackStyle.SinglePoint:
                    Helper.PlayPitched(CoraliteSoundID.MagicStaff_Item8, player.Center, 0.2f);
                    PlaceBarrier(Item, p);
                    break;
                case AttackStyle.Area:
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<BrillantRubiksCubeArea>(), 0, 0, player.whoAmI, ai2: 0);
                    break;
                case AttackStyle.Magic:
                    MagicShoot(player, Item, source, position, ModContent.ProjectileType<BrillantRubiksMagicProj>(), damage, knockback);
                    break;
                default:
                    break;
            }

            return false;
        }

        public static void PlaceBarrier(Item item, Point p)
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

        public static void MagicShoot(Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, int type, int damage, float knockback)
        {
            for (int i = 0; i < 4; i++)
            {
                if (MagikeHelper.TryCosumeMagike(item, 1))
                {
                    Point pos = Main.MouseWorld.ToTileCoordinates() + new Point(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2));

                    position += Main.rand.NextVector2Circular(35, 35);

                    Projectile.NewProjectile(source, position, (pos.ToWorldCoordinates() - position).SafeNormalize(Vector2.Zero) * 16, type, damage, knockback, player.whoAmI, pos.X, pos.Y);
                }
            }

            Helper.PlayPitched(CoraliteSoundID.IceMagic_Item28, player.Center, 0.2f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "Coralite:BrilliantRubiksCubeTips", style switch
            {
                AttackStyle.SinglePoint => SingleText.Value,
                AttackStyle.Area => AreaText.Value,
                _ => MagicText.Value
            }));
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Loot && Item.TryGetGlobalItem(out MagikeItem mi))
                mi.FullChargeMagike();
        }
    }

    public class BrillantRubiksCubeHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public ref float State => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;
            Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;

            float x = 32;

            switch (State)
            {
                default:
                case 0:
                    {
                        if (++Projectile.frameCounter > 3)
                        {
                            Projectile.frame++;
                            if (Projectile.frame > 13)
                            {
                                State = 1;
                            }
                        }

                        x = Projectile.frame / 13f * 32;
                    }
                    break;
                case 1:
                    {
                        if (Owner.itemAnimation != 0 || Projectile.frame != 13)
                        {
                            Projectile.UpdateFrameNormally(3, 13 + 6, false, 13);
                        }

                        if (Item.type != ModContent.ItemType<BrillantRubiksCube>())
                        {
                            State = 2;
                            Projectile.frame = 13 + 6;
                            Projectile.frameCounter = 0;
                        }

                    }
                    break;
                case 2:
                    {
                        for (int k = 0; k < 2; k++)
                            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BarrierDust>(), Helper.NextVec2Dir(1f, 2f));

                        if (++Projectile.frameCounter > 3)
                        {
                            Projectile.frame++;
                            if (Projectile.frame > 13 + 6 + 5)
                            {
                                Projectile.Kill();
                            }
                        }

                        x = (1 - (Projectile.frame - 13 - 6) / 5f) * 32;

                    }
                    break;
            }

            Projectile.Center = Owner.MountedCenter + new Vector2(Owner.direction * x, Owner.gfxOffY);

            Lighting.AddLight(Projectile.Center, Coralite.CrystallinePurple.ToVector3() * 0.75f);

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemRotation = (Owner.gravDir > 0 ? 0f : MathHelper.Pi);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickFrameDraw(new Rectangle(0, Projectile.frame, 1, 25), lightColor, 0);

            return false;
        }
    }

    public class BrillantRubiksMagicProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public ref float TargetX => ref Projectile.ai[0];
        public ref float TargetY => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float Alpha => ref Projectile.localAI[1];

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = MagikeDamage.Instance;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 60 * 20;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                    break;
                case 0:
                    Alpha = 1;
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
                            Timer = 0;

                            Point p = Projectile.Center.ToTileCoordinates();

                            Tile t = Framing.GetTileSafely(p);
                            if (t.HasTile)//有物块直接紫砂
                            {
                                //if (t.TileType== ModContent.TileType<CrystallineBarrier>())
                                {
                                    State = 3;
                                    Projectile.timeLeft = 30;
                                    Projectile.tileCollide = false;
                                    Projectile.velocity = new Vector2(0, -12);
                                    return;
                                }

                                //Projectile.Kill();
                                //return;
                            }

                            WorldGen.PlaceTile(p.X, p.Y, ModContent.TileType<CrystallineBarrier>(), true);

                            Projectile.velocity = Vector2.Zero;
                            Projectile.Center = p.ToWorldCoordinates();
                            TargetX = p.X;
                            TargetY = p.Y;
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
                case 3:
                    {
                        Projectile.rotation += 0.1f;
                        Alpha = Projectile.timeLeft / 30;
                    }
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Point p = (Projectile.Center).ToTileCoordinates();
            Point p2 = (Projectile.Center + oldVelocity).ToTileCoordinates();
            //Tile t = Framing.GetTileSafely(p);
            //Tile t2 = Framing.GetTileSafely(p2);
            //if (t2.HasTile && t2.TileType == ModContent.TileType<CrystallineBarrier>())//有物块直接紫砂
            {
                State = 3;
                Projectile.timeLeft = 30;
                Projectile.tileCollide = false;
                Projectile.velocity = new Vector2(0, -12);
                return false;
            }

            //if (t.HasTile)
            //{
            //    return true;
            //}

            //WorldGen.PlaceTile(p.X, p.Y, ModContent.TileType<CrystallineBarrier>(), true);
            //Projectile.velocity = Vector2.Zero;
            //Projectile.Center = Projectile.Center.ToTileCoordinates().ToWorldCoordinates();
            //State = 2;
            //TargetX = p.X;
            //TargetY = p.Y;

            //return tr;
        }

        public override void OnKill(int timeLeft)
        {
            PRTLoader.NewParticle<BarrierShineParticle>(Projectile.Center, Vector2.Zero, Color.White);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (State != 2)
            {
                Texture2D tex = Projectile.GetTextureValue();

                Rectangle box = tex.Frame(2, 1, 0, 0);
                Vector2 origin = box.Size() / 2;
                Vector2 pos = Projectile.Center - Main.screenPosition;

                Color selfC = Color.Lerp(Color.White * 0.9f, Color.White * 0.2f, MathF.Cos(Main.GlobalTimeWrappedHourly * 2) / 2 + 0.5f) * Alpha;
                Main.spriteBatch.Draw(tex, pos, box, Color.White * 0.5f, Projectile.rotation, origin, 1, 0, 0);

                Color c2 = selfC * 0.5f;
                c2.A = 0;

                for (int k = 0; k < 4; k++)
                {
                    Vector2 off = (Main.GlobalTimeWrappedHourly + k * MathHelper.PiOver4).ToRotationVector2() * 2;
                    Main.spriteBatch.Draw(tex, pos + off, box, c2, Projectile.rotation, origin, 1, 0, 0);
                }

                Main.spriteBatch.Draw(tex, pos, box, Color.White * 0.4f * Alpha, Projectile.rotation, origin, 1, 0, 0);

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

            bool isUsing = UseType == 1 ? Owner.GetModPlayer<CoralitePlayer>().useSpecialAttack : DownLeft;

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
            {
                Helper.PlayPitched(CoraliteSoundID.MagicStaff_Item8, Owner.Center, 0.2f);
                //遍历一个矩形区域，并直接检测该位置是否有魔能仪器的物块实体
                for (int j = baseY; j < baseY + yLength; j++)
                    for (int i = baseX; i < baseX + xLength; i++)
                    {
                        Point p = new Point(i, j);
                        BrillantRubiksCube.PlaceBarrier(Item, p);
                    }
            }
        }

        public override Color GetDrawColor() => Coralite.CrystallinePurple;
    }
}
