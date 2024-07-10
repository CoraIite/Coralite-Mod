using Coralite.Content.NPCs.Town;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class Page_MagikeBase : ModItem
    {
        public override string Texture => AssetDirectory.MagikeItems + Name;

        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.UseSound = CoraliteSoundID.IceMagic_Item28;
            //Item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            //MagikeSystem.learnedMagikeBase = false;

            if (!MagikeSystem.learnedMagikeBase)
            {
                MagikeSystem.learnedMagikeBase = true;
                //MagikeHelper.SpawnDustOnGenerate(3, 3, player.Center.ToPoint16(), Coralite.Instance.MagicCrystalPink);
                if (Main.myPlayer == player.whoAmI)
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, new Vector2(player.direction * 8, -4),
                        ModContent.ProjectileType<Page_MagikeBaseProj>(), 1, 0, player.whoAmI);
            }

            return true;
        }
    }

    public class Page_MagikeBaseProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeItems + "Page_MagikeBase";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 500;
            //Projectile.friendly = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            Projectile.velocity *= 0.9f;

            Projectile.localAI[0]++;
            float factor = Projectile.localAI[0] / 180;
            float length = MathF.Sin(factor * MathHelper.Pi) * 80;
            Projectile.rotation += 0.05f;

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + (Projectile.rotation + i * MathHelper.TwoPi / 6).ToRotationVector2() * length,
                    DustID.CrystalSerpent_Pink, Vector2.Zero, Scale: 1.2f);
                d.noGravity = true;
            }

            if (Projectile.localAI[0] > 180)
            {
                Projectile.Kill();
                for (int i = 0; i < 16; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.CrystalSerpent_Pink, (i * MathHelper.TwoPi / 16).ToRotationVector2() * Main.rand.NextFloat(2, 3));
                    dust.noGravity = true;
                }

                SoundEngine.PlaySound(CoraliteSoundID.ManaCrystal_Item29, Projectile.Center);
                if (!NPC.AnyNPCs(ModContent.NPCType<CrystalRobot>()))
                {
                    NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<CrystalRobot>());
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Projectile.GetTexture();
            Rectangle frame = texture.Frame();

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Projectile.timeLeft / 240f + time * 0.04f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;
                Main.spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians), frame, Coralite.Instance.MagicCrystalPink, 0, frameOrigin, 1, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(texture, drawPos, frame, Color.White, 0, frameOrigin, 1, SpriteEffects.None, 0);
            return false;
        }
    }
}
