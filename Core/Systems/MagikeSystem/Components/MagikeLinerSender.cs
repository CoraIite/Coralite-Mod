using Coralite.Content.CustomHooks;
using Coralite.Content.UI.MagikeApparatusPanel;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public class MagikeLinerSender : MagikeSender, IUIShowable, IConnectLengthModify
    {
        /// <summary> 基础连接数量 </summary>
        public int MaxConnectBase { get; protected set; }
        /// <summary> 额外连接数量 </summary>
        public int MaxConnectExtra { get; set; }

        /// <summary> 可连接数量 </summary>
        public int MaxConnect { get => MaxConnectBase + MaxConnectExtra; }

        /// <summary> 基础连接距离 </summary>
        public int ConnectLengthBase { get => LengthBase; protected set => LengthBase = value; }
        /// <summary> 额外连接距离 </summary>
        public int ConnectLengthExtra { get; set; }

        /// <summary> 连接距离 </summary>
        public int ConnectLength { get => ConnectLengthBase + ConnectLengthExtra; }

        /// <summary> 当前连接者 </summary>
        public int CurrentConnector => _receivers.Count;

        protected List<Point16> _receivers = [];

        /// <summary>
        /// 仅供获取使用，那么为什么不用private set 呢，因为懒得改了，反正区别不大
        /// </summary>
        public List<Point16> Receivers { get => _receivers; }

        public int LengthExtra { get ; set; }
        public int LengthBase { get ; set ; }

        public MagikeLinerSender()
        {
            _receivers = new List<Point16>(MaxConnect);
        }

        #region 发送工作相关

        public override void Update()
        {
            Point16 p = Entity.Position;
            Vector2 size = new Vector2(ConnectLength);
            if (Helper.OnScreen(p.ToWorldCoordinates() - Main.screenPosition-size/2, new Vector2(ConnectLength)))
                DrawMagikeDevice.LinerSenders.Add(this);

            //发送时间限制
            if (!CanSend())
                return;

            //获取魔能容器并检测能否发送魔能
            MagikeContainer container = Entity.GetMagikeContainer();
            if (!GetSendAmount(container, out int amount))
                return;

            //直接发送
            for (int i = 0; i < _receivers.Count; i++)
            {
                if (!container.HasMagike)//自身没魔能了就跳出
                    break;
                Send(container, _receivers[i], amount);
            }
        }

        /// <summary>
        /// 获取具体发送多少，最少为剩余量除以所有连接数量
        /// </summary>
        /// <param name="container"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool GetSendAmount(MagikeContainer container, out int amount)
        {
            amount = 0;
            //没有魔能直接返回
            if (!container.HasMagike)
                return false;

            int currentMagike = container.Magike;

            //设置初始发送量
            amount = UnitDelivery;

            //如果魔能量不够挨个发一份那么就把当前剩余的魔能挨个发一份
            if (currentMagike < amount * CurrentConnector)
                amount = currentMagike / CurrentConnector;

            //防止小于1
            if (amount < 1)
                amount = 1;

            return true;
        }

        /// <summary>
        /// 发送魔能
        /// </summary>
        public void Send(MagikeContainer selfMagikeContainer, Point16 position, int amount)
        {
            //如果无法获取物块实体就移除
            if (!MagikeHelper.TryGetEntity(position, out MagikeTP receiverEntity))
                goto remove;

            //如果不是魔能容器那么就丢掉喽
            if (!receiverEntity.IsMagikeContainer())
                goto remove;

            /*
             * 对于传入的魔能数量，分为以下几种情况
             *  - 接收者能接受，就全额发送
             *  - 接收者无法接受全部，就发送接收者能接受的数量
             *  
             *  由于已经事先判断过自身魔能容量所以这里必定有足够的魔能来发送
             */

            MagikeContainer receiver = receiverEntity.GetMagikeContainer();
            if (receiver.FullMagike)
                return;

            //限制不溢出
            receiver.LimitReceiveOverflow(ref amount);

            receiver.AddMagike(amount);
            selfMagikeContainer.ReduceMagike(amount);
            OnSend(Entity.Position, position);

            return;
        remove:
            RemoveReceiver(position);
        }

        #endregion

        #region 连接相关

        /// <summary>
        /// 检测是否能连接
        /// </summary>
        /// <param name="receiverPoint"></param>
        /// <returns></returns>
        public virtual bool CanConnect(Point16 receiverPoint, out string failSource)
        {
            failSource = "";

            if (!CanConnect_CheckHasEntity(receiverPoint,ref failSource))
                return false;

            if (!CanConnect_CheckConnected(receiverPoint,ref failSource))
                return false;

            if (!CanConnect_CheckCapacity(ref failSource))
                return false;

            Point16 selfPoint = Entity.Position;

            if (!CanConnect_CheckSelf(selfPoint, receiverPoint, ref failSource))
                return false;

            if (!CanConnect_CheckLength(selfPoint, receiverPoint, ref failSource))
                return false;

            return true;
        }

        public virtual bool CanConnect_CheckHasEntity(Point16 receiverPoint, ref string failSource)
        {
            if (!MagikeHelper.TryGetEntityWithComponent(receiverPoint.X, receiverPoint.Y, MagikeComponentID.MagikeContainer, out _))
            {
                failSource = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ChooseReceiver_NotFound);
                return false;
            }

            return true;
        }

        public virtual bool CanConnect_CheckConnected(Point16 receiverPoint, ref string failSource)
        {
            if (Receivers.Contains(receiverPoint))
            {
                failSource = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ChooseReceiver_AlreadyConnect);
                return false;
            }

            return true;
        }

        public virtual bool CanConnect_CheckCapacity(ref string failSource)
        {
            if (FillUp())
            {
                failSource = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ConnectFail_ConnectorFillUp);
                return false;
            }

            return true;
        }

        public virtual bool CanConnect_CheckSelf(Point16 selfPoint, Point16 receiverPoint,ref string failSource)
        {
            //检测是否是自己
            if (receiverPoint == selfPoint)
            {
                failSource = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ConnectFail_CantBeSelf);
                return false;
            }

            return true;
        }

        public virtual bool CanConnect_CheckLength(Point16 selfPoint, Point16 receiverPoint, ref string failSource)
        {
            Vector2 selfCenter = Helper.GetMagikeTileCenter(selfPoint);
            Vector2 targetCenter = Helper.GetMagikeTileCenter(receiverPoint);

            //太远了导致无法连接
            if (Vector2.Distance(selfCenter, targetCenter) > ConnectLength)
            {
                failSource = MagikeSystem.GetConnectStaffText(MagikeSystem.StaffTextID.ConnectFail_TooFar);
                return false;
            }

            return true;
        }

        public void Connect(Point16 receiverPoint)
        {
            _receivers.Add(receiverPoint);
        }

        /// <summary>
        /// 移除接收者
        /// </summary>
        /// <param name="receiverPoint"></param>
        public void RemoveReceiver(Point16 receiverPoint) => _receivers.Remove(receiverPoint);

        /// <summary>
        /// 是否已经装满
        /// </summary>
        /// <returns></returns>
        public bool FillUp() => MaxConnect <= _receivers.Count;

        /// <summary>
        /// 啥也没链接
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty() => _receivers.Count == 0;

        /// <summary>
        /// 第一个连接者，请自行判断是否有这么一个
        /// </summary>
        /// <returns></returns>
        public Point16 FirstConnector() => _receivers[0];

        /// <summary>
        /// 重新检测是否能连接，超出长度直接断开
        /// </summary>
        public void RecheckConnect()
        {
            if (Entity is null)
                return;

            Timer = SendDelay;
            Vector2 selfPos = Helper.GetMagikeTileCenter(Entity.Position);

            for (int i = _receivers.Count - 1; i >= 0; i--)
            {
                if (i + 1 > MaxConnect || !TileEntity.ByPosition.ContainsKey(_receivers[i]))
                {
                    _receivers.RemoveAt(i);
                    continue;
                }

                Vector2 targetPos = Helper.GetMagikeTileCenter(_receivers[i]);
                if (Vector2.Distance(selfPos, targetPos) > ConnectLength)
                    _receivers.RemoveAt(i);
            }
        }

        #endregion

        #region UI部分

        public virtual void ShowInUI(UIElement parent)
        {
            //添加显示在最上面的组件名称
            UIElement title = this.AddTitle(MagikeSystem.UITextID.MagikeLinerSenderName, parent);

            UIList list =
            [
                //发送时间
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.MagikeSendTime), parent),
                this.NewTextBar(SendDelayText, parent),

                //发送量
                this.NewTextBar(c =>MagikeSystem.GetUIText(MagikeSystem.UITextID.MagikeSendAmount), parent),
                this.NewTextBar(UnitDeliveryText, parent),

                //连接距离
                this.NewTextBar(c =>MagikeSystem.GetUIText(MagikeSystem.UITextID.MagikeConnectLength), parent),
                this.NewTextBar(ConnectLengthText, parent),

                this.NewTextBar(c =>MagikeSystem.GetUIText(MagikeSystem.UITextID.CurrentConnect),parent)
            ];

            list.SetSize(0, -title.Height.Pixels, 1, 1);
            list.SetTopLeft(title.Height.Pixels + 8, 0);

            list.QuickInvisibleScrollbar();

            for (int i = 0; i < MaxConnect; i++)
                list.Add(new ConnectButtonForComponent(i, this));

            parent.Append(list);
        }

        public virtual string SendDelayText(MagikeLinerSender s)
        {
            float timer = MathF.Round(s.Timer / 60f, 1);
            float delay = MathF.Round(s.SendDelay / 60f, 1);
            float delayBase = MathF.Round(s.SendDelayBase / 60f, 1);
            float DelayBonus = s.SendDelayBonus;

            return $"  ▶ {timer} / {MagikeHelper.BonusColoredText(delay.ToString(), DelayBonus,true)} ({delayBase} * {MagikeHelper.BonusColoredText(DelayBonus.ToString(), DelayBonus,true)})";
        }

        public virtual string UnitDeliveryText(MagikeLinerSender s)
            => $"\n  ▶ {MagikeHelper.BonusColoredText(s.UnitDelivery.ToString(), UnitDeliveryBonus)} ({s.UnitDeliveryBase} * {MagikeHelper.BonusColoredText(s.UnitDeliveryBonus.ToString(), UnitDeliveryBonus)})";

        public virtual string ConnectLengthText(MagikeLinerSender s)
        {
            float length = MathF.Round(s.ConnectLength / 16f, 1);
            float lengthBase = MathF.Round(s.ConnectLengthBase / 16f, 1);
            string sign = s.ConnectLengthExtra >= 0 ? "+" : "- ";
            float lengthExtra = MathF.Round(s.ConnectLengthExtra / 16f, 1);

            return $"  ▶ {MagikeHelper.BonusColoredText2(length.ToString(), s.ConnectLengthExtra)} ({lengthBase} {sign} {MagikeHelper.BonusColoredText2(lengthExtra.ToString(), s.ConnectLengthExtra)})";
        }

        public void AddText(UIList list, Func<MagikeLinerSender, string> textFunc, UIElement parent)
        {
            list.Add(new ComponentUIElementText<MagikeLinerSender>(textFunc, this, parent));
        }

        #endregion

        /// <summary>
        /// 获取魔能量，一定得有唯一的魔能容器才行，没有的话我给你一拳
        /// </summary>
        /// <returns></returns>
        public int GetMagikeAmount()
            => Entity.GetMagikeContainer().Magike;

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);
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
            base.LoadData(preName, tag);

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

    public class ConnectButtonForComponent : UIElement
    {
        private int _index;
        private MagikeLinerSender _sender;

        public ConnectButtonForComponent(int index, MagikeLinerSender sender)
        {
            _index = index;
            _sender = sender;

            Texture2D tex = MagikeSystem.ConnectUI[(int)MagikeSystem.ConnectUIAssetID.Botton].Value;

            Width.Set(tex.Width + 40, 0);
            Height.Set(tex.Height + 10, 0);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (_sender.Receivers.IndexInRange(_index))
                _sender.Receivers.RemoveAt(_index);

            base.LeftClick(evt);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D tex = MagikeSystem.ConnectUI[(int)MagikeSystem.ConnectUIAssetID.Botton].Value;

            var style = GetDimensions();
            Vector2 pos = style.Position() + new Vector2(style.Width - (tex.Width / 2), style.Height / 2);
            Vector2 origin = tex.Size() / 2;

            bool ishover = IsMouseHovering;
            float scale = ishover ? 1.2f : 1f;

            spriteBatch.Draw(tex, pos, null, Color.White, 0, origin, scale, 0, 0);

            bool indexInRange = _sender.Receivers.IndexInRange(_index);

            if (indexInRange)
            {
                Texture2D tex2 = MagikeSystem.ConnectUI[(int)MagikeSystem.ConnectUIAssetID.Flow].Value;
                spriteBatch.Draw(tex2, pos, null, Color.White, 0, origin, scale, 0, 0);
            }

            if (ishover)
            {
                if (indexInRange)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(default, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

                    Color drawColor = Color.Lerp(Color.White, Color.Coral, (MathF.Sin((int)Main.timeForVisualEffects * 0.1f) / 2) + 0.5f);

                    Vector2 selfPos = Helper.GetMagikeTileCenter(MagikeApparatusPanel.CurrentEntity.Position);
                    Vector2 aimPos = Helper.GetMagikeTileCenter(_sender.Receivers[_index]);

                    MagikeSystem.DrawConnectLine(spriteBatch, selfPos, aimPos, Main.screenPosition, drawColor);

                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

                    UICommon.TooltipMouseText(MagikeSystem.GetUIText(MagikeSystem.UITextID.ClickToDisconnect));
                }
            }

            //位置在右侧往左按钮的距离再一半
            pos.X -= style.Width * 3 / 4;
            pos.Y = style.Center().Y + 4;

            string temp = indexInRange ? "◆" : "◇";

            if (_index >= _sender.MaxConnectBase)
                temp = $"[c/80d3ff:{temp}]";

            Utils.DrawBorderString(spriteBatch, temp, pos, Color.White, 1, 0f, 0.5f);
        }
    }
}
