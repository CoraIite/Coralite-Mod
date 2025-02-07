using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.ShadowCastle
{
    public class ShadowWave : ModItem
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + Name;

        public int ShootCount = 9;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.Handgun;
            ItemID.Sets.ShimmerTransformToItem[ItemID.Handgun] = Type;
        }

        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.shootSpeed = 12f;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.knockBack = 2;

            Item.autoReuse = false;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Orange;
            //Item.UseSound = CoraliteSoundID.Gun3_Item41;
            Item.useAmmo = AmmoID.Bullet;

            Item.shoot = ProjectileID.Bullet;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return ShootCount > 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == 2)//右键填充弹药
            {
                SoundEngine.PlaySound(CoraliteSoundID.AmmoBox_Item149, player.Center);
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), position, Vector2.Zero, ModContent.ProjectileType<ShadowWaveReLoad>(), damage, knockback, player.whoAmI);
                return false;
            }

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), position, Vector2.Zero, ModContent.ProjectileType<ShadowWaveHeldProj>(), 1, knockback, player.whoAmI);

            if (ShootCount < 1)//没弹药了射不出来
            {
                CombatText.NewText(new Rectangle((int)player.Center.X, (int)player.Center.Y, 1, 1), Color.White, "弹夹是空的！");
                return false;
            }

            SoundEngine.PlaySound(CoraliteSoundID.Gun3_Item41, player.Center);
            ShootCount--;
            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            position += Main.screenPosition;
            Main.instance.DrawHealthBar(position.X, position.Y + 12, ShootCount + 1, 10, 1, 1);
        }
    }

    public class ShadowWaveHeldProj : BaseGunHeldProj
    {
        public ShadowWaveHeldProj() : base(0.4f, 16, -8, AssetDirectory.ShadowCastleItems)
        { }

        public override void Initialize()
        {
            base.Initialize();
            float rotation = TargetRot + (DirSign > 0 ? 0 : MathHelper.Pi);
            Vector2 dir = rotation.ToRotationVector2();
            Vector2 center = Projectile.Center + (dir * 24);
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(center + Main.rand.NextVector2Circular(8, 8),
                    DustID.Shadowflame, dir.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(4f, 12f), Scale: Main.rand.NextFloat(0.8f, 1.2f));
                dust.noGravity = true;
            }
        }
    }

    public class ShadowWaveReLoad : BaseHeldProj
    {
        public override string Texture => AssetDirectory.ShadowCastleItems + "ShadowWave";

        ref float State => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = ToMouseA;
        }

        public override void AI()
        {
            Owner.itemTime = Owner.itemAnimation = 2;
            SetHeld();
            Owner.itemRotation = ToMouseA + (DirSign > 0 ? 0f : MathHelper.Pi) + (Owner.gravDir > 0 ? 0f : MathHelper.Pi) + (DirSign * 0.3f);
            Projectile.Center = Owner.Center + (UnitToMouseV * 20);
            Owner.direction = Main.MouseWorld.X > Owner.Center.X ? 1 : -1;

            switch (State)
            {
                default:
                case 0://转枪并填充弹药，如果之后一直按住那么就进入下一个阶段
                    {
                        if (Timer < 20)
                        {
                            Projectile.rotation += MathHelper.TwoPi / 10;
                        }
                        else
                        {
                            if (Owner.HeldItem.ModItem is ShadowWave sw)//填充弹药
                                sw.ShootCount = 9;

                            if (Main.mouseRight)
                            {
                                Timer = 0;
                                State++;
                                break;
                            }
                            else
                                Projectile.Kill();
                        }
                        Timer++;
                    }
                    break;
                case 1:
                    {
                        if (!Main.mouseRight)//清空弹夹！！
                        {
                            if (Owner.HeldItem.ModItem is ShadowWave sw)//填充弹药
                                sw.ShootCount = 3;

                            SoundEngine.PlaySound(CoraliteSoundID.Shotgun_Item36, Owner.Center);

                            if (Owner.PickAmmo(Owner.HeldItem, out int proj, out float speed, out int damage, out float knockBack, out _))
                                for (int i = 0; i < 6; i++)
                                    Projectile.NewProjectileFromThis(Owner.Center, UnitToMouseV.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)) * speed, proj,
                                    (int)(damage * 0.55f), knockBack);

                            State++;
                            Projectile.Kill();
                            Projectile.NewProjectileFromThis(Owner.Center, Vector2.Zero, ModContent.ProjectileType<ShadowWaveHeldProj>(),
                                1, 1);

                            Owner.itemTime = Owner.itemAnimation = (int)(Owner.itemTimeMax * 0.8f);
                        }
                        else
                            Projectile.timeLeft = 20;

                        Projectile.rotation = ToMouseA;
                    }
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            SpriteEffects effects = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = Projectile.rotation + (DirSign > 0 ? 0f : MathHelper.Pi);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, lightColor, rot, mainTex.Size() / 2, Projectile.scale, effects, 0f);

            return false;
        }
    }
}
