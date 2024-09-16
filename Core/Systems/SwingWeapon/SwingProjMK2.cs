using Coralite.Core.Prefabs.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Map;
using static Coralite.Core.Systems.SwingWeapon.SwingProjMK2;

namespace Coralite.Core.Systems.SwingWeapon
{
    /// <summary>
    /// 使用ai0控制combo
    /// </summary>
    public abstract class SwingProjMK2 : BaseHeldProj
    {
        /// <summary>
        /// 手柄长度
        /// </summary>
        public virtual float HandleLength { get; } = 5;
        public float ScaledHandleLength { get => HandleLength * Projectile.scale; }

        /// <summary>
        /// 剑刃长度
        /// </summary>
        public virtual float BladeLength { get; } = 40;
        public float ScaledBladeLength { get => BladeLength * Projectile.scale; }

        /// <summary>
        /// 弹幕中心与中心点之间的距离
        /// </summary>
        public float DistanceToCenter { get; set; }
        /// <summary>
        /// 自身的旋转角度
        /// </summary>
        public float SelfRotation { get; set; }
        /// <summary>
        /// 手柄距离的百分比
        /// </summary>
        public float HandlePercent { get; set; }

        /// <summary>
        /// 甜点攻击
        /// </summary>
        public SweetController Sweet {  get; protected set; }
        public TrailController Trail {  get; protected set; }

        public ref float Combo => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];



        /// <summary>
        /// 可以根据连段与挥舞阶段获取对应的起始角度
        /// </summary>
        public SwingController[] Swings { get; protected set; }

        protected bool Init = true;


        public ref Vector2 RotateVec2 => ref Projectile.velocity;
        public ref float Rotation => ref Projectile.rotation;

        /// <summary>
        /// 绝对角度的记录器
        /// </summary>
        public record struct SwingAngle()
        {
            /// <summary>
            /// 起始角度
            /// </summary>
            public required float StartAngle;

            /// <summary>
            /// 总旋转角度
            /// </summary>
            public required float TotalAngle;

            /// <summary>
            /// 额外缩放旋转角
            /// </summary>
            public float ExtraScaleAngle;

            /// <summary>
            /// 使用当前角度作为起始角度
            /// </summary>
            public bool UseCurrentAngleAsStart = false;

            /// <summary>
            /// 变换过的起始角度
            /// </summary>
            public float TransformedStartAngle { get;private set; }

            /// <summary>
            /// 变换过的起始角度
            /// </summary>
            public float TransformedTotalAngle { get;private set; }

            /// <summary>
            /// 角度的渐变
            /// </summary>
            public ISmoother AngleSmoothFunction = Coralite.Instance.NoSmootherInstance;

            /// <summary>
            /// 计算角度的变换
            /// </summary>
            /// <param name="aimAngle"></param>
            /// <param name="direction"></param>
            public void CalculateAngle(SwingController controller)
            {
                float aimAngle = controller.GetAimAngle(controller.Proj.Owner);
                int direction = controller.RecordPlaerDirection;
                TransformedStartAngle = UseCurrentAngleAsStart ? controller.Proj.Rotation : (aimAngle - (direction * StartAngle));
                TransformedTotalAngle = TotalAngle * direction;
            }

            /// <summary>
            /// 获取当前的角度
            /// </summary>
            /// <param name="factor"></param>
            /// <returns></returns>
            public readonly float GetAngle(float factor)
              => TransformedStartAngle + (TransformedTotalAngle * AngleSmoothFunction.Smoother(factor));
        }



        public struct TrailController()
        {
            /// <summary>
            /// 绘制残影拖尾
            /// </summary>
            public bool DrawShadowTrail;
            /// <summary>
            /// 绘制顶点拖尾
            /// </summary>
            public bool DrawVertexTrail;

            public float TrailAlpha = 1;

            public Vector2[] TopOffset { get; set; }
            public Vector2[] BottomOffset { get; set; }

            public void InitTrails(int pointCount)
            {

            }
        }

        public struct CollisionColtroller
        {
            public Vector2 Top;
            public Vector2 Bottom;


        }

        public struct SweetController
        {
            public bool SweetHit;

            /// <summary>
            /// 甜点位置的百分比
            /// </summary>
            public float SweetPercent;

            /// <summary>
            /// 酸点位置的百分比集合
            /// </summary>
            public float[] SourPercents;
        }

        public struct HitFreezeController
        {
            public bool HasFreeze;

            public int FreezeTime;
            public int FreezeMaxTime;
        }

        public enum SwingStates
        {
            BeforeSwing,
            OnSwing,
            AfterSwing,
        }

        public override bool ShouldUpdatePosition() => false;



        public sealed override void AI()
        {

        }


        #region 绘制部分

        public virtual Texture2D GetTrailTex()
            => CoraliteAssets.Trail.Slash.Value;

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        #endregion
    }

    public class SwingController
    {
        /*
         * 包含基本的挥舞控制
         * 
         * 1.角度控制
         *  通过传入角度控制器来控制转向
         *  包含起始角度，总旋转角度，可以使用GetAimAngle获取瞄准的角度
         *  在生成时传入角度基础角度控制器，进入准备阶段时会改变一次角度，之后进入挥舞阶段时会再次改变角度
         *  
         *  2.缩放控制
         *  使用一些委托控制缩放
         *  3.距离控制
         *  
         *  4.自转控制
         *  
         */

        /// <summary>
        /// 当前的状态
        /// </summary>
        public SwingStates SwingState { get; protected set; }
        /// <summary>
        /// 基础角度记录
        /// </summary>
        public SwingAngle Angle { get; protected set; }

        public SwingProjMK2 Proj { get; set; }

        public int BeforeSlashTime;
        public int SlashTime;
        public int AfterSlashTime;

        public int RecordPlaerDirection;

        /// <summary>
        /// 获取瞄准的角度
        /// </summary>
        public Func<Player, float> GetAimAngle;

        public Action<SwingProjMK2> OnBeforeSlash;
        public Action<SwingProjMK2> OnSlash;
        public Action<SwingProjMK2> OnAfterSlash;

        public SwingController(SwingProjMK2 projMK2, SwingAngle Angle, Func<Player, float> GetAimAngle)
        {
            this.Angle = Angle;
            Proj = projMK2;
            RecordPlaerDirection = projMK2.Owner.direction;
            this.GetAimAngle = GetAimAngle;
        }

        public void Initialize()
        {
            Proj.Rotation = Angle.GetAngle(0);
        }

        public void Update()
        {
            switch (SwingState)
            {
                default:
                case SwingStates.BeforeSwing:
                    OnBeforeSlash?.Invoke(Proj);
                    break;
                case SwingStates.OnSwing:
                    OnSlash?.Invoke(Proj);
                    break;
                case SwingStates.AfterSwing:
                    OnAfterSlash?.Invoke(Proj);
                    break;
            }

            Proj.Timer--;
            if (Proj.Timer < 1)
            {
                SwingState++;
                if (SwingState > SwingStates.AfterSwing)
                    Proj.Projectile.Kill();
            }
        }

        public virtual void SetAngle()
        {

        }

        public virtual void UpdateAngle()
        {

        }
    }
}
