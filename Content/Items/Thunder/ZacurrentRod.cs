using Coralite.Content.Bosses.ModReinforce.PurpleVolt;
using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Items.Materials;
using Coralite.Content.Tiles.Thunder;
using Coralite.Core;
using Coralite.Core.Prefabs.Items;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Thunder
{
    public class ZacurrentRod : BasePlaceableItem, IMagikeCraftable
    {
        public ZacurrentRod() : base(Item.sellPrice(0, 1), ItemRarityID.Purple, ModContent.TileType<ZacurrentRodTile>(), AssetDirectory.ThunderItems)
        {
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<LightningRods, ZacurrentRod>(MagikeHelper.CalculateMagikeCost<HolyLightLevel>( 12, 60 * 4))
                .AddIngredient<FragmentsOfLight>()
                .AddIngredient<DukeFishronSkin>()
                .Register();
        }
    }

    /// <summary>
    /// 使用速度传入目标点位
    /// ai0传入闪电降下的时间
    /// ai1传入主人
    /// </summary>
    public class ZacurrentSpawn : ThunderFalling
    {
        public override string Texture => AssetDirectory.Blank;

        const int DelayTime = 30;

        public override bool? CanDamage() => false;

        public override Color ThunderColorFunc2_Orange(float factor)
        {
            return ZacurrentDragon.ZacurrentPurple;
        }

        public override Color ThunderColorFunc_Yellow(float factor)
        {
            return ZacurrentDragon.ZacurrentPink;
        }

        public override void AI()
        {
            if (thunderTrails == null)
            {
                Projectile.Resize((int)PointDistance, 40);
                thunderTrails = new ThunderTrail[3];
                ATex trailTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBodyF");
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc, ThunderColorFunc2_Orange, GetAlpha);
                    else
                        thunderTrails[i] = new ThunderTrail(trailTex, ThunderWidthFunc, ThunderColorFunc_Yellow, GetAlpha);
                    thunderTrails[i].CanDraw = false;
                    thunderTrails[i].UseNonOrAdd = true;
                    thunderTrails[i].SetRange((0, 15));
                    thunderTrails[i].BasePositions =
                    [
                    Projectile.Center,Projectile.Center,Projectile.Center
                    ];
                }
            }

            if (Timer < LightingTime)
            {
                float factor = Timer / LightingTime;
                for (int j = 0; j < thunderTrails.Length; j++)
                {
                    var trail = thunderTrails[j];
                    Vector2 targetPos = Vector2.Lerp(Projectile.Center, Projectile.velocity, factor);
                    Vector2 pos2 = targetPos;

                    List<Vector2> pos = new()
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

                    trail.BasePositions = [.. pos];
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

                ThunderWidth = 30 + (40 * factor);
                ThunderAlpha = factor;
            }
            else if ((int)Timer == (int)LightingTime)
            {
                int npcType = ModContent.NPCType<ZacurrentDragon>();

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
                float factor = (Timer - LightingTime) / DelayTime;
                float sinFactor = MathF.Sin(factor * MathHelper.Pi);
                ThunderWidth = 20 + ((1 - factor) * 60);
                ThunderAlpha = 1 - Helper.X2Ease(factor);

                foreach (var trail in thunderTrails)
                {
                    trail.SetRange((0, 10 + ((1 - factor) * PointDistance / 2)));
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
