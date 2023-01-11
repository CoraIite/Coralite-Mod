using Microsoft.Xna.Framework;

namespace Coralite.Core.Systems.IKSystem
{
    public class IKSolverCCD
    {
        public Arrow[] arrows;//箭头集合

        public Vector2 target;//目标位置

        public bool useLimt ;//是否应用角度限制

        public int iterations ;//迭代次数

        public IKSolverCCD(Arrow[] arrows, Vector2 target, bool useLimt = false, int iterations = 4)
        {
            this.arrows = arrows;
            this.target = target;
            this.useLimt = useLimt;
            this.iterations = iterations;
        }

        /// <summary>
        /// 箭头线段跟踪目标位置
        /// </summary>
        public void FollowTarget()
        {
            for (int n = 0; n < iterations; n++)
            {
                for (int i = arrows.Length - 1; i >= 0; i--)
                {
                    arrows[i].Follow(target, arrows[arrows.Length - 1].EndPos, useLimt);
                    UpdatePosition(i == 0 ? Vector2.UnitX: arrows[i - 1].Forward, i);
                }
            }
        }

        /// <summary>
        /// 从指定顺序索引开始更新箭头起始位置
        /// </summary>
        /// <param name="right"></param>
        /// <param name="n"></param>
        public void UpdatePosition(Vector2 right, int n = 0)
        {
            Vector2 origin = arrows[n].StartPos;
            for (; n < arrows.Length; n++)
            {
                arrows[n].CalculateStartAndEnd(origin, right);
                origin = arrows[n].EndPos;
                right = arrows[n].Forward;
            }
        }
    }
}
