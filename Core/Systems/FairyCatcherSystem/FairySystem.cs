using Coralite.Core.Loaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;
using static Coralite.Core.Systems.FairyCatcherSystem.FairyAttempt;

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

        /// <summary>
        /// 是否抓到过该仙灵
        /// </summary>
        public static bool[] FairyCaught;

        public string LocalizationCategory => "Systems";

        public override void Load()
        {
            ProgressBarOuter = ModContent.Request<Texture2D>(AssetDirectory.Misc + "ProgressBarOuter");
            ProgressBarInner = ModContent.Request<Texture2D>(AssetDirectory.Misc + "ProgressBarInner");

            LoadLocalization();
        }

        public override void OnModLoad()
        {
            fairySpawnConditions = new Dictionary<int, List<FairySpawnController>>();
            fairySpawnConditions_InEncyclopedia = new Dictionary<int, FairySpawnController>();

            foreach (var fairy in FairyLoader.fairys)  //添加生成条件
                fairy.RegisterSpawn();

            LoadFairyTexture();

            FairyCaught = new bool[FairyLoader.FairyCount];
        }

        public override void PostAddRecipes()
        {
            RegisterElfTrade();
        }

        public override void Unload()
        {
            fairySpawnConditions = null;
            ProgressBarOuter = null;
            ProgressBarInner = null;
            UnloadLocalization();
            UnloadFairyTexture();
            FairyLoader.Unload();
            ElfPortalTrades = null;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            List<string> caught = new();

            for (int i = 0; i < FairyLoader.FairyCount; i++)
            {
                if (FairyCaught[i])
                    caught.Add(FairyLoader.GetFairy(i).Name);
            }

            tag.Add("FairyCaught", caught);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> list = tag.GetList<string>("FairyCaught");

            for (int i = 0; i < FairyLoader.FairyCount; i++)
                FairyCaught[i] = list.Contains(FairyLoader.GetFairy(i).Name);
        }


        public static bool SpawnFairy(FairyAttempt attempt, out Fairy fairy)
        {
            fairy = null;

            if (fairySpawnConditions.ContainsKey(attempt.wallType) && fairySpawnConditions[attempt.wallType] != null)
            {
                List<FairySpawnController> currentCondition = new();
                List<FairySpawnController> totalCondition = fairySpawnConditions[attempt.wallType];
                foreach (var condition in totalCondition)
                    if (condition.CheckCondition(attempt) //稀有度是额外判定的
                        && attempt.rarity == FairyLoader.GetFairy(condition.fairyType).Rarity)
                        currentCondition.Add(condition);

                if (currentCondition.Count == 0)
                    return false;

                fairy = Main.rand.NextFromList(currentCondition.ToArray()).SpawnFairy(attempt);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置捕捉的bool
        /// </summary>
        /// <param name="fairy"></param>
        public static void SetFairyCaught(Fairy fairy)
        {
            FairyCaught[fairy.Type] = true;
        }

        public static string GetRarityDescription(FairyRarity rarity)
        {
            if (RarityText.TryGetValue(rarity, out Terraria.Localization.LocalizedText value))
                return value.Value;

            return Rarity_SP.Value;//特殊
        }

        public static Color GetRarityColor(FairyRarity rarity)
        {
            if (RarityColors.TryGetValue(rarity, out Color value))
                return value;

            return RaritySP_Purple;//特殊
        }
    }
}
