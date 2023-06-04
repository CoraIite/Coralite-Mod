using Coralite.Core;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Coralite.Content.Items.RedJades
{
    public class RedianciePetBuff : ModBuff
    {
        public override string Texture => AssetDirectory.PetBuffs + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<RedianciePetProj>();

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);
                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
            }
        }

    }
}
