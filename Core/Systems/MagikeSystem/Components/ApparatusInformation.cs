using Coralite.Content.UI;
using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.ObjectData;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    [VaultLoaden(AssetDirectory.MagikeUI)]
    public class ApparatusInformation : MagikeComponent, IUpgradeable, IUIShowable
    {
        [VaultLoaden("{@classPath}" + "UpgradeableUI")]
        public static ATex UpgradeableUI { get; private set; }

        public override int ID => MagikeComponentID.ApparatusInformation;

        /// <summary>
        /// 当前仪器的等级，用于显示名字
        /// </summary>
        public ushort CurrentLevel { get; private set; }

        /// <summary>
        /// 是否显示需要插入偏振滤镜
        /// </summary>
        public virtual bool ShowPolarizedTip { get; } = true;

        public virtual bool ShowUpgradeUI { get; set; } = true;

        public sealed override void Update() { }

        #region 升级

        public override void Initialize()
        {
            InitializeLevel();
        }

        public void InitializeLevel()
        {
            CurrentLevel = CoraliteContent.MagikeLevelType<NoneLevel>();
        }

        public virtual void Upgrade(ushort incomeLevel)
        {
            CurrentLevel = incomeLevel;
        }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        #endregion

        #region UI

        public void ShowInUI(UIElement parent)
        {
            //标题
            Item i = ContentSamples.ItemsByType[TileLoader.GetItemDropFromTypeAndStyle(Framing.GetTileSafely(Entity.Position).TileType)];
            UIElement title = new ComponentUIElementText<ApparatusInformation>(c =>
                 i.Name, this, parent, new Vector2(1.3f));
            parent.Append(title);
            UIElement top = title;

            //当前等级
            if (ShowPolarizedTip)
            {
                UIElement text = this.NewTextBar(c =>
                {
                    return MagikeSystem.GetUIText(MagikeSystem.UITextID.CurrentLevel) + MagikeSystem.GetMALevelText(c.CurrentLevel);
                }, parent);

                top = text;
                text.SetTopLeft(title.Height.Pixels + 8, 0);
                parent.Append(text);
            }

            //物品格
            ShowOnlySlot slot = new ShowOnlySlot(this);
            slot.SetTopLeft(top.Top.Pixels + top.Height.Pixels + 8, 0);
            parent.Append(slot);

            //开关
            UpgradeShowButton bnutton = new UpgradeShowButton(this);
            bnutton.SetTopLeft(slot.Top.Pixels , slot.Width.Pixels+6);
            parent.Append(bnutton);

            if (Entity.ExtendFilterCapacity > 0)
                AddFilterController(parent, slot.Top.Pixels + slot.Height.Pixels);
        }

        /// <summary>
        /// 添加滤镜控制器
        /// </summary>
        /// <param name="parent"></param>
        public void AddFilterController(UIElement parent, float height)
        {
            UIElement title = new ComponentUIElementText(() => MagikeSystem.GetUIText(MagikeSystem.UITextID.FilterController), parent, new Vector2(1.3f));
            title.SetTopLeft(height + 10, 0);

            parent.Append(title);
            UIElement Text1 = new ComponentUIElementText(() => MagikeSystem.GetUIText(MagikeSystem.UITextID.ClickToRemove), parent);
            Text1.SetTopLeft(title.Top.Pixels + title.Height.Pixels + 8, 0);
            parent.Append(Text1);

            FixedUIGrid grid = [];
            for (int i = 0; i < Entity.ExtendFilterCapacity; i++)
                grid.Add(new FilterButton(i));

            grid.SetSize(0, 0, 1, 1);
            grid.SetTopLeft(Text1.Top.Pixels + title.Height.Pixels + 8, 0);

            grid.QuickInvisibleScrollbar();

            parent.Append(grid);
        }
        #endregion

        #region 网络同步

        public override void SendData(ModPacket data)
        {
            data.Write(CurrentLevel);
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            CurrentLevel = (ushort)reader.ReadInt16();
        }

        #endregion

        #region 数据存储

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(CurrentLevel), CoraliteContent.GetMagikeLevel(CurrentLevel).LevelName);
            tag.Add(preName + nameof(ShowUpgradeUI), ShowUpgradeUI);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            if (tag.TryGet(preName + nameof(CurrentLevel), out dynamic level))
            {
                if (level is int b)
                    CurrentLevel = OldVersionLoadLevel(b);
                else if (level is string)
                {
                    if (MagikeLoader.NameToLevel.TryGetValue(tag.GetString(preName + nameof(CurrentLevel)), out ushort id))
                        CurrentLevel = id;
                    else
                        CurrentLevel = NoneLevel.ID;
                }
            }

            if (tag.TryGet(preName + nameof(ShowUpgradeUI), out bool value))
                ShowUpgradeUI = value;
        }

        /// <summary>
        /// TODO: 在久远的未来版本删除这个
        /// </summary>
        /// <param name="oldVersionLevel"></param>
        /// <returns></returns>
        private static ushort OldVersionLoadLevel(int oldVersionLevel)
        {
            return oldVersionLevel switch
            {
                1 => CoraliteContent.MagikeLevelType<CrystalLevel>(),
                2 => CoraliteContent.MagikeLevelType<GlistentLevel>(),
                3 => CoraliteContent.MagikeLevelType<CrimsonLevel>(),
                4 => CoraliteContent.MagikeLevelType<CorruptionLevel>(),
                5 => CoraliteContent.MagikeLevelType<IcicleLevel>(),
                6 => CoraliteContent.MagikeLevelType<ShadowLevel>(),
                7 => CoraliteContent.MagikeLevelType<BrilliantLevel>(),
                8 => CoraliteContent.MagikeLevelType<HallowLevel>(),
                9 => CoraliteContent.MagikeLevelType<SoulLevel>(),
                10 => CoraliteContent.MagikeLevelType<FeatherLevel>(),
                11 => CoraliteContent.MagikeLevelType<HolyLightLevel>(),
                12 => CoraliteContent.MagikeLevelType<SplendorLevel>(),
                13 => CoraliteContent.MagikeLevelType<InfinityLevel>(),
                14 => CoraliteContent.MagikeLevelType<SeashoreLevel>(),
                15 => CoraliteContent.MagikeLevelType<EiderdownLevel>(),
                16 => CoraliteContent.MagikeLevelType<RedJadeLevel>(),
                17 => CoraliteContent.MagikeLevelType<EmperorLevel>(),
                18 => CoraliteContent.MagikeLevelType<BeeswaxLevel>(),
                19 => CoraliteContent.MagikeLevelType<HellstoneLevel>(),
                20 => CoraliteContent.MagikeLevelType<BoneLevel>(),
                21 => CoraliteContent.MagikeLevelType<QuicksandLevel>(),
                22 => CoraliteContent.MagikeLevelType<PelagicLevel>(),
                23 => CoraliteContent.MagikeLevelType<FlightLevel>(),
                24 => CoraliteContent.MagikeLevelType<ForbiddenLevel>(),
                25 => CoraliteContent.MagikeLevelType<FrostLevel>(),
                26 => CoraliteContent.MagikeLevelType<BloodJadeLevel>(),
                27 => CoraliteContent.MagikeLevelType<EternalFlameLevel>(),
                28 => CoraliteContent.MagikeLevelType<ShroomiteLevel>(),
                _ => CoraliteContent.MagikeLevelType<NoneLevel>()
            };
        }

        #endregion

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!ShowPolarizedTip || !ShowUpgradeUI || !GamePlaySystem.ShowUpgradeableItemIcon)//不需要升级的直接不管它
                return;

            if (!TryGetUpgradeableLevel(out MagikeLevel upgradeLevel))
                return;

            if (upgradeLevel == null)
                return;

            MagikeHelper.GetMagikeAlternateData(Entity.Position.X, Entity.Position.Y, out TileObjectData data, out _);
            Vector2 pos = Entity.Position.ToWorldCoordinates(0, 0)
                + new Vector2(data.Width * 16 / 2f, -32 + MathF.Sin((int)Main.timeForVisualEffects * 0.05f) * 8)
                - Main.screenPosition;

            Helper.QuickCenteredDraw(UpgradeableUI.Value, spriteBatch, pos, upgradeLevel.LevelColor * 0.75f);

            MagikeHelper.DrawItem(spriteBatch, ContentSamples.ItemsByType[upgradeLevel.PolarizedFilterItemType]
                , pos, 38, Color.White);
        }

        public bool TryGetUpgradeableLevel(out MagikeLevel upgradeLevel)
        {
            upgradeLevel = null;

            if (!MagikeSystem.MagikeApparatusLevels.TryGetValue(Entity.TargetTileID, out var levels))
                return false;

            //检测能否升级
            int index = levels.IndexOf(CurrentLevel);
            if (index < 0 || index == levels.Count - 1)
                return false;

            //找到最高的可用等级
            for (int i = index + 1; i < levels.Count; i++)
            {
                ushort levelID = levels[i];

                MagikeLevel level = CoraliteContent.GetMagikeLevel(levelID);//获取等级

                if (level.Available)
                    upgradeLevel = level;
            }

            if (upgradeLevel is null)
                return false;

            return true;
        }
    }

    public class ApparatusInformation_NoPolar : ApparatusInformation
    {
        public override bool ShowPolarizedTip => false;
    }

    public class ShowOnlySlot : UIElement
    {
        private readonly ApparatusInformation _Component;
        private float _scale = 1f;

        public ShowOnlySlot(ApparatusInformation container)
        {
            _Component = container;
            this.SetSize(54, 54);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(_Component.Entity.Position);
            if (!tile.HasTile)
                return;

            Item i = ContentSamples.ItemsByType[TileLoader.GetItemDropFromTypeAndStyle(tile.TileType)].Clone();

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                ItemSlot.OverrideHover(ref i, ItemSlot.Context.ShopItem);
                ItemSlot.MouseHover(ref i, ItemSlot.Context.ShopItem);
                _scale = Helper.Lerp(_scale, 1.1f, 0.2f);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            float scale = Main.inventoryScale;
            Main.inventoryScale = _scale;

            Vector2 position = GetDimensions().Center() + (new Vector2(52f, 52f) * -0.5f * Main.inventoryScale);
            ItemSlot.Draw(spriteBatch, ref i, ItemSlot.Context.ShopItem, position, Color.White);

            Main.inventoryScale = scale;
        }
    }

    public class UpgradeShowButton : UIElement
    {
        private float _scale = 1f;
        private readonly ApparatusInformation information;

        public UpgradeShowButton(ApparatusInformation information)
        {
            Texture2D mainTex = MagikeAssets.UpgradeableShow.Value;

            var frameBox = mainTex.Frame(2, 1);
            this.SetSize(frameBox.Width + 6, frameBox.Height + 6);
            this.information = information;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            information.ShowUpgradeUI = !information.ShowUpgradeUI;

            Helper.PlayPitched("UI/Tick", 0.4f, 0);
            UILoader.GetUIState<MagikeApparatusPanel>().ResetComponentPanel();
            UILoader.GetUIState<MagikeApparatusPanel>().RecalculateChildren();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = MagikeAssets.UpgradeableShow.Value;
            var dimensions = GetDimensions();

            if (IsMouseHovering)
            {
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);

                string text = MagikeSystem.GetUIText(MagikeSystem.UITextID.SwitchUpgradeShow);

                UICommon.TooltipMouseText(text);
            }
            else
                _scale = Helper.Lerp(_scale, 1f, 0.2f);

            var framebox = mainTex.Frame(2, 1, information.ShowUpgradeUI ? 0 : 1);
            spriteBatch.Draw(mainTex, dimensions.Center(), framebox, Color.White, 0, framebox.Size() / 2, _scale, 0, 0);
        }
    }

}
