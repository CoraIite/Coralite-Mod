using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Gels
{
    [AutoloadEquip(EquipType.Legs)]
    [PlayerEffect(ExtraEffectNames = [AttackSet, DefenceSet])]
    public class EmperorSlimeBoots : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.GelItems + Name;

        public const string AttackSet = "EmperorSlimeBootsA";
        public const string DefenceSet = "EmperorSlimeBootsB";

        public static LocalizedText bonus0;
        public static LocalizedText bonus1;

        private enum ArmorSetType
        {
            GelFiber,
            Ninja
        }

        public override void Load()
        {
            bonus0 = this.GetLocalization("ArmorBonus0");
            bonus1 = this.GetLocalization("ArmorBonus1");
        }

        public override void Unload()
        {
            bonus0 = null;
            bonus1 = null;
        }

        public void AddMagikeCraftRecipe()
        {

        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(EmperorSlimeBoots));
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => CheckArmorSet(head, body, out _);

        private static bool CheckArmorSet(Item head, Item body, out ArmorSetType? type)
        {
            type = null;
            if (head.type == ItemType<GelFiberHelmet>() && body.type == ItemType<GelFiberBreastplate>())
            {
                type = ArmorSetType.GelFiber;
                return true;
            }

            if (head.type == ItemID.NinjaHood && body.type == ItemID.NinjaShirt)
            {
                type = ArmorSetType.Ninja;
                return true;
            }

            return false;
        }

        public override void UpdateArmorSet(Player player)
        {
            CheckArmorSet(player.HeadArmor(), player.BodyArmor(), out ArmorSetType? type);

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                switch (type.Value)
                {
                    case ArmorSetType.GelFiber:
                        player.setBonus = bonus0.Value;
                        break;
                    case ArmorSetType.Ninja:
                        player.setBonus = bonus1.Value;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
