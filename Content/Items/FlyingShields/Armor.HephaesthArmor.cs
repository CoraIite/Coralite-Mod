using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.FlyingShields
{
    [AutoloadEquip(
    [
        EquipType.Head
    ])]
    public class HephaesthHelmet : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<SoulOfDeveloper, HephaesthHelmet>(1000)
                .AddConditions(CoraliteConditions.FullCollectHephaesth)
                .Register();
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;

            Item.value = Item.sellPrice(0, 11, 0, 0);
            Item.rare = ItemRarityID.Purple;

            Item.vanity = true;
        }
    }

    [AutoloadEquip(
    [
        EquipType.Body
    ])]
    [PlayerEffect]
    public class HephaesthChestplate : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        //public static short GlowMaskID;

        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.showsShouldersWhileJumping[Item.bodySlot] = true;

            //Array.Resize(ref TextureAssets.GlowMask, TextureAssets.GlowMask.Length + 1);
            //TextureAssets.GlowMask[^1] = ModContent.Request<Texture2D>(Texture + "_Body_Glow");
            //GlowMaskID = (short)(TextureAssets.GlowMask.Length - 1);
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 22;

            Item.value = Item.sellPrice(0, 21, 0, 0);
            Item.rare = ItemRarityID.Purple;

            Item.vanity = true;
        }

        public override void PreUpdateVanitySet(Player player)
        {
        }

        public override bool IsVanitySet(int head, int body, int legs)
            => true;

        public override void UpdateVanitySet(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddEffect(nameof(HephaesthChestplate));
                Lighting.AddLight(player.Center, new Vector3(0.35f, 0.15f, 0.1f));
            }
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<SoulOfDeveloper, HephaesthChestplate>(1000)
                .AddConditions(CoraliteConditions.FullCollectHephaesth)
                .Register();
        }

        //public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
        //{
        //    glowMask = GlowMaskID;
        //    glowMaskColor = Color.White * (0.9f + MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
        //}
    }

    [AutoloadEquip(
    [
        EquipType.Legs
    ])]
    public class HephaesthGreave : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.FlyingShieldItems + Name;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 16;

            Item.value = Item.sellPrice(0, 14, 0, 0);
            Item.rare = ItemRarityID.Purple;

            Item.vanity = true;
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<SoulOfDeveloper, HephaesthGreave>(1000)
                .AddConditions(CoraliteConditions.FullCollectHephaesth)
                .Register();
        }
    }
}
