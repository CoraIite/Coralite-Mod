
namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    /// <summary>
    /// 魔能生产器，能够存储，发送和生产魔能，如何生产请自定义
    /// </summary>
    public abstract class MagikeGenerator : MagikeSender
    {
        public MagikeGenerator(int magikeMax, int howManyCanConnect = 1) : base(magikeMax, howManyCanConnect)
        {

        }

        public override void Update()
        {
            Generate();
        }

        /// <summary>
        /// 如何生产魔能
        /// </summary>
        public virtual void Generate() { }
    }
}
