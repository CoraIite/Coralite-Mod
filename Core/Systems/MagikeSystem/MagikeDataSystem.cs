using Coralite.Core.Systems.MagikeSystem.Attributes;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria.ID;
using Terraria.ModLoader.Core;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        /// <summary>
        /// 存储了各种可升级仪器的信息<br></br>
        /// 键名为：仪器名+组件名+变量名，获取到的字典使用等级名获取对应值
        /// </summary>
        internal static Dictionary<string, HybridDictionary> MagikeApparatusData { get; set; } = [];

        public static Dictionary<Mod, List<string>> PropNames { get; set; } = [];

        public override void PostSetupContent()
        {
            RegisterMagikeTileLevels();

            const string DataName = "MagikeApparatusData.json";
#if DEBUG
            CheckMagikeData(Mod, "D:/My Games/Terraria/tModLoader/ModSources/Coralite/Datas/" + DataName);
            using Stream stream = new FileStream("D:/My Games/Terraria/tModLoader/ModSources/Coralite/Datas/" + DataName, FileMode.Open);
#else
            using Stream stream = Coralite.Instance.GetFileStream(AssetDirectory.Datas + DataName, true);//读取文件
#endif
            LoadMagikeDatas(Mod, stream);

            //加载其他模组的
            foreach (Mod mod in ModLoader.Mods)
            {
                if (mod is ICoralite ic)
                {
                    using Stream stream2 = mod.GetFileStream(ic.DataPath + DataName, true);//读取文件
                    LoadMagikeDatas(mod, stream);
                }
            }
        }

        /// <summary>
        /// 加载等级字典
        /// </summary>
        public static void RegisterMagikeTileLevels()
        {
            for (int i = TileID.Count; i < TileLoader.TileCount; i++)
            {
                ModTile mt = TileLoader.GetTile(i);
                if (mt is BaseMagikeTile magikeTile)
                {
                    List<ushort> levels = magikeTile.GetAllLevels();
                    if (levels == null || levels.Count == 0)
                        continue;
                    RegisterApparatusLevel(i, levels);
                }
            }
        }

        /// <summary>
        /// 根据属性自动加载
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="stream"></param>
        private static void LoadMagikeDatas(Mod mod, Stream stream)
        {
            using StreamReader file = new StreamReader(stream);
            string originString = file.ReadToEnd();
            JObject obj = JObject.Parse(originString);

            foreach (var type in AssemblyManager.GetLoadableTypes(mod.Code))
            {
                if (type.IsAbstract || !type.GetInterfaces().Contains(typeof(IUpgradeLoadable)))
                    continue;

                int tileType = (Activator.CreateInstance(type) as IUpgradeLoadable).TileType;
                ModTile mt = TileLoader.GetTile(tileType);

                if (mt is not BaseMagikeTile magTile)
                    continue;
                if (!MagikeApparatusLevels.TryGetValue(tileType, out var levels))
                    continue;

                string tileName = magTile.Name.Replace("Tile", "");
                if (!obj.ContainsKey(tileName))
                    obj[tileName] = new JObject();

                //物块的Obj
                JObject tileObj = (JObject)obj[tileName];

                PropertyInfo[] pInfos = type.GetProperties();

                //写入所有所有属性
                foreach (var pInfo in pInfos)
                {
                    var att = pInfo.GetCustomAttribute<UpgradeablePropAttribute>();
                    if (att == null)
                        continue;

                    //属性的Obj
                    string propName = pInfo.Name;
                    if (!tileObj.ContainsKey(propName))
                        tileObj[propName] = new JObject();

                    if (!PropNames.ContainsKey(mod))
                        PropNames.Add(mod, []);

                    if (!PropNames[mod].Contains(propName))
                    {
                        PropNames[mod].Add(propName);
                        mod.GetLocalization($"MagikeSystem.UpgradableProps.{propName}");
                    }

                    JObject propObj = (JObject)tileObj[propName];
                    HybridDictionary dic = [];

                    foreach (var level in levels)
                    {
                        MagikeLevel mLevel = CoraliteContent.GetMagikeLevel(level);
                        string levelName = mLevel.LevelName;

                        //把东西加进去
                        dic.Add(level, propObj[levelName].Value<string>());
                    }

                    MagikeApparatusData.Add(string.Concat(tileName, propName), dic);
                }
            }
        }

        /// <summary>
        /// 供你自动创建魔能数据
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="fullPath"></param>
        public static void CheckMagikeData(Mod mod, string fullPath)
        {
            //检测是否存在键名
            //读取JSON文件内容
            using FileStream stream = new FileStream(fullPath, FileMode.Open);
            using StreamReader file = new StreamReader(stream);

            string originString = file.ReadToEnd();
            JObject obj = JObject.Parse(originString);

            foreach (var type in AssemblyManager.GetLoadableTypes(mod.Code))
            {
                if (type.IsAbstract || !type.GetInterfaces().Contains(typeof(IUpgradeLoadable)))
                    continue;

                int tileType = (Activator.CreateInstance(type) as IUpgradeLoadable).TileType;
                ModTile mt = TileLoader.GetTile(tileType);

                if (mt is not BaseMagikeTile magTile)
                    continue;
                if (!MagikeApparatusLevels.TryGetValue(tileType, out var levels))
                    continue;

                string tileName = magTile.Name.Replace("Tile", "");
                if (!obj.ContainsKey(tileName))
                    obj[tileName] = new JObject();

                //物块的Obj
                JObject tileObj = (JObject)obj[tileName];

                PropertyInfo[] pInfos = type.GetProperties();

                //写入所有所有属性
                foreach (var pInfo in pInfos)
                {
                    var att = pInfo.GetCustomAttribute<UpgradeablePropAttribute>();
                    if (att == null)
                        continue;

                    //属性的Obj
                    string propName = pInfo.Name;
                    if (!tileObj.ContainsKey(propName))
                        tileObj[propName] = new JObject();

                    JObject propObj = (JObject)tileObj[propName];

                    foreach (var level in levels)
                    {
                        MagikeLevel mLevel = CoraliteContent.GetMagikeLevel(level);
                        string levelName = mLevel.LevelName;

                        if (!propObj.ContainsKey(levelName))
                            propObj.Add(new JProperty(levelName, 0));
                    }
                }
            }

            using StreamWriter writer = new StreamWriter(stream);
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            writer.Write(obj.ToString());
        }

        public static int GetLevelDataInt(ushort level, string name)
            => Convert.ToInt32((string)MagikeApparatusData[name][level]);
        public static float GetLevelDataFloat(ushort level, string name)
            => Convert.ToSingle((string)MagikeApparatusData[name][level]);
        public static byte GetLevelDataByte(ushort level, string name)
            => Convert.ToByte((string)MagikeApparatusData[name][level]);
        public static short GetLevelDataShort(ushort level, string name)
            => Convert.ToInt16((string)MagikeApparatusData[name][level]);

        public static int GetLevelData4Time(ushort level, string name)
        {
            float time = GetLevelDataFloat(level, name);
            if (time < 0)
                return -1;
            else
                return (int)(60 * time);
        }

        public static int GetLevelData4Tile(ushort level, string name)
        {
            return (int)(16 * GetLevelDataFloat(level, name));
        }
    }
}
