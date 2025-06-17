using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyItem : ModItem//, IFairyItem
    {
        public override string Texture => AssetDirectory.FairyItems + Name;

        /// <summary> 仙灵的个体数据，用于存放各类增幅 </summary>
        public FairyIV FairyIV { get; set; }

        /// <summary> 仙灵是否存活 </summary>
        protected bool dead;
        /// <summary>
        /// 复活时间
        /// </summary>
        protected int resurrectionTime;

        private static int showLineValueCount;

        public abstract int FairyType { get; }
        public abstract FairyRarity Rarity { get; }
        public bool IsDead => dead;
        public int Life { get; private set; }
        /// <summary>
        /// 复活时间，默认3分钟（3*60*60）
        /// </summary>
        public virtual int MaxResurrectionTime => 60 * 60 * 3;


        ///用于记录仙灵的弹幕的唯一ID，如果没有就在能够再次射出仙灵
        private int _fairyProjUUID = -1;

        public bool IsOut { get; set; }


        private static int[] indexes = [0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6, 0, 6, 7];
        private const string FairyLifeMax = "FairyLifeMax";
        private const string FairyDamage = "FairyDamage";
        private const string FairyDefence = "FairyDefence";
        private const string FairySpeed = "FairySpeed";
        private const string FairySkillLevel = "FairySkillLevel";
        private const string FairyStamina = "FairyStamina";
        private const string FairyScale = "FairyScale";


        public override void SetStaticDefaults()
        {
            CoraliteSets.Items.IsFairy[Type] = true;
        }

        public sealed override void SetDefaults()
        {
            IsOut = false;
            Item.DamageType = Content.DamageClasses.FairyDamage.Instance;

            SetOtherDefaults();
        }

        /// <summary>
        /// 初始化仙灵的各项数值
        /// </summary>
        /// <param name="fairyIV"></param>
        public void Initialize(FairyIV fairyIV)
        {
            FairyIV = fairyIV;
            Life = fairyIV.LifeMax;
        }

        public virtual void SetOtherDefaults() { }

        //public override ModItem Clone(Item newEntity)
        //{
        //    ModItem modItem = base.Clone(newEntity);
        //    if (modItem != null)
        //        (modItem as IFairyItem).IV = IV;

        //    return modItem;
        //}

        public virtual bool Hurt(Player owner, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int damage = target.damage;

            //damage -= (int)FairyDefence;

            if (damage < 1)
                damage = 1;

            Life -= damage;
            if (Life <= 0)
                Dead(owner, target);
            LimitLife();

            return dead;
        }

        public void Dead(Player owner, NPC target)
        {
            dead = true;

            int time = MaxResurrectionTime;
            if (owner.TryGetModPlayer(out FairyCatcherPlayer fcp))
                fcp.fairyResurrectionTimeBous.ApplyTo(time);

            resurrectionTime = time;

            OnDead(owner, target);
        }

        public virtual void OnDead(Player owner, NPC target) { }

        /// <summary>
        /// 将生命值限制在0-最大值之间
        /// </summary>
        public void LimitLife()
        {
            //life = Math.Clamp(life, 0, (int)FairyLifeMax);
        }

        /// <summary>
        /// 将仙灵发射出去
        /// </summary>
        /// <returns></returns>
        public virtual bool ShootFairy(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int catcherDamage, float knockBack)
        {
            if (dead)
                return false;

            //catcherDamage += (int)FairyDamage;

            //生成仙灵弹幕
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, Item.shoot, catcherDamage, knockBack, player.whoAmI);
            //将弹幕的item赋值为自身
            if (proj.ModProjectile is IFairyProjectile fairyProjectile)
            {
                //fairyProjectile.FairyItem = this;
                //proj.scale = FairyScale;
            }

            IsOut = true;
            return true;
        }

        /// <summary>
        /// 回血或者执行复活
        /// </summary>
        /// <param name="lifeRegan"></param>
        public virtual void LifeRegan(int lifeRegan, int resurrectionTimeReduce = 1)
        {
            if (dead)
            {
                resurrectionTime -= resurrectionTimeReduce;
                if (resurrectionTime <= 0)
                    Resurrection();

                return;
            }

            Life += lifeRegan;
            LimitLife();
        }

        public virtual void Resurrection()
        {
            dead = false;
            //life = (int)FairyLifeMax;
            resurrectionTime = 0;
        }

        public override bool CanReforge() => false;

        #region 描述相关

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //稀有度
            tooltips.Add(RarityDescription());
            //当前血量
            //tooltips.Add(SurvivalStatus());

            if (Main.keyState.PressingShift())
                showLineValueCount += 2;

            showLineValueCount--;
            showLineValueCount = Math.Clamp(showLineValueCount, 0, 10);

            if (showLineValueCount >= 10)
            {
                //各种增幅数值
                tooltips.Add(LifeMaxDescription());
                tooltips.Add(DamageDescription());
                tooltips.Add(DefenceDescription());
                tooltips.Add(SpeedDescription());
                tooltips.Add(SkillLevelDescription());
                tooltips.Add(StaminaDescription());
                tooltips.Add(ScaleBonusDescription());
            }
            else
            {

                tooltips.Add(new TooltipLine(Mod, "SeeMore", FairySystem.SeeMore.Value));

                tooltips.Add(new TooltipLine(Mod, "RaderChart"
                    , "                                \n\n\n\n\n\n\n\n\n\n"));
            }
        }

        public TooltipLine RarityDescription()
        {
            string description = FairySystem.GetRarityDescription(Rarity);
            Color color = FairySystem.GetRarityColor(Rarity);

            TooltipLine line = new(Mod, "FairyRarity", description);
            line.OverrideColor = color;

            return line;
        }

        //public TooltipLine SurvivalStatus()
        //{
        //    string status;
        //    Color newColor;
        //    if (dead)
        //    {
        //        newColor = Color.OrangeRed;
        //        status = FairySystem.ResurrectionTime.Format($"{resurrectionTime / (60 * 60)}:{resurrectionTime / 60 % 60}");
        //    }
        //    else
        //    {
        //        newColor = Color.LightGreen;
        //        status = FairySystem.CurrentLife.Format(life, (int)FairyLifeMax);
        //    }

        //    TooltipLine line = new(Mod, "SurvivalStatus", status);
        //    line.OverrideColor = newColor;

        //    return line;
        //}

        public virtual TooltipLine LifeMaxDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyLocalize(FairyIV.LifeMaxLevel);

            TooltipLine line = new(Mod, FairyLifeMax
                , FairySystem.FormatIVDescription(FairySystem.FairyLifeMax, text, FairyIV.LifeMax));
            line.OverrideColor = c;
            return line;
        }

        public virtual TooltipLine DamageDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyLocalize(FairyIV.DamageLevel);

            TooltipLine line = new(Mod, FairyDamage
                , FairySystem.FormatIVDescription(FairySystem.FairyDamage, text, FairyIV.Damage));
            line.OverrideColor = c;

            return line;
        }

        public virtual TooltipLine DefenceDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyLocalize(FairyIV.DefenceLevel);

            TooltipLine line = new(Mod, FairyDefence
                , FairySystem.FormatIVDescription(FairySystem.FairyDefence, text, FairyIV.Defence));
            line.OverrideColor = c;
            return line;
        }

        public virtual TooltipLine SpeedDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyLocalize(FairyIV.SpeedLevel);

            TooltipLine line = new(Mod, FairySpeed
                , FairySystem.FormatIVDescription(FairySystem.FairySpeed, text, FairyIV.Speed));
            line.OverrideColor = c;
            return line;
        }

        public virtual TooltipLine SkillLevelDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyLocalize(FairyIV.SkillLevelLevel);

            TooltipLine line = new(Mod, FairySkillLevel
                , FairySystem.FormatIVDescription(FairySystem.FairySkillLevel, text, FairyIV.SkillLevel));
            line.OverrideColor = c;
            return line;
        }

        public virtual TooltipLine StaminaDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyLocalize(FairyIV.StaminaLevel);

            TooltipLine line = new(Mod, FairyStamina
                , FairySystem.FormatIVDescription(FairySystem.FairyStamina, text, FairyIV.Stamina));
            line.OverrideColor = c;
            return line;
        }

        public virtual TooltipLine ScaleBonusDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyLocalize(FairyIV.ScaleLevel);

            TooltipLine line = new(Mod, FairyScale
                , FairySystem.FormatIVDescription(FairySystem.FairyScale, text, FairyIV.Scale));
            line.OverrideColor = c;
            return line;
        }

        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (showLineValueCount >= 0 && line.Name == "RaderChart")
            {
                Vector2 topLeft = new(line.OriginalX, line.OriginalY);
                float factor = Helper.SqrtEase(1 - showLineValueCount / 10f);

                Vector2 size = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
                Vector2 center = topLeft + (size / 2);

                float length = factor * 7 * 12;

                SpriteBatch spriteBatch = Main.spriteBatch;

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendState, spriteBatch.GraphicsDevice.SamplerStates[0],
                                spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

                //绘制底层
                DrawRaderBack(center, length, new Color(99, 155, 255) * 0.7f);
                DrawRaderBack(center, factor * 5 * 12, new Color(36, 88, 179) * 0.85f);
                DrawRaderBack(center, factor * 3 * 12, new Color(28, 60, 116));

                //绘制雷达图
                DrawRaderChart(center, length);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendState, spriteBatch.GraphicsDevice.SamplerStates[0],
                                spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

                length = factor * (7 * 12 + 30);
                const float HexAngle = MathHelper.TwoPi / 6;

                //绘制上层图标
                //生命值
                DrawRaderIcon(center + (-MathHelper.PiOver2).ToRotationVector2() * length, 0, FairyIV.LifeMax, FairyIV.LifeMaxLevel);
                //防御
                DrawRaderIcon(center + (-MathHelper.PiOver2 + HexAngle).ToRotationVector2() * length, 2, FairyIV.Defence, FairyIV.DefenceLevel);
                //耐力
                DrawRaderIcon(center + (-MathHelper.PiOver2 + HexAngle * 2).ToRotationVector2() * length, 5, FairyIV.Stamina, FairyIV.StaminaLevel);
                //速度
                DrawRaderIcon(center + (-MathHelper.PiOver2 + HexAngle * 3).ToRotationVector2() * length, 3, FairyIV.Speed, FairyIV.SpeedLevel);
                //等级
                DrawRaderIcon(center + (-MathHelper.PiOver2 + HexAngle * 4).ToRotationVector2() * length, 4, FairyIV.SkillLevel, FairyIV.SkillLevelLevel);
                //攻击
                DrawRaderIcon(center + (-MathHelper.PiOver2 + HexAngle * 5).ToRotationVector2() * length, 1, FairyIV.Damage, FairyIV.DamageLevel);
            }
            else if (line.Name == FairyLifeMax)
                DrawIVIcon(0, new Vector2(line.X + 10, line.Y + 10));
            else if (line.Name == FairyDamage)
                DrawIVIcon(1, new Vector2(line.X + 10, line.Y + 10));
            else if (line.Name == FairyDefence)
                DrawIVIcon(2, new Vector2(line.X + 10, line.Y + 10));
            else if (line.Name == FairySpeed)
                DrawIVIcon(3, new Vector2(line.X + 10, line.Y + 10));
            else if (line.Name == FairySkillLevel)
                DrawIVIcon(4, new Vector2(line.X + 10, line.Y + 10));
            else if (line.Name == FairyStamina)
                DrawIVIcon(5, new Vector2(line.X + 10, line.Y + 10));
            else if (line.Name == FairyScale)
                DrawIVIcon(6, new Vector2(line.X + 10, line.Y + 10));
        }

        private static void DrawIVIcon(int frame, Vector2 center)
        {
            FairySystem.FairyIVIcon.Value.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, frame, 1, 8),
                center,scale:0.8f);
        }

        public static void DrawRaderBack(Vector2 center, float length,Color c)
        {
            Texture2D Texture = CoraliteAssets.Misc.WToT32x.Value;

            ColoredVertex centerVertex = new ColoredVertex(center, c, new Vector3(0, 1, 1));

            const float HexAngle = MathHelper.TwoPi / 6;

            ColoredVertex[] bars =
            [
                    centerVertex,
                new(center + (-MathHelper.PiOver2).ToRotationVector2()*length,
                    c, new Vector3(0, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle).ToRotationVector2()*length,
                    c, new Vector3(1 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*2).ToRotationVector2()*length,
                    c, new Vector3(2 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*3).ToRotationVector2()*length,
                    c, new Vector3(3 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*4).ToRotationVector2()*length,
                    c, new Vector3(4 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*5).ToRotationVector2()*length,
                    c, new Vector3(5 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2).ToRotationVector2()*length,
                    c, new Vector3(1, 0, 1)),
            ];


            Main.spriteBatch.GraphicsDevice.Textures[0] = Texture;
            Main.spriteBatch.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, bars, 0, 8
                ,indexes,0,6);
        }

        public void DrawRaderChart(Vector2 center, float baseLength)
        {
            float lifeMaxLength = GetLevelLength(baseLength, FairyIV.LifeMaxLevel);
            float damageLength = GetLevelLength(baseLength, FairyIV.DamageLevel);
            float defenceLength = GetLevelLength(baseLength, FairyIV.DefenceLevel);
            float speedLength = GetLevelLength(baseLength, FairyIV.SpeedLevel);
            float skillLevelLength = GetLevelLength(baseLength, FairyIV.SkillLevelLevel);
            float staminaLength = GetLevelLength(baseLength, FairyIV.StaminaLevel);

            Texture2D Texture = CoraliteAssets.Misc.White32x32.Value;

            ColoredVertex centerVertex = new ColoredVertex(center, Color.DarkCyan * 0.8f, new Vector3(0, 1, 1));

            const float HexAngle = MathHelper.TwoPi / 6;

            ColoredVertex[] bars =
            [
                    centerVertex,
                new(center + (-MathHelper.PiOver2).ToRotationVector2()*lifeMaxLength,
                    GetLevelColor(FairyIV.LifeMaxLevel), new Vector3(0, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle).ToRotationVector2()*defenceLength,
                    GetLevelColor(FairyIV.DefenceLevel), new Vector3(1 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*2).ToRotationVector2()*staminaLength,
                    GetLevelColor(FairyIV.StaminaLevel), new Vector3(2 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*3).ToRotationVector2()*speedLength,
                    GetLevelColor(FairyIV.SpeedLevel), new Vector3(4 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*4).ToRotationVector2()*skillLevelLength,
                    GetLevelColor(FairyIV.SkillLevelLevel), new Vector3(3 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*5).ToRotationVector2()*damageLength,
                    GetLevelColor(FairyIV.DamageLevel), new Vector3(5 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2).ToRotationVector2()*lifeMaxLength,
                    GetLevelColor(FairyIV.LifeMaxLevel), new Vector3(1, 0, 1)),
            ];

            Main.spriteBatch.GraphicsDevice.Textures[0] = Texture;
            Main.spriteBatch.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, bars, 0, 8
                , indexes, 0, 6);
        }

        private static float GetLevelLength(float baseLength, float level)
        {
            if (level < FairyIVLevelID.Eternal)
                return baseLength * level / FairyIVLevelID.Eternal;
            else
                return baseLength + baseLength * Math.Clamp( (level - FairyIVLevelID.Eternal) / (FairyIVLevelID.Over - FairyIVLevelID.Eternal),0,1);
        }

        private static Color GetLevelColor(float level)
        {
            if (level < FairyIVLevelID.Eternal)
                return Color.Lerp(Color.DarkCyan * 0.7f, Color.LightCyan*0.7f, level / FairyIVLevelID.Eternal);
            else
                return Color.LightCyan * 0.7f;
        }

        public static void GetScaledFactor(ref float baseValue, float maxValue)
        {
            if (baseValue < 1)//小于1的时候给它变成0-0.3
            {
                baseValue /= 3;
                return;
            }

            /*
             * 当基础值为1时为0.3
             * 当基础值为1到最大值时为0.3-1
             * 当基础值大于最大值时为1以上
             */
            baseValue = 0.3f + (0.7f * (baseValue - 1) / (maxValue - 1));
        }

        public static void DrawRaderIcon(Vector2 pos, int frame, float value, float level)
        {
            //绘制图标
            DrawIVIcon(frame,pos + new Vector2(0, -12));

            (Color c, _) = FairyIV.GetFairyLocalize(level);

            //绘制数字
            Utils.DrawBorderString(Main.spriteBatch, value.ToString(), pos + new Vector2(0, 18), c, 1, 0.5f, 0.5f);
        }

        #endregion

        //public override void SaveData(TagCompound tag)
        //{
        //    fairyData.SaveData(tag);
        //    tag.Add("Life", life);
        //    if (dead)
        //        tag.Add("ResurrectionTime", resurrectionTime);

        //    IsOut = false;
        //}

        //public override void LoadData(TagCompound tag)
        //{
        //    fairyData = new FairyData();
        //    fairyData.LoadData(tag);
        //    life = tag.GetInt("Life");
        //    if (tag.TryGet("ResurrectionTime", out int time))
        //    {
        //        dead = true;
        //        resurrectionTime = time;
        //    }

        //    IsOut = false;
        //}
    }

    //public interface IFairyItem
    //{
    //    public bool IsDead { get; }
    //    public bool IsOut { get; set; }
    //    public FairyData IV { get; set; }
    //    public int Life { get; set; }
    //    public float FairyLifeMax { get; }
    //    public int FairyType { get; }
    //    public float FairyDamage { get; }
    //    public FairyRarity Rarity { get; }

    //    /// <summary>
    //    /// 返回值是仙灵是否死亡
    //    /// </summary>
    //    /// <param name="target"></param>
    //    /// <param name="hit"></param>
    //    /// <param name="damageDon"></param>
    //    /// <returns></returns>
    //    public bool Hurt(Player owner, NPC target, NPC.HitInfo hit, int damageDon);
    //    public bool ShootFairy(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int catcherDamage, float knockBack);
    //}
}
