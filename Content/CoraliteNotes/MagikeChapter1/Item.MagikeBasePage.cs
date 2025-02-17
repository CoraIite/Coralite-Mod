using Coralite.Content.NPCs.Town;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.CoraliteNotes.MagikeChapter1
{
    public class MagikeBasePage : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.UseSound = CoraliteSoundID.IceMagic_Item28;
        }

        public override bool CanUseItem(Player player)
        {
            //MagikeSystem.learnedMagikeBase = false;
            //CoraliteContent.GetKKnowledge(KeyKnowledgeID.MagikeS1).Unlock = false;
            //return true;

            if (!CoraliteContent.GetKKnowledge<MagikeS1Knowledge>().Unlock)
            {
                MagikeSystem.learnedMagikeBase = true;
                CoraliteContent.GetKKnowledge<MagikeS1Knowledge>().UnlockKnowledge();
                //TODO: 同步知识改变

                if (Main.myPlayer == player.whoAmI)
                {
                    KnowledgeSystem.SpawnKnowledgeUnlockText(player.Center, Coralite.MagicCrystalPink);
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, new Vector2(player.direction * 8, -4),
                        ModContent.ProjectileType<Page_MagikeBaseProj>(), 1, 0, player.whoAmI);
                }

                return true;
            }

            return false;
        }
    }

    public class Page_MagikeBaseProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + "MagikeBasePage";

        private bool span;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 500;
            //Projectile.friendly = true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        
        public void Initialize()
        {
            Projectile.rotation = Main.rand.NextFloat(6.282f);
        }

        public override void AI()
        {
            if (!span)
            {
                Initialize();
                span = true;
            }
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
                Main.spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians), frame, Coralite.MagicCrystalPink, 0, frameOrigin, 1, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(texture, drawPos, frame, Color.White, 0, frameOrigin, 1, SpriteEffects.None, 0);
            return false;
        }
    }
}
