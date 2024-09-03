using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static Coralite.Core.AssetDirectory;
using static Terraria.ModLoader.ModContent;
using ATex = ReLogic.Content.Asset<Microsoft.Xna.Framework.Graphics.Texture2D>;

namespace Coralite.Core
{
    public class CoraliteAssets : IOrderedLoadable
    {
        public float Priority => 1;

        /// <summary>
        /// 拖尾所使用的贴图
        /// </summary>
        public static class Trail
        {
            #region 基础刀光贴图及其变种

            /// <summary> 
            /// 最初始的刀光贴图，是一堆线条 
            /// </summary>
            public static ATex Slash { get; private set; }

            /// <summary> 
            /// 最初始的刀光贴图，是一堆线条<br></br>
            /// 与原本的区别是这个的高度只有之前的一半，被压缩了
            /// </summary>
            public static ATex SlashFlat { get; private set; }

            /// <summary> 
            /// 最初始的刀光贴图，是一堆线条<br></br>
            /// 与原本的区别是这个的高度只有之前的一半，被压缩了<br></br>
            /// 镜像叠加，变成了两侧短中间长的类似于推进器尾焰的形状
            /// </summary>
            public static ATex SlashFlatVMirror { get; private set; }

            /// <summary> 
            /// 最初始的刀光贴图，是一堆线条<br></br>
            /// 与原本的区别是这个的高度只有之前的一半，被压缩了<br></br>
            /// 同时还额外添加了高斯模糊，较为圆滑，没那么锐利
            /// </summary>
            public static ATex SlashFlatBlur { get; private set; }

            /// <summary> 
            /// 最初始的刀光贴图，是一堆线条<br></br>
            /// 与原本的区别是这个的高度只有之前的一半，被压缩了<br></br>
            /// 同时还额外添加了高斯模糊，较为圆滑，没那么锐利<br></br>
            /// 镜像叠加，变成了两侧短中间长的类似于推进器尾焰的形状
            /// </summary>
            public static ATex SlashFlatBlurVMirror { get; private set; }

            /// <summary> 
            /// 最初始的刀光贴图，是一堆线条<br></br>
            /// 与原本的区别是这个的高度只有之前的一半，被压缩了<br></br>
            /// 同时还额外添加了高斯模糊，较为圆滑，没那么锐利<br></br>
            /// 双重镜像叠加
            /// </summary>
            public static ATex SlashFlatBlurHVMirror { get; private set; }

            /// <summary> 
            /// 最初始的刀光贴图，是一堆线条<br></br>
            /// 与原本的区别是这个的高度只有之前的一半，被压缩了<br></br>
            /// 同时还额外添加了高斯模糊，较为圆滑，没那么锐利<br></br>
            /// 图片尺寸更小
            /// </summary>
            public static ATex SlashFlatBlurSmall { get; private set; }

            /// <summary> 
            /// 最初始的刀光贴图，是一堆线条<br></br>
            /// 与原本的区别是这个的高度只有之前的一半，被压缩了<br></br>
            /// 下半部分逐渐消散
            /// </summary>
            public static ATex SlashFlatFade { get; private set; }

            #endregion

            #region 轻刀光及其变种

            /// <summary> 
            /// 颜色很浅的刀光，只有顶部有一条较亮
            /// </summary>
            public static ATex LiteSlash { get; private set; }

            /// <summary> 
            /// 颜色很浅的刀光，只有顶部有一条较亮<br></br>
            /// 变亮了很多，顶部的一条亮线也更粗
            /// </summary>
            public static ATex LiteSlashBright { get; private set; }

            /// <summary> 
            /// 颜色很浅的刀光，只有顶部有三条较亮<br></br>
            /// 变亮了很多，顶部的一条亮线也更粗<br></br>
            /// 图片镜像叠加，同时有3层亮线条
            /// </summary>
            public static ATex LiteSlashBrightHMirror { get; private set; }

            #endregion

            #region 爪形刀光

            /// <summary> 
            /// 有2小一大3个抓痕的刀光<br></br>
            /// 透明底
            /// </summary>
            public static ATex ClawSlash3A { get; private set; }

