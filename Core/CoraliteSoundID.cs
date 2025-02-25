using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Core
{
    /// <summary>
    /// 音效合集，可以使用Ctrl+F搜索中文名来查找你想要的音效
    /// 部分重名了的不包含在内，例如那些吉他和架子鼓的音效（只是避免出现2个一模一样的东西而已）
    /// </summary>
    public static class CoraliteSoundID
    {
        #region 玩家音效

        /// <summary> 男性玩家受伤，共3种</summary>
        public static SoundStyle MalePlayerHit => SoundID.PlayerHit;

        /// <summary> 女性玩家受伤的音效，共3种 </summary>
        public static SoundStyle FemalePlayerHit => SoundID.FemaleHit;

        /// <summary> 饥荒世界中男性玩家受伤，共3种 </summary>
        public static SoundStyle DSTMaleHurt => SoundID.DSTMaleHurt;

        /// <summary> 饥荒世界中女性玩家受伤，共3种 </summary>
        public static SoundStyle DSTFemaleHurt => SoundID.DSTFemaleHurt;

        /// <summary> 玩家死亡</summary>
        public static SoundStyle PlayerKilled => SoundID.PlayerKilled;

        /// <summary>捡起物品的声音（也可用于点击按钮）</summary>
        public static SoundStyle Grab => SoundID.Grab;

        /// <summary> 魔力值回复至满时的音效，防反饰品冷却结束，部分武器射弹幕冷却时间结束时的音效（如冰雪剑） </summary>
        public static SoundStyle MaxMana => SoundID.MaxMana;

        /// <summary> 玩家溺水/淹死/缺少氧气的音效 </summary>
        public static SoundStyle Drown => SoundID.Drown;

        /// <summary> 打开NPC聊天面板时的音效</summary>
        public static SoundStyle Chat => SoundID.Chat;

        /// <summary> 二段跳的音效，例如云瓶，独角兽坐骑等</summary>
        public static SoundStyle DoubleJump => SoundID.DoubleJump;

        /// <summary> 疾跑的音效，比如赫尔墨斯鞋，泰拉闪耀鞋等</summary>
        public static SoundStyle Run => SoundID.Run;

        /// <summary> 微光BUFF消失时的音效</summary>
        public static SoundStyle ShimmeringExpires => SoundID.Shimmer2;

        #endregion

        #region 物品音效

        #region 各种枪

        /// <summary> 大部分枪的声音 </summary>
        public static SoundStyle Gun_Item11 => SoundID.Item11;

        /// <summary> 发条式突击步枪的音效，是突突突的声音 </summary>
        public static SoundStyle TripleGun_Item31 => SoundID.Item31;

        /// <summary> 霰弹枪的声音，特点是枪声之后会有个抛弹壳的声音</summary>
        public static SoundStyle Shotgun_Item36 => SoundID.Item36;

        /// <summary> 霰弹枪的声音，只有枪声，没有抛弹壳的声音 </summary>
        public static SoundStyle Shotgun2_Item38 => SoundID.Item38;

        /// <summary> 太空海豚枪，狙击步枪使用的音效 </summary>
        public static SoundStyle Gun2_Item40 => SoundID.Item40;

        /// <summary>一些机枪和手枪使用的音效，比如凤凰冲击波 </summary>
        public static SoundStyle Gun3_Item41 => SoundID.Item41;

        /// <summary> 外星泡泡枪的声音</summary>
        public static SoundStyle Xenopopper_Bubble_Item95 => SoundID.Item95;

        /// <summary> 外星泡泡枪射击的声音 </summary>
        public static SoundStyle Xenopopper_BubbleBroke_Item96 => SoundID.Item96;

        /// <summary>飞镖手枪射击的声音</summary>
        public static SoundStyle DartPistol_Item98 => SoundID.Item98;

        /// <summary>飞镖步枪射击的声音</summary>
        public static SoundStyle DartRifle_Item99 => SoundID.Item99;

        /// <summary>钉枪射击的声音</summary>
        public static SoundStyle NailGun_Item108 => SoundID.Item108;


        #endregion
        #region 各种激光

        /// <summary> 各种激光射线类武器的声音，Biu~ </summary>
        public static SoundStyle Laser_Item12 => SoundID.Item12;

        /// <summary> 棱镜，美杜莎头，各种陨石光剑的声音，嗡~~ </summary>
        public static SoundStyle LaserSwing_Item15 => SoundID.Item15;

        /// <summary> 射激光的声音，比方说激光眼的射击声音，啾！啾！啾！ </summary>
        public static SoundStyle LaserShoot_Item33 => SoundID.Item33;

        /// <summary> 脉冲弓的声音，Jiu~</summary>
        public static SoundStyle LaserShoot2_Item75 => SoundID.Item75;

        /// <summary> 激光加特林的使用声音</summary>
        public static SoundStyle LaserMachinegun_Item91 => SoundID.Item91;

        /// <summary> 太空枪的声音 </summary>
        public static SoundStyle SpaceGun_Item157 => SoundID.Item157;

        /// <summary>灰光枪，橙光枪的声音 </summary>
        public static SoundStyle LaserGun_Item158 => SoundID.Item158;



        #endregion
        #region 乐器

        /// <summary> 竖琴的声音（你可以自己调整音高来实现原版那种根据鼠标与人物距离不同音高不同的效果） </summary>
        public static SoundStyle Harp_Item26 => SoundID.Item26;

        /// <summary> The Axe ：电吉他斧的声音 </summary>
        public static SoundStyle ElectricGuitar_Item47 => SoundID.Item47;

        /// <summary> lvy，雨歌，星星吉他弹奏的声音</summary>
        public static SoundStyle GuitarC_Item133 => SoundID.Item133;

        /// <summary> lvy，雨歌，星星吉他弹奏的声音 </summary>
        public static SoundStyle GuitarD_Item134 => SoundID.Item134;

        /// <summary> lvy，雨歌，星星吉他弹奏的声音 </summary>
        public static SoundStyle GuitarEm_Item135 => SoundID.Item135;

        /// <summary> lvy，雨歌，星星吉他弹奏的声音 </summary>
        public static SoundStyle GuitarG_Item136 => SoundID.Item136;

        /// <summary> lvy，雨歌，星星吉他弹奏的声音</summary>
        public static SoundStyle GuitarBm_Item137 => SoundID.Item137;

        /// <summary> lvy，雨歌，星星吉他弹奏的声音 </summary>
        public static SoundStyle GuitarAm_Item138 => SoundID.Item138;

        /// <summary> 架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumHiHat_Item139 => SoundID.Item139;

        /// <summary> 架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumTomHigh_Item140 => SoundID.Item140;

        /// <summary> 架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumTomLow_Item141 => SoundID.Item141;

        /// <summary> 架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumTomMid_Item142 => SoundID.Item142;

        /// <summary> 架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音） </summary>
        public static SoundStyle DrumClosedHiHat_Item143 => SoundID.Item143;

        /// <summary>架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumCymbal1_Item144 => SoundID.Item144;

        /// <summary> 架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumCymbal2_Item145 => SoundID.Item145;

        /// <summary> 架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumKick_Item146 => SoundID.Item146;

        /// <summary>架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音）</summary>
        public static SoundStyle DrumTamaSnare_Item147 => SoundID.Item147;

        /// <summary> 架子鼓的声音（因为不太了解架子鼓所以也不知道该如何称呼这些声音） </summary>
        public static SoundStyle DrumFloorTom_Item148 => SoundID.Item148;

        #endregion
        #region 旧日军团武器

        /// <summary> 弩车塔，共3种，很正常的弩箭射出的声音</summary>
        public static SoundStyle DD2_BallistaTowerShot => SoundID.DD2_BallistaTowerShot;

        /// <summary> 爆炸机关，共3种，火球爆炸</summary>
        public static SoundStyle FireBallExplosion_DD2_ExplosiveTrapExplode => SoundID.DD2_ExplosiveTrapExplode;

        /// <summary> 爆炸烈焰塔，共3种，火焰射击</summary>
        public static SoundStyle FireShoot_DD2_FlameburstTowerShot => SoundID.DD2_FlameburstTowerShot;

        /// <summary> 闪电光环，共4种，动静很小的电击声</summary>
        public static SoundStyle QuietElectric_DD2_LightningAuraZap => SoundID.DD2_LightningAuraZap;

        /// <summary> 弩车、爆炸机关、爆炸烈焰或闪电光环被召唤，生成建筑的声音</summary>
        public static SoundStyle Summon_DD2_DefenseTowerSpawn => SoundID.DD2_DefenseTowerSpawn;


        /// <summary> 双足翼龙怒气，共3种</summary>
        public static SoundStyle FireShoot_DD2_BetsysWrathShot => SoundID.DD2_BetsysWrathShot;

        /// <summary> 双足翼龙怒气，共3种</summary>
        public static SoundStyle FireHitDD2_BetsysWrathImpact => SoundID.DD2_BetsysWrathImpact;

        /// <summary> 无限智慧巨著，共3种</summary>
        public static SoundStyle DD2_BookStaffCast => SoundID.DD2_BookStaffCast;

        /// <summary> 无限智慧巨著，共3种</summary>
        public static SoundStyle DD2_BookStaffTwisterLoop => SoundID.DD2_BookStaffTwisterLoop;

        /// <summary> 恐怖关刀，共3种</summary>
        public static SoundStyle Swing_DD2_GhastlyGlaiveImpactGhost => SoundID.DD2_GhastlyGlaiveImpactGhost;

        /// <summary> 恐怖关刀，共3种</summary>
        public static SoundStyle Swing_DD2_GhastlyGlaivePierce => SoundID.DD2_GhastlyGlaivePierce;

        /// <summary> 瞌睡章鱼，共3种</summary>
        public static SoundStyle DD2_MonkStaffGroundImpact => SoundID.DD2_MonkStaffGroundImpact;

        /// <summary> 瞌睡章鱼，共4种</summary>
        public static SoundStyle Swing_DD2_MonkStaffSwing => SoundID.DD2_MonkStaffSwing;

        /// <summary> 幽灵凤凰，共3种</summary>
        public static SoundStyle FireShoot_DD2_PhantomPhoenixShot => SoundID.DD2_PhantomPhoenixShot;

        /// <summary> 飞龙，共3种</summary>
        public static SoundStyle DD2_SonicBoomBladeSlash => SoundID.DD2_SonicBoomBladeSlash;

        /// <summary> 天龙之怒，共3种</summary>
        public static SoundStyle DD2_SkyDragonsFuryCircle => SoundID.DD2_SkyDragonsFuryCircle;

        /// <summary> 天龙之怒，共3种</summary>
        public static SoundStyle DD2_SkyDragonsFuryShot => SoundID.DD2_SkyDragonsFuryShot;

        /// <summary> 天龙之怒，共4种</summary>
        public static SoundStyle DD2_SkyDragonsFurySwing => SoundID.DD2_SkyDragonsFurySwing;

        #endregion

        //----------其他-----------


        /// <summary> 大多数近战武器挥动，各种工具的使用等的声音 </summary>
        public static SoundStyle Swing_Item1 => SoundID.Item1;

        /// <summary> 吃东西，召唤宠物的声音</summary>
        public static SoundStyle Eat_Item2 => SoundID.Item2;

        /// <summary> 喝药水的声音 </summary>
        public static SoundStyle Drink_Item3 => SoundID.Item3;

        /// <summary> 生命水晶等的Ding~ </summary>
        public static SoundStyle Ding_Item4 => SoundID.Item4;

        /// <summary> 弓射箭的声音，大概是Jiu~ </summary>
        public static SoundStyle Bow_Item5 => SoundID.Item5;

        /// <summary> 各种传送的声音，比如魔镜</summary>
        public static SoundStyle Teleport_Item6 => SoundID.Item6;

        /// <summary> 各种回旋镖，圣骑士锤，吹叶机（世花掉的那个魔法武器）的声音</summary>
        public static SoundStyle Swing2_Item7 => SoundID.Item7;

        /// <summary> 前期各种矿物法杖法杖，一些NPC的声音，类似weng~ </summary>
        public static SoundStyle MagicStaff_Item8 => SoundID.Item8;

        /// <summary> 魔晶风暴，裂天剑等的声音，类似Xiu~</summary>
        public static SoundStyle MagicShoot_Item9 => SoundID.Item9;

        /// <summary> 石巨人拳头之类的，还有子弹落地的声音，Da! </summary>
        public static SoundStyle Hit_Item10 => SoundID.Item10;

        /// <summary> 喷水的声音，例如海蓝法杖，黄金雨，滋~~~ </summary>
        public static SoundStyle WaterShoot_Item13 => SoundID.Item13;

        /// <summary> 小炸弹的声音，例如各种手雷 </summary>
        public static SoundStyle Boom_Item14 => SoundID.Item14;

        /// <summary> 放屁声音</summary>
        public static SoundStyle Fart_Item16 => SoundID.Item16;

        /// <summary> 各种怪射毒刺的声音，黄蜂，毒刺史莱姆之类的 </summary>
        public static SoundStyle Stinger_Item17 => SoundID.Item17;

        /// <summary> 原版中没用到 </summary>
        public static SoundStyle NoUse_Item18 => SoundID.Item18;

        /// <summary> 原版中没用到 </summary>
        public static SoundStyle NoUse_Item19 => SoundID.Item19;

        /// <summary> 类似火焰的声音，另外星云拳套，恶魔三叉戟等也用到了 </summary>
        public static SoundStyle Flame_Item20 => SoundID.Item20;

        /// <summary> 水箭魔法书的声音 </summary>
        public static SoundStyle WaterBolt_Item21 => SoundID.Item21;

        /// <summary> 电锯，钻头之类的使用声音，类似拉动引擎 </summary>
        public static SoundStyle Drill_Item22 => SoundID.Item22;

        /// <summary> 电锯，钻头之类的的使用声音</summary>
        public static SoundStyle Drill2_Item23 => SoundID.Item23;

        /// <summary> 各类悬浮推进器悬浮时的声音，嗡嗡嗡 </summary>
        public static SoundStyle Floating_Item24 => SoundID.Item24;

        /// <summary> 召唤坐骑以及各种小妖精宠物的声音 </summary>
        public static SoundStyle MountSummon_Item25 => SoundID.Item25;

        /// <summary> 碎冰和各种冰弹幕碎裂的声音（还有穿着寒霜套的玩家受伤的声音） </summary>
        public static SoundStyle CrushedIce_Item27 => SoundID.Item27;

        /// <summary> 各种冰魔法杖的声音，还有符文法师的攻击声音 </summary>
        public static SoundStyle IceMagic_Item28 => SoundID.Item28;

        /// <summary> 使用魔力星的声音 </summary>
        public static SoundStyle ManaCrystal_Item29 => SoundID.Item29;

        /// <summary> 冰魔杖放置冰块的声音</summary>
        public static SoundStyle IcePlaced_Item30 => SoundID.Item30;

        /// <summary> 翅膀飞行的声音 </summary>
        public static SoundStyle Wing_Item32 => SoundID.Item32;

        /// <summary> 喷火器的声音 </summary>
        public static SoundStyle Flamethrower_Item34 => SoundID.Item34;

        /// <summary> 铃铛的声音，叮~（听起来可能更接近于噔~） </summary>
        public static SoundStyle Bell_Item35 => SoundID.Item35;

        /// <summary> 锤子敲铁砧的声音，也是哥布林重铸的声音 </summary>
        public static SoundStyle Knock_Item37 => SoundID.Item37;

        /// <summary> 扔飞刀的声音，吸血鬼刀，腐化灾兵，剃刀松使用的音效。 </summary>
        public static SoundStyle ThrowKnives_Item39 => SoundID.Item39;

        /// <summary> 高级些的法杖的使用音效，比如雷电法杖，毒液法杖等 </summary>
        public static SoundStyle MagicStaff2_Item43 => SoundID.Item42;

        /// <summary> 一些召唤杖的声音，比如史莱姆法杖 </summary>
        public static SoundStyle SummonStaff_Item44 => SoundID.Item44;

        /// <summary> 巨鹿掉的眼球塔召唤杖的攻击音效 </summary>
        public static SoundStyle FireBall_Item45 => SoundID.Item45;

        /// <summary> 蜘蛛女王召唤杖和冰霜九头蛇召唤杖的声音 </summary>
        public static SoundStyle HorribleSummon_Item46 => SoundID.Item46;

        /// <summary> 雪球碎裂的声音 </summary>
        public static SoundStyle SnowBall_Item51 => SoundID.Item51;

        /// <summary> 矿车铁轨放置时的声音，Ding! </summary>
        public static SoundStyle MinecartTrack_Item52 => SoundID.Item52;

        /// <summary> 矿车上铁轨时的声音，Ding- </summary>
        public static SoundStyle MinecartHit_Item53 => SoundID.Item53;

        /// <summary>气泡破了时候的声音，Pa- </summary>
        public static SoundStyle Bubble_Item54 => SoundID.Item54;

        /// <summary> 矿车减速的声音，chi- </summary>
        public static SoundStyle MinecartSlowDown_Item55 => SoundID.Item55;

        /// <summary> 矿车反弹的声音（我不知道这个在哪里用到了），duang~~~ </summary>
        public static SoundStyle MinecartBounces_Item56 => SoundID.Item56;

        /// <summary> 彩虹猫剑的音效，共2种 </summary>
        public static SoundStyle Meowmere => SoundID.Meowmere;

        /// <summary> 彩虹猫剑的声音，喵呜~ </summary>
        public static SoundStyle Mio_Mewo_Miao_Item57 => SoundID.Item57;

        /// <summary> 彩虹猫剑的声音，喵呜~ </summary>
        public static SoundStyle Mio_Mewo_Miao_Item58 => SoundID.Item58;

        /// <summary> 猪猪存钱罐召唤时的声音，哼~ </summary>
        public static SoundStyle MoneyPig_Item59 => SoundID.Item59;

        /// <summary> 泰拉之刃，沙漠精的某个招式的声音，类似能量波 </summary>
        public static SoundStyle TerraBlade_Item60 => SoundID.Item60;

        /// <summary> 榴弹发射器的声音（世纪之花掉落），巨鹿掉落的气喇叭的声音 </summary>
        public static SoundStyle GrenadeLauncher_Item61 => SoundID.Item61;

        /// <summary> 榴弹爆炸的声音 </summary>
        public static SoundStyle BigBOOM_Item62 => SoundID.Item62;

        /// <summary> 吹管的声音 </summary>
        public static SoundStyle Blowpipe_Item63 => SoundID.Item63;

        /// <summary> 强化吹管，工程蓝图的声音（这玩意也有声音？）/ </summary>
        public static SoundStyle Blowgun_Item64 => SoundID.Item64;

        /// <summary> 原版未使用，感觉像是强化再强化的吹管的声音</summary>
        public static SoundStyle NoUse_BlowgunPlus_Item65 => SoundID.Item65;

        /// <summary> 雷云法杖，巨鹿掉的天气魔棒的声音 </summary>
        public static SoundStyle StrongWinds_Item66 => SoundID.Item66;

        /// <summary> 彩虹枪的声音 </summary>
        public static SoundStyle RainbowGun_Item67 => SoundID.Item67;

        /// <summary> 原版未使用，听上去像那种非常炫酷的魔法射击的声音 </summary>
        public static SoundStyle NoUse_SuperMagicShoot_Item68 => SoundID.Item68;

        /// <summary> 大地魔杖的声音（石巨人掉的那个放滚石的)</summary>
        public static SoundStyle StaffOfEarth_Item69 => SoundID.Item69;

        /// <summary> 石头碎裂的声音</summary>
        public static SoundStyle StoneBurst_Item70 => SoundID.Item70;

        /// <summary> 斩击的声音，死神镰刀使用了 </summary>
        public static SoundStyle Slash_Item71 => SoundID.Item71;

        /// <summary> 暗影束法杖的声音，Diu-- </summary>
        public static SoundStyle ShadowBeam_Item72 => SoundID.Item72;

        /// <summary> 狱火叉的使用声音</summary>
        public static SoundStyle FireFork_Item73 => SoundID.Item73;

        /// <summary> 狱火叉的火球爆炸的声音 </summary>
        public static SoundStyle FireBallExplosion_Item74 => SoundID.Item74;

        /// <summary> 黄蜂法杖的召唤声音</summary>
        public static SoundStyle HoneyStaffSummon_Item76 => SoundID.Item76;

        /// <summary> 小鬼法杖的召唤声音</summary>
        public static SoundStyle FireStaffSummon_Item77 => SoundID.Item77;

        /// <summary> 月亮门法杖，蜘蛛女王法杖，七彩水晶法杖的召唤声音</summary>
        public static SoundStyle SpecialSummon_Item78 => SoundID.Item78;

        /// <summary> 白色的胡萝卜的召唤声音，召唤兔子坐骑 </summary>
        public static SoundStyle RabbitMount_Item79 => SoundID.Item79;

        /// <summary> 猪鱼坐骑的召唤声音（是猪龙稀有掉落物的那个）</summary>
        public static SoundStyle PigronMount_Item80 => SoundID.Item80;

        /// <summary> 史莱姆坐骑的召唤声音 </summary>
        public static SoundStyle SlimeMount_Item81 => SoundID.Item81;

        /// <summary> 泰拉棱镜，双子眼召唤杖的召唤声音，星光灯笼的使用声音</summary>
        public static SoundStyle TerraprismaSummon_Item82 => SoundID.Item82;

        /// <summary> 蜘蛛法杖的声音</summary>
        public static SoundStyle SpiderStaff_Item83 => SoundID.Item83;

        /// <summary> 水龙卷刃的使用声音</summary>
        public static SoundStyle WaterTyphoon_Item84 => SoundID.Item84;

        /// <summary> 泡泡枪的使用声音</summary>
        public static SoundStyle BubbleGun_Item85 => SoundID.Item85;

        /// <summary> 原版未使用，水滴落入水中的声音</summary>
        public static SoundStyle NoUse_WaterDrop_Item86 => SoundID.Item86;

        /// <summary> 原版未使用，水滴落入水中的声音 </summary>
        public static SoundStyle NoUse_WaterDrop2_Item87 => SoundID.Item87;

        /// <summary> 月曜，陨石法杖的使用声音 </summary>
        public static SoundStyle LunarFlare_Item88 => SoundID.Item88;

        /// <summary> 陨石法杖的爆炸声音</summary>
        public static SoundStyle MeteorImpact_Item89 => SoundID.Item89;

        /// <summary> 外星蛞蝓的召唤声音，感觉可以用于魔法弹幕射击的声音</summary>
        public static SoundStyle ScutlixMount_Item90 => SoundID.Item90;

        /// <summary> 战斗指南1，2的使用声音，3种动物通行卷的使用声音</summary>
        public static SoundStyle NPCSummon_NPCStrengthen_Item92 => SoundID.Item92;

        /// <summary> 原版未使用，类似电流的声音 </summary>
        public static SoundStyle NoUse_Electric_Item93 => SoundID.Item93;

        /// <summary> 电圈发射器的声音</summary>
        public static SoundStyle ElectricExplosion_Item94 => SoundID.Item94;

        /// <summary> 蜂膝弓射击的声音 </summary>
        public static SoundStyle TheBeesKnees_Item97 => SoundID.Item97;

        /// <summary> 诅咒焰法杖的声音（放诅咒焰火墙的那个） </summary>
        public static SoundStyle ClingerStaff_Item100 => SoundID.Item100;

        /// <summary> 水晶魔棒的声音（神圣大宝箱怪的那个） </summary>
        public static SoundStyle Crystal_Item101 => SoundID.Item101;

        /// <summary> 暗影焰弓，空中灾祸的声音</summary>
        public static SoundStyle Bow2_Item102 => SoundID.Item102;

        /// <summary> 暗影焰妖娃的声音 </summary>
        public static SoundStyle DeathCalling_Item103 => SoundID.Item103;

        /// <summary> 原版未使用，和暗影焰妖娃的声音类似 </summary>
        public static SoundStyle NoUse_DeathCalling2_Item104 => SoundID.Item104;

        /// <summary> 狂星之怒的声音 </summary>
        public static SoundStyle StarFalling_Item105 => SoundID.Item105;

        /// <summary> 扔瓶子的的声音，各种环境球也是这个声音（改变背景的那个东西）</summary>
        public static SoundStyle ThrowBottle_Item106 => SoundID.Item106;

        /// <summary> 瓶子碎裂的的声音，各种环境球也是这个声音（改变背景的那个东西） </summary>
        public static SoundStyle BottleExplosion_Item107 => SoundID.Item107;

        /// <summary> 水晶蛇的声音 </summary>
        public static SoundStyle CrystalSerpent_Item109 => SoundID.Item109;

        /// <summary> 水晶蛇弹幕的声音，但是听上去像是烟花的声音</summary>
        public static SoundStyle Firework_Item110 => SoundID.Item110;

        /// <summary> 叫什么忘记了，总之是肉后腐化之地钓鱼获得的那个武器，射出毒泡泡的声音 </summary>
        public static SoundStyle ToxicBubble_Item111 => SoundID.Item111;

        /// <summary> 泡泡的声音，和上面的类似 </summary>
        public static SoundStyle NoUse_ToxicBubble2_Item112 => SoundID.Item112;

        /// <summary> 血荆棘的声音，致命球法杖召唤的声音 </summary>
        public static SoundStyle BloodThron_Item113 => SoundID.Item113;

        /// <summary> 传送枪的声音</summary>
        public static SoundStyle PortalGun_Item114 => SoundID.Item114;

        /// <summary> 传送枪的声音</summary>
        public static SoundStyle PortalGun2_Item115 => SoundID.Item115;

        /// <summary> 太阳能喷发的声音</summary>
        public static SoundStyle SolarEruption_Item116 => SoundID.Item116;

        /// <summary> 神灯烈焰，星云奥秘的声音</summary>
        public static SoundStyle SpiritFlame_Item117 => SoundID.Item117;

        /// <summary> 还是水晶蛇弹幕的声音（一把武器3个音效可还行） </summary>
        public static SoundStyle LiteBoom_Item118 => SoundID.Item118;

        /// <summary> 高尔夫挥杆的声音 </summary>
        public static SoundStyle Golf_Item126 => SoundID.Item126;

        /// <summary> 高尔夫球哨的声音</summary>
        public static SoundStyle GolfWhistle_Item128 => SoundID.Item128;

        /// <summary> 高尔夫进球得分的声音 </summary>
        public static SoundStyle GetScores_Item129 => SoundID.Item129;

        /// <summary> 虚空袋的声音 </summary>
        public static SoundStyle VoidBag_Item130 => SoundID.Item130;

        /// <summary> 原版未使用，听不出是什么，类似什么东西摩擦滑行的声音</summary>
        public static SoundStyle NoUse_Item131 => SoundID.Item131;

        /// <summary> 激光钻头的声音（这玩意是这个声音的？） </summary>
        public static SoundStyle LaserDrill_Item132 => SoundID.Item132;

        /// <summary> 弹幕被反弹时的声音，比如ftw克眼旋转时，还有微光史莱姆，大宝箱怪等 </summary>
        public static SoundStyle ProjectileReflected_Item150 => SoundID.Item150;

        /// <summary> 耍蛇者长笛的声音（召唤蛇蛇绳索的那个笛子）</summary>
        public static SoundStyle Snake_Item151 => SoundID.Item151;

        /// <summary> 鞭子甩出的声音 </summary>
        public static SoundStyle WhipSwing_Item152 => SoundID.Item152;

        /// <summary> 鞭子命中的声音，Pia!</summary>
        public static SoundStyle WhipHit_Item153 => SoundID.Item153;

        /// <summary> 喜庆弹射器的声音，放烟花 </summary>
        public static SoundStyle Firework2_Item156 => SoundID.Item156;

        /// <summary> 音乐盒记录的声音 </summary>
        public static SoundStyle MusicBoxRecorded_Item166 => SoundID.Item166;

        /// <summary> 蹦蹦跷召唤的声音 </summary>
        public static SoundStyle PogoStickMount_Item168 => SoundID.Item168;

        /// <summary> 天顶剑的声音，其实是复制的Item1，只不过调整了音高，听起来更沉重 </summary>
        public static SoundStyle Zenith_Item169 => SoundID.Item169;

        /// <summary> 1.4.4蜂后召唤物，幼虫的声音 </summary>
        public static SoundStyle BugsScream_Item173 => SoundID.Item173;

        /// <summary> 1.4.4KO大炮的声音</summary>
        public static SoundStyle KOCannon_Item174 => SoundID.Item174;

        /// <summary> 1.4.4拍拍手的声音（超高击退的那个玩具武器） </summary>
        public static SoundStyle SlapHand_Item175 => SoundID.Item175;

        /// <summary> 1.4.4便便的声音 </summary>
        public static SoundStyle Poo_Item177 => SoundID.Item177;

        /// <summary>1.4.4华夫铁的声音 </summary>
        public static SoundStyle WafflesIron_Item178 => SoundID.Item178;

        /// <summary> 钱币放置，堆叠，从NPC处买卖物品的音效 </summary>
        public static SoundStyle Coins => SoundID.Coins;

        /// <summary> 钱币捡起的声音，共5种</summary>
        public static SoundStyle CoinPickup => SoundID.CoinPickup;


        /// <summary> 露西斧说话，共5种</summary>
        public static SoundStyle LucyTheAxeTalk => SoundID.LucyTheAxeTalk;

        /// <summary> 切斯特，共2种</summary>
        public static SoundStyle ChesterOpen => SoundID.ChesterOpen;

        /// <summary> 切斯特，共2种</summary>
        public static SoundStyle ChesterClose => SoundID.ChesterClose;

        /// <summary> 阿比盖尔</summary>
        public static SoundStyle AbigailSummon => SoundID.AbigailSummon;

        /// <summary> 阿比盖尔</summary>
        public static SoundStyle AbigailAttack => SoundID.AbigailAttack;

        /// <summary> 阿比盖尔，共3种</summary>
        public static SoundStyle AbigailCry => SoundID.AbigailCry;

        /// <summary> 阿比盖尔，共3种</summary>
        public static SoundStyle AbigailUpgrade => SoundID.AbigailUpgrade;

        /// <summary> 咕噜咪，共2种</summary>
        public static SoundStyle GlommerBounce => SoundID.GlommerBounce;

        /// <summary> 四轴竞速无人机飞行</summary>
        public static SoundStyle JimsDrone => SoundID.JimsDrone;

        #endregion

        #region NPC相关音效

        #region NPC受击音效

        /// <summary> 大多数肉质生物的受击的声音，举例：史莱姆，僵尸，克眼 </summary>
        public static SoundStyle Fleshy_NPCHit1 => SoundID.NPCHit1;

        /// <summary> 大多数骨头生物的受击的声音，举例：骷髅 </summary>
        public static SoundStyle Bone_NPCHit2 => SoundID.NPCHit2;

        /// <summary> 火球命中的声音 </summary>
        public static SoundStyle FireBallHit_NPCHit3 => SoundID.NPCHit3;

        /// <summary> 大部分金属制敌人的受击声音，举例：圣骑士，附魔圣剑，双子魔眼2阶段 </summary>
        public static SoundStyle Metal_NPCHit4 => SoundID.NPCHit4;

        /// <summary> 小精灵，冰精灵，冰霜巨人的受击音效 </summary>
        public static SoundStyle Fairy_NPCHit5 => SoundID.NPCHit5;

        /// <summary>类似狼的叫声，各种狼，狼人还有一些“野兽”的受击音效 </summary>
        public static SoundStyle Wolf_NPCHit6 => SoundID.NPCHit6;

        /// <summary>也是类似肉体的受击音效，例如小白龙，冰霜女皇等</summary>
        public static SoundStyle Fleshy2_NPCHit7 => SoundID.NPCHit7;

        /// <summary> 血肉之墙 / 肉山的受击音效 </summary>
        public static SoundStyle WallOfFlesh_NPCHit8 => SoundID.NPCHit8;

        /// <summary> 类似血肉的受击音效，克苏鲁之脑，飞眼怪（Creeper?），肉山及肉山召唤的蠕虫的受击音效</summary>
        public static SoundStyle Bloody_NPCHit9 => SoundID.NPCHit9;

        /// <summary> 原版未使用，听上去像什么野兽的受击音效，和上面的Wolf挺像</summary>
        public static SoundStyle NoUse_Beast_NPCHit10 => SoundID.NPCHit10;

        /// <summary>雪人军团的怪，风滚草，日耀蜈蚣的受击音效（很难形容是个什么......） </summary>
        public static SoundStyle SnowMan_NPCHit11 => SoundID.NPCHit11;

        /// <summary> 无头骑士，独角兽的受击音效 </summary>
        public static SoundStyle Unicorn_NPCHit12 => SoundID.NPCHit12;

        /// <summary> 脓血乌贼，血腥水母，沙漠蝎子，血腥食人鱼的受击音效 </summary>
        public static SoundStyle Bloody2_NPCHit13 => SoundID.NPCHit13;

        /// <summary> 猪鲨的受击音效 </summary>
        public static SoundStyle DukeFishron_NPCHit14 => SoundID.NPCHit14;

        /// <summary> 傀儡 / 训练假人的受击音效 </summary>
        public static SoundStyle TargetDummy_NPCHit15 => SoundID.NPCHit15;

        /// <summary> 傀儡 / 训练假人的受击音效</summary>
        public static SoundStyle TargetDummy2_NPCHit16 => SoundID.NPCHit16;

        /// <summary> 傀儡 / 训练假人的受击音效</summary>
        public static SoundStyle TargetDummy3_NPCHit17 => SoundID.NPCHit17;

        /// <summary>鲜血僵尸，僵尸鱼人的受击音效（都是血月的怪）</summary>
        public static SoundStyle Bloddy3_NPCHit18 => SoundID.NPCHit18;

        /// <summary>滴滴怪的受击音效，还有荷兰人飞艇（这东西是这个音效的？感觉可能是因为平常打的都是它的炮）</summary>
        public static SoundStyle Drippler_NPCHit19 => SoundID.NPCHit19;

        /// <summary>血蜘蛛的受击音效 </summary>
        public static SoundStyle BloodCrawler_NPCHit20 => SoundID.NPCHit20;

        /// <summary>恶魔，红恶魔，巫毒恶魔的受击音效 </summary>
        public static SoundStyle Demon_NPCHit21 => SoundID.NPCHit21;

        /// <summary>跳跳兽的受击音效（肉后丛林的那个蓝色的虫子） </summary>
        public static SoundStyle Derpling_NPCHit22 => SoundID.NPCHit22;

        /// <summary> 飞蛇，沙尘精的受击音效</summary>
        public static SoundStyle FlyingSnake_NPCHit23 => SoundID.NPCHit23;

        /// <summary> 巨型陆龟 / 丛林王八 / 丛林核弹 / 萌新杀手......（编不下去了），冰雪陆龟的受击音效 </summary>
        public static SoundStyle GiantTortoise_NPCHit24 => SoundID.NPCHit24;

        /// <summary>3种颜色的水母的受击音效 </summary>
        public static SoundStyle Jellyfish_NPCHit25 => SoundID.NPCHit25;

        /// <summary> 丛林蜥蜴的受击音效</summary>
        public static SoundStyle Lihzahrd_NPCHit26 => SoundID.NPCHit26;

        /// <summary> 猪龙的受击音效 </summary>
        public static SoundStyle Pigron_NPCHit27 => SoundID.NPCHit27;

        /// <summary> 秃鹫/秃鹰的受击音效 </summary>
        public static SoundStyle Vulture_NPCHit28 => SoundID.NPCHit28;

        /// <summary> 黑隐士，爬藤怪的受击音效（蜘蛛洞的2种蜘蛛）</summary>
        public static SoundStyle Spider_NPCHit29 => SoundID.NPCHit29;

        /// <summary> 愤怒雨云的受击音效 </summary>
        public static SoundStyle AngryNimbus_NPCHit30 => SoundID.NPCHit30;

        /// <summary> 各种蚁狮的受击音效（除了蚁狮蜂）</summary>
        public static SoundStyle JuicyBug_NPCHit31 => SoundID.NPCHit31;

        /// <summary> 蚁狮蜂的受击音效</summary>
        public static SoundStyle FlyingBug_NPCHit32 => SoundID.NPCHit32;

        /// <summary> 龙虾的受击音效 </summary>
        public static SoundStyle Crawdad_NPCHit33 => SoundID.NPCHit33;

        /// <summary> 地牢幽魂/灵魂/鬼魂的受击音效/ </summary>
        public static SoundStyle DungeonSpirit_NPCHit36 => SoundID.NPCHit36;

        /// <summary> 各种食尸鬼的受击音效（肉后地底沙漠的那个） </summary>
        public static SoundStyle Ghouls_NPCHit37 => SoundID.NPCHit37;

        /// <summary> 巨型卷壳怪的受击音效 </summary>
        public static SoundStyle GiantShelly_NPCHit38 => SoundID.NPCHit38;

        /// <summary> 哥布林巫师/召唤师的受击音效（因为没有召唤的掉落物而惨遭改名的那个） </summary>
        public static SoundStyle GoblinWarlock_NPCHit40 => SoundID.NPCHit40;

        /// <summary>岩石巨人，花岗岩傀儡/巨人的受击音效</summary>
        public static SoundStyle RockGolem_NPCHit41 => SoundID.NPCHit41;

        /// <summary> 蘑菇瓢虫的受击音效 </summary>
        public static SoundStyle MushiLadybug_NPCHit45 => SoundID.NPCHit45;

        /// <summary> 鹦鹉的受击音效</summary>
        public static SoundStyle Parrot_NPCHit46 => SoundID.NPCHit46;

        /// <summary> 蝾螈 / 鸭鸭怪的受击音效</summary>
        public static SoundStyle Salamander_NPCHit50 => SoundID.NPCHit50;

        /// <summary> 暗影焰幻鬼的受击音效（哥布林巫师/召唤师召唤出来的）</summary>
        public static SoundStyle ShadowflameApparition_NPCHit52 => SoundID.NPCHit52;

        /// <summary> 幻灵/幽灵的受击音效（砸恶魔祭坛出一堆的那个）</summary>
        public static SoundStyle Wraith_NPCHit54 => SoundID.NPCHit54;

        #endregion
        #region NPC死亡

        /// <summary> 各种肉体生物的死亡音效，听上去像是肉被碾碎了的声音，举例：各种小动物，克眼，史莱姆 </summary>
        public static SoundStyle Fleshy_NPCDeath1 => SoundID.NPCDeath1;

        /// <summary>各种类人生物的死亡音效，举例：僵尸，骷髅</summary>
        public static SoundStyle HumanoidsDeath_NPCDeath2 => SoundID.NPCDeath2;

        /// <summary> 火球命中的声音</summary>
        public static SoundStyle FireBallDrath_NPCDeath3 => SoundID.NPCDeath3;

        /// <summary> 类似野兽一样的死亡声音，比如冰霜女皇，哀木，骨蛇等</summary>
        public static SoundStyle BeastDeath_NPCDeath5 => SoundID.NPCDeath5;

        /// <summary> 听上去像灵魂消散的声音的死亡声音，比如渔夫，鬼魂，荧光蝙蝠，荧光史莱姆，沙尘精等 </summary>
        public static SoundStyle SpiritDeath_NPCDeath6 => SoundID.NPCDeath6;

        /// <summary> 小精灵的死亡声音，比如小精灵，冰精灵，冰霜巨人等</summary>
        public static SoundStyle FairyDeath_NPCDeath7 => SoundID.NPCDeath7;

        /// <summary> 小白龙，不感恩的火鸡的死亡声音 </summary>
        public static SoundStyle Wyvern_NPCDeath8 => SoundID.NPCDeath8;

        /// <summary> 魔唾液的死亡声音（腐化者和世界吞噬者吐出的东西，这东西也有死亡音效啊...） </summary>
        public static SoundStyle VileSpit_NPCDeath9 => SoundID.NPCDeath9;

        /// <summary> 血肉之墙 / 肉山的死亡声音 </summary>
        public static SoundStyle WallOfFlesh_NPCDeath10 => SoundID.NPCDeath10;

        /// <summary>克苏鲁之脑，飞眼怪（Creeper?），肉山，蚁狮卵的死亡声音</summary>
        public static SoundStyle BloodyDeath_NPCDeath11 => SoundID.NPCDeath11;

        /// <summary> 饿鬼，肉山召唤的蠕虫的死亡声音</summary>
        public static SoundStyle BloodyDeath2_NPCDeath12 => SoundID.NPCDeath12;

        /// <summary> 肉山召唤的蠕虫时声音，听上去像呕吐的声音</summary>
        public static SoundStyle LeechSummoned_NPCDeath13 => SoundID.NPCDeath13;

        /// <summary> 和Item14基本一样，各种机器生物死亡音效 </summary>
        public static SoundStyle Boom_NPCDeath14 => SoundID.NPCDeath14;

        /// <summary> 雪人军团的怪死亡音效，其他还有愤怒雨云，永恒水晶（旧日军团召唤物）</summary>
        public static SoundStyle SnowManDeath_NPCDeath15 => SoundID.NPCDeath15;

        /// <summary>各种染料甲虫的死亡音效 </summary>
        public static SoundStyle Beetles_BugDeath_NPCDeath16 => SoundID.NPCDeath16;

        /// <summary>无头骑士，独角兽的死亡音效</summary>
        public static SoundStyle Unicorn_NPCDeath18 => SoundID.NPCDeath18;

        /// <summary>脓血乌贼，血腥水母，沙漠蝎子，血腥食人鱼，猪鲨泡泡的死亡音效</summary>
        public static SoundStyle BloodyDeath3_NPCDeath19 => SoundID.NPCDeath19;

        /// <summary> 猪鲨的死亡音效</summary>
        public static SoundStyle DukeFishron_NPCDeath20 => SoundID.NPCDeath20;

        /// <summary> 鲜血僵尸，僵尸鱼人的死亡音效 </summary>
        public static SoundStyle BloodyDeath4_NPCDeath21 => SoundID.NPCDeath21;

        /// <summary>滴滴怪，日耀蜈蚣，荷兰人飞艇的死亡音效 </summary>
        public static SoundStyle Drippler_NPCDeath22 => SoundID.NPCDeath22;

        /// <summary>血蜘蛛的死亡音效 </summary>
        public static SoundStyle BloodCrawler_NPCDeath23 => SoundID.NPCDeath23;

        /// <summary> 恶魔，红恶魔，巫毒恶魔的死亡音效 </summary>
        public static SoundStyle Demon_NPCDeath24 => SoundID.NPCDeath24;

        /// <summary> 跳跳兽的死亡音效（肉后丛林的那个蓝色的虫子） </summary>
        public static SoundStyle Derpling_NPCDeath25 => SoundID.NPCDeath25;

        /// <summary> 飞蛇的死亡音效 </summary>
        public static SoundStyle FlyingSnake_NPCDeath26 => SoundID.NPCDeath26;

        /// <summary> 巨型陆龟 / 丛林王八 / 丛林核弹 / 萌新杀手......（编不下去了），冰雪陆龟的死亡音效</summary>
        public static SoundStyle GiantTortoise_NPCDeath27 => SoundID.NPCDeath27;

        /// <summary> 3种颜色的水母的死亡音效 </summary>
        public static SoundStyle Jellyfish_NPCDeath28 => SoundID.NPCDeath28;

        /// <summary>丛林蜥蜴的死亡音效 </summary>
        public static SoundStyle Lihzahrd_NPCDeath29 => SoundID.NPCDeath29;

        /// <summary>猪龙的死亡音效 </summary>
        public static SoundStyle Pigron_NPCDeath30 => SoundID.NPCDeath30;

        /// <summary>秃鹫/秃鹰的死亡音效 </summary>
        public static SoundStyle Vulture_NPCDeath31 => SoundID.NPCDeath31;

        /// <summary> 黑隐士，爬藤怪的死亡音效（蜘蛛洞的2种蜘蛛） </summary>
        public static SoundStyle Spider_NPCDeath32 => SoundID.NPCDeath32;

        /// <summary> 愤怒雨云的死亡音效</summary>
        public static SoundStyle AngryNimbus_NPCDeath33 => SoundID.NPCDeath33;

        /// <summary>各种蚁狮的死亡音效（除了蚁狮蜂）</summary>
        public static SoundStyle JuicyBugDeath_NPCDeath34 => SoundID.NPCDeath34;

        /// <summary> 蚁狮蜂的死亡音效</summary>
        public static SoundStyle FlyingBugDeath_NPCDeath35 => SoundID.NPCDeath35;

        /// <summary> 龙虾的死亡音效</summary>
        public static SoundStyle Crawdad_NPCDeath36 => SoundID.NPCDeath36;

        /// <summary>地牢幽魂/灵魂/鬼魂，沙尘精的死亡音效</summary>
        public static SoundStyle DungeonSpirit_NPCDeath39 => SoundID.NPCDeath39;

        /// <summary> 各种食尸鬼的死亡音效（肉后地底沙漠的那个） </summary>
        public static SoundStyle Ghouls_NPCDeath40 => SoundID.NPCDeath40;

        /// <summary> 巨型卷壳怪的死亡音效 </summary>
        public static SoundStyle GiantShelly_NPCDeath41 => SoundID.NPCDeath41;

        /// <summary>哥布林巫师/召唤师的死亡音效（因为没有召唤的掉落物而惨遭改名的那个） </summary>
        public static SoundStyle GoblinWarlock_NPCDeath42 => SoundID.NPCDeath42;

        /// <summary> 岩石巨人，花岗岩傀儡/巨人的死亡音效 </summary>
        public static SoundStyle RockGolem_NPCDeath43 => SoundID.NPCDeath43;

        /// <summary>蘑菇瓢虫的死亡音效</summary>
        public static SoundStyle MushiLadybug_NPCDeath47 => SoundID.NPCDeath47;

        /// <summary> 鹦鹉的死亡音效 </summary>
        public static SoundStyle Parrot_NPCDeath48 => SoundID.NPCDeath48;

        /// <summary> 幻灵/幽灵的死亡音效（砸恶魔祭坛出一堆的那个）</summary>
        public static SoundStyle Wraith_NPCDeath52 => SoundID.NPCDeath52;

        /// <summary> 蝾螈 / 鸭鸭怪的死亡音效</summary>
        public static SoundStyle Salamander_NPCDeath53 => SoundID.NPCDeath53;

        /// <summary> 暗影焰幻鬼的死亡音效（哥布林巫师/召唤师召唤出来的） </summary>
        public static SoundStyle ShadowflameApparition_NPCDeath55 => SoundID.NPCDeath55;

        /// <summary>四柱护盾碎裂的音效 </summary>
        public static SoundStyle ShieldDestroyed_NPCDeath58 => SoundID.NPCDeath58;

        /// <summary>气球破了的声音 </summary>
        public static SoundStyle WindyBalloon_NPCDeath63 => SoundID.NPCDeath63;

        /// <summary> 1.4.4蜂后的死亡音效 </summary>
        public static SoundStyle QueenBee_NPCDeath66 => SoundID.NPCDeath66;

        #endregion
        #region NPC其他音效

        /// <summary> 僵尸音效，共3种</summary>
        public static SoundStyle Zombie => SoundID.ZombieMoan;

        /// <summary>僵尸音效2</summary>
        public static SoundStyle Zombie_Zombie1 => SoundID.Zombie1;

        /// <summary>僵尸音效3</summary>
        public static SoundStyle Zombie_Zombie2 => SoundID.Zombie2;


        /// <summary> 木乃伊的音效 </summary>
        public static SoundStyle Mummy => SoundID.Mummy;

        /// <summary>木乃伊音效1</summary>
        public static SoundStyle Mummy1_Zombie3 => SoundID.Zombie3;

        /// <summary>木乃伊音效2</summary>
        public static SoundStyle Mummy2_Zombie4 => SoundID.Zombie4;


        /// <summary>原版未使用，哼叫声，有回音所以听上去像是在洞穴里</summary>
        public static SoundStyle NoUse_CaveRoar_Zombie5 => SoundID.Zombie5;

        /// <summary>大脸怪/脸怪的音效</summary>
        public static SoundStyle FaceMonster_Zombie8 => SoundID.Zombie8;

        /// <summary>猪鲨的音效</summary>
        public static SoundStyle DukeFishron_Zombie20 => SoundID.Zombie20;


        /// <summary>血僵尸，僵尸鱼人的音效，共3种</summary>
        public static SoundStyle BloodZombie => SoundID.BloodZombie;

        /// <summary>血僵尸，僵尸鱼人的音效</summary>
        public static SoundStyle BloodZombie1_Zombie21 => SoundID.Zombie21;

        /// <summary>血僵尸，僵尸鱼人的音效</summary>
        public static SoundStyle BloodZombie2_Zombie22 => SoundID.Zombie22;

        /// <summary>血僵尸，僵尸鱼人的音效</summary>
        public static SoundStyle BloodZombie3_Zombie23 => SoundID.Zombie23;


        /// <summary>血蜘蛛的音效</summary>
        public static SoundStyle BloodCrawler1_Zombie24 => SoundID.Zombie24;

        /// <summary>血蜘蛛的音效</summary>
        public static SoundStyle BloodCrawler2_Zombie25 => SoundID.Zombie25;


        /// <summary>恶魔，红恶魔的音效</summary>
        public static SoundStyle Demon1_Zombie26 => SoundID.Zombie26;

        /// <summary>恶魔，红恶魔的音效</summary>
        public static SoundStyle Demon2_Zombie27 => SoundID.Zombie27;

        /// <summary>恶魔，红恶魔的音效</summary>
        public static SoundStyle Demon3_Zombie28 => SoundID.Zombie28;

        /// <summary>恶魔，红恶魔的音效</summary>
        public static SoundStyle Demon4_Zombie29 => SoundID.Zombie29;


        /// <summary>跳跳兽的音效（肉后丛林的那个蓝色的虫子）</summary>
        public static SoundStyle Derpling1_Zombie30 => SoundID.Zombie30;

        /// <summary>跳跳兽的音效（肉后丛林的那个蓝色的虫子）</summary>
        public static SoundStyle Derpling2_Zombie31 => SoundID.Zombie31;


        /// <summary>飞蛇的音效</summary>
        public static SoundStyle FlyingSnake_Zombie32 => SoundID.Zombie32;

        /// <summary>巨型陆龟 / 丛林王八 / 丛林核弹 / 萌新杀手......（编不下去了）的音效</summary>
        public static SoundStyle GiantTortoise_Zombie33 => SoundID.Zombie33;


        /// <summary>3种颜色的水母在液体中的音效</summary>
        public static SoundStyle Jellyfish1_Zombie34 => SoundID.Zombie34;

        /// <summary>3种颜色的水母在液体中的音效</summary>
        public static SoundStyle Jellyfish2_Zombie35 => SoundID.Zombie35;


        /// <summary>丛林蜥蜴的音效</summary>
        public static SoundStyle Lihzahrd1_Zombie36 => SoundID.Zombie36;

        /// <summary>丛林蜥蜴的音效</summary>
        public static SoundStyle Lihzahrd2_Zombie37 => SoundID.Zombie37;


        /// <summary>猪龙的音效</summary>
        public static SoundStyle Pigron1_Zombie9 => SoundID.Zombie9;

        /// <summary>猪龙的音效</summary>
        public static SoundStyle Pigron2_Zombie38 => SoundID.Zombie38;

        /// <summary>猪龙的音效</summary>
        public static SoundStyle Pigron3_Zombie39 => SoundID.Zombie39;

        /// <summary>猪龙的音效</summary>
        public static SoundStyle Pigron4_Zombie40 => SoundID.Zombie40;


        /// <summary> 愤怒雨云的音效 </summary>
        public static SoundStyle AngryNimbus1_Zombie41 => SoundID.Zombie41;

        /// <summary> 愤怒雨云的音效 </summary>
        public static SoundStyle AngryNimbus2_Zombie42 => SoundID.Zombie42;

        /// <summary> 愤怒雨云的音效 </summary>
        public static SoundStyle AngryNimbus3_Zombie43 => SoundID.Zombie43;


        /// <summary> 大部分蚁狮的音效 </summary>
        public static SoundStyle BugRore_Zombie44 => SoundID.Zombie44;

        /// <summary> 蚁狮蜂，巨型蚁狮蜂的音效 </summary>
        public static SoundStyle BugWingsShaking1_Zombie45 => SoundID.Zombie45;

        /// <summary> 蚁狮蜂，巨型蚁狮蜂的音效 </summary>
        public static SoundStyle BugWingsShaking2_Zombie46 => SoundID.Zombie46;


        /// <summary> 龙虾的音效 </summary>
        public static SoundStyle Crawdad_Zombie47 => SoundID.Zombie47;


        /// <summary> 地牢幽魂/灵魂/鬼魂的音效 </summary>
        public static SoundStyle DungeonSpirit1_Zombie53 => SoundID.Zombie53;

        /// <summary> 地牢幽魂/灵魂/鬼魂的音效 </summary>
        public static SoundStyle DungeonSpirit2_Zombie54 => SoundID.Zombie54;


        /// <summary> 各种食尸鬼的音效（肉后地底沙漠的那个）的音效 </summary>
        public static SoundStyle Ghoul1_Zombie55 => SoundID.Zombie55;

        /// <summary> 各种食尸鬼的音效（肉后地底沙漠的那个）的音效 </summary>
        public static SoundStyle Ghoul2_Zombie56 => SoundID.Zombie56;


        /// <summary> 巨型卷壳怪的音效 </summary>
        public static SoundStyle GiantShelly1_Zombie57 => SoundID.Zombie57;

        /// <summary> 巨型卷壳怪的音效 </summary>
        public static SoundStyle GiantShelly2_Zombie58 => SoundID.Zombie58;


        /// <summary>哥布林巫师/召唤师的音效 </summary>
        public static SoundStyle GoblinWarlock1_Zombie61 => SoundID.Zombie61;

        /// <summary>哥布林巫师/召唤师的音效 </summary>
        public static SoundStyle GoblinWarlock2_Zombie62 => SoundID.Zombie62;


        /// <summary>花岗岩巨人/傀儡的音效 </summary>
        public static SoundStyle GraniteGolem1_Zombie63 => SoundID.Zombie63;

        /// <summary>花岗岩巨人/傀儡的音效 </summary>
        public static SoundStyle GraniteGolem2_Zombie64 => SoundID.Zombie64;

        /// <summary>花岗岩巨人/傀儡的音效 </summary>
        public static SoundStyle GraniteGolem3_Zombie65 => SoundID.Zombie65;


        /// <summary>蘑菇瓢虫的音效 </summary>
        public static SoundStyle MushiLadybug1_Zombie74 => SoundID.Zombie74;

        /// <summary>蘑菇瓢虫的音效 </summary>
        public static SoundStyle MushiLadybug2_Zombie75 => SoundID.Zombie75;

        /// <summary>蘑菇瓢虫的音效 </summary>
        public static SoundStyle MushiLadybug3_Zombie76 => SoundID.Zombie76;

        /// <summary>蘑菇瓢虫的音效 </summary>
        public static SoundStyle MushiLadybug4_Zombie77 => SoundID.Zombie77;


        /// <summary> 海盗船长的鹦鹉的音效 </summary>
        public static SoundStyle Parrot_Zombie78 => SoundID.Zombie78;


        /// <summary>幻灵/幽灵，死神的音效 </summary>
        public static SoundStyle Wraith_Reaper1_Zombie81 => SoundID.Zombie81;

        /// <summary>幻灵/幽灵，死神的音效 </summary>
        public static SoundStyle Wraith_Reaper2_Zombie82 => SoundID.Zombie82;

        /// <summary>幻灵/幽灵，死神的音效 </summary>
        public static SoundStyle Wraith_Reaper3_Zombie83 => SoundID.Zombie83;


        /// <summary>蝾螈/鸭鸭怪的音效 </summary>
        public static SoundStyle Salamander1_Zombie84 => SoundID.Zombie84;

        /// <summary>蝾螈/鸭鸭怪的音效 </summary>
        public static SoundStyle Salamander2_Zombie85 => SoundID.Zombie85;


        /// <summary>原版未使用，奇怪的叫声 </summary>
        public static SoundStyle NoUse_StrangeRore_Zombie87 => SoundID.Zombie87;

        /// <summary>小丑的声音 </summary>
        //public static SoundStyle Clown => SoundID.Clown;

        /// <summary>沙鲨的音效</summary>
        public static SoundStyle SandShark => SoundID.SandShark;

        #endregion
        #region 小动物音效

        /// <summary>鸭鸭的音效，共3种</summary>
        public static SoundStyle Duck => SoundID.Duck;

        /// <summary>鸭鸭的音效</summary>
        public static SoundStyle Duck1_Zombie10 => SoundID.Zombie10;

        /// <summary>鸭鸭的音效</summary>
        public static SoundStyle Duck2_Zombie11 => SoundID.Zombie11;

        /// <summary>鸭鸭的音效</summary>
        public static SoundStyle Duck3_Zombie12 => SoundID.Zombie12;


        /// <summary>青蛙/呱呱/蛙蛙的音效</summary>
        public static SoundStyle Frog => SoundID.Frog;

        /// <summary>青蛙/呱呱/蛙蛙的音效</summary>
        public static SoundStyle Frog_Zombie13 => SoundID.Zombie13;


        /// <summary>鼠鼠/老鼠的音效（别打我兄弟TAT）</summary>
        public static SoundStyle Critter => SoundID.Critter;

        /// <summary>鼠鼠/老鼠的音效（别打我兄弟TAT）</summary>
        public static SoundStyle Mouse_Rat_Critter_Zombie15 => SoundID.Zombie15;

        /// <summary>各种啮齿类生物死亡声音，比如老鼠，蝙蝠等（别鲨我鼠鼠兄弟TAT） </summary>
        public static SoundStyle Mouse_Rat_Critter_Bat_NPCDeath4 => SoundID.NPCDeath4;


        /// <summary>鸟的音效，共5种</summary>
        public static SoundStyle Bird => SoundID.Bird;

        /// <summary>鸟的音效</summary>
        public static SoundStyle Bird1_Zombie14 => SoundID.Zombie14;

        /// <summary>鸟的音效</summary>
        public static SoundStyle Bird2_BlueJay_Zombie16 => SoundID.Zombie16;

        /// <summary>鸟的音效</summary>
        public static SoundStyle Bird3_Cardinal_Zombie17 => SoundID.Zombie17;

        /// <summary>鸟的音效</summary>
        public static SoundStyle Bird4_Zombie18 => SoundID.Zombie18;

        /// <summary>鸟的音效</summary>
        public static SoundStyle Bird5_Cardinal_Zombie19 => SoundID.Zombie19;


        /// <summary>海鸥的音效，共3种</summary>
        public static SoundStyle Seagull => SoundID.Seagull;

        /// <summary>海鸥的音效 </summary>
        public static SoundStyle Seagull1_Zombie106 => SoundID.Zombie106;

        /// <summary>海鸥的音效 </summary>
        public static SoundStyle Seagull2_Zombie107 => SoundID.Zombie107;

        /// <summary>海鸥的音效 </summary>
        public static SoundStyle Seagull3_Zombie108 => SoundID.Zombie108;


        /// <summary>海豚的音效 </summary>
        public static SoundStyle Dolphin => SoundID.Dolphin;

        /// <summary>海豚的音效 </summary>
        public static SoundStyle Dolphin_Zombie109 => SoundID.Zombie109;


        /// <summary>猫头鹰的音效，共5种 </summary>
        public static SoundStyle Owl => SoundID.Owl;

        /// <summary>猫头鹰的音效 </summary>
        public static SoundStyle Owl1_Zombie110 => SoundID.Zombie110;

        /// <summary>猫头鹰的音效 </summary>
        public static SoundStyle Owl2_Zombie111 => SoundID.Zombie111;

        /// <summary>猫头鹰的音效 </summary>
        public static SoundStyle Owl3_Zombie112 => SoundID.Zombie112;

        /// <summary>猫头鹰的音效 </summary>
        public static SoundStyle Owl4_Zombie113 => SoundID.Zombie113;

        /// <summary>猫头鹰的音效 </summary>
        public static SoundStyle Owl5_Zombie114 => SoundID.Zombie114;

        /// <summary>玄凤鹦鹉的声音 </summary>
        //public static SoundStyle Cockatiel => SoundID.Cockatiel;

        /// <summary>金刚鹦鹉的声音 </summary>
        //public static SoundStyle Macaw => SoundID.Macaw;

        /// <summary>巨嘴鸟的声音 </summary>
        //public static SoundStyle Toucan => SoundID.Toucan;

        #endregion

        #region BOSS-邪教徒

        /// <summary> 邪教徒召唤时的音效 </summary>
        public static SoundStyle LunaticCultistSummoned_Zombie105 => SoundID.Zombie105;

        /// <summary> 教徒放冰刺时的声音 </summary>
        public static SoundStyle IceMist_Item120 => SoundID.Item120;

        /// <summary> 邪教徒放闪电球时的声音</summary>
        public static SoundStyle LightningOrb_Item121 => SoundID.Item121;

        /// <summary>原版未使用，类似电击魔法的声音 </summary>
        public static SoundStyle NoUse_ElectricMagic_Item122 => SoundID.Item122;

        /// <summary> 教徒的某个动作的声音（不知道在哪用上了）</summary>
        public static SoundStyle LightningRitual_Item123 => SoundID.Item123;

        /// <summary> 直译：幻影闪电，也是不知道哪里用上了，听上去像Jiu ! Jiu ! 嚎呜~~ </summary>
        public static SoundStyle PhantasmalBolt_Item124 => SoundID.Item124;

        /// <summary> 直译：幻影闪电，也是不知道哪里用上了，听上去像Jiu ! Jiu ! Jiu ! </summary>
        public static SoundStyle PhantasmalBolt2_Item125 => SoundID.Item125;

        /// <summary> 邪教徒的受击音效 </summary>
        public static SoundStyle LunaticCultist_NPCHit55 => SoundID.NPCHit55;

        /// <summary> 邪教徒的音效 </summary>
        public static SoundStyle LunaticCultist1_Zombie88 => SoundID.Zombie88;

        /// <summary> 邪教徒的音效 </summary>
        public static SoundStyle LunaticCultist2_Zombie89 => SoundID.Zombie89;

        /// <summary> 邪教徒的音效 </summary>
        public static SoundStyle LunaticCultist3_Zombie90 => SoundID.Zombie90;

        /// <summary> 邪教徒的音效 </summary>
        public static SoundStyle LunaticCultist4_Zombie91 => SoundID.Zombie91;

        /// <summary> 邪教徒的死亡音效，同伴方块的音效（致敬传送门的那个宠物）</summary>
        public static SoundStyle LunaticCultist_NPCDeath59 => SoundID.NPCDeath59;

        /// <summary> 幻影龙/幻影弓龙召唤时的声音 </summary>
        public static SoundStyle DragonRoar_Item119 => SoundID.Item119;

        /// <summary> 幻影龙/幻影弓龙的受击音效 </summary>
        public static SoundStyle PhantasmDragon_NPCHit56 => SoundID.NPCHit56;

        /// <summary> 幻影龙/幻影弓龙的死亡音效 </summary>
        public static SoundStyle PhantasmDragon_NPCDeath60 => SoundID.NPCDeath60;

        #endregion
        #region BOSS - 月球领主/月总

        /// <summary> 月亮领主 / 月总，月蛭凝块（月总舌头）的受击音效</summary>
        public static SoundStyle MoonLord_NPCHit57 => SoundID.NPCHit57;

        /// <summary> 月球领主/月总召唤时的音效 </summary>
        public static SoundStyle MoonLordSummoned_Zombie92 => SoundID.Zombie92;

        /// <summary> 月球领主/月总的音效 </summary>
        public static SoundStyle MoonLord1_Zombie93 => SoundID.Zombie93;

        /// <summary> 月球领主/月总的音效 </summary>
        public static SoundStyle MoonLord2_Zombie94 => SoundID.Zombie94;

        /// <summary> 月球领主/月总的音效 </summary>
        public static SoundStyle MoonLord3_Zombie95 => SoundID.Zombie95;

        /// <summary> 月球领主/月总的音效 </summary>
        public static SoundStyle MoonLord4_Zombie96 => SoundID.Zombie96;

        /// <summary> 月球领主/月总的音效 </summary>
        public static SoundStyle MoonLord5_Zombie97 => SoundID.Zombie97;

        /// <summary> 月球领主/月总的音效 </summary>
        public static SoundStyle MoonLord6_Zombie98 => SoundID.Zombie98;

        /// <summary> 月球领主/月总的音效 </summary>
        public static SoundStyle MoonLord7_Zombie99 => SoundID.Zombie99;

        /// <summary >月球领主/月总真眼/克苏鲁真眼的音效 </summary>
        public static SoundStyle TrueEyeOfCthulhu1_Zombie100 => SoundID.Zombie100;

        /// <summary> 月球领主/月总真眼/克苏鲁真眼的音效 </summary>
        public static SoundStyle TrueEyeOfCthulhu2_Zombie101 => SoundID.Zombie101;

        /// <summary> 月球领主/月总真眼/克苏鲁真眼的幻影球音效 </summary>
        public static SoundStyle PhantasmalSphere_Zombie102 => SoundID.Zombie102;

        /// <summary> 月球领主/月总的幻影眼音效 </summary>
        public static SoundStyle PhantasmalEyeImpact_Zombie103 => SoundID.Zombie103;

        /// <summary> 月球领主/月总的幻影死亡射线音效 </summary>
        public static SoundStyle PhantasmalDeathray_Zombie104 => SoundID.Zombie104;

        /// <summary> 月亮领主 / 月总的死亡音效，同伴方块的音效（致敬传送门的那个宠物）</summary>
        public static SoundStyle MoonLord_NPCDeath61 => SoundID.NPCDeath61;

        /// <summary> 月亮领主 / 月总的手，头，月蛭凝块（月总舌头）的死亡音效</summary>
        public static SoundStyle MoonLord2_NPCDeath62 => SoundID.NPCDeath62;

        /// <summary> 不知道什么音效，但名字叫做月总</summary>
        public static SoundStyle MoonLord => SoundID.MoonLord;

        #endregion
        #region BOSS-史莱姆皇后

        /// <summary>史莱姆皇后的声音，类似果冻弹弹的声音</summary>
        public static SoundStyle QueenSlime_Item154 => SoundID.Item154;

        /// <summary> 史莱姆皇后的音效，共3种 </summary>
        public static SoundStyle QueenSlime => SoundID.QueenSlime;

        /// <summary>史莱姆皇后的音效 </summary>
        public static SoundStyle QueenSlime1_Zombie115 => SoundID.Zombie115;

        /// <summary>史莱姆皇后的音效 </summary>
        public static SoundStyle QueenSlime2_Zombie116 => SoundID.Zombie116;

        /// <summary>史莱姆皇后的音效 </summary>
        public static SoundStyle QueenSlime3_Zombie117 => SoundID.Zombie117;

        /// <summary>史莱姆皇后的声音，类似一堆泡泡的声音</summary>
        public static SoundStyle QueenSlime2_Bubble_Item155 => SoundID.Item155;

        /// <summary> 史莱姆皇后，没听错的话应该是落地的声音 </summary>
        public static SoundStyle QueneSlimeFalling_Item167 => SoundID.Item167;

        /// <summary> 史莱姆皇后的死亡音效 </summary>
        public static SoundStyle QueenSlime_NPCDeath64 => SoundID.NPCDeath64;

        #endregion
        #region BOSS-光之女皇
        /// <summary>光之女皇的太阳舞的声音 </summary>
        public static SoundStyle EmpressOfLight_SunDance_Item159 => SoundID.Item159;

        /// <summary>光之女皇的猛冲的声音 </summary>
        public static SoundStyle EmpressOfLight_Dash_Item160 => SoundID.Item160;

        /// <summary> 光之女皇召唤时的声音 </summary>
        public static SoundStyle EmpressOfLight_Summoned_Item161 => SoundID.Item161;

        /// <summary> 光之女皇的空灵长枪的声音 </summary>
        public static SoundStyle EmpressOfLight_EtherealLance_Item162 => SoundID.Item162;

        /// <summary> 光之女皇的永恒彩虹的声音 </summary>
        public static SoundStyle EmpressOfLight_EverlastingRainbow_Item163 => SoundID.Item163;

        /// <summary>光之女皇的七彩矢的声音 </summary>
        public static SoundStyle EmpressOfLight_PrismaticBolts_Item164 => SoundID.Item164;

        /// <summary>光之女皇的七彩矢2的声音（Wiki上就这么写的）</summary>
        public static SoundStyle EmpressOfLight_PrismaticBolts2_Item165 => SoundID.Item165;

        /// <summary> 光之女皇的死亡音效 </summary>
        public static SoundStyle EmpressOfLight_NPCDeath65 => SoundID.NPCDeath65;

        #endregion
        #region BOSS-恐惧鹦鹉螺

        /// <summary> 恐惧鹦鹉螺蓄力的声音 </summary>
        public static SoundStyle Dreadnautilus_ChargeUp_Item170 => SoundID.Item170;

        /// <summary> 恐惧鹦鹉螺发射血弹的声音</summary>
        public static SoundStyle Dreadnautilus_FireProjectiles_Item171 => SoundID.Item171;

        /// <summary> 恐惧鹦鹉螺冲刺的声音 </summary>
        public static SoundStyle Dreadnautilus_Dash_Item172 => SoundID.Item172;

        #endregion

        #region BOSS-黑暗魔法师

        /// <summary> 暗黑能量被召唤，共3种，听着像木棍挥舞声音 </summary>
        public static SoundStyle Swing_DD2_DarkMageAttack => SoundID.DD2_DarkMageAttack;

        /// <summary> 黑暗魔法师，共3种 </summary>
        public static SoundStyle DD2_DarkMageCastHeal => SoundID.DD2_DarkMageCastHeal;

        /// <summary> 黑暗魔法师死亡音效，共3种 </summary>
        public static SoundStyle DD2_DarkMageDeath => SoundID.DD2_DarkMageDeath;

        /// <summary> 暗黑魔符撞击，共3种，听着像是什么光属性魔法 </summary>
        public static SoundStyle LightMagic_DD2_DarkMageHealImpact => SoundID.DD2_DarkMageHealImpact;

        /// <summary> 黑暗魔法师受击，共3种 </summary>
        public static SoundStyle DD2_DarkMageHurt => SoundID.DD2_DarkMageHurt;

        /// <summary> 撒旦骷髅被召唤，共3种 </summary>
        public static SoundStyle DD2_DarkMageSummonSkeleton => SoundID.DD2_DarkMageSummonSkeleton;

        #endregion
        #region 食人魔

        /// <summary> 食人魔，共3种 </summary>
        public static SoundStyle DD2_OgreAttack => SoundID.DD2_OgreAttack;

        /// <summary> 食人魔，共3种 </summary>
        public static SoundStyle DD2_OgreDeath => SoundID.DD2_OgreDeath;

        /// <summary> 食人魔砸地板 </summary>
        public static SoundStyle DD2_OgreGroundPound => SoundID.DD2_OgreGroundPound;

        /// <summary> 食人魔唾液 </summary>
        public static SoundStyle DD2_OgreSpit => SoundID.DD2_OgreSpit;

        /// <summary> 食人魔，共3种 </summary>
        public static SoundStyle DD2_OgreHurt => SoundID.DD2_OgreHurt;

        /// <summary> 食人魔，共3种 </summary>
        public static SoundStyle DD2_OgreRoar => SoundID.DD2_OgreRoar;

        #endregion
        #region BOSS-双足翼龙

        /// <summary> 双足翼龙死亡音效，共3种 </summary>
        public static SoundStyle DD2_BetsyDeath => SoundID.DD2_BetsyDeath;

        /// <summary> 双足翼龙发射火球，共3种 </summary>
        public static SoundStyle FireShoot_DD2_BetsyFireballShot => SoundID.DD2_BetsyFireballShot;

        /// <summary> 双足翼龙火球爆炸，共3种 </summary>
        public static SoundStyle FireBallExplosion_DD2_BetsyFireballImpact => SoundID.DD2_BetsyFireballImpact;

        /// <summary> 双足翼龙火焰吐息攻击</summary>
        public static SoundStyle FireShoot_DD2_BetsyFlameBreath => SoundID.DD2_BetsyFlameBreath;

        /// <summary> 双足翼龙环形飞行攻击</summary>
        public static SoundStyle DD2_BetsyFlyingCircleAttack => SoundID.DD2_BetsyFlyingCircleAttack;

        /// <summary> 双足翼龙受击，共3种</summary>
        public static SoundStyle DD2_BetsyHurt => SoundID.DD2_BetsyHurt;

        /// <summary> 双足翼龙尖叫，哇奥奥奥奥奥！！！！！┗|｀O′|┛ 嗷~~</summary>
        public static SoundStyle Roar_DD2_BetsyScream => SoundID.DD2_BetsyScream;

        /// <summary> 双足翼龙被召唤，共3种</summary>
        public static SoundStyle DD2_BetsySummon => SoundID.DD2_BetsySummon;

        /// <summary> 双足翼龙狂风攻击，共3种</summary>
        public static SoundStyle DD2_BetsyWindAttack => SoundID.DD2_BetsyWindAttack;

        #endregion

        #region BOSS-独眼巨鹿

        /// <summary> 独眼巨鹿，共3种 </summary>
        public static SoundStyle DeerclopsHit => SoundID.DeerclopsHit;

        /// <summary> 独眼巨鹿 </summary>
        public static SoundStyle DeerclopsDeath => SoundID.DeerclopsDeath;

        /// <summary> 独眼巨鹿，共3种 </summary>
        public static SoundStyle DeerclopsScream => SoundID.DeerclopsScream;

        /// <summary> 独眼巨鹿，共3种 </summary>
        public static SoundStyle IceSpike_DeerclopsIceAttack => SoundID.DeerclopsIceAttack;

        /// <summary> 独眼巨鹿掀地板 </summary>
        public static SoundStyle DeerclopsRubbleAttack => SoundID.DeerclopsRubbleAttack;

        /// <summary> 独眼巨鹿走路 </summary>
        public static SoundStyle DeerclopsStep => SoundID.DeerclopsStep;


        #endregion

        #region 日食NPC

        /// <summary>科学怪人/弗兰肯斯坦的音效</summary>
        public static SoundStyle Frankenstein_Zombie6 => SoundID.Zombie6;

        /// <summary>吸血鬼，沙鲨的音效</summary>
        public static SoundStyle Vampire_SandShark_Zombie7 => SoundID.Zombie7;

        /// <summary> 屠夫（日食的），美杜莎的死亡音效</summary>
        public static SoundStyle Medusa_NPCDeath17 => SoundID.NPCDeath17;

        /// <summary> 致命球的受击音效 </summary>
        public static SoundStyle DeadlySphere_NPCHit34 => SoundID.NPCHit34;

        /// <summary> 致命球的音效 </summary>
        public static SoundStyle DeadlySphere1_Zombie48 => SoundID.Zombie48;

        /// <summary> 致命球的音效 </summary>
        public static SoundStyle DeadlySphere2_Zombie49 => SoundID.Zombie49;

        /// <summary> 致命球的死亡音效 </summary>
        public static SoundStyle DeadlySphere_NPCDeath37 => SoundID.NPCDeath37;

        /// <summary>苍蝇人博士的受击音效</summary>
        public static SoundStyle DrManFly_NPCHit35 => SoundID.NPCHit35;

        /// <summary> 苍蝇人博士的音效 </summary>
        public static SoundStyle DrManFly1_Zombie50 => SoundID.Zombie50;

        /// <summary> 苍蝇人博士的音效 </summary>
        public static SoundStyle DrManFly2_Zombie51 => SoundID.Zombie51;

        /// <summary> 苍蝇人博士的音效 </summary>
        public static SoundStyle DrManFly3_Zombie52 => SoundID.Zombie52;

        /// <summary> 苍蝇人博士的死亡音效 </summary>
        public static SoundStyle DrManFly_NPCDeath38 => SoundID.NPCDeath38;

        /// <summary>蛾怪 / 魔斯拉的受击音效</summary>
        public static SoundStyle Mothron_NPCHit44 => SoundID.NPCHit44;

        /// <summary>蛾怪 / 魔斯拉的音效 </summary>
        public static SoundStyle Mothron_Zombie73 => SoundID.Zombie73;

        /// <summary>蛾怪 / 魔斯拉的死亡音效</summary>
        public static SoundStyle Mothron_NPCDeath46 => SoundID.NPCDeath46;

        /// <summary> 攀爬魔的受击音效</summary>
        public static SoundStyle ThePossessed_NPCHit47 => SoundID.NPCHit47;

        /// <summary>攀爬魔的音效 </summary>
        public static SoundStyle ThePossessed1_Zombie79 => SoundID.Zombie79;

        /// <summary>攀爬魔的音效 </summary>
        public static SoundStyle ThePossessed2_Zombie80 => SoundID.Zombie80;

        /// <summary>攀爬魔的死亡音效 </summary>
        public static SoundStyle ThePossessed_NPCDeath49 => SoundID.NPCDeath49;

        /// <summary> 变态人的死亡音效</summary>
        public static SoundStyle Psycho_NPCDeath50 => SoundID.NPCDeath50;

        /// <summary> 变态人的受击音效</summary>
        public static SoundStyle Psycho_NPCHit48 => SoundID.NPCHit48;

        /// <summary> 死神的受击音效 </summary>
        public static SoundStyle Reaper_NPCHit49 => SoundID.NPCHit49;

        /// <summary> 死神的死亡音效</summary>
        public static SoundStyle Reaper_NPCDeath51 => SoundID.NPCDeath51;

        #endregion
        #region 火星入侵NPC

        /// <summary> 电击怪的音效（某个火星敌怪） </summary>
        public static SoundStyle Gigazapper1_Zombie59 => SoundID.Zombie59;

        /// <summary> 电击怪的音效（某个火星敌怪） </summary>
        public static SoundStyle Gigazapper2_Zombie60 => SoundID.Zombie60;

        /// <summary>火星走妖的音效 </summary>
        public static SoundStyle MartianWalker1_Zombie69 => SoundID.Zombie69;

        /// <summary>火星走妖的音效 </summary>
        public static SoundStyle MartianWalker2_Zombie70 => SoundID.Zombie70;

        /// <summary>火星走妖的音效 </summary>
        public static SoundStyle MartianWalker3_Zombie71 => SoundID.Zombie71;

        /// <summary>火星走妖的音效 </summary>
        public static SoundStyle MartianWalker4_Zombie72 => SoundID.Zombie72;

        /// <summary>火星自爆飞船的音效 </summary>
        public static SoundStyle MartianDrone1_Zombie66 => SoundID.Zombie66;

        /// <summary>火星自爆飞船的音效 </summary>
        public static SoundStyle MartianDrone2_Zombie67 => SoundID.Zombie67;

        /// <summary>火星自爆飞船的音效 </summary>
        public static SoundStyle MartianDrone3_Zombie68 => SoundID.Zombie68;

        /// <summary> 火星自爆飞船的受击音效</summary>
        public static SoundStyle MartianDrone_HitMetal_NPCHit42 => SoundID.NPCHit42;

        /// <summary> 火星自爆飞船的死亡音效，听上去像护盾碎了的声音 </summary>
        public static SoundStyle MartianDrone_ShieldBroken_NPCDeath44 => SoundID.NPCDeath44;

        /// <summary>火星人的泡泡盾的受击音效 </summary>
        public static SoundStyle BubbleShield_Electric_NPCHit43 => SoundID.NPCHit43;

        /// <summary> 火星人的泡泡盾的死亡音效</summary>
        public static SoundStyle BubbleShield_NPCDeath45 => SoundID.NPCDeath45;

        /// <summary> 鳞甲怪 / 火星蛞蝓的受击音效</summary>
        public static SoundStyle Scutlix_NPCHit51 => SoundID.NPCHit51;

        /// <summary>鳞甲怪 / 火星蛞蝓的音效 </summary>
        public static SoundStyle Scutlix_Zombie86 => SoundID.Zombie86;

        /// <summary> 鳞甲怪 / 火星蛞蝓的死亡音效</summary>
        public static SoundStyle Scutlix_NPCDeath54 => SoundID.NPCDeath54;

        /// <summary> 特斯拉炮塔的受击音效</summary>
        public static SoundStyle TeslaTurret_Electric_NPCHit53 => SoundID.NPCHit53;

        /// <summary> 特斯拉炮塔的死亡音效</summary>
        public static SoundStyle TeslaTurret_NPCDeath56 => SoundID.NPCDeath56;

        /// <summary> 各种火星人的受击音效 </summary>
        public static SoundStyle Martian_NPCHit39 => SoundID.NPCHit39;

        /// <summary>各种火星人的死亡音效</summary>
        public static SoundStyle Martian_NPCDeath57 => SoundID.NPCDeath57;

        #endregion
        #region 旧日军团NPC

        /// <summary> 德拉克龙，共3种 </summary>
        public static SoundStyle DD2_DrakinBreathIn => SoundID.DD2_DrakinBreathIn;

        /// <summary> 德拉克龙，共3种 </summary>
        public static SoundStyle DD2_DrakinDeath => SoundID.DD2_DrakinDeath;

        /// <summary> 德拉克龙，共3种 </summary>
        public static SoundStyle DD2_DrakinHurt => SoundID.DD2_DrakinHurt;

        /// <summary> 德拉克龙，共3种，爆炸声 </summary>
        public static SoundStyle Explosion_DD2_DrakinShot => SoundID.DD2_DrakinShot;

        /// <summary> 埃特尼亚哥布林，共3种 </summary>
        public static SoundStyle DD2_GoblinDeath => SoundID.DD2_GoblinDeath;

        /// <summary> 埃特尼亚哥布林，共6种 </summary>
        public static SoundStyle DD2_GoblinHurt => SoundID.DD2_GoblinHurt;

        /// <summary> 埃特尼亚哥布林，共3种 </summary>
        public static SoundStyle DD2_GoblinScream => SoundID.DD2_GoblinScream;

        /// <summary> 埃特尼亚哥布林投弹手，共3种 </summary>
        public static SoundStyle DD2_GoblinBomberDeath => SoundID.DD2_GoblinBomberDeath;

        /// <summary> 埃特尼亚哥布林投弹手，共3种 </summary>
        public static SoundStyle DD2_GoblinBomberHurt => SoundID.DD2_GoblinBomberHurt;

        /// <summary> 埃特尼亚哥布林投弹手，共3种 </summary>
        public static SoundStyle DD2_GoblinBomberScream => SoundID.DD2_GoblinBomberScream;

        /// <summary> 埃特尼亚哥布林投弹手，共3种 </summary>
        public static SoundStyle DD2_GoblinBomberThrow => SoundID.DD2_GoblinBomberThrow;

        /// <summary> 埃特尼亚标枪投掷怪，共3种 </summary>
        public static SoundStyle Shoot_DD2_JavelinThrowersAttack => SoundID.DD2_JavelinThrowersAttack;

        /// <summary> 埃特尼亚标枪投掷怪，共3种 </summary>
        public static SoundStyle DD2_JavelinThrowersDeath => SoundID.DD2_JavelinThrowersDeath;

        /// <summary> 埃特尼亚标枪投掷怪，共3种 </summary>
        public static SoundStyle DD2_JavelinThrowersHurt => SoundID.DD2_JavelinThrowersHurt;

        /// <summary> 埃特尼亚标枪投掷怪，共3种 </summary>
        public static SoundStyle DD2_JavelinThrowersTaunt => SoundID.DD2_JavelinThrowersTaunt;

        /// <summary> 小妖魔，共3种 </summary>
        public static SoundStyle DD2_KoboldDeath => SoundID.DD2_KoboldDeath;

        /// <summary> 小妖魔，共3种，爆炸声 </summary>
        public static SoundStyle Explosion_DD2_KoboldExplosion => SoundID.DD2_KoboldExplosion;

        /// <summary> 小妖魔，共3种 </summary>
        public static SoundStyle DD2_KoboldHurt => SoundID.DD2_KoboldHurt;

        /// <summary> 小妖魔，用打火石点火的声音 </summary>
        public static SoundStyle Fire_DD2_KoboldIgnite => SoundID.DD2_KoboldIgnite;

        /// <summary> 小妖魔，炸弹引线燃烧的声音 </summary>
        public static SoundStyle DD2_KoboldIgniteLoop => SoundID.DD2_KoboldIgniteLoop;

        /// <summary> 小妖魔 </summary>
        public static SoundStyle DD2_KoboldScreamChargeLoop => SoundID.DD2_KoboldScreamChargeLoop;

        /// <summary> 小妖魔，共3种 </summary>
        public static SoundStyle DD2_KoboldFlyerChargeScream => SoundID.DD2_KoboldFlyerChargeScream;

        /// <summary> 小妖魔滑翔怪，共3种 </summary>
        public static SoundStyle DD2_KoboldFlyerDeath => SoundID.DD2_KoboldFlyerDeath;

        /// <summary> 小妖魔滑翔怪，共3种 </summary>
        public static SoundStyle DD2_KoboldFlyerHurt => SoundID.DD2_KoboldFlyerHurt;

        /// <summary> 埃特尼亚荧光虫，共3种 </summary>
        public static SoundStyle DD2_LightningBugDeath => SoundID.DD2_LightningBugDeath;

        /// <summary> 埃特尼亚荧光虫，共3种 </summary>
        public static SoundStyle DD2_LightningBugHurt => SoundID.DD2_LightningBugHurt;

        /// <summary> 埃特尼亚荧光虫，共3种 </summary>
        public static SoundStyle DD2_LightningBugZap => SoundID.DD2_LightningBugZap;

        /// <summary> 撒旦骷髅，共3种 </summary>
        public static SoundStyle DD2_SkeletonDeath => SoundID.DD2_SkeletonDeath;

        /// <summary> 撒旦骷髅，共3种 </summary>
        public static SoundStyle DD2_SkeletonHurt => SoundID.DD2_SkeletonHurt;

        /// <summary> 撒旦骷髅被召唤 </summary>
        public static SoundStyle DD2_SkeletonSummoned => SoundID.DD2_SkeletonSummoned;

        /// <summary> 枯萎兽，共2种 </summary>
        public static SoundStyle DD2_WitherBeastAuraPulse => SoundID.DD2_WitherBeastAuraPulse;

        /// <summary> 枯萎兽，共3种，水晶受击声 </summary>
        public static SoundStyle CrystalHit_DD2_WitherBeastCrystalImpact => SoundID.DD2_WitherBeastCrystalImpact;

        /// <summary> 枯萎兽，共3种，水晶碎裂声 </summary>
        public static SoundStyle CrystalBroken_DD2_WitherBeastDeath => SoundID.DD2_WitherBeastDeath;

        /// <summary> 枯萎兽，共3种，水晶受击声 </summary>
        public static SoundStyle CrystalHit_DD2_WitherBeastHurt => SoundID.DD2_WitherBeastHurt;

        /// <summary> 埃特尼亚飞龙，共3种 </summary>
        public static SoundStyle DD2_WyvernDeath => SoundID.DD2_WyvernDeath;

        /// <summary> 埃特尼亚飞龙，共3种 </summary>
        public static SoundStyle DD2_WyvernHurt => SoundID.DD2_WyvernHurt;

        /// <summary> 埃特尼亚飞龙，共3种 </summary>
        public static SoundStyle DD2_WyvernScream => SoundID.DD2_WyvernScream;

        /// <summary> 埃特尼亚飞龙，共3种 </summary>
        public static SoundStyle DD2_WyvernDiveDown => SoundID.DD2_WyvernDiveDown;

        /// <summary> 接触神秘传送门？ </summary>
        public static SoundStyle MagicShoot_DD2_EtherianPortalDryadTouch => SoundID.DD2_EtherianPortalDryadTouch;

        /// <summary> 神秘传送门 </summary>
        public static SoundStyle DD2_EtherianPortalIdleLoop => SoundID.DD2_EtherianPortalIdleLoop;

        /// <summary> 神秘传送门开启 </summary>
        public static SoundStyle DD2_EtherianPortalOpen => SoundID.DD2_EtherianPortalOpen;

        /// <summary> 神秘传送门刷怪，共3种 </summary>
        public static SoundStyle DD2_EtherianPortalSpawnEnemy => SoundID.DD2_EtherianPortalSpawnEnemy;

        /// <summary> 永恒水晶，共3种，水晶受击声 </summary>
        public static SoundStyle CrystalHit_DD2_CrystalCartImpact => SoundID.DD2_CrystalCartImpact;


        /// <summary> 撒旦军队事件失败 </summary>
        public static SoundStyle DD2_DefeatScene => SoundID.DD2_DefeatScene;

        /// <summary> 撒旦军队事件胜利 </summary>
        public static SoundStyle DD2_WinScene => SoundID.DD2_WinScene;



        #endregion

        //----------其他-----------


        /// <summary> 敌怪咆哮的音效，大部分召唤BOSS的物品的音效</summary>
        public static SoundStyle Roar => SoundID.Roar;

        /// <summary> 蠕虫挖地的声音</summary>
        public static SoundStyle WormDig => SoundID.WormDig;

        /// <summary> 原版未使用，是尖叫声，带点机械的感觉</summary>
        public static SoundStyle NoUse_ScaryScream => SoundID.ScaryScream;

        /// <summary> 小精灵自身的音效</summary>
        public static SoundStyle Pixie => SoundID.Pixie;

        /// <summary> 哀木等的扔火球的声音 </summary>
        public static SoundStyle FireThrow_Item42 => SoundID.Item42;

        /// <summary> 专家模式克苏鲁之眼/克眼的叫声 </summary>
        public static SoundStyle ForceRoar => SoundID.ForceRoar;

        /// <summary> 专家模式克苏鲁之眼/克眼的叫声，调整音高之后的版本，疯狗冲刺时的叫声 </summary>
        public static SoundStyle ForceRoarPitched => SoundID.ForceRoarPitched;

        #endregion

        #region 物块音效

        /// <summary>植物物块/墙壁破坏的声音，部分弹幕的声音，如吹叶机 </summary>
        public static SoundStyle Grass => SoundID.Grass;

        /// <summary> 挖掘时的声音，共3种 </summary>
        public static SoundStyle Dig => SoundID.Dig;

        /// <summary> 开门的声音</summary>
        public static SoundStyle DoorOpen => SoundID.DoorOpen;

        /// <summary>关门的声音</summary>
        public static SoundStyle DoorClosed => SoundID.DoorClosed;

        /// <summary>
        /// 听上去像玻璃破碎的声音，一些墙壁的声音，例如玻璃墙，落雪墙等<br></br>
        /// 玻璃瓶，猪猪存钱罐
        /// </summary>
        public static SoundStyle GlassBroken_Shatter => SoundID.Shatter;

        /// <summary>大部分电路开关的音效</summary>
        public static SoundStyle Trigger_Mech => SoundID.Mech;

        /// <summary>石制物块和墙壁的挖掘音效 </summary>
        public static SoundStyle DigStone_Tink => SoundID.Tink;

        /// <summary>挖冰雪块的声音，从3个中随机一个 </summary>
        public static SoundStyle DigIce
        {
            get => Main.rand.Next(3) switch
            {
                0 => SoundID.Item48,
                1 => SoundID.Item49,
                _ => SoundID.Item50,
            };
        }

        /// <summary>挖冰雪块的声音 </summary>
        public static SoundStyle DigIce1_Item48 => SoundID.Item48;

        /// <summary> 挖冰雪块的声音</summary>
        public static SoundStyle DigIce2_Item49 => SoundID.Item49;

        /// <summary>挖冰雪块的声音 </summary>
        public static SoundStyle DigIce3_Item50 => SoundID.Item50;

        /// <summary>破碎地牢砖的声音 </summary>
        public static SoundStyle CrackedDungeonBricks_Item127 => SoundID.Item127;

        /// <summary> 子弹盒的声音（放地上的物块，可以右键加BUFF的那个） </summary>
        public static SoundStyle AmmoBox_Item149 => SoundID.Item149;

        /// <summary> 箱子解锁时的音效 </summary>
        public static SoundStyle Chest_Unlock => SoundID.Unlock;

        /// <summary> 以太块/镒块产生时的音效 </summary>
        public static SoundStyle SpawnAetheriumBlock => SoundID.ShimmerWeak1;
        
        /// <summary> 蜂蜜块/蜜蜂块产生时的音效，共3种 </summary>
        public static SoundStyle SpawnHoneyBlock => SoundID.LiquidsHoneyWater;

        /// <summary> 松脆蜂蜜块产生时的音效，共3种 </summary>
        public static SoundStyle SpawnCrispyHoneyBlock => SoundID.LiquidsHoneyLava;

        /// <summary> 黑曜石产生时的音效，共3种 </summary>
        public static SoundStyle SpawnObsidian => SoundID.LiquidsWaterLava;

        #endregion

        #region 液体音效

        /// <summary> 接触水的音效 </summary>
        public static SoundStyle Water_Splash => SoundID.Splash;

        /// <summary> 向下流动的水的声音 </summary>
        public static SoundStyle Waterfall => SoundID.Waterfall;

        /// <summary> 向下流动的岩浆的声音 </summary>
        public static SoundStyle Lavafall => SoundID.Lavafall;

        /// <summary> 接触岩浆，蜂蜜的音效，用桶装起液体的音效 </summary>
        public static SoundStyle Lava_Honey_SplashWeak => SoundID.SplashWeak;

        /// <summary> 滴落的液体的声音，共3种 </summary>
        public static SoundStyle Drip => SoundID.Drip;

        /// <summary> 接触微光或离开微光的音效 </summary>
        public static SoundStyle ShimmerContract => SoundID.Shimmer1;

        /// <summary>1.4.4微光的声音 </summary>
        public static SoundStyle Shlimmer_Item176 => SoundID.Item176;

        /// <summary> 不知道在哪用了，反正也是液体流动的音效 </summary>
        public static SoundStyle IDontKnow_ShimmerWeak2 => SoundID.ShimmerWeak2;

        #endregion

        #region UI音效

        /// <summary> 打开UI的声音，例如打开背包</summary>
        public static SoundStyle MenuOpen => SoundID.MenuOpen;

        /// <summary> 关闭UI的声音，例如关闭背包</summary>
        public static SoundStyle MenuClose => SoundID.MenuClose;

        /// <summary> 鼠标悬浮在UI上时发出的声音</summary>
        public static SoundStyle MenuTick => SoundID.MenuTick;

        /// <summary> 照相模式照相的声音</summary>
        public static SoundStyle Camera => SoundID.Camera;

        /// <summary> 搜索物品的声音，共3种</summary>
        public static SoundStyle Research => SoundID.Research;

        /// <summary> 搜索物品完成时的声音</summary>
        public static SoundStyle ResearchComplete => SoundID.ResearchComplete;

        /// <summary> 成就解锁时的声音</summary>
        public static SoundStyle AchievementComplete => SoundID.AchievementComplete;

        #endregion

        #region 天气音效

        /// <summary> 打雷/闪电天气的声音，共7种 </summary>
        public static SoundStyle Thunder => SoundID.Thunder;

        /// <summary> 在建筑内（背景墙前）的暴雪音效 </summary>
        public static SoundStyle BlizzardInsideBuildingLoop => SoundID.BlizzardInsideBuildingLoop;
        
        /// <summary> 暴雪音效 </summary>
        public static SoundStyle BlizzardStrongLoop => SoundID.BlizzardStrongLoop;

        #endregion
    }
}
