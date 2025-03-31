using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    [AutoloadEquip(EquipType.Wings)]
    public class ChalcedonyWing() : BaseAccessory(ItemRarityID.LightRed, Item.sellPrice(0, 2))
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(130, 6.75f, 1.2f);
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.6f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 0.6f;
            maxAscentMultiplier = 1.6f;
            constantAscend = 0.1f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetJumpState<ChalcedonyDoubleJump>().Enable();
        }
    }

    public class ChalcedonyDoubleJump : ExtraJump
    {
        public override Position GetDefaultPosition()
        {
            return new After(CloudInABottle);
        }

        public override float GetDurationMultiplier(Player player)
        {
            return 3;
        }

        public override void OnEnded(Player player)
        {
            base.OnEnded(player);
        }

        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= 1.5f;
            player.maxRunSpeed *= 1.25f;
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
        }
    }
}
