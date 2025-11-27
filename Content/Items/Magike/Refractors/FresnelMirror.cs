using Coralite.Content.Items.Magike.Columns;
using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.Attributes;
using Coralite.Core.Systems.MagikeSystem.BaseItems;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Core.Systems.MagikeSystem.Tiles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Magike.Refractors
{
    public class FresnelMirror() : MagikeApparatusItem(TileType<FresnelMirrorTile>(), Item.sellPrice(silver: 5)
            , RarityType<MagicCrystalRarity>(), AssetDirectory.MagikeRefractors)
    {
        public static LocalizedText SpecialCraft;

        public override void Load()
        {
            if (!Main.dedServ)
                SpecialCraft = this.GetLocalization(nameof(SpecialCraft));
        }

        public override void Unload()
        {
            SpecialCraft = null;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GiantLens>()
                .AddIngredient<BasicColumn>()
                .AddCondition(SpecialCraft, () => false)
                .Register();
        }
    }

    public class FresnelMirrorTile() : BaseMagikeTile
    (3, 3, Coralite.MagicCrystalPink, DustID.CorruptionThorns,0,true)
    {
        public override string Texture => AssetDirectory.MagikeRefractorTiles + Name;
        public override int DropItemType => ItemType<FresnelMirror>();

        public override CoraliteSetsSystem.MagikeTileType PlaceType => CoraliteSetsSystem.MagikeTileType.None;

        public override List<ushort> GetAllLevels()
        {
            return [
                NoneLevel.ID,
                CrystalLevel.ID,
                GlistentLevel.ID,
                BrilliantLevel.ID,
                FeatherLevel.ID,
                ];
        }

        public override void DrawExtraTex(SpriteBatch spriteBatch, Texture2D tex, Rectangle tileRect, Vector2 offset, Color lightColor, float rotation, MagikeTP entity, ushort level)
        {
            Vector2 selfCenter = tileRect.Center();
            Vector2 drawPos = selfCenter + offset+new Vector2(0,2);

            //绘制旋转的框框
            tex.QuickCenteredDraw(spriteBatch, new Rectangle(0, 0, 3, 1)
                , drawPos, lightColor, (float)(Main.timeForVisualEffects * 0.05f));
            //绘制旋转的框框的高光
            tex.QuickCenteredDraw(spriteBatch, new Rectangle(1, 0, 3, 1)
                , drawPos, lightColor, 0.1f*MathF.Sin((float)(Main.timeForVisualEffects * 0.1f)));
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
            AddComponent(new FresnelMirrorSender());
        }
    }

    public class FresnelMirrorContainer : UpgradeableContainer<FresnelMirrorTile>
    {
        //public override void Upgrade(MALevel incomeLevel)
        //{
        //    MagikeMaxBase = incomeLevel switch
        //    {
        //        MALevel.MagicCrystal => 240,
        //        MALevel.Glistent => 720,
        //        MALevel.CrystallineMagike => 5760,
        //        MALevel.Hallow => 7200,
        //        MALevel.Feather => 20480,
        //        _ => 0,
        //    } * 2;
        //    LimitMagikeAmount();
        //}
    }

    public class FresnelMirrorSender : MagikeSender, IUIShowable, IUpgradeable,IUpgradeLoadable
    {
        /// <summary>
        /// 发送半径，一个正方形<br></br>
        /// 与其他的半径不一样，这个以格为单位
        /// </summary>
        [UpgradeableProp]
        public byte SendRadius { get;private set; }

        public int TileType => TileType<FresnelMirrorTile>();

        private static List<Point16> recordPoints=new List<Point16>();

        public override void Initialize()
        {
            InitializeLevel();
        }

        public void InitializeLevel()
        {
            SendDelayBase = -1;
            UnitDeliveryBase = 0;
            SendRadius = 0;
        }

        public virtual bool CanUpgrade(ushort incomeLevel)
            => Entity.CheckUpgrageable(incomeLevel);

        public void Upgrade(ushort incomeLevel)
        {
            string name = this.GetDataPreName();
            SendDelayBase = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(SendDelayBase));
            UnitDeliveryBase = MagikeSystem.GetLevelDataInt(incomeLevel, name + nameof(UnitDeliveryBase));
            SendRadius = MagikeSystem.GetLevelDataByte(incomeLevel, name + nameof(SendRadius));

            //switch (incomeLevel)
            //{
            //    default:
            //    case MALevel.None:
            //        break;
            //    case MALevel.MagicCrystal:
            //        SendDelayBase = 60 * 2 + 30;
            //        UnitDeliveryBase = 18;
            //        SendRadius = 4;
            //        break;
            //    case MALevel.Glistent:
            //        SendDelayBase = 60 * 2;
            //        UnitDeliveryBase = 36;
            //        SendRadius = 4;
            //        break;
            //    case MALevel.CrystallineMagike:
            //        SendDelayBase = 60 * 1 + 30;
            //        UnitDeliveryBase = 144;
            //        SendRadius = 5;
            //        break;
            //    case MALevel.Feather:
            //        SendDelayBase = 60 * 1 + 30;
            //        UnitDeliveryBase = 512;
            //        SendRadius = 5;
            //        break;
            //}

            Timer = SendDelay;
        }

        public void ShowInUI(UIElement parent)
        {
            UIElement title = this.AddTitle(MagikeSystem.UITextID.FresnelMirrorSenderName, parent);
            UIList list =
            [
                //发送时间
                new TimerProgressBar(this),

                //发送量
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.MagikeSendAmount), parent),
                this.NewTextBar(UnitDeliveryText, parent),

                //连接距离
                this.NewTextBar(c => MagikeSystem.GetUIText(MagikeSystem.UITextID.FresnelConnectLength), parent),
                this.NewTextBar(FresnelLengthText, parent),
            ];

            list.SetSize(0, -title.Height.Pixels, 1, 1);
            list.SetTopLeft(title.Height.Pixels + 8, 0);

            list.QuickInvisibleScrollbar();

            parent.Append(list);
        }

        public virtual string FresnelLengthText(FresnelMirrorSender s)
        {
            int length = SendRadius - 1;
            if (length < 1)
                length = 0;

            return $"  ▶ {length}";
        }

        public override void Update()
        {
            if (SendDelayBase < 0)
                return;

            if (!CanSend())
                return;

            FresnelSend(container);
        }

        public override bool CanSend()
        {
            if (!Container.HasMagike)
                return false;

            bool can=base.CanSend();

            if (Timer % (SendDelay / 3) == 0)
            {
                Vector2 center = Helper.GetMagikeTileCenter(Entity.Position);
                Color c = Color.White;
                if (MagikeHelper.TryGetMagikeApparatusLevel(Entity.Position, out ushort level))
                    c = CoraliteContent.GetMagikeLevel(level).LevelColor;

                FresnelRectParticle p = PRTLoader.NewParticle<FresnelRectParticle>(center, Vector2.Zero, c);
                p.CurrentRadius = p.MinRadius = 16 + 8;
                p.TargetRadius = SendRadius * 16 + 8;
                p.MaxTime = Timer;

                int t = Timer / (SendDelay / 3);
                if (t==3)
                    p.smoother = Coralite.Instance.SqrtSmoother;
                else if (t == 2)
                    p.smoother = Coralite.Instance.NoSmootherInstance;
                else
                    p.smoother = Coralite.Instance.X2Smoother;
            }

            return can;
        }

        /// <summary>
        /// 发送魔能
        /// </summary>
        public void FresnelSend(MagikeContainer selfContainer)
        {
            Point16 pos = Entity.Position;
            pos += new Point16(1 - SendRadius, 1 - SendRadius);

            for (int i = 0; i < SendRadius * 2 + 1; i++)
                for (int j = 0; j < SendRadius * 2 + 1; j++)
                {
                    Point16 aimPos = pos + new Point16(i, j);
                    Tile t = Framing.GetTileSafely(aimPos);
                    if (!t.HasTile || !Main.tileFrameImportant[t.TileType])
                        continue;

                    if (!MagikeHelper.TryGetEntityWithComponent(aimPos.X, aimPos.Y, MagikeComponentID.MagikeContainer, out MagikeTP entity))
                        continue;

                    if (recordPoints.Contains(entity.Position))
                        continue;

                    //找到一个容器，然后发送
                    recordPoints.Add(entity.Position);
                    MagikeContainer receiver = entity.GetMagikeContainer();

                    if (receiver.FullMagike)
                        continue;

                    //获得发送量
                    GetSendAmount(selfContainer, out int sendAmount);

                    SendMagike(selfContainer, receiver, sendAmount);
                    if (!selfContainer.HasMagike)
                        goto Over;
                }

            Over:
            recordPoints.Clear();
        }

        public override void LoadData(string preName, TagCompound tag)
        {
            base.LoadData(preName, tag);
            if (tag.TryGet(preName + nameof(SendRadius),out byte r))
                SendRadius = r;
        }

        public override void SaveData(string preName, TagCompound tag)
        {
            base.SaveData(preName, tag);

            tag.Add(preName + nameof(SendRadius), SendRadius);
        }

        public override void SendData(ModPacket data)
        {
            base.SendData(data);

            data.Write(SendRadius);
        }

        public override void ReceiveData(BinaryReader reader, int whoAmI)
        {
            base.ReceiveData(reader, whoAmI);

            SendRadius = reader.ReadByte();
        }
    }

    public class FresnelRectParticle : Particle
    {
        public override string Texture => AssetDirectory.Particles+Name;

        public int MaxTime;
        public int TargetRadius;
        public int MinRadius;
        public float CurrentRadius;
        public float alpha;

        public ISmoother smoother;

        public override void AI()
        {
            Opacity++;

            if (Opacity < MaxTime)
            {
                if (smoother != null)
                    CurrentRadius = Helper.Lerp(MinRadius, TargetRadius, smoother.Smoother(Opacity / MaxTime));
                if (alpha < 1)
                {
                    alpha += 0.05f;
                    if (alpha > 1)
                    {
                        alpha = 1;
                    }
                }
                return;
            }

            if (Opacity < MaxTime+20)
            {
                alpha -= 1 / 20f;
                return;
            }

            active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Vector2 topLeft = Position - Main.screenPosition + new Vector2(-CurrentRadius);
            Texture2D tex = TexValue;
            Color c = Color * 0.6f * alpha;

            DrawRect(spriteBatch, topLeft, tex, c);
            c *= 0.2f;
            c.A = 0;
            DrawRect(spriteBatch, topLeft, tex, c);

            return false;
                
            void DrawRect(SpriteBatch spriteBatch, Vector2 topLeft, Texture2D tex, Color c)
            {
                //绘制左边的
                spriteBatch.Draw(tex, new Rectangle((int)topLeft.X, (int)topLeft.Y, tex.Width
                    , (int)(CurrentRadius * 2)), c);
                //绘制上边
                spriteBatch.Draw(tex, new Rectangle((int)topLeft.X, (int)topLeft.Y
                    , tex.Width, (int)(CurrentRadius * 2)), null, c, MathHelper.PiOver2, new Vector2(0, tex.Height), 0, 0);

                Vector2 bottomRight = Position - Main.screenPosition + new Vector2(CurrentRadius);
                //绘制右边
                spriteBatch.Draw(tex, new Rectangle((int)bottomRight.X - tex.Width, (int)(bottomRight.Y - CurrentRadius * 2)
                    , tex.Width, (int)(CurrentRadius * 2)), null, c, MathHelper.Pi, new Vector2(tex.Width, tex.Height), SpriteEffects.FlipVertically, 0);
                //绘制下边的
                spriteBatch.Draw(tex, new Rectangle((int)(bottomRight.X - CurrentRadius * 2), (int)bottomRight.Y - tex.Width
                   , tex.Width, (int)(CurrentRadius * 2)), null, c, -MathHelper.PiOver2, new Vector2(tex.Width, 0), 0, 0);
            }
        }
    }
}
