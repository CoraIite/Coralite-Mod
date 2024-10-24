using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class PluseSender:MagikeSender, IConnectLengthModify
    {
        /// <summary>
        /// 是否会真的发送，仅在容器内有魔能的时候才会执行这一项操作
        /// </summary>
        public bool DoSend;
        /// <summary>
        /// 两侧框架的旋转
        /// </summary>
        public float Rotation;

        /// <summary> 基础连接距离 </summary>
        public int ConnectLengthBase { get => LengthBase; protected set => LengthBase = value; }
        /// <summary> 额外连接距离 </summary>
        public int ConnectLengthExtra { get; set; }

        /// <summary> 连接距离 </summary>
        public int ConnectLength { get => ConnectLengthBase + ConnectLengthExtra; }

        public int LengthExtra { get; set; }
        public int LengthBase { get; set; }

        public bool UpdateTime()
        {
            Timer--;
            if (Timer <= 0)
            {
                Timer = SendDelay;
                DoSend = Entity.GetMagikeContainer().Magike > 0;
                return true;
            }

            return false;
        }

        public override void Update(IEntity entity)
        {
            //发送时间限制
            if (!CanSend())
                return;

            //获取魔能容器并检测能否发送魔能
            MagikeContainer container = Entity.GetMagikeContainer();
            if (container.Magike < 1)
                return;

        }

        /// <summary>
        /// 尝试找到接收者
        /// </summary>
        /// <returns></returns>
        private Point? TryFindReceiver()
        {
            Point16 topleft = (Entity as MagikeTileEntity).Position;

            //获取基础属性，确定寻找的方向
            MagikeHelper.GetMagikeAlternateData(topleft.X, topleft.Y, out var data, out var alternate);
            Point16 dir = alternate switch
            {
                MagikeHelper.MagikeAlternateStyle.Bottom => new Point16(0, -1),
                MagikeHelper.MagikeAlternateStyle.Top => new Point16(0, 1),
                MagikeHelper.MagikeAlternateStyle.Left => new Point16(1, 0),
                MagikeHelper.MagikeAlternateStyle.Right => new Point16(-1, 0),
                _ => Point16.Zero,
            };

            Point16 center = topleft + new Point16(data.Width/2, data.Height/2);

            int howMany = ConnectLength / 16;

            for (int k = 2; k < howMany; k++)//向指定方向找寻接收器
            {
                Point16 targetPoiint = center + new Point16(dir.X*k,dir.Y*k) ;

                if (!WorldGen.InWorld(targetPoiint.X,targetPoiint.Y))
                    return null;


            }

            return null;
        }
    }
}
