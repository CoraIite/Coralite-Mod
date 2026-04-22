using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.IceDragonChapter1;
using Coralite.Content.Dusts;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using InnoVault.PRT;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.Icicle
{
    [AutoloadEquip(EquipType.Wings)]
    public class BabyIceWing() : BaseAccessory(ItemRarityID.Green, Item.sellPrice(0, 2)), IConsultableItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<IceDragon1Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<IciclePage1>();

        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(10, 3f);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetJumpState<BabyIceWingJump>().Enable();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<IcicleCrystal>(3)
                .AddIngredient<IcicleScale>(5)
                .AddTile(TileID.IceMachine)
                .Register();
        }
    }


    public class BabyIceWingPlayer : ModPlayer
    {
        /// <summary>
        /// <see cref="BabyIceWingJump"/> 的剩余跳跃次数
        /// </summary>
        public int MyExtraJumpCount { get; private set; }
        /// <summary>
        /// <see cref="BabyIceWingJump"/> 的跳跃次数最大值
        /// </summary>
        private const int MyExtraJumpCountMax = 3; 
        /// <summary>
        /// 能否使用 <see cref="BabyIceWingJump"/>
        /// </summary>
        public bool CanUseBabyIceWingJump => MyExtraJumpCount > 0;
        /// <summary>
        /// <see cref="BabyIceWingJump"/> 是否已经消耗过了
        /// </summary>
        public bool BabyIceWingJumpCosted => MyExtraJumpCount != MyExtraJumpCountMax;
        /// <summary>
        /// 重置 <see cref="BabyIceWingJump"/> 的跳跃次数
        /// </summary>
        public void ResetBabyIceWingJump()
            => MyExtraJumpCount = MyExtraJumpCountMax;
        /// <summary>
        /// 消耗 <see cref="BabyIceWingJump"/> 的跳跃次数
        /// </summary>
        public void CostBabyIceWingJump()
        {
            if (MyExtraJumpCount > 0)
                MyExtraJumpCount--;
        }
    }

    public class BabyIceWingJump : ExtraJump
    {
        public override Position GetDefaultPosition()
        {
            return new Before(CloudInABottle);
        }

        public override float GetDurationMultiplier(Player player)
        {
            return 1f;
        }

        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= 1.25f;
            player.maxRunSpeed *= 1.25f;
        }

        public override bool CanStart(Player player)
        {
            return player.TryGetModPlayer(out BabyIceWingPlayer biwp) && biwp.CanUseBabyIceWingJump;
        }

        public override void OnStarted(Player player, ref bool playSound)
        {
            if (player.TryGetModPlayer(out BabyIceWingPlayer biwp))
            {
                biwp.CostBabyIceWingJump();//消耗跳跃次数
                if (biwp.CanUseBabyIceWingJump)
                    player.GetJumpState<BabyIceWingJump>().Available = true;//重新启用跳跃
            }

            WindCircle.Spawn(player.Center, -player.velocity * 0.2f, player.velocity.ToRotation(), Coralite.IcicleCyan
                , 0.6f, 1.1f, new Vector2(1.25f, 1f));
        }

        public override void OnRefreshed(Player player)
        {
            if (player.TryGetModPlayer(out BabyIceWingPlayer biwp))
                biwp.ResetBabyIceWingJump();
        }

        public override void ShowVisuals(Player player)
        {
            int height = player.height;
            if (player.gravDir == -1f)
                height = -6;

            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustDirect(new Vector2(player.position.X - 8f, player.position.Y + height), player.width + 16, 4
                    , DustID.ApprenticeStorm, (0f - player.velocity.X) * 0.6f, player.velocity.Y * 0.6f, 100, default, 1.5f);
                d.velocity.X = d.velocity.X * 0.5f - player.velocity.X * 0.1f;
                d.velocity.Y = d.velocity.Y * 0.5f - player.velocity.Y * 0.3f;

                var p = PRTLoader.NewParticle<PixelLine>(player.Center + Main.rand.NextVector2Circular(32, 24), player.velocity * Main.rand.NextFloat(-0.4f, 0.4f) , Coralite.IcicleCyan, Main.rand.NextFloat(1, 1.5f));

                p.TrailCount = Main.rand.Next(14, 20);
                p.fadeFactor = Main.rand.NextFloat(0.87f, 0.95f);
            }
        }
    }
}
