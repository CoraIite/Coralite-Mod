﻿using Coralite.Content.Items.Materials;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Base;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Columns
{
    public class CrimtaneColumn : BaseMagikePlaceableItem,IMagikePolymerizable
    {
        public CrimtaneColumn() : base(TileType<CrimtaneColumnTile>(), Item.sellPrice(0, 0, 50, 0), RarityType<MagicCrystalRarity>(), 50)
        { }

        public void AddMagikePolymerizeRecipe()
        {
            PolymerizeRecipe.CreateRecipe<CrimtaneColumn>(75)
                .SetMainItem<CrystalColumn>()
                .AddIngredient<GlistentBar>(4)
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.TissueSample, 8)
                .Register();
        }
    }

    public class CrimtaneColumnTile : BaseColumnTile
    {
        public override void SetStaticDefaults()
        {
            //Main.tileShine[Type] = 400;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[3] {
                16,
                16,
                16
            };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<CrimtaneColumnEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Color.Red);
            DustType = DustID.Crimson;
        }
    }

    public class CrimtaneColumnEntity : MagikeSender_Line
    {
        public const int sendDelay = 4 * 60 + 45;
        public int sendTimer;
        public CrimtaneColumnEntity() : base(750, 5 * 16) { }

        public override ushort TileType => (ushort)TileType<CrimtaneColumnTile>();

        public override int HowManyPerSend => 30;

        public override bool CanSend()
        {
            sendTimer++;
            if (sendTimer > sendDelay)
            {
                sendTimer = 0;
                return true;
            }

            return false;
        }

        public override void SendVisualEffect(IMagikeContainer container)
        {
            MagikeHelper.SpawnDustOnSend(2, 3, Position, container, Color.Red);
        }

        public override void OnReceiveVisualEffect()
        {
            MagikeHelper.SpawnDustOnGenerate(2, 3, Position, Color.Red);
        }
    }
}