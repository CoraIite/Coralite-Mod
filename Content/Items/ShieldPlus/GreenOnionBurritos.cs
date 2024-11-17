using Coralite.Content.GlobalItems;
using Coralite.Content.Items.Glistent;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Systems.FlyingShieldSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.ShieldPlus
{
    /*
     * 左键甩葱，大葱碰到盾或者飞盾后变为卷饼重新射出
     */
    public class GreenOnionBurritos()
        : BaseShieldPlusWeapon<GreenOnionBurritosGuard>(Item.sellPrice(0, 1), ItemRarityID.LightRed, AssetDirectory.ShieldPlusItems)
        , IFlyingShieldAccessory
    {
        public override int FSProjType => ModContent.ProjectileType<GreenOnionBurritosFSProj>();

        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.useTime = Item.useAnimation = 10;
            Item.shoot = ModContent.ProjectileType<SpringOnion>();
            Item.shootSpeed = 10f;
            Item.knockBack = 3f;
            Item.damage = 36;

            Item.UseSound = null;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.UseSound = CoraliteSoundID.Swing_Item1;

            CoraliteGlobalItem.SetEdibleDamage(Item);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.useSpecialAttack)//特殊攻击键丢飞盾
                {
                    int max = cp.MaxFlyingShield;
                    if (player.ownedProjectileCounts[FSProjType] >= max)
                        return false;

                    return true;
                }

                if (player.altFunctionUse == 2 || Main.mouseRight)//右键了
                {
                    if (cp.FlyingShieldGuardIndex != -1)
                        return false;

                    if (player.ownedProjectileCounts[FSProjType] > 0)//如果右键时有左键弹幕
                    {
                        if (cp.FlyingShieldLRMeantime)//如果能同时使用
                            return true;
                        return false;
                    }

                    return true;//右键时没有左键弹幕
                }

                int max2 = cp.MaxFlyingShield;
                if (player.ownedProjectileCounts[Item.shoot] >= max2)
                    return false;

                return true;
            }

            return true;
        }

        #region 攻击相关

        public override void LeftAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);
        }

        public override void ShootFlyingShield(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 velocity, int type, int damage, float knockback)
        {
            damage = (int)(damage * 0.75f);
            SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, player.Center);
            base.ShootFlyingShield(player, source, velocity, type, damage, knockback);
        }

        public override void RightShoot(Player player, IEntitySource source, int damage)
        {
            damage = (int)(damage * 0.7f);
            base.RightShoot(player, source, damage);
        }

        #endregion

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 35)
                .AddIngredient<GlistentBar>(4)
                .AddIngredient(ItemID.PlatinumBar, 2)
                .AddIngredient(ItemID.MythrilBar, 2)
                .AddTile(TileID.LivingLoom)
                .Register();
        }
    }

    public class GreenOnionBurritosFSProj : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.ShieldPlusItems + "Pancake";

        public bool hited;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            flyingTime = 16 * 2;
            backTime = 6 * 2;
            backSpeed = 14;
            trailCachesLength = 9;
            trailWidth = 25 / 2;
            Projectile.extraUpdates = 1;
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.2f;
            SpawnDusts();
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.2f;
            SpawnDusts();
        }

        public void SpawnDusts()
        {
            if (Timer % 4 == 0)
                for (int i = 0; i < 2; i++)
                {
                    Projectile.SpawnTrailDust((float)(Projectile.width / 2), DustID.AncientLight, Main.rand.NextFloat(0.2f, 0.5f),
                        newColor: Color.DarkOrange, Scale: Main.rand.NextFloat(0.5f, 1f));
                }
        }

        public override Color GetColor(float factor)
        {
            return new Color() * factor;
        }
    }

    public class GreenOnionBurritosGuard : BaseFlyingShieldPlusGuard
    {
        public override string Texture => AssetDirectory.ShieldPlusItems + "Pancake";

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 42;
            Projectile.height = 42;
        }

        public override void SetOtherValues()
        {
            scalePercent = 2f;
            distanceAdder = 2;
            delayTime = 10;
        }

        public override void LimitFields()
        {
            base.LimitFields();
        }

        public override void OnGuard()
        {
            DistanceToOwner /= 2;
            Helper.PlayPitched("TheLegendOfZelda/Guard_Wood_Wood_" + Main.rand.Next(4), 0.5f, 0.2f, Projectile.Center);
            SpawnWoodDust();
        }

        public void SpawnWoodDust()
        {
            for (int i = 0; i < 8; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width / 2, Projectile.height / 2), DustID.WoodFurniture,
                    (Projectile.rotation + Main.rand.NextFloat(-0.3f, 0.3f)).ToRotationVector2() * Main.rand.NextFloat(2, 6)
                    , Scale: 1);

                d.noGravity = true;
            }
        }

        public override float GetWidth()
        {
            return Projectile.width * 0.5f / Projectile.scale;
        }
    }

    public class SpringOnion : BaseFlyingShield
    {
        public override string Texture => AssetDirectory.ShieldPlusItems + Name;

        public static Asset<Texture2D> EXTex;
        public bool Special;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                EXTex = ModContent.Request<Texture2D>(AssetDirectory.ShieldPlusItems + "GreenOnionBurritosProj");
            }
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                EXTex = null;
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 50;
        }

        public override void SetOtherValues()
        {
            flyingTime = 18 * 2;
            backTime = 15 * 2;
            backSpeed = 12;
            trailCachesLength = 9;
            trailWidth = 25 / 2;
            Projectile.extraUpdates = 1;
        }

        public override void OnShootDusts()
        {
            extraRotation += 0.15f;
            SpawnDusts();

            if (Special)
                return;

            Rectangle rect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.hostile || proj.whoAmI == Projectile.whoAmI || proj.type != ModContent.ProjectileType<GreenOnionBurritosFSProj>())
                    continue;

                if (Projectile.Colliding(rect, proj.getRect()))
                {
                    proj.Kill();
                    Timer = flyingTime;
                    Special = true;

                    Projectile.damage = (int)(Projectile.damage * 1.5f);
                    SoundEngine.PlaySound(CoraliteSoundID.Fleshy2_NPCHit7);
                }
            }
        }

        public override void OnBackDusts()
        {
            extraRotation += 0.15f;
            SpawnDusts();

            if (Special)
                return;

            Rectangle rect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.hostile || proj.whoAmI == Projectile.whoAmI)
                    continue;

                if (proj.type == ModContent.ProjectileType<GreenOnionBurritosFSProj>() && Projectile.Colliding(rect, proj.getRect()))
                {
                    proj.Kill();
                    Timer = flyingTime;
                    Special = true;
                    State = (int)FlyingShieldStates.Shooting;

                    Projectile.damage = (int)(Projectile.damage * 4.5f);
                    Projectile.velocity = proj.velocity;
                    SoundEngine.PlaySound(CoraliteSoundID.Fleshy2_NPCHit7);
                }

                if (proj.type == ModContent.ProjectileType<GreenOnionBurritosGuard>() && Projectile.Colliding(rect, proj.getRect()))
                {
                    proj.Kill();
                    Timer = flyingTime;
                    Special = true;
                    State = (int)FlyingShieldStates.Shooting;

                    Projectile.damage = (int)(Projectile.damage * 4.5f);
                    Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * shootSpeed;
                    SoundEngine.PlaySound(CoraliteSoundID.Fleshy2_NPCHit7);
                }
            }
        }

        public void SpawnDusts()
        {
            if (Timer % 4 == 0)
                for (int i = 0; i < 2; i++)
                {
                    Projectile.SpawnTrailDust((float)(Projectile.width / 2), DustID.Grass, Main.rand.NextFloat(0.2f, 0.5f)
                        , Scale: Main.rand.NextFloat(0.7f, 1f));
                }

            if (Special)
            {
                if (Timer % 3 == 0)
                    Projectile.SpawnTrailDust((float)(Projectile.width / 2), DustID.AncientLight, Main.rand.NextFloat(0.2f, 0.5f),
                    newColor: Color.DarkOrange, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
        }

        public override Color GetColor(float factor)
            => Color.Transparent;

        public override void DrawSelf(Color lightColor)
        {
            Texture2D mainTex = Special ? EXTex.Value : Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            Vector2 origin = mainTex.Size() / 2;

            for (int i = trailCachesLength - 1; i > 2; i--)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                lightColor * 0.4f * ((i - 2) * 1 / 6f), -0.2f * (i - 8) + extraRotation, origin, Projectile.scale, 0, 0);

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, extraRotation, origin, Projectile.scale, 0, 0);
        }
    }
}
