using Coralite.Core.Systems.MagikeSystem.Components;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using Terraria;
using Terraria.GameContent;

namespace Coralite.Core.Systems.MagikeSystem.TileEntities
{
    public abstract class Pedestal<TModTile>() : MagikeTP()
        where TModTile : ModTile
    {
        public sealed override int TargetTileID => (ushort)ModContent.TileType<TModTile>();
        private bool onNet;
        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartItemContainer());
        }

        public abstract ItemContainer GetStartItemContainer();

        public override void Update()
        {
            base.Update();
            if (TryGetComponent<ItemContainer>(MagikeComponentID.ItemContainer, out var itemContainer))
            {
                if ((Main.MouseWorld - PosInWorld).LengthSquared() < 1600)
                {
                    itemContainer.value += 1;
                    if (onNet)
                    {
                        if (!Main.dedServ)
                        {
                            SendData();
                        }
                    }
                    onNet = false;
                }
                else
                {
                    onNet = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (TryGetComponent<ItemContainer>(MagikeComponentID.ItemContainer, out var itemContainer))
            {
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, itemContainer.value.ToString()
                , PosInWorld.X - Main.screenPosition.X, PosInWorld.Y - 60 - Main.screenPosition.Y
                , Color.White, Color.Black, new Vector2(0.3f), 1.4f);
            }
        }
    }
}
