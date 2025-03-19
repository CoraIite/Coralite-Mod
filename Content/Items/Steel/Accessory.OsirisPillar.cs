using Coralite.Content.CustomHooks;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Steel
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect(ExtraEffectNames = [nameof(OsirisPillar) + "Vanity"])]
    public class OsirisPillar : BaseAccessory, ISpecialDrawHead
    {
        public override string Texture => AssetDirectory.SteelItems + Name;

        public override void SetStaticDefaults()
        {
            int id = EquipLoader.GetEquipSlot(Mod, nameof(OsirisPillar), EquipType.Head);
            ArmorIDs.Head.Sets.DrawFullHair[id] = true;
        }

        public OsirisPillar() : base(ItemRarityID.Pink, Item.sellPrice(0, 4, 0, 0))
        {
        }

        public override void AutoDefaults() { }

        public override bool CanAccessoryBeEquippedWith(Item equippedItem, Item incomingItem, Player player)
        {
            return !((equippedItem.type == ModContent.ItemType<LifePulseDevice>())//下位
                && incomingItem.type == ModContent.ItemType<OsirisPillar>());
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(OsirisPillar));

                if (!hideVisual)
                    cp.AddEffect(nameof(OsirisPillar) + "Vanity");
            }

            player.statManaMax2 += 20;
            player.manaRegenDelayBonus += 1f;
            player.manaRegenBonus += 30;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LifePulseDevice>()
                .AddIngredient(ItemID.ManaRegenerationBand)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
