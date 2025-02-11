using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        public static ConnectLineType CurrentConnectLineType;
        public static ConnectUIAssetID CurrentMagikeUIType;

        public static ATex[] ConnectLines { get; private set; }
        public static ATex[] ConnectUI { get; private set; }

        public static ATex SelectFrame { get; private set; }

        public static ATex[] UIApparatusButton { get; private set; }
        public static ATex[] UIComponentButton { get; private set; }
        //public static ATex[] UIShowTypeButton { get; private set; }
        //public static ATex[] ComponentRollingBar { get; private set; }

        public static ATex MagikeContainerBar { get; private set; }
        public static ATex CraftArrow { get; private set; }
        public static ATex FilterRemoveButton { get; private set; }

        public static ATex CraftSelectButton { get; private set; }
        public static ATex CraftShowButton { get; private set; }
        public static ATex CraftItemSpawnButton { get; private set; }
        public static ATex CraftShowRecipeButton { get; private set; }
        public static ATex ChargerItemButton { get; private set; }
        public static ATex ChargerPlayerButton { get; private set; }
        public static ATex CanCraftShow { get; private set; }
        public static ATex AlphaBar { get; private set; }
        public static ATex CraftMagikeBar { get; private set; }

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

            SelectFrame = Request<Texture2D>(AssetDirectory.Misc + "SelectFrame");
            MagikeContainerBar = Request<Texture2D>(AssetDirectory.MagikeUI + "MagikeContainerBar");
            CraftArrow = Request<Texture2D>(AssetDirectory.MagikeUI + "CraftArrow");
            FilterRemoveButton = Request<Texture2D>(AssetDirectory.MagikeUI + "FilterRemoveButton");
            CraftSelectButton = Request<Texture2D>(AssetDirectory.MagikeUI + "CraftSelectButton");
            CraftShowButton = Request<Texture2D>(AssetDirectory.MagikeUI + "CraftShowButton");
            CraftItemSpawnButton = Request<Texture2D>(AssetDirectory.MagikeUI + "CraftItemSpawnButton");
            CraftShowRecipeButton = Request<Texture2D>(AssetDirectory.MagikeUI + "CraftShowRecipeButton");
            ChargerItemButton = Request<Texture2D>(AssetDirectory.MagikeUI + "ChargerItemButton");
            ChargerPlayerButton = Request<Texture2D>(AssetDirectory.MagikeUI + "ChargerPlayerButton");
            CanCraftShow = Request<Texture2D>(AssetDirectory.MagikeUI + "CanCraftShow");
            AlphaBar = Request<Texture2D>(AssetDirectory.UI + "AlphaBar");
            CraftMagikeBar = Request<Texture2D>(AssetDirectory.MagikeUI + "CraftMagikeBar");
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

            //if (UIShowTypeButton == null)
            //{
            //    UIShowTypeButton = new ATex[count];
            //    UIShowTypeButton[(int)MagikeUIType.MagicCrystal] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagicCrystalShowTypeButton");
            //}

            //if (ComponentRollingBar == null)
            //{
            //    ComponentRollingBar = new ATex[count];
            //    ComponentRollingBar[(int)MagikeUIType.MagicCrystal] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagicCrystalShowRollingBar");
            //}
        }

        public static void UnloadAssets()
        {
            ConnectLines = null;
            ConnectUI = null;

            SelectFrame = null;
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

        //public static ATex GetUIShowTypeButton()
        //{
        //    if (UIShowTypeButton == null)
        //        LoadUIAsset();
        //    return UIShowTypeButton[(int)CurrentMagikeUIType];
        //}

        //public static ATex GetUIRollingBar()
        //{
        //    if (ComponentRollingBar == null)
        //        LoadUIAsset();
        //    return ComponentRollingBar[(int)CurrentMagikeUIType];
        //}

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
    }
}
