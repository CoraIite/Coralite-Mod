using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies
{
    public class GreenFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<GreenFairy>();
        public override FairyAttempt.Rarity Rarity => FairyAttempt.Rarity.C;

        public override void SetOtherDefaults()
        {
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(0,0,5);
        }

        public override void SetFairyDefault(FairyGlobalItem fairyItem)
        {
            fairyItem.baseDamage = 10;
            fairyItem.baseDefence = 4;
            fairyItem.baseLifeMax = 240;
        }
    }

    public class GreenFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<GreenFairyItem>();
        public override int VerticalFrames => 4;
        public int Timer;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneForest)
                .RegisterToWall();
        }

        public override void AI_InCatcher(Rectangle cursor,BaseFairyCatcherProj catcher)
        {
            spriteDirection = Math.Sign(velocity.X);
            if (++frameCounter > 6)
            {
                frameCounter = 0;
                if (++frame.Y > 3)
                {
                    frame.Y = 0;
                }
            }

            if (catching)
            {
                if (cursorIntersects)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust d = Dust.NewDustPerfect(Center, DustID.GreenFairy, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                        d.noGravity = true;
                    }
                    else
                        this.SpawnTrailDust(DustID.GreenFairy, Main.rand.NextFloat(0.05f, 0.5f), 200);
                }

                Timer++;
                if (Timer > 60)
                {
                    if (Main.rand.NextBool(3))
                        Helper.PlayPitched("Fairy/FairyMove" + Main.rand.Next(2), 0.3f, 0, position);
                    Timer = 0;
                    Vector2 webCenter = catcher.webCenter.ToWorldCoordinates();
                    if (Vector2.Distance(Center, webCenter) > catcher.webRadius / 2)
                    {
                        velocity = (webCenter - Center)
                            .SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f)
                            * Main.rand.NextFloat(0.3f, 0.8f);
                    }
                    else
                        velocity = (Center - cursor.Center.ToVector2())
                                .SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f)
                                * Main.rand.NextFloat(0.3f, 0.8f);
                }

                return;
            }

            Timer++;
            if (Timer > 60)
            {
                velocity = Helper.NextVec2Dir(0.1f, 0.8f);
                Timer = 0;
            }
        }
    }
}
