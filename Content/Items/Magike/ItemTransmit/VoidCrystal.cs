using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.ItemTransmit
{
    public class VoidCrystal : BaseMagikePlaceableItem, IMagikePolymerizable, IMagikeFactoryItem
    {
        public VoidCrystal() : base(TileType<VoidCrystalTile>(), Item.sellPrice(0, 1, 0, 0)
            , RarityType<MagicCrystalRarity>(), 50, AssetDirectory.MagikeFactories)
        { }

        public override int MagikeMax => 250;
        public string WorkTimeMax => "5";
        public string WorkCost => "5（发送物品）/ 10（吸取物品）";

        public void AddMagikePolymerizeRecipe()
        {
            PolymerizeRecipe.CreateRecipe<VoidCrystal>(150)
                 .SetMainItem(ItemID.Bone, 10)
                 .AddIngredient(ItemID.JungleSpores, 5)
                 .AddIngredient<MagicCrystal>(10)
                 .Register();
        }
    }

    public class VoidCrystalTile : BaseItemSiphonTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoFail[Type] = true; //不会出现挖掘失败的情况
            TileID.Sets.IgnoredInHouseScore[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);

            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(GetInstance<VoidCrystalEntity>().Hook_AfterPlacement, -1, 0, true);

            TileObjectData.addTile(Type);

            AddMapEntry(Coralite.MagicCrystalPink);
            DustType = DustID.CrystalSerpent_Pink;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 6)
            {
                frameCounter = 0;
                frame++;
                if (frame > 7)
                    frame = 0;
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameYOffset = 0;
            frameXOffset = Main.tileFrame[type] * 18;
        }
    }

    public class VoidCrystalEntity : MagikeItemSiphon
    {
        public VoidCrystalEntity() : base(250, 1, 1, 16 * 16, 5, 10, 1, 60 * 5, 16 * 5)
        { }

        public override Color MainColor => Coralite.MagicCrystalPink;

        public override ushort TileType => (ushort)TileType<VoidCrystalTile>();

        public override void SpawnDustDuringWork()
        {
            Vector2 _position = GetWorldPosition() + new Vector2(-8, 8);
            Lighting.AddLight(_position, 0.4f, 0.2f, 0.9f);

            if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool(2))
                {
                    Vector2 vector = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                    vector *= new Vector2(0.5f, 1f);
                    Dust dust = Dust.NewDustDirect(_position - vector * 30f, 0, 0, Utils.SelectRandom(Main.rand, 86, 88));
                    dust.noGravity = true;
                    dust.noLightEmittence = true;
                    dust.position = _position - vector.SafeNormalize(Vector2.Zero) * Main.rand.Next(10, 21);
                    dust.velocity = vector.RotatedBy(MathHelper.PiOver2) * 2f;
                    dust.scale = 0.5f + Main.rand.NextFloat();
                    dust.fadeIn = 0.5f;
                    dust.customData = this;
                    dust.position += dust.velocity * 10f;
                    dust.velocity *= -1f;
                }
                else
                {
                    Vector2 vector2 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                    vector2 *= new Vector2(0.5f, 1f);
                    Dust dust2 = Dust.NewDustDirect(_position - vector2 * 30f, 0, 0, Utils.SelectRandom(Main.rand, 86, 88));
                    dust2.noGravity = true;
                    dust2.noLightEmittence = true;
                    dust2.position = _position - vector2.SafeNormalize(Vector2.Zero) * Main.rand.Next(5, 10);
                    dust2.velocity = vector2.RotatedBy(-MathHelper.PiOver2) * 3f;
                    dust2.scale = 0.5f + Main.rand.NextFloat();
                    dust2.fadeIn = 0.5f;
                    dust2.customData = this;
                    dust2.position += dust2.velocity * 10f;
                    dust2.velocity *= -1f;
                }
            }

        }
    }
}
