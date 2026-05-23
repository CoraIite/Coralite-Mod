using Coralite.Content.CoraliteNotes.Readfragment;
using Coralite.Content.Items.Gels;
using Coralite.Content.UI.UILib;
using Coralite.Core;
using Coralite.Core.Systems.KeySystem;
using System;

namespace Coralite.Content.CoraliteNotes.SlimeChapter1
{
    public class Slime1Knowledge : DangerousKnowledge
    {
        public override string Texture => AssetDirectory.SlimeEmperor + "SlimeEmperor_Head_Boss";

        public override int FirstPageInCoraliteNote => CoraliteNoteUIState.BookPanel.GetPageIndex<SlimePage1>();

        public override KnowledgeButtonType ButtonStyle => KnowledgeButtonType.Ball;

        public override int MaxDangerousLevel 
            => 3 + 2 + 1 * 3 + 4 + 4 + 5 + 1 + 2 + 1 + 2 + 2 + 1 + 2 + 2 + 1;

        public enum Dangerous
        {
            LifeMaxBonus_1 = 0,
            LifeMaxBonus_2,
            LifeMaxBonus_3,

            DefenceBonus_1,
            DefenceBonus_2,

            SpeedBonus1_1,
            SpeedBonus2_1,
            SpeedBonus3_1,

            WeaponLimit_4,
            ArmorLimit_4,
            HitLimit_3,
            HitLimit_S_5,

            FlippyBonus_1,
            FlippyBonus_S_2,
            AvatarBonus_1,
            AvatarBonus_S_2,

            CrownBonus_1,
            CrownBonus_S_2,
            ElasticBonus_1,
            GelBallBonus_P1_2,
            GelBallBonus_P2_2,
            SpilkeBallBonus_P1_2,
            SpilkeBallBonus_P2_2,
            StickyBonus_1,
        }

        public bool DangerousSet(Dangerous d)
            => DangerousTurnOn[(int)d];

        public override int[] GetDangerousLevels()
            => [
                1,2,3,
                1,2,
                1,1,1,
                4,4,3,5,
                1,2,1,2,
                1,2,1,2,2,2,2,1
                ];
        
        public override DangerousRewardInfo[] GetRewards()
        {
            return [
                new DangerousRewardInfo(new Terraria.Item(ModContent.ItemType<EmperorSlimeBoots>()),8),
                new DangerousRewardInfo(new Terraria.Item(ModContent.ItemType<RoyalGelCannon>()),16),
                new DangerousRewardInfo(new Terraria.Item(ModContent.ItemType<GelFlask>()),25),
                ];
        }

        public override string GetTexName(int index)
        {
            return (Dangerous)index switch
            {
                Dangerous.LifeMaxBonus_1 => AssetDirectory.NoteDangerousIcon+ "LifeMaxBonus_1",
                Dangerous.LifeMaxBonus_2 => AssetDirectory.NoteDangerousIcon + "LifeMaxBonus_2",
                Dangerous.LifeMaxBonus_3 => AssetDirectory.NoteDangerousIcon + "LifeMaxBonus_3",
                Dangerous.DefenceBonus_1 => AssetDirectory.NoteDangerousIcon + "DefenceBonus_1",
                Dangerous.DefenceBonus_2 => AssetDirectory.NoteDangerousIcon + "DefenceBonus_2",
                Dangerous.SpeedBonus1_1 or
                Dangerous.SpeedBonus2_1 or
                Dangerous.SpeedBonus3_1 => AssetDirectory.NoteDangerousIcon + "SpeedBonus1",
                Dangerous.WeaponLimit_4 => AssetDirectory.NoteDangerousIcon + "WeaponLimit",
                Dangerous.ArmorLimit_4 => AssetDirectory.NoteDangerousIcon + "ArmorLimit",
                Dangerous.HitLimit_3 => AssetDirectory.NoteDangerousIcon + "HitLimit",
                Dangerous.HitLimit_S_5 => AssetDirectory.NoteDangerousIcon + "HitLimit_S",
                Dangerous.FlippyBonus_1 or
                Dangerous.FlippyBonus_S_2 => AssetDirectory.NoteSlime1 + "FlippyBonus",
                Dangerous.AvatarBonus_1 or
                Dangerous.AvatarBonus_S_2 => AssetDirectory.NoteSlime1 + "AvatarBonus",
                Dangerous.CrownBonus_1 or
                Dangerous.CrownBonus_S_2 => AssetDirectory.NoteSlime1 + "CrownBonus",
                Dangerous.ElasticBonus_1 => AssetDirectory.NoteSlime1 + "ElasticBonus",
                Dangerous.GelBallBonus_P1_2 or
                Dangerous.GelBallBonus_P2_2 => AssetDirectory.NoteSlime1 + "GelBallBonus",
                Dangerous.SpilkeBallBonus_P1_2 or
                Dangerous.SpilkeBallBonus_P2_2 => AssetDirectory.NoteSlime1 + "SpilkeBallBonus",
                Dangerous.StickyBonus_1 => AssetDirectory.NoteSlime1 + "StickyBonus",
                _ => AssetDirectory.NoteDangerousIcon + "LifeMaxBonus_1",
            };
        }

        public override string GetTextName(int index)
            => Enum.GetName((Dangerous)index);

        public override UIPage[] GetUIPages()
        {
            return [
                new SlimePage1(),
                new SlimePage2(),
                new SlimePage3(),
                new Slime1DangerousPage(),
                ];
        }
    }
}
