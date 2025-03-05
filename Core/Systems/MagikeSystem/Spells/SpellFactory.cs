using Coralite.Core.Systems.MagikeSystem.Components;

namespace Coralite.Core.Systems.MagikeSystem.Spells
{
    public class SpellFactory : MagikeFactory
    {
        public override bool CanActivated_SpecialCheck(out string text)
        {
            text = "";

            if (Entity.TryGetFilters(out var filters))
            {
                bool hasSuccess = false;
                foreach (var filter in filters)
                {
                    if (filter is SpellFilter spellFilter)//检测法术滤镜是否能启动
                    {
                        if (spellFilter.IsWorking)
                            continue;

                        if (spellFilter.CanActivated_SpecialCheck())
                            hasSuccess = true;
                    }
                }

                return hasSuccess;
            }
            else
            {
                return false;
            }
        }

        public override void StarkWork()
        {
            if (Entity.TryGetFilters(out var filters))
                foreach (var filter in filters)
                    if (filter is SpellFilter spellFilter)//启动所有的法术滤镜
                    {
                        if (spellFilter.IsWorking)
                            continue;

                        spellFilter.StartWork();
                    }
        }
    }
}
