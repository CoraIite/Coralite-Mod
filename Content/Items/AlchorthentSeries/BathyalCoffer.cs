using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public class BathyalCoffer
    {
        public static LineDrawer NewWaterAlchSymbol()
        {
            float height = 2 * 1.732f - 8 / 3f;

            return new LineDrawer([
                    new LineDrawer.StraightLine(new Vector2(0,8/3f),new Vector2(2,-height)),
                new LineDrawer.StraightLine(new Vector2(2,-height),new Vector2(-2,-height)),
                new LineDrawer.StraightLine(new Vector2(-2,-height),new Vector2(0,8/3f)),
                ]);
        }
    }
}
