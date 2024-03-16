using Coralite.Content.DamageClasses;
using Coralite.Content.ModPlayers;
using Coralite.Content.UI;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.YujianSystem.HuluEffects;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.YujianSystem
{
    public abstract class BaseHulu : ModItem
    {
        private readonly string TexturePath;
        private readonly bool PathHasName;
        private readonly int Rare;
        private readonly int Value;
        private readonly int Damage;
        private readonly float Knockback;
        public readonly int slotCount;

        public Item[] Yujians;
        public bool[] CanChannel;
        private bool rightClick;

        public override string Texture => string.IsNullOrEmpty(TexturePath) ? base.Texture : TexturePath + (PathHasName ? string.Empty : Name);
        public override bool CanRightClick() => true;
        public override bool AltFunctionUse(Player player) => true;

        public BaseHulu(int slotCount, int rare, int value, int damage, float knockback, string texturePath = AssetDirectory.YujianHulu, bool pathHasName = false)
        {
            TexturePath = texturePath;
            PathHasName = pathHasName;
            Rare = rare;
            Value = value;
            Damage = damage;
            Knockback = knockback;
            this.slotCount = slotCount;
            if (slotCount <= 0)
                throw new System.Exception("至少要有一个御剑槽位！");
            if (slotCount > Coralite.YujianHuluContainsMax)
                throw new System.Exception("最多只能有10个！");

            Yujians = new Item[slotCount];
            CanChannel = new bool[slotCount];
            for (int i = 0; i < slotCount; i++)
            {
                Yujians[i] = new Item();
                CanChannel[i] = true;
            }

        }

        #region 基础设置

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.maxStack = 1;
            Item.useTime = 15;
            Item.useAnimation = 15;

            Item.rare = Rare;
            Item.value = Value;
            Item.damage = Damage;
            Item.knockBack = Knockback;

            Item.DamageType = ModContent.GetInstance<YujianDamage>();
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.noMelee = true;
            Item.channel = true;

            Item.shoot = ProjectileID.WoodenArrowFriendly;         //只是为了能够去调用shoot方法而已
        }

        public override void SaveData(TagCompound tag)
        {
            for (int i = 0; i < slotCount; i++)
            {
                tag.Add("Yujian" + i.ToString(), Yujians[i]);
            }
            for (int i = 0; i < slotCount; i++)
            {
                tag.Add("CanChannel" + i.ToString(), CanChannel[i]);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            for (int i = 0; i < slotCount; i++)
            {
                Yujians[i] = tag.Get<Item>("Yujian" + i.ToString());
            }
            for (int i = 0; i < slotCount; i++)
            {
                CanChannel[i] = tag.GetBool("CanChannel" + i.ToString());
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(Mod, "HuluDescription", Language.GetOrRegister($"Mods.Coralite.Systems.YujianSystem.HuluDescription", () => "右键单击打开UI面板，将御剑放进栏位后可召唤出御剑").Value);
            tooltips.Add(line);
        }

        #endregion

        #region 控制御剑

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                rightClick = true;
            else
                rightClick = false;

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (rightClick)
            {
                player.GetModPlayer<CoralitePlayer>().ownedYujianProj = true;
                //Kill 了所有的御剑弹幕，再生成自己的
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.friendly && proj.owner == player.whoAmI &&
                            (proj.ModProjectile is BaseYujianProj || proj.minion))
                        proj.Kill();
                }

                for (int i = 0; i < slotCount; i++)
                {
                    //虽说不是御剑物品根本放不进来，但是这里还是判断一下以防止出现意想不到的意外状况
                    if (Yujians[i].ModItem is BaseYujian yujianItem)
                    {
                        yujianItem.ShootYujian(player, source, damage, SetHuluEffect(), CanChannel[i]);
                    }

                }
            }

            return false;
        }

        public virtual IHuluEffect SetHuluEffect()
        {
            return new Hulu_NoEffect();
        }

        #endregion

        #region UI交互

        public override void RightClick(Player player)
        {
            YujianHuluBackpack.visible = true;
            YujianHuluBackpack.huluItem = this;
            UILoader.GetUIState<YujianHuluBackpack>().Recalculate();
        }

        public override bool ConsumeItem(Player player) => false;

        /// <summary>
        /// 供UI调用的方法，用于存储物品
        /// </summary>
        /// <param name="index"></param>
        public void SaveItem(int index, Item putInItem)
        {
            Yujians[index] = putInItem;
        }

        /// <summary>
        /// 检测这个栏位是否可用
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool CanUseSlot(int index)
        {
            if (index < slotCount)
                return true;

            return false;
        }

        /// <summary>
        /// 获取特定栏位中的物品，使用前请一定要判断索引是否小于最大栏位数！！！
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Item GetItem(int index)
        {
            return Yujians[index];
        }

        /// <summary>
        /// 设置对应槽位的是否可蓄力
        /// </summary>
        /// <param name="index"></param>
        public void SetCanChannel(int index)
        {
            CanChannel[index] = !CanChannel[index];
        }

        #endregion
    }
}
