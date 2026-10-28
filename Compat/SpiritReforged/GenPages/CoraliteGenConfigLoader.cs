namespace Coralite.Compat.SpiritReforged.GenPages
{
    public class CoraliteGenConfigLoader : ILoadable
    {
        void ILoadable.Load(Mod mod)
        {
            if (!ModLoader.HasMod("SpiritReforged"))
                return;

            LoadBindings();
        }

        [JITWhenModsEnabled("SpiritReforged")]
        private static void LoadBindings() => CoraliteGenerationLoading.LoadBindings();

        void ILoadable.Unload()
        {
        }
    }
}
