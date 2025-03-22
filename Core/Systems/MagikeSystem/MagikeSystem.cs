using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ModSystem
    {
        public static MagikeSystem Instance { get; private set; }

        public static ATex[] ConnectLines { get; private set; }
        public static ATex[] ConnectUI { get; private set; }
        public static ATex[] UIApparatusButton { get; private set; }
        public static ATex[] UIComponentButton { get; private set; }

        public static ConnectLineType CurrentConnectLineType;
        public static ConnectUIAssetID CurrentMagikeUIType;

        public static bool DrawSpecialTileText;
        public static string SpecialTileText;

        public MagikeSystem()
        {
            Instance = this;
        }

        /// <summary> 是否学习过 书页：魔能基础 </summary>
        public static bool learnedMagikeBase;
        /// <summary> 是否学习过卷轴：强化魔能提炼 </summary>
        public static bool learnedMagikeAdvanced;

        public override void PostAddRecipes()
        {
            if (Main.dedServ)
                return;

            RegisterMagikeCraft();
            LoadPolarizeFilter();
        }

        public override void Load()
        {
            LoadLocalization();
            LoadAssets();
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            UnloadLocalization();
            UnloadAssets();
            UnLoadPolarizeFilter();

            MagikeCraftRecipesDic = null;
            MagikeCraftRecipesFrozen = null;
        }

        public enum ConnectLineType
        {
            Basic = 0,
            ThinSpeed,
            Star,

            Speed,
            Wave,
        }

        public enum ConnectUIAssetID
        {
            Botton = 0,
            Flow,
            Close,
        }

        /// <summary>
        /// 魔能UI的类型
        /// </summary>
        public enum MagikeUIType
        {
            MagicCrystal,
            CrystallineMagike,
        }

        #region 加载与卸载

        public static void LoadAssets()
        {
            LoadConnectLine();
            LoadConnectUI();
            LoadUIAsset();
        }

        public static void LoadConnectLine()
        {
            ConnectLines = new ATex[(int)ConnectLineType.Wave + 1];

            ConnectLines[(int)ConnectLineType.Basic] = Request<Texture2D>(AssetDirectory.MagikeUI + "BasicConnectLine");
            ConnectLines[(int)ConnectLineType.ThinSpeed] = Request<Texture2D>(AssetDirectory.MagikeUI + "ThinSpeedConnectLine");
            ConnectLines[(int)ConnectLineType.Star] = Request<Texture2D>(AssetDirectory.MagikeUI + "StarConnectLine");

            ConnectLines[(int)ConnectLineType.Speed] = Request<Texture2D>(AssetDirectory.MagikeUI + "SpeedConnectLine");
            ConnectLines[(int)ConnectLineType.Wave] = Request<Texture2D>(AssetDirectory.MagikeUI + "WaveConnectLine");
        }

        public static void LoadConnectUI()
        {
            ConnectUI = new ATex[(int)ConnectUIAssetID.Close + 1];

            ConnectUI[(int)ConnectUIAssetID.Botton] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagikeConnectButton");
            ConnectUI[(int)ConnectUIAssetID.Flow] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagikeConnectButtonFlow");
            ConnectUI[(int)ConnectUIAssetID.Close] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagikeConnectCloseButton");
        }

        public static void LoadUIAsset()
        {
            int count = (int)MagikeUIType.CrystallineMagike + 1;
            if (UIApparatusButton == null)
            {
                UIApparatusButton = new ATex[count];
                UIApparatusButton[(int)MagikeUIType.MagicCrystal] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagicCrystalApparatusButton");
            }

            if (UIComponentButton == null)
            {
                UIComponentButton = new ATex[count];
                UIComponentButton[(int)MagikeUIType.MagicCrystal] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagicCrystalComponentButton");
            }
        }

        public static void UnloadAssets()
        {
            ConnectLines = null;
            ConnectUI = null;
        }

        #endregion

        #region 帮助方法

        public static Texture2D GetConnectLine()
            => ConnectLines[(int)CurrentConnectLineType].Value;

        public static ATex GetUIApparatusButton()
        {
            if (UIApparatusButton == null)
                LoadUIAsset();
            return UIApparatusButton[(int)CurrentMagikeUIType];
        }

        public static ATex GetComponentButton()
        {
            if (UIComponentButton == null)
                LoadUIAsset();
            return UIComponentButton[(int)CurrentMagikeUIType];
        }

        public static void DrawConnectLine(SpriteBatch spriteBatch, Vector2 startPos, Vector2 endPos, Vector2 screenPos, Color drawColor)
        {
            Texture2D laserTex = GetConnectLine();

            int width = (int)(startPos - endPos).Length();   //这个就是激光长度
            var origin = new Vector2(0, laserTex.Height / 2);

            Vector2 startPosScreen = startPos - screenPos;

            var laserTarget = new Rectangle((int)startPosScreen.X, (int)startPosScreen.Y, width, laserTex.Height);
            var laserSource = new Rectangle((int)(-Main.GlobalTimeWrappedHourly * laserTex.Width), 0, width, laserTex.Height);

            spriteBatch.Draw(laserTex, laserTarget, laserSource, drawColor, (endPos - startPos).ToRotation(), origin, 0, 0);
        }

        #endregion


        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (DrawSpecialTileText)
            {
                DrawSpecialTileText = false;
                UICommon.TooltipMouseText(SpecialTileText);
            }
        }

        //额...每增加一个变量就要在这边多写一段，说实话显得很蠢，以后有机会需要修改掉
        public override void SaveWorldData(TagCompound tag)
        {
            List<string> Knowledge = new();
            if (learnedMagikeBase)
                Knowledge.Add("learnedMagikeBase");
            if (learnedMagikeAdvanced)
                Knowledge.Add("learnedMagikeAdvanced");

            tag.Add("ConnectLineType", (int)CurrentConnectLineType);

            SaveData_2_1(Knowledge);
            SaveUI(tag);

            tag.Add("Knowledge", Knowledge);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> list = tag.GetList<string>("Knowledge");
            learnedMagikeBase = list.Contains("learnedMagikeBase");
            learnedMagikeAdvanced = list.Contains("learnedMagikeAdvanced");

            CurrentConnectLineType = (ConnectLineType)tag.GetInt("ConnectLineType");

            LoadData_2_1(list);
            LoadUI(tag);
        }
    }
}
