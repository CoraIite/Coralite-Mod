using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public partial class FairySystem : ModSystem, ILocalizedModType
    {
        public static Asset<Texture2D> ProgressBarOuter;
        public static Asset<Texture2D> ProgressBarInner;

        /// <summary>
        /// 键值是墙壁的type，-1表示没有墙壁
        /// </summary>
        public static Dictionary<int, List<FairySpawnController>> fairySpawnConditions;
        /// <summary>
        /// 键值是仙灵的种类，这个是用来在仙灵百科全书中使用的
        /// </summary>
        public static Dictionary<int, FairySpawnController> fairySpawnConditions_InEncyclopedia;

        public string LocalizationCategory => "Systems";

        public override void Load()
        {
            Mod Mod = Coralite.Instance;

            fairySpawnConditions = new Dictionary<int, List<FairySpawnController>>();
            fairySpawnConditions_InEncyclopedia = new Dictionary<int, FairySpawnController>();

            foreach (var fairy in FairyLoader.fairys)  //添加生成条件
                fairy.RegisterSpawn();

            ProgressBarOuter = ModContent.Request<Texture2D>(AssetDirectory.Misc + "ProgressBarOuter");
            ProgressBarInner = ModContent.Request<Texture2D>(AssetDirectory.Misc + "ProgressBarInner");

            LoadLocalization();
        }

        public override void Unload()
        {
            fairySpawnConditions = null;
            ProgressBarOuter = null;
            ProgressBarInner = null;
            UnloadLocalization();
        }

        public static bool SpawnFairy(FairyAttempt attempt, out Fairy fairy)
        {
            fairy = null;

            if (fairySpawnConditions.ContainsKey(attempt.wallType) && fairySpawnConditions[attempt.wallType] != null)
            {
                List<FairySpawnController> currentCondition = new List<FairySpawnController>();
                List<FairySpawnController> totalCondition = fairySpawnConditions[attempt.wallType];
                foreach (var condition in totalCondition)
                    if (condition.CheckCondition(attempt))
                        currentCondition.Add(condition);

                if (currentCondition.Count == 0)
                    return false;

                fairy = Main.rand.NextFromList(currentCondition.ToArray()).SpawnFairy(attempt);
                return true;
            }

            return false;
        }
    }
}
