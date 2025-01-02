using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class FullMoon : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(44, 6f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 21, 4f);

            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 3);

            Item.noUseGraphic = true;
            Item.useTurn = false;
            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddDash(this);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item)
                , player.Center, Vector2.Zero, ProjectileType<FullMoonHeldProj>(), damage, knockback, player.whoAmI, rot);

            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.ShadowFlameBow)
                .AddIngredient(ItemID.DemonBow)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.ShadowFlameBow)
                .AddIngredient(ItemID.TendonBow)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        break;
                    }
                default:
                    return false;
            }

            int mouseDir = Main.MouseWorld.X > Player.Center.X ? 1 : -1;

            if (mouseDir>0)
            {
                if (dashDirection > 0)
                    newVelocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero);
                else
                    newVelocity = new Vector2(-1, 0);
            }
            else
            {
                if (dashDirection > 0)
                    newVelocity = new Vector2(1, 0);
                else
                    newVelocity = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.Zero);
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 70;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 15;
            Player.velocity = newVelocity;
            //Player.direction = (int)dashDirection;
            Player.AddImmuneTime(ImmunityCooldownID.General, 14);
            Player.immune = true;

            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 15));

            if (Player.whoAmI == Main.myPlayer)
            {
                //Helper.PlayPitched("Misc/HallowDash", 0.4f, -0.2f, Player.Center);
                Helper.PlayPitched(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<FullMoonHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }



                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, new Vector2(dashDirection, 0), ProjectileType<FullMoonHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, newVelocity.ToRotation(), 1, 15);
            }

            return true;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.ThyphionSeriesItems)]
    public class FullMoonHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + nameof(FullMoon);

        public ref float DashState => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public float handOffset;

        public override int GetItemType()
            => ItemType<FullMoon>();

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

        #region 冲刺

        public override void DashAttackAI()
        {
            switch (DashState)
            {
                default:
                case 0://朝向目标方向冲刺，并检测碰撞
                    if (Timer < DashTime + 2)
                    {
                        Rotation = RecordAngle;
                        Vector2 dir = Rotation.ToRotationVector2();

                        Owner.velocity = dir * 10;

                        if (Dashing_CheckCollide())
                        {
                            DashState = 1;
                            Timer = 0;
                            Owner.velocity = -dir * 10;
                            return;
                        }
                    }
                    else
                        Projectile.Kill();

                    break;
                case 1://与目标产生碰撞，向后反射并旋转弓
                    const int maxTime = 20;
                    Rotation += 1f / maxTime * MathHelper.TwoPi;

                    if (Timer>maxTime)
                    {
                        DashState = 2;
                        Timer = 0;

                        if (Projectile.IsOwnedByLocalPlayer())//生成弹幕
                        {

                        }
                        return;
                    }

                    break;
                case 2://射击阶段



                    Projectile.Kill();
                    break;
            }

            Projectile.rotation = Rotation;
            Timer++;
        }

        public bool Dashing_CheckCollide()
        {
            Rectangle rect = GetDashRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.IsActiveAndHostile() || proj.whoAmI == Projectile.whoAmI)
                    continue;

                if (proj.Colliding(proj.getRect(), rect))
                    return true;
            }

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                if (!npc.active || npc.friendly || npc.dontTakeDamage)
                    continue;

                if (Projectile.Colliding(rect, npc.getRect()))
                {
                    JustCollideNPC(npc);
                    return true;
                }
            }

            return false;
        }

        public Rectangle GetDashRect()
        {
            return Utils.CenteredRectangle(Projectile.Center, new Vector2(48, 48));
        }

        public void JustCollideNPC(NPC target)
        {
            Helper.PlayPitched(CoraliteSoundID.Ding_Item4, Projectile.Center,pitchAdjust:-0.3f);

            if (target.CanBeChasedBy())//踢一脚
                target.SimpleStrikeNPC(Owner.GetWeaponDamage(Owner.HeldItem), Owner.direction, knockBack: 10, damageType: DamageClass.Ranged);

            if (!VisualEffectSystem.HitEffect_SpecialParticles)
                return;

        }



        #endregion

        public override Vector2 GetOffset()
            => new Vector2(20 + handOffset, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1, DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            return false;
        }
    }

    public class FullMoonStrike:BaseHeldProj,IDrawNonPremultiplied,IPostDrawAdditive
    {
        public override string Texture => AssetDirectory.Blank;

        public override void AI()
        {
            if (!Projectile.ai[0].GetProjectileOwner(out Projectile owner, () => Projectile.Kill()))
                return;


        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
        }

        public void DrawNonPremultiplied(SpriteBatch spriteBatch)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = CoraliteAssets.Trail.BoosterASP.Value;



            return base.PreDraw(ref lightColor);
        }
    }
}
