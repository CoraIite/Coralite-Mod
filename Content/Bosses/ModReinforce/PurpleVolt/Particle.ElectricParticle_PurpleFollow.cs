using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Core;
using InnoVault.PRT;
using System;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    public class ElectricParticle_PurpleFollow : ElectricParticle
    {
        public override string Texture => AssetDirectory.ThunderveinDragon + "ElectricParticle_Purple";

        public override bool ShouldUpdatePosition() => false;

        private Func<Vector2> GetParentCenter;

        public override void AI()
        {
            if (!GetCenter(out Vector2 parentCenter))
                return;

            Position = parentCenter + Velocity;
            Opacity++;
            if (Opacity > 1 && Opacity % 4 == 0)
            {
                Frame.X += 80;
                if (Frame.X > 80 * 6)
                    active = false;
            }
        }

        public bool GetCenter(out Vector2 parentCenter)
        {
            if (GetParentCenter != null)
            {
                parentCenter = GetParentCenter();
                return true;
            }

            parentCenter = Vector2.Zero;
            active = false;
            return false;
        }

        public static void Spawn(Vector2 parentCenter, Vector2 offset, Func<Vector2> GetParentCenter, float scale = 1f)
        {
            if (VaultUtils.isServer)
            {
                return;
            }
            ElectricParticle_PurpleFollow p = PRTLoader.NewParticle<ElectricParticle_PurpleFollow>(parentCenter + offset, offset, Scale: scale);
            p.GetParentCenter = GetParentCenter;
        }
    }
}
