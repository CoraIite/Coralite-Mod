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
