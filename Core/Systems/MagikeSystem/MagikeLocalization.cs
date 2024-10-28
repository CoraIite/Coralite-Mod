using System;
using Terraria.Localization;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ILocalizedModType
    {
        public string LocalizationCategory => "MagikeSystem";

        public static LocalizedText NewKnowledgeUnlocked { get; private set; }

        public static LocalizedText LearnedMagikeBase { get; private set; }
        public static LocalizedText LearnedMagikeAdvanced { get; private set; }

        public static LocalizedText Error { get; private set; }

        public static LocalizedText CanMagikeCraft { get; private set; }
        public static LocalizedText ItemMagikeAmount { get; private set; }

        public void LoadLocalization()
        {
            LearnedMagikeBase = this.GetLocalization("learnedMagikeBase");
            LearnedMagikeAdvanced = this.GetLocalization("learnedMagikeAdvanced");
            NewKnowledgeUnlocked = this.GetLocalization("NewKnowledgeUnlocked", () => "魔能辞典中解锁了新的知识");
            Error = this.GetLocalization("Error");
            CanMagikeCraft = this.GetLocalization(nameof(CanMagikeCraft));
            ItemMagikeAmount = this.GetLocalization(nameof(ItemMagikeAmount));

            this.GetLocalization("PolarizedFilterTooltip");

            LoadConnectStaff();
            LoadFilter();
            LoadItemDescription();
            LoadApparatusDescription();
            LoadUIText();
            LoadCraftText();
        }

        public void UnloadLocalization()
        {
            LearnedMagikeBase = null;
            LearnedMagikeAdvanced = null;
            NewKnowledgeUnlocked = null;

            Staffs = null;
            Filter = null;
            ItemDescription = null;
            ApparatusDescription = null;
            UIText = null;
            CraftText = null;
        }

        #region 魔能连接仪相关

        public static LocalizedText[] Staffs { get; private set; }

        public enum StaffTextID
        {
            /*
             * 魔能连接仪的各种情况
             *  - 第一步：选择发送器，分为选中了和妹找到
             *  - 第二步：选择接收器，分为没有接收器，太远了无法连接和连接成功3种情况
             *  - 右键点击为打开连接面板，左键单击面板和选择发送器一样，右键单击取消连接
             */
            ChooseSender_Found = 0,
            ChooseSender_NotFound,

            ChooseReceiver_NotFound,
            ChooseReceiver_AlreadyConnect,
            ConnectFail_TooFar,
            ConnectFail_CantBeSelf,
            ConnectFail_ConnectorFillUp,
            Connect_Success,

            Deconnect,

            /*
             * 璀璨连接仪
             *  - 和魔能连接仪差不多，多一个选中发送器后右键框选接收器
             *  - 会显示连接数量和失败数量
             */

            BrilliantConnect,

            //充能球：未找到魔能容器
            ChargeNotFound,
            //激活杖：未找到魔能工厂
            FactoryNotFound,

            Count
        }

        public void LoadConnectStaff()
        {
            Staffs = new LocalizedText[(int)StaffTextID.Count];

            for (int i = 0; i < (int)StaffTextID.Count; i++)
                Staffs[i] = this.GetLocalization(nameof(Staffs) + "." + Enum.GetName((StaffTextID)i));
        }

        public static string GetConnectStaffText(StaffTextID id)
            => Staffs[(int)id].Value;

        #endregion

        #region 滤镜相关

        public static LocalizedText[] Filter { get; private set; }

        public enum FilterID
        {
            /*
             * 魔能滤镜的ID
             * 首先通用的为：未找到魔能容器和成功放置，以及滤镜已满
             */

           ApparatusNotFound = 0,
           FilterFillUp = 1,
           InsertSuccess = 2,
           UpgradeableNotFound = 3,
           CantUpgrade = 4,

           MagikeContainerNotFound,
           MagikeSenderNotFound,
           MagikeLinerSenderNotFound,
           MagikeProducerNotFound,
           TimerNotFound,

           Count,
        }

        public void LoadFilter()
        {
            Filter = new LocalizedText[(int)FilterID.Count];

            for (int i = 0; i < (int)FilterID.Count; i++)
                Filter[i] = this.GetLocalization(nameof(Filter) + "." + Enum.GetName((FilterID)i));
        }

        public static string GetFilterText(FilterID id)
            => Filter[(int)id].Value;

        #endregion

        #region 仪器显示

        public static LocalizedText[] ApparatusDescription { get; private set; }

        public enum ApparatusDescriptionID
        {
            /*
             * 魔能容量
             * 连接数量
             * 
             */
            MagikeAmount = 0,
            ConnectAmount = 1,
            IsWorking = 2,

            Count = 3,
        }

        public void LoadApparatusDescription()
        {
            ApparatusDescription = new LocalizedText[(int)ApparatusDescriptionID.Count];

            for (int i = 0; i < (int)ApparatusDescriptionID.Count; i++)
                ApparatusDescription[i] = this.GetLocalization(nameof(ApparatusDescription) + "." + Enum.GetName((ApparatusDescriptionID)i));
        }

        public static string GetApparatusDescriptionText(ApparatusDescriptionID id)
            => ApparatusDescription[(int)id].Value;

        #endregion

        #region 魔能合成相关

        public static LocalizedText[] CraftText { get; private set; }

        public enum CraftTextID
        {
            NoMainItem,
            MainItemIncorrect,
            MainItemNotEnough,
            ConditionNotMet,
            OtherItemNotEnough,
            OtherItemLack,

            MagikeNotEnough,
            MagikeEnough,
            AntimagikeNotEnough,
            NoCraftRecipe,
            Count
        }

        public void LoadCraftText()
        {
            CraftText = new LocalizedText[(int)CraftTextID.Count];

            for (int i = 0; i < (int)CraftTextID.Count; i++)
                CraftText[i] = this.GetLocalization(nameof(CraftText) + "." + Enum.GetName((CraftTextID)i));
        }

        public static string GetCraftText(CraftTextID id)
            => CraftText[(int)id].Value;

        #endregion

        #region 物品相关

        public static LocalizedText[] ItemDescription { get; private set; }

        public class ItemDescriptionID
        {
            /*
             * 物品描述部分
             *      - 能够插入哪些偏振滤镜
             */
            public const int PolarizedFilter = 0;

            public const int Count = 1;
        }

        public void LoadItemDescription()
        {
            ItemDescription = new LocalizedText[ItemDescriptionID.Count];

            ItemDescription[ItemDescriptionID.PolarizedFilter] = this.GetLocalization(nameof(ItemDescription) + "PolarizedFilter");
        }

        public static string GetItemDescriptionText(int id)
            => ItemDescription[id].Value;

        #endregion

        #region UI显示

        public static LocalizedText[] UIText { get; private set; }

        public enum UITextID
        {
            /*
             * UI部分
             */

            /*
             * 魔能容器
             *      - 当前魔能量
             *      - 魔能上限
             *      - 当前反魔能量
             *      - 反魔能上限
             *      - 魔能存储器名称
             */
            ContainerMagikeAmount,
            ContainerMagikeMax,
            ContainerAntiMagikeAmount,
            ContainerAntiMagikeMax,
            MagikeContainerName,
            CraftAltarName,

            /*
             * 魔能线性发送器
             *      - 连接情况
             *      - 发送量
             *      - 发送间隔
             *      - 连接距离
             *      - 线性发送器名称
             */
            CurrentConnect,
            MagikeSendTime,
            MagikeSendAmount,
            MagikeConnectLength,
            MagikeLinerSenderName,
            PluseSenderName,
            CheckOnlyLinerSenderName,
            ClickToDisconnect,

            /*
             * 偏振滤镜
             *      - 当前等级
             *      - 点击取出
             *      - 名称
             */
            PolarizedFilterLevel,
            ClickToRemove,
            MagikePolarizedFilterName,
            TotalReflectionFilterName,
            TotalReflectionBonus,
            InterferenceFilterName,
            InterferenceBonus,
            DiffractionFilterName,
            DiffractionBonus,
            PulseFilterName,
            PulseBonus,
            ExcitedFilterName,
            ExcitedBonus,

            /*
             * 物品生产器
             *      - 生产时间
             *      - 生产量
             * - 提取生产器
             *      - 名称
             *      - 生产条件
             *      - 含有魔能的物品
             * - 其他生产器
             *      
             */

            FactoryWorkTime,
            StoneMakerName,
            StoneMakerOutPut,
            LaserCollectorName,
            LaserCollectorOutPut,
            LaserCollectorCost,

            ExtractProducerName,
            ProduceTime,
            ProduceAmount,
            ProduceCondition,
            ItemWithMagike,
            ItemWithValue,
            ForestLensName,
            ForestCondition,
            OceanLensName,
            OceanCondition,
            SkyLensName,
            SkyCondition,
            DesertLensName,
            DesertCondition,
            GlowingMushroomLensName,
            GlowingMushroomCondition,
            SunlightLensName,
            SunlightCondition,
            SnowfieldLensName,
            SnowfieldCondition,
            MoonlightLensName,
            MoonlightCondition,
            LavaLensName,
            LavaCondition,
            HellLensName,
            HellCondition,
            WaterflowLensName,
            WaterflowCondition,
            HoneyLensName,
            HoneyCondition,
            DungeonLensName,
            DungeonCondition,
            HallowLensName,
            HallowCondition,
            GelLensName,
            TresureLensName,
            CogLensName,

            /*
             * 物品容器
             *      - 当前容量
             *      - 强夺全部
             *      - 快速堆叠
             *      - 名称
             */

            ItemMax,
            OutPutAll,
            FastStack,
            ItemContainerName,
            GetOnlyItemContainerName,

            Count
        }

        public void LoadUIText()
        {
            UIText = new LocalizedText[(int)UITextID.Count];

            for (int i = 0; i < (int)UITextID.Count; i++)
            {
                UIText[i] = this.GetLocalization(nameof(UIText) + "." + Enum.GetName((UITextID)i));
            }
        }

        public static string GetUIText(UITextID id)
            => UIText[(int)id].Value;

        #endregion
    }
}
