using Coralite.Content.DamageClasses;
using Coralite.Content.Items.Glistent;
using Coralite.Helpers;
using Terraria;

namespace Coralite.Content.GlobalNPCs
{
    public class FairyGlobalNPC : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.DamageType == TrueFairyDamage.Instance)
            {
                if (npc.HasBuff<GlistentJarDebuff>())
                {
                    npc.DelBuff(npc.FindBuffIndex(ModContent.BuffType<GlistentJarDebuff>()));
                    projectile.NewProjectileFromThis<GlistentJarExplode>(npc.Center, Vector2.Zero
                        , 25, 0.5f);
                }
            }
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (npc.HasBuff<GlistentJarDebuff>())
                return Lighting.GetColor(npc.Center.ToTileCoordinates(), Color.LightGreen);

            return base.GetAlpha(npc, drawColor);
        }
    }
}
