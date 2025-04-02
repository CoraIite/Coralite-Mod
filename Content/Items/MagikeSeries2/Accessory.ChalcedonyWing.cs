using Coralite.Content.Dusts;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using InnoVault.PRT;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    [AutoloadEquip(EquipType.Wings)]
    public class ChalcedonyWing() : BaseAccessory(ItemRarityID.LightRed, Item.sellPrice(0, 2)),IMagikeCraftable
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(95, 6.75f, 1.2f);
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.6f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 0.6f;
            maxAscentMultiplier = 1.55f;
            constantAscend = 0.1f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetJumpState<ChalcedonyDoubleJump>().Enable();
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<Chalcedony, ChalcedonyWing>(MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 12, 60 * 2)
                , 24)
                .AddIngredient<CrystallineMagike>(3)
                .AddIngredient(ItemID.SoulofFlight, 8)
                .AddIngredient(ItemID.CloudinaBottle)
                .Register();
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
            return 0.4f;
        }

        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= 3.8f;
            player.maxRunSpeed *= 2.8f;
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            //生成叶子


            WindCircle.Spawn(player.Center, -player.velocity * 0.2f, player.velocity.ToRotation(), Color.Silver
                , 0.6f, 1.1f, new Vector2(1.25f, 1f));
        }

        public override void ShowVisuals(Player player)
        {
            int height = player.height;
            if (player.gravDir == -1f)
                height = -6;

            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustDirect(new Vector2(player.position.X - 8f, player.position.Y + height), player.width + 16, 4
                    , DustID.Cloud, (0f - player.velocity.X) * 0.6f, player.velocity.Y * 0.6f, 100, default(Color), 1.5f);
                d.velocity.X = d.velocity.X * 0.5f - player.velocity.X * 0.1f;
                d.velocity.Y = d.velocity.Y * 0.5f - player.velocity.Y * 0.3f;

                var p = PRTLoader.NewParticle<PixelLine>(player.Center + Main.rand.NextVector2Circular(32, 24), player.velocity * Main.rand.NextFloat(-0.4f, 0.4f)
                     , new Color(155, 186, 144), Main.rand.NextFloat(1, 1.5f));

                p.TrailCount = Main.rand.Next(14, 20);
                p.fadeFactor = Main.rand.NextFloat(0.87f, 0.95f);
            }

            if (player.jump < 60 && player.jump % 4 == 0)
            {
                WindCircle.Spawn(player.Center, -player.velocity * (-0.22f + player.jump / 4 * 0.15f), player.velocity.ToRotation(), Color.Silver
                    , 0.6f, 1, new Vector2(1.2f, 0.4f + 0.2f * player.jump / 4));
            }
        }
    }
}
