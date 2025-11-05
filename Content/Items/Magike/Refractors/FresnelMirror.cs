using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class FresnelMirror() : MagikeApparatusItem(TileType<FresnelMirrorTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeRefractors)
    {

    }

    public class FresnelMirrorTile() : BaseMagikeTile
    (3, 3, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;
        public override int DropItemType => ItemType<FresnelMirror>();

        public override CoraliteSetsSystem.MagikeTileType PlaceType => CoraliteSetsSystem.MagikeTileType.None;

        public override MALevel[] GetAllLevels()
        {
            return [
                MALevel.None,
                MALevel.MagicCrystal,
                MALevel.Glistent,
                MALevel.CrystallineMagike,
                MALevel.Feather,
                ];
        }

        public override void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity, MALevel level)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset+new Vector2(0,2);

            //虽然一般不会没有 但是还是检测一下
            if (!entity.TryGetComponent(MagikeComponentID.MagikeSender, out MagikeLinerSender sender))
                return;

            //绘制旋转的框框
            tex.QuickCenteredDraw(spriteBatch, new Rectangle(0, 0, 3, 1)
                , drawPos, lightColor, (float)(Main.timeForVisualEffects * 0.1f));
            //绘制旋转的框框的高光
            tex.QuickCenteredDraw(spriteBatch, new Rectangle(1, 0, 3, 1)
                , drawPos, lightColor, MathF.Sin((float)(Main.timeForVisualEffects * 0.1f)));
            //绘制外边的框框
            tex.QuickCenteredDraw(spriteBatch, new Rectangle(2, 0, 3, 1)
                , drawPos, lightColor, 0);
        }
    }

    public class FresnelMirrorEntity : MagikeTP
    {
        public override int TargetTileID => TileType<FresnelMirrorTile>();

        public override int MainComponentID => MagikeComponentID.MagikeSender;

        public override void InitializeBeginningComponent()
        {
            AddComponent(new FresnelMirrorContainer());
            AddComponent(new LASERSender());
        }
    }

    public class FresnelMirrorContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.MagicCrystal => 240,
                MALevel.Glistent => 720,
                MALevel.CrystallineMagike => 5760,
                MALevel.Hallow => 7200,
                MALevel.Feather => 20480,
                _ => 0,
            };
            LimitMagikeAmount();
        }
    }

    public class FresnelMirrorSender : MagikeSender, IUIShowable, IUpgradeable
    {
        /// <summary>
        /// 发送半径，一个正方形<br></br>
        /// 与其他的半径不一样，这个以格为单位
        /// </summary>
        public int SendRadius { get;private set; }

        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public virtual bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public void ShowInUI(UIElement parent)
        {
        }

        public override void Update()
        {
        }

        public void Upgrade(MALevel incomeLevel)
        {
            switch (incomeLevel)
            {
                default:
                case MALevel.None:
                    SendDelayBase = -1;
                    UnitDeliveryBase = 0;
                    SendDelayBase = -1;
                    SendRadius = 0;
                    break;
                case MALevel.MagicCrystal:
                    SendDelayBase = 60 * 2 + 30;
                    SendRadius = 4;
                    break;
                case MALevel.Glistent:
                    SendDelayBase = 60 * 2;
                    SendRadius = 4;
                    break;
                case MALevel.CrystallineMagike:
                    SendDelayBase = 60 * 1 + 30;
                    SendRadius =5;
                    break;
                case MALevel.CrystallineMagike:
                    SendDelayBase = 60 * 1 + 30;
                    SendRadius =5;
                    break;
            }

            Timer = SendDelay;
        }
    }

}
