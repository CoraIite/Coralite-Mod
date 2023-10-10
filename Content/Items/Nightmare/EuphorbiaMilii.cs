using Coralite.Content.ModPlayers;
using Coralite.Core;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Microsoft.Xna.Framework;

namespace Coralite.Content.Items.Nightmare
{
    /// <summary>
    /// 刺刺=》下挥后再次向下挥=》上至下挥砍后下至上挥舞2圈=》蓄力刺，蓄满在鼠标位置召唤特殊刺并获得3层<br></br>
    /// 右键消耗能量释放特殊攻击
    /// </summary>
    public class EuphorbiaMilii:ModItem,INightmareWeapon
    {
        public override string Texture => AssetDirectory.NightmareItems + Name;

        public int combo;

        public override void SetDefaults()
        {
            Item.useAnimation = Item.useTime = 23;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.shoot = ProjectileType<DreamShearsSlash>();
            Item.DamageType = DamageClass.Melee;
            Item.rare = RarityType<NightmareRarity>();
            Item.value = Item.sellPrice(2, 0, 0, 0);
            Item.SetWeaponValues(265, 4, 4);
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                PlayerNightmareEnergy.Spawn(player, source);

                if (player.altFunctionUse == 2)
                {
                    //生成弹幕
                    if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.nightmareEnergy >= 7)//射出特殊弹幕
                    {
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<DreamShearsCut>(), (int)(damage * 14f), knockback,
                            player.whoAmI, 1);
                        cp.nightmareEnergy = 0;
                    }
                    else
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<DreamShearsCut>(), (int)(damage * 1.4f), knockback, player.whoAmI, 0);
                    return false;
                }

                // 生成弹幕
                switch (combo)
                {
                    default:
                    case 0:
                    case 1://挥
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, combo);
                        break;
                    case 2://转圈圈
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<DreamShearsRolling>(), (int)(damage * 1.2f), knockback, player.whoAmI);
                        if (player.TryGetModPlayer(out CoralitePlayer cp2) && cp2.nightmareEnergy < 7)//获得能量
                            cp2.nightmareEnergy++;
                        break;
                    case 3://剪
                        Projectile.NewProjectile(source, player.Center, Vector2.Zero, ProjectileType<DreamShearsCut>(), (int)(damage * 1.4f), knockback, player.whoAmI, 0);

                        if (player.TryGetModPlayer(out CoralitePlayer cp3) && cp3.nightmareEnergy < 7)//获得能量
                            cp3.nightmareEnergy++;
                        break;
                }

                combo++;
                if (combo > 3)
                    combo = 0;
            }

            return false;
        }

    }
}
