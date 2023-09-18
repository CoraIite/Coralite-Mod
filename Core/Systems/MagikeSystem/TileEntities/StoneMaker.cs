using Coralite.Helpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class StoneMaker : MagikeFactory
    {
        public int stack;
        public int magikeCost;

        public abstract Color MainColor { get; }

        public StoneMaker(int magikeMax, int workTimeMax, int magikeCost, int stack) : base(magikeMax, workTimeMax)
        {
            this.magikeCost = magikeCost;
            this.stack = stack;
        }

        public override bool StartWork()
        {
            if (magike >= magikeCost)
                return base.StartWork();

            return false;
        }

        public override void DuringWork()
        {
            float factor = workTimer / (float)workTimeMax;

            Vector2 center = Position.ToWorldCoordinates(16, -8);

            float width = 24 - factor * 22;
            Dust dust = Dust.NewDustPerfect(center + Main.rand.NextVector2CircularEdge(width, width), DustID.LastPrism, Vector2.Zero, newColor: MainColor);
            dust.noGravity = true;
        }

        public override void WorkFinish()
        {
            if (Charge(-magikeCost))
            {
                Vector2 position = Position.ToWorldCoordinates(16, -8);

                Item.NewItem(new EntitySource_TileEntity(this), position, ItemID.StoneBlock,stack);
                SoundEngine.PlaySound(CoraliteSoundID.ManaCrystal_Item29, position);
                MagikeHelper.SpawnDustOnGenerate(3, 2, Position + new Point16(0, -2), MainColor);
            }
        }
    }
}
