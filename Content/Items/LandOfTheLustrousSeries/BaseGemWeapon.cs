using Coralite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Content.Items.LandOfTheLustrousSeries
{
    public abstract class BaseGemWeapon:ModItem
    {
        public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + Name;

        public sealed override void SetDefaults()
        {
            Item.DamageType = DamageClass.Magic;

            Item.noMelee = true;
            SetDefs();
        }

        public virtual void SetDefs() { }
    }
}
