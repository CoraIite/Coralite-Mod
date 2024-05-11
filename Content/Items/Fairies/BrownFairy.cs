using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies
{
    public class BrownFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<BrownFairy>();
        public override FairyAttempt.Rarity Rarity => FairyAttempt.Rarity.C;

        public override int MaxResurrectionTime => 60 * 60;

        public override void SetOtherDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 5);
            Item.shoot = ModContent.ProjectileType<BrownFairyProj>();
        }

        public override void SetFairyDefault(FairyGlobalItem fairyItem)
        {
            fairyItem.FairyItemSets(6, 8, 300);
        }
    }

    public class BrownFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<BrownFairyItem>();
        public override int VerticalFrames => 4;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneRockLayerHeight)
                .RegisterToWall();
        }

        public override void Catching(Rectangle cursor, BaseFairyCatcherProj catcher)
        {
            Timer--;
            if (Timer < 1)
            {
                Timer = Main.rand.Next(60, 80);
                if (Main.rand.NextBool(3))
                    Helper.PlayPitched("Fairy/FairyMove" + Main.rand.Next(2), 0.3f, 0, position);
                Vector2 webCenter = catcher.webCenter.ToWorldCoordinates();
                Vector2 dir;
                if (Vector2.Distance(Center, webCenter) > catcher.webRadius * 2 / 3)
                    dir = (webCenter - Center)
                        .SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f);
                else
                    dir = (Center - cursor.Center.ToVector2())
                            .SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f);

                velocity = dir * Main.rand.NextFloat(0.3f, 0.8f);
            }
        }

        public override void OnCursorIntersects(Rectangle cursor, BaseFairyCatcherProj catcher)
        {
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.BrownMoss, Helper.NextVec2Dir(0.5f, 1.5f), 150);
                d.noGravity = true;
            }
            else if (Main.rand.NextBool())
                this.SpawnTrailDust(DustID.BrownMoss, Main.rand.NextFloat(0.05f, 0.5f), 150);
        }

        public override void FreeMoving()
        {
            Timer--;
            if (Timer < 1)
            {
                velocity = Helper.NextVec2Dir(0.1f, 0.8f);
                Timer = Main.rand.Next(60, 80);
            }
        }

        public override void PreAI_InCatcher()
        {
            SetDirectionNormally();
            UpdateFrameY(6);
        }
    }

    public class BrownFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "BrownFairy";
        protected override int FrameY => 4;

        protected override string SkillName => "撞击！";

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.penetrate = -1;
        }

        public override void PostAI()
        {
            SetDirectionNormally();
            UpdateFrameY(6);
        }

        public override void OnInitialize()
        {
            Projectile.velocity = Projectile.velocity.RotateByRandom(-0.3f, 0.3f);
            Timer = Main.rand.Next(30, 45);
        }

        public override void Shooting()
        {
            Timer--;
            Projectile.timeLeft = 20;
            Projectile.velocity *= 0.98f;
            if (Main.rand.NextBool(3))
                Projectile.SpawnTrailDust(DustID.BrownMoss, Main.rand.NextFloat(0.1f, 0.5f), 200);

            if (Timer < 1)
                ExchangeToAction();
        }

        public override void Action()
        {
            Projectile.timeLeft = 20;
            Projectile.SpawnTrailDust(DustID.BrownMoss, Main.rand.NextFloat(0.1f, 0.5f), 200);
            Lighting.AddLight(Projectile.Center, 0, 0.2f, 0);

            if (Projectile.velocity.Length() < 10)
                Projectile.velocity = (Projectile.velocity.Length() + 0.4f) * Projectile.velocity.SafeNormalize(Vector2.Zero);

            Timer++;
            if (Timer > 50)
                ExchangeToBack();
        }

        public override void Backing()
        {
            Timer++;
            if (Timer < 40)
            {
                Projectile.velocity *= 0.95f;
            }
            else if (Timer == 40)
            {
                Projectile.velocity = Helper.NextVec2Dir(4, 6);
            }
            else if (Timer < 40 + 120)
            {
                if (Timer % 15 == 0)
                    Projectile.velocity = Helper.NextVec2Dir(2,4);
                if (Main.rand.NextBool())
                    Projectile.SpawnTrailDust(DustID.BrownMoss, Main.rand.NextFloat(0.1f, 0.5f), 200);
            }
            else if (Timer == 40 + 120)
                RestartAction();
            else
            {
                Projectile.velocity = (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 2;
                Projectile.Center = Vector2.Lerp(Projectile.Center, Owner.Center, 0.07f);
                if (Main.rand.NextBool())
                    Projectile.SpawnTrailDust(DustID.BrownMoss, Main.rand.NextFloat(0.1f, 0.5f), 200);

                if (Vector2.Distance(Projectile.Center, Owner.Center) < 24)
                    Projectile.Kill();
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0);
        }

        public override void OnExchangeToAction(NPC target)
        {
            SpawnSkillText<BrownFairyProj>(Color.SandyBrown);
            SoundEngine.PlaySound(CoraliteSoundID.Fairy_NPCHit5, Projectile.Center);

            Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.velocity = (Projectile.Center - target.Center).SafeNormalize(Vector2.Zero)
                .RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(2f, 5f);
            ExchangeToBack();
        }
    }
}
