using Coralite.Content.Items.Icicle;
using Coralite.Core;
using Coralite.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Bosses.BabyIceDragon
{
    public class IceyCloud : ModProjectile
    {
        public override string Texture => AssetDirectory.BabyIceDragon + Name;
        private bool spwan;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 28;

            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 1000;
            Projectile.netImportant = true;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.coldDamage = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Frost, -Vector2.UnitY.RotatedBy(i * 0.785f) * 3f);
                dust.noGravity = true;
            }
        }

        public override void AI()
        {
            if (!spwan)
            {
                if (!VaultUtils.isServer)
                {
                    NPC boss = null;
                    foreach (var npc in Main.ActiveNPCs)
                    {
                        if (!npc.active || npc.type != ModContent.NPCType<BabyIceDragon>())
                        {
                            continue;
                        }
                        boss = npc;
                    }
                    if (boss != null)
                    {
                        ((BabyIceDragon)boss.ModNPC).GetMouseCenter(out _, out Vector2 mouseCenter);

                        for (int j = 0; j < 2; j++)
                            IceStarLight.Spawn(mouseCenter,
                                (Projectile.Center - boss.Center).SafeNormalize(Vector2.One).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 10,
                                1f, () => Projectile.Center, 16);
                    }
                }

                spwan = true;
            }

            Projectile.UpdateFrameNormally(5, 5);

            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.timeLeft = Helper.ScaleValueForDiffMode(8 * 60, 12 * 60, 18 * 60, 30 * 60);
            }

            if (Projectile.ai[0] > 45)
            {
                Projectile.ai[0] = 0;
                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position + new Microsoft.Xna.Framework.Vector2(Main.rand.Next(Projectile.width), 20),
                        Vector2.UnitY * 8, ModContent.ProjectileType<IcicleProj_Hostile>(), 10, 0);
                }
            }

            Projectile.ai[0]++;
        }
    }
}
