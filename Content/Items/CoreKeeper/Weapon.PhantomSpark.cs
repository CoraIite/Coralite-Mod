using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    /*
     * 攻击方式
     * 核心特色：通过射击积攒灵魂，并且召唤出灵魂协同攻击
     * 
     * 【普通形态】
     * 左键右键都是平平无奇射击，射速比较慢
     * 冲刺是闪现，如果位移过程闪避了攻击则会进入超级强化状态
     *  如果没有则会进入普通强化状态
     * 
     * 【普通强化状态】
     * 左键散射，右键射出追踪箭
     * 
     * 【超级强化状态】
     * 左键射出扩散箭，并且箭矢会产生爆炸
     * 右键射出追踪箭（也会爆炸）与几颗小水晶，小水晶命中后会弹回并再次追踪（有次数限制）
     */
    public class PhantomSpark : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public int useCount;
        public int oldCombo;
        //private int holdItemCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.SetWeaponValues(216, 5, 12);
            Item.useTime = 26;
            Item.useAnimation = 26;

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = RarityType<LegendaryRarity>();
            //Item.shoot = ProjectileType<RuneSongSlash>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            //Item.expert = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltip = tooltips.FirstOrDefault(t => t.Mod == "Terraria" && t.Name == "Damage", null);
            if (tooltip != null)
            {
                bool addLeadingSpace = Item.DamageType is not VanillaDamageClass;
                string tip = (addLeadingSpace ? " " : "") + Item.DamageType.DisplayName;

                tooltip.Text = string.Concat(((int)(Item.damage * 0.903f)).ToString()
                    , "-", ((int)(Item.damage * 1.098f)).ToString(), tip);
            }
        }

        public bool Dash(Player Player, int DashDir)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlyphParchment>()
                .AddIngredient<ChannelingGemstone>()
                .AddIngredient<FracturedLimbs>()
                .AddIngredient<EnergyString>()
                .AddIngredient(ItemID.HallowedBar, 100)
                .AddIngredient<AncientGemstone>(20)
                .AddCondition(CoraliteConditions.UseRuneParchment)
                .Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
    }

    /// <summary>
    /// ai0输入1表示追踪，ai1输入1表示会发生爆炸
    /// </summary>
    public class PhantomSparkNormalArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public bool Chase => Projectile.ai[0] == 1;
        public bool Explode => Projectile.ai[1] == 1;
        public ref float Target => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];

        public bool init = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (init)
            {
                init = false;
                Target = -1;
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.3f, 0.5f));

            UpdateFrame();

            if (Chase)
            {
                if (Target.GetNPCOwner(out NPC target, () => Target = -1))
                {
                    return;
                }

                Timer++;
                if (Timer > 40)
                {
                    if (Helper.TryFindClosestEnemy(Projectile.Center, 1000
                        , n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC t))
                        Target = t.whoAmI;
                    Timer = 0;
                }
            }
        }

        private void UpdateFrame()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 3)
                    Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickFrameDraw(new Rectangle(Projectile.frame, 1, 4, 1), Color.White, 0);

            return false;
        }
    }

    public class PhantomSparkExplode
    {

    }

    public class PhantomSparkPhantomNPC : ModProjectile
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];

        private int frameY;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 48;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            switch (State)
            {
                case 0://Idle状态，跟随玩家
                    {
                        if (Projectile.frame > 5)
                            Projectile.frame = 0;
                        Projectile.UpdateFrameNormally(4, 5);

                        if (Projectile.velocity.Length() < 1)
                            frameY = 0;
                        else
                            frameY = 2;
                    }
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle frame = new(Projectile.frame, frameY, 8, 3);
            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Projectile.QuickFrameDraw(frame, Color.White * 0.75f, 0, effect);
            Projectile.QuickFrameDraw(frame, Color.White * 0.75f, 0, effect);

            return false;
        }
    }

    public class PhantomSparkPhantomMagicBall
    {

    }
}
