using Coralite.Content.CustomHooks;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System;
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

        public override void SaveData(string preName, TagCompound tag)
        {
            tag.Add(preName + nameof(MaxConnectBase), MaxConnectBase);
            tag.Add(preName + nameof(MaxConnectExtra), MaxConnectExtra);

            tag.Add(preName + nameof(ConnectLengthBase), ConnectLengthBase);
            tag.Add(preName + nameof(ConnectLengthExtra), ConnectLengthExtra);

            for (int i = 0; i < _receivers.Count; i++)
            {
                tag.Add(string.Concat(preName, nameof(_receivers), i.ToString(), "X"), _receivers[i].X);
                tag.Add(string.Concat(preName, nameof(_receivers), i.ToString(), "Y"), _receivers[i].Y);
            }
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            MaxConnectBase = tag.GetInt(preName + nameof(MaxConnectBase));
            MaxConnectExtra = tag.GetInt(preName + nameof(MaxConnectExtra));

            ConnectLengthBase = tag.GetInt(preName + nameof(ConnectLengthBase));
            ConnectLengthExtra = tag.GetInt(preName + nameof(ConnectLengthExtra));

            int i = 0;

            while (tag.TryGet(string.Concat(preName, nameof(_receivers), i.ToString(), "X"), out short X))
            {
                _receivers.Add(new Point16(X, tag.GetShort(string.Concat(preName, nameof(_receivers), i.ToString(), "Y"))));
                i++;
            }
        }
    }
}
