using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Core.Systems.Trails;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ObjectData;
using static Coralite.Helpers.MagikeHelper;

namespace Coralite.Core.Systems.MagikeSystem.Particles
{
    /// <summary>
    /// 绘制本体的不断收缩圆圈
    /// 绘制连接点位的不变的圆圈
    /// 绘制本体圆圈中间的法阵
    /// 绘制连接点位的圆圈与本体之间的连接线
    /// </summary>
    public class CraftParticle:Particle,IDrawParticlePrimitive
    {
        public override string Texture => AssetDirectory.OtherProjectiles+ "Circle3";

        private Point16 _pos;
        private int _totalTime;

        /// <summary>
        /// 各个接收器与本体之间的连接拖尾
        /// </summary>
        private List<Trail> _trails;
        private List<OtherConnectData> otherConnects;

        /// <summary>
        /// 法阵的点
        /// </summary>
        private Vector2[][] points;

        private struct OtherConnectData
        {
            public OtherConnectData(Point16 pos)
            {
                _pos = pos;
                _ = 1;
            }

            private readonly Point16 _pos;
            private readonly Vector2 _offset;
        }

        public override void Update()
        {
            if (!TryGetEntity(_pos,out MagikeTileEntity entity))
            {
                active=false; 
                return;
            }

            fadeIn--;
            if (fadeIn < 0)
                active = false;
        }

        public static CraftParticle Spawn(Point16 pos, int craftTime)
        {
            Tile t = Framing.GetTileSafely(pos);
            ModTile mt = TileLoader.GetTile(t.TileType);
            if (mt == null || mt is not BaseCraftAltarTile cat)
                return null;

            //获取alt对应的偏转量
            GetMagikeAlternateData(pos.X, pos.Y, out TileObjectData data, out MagikeAlternateStyle alternate);
            
            Vector2 position = Helper.GetMagikeTileCenter(pos)+cat.GetFloatingOffset(alternate);
            CraftParticle p = NewParticle<CraftParticle>(position,Vector2.Zero,Coralite.MagicCrystalPink);

            p._pos = pos;
            p.fadeIn =p._totalTime= craftTime;

            return p;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public void DrawPrimitives()
        {
        }
    }
}
