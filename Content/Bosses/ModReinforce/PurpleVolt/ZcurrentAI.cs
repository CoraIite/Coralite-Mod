namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public partial class  ZacurrentDragon
    {
        /// <summary>
        /// 用于记录当前的攻击状态
        /// </summary>
        internal ref float State => ref NPC.ai[0];
        /// <summary>
        /// 阶段内部使用，用于处理连招
        /// </summary>
        internal ref float SonState => ref NPC.ai[1];
        internal ref float RecorderAI => ref NPC.ai[2];
        internal ref float Timer => ref NPC.ai[3];

        internal ref float Recorder => ref NPC.localAI[0];
        internal ref float Recorder2 => ref NPC.localAI[1];
        internal ref float StateRecorder => ref NPC.localAI[2];
        internal ref float UseMoveCount => ref NPC.localAI[3];

        #region AI控制部分

        private enum AIStates
        {
            //动画阶段
            onSpawnAnmi = 1,
            onKillAnim,

            /// <summary> 紫伏形态的切换 </summary>
            PurpleVoltExchange,

            //单招
            /// <summary> 闪电突袭，先短冲后进行一次长冲 </summary>
            LightningRaid,


            //连段
        }

        public override void AI()
        {
        }

        #endregion

        #region AI切换部分

        /// <summary>
        /// 重新设置各类与状态相关的数值
        /// </summary>
        public void ResetFields()
        {

        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState()
        {

        }

        #endregion
    }
}
