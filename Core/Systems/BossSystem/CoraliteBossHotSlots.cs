using System.IO;

namespace Coralite.Core.Systems.BossSystem
{
    /// <summary>
    /// Boss 热字段槽：随 <c>SendExtraAI / ReceiveExtraAI</c> 与位置、速度、<c>npc.ai[]</c> 同包原子过线的一组浮点量。<br/>
    /// 运动数学在两端各自确定性积分，这些槽只负责把权威端的状态计时器、子拍与一次性决策带给客户端，
    /// 让两端的状态数学从同一起点推进（C3：运动数学读到的每个量都要能在客户端重建）。<br/>
    /// 槽位约定：0 Timer、1 Counter、2 Beat、3 Flags 位标志、4..11 状态自用 A..H。
    /// 基座 <see cref="CoraliteBossState{TContext}.WriteHot"/> / <see cref="CoraliteBossState{TContext}.ReadHot"/> 只碰前三个，
    /// 子类先调 base 再写读自用槽。
    /// </summary>
    public sealed class CoraliteBossHotSlots
    {
        /// <summary>槽数。12 = 3 个基座槽 + 1 个位标志 + 8 个自用槽，够覆盖已盘点 boss 里最多的 localAI[0..3] + 4 个决策量。</summary>
        public const int SlotCount = 12;

        /// <summary>状态 <c>Timer</c>（收养带 ±2 帧容差）。</summary>
        public const int Timer = 0;
        /// <summary>状态 <c>Counter</c>（超时兜底计数，硬收养）。</summary>
        public const int Counter = 1;
        /// <summary>招式内子拍号 <c>BeatIndex</c>（硬收养）。</summary>
        public const int Beat = 2;
        /// <summary>位标志（bit0..bit23 可用；float 精确表达到 2^24），用 <see cref="HasFlag"/> / <see cref="SetFlag"/> 读写。</summary>
        public const int Flags = 3;
        /// <summary>状态自用槽 A..H：锁向、锚点、连段号、选中的方向等，含义由各状态自己约定并写注释。</summary>
        public const int A = 4;
        public const int B = 5;
        public const int C = 6;
        public const int D = 7;
        public const int E = 8;
        public const int F = 9;
        public const int G = 10;
        public const int H = 11;

        private readonly float[] values = new float[SlotCount];

        /// <summary>
        /// 客户端：收到的包里 <c>ai[0]</c> 与本地当前状态 id 不一致（这包同时带来了换态），
        /// 热字段先落在这里，等 <c>StateMachine.Update()</c> 完成 NetSync 换态、新状态 <c>OnEnter</c> 清零之后再收养。
        /// 由 <see cref="CoraliteBossContext.ReadNet"/> 置位、<see cref="CoraliteBossContext.ConsumePendingHotAdopt"/> 消费。
        /// </summary>
        public bool PendingAdopt { get; set; }

        /// <summary>按槽读写。</summary>
        public float this[int slot]
        {
            get => values[slot];
            set => values[slot] = value;
        }

        /// <summary>读位标志。</summary>
        public bool HasFlag(int bit) => (((int)values[Flags]) & (1 << bit)) != 0;

        /// <summary>写位标志。</summary>
        public void SetFlag(int bit, bool on)
        {
            int flags = (int)values[Flags];
            if (on)
            {
                flags |= 1 << bit;
            }
            else
            {
                flags &= ~(1 << bit);
            }

            values[Flags] = flags;
        }

        /// <summary>全部清零（换态时不要调：客户端靠旧值撑到收养，见 <see cref="PendingAdopt"/>）。</summary>
        public void Clear()
        {
            for (int i = 0; i < SlotCount; i++)
            {
                values[i] = 0f;
            }
        }

        /// <summary>权威端 <c>SendExtraAI</c>：整块写出，定长，不需要长度前缀。</summary>
        public void Write(BinaryWriter writer)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                writer.Write(values[i]);
            }
        }

        /// <summary>客户端 <c>ReceiveExtraAI</c>：整块读入。先读完再判断，保证流对齐。</summary>
        public void Read(BinaryReader reader)
        {
            for (int i = 0; i < SlotCount; i++)
            {
                values[i] = reader.ReadSingle();
            }
        }
    }
}
