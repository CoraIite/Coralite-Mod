using Coralite.Core.Loaders;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
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
        /// 是否抓到过该仙灵
        /// </summary>
        public static bool[] FairyCaught;

        public string LocalizationCategory => "Systems";

        public override void Load()
        {
            LoadLocalization();
        }

        public override void OnModLoad()
        {
            fairySpawnConditions = new Dictionary<int, List<FairySpawnController>>();
            fairySpawnConditions_InEncyclopedia = new Dictionary<int, FairySpawnController>();
            fairyDatas = new Dictionary<int, FairyData>();
            FairyCaught = new bool[FairyLoader.FairyCount];

            string path = AssetDirectory.Datas + "FairyData.json";

#if DEBUG
            CheckFairyData(path);
#endif

            LoadFairyDatas(path);
            LoadFairyTexture();
        }

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
                    LifeMaxData = GetLevelValues(fairyData, "LifeMax", out int overLifeMax),
                    OverLifeMax = overLifeMax,
                    DamageData = GetLevelValues(fairyData, "Damage", out int overDamage),
                    OverDamage = overDamage,
                    DefenceData = GetLevelValues(fairyData, "Defence", out int overDefence),
                    OverDefence = overDefence,
                    SpeedData = GetLevelValues(fairyData, "Speed", out float overSpeed),
                    OverSpeed = overSpeed,
                    SkillLevelData = GetLevelValues(fairyData, "SkillLevel", out int overSkillLevel),
                    OverSkillLevel = overSkillLevel,
                    StaminaData = GetLevelValues<int>(fairyData, "Stamina", out int overStamina),
                    OverStamina = overStamina,
                };

                fairyDatas.Add(fairy.Type, data);
            }
        }

        private static List<T> GetLevelValues<T>(JObject fairyData, string name, out T overValue) where T : struct
        {
            List<T> l = [];

            overValue = default;

            foreach (var item in fairyData[name])
            {
                if (item is JProperty p)
                {
                    if (p.Name == "Over")
                        overValue = p.Value.Value<T>();
                    else
                        l.Add(p.Value.Value<T>());
                }
            }

            return l;
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
            using Stream stream = new FileStream("D:/My Games/Terraria/tModLoader/ModSources/Coralite/" + path, FileMode.Open);// Coralite.Instance.GetFileStream(path, true);//读取文件
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

            string[] fairyIVLevel = [
                "Weak",
                "WeakCommon",
                "Common",
                "Uncommon",
                "Rare",
                "Epic",
                "Legendary",
                "Eternal",
                "Over",
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
                        fairyNameobj[ivName] = new JObject();

                    //查找所有的等级
                    JObject ivObj = (JObject)fairyNameobj[ivName];

                    foreach (var levelName in fairyIVLevel)
                    {
                        if (!ivObj.ContainsKey(levelName))
                            ivObj.Add(new JProperty(levelName, 0));
                    }
                }
            }

            using StreamWriter writer = new StreamWriter(stream);
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            writer.Write(obj.ToString());
        }

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
