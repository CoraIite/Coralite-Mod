using Coralite.Content.Items.Magike;
using Coralite.Core.Loaders;
using Coralite.Core.Network;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace Coralite.Content.UI.MagikeApparatusPanel
{
    public class FilterButton : UIElement
    {
        private float _scale = 1f;
        private int _index;

        public FilterButton(int index)
        {
            this.SetSize(40, 46);
            _index = index;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            Helper.PlayPitched("Fairy/FairyBottleClick", 0.3f, 0.4f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            if (MagikeApparatusPanel.CurrentEntity == null || !MagikeApparatusPanel.CurrentEntity.TryGetFilters(out var filter))
                return;

            if (filter.Count <= _index)
                return;

            Helper.PlayPitched("UI/Tick", 0.4f, 0);
            if (VaultUtils.isClient)
                Send_LeftClick_Data(MagikeApparatusPanel.CurrentEntity, (filter[_index] as MagikeFilter).whoAmI);
            else
                MagikeApparatusPanel.CurrentEntity.RemoveComponent(filter[_index]);

            UILoader.GetUIState<MagikeApparatusPanel>().Recalculate();
        }

        internal void Send_LeftClick_Data(MagikeTP tP, int whoAmI)
        {
            ModPacket modPacket = Coralite.Instance.GetPacket();
            modPacket.Write((byte)CoraliteNetWorkEnum.FilterRemoveButton_LeftClick);
            modPacket.Write(tP.ID);
            modPacket.Write(tP.Position.X);
            modPacket.Write(tP.Position.Y);
            modPacket.Write(whoAmI);
            modPacket.Send();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (MagikeApparatusPanel.CurrentEntity == null)
                return;

            Texture2D tex;
            var position = GetDimensions().Center();

            if (IsMouseHovering)
                _scale = Helper.Lerp(_scale, 1.2f, 0.2f);
            else
                _scale = Helper.Lerp(_scale, 1, 0.2f);

            if (MagikeApparatusPanel.CurrentEntity.TryGetFilters(out var filter) && filter.Count > 0)
            {
                tex = filter.Count <= _index
                     ? TextureAssets.Item[ModContent.ItemType<BasicFilter>()].Value
                     : TextureAssets.Item[(filter[_index] as MagikeFilter).ItemType].Value;
                if (IsMouseHovering && filter.Count > _index)
                {
                    Main.HoverItem = ContentSamples.ItemsByType[(filter[_index] as MagikeFilter).ItemType].Clone();
                    Main.hoverItemName = "a";
                }

                spriteBatch.Draw(tex, position, null, Color.White, 0, tex.Size() / 2, _scale, 0, 0);
                return;
            }

            tex = TextureAssets.Item[ModContent.ItemType<BasicFilter>()].Value;
            spriteBatch.Draw(tex, position, null, Color.White, 0, tex.Size() / 2, _scale, 0, 0);
        }
    }
}
