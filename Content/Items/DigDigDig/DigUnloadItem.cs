using Coralite.Content.WorldGeneration;
using Coralite.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Items.DigDigDig
{
    public class DigUnloadItem : ModItem
    {
        public override string Texture => AssetDirectory.DigDigDigItem + Name;

        public Item containsItem;

        public static LocalizedText ContainsItemText { get;private set; }

        public override void Load()
        {
            if (!Main.dedServ)
                ContainsItemText = this.GetLocalization(nameof(ContainsItemText));
        }

        public override void Unload()
        {
            ContainsItemText = null;
        }

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(2, 30));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Pink;
        }

        public override bool CanStack(Item source) => false;
        public override bool CanStackInWorld(Item source) => false;

        public override void UpdateInventory(Player player)
        {
            if (containsItem == null)
            {
                Item.TurnToAir();
                return;
            }

            if (!CoraliteWorld.DigDigDigWorld)
            {
                player.QuickSpawnItem(new EntitySource_ItemUse(player, containsItem), containsItem);
                Item.TurnToAir();
            }
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (containsItem == null)
            {
                Item.TurnToAir();
                return;
            }

            if (!CoraliteWorld.DigDigDigWorld)
            {
                Item.NewItem(new EntitySource_Loot(containsItem), Item.position, containsItem);
                Item.TurnToAir();
            }
        }

        public override ModItem Clone(Item newEntity)
        {
            if (newEntity.ModItem is DigUnloadItem dig)
                dig.containsItem = containsItem;

            return Item.ModItem;
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("ContainsItem", containsItem);
        }

        public override void LoadData(TagCompound tag)
        {
            containsItem = tag.Get<Item>("ContainsItem");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (containsItem != null)
            {
                var line = new TooltipLine(Mod, nameof(ContainsItemText), ContainsItemText.Value + containsItem.Name);
                tooltips.Add(line);
            }
        }
    }
}
