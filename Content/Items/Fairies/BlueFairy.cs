using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Core.Systems.FairyCatcherSystem.Bases.Items;
using Coralite.Core.Systems.FairyCatcherSystem.NormalSkills;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies
{
    public class BlueFairyItem : TackleFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<BlueFairy>();
        public override FairyRarity Rarity => FairyRarity.C;

        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper:50);
            Item.shoot = ModContent.ProjectileType<BlueFairyProj>();
        }
    }

    public class BlueFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<BlueFairyItem>();

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneBeach)
                .RegisterToWall();
        }

        public override void Catching(FairyCatcherProj catcher)
        {
            EscapeNormally(catcher, (60, 80), (1, 1.6f));
        }

        public override void OnCatch(Player player, ref int catchPower)
        {
            targetVelocity = Helper.NextVec2Dir(1.4f, 1.6f);

            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.GreenFairy, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
        }
    }

    public class BlueFairyProj : BaseFairyProjectile
    {
        public override string Texture => AssetDirectory.FairyItems + "BlueFairy";

        public override FairySkill[] InitSkill()
            => [
                NewSkill<FSkill_Tackle>()
                ];

        public override void SpawnFairyDust()
        {
            switch (State)
            {
                case AIStates.Shooting:
                case AIStates.Rest:
                case AIStates.Backing:
                    if (Main.rand.NextBool(3))
                        Projectile.SpawnTrailDust(DustID.BlueFairy, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
                case AIStates.Skill:
                default:
                    Projectile.SpawnTrailDust(DustID.BlueFairy, Main.rand.NextFloat(0.1f, 0.5f), 200);
                    break;
            }
        }

        public override void AIAfter()
        {
            Lighting.AddLight(Projectile.Center, 0.1f, 0.1f, 0.05f);
        }

        public override Vector2 GetRestSpeed()
        {
            return (Timer * 0.1f + Projectile.identity * MathHelper.TwoPi / 6).ToRotationVector2() * Math.Clamp(Timer / 120f, 0, 1) * 3;
        }

        public override void OnStartUseSkill(NPC target)
        {
            SoundEngine.PlaySound(CoraliteSoundID.Fairy_NPCHit5, Projectile.Center);
        }

        public override void OnKill_DeadEffect()
        {
            SoundEngine.PlaySound(CoraliteSoundID.Fairy_NPCHit5, Projectile.Center);

            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueFairy, Helper.NextVec2Dir(1, 2), 200);
                d.noGravity = true;
            }
        }
    }
}
