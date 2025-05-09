﻿using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect(ExtraEffectNames = [Vanity])]
    public class CharmOfIsis : BaseAccessory, ISpecialDrawHead
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public Vector2 ExtraOffset => new Vector2(0, 8);

        public const string Vanity = nameof(CharmOfIsis) + "Vanity";

        public CharmOfIsis() : base(ItemRarityID.Pink, Item.sellPrice(0, 6, 0, 0))
        {
        }

        public override void SetStaticDefaults()
        {
            int slot = EquipLoader.GetEquipSlot(Mod, nameof(CharmOfIsis), EquipType.Head);
            //ArmorIDs.Head.Sets.DrawHatHair[slot] = true;
            ArmorIDs.Head.Sets.DrawFullHair[slot] = true;
        }

        public override void AutoDefaults() { }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.lifeRegen = 2;
        }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<MedalOfLife>())//下位
                && incomingItem.type == ModContent.ItemType<CharmOfIsis>());
        }

        public override void UpdateVanity(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(Vanity);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(CharmOfIsis));
                if (!hideVisual)
                    cp.AddEffect(Vanity);
            }

            player.pStone = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<MedalOfLife>()
                .AddIngredient(ItemID.CharmofMyths)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
