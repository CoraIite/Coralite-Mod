using Coralite.Core.Systems.BossSystems;
using System;

namespace Coralite
{
    public partial class Coralite
    {
        public override object Call(params object[] args)
        {
            int argsLength = args.Length;
            //Array.Resize(ref args, 15);

            try
            {
                string methodName = args[0] as string;

                switch (methodName)
                {
                    default: break;
                    case "GetBossDowned"://获取击败BOSS的bool值
                    case "BossDowned":
                        if (argsLength < 2)
                            return new ArgumentNullException("ERROR: Must specify a boss or event name as a string.");
                        if (args[1] is not string)
                            return new ArgumentException("ERROR: The argument to \"Downed\" must be a string.");
                        return GetBossDowned(args[1].ToString());
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Call Error: {e.StackTrace} {e.Message}");
            }

            return new ArgumentException("ERROR: Invalid method name.");
        }

        /// <summary>
        /// 获取击败BOSS的bool
        /// </summary>
        /// <param name="bossName"></param>
        /// <returns></returns>
        public static bool GetBossDowned(string bossName)
        {
            return bossName.ToLower() switch
            {
                "赤玉灵" or
                "Rediancie" 
                    => DownedBossSystem.downedRediancie,
                "冰龙宝宝" or
                "BabyIceDragon" 
                    => DownedBossSystem.downedBabyIceDragon,
                // "影子球" or "ShadowBalls" => DownedBossSystem.xxxx,
                "荒雷龙" or
                "ThunderveinDragon" 
                    => DownedBossSystem.downedThunderveinDragon,
                "史莱姆皇帝" or
                "至高帝史莱姆王" or
                "至高帝·史莱姆王" or
                "至高帝" or
                "SlimeEmperor" 
                    => DownedBossSystem.downedSlimeEmperor,
                "赤血玉灵" or
                "血咒精赤玉灵" or
                "血咒精·赤玉灵" or
                "血咒精" or
                "Bloodiancie" 
                    => DownedBossSystem.downedBloodiancie,
                "梦魇之花" or 
                "梦界主世纪之花" or 
                "梦界主·世纪之花" or 
                "梦界主" or 
                "NightmarePlantera" or 
                "Nightmare Plantera" 
                    => DownedBossSystem.downedNightmarePlantera,
                _ => false,
            };
        }
    }
}
