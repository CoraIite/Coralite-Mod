using Coralite.Content.Items.Magike.OtherPlaceables;
using Coralite.Content.Items.Materials;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.FairyCatcherSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace Coralite.Content.Items.FairyCatcher
{
    public class ElfPortal() : BasePlaceableItem(Item.sellPrice(0, 0, 10), ItemRarityID.Green, ModContent.TileType<ElfPortalTile>(), AssetDirectory.FairyCatcherItems)
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BotanicusPowder>(10)
                .AddIngredient<HardBasalt>(6)
                .AddTile<MagicCraftStation>()
                .Register();
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

    public class ElfPortalTile : ModTile
    {
        public override string Texture => AssetDirectory.FairyCatcherItems + Name;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!closer)
                return;

            Main.LocalPlayer.AddBuff(ModContent.BuffType<ElfBless>(), 60);

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

                if (item == null || item.IsAir || item.timeSinceItemSpawned < 60)
                    continue;

                if (FairySystem.TryGetElfPortalTrades(item.type, out _))
                {
                    Vector2 pos = new Vector2(i, j) * 16 + new Vector2(16 * 3 / 2);
                    Projectile.NewProjectile(new EntitySource_TileUpdate(i, j), pos, Vector2.Zero, ModContent.ProjectileType<ElfTradeProj>()
                        , 0, 0, ai0: k);
                }
            }
        }
    }

    public class ElfTradeProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float ItemIndex => ref Projectile.ai[0];

        private Item item;
        private Vector2 itemPos;

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                item = Main.item[(int)ItemIndex];
                Projectile.localAI[0] = 1;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //绘制自己
            //绘制物品
            return false;
        }
    }
}
