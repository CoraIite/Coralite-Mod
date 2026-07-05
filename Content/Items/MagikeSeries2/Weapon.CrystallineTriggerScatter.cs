using Coralite.Content.DamageClasses;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineTriggerScatter : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.DefaultToRangedWeapon(ModContent.ProjectileType<CrystallineSentinelBullet2>(), AmmoID.Bullet, 28, 12f, true);
            Item.DamageType = MagikeDamage.Instance;
            Item.SetWeaponValues(45, 12f, 0);
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 4);

            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = CoraliteSoundID.Gun3_Item41;

            Item.useTurn = false;
            Item.noUseGraphic = true;

            Item.GetMagikeItem().MagikeMax = 7500;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int sp = 0;
            int count = 2;
            if (MagikeHelper.TryCosumeMagike(15, Item, player))
            {
                sp = 1;

                type = ModContent.ProjectileType<CrystallineSentinelBullet2>();
                damage = (int)(damage * 1.35f);
                count = 4;

                Helper.PlayPitched(CoraliteSoundID.Crystal_Item101, position, pitch: 0.5f);
            }

            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ModContent.ProjectileType<CrystallineTriggerScatterHeldProj>(), 0, knockback, player.whoAmI, ai2: sp);

            for (int i = 0; i < count; i++)
            {
                Projectile.NewProjectile(source, position, velocity.RotateByRandom(-0.1f, 0.1f) * Main.rand.NextFloat(0.8f, 1.15f), type, damage, knockback, player.whoAmI);
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }
    }

    [VaultLoaden(AssetDirectory.MagikeSeries2Item)]
    public class CrystallineTriggerScatterHeldProj : BaseGunHeldProj
    {
        public CrystallineTriggerScatterHeldProj() : base(0.2f, 24, -8, AssetDirectory.MagikeSeries2Item) { }

        public static ATex CrystallineTriggerPreciseFire { get; private set; }
        public static ATex CrystallineTriggerPreciseFire2 { get; private set; }
        public ref float SpAttack => ref Projectile.ai[2];

        public override void ModifyAI(float factor)
        {
            Lighting.AddLight(Projectile.Center, Coralite.CrystallinePurple.ToVector3() * 0.4f);
            if (Projectile.timeLeft != MaxTime && Projectile.timeLeft % 2 == 0)
                Projectile.frame++;

            if (Projectile.timeLeft == MaxTime)
            {
                float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
                float n = rot - DirSign * MathHelper.PiOver2;

                Vector2 pos = Projectile.Center + rot.ToRotationVector2() * 50 + n.ToRotationVector2() * (SpAttack == 0 ? 8 : -4);

                for (int i = 0; i < 6; i++)
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<CrystallineTriggerDust>(), (rot + Main.rand.NextFloat(-0.6f, 0.6f)).ToRotationVector2() * Main.rand.NextFloat(1, 5f), newColor: Color.White, Scale: Main.rand.NextFloat(1, 1.5f));
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);

            if (Projectile.frame > 3)
                return false;

            Texture2D effect = SpAttack == 0 ? CrystallineTriggerPreciseFire.Value : CrystallineTriggerPreciseFire2.Value;

            Rectangle frameBox = effect.Frame(1, 4, 0, Projectile.frame);

            float rot = Projectile.rotation + (DirSign > 0 ? 0 : MathHelper.Pi);
            float n = rot - DirSign * MathHelper.PiOver2;

            Main.spriteBatch.Draw(effect, Projectile.Center + rot.ToRotationVector2() * 46 + n.ToRotationVector2() * (SpAttack == 0 ? 8 : -4) - Main.screenPosition, frameBox, Color.Lerp(lightColor, Color.White, 0.5f)
                , rot, new Vector2(0, frameBox.Height / 2), Projectile.scale, 0, 0f);
            return false;
        }
    }
}
