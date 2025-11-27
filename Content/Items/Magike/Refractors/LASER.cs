using Coralite.Content.Items.MagikeSeries1;
using Coralite.Content.Items.Shadow;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class LASER() : MagikeApparatusItem(TileType<LASERTile>(), Item.sellPrice(silver: 5)
        , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeRefractors), IMagikeCraftable
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<LASERCore>()
                .AddIngredient<Basalt>(4)
                .AddIngredient<HardBasalt>(9)
                .AddIngredient<MagicCrystalBlock>(2)
                .AddIngredient(ItemID.CopperPlating, 2)
                .AddCondition(CoraliteConditions.UseMultiBlockStructure)
                .Register();
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<LASERCore, LASER>(MagikeHelper.CalculateMagikeCost<ShadowLevel>(3))
                .AddIngredient<ShadowEnergy>(4)
                .AddIngredient(ItemID.CopperBar, 10)
                .AddIngredient<HardBasalt>(6)
                .Register();
        }
    }

    public class LASERTile() : BaseMagikeTile
        (3, 3, Coralite.MagicCrystalPink, DustID.CorruptionThorns)
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;
        public override int DropItemType => ItemType<LASER>();

        public override CoraliteSetsSystem.MagikeTileType PlaceType => CoraliteSetsSystem.MagikeTileType.FourWayNormal;

        public override List<ushort> GetAllLevels()
        {
            return [
                NoneLevel.ID,
                CrystalLevel.ID,
                GlistentLevel.ID,
                BrilliantLevel.ID,
                ];
        }

        public override void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity, ushort level)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset - rotation.ToRotationVector2() * (tileRect.Height / 2 - 8);
            int halfHeight = Math.Max(tileRect.Height / 2, tileRect.Width / 2);

            Rectangle frameBox = tex.Frame(2, 1);

            //虽然一般不会没有 但是还是检测一下
            if (!entity.TryGetComponent(MagikeComponentID.MagikeSender, out PluseSender senderComponent))
                return;

            rotation += 1.57f;//根据贴图矫正旋转角度

            float rot = rotation;
            float rot2 = rotation;
            rot2 += senderComponent.Rotation;
            rot -= senderComponent.Rotation;

            float factor = (float)senderComponent.Timer / senderComponent.SendDelay;

            // 绘制主帖图
            Vector2 origin = new Vector2(frameBox.Width / 2, frameBox.Height - 4);
            Vector2 dir = rotation.ToRotationVector2() * frameBox.Width / 2;
            spriteBatch.Draw(tex, drawPos - dir, frameBox, lightColor, rot, origin, 1f, 0, 0f);

            frameBox = tex.Frame(2, 1, 1);
            spriteBatch.Draw(tex, drawPos + dir, frameBox, lightColor, rot2, origin, 1f, 0, 0f);

            if (!Main.gamePaused && senderComponent.DoSend && factor < 0.8f)//生成粒子
            {
                rotation -= 1.57f;
                drawPos = selfCenter - rotation.ToRotationVector2() * (tileRect.Height / 2 - 8);
                int width = frameBox.Height - 8;
                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustPerfect(drawPos - dir - (rot + 1.57f).ToRotationVector2() * Main.rand.NextFloat(width), DustID.CrystalSerpent_Pink
                        , rot.ToRotationVector2() * 3f, Scale: 1f);
                    d.noGravity = true;

                    d = Dust.NewDustPerfect(drawPos + dir - (rot2 + 1.57f).ToRotationVector2() * Main.rand.NextFloat(width), DustID.CrystalSerpent_Pink
                        , (rot2 + MathHelper.Pi).ToRotationVector2() * 3f, Scale: 1f);
                    d.noGravity = true;
                }
            }
        }
    }

    public class LASERTileEntity : MagikeTP
    {
        public override int TargetTileID => TileType<LASERTile>();

        public override int MainComponentID => MagikeComponentID.MagikeSender;

        public override void InitializeBeginningComponent()
        {
            AddComponent(new LASERContainer());
            AddComponent(new LASERSender());
        }
    }

    public class LASERContainer : UpgradeableContainer<LASERTile>
    {
        //public override void Upgrade(ushort incomeLevel)
        //{
            //    string name = this.GetDataPreName();
            //    MagikeMaxBase = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(MagikeMaxBase));

            //    //MagikeMaxBase = incomeLevel switch
            //    //{
            //    //    MALevel.MagicCrystal => 720,
            //    //    MALevel.Glistent => 1440,
            //    //    MALevel.CrystallineMagike => 3240,
            //    //    _ => 0,
            //    //};
            //    LimitMagikeAmount();

            //    //AntiMagikeMaxBase = MagikeMaxBase;
            //    //LimitAntiMagikeAmount();
            //}
        //}
    }

    public class LASERSender : PluseSender, IUpgradeable, IUpgradeLoadable
    {
        public int TileType => TileType<LASERTile>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public void InitializeLevel()
        {
            SendDelayBase = -1;
            ConnectLengthBase = 0;
        }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public void Upgrade(ushort incomeLevel)
        {
            string name = this.GetDataPreName();
            SendDelayBase = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(SendDelayBase));
            ConnectLengthBase = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(ConnectLengthBase));

            //switch (incomeLevel)
            //{
            //    default:
            //    case MALevel.None:
            //        break;
            //    case MALevel.MagicCrystal:
            //        SendDelayBase = 60 * 2 + 30;
            //        ConnectLengthBase = 50 * 16;
            //        break;
            //    case MALevel.Glistent:
            //        SendDelayBase = 60 * 2;
            //        ConnectLengthBase = 55 * 16;
            //        break;
            //    case MALevel.CrystallineMagike:
            //        SendDelayBase = 60 * 1 + 30;
            //        ConnectLengthBase = 60 * 16;
            //        break;
            //}

            Timer = SendDelay;
        }
    }
}
