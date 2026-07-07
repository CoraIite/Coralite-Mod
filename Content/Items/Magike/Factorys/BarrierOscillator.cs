using Coralite.Content.DamageClasses;
using Coralite.Content.Dusts;
using Coralite.Content.Items.Magike.Refractors;
using Coralite.Content.Items.MagikeSeries2;
using Coralite.Content.Raritys;
using Coralite.Content.Tiles.MagikeSeries2;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Attributes;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;
using Terraria.UI;
using static Coralite.Helpers.MagikeHelper;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Factorys
{
    public class BarrierOscillator() : MagikeApparatusItem(TileType<BarrierOscillatorTile>(), Item.sellPrice(silver: 10)
        , RarityType<CrystallineMagikeRarity>(), AssetDirectory.MagikeFactories), IMagikeCraftable
    {
        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<CrystallineMagike, BarrierOscillator>(CalculateMagikeCost<BrilliantLevel>(6, 10), 5)
                .AddIngredient<SkarnBrick>(10)
                .Register();
        }
    }

    public class BarrierOscillatorTile() : BaseMagikeTile
        (3, 3, Coralite.CrystallinePurple, DustType<SkarnDust>())
    {
        public override string Texture => AssetDirectory.MagikeFactoryTiles + Name;
        public override int DropItemType => ItemType<BarrierOscillator>();

        public override CoraliteSetsSystem.MagikeTileType PlaceType => CoraliteSetsSystem.MagikeTileType.None;

        public override List<ushort> GetAllLevels()
        {
            return
            [
                BrilliantLevel.ID,
            ];
        }

        public override void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity, ushort level)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset + new Vector2(0, 2);

            if (entity.TryGetComponent(MagikeComponentID.MagikeFactory,out BarrierOscillatorFactory factory))
            {
                tex.QuickCenteredDraw(spriteBatch, new Rectangle(0, factory.frame, 1, 19) , drawPos, lightColor, 0);
            }
        }
    }

    public class BarrierOscillatorEntity : MagikeTP
    {
        public override int TargetTileID => TileType<BarrierOscillatorTile>();

        public override int MainComponentID => MagikeComponentID.MagikeSender;

        public override void InitializeBeginningComponent()
        {
            AddComponent(new BarrierOscillatorContainer());
            AddComponent(new BarrierOscillatorFactory());
        }
    }

    public class BarrierOscillatorContainer : UpgradeableContainer<BarrierOscillatorTile>
    {
    }

    public class BarrierOscillatorFactory : MagikeFactory, IUIShowable, IUpgradeable, IUpgradeLoadable
    {
        /// <summary>
        /// 工作消耗
        /// </summary>
        [UpgradeableProp]
        public int WorkCost { get; set; }

        public int TileType => TileType<BarrierOscillatorTile>();

        public byte frame;
        public byte frameCounter;

        #region 升级部分

        public bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public override void Initialize()
        {
            InitializeLevel();
        }

        public void InitializeLevel()
        {
            WorkTimeBase = -1;
            WorkCost = 0;
        }

        public void Upgrade(ushort incomeLevel)
        {
            string name = this.GetDataPreName();
            WorkTimeBase = MagikeSystem.GetLevelData4Time(incomeLevel, name + nameof(WorkTimeBase));
            WorkCost = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(WorkCost));
        }

        #endregion

        public override bool CanActivated_SpecialCheck(out string text)
        {
            text = "";

            if (Entity.GetMagikeContainer().Magike < WorkCost)
            {
                text = MagikeSystem.MagikeNotEnough.Value;
                return false;
            }

            return true;
        }

        public override void Update()
        {
            if (!IsWorking && (frame > 0 && frame < 18))
            {
                if (++frameCounter > 3)
                {
                    frameCounter = 0;
                    frame++;
                }
            }

            base.Update();
        }

        public override void Work()
        {
            string text = "";

            Point16 point = Entity.Position;
            GetMagikeAlternateData(point.X, point.Y, out TileObjectData data, out _);

            if (Entity.GetMagikeContainer().Magike < WorkCost)
                text = MagikeSystem.MagikeNotEnough.Value;

            if (!string.IsNullOrEmpty(text))
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Coralite.CrystallinePurple,
                    Text = text,
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, Helper.GetMagikeTileCenter(Entity.Position) - (Vector2.UnitY * 32));
                return;
            }

            Entity.GetMagikeContainer().ReduceMagike(WorkCost);

            //范围摧毁
            const int width = 19;

            point += new Point16(data.Width / 2-width/2, data.Height / 2 - width / 2);

            for (int i = 0; i < width; i++)
                for (int j = 0; j < width; j++)//破坏屏障
                {
                    Point16 p = point + new Point16(i, j);
                    Tile t = Framing.GetTileSafely(p);
                    if (t.TileType == TileType<CrystallineBarrier>())
                    {
                        WorldGen.KillTile(p.X, p.Y);

                        WorldGen.PlaceTile(p.X, p.Y, TileType<CrystallineBarrierTemporary>(), true, true);
                    }
                }

            Vector2 postion = (Entity.Position + new Point16(data.Width / 2, data.Height / 2)).ToWorldCoordinates();

            //弹开NPC
            foreach (var npc in Main.ActiveNPCs)
                if (!npc.friendly && !npc.dontTakeDamage)
                    npc.SimpleStrikeNPC(200, postion.X > npc.Center.X ? -1 : 1, false, 20, MagikeDamage.Instance);
        }

        public override void StarkWork()
        {
            base.StarkWork();
            frame = 0;
        }

        public override void OnWorking()
        {
            if (Timer % 3 == 0)
                if (frame < 13)
                    frame++;

            if (Timer % (WorkTime / 3) == 0)
            {
                Vector2 center = Helper.GetMagikeTileCenter(Entity.Position);

                FresnelRectParticle p = PRTLoader.NewParticle<FresnelRectParticle>(center, Vector2.Zero, Color.White);
                p.CurrentRadius = p.MinRadius = 16 + 8;
                p.TargetRadius = 9 * 16 + 8;
                p.MaxTime = Timer;

                int t = Timer / (WorkTime / 3);
                if (t == 3)
                {
                    p.smoother = Coralite.Instance.SqrtSmoother;
                    p.Color = new Color(241, 130, 255);
                }
                else if (t == 2)
                {
                    p.smoother = Coralite.Instance.NoSmootherInstance;
                    p.Color = new Color(134, 156, 255);
                }
                else
                {
                    p.smoother = Coralite.Instance.X2Smoother;
                    p.Color = new Color(151, 217, 250);
                }
            }

            if (Timer == 10 || Timer == 20)
            {
                Vector2 center = Helper.GetMagikeTileCenter(Entity.Position);

                FresnelRectParticle p = PRTLoader.NewParticle<FresnelRectParticle>(center, Vector2.Zero, Color.White);
                p.CurrentRadius = p.MinRadius = 16 + 8;
                p.TargetRadius = 9 * 16 + 8;
                p.MaxTime = Timer;
                p.smoother = Coralite.Instance.NoSmootherInstance;
            }
        }

        #region UI部分

        public void ShowInUI(UIElement parent)
        {
            //添加显示在最上面的组件名称
            UIElement title = this.AddTitle(MagikeSystem.UITextID.BarrierOscillatorName, parent);

            UIList list =
            [
                //工作时间
                new TimerProgressBar(this),
                this.NewTextBar(c =>
                {
                    return MagikeSystem.GetUIText(MagikeSystem.UITextID.BarrierOscillatorCost)+c.WorkCost;
                } , parent),
            ];

            list.SetSize(0, -title.Height.Pixels, 1, 1);
            list.SetTopLeft(title.Height.Pixels + 8, 0);

            list.QuickInvisibleScrollbar();

            parent.Append(list);
        }

        #endregion

        #region 存取部分

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);
            tag.Add(preName + nameof(WorkCost), WorkCost);
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);
            WorkCost = tag.GetInt(preName + nameof(WorkCost));
        }

        #endregion
    }
}
