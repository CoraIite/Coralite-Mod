using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.BossSystems
{
    public partial class DownedBossSystem : ModSystem
    {
        public override void SaveWorldData(TagCompound tag)
        {
            //不存点什么就不会调用加载，现在满意了吧
            tag.Add("F**K", true);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.TryGet("downed", out IList<string> list))
            {
                if (list.Contains("Rediancie"))
                    ModContent.GetInstance<DownedRediancie>().Set(true);
                if (list.Contains("BabyIceDragon"))
                    ModContent.GetInstance<DownedBabyIceDragon>().Set(true);
                if (list.Contains("SlimeEmperor"))
                    ModContent.GetInstance<DownedSlimeEmperor>().Set(true);
                if (list.Contains("Bloodiancie"))
                    ModContent.GetInstance<DownedBloodiancie>().Set(true);
                if (list.Contains("ThunderveinDragon"))
                    ModContent.GetInstance<DownedThunderveinDragon>().Set(true);
                if (list.Contains("ZacurrentDragon"))
                    ModContent.GetInstance<DownedZacurrentDragon>().Set(true);
                if (list.Contains("NightmarePlantera"))
                    ModContent.GetInstance<DownedNightmarePlantera>().Set(true);
            }
        }
    }
}
