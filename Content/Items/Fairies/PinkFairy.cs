using Coralite.Core;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;

namespace Coralite.Content.Items.Fairies
{
    public class PinkFairyItem : BaseFairyItem
    {
        public override int FairyType => CoraliteContent.FairyType<PinkFairy>();
        public override FairyAttempt.Rarity Rarity => FairyAttempt.Rarity.R;

        public override int MaxResurrectionTime => 60 * 60;

        public override void SetOtherDefaults()
        {
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(0, 0, 3);
            //Item.shoot = ModContent.ProjectileType<PinkFairyProj>();
        }

        public override void SetFairyDefault(FairyGlobalItem fairyItem)
        {
            fairyItem.baseDamage = 9;
            fairyItem.baseDefence = 8;
            fairyItem.baseLifeMax = 400;
        }
    }

    public class PinkFairy : Fairy
    {
        public override int ItemType => ModContent.ItemType<PinkFairyItem>();
        public override int VerticalFrames => 4;

        public override FairyAttempt.Rarity Rarity => FairyAttempt.Rarity.R;

        public override void RegisterSpawn()
        {
            FairySpawnController.Create(Type)
                .AddCondition(FairySpawnCondition.ZoneForest)
                .RegisterToWall();
        }

        public override void Catching(Rectangle cursor, BaseFairyCatcherProj catcher)
        {
            Timer--;
            if (Timer % 40 == 0)
                velocity = velocity.RotateByRandom(-MathHelper.PiOver4 / 2, MathHelper.PiOver4 / 2);

            if (Timer < 1)
            {
                Timer = Main.rand.Next(60, 90);
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

                velocity = dir * Main.rand.NextFloat(0.6f, 1.2f);
            }
        }

        public override void OnCursorIntersects(Rectangle cursor, BaseFairyCatcherProj catcher)
        {
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Center, DustID.PinkFairy, Helper.NextVec2Dir(0.5f, 1.5f), 200);
                d.noGravity = true;
            }
            else if (Main.rand.NextBool())
                this.SpawnTrailDust(DustID.PinkFairy, Main.rand.NextFloat(0.05f, 0.5f), 200);
        }

        public override void FreeMoving()
        {
            Timer--;
            if (Timer < 1)
            {
                velocity = Helper.NextVec2Dir(0.5f, 1f);
                Timer = Main.rand.Next(70, 110);
            }
        }

        public override void PreAI_InCatcher()
        {
            SetDirectionNormally();
            UpdateFrameY(6);
        }
    }
}
