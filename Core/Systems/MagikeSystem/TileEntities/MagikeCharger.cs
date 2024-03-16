using Coralite.Content.UI;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class MagikeCharger : MagikeFactory, IMagikeSingleItemContainer
    {
        protected MagikeCharger(int magikeMax, int howManyPerCharge) : base(magikeMax, 60)
        {
            this.howManyPerCharge = howManyPerCharge;
        }

        public readonly int howManyPerCharge;
        public Item containsItem = new Item();
        public float rotation;

        public Item ContainsItem { get => containsItem; set => containsItem = value; }
        public abstract Color MainColor { get; }

        public override bool StartWork()
        {
            if (containsItem is not null && !containsItem.IsAir &&
                containsItem.TryGetGlobalItem(out MagikeItem mi) && mi.magikeMax >= 0 && mi.magike < mi.magikeMax
                && magike >= howManyPerCharge)
                return base.StartWork();

            return false;
        }

        public override void DuringWork()
        {
            float factor = workTimer / (float)workTimeMax;

            Vector2 center = Position.ToWorldCoordinates(16, -16);
            if (workTimer % 5 == 0)
            {
                Dust dust = Dust.NewDustPerfect(center + new Vector2(Main.rand.Next(-16, 16), 16), DustID.FireworksRGB, -Vector2.UnitY * Main.rand.NextFloat(0.8f, 3f), newColor: MainColor);
                dust.noGravity = true;
            }

            rotation += 0.05f;
            if (workTimer % 2 == 0)
            {
                float length = MathF.Sin(factor * MathHelper.Pi) * 32;

                for (int i = 0; i < 6; i++)
                {
                    Dust dust = Dust.NewDustPerfect(center + (rotation + i * MathHelper.TwoPi / 6).ToRotationVector2() * length
                        , DustID.LastPrism, Vector2.Zero, newColor: MainColor);
                    dust.noGravity = true;
                }
            }
        }

        public override void WorkFinish()
        {
            if (containsItem is not null && !containsItem.IsAir &&
                containsItem.TryGetGlobalItem(out MagikeItem mi) && mi.magikeMax >= 0 && mi.magike < mi.magikeMax)
            {
                int howMany = howManyPerCharge;
                if (howMany > mi.magikeMax - mi.magike)
                    howMany = mi.magikeMax - mi.magike;

                mi.magike += howMany;
                Charge(-howMany);

                if (mi.magike < mi.magikeMax)
                    StartWork();

                Vector2 position = Position.ToWorldCoordinates(24, -8);

                SoundEngine.PlaySound(CoraliteSoundID.ManaCrystal_Item29, position);
                MagikeHelper.SpawnDustOnGenerate(3, 2, Position + new Point16(0, -2), MainColor);
            }
        }

        public override void OnKill()
        {
            MagikeItemSlotPanel.visible = false;
            UILoader.GetUIState<MagikeItemSlotPanel>().Recalculate();

            if (!containsItem.IsAir)
                Item.NewItem(new EntitySource_TileEntity(this), Position.ToWorldCoordinates(16, -8), containsItem);
        }


        public virtual bool CanInsertItem(Item item)
        {
            return true;
        }

        public virtual Item GetItem()
        {
            return containsItem;
        }

        public virtual bool InsertItem(Item item)
        {
            containsItem = item;
            return true;
        }

        public bool CanGetItem()
        {
            return workTimer == -1;
        }

        bool ISingleItemContainer.TryOutputItem(Func<bool> rule, out Item item)
        {
            throw new NotImplementedException();
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            if (!containsItem.IsAir)
                tag.Add("containsItem", containsItem);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.TryGet("containsItem", out Item item))
                containsItem = item;
        }
    }
}
