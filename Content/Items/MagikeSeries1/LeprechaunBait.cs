using Coralite.Content.WorldGeneration;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.SmoothFunctions;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class LeprechaunBait : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }

    public class LeprechaunBaitHeldProj:BaseHeldProj
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public ref float State => ref Projectile.ai[0]; 
        public ref float Timer => ref Projectile.ai[1]; 

        public bool hasFairy;
        public Vector2 fairyPos;
        public SecondOrderDynamics_Vec2 posSmoother;

        public static Asset<Texture2D> baitTex;
        public static Asset<Texture2D> fairyTex;

        public override void SetDefaults()
        {

        }

        public override void AI()
        {
            SetHeldProj();

            if (Owner.HeldItem.type!=ModContent.ItemType<LeprechaunBait>())
            {
                Projectile.Kill();
                return;
            }

            switch (State)
            {
                default:
                case 0://刚拿出来
                    {
                        Projectile.velocity.X *= 0.9f;
                        if (Projectile.velocity.Y < 5)
                            Projectile.velocity.Y += 0.2f;
                        else
                            Projectile.velocity.Y = 5;

                        Timer++;
                        if (Timer > 80)
                        {
                            if (CoraliteWorld.MagicCrystalCaveCenters==null|| CoraliteWorld.MagicCrystalCaveCenters.Count<1)
                                Projectile.Kill();

                            State = 1;
                            Timer = 0;
                            hasFairy = true;
                            fairyPos = Projectile.Center;

                            posSmoother = new SecondOrderDynamics_Vec2(1, 0.5f, 0, fairyPos);

                            for (int i = 0; i < 10; i++)
                            {
                                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.CrystalSerpent_Pink
                                    , (i * MathHelper.TwoPi / 10).ToRotationVector2() * Main.rand.NextFloat(2, 3));
                                d.noGravity = true;
                            }
                        }
                    }
                    break;
                case 1://被仙灵追
                    {
                        if (Timer < 60)
                        {
                            fairyPos = posSmoother.Update(1 / 60f, Projectile.Center + (Timer * 0.1f).ToRotationVector2() * 32);
                        }

                        Vector2 center = CoraliteWorld.MagicCrystalCaveCenters
                            .MinBy(p => p.ToWorldCoordinates().Distance(Owner.MountedCenter)).ToWorldCoordinates();

                        Timer++;
                    }
                    break;
            }

            LimitPosition();
        }

        public void LimitPosition()
        {
            Vector2 handlePos = GetHandleTopPos();
            if (Projectile.Center.Distance(handlePos) > 60)
            {
                Vector2 dir = (Projectile.Center - handlePos).SafeNormalize(Vector2.Zero);
                Projectile.Center = dir * 60;
            }
        }

        public Vector2 GetHandleTopPos()
        {
            return Owner.MountedCenter + new Vector2(OwnerDirection * 6, -10);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }


}
