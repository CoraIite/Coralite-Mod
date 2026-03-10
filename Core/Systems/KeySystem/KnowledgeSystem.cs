using Coralite.Content.CoraliteNotes;
using Coralite.Core.Loaders;
using Terraria;
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
        public static void CheckForUnlock<T>(Vector2 position, Color color) where T : Knowledge
        {
            Knowledge keyKnowledge = CoraliteContent.GetKnowledge<T>();
            if (!keyKnowledge.Unlock)
            {
                keyKnowledge.UnlockKnowledge();
                SpawnKnowledgeUnlockText(position, color);
            }
        }

        public override void PostSetupContent()
        {
            KnowledgeLoader.SetUp();

            if (!Main.dedServ)
                UILoader.GetUIState<CoraliteNoteUIState>().Init();
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
