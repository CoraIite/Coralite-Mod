using Coralite.Content.CoraliteNotes;
using Coralite.Content.CoraliteNotes.LandOfTheLustrousChapter;
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

        public string LocalizationCategory => "Systems";

        public override void Load()
        {
            if (!Main.dedServ)
            {
                NewKnowledgeUnlock = this.GetLocalization(nameof(NewKnowledgeUnlock));
                ClickToJump = this.GetLocalization(nameof(ClickToJump));
            }
        }

        public override void Unload()
        {
            NewKnowledgeUnlock = null;
            ClickToJump = null;
        }

        /// <summary>
        /// 用于生成获取知识时的跳字
        /// </summary>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public static void SpawnKnowledgeUnlockText(Vector2 position, Color color)
        {
            if (VaultUtils.isServer)
                return;

            CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, 1, 1)
                , color, NewKnowledgeUnlock.Value, true);
        }

        /// <summary>
        /// 检测并解锁知识，解锁时跳字
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public static void CheckForUnlock(int id, Vector2 position, Color color)
        {
            Knowledge keyKnowledge = CoraliteContent.GetKnowledge(id);

            if (VaultUtils.isClient)
            {

            }

            if (!keyKnowledge.Unlock)
            {
                keyKnowledge.UnlockKnowledge();
                SpawnKnowledgeUnlockText(position, color);
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
                //SpawnKnowledgeUnlockText(position, color);
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
                    JumpToPage<LandOfTheLustrousKnowledge,LandOfTheLustrousPage2>();
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
