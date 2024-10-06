using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Thyphion : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public int shootCount;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(235, 6f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 20, 19f,true);

            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 35);

            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();

            switch (shootCount)
            {
                default://普普通通连射
                case 0:
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

                    Helper.PlayPitched("Misc/Do", 0.7f, 0, player.Center);
                    Helper.PlayPitched("Misc/Arrow", 0.4f, 0, player.Center);
                    Helper.PlayPitched(CoraliteSoundID.Bow_Item5, player.Center);
                    break;
                case 1:
                    Projectile.NewProjectile(source, position, velocity, ProjectileType<ThyphionTagProj>()
                        , damage, knockback, player.whoAmI,1,type,velocity.Length());

                    Helper.PlayPitched("Misc/Do", 0.7f, 4 / 13f, player.Center);
                    Helper.PlayPitched("Misc/Arrow", 0.4f, 0, player.Center);
                    Helper.PlayPitched(CoraliteSoundID.Bow_Item5, player.Center);
                    break;
                case 2:
                    Projectile.NewProjectile(source, position, velocity, ProjectileType<ThyphionTagProj>()
                        , damage, knockback, player.whoAmI, 2, type, velocity.Length());

                    Helper.PlayPitched("Misc/Do", 0.7f, 7 / 13f, player.Center);
                    Helper.PlayPitched("Misc/Arrow", 0.4f, 0, player.Center);
                    Helper.PlayPitched(CoraliteSoundID.Bow_Item5, player.Center);
                    break;
                case 3://射出贯穿箭
                    {
                        Helper.PlayPitched("Misc/Do", 0.7f, 1f, player.Center);
                        Helper.PlayPitched("Misc/Arrow", 0.4f, 0.1f, player.Center);
                        Helper.PlayPitched("Misc/EnergyBurst", 0.2f, 0.3f, player.Center);
                        Helper.PlayPitched(CoraliteSoundID.StrongWinds_Item66, player.Center,volume:0.4f);
                    }
                    break;
            }

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero
                , ProjectileType<ThyphionHeldProj>(), damage, knockback, player.whoAmI, rot, 0);

            shootCount++;

            if (shootCount > 3)
                shootCount = 0;

            return false;
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
                        newVelocity.X = dashDirection * 11;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 100;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.immune = true;
            Player.AddImmuneTime(ImmunityCooldownID.General, 20);

            Player.velocity = newVelocity;
            Player.direction = (int)dashDirection;

            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 15));

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<RadiantSunHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<RadiantSunHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f - dashDirection * 0.3f, 1, 20);
            }

            return true;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage = new StatModifier(1 + (damage.Additive - 1) / 10, 1);
        }
    }

    public class ThyphionTagProj:BaseHeldProj
    {
        public override string Texture => AssetDirectory.Blank;

        public ref float Timer => ref Projectile.localAI[0];
        public ref float State  =>ref Projectile.ai[0];
        public ref float ArrowType  =>ref Projectile.ai[1];
        public ref float Speed  =>ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Owner.HeldItem.type != ItemType<Thyphion>())
                Projectile.Kill();

            switch (State)
            {
                default:
                case 1:
                    {
                        if (Timer < Owner.itemTimeMax / 3-1 && Timer % (Owner.itemTimeMax / 9) == 0)
                        {
                            Vector2 dir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One);

                            Projectile.NewProjectileFromThis(Owner.Center+Main.rand.NextVector2Circular(20,20), dir * Speed, (int)ArrowType,
                                Projectile.damage, Projectile.knockBack);
                        }
                    }
                    break;
                case 2:
                    {
                        if (Timer < Owner.itemTimeMax / 2 - 1 && Timer % (Owner.itemTimeMax / 10) == 0)
                        {
                            Vector2 dir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.One);

                            Projectile.NewProjectileFromThis(Owner.Center + Main.rand.NextVector2Circular(20, 20), dir * Speed, (int)ArrowType,
                                Projectile.damage, Projectile.knockBack);
                        }
                    }
                    break;
            }

            Timer++;
            if (Timer > Owner.itemTimeMax)
                Projectile.Kill();
        }

        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor) => false;
    }

    public class ThyphionHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "Thyphion";

        public float handOffset = 34;

        private static Asset<Texture2D> GlowTex;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            GlowTex = Request<Texture2D>(AssetDirectory.ThyphionSeriesItems + "Thyphion_glow");
        }

        public override void Unload()
        {
            GlowTex = null;
        }

        public override int GetItemType()
            => ItemType<Thyphion>();

        public override Vector2 GetOffset()
            => new(handOffset, -12);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            var effect = OwnerDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1, effect, 0f);
            Main.spriteBatch.Draw(GlowTex.Value, center, null, Color.White, Projectile.rotation, origin, 1, effect, 0f);

            if (Special == 0)
                return false;

            return false;
        }
    }

    public class ThyphionArrow
    {

    }

    public class ThyphionPhantomArrow
    {
        public enum BowType
        {
            /// <summary> 木弓 </summary>
            WoodenBow = 0,
            /// <summary> 灰烬木弓 </summary>
            AshWoodBow,
            /// <summary> 云杉木弓 </summary>
            BorealWoodBow,
            /// <summary> 棕榈木弓 </summary>
            PalmWoodBow,
            /// <summary> 红木弓 </summary>
            RichMahoganyBow,
            /// <summary> 乌木弓 </summary>
            EbonwoodBow,
            /// <summary> 暗影木弓 </summary>
            ShadewoodBow,
            /// <summary> 珍珠木弓 </summary>
            PearlwoodBow,

            /// <summary> 晚霞 </summary>
            AfterGlow,
            /// <summary> 遥远青空 </summary>
            FarAwaySky,
            /// <summary> 冰雹 </summary>
            Hail,
            /// <summary> 熔火之怒 </summary>
            MoltenFury,
            /// <summary> 地狱蝙蝠弓 </summary>
            HellwingBow,
            /// <summary> 旭日 </summary>
            RadiantSun,

            /// <summary> 逆闪电 </summary>
            ReversedFlash,
            End
        }

        public struct ArrowData
        {
            public int ItemType;

            public Color ArrowColor;
        }

        public static ArrowData GetArrowData(BowType type)
        {
            return default;
        }

    }
}
