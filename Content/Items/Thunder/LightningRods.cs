using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Tiles.RedJades;
using Coralite.Content.Tiles.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class LightningRods : BasePlaceableItem
    {
        public LightningRods() : base(Item.sellPrice(0, 0, 50), ItemRarityID.Yellow, ModContent.TileType<LightningRodsTile>(), AssetDirectory.ThunderItems)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 5)
                .AddIngredient(ItemID.RainCloud, 5)
                .AddIngredient(ItemID.SoulofMight)
                .AddIngredient(ItemID.SoulofSight)
                .AddIngredient(ItemID.SoulofFright)
                .AddTile<MagicCraftStation>()
                .Register();
        }
    }

    /// <summary>
    /// 使用速度传入目标点位
    /// ai0传入闪电降下的时间
    /// ai1传入主人
    /// </summary>
    public class ThunderSpawn : ThunderFalling
    {
        public override string Texture => AssetDirectory.Blank;

        const int DelayTime = 30;

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (thunderTrails == null)
            {
                Projectile.Resize((int)PointDistance, 40);
                thunderTrails = new ThunderTrail[3];
                Asset<Texture2D> trailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc, ThunderColorFunc2_Orange);
                    else
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc, ThunderColorFunc_Yellow);
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].SetRange((0, 15));
                    thunderTrails[i].BasePositions = new Vector2[3]
                    {
                    Projectile.Center,Projectile.Center,Projectile.Center
                    };
                }
            }

            if (Timer < LightingTime)
            {
                float factor = Timer / LightingTime;
                Vector2 targetPos = Vector2.Lerp(Projectile.Center, Projectile.velocity, factor);
                Vector2 pos2 = targetPos;

                List<Vector2> pos = new List<Vector2>
                {
                    targetPos
                };
                if (Vector2.Distance(targetPos, Projectile.Center) < PointDistance)
                    pos.Add(Projectile.Center);
                else
                    for (int i = 0; i < 40; i++)
                    {
                        pos2 = pos2.MoveTowards(Projectile.Center, PointDistance);
                        if (Vector2.Distance(pos2, Projectile.Center) < PointDistance)
                        {
                            pos.Add(Projectile.Center);
                            break;
                        }
                        else
                            pos.Add(pos2);
                    }

                foreach (var trail in thunderTrails)
                {
                    trail.BasePositions = pos.ToArray();
                    trail.SetExpandWidth(4);
                }

                if (Timer % 4 == 0)
                {
                    foreach (var trail in thunderTrails)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        if (trail.CanDraw)
                            trail.RandomThunder();
                    }
                }

                ThunderWidth = 50 + 70 * factor;
                ThunderAlpha = factor;
            }
            else if ((int)Timer == (int)LightingTime)
            {
                int npcType = ModContent.NPCType<ThunderveinDragon>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(Main.LocalPlayer.whoAmI, npcType);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: Projectile.owner, number2: npcType);

                foreach (var trail in thunderTrails)
                {
                    trail.CanDraw = Main.rand.NextBool();
                    trail.RandomThunder();
                }
            }
            else
            {
                float factor = (Timer - LightingTime) / (DelayTime);
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);
                ThunderWidth = 20 + (1 - factor) * 100;
                ThunderAlpha = 1 - Coralite.Instance.X2Smoother.Smoother(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange((0, 10 + (1 - factor) * PointDistance / 2));
                    trail.SetExpandWidth((1 - factor) * PointDistance / 3);

                    if (Timer % 6 == 0)
                    {
                        trail.CanDraw = Main.rand.NextBool();
                        trail.RandomThunder();
                    }
                }

                if (Timer > LightingTime + DelayTime)
                    Projectile.Kill();
            }

            Timer++;
        }
    }
}
