using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using Terraria;
using static Coralite.Core.AssetDirectory;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Core
{
    public partial class CoraliteAssets : IOrderedLoadable
    {
        public float Priority => 1;

        [AttributeUsage(AttributeTargets.Class)]
        private class AutoLoadTextureAttribute(string texturePath) : Attribute
        {
            public string TexturePath => texturePath;
        }

        /// <summary>
        /// 拖尾所使用的贴图
        /// </summary>
        [AutoLoadTexture(Trails)]
        public class Trail
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
            /// 更亮
            /// </summary>
            public static ATex SlashFlatBright { get; private set; }

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
            /// 有2小一大3个抓痕的刀光<br></br>
            /// 透明底<br></br>
            /// 紫色
            /// </summary>
            public static ATex ClawSlash3APurple { get; private set; }

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

            /// <summary> 
            /// 刺很多的拖尾
            /// </summary>
            public static ATex Spike { get; private set; }

            /// <summary> 
            /// 边缘非常硬的拖尾
            /// </summary>
            public static ATex Hard { get; private set; }

            /// <summary> 
            /// 火焰拖尾
            /// </summary>
            public static ATex Fire { get; private set; }

            /// <summary> 
            /// 很亮，尾部像气流一样的拖尾
            /// </summary>
            public static ATex Highlight { get; private set; }

            /// <summary> 
            /// 原版泰拉棱镜使用的刀光贴图
            /// </summary>
            public static ATex Vanilla { get; private set; }

            /// <summary> 
            /// 顶部一条横线，整个刀光透明度较低，由主要几条横线组成
            /// </summary>
            public static ATex Split { get; private set; }

            /// <summary> 
            /// 类似推进器尾焰，高亮的柔和渐变
            /// </summary>
            public static ATex Booster { get; private set; }

            /// <summary> 
            /// 类似推进器尾焰，高亮的柔和渐变<br></br>
            /// 特殊透明底，能够使用<see cref="BlendState.AlphaBlend"/>
            /// </summary>
            public static ATex BoosterASP { get; private set; }

            /// <summary> 
            /// 类似推进器尾焰，高亮的柔和渐变
            /// </summary>
            public static ATex Strike { get; private set; }

            /// <summary> 
            /// 箭
            /// </summary>
            public static ATex Arrow { get; private set; }

            /// <summary> 
            /// 箭
            /// </summary>
            public static ATex ArrowSPA { get; private set; }

            /// <summary> 
            /// 一束光，竖过来看是一个水滴
            /// </summary>
            public static ATex LightShot { get; private set; }

            /// <summary> 
            /// 一束光，竖过来看是一个水滴
            /// </summary>
            public static ATex LightShotSPA { get; private set; }

            #endregion
        }

        /// <summary>
        /// 激光所使用的贴图，特点是左右连续
        /// </summary>
        [AutoLoadTexture(Lasers)]
        public class Laser
        {
            /// <summary> 
            /// 激光主体，一条发光横线
            /// </summary>
            public static ATex Body { get; private set; }

            /// <summary> 
            /// 激光主体，一条发光横线，更细
            /// </summary>
            public static ATex BodyThin { get; private set; }

            /// <summary> 
            /// 激光核心，左右连读的流动贴图<br></br>
            /// 原版使用，透明底
            /// </summary>
            public static ATex VanillaCoreA { get; private set; }

            /// <summary> 
            /// 激光外层的流动部分，左右连读的流动贴图<br></br>
            /// 原版使用，透明底
            /// </summary>
            public static ATex VanillaFlowA { get; private set; }

            /// <summary> 
            /// 能量流
            /// </summary>
            public static ATex EnergyFlow { get; private set; }

            /// <summary> 
            /// 气流一样的流动图
            /// </summary>
            public static ATex Airflow { get; private set; }

            /// <summary> 
            /// 气流一样的流动图
            /// </summary>
            public static ATex AirflowA { get; private set; }

            /// <summary> 
            /// 气流一样的流动图
            /// </summary>
            public static ATex AirFlow2 { get; private set; }

            /// <summary> 
            /// 主要用于溶解的贴图，类似时空隧道的样子
            /// </summary>
            public static ATex Tunnel { get; private set; }

            /// <summary> 
            /// 闪电激光
            /// </summary>
            public static ATex LightingBody { get; private set; }

            /// <summary> 
            /// 更加闪电的闪电激光
            /// </summary>
            public static ATex LightingBody2 { get; private set; }

            /// <summary> 
            /// 闪电
            /// </summary>
            public static ATex ThunderTrail { get; private set; }
        }

        /// <summary>
        /// 闪光贴图
        /// </summary>
        [AutoLoadTexture(Sparkles)]
        public class Sparkle
        {
            /// <summary> 
            /// 标准十字闪光<br></br>
            /// 横竖纵向
            /// </summary>
            public static ATex Cross { get; private set; }

            /// <summary> 
            /// 横向光外加一些散光束<br></br>
            /// 透明底
            /// </summary>
            public static ATex HShotA { get; private set; }

            /// <summary> 
            /// 横向光外加光球<br></br>
            /// 透明底
            /// </summary>
            public static ATex HShotBallA { get; private set; }

            /// <summary> 
            /// 横向光条<br></br>
            /// 透明底
            /// </summary>
            public static ATex ShotLineSPA { get; private set; }
        }

        /// <summary>
        /// 光圈贴图
        /// </summary>
        [AutoLoadTexture(Halos)]
        public class Halo
        {
            /// <summary> 
            /// 看上去像是一圈符文
            /// </summary>
            public static ATex Rune { get; private set; }
            /// <summary> 
            /// 看上去像是一圈符文
            /// </summary>
            public static ATex RuneSPA { get; private set; }

            /// <summary> 
            /// 看上去像是一圈符文<br></br>
            /// 符文在一个光圈上
            /// </summary>
            public static ATex EdgeRune { get; private set; }

            /// <summary> 
            /// 一圈亮到暗，外加外发光
            /// </summary>
            public static ATex Circle { get; private set; }
            /// <summary> 
            /// 一圈亮到暗，外加外发光
            /// </summary>
            public static ATex CircleSPA { get; private set; }

            /// <summary> 
            /// 若隐若现的一圈
            /// </summary>
            public static ATex FadeCircle { get; private set; }

            /// <summary> 
            /// 高亮的一圈
            /// </summary>
            public static ATex HighlightCircle { get; private set; }

            /// <summary> 
            /// 高亮的一圈<br></br>
            /// 透明底
            /// </summary>
            public static ATex HighlightCircleA { get; private set; }

            /// <summary> 
            /// 几个光圈叠加，同时在8个方向上有光束
            /// </summary>
            public static ATex EightLightShot { get; private set; }

            /// <summary> 
            /// 几个光圈叠加，同时在8个方向上有光束<br></br>
            /// 透明底
            /// </summary>
            public static ATex EightLightShotA { get; private set; }

            /// <summary> 
            /// 从中心向外的圆环状结构，类似能量体向周围扩散
            /// </summary>
            public static ATex Energy { get; private set; }

            /// <summary> 
            /// 从中心向外的圆环状结构，类似能量体向周围扩散<br></br>
            /// 透明底
            /// </summary>
            public static ATex EnergyA { get; private set; }

            /// <summary> 
            /// 受击扩散效果
            /// </summary>
            public static ATex Impact { get; private set; }
            /// <summary> 
            /// 受击扩散效果
            /// </summary>
            public static ATex ImpactA { get; private set; }
        }

        /// <summary>
        /// 光圈贴图
        /// </summary>
        [AutoLoadTexture(LightBalls)]
        public class LightBall
        {
            /// <summary> 
            /// 圆形亮光球
            /// </summary>
            public static ATex Ball { get; private set; }

            public static ATex BallA { get; private set; }
        }

        public void Load()
        {
            if (Main.dedServ)
                return;

            //获取所有内部类
            Type[] types = typeof(CoraliteAssets).GetNestedTypes(BindingFlags.Public | BindingFlags.Instance);

            foreach (var type in types)
            {
                AutoLoadTextureAttribute pathAttribute = type.GetCustomAttribute<AutoLoadTextureAttribute>();

                if (!type.IsAbstract && pathAttribute != null)
                {
                    //获取贴图路径
                    var ins = Activator.CreateInstance(type, null);

                    string path = pathAttribute.TexturePath;

                    //找到所有静态属性
                    var infos = type.GetProperties(BindingFlags.Public | BindingFlags.Static);

                    //为所有静态属性赋值
                    foreach (var info in infos)
                        info.SetValue(null, Get(path + info.Name));
                }
            }
        }

        public void Unload()
        {
            if (Main.dedServ)
                return;

            //获取所有内部类
            Type[] types = typeof(CoraliteAssets).GetNestedTypes(BindingFlags.Public | BindingFlags.Instance);

            foreach (var type in types)
            {
                AutoLoadTextureAttribute pathAttribute = type.GetCustomAttribute<AutoLoadTextureAttribute>();

                if (!type.IsAbstract && pathAttribute != null)
                {
                    //获取贴图路径
                    var ins = Activator.CreateInstance(type, null);

                    string path = pathAttribute.TexturePath;

                    //找到所有静态属性
                    var infos = type.GetProperties(BindingFlags.Public | BindingFlags.Static);

                    //为所有静态属性赋值
                    foreach (var info in infos)
                        info.SetValue(null, null);
                }
            }
        }

        private static ATex Get(string path)
            => Request<Texture2D>(path);
    }
}
