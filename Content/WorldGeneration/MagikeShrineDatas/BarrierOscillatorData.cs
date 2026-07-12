using InnoVault.GameSystem;
using System.IO;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;

namespace Coralite.Content.WorldGeneration.MagikeShrineDatas
{
    public class BarrierOscillatorData : SaveStructure
    {
        public override string SavePath => Path.Combine(StructurePath, "BarrierOscillatorData_v1.nbt");
        public override void Load() => Mod.EnsureFileFromMod("Datas/StructureDatas/BarrierOscillatorData_v1.nbt", SavePath);
#if DEBUG
        public override void SaveData(TagCompound tag)
            => SaveRegion(tag, new Point16(2324, 129).GetRectangleFromPoints(new Point16(2326, 131)));
#endif
        public override void LoadData(TagCompound tag)
        {
            RegionSaveData region = tag.GetRegionSaveData();

            region.ApplyToWorld((short)CoraliteWorld.BarrierOscillatorPos.X, (short)CoraliteWorld.BarrierOscillatorPos.Y);

            TagCache.Invalidate(SavePath);//释放缓存
        }
    }
}
