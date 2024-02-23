using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Nightmare
{
    public class DevilsClaw : ModItem, INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public int shootStyle;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<DevilsClawLeaf>();
            Item.DamageType = DamageClass.Magic;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(0, 30, 0, 0);
            Item.SetWeaponValues(166, 4, 4);
            Item.mana = 8;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.useTurn = false;
            Item.shootSpeed = 13;
            Item.UseSound = CoraliteSoundID.NoUse_BlowgunPlus_Item65;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override bool MagicPrefix() => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, source);

                if (player.altFunctionUse == 2)
                {
                    CoralitePlayer cp = player.GetModPlayer<CoralitePlayer>();
                    if (cp.nightmareEnergy >= 2)
                    {
                        cp.nightmareEnergy -= 2;
                        int damage2 = (int)(damage * 0.75f);
                        for (int i = 0; i < 7; i++)
                            Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.08f, 0.08f)) * Main.rand.NextFloat(0.95f, 1.15f), type, damage2, knockback, player.whoAmI, 1);
                        return false;
                    }

                    Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)), type, damage, knockback, player.whoAmI);
                    return false;
                }

                if (++shootStyle < 7)
                {
                    Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.06f, 0.06f)), type, damage, knockback, player.whoAmI);
                }
                else
                {
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, 1);
                    shootStyle = 0;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 使用ai0传入颜色，为1时是强化大小<br></br>
    /// 使用ai1传入状态，为1时能够在命中后获得梦魇光能
    /// </summary>
    public class DevilsClawLeaf : ModProjectile
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "DarkLeaf";

        private bool init = true;
        private Color glowColor;
        private bool hited = true;

        public ref float CanGetEnergy => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 2;

            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            Projectile.DamageType = DamageClass.Magic;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(0, 7);
        }

        public override void AI()
        {
            if (init)   //根据ai设置不同的弹幕大小
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Projectile.ai[0] == 1)
                {
                    glowColor = NightmarePlantera.nightmareRed;
                    Vector2 center = Projectile.Center;
                    Projectile.scale = 1.2f;
                    Projectile.width = (int)(Projectile.width * Projectile.scale);
                    Projectile.height = (int)(Projectile.height * Projectile.scale);
                    Projectile.Center = center;
                }
                else
                {
                    glowColor = NightmarePlantera.lightPurple;
                }
                init = false;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 6)
                    Projectile.frame = 0;
            }

            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.VilePowder,
                      Projectile.velocity * 0.4f, 240, glowColor, Main.rand.NextFloat(0.8f, 1.2f));
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.75f);
            Projectile.velocity *= 0.84f;
            if (CanGetEnergy == 1 && hited)
            {
                Main.player[Projectile.owner].GetModPlayer<CoralitePlayer>().GetNightmareEnergy(1);
                hited = false;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Dust d;
            for (int i = 0; i < 3; i++)
            {
                d = Dust.NewDustPerfect(Projectile.Center, DustID.SpookyWood, Helper.NextVec2Dir(0.5f, 2), Scale: Main.rand.NextFloat(1f, 2f));
                d.noGravity = true;
                Dust.NewDustPerfect(Projectile.Center, DustID.VilePowder, Helper.NextVec2Dir(1f, 3), Scale: Main.rand.NextFloat(1f, 2f));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Texture2D highlightTex = DarkLeaf.HighlightTex.Value;
            var pos = Projectile.Center - Main.screenPosition;

            Color color = glowColor;
            var mainFrameBox = mainTex.Frame(1, 7, 0, Projectile.frame);
            var highlightFrameBox = highlightTex.Frame(1, 7, 0, Projectile.frame);
            Vector2 hightlightOrigin = highlightFrameBox.Size() / 2;

            //绘制发光
            Main.spriteBatch.Draw(highlightTex, pos, highlightFrameBox, color, Projectile.rotation, hightlightOrigin, Projectile.scale, 0, 0);
            Vector2 toCenter = new Vector2(Projectile.width / 2, Projectile.height / 2);

            for (int i = 1; i < 6; i++)
                Main.spriteBatch.Draw(highlightTex, Projectile.oldPos[i] + toCenter - Main.screenPosition, highlightFrameBox,
                    color * (0.4f - i * 0.4f / 6), Projectile.oldRot[i], hightlightOrigin, (Projectile.scale - i * 0.05f), 0, 0);

            //绘制自己
            Main.spriteBatch.Draw(mainTex, pos, mainFrameBox, Color.MediumPurple, Projectile.rotation, mainFrameBox.Size() / 2, Projectile.scale, 0, 0);

            return false;
        }
    }
}
