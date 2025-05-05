using Coralite.Content.GlobalItems;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyItem : ModItem, IFairyItem
    {
        public override string Texture => AssetDirectory.FairyItems + Name;

        /// <summary>
        /// 仙灵的个体数据，用于存放各类增幅
        /// </summary>
        protected FairyData fairyData = new();

        /// <summary> 仙灵的实际血量 </summary>
        protected int life;
        /// <summary> 仙灵是否存活 </summary>
        protected bool dead;
        /// <summary>
        /// 复活时间
        /// </summary>
        protected int resurrectionTime;

        private static int showRadarCount;

        public abstract int FairyType { get; }
        public abstract FairyAttempt.Rarity Rarity { get; }
        public FairyData IV { get => fairyData; set => fairyData = value; }
        public bool IsDead => dead;
        public int Life { get => life; set => life = value; }
        /// <summary>
        /// 复活时间，默认3分钟（3*60*60）
        /// </summary>
        public virtual int MaxResurrectionTime => 60 * 60 * 3;

        /// <summary>
        /// 受到个体值加成过的仙灵自身的伤害
        /// </summary>
        public float FairyDamage => fairyData.damageBonus.ApplyTo(Item.damage);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的大小
        /// </summary>
        public float FairyScale
        {
            get
            {
                float scale = fairyData.scaleBonus * Item.GetGlobalItem<CoraliteGlobalItem>().baseScale;
                scale = Math.Clamp(scale, 0.5f, 2.5f);
                return scale;
            }
        }
        /// <summary>
        /// 受到个体值加成过的仙灵自身的防御
        /// </summary>
        public float FairyDefence => fairyData.defenceBonus.ApplyTo(Item.GetGlobalItem<CoraliteGlobalItem>().baseDefence);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的生命值上限
        /// </summary>
        public float FairyLifeMax => fairyData.lifeMaxBonus.ApplyTo(Item.GetGlobalItem<CoraliteGlobalItem>().baseLifeMax);

        public bool IsOut { get; set; }

        public override void SetStaticDefaults()
        {
            CoraliteSets.Items.IsFairy[Type] = true;
        }

        public sealed override void SetDefaults()
        {
            IsOut = false;
            Item.DamageType = Content.DamageClasses.FairyDamage.Instance;

            SetOtherDefaults();
            if (Item.TryGetGlobalItem(out CoraliteGlobalItem fairyItem))
            {
                SetFairyDefault(fairyItem);
                life = fairyItem.baseLifeMax;
            }
        }

        public virtual void SetOtherDefaults() { }

        /// <summary>
        /// 在这里设置仙灵的<br></br>
        /// <see cref="FairyGlobalItem.baseDamage"/><br></br>
        /// <see cref="FairyGlobalItem.baseDefence"/><br></br>
        /// <see cref="FairyGlobalItem.baseLifeMax"/><br></br>
        /// <see cref="FairyGlobalItem.baseLifeMax"/><br></br>
        /// 等字段
        /// </summary>
        /// <param name="fairyItem"></param>
        public virtual void SetFairyDefault(CoraliteGlobalItem fairyItem) { }

        public override ModItem Clone(Item newEntity)
        {
            ModItem modItem = base.Clone(newEntity);
            if (modItem != null)
                (modItem as IFairyItem).IV = IV;

            return modItem;
        }

        public virtual bool Hurt(Player owner, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int damage = target.damage;

            damage -= (int)FairyDefence;

            if (damage < 1)
                damage = 1;

            life -= damage;
            if (life <= 0)
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
            life = Math.Clamp(life, 0, (int)FairyLifeMax);
        }

        /// <summary>
        /// 将仙灵发射出去
        /// </summary>
        /// <returns></returns>
        public virtual bool ShootFairy(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int catcherDamage, float knockBack)
        {
            if (dead)
                return false;

            catcherDamage += (int)FairyDamage;

            //生成仙灵弹幕
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, Item.shoot, catcherDamage, knockBack, player.whoAmI);
            //将弹幕的item赋值为自身
            if (proj.ModProjectile is IFairyProjectile fairyProjectile)
            {
                fairyProjectile.FairyItem = this;
                proj.scale = FairyScale;
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

            life += lifeRegan;
            LimitLife();
        }

        public virtual void Resurrection()
        {
            dead = false;
            life = (int)FairyLifeMax;
            resurrectionTime = 0;
        }

        public override bool CanReforge() => false;

        #region 描述相关

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //稀有度
            tooltips.Add(RarityDescription());
            //当前血量
            tooltips.Add(SurvivalStatus());

            if (showRadarCount > 0)
            {
                if (Main.keyState.PressingShift())
                    showRadarCount += 2;

                tooltips.Add(new TooltipLine(Mod, "RaderChart"
                    , "                        \n\n\n\n\n"));

                showRadarCount--;

                if (showRadarCount > 15)
                    showRadarCount = 15;
            }
            else
            {
                if (Main.keyState.PressingShift())
                    showRadarCount = 2;

                //各种增幅数值
                tooltips.Add(LifeMaxeBonusDescription());
                tooltips.Add(DamageBonusDescription());
                tooltips.Add(DefenceBonusDescription());
                tooltips.Add(ScaleBonusDescription());
                tooltips.Add(new TooltipLine(Mod, "SeeMore", FairySystem.SeeMore.Value));
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

        public TooltipLine SurvivalStatus()
        {
            string status;
            Color newColor;
            if (dead)
            {
                newColor = Color.OrangeRed;
                status = FairySystem.ResurrectionTime.Format($"{resurrectionTime / (60 * 60)}:{resurrectionTime / 60 % 60}");
            }
            else
            {
                newColor = Color.LightGreen;
                status = FairySystem.CurrentLife.Format(life, (int)FairyLifeMax);
            }

            TooltipLine line = new(Mod, "SurvivalStatus", status);
            line.OverrideColor = newColor;

            return line;
        }

        public virtual TooltipLine LifeMaxeBonusDescription()
        {
            float @base = Item.GetGlobalItem<CoraliteGlobalItem>().baseLifeMax;
            float bonused = FairyLifeMax;
            (Color, LocalizedText) group = FairyIVAppraise.FairyLifeMaxAppraise.GetAppraiseResult(@base, bonused);

            TooltipLine line = new(Mod, "LifeMaxBonus"
                , FairySystem.FormatIVDescription(FairySystem.FairyLifeMax, group.Item2, @base, (int)bonused));
            line.OverrideColor = group.Item1;
            return line;
        }

        public virtual TooltipLine DamageBonusDescription()
        {
            float @base = Item.damage;
            float bonused = FairyDamage;
            (Color, LocalizedText) group = FairyIVAppraise.FairyDamageAppraise.GetAppraiseResult(@base, bonused);

            TooltipLine line = new(Mod, "DamageBonus"
                , FairySystem.FormatIVDescription(FairySystem.FairyDamage, group.Item2, @base, (int)bonused));
            line.OverrideColor = group.Item1;
            return line;
        }

        public virtual TooltipLine DefenceBonusDescription()
        {
            float @base = Item.GetGlobalItem<CoraliteGlobalItem>().baseDefence;
            float bonused = FairyDefence;
            (Color, LocalizedText) group = FairyIVAppraise.FairyDefenceAppraise.GetAppraiseResult(@base, bonused);

            TooltipLine line = new(Mod, "DefenceBonus"
                , FairySystem.FormatIVDescription(FairySystem.FairyDefence, group.Item2, @base, (int)bonused));
            line.OverrideColor = group.Item1;
            return line;
        }

        public virtual TooltipLine ScaleBonusDescription()
        {
            float @base = Item.GetGlobalItem<CoraliteGlobalItem>().baseScale;
            float bonused = MathF.Round(FairyScale, 2);

            TooltipLine line = new(Mod, "ScaleBonus"
                , FairySystem.FairyScale.Value + $"{bonused} ({@base})");
            return line;
        }

        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (showRadarCount > 0 && line.Name == "RaderChart")
            {
                Vector2 topLeft = new(line.OriginalX, line.OriginalY);
                float factor = showRadarCount / 15f;

                Vector2 size = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);

                Texture2D backgroundTex = ModContent.Request<Texture2D>(AssetDirectory.Misc + "FairyRaderBack").Value;

                Vector2 center = topLeft + (size / 2);
                //绘制底层
                DrawRaderBack(center, factor, backgroundTex);
                //绘制雷达图
                DrawRaderChart(center, factor, backgroundTex.Width / 2);

                //绘制上层图标
                //生命值
                DrawRaderIcon(center, center + new Vector2(0, (-backgroundTex.Height / 2) - 12), factor, "[i:58]");
                //攻击力
                DrawRaderIcon(center, center + new Vector2((-backgroundTex.Width / 2) - 12, 0), factor, "[i:3507]");
                //防御
                DrawRaderIcon(center, center + new Vector2((backgroundTex.Width / 2) + 12, 0), factor, "[i:3811]");
                //大小
                DrawRaderIcon(center, center + new Vector2(0, (backgroundTex.Height / 2) + 12), factor, "[i:486]");
            }
        }

        public static void DrawRaderBack(Vector2 center, float factor, Texture2D backgroundTex)
        {
            factor = Coralite.Instance.SqrtSmoother.Smoother(factor);

            Main.spriteBatch.Draw(backgroundTex, center, null, Color.White * factor, 0, backgroundTex.Size() / 2, factor, 0, 0);
        }

        public void DrawRaderChart(Vector2 center, float factor, float baseLength)
        {
            float damageFactor = FairyDamage / Item.damage;
            float defenceFactor = FairyDefence / Item.GetGlobalItem<CoraliteGlobalItem>().baseDefence;
            float lifeMaxFactor = FairyLifeMax / Item.GetGlobalItem<CoraliteGlobalItem>().baseLifeMax;
            float scaleFactor = FairyScale;

            GetScaledFactor(ref damageFactor, FairyIVAppraise.FairyDamageAppraise.AppraiseLevels[^2].Item1);
            GetScaledFactor(ref defenceFactor, FairyIVAppraise.FairyDefenceAppraise.AppraiseLevels[^2].Item1);
            GetScaledFactor(ref lifeMaxFactor, FairyIVAppraise.FairyLifeMaxAppraise.AppraiseLevels[^2].Item1);

            //缩放比较特别，因为它固定0.5-2.5
            scaleFactor = (scaleFactor - 0.5f) / (2.5f - 0.5f);

            damageFactor *= factor;
            defenceFactor *= factor;
            lifeMaxFactor *= factor;
            scaleFactor *= factor;

            Texture2D Texture = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "White32x32").Value;

            ColoredVertex[] bars = new ColoredVertex[4];

            bars[0] = new(center + new Vector2(0, -baseLength * lifeMaxFactor),
                GetScaledColor(lifeMaxFactor, factor), new Vector3(0, 0, 1));
            bars[1] = new(center + new Vector2(baseLength * defenceFactor, 0),
                GetScaledColor(defenceFactor, factor), new Vector3(0, 1, 1));

            bars[2] = new(center + new Vector2(-baseLength * damageFactor, 0),
                GetScaledColor(damageFactor, factor), new Vector3(1, 0, 1));
            bars[3] = new(center + new Vector2(0, baseLength * scaleFactor),
                GetScaledColor(scaleFactor, factor), new Vector3(1, 1, 1));

            SpriteBatch spriteBatch = Main.spriteBatch;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendState, spriteBatch.GraphicsDevice.SamplerStates[0],
                            spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

            Main.spriteBatch.GraphicsDevice.Textures[0] = Texture;
            Main.spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars, 0, 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendState, spriteBatch.GraphicsDevice.SamplerStates[0],
                            spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);
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

        public static Color GetScaledColor(float factor, float alphaFactor)
        {
            Color green = new(108, 234, 255);
            Color pink = new(238, 206, 231);
            Color c;
            if (factor < 0.3f)
                c = Color.Lerp(Color.Gray, pink, factor / 0.3f);
            else
                c = Color.Lerp(pink, green, (factor - 0.3f) / 0.7f);

            c *= alphaFactor * 0.7f;
            return c;
        }

        public static void DrawRaderIcon(Vector2 originPos, Vector2 aimPos, float factor, string text)
        {
            Vector2 currentPos = Vector2.SmoothStep(originPos, aimPos, factor);
            currentPos += new Vector2(-12);
            Utils.DrawBorderString(Main.spriteBatch, text, currentPos, Color.White, 1);
        }

        #endregion

        public override void SaveData(TagCompound tag)
        {
            fairyData.SaveData(tag);
            tag.Add("Life", life);
            if (dead)
                tag.Add("ResurrectionTime", resurrectionTime);

            IsOut = false;
        }

        public override void LoadData(TagCompound tag)
        {
            fairyData = new FairyData();
            fairyData.LoadData(tag);
            life = tag.GetInt("Life");
            if (tag.TryGet("ResurrectionTime", out int time))
            {
                dead = true;
                resurrectionTime = time;
            }

            IsOut = false;
        }
    }

    public interface IFairyItem
    {
        public bool IsDead { get; }
        public bool IsOut { get; set; }
        public FairyData IV { get; set; }
        public int Life { get; set; }
        public float FairyLifeMax { get; }
        public int FairyType { get; }
        public float FairyDamage { get; }
        public FairyAttempt.Rarity Rarity { get; }

        /// <summary>
        /// 返回值是仙灵是否死亡
        /// </summary>
        /// <param name="target"></param>
        /// <param name="hit"></param>
        /// <param name="damageDon"></param>
        /// <returns></returns>
        public bool Hurt(Player owner, NPC target, NPC.HitInfo hit, int damageDon);
        public bool ShootFairy(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int catcherDamage, float knockBack);
    }
}
