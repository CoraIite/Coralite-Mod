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
            public const int Connect_TooFar = 3;
            public const int Connect_Success = 4;

            public const int Deconnect = 5;

            /*
             * 璀璨连接仪
             *  - 和魔能连接仪差不多，多一个选中发送器后右键框选接收器
             *  - 会显示连接数量和失败数量
             */

            public const int BrilliantConnect = 6;

            public const int Count = BrilliantConnect + 1;
        }

        public void LoadLocalization()
        {
            LearnedMagikeBase = this.GetLocalization("learnedMagikeBase");
            LearnedMagikeAdvanced = this.GetLocalization("learnedMagikeAdvanced");
            NewKnowledgeUnlocked = this.GetLocalization("NewKnowledgeUnlocked", () => "魔能辞典中解锁了新的知识");

            ConnectStaff = new LocalizedText[ConnectStaffID.Count];
            ConnectStaff[ConnectStaffID.ChooseSender_Found] = this.GetLocalization("ChooseSender_Found"
                , () => "已选择魔能发送器");
            ConnectStaff[ConnectStaffID.ChooseSender_NotFound] = this.GetLocalization("ChooseSender_NotFound"
                , () => "未找到魔能发送器");
            ConnectStaff[ConnectStaffID.ChooseReceiver_NotFound] = this.GetLocalization("ChooseReceiver_NotFound"
                , () => "未找到接收器");
            ConnectStaff[ConnectStaffID.Connect_TooFar] = this.GetLocalization("Connect_TooFar"
                , () => "接收器距离太远");
            ConnectStaff[ConnectStaffID.Connect_Success] = this.GetLocalization("ChooseReceiver_Success"
                , () => "成功建立连接");
            ConnectStaff[ConnectStaffID.Deconnect] = this.GetLocalization("Deconnect"
                , () => "成功取消连接");
            ConnectStaff[ConnectStaffID.BrilliantConnect] = this.GetLocalization("BrilliantConnect"
                , () => "成功连接{0}个仪器，未成功连接{1}个仪器");
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
