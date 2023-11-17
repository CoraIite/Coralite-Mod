using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public interface IMagikeSender_Line : IMagikeSender
    {
#pragma warning disable IDE1006 // 命名样式
        public Point16[] receiverPoints { get; }
        public int connectLenghMax { get; }

        public bool CanConnect(IMagikeContainer container);
        public bool CanConnect(Point16 targetPoint);


#pragma warning restore IDE1006 // 命名样式
    }
}
