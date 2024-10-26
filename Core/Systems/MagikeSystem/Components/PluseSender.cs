using Coralite.Content.CustomHooks;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Humanizer;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Core.Systems.MagikeSystem.Components
{
    public abstract class PluseSender:MagikeSender, IConnectLengthModify
    {
        /// <summary>
        /// 是否会真的发送，仅在容器内有魔能的时候才会执行这一项操作
        /// </summary  
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

        public override bool CanSend()
        {
            Timer--;

            if (DoSend)
            {
                float factor = (float)Timer / SendDelay;

                if (factor > 0.2f)
                {
                    Rotation = Helper.Lerp( Rotation,MathHelper.Pi/6, 0.05f);
                }
                else
                {
                    //生成粒子
                    Point16 topleft = (Entity as MagikeTileEntity).Position;

                    MagikeHelper.GetMagikeAlternateData(topleft.X, topleft.Y, out var data, out var alternate);
                    Point16 dir = alternate switch
                    {
                        MagikeHelper.MagikeAlternateStyle.Bottom => new Point16(0, -1),
                        MagikeHelper.MagikeAlternateStyle.Top => new Point16(0, 1),
                        MagikeHelper.MagikeAlternateStyle.Left => new Point16(1, 0),
                        MagikeHelper.MagikeAlternateStyle.Right => new Point16(-1, 0),
                        _ => Point16.Zero,
                    };

                    Point16 center = topleft + new Point16(data.Width / 2, data.Height / 2) + dir;

                    if (Main.rand.NextBool())
                    {
                        Dust d = Dust.NewDustPerfect(center.ToWorldCoordinates() + Main.rand.NextVector2Circular(8, 8), DustID.CrystalSerpent_Pink
                            , new Vector2(dir.X, dir.Y) * 2f, Scale: 1f);
                        d.noGravity = true;
                    }
                }
            }
            else if(Rotation>0.001f)
                Rotation = Helper.Lerp(Rotation, 0, 0.02f);

            if (Timer <= 0)
            {
                Timer = SendDelay;
                Point16? p = TryFindReceiver(out MagikeContainer container, out _);

                bool old = DoSend;

                DoSend = Entity.GetMagikeContainer().Magike > 0 && p.HasValue && !container.FullMagike;


                return old && DoSend;
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

            Point16? p = TryFindReceiver(out var targetContainer,out Point16 center);
            if (!p.HasValue)
                return;

            if (targetContainer.FullMagike)//如果满了就不发送
                return;

            int sendCount =Math.Min( container.Magike, targetContainer.MagikeMax - targetContainer.Magike);
            
            targetContainer.AddMagike(sendCount);
            container.ReduceMagike(sendCount);

            DoSend = Entity.GetMagikeContainer().Magike > 0 && !targetContainer.FullMagike;

            //生成粒子特效
            SendParticles(center.ToWorldCoordinates(), p.Value.ToWorldCoordinates());
        }

        /// <summary>
        /// 尝试找到接收者
        /// </summary>
        /// <returns></returns>
        private Point16? TryFindReceiver(out MagikeContainer targetContainer,out Point16 center)
        {
            targetContainer= null;  
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

            center = topleft + new Point16(data.Width/2, data.Height/2);

            int howMany = ConnectLength / 16;

            for (int k = 2; k < howMany; k++)//向指定方向找寻接收器
            {
                Point16 targetPoiint = center + new Point16(dir.X*k,dir.Y*k) ;

                if (!WorldGen.InWorld(targetPoiint.X,targetPoiint.Y))
                    return null;

                if (!MagikeHelper.TryGetEntity(targetPoiint.X, targetPoiint.Y, out var entity))
                    continue;

                if (!entity.IsMagikeContainer())
                    continue;

                targetContainer=entity.GetMagikeContainer();
                return targetPoiint;
            }

            return null;
        }

        private void SendParticles(Vector2 selfPoint, Vector2 TargetPoint)
        {
            Vector2 dir = (TargetPoint - selfPoint).SafeNormalize(Vector2.Zero);
            int count = (int)(TargetPoint - selfPoint).Length();
            count /= 4;

            for (int k = 0; k < count; k++)
            {
                Vector2 pos = selfPoint + dir * k * 4;
                int num402 = Dust.NewDust(pos + Main.rand.NextVector2Circular(4, 4), 0, 0, DustID.RainbowTorch
                    , 0f, 0f, 150, Color.Transparent, 1.2f);
                Main.dust[num402].color = Coralite.MagicCrystalPink;
                Main.dust[num402].noGravity = true;

                Dust d = Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(4, 4), DustID.CrystalSerpent_Pink
                    , new Vector2(dir.X, dir.Y) * 2f, Scale: 1f);
                d.noGravity = true;
            }
        }
    }
}
