using Coralite.Content.Items.Fairies.FairyEVBonus;
using Coralite.Content.Items.Materials;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem.FairyFreePart;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem
{
    public partial class FairySystem : ModSystem, ILocalizedModType
    {
        /// <summary>
        /// 键值是墙壁的type，-1表示没有墙壁
        /// </summary>
        public static Dictionary<int, List<FairySpawnController>> fairySpawnConditions;
        /// <summary>
        /// 键值是仙灵的种类，这个是用来在仙灵百科全书中使用的
        /// </summary>
        public static Dictionary<int, FairySpawnController> fairySpawnConditions_InEncyclopedia;
        /// <summary>
        /// 记录仙灵的不同等级的数值
        /// </summary>
        public static Dictionary<int, FairyData> fairyDatas;

        /// <summary>
        /// 记录<see cref="FairyRarity"/>对应的灵光生成
        /// </summary>
        public static Dictionary<int, FairyFreeInfo> RarityAuraSpawners { get; set; } = new Dictionary<int, FairyFreeInfo>();

        /// <summary>
        /// 记录仙灵自身对应的灵光生成
        /// </summary>
        public static Dictionary<int, FairyFreeInfo> FairyAuraSpawners { get; set; } = new Dictionary<int, FairyFreeInfo>();

        /// <summary>
        /// 是否抓到过该仙灵
        /// </summary>
        public static bool[] FairyCaught;

        /// <summary>
        /// 获取tml的墙壁到物品的映射，为什么为什么为什么它不是public的！？！？！？！？！？！？！？！？！？！？
        /// </summary>
        public static Dictionary<int, int> GetWallTypeToItemType;

        public string LocalizationCategory => "Systems";

        /// <summary>
        /// 洗点道具
        /// </summary>
        public static int EVReturnItemType => ModContent.ItemType<GloomInk>();

        /// <summary>
        /// 获取风石碑牌后解锁
        /// </summary>
        public static bool UnlockFairyThings;

        public override void Load()
        {
            LoadLocalization();
        }

        public override void OnModLoad()
        {
            GetWallTypeToItemType = (Dictionary<int, int>)typeof(WallLoader).GetField("wallTypeToItemType"
                , BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField).GetValue(null);

            fairySpawnConditions = [];
            fairySpawnConditions_InEncyclopedia = [];
            fairyDatas = [];
            FairyCaught = new bool[FairyLoader.FairyCount];

            string path = AssetDirectory.Datas + "FairyData.json";

#if DEBUG
            CheckFairyData(path);
#endif

            LoadFairyDatas(path);
        }

        public override void SetStaticDefaults()
        {
            RarityAuraSpawners.Add((int)FairyRarity.C,
                new FairyFreeInfo()
                    .AddRule(FairyFree.Common<FaintAura>(4))
                    .AddRule(FairyFree.Common<MagicalPowder>(3))
                    .AddRule(FairyFree.Coin(Item.buyPrice(0, 0, 1)))
                );

            foreach (var fairy in FairyLoader.fairys)
            {
                FairyFreeInfo? info = fairy.RegisterFairyFreeAura();
                if (info.HasValue)
                    FairyAuraSpawners.Add(fairy.Type, info.Value);
            }

            //(fairyType, pos) =>
            //{
            //    int index;
            //    if (Main.rand.NextBool(3))//三分之一概率生成
            //        index = Item.NewItem(new EntitySource_FairyFree(fairyType), pos
            //             , ModContent.ItemType<FaintAura>());
            //    else if (Main.rand.NextBool(5))//三分之一概率生成
            //        index = Item.NewItem(new EntitySource_FairyFree(fairyType), pos
            //             , ModContent.ItemType<MagicalPowder>());
            //    else
            //        index = Item.NewItem(new EntitySource_FairyFree(fairyType), pos
            //             , ItemID.SilverCoin, Main.rand.Next(1, 3));

            //    Main.item[index].shimmered = true;
            //});
        }

        #region 仙灵数据

        private static void LoadFairyDatas(string path)
        {
#if DEBUG
            using Stream stream = new FileStream("D:/My Games/Terraria/tModLoader/ModSources/Coralite/" + path, FileMode.Open);
#else
            using Stream stream = Coralite.Instance.GetFileStream(path, true);//读取文件
#endif

            using StreamReader file = new StreamReader(stream);
            string originString = file.ReadToEnd();
            JObject obj = JObject.Parse(originString);

            foreach (var fairy in FairyLoader.fairys)
            {
                //添加生成条件
                fairy.RegisterSpawn();

                //添加仙灵数据
                JObject fairyData = (JObject)obj[fairy.Name];

                FairyData data = new FairyData()
                {
                    LifeMaxSSV = GetLevelValues(fairyData, "LifeMax"),
                    DamageSSV = GetLevelValues(fairyData, "Damage"),
                    DefenceSSV = GetLevelValues(fairyData, "Defence"),
                    SpeedSSV = GetLevelValues(fairyData, "Speed"),
                    SkillLevelSSV = GetLevelValues(fairyData, "SkillLevel"),
                    StaminaSSV = GetLevelValues(fairyData, "Stamina"),
                };

                fairyDatas.Add(fairy.Type, data);
            }
        }

        private static short GetLevelValues(JObject fairyData, string name)
        {
            if (fairyData[name] != null)
                return fairyData[name].Value<short>();

            return 1;
        }

        private static void CheckFairyData(string path)
        {
            //检测是否存在文件，不存在就新建一个
            //if (!Coralite.Instance.FileExists(path))
            //{
            //    Console.WriteLine("JSON文件不存在，创建新文件...");
            //    File.WriteAllText(path, "");
            //}

            //检测是否存在键名
            // 2. 读取JSON文件内容
            using FileStream stream = new FileStream("D:/My Games/Terraria/tModLoader/ModSources/Coralite/" + path, FileMode.Open);// Coralite.Instance.GetFileStream(path, true);//读取文件
            using StreamReader file = new StreamReader(stream);

            string originString = file.ReadToEnd();
            JObject obj = JObject.Parse(originString);

            //Console.WriteLine("\n原始JSON内容:");
            //Console.WriteLine(originString);

            string[] fairyIVs = [
                "LifeMax",
                "Damage",
                "Defence",
                "Speed",
                "SkillLevel",
                "Stamina",
                ];

            foreach (var fairy in FairyLoader.fairys)
            {
                //不存在键名直接新建
                if (!obj.ContainsKey(fairy.Name))
                    obj[fairy.Name] = new JObject();

                //查找各种属性的值
                JObject fairyNameobj = (JObject)obj[fairy.Name];
                foreach (var ivName in fairyIVs)
                {
                    if (!fairyNameobj.ContainsKey(ivName))
                        fairyNameobj.Add(new JProperty(ivName, 50));
                }
            }

            using StreamWriter writer = new StreamWriter(stream);
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            writer.Write(obj.ToString());
        }

        #endregion

        public override void PostAddRecipes()
        {
            RegisterElfTrade();
        }

        public override void Unload()
        {
            fairySpawnConditions = null;
            //ProgressBarOuter = null;
            //ProgressBarInner = null;
            UnloadLocalization();
            FairyLoader.Unload();
            ElfPortalTrades = null;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (UnlockFairyThings)
                tag.Add(nameof(UnlockFairyThings), true);

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
            UnlockFairyThings = tag.ContainsKey("UnlockFairyThings");

            IList<string> list = tag.GetList<string>("FairyCaught");

            for (int i = 0; i < FairyLoader.FairyCount; i++)
                FairyCaught[i] = list.Contains(FairyLoader.GetFairy(i).Name);
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

        public static void VanillaFairyPowder(ref FairyAttempt attempt, Item powder)
        {

        }
    }
}
