﻿using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Melee
{
    /// <summary>
    /// 唯一指定用调试物品，用来测试各种乱七八糟的东西，可能有时候忘了删除调试用代码
    /// </summary>
    public class CatClaws : ModItem
    {
        public override string Texture => AssetDirectory.Misc_Melee + Name;

        public override bool AltFunctionUse(Player Player) => true;

        public int comboNormal = 0;
        public int comboBoost = 0;
        public int count = 0;
        public bool rightClick = false;
        public bool canCount = false;
        public bool reinforced = false;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("");

            // Tooltip.SetDefault("");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 60;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.knockBack = 6;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Melee;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            Item.shoot = ProjectileType<CatClawsProj_Slash>();
        }

        public override void HoldItem(Player player)
        {
            if (!canCount)
                return;
            count++;
            if (count == 60)
            {
                reinforced = true;
                if (Main.netMode != NetmodeID.Server)
                    for (int i = 0; i < 8; i++)
                        Dust.NewDustPerfect(player.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(5, 5), 0, Color.Orange, 2f);
                SoundEngine.PlaySound(SoundID.Item4, player.Center);
            }

            if (count == 120)
            {
                if (Main.netMode != NetmodeID.Server)
                    for (int i = 0; i < 8; i++)
                        Dust.NewDustPerfect(player.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(5, 5), 0, Color.DarkRed, 2f);

                count = 0;
                reinforced = false;
                canCount = false;
            }

        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                rightClick = true;
            else
                rightClick = false;

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (rightClick)
            {
                Projectile.NewProjectile(source, position, velocity, type, (int)(damage * 1.5f), knockback, player.whoAmI, -1);
                reinforced = false;
                canCount = true;
                comboBoost = 0;
                count = 0;

                return false;
            }

            if (reinforced)
            {
                switch (comboBoost)
                {
                    default:
                    case 0:
                        Projectile.NewProjectile(source, position, velocity, type, (int)(damage * 1.5f), knockback, player.whoAmI, 4);

                        break;
                    case 1:
                        if (!Main.projectile.Any(n => n.active && n.type == ProjectileType<CatClawsProj_shield>() && n.owner == player.whoAmI))
                            Projectile.NewProjectile(source, position, velocity, ProjectileType<CatClawsProj_shield>(), (int)(damage * 1.3f), 3, player.whoAmI, 0, 2);

                        break;
                    case 2:
                        Projectile.NewProjectile(source, position, velocity, type, damage * 2, 3, player.whoAmI, 5);
                        comboBoost = 0;
                        canCount = false;
                        reinforced = false;
                        count = 0;
                        return false;
                }

                SoundEngine.PlaySound(SoundID.Item1, player.Center);
                comboBoost++;
                canCount = true;
                reinforced = false;
                count = 0;
                return false;
            }

            count = 0;
            canCount = false;
            switch (comboNormal)
            {
                default:
                case 0:
                case 1:
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, comboNormal);

                    break;
                case 2:
                    if (!Main.projectile.Any(n => n.active && n.type == ProjectileType<CatClawsProj_shield>() && n.owner == player.whoAmI))
                        Projectile.NewProjectile(source, position, velocity, ProjectileType<CatClawsProj_shield>(), damage, 3, player.whoAmI, 0, 0);

                    break;
                case 3:
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, comboNormal);
                    SoundEngine.PlaySound(SoundID.Item1, player.Center);
                    comboNormal = 0;
                    return false;
            }

            comboNormal++;
            SoundEngine.PlaySound(SoundID.Item1, player.Center);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.LicenseCat)
            .AddIngredient(ItemID.Ruby)
            .AddIngredient(ItemID.Wood, 20)
            .AddIngredient(ItemID.RichMahogany, 20)
            .AddIngredient(ItemID.Ebonwood, 20)
            .AddIngredient(ItemID.Shadewood, 20)
            .AddIngredient(ItemID.Pearlwood, 20)
            .AddIngredient(ItemID.BorealWood, 20)
            .AddIngredient(ItemID.PalmWood, 20)
            .AddIngredient(ItemID.SpookyWood, 20)
            .AddIngredient(ItemID.DynastyWood, 20)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }

    /// <summary>
    /// 右键 -1
    /// 左键不强化 0，1，3
    /// 左键强化4，5
    /// </summary>
    public class CatClawsProj_Slash : BaseSwingProj
    {
        public override string Texture => AssetDirectory.Projectiles_Melee + "CatClawsProj_3";
        public ref float Combo => ref Projectile.ai[0];

        private byte tripleCombo = 0;

        public CatClawsProj_Slash() : base(1.57f, trailCount: 30) { }

        public override void SetDefs()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.localNPCHitCooldown = 30;
            Projectile.width = 22;
            Projectile.height = 62;
            distanceToOwner = 10;
            minTime = 0;
            TrailTexture = AssetDirectory.VFX + "ClawSlash3aYellow";
            onHitFreeze = 20;
        }

        protected override void Initializer()
        {
            switch (Combo)
            {
                case -1://右键
                    maxTime = 45;
                    startAngle = 1.9f;
                    totalAngle = 3.8f;
                    useSlashTrail = true;
                    trailBottomWidth = 40;
                    Projectile.extraUpdates = 2;
                    break;

                case 0://普通挥舞0：上至下小幅度
                    maxTime = 45;
                    startAngle = 1.3f;
                    totalAngle = 2f;
                    Projectile.extraUpdates = 2;
                    break;

                case 1://普通挥舞1：下至上小幅度
                    maxTime = 45;
                    startAngle = -1.3f;
                    totalAngle = -2f;
                    Projectile.extraUpdates = 2;
                    break;

                case 3://普通挥舞2：上至下大幅度
                    maxTime = 60;
                    startAngle = 1.8f;
                    totalAngle = 3.5f;
                    Projectile.extraUpdates = 2;
                    break;

                case 4://强化攻击0：三连击
                    maxTime = 26;
                    startAngle = 1f;
                    totalAngle = 1.4f;
                    useSlashTrail = true;
                    Projectile.extraUpdates = 1;
                    break;

                case 5://强化攻击1：咸鱼突刺
                    maxTime = 0;
                    break;

                default: goto case 0;
            }

            base.Initializer();
        }

        protected override void AfterSlash()
        {
            switch (Combo)
            {
                default:
                case -1:
                case 0:
                case 1:
                case 3:
                    Projectile.Kill();
                    break;

                case 4:
                    if (tripleCombo == 2)
                        Projectile.Kill();

                    tripleCombo++;
                    startAngle *= -1;
                    Timer = 0;
                    onStart = true;
                    break;

                case 5:
                    if ((int)Timer <= 2)
                    {
                        if (Owner.whoAmI == Main.myPlayer)
                        {
                            _Rotation = (Main.MouseWorld - Owner.Center).ToRotation();
                            Owner.velocity += Vector2.Normalize(Main.MouseWorld - Owner.Center) * 7;
                        }
                    }
                    else if ((int)Timer < 30)
                        Slasher();
                    else if ((int)Timer > 30)
                        Projectile.Kill();

                    break;
            }
        }

        protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (VisualEffectSystem.HitEffect_SpecialParticles)
            {
                BasePRT particle = PRTLoader.NewParticle(Vector2.Lerp(Projectile.Center, target.Center, 0.5f), Vector2.Zero, CoraliteContent.ParticleType<Strike>(), Color.Orange, 1f);
                particle.Rotation = _Rotation + 2.2f + Main.rand.NextFloat(-0.5f, 0.5f);
            }
        }
    }

    /// <summary>
    /// 飞盾攻击，无拖尾
    /// </summary>
    public class CatClawsProj_shield : ProjStateMachine
    {
        public override string Texture => AssetDirectory.Projectiles_Melee + "CatClawsProj_4";

        public ref float Reinforced => ref Projectile.ai[1];

        public sealed override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;

            Projectile.timeLeft = 600;
        }

        public override void AIBefore()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                if (Main.rand.NextBool(3) && Reinforced == 2)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, null, 0, Color.Orange, 2f);
                    dust.noGravity = true;
                }
            }
        }

        public class ThrowState : ProjState
        {
            public override void AI(ProjStateMachine proj)
            {
                Projectile Projectile = proj.Projectile;
                Projectile.rotation += 0.3f;
                float timer = 600 - Projectile.timeLeft;
                if (timer == 0)
                {
                    Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Main.player[Projectile.owner].Center) * 15f;
                }
                float factor = timer / 30;
                Projectile.velocity *= 1 - (factor * factor);
                if (timer == 30)
                    proj.SetState<BackState>();
            }
        }

        public class BackState : ProjState
        {
            public override void AI(ProjStateMachine proj)
            {
                Projectile Projectile = proj.Projectile;
                Projectile.velocity = Vector2.Normalize(Main.player[Projectile.owner].Center - Projectile.Center) * 15f;
                Projectile.rotation += 0.3f;
                if ((Main.player[Projectile.owner].Center - Projectile.Center).Length() < 15f)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void Initialize()
        {
            RegisterState(new ThrowState());
            RegisterState(new BackState());
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var tex = Request<Texture2D>(Texture).Value;
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tex.Width / 2, tex.Height / 2), 1, 0, 0);

            return false;
        }
    }
}
