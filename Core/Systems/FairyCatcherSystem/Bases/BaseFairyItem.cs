using Coralite.Content.DamageClasses;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.FairyCatcherSystem.Bases
{
    public abstract class BaseFairyItem : ModItem, IFairyItem
    {
        /// <summary>
        /// 仙灵的个体数据，用于存放各类增幅
        /// </summary>
        protected FairyData fairyData;

        /// <summary> 仙灵的实际血量 </summary>
        protected int life;
        /// <summary> 仙灵是否存活 </summary>
        protected bool dead;

        public abstract int GetFairyType();

        /// <summary>
        /// 受到个体值加成过的仙灵自身的伤害
        /// </summary>
        public float FairyDamage => fairyData.damageBonus.ApplyTo(Item.GetGlobalItem<FairyGlobalItem>().baseDamage);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的大小
        /// </summary>
        public float FairyScale
        {
            get
            {
                float scale = fairyData.scaleBonus * Item.GetGlobalItem<FairyGlobalItem>().baseScale;
                scale = Math.Clamp(scale, 0.5f, 2.5f);
                return scale;
            }
        }
        /// <summary>
        /// 受到个体值加成过的仙灵自身的防御
        /// </summary>
        public float FairyDefence => fairyData.defenceBonus.ApplyTo(Item.GetGlobalItem<FairyGlobalItem>().baseDefence);
        /// <summary>
        /// 受到个体值加成过的仙灵自身的生命值上限
        /// </summary>
        public float FairyLifeMax => fairyData.lifeMaxBonus.ApplyTo(Item.GetGlobalItem<FairyGlobalItem>().baseLifeMax);

        public FairyData IV  { get => fairyData; set => fairyData = value; }
        public bool IsDead => dead;
        public int Life { get => life; set => life = value; }

        public virtual bool Hurt(Player owner,NPC target,NPC.HitInfo hit, int damageDon)
        {
            int damage = target.damage;

            damage -= (int)FairyDefence;

            if (damage < 1)
                damage = 1;

            life -= damage;
            if (life <= 0)
                dead = true;
            LimitLife();

            return dead;
        }

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
        public virtual bool ShootFairy(Player player,EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int catcherDamage, float knockBack)
        {
            if (dead)
                return false;

            catcherDamage += (int)player.GetTotalDamage<FairyDamage>().ApplyTo(Item.damage);

            //生成仙灵弹幕
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, Item.shoot, catcherDamage, knockBack, player.whoAmI);
            //将弹幕的item赋值为自身
            if (proj.ModProjectile is IFairyProjectile fairyProjectile)

                fairyProjectile.FairyItem = this;
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
        }

        public override void SaveData(TagCompound tag)
        {

        }

        public override void LoadData(TagCompound tag)
        {

        }
    }

    public interface IFairyItem
    {
        public bool IsDead { get; }
        public FairyData IV { get; set; }
        public int Life { get; set; }
        public float FairyLifeMax { get; }
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
