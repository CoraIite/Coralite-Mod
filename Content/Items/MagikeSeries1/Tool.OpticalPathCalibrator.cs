using Coralite.Content.Raritys;
using Coralite.Core;
using Coralite.Core.Systems.CoraliteActorComponent;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MagikeSystem.TileEntities;
using Coralite.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.MagikeSeries1
{
    public class OpticalPathCalibrator : ModItem
    {
        public override string Texture => AssetDirectory.MagikeSeries1Item + Name;

        public static LocalizedText TimerNotFound {  get; private set; }
        public static LocalizedText TimerChosen {  get; private set; }
        public static LocalizedText TimerSynced {  get; private set; }

        private MagikeTileEntity entity1;

        public override void Load()
        {
            TimerNotFound=this.GetLocalization(nameof(TimerNotFound));
            TimerChosen = this.GetLocalization(nameof(TimerChosen));
            TimerSynced = this.GetLocalization(nameof(TimerSynced));
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = Item.useTime = 10;
            Item.useTurn = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.rare = ModContent.RarityType<MagicCrystalRarity>();
            Item.GetMagikeItem().magikeAmount = 50;
        }

        public override bool CanUseItem(Player player)
        {
            Point16 pos = Main.MouseWorld.ToTileCoordinates16();

            //左键寻找发送器
            if (entity1 == null)
            {
                if (MagikeHelper.TryGetEntity(pos.X,pos.Y, out MagikeTileEntity entity))    //找到了
                {
                    foreach (var component in entity.ComponentsCache)//寻找计时器
                    {
                        if (component is ITimerTriggerComponent timer && timer.TimeResetable)
                        {
                            entity1 = entity;

                            PopupText.NewText(new AdvancedPopupRequest()
                            {
                                Color = Coralite.MagicCrystalPink,
                                Text = TimerChosen.Value,
                                DurationInFrames = 60,
                                Velocity = -Vector2.UnitY
                            }, Main.MouseWorld - (Vector2.UnitY * 32));

                            Helper.PlayPitched("UI/Tick", 0.4f, 0, player.Center);
                            break;
                        }
                    }

                    if (entity1 == null)
                    {
                        PopupText.NewText(new AdvancedPopupRequest()
                        {
                            Color = Coralite.MagicCrystalPink,
                            Text = TimerNotFound.Value,
                            DurationInFrames = 60,
                            Velocity = -Vector2.UnitY
                        }, Main.MouseWorld - (Vector2.UnitY * 32));

                        Helper.PlayPitched("UI/Error", 0.4f, 0, player.Center);
                    }
                }
                else
                {
                    PopupText.NewText(new AdvancedPopupRequest()
                    {
                        Color = Coralite.MagicCrystalPink,
                        Text = TimerNotFound.Value,
                        DurationInFrames = 60,
                        Velocity = -Vector2.UnitY
                    }, Main.MouseWorld - (Vector2.UnitY * 32));

                    Helper.PlayPitched("UI/Error", 0.4f, 0, player.Center);
                }
            }
            else//寻找另一个魔能仪器并同步
            {
                if (MagikeHelper.TryGetEntity(pos.X, pos.Y, out MagikeTileEntity entity))    //找到了
                {
                    bool hasTimer = false;
                    foreach (var component in entity.ComponentsCache)//寻找计时器
                    {
                        if (component is ITimerTriggerComponent timer && timer.TimeResetable)
                        {
                            hasTimer = true;
                            break;
                        }
                    }

                    if (hasTimer)
                    {
                        foreach (var component in entity.ComponentsCache)//设置时间
                        {
                            if (component is ITimerTriggerComponent timer && timer.TimeResetable)
                                foreach (var component2 in entity1.ComponentsCache)
                                {
                                    if (component is ITimerTriggerComponent timer2 && timer2.TimeResetable
                                        && component.ID == component2.ID)
                                    {
                                        int time = Math.Max(timer.Timer, timer2.Timer);
                                        timer.Timer = timer2.Timer = time;
                                    }
                                }
                        }

                        PopupText.NewText(new AdvancedPopupRequest()
                        {
                            Color = Coralite.MagicCrystalPink,
                            Text = TimerSynced.Value,
                            DurationInFrames = 60,
                            Velocity = -Vector2.UnitY
                        }, Main.MouseWorld - (Vector2.UnitY * 32));
                        Helper.PlayPitched("Fairy/FairyBottleClick2", 0.4f, 0, player.Center);
                        entity1 = null;
                    }
                    else
                    {
                        PopupText.NewText(new AdvancedPopupRequest()
                        {
                            Color = Coralite.MagicCrystalPink,
                            Text = TimerNotFound.Value,
                            DurationInFrames = 60,
                            Velocity = -Vector2.UnitY
                        }, Main.MouseWorld - (Vector2.UnitY * 32));

                        Helper.PlayPitched("UI/Error", 0.4f, 0, player.Center);
                        entity1 = null;
                    }
                }
                else
                {
                    PopupText.NewText(new AdvancedPopupRequest()
                    {
                        Color = Coralite.MagicCrystalPink,
                        Text = TimerNotFound.Value,
                        DurationInFrames = 60,
                        Velocity = -Vector2.UnitY
                    }, Main.MouseWorld - (Vector2.UnitY * 32));

                    Helper.PlayPitched("UI/Error", 0.4f, 0, player.Center);
                    entity1 = null;
                }
            }

            return true;
        }
    }
}
