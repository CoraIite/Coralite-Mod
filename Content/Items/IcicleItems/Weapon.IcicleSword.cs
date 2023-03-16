using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.IcicleItems
{
    public class IcicleSword : ModItem
    {
        public override string Texture => AssetDirectory.IcicleItems + Name;

        public byte useCount;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 30;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.knockBack = 3f;
            Item.crit = 0;
            Item.reuseDelay = 0;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ProjectileType<CrushedIceProj>();
            Item.UseSound = CoraliteSoundID.Swing_Item1;

            Item.useTurn = false;
            Item.noUseGraphic = false;
            Item.autoReuse = true;
        }

        public override void Load()
        {
            for (int i = 0; i < 3; i++)
                GoreLoader.AddGoreFromTexture<SimpleModGore>(Mod, AssetDirectory.IcicleItems + Name + "_Gore" + i);
        }

        public override bool CanUseItem(Player player)
        {
            //每挥舞5次最后一次使这把剑碎裂并射出更多弹幕
            if (useCount > 4)
            {
                Item.useTime = 60;
                Item.useAnimation = 60;
                Item.noUseGraphic = true;
                Item.noMelee = true;
            }
            else
            {
                Item.useTime = 30;
                Item.useAnimation = 30;
                Item.noUseGraphic = false;
                Item.noMelee = false;
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (useCount == 5)
                {
                    Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero);
                    //射出3根冰锥
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(source, player.Center, dir.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * 10f, ProjectileType<IcicleProj>(),
                            (int)(damage * 0.8f), knockback, player.whoAmI, i * 8);
                        //生成碎剑gore
                        Gore.NewGore(source, player.Center, new Vector2(player.direction, 0).RotatedBy(-player.direction * Main.rand.NextFloat(1.57f)) * Main.rand.Next(2, 3), 
                            Mod.Find<ModGore>("IcicleSword_Gore" + i.ToString()).Type);
                    }
                    //射出5个碎冰弹幕
                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 dir2 = -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
                        Projectile.NewProjectile(source, player.Center, dir2 * Main.rand.NextFloat(4.5f, 6f), ProjectileType<CrushedIceProj>(),
                            (int)(damage * 0.6f), knockback, player.whoAmI, 0.2f);

                        //生成一堆粒子
                        for (int j = 0; j < 3; j++)
                            Dust.NewDustPerfect(player.Center, DustID.FrostStaff, dir2 * Main.rand.Next(2, 5), 0, default, Main.rand.NextFloat(0.8f, 1.3f));

                    }
                    Helper.PlayPitched("Icicle/IcicleSword", 0.4f, 0f, player.Center);
                    useCount = 0;
                    return false;
                }
                else
                {
                    //射出碎冰弹幕
                    Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * 10f, ProjectileType<CrushedIceProj>(),
                        (int)(damage * 0.6f), knockback, player.whoAmI, 0.04f);
                }
            }

            useCount++;
            return false;
        }
    }
}
