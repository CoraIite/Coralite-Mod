using Coralite.Content.Items.Magike.Filters.PolarizedFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Coralite.Core.Systems.MagikeSystem.MagikeLevels
{
    /// <summary>
    /// 禁戒
    /// </summary>
    public class ForbiddenLevel : MagikeLevel
    {
        public override bool Available => Main.hardMode;

        public override float MagikeCostValue => 7.5f;

        public override int PolarizedFilterItemType => ModContent.ItemType<ForbiddenPolarizedFilter>();
    }
}
