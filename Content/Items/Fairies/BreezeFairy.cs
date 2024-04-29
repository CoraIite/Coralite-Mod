using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies
{
    public class BreezeFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<BreezeFairy>();
        public override FairyAttempt.Rarity Rarity => FairyAttempt.Rarity.U;

        public override int MaxResurrectionTime => 90 * 60;

        public override void SetOtherDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 0, 5);
            //Item.shoot = ModContent.ProjectileType<GreenFairyProj>();
        }

        public override void SetFairyDefault(FairyGlobalItem fairyItem)
        {
            fairyItem.FairyItemSets(2, 6, 200);
        }
    }

    public class BreezeFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<BreezeFairyItem>();
        public override int VerticalFrames => 4;

        public override FairyAttempt.Rarity Rarity => FairyAttempt.Rarity.U;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneForest)
                .RegisterToWall();
        }

        public override void Catching(Rectangle cursor, BaseFairyCatcherProj catcher)
        {
            Timer--;
            velocity = velocity.RotateByRandom(-0.08f, 0.04f);
            if (Timer <= 0)
            {
                Timer = Main.rand.Next(60, 100);
                if (Main.rand.NextBool(3))
                    Helper.PlayPitched("Fairy/FairyMove" + Main.rand.Next(2), 0.3f, 0, position);
                Vector2 webCenter = catcher.webCenter.ToWorldCoordinates();
                Vector2 dir;
                if (Vector2.Distance(Center, webCenter) > catcher.webRadius * 2 / 3)
                    dir = (webCenter - Center)
                        .SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f);
                else
                    dir = (Center - cursor.Center.ToVector2())
                            .SafeNormalize(Vector2.Zero).RotateByRandom(-0.2f, 0.2f);

                velocity = dir * Main.rand.NextFloat(0.4f, 0.9f);
            }
        }

        public override void OnCursorIntersects(Rectangle cursor, BaseFairyCatcherProj catcher)
        {
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.Cloud, Helper.NextVec2Dir(0.5f, 1.5f), 50);
                d.noGravity = true;
            }
            else if (Main.rand.NextBool())
                this.SpawnTrailDust(DustID.Cloud, Main.rand.NextFloat(0.05f, 0.5f), 50);
        }

        public override void FreeMoving()
        {
            Timer--;
            velocity = velocity.RotateByRandom(-0.02f, 0.01f);
            if (Timer < 1)
            {
                velocity = Helper.NextVec2Dir(0.2f, 1f);
                Timer = Main.rand.Next(60, 80);
            }
        }

        public override void PreAI_InCatcher()
        {
            SetDirectionNormally();
            UpdateFrameY(6);
        }
    }
}
