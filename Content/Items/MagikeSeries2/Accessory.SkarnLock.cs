using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.MagikeInterstitial3;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.KeySystem;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.MagikeSeries2
{
    public class SkarnLock() : BaseAccessory(ModContent.RarityType<CrystallineMagikeRarity>(), Item.sellPrice(0, 2)), IConsultableItem
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;
        public Knowledge GetKnowledge => CoraliteContent.GetKnowledge<MagikeInterstitial3Knowledge>();
        public int GetPageIndex => CoraliteNoteUIState.BookPanel.GetPageIndex<MagikeInterstitial3Page4>();

        private int EquipCount;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Summon;
            Item.damage = 40;
            Item.knockBack = 1.5f;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            int damage = player.GetWeaponDamage(Item);
            float knockBack = player.GetWeaponKnockback(Item);

            EquipCount++;

            if (EquipCount < 10)
                return;

            EquipCount = 0;
            int projType = ModContent.ProjectileType<SkarnLockProj>();

            int projCount = 0;
            foreach (var proj in Main.ActiveProjectiles)
                if (proj.owner == player.whoAmI && proj.friendly && proj.type == projType && proj.ai[0] == 0)
                    projCount++;

            if (Main.rand.Next(10) < projCount || projCount >= 5)
                return;

            int tryCount = 50;
            int size = 24;
            int searchWidth = 90;
            for (int j = 0; j < tryCount; j++)
            {
                int num5 = Main.rand.Next(200 - j * 2, 400 + j * 2);
                Vector2 center = player.Center;
                center.X += Main.rand.Next(-num5, num5 + 1);
                center.Y += Main.rand.Next(-num5, num5 + 1);
                if (Collision.SolidCollision(center, size, size) || Collision.WetCollision(center, size, size))
                    continue;

                center.X += size / 2;
                center.Y += size / 2;
                if (!Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) && !Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1))
                    continue;

                int x = (int)center.X / 16;
                int y = (int)center.Y / 16;
                bool flag = false;
                if (Main.rand.NextBool(3) && Main.tile[x, y] != null && Main.tile[x, y].WallType > WallID.None)
                {
                    flag = true;
                }
                else
                {
                    center.X -= searchWidth / 2;
                    center.Y -= searchWidth / 2;
                    if (Collision.SolidCollision(center, searchWidth, searchWidth))
                    {
                        center.X += searchWidth / 2;
                        center.Y += searchWidth / 2;
                        flag = true;
                    }
                    else if (Main.tile[x, y] != null && Main.tile[x, y].HasTile && Main.tile[x, y].TileType == TileID.Platforms)
                    {
                        flag = true;
                    }
                }

                if (!flag)
                    continue;

                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.owner == player.whoAmI && proj.friendly && proj.type == projType && proj.ai[0] == 0 && (center - proj.Center).Length() < size * 2)
                        return;
                }

                if (flag && Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_Accessory(Item), center, Vector2.Zero, projType, damage, knockBack, player.whoAmI, 0);
                    break;
                }
            }
        }
    }

    [VaultLoaden(AssetDirectory.MagikeSeries2Item)]
    public class SkarnLockProj : ModProjectile
    {
        public override string Texture => AssetDirectory.MagikeSeries2Item + Name;

        public static ATex SkarnLockProjHighlight { get; private set; }
    }
}
