using Coralite.Content.Items.Materials;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.SmoothFunctions;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Misc_Equip
{
    [AutoloadEquip(EquipType.Head)]
    [PlayerEffect(ExtraEffectNames = [BloodSet, ShadowSet, PrisonSet])]
    public class BloodmarkTopper : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        /// <summary> 与血腥套触发的套装效果 </summary>
        public const string BloodSet = "BloodmarkTopperA";
        /// <summary> 与暗影套触发的套装效果 </summary>
        public const string ShadowSet = "BloodmarkTopperB";
        public const string ShadowSetVinity = $"{AssetDirectory.Misc_Equip}BloodmarkTopperShadow_Head";
        public const string ShadowSetVinityName = "BloodmarkTopperShadow";
        /// <summary> 与黑曜石套触发的套装效果 </summary>
        public const string PrisonSet = "BloodmarkTopperC";
        public const string PrisonSetVinity = $"{AssetDirectory.Misc_Equip}BloodmarkTopperPrison_Head";
        public const string PrisonSetVinityName = "BloodmarkTopperPrison";

        public static LocalizedText[] EXToolTip { get; private set; }
        public static LocalizedText[] Bonus { get; private set; }
        public static LocalizedText[] EXName { get; private set; }
        public static LocalizedText SetTip { get; private set; }

        private enum ArmorSetType
        {
            Blood,
            Shadow,
            Prison,

            Count
        }

        private enum EXToolTipID//额外的物品提示
        {
            None,
            Blood,
            Shadow,
            Prison,

            Count
        }

        public override void Load()
        {
            EquipLoader.AddEquipTexture(Mod, ShadowSetVinity, EquipType.Head, name: ShadowSetVinityName);
            EquipLoader.AddEquipTexture(Mod, PrisonSetVinity, EquipType.Head, name: PrisonSetVinityName);

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

        public void AddMagikeCraftRecipe()
        {
            int magikeCost = MagikeHelper.CalculateMagikeCost(MALevel.CrystallineMagike, 12, 60 * 4);
            MagikeRecipe.CreateCraftRecipe(ItemID.CrimsonHelmet, ItemType<BloodmarkTopper>(), magikeCost)
                .AddIngredient<BloodyOrb>()
                .AddIngredient(ItemID.TopHat)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<MutatusInABottle>()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.BloodMoonStarter)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.ShadowHelmet, ItemType<BloodmarkTopper>(), magikeCost)
                .AddIngredient<BloodyOrb>()
                .AddIngredient(ItemID.TopHat)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<MutatusInABottle>()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.BloodMoonStarter)
                .Register();
            MagikeRecipe.CreateCraftRecipe(ItemID.AncientShadowHelmet, ItemType<BloodmarkTopper>(), magikeCost)
                .AddIngredient<BloodyOrb>()
                .AddIngredient(ItemID.TopHat)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<MutatusInABottle>()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.BloodMoonStarter)
                .Register();

            MagikeRecipe.CreateCraftRecipe(ItemID.ObsidianHelm, ItemType<BloodmarkTopper>(), magikeCost)
                .AddIngredient<BloodyOrb>()
                .AddIngredient(ItemID.TopHat)
                .AddIngredient<DeorcInABottle>()
                .AddIngredient<MutatusInABottle>()
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddIngredient(ItemID.BloodMoonStarter)
                .Register();
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

                        break;
                    case ArmorSetType.Shadow:
                        cp.AddEffect(ShadowSet);
                        player.statDefense += 8;

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
                if (cp.HasEffect(ShadowSet))
                    player.armorEffectDrawShadow = true;
            }
        }
    }

    public class BloodRarity : ModRarity
    {
        public override Color RarityColor =>
            Color.Lerp(new Color(255, 30, 30), new(160, 14, 46), 0.5f + 0.5f * MathF.Sin((int)Main.timeForVisualEffects * 0.1f));
    }

    [AutoLoadTexture(Path = AssetDirectory.Misc_Equip)]
    public class BloodmarkTopperProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Misc_Equip + Name;

        public static ATex BloodTopperSpawn { get; private set; }
        [AutoLoadTexture(Name = "BloodmarkTopperProjShadow")]
        public static ATex ShadowTopper { get; private set; }
        [AutoLoadTexture(Name = "BloodmarkTopperProjPrison")]
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

        /// <summary>
        /// 血礼帽的数量
        /// </summary>
        public int BloodTopperCount {  get; set; }

        public List<BloodTopper> BloodToppers { get; private set; }

        private Vector2 Scale;
        private int FrameX;
        private bool init = false;

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

            public Vector2 Pos;
            public float Rot;

            public int frame;
            public int frameCounter;

            public BloodTopper(Projectile projectile,int index)
            {
                Pos = projectile.Center + new Vector2(0, 8 + index * 24);
                posMover = new SecondOrderDynamics_Vec2(0.97f - index * 0.03f, 0.5f, 0, Pos);
            }

            public void Update(Projectile projectile, int index)
            {
                Vector2 targetP = projectile.Center + new Vector2(0, 8 + index * 24);
                Pos = posMover.Update(1 / 60f, targetP);

            }

            public void Boom(Projectile projectile)
            {

            }

            public void Draw()
            {

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
                    NormalAI<BloodBite>(65, null);
                    break;
                case TopperTypes.Blood:
                    UpdateBloodTopper();
                    NormalAI<BloodBite>(80, OnBloodAttack);
                    break;
                case TopperTypes.Shadow:
                    NormalAI<ShadowBite>(80, OnShadowAttack);
                    break;
                case TopperTypes.Prison:

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
                    break;
                case 0://寻敌
                    {
                        Projectile.Center = IdlePos;

                        if (Projectile.IsOwnedByLocalPlayer() && Timer > 20)
                        {
                            Timer = 0;
                            if (FindEnemy())
                                StartAttack();
                        }

                        Timer++;
                        FlyFrame();
                        IdleDirection();
                    }
                    break;
                case 1://冲刺到敌怪头上
                    {
                        if (!TargetIndex.GetNPCOwner(out NPC target, SetToBack))
                            return;

                        if (Timer < 20)//准备动作
                        {
                            Projectile.Center = IdlePos;

                            Projectile.rotation =
                                Projectile.rotation.AngleLerp((target.Center - Projectile.Center).ToRotation(), 0.16f);
                        }
                        else if (Timer < 28)//冲刺到目标头顶上
                        {
                            Projectile.Center = Vector2.Lerp(Projectile.Center, target.Top, 0.5f);
                            Projectile.rotation = (target.Center - Projectile.Center).ToRotation();
                        }
                        else if (Timer < 28 + 12)//摆正位置
                        {
                            Projectile.Center = target.Top;
                            Projectile.rotation = Projectile.rotation.AngleLerp(0, 0.2f);
                        }
                        else
                        {
                            State++;
                            Timer = 0;
                            Projectile.frame = 1;
                            return;
                        }

                        Timer++;
                        FaceToEnemy(target);
                        FlyFrame();
                    }
                    break;
                case 2://向下咬攻击敌怪
                    {
                        if (!TargetIndex.GetNPCOwner(out NPC target, SetToBack))
                            return;

                        AttackFrame();
                        Projectile.Center = target.Top;

                        if (Projectile.IsOwnedByLocalPlayer() && Timer == 3 * 4)//生成咬合弹幕
                        {
                            int damage = projDamage;
                            if (!Owner.HeldItem.IsAir && Owner.HeldItem.damage > 0 && !Owner.HeldItem.IsTool())
                                damage = (int)(Owner.GetDamage(Owner.HeldItem.DamageType).ApplyTo(damage));

                            Projectile.NewProjectileFromThis<T>(Main.rand.NextVector2FromRectangle(target.getRect())
                                , Vector2.Zero, damage, 0);

                            onAttack?.Invoke();
                        }

                        if (Timer > 50)//重设状态
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

        #region 血腥套装效果

        public void UpdateBloodTopper()
        {
            BloodToppers ??= new List<BloodTopper>(8);

            for (int i = 0; i < BloodToppers.Count; i++)
                BloodToppers[i].Update(Projectile, i);
        }

        public void OnBloodAttack()
        {
            int count = BloodToppers.Count;
            if (count > 7)//8个帽子再叠加就全部爆开生成血滴
            {
                BloodTopperCount = 0;
                if (Projectile.IsOwnedByLocalPlayer())
                    foreach (var bt in BloodToppers)
                    {
                        bt.Boom(Projectile);
                    }

                BloodToppers.Clear();
                return;
            }

            //叠加礼帽


        }

        #endregion

        #region 暗影套装效果

        public void OnShadowAttack()
        {
            //生成小影怪
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
                    SetToBack();
            }
        }

        public void SetToBack()
        {
            State = -1;
            TargetIndex = -1;

            Timer = 0;
        }

        public void BackToOwner()
        {
            Vector2 pos = IdlePos;
            if (Vector2.Distance(pos, Projectile.Center) > 3000 || Timer > 60 * 12)
            {
                Projectile.Center = pos;
                State = 0;
                SpawnScale();
                return;
            }

            Projectile.Center = Vector2.SmoothStep(Projectile.Center, pos, 0.25f);
            if (Vector2.Distance(pos, Projectile.Center) < 16)
            {
                Projectile.Center = pos;
                State = 0;
            }

            Timer++;
        }

        public bool FindEnemy()
        {
            if (Owner.HasMinionAttackTargetNPC)
            {
                NPC n = Main.npc[Owner.MinionAttackTargetNPC];
                if (n.CanBeChasedBy() && Vector2.Distance(n.Center, Owner.Center) < 1500)
                {
                    TargetIndex = n.whoAmI;
                    return true;
                }
            }

            if (TargetIndex.GetNPCOwner(out NPC target, SetToBack))
            {
                if (target.CanBeChasedBy()
                    && Vector2.Distance(target.Center, Owner.Center) < 1500)
                    return true;
                else
                    return false;
            }

            NPC n2 = Helper.FindClosestEnemy(Owner.Center, 1000, n => n.CanBeChasedBy());
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
                        if (ScaleTimer < 6)
                            Scale.Y = Helper.Lerp(1f, 1.75f, ScaleTimer / 6);
                        else
                            Scale.Y = Helper.Lerp(1f, 1.75f, (ScaleTimer - 6) / 8);

                        Scale.X = Helper.Lerp(0.2f, 1, ScaleTimer / 14);

                        ScaleTimer++;

                        if (ScaleTimer > 14)
                        {
                            ScaleTimer = 0;
                            ScaleType = ScaleTypes.None;
                        }
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
            Projectile.UpdateFrameNormally(4, 7);
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
                case TopperTypes.Blood:
                    DrawTopper(Projectile.GetTexture(), lightColor);
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
        }

        public override void AI()
        {
            if (Projectile.localAI[1]==0)
            {
                Projectile.localAI[1] = 1;
                Projectile.localAI[2] = Main.rand.NextFloat(-0.2f, 0.2f);
            }

            if (Timer < 4)//张嘴
                mouseDistance += 6;
            else if (Timer < 7)//咬合
                mouseDistance -= 7.5f;
            else//抖动加消失
            {
                if (Timer % 2 == 0)
                    offset = Helper.NextVec2Dir(2, 4);

                if (Timer > 14)
                {
                    alpha *= 0.8f;
                    if (alpha<0.05f)
                        Projectile.Kill();
                }
            }

            Timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 pos = Projectile.Center - Main.screenPosition + offset;
            Color c = lightColor * alpha;
            float rotation = Projectile.rotation + Projectile.localAI[2];
            Vector2 normal = (rotation - MathHelper.PiOver2).ToRotationVector2();
            normal *= mouseDistance;

            //绘制底层
            var frameBox = tex.Frame(2, 2, 1, 0);
            spriteBatch.Draw(tex, pos + normal, frameBox, c * 0.5f, (float)rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);
            frameBox = tex.Frame(2, 2, 1, 1);
            spriteBatch.Draw(tex, pos - normal, frameBox, c * 0.5f, (float)rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);

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

    }
}
