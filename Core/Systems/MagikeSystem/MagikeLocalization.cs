using Terraria.Localization;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ILocalizedModType
    {
        public string LocalizationCategory => "MagikeSystem";

        public static LocalizedText NewKnowledgeUnlocked { get; private set; }

        public LocalizedText LearnedMagikeBase { get; private set; }
        public LocalizedText LearnedMagikeAdvanced { get; private set; }


        public void LoadLocalization()
        {
            LearnedMagikeBase = this.GetLocalization("learnedMagikeBase");
            LearnedMagikeAdvanced = this.GetLocalization("learnedMagikeAdvanced");
            NewKnowledgeUnlocked = this.GetLocalization("NewKnowledgeUnlocked", () => "魔能辞典中解锁了新的知识");

            this.GetLocalization("PolarizedFilterTooltip");

            LoadConnectStaff();
            LoadFilter();
            LoadItemDescription();
            LoadApparatusDescription();
            LoadUIText();
        }

        public void UnloadLocalization()
        {
            LearnedMagikeBase = null;
            LearnedMagikeAdvanced = null;
            NewKnowledgeUnlocked = null;

            ConnectStaff = null;
            Filter = null;
            ItemDescription = null;
            ApparatusDescription = null;
            UIText = null;
        }

        #region 魔能连接仪相关

        public static LocalizedText[] ConnectStaff { get; private set; }

        public class ConnectStaffID
        {
            /*
             * 魔能连接仪的各种情况
             *  - 第一步：选择发送器，分为选中了和妹找到
             *  - 第二步：选择接收器，分为没有接收器，太远了无法连接和连接成功3种情况
             *  - 右键点击为打开连接面板，左键单击面板和选择发送器一样，右键单击取消连接
             */
            public const int ChooseSender_Found = 0;
            public const int ChooseSender_NotFound = 1;

            public const int ChooseReceiver_NotFound = 2;
            public const int ConnectFail_TooFar = 3;
            public const int ConnectFail_CantBeSelf = 4;
            public const int ConnectFail_ConnectorFillUp = 5;
            public const int Connect_Success = 6;

            public const int Deconnect = 7;

            /*
             * 璀璨连接仪
             *  - 和魔能连接仪差不多，多一个选中发送器后右键框选接收器
             *  - 会显示连接数量和失败数量
             */

            public const int BrilliantConnect = 8;

            public const int Count = BrilliantConnect + 1;
        }

        public void LoadConnectStaff()
        {
            ConnectStaff = new LocalizedText[ConnectStaffID.Count];

            ConnectStaff[ConnectStaffID.ChooseSender_Found] = this.GetLocalization("ChooseSender_Found");
            ConnectStaff[ConnectStaffID.ChooseSender_NotFound] = this.GetLocalization("ChooseSender_NotFound");
            ConnectStaff[ConnectStaffID.ChooseReceiver_NotFound] = this.GetLocalization("ChooseReceiver_NotFound");
            ConnectStaff[ConnectStaffID.ConnectFail_TooFar] = this.GetLocalization("ConnectFail_TooFar");
            ConnectStaff[ConnectStaffID.ConnectFail_CantBeSelf] = this.GetLocalization("ConnectFail_CantBeSelf");
            ConnectStaff[ConnectStaffID.ConnectFail_ConnectorFillUp] = this.GetLocalization("ConnectFail_ConnectorFillUp");
            ConnectStaff[ConnectStaffID.Connect_Success] = this.GetLocalization("ChooseReceiver_Success");
            ConnectStaff[ConnectStaffID.Deconnect] = this.GetLocalization("Deconnect");
            ConnectStaff[ConnectStaffID.BrilliantConnect] = this.GetLocalization("BrilliantConnect");
        }

        public static string GetConnectStaffText(int id)
            => ConnectStaff[id].Value;

        #endregion

        #region 滤镜相关

        public static LocalizedText[] Filter { get; private set; }

        public class FilterID
        {
            /*
             * 魔能滤镜的ID
             * 首先通用的为：未找到魔能容器和成功放置，以及滤镜已满
             */

            public const int ApparatusNotFound = 0;
            public const int FilterFillUp = 1;
            public const int InsertSuccess = 2;

            public const int UpgradeableNotFound = 3;
            public const int CantUpgrade = 4;

            public const int Count = 5;
        }

        public void LoadFilter()
        {
            Filter = new LocalizedText[FilterID.Count];
            Filter[FilterID.ApparatusNotFound] = this.GetLocalization("ApparatusNotFound");
            Filter[FilterID.FilterFillUp] = this.GetLocalization("FilterFillUp");
            Filter[FilterID.InsertSuccess] = this.GetLocalization("InsertSuccess");
            Filter[FilterID.UpgradeableNotFound] = this.GetLocalization("UpgradeableNotFound");
            Filter[FilterID.CantUpgrade] = this.GetLocalization("CantUpgrade");
        }

        public static string GetFilterText(int id)
            => Filter[id].Value;

        #endregion

        #region 仪器显示

        public static LocalizedText[] ApparatusDescription { get; private set; }

        public class ApparatusDescriptionID
        {
            /*
             * 魔能容量
             * 连接数量
             * 
             */
            public const int MagikeAmount = 0;
            public const int ConnectAmount = 1;

            public const int Count = 2;
        }

        public void LoadApparatusDescription()
        {
            ApparatusDescription = new LocalizedText[ApparatusDescriptionID.Count];

            ApparatusDescription[ApparatusDescriptionID.MagikeAmount] = this.GetLocalization(nameof(ApparatusDescription) + "MagikeAmount");
            ApparatusDescription[ApparatusDescriptionID.ConnectAmount] = this.GetLocalization(nameof(ApparatusDescription) + "ConnectAmount");
        }

        public static string GetApparatusDescriptionText(int id)
            => ApparatusDescription[id].Value;

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

            ItemDescription[ItemDescriptionID.PolarizedFilter] = this.GetLocalization(nameof(ItemDescription) +"PolarizedFilter");
        }

        public static string GetItemDescriptionText(int id)
            => ItemDescription[id].Value;

        #endregion

        #region UI显示

        public static LocalizedText[] UIText { get; private set; }

        public class UITextID
        {
            /*
             * UI部分
             */

            /*
             * 魔能容器
             *      - 当前魔能量
             *      - 魔能上限
             *      - 魔能存储器名称
             */
            public const int ContainerMagikeAmount = 0;
            public const int ContainerMagikeMax = 1;
            public const int MagikeContainerName = 2;

            /*
             * 魔能线性发送器
             *      - 连接情况
             *      - 线性发送器名称
             *      - 发送量
             *      - 发送间隔
             *      - 连接距离
             */
            public const int CurrentConnect = 3;
            public const int MagikeLinerSenderName = 4;
            public const int MagikeSendTime = 5;
            public const int MagikeSendAmount = 6;
            public const int MagikeConnectLength = 7;

            public const int Count = 8;
        }

        public void LoadUIText()
        {
            UIText = new LocalizedText[UITextID.Count];

            UIText[UITextID.ContainerMagikeAmount] = this.GetLocalization(nameof(UIText) + "ContainerMagikeAmount");
            UIText[UITextID.ContainerMagikeMax] = this.GetLocalization(nameof(UIText) + "ContainerMagikeMax");
            UIText[UITextID.MagikeContainerName] = this.GetLocalization(nameof(UIText) + "MagikeContainerName");
            
            UIText[UITextID.CurrentConnect] = this.GetLocalization(nameof(UIText) + "CurrentConnect");
            UIText[UITextID.MagikeLinerSenderName] = this.GetLocalization(nameof(UIText) + "MagikeLinerSenderName");
            UIText[UITextID.MagikeSendTime] = this.GetLocalization(nameof(UIText) + "MagikeSendTime");
            UIText[UITextID.MagikeSendAmount] = this.GetLocalization(nameof(UIText) + "MagikeSendAmount");
            UIText[UITextID.MagikeConnectLength] = this.GetLocalization(nameof(UIText) + "MagikeConnectLength");
        }

        public static string GetUIText(int id)
            => UIText[id].Value;


        #endregion
    }
}
