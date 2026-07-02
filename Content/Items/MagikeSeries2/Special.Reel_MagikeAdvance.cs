using Coralite.Content.CoraliteNotes.MagikeChapter2;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Coralite.Core.Systems.MagikeSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class Reel_MagikeAdvance : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(3, 17));
        }

        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = CoraliteSoundID.IceMagic_Item28;
        }

        public override bool CanUseItem(Player player)
        {
            //CanUseItem 只在持有者端运行，服务端不会替远程玩家执行；不能再用 !isClient 包裹否则多人下解锁不了。
            //无条件 SetAndSync：单人/服务端直接写入广播，客户端发单 flag 请求（LearnedMagikeAdvanced 已允许客户端请求）。
            if (!ModContent.GetInstance<LearnedMagikeAdvanced>().Value)
                ModContent.GetInstance<LearnedMagikeAdvanced>().SetAndSync(true);
            KnowledgeSystem.CheckForUnlock<MagikeS2Knowledge>(Coralite.CrystallinePurple);

            return base.CanUseItem(player);
        }
    }
}
