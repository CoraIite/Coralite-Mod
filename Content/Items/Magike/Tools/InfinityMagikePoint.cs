using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Tools
{
    public class InfinityMagikePoint() : MagikeApparatusItem(TileType<InfinityMagikePointTile>(), Item.sellPrice(silver: 5)
            , ItemRarityID.Red, AssetDirectory.MagikeTools)
    {
    }

    public class InfinityMagikePointTile() : BaseMagikeTile
        (1, 1, Coralite.RedJadeRed, DustID.GoldCoin, topSoild: false)
    {
        public override string Texture => AssetDirectory.MagikeTiles + Name;
        public override int DropItemType => ItemType<InfinityMagikePoint>();

        public override CoraliteSetsSystem.MagikeTileType PlaceType => CoraliteSetsSystem.MagikeTileType.None;

        public override void QuickLoadAsset(ushort level) { }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) { }

        public override List<ushort> GetAllLevels()
        {
            return [
                CoraliteContent.MagikeLevelType<InfinityLevel>(),
                ];
        }

        public override string MagikeAmountText(MagikeTP tp)
        {
            return "∞";
        }
    }

    public class InfinityMagikePointTileEntity : BaseSenderTileEntity<InfinityMagikePointTile>
    {
        public override int ExtendFilterCapacity => 0;
        public override int MainComponentID => MagikeComponentID.MagikeContainer;

        public override ApparatusInformation AddInformation() => null;

        public override MagikeContainer GetStartContainer()
            => new InfinityMagikePointContainer();

        public override MagikeLinerSender GetStartSender()
            => new InfinityMagikePointSender();
    }

    public class InfinityMagikePointContainer : MagikeContainer
    {
        public override void Initialize()
        {
            MagikeMaxBase = 1;
            Magike = 1;
        }

        public override void AddMagike(int amount)
        {
        }

        public override bool FullMagike => true;
        public override bool HasMagike => true;

        public override string MagikeText => "∞";
        public override string MagikeMaxText => "∞";

        public override void ShowInUI(UIElement parent)
        {
            //添加显示在最上面的组件名称
            UIElement title = this.AddTitle(MagikeSystem.UITextID.MagikeContainerName, parent);

            //添加魔能量显示条，在左边
            ContainerBar bar = new(this);
            bar.Top.Set(title.Height.Pixels + 8, 0);
            parent.Append(bar);

            //其他的文本信息在右侧
            UIList list =
            [
                //魔能量
                this.NewTextBar(MagikeAmountTitle , parent),
                new InfinityContainerShow(this),
            ];

            list.SetSize(0, -title.Height.Pixels, 1, 1);
            list.SetTopLeft(title.Height.Pixels + 8, bar.Width.Pixels + 6);

            list.QuickInvisibleScrollbar();

            parent.Append(list);
        }
    }

    public class InfinityContainerShow : UIElement
    {
        protected MagikeContainer container;

        private const int LeftPaddling = 20;

        public InfinityContainerShow(MagikeContainer container)
        {
            this.container = container;

            ResetSize();
        }

        public void ResetSize()
        {
            Vector2 magikeSize = GetStringSize("∞");
            Vector2 magikeMaxSize = GetStringSize("∞");

            float width = magikeSize.X + 10;
            if (magikeMaxSize.X + 10 > width)
                width = magikeMaxSize.X + 10;

            if (width < 84)
                width = 84;

            Width.Set(width + LeftPaddling, 0);
            Height.Set(magikeSize.Y * 3.5f, 0);
        }

        private static Vector2 GetStringSize(string value)
        {
            TextSnippet[] textSnippets = [.. ChatManager.ParseMessage(value.ToString(), Color.White)];
            ChatManager.ConvertNormalSnippets(textSnippets);

            return ChatManager.GetStringSize(FontAssets.MouseText.Value, textSnippets, Vector2.One * 1.1f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle size = GetDimensions().ToRectangle();

            float per = size.Height / 3.5f;
            int width = size.Width - LeftPaddling;
            Vector2 pos = size.TopLeft() + new Vector2(30 + width / 2, per / 2);

            //绘制魔能量
            Utils.DrawBorderString(spriteBatch, "∞", pos + new Vector2(0, 4), Color.Orange
                , 1.1f, anchorx: 0.5f, anchory: 0.5f);

            pos += new Vector2(0, per * 0.75f);

            Color lineC = Color.Orange;

            //绘制中间那条线
            Texture2D lineTex = TextureAssets.FishingLine.Value;
            spriteBatch.Draw(lineTex, pos, null, lineC, 1.57f + 0.1f * MathF.Sin((float)Main.timeForVisualEffects * 0.01f), lineTex.Size() / 2
                , new Vector2(1.25f, width * 1.2f / lineTex.Height), 0, 0);

            pos += new Vector2(0, per * 0.75f);

            //绘制魔能上限
            Color color = Color.Orange;
            Utils.DrawBorderString(spriteBatch, "∞", pos + new Vector2(0, 4), color
                , 1.1f, anchorx: 0.5f, anchory: 0.5f);
        }
    }

    public class InfinityMagikePointSender : MagikeLinerSender
    {
        public override void Initialize()
        {
            MaxConnectBase = 8;
            ConnectLengthBase = 1000 * 16;
            SendDelayBase = 10;
        }

        public override bool GetSendAmount(MagikeContainer container, out int amount)
        {
            amount = 1;
            return true;
        }

        public override void Send(MagikeContainer selfMagikeContainer, Point8 position, int amount)
        {
            Point16 targetPos = Entity.Position + position;

            //如果无法获取物块实体就移除
            if (!MagikeHelper.TryGetEntityWithTopLeft(targetPos, out MagikeTP receiverEntity))
                goto remove;

            //如果不是魔能容器那么就丢掉喽
            if (!receiverEntity.IsMagikeContainer())
                goto remove;

            MagikeContainer receiver = receiverEntity.GetMagikeContainer();
            if (receiver.FullMagike)
                return;

            receiver.FullChargeMagike();
            OnSend(Entity.Position, targetPos);

            return;
        remove:
            RemoveReceiver(position);
        }
    }
}
