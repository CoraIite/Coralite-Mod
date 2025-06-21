using Coralite.Content.DamageClasses;
using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies
{
    public class BreezeFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<BreezeFairy>();
        public override FairyRarity Rarity => FairyRarity.U;

        public override int MaxResurrectionTime => 90 * 60;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 5);
            Item.shoot = ModContent.ProjectileType<BreezeFairyProj>();
        }
    }

    public class BreezeFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<BreezeFairyItem>();
        public override int VerticalFrames => 4;

        public override FairyRarity Rarity => FairyRarity.U;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneForest)
                .RegisterToWall();
        }

        public override void Catching(FairyCatcherProj catcher)
        {
            FairyTimer--;
            velocity = velocity.RotateByRandom(-0.08f, 0.04f);
            if (FairyTimer <= 0)
            {
                FairyTimer = Main.rand.Next(60, 100);
                if (Main.rand.NextBool(3))
                    Helper.PlayPitched("Fairy/FairyMove" + Main.rand.Next(2), 0.3f, 0, position);
                Vector2 webCenter = catcher.webCenter;
                Vector2 dir;
                if (Vector2.Distance(Center, webCenter) > catcher.webRadius * 2 / 3)
                    dir = (webCenter - Center)
                        .SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f);
                else
                    dir = (Center - catcher.Owner.Center)
                            .SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f);

                velocity = dir * Main.rand.NextFloat(0.4f, 0.9f);
            }
        }

        //public override void OnCursorIntersects(Rectangle cursor, FairyCatcherProj catcher)
        //{
        //    if (Main.rand.NextBool(3))
        //    {
        //        Dust d = Dust.NewDustPerfect(Center, DustID.Cloud, Helper.NextVec2Dir(0.5f, 1.5f), 50);
        //        d.noGravity = true;
        //    }
        //    else if (Main.rand.NextBool())
        //        this.SpawnTrailDust(DustID.Cloud, Main.rand.NextFloat(0.05f, 0.5f), 50);
        //}

        public override void FreeMoving()
        {
            FairyTimer--;
            velocity = velocity.RotateByRandom(-0.02f, 0.01f);
            if (FairyTimer < 1)
            {
                velocity = Helper.NextVec2Dir(0.2f, 1f);
                FairyTimer = Main.rand.Next(60, 80);
            }
        }

        public override void PreAI_InCatcher()
        {
            SetDirectionNormally();
            UpdateFrameY(6);
        }
    }

    public class BreezeFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "BreezeFairy";
        protected override int FrameY => 4;

        protected override string SkillName => "吹散！";

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.penetrate = -1;
            AttackDistance = 120;
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
            canDamage = true;
            Timer--;
            Projectile.timeLeft = 20;
            Projectile.velocity *= 0.98f;
            if (Main.rand.NextBool(3))
                Projectile.SpawnTrailDust(DustID.Cloud, Main.rand.NextFloat(0.1f, 0.5f), 100);

            if (Timer < 1)
                ExchangeToAction();
        }

        public override void Action()
        {
            Projectile.timeLeft = 20;
            Projectile.SpawnTrailDust(DustID.AncientLight, Main.rand.NextFloat(0.1f, 0.5f));
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.2f);

            Timer++;

            if (Timer < 20)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(120, 120);
                Dust d = Dust.NewDustPerfect(pos, DustID.AncientLight, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 4f));
                d.noGravity = true;
                Projectile.velocity *= 0.8f;
            }
            else if (Timer == 20)
            {
                Color c = Color.SkyBlue;
                //c.A = 50;
                //RedJades.RedExplosionParticle.Spawn(Projectile.Center, 1.2f, c); 
                c.A = 200;
                RedJades.RedGlowParticle.Spawn(Projectile.Center, 1.2f, c, 0.3f);
            }
            else if (Timer < 20 + 10)
            {
                if (Timer == 25)
                {
                    Color c = Color.SkyBlue;
                    c.A = 150;
                    RedJades.RedGlowParticle.Spawn(Projectile.Center, 1.2f, c, 0.3f);
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC n = Main.npc[i];
                    if (n.CanBeChasedBy() && Vector2.Distance(n.Center, Projectile.Center) < 120)
                    {
                        n.velocity += (n.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 2 * n.knockBackResist;
                        if (Timer == 23)
                            n.StrikeNPC(n.CalculateHitInfo(Projectile.damage, 0, knockBack: 0, damageType: FairyDamage.Instance));
                    }
                }
            }

            if (Timer > 60)
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
                Projectile.velocity = new Vector2(0, MathF.Sin(Timer * 0.1f) * 2);

                if (Main.rand.NextBool())
                    Projectile.SpawnTrailDust(DustID.Cloud, Main.rand.NextFloat(0.1f, 0.5f), 100);
            }
            else if (Timer == 40 + 120)
                RestartAction();
            else
            {
                Backing_LerpToOwner();
                if (Main.rand.NextBool())
                    Projectile.SpawnTrailDust(DustID.Cloud, Main.rand.NextFloat(0.1f, 0.5f), 100);
            }

            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.2f);
        }

        public override void OnExchangeToAction(NPC target)
        {
            SpawnSkillText(Color.Blue);
            SoundEngine.PlaySound(CoraliteSoundID.Fairy_NPCHit5, Projectile.Center);
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
