using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.GlobalNPCs;
using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Particles;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Equip
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect(ExtraEffectNames = [BloodSet, ShadowSet, PrisonSet, ShadowSetVanityName, PrisonSetVanityName])]
    public class BloodmarkTopper : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        /// <summary> 与血腥套触发的套装效果 </summary>
        public const string BloodSet = "BloodmarkTopperA";
        /// <summary> 与暗影套触发的套装效果 </summary>
        public const string ShadowSet = "BloodmarkTopperB";
        public const string ShadowSetVanity = $"{AssetDirectory.Misc_Equip}BloodmarkTopperShadow_Head";
        public const string ShadowSetVanityName = "BloodmarkTopperShadow";
        /// <summary> 与黑曜石套触发的套装效果 </summary>
        public const string PrisonSet = "BloodmarkTopperC";
        public const string PrisonSetVanity = $"{AssetDirectory.Misc_Equip}BloodmarkTopperPrison_Head";
        public const string PrisonSetVanityName = "BloodmarkTopperPrison";

        public static LocalizedText[] EXToolTip { get; private set; }
        public static LocalizedText[] Bonus { get; private set; }
        public static LocalizedText[] EXName { get; private set; }
        public static LocalizedText SetTip { get; private set; }

        private ArmorSetType vanityType = ArmorSetType.Blood;

        private enum ArmorSetType : byte
        {
            Blood,
            Shadow,
            Prison,

            Count
        }

        private enum EXToolTipID : byte//额外的物品提示
        {
            None,
            Blood,
            Shadow,
            Prison,

            Count
        }

        public override void Load()
        {
            EquipLoader.AddEquipTexture(Mod, ShadowSetVanity, EquipType.Head, name: ShadowSetVanityName);
            EquipLoader.AddEquipTexture(Mod, PrisonSetVanity, EquipType.Head, name: PrisonSetVanityName);

            EXToolTip = new LocalizedText[(int)EXToolTipID.Count];
            for (int i = 0; i < (int)EXToolTipID.Count; i++)
                EXToolTip[i] = this.GetLocalization("ExtraToolTip" + Enum.GetName((EXToolTipID)i));

            EXName = new LocalizedText[(int)ArmorSetType.Count];
            Bonus = new LocalizedText[(int)ArmorSetType.Count];

            for (int i = 0; i < (int)ArmorSetType.Count; i++)
                EXName[i] = this.GetLocalization("SpecialPreName" + Enum.GetName((ArmorSetType)i));
            for (int i = 0; i < (int)ArmorSetType.Count; i++)
                Bonus[i] = this.GetLocalization("ArmorSet" + Enum.GetName((ArmorSetType)i));

            SetTip = this.GetLocalization("SetTip");
        }

        public override void Unload()
        {
            EXToolTip = null;
            Bonus = null;
            EXName = null;
            SetTip = null;
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = RarityType<BloodRarity>();
            Item.defense = 9;
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            vanityType++;
            if (vanityType > ArmorSetType.Prison)
                vanityType = ArmorSetType.Blood;
        }

        public override bool ConsumeItem(Player player) => false;

        public void AddMagikeCraftRecipe()
        {
            int magikeCost = MagikeHelper.CalculateMagikeCost<BrilliantLevel>( 12, 60 * 4);

            var recipes = MagikeRecipe.CreateCraftRecipes(
                [
                        ItemID.CrimsonHelmet,
                        ItemID.ShadowHelmet,
                        ItemID.AncientShadowHelmet,
                        ItemID.ObsidianHelm
                     ], ItemType<BloodmarkTopper>(), magikeCost);

            foreach (var recipe in recipes)
            {
                recipe
                    .AddIngredient<BloodyOrb>(3)
                    .AddIngredient(ItemID.TopHat)
                    .AddIngredient<DeorcInABottle>()
                    .AddIngredient<MutatusInABottle>()
                    .AddIngredient(ItemID.SoulofNight, 5)
                    .AddIngredient(ItemID.BloodMoonStarter)
                    .Register();
            }
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddEffect(nameof(BloodmarkTopper));

            //暴击率提高5%
            player.GetCritChance(DamageClass.Generic) += 5;
            //伤害提高8%
            player.GetDamage(DamageClass.Generic) += 0.08f;

            int projType = ProjectileType<BloodmarkTopperProj>();
            int count = player.ownedProjectileCounts[projType];
            if (count < 1)
                Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero
                    , projType, 0, 0, player.whoAmI);
            else if (count > 1)
                foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == projType))
                    proj.Kill();
        }

        public override bool IsVanitySet(int head, int body, int legs)
        {
            return true;
        }

        public override void UpdateVanitySet(Player player)
        {
            if (!player.armor[10].IsAir && player.armor[10].ModItem is BloodmarkTopper bt && player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (bt.vanityType == ArmorSetType.Shadow)
                    cp.AddEffect(ShadowSetVanityName);
                else if (bt.vanityType == ArmorSetType.Prison)
                    cp.AddEffect(PrisonSetVanityName);
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
            => CheckArmorSet(head, body, legs, out _);

        private static bool CheckArmorSet(Item head, Item body, Item legs, out ArmorSetType? type)
        {
            type = null;

            if (head.type != ItemType<BloodmarkTopper>())
                return false;

            if (body.type == ItemID.CrimsonScalemail && legs.type == ItemID.CrimsonGreaves)
            {
                type = ArmorSetType.Blood;
                return true;
            }

            if (body.type is ItemID.ShadowScalemail or ItemID.AncientShadowScalemail
                && legs.type is ItemID.ShadowGreaves or ItemID.AncientShadowGreaves)
            {
                type = ArmorSetType.Shadow;
                return true;
            }

            if (body.type == ItemID.ObsidianShirt && legs.type == ItemID.ObsidianPants)
            {
                type = ArmorSetType.Prison;
                return true;
            }

            return false;
        }

        public override void UpdateArmorSet(Player player)
        {
            CheckArmorSet(player.HeadArmor(), player.BodyArmor(), player.LegArmor(), out ArmorSetType? type);

            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                player.setBonus = Bonus[(int)type.Value].Value;

                switch (type.Value)
                {
                    default:
                    case ArmorSetType.Blood:
                        cp.AddEffect(BloodSet);
                        player.statDefense += 10;

                        int projType = ProjectileType<BloodPool>();

                        int count = player.ownedProjectileCounts[projType];
                        if (count < 1)
                            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero
                                , projType, 0, 0, player.whoAmI);
                        else if (count > 1)
                            foreach (var proj in Main.projectile.Where(p => p.active && p.owner == player.whoAmI && p.type == projType))
                                proj.Kill();

                        break;
                    case ArmorSetType.Shadow:
                        cp.AddEffect(ShadowSet);
                        player.statDefense += 6;

                        cp.UpdateShadowTopper();

                        player.GetDamage(DamageClass.Generic) += cp.shadowAttackBonus * 0.03f;
                        cp.LifeMaxModifyer.Flat += 5 * cp.shadowLifeMaxBonus;
                        player.statDefense += cp.shadowDefenceBonus;

                        if (cp.shadowAttackBonus > 3
                            && Main.timeForVisualEffects % 6 == 0)
                        {
                            Dust d = Dust.NewDustPerfect(player.MountedCenter, DustID.Granite, Helper.NextVec2Dir(3, 7)
                                , 150, Scale: Main.rand.NextFloat(1, 1.5f));
                            d.noGravity = true;
                        }

                        break;
                    case ArmorSetType.Prison:
                        cp.AddEffect(PrisonSet);
                        player.statDefense += 6;
                        player.whipRangeMultiplier += 0.3f;
                        player.GetDamage(DamageClass.Summon) += 0.15f;
                        player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += 0.15f;

                        break;
                }
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (!CheckArmorSet(Main.LocalPlayer.HeadArmor(), Main.LocalPlayer.BodyArmor(), Main.LocalPlayer.LegArmor(), out ArmorSetType? type))
            {
                int index2 = tooltips.FindIndex(l => l.Name == "Tooltip0");
                if (index2 != -1)
                {
                    TooltipLine line3 = new TooltipLine(Mod, "SpecialToolTip", EXToolTip[0].Value);
                    tooltips.Insert(index2 + 1, line3);
                }

                TooltipLine line2 = new TooltipLine(Mod, "SpecialArmorSet", SetTip.Value);
                tooltips.Add(line2);
                return;
            }

            string name = "ItemName";
            TooltipLine line = tooltips.FirstOrDefault(l => l.Name == name && l.Mod == "Terraria");
            if (line != null)
            {
                string text = line.Text;
                line.Text = EXName[(int)type.Value] + " " + text;
            }

            int index = tooltips.FindIndex(l => l.Name == "Tooltip0");
            if (index != -1)
            {
                TooltipLine line2 = new TooltipLine(Mod, "SpecialToolTip", EXToolTip[(int)type.Value + 1].Value);
                tooltips.Insert(index + 1, line2);
            }
        }

        public override void ArmorSetShadows(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.HasEffect(ShadowSet) && cp.shadowDefenceBonus > 3)
                    player.armorEffectDrawShadow = true;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add(nameof(vanityType), (byte)vanityType);
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet(nameof(vanityType), out byte b1))
                vanityType = (ArmorSetType)b1;
        }
    }

    public class BloodRarity : ModRarity
    {
        public override Color RarityColor =>
            Color.Lerp(new Color(255, 30, 30), new(160, 14, 46), 0.5f + 0.5f * MathF.Sin((int)Main.timeForVisualEffects * 0.1f));
    }

    [VaultLoaden(AssetDirectory.Misc_Equip)]
    public class BloodmarkTopperProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        public static ATex BloodTopperSpawn { get; private set; }
        [VaultLoaden("{@classPath}" + "BloodmarkTopperProjShadow")]
        public static ATex ShadowTopper { get; private set; }
        [VaultLoaden("{@classPath}" + "BloodmarkTopperProjPrison")]
        public static ATex PrisonTopper { get; private set; }

        public Player Owner => Main.player[Projectile.owner];

        private TopperTypes TopperType
        {
            get => (TopperTypes)Projectile.ai[0];
            set => Projectile.ai[0] = (int)value;
        }

        private ref float State => ref Projectile.ai[1];
        private ref float TargetIndex => ref Projectile.ai[2];

        private ref float Timer => ref Projectile.localAI[0];
        private ScaleTypes ScaleType
        {
            get => (ScaleTypes)Projectile.localAI[1];
            set => Projectile.localAI[1] = (int)value;
        }

        private ref float ScaleTimer => ref Projectile.localAI[2];

        public Vector2 IdlePos => Owner.MountedCenter + new Vector2(0, -16 * 4);

        public byte ShadowMonsterType;
        public byte PrisonShootCount;

        public List<BloodTopper> BloodToppers { get; private set; }

        private Vector2 Scale;
        private int FrameX;
        private bool init = false;

        private SecondOrderDynamics_Float RotController = new SecondOrderDynamics_Float(0.88f, 0.25f, 0, 0);

        private enum TopperTypes
        {
            Error = -1,
            None,
            Blood,
            Shadow,
            Prison
        }

        private enum ScaleTypes
        {
            None = -1,
            Spawn,
            Attack,
            Back,
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 32;

            Projectile.tileCollide = false;
            Projectile.friendly = true;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        /// <summary>
        /// 堆叠的血礼帽
        /// </summary>
        public class BloodTopper
        {
            public SecondOrderDynamics_Vec2 posMover;
            public SecondOrderDynamics_Float rotMover;

            public Vector2 Pos;
            public float Rot;

            public int frame;
            public int frameCounter;

            public BloodTopper(Projectile projectile, int index)
            {
                Pos = projectile.Top;
                float f = 1.5f + Helper.X3Ease(1 - index / 5f) * 10;
                //Main.NewText(f);
                posMover = new SecondOrderDynamics_Vec2(f, 0.9f - index * 0.05f, 0, Pos);
                Rot = Main.rand.NextFromList(-0.5f, 0.5f);
                rotMover = new SecondOrderDynamics_Float(0.88f - index * 0.05f, 0.5f - index * 0.05f, 0.5f, Rot);
            }

            public void Update(Projectile projectile, int index)
            {
                Vector2 targetP = projectile.Top + new Vector2(0, -8 - index * 20);
                Pos = posMover.Update(1 / 60f, targetP);
                Rot = rotMover.Update(1 / 60f, projectile.rotation - Math.Clamp((projectile.Center.X - Pos.X) / 25, -0.5f, 0.5f));

                if (++frameCounter > 4)
                {
                    frameCounter = 0;
                    if (++frame > 5)
                        frame = 5;
                }
            }

            public void Boom(Projectile projectile, int damage)
            {
                PRTLoader.NewParticle<BloodStrike>(Pos, Vector2.Zero);

                for (int i = 0; i < 10; i++)
                {
                    Dust d = Dust.NewDustPerfect(Pos, DustID.Blood, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                    d.noGravity = true;
                }

                projectile.NewProjectileFromThis<BloodTearProj>(Pos, new Vector2(0, -8).RotateByRandom(-0.75f, 0.75f)
                    , damage, 2);
            }

            public void Draw(SpriteEffects effect)
            {
                Texture2D tex = BloodTopperSpawn.Value;
                Color drawColor = Lighting.GetColor(Pos.ToTileCoordinates());
                var frameBox = tex.Frame(1, 6, 0, frame);

                Main.spriteBatch.Draw(tex, Pos - Main.screenPosition, frameBox, drawColor, Rot, frameBox.Size() / 2, 1, effect, 0);
            }
        }

        #region AI

        public override void AI()
        {
            if (init)
            {
                init = false;
                TargetIndex = -1;
            }

            SetTopperType();

            switch (TopperType)
            {
                default:
                case TopperTypes.Error:
                    break;
                case TopperTypes.None:
                    NormalAI<BloodBite>(70, null);
                    Lighting.AddLight(Projectile.Center, Coralite.CrimsonRed.ToVector3() / 3);
                    break;
                case TopperTypes.Blood:
                    BloodToppers ??= new List<BloodTopper>(8);
                    NormalAI<BloodBite>(85, OnBloodAttack);
                    Lighting.AddLight(Projectile.Center, Coralite.CrimsonRed.ToVector3() / 3);
                    UpdateBloodTopper();

                    break;
                case TopperTypes.Shadow:
                    NormalAI<ShadowBite>(80, OnShadowAttack);
                    Lighting.AddLight(Projectile.Center, Coralite.CorruptionPurple.ToVector3() / 3);
                    break;
                case TopperTypes.Prison:
                    PrisonAI();
                    Lighting.AddLight(Projectile.Center, Color.Silver.ToVector3() / 3);
                    break;
            }

            ControlScale();
        }

        private void NormalAI<T>(int projDamage, Action onAttack) where T : ModProjectile
        {
            switch (State)
            {
                default:
                case -1://返回玩家
                    BackToOwner();
                    FlyFrame();

                    if (Math.Abs(Owner.Center.X - Projectile.Center.X) > 18)
                        Projectile.spriteDirection = Math.Sign(Owner.Center.X - Projectile.Center.X);

                    Projectile.rotation = RotController.Update(1 / 60f, 0);
                    break;
                case 0://寻敌
                    {
                        Projectile.Center = Vector2.SmoothStep(Projectile.Center, IdlePos + new Vector2(Owner.velocity.X * 6, -Owner.velocity.Y / 2), 0.4f);

                        if (Projectile.IsOwnedByLocalPlayer() && Timer > 20)
                        {
                            Timer = 0;
                            if (FindEnemy())
                                StartAttack();
                        }

                        Timer++;
                        FlyFrame();
                        IdleDirection();
                        Projectile.rotation = RotController.Update(1 / 60f, Owner.velocity.X / 15);
                    }
                    break;
                case 1://冲刺到敌怪头上
                    {
                        if (!TargetIndex.GetNPCOwner(out NPC target, SetToBack))
                            return;

                        if (Timer < 20)//准备动作
                        {
                            Projectile.Center = Vector2.SmoothStep(Projectile.Center, IdlePos + new Vector2(Owner.velocity.X * 6, -Owner.velocity.Y / 2), 0.4f);

                            Projectile.rotation =
                                Projectile.rotation.AngleLerp(
                                    (target.Center - Projectile.Center).ToRotation() - MathHelper.PiOver2, 0.16f);

                            Timer++;
                        }
                        else if (Timer < 21)//冲刺到目标头顶上
                        {
                            ScaleType = ScaleTypes.Attack;
                            Projectile.velocity = (target.Top - Projectile.Center).SafeNormalize(Vector2.Zero) * 22;
                            Projectile.rotation =
                                Projectile.rotation.AngleLerp(
                                    (target.Center - Projectile.Center).ToRotation() - MathHelper.PiOver2, 0.16f);

                            if (Vector2.Distance(target.Top, Projectile.Center) < 24)
                            {
                                Timer++;
                                Projectile.velocity = Vector2.Zero;
                            }
                        }
                        else if (Timer < 21 + 12)//摆正位置
                        {
                            ScaleType = ScaleTypes.Back;
                            Projectile.Center = target.Top;
                            Projectile.rotation = Projectile.rotation.AngleLerp(0, 0.2f);
                            Timer++;
                        }
                        else
                        {
                            State++;
                            Timer = 0;
                            Projectile.frame = 1;
                            return;
                        }

                        FaceToEnemy(target);
                        FlyFrame();
                    }
                    break;
                case 2://向下咬攻击敌怪
                    {
                        if (!TargetIndex.GetNPCOwner(out NPC target, SetToBack))
                            return;

                        AttackFrame();
                        Projectile.spriteDirection = target.direction;
                        Projectile.Center = target.Top;
                        Projectile.rotation = RotController.Update(1 / 60f, Math.Clamp(-target.velocity.X / 20, -0.6f, 0.6f));

                        if (Projectile.IsOwnedByLocalPlayer() && Timer == 3 * 4)//生成咬合弹幕
                        {
                            int damage = Owner.GetDamageByHeldItem(projDamage);
                            Projectile.NewProjectileFromThis<T>(Main.rand.NextVector2FromRectangle(target.getRect())
                                , Vector2.Zero, damage, 0);

                            onAttack?.Invoke();
                        }

                        if (Timer > 60)//重设状态
                        {
                            if (FindEnemy())
                            {
                                State = 2;
                                Timer = 0;
                                Projectile.frame = 1;
                            }
                            else
                                SetToBack();
                        }

                        Timer++;
                    }
                    break;
            }
        }

        private void PrisonAI()
        {
            switch (State)
            {
                default:
                case -1://返回玩家
                    BackToOwner();
                    FlyFrame();

                    if (Math.Abs(Owner.Center.X - Projectile.Center.X) > 18)
                        Projectile.spriteDirection = Math.Sign(Owner.Center.X - Projectile.Center.X);

                    Projectile.rotation = RotController.Update(1 / 60f, 0);
                    break;
                case 0://寻敌
                    {
                        Projectile.Center = Vector2.SmoothStep(Projectile.Center, IdlePos + new Vector2(Owner.velocity.X * 6, -Owner.velocity.Y / 2), 0.4f);

                        if (Projectile.IsOwnedByLocalPlayer() && Timer > 10)
                        {
                            Timer = 10;
                            if (!Owner.ItemTimeIsZero && ProjectileID.Sets.IsAWhip[Owner.HeldItem.shoot])
                                StartAttack();
                        }

                        Timer++;
                        FlyFrame();
                        IdleDirection();
                        Projectile.rotation = RotController.Update(1 / 60f, Owner.velocity.X / 15);
                    }
                    break;
                case 1://寻找目标
                    {
                        FindEnemy();
                        if (TargetIndex.GetNPCOwner(out NPC target))
                        {
                            FaceToEnemy(target);
                        }

                        State = 2;
                        Projectile.frame = 1;
                    }
                    break;
                case 2://射弹幕
                    {
                        Projectile.Center = Vector2.SmoothStep(Projectile.Center, IdlePos + new Vector2(Owner.velocity.X * 6, -Owner.velocity.Y / 2), 0.4f);
                        Projectile.rotation = Projectile.rotation.AngleLerp(0, 0.02f);

                        if (Projectile.IsOwnedByLocalPlayer())
                        {
                            Vector2 pos = Main.MouseWorld;

                            if (TargetIndex.GetNPCOwner(out NPC target))
                                pos = target.Center;

                            float r = pos.X - Projectile.Center.X;
                            if (Math.Abs(r) > 8)
                                Projectile.spriteDirection = Math.Sign(r);

                            if (PrisonShootCount == 4)
                            {
                                if (Timer == 4 * 4)
                                {
                                    int damage = Owner.GetDamageByHeldItem(70);

                                    Vector2 dir = (pos - Projectile.Center).SafeNormalize(Vector2.Zero);
                                    Projectile.NewProjectileFromThis<FishScissors>(Projectile.Center
                                        , dir * Main.rand.NextFloat(10, 12)
                                        , damage, 2);

                                    var p = PRTLoader.NewParticle<PrisonShootParticle>(Projectile.Center + new Vector2(Projectile.spriteDirection * 20, 0) + Main.rand.NextVector2Circular(6, 2)
                                         , dir * 2);
                                    p.Effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                                    Helper.PlayPitched(CoraliteSoundID.Stinger_Item17, Projectile.Center, pitchAdjust: 0.5f);
                                    Helper.PlayPitched(CoraliteSoundID.Metal_NPCHit4, Projectile.Center, pitchAdjust: -0.6f, volumeAdjust: -0.7f);
                                }
                            }
                            else
                            if (Timer is 4 * 2 or 4 * 3 or 4 * 4)
                            {
                                int damage = Owner.GetDamageByHeldItem(25);

                                Vector2 dir = (pos - Projectile.Center).SafeNormalize(Vector2.Zero);
                                Projectile.NewProjectileFromThis<PrisonProj>(Projectile.Center
                                    , dir.RotateByRandom(-0.07f, 0.07f) * Main.rand.NextFloat(10, 12)
                                    , damage, 2, Main.rand.Next(5));

                                var p = PRTLoader.NewParticle<PrisonShootParticle>(Projectile.Center + new Vector2(Projectile.spriteDirection * 20, 0) + Main.rand.NextVector2Circular(6, 2)
                                     , dir * 2);

                                p.Effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                                Helper.PlayPitched(CoraliteSoundID.Stinger_Item17, Projectile.Center, pitchAdjust: 0.5f);
                                Helper.PlayPitched(CoraliteSoundID.Metal_NPCHit4, Projectile.Center, pitchAdjust: -0.6f, volumeAdjust: -0.7f);
                            }
                        }

                        AttackFrame();

                        if (Timer > 40)//重设状态
                        {
                            PrisonShootCount++;
                            if (PrisonShootCount > 4)
                                PrisonShootCount = 0;

                            if (!Owner.ItemTimeIsZero && ProjectileID.Sets.IsAWhip[Owner.HeldItem.shoot])
                            {
                                State = 1;
                                Timer = 0;
                            }
                            else
                                SetToBack();
                        }

                        Timer++;
                    }
                    break;
            }
        }

        #region 血腥套装效果

        public void UpdateBloodTopper()
        {
            for (int i = 0; i < BloodToppers.Count; i++)
                BloodToppers[i].Update(Projectile, i);
        }

        public void OnBloodAttack()
        {
            int count = BloodToppers.Count;
            if (count > 4)//5个帽子再叠加就全部爆开生成血滴
            {
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    int damage = Owner.GetDamageByHeldItem(25);
                    foreach (var bt in BloodToppers)
                        bt.Boom(Projectile, damage);

                    Helper.PlayPitched(CoraliteSoundID.Fleshy_NPCDeath1, Projectile.Center);
                }

                BloodToppers.Clear();
                return;
            }

            //叠加礼帽
            BloodToppers.Add(new BloodTopper(Projectile, BloodToppers.Count));
        }

        #endregion

        #region 暗影套装效果

        public void OnShadowAttack()
        {
            //生成小影怪
            Projectile.NewProjectileFromThis<LittleShadowMonster>(Projectile.Center, new Vector2(0, -8).RotateByRandom(-0.3f, 0.3f)
                , 25, 2, ShadowMonsterType);

            ShadowMonsterType++;
            if (ShadowMonsterType > 2)
                ShadowMonsterType = 0;
        }

        #endregion

        public void SetTopperType()
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.HasEffect(nameof(BloodmarkTopper)))
                    Projectile.timeLeft = 2;
                else
                    Projectile.Kill();

                TopperTypes oldType = TopperType;

                if (cp.HasEffect(BloodmarkTopper.BloodSet))
                    TopperType = TopperTypes.Blood;
                else if (cp.HasEffect(BloodmarkTopper.ShadowSet))
                    TopperType = TopperTypes.Shadow;
                else if (cp.HasEffect(BloodmarkTopper.PrisonSet))
                    TopperType = TopperTypes.Prison;
                else
                    TopperType = TopperTypes.None;

                if (TopperType != oldType)
                {
                    BloodToppers = null;
                    SetToBack();
                }
            }
        }

        public void SetToBack()
        {
            if (State != -1 && State != 0)
                State = -1;
            TargetIndex = -1;

            Timer = 0;
        }

        public void BackToOwner()
        {
            Projectile.velocity = Vector2.Zero;
            Vector2 pos = IdlePos;
            if (Vector2.Distance(pos, Projectile.Center) > 3000 || Timer > 60 * 12)
            {
                State = 0;
                SpawnScale();
                return;
            }

            Projectile.Center = Vector2.SmoothStep(Projectile.Center, pos, 0.25f);
            if (Vector2.Distance(pos, Projectile.Center) < 48)
            {
                ScaleType = ScaleTypes.Back;
                State = 0;
            }

            Timer++;
        }

        public bool FindEnemy()
        {
            if (Owner.HasMinionAttackTargetNPC)
            {
                NPC n = Main.npc[Owner.MinionAttackTargetNPC];
                if (n.CanBeChasedBy() && Vector2.Distance(n.Center, Owner.Center) < 1000)
                {
                    TargetIndex = n.whoAmI;
                    return true;
                }
            }

            if (TargetIndex.GetNPCOwner(out NPC target, SetToBack))
            {
                if (target.CanBeChasedBy()
                    && Vector2.Distance(target.Center, Owner.Center) < 1000)
                    return true;
                else
                {
                    TargetIndex = -1;
                    return false;
                }
            }

            NPC n2 = Helper.FindClosestEnemy(Owner.Center, 800, n => n.CanBeChasedBy());
            if (n2 != null)
            {
                TargetIndex = n2.whoAmI;
                return true;
            }

            return false;
        }

        public void StartAttack()
        {
            State = 1;
            Timer = 0;
        }

        public void IdleDirection()
        {
            float r = Owner.velocity.X;
            if (Math.Abs(r) > 0.2f)
                Projectile.spriteDirection = Math.Sign(r);
            else
                Projectile.spriteDirection = Owner.direction;
        }

        public void FaceToEnemy(NPC target)
        {
            float r = target.Center.X - Projectile.Center.X;
            if (Math.Abs(r) > 8)
                Projectile.spriteDirection = Math.Sign(r);
        }

        #region 缩放控制部分

        private void ControlScale()
        {
            switch (ScaleType)
            {
                default:
                case ScaleTypes.None:
                    Scale = Vector2.One;
                    break;
                case ScaleTypes.Spawn:
                    {
                        if (ScaleTimer < 4)
                            Scale.Y = Helper.Lerp(1f, 2.75f, ScaleTimer / 4);
                        else
                            Scale.Y = Helper.Lerp(2.75f, 1f, (ScaleTimer - 4) / 4);

                        Scale.X = Helper.Lerp(0.2f, 1, ScaleTimer / 8);

                        ScaleTimer++;

                        if (ScaleTimer > 8)
                        {
                            ScaleTimer = 0;
                            ScaleType = ScaleTypes.None;
                        }
                    }
                    break;
                case ScaleTypes.Attack:
                    Scale = Vector2.Lerp(Scale, new Vector2(0.75f, 1.3f), 0.2f);
                    break;
                case ScaleTypes.Back:
                    Scale = Vector2.Lerp(Scale, Vector2.One, 0.2f);
                    ScaleTimer++;

                    if (ScaleTimer > 20)
                    {
                        ScaleTimer = 0;
                        ScaleType = ScaleTypes.None;
                    }

                    break;
            }
        }

        public void SpawnScale()
        {
            ScaleTimer = 0;
            ScaleType = ScaleTypes.Spawn;
            Scale = new Vector2(0.2f, 1f);
        }

        #endregion

        #region 帧图

        public void FlyFrame()
        {
            FrameX = 0;
            Projectile.UpdateFrameNormally(5, 7);
        }

        /// <summary>
        /// 在调用之前将其<see cref="Projectile.frame"/>设置为1，不然将不起作用
        /// </summary>
        public void AttackFrame()
        {
            FrameX = 1;
            if (Projectile.frame == 0)
                return;

            Projectile.UpdateFrameNormally(4, 7);
        }

        #endregion

        #endregion

        #region 绘制

        public override bool PreDraw(ref Color lightColor)
        {
            switch (TopperType)
            {
                default:
                case TopperTypes.Error:
                case TopperTypes.None:
                    DrawTopper(Projectile.GetTexture(), lightColor);
                    break;
                case TopperTypes.Blood:
                    DrawTopper(Projectile.GetTexture(), lightColor);

                    var effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    if (BloodToppers != null)
                        foreach (var bt in BloodToppers)
                            bt.Draw(effect);
                    break;
                case TopperTypes.Shadow:
                    DrawTopper(ShadowTopper.Value, lightColor);
                    break;
                case TopperTypes.Prison:
                    DrawTopper(PrisonTopper.Value, lightColor);
                    break;
            }

            return false;
        }

        public void DrawTopper(Texture2D tex, Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;
            var frameBox = tex.Frame(2, 8, FrameX, Projectile.frame);

            Main.spriteBatch.Draw(tex, pos, frameBox, lightColor, Projectile.rotation, frameBox.Size() / 2, Scale
                , Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        #endregion
    }

    public class BloodBite : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        private float mouseDistance;
        private Vector2 offset;

        private ref float Timer => ref Projectile.localAI[0];
        private float alpha = 1;

        public override void SetDefaults()
        {
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = 48;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[1] = 1;
                Projectile.localAI[2] = Main.rand.NextFloat(-0.2f, 0.2f);
            }

            if (Timer < 5)//张嘴
                mouseDistance += 6.5f;
            else if (Timer < 9)//咬合
                mouseDistance -= 8f;
            else if (Timer == 9)
                SpawnDustOnBite();
            else//抖动加消失
            {
                if (Timer % 2 == 0)
                    offset = Helper.NextVec2Dir(2, 4);

                if (Timer > 16)
                {
                    alpha *= 0.8f;
                    if (alpha < 0.05f)
                        Projectile.Kill();
                }
            }

            Timer++;
        }

        public virtual void SpawnDustOnBite()
        {
            for (int i = 0; i < 24; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 8)
                    , DustID.Blood, Helper.NextVec2Dir(1, 3), Scale: Main.rand.NextFloat(1, 2f));
            }

            for (int i = 0; i < 2; i++)
            {
                Helper.SpawnDirDustJet(Projectile.Center + Main.rand.NextVector2Circular(32, 8)
                    , () => Helper.NextVec2Dir(), 1, 7, i => 0.5f + i * 0.35f, DustID.Blood, Scale: Main.rand.NextFloat(1f, 2f), noGravity: false);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 pos = Projectile.Center - Main.screenPosition + offset;
            Color c = Color.White * alpha;
            float rotation = Projectile.rotation + Projectile.localAI[2];
            Vector2 normal = (rotation - MathHelper.PiOver2).ToRotationVector2();
            normal *= mouseDistance;

            //绘制底层
            var frameBox = tex.Frame(2, 2, 1, 0);
            spriteBatch.Draw(tex, pos + normal, frameBox, c * 0.4f, (float)rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);
            frameBox = tex.Frame(2, 2, 1, 1);
            spriteBatch.Draw(tex, pos - normal, frameBox, c * 0.4f, (float)rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);

            //绘制顶层
            frameBox = tex.Frame(2, 2, 0, 0);
            spriteBatch.Draw(tex, pos + normal, frameBox, c, (float)rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);
            frameBox = tex.Frame(2, 2, 0, 1);
            spriteBatch.Draw(tex, pos - normal, frameBox, c, (float)rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);

            return false;
        }
    }

    public class ShadowBite : BloodBite
    {
        public override void SpawnDustOnBite()
        {
            for (int i = 0; i < 24; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 8)
                    , DustID.Shadowflame, Helper.NextVec2Dir(2, 8), Scale: Main.rand.NextFloat(1, 2f));
                d.noGravity = true;
            }
        }
    }

    public class BloodTearProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 24;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://下落
                    {
                        if (Projectile.velocity.Y < 7)
                            Projectile.velocity.Y += 0.15f;

                        Timer++;
                        if (Timer > 45)
                        {
                            if (Helper.TryFindClosestEnemy(Projectile.Center, 600, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
                            {
                                Target = target.whoAmI;
                                State = 1;
                                Projectile.timeLeft = 200;
                                Projectile.extraUpdates = 1;
                            }

                            Projectile.tileCollide = true;
                        }
                    }
                    break;
                case 1://追踪
                    {
                        if (!Target.GetNPCOwner(out NPC target))
                        {
                            Timer = 0;
                            State = 1;
                            Target = -1;

                            return;
                        }

                        float num481 = 12f;
                        Vector2 center = Projectile.Center;
                        Vector2 targetCenter = target.Center;
                        Vector2 dir = targetCenter - center;
                        float length = dir.Length();
                        if (length < 100f)
                            num481 = 10f;

                        length = num481 / length;
                        dir *= length;
                        Projectile.velocity.X = ((Projectile.velocity.X * 29f) + dir.X) / 30f;
                        Projectile.velocity.Y = ((Projectile.velocity.Y * 29f) + dir.Y) / 30f;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    break;
            }

            Projectile.UpdateFrameNormally(4, 10);
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() / 4);

            for (int i = 0; i < 4; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(12, 12)
                    , DustID.Blood, Helper.NextVec2Dir(1, 2), Scale: Main.rand.NextFloat(0.5f, 1f));
                d.noGravity = i < 2;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.IsOwnedByLocalPlayer())
            {
                if (Main.player[Projectile.owner].TryGetModPlayer(out CoralitePlayer cp))
                    cp.GetBloodPool(2);
            }

            Helper.SpawnDirDustJet(Projectile.Center
                , () => Helper.NextVec2Dir(), 1, 7, i => 0.5f + i * 0.35f, DustID.Blood, Scale: Main.rand.NextFloat(1f, 2f), noGravity: false);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectileFromThis(Projectile.Center, Vector2.Zero, ProjectileID.VampireHeal
                , 0, 0f, Projectile.owner, 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 pos = Projectile.Center - Main.screenPosition;

            Vector2 offset = Projectile.Size / 2 - Main.screenPosition;
            for (int i = 0; i < 12; i++)
            {
                float factor = 1 - i / 12f;
                Helper.DrawPrettyLine(1, 0, Projectile.oldPos[i] + offset, (Color.Red * factor) with { A = 150 }, Color.Red * factor
                    , 0.5f, 0, 0.5f, 0.5f, 1, Projectile.rotation, 1, new Vector2(0.25f, 0.75f));
            }

            Helper.DrawPrettyLine(1, 0, pos, Color.Red, Color.Red
                , 0.5f, 0, 0.5f, 0.5f, 1, Projectile.rotation, 1, new Vector2(1f, 1f));

            Projectile.QuickDraw(Projectile.GetTexture().Frame(1, 11, 0, Projectile.frame)
                , Color.White, -MathHelper.PiOver4);

            return false;
        }
    }

    public class BloodPool : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        private SecondOrderDynamics_Vec2 PosController;
        private SecondOrderDynamics_Float RotController;
        private RotateTentacle tentacle;
        public PrimitivePRTGroup group;

        public Player Owner => Main.player[Projectile.owner];

        public ref float Scale => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];

        public override bool ShouldUpdatePosition() => false;

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 24;
        }

        public override bool? CanCutTiles() => false;

        public override void AI()
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (!cp.HasEffect(BloodmarkTopper.BloodSet))
                {
                    Projectile.Kill();
                    return;
                }

                Scale = 0.6f + 0.4f * (cp.bloodPoolCount / (float)CoralitePlayer.BloodPoolCountMax);
            }

            Projectile.timeLeft = 2;

            PosController ??= new SecondOrderDynamics_Vec2(1.25f, 0.5f, 0, Projectile.Center);
            if (RotController == null)
            {
                RotController = new SecondOrderDynamics_Float(0.8f, 0.5f, 0, MathHelper.PiOver2);
                Projectile.rotation = MathHelper.PiOver2;
            }

            if (!VaultUtils.isServer)
            {
                if (tentacle == null)
                {
                    tentacle = new RotateTentacle(12, TentacleColor, f => 1 + f * 5, NightmarePlantera.tentacleTex, NightmarePlantera.tentacleFlowTex);
                    tentacle.RepeatCount = 2;
                }

                Vector2 dir = Owner.Center - Projectile.Center;
                float distance = dir.Length();
                float tentacleLength = distance * 0.8f / 10f;

                tentacle.SetValue(Projectile.Center, Owner.Center, Projectile.rotation);
                tentacle.UpdateTentacle(tentacleLength, 0.75f);

                group ??= [];

                if (Owner.TryGetModPlayer(out CoralitePlayer cp2) && cp2.bloodPoolCount == CoralitePlayer.BloodPoolCountMax && Timer > 15)
                {
                    float r = 18 + Main.rand.Next(2) * 6;
                    float startRot = Main.rand.NextFloat(-0.6f, -0.4f);
                    //总旋转路程除以转速
                    float time = (Main.rand.NextFloat(3.6f, 5.2f) - startRot) / BloodCircle.MaxRotateSpeed;

                    var p = BloodCircle.Spawn(Projectile.Center,
                             r, time, startRot, Main.rand.NextFromList(0.6f, MathHelper.Pi - 0.6f) + Main.rand.NextFloat(-0.4f, 0.4f)
                             , Main.rand.Next(4) * MathHelper.TwoPi * 0.6f + Main.rand.NextFloat(-0.4f, 0.4f), Projectile);
                    p.Color = Color.Red;
                    group.Add(p);
                    Timer = 0;
                }

                Timer++;

                if (Scale > 0.9f && Timer % 2 == 0)
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(28, 28)
                        , DustID.BloodWater, Vector2.Zero, 150);
                }

                group?.Update();
            }

            Player owner = Main.player[Projectile.owner];
            Projectile.Center = PosController.Update(1 / 60f
                , owner.MountedCenter + new Vector2(-owner.direction * 50, -16 * 3.5f));
            Projectile.rotation = RotController.Update(1 / 60f, owner.direction * 0.5f
                + Math.Clamp((Projectile.oldPosition.X - Projectile.position.X) / 20, -0.4f, 0.4f)
                + MathHelper.PiOver2
                - owner.direction * Math.Clamp(Projectile.Center.Y + 16 * 3.5f - owner.Center.Y, 0, 2f));

            Projectile.UpdateFrameNormally(7, 8, true);
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() / 2 * Scale);
        }

        public Color TentacleColor(float factor)
        {
            return Color.Lerp(Color.Transparent, Color.Red * Scale, MathF.Sin(factor * MathHelper.Pi));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            tentacle?.DrawTentacle((i) => 4 * MathF.Sin(i / 2f + (int)Main.timeForVisualEffects * 0.1f));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None
                , RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = Projectile.GetTexture();

            Vector2 pos = Projectile.Center - Main.screenPosition;
            var frameBox = tex.Frame(1, 9, 0, Projectile.frame);
            var origin = frameBox.Size() / 2;
            float scale = Scale * 0.9f;

            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                float factor = cp.bloodPoolCount / (float)CoralitePlayer.BloodPoolCountMax;

                Main.spriteBatch.Draw(tex, pos, frameBox
                    , new Color(255, 255, 255, 20 + (byte)(180 * factor)), 0, origin, scale, 0, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None
                , RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public void DrawPrimitives()
        {
            Main.graphics.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            group?.DrawPrimitive();
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }

    public class BloodCircle : TrailParticle
    {
        public override string Texture => AssetDirectory.Particles + "RedLine";

        public Projectile proj;
        public float zRot;
        public float alpha = 0;
        public float exRot;
        public float startRot;

        public static float MaxRotateSpeed = 0.25f;

        public BloodCircle()
        {
            if (Main.dedServ)
                return;
        }

        public override void SetProperty()
        {
            MaxRotateSpeed = 0.3f;
            int trailCount = 10;
            trail = new Trail(Main.instance.GraphicsDevice, trailCount, new EmptyMeshGenerator(), factor => 10 * Scale, factor =>
            {
                return new Color(Color.R, Color.G, Color.B, (byte)(255 * alpha));
            });

            oldPositions = new Vector2[trailCount];
            float r = Rotation - trailCount * MaxRotateSpeed;
            for (int i = 0; i < oldPositions.Length; i++)
            {
                float length2 = Helper.EllipticalEase(r, zRot, out float overrideAngle2) * Velocity.X;

                oldPositions[i] = (overrideAngle2 + exRot).ToRotationVector2() * length2;
                r += MaxRotateSpeed;
            }
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Rotation += MaxRotateSpeed;

            Vector2 pos = proj.Center;//特殊判定，需要手动给这个oldPos赋值

            Velocity.X *= 1.015f;
            float length = Helper.EllipticalEase(Rotation, zRot, out float overrideAngle) * Velocity.X;

            for (int i = 0; i < oldPositions.Length - 1; i++)
                oldPositions[i] = oldPositions[i + 1];

            oldPositions[^1] = (overrideAngle + exRot).ToRotationVector2() * length;

            float time = Velocity.Y;

            if (Opacity < (int)(time * 0.4f))
            {
                float factor = Opacity / (time * 0.4f);
                alpha = factor;
            }
            else if (Opacity == (int)(time * 0.4f))
            {
                alpha = 1;
            }

            if (Opacity > (int)(time * 0.9f))
            {
                alpha *= 0.9f;

                if (alpha < 0.02f)
                {
                    active = false;
                }
            }

            Opacity++;

            Vector2[] pos2 = new Vector2[oldPositions.Length];
            for (int i = 0; i < pos2.Length; i++)
                pos2[i] = pos + oldPositions[i];

            trail.TrailPositions = pos2;
        }

        public static BloodCircle Spawn(Vector2 center, float r, float time, float startRot, float zRot, float exRot, Projectile proj)
        {
            if (VaultUtils.isServer)
                return null;

            BloodCircle p = PRTLoader.PRT_IDToInstances[CoraliteContent.ParticleType<BloodCircle>()].Clone() as BloodCircle;
            p.Position = center;
            p.Velocity = new Vector2(r, time);
            p.Rotation = startRot;
            p.active = true;
            p.ShouldKillWhenOffScreen = false;
            p.Scale = 1;
            p.proj = proj;
            p.zRot = zRot;
            p.exRot = exRot;

            p.SetProperty();

            return p;
        }

        public override void DrawPrimitive()
        {
            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            //effect.Texture = Texture2D.Value;
            EffectLoader.TextureColorEffect.World = world;
            EffectLoader.TextureColorEffect.View = view;
            EffectLoader.TextureColorEffect.Projection = projection;
            EffectLoader.TextureColorEffect.Texture = TexValue;

            trail?.DrawTrail(EffectLoader.TextureColorEffect);
        }
    }

    public class BloodStrike() : BaseFrameParticle(4, 5, 3)
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;
    }

    public class LittleShadowMonster : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        public ref float ShadowStyle => ref Projectile.ai[0];
        public ref float State => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float Target => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 12);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage()
        {
            if (State != 1)
            {
                return false;
            }

            return null;
        }

        public override void AI()
        {
            Projectile.UpdateFrameNormally(4, 3);

            switch (State)
            {
                default:
                case 0://飞
                    {
                        Timer++;
                        Projectile.velocity = Projectile.velocity.RotatedBy((ShadowStyle - 1) * 0.08f);
                        if (Timer > 20)
                        {
                            if (Helper.TryFindClosestEnemy(Projectile.Center, 600, n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC target))
                            {
                                Target = target.whoAmI;
                                State = 1;
                                Projectile.timeLeft = 200;
                                Projectile.extraUpdates = 1;
                            }
                            else
                                Projectile.Kill();
                        }
                    }
                    break;
                case 1://追
                    {
                        if (!Target.GetNPCOwner(out NPC target))
                        {
                            Timer = 0;
                            State = 1;
                            Target = -1;

                            return;
                        }

                        float num481 = 12f;
                        Vector2 center = Projectile.Center;
                        Vector2 targetCenter = target.Center;
                        Vector2 dir = targetCenter - center;
                        float length = dir.Length();
                        if (length < 100f)
                            num481 = 10f;

                        length = num481 / length;
                        dir *= length;
                        Projectile.velocity.X = ((Projectile.velocity.X * 19f) + dir.X) / 20f;
                        Projectile.velocity.Y = ((Projectile.velocity.Y * 19f) + dir.Y) / 20f;
                    }
                    break;
                case 2://回
                    {
                        Projectile.tileCollide = false;
                        float num481 = 22f;
                        Vector2 center = Projectile.Center;
                        Vector2 targetCenter = Main.player[Projectile.owner].Center;
                        Vector2 dir = targetCenter - center;
                        float length = dir.Length();
                        if (length < 100f)
                            num481 = 20f;

                        length = num481 / length;
                        dir *= length;
                        Projectile.velocity.X = ((Projectile.velocity.X * 19f) + dir.X) / 20f;
                        Projectile.velocity.Y = ((Projectile.velocity.Y * 19f) + dir.Y) / 20f;

                        if (Vector2.Distance(Projectile.Center, targetCenter) < 20)
                        {
                            Projectile.Kill();
                            if (Main.player[Projectile.owner].TryGetModPlayer(out CoralitePlayer cp))
                            {
                                cp.ResetShadowTopperTime();
                                switch (ShadowStyle)
                                {
                                    default:
                                    case 0:
                                        cp.GetShadowAttackBonus();
                                        break;
                                    case 1:
                                        cp.GetShadowLifeMaxBonus();
                                        break;
                                    case 2:
                                        cp.GetshadowDefenceBonus();
                                        break;
                                }
                            }
                        }
                    }
                    break;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6)
                    , DustID.Granite, Vector2.Zero, Alpha: 150);
                d.noGravity = true;
            }

            if (Main.rand.NextBool())
            {
                Dust d2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6)
                    , DustID.Shadowflame, Vector2.Zero, Alpha: 150);
                d2.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -oldVelX * 0.7f;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY * 0.7f;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            State = 2;
            PRTLoader.NewParticle<ShadowMonsterHitParticle>(Vector2.Lerp(Projectile.Center, target.Center, 0.5f) + Main.rand.NextVector2Circular(12, 12)
                , Vector2.Zero, Scale: Main.rand.NextFloat(1, 1.5f));
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust d2 = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(6, 6)
                    , DustID.Granite, Helper.NextVec2Dir(1, 3), Alpha: 150);
                d2.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(3, 4, (int)ShadowStyle, Projectile.frame);

            Projectile.DrawShadowTrails(Color.Magenta, 0.3f, 0.3f / 12, 1, 12, 1
                , 0.03f, frameBox, 0, -1);
            Projectile.QuickDraw(frameBox, Color.White * 0.8f, 0);

            return false;
        }
    }

    public class ShadowMonsterHitParticle() : BaseFrameParticle(4, 5, 4, randRot: true)
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;
    }

    public class PrisonProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        public ref float ProjStyle => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 8);
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 14;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Timer++;

            if (ProjStyle == 0)
                Projectile.rotation = Projectile.velocity.ToRotation();
            else
                Projectile.rotation += Math.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 30;

            if (Timer > 45)
            {
                if (Projectile.velocity.Y < 7)
                    Projectile.velocity.Y += 0.07f;

                Projectile.velocity.X *= 0.98f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            PRTLoader.NewParticle<PrisonHitParticle>(Vector2.Lerp(Projectile.Center, target.Center, 0.5f) + Main.rand.NextVector2Circular(12, 12)
                , Vector2.Zero, Scale: Main.rand.NextFloat(1, 1.5f));

            for (int i = 0; i < 8; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 8)
                    , DustID.Blood, Helper.NextVec2Dir(1, 3), Scale: Main.rand.NextFloat(1, 2f));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 8)
                    , DustID.Blood, Helper.NextVec2Dir(1, 3), Scale: Main.rand.NextFloat(1, 2f));
            }

            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(1, 5, 0, (int)ProjStyle);

            Projectile.DrawShadowTrails(Color.DarkRed, 0.3f, 0.3f / 8, 1, 8, 1
                , Projectile.scale, frameBox, 0);
            Projectile.QuickDraw(frameBox, lightColor, 0);

            return false;
        }
    }

    public class FishScissors : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float ExRot => ref Projectile.localAI[0];
        public ref float Alpha => ref Projectile.localAI[1];

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            switch (State)
            {
                default:
                case 0://刚生成
                    {
                        Alpha += 0.1f;
                        if (Alpha > 1)
                            Alpha = 1;
                        Timer++;
                        Projectile.rotation = Projectile.velocity.ToRotation();

                        if (Timer > 10)
                        {
                            State++;
                            Timer = 0;
                            Projectile.tileCollide = true;
                        }
                    }
                    break;
                case 1://飞
                    {
                        Timer++;
                        Projectile.rotation = Projectile.velocity.ToRotation();

                        if (Timer > 80)
                        {
                            Timer = 0;
                            State = 3;
                        }
                    }
                    break;
                case 2://命中目标开始剪
                    {
                        if (Timer < 20)
                            ExRot += 0.04f;
                        else if (Timer < 25)
                            ExRot -= 0.04f * 2f;
                        else if (Timer == 25)
                        {
                            ExRot = 0;
                            Projectile.StartAttack();
                            for (int i = 0; i < 24; i++)
                            {
                                Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 8)
                                    , DustID.Blood, Helper.NextVec2Dir(1, 3), Scale: Main.rand.NextFloat(1, 2f));
                            }

                            for (int i = 0; i < 2; i++)
                            {
                                Helper.SpawnDirDustJet(Projectile.Center + Main.rand.NextVector2Circular(32, 8)
                                    , () => Helper.NextVec2Dir(), 1, 7, i => 0.5f + i * 0.35f, DustID.Blood, Scale: Main.rand.NextFloat(1f, 2f), noGravity: false);
                            }
                        }
                        else
                        {
                            State = 3;
                            Timer = 0;
                            Projectile.tileCollide = false;
                        }

                        Timer++;
                    }
                    break;
                case 3://消失
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 8)
                                , DustID.Blood, Helper.NextVec2Dir(1, 3), Scale: Main.rand.NextFloat(1, 2f));
                        }

                        Alpha -= 0.1f;
                        if (Timer > 10)
                            Projectile.Kill();
                        Projectile.rotation = Projectile.velocity.ToRotation();


                        Timer++;
                    }
                    break;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State < 2)
            {
                State = 2;
                Projectile.velocity *= 0.1f;
                Timer = 0;

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(32, 8)
                        , DustID.Blood, Helper.NextVec2Dir(1, 3), Scale: Main.rand.NextFloat(1, 2f));
                }
            }

            target.AddBuff(BuffType<PrisonBuff>(), 60 * 8);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            State = 3;
            Timer = 0;
            Projectile.tileCollide = false;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(1, 2, 0, 0);

            lightColor *= Alpha;
            Projectile.QuickDraw(frameBox, lightColor, MathHelper.PiOver4 + ExRot);
            frameBox = mainTex.Frame(1, 2, 0, 1);
            Projectile.QuickDraw(frameBox, lightColor, MathHelper.PiOver4 - ExRot);

            return false;
        }
    }

    public class PrisonBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.TryGetGlobalNPC(out CoraliteGlobalNPC cnpc))
                cnpc.PrisonArmorBreak = true;
        }
    }

    public class PrisonShootParticle() : BaseFrameParticle(4, 4, 3)
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;
    }
    public class PrisonHitParticle() : BaseFrameParticle(4, 3, 3, randRot: true)
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;
    }
}
