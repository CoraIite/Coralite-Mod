using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Items.FairyCatcher
{
    public class ElfPortal() : BasePlaceableItem(Item.sellPrice(0, 0, 10), ItemRarityID.Green, ModContent.TileType<ElfPortalTile>(), AssetDirectory.FairyCatcherItems)
    {
        public override void AddRecipes()
        {
            //CreateRecipe()
            //    .AddIngredient<BotanicusPowder>(10)
            //    .AddIngredient<HardBasalt>(6)
            //    .AddTile<MagicCraftStation>()
            //    .Register();
        }
    }

    public class ElfBless : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                fcp.fairyCatchPowerBonus += 0.05f;
        }
    }

    [VaultLoaden(AssetDirectory.FairyCatcherItems)]
    public class ElfPortalTile : ModTile
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        [VaultLoaden("{@classPath}" + "ElfPortalEye")]
        public static ATex EyeTex { get; set; }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Width = 12;
            TileObjectData.newTile.Height = 11;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16];

            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(6, 10);

            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(Color.Purple);

            AnimationFrameHeight = 11 * 18;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.25f;
            g = 0.1f;
            b = 0.4f;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer)
                return;

            //Main.LocalPlayer.AddBuff(ModContent.BuffType<ElfBless>(), 60);

            Tile t = Framing.GetTileSafely(i, j);
            if (t.TileFrameX != 0 || t.TileFrameY != 0)
                return;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            SpawnFairy(i, j);
        }

        public static void SpawnFairy(int i, int j)
        {
            for (int k = 0; k < Main.maxItems; k++)
            {
                Item item = Main.item[k];

                if (item == null || item.IsAir || item.timeSinceItemSpawned < 60
                    || item.velocity != Vector2.Zero || item.shimmered)
                    continue;

                Vector2 pos = (new Vector2(i, j) * 16) + new Vector2((16 * 6) + 4, (16 * 6) + 2);

                if (Vector2.Distance(pos, item.Center) > 16 * 40)
                    continue;

                if (FairySystem.TryGetElfPortalTrades(item.type, out _))
                {
                    int p = Projectile.NewProjectile(new EntitySource_TileUpdate(i, j), pos, Helper.NextVec2Dir(2, 3), ModContent.ProjectileType<ElfTradeProj>()
                         , 0, 0, ai0: item.type, ai1: item.stack);

                    (Main.projectile[p].ModProjectile as ElfTradeProj).itemCenter = item.Center;
                    Main.item[k].TurnToAir();
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ShimmerArrow, new ParticleOrchestraSettings
                    {
                        PositionInWorld = pos,
                        MovementVector = Vector2.Zero
                    });

                    Helper.PlayPitched(CoraliteSoundID.Fairy_NPCHit5, pos);
                }
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter > 3)
            {
                frameCounter = 0;
                if (++frame > 35)
                    frame = 0;
            }
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            if (drawData.tileFrameX % (12 * 16) == 0 && drawData.tileFrameY % AnimationFrameHeight == 0)
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Vector2 offScreen = new(Main.offScreenRange);
            if (Main.drawToScreen)
                offScreen = Vector2.Zero;

            Point p = new(i, j);
            Tile tile = Main.tile[p.X, p.Y];
            if (tile == null || !tile.HasTile)
                return;

            Texture2D texture = EyeTex.Value;

            Vector2 origin = EyeTex.Size() / 2f;
            Vector2 worldPos = p.ToWorldCoordinates(0, 0);

            const float TwoPi = (float)Math.PI * 2f;
            float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
            Vector2 toCenter = new(98f, 92);
            Vector2 drawPos = worldPos + offScreen - Main.screenPosition + toCenter + new Vector2(0f, offset * 4f);

            Vector2 offset2 = Vector2.Zero;

            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];

                if (player.active && !player.dead)
                {
                    float dis = Vector2.Distance(player.Center, worldPos + toCenter);
                    if (dis < 1000)
                    {
                        offset2 = (player.Center - (worldPos + toCenter)).SafeNormalize(Vector2.Zero) * dis / 50;
                        break;
                    }
                }
            }

            drawPos += offset2;

            // 绘制主帖图
            spriteBatch.Draw(texture, drawPos, null, Color.White, 0f, origin, 0.4f, 0, 0f);
        }
    }

    public class ElfPortalTile2 : ModTile
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        //[VaultLoaden("{@classPath}" + "ElfPortalEye")]
        //public static ATex EyeTex { get; set; }

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsSandfall[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.CoordinatePadding = 0;
            TileObjectData.newTile.Width = 22;
            TileObjectData.newTile.Height = 11;
            TileObjectData.newTile.CoordinateHeights = new int[22];
            Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);

            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(6, 10);

            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(Color.Purple);

            AnimationFrameHeight = 11 * 16;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.25f;
            g = 0.1f;
            b = 0.4f;
        }

        //public override void NearbyEffects(int i, int j, bool closer)
        //{
        //    if (!closer)
        //        return;

        //    //Main.LocalPlayer.AddBuff(ModContent.BuffType<ElfBless>(), 60);

        //    Tile t = Framing.GetTileSafely(i, j);
        //    if (t.TileFrameX != 0 || t.TileFrameY != 0)
        //        return;

        //    if (Main.netMode == NetmodeID.MultiplayerClient)
        //        return;

        //    SpawnFairy(i, j);
        //}

        public static void SpawnFairy(int i, int j)
        {
            for (int k = 0; k < Main.maxItems; k++)
            {
                Item item = Main.item[k];

                if (item == null || item.IsAir || item.timeSinceItemSpawned < 60
                    || item.velocity != Vector2.Zero || item.shimmered)
                    continue;

                Vector2 pos = (new Vector2(i, j) * 16) + new Vector2((16 * 6) + 4, (16 * 6) + 2);

                if (Vector2.Distance(pos, item.Center) > 16 * 40)
                    continue;

                if (FairySystem.TryGetElfPortalTrades(item.type, out _))
                {
                    int p = Projectile.NewProjectile(new EntitySource_TileUpdate(i, j), pos, Helper.NextVec2Dir(2, 3), ModContent.ProjectileType<ElfTradeProj>()
                         , 0, 0, ai0: item.type, ai1: item.stack);

                    (Main.projectile[p].ModProjectile as ElfTradeProj).itemCenter = item.Center;
                    Main.item[k].TurnToAir();
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ShimmerArrow, new ParticleOrchestraSettings
                    {
                        PositionInWorld = pos,
                        MovementVector = Vector2.Zero
                    });

                    Helper.PlayPitched(CoraliteSoundID.Fairy_NPCHit5, pos);
                }
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (++frameCounter > 3)
            {
                frameCounter = 0;
                if (++frame > 29)
                    frame = 0;
            }
        }

        //public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        //{
        //    if (drawData.tileFrameX % (12 * 16) == 0 && drawData.tileFrameY % AnimationFrameHeight == 0)
        //        Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        //}

        //public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        //{
        //    Vector2 offScreen = new(Main.offScreenRange);
        //    if (Main.drawToScreen)
        //        offScreen = Vector2.Zero;

        //    Point p = new(i, j);
        //    Tile tile = Main.tile[p.X, p.Y];
        //    if (tile == null || !tile.HasTile)
        //        return;

        //    Texture2D texture = EyeTex.Value;

        //    Vector2 origin = EyeTex.Size() / 2f;
        //    Vector2 worldPos = p.ToWorldCoordinates(0, 0);

        //    const float TwoPi = (float)Math.PI * 2f;
        //    float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
        //    Vector2 toCenter = new(98f, 92);
        //    Vector2 drawPos = worldPos + offScreen - Main.screenPosition + toCenter + new Vector2(0f, offset * 4f);

        //    Vector2 offset2 = Vector2.Zero;

        //    for (int k = 0; k < Main.maxPlayers; k++)
        //    {
        //        Player player = Main.player[k];

        //        if (player.active && !player.dead)
        //        {
        //            float dis = Vector2.Distance(player.Center, worldPos + toCenter);
        //            if (dis < 1000)
        //            {
        //                offset2 = (player.Center - (worldPos + toCenter)).SafeNormalize(Vector2.Zero) * dis / 50;
        //                break;
        //            }
        //        }
        //    }

        //    drawPos += offset2;

        //    // 绘制主帖图
        //    spriteBatch.Draw(texture, drawPos, null, Color.White, 0f, origin, 0.4f, 0, 0f);
        //}
    }

    public class ElfTradeProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float ItemType => ref Projectile.ai[0];
        public ref float ItemStack => ref Projectile.ai[1];
        public ref float State => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[1];

        public Vector2 itemCenter;
        public Vector2 PortalCenter;
        private float alpha = 1;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 30);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                PortalCenter = Projectile.Center;
                Projectile.localAI[0] = 1;
            }

            Projectile.UpdateFrameNormally(8, 4);

            if (State == 0)
            {
                Timer++;
                if (Timer < 40)
                {
                    Projectile.velocity *= 0.97f;
                }
                else
                {
                    Vector2 targetPos = itemCenter + new Vector2(0, -20);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 1.5f, 0.02f);
                    if (Vector2.Distance(targetPos, Projectile.Center) < 6)
                    {
                        Projectile.Center = targetPos;
                        State = 1;
                    }
                }
            }
            else if (State == 1)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (PortalCenter - Projectile.Center).SafeNormalize(Vector2.Zero) * 2, 0.02f);
                itemCenter = Projectile.Center + Projectile.velocity + new Vector2(0, 20);
                if (Vector2.Distance(PortalCenter, Projectile.Center) < 4)
                {
                    Projectile.velocity *= 0.1f;
                    State = 2;
                }
            }
            else
            {
                alpha -= 0.025f;
                if (alpha < 0.1f)
                {
                    FairySystem.ElfTrade((int)ItemType, (int)ItemStack, PortalCenter);
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.ShimmerArrow, new ParticleOrchestraSettings
                    {
                        PositionInWorld = Projectile.Center,
                        MovementVector = Vector2.Zero
                    });

                    Helper.PlayPitched(CoraliteSoundID.FairyDeath_NPCDeath7, Projectile.Center);

                    Projectile.Kill();
                }
            }

            Projectile.rotation = Projectile.velocity.X * 0.3f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制物品
            DrawItem();
            //绘制自己
            DrawSelf();

            return false;
        }

        public void DrawItem()
        {
            Vector2 center = itemCenter - Main.screenPosition;

            int itemType = (int)ItemType;
            Main.instance.LoadItem(itemType);
            Texture2D mainTex = TextureAssets.Item[itemType].Value;
            Rectangle rectangle2;
            Color c = Lighting.GetColor(itemCenter.ToTileCoordinates()) * alpha;

            if (Main.itemAnimations[itemType] != null)
                rectangle2 = Main.itemAnimations[itemType].GetFrame(mainTex, -1);
            else
                rectangle2 = mainTex.Frame();

            float itemScale = 1f;

            Main.spriteBatch.Draw(mainTex, center, new Rectangle?(rectangle2), c, 0f, rectangle2.Size() / 2, itemScale, 0, 0f);
        }

        public void DrawSelf()
        {
            Texture2D mainTex = TextureAssets.Npc[NPCID.Shimmerfly].Value;
            Vector2 vector = Projectile.Center - Main.screenPosition;
            int num = 5;
            int horizontalFrames = 4;
            float num2 = ((Projectile.whoAmI * 0.11f) + ((float)Main.timeForVisualEffects / 360f)) % 1f;
            Color color = Main.hslToRgb(num2, 1f, 0.65f);
            color.A /= 2;
            float rotation = Projectile.rotation;
            Rectangle rectangle = mainTex.Frame(horizontalFrames, num, 0, Projectile.frame);
            Vector2 origin = rectangle.Size() / 2f;
            float scale = Projectile.scale;
            Rectangle value2 = mainTex.Frame(horizontalFrames, num, 2);
            Color color2 = new Color(255, 255, 255, 0) * 1f;
            int num3 = Projectile.oldPos.Length;
            int num4 = num3 - 1 - 5;
            int num5 = 5;
            int num6 = 3;
            float num7 = 32f;
            float num8 = 16f;
            float fromMax = new Vector2(num7, num8).Length();
            float num9 = Utils.Remap(Vector2.Distance(Projectile.oldPos[num4], Projectile.position), 0f, fromMax, 0f, 100f);
            num9 = (int)num9 / 5;
            num9 *= 5f;
            num9 /= 100f;
            num8 *= num9;
            num7 *= num9;
            float num10 = 9f;
            float num11 = 0.5f;
            float num12 = (float)Math.PI;
            for (int i = num4; i >= num5; i -= num6)
            {
                Vector2 vector2 = Projectile.oldPos[i] - Projectile.position;
                float num14 = Utils.Remap(i, 0f, num3, 1f, 0f);
                float num15 = 1f - num14;
                Vector2 spinningpoint = new Vector2((float)Math.Sin((double)(Projectile.whoAmI / 17f) + (Main.timeForVisualEffects / (double)num10) + (double)(num14 * 2f * ((float)Math.PI * 2f))) * num8, 0f - num7) * num15;
                vector2 += spinningpoint.RotatedBy(num12);
                Color color3 = Main.hslToRgb((num2 + (num15 * num11)) % 1f, 1f, 0.5f);
                color3.A = 0;
                Main.spriteBatch.Draw(mainTex, vector + vector2, value2, color3 * num14 * 0.16f, rotation, origin, scale * Utils.Remap(num14 * num14, 0f, 1f, 0f, 2.5f), 0, 0f);
            }

            Main.spriteBatch.Draw(mainTex, vector, value2, color2, rotation, origin, scale, 0, 0f);
            Rectangle value3 = mainTex.Frame(horizontalFrames, num, 1, Projectile.frame);
            Color white = Color.White;
            white.A /= 2;
            Main.spriteBatch.Draw(mainTex, vector, value3, white, rotation, origin, scale, 0, 0f);
            Main.spriteBatch.Draw(mainTex, vector, rectangle, color, rotation, origin, scale, 0, 0f);
            float num16 = MathHelper.Clamp(((float)Math.Sin(Main.timeForVisualEffects / 60.0) * 0.3f) + 0.3f, 0f, 1f);
            float num17 = 0.8f + ((float)Math.Sin(Main.timeForVisualEffects / 15.0 * 6.2831854820251465) * 0.3f);
            Rectangle value4 = mainTex.Frame(horizontalFrames, num, 3, Projectile.whoAmI % num);
            Color color4 = Color.Lerp(color, new Color(255, 255, 255, 0), 0.5f) * num16;
            Main.spriteBatch.Draw(mainTex, vector, value4, color4, rotation, origin, scale * num17, SpriteEffects.None, 0f);
            Rectangle value5 = mainTex.Frame(horizontalFrames, num, 3, 1);
            Color color5 = Color.Lerp(color, new Color(255, 255, 255, 0), 0.5f) * num16;
            Main.spriteBatch.Draw(mainTex, vector, value5, color5, rotation, origin, scale * num17, SpriteEffects.None, 0f);
        }
    }
}
