using Coralite.Content.Items.Icicle;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.ParticleSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class QueensWreath : ModItem, IDashable, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public int Combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 18;
            Item.reuseDelay = 20;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAmmo = AmmoID.Arrow;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(320, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 18;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, source);

                if (Combo>=3)//射出能获得梦魇光能的箭矢
                {
                    Combo = 0;

                }
                else//就只是普通地射箭
                    for (int i = 0; i < 3; i++)
                        Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)), type, (int)(damage * 0.9f), knockback, player.whoAmI);

                Combo++;
            }
            return false;
        }

        public bool Dash(Player Player, int DashDir)
        {
            switch (DashDir)
            {
                default:
                case CoralitePlayer.DashLeft://向左右方向冲刺，直接冲
                case CoralitePlayer.DashRight:
                    break;
                case CoralitePlayer.DashUp://向上冲，需要消耗一个梦魇光能
                    break;
                case CoralitePlayer.DashDown://向下冲，需要消耗一个梦魇光能
                    break;
            }

            return true;
        }

    }

    /// <summary>
    /// ai0控制状态，0普通发射，1普通射能获得梦魇光能的，2强化射，3吊射
    /// ai2控制是否右键过
    /// </summary>
    public class QueensWreathHeldProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.NightmareItems + "QueensWreath";

        public ref float State => ref Projectile.ai[0];
        public ref float Rotation => ref Projectile.ai[1];
        public bool NotRightClicked => Projectile.ai[2] == 0;

        public ref float Alpha => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];

        public bool fadeIn = true;
        public bool Init = true;

        public int ShootTime;
        public int DelayTime;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Owner.heldProj = Projectile.whoAmI;
            if (Init)
            {
                ShootTime = Owner.itemTimeMax;
                DelayTime = NotRightClicked ? 20 : 0;
                switch (State)
                {
                    default:
                    case 0:

                        break;
                }

                Owner.itemTime = ShootTime + DelayTime;
                Init = false;
            }

            switch (State)
            {
                default: Projectile.Kill(); break;
                case 0: //普普通通的射箭
                    Projectile.Center = Owner.Center + Projectile.ai[0].ToRotationVector2() * 8;

                    //如果满足条件且没有右键过 那么就再次射击
                    if (Timer >= ShootTime && NotRightClicked 
                        && Main.mouseRight && Main.mouseRightRelease
                        &&Owner.TryGetModPlayer(out CoralitePlayer cp)&&cp.nightmareEnergy>0)
                    {
                        cp.nightmareEnergy -= 1;

                        Projectile.Kill();
                    }
                    break;
                case 1: //射出能够获得梦魇光能的
                    do
                    {
                        Lighting.AddLight(Owner.Center, Coralite.Instance.IcicleCyan.ToVector3() * Alpha);

                        if (Timer < 21)
                        {
                            Rotation += 0.3141f; //1/10 Pi

                            Owner.itemTime = Owner.itemAnimation = 2;
                            break;
                        }

                        if (Owner.controlUseItem)
                        {
                            if (Main.myPlayer == Projectile.owner)
                            {
                                Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;
                                Rotation = Rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.25f);
                                Projectile.netUpdate = true;

                                if (Main.rand.NextBool(20))
                                {
                                    Vector2 dir = Rotation.ToRotationVector2();
                                    Particle.NewParticle(Owner.Center + dir * 16 + Main.rand.NextVector2Circular(8, 8), dir * 1.2f, CoraliteContent.ParticleType<HorizontalStar>(), Coralite.Instance.IcicleCyan, Main.rand.NextFloat(0.1f, 0.15f));
                                }
                            }
                            Projectile.timeLeft = 2;
                            Owner.itemTime = Owner.itemAnimation = 2;
                        }
                        else
                        {
                            if (Main.myPlayer == Projectile.owner)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.One) * 9.5f, ModContent.ProjectileType<IcicleStarArrow>(), (int)(Projectile.damage * 1.5f), Projectile.knockBack, Projectile.owner);
                                SoundEngine.PlaySound(CoraliteSoundID.Bow_Item5, Owner.Center);
                            }

                            Projectile.Kill();
                        }

                    } while (false);

                    Projectile.rotation = Rotation;
                    Projectile.Center = Owner.Center + Rotation.ToRotationVector2() * 8;
                    break;
                case 2://横向冲刺
                case 3://向上冲刺
                case 4://向下冲刺
                    break;
            }

            Timer += 1f;
            Owner.itemRotation = Rotation + (OwnerDirection > 0 ? 0 : 3.141f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 center = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, mainTex.Size() / 2, 1.1f, OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically, 0f);

            if (Alpha > 0.001f)
            {
                Texture2D starTex = Request<Texture2D>(AssetDirectory.IcicleItems + "IcicleStarArrow").Value;
                float factor = Timer % 80 / 80;
                float num3 = Utils.Remap(factor, 0f, 0.6f, 0f, 1f) * Utils.Remap(factor, 0.6f, 1f, 1f, 0f);
                Vector2 dir = Rotation.ToRotationVector2();
                Main.spriteBatch.Draw(starTex, center + dir * 6, null, Color.White * Alpha, Projectile.rotation + 1.57f, starTex.Size() / 2, 1.4f, SpriteEffects.None, 0f);

                Helpers.ProjectilesHelper.DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, center + dir * 18, new Color(255, 255, 255, 0) * num3 * 0.5f, Coralite.Instance.IcicleCyan,
                    factor, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(1.3f, 1.3f), Vector2.One);
            }

            return false;
        }

    }

    /// <summary>
    /// ai0控制状态，0普通发射，1吊射
    /// ai1控制是否能获得梦魇光能，为1时可以
    /// </summary>
    public class QueensWreathArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public ref float State => ref Projectile.ai[0];
        public bool CanGetNightmareEnergy => Projectile.ai[1] == 1;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (CanGetNightmareEnergy && Main.player[Projectile.owner].TryGetModPlayer(out CoralitePlayer cp))
                cp.GetNightmareEnergy(1);
        }
    }
}
