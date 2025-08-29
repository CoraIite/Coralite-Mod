using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using static Coralite.Helpers.MagikeHelper;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Factorys
{
    /// <summary>
    /// 药剂台
    /// </summary>
    public class ApothecaryTable() : MagikeApparatusItem(TileType<ApothecaryTableTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeFactories)
    {
        public static LocalizedText NotHavePotion { get; private set; }

        public override void Load()
        {
            NotHavePotion = this.GetLocalization(nameof(NotHavePotion));
        }

        public override void Unload()
        {
            NotHavePotion = null;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Basalt>(10)
                .AddIngredient<MagicCrystalBlock>(5)
                .AddCondition(CoraliteConditions.LearnedMagikeBase)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class ApothecaryTableTile() : BaseChargerTile
        (2, 2, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeFactoryTiles + Name;
        public override int DropItemType => ItemType<ApothecaryTable>();

        public override MALevel[] GetAllLevels()
        {
            return
            [
                MALevel.None,
                MALevel.MagicCrystal,
            ];
        }

        public override void QuickLoadAsset(MALevel level) { }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) { }
    }

    public class ApothecaryTableTileEntity() : MagikeTP()
    {
        public sealed override int TargetTileID => TileType<ApothecaryTableTile>();

        public override int MainComponentID => MagikeComponentID.MagikeFactory;

        public override void InitializeBeginningComponent()
        {
            AddComponent(GetStartContainer());
            AddComponent(GetStartFactory());
            AddComponent(GetStartItemContainer());
        }

        public MagikeContainer GetStartContainer()
            => new ApothecaryTableContainer();

        public MagikeFactory GetStartFactory()
            => new ApothecaryTableFactory();

        public ItemContainer GetStartItemContainer()
            => new ItemContainer()
            {
                CapacityBase = 4,
            };
    }

    public class ApothecaryTableContainer : UpgradeableContainer
    {
        public override void Upgrade(MALevel incomeLevel)
        {
            MagikeMaxBase = incomeLevel switch
            {
                MALevel.MagicCrystal => 100,
                _ => 0,
            };
            LimitMagikeAmount();

            //AntiMagikeMaxBase = MagikeMaxBase * 2;
            //LimitAntiMagikeAmount();
        }
    }

    public class ApothecaryTableFactory : MagikeFactory, IUIShowable, IUpgradeable
    {
        public bool CanUpgrade(MALevel incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public override void Initialize()
        {
            Upgrade(MALevel.None);
        }

        public override bool CanActivated_SpecialCheck(out string text)
        {
            text = "";

            var magikeContainer = Entity.GetMagikeContainer();
            if (!magikeContainer.FullMagike)
            {
                text = StoneMaker.MagikeNotEnough.Value;
                return false;
            }

            if (Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
            {
                foreach (var item in container.Items)//有药水就返回true
                    if (!item.IsAir && item.buffType > 0 && item.buffTime > 0)
                        return true;
            }

            text = ApothecaryTable.NotHavePotion.Value;
            return false;
        }

        public override void Work()
        {
            Point16 point = Entity.Position;
            Vector2 center = point.ToWorldCoordinates(16, 16);
            Rectangle selfRect = Utils.CenteredRectangle(center, new Vector2(16 * 40));

            if (!Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
                return;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player p = Main.player[i];
                if (p.Alives() && p.getRect().Intersects(selfRect))//为玩家加BUFF
                    foreach (var item in container.Items)
                        if (!item.IsAir && item.buffType > 0 && item.buffTime > 0)
                            p.AddBuff(item.buffType, item.buffTime);
            }

            Entity.GetMagikeContainer().ClearMagike();

            for (int i = 0; i < 50; i++)
            {
                Dust d = Dust.NewDustPerfect(center, DustID.FireworksRGB, (i * MathHelper.TwoPi / 50).ToRotationVector2() * Main.rand.NextFloat(4, 20), newColor: Coralite.MagicCrystalPink);
                d.noGravity = true;
            }
        }

        public void ShowInUI(UIElement parent)
        {
            //添加显示在最上面的组件名称
            UIElement title = this.AddTitle(MagikeSystem.UITextID.ApothecaryTable, parent);

            var text = this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.ApothecaryTableDescription), parent);
            text.SetTopLeft(title.Height.Pixels + 5, 0);

            parent.Append(text);

            if (Entity.TryGetComponent(MagikeComponentID.ItemContainer, out ItemContainer container))
            {
                UIGrid grid = new()
                {
                    OverflowHidden = false
                };

                for (int i = 0; i < container.Items.Length; i++)
                {
                    ItemContainerSlot slot = new(container, i);
                    grid.Add(slot);
                }

                grid.SetSize(0, -text.Top.Pixels - text.Height.Pixels, 1, 1);
                grid.SetTopLeft(text.Top.Pixels + text.Height.Pixels + 6, 0);

                parent.Append(grid);
            }
        }

        public void Upgrade(MALevel incomeLevel)
        {
            WorkTimeBase = 5;
        }
    }
}
