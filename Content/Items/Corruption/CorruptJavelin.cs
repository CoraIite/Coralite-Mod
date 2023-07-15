using Coralite.Content.Items.Shadow;
using Coralite.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Corruption
{
    public class CorruptJavelin : ModItem
    {
        public override string Texture => AssetDirectory.CorruptionItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 23;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = 3;

            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<CorruptJavelinProj>();

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer != player.whoAmI)
                return false;

            if (player.altFunctionUse == ItemAlternativeFunctionID.ActivatedAndUsed)
            {
                //检测扎在NPC身上的投矛的数量，并让投矛消失，之后射出2倍数量的小投矛
            }

            var projectile = Projectile.NewProjectileDirect(source, player.Center + new Vector2(player.direction * Main.rand.Next(24, 32), -64 + Main.rand.Next(8, 8)),
                Vector2.Zero, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;


            return false;
        }
    }

    public class CorruptJavelinProj : ModProjectile
    {
        public override string Texture => AssetDirectory.CorruptionItems + "CorruptJavelin";

        public const int MaxTimeLeft = 600;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.localAI[0];

        private Vector2 offset;

        private float alpha;
        private float shadowScale;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 0;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.alpha = 0;
            Projectile.timeLeft = MaxTimeLeft;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 20;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            //状态0：在手中短暂蓄力，有一个短暂的出场动画
            //状态1：射出
            //状态2：扎在NPC身上时
            //状态3：死亡时，向后退并消失

            switch ((int)State)
            {
                default:
                case 0:
                    do
                    {
                        if (Timer < 1 && Main.myPlayer == Projectile.owner)
                        {
                            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.3f;
                            alpha = 0;
                            shadowScale = 1.4f;
                        }

                        if (Timer < 12)
                        {
                            alpha += 1 / 12f;
                            shadowScale -= 0.4f / 12;
                            Projectile.timeLeft = MaxTimeLeft;
                            Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Projectile.Center).ToRotation(), 0.1f);

                            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                                DustID.CorruptSpray, Projectile.velocity * Main.rand.NextFloat(6f, 9f), Scale: Main.rand.NextFloat(1f, 1.5f));
                            dust.noGravity = true;
                        }

                        State = 1;
                        Timer = 0;
                        Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 16f;

                    } while (false);

                    Timer++;
                    break;
                case 1:
                    //仅仅是生成粒子而已
                    int type = Main.rand.Next(4) switch
                    {
                        0 => DustID.Shadowflame,
                        _=>DustID.CorruptSpray;
                    };
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                        type, Projectile.velocity * Main.rand.NextFloat(6f, 9f), Scale: Main.rand.NextFloat(1f, 1.5f));
                    dust.noGravity = true;

                    break;
                case 2:

                    break;
                case 3:

                    break;
            }
        }

        public override void Kill(int timeLeft)
        {
            Vector2 dir = (Projectile.rotation + MathHelper.Pi).ToRotationVector2();
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(24, 24),
                    DustID.CorruptSpray, dir* Main.rand.NextFloat(6f, 9f), Scale: Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            var origin = new Vector2(3 * mainTex.Width / 4, mainTex.Height / 4);
            float Rot = Projectile.rotation + MathHelper.Pi / 4;
            SpriteEffects effect = SpriteEffects.None;

            if (Projectile.direction < 0)
            {
                Rot += MathHelper.Pi;
                effect = SpriteEffects.FlipHorizontally;
            }

            if ((int)State == 1)//残影绘制
            {

            }
            else if ((int)State == 0)
                Main.spriteBatch.Draw(mainTex, pos, null, Color.White * 0.5f, Rot, origin, shadowScale, effect, 0);

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor * alpha, Rot, origin, Projectile.scale, effect, 0);
            return false;
        }
    }

    public class CorruptJavelinSpecial : ModProjectile
    {
        public override string Texture => AssetDirectory.CorruptionItems + "CorruptJavelin";

        public override void AI()
        {
        }

    }

    public class SmallCorruptJavelin : ModProjectile
    {
        public override string Texture => AssetDirectory.CorruptionItems + Name;

        public override void AI()
        {
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = TextureAssets.Projectile[Type].Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;
            var origin = new Vector2(3 * mainTex.Width / 4, mainTex.Height / 4);
            float Rot = Projectile.rotation + MathHelper.Pi / 4;
            SpriteEffects effect = SpriteEffects.None;

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor , Rot, origin, Projectile.scale, effect, 0);
            return false;
        }

    }
}
