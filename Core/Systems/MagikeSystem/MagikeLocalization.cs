using Terraria.Localization;

namespace Coralite.Core.Systems.MagikeSystem
{
    public partial class MagikeSystem : ILocalizedModType
    {
        public string LocalizationCategory => "MagikeSystem";

        public static LocalizedText NewKnowledgeUnlocked { get; private set; }

        public LocalizedText LearnedMagikeBase { get; private set; }
        public LocalizedText LearnedMagikeAdvanced { get; private set; }

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

        public void LoadLocalization()
        {
            LearnedMagikeBase = this.GetLocalization("learnedMagikeBase");
            LearnedMagikeAdvanced = this.GetLocalization("learnedMagikeAdvanced");
            NewKnowledgeUnlocked = this.GetLocalization("NewKnowledgeUnlocked", () => "魔能辞典中解锁了新的知识");

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

        public void UnloadLocalization()
        {
            LearnedMagikeBase = null;
            LearnedMagikeAdvanced = null;
            NewKnowledgeUnlocked = null;

            ConnectStaff = null;
        }

        public static string GetConnectStaffText(int id)
            => ConnectStaff[id].Value;
    }
}
