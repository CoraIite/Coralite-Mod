﻿using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Core
{
    public static class CoraliteSoundID
    {
        /// <summary>
        /// 挖掘时的声音，共3种
        /// </summary>
        public static SoundStyle Dig => SoundID.Dig;

        /// <summary>
        /// 玩家受伤，共3种
        /// </summary>
        public static SoundStyle PlayerHit => SoundID.PlayerHit;

        #region 物品使用声音

        /// <summary>
        /// 大多数近战武器挥动，各种工具的使用等的声音
        /// </summary>
        public static SoundStyle Swing_Item1 => SoundID.Item1;

        /// <summary>
        /// 吃东西，召唤宠物的声音
        /// </summary>
        public static SoundStyle Eat_Item2 => SoundID.Item2;

        /// <summary>
        /// 喝药水的声音
        /// </summary>
        public static SoundStyle Drink_Item3 => SoundID.Item3;

        /// <summary>
        /// 生命水晶等的Ding~
        /// </summary>
        public static  SoundStyle Ding_Item4 => SoundID.Item4;

        /// <summary>
        /// 弓射箭的声音，大概是Jiu~
        /// </summary>
        public static SoundStyle Bow_Item5 => SoundID.Item5;

        /// <summary>
        /// 各种传送的声音，比如魔镜
        /// </summary>
        public static SoundStyle Teleport_Item6 => SoundID.Item6;

        /// <summary>
        /// 各种回旋镖，圣骑士锤，吹叶机（世花掉的那个魔法武器）的声音
        /// </summary>
        public static  SoundStyle Swing2_Item7 => SoundID.Item7;

        /// <summary>
        /// 前期各种矿物法杖法杖，一些NPC的声音，类似weng~
        /// </summary>
        public static  SoundStyle MagicStaff_Item8 => SoundID.Item8;

        /// <summary>
        /// 魔晶风暴，裂天剑等的声音，类似Xiu~
        /// </summary>
        public static SoundStyle MagicShoot_Item9 => SoundID.Item9;

        /// <summary>
        /// 石巨人拳头之类的，还有子弹落地的声音，Da!
        /// </summary>
        public static SoundStyle Hit_Item10 => SoundID.Item10;

        /// <summary>
        /// 大部分枪的声音
        /// </summary>
        public static SoundStyle Gun_Item11 => SoundID.Item11;

        /// <summary>
        /// 各种激光射线类武器的声音，例如太空枪，Biu~
        /// </summary>
        public static SoundStyle Laser_Item12 => SoundID.Item12;

        /// <summary>
        /// 喷水的声音，例如海蓝法杖，黄金雨，滋~~~
        /// </summary>
        public static SoundStyle WaterGun_Item13 => SoundID.Item13;

        /// <summary>
        /// 小炸弹的声音，例如各种手雷
        /// </summary>
        public static SoundStyle Boom_Item14 => SoundID.Item14;

        /// <summary>
        /// 棱镜，美杜莎头，各种陨石光剑的声音，嗡~~
        /// </summary>
        public static SoundStyle LaserSwing_Item15 => SoundID.Item15;

        /// <summary>
        /// 放屁声音
        /// </summary>
        public static SoundStyle Fart_Item16 => SoundID.Item16;

        /// <summary>
        /// 各种怪射毒刺的声音，黄蜂，毒刺史莱姆之类的
        /// </summary>
        public static SoundStyle Stinger_Item17 => SoundID.Item17;

        /// <summary>
        /// 原版中没用到
        /// </summary>
        public static SoundStyle NoUse_Item18 => SoundID.Item18;

        /// <summary>
        /// 原版中没用到
        /// </summary>
        public static SoundStyle NoUse_Item19 => SoundID.Item19;

        /// <summary>
        /// 类似火焰的声音，另外星云拳套，恶魔三叉戟等也用到了
        /// </summary>
        public static SoundStyle Flame_Item20 => SoundID.Item20;

        /// <summary>
        /// 水箭魔法书的声音
        /// </summary>
        public static SoundStyle WaterBolt_Item21 => SoundID.Item21;

        /// <summary>
        /// 电锯，钻头之类的使用声音，类似拉动引擎
        /// </summary>
        public static  SoundStyle Drill_Item22 => SoundID.Item22;

        /// <summary>
        /// 电锯，钻头之类的的使用声音
        /// </summary>
        public static SoundStyle Drill2_Item23 => SoundID.Item23;

        /// <summary>
        /// 各类悬浮推进器悬浮时的声音，嗡嗡嗡
        /// </summary>
        public static SoundStyle Floating_Item24 => SoundID.Item24;

        /// <summary>
        /// 召唤坐骑以及各种小妖精宠物的声音
        /// </summary>
        public static SoundStyle MountSummon_Item25 => SoundID.Item25;

        /// <summary>
        /// 竖琴的声音（你可以自己调整音高来实现原版那种根据鼠标与人物距离不同音高不同的效果）
        /// </summary>
        public static SoundStyle Harp_Item26 => SoundID.Item26;

        /// <summary>
        /// 碎冰和各种冰弹幕碎裂的声音（还有穿着寒霜套的玩家受伤的声音）
        /// </summary>
        public static SoundStyle CrushedIce_Item27 => SoundID.Item27;

        /// <summary>
        /// 各种冰魔法杖的声音，还有符文法师的攻击声音
        /// </summary>
        public static SoundStyle IceMagic_Item28 => SoundID.Item28;

        /// <summary>
        /// 使用魔力星的声音
        /// </summary>
        public static SoundStyle ManaCrystal_Item29 => SoundID.Item29;

        /// <summary>
        /// 冰魔杖放置冰块的声音
        /// </summary>
        public static SoundStyle IcePlaced_Item30 => SoundID.Item30;

        /// <summary>
        /// 发条式突击步枪的音效，是突突突的声音
        /// </summary>
        public static SoundStyle TripleGun_Item31 => SoundID.Item31;

        /// <summary>
        /// 翅膀飞行的声音
        /// </summary>
        public static SoundStyle Wing_Item32 => SoundID.Item32;

        /// <summary>
        /// 射激光的声音，比方说激光眼的射击声音，啾！啾！啾！
        /// </summary>
        public static SoundStyle LaserShoot_Item33 => SoundID.Item33;

        /// <summary>
        /// 喷火器的声音
        /// </summary>
        public static SoundStyle Flamethrower_Item34 => SoundID.Item34;

        /// <summary>
        /// 铃铛的声音，叮~（听起来可能更接近于噔~）
        /// </summary>
        public static SoundStyle Bell_Item35 => SoundID.Item35;

        /// <summary>
        /// 霰弹枪的声音，特点是枪声之后会有个抛弹壳的声音
        /// </summary>
        public static SoundStyle Shotgun_Item36 => SoundID.Item36;

        /// <summary>
        /// 锤子敲铁砧的声音，也是哥布林重铸的声音
        /// </summary>
        public static SoundStyle Knock_Item37 => SoundID.Item37;

        /// <summary>
        /// 霰弹枪的声音，只有枪声，没有抛弹壳的声音
        /// </summary>
        public static SoundStyle Shotgun2_Item38 => SoundID.Item38;

        /// <summary>
        /// 扔飞刀的声音，吸血鬼刀，腐化灾兵，剃刀松使用的音效。
        /// </summary>
        public static SoundStyle ThrowKnives_Item39 => SoundID.Item39;

        /// <summary>
        /// 太空海豚枪，狙击步枪使用的音效
        /// </summary>
        public static SoundStyle Gun2_Item40 => SoundID.Item40;

        /// <summary>
        /// 一些机枪和手枪使用的音效，比如凤凰冲击波
        /// </summary>
        public static SoundStyle Gun3_Item41 => SoundID.Item41;

        /// <summary>
        /// 哀木等的扔火球的声音
        /// </summary>
        public static SoundStyle FireThrow_Item42 => SoundID.Item42;

        /// <summary>
        /// 高级些的法杖的使用音效，比如雷电法杖，毒液法杖等
        /// </summary>
        public static SoundStyle MagicStaff2_Item43 => SoundID.Item42;

        /// <summary>
        /// 一些召唤杖的声音，比如史莱姆法杖
        /// </summary>
        public static SoundStyle SummonStaff_Item44 => SoundID.Item44;

        /// <summary>
        /// 巨鹿掉的眼球塔召唤杖的攻击音效
        /// </summary>
        public static SoundStyle FireBall_Item45 => SoundID.Item45;

        /// <summary>
        /// 蜘蛛女王召唤杖和冰霜九头蛇召唤杖的声音
        /// </summary>
        public static SoundStyle HorribleSummon_Item46 => SoundID.Item46;

        /// <summary>
        /// The Axe ：电吉他斧的声音
        /// </summary>
        public static SoundStyle ElectricGuitar_Item47 => SoundID.Item47;

        /// <summary>
        /// 挖冰雪块的声音
        /// </summary>
        public static SoundStyle DigIce_Item48 => SoundID.Item48;

        /// <summary>
        /// 挖冰雪块的声音
        /// </summary>
        public static SoundStyle DigIce2_Item49 => SoundID.Item49;

        /// <summary>
        /// 挖冰雪块的声音
        /// </summary>
        public static SoundStyle DigIce3_Item50 => SoundID.Item50;








        #endregion
    }
}