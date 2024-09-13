using Coralite.Core.Systems.MagikeSystem.Components;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem
    {
        public static ConnectLineType CurrentConnectLineType;
        public static ConnectUIAssetID CurrentMagikeUIType;

        public static Asset<Texture2D>[] ConnectLines { get; private set; }
        public static Asset<Texture2D>[] ConnectUI { get; private set; }

        public static Asset<Texture2D> SelectFrame { get; private set; }

        public static Asset<Texture2D>[] UIApparatusButton { get; private set; }
        public static Asset<Texture2D>[] UIComponentButton { get; private set; }
        public static Asset<Texture2D>[] UIShowTypeButton { get; private set; }
        public static Asset<Texture2D>[] ComponentRollingBar { get; private set; }

        public static Asset<Texture2D> MagikeContainerBar { get; private set; }
        public static Asset<Texture2D> CraftArrow { get; private set; }
        public static Asset<Texture2D> FilterRemoveButton { get; private set; }

        public static Asset<Texture2D> CraftSelectButton { get; private set; }
        public static Asset<Texture2D> CraftShowButton { get; private set; }
        public static Asset<Texture2D> CanCraftShow { get; private set; }

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
            CanCraftShow = Request<Texture2D>(AssetDirectory.MagikeUI + "CanCraftShow");
        }

        public static void LoadConnectLine()
        {
            ConnectLines = new Asset<Texture2D>[(int)ConnectLineType.Wave + 1];

            ConnectLines[(int)ConnectLineType.Basic] = Request<Texture2D>(AssetDirectory.MagikeUI + "BasicConnectLine");
            ConnectLines[(int)ConnectLineType.ThinSpeed] = Request<Texture2D>(AssetDirectory.MagikeUI + "ThinSpeedConnectLine");
            ConnectLines[(int)ConnectLineType.Star] = Request<Texture2D>(AssetDirectory.MagikeUI + "StarConnectLine");

            ConnectLines[(int)ConnectLineType.Speed] = Request<Texture2D>(AssetDirectory.MagikeUI + "SpeedConnectLine");
            ConnectLines[(int)ConnectLineType.Wave] = Request<Texture2D>(AssetDirectory.MagikeUI + "WaveConnectLine");
        }

        public static void LoadConnectUI()
        {
            ConnectUI = new Asset<Texture2D>[(int)ConnectUIAssetID.Close + 1];

            ConnectUI[(int)ConnectUIAssetID.Botton] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagikeConnectButton");
            ConnectUI[(int)ConnectUIAssetID.Flow] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagikeConnectButtonFlow");
            ConnectUI[(int)ConnectUIAssetID.Close] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagikeConnectCloseButton");
        }

        public static void LoadUIAsset()
        {
            int count = (int)MagikeUIType.CrystallineMagike + 1;
            if (UIApparatusButton == null)
            {
                UIApparatusButton = new Asset<Texture2D>[count];
                UIApparatusButton[(int)MagikeUIType.MagicCrystal] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagicCrystalApparatusButton");
            }

            if (UIComponentButton == null)
            {
                UIComponentButton = new Asset<Texture2D>[count];
                UIComponentButton[(int)MagikeUIType.MagicCrystal] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagicCrystalComponentButton");
            }

            if (UIShowTypeButton == null)
            {
                UIShowTypeButton = new Asset<Texture2D>[count];
                UIShowTypeButton[(int)MagikeUIType.MagicCrystal] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagicCrystalShowTypeButton");
            }

            if (ComponentRollingBar == null)
            {
                ComponentRollingBar = new Asset<Texture2D>[count];
                ComponentRollingBar[(int)MagikeUIType.MagicCrystal] = Request<Texture2D>(AssetDirectory.MagikeUI + "MagicCrystalShowRollingBar");
            }
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

        public static Asset<Texture2D> GetUIApparatusButton()
        {
            if (UIApparatusButton == null)
                LoadUIAsset();
            return UIApparatusButton[(int)CurrentMagikeUIType];
        }

        public static Asset<Texture2D> GetComponentButton()
        {
            if (UIComponentButton == null)
                LoadUIAsset();
            return UIComponentButton[(int)CurrentMagikeUIType];
        }

        public static Asset<Texture2D> GetUIShowTypeButton()
        {
            if (UIShowTypeButton == null)
                LoadUIAsset();
            return UIShowTypeButton[(int)CurrentMagikeUIType];
        }

        public static Asset<Texture2D> GetUIRollingBar()
        {
            if (ComponentRollingBar == null)
                LoadUIAsset();
            return ComponentRollingBar[(int)CurrentMagikeUIType];
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
    }
}