            /// <summary> 
            /// 有2小一大3个抓痕的刀光<br></br>
            /// 透明底<br></br>
            /// 黄色
            /// </summary>
            public static ATex ClawSlash3AYellow { get; private set; }

            /// <summary> 
            /// 有4个抓痕的刀光
            /// </summary>
            public static ATex ClawSlash4 { get; private set; }

            #endregion

            #region 其他特殊造型

            /// <summary> 
            /// 从右向左的由窄到宽，看上去像是流星的尾迹一样
            /// </summary>
            public static ATex Meteor { get; private set; }

            /// <summary> 
            /// 从右向左的由窄到宽，看上去像是流星的尾迹一样<br></br>
            /// 透明底
            /// </summary>
            public static ATex MeteorA { get; private set; }

            /// <summary> 
            /// 一个被拉伸的亮光圆球，大概类似推进器火焰的形状<br></br>
            /// 透明底
            /// </summary>
            public static ATex CircleA { get; private set; }

            /// <summary> 
            /// 从右向左由白色渐变为透明，同时上下两侧有线条<br></br>
            /// 透明底
            /// </summary>
            public static ATex EdgeA { get; private set; }

            #endregion

            internal static void Load()
            {
                Slash = Get(Trails + nameof(Slash));
                SlashFlat = Get(Trails + nameof(SlashFlat));
                SlashFlatVMirror = Get(Trails + nameof(SlashFlatVMirror));
                SlashFlatBlur = Get(Trails + nameof(SlashFlatBlur));
                SlashFlatBlurVMirror = Get(Trails + nameof(SlashFlatBlurVMirror));
                SlashFlatBlurHVMirror = Get(Trails + nameof(SlashFlatBlurHVMirror));
                SlashFlatBlurSmall = Get(Trails + nameof(SlashFlatBlurSmall));
                SlashFlatFade = Get(Trails + nameof(SlashFlatFade));

                LiteSlash = Get(Trails + nameof(LiteSlash));
                LiteSlashBright = Get(Trails + nameof(LiteSlashBright));
                LiteSlashBrightHMirror = Get(Trails + nameof(LiteSlashBrightHMirror));

                ClawSlash3A = Get(Trails + nameof(ClawSlash3A));
                ClawSlash3AYellow = Get(Trails + nameof(ClawSlash3AYellow));
                ClawSlash4 = Get(Trails + nameof(ClawSlash4));

                Meteor = Get(Trails + nameof(Meteor));
                MeteorA = Get(Trails + nameof(MeteorA));
                CircleA = Get(Trails + nameof(CircleA));
                EdgeA = Get(Trails + nameof(EdgeA));
            }

            internal static void Unload()
            {
                Slash = null;
                SlashFlat = null;
                SlashFlatVMirror = null;
                SlashFlatBlur = null;
                SlashFlatBlurVMirror = null;
                SlashFlatBlurHVMirror = null;
                SlashFlatBlurSmall = null;
                SlashFlatFade = null;

                LiteSlash = null;
                LiteSlashBright = null;
                LiteSlashBrightHMirror = null;

                ClawSlash3A = null;
                ClawSlash3AYellow = null;
                ClawSlash4 = null;

                Meteor = null;
                MeteorA = null;
                CircleA = null;
                EdgeA = null;
            }
        }

        /// <summary>
        /// 激光所使用的贴图，特点是左右连续
        /// </summary>
        public static class Laser
        {

        }

        /// <summary>
        /// 闪光贴图
        /// </summary>
        public static class Sparkle
        {

        }

        /// <summary>
        /// 光圈贴图
        /// </summary>
        public static class Halo
        {

        }

        /// <summary>
        /// 光圈贴图
        /// </summary>
        public static class LightBall
        {

        }

        public void Load()
        {
            if (Main.dedServ)
                return;
            
            Trail.Load();
        }

        public void Unload()
        {
            if (Main.dedServ)
                return;

            Trail.Unload();
        }

        private static ATex Get(string path)
            => Request<Texture2D>(path);
    }
}
