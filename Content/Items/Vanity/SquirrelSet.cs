using Coralite.Content.CustomHooks;
using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Content.Items.Vanity
{
    [AutoloadEquip(EquipType.Head, EquipType.Body, EquipType.Legs, EquipType.Neck, EquipType.Back)]
    public class SquirrelSet : ModItem, ISpecialDrawBackpacks, IMagikeRemodelable
    {
        public override string Texture => AssetDirectory.Vanity + Name;

        public bool Special;

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.PreventBeardDraw[EquipLoader.GetEquipSlot(Mod, nameof(SquirrelSet), EquipType.Head)] = true;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 20;
            Item.UseSound = CoraliteSoundID.SummonStaff_Item44;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 0, 50);
        }

        public override void AutoDefaults() { }

        public override bool CanUseItem(Player player)
        {
            if (Special)
                Special = false;
            else
                Special = true;

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hideVisual)
                AddEffect(player);
        }

        public override void UpdateVanity(Player player)
        {
            AddEffect(player);
        }

        public void AddEffect(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(SquirrelSet));

            if (Special)
                cp.AddEffect(nameof(SquirrelSet) + "Special");
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("Special", Special);
        }

        public override void LoadData(TagCompound tag)
        {
            Special = tag.GetBool("Special");
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddCondition(this.GetLocalization("ShlimmerTranslation", () => "此为微光转化合成表"), () => false)
                .AddCustomShimmerResult(ItemID.Squirrel)
                .AddCustomShimmerResult(ItemID.Chest, 2)
                .AddCustomShimmerResult(ItemID.RedDye)
                .Register();
        }

        public void AddMagikeRemodelRecipe()
        {
            MagikeSystem.AddRemodelRecipe<SoulOfDeveloper, SquirrelSet>(1000);
        }
    }
}
