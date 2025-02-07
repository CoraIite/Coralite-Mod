using Coralite.Content.CustomHooks;
using Coralite.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    /// <summary>
    /// 仅用于检测连接，不提供任何发送魔能功能的发送器
    /// </summary>
    public class CheckOnlyLinerSender() : MagikeLinerSender()
    {
        public override bool CanConnect_CheckHasEntity(Point16 receiverPoint, ref string failSource)
        {
            if (!MagikeHelper.TryGetEntity(receiverPoint.X, receiverPoint.Y, out _))
            {
                failSource = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ChooseReceiver_AlreadyConnect);
                return false;
            }

            return true;
        }

        public override void Update()
        {
            Point16 p = Entity.Position;
            Vector2 size = new Vector2(ConnectLength);
            if (Helper.IsAreaOnScreen(p.ToWorldCoordinates() - Main.screenPosition - size / 2, new Vector2(ConnectLength)))
                DrawMagikeDevice.LinerSenders.Add(this);
        }

        public override void ShowInUI(UIElement parent)
        {
            //添加显示在最上面的组件名称
            UIElement title = this.AddTitle(MagikeSystem.UITextID.CheckOnlyLinerSenderName, parent);

            UIList list =
            [
                //连接距离
                this.NewTextBar(c =>MagikeSystem.GetUIText(MagikeSystem.UITextID.MagikeConnectLength), parent),
                this.NewTextBar(c =>{
                    float length= MathF.Round(c.ConnectLength/16f,1);
                    float lengthBase= MathF.Round(c.ConnectLengthBase/16f,1);
                    string sign= c.ConnectLengthExtra >= 0 ? "+" : "- ";
                    float lengthExtra= MathF.Round(c.ConnectLengthExtra/16f,1);

                    return $"  ▶ {length} ({lengthBase} {sign} {lengthExtra})";
                }, parent),

                this.NewTextBar(c =>MagikeSystem.GetUIText(MagikeSystem.UITextID.CurrentConnect),parent)
            ];

            list.SetSize(0, -title.Height.Pixels, 1, 1);
            list.SetTopLeft(title.Height.Pixels + 8, 0);

            list.QuickInvisibleScrollbar();

            for (int i = 0; i < MaxConnect; i++)
                list.Add(new ConnectButtonForComponent(i, this));

            parent.Append(list);
        }

        public override void SendData(ModPacket data)
        {
            data.Write(MaxConnectBase);
            data.Write(MaxConnectExtra);

            data.Write(ConnectLengthBase);
            data.Write(ConnectLengthExtra);

            data.Write(_receivers.Count);
            for (int i = 0; i < _receivers.Count; i++)
            {
                data.Write(_receivers[i].X);
                data.Write(_receivers[i].Y);
            }
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            MaxConnectBase = reader.ReadByte();
            MaxConnectExtra = reader.ReadByte();

            ConnectLengthBase = reader.ReadInt32();
            ConnectLengthExtra = reader.ReadInt32();

            int length = reader.ReadInt32();
            _receivers = new List<Point16>(MaxConnect);

            for (int i = 0; i < length; i++)
                _receivers[i] = new Point16(reader.ReadInt16(), reader.ReadInt16());
        }

        public override void SaveData(string preName, TagCompound tag)
            => SaveLinerSender(preName, tag);
        public override void LoadData(string preName, TagCompound tag)
            => LoadLinerSender(preName, tag);
    }
}
