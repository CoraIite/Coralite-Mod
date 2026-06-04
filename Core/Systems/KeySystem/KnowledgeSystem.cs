using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter;
using Coralite.Content.CoraliteNotes.SwordChapter;
using Coralite.Content.UI.NewKnowledgeUnlock;
using Coralite.Content.UI.UILib;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Core.Systems.KeySystem
{
    public class KnowledgeSystem : ModSystem, ILocalizedModType
    {
        public static LocalizedText NewKnowledgeUnlock { get; private set; }
        public static LocalizedText ClickToJump { get; private set; }
        public static LocalizedText ChallengeFailText { get; private set; }
        public static LocalizedText CurrentChallengeLevel { get; private set; }

        public string LocalizationCategory => "Systems";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                NewKnowledgeUnlock = this.GetLocalization(nameof(NewKnowledgeUnlock));
                ClickToJump = this.GetLocalization(nameof(ClickToJump));
                ChallengeFailText = this.GetLocalization(nameof(ChallengeFailText));
                CurrentChallengeLevel = this.GetLocalization(nameof(CurrentChallengeLevel));
            }
        }

        public override void Unload()
        {
            NewKnowledgeUnlock = null;
            ClickToJump = null;
            ChallengeFailText = null;
            CurrentChallengeLevel = null;
        }

        /// <summary>
        /// 用于生成获取知识时的跳字
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        //public static void KnowledgeUnlockUI(Vector2 position, Color color)
        //{
        //    if (VaultUtils.isServer)
        //        return;

        //    CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, 1, 1)
        //        , color, NewKnowledgeUnlock.Value, true);
        //}

        /// <summary>
        /// 检测并解锁知识，解锁时跳字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public static void CheckForUnlock(int id, Color color)
        {
            Knowledge keyKnowledge = CoraliteContent.GetKnowledge(id);

            if (!keyKnowledge.Unlock)
            {
                keyKnowledge.UnlockKnowledge();
                NewKnowledgeState.AddNewTip(id, color);
            }
        }

        /// <summary>
        /// 检测并解锁知识，解锁时跳字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public static void CheckForUnlock<T>(Color color) where T : Knowledge
        {
            Knowledge keyKnowledge = CoraliteContent.GetKnowledge<T>();
            if (!keyKnowledge.Unlock)
            {
                keyKnowledge.UnlockKnowledge();
                NewKnowledgeState.AddNewTip(keyKnowledge, color);
            }
        }

        public override void PostSetupContent()
        {
            KnowledgeLoader.SetUp();

            if (!Main.dedServ)
                UILoader.GetUIState<CoraliteNoteUIState>().Init();
        }

        /// <summary>
        /// 是否能在珊瑚笔记中查阅
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static bool CanConsultInCoraliteNote(Item i)
        {
            if (i.type < ItemID.Count)
            {
                var knowledge = CanConsultInCoraliteNote_Vanilla(i);
                if (knowledge != null)
                    return knowledge.Unlock;

                return false;
            }

            if (i.ModItem is IConsultableItem consultableItem)
                return consultableItem.GetKnowledge.Unlock;

            return false;
        }

        public static Knowledge CanConsultInCoraliteNote_Vanilla(Item i)
        {
            return i.type switch
            {
                ItemID.Amethyst or ItemID.Diamond or ItemID.Topaz or ItemID.Sapphire or ItemID.WhitePearl
                    => CoraliteContent.GetKnowledge<LandOfTheLustrousKnowledge>(),
                ItemID.CopperShortsword 
                or ItemID.EnchantedSword 
                or ItemID.BeeKeeper 
                or ItemID.Starfury
                or ItemID.Seedler
                or ItemID.TheHorsemansBlade
                or ItemID.InfluxWaver
                or ItemID.StarWrath
                or ItemID.Meowmere

                or ItemID.LightsBane
                or ItemID.BloodButcherer
                or ItemID.FieryGreatsword
                or ItemID.Muramasa
                or ItemID.BladeofGrass
                or ItemID.NightsEdge
                or ItemID.TrueNightsEdge
                or ItemID.BrokenHeroSword
                or ItemID.Excalibur
                or ItemID.TrueExcalibur
                or ItemID.TerraBlade

                or ItemID.Zenith
                    => CoraliteContent.GetKnowledge<SwordKnowledge>(),
                _ => null,
            };
        }

        /// <summary>
        /// 跳转到对应的页数
        /// </summary>
        /// <param name="i"></param>
        public static void ConsultInCoraliteNote(Item i)
        {
            if (i.type < ItemID.Count)
            {
                ConsultInCoraliteNote_Vanilla(i);
                return;
            }
            if (i.ModItem is IConsultableItem consultableItem)
            {
                UILoader.GetUIState<CoraliteNoteUIState>().Recalculate();
                JumpToPage(consultableItem.GetKnowledge, consultableItem.GetPageIndex);
            }
        }


        public static void ConsultInCoraliteNote_Vanilla(Item i)
        {
            switch (i.type)
            {
                default:
                    return;
                case ItemID.Amethyst:
                case ItemID.Diamond:
                case ItemID.Topaz:
                case ItemID.Sapphire:
                case ItemID.WhitePearl:
                    UILoader.GetUIState<CoraliteNoteUIState>().Recalculate();
                    JumpToPage<LandOfTheLustrousKnowledge, LandOfTheLustrousPage2>();
                    return;
                case ItemID.CopperShortsword:
                case ItemID.EnchantedSword:
                case ItemID.BeeKeeper:
                case ItemID.Starfury:
                case ItemID.Seedler:
                case ItemID.TheHorsemansBlade:
                case ItemID.InfluxWaver:
                case ItemID.StarWrath:
                case ItemID.Meowmere:
                case ItemID.LightsBane:
                case ItemID.BloodButcherer:
                case ItemID.FieryGreatsword:
                case ItemID.Muramasa:
                case ItemID.BladeofGrass:
                case ItemID.NightsEdge:
                case ItemID.TrueNightsEdge:
                case ItemID.BrokenHeroSword:
                case ItemID.Excalibur:
                case ItemID.TrueExcalibur:
                case ItemID.TerraBlade:
                case ItemID.Zenith:
                    UILoader.GetUIState<CoraliteNoteUIState>().Recalculate();
                    JumpToPage<SwordKnowledge, SwordCollect>();
                    return;
            }
        }

        /// <summary>
        /// 打开珊瑚笔记并翻到对应页数
        /// </summary>
        public static void JumpToPage(Knowledge knowledge, int pageIndex)
        {
            int index = pageIndex;
            if (index < 0)
                index = knowledge.FirstPageInCoraliteNote;

            Helper.PlayPitched("Misc/Pages", 0.4f, 0f, Main.LocalPlayer.Center);

            if (index >= 0)
            {
                UILoader.GetUIState<CoraliteNoteUIState>().Recalculate();

                var ui = UILoader.GetUIState<CoraliteNoteUIState>();
                if (!ui.visible)
                    ui.OpenBook();

                CoraliteNoteUIState.BookPanel.CurrentDrawingPage = index;
                Main.playerInventory = false;

                ui.Recalculate();
            }
        }

        /// <summary>
        /// 打开珊瑚笔记并翻到对应页数，一般原版用这个
        /// </summary>
        public static void JumpToPage<TKnowledge, TPage>()
            where TKnowledge : Knowledge
            where TPage : UIPage
        {
            int index = CoraliteNoteUIState.BookPanel.GetPageIndex<TPage>();
            if (index < 0)
                index = CoraliteContent.GetKnowledge<TKnowledge>().FirstPageInCoraliteNote;

            Helper.PlayPitched("Misc/Pages", 0.4f, 0f, Main.LocalPlayer.Center);

            if (index >= 0)
            {
                UILoader.GetUIState<CoraliteNoteUIState>().Recalculate();

                var ui = UILoader.GetUIState<CoraliteNoteUIState>();
                if (!ui.visible)
                    ui.OpenBook();

                CoraliteNoteUIState.BookPanel.CurrentDrawingPage = index;
                Main.playerInventory = false;

                ui.Recalculate();
            }
        }


        //public override void PostWorldGen()
        //{
        //    foreach (var knowledge in KeyKnowledgeLoader.knowledges)
        //    {
        //        knowledge.Value.Unlock = false;
        //    }
        //}

        //public override void SaveWorldData(TagCompound tag)
        //{
        //tag.Add("ThereNeedOneThingToSave!!!", 0);
        //foreach (var knowledge in KeyKnowledgeLoader.knowledgesF)
        //{
        //    knowledge.Value.SaveSelfData(tag);
        //}
        //}

        //public override void LoadWorldData(TagCompound tag)
        //{
        //foreach (var knowledge in KeyKnowledgeLoader.knowledgesF)
        //{
        //    knowledge.Value.LoadSelfData(tag);
        //}
        //}
    }
}
