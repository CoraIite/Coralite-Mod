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
            Item.useAmmo = AmmoID.Arrow;
            Item.damage = 1000;
            Item.shootSpeed = 7f;
            Item.knockBack = 0;
            Item.shoot = ProjectileID.PurificationPowder;

            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Red;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = Item.sellPrice(0, 35);

            Item.useTurn = false;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.channel = true;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            switch (shootCount)
            {
                default://普普通通连射
                    {

                    }
                    break;
                case 3://射出贯穿箭
                    break;
            }

            shootCount++;

            if (shootCount > 4)
            {
                shootCount = 0;
            }

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

    public class ThyphionHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "Thyphion";

        public float handOffset = 14;

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
