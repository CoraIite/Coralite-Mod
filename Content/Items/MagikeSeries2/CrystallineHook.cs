using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.NPCs.Crystalline;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class CrystallineHook : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.shootSpeed = 13.5f; // This defines how quickly the hook is shot.
            Item.shoot = ModContent.ProjectileType<CrystallineHookProjectile>(); // Makes the item shoot the hook's projectile when used.
            Item.rare = ModContent.RarityType<CrystallineMagikeRarity>();
            Item.value = Item.sellPrice(0, 2);
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<CrystallineSeaOats, CrystallineHook>(MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 12, 60 * 3), 12)
                .AddIngredient<MagicCrystalHook>()
                .AddIngredient<CrystallineMagike>(3)
                .Register();
        }
    }

    public class CrystallineHookProjectile : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public bool Hit = false;
        public ref float Timer => ref Projectile.localAI[2];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.SingleGrappleHook[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.GemHookAmethyst);
        }

        public override bool? CanUseGrapple(Player player)
        {
            for (int l = 0; l < 1000; l++)
                if (Main.projectile[l].active && Main.projectile[l].owner == Main.myPlayer && Main.projectile[l].type == Projectile.type)
                    return false;

            return null;
        }

        // Amethyst Hook is 300, Static Hook is 600.
        public override float GrappleRange()
        {
            return 460f;
        }

        public override void NumGrappleHooks(Player player, ref int numHooks)
        {
            numHooks = 1;
        }

        // default is 11, Lunar is 24
        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 18f;
        }

        public override void GrapplePullSpeed(Player player, ref float speed)
        {
            speed = 12f; // How fast you get pulled to the grappling hook projectile's landing position
        }

        public override bool PreAI()
        {
            Hit = Projectile.ai[0] != 2;
            return true;
        }

        public override void PostAI()
        {
            if (Projectile.ai[0] == 0)//射出的时候，生成特效
            {
                if (Timer % 4 == 0)
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<CrystallineHookDust>()
                        , Projectile.velocity.SafeNormalize(Vector2.Zero).RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(1, 2));
            }
            else if (Projectile.ai[0] == 2)
            {
                if (Hit)//刚刚抓到物块，生成特效
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(22, 22), ModContent.DustType<CrystallineHookDust>()
                            , Helper.NextVec2Dir(3, 6), Scale: 0.8f);

                        Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2CircularEdge(22, 22), ModContent.DustType<CrystallineImpact>()
                             , Helper.NextVec2Dir(3, 6));
                        d.rotation = (d.position - Projectile.Center).ToRotation();
                    }

                    Helper.PlayPitched(CoraliteSoundID.CrystalHit_DD2_CrystalCartImpact, Projectile.Center);
                }

                if (Vector2.Distance(Projectile.Center, Main.player[Projectile.owner].Center) < 16 * 8)
                {
                    //生成特效
                    Vector2 dir = (Projectile.rotation + 1.57f).ToRotationVector2();
                    for (int i = 0; i < 4; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center + i * dir * 28, ModContent.DustType<CrystallineHookDust>()
                            , -(Projectile.rotation + 1.57f).ToRotationVector2().RotateByRandom(-0.2f, 0.2f) * Main.rand.NextFloat(1, 2));
                    }

                    Helper.PlayPitched(CoraliteSoundID.CrystalHit_DD2_WitherBeastCrystalImpact, Projectile.Center, pitch: -0.8f);
                    Projectile.Kill();
                }
            }

            Timer++;
        }

        public override bool PreDrawExtras()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Rectangle frameBox;

            //如果距离小于顶部长度那么就只绘制顶部
            float distance = Projectile.Center.Distance(Main.player[Projectile.owner].Center);
            if (distance < 110 - 8)
            {
                //只绘制尖端
                frameBox = new Rectangle(0, 0, mainTex.Width, (int)distance + 8);
                Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor, Projectile.rotation
                    , new Vector2(mainTex.Width / 2, 8), Projectile.scale, 0, 0);

                return false;
            }

            //绘制完整尖端
            frameBox = new Rectangle(0, 0, mainTex.Width, 110);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, frameBox, lightColor, Projectile.rotation
                , new Vector2(mainTex.Width / 2, 8), Projectile.scale, 0, 0);

            Vector2 dir = (Projectile.rotation - 1.57f).ToRotationVector2();
            Vector2 pos = Projectile.Center - dir * (110 - 8);
            distance -= 110 - 8;
            while (distance > 0)
            {
                frameBox = new Rectangle(0, 110, mainTex.Width, (int)(distance > 28 ? 28 : distance));
                Main.spriteBatch.Draw(mainTex, pos - Main.screenPosition, frameBox, Lighting.GetColor(pos.ToTileCoordinates()), Projectile.rotation
                    , new Vector2(mainTex.Width / 2, 0), Projectile.scale, 0, 0);

                pos -= dir * 28;
                distance -= 28;
            }

            return false;
        }
    }

    public class CrystallineHookDust : ModDust
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Texture2D.Frame(1, 5, 0, 0);
            dust.color = Color.White;
        }

        public override bool Update(Dust dust)
        {
            dust.fadeIn++;
            if (dust.fadeIn % 2 == 0)
            {
                dust.frame.Y += Texture2D.Frame(1, 5, 0, 0).Height;
                if (dust.frame.Height >= Texture2D.Height())
                    dust.active = false;
            }

            dust.rotation = dust.velocity.ToRotation();
            dust.position += dust.velocity;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            var tex = Texture2D.Value;
            Main.spriteBatch.Draw(tex, dust.position - Main.screenPosition, dust.frame
                , Lighting.GetColor(dust.position.ToTileCoordinates(), dust.color), dust.rotation + MathHelper.PiOver2, dust.frame.Size() / 2, dust.scale, 0, 0);
            return false;
        }
    }
}
