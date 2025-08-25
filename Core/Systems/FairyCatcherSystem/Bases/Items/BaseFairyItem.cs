using Coralite.Core.Systems.FairyCatcherSystem.FairyFreePart;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases.Items
{
    public abstract class BaseFairyItem : ModItem
    {
        public override string Texture => AssetDirectory.FairyItems + Name;

        /// <summary> 仙灵的个体数据，用于存放各类增幅 </summary>
        public FairyIV FairyIV { get; set; }

        /// <summary> 仙灵是否存活 </summary>
        protected bool dead;
        /// <summary> 仅在UI中使用，不显示等级 </summary>
        public bool DontShowIV = true;
        //private static int showLineValueCount;

        public abstract int FairyType { get; }
        public abstract FairyRarity Rarity { get; }
        public bool IsDead => dead;
        public int Life { get; private set; }

        /// <summary>
        /// 用于记录仙灵弹幕的索引，便于查找
        /// </summary>
        private int _fairyProjIndex = -1;
        /// <summary>
        /// 用于记录仙灵的弹幕的唯一ID，如果没有就在能够再次射出仙灵
        /// </summary>
        private int _fairyProjUUID = -1;

        /// <summary>
        /// 仙林是否外出
        /// </summary>
        public bool IsOut { get; private set; }


        private static readonly int[] indexes = [0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6, 0, 6, 7];
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

        /// <summary>
        /// 初始化仙灵的各项数值
        /// </summary>
        /// <param name="fairyIV"></param>
        public void Initialize(FairyIV fairyIV)
        {
            FairyIV = fairyIV;
            Life = fairyIV.LifeMax;
            DontShowIV = false;
        }

        public override ModItem Clone(Item newEntity)
        {
            ModItem modItem = base.Clone(newEntity);
            if (modItem != null && modItem is BaseFairyItem bfi)
            {
                bfi.FairyIV = FairyIV;
                bfi.Life = Life;
                bfi.dead = dead;
                bfi.DontShowIV = false;
            }

            return modItem;
        }

        public virtual void HurtByProjectile(BaseFairyProjectile proj, Projectile target, int damage)
        {
            //防御计算与限制
            damage -= FairyIV.Defence;
            if (damage < 1)
                damage = 1;

            Life -= damage;
            LimitLife();
            proj.ImmuneTimer = proj.Projectile.localNPCHitCooldown;

            //死亡判定
            if (Life <= 0)
            {
                Dead(proj.Owner);

                proj.Projectile.Kill();
            }
        }

        /// <summary>
        /// 仙灵受到NPC攻击时调用
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="owner"></param>
        /// <param name="target"></param>
        /// <param name="hit"></param>
        /// <param name="damageDone"></param>
        public virtual void HurtByNPC(BaseFairyProjectile proj, NPC target, NPC.HitModifiers hit, int adjustedDamage)
        {
            //防御计算与限制
            adjustedDamage -= FairyIV.Defence;
            if (adjustedDamage < 1)
                adjustedDamage = 1;

            Life -= adjustedDamage;
            LimitLife();
            proj.ImmuneTimer = proj.Projectile.localNPCHitCooldown;

            //死亡判定
            if (Life <= 0)
            {
                Dead(proj.Owner);

                proj.Projectile.Kill();
                proj.OnKillByNPC(target);
            }
        }

        public void Dead(Player owner)
        {
            dead = true;

            //杀掉仙灵弹幕
            if (Main.projectile.IndexInRange(_fairyProjIndex))
            {
                Projectile p = Main.projectile[_fairyProjIndex];
                if (!p.active || p.owner != owner.whoAmI || p.projUUID != _fairyProjUUID)
                {
                    IsOut = false;
                    _fairyProjIndex = -1;
                    _fairyProjUUID = -1;
                }
            }

            OnDead(owner);
        }

        public virtual void OnDead(Player owner) { }

        /// <summary>
        /// 将生命值限制在0-最大值之间
        /// </summary>
        public void LimitLife()
        {
            Life = Math.Clamp(Life, 0, FairyIV.LifeMax);
        }

        /// <summary>
        /// 将仙灵发射出去
        /// </summary>
        /// <returns></returns>
        public virtual bool ShootFairy(Player player, IEntitySource source, Vector2 position, Vector2 velocity, float knockBack, float flyTime, float staminaAdjust = 0, float ai2 = 0)
        {
            //生成仙灵弹幕
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, Item.shoot
                , FairyIV.Damage, knockBack, player.whoAmI, flyTime, staminaAdjust, ai2);

            _fairyProjIndex = proj.identity;
            _fairyProjUUID = proj.projUUID;

            //将弹幕的item赋值为自身
            if (proj.ModProjectile is BaseFairyProjectile fairyProjectile)
                fairyProjectile.FairyItem = this;

            IsOut = true;
            return true;
        }

        /// <summary>
        /// 治疗仙灵，如果仙灵死掉了会根据仙灵等级增加治疗量，回满血后复活
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="healValue"></param>
        public virtual void HealFairy(float percent, int healValue)
        {
            //基础治疗数值，数+血量百分比
            float heal = healValue + percent * FairyIV.LifeMax;

            //挂掉的时候根据等级提升治疗量
            if (dead)
            {
                heal *= 1.25f + FairyIV.ScaleLV * 0.075f;
            }

            Life += (int)heal;
            LimitLife();

            if (dead)
            {
                if (Life >= FairyIV.LifeMax)
                    dead = false;
                return;
            }
        }

        public override bool CanReforge() => false;

        #region 仙灵出动相关逻辑

        /// <summary>
        /// 放入仙灵瓶时调用（仅本地端调用）
        /// </summary>
        public void OnBottleActive()
        {
            _fairyProjIndex = -1;
            _fairyProjUUID = -1;
        }

        /// <summary>
        /// 在仙灵瓶内更新，检测仙灵弹幕是否存在
        /// </summary>
        public void UpdateInBottle_Inner(BaseFairyBottle bottle, Player player)
        {
            if (IsOut)
            {
                if (Main.projectile.IndexInRange(_fairyProjIndex))
                {
                    Projectile p = Main.projectile[_fairyProjIndex];
                    if (!p.active || p.owner != player.whoAmI || p.projUUID != _fairyProjUUID)
                    {
                        IsOut = false;
                        _fairyProjIndex = -1;
                        _fairyProjUUID = -1;
                    }
                }
                else
                    IsOut = false;
            }

            UpdateInBottle(bottle, player);
        }

        /// <summary>
        /// 在仙灵瓶内更新，执行各种特殊操作
        /// </summary>
        /// <param name="player"></param>
        public virtual void UpdateInBottle(BaseFairyBottle bottle, Player player) { }

        /// <summary>
        /// 仙灵瓶被取出时调用
        /// </summary>
        public void OnBottleInactive(Player player)
        {
            if (Main.projectile.IndexInRange(_fairyProjIndex))
            {
                Projectile p = Main.projectile[_fairyProjIndex];
                if (p.active && p.owner != player.whoAmI && p.projUUID == _fairyProjUUID)
                {
                    p.Kill();
                    _fairyProjIndex = -1;
                    _fairyProjUUID = -1;
                }
            }
        }

        #endregion

        #region 放生相关

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            if (Item.shimmerWet && !Item.shimmered && !Item.CanShimmer())
            {
                int num = (int)(Item.Center.X / 16f);
                int num2 = (int)(Item.position.Y / 16f - 1f);
                if (!WorldGen.InWorld(num, num2) || Main.tile[num, num2] == null || Main.tile[num, num2].LiquidAmount <= 0 || Main.tile[num, num2].LiquidType!=LiquidID.Shimmer)
                    return;

                if (Item.playerIndexTheItemIsReservedFor == Main.myPlayer && !VaultUtils.isClient)
                {
                    Item.shimmerTime += 0.02f;
                    if (Item.shimmerTime > 0.9f)
                    {
                        Item.shimmerTime = 0.9f;
                        SpawnFairyFreeProj(Item.Center);
                        Item.TurnToAir();
                    }
                }
                else
                {
                    Item.shimmerTime += 0.02f;
                    if (Item.shimmerTime > 1f)
                        Item.shimmerTime = 1f;
                }
            }
        }

        public void SpawnFairyFreeProj(Vector2 pos)
        {
            Item.ShimmerEffect(pos);
            int owner = 255;
            if (VaultUtils.isSinglePlayer)
                owner = Main.myPlayer;
            else//找到最近的玩家
            {
                float dis = -1;
                foreach (var player in Main.ActivePlayers)
                {
                    if (dis == -1)
                    {
                        dis = Vector2.Distance(player.Center, pos);
                        owner = player.whoAmI;
                    }
                    else
                    {
                        float dis2 = Vector2.Distance(player.Center, pos);
                        if (dis2 < dis)
                        {
                            dis = dis2;
                            owner = player.whoAmI;
                        }
                    }
                }
            }

            Projectile.NewProjectile(new EntitySource_FairyFree(FairyType)
                , pos, new Vector2(0, -2), ModContent.ProjectileType<FairyFreeProj>(), 0, 0, owner, FairyType, Item.shoot);
        }

        #endregion

        #region 个体值增幅相关

        public override bool CanRightClick()
        {
            return base.CanRightClick();
        }

        public override bool ConsumeItem(Player player) => false;

        #endregion

        #region 描述相关

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            //稀有度
            tooltips.Add(RarityDescription());
            if (DontShowIV)
                return;

            //当前血量
            tooltips.Add(SurvivalStatus());

            //if (Main.keyState.PressingShift())
            //    showLineValueCount += 2;

            //showLineValueCount--;
            //showLineValueCount = Math.Clamp(showLineValueCount, 0, 10);

            //if (showLineValueCount >= 10)
            //{
            //各种增幅数值
            tooltips.Add(new TooltipLine(Mod, "RaderChart"
                , "一一一一一                                        "));

            tooltips.Add(LifeMaxDescription());
            tooltips.Add(DamageDescription());
            tooltips.Add(DefenceDescription());
            tooltips.Add(SpeedDescription());
            tooltips.Add(SkillLevelDescription());
            tooltips.Add(StaminaDescription());
            tooltips.Add(ScaleBonusDescription());
            ////}
            //else
            //{

            //    tooltips.Add(new TooltipLine(Mod, "SeeMore", FairySystem.SeeMore.Value));

            //    tooltips.Add(new TooltipLine(Mod, "RaderChart"
            //        , "                                \n\n\n\n\n\n\n\n\n\n"));
            //}
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
            TooltipLine line = new(Mod, "SurvivalStatus", ".                                     ");
            line.OverrideColor = Color.Transparent;
            return line;
        }

        public virtual TooltipLine LifeMaxDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyIVColorAndText(FairyIV.LifeMaxLV);

            TooltipLine line = new(Mod, FairyLifeMax
                , FairySystem.FormatIVDescription(FairySystem.FairyLifeMax, text, FairyIV.LifeMax));
            line.OverrideColor = c;
            return line;
        }

        public virtual TooltipLine DamageDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyIVColorAndText(FairyIV.DamageLV);

            TooltipLine line = new(Mod, FairyDamage
                , FairySystem.FormatIVDescription(FairySystem.FairyDamage, text, FairyIV.Damage));
            line.OverrideColor = c;

            return line;
        }

        public virtual TooltipLine DefenceDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyIVColorAndText(FairyIV.DefenceLV);

            TooltipLine line = new(Mod, FairyDefence
                , FairySystem.FormatIVDescription(FairySystem.FairyDefence, text, FairyIV.Defence));
            line.OverrideColor = c;
            return line;
        }

        public virtual TooltipLine SpeedDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyIVColorAndText(FairyIV.SpeedLV);

            TooltipLine line = new(Mod, FairySpeed
                , FairySystem.FormatIVDescription(FairySystem.FairySpeed, text, FairyIV.Speed));
            line.OverrideColor = c;
            return line;
        }

        public virtual TooltipLine SkillLevelDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyIVColorAndText(FairyIV.SkillLevelLV);

            TooltipLine line = new(Mod, FairySkillLevel
                , FairySystem.FormatIVDescription(FairySystem.FairySkillLevel, text, FairyIV.SkillLevel));
            line.OverrideColor = c;
            return line;
        }

        public virtual TooltipLine StaminaDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyIVColorAndText(FairyIV.StaminaLV);

            TooltipLine line = new(Mod, FairyStamina
                , FairySystem.FormatIVDescription(FairySystem.FairyStamina, text, FairyIV.Stamina));
            line.OverrideColor = c;
            return line;
        }

        public virtual TooltipLine ScaleBonusDescription()
        {
            (Color c, LocalizedText text) = FairyIV.GetFairyIVColorAndText(FairyIV.ScaleLV);

            TooltipLine line = new(Mod, FairyScale
                , FairySystem.FormatIVDescription(FairySystem.FairyScale, text, FairyIV.Scale));
            line.OverrideColor = c;
            return line;
        }

        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (line.Name== "SurvivalStatus")//绘制血量条{
            {
                Vector2 size = Helper.GetStringSize(line.Text, Vector2.One);
                DrawHealthBar(new Vector2(line.X, line.Y) + size/2, Life, FairyIV.LifeMax, size.X);
            }
            else if (/*showLineValueCount >= 0 && */line.Name == "RaderChart")
                DrawRaderChat(line);
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

        private void DrawRaderChat(DrawableTooltipLine line)
        {
            Vector2 topLeft = new(line.OriginalX, line.OriginalY);
            float factor = 1;/*Helper.SqrtEase(1 - showLineValueCount / 10f)*/;

            Vector2 size = ChatManager.GetStringSize(line.Font, line.Text, line.BaseScale);
            size.Y *= 8;
            Vector2 center = topLeft + new Vector2(size.X - 7 * 9 - 30, size.Y / 2);

            float length = factor * 7 * 9;

            SpriteBatch spriteBatch = Main.spriteBatch;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendState, spriteBatch.GraphicsDevice.SamplerStates[0],
                            spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

            //绘制底层
            DrawRaderBack(center, length, new Color(99, 155, 255) * 0.7f);
            DrawRaderBack(center, factor * 5 * 9, new Color(36, 88, 179) * 0.85f);
            DrawRaderBack(center, factor * 3 * 9, new Color(28, 60, 116));

            //绘制雷达图
            DrawRaderChart(center, length);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, spriteBatch.GraphicsDevice.BlendState, spriteBatch.GraphicsDevice.SamplerStates[0],
                            spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState, null, Main.UIScaleMatrix);

            length = factor * (7 * 9 + 26);
            const float HexAngle = MathHelper.TwoPi / 6;

            //绘制上层图标
            //生命值
            DrawRaderIcon(center + (-MathHelper.PiOver2).ToRotationVector2() * length, 0, FairyIV.LifeMax, FairyIV.LifeMaxLV);
            //防御
            DrawRaderIcon(center + (-MathHelper.PiOver2 + HexAngle).ToRotationVector2() * length, 2, FairyIV.Defence, FairyIV.DefenceLV);
            //耐力
            DrawRaderIcon(center + (-MathHelper.PiOver2 + HexAngle * 2).ToRotationVector2() * length, 5, FairyIV.Stamina, FairyIV.StaminaLV);
            //速度
            DrawRaderIcon(center + (-MathHelper.PiOver2 + HexAngle * 3).ToRotationVector2() * length, 3, FairyIV.Speed, FairyIV.SpeedLV);
            //等级
            DrawRaderIcon(center + (-MathHelper.PiOver2 + HexAngle * 4).ToRotationVector2() * length, 4, FairyIV.SkillLevel, FairyIV.SkillLevelLV);
            //攻击
            DrawRaderIcon(center + (-MathHelper.PiOver2 + HexAngle * 5).ToRotationVector2() * length, 1, FairyIV.Damage, FairyIV.DamageLV);
        }

        public void DrawHealthBar(Vector2 center, float Health, float MaxHealth,float maxLength, float alpha = 0.9f)
        {
            if (MaxHealth == 0)
                MaxHealth = 1;

            float factor = Health / (float)MaxHealth;
            if (factor > 1f)
                factor = 1f;

            Color newColor;
            Color backColor;
            string status;
            if (dead)
            {
                newColor = Color.OrangeRed;
                backColor = Color.OrangeRed;
                status = FairySystem.ResurrectionTime.Format(Health, MaxHealth);
            }
            else
            {
                 backColor = Color.Lerp(Color.DarkRed, Color.DarkGreen, factor);
                newColor = Color.Lerp(Color.OrangeRed, Color.LawnGreen, factor);
                status = FairySystem.CurrentLife.Format(Health, MaxHealth);
            }

            Color barColor = newColor;
            float totalBarLength = maxLength - 6;

            Texture2D tex = CoraliteAssets.Sparkle.BarSPA.Value;

            Vector2 scale =new Vector2( totalBarLength / tex.Width ,1);
            backColor *= alpha*0.4f;
            barColor *= alpha  * 0.4f;

            barColor.A = 0;

            //绘制底部条，固定绘制一个横杠
            Main.spriteBatch.Draw(tex, center, null, backColor
                , 0, tex.Size() / 2, scale, 0, 0);

            //绘制顶部，裁剪矩形绘制
            Rectangle rect = new Rectangle(0, 0, (int)(factor * tex.Width), tex.Height);
            Main.spriteBatch.Draw(tex, center + new Vector2(-scale.X * tex.Width / 2, 0), rect, barColor, 0, new Vector2(0, tex.Height / 2), scale, 0, 0);

            //绘制指针
            Texture2D tex2 = CoraliteAssets.Sparkle.ShotLineSPA.Value;
            //scale = totalBarLength / tex.Width;

            Vector2 pos = center + new Vector2(-scale.X * tex.Width / 2 + factor * scale.X * tex.Width, 0);

            Vector2 scale1 = new Vector2((float)tex2.Height / tex2.Width, 1) * 0.65f * 0.55f;
            Main.spriteBatch.Draw(tex2, pos, null, barColor
                , MathHelper.PiOver2, tex2.Size() / 2, scale1, 0, 0);
            Main.spriteBatch.Draw(tex2, pos, null, barColor with { A = 50 }
                , MathHelper.PiOver2, tex2.Size() / 2, scale1, 0, 0);


            Utils.DrawBorderString(Main.spriteBatch, status, center+new Vector2(0,4), newColor, 1, 0.5f, 0.5f);
        }

        private static void DrawIVIcon(int frame, Vector2 center)
        {
            FairySystem.FairyIVIcon.Value.QuickCenteredDraw(Main.spriteBatch, new Rectangle(0, frame, 1, 8),
                center, scale: 0.76f);
        }

        public static void DrawRaderBack(Vector2 center, float length, Color c)
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
                , indexes, 0, 6);
        }

        public void DrawRaderChart(Vector2 center, float baseLength)
        {
            float lifeMaxLength = GetLevelLength(baseLength, FairyIV.LifeMaxLV);
            float damageLength = GetLevelLength(baseLength, FairyIV.DamageLV);
            float defenceLength = GetLevelLength(baseLength, FairyIV.DefenceLV);
            float speedLength = GetLevelLength(baseLength, FairyIV.SpeedLV);
            float skillLevelLength = GetLevelLength(baseLength, FairyIV.SkillLevelLV);
            float staminaLength = GetLevelLength(baseLength, FairyIV.StaminaLV);

            Texture2D Texture = CoraliteAssets.Misc.White32x32.Value;

            ColoredVertex centerVertex = new ColoredVertex(center, Color.DarkCyan * 0.8f, new Vector3(0, 1, 1));

            const float HexAngle = MathHelper.TwoPi / 6;

            ColoredVertex[] bars =
            [
                    centerVertex,
                new(center + (-MathHelper.PiOver2).ToRotationVector2()*lifeMaxLength,
                    GetLevelColor(FairyIV.LifeMaxLV), new Vector3(0, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle).ToRotationVector2()*defenceLength,
                    GetLevelColor(FairyIV.DefenceLV), new Vector3(1 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*2).ToRotationVector2()*staminaLength,
                    GetLevelColor(FairyIV.StaminaLV), new Vector3(2 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*3).ToRotationVector2()*speedLength,
                    GetLevelColor(FairyIV.SpeedLV), new Vector3(4 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*4).ToRotationVector2()*skillLevelLength,
                    GetLevelColor(FairyIV.SkillLevelLV), new Vector3(3 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2+HexAngle*5).ToRotationVector2()*damageLength,
                    GetLevelColor(FairyIV.DamageLV), new Vector3(5 / 6f, 0, 1)),
                new(center + (-MathHelper.PiOver2).ToRotationVector2()*lifeMaxLength,
                    GetLevelColor(FairyIV.LifeMaxLV), new Vector3(1, 0, 1)),
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
                return baseLength + baseLength * Math.Clamp((level - FairyIVLevelID.Eternal) / (FairyIVLevelID.Over - FairyIVLevelID.Eternal), 0, 1);
        }

        private static Color GetLevelColor(float level)
        {
            if (level < FairyIVLevelID.Eternal)
                return Color.Lerp(Color.DarkCyan * 0.7f, Color.LightCyan * 0.7f, level / FairyIVLevelID.Eternal);
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
            baseValue = 0.3f + 0.7f * (baseValue - 1) / (maxValue - 1);
        }

        public static void DrawRaderIcon(Vector2 pos, int frame, float value, float level)
        {
            //绘制图标
            DrawIVIcon(frame, pos /*+ new Vector2(0, -12)*/);

            (Color c, _) = FairyIV.GetFairyIVColorAndText(level);

            //绘制数字
            //Utils.DrawBorderString(Main.spriteBatch, value.ToString(), pos + new Vector2(0, 18), c, 1, 0.5f, 0.5f);
        }

        #endregion

        /// <summary>
        /// 获得仙灵技能
        /// </summary>
        /// <returns></returns>
        public virtual int[] GetFairySkills()
        {
            return [];
        }

        public override void SaveData(TagCompound tag)
        {
            FairyIV.Save(tag);
            if (IsDead)
                tag.Add(nameof(IsDead), IsDead);
            tag.Add(nameof(Life), Life);
        }

        public override void LoadData(TagCompound tag)
        {
            FairyIV = FairyIV.Load(Item, FairyType, tag);
            dead = tag.ContainsKey(nameof(IsDead));
            if (tag.TryGet(nameof(Life), out int l))
                Life = l;

            DontShowIV = false;
        }
    }
}
