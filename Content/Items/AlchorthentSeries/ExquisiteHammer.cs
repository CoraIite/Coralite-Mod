using Coralite.Content.GlobalNPCs;
using Coralite.Content.Particles;
using Coralite.Content.Tiles.RedJades;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Core.Prefabs.Particles;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.AlchorthentSeries;

public class ExquisiteHammer : BaseAlchorthentItem
{
    public override string Texture => AssetDirectory.AlchorthentSeriesItems + "ExquisiteHammerItemSmall";

    public static Color ShineIronColor = new Color(250, 97, 105);

    public override void SetStaticDefaults()
    {
        ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
    }

    public override void SetOtherDefaults()
    {
        Item.noUseGraphic = true;
        Item.useStyle = ItemUseStyleID.Rapier;
        Item.useTime = Item.useAnimation = 30;
        Item.shoot = ProjectileType<ExquisiteAwl>();

        Item.SetWeaponValues(32, 7);
        Item.SetShopValues(Terraria.Enums.ItemRarityColor.Green2, Item.sellPrice(0, 2));
    }

    public override void Summon(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        int p = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        Main.projectile[p].originalDamage = Item.damage;

        Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<ExquisiteHammerHeldProj>(), damage * 5, knockback * 1.5f, player.whoAmI, 0, p);
    }

    public override void MinionAim(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.slotsMinions < player.maxMinions)
        {
            int p = Projectile.NewProjectile(source, player.MountedCenter + new Vector2(-player.direction * (Main.screenWidth / 2 + 100), -400), Vector2.Zero, type, damage, knockback, player.whoAmI);
            Main.projectile[p].originalDamage = Item.damage;

            (Main.projectile[p].ModProjectile as ExquisiteAwl).State = (byte)ExquisiteAwl.AIStates.OnQuickSummon;
        }

        Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<ExquisiteHammerHeldProj>(), damage * 5, knockback * 1.5f, player.whoAmI, Main.rand.Next(2, 4));
    }

    public override void SpecialAttack(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, ProjectileType<ExquisiteHammerHeldProj>(), damage * 5, knockback * 1.5f, player.whoAmI, 1);
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.FleshGrinder)
            .AddIngredient(ItemID.IronBar, 12)
            .AddIngredient(ItemID.AshGrassSeeds)
            .AddTile<MagicCraftStation>()
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.FleshGrinder)
            .AddIngredient(ItemID.LeadBar, 12)
            .AddIngredient(ItemID.AshGrassSeeds)
            .AddTile<MagicCraftStation>()
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.TheBreaker)
            .AddIngredient(ItemID.IronBar, 12)
            .AddIngredient(ItemID.AshGrassSeeds)
            .AddTile<MagicCraftStation>()
            .Register();

        CreateRecipe()
            .AddIngredient(ItemID.TheBreaker)
            .AddIngredient(ItemID.LeadBar, 12)
            .AddIngredient(ItemID.AshGrassSeeds)
            .AddTile<MagicCraftStation>()
            .Register();
    }

    public static LineDrawer NewIronAlchSymbol()
    {
        const float sqrt2D2 = 1.414f / 2;
        const float circleScale = 0.7f;

        Vector2 circleOrigin = new Vector2(sqrt2D2, -sqrt2D2) * circleScale;

        return new LineDrawer([
            //圆圈
         new LineDrawer.WarpLine(circleOrigin,36
            ,f => (-MathHelper.PiOver4+f * (MathHelper.TwoPi+0.1f)).ToRotationVector2()*circleScale,linwWidthScale:1.2f),
            //圆圈上的一条线
                new LineDrawer.StraightLine(circleOrigin,new Vector2(sqrt2D2*2,-sqrt2D2*2),AlchorthentAssets.DoubleSideBigLine),
                //箭头
                new LineDrawer.StraightLine(new Vector2(sqrt2D2*2,-sqrt2D2*2),new Vector2(sqrt2D2*2-0.6f,-sqrt2D2*2+0.2f),linwWidthScale:0.7f),
                new LineDrawer.StraightLine(new Vector2(sqrt2D2*2,-sqrt2D2*2),new Vector2(sqrt2D2*2-0.2f,-sqrt2D2*2+0.6f),linwWidthScale:0.7f),
                ]);
    }

    public static LineDrawer NewRustAlchSymbol()
    {
        const float height = 1.7f * 1.732f;

        Vector2 t = new Vector2(0, -1.1f);
        Vector2 b = new Vector2(0, 1.1f);

        Vector2 ml = new Vector2(-1, 0);
        Vector2 mr = new Vector2(1, 0);

        Vector2 bottom = b + new Vector2(0, height);
        Vector2 left = b + new Vector2(-1.7f, 0);
        Vector2 right = b + new Vector2(1.7f, 0);

        return new LineDrawer([
                //三角形
                new LineDrawer.StraightLine(bottom,right,linwWidthScale:1.2f),
                new LineDrawer.StraightLine(right,left,linwWidthScale:1.2f),
                new LineDrawer.StraightLine(left,bottom,linwWidthScale:1.2f),
                //竖线
                new LineDrawer.StraightLine(b,t),
                //横线
                new LineDrawer.StraightLine(ml,mr),
                //箭头
                new LineDrawer.StraightLine(t+new Vector2(-0.4f,0.5f),t,linwWidthScale:0.8f),
                new LineDrawer.StraightLine(t+new Vector2(0.4f,0.5f),t,linwWidthScale:0.8f),
                ]);
    }
}

/// <summary>
/// ai0传入combo，ai1传入牵引弹幕索引
/// </summary>
[VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
public class ExquisiteHammerHeldProj() : BaseSwingProj(1, 30)
{
    public override string Texture => AssetDirectory.AlchorthentSeriesItems + nameof(ExquisiteHammer);

    public ref float Combo => ref Projectile.ai[0];
    public ref float ChainedProjIndex => ref Projectile.ai[1];

    private float recordStartAngle;
    private float recordTotalAngle;
    private float extraScaleAngle;

    private float exRot;

    public int delay;
    public int alpha;

    public int StartDirection;
    public int ExDirection;

    public bool hitted = false;

    public Color highlightColor;
    public Vector2 lineMiddlePos;
    public float lineAlpha;

    public PRTGroup flameGroup;
    public int updateCount = 0;

    [VaultLoaden("{@classPath}" + "ExquisiteHammerGradient")]
    public static ATex GradientTexture { get; set; }
    [VaultLoaden("{@classPath}" + "ExquisiteHammer_Highlight")]
    public static ATex HighlightTexture { get; set; }
    [VaultLoaden("{@classPath}" + "ExquisiteHammerLine")]
    public static ATex LineTexture { get; set; }

    public override void SetSwingProperty()
    {
        Projectile.DamageType = DamageClass.SummonMeleeSpeed;
        Projectile.localNPCHitCooldown = -1;
        Projectile.width = 40;
        Projectile.height = 95;
        trailTopWidth = -10;
        distanceToOwner = -10;
        onHitFreeze = 80;
        Projectile.hide = true;
        useSlashTrail = true;
    }

    protected override float ControlTrailBottomWidth(float factor)
    {
        return 60 * Projectile.scale;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (Timer <= minTime || Timer > maxTime || Timer < 2)
            return false;

        if (target.noTileCollide || target.friendly || Projectile.hostile)
            return null;

        if (Collision.CanHit(Owner, target))
            return null;

        return false;
    }

    protected override void InitializeSwing()
    {
        if (Projectile.IsOwnedByLocalPlayer())
            Owner.direction = InMousePos.X > Owner.Center.X ? 1 : -1;

        alpha = 0;
        flameGroup ??= new PRTGroup();
        Smoother = Coralite.Instance.HeavySmootherInstance;

        switch (Combo)
        {
            default:
            case 0://召唤，连线
                ExDirection = -1;
                startAngle = -0.3f;
                totalAngle = 3f;
                minTime = 79;
                maxTime = minTime + (int)(Owner.itemTimeMax) + 14 * 5;
                delay = 5 * 12;
                extraScaleAngle = 0;
                ExtraInit();
                InitScale();

                StartDirection = Owner.direction;

                Projectile.velocity *= 0f;
                if (Owner.whoAmI == Main.myPlayer)
                    _Rotation = GetStartAngle() - (DirSign * startAngle);//设定起始角度

                Slasher();
                Smoother.ReCalculate(maxTime - minTime);

                if (useShadowTrail || useSlashTrail)
                {
                    oldRotate = new float[trailCount];
                    oldDistanceToOwner = new float[trailCount];
                    oldLength = new float[trailCount];
                    InitializeCaches();
                }

                onStart = false;
                Projectile.netUpdate = true;

                if (!ChainedProjIndex.GetProjectileOwner<ExquisiteAwl>(out _, () => ChainedProjIndex = -1))
                {
                    foreach (var proj in Main.ActiveProjectiles)
                    {
                        if (proj.owner == Projectile.owner && proj.type == ProjectileType<ExquisiteAwl>() && (proj.ModProjectile as ExquisiteAwl).CanStartSPAttack())
                        {
                            ChainedProjIndex = proj.whoAmI;
                            (proj.ModProjectile as ExquisiteAwl).SwitchToSpecailAttack();
                            break;
                        }
                    }
                }

                Helper.PlayPitchedVariants(AssetDirectory.Sounds.AlchSeries + "ExquisiteHammerRot", 0.4f, 0, 1, 2, Owner.Center);

                return;
            case 1:
                {
                    distanceToOwner = -50;

                    ExDirection = -1;
                    startAngle = -0.3f;
                    totalAngle = 3f;
                    minTime = 68;
                    maxTime = minTime + (int)(Owner.itemTimeMax) + 14 * 5;
                    delay = 5 * 12;
                    extraScaleAngle = 0;
                    ExtraInit();
                    InitScale();

                    StartDirection = Owner.direction;

                    Projectile.velocity *= 0f;
                    if (Owner.whoAmI == Main.myPlayer)
                        _Rotation = GetStartAngle() - (DirSign * startAngle);//设定起始角度

                    Slasher();
                    Smoother.ReCalculate(maxTime - minTime);

                    if (useShadowTrail || useSlashTrail)
                    {
                        oldRotate = new float[trailCount];
                        oldDistanceToOwner = new float[trailCount];
                        oldLength = new float[trailCount];
                        InitializeCaches();
                    }

                    onStart = false;
                    Projectile.netUpdate = true;
                }
                break;
            case 2:
                startAngle = 2.4f;
                totalAngle = 3.5f;
                maxTime = (int)(Owner.itemTimeMax * 0.7f) + 68;
                delay = 5 * 12;
                extraScaleAngle = Main.rand.Next(-1, 2) * 0.3f;
                ExtraInit();
                InitScale();
                SwingSound(Owner.Center);
                base.InitializeSwing();

                break;
            case 3:
                startAngle = 1.8f;
                totalAngle = 3.9f;
                maxTime = (int)(Owner.itemTimeMax * 0.7f) + 68;
                delay = 5 * 12;
                extraScaleAngle = Main.rand.Next(-1, 2) * 0.3f;
                ExtraInit();
                InitScale();
                SwingSound(Owner.Center);
                base.InitializeSwing();

                break;
        }
    }

    private void ExtraInit()
    {
        recordStartAngle = Math.Abs(startAngle);
        recordTotalAngle = Math.Abs(totalAngle);
    }

    public void InitScale()
    {
        Projectile.scale = Helper.EllipticalEase(recordStartAngle + extraScaleAngle - recordTotalAngle, 1.1f, 1.3f);
    }

    protected override void AIBefore()
    {
        Lighting.AddLight(Projectile.Center, highlightColor.ToVector3());
        base.AIBefore();
        updateCount++;
        if (updateCount >= Projectile.MaxUpdates)
        {
            updateCount = 0;
            if (flameGroup != null)
            {
                flameGroup.Update();
                Vector2 off = Owner.position - Owner.oldPosition;
                foreach (var p in flameGroup)
                    p.Position += off;
            }
        }
    }

    protected override void BeforeSlash()
    {
        switch (Combo)
        {
            default:
                break;
            case 0:
                Summon();
                break;
            case 1:
                SpecialAtack();
                break;
        }
    }

    #region Summon

    public void Summon()
    {
        /*
         * 召唤
         * 先向下滑动一点点，然后提起来，再偏转到对应位置
         */

        const int DownTime = 15;
        const int UpTime = 20;
        const int ChannelTime = 26;

        const float StartAngle = -1.5f;
        const float downAngle = 0.3f;
        const float UpAngle = -1.3f;

        if (Timer <= DownTime)//放下，接一个自身自转一圈
        {
            Owner.direction = StartDirection;
            float f = Timer / DownTime;
            float x2f = Helper.X2Ease(f);
            _Rotation = (StartDirection > 0 ? 0 : MathHelper.Pi) + StartDirection * StartAngle + StartDirection * (downAngle - StartAngle) * x2f;

            exRot = MathHelper.TwoPi * f * StartDirection;
            if (Timer == DownTime)
                ChannelSound();
        }
        else if (Timer < DownTime + UpTime)//举起
        {
            if (lineAlpha < 1)
            {
                lineAlpha += 0.2f;
                if (lineAlpha > 1)
                    lineAlpha = 1;
            }

            float baseF = (Timer - DownTime) / UpTime;
            float f = Helper.BezierEase(baseF);

            //连线中点的位置
            lineMiddlePos = Top - RotateVec2 * MathF.Sin(Helper.X2Ease(baseF) * MathHelper.Pi * 3) * 50;

            Owner.direction = StartDirection;
            _Rotation = (StartDirection > 0 ? 0 : MathHelper.Pi) + StartDirection * downAngle + StartDirection * (UpAngle - downAngle) * f;
        }
        else if (Timer == DownTime + UpTime)
        {
            startAngle = downAngle - UpAngle;
            ExDirection = 1;
            exRot = 0;
        }
        else if (Timer < DownTime + UpTime + ChannelTime)//蓄力
        {
            if (lineAlpha > 0)
            {
                lineAlpha -= 1f / ChannelTime;
                if (lineAlpha < 0)
                    lineAlpha = 0;

                if (ChainedProjIndex.GetProjectileOwner<ExquisiteAwl>(out Projectile p))
                    lineMiddlePos = Vector2.Lerp(Top + RotateVec2 * 50, (Top + p.Center) / 2, lineAlpha);
            }

            float f = Helper.SqrtEase((Timer - DownTime - UpTime) / ChannelTime);

            float rotAdd = (1 - f) * 0.13f;
            startAngle += rotAdd;
            totalAngle += rotAdd * 1.4f;
            _Rotation = _Rotation.AngleLerp(GetStartAngle() - (DirSign * startAngle), 0.25f);

            ChannelParticle();
            highlightColor = Color.Lerp(Color.Transparent, new Color(246, 71, 99) * 0.8f, f);
        }
        else if (Timer == DownTime + UpTime + ChannelTime)//完成蓄力
        {
            lineAlpha = 0;
            var p = PRTLoader.NewParticle<ExquisiteHammerSparkle>(GetTop(), Vector2.Zero);
            p.GetPos = GetTop;
        }

        Slasher();

        if (Timer == minTime)
        {
            ExtraInit();
            InitScale();

            _Rotation = startAngle = GetStartAngle() - (DirSign * startAngle);//设定起始角度
            totalAngle *= DirSign;

            InitializeCaches();
            Projectile.extraUpdates = 4;
            SwingSound(Top);
        }

        void ChannelParticle()
        {
            if (Timer < DownTime + UpTime + 20 && Timer % 3 == 0)
            {
                float rot = Timer / 3 * (MathHelper.TwoPi / 3 + 0.34372f);
                Vector2 dir = rot.ToRotationVector2();
                float currT = Timer - DownTime - UpTime;

                var p = ExquisiteBurst.Spawn(GetTop(), dir * Main.rand.NextFloat(1, 2), (ExquisiteBurst.Scales)Math.Clamp((int)currT / 6, 0, (int)ExquisiteBurst.Scales.Big), 0, GetTop, dir * Main.rand.NextFloat(4, 8));

                p.Rotation = rot;
            }
        }
    }

    public Vector2 GetTop()
        => Top - RotateVec2 * 14;

    public Vector2 GetStringPos()
    {
        Vector2 dir = RotateVec2.RotatedBy((CheckEffect() == SpriteEffects.None ? -1 : 1) * MathHelper.PiOver2);
        return Top - RotateVec2 * 28 + dir * 6;
    }

    #endregion

    #region SpecialAtack

    public void SpecialAtack()
    {
        /*
         * 开始的时候检测可以被拉拽的锥子
         * 先转向锥子并拉拽
         * 
         * 之后转向鼠标位置
         */

        const int chainTime = 15;
        const int ChannelTime = 26;

        const float downAngle = 0.3f;
        const float UpAngle = -1.3f;

        if (Timer == 0)//锥子检测
        {
            ChainedProjIndex = -1;
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.owner == Projectile.owner && proj.type == ProjectileType<ExquisiteAwl>() && (proj.ModProjectile as ExquisiteAwl).CanStartSPAttack())
                {
                    ChainedProjIndex = proj.whoAmI;
                    (proj.ModProjectile as ExquisiteAwl).SwitchToSpecailAttack();
                    break;
                }
            }
        }

        if (Timer < chainTime)//连线
        {
            if (Timer == 1)
                ChannelSound();

            float f = Timer / chainTime;
            float x2f = Helper.X2Ease(f);
            float sqf = Helper.SqrtEase(f);
            distanceToOwner = Helper.Lerp(-50, -10, sqf);

            if (ChainedProjIndex.GetProjectileOwner<ExquisiteAwl>(out Projectile proj2))
            {
                if (lineAlpha < 1)
                {
                    lineAlpha += 0.2f;
                    if (lineAlpha > 1)
                        lineAlpha = 1;
                }
                _Rotation = (proj2.Center - Owner.MountedCenter).ToRotation();
                Vector2 dir = (proj2.Center - Top).SafeNormalize(Vector2.Zero).RotatedBy(-MathHelper.PiOver2);
                lineMiddlePos = (proj2.Center + Top) / 2 + MathF.Sin(x2f * MathHelper.TwoPi) * Utils.Remap(Vector2.Distance(Top, proj2.Center), 20, 300, 10, 60) * dir;
            }
            else
                _Rotation = GetStartAngle();
            //连线中点的位置

            Owner.direction = StartDirection;
            _Rotation = (StartDirection > 0 ? 0 : MathHelper.Pi) + StartDirection * downAngle + StartDirection * (UpAngle - downAngle) * f;
        }
        else if (Timer == chainTime)
        {
            startAngle = -1.6f;
            ExDirection = 1;
            exRot = 0;
        }
        else if (Timer < chainTime + ChannelTime)//蓄力
        {
            float f = Helper.SqrtEase((Timer - chainTime) / ChannelTime);

            if (lineAlpha > 0)
            {
                lineAlpha -= 1f / ChannelTime;
                if (lineAlpha < 0)
                    lineAlpha = 0;

                if (ChainedProjIndex.GetProjectileOwner<ExquisiteAwl>(out Projectile p))
                {
                    Vector2 dir = (p.Center - Top).SafeNormalize(Vector2.Zero).RotatedBy(-MathHelper.PiOver2);
                    lineMiddlePos = (p.Center + Top) / 2 + MathF.Sin(f * MathHelper.TwoPi) * Utils.Remap(Vector2.Distance(Top, p.Center), 20, 300, 10, 60) * dir;
                }
            }

            float rotAdd = (1 - f) * 0.13f;
            startAngle -= rotAdd;
            totalAngle += rotAdd * 1.4f;
            _Rotation = _Rotation.AngleLerp(GetStartAngle() + (Owner.direction * startAngle), 0.25f);

            ChannelParticle();
            highlightColor = Color.Lerp(Color.Transparent, new Color(246, 71, 99) * 0.8f, f);
        }
        else if (Timer == chainTime + ChannelTime)//完成蓄力
        {
            lineAlpha = 0;
            var p = PRTLoader.NewParticle<ExquisiteHammerSparkle>(GetTop(), Vector2.Zero);
            p.GetPos = GetTop;
        }

        Slasher();

        if (Timer == minTime)
        {
            ExtraInit();
            InitScale();
            SwingSound(Top);

            _Rotation = startAngle = GetStartAngle() + (Owner.direction * startAngle);//设定起始角度
            totalAngle *= Owner.direction;

            InitializeCaches();
            Projectile.extraUpdates = 4;
        }

        void ChannelParticle()
        {
            if (Timer < chainTime + 20 && Timer % 3 == 0)
            {
                float rot = Timer / 3 * (MathHelper.TwoPi / 3 + 0.34372f);
                Vector2 dir = rot.ToRotationVector2();
                float currT = Timer - chainTime;

                var p = ExquisiteBurst.Spawn(GetTop(), dir * Main.rand.NextFloat(1, 2), (ExquisiteBurst.Scales)Math.Clamp((int)currT / 6, 0, (int)ExquisiteBurst.Scales.Big), 0, GetTop, dir * Main.rand.NextFloat(4, 8));

                p.Rotation = rot;
            }
        }
    }

    #endregion

    public void SwingSound(Vector2 pos)
    {
        Helper.PlayPitchedVariants(AssetDirectory.Sounds.AlchSeries + "ExquisiteHammerSwing", 0.35f, 0, 1, 3, pos);
    }

    public void ChannelSound()
    {
        Helper.PlayPitchedVariants(AssetDirectory.Sounds.AlchSeries + "ExquisiteHammerChannel", 0.2f, 0, 1, 2, Top);
    }

    protected override void OnSlash()
    {
        if (Projectile.extraUpdates != 4)
            Projectile.extraUpdates = 4;

        int timer = (int)Timer - minTime;

        bool preSwing = timer < maxTime * 0.3f;
        int timePer = Projectile.MaxUpdates;

        if (preSwing)
            SwingParticle(timer, timePer);

        if (Combo == 0)
            HitAwl();
        else if (Combo == 1)
            HitAwlSpecial();

        if (alpha < 255)
            alpha += 8;

        if (Item.type != ItemType<ExquisiteHammer>())
            Projectile.Kill();

        float angle = recordStartAngle + extraScaleAngle - (recordTotalAngle * Smoother.Smoother(timer, maxTime - minTime));

        Projectile.scale = Helper.EllipticalEase(angle, 1.1f, 1.3f);

        base.OnSlash();
    }

    private void HitAwl()
    {
        if (!hitted && ChainedProjIndex.GetProjectileOwner<ExquisiteAwl>(out Projectile chain))
        {
            //检测和锥子的碰撞
            if (Utils.CenteredRectangle(Top, new Vector2(100, 100)).Contains(chain.getRect()))
            {
                (chain.ModProjectile as ExquisiteAwl).BoostShoot();
                onHitTimer = 1;
                hitted = true;

                HitEffect(chain.Center, chain.rotation);
                Helper.PlayPitchedVariants(AssetDirectory.Sounds.AlchSeries + "ExquisiteHammerHitAwl", 0.4f, 0, 1, 2, Projectile.Center);
            }
        }
    }

    private void HitAwlSpecial()
    {
        if (!hitted && ChainedProjIndex.GetProjectileOwner<ExquisiteAwl>(out Projectile chain))
        {
            //检测和锥子的碰撞
            if (Utils.CenteredRectangle(Top, new Vector2(100, 100)).Contains(chain.getRect()))
            {
                (chain.ModProjectile as ExquisiteAwl).BoostShootSP();
                onHitTimer = 1;
                hitted = true;

                HitEffect(chain.Center, chain.rotation);
                Helper.PlayPitchedVariants(AssetDirectory.Sounds.AlchSeries + "ExquisiteHammerHitAwl", 0.4f, 0, 1, 2, Projectile.Center);
            }
        }
    }

    private void SwingParticle(int timer, int timePer)
    {
        if (Main.rand.NextBool(3) && timer % (timePer / 2) == 0)
        {
            flameGroup.NewParticle<RustParticle>(GetTop() + Main.rand.NextVector2Circular(20, 20), RotateVec2.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-3, 2), Color.White);
        }

        if (Main.rand.NextBool(2) && timer % (timePer / 2) == 0)
        {
            Color c = Main.rand.NextFromList(ExquisiteHammer.ShineIronColor, new Color(230, 48, 48)) * 0.65f;

            var p = flameGroup.NewParticle<ExquisiteFire>(GetTop() - RotateVec2 * 10 + Main.rand.NextVector2Circular(18, 18), RotateVec2.RotatedBy(Owner.direction * 1f) * Main.rand.NextFloat(1, 2), c, Main.rand.NextFloat(0.2f, 0.5f));

            if (p != null)
                p.MaxFrameCount = 2;
        }
    }

    protected override void AfterSlash()
    {
        highlightColor *= 0.97f;
        if (alpha > 20)
            alpha -= 10;

        Slasher();
        if (Timer > maxTime + delay)
            Projectile.Kill();
    }

    protected override void OnHitEvent(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Projectile.damage = (int)(Projectile.damage * 0.9f);

        if (onHitTimer != 1 || !VisualEffectSystem.HitEffect_SpecialParticles)
            return;

        Owner.MinionAttackTargetNPC = target.whoAmI;
        target.AddBuff(BuffType<RustBuff>(), 60 * 4);

        HitEffect(Vector2.Lerp(Top, target.Center, 0.1f), RotateVec2.ToRotation());
        Helper.PlayPitchedVariants(AssetDirectory.Sounds.AlchSeries + "ExquisiteHammerHitAwl", 0.35f, 0, 1, 2, Projectile.Center);
    }

    public static void HitEffect(Vector2 center, float rot)
    {
        Vector2 dir = rot.ToRotationVector2();
        //生成铁锈爆破粒子
        ExquisiteBurst p1;

        for (int i = 0; i < 2; i++)
        {
            float rot2 = rot + Main.rand.NextFromList(-1, 1) * 0.7f + Main.rand.NextFloat(-0.2f, 0.2f);
            int speed = Main.rand.Next(0, 2);
            ExquisiteBurst.Scales scale = i switch
            {
                0 => ExquisiteBurst.Scales.Small,
                _ => ExquisiteBurst.Scales.SuperSmall,
            };

            p1 = ExquisiteBurst.Spawn(center + rot2.ToRotationVector2() * 20, Vector2.Zero, scale, speed);

            p1.Rotation = rot2;
        }

        p1 = ExquisiteBurst.Spawn(center + dir * 20, Vector2.Zero, ExquisiteBurst.Scales.Big, 0);
        p1.Rotation = rot;

        //生成圆圈闪光
        var p = PRTLoader.NewParticle<ExplodeCircle>(center, Vector2.Zero, Color.DarkBlue, 0.01f);

        p.targetScale = 0.3f;
        p.fadeStartTime = 9;
        p.scaleTime = 9;
        p.highlightAlpha = 0.7f;
        p.Rotation = Main.rand.NextFloat(MathHelper.TwoPi);

        //生成高亮
        RedJades.RedExplosionParticle2.Spawn(center, 0.3f, ExquisiteHammer.ShineIronColor, 4, 8);
        RedJades.RedExplosionParticle2.Spawn(center, 0.15f, ExquisiteHammer.ShineIronColor * 1.7f, 4, 8);

        //生成一团粒子
        for (int i = 0; i < 12; i++)
            PRTLoader.NewParticle<RustParticle>(center + Main.rand.NextVector2Circular(8, 8), dir.RotateByRandom(-0.5f, 0.5f) * Main.rand.NextFloat(1, 7), Color.White, Main.rand.NextFloat(1, 1.5f));

        //速度线
        for (int i = 0; i < 4; i++)
        {
            PRTLoader.NewParticle<SpeedLine>(center + Main.rand.NextVector2Circular(8, 8), dir.RotateByRandom(-0.5f, 0.5f) * Main.rand.NextFloat(2, 7), Color.DarkBlue, Main.rand.NextFloat(0.2f, 0.4f));
        }
    }

    protected override float GetExRot()
    {
        int dir = Math.Sign(totalAngle);

        if (Timer < minTime && (Combo == 0 || Combo == 1))
            dir = DirSign * ExDirection;

        float extraRot = DirSign < 0 ? MathHelper.Pi : 0;
        extraRot += DirSign == dir ? 0 : MathHelper.Pi;
        extraRot += spriteRotation * dir;

        return extraRot + exRot;
    }

    protected override SpriteEffects CheckEffect()
    {
        if (Timer < minTime && (Combo == 0 || Combo == 1))
        {
            if (DirSign * ExDirection < 0)
                return SpriteEffects.FlipHorizontally;
            return SpriteEffects.None;
        }
        if (DirSign * ExDirection < 0)
        {
            if (totalAngle > 0)
                return SpriteEffects.None;
            return SpriteEffects.FlipHorizontally;
        }

        if (totalAngle > 0)
            return SpriteEffects.None;
        return SpriteEffects.FlipHorizontally;
    }

    #region 绘制

    public void DrawWarp()
    {
        if (oldRotate != null)
            WarpDrawer(0.75f);
    }

    protected override void DrawSlashTrail()
    {
        List<VertexPositionColorTexture> bars = new();
        GetCurrentTrailCount(out float count);

        for (int i = 0; i < count; i++)
        {
            if (oldRotate[i] == 100f)
                continue;

            float factor = 1f - (i / count);
            Vector2 Center = GetCenter(i);
            Vector2 Top = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] + trailTopWidth + oldDistanceToOwner[i]));
            Vector2 Bottom = Center + (oldRotate[i].ToRotationVector2() * (oldLength[i] - ControlTrailBottomWidth(factor) + oldDistanceToOwner[i]));

            var c = new Color(255, 255, 255) * Helper.Lerp(alpha, 0, 1 - factor);
            bars.Add(new(Top.Vec3(), c, new Vector2(factor, 0)));
            bars.Add(new(Bottom.Vec3(), c, new Vector2(factor, 1)));
        }

        if (bars.Count > 2)
        {
            Helper.DrawTrail(Main.graphics.GraphicsDevice, () =>
            {
                Effect effect = ShaderLoader.GetShader("ExquisiteHammer");

                effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
                effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.02f);
                effect.Parameters["uTimeG"].SetValue(Main.GlobalTimeWrappedHourly * 0.01f);
                effect.Parameters["udissolveS"].SetValue(0.8f);
                effect.Parameters["uBaseImage"].SetValue(CoraliteAssets.Trail.Split2.Value);
                effect.Parameters["uFlow"].SetValue(CoraliteAssets.Laser.Airflow.Value);
                effect.Parameters["uGradient"].SetValue(GradientTexture.Value);
                effect.Parameters["uDissolve"].SetValue(CoraliteAssets.Laser.EnergyFlow.Value);
                effect.Parameters["uflowPercent"].SetValue(0.8f);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes) //应用shader，并绘制顶点
                {
                    pass.Apply();
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                    Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                }
            }, BlendState.NonPremultiplied, SamplerState.PointWrap, RasterizerState.CullNone);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
        }
    }

    protected override void DrawSelf(Texture2D mainTex, Vector2 origin, Color lightColor, float extraRot)
    {
        if (Timer == 0)
            return;

        SpriteEffects effects = CheckEffect();
        Vector2 position = Projectile.Center - Main.screenPosition;
        float rotation = Projectile.rotation + extraRot;

        Main.spriteBatch.Draw(mainTex, position, null, lightColor, rotation, origin, Projectile.scale, effects, 0f);
        //Main.spriteBatch.Draw(HighlightTexture.Value, position, null, highlightColor * 1f, rotation, HighlightTexture.Size() / 2, Projectile.scale, effects, 0f);
        Main.spriteBatch.Draw(HighlightTexture.Value, position, null, highlightColor with { A = 0 }, rotation, HighlightTexture.Size() / 2, Projectile.scale, effects, 0f);

        DrawLine();
        flameGroup?.Draw(Main.spriteBatch);
    }

    public void DrawLine()
    {
        if (lineAlpha <= 0 || !ChainedProjIndex.GetProjectileOwner<ExquisiteAwl>(out Projectile p))
            return;

        DrawLine_Inner(LineTexture.Value, p.Center - Main.screenPosition);
    }

    public void DrawLine_Inner(Texture2D lineTex, Vector2 endPos)
    {
        CoraliteSystem.InitBars();
        List<ColoredVertex> bars = CoraliteSystem.Vertexes;

        float halfLineWidth = lineTex.Height / 2;
        Vector2 startPos = GetStringPos() - Main.screenPosition;

        Vector2 recordPos = startPos;
        float recordUV = 0;

        int lineLength = (int)(startPos - endPos).Length();   //链条长度
        int pointCount = lineLength / 16 + 3;
        Vector2 controlPos = lineMiddlePos - Main.screenPosition;

        //贝塞尔曲线
        for (int i = 0; i <= pointCount; i++)
        {
            float factor = (float)i / pointCount;

            Vector2 P1 = Vector2.Lerp(startPos, controlPos, factor);
            Vector2 P2 = Vector2.Lerp(controlPos, endPos, factor);

            Vector2 Center = Vector2.Lerp(P1, P2, factor);
            Vector2 p = Center + Main.screenPosition;
            var color = Lighting.GetColor((int)p.X / 16, (int)(p.Y / 16f), Color.White) * lineAlpha;

            if (factor < 0.1f)
                color *= factor / 0.1f;
            if (factor > 0.8f)
                color *= 1 - (factor - 0.8f) / 0.2f;

            Vector2 normal = (P2 - P1).SafeNormalize(Vector2.One).RotatedBy(MathHelper.PiOver2);
            Vector2 Top = Center + normal * halfLineWidth;
            Vector2 Bottom = Center - normal * halfLineWidth;

            recordUV += (Center - recordPos).Length() / lineTex.Width;

            bars.Add(new(Top, color, new Vector3(recordUV, 0, 1)));
            bars.Add(new(Bottom, color, new Vector3(recordUV, 1, 1)));

            recordPos = Center;
        }

        var state = Main.graphics.GraphicsDevice.SamplerStates[0];
        Main.graphics.GraphicsDevice.Textures[0] = lineTex;
        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
        Main.graphics.GraphicsDevice.SamplerStates[0] = state;
    }

    #endregion
}

public class ExquisiteAwlBuff : BaseAlchorthentBuff<ExquisiteAwl>
{
    public override string Texture => AssetDirectory.MinionBuffs + Name;
}

public class RustBuff : ModBuff
{
    public override string Texture => AssetDirectory.Buffs + "Buff";

    public override void Update(NPC npc, ref int buffIndex)
    {
        float strength = npc.buffTime[buffIndex] / (60 * 2f) + 1;

        if (strength > 3)
            strength = 3;

        if (npc.TryGetGlobalNPC(out CoraliteGlobalNPC cnpc))
            cnpc.Rust = (byte)strength;

        if (Main.rand.NextBool(6))
        {
            PRTLoader.NewParticle<RustParticle>(Main.rand.NextVector2FromRectangle(npc.getRect()), Helper.NextVec2Dir(0.5f, 1f), Color.White, Main.rand.NextFloat(1, 1.5f));
        }
    }
}

[VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
public class ExquisiteAwl : BaseAlchorthentMinion<ExquisiteAwlBuff>
{
    public int TexType { get; set; }

    public ref float Recorder => ref Projectile.ai[1];
    public ref float Recorder2 => ref Projectile.ai[2];
    public ref float Recorder3 => ref Projectile.localAI[1];
    public ref float Recorder4 => ref Projectile.localAI[2];

    public int WingFrame = -1;
    public int WingFrameCounter;

    /// <summary>
    /// 为1时进行一个静止状态帧图更新，为2时进行飞行中帧图更新
    /// </summary>
    public int SelfFrameState { get; private set; } = -1;

    /// <summary> 本体透明度 </summary>
    public float alpha;
    /// <summary> 拖尾透明度 </summary>
    public float trailAlpha;
    /// <summary> 特效透明度 </summary>
    public float effectAlpha;

    public bool UpdateOldPosSpecial;

    /// <summary>
    /// 翅膀帧图
    /// </summary>
    public static ATex ExquisiteWing { get; private set; }
    public static ATex ExquisiteAwl_Highlight { get; private set; }

    const int TeleportDistance = 2000;

    public const int TexTypes = 3;
    public const int IdleFrame = 14;
    public const int FlyFrame = IdleFrame + 15;

    public const int AwlFrameMax = FlyFrame + 1;
    public const int WingFrameMax = 14;

    public const float DrawOriginAdd = 10;

    public const int TrailCount = 16;

    public enum AIStates : byte
    {
        /// <summary> 刚召唤出来 </summary>
        OnSummon,
        /// <summary> 快速召唤 </summary>
        OnQuickSummon,
        /// <summary> 飞回玩家的过程 </summary>
        BackToOwner,
        /// <summary> 待机，玩家飞行 </summary>
        Idle_Flying,
        /// <summary> 待机，玩家在地面上 </summary>
        Idle_Landing,
        /// <summary> 弱化版戳刺攻击 </summary>
        SpikeAttackHeavy,
        /// <summary> 弱化版冲刺攻击 </summary>
        SpikeAttack,
        /// <summary> 将敌人钉在圈内 </summary>
        SpikeAttackSpecial,
        /// <summary> 撞到墙了 </summary>
        TileHit
    }

    public override void SetOtherDefault()
    {
        Projectile.tileCollide = false;
        Projectile.minion = true;
        Projectile.minionSlots = 1;
        Projectile.width = Projectile.height = 32;
        Projectile.localNPCHitCooldown = 20;
    }

    #region AI

    public override void Initialize()
    {
        TexType = Main.rand.Next(0, 3);
        Projectile.InitOldPosCache(TrailCount);
    }

    public override void AIMoves()
    {
        Timer++;

        switch (State)
        {
            default:
            case (int)AIStates.OnSummon:
                OnSummon();
                break;
            case (int)AIStates.OnQuickSummon:
                OnQuickSummon();
                break;
            case (int)AIStates.BackToOwner:
                BackToOwner();
                break;
            case (int)AIStates.Idle_Flying:
                //发现目标
                if (TryStartAttack())
                    break;

                //切换落地
                if (Owner.velocity.Y == 0)
                {
                    Recorder3++;
                    if (Recorder3 > 10)
                        SwitchState(AIStates.Idle_Landing);
                }

                Idle_Flying();
                break;
            case (int)AIStates.Idle_Landing:
                //发现目标
                if (TryStartAttack())
                    break;

                if (Owner.velocity.Y != 0)
                {
                    Recorder3++;
                    if (Recorder3 > 35)
                        SwitchState(AIStates.Idle_Flying);
                }

                Idle_Landing();
                break;
            case (int)AIStates.SpikeAttack:
            case (int)AIStates.SpikeAttackHeavy:
                SpikeAttack();
                break;
            case (int)AIStates.SpikeAttackSpecial:
                SpikeAttackSpecial();
                break;
            case (int)AIStates.TileHit:
                TileHit();
                break;
        }

        UpdateWingFrame();
        UpdateSelfFrame();

        if (!UpdateOldPosSpecial)
            Projectile.UpdateOldPosCache();
    }

    /// <summary>
    /// 尝试进入攻击状态
    /// </summary>
    private bool TryStartAttack()
    {
        if (Recorder2 == 0 && FindEnemy())
        {
            Helper.GetMyGroupIndexAndFillBlackList(Projectile, out int index, out _);

            Recorder3 = index * 10;
            Recorder2 = 1;
        }

        if (Recorder2 > 0)
        {
            Recorder2++;
            if (Recorder2 > Recorder3)
            {
                SwitchState(AIStates.SpikeAttack);
                return true;
            }
        }

        return false;
    }

    private void UpdateWingFrame()
    {
        if (WingFrame < 0)
            return;

        if (++WingFrameCounter > 3 * Projectile.MaxUpdates)
        {
            WingFrameCounter = 0;
            WingFrame++;
            if (WingFrame > WingFrameMax)
                WingFrame = -1;
        }
    }

    private void UpdateSelfFrame()
    {
        if (SelfFrameState == 1)
        {
            int frame = Projectile.frame;
            Projectile.UpdateFrameNormally(3 * Projectile.MaxUpdates, IdleFrame + 1);
            if (Projectile.frame == 0 && frame != 0)
                SelfFrameState = -1;
        }
        else if (SelfFrameState == 2)
        {
            int frame = Projectile.frame;
            Projectile.UpdateFrameNormally(3 * Projectile.MaxUpdates, FlyFrame);
            if (Projectile.frame == 0 && frame != 0)
                SelfFrameState = -1;
        }
    }

    public void OnSummon()
    {
        Vector2 targetCenter = Owner.MountedCenter + new Vector2(Owner.direction * 60, 80);

        const float WaitTime = 22;
        const int UpTime = 35;

        Projectile.UpdateFrameNormally(3, IdleFrame + 1);

        if (Timer < WaitTime)
        {
            Projectile.Center = targetCenter;
            return;
        }

        if (alpha < 1)
        {
            alpha += 0.08f;
            if (alpha > 1)
                alpha = 1;
        }

        if (Timer == WaitTime)
            SpreadWings(false);
        if (Timer < WaitTime + UpTime)//从地下向上飞出来
        {
            Projectile.spriteDirection = Owner.direction;
            //Projectile.rotation = MathHelper.PiOver2;

            float f = Helper.BezierEase((Timer - WaitTime) / UpTime);

            float rot;
            float length = 60 + 80 * MathF.Sin(f * MathHelper.Pi);
            if (Owner.direction > 0)
            {
                rot = 1.2f - 1.5f * f;
                Projectile.rotation = (Projectile.Center - Owner.MountedCenter).ToRotation() + MathHelper.PiOver2 * (1 - f);
            }
            else
            {
                rot = MathHelper.PiOver2 + (MathHelper.PiOver2 - 1.2f) + 1.5f * f;
                Projectile.rotation = (Projectile.Center - Owner.MountedCenter).ToRotation() - MathHelper.PiOver2 * (1 - f);
            }

            targetCenter = Owner.MountedCenter + rot.ToRotationVector2() * length;
            Projectile.Center = targetCenter;
        }
        else if (Timer == WaitTime + UpTime)//
        {
            Projectile.velocity = Vector2.Zero;
        }
        else
        {
            if (Timer > WaitTime + UpTime + 60 * 3)
                SwitchState(FindEnemy() ? AIStates.SpikeAttack : AIStates.BackToOwner);

            if (Projectile.IsOwnedByLocalPlayer())
            {
                float currTime = Timer - WaitTime - UpTime;

                targetCenter = Projectile.Center - Owner.MountedCenter;
                targetCenter = Vector2.SmoothStep(targetCenter, (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.Zero) * 16 * 5, Helper.Clamp(currTime, 0, 15) / 15f * 0.5f);

                Projectile.netUpdate = true;
                Projectile.Center = Owner.MountedCenter + targetCenter;
                //转向鼠标位置
                Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.4f);

                Projectile.spriteDirection = Math.Sign(Main.MouseWorld.X - Owner.MountedCenter.X);
            }
        }
    }

    public void OnQuickSummon()
    {
        if (Timer == 2)
        {
            SpreadWings();
            Projectile.InitOldPosCache(TrailCount);
            UpdateOldPosSpecial = false;
        }

        alpha = 1;
        trailAlpha = 1;
        Vector2 pos = Owner.MountedCenter + new Vector2(-Owner.direction * 120, -120);
        FlyToAimPos(pos, 0.75f, 15);

        Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation(), 0.2f);

        if (Vector2.Distance(Projectile.Center, pos) < 500)
            SwitchState(FindEnemy() ? AIStates.SpikeAttack : AIStates.BackToOwner);
    }

    public void BackToOwner()
    {
        Helper.GetMyGroupIndexAndFillBlackList(Projectile, out int index, out int total);
        Vector2 aimPos = (Owner.velocity.Y == 0 ? GetIdlePos2(index, total) : GetIdlePos(index, total)) + new Vector2(0, -150);

        /*
         * 距离大于2000直接传送
         * 
         * 3折现运动后冲向目标位置
         * 之后进行一小段渐进，之后冲刺出去
         * 距离小于一定后进入idle
         */
        float distanceToAimPos = Vector2.Distance(aimPos, Projectile.Center);

        if (distanceToAimPos > TeleportDistance || Recorder3 > 2)
        {
            Teleport(aimPos);
            SwitchState(AIStates.Idle_Flying);

            return;
        }

        switch (Recorder)
        {
            default:
            case 0://准备

                if (Timer == 1)
                    SpreadWings();

                RotTo(aimPos);
                Projectile.velocity *= 0.7f;
                if (Timer > 12)
                {
                    Timer = 0;
                    Recorder = 1;
                    SelfFrameState = 2;
                }

                break;
            case 1://3折线运动

                const int rot1Time = 5 + 1 + 2;
                const int rot2Time = rot1Time + 10;
                const int endRotTime = rot2Time + 5;

                FlyDust();

                if (Timer == 1)//第一段
                    PolylineMove(aimPos, distanceToAimPos, 10, MathHelper.PiOver2 - 0.1f);
                else if (Timer == rot1Time)//第二段
                    PolylineMove(aimPos, distanceToAimPos, 14, -MathHelper.PiOver2 + 0.2f);
                else if (Timer == rot2Time)//第三段
                    PolylineMove(aimPos, distanceToAimPos, 11, MathHelper.PiOver2 - 0.3f);
                else if (Timer > endRotTime)
                {
                    Timer = 0;
                    Recorder = 2;
                }
                break;
            case 2://太远就渐进
                {
                    if (distanceToAimPos > 16 * Math.Clamp(50 - Recorder3 * 20, 5, 50))
                        Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, (aimPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 16, 0.2f);
                    else
                        Projectile.velocity *= 0.2f;

                    SpriteDirectionTo(aimPos);
                    RotTo(aimPos);

                    if (Timer > 12)
                    {
                        Recorder2 = distanceToAimPos;
                        Recorder = 3;
                        //刺出
                        BoostSound();

                        Projectile.velocity = (aimPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 40;
                    }
                }
                break;
            case 3:
                {
                    FlyDust();

                    if (distanceToAimPos > Recorder2)
                    {
                        Recorder3++;//为了防止一直进入这个状态
                        Recorder = 0;
                        return;
                    }

                    if (distanceToAimPos < 40 * 5)
                    {
                        SwitchState(Owner.velocity.Y == 0 ? AIStates.Idle_Landing : AIStates.Idle_Flying);
                        return;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 折线运动，只改变速度和朝向
    /// </summary>
    /// <param name="aimPos"></param>
    /// <param name="distanceToAimPos"></param>
    /// <param name="ySpeed"></param>
    /// <param name="yRot"></param>
    /// <param name="maxDis"></param>
    /// <param name="xSpeed"></param>
    private void PolylineMove(Vector2 aimPos, float distanceToAimPos, float ySpeed, float yRot, float maxDis = 200, float xSpeed = 2)
    {
        Vector2 dirX = (aimPos - Projectile.Center).SafeNormalize(Vector2.Zero);

        Projectile.spriteDirection = MathF.Sign(aimPos.X - Projectile.Center.X);

        Projectile.velocity = dirX * Math.Clamp(Helper.SqrtEase(distanceToAimPos / maxDis), 0, 1) * xSpeed + dirX.RotatedBy(yRot) * ySpeed;
    }

    public void Idle_Flying()
    {
        /*
         * 距离大于2000直接传送
         * 
         * 渐进之后直接锁定
         */

        Helper.GetMyGroupIndexAndFillBlackList(Projectile, out int index, out int total);
        Vector2 aimPos = GetIdlePos(index, total);

        float distanceToAimPos = Vector2.Distance(aimPos, Projectile.Center);

        if (distanceToAimPos > TeleportDistance)
        {
            Teleport(aimPos);
            SwitchState(AIStates.Idle_Flying);

            return;
        }

        switch (Recorder)
        {
            default:
            case 0:
                {
                    SpriteDirectionTo(aimPos);
                    RotTo(aimPos);
                    Projectile.velocity *= 0.7f;

                    float f = 0.1f + Math.Clamp(Timer / 40f, 0, 1) * 0.9f;

                    Projectile.Center = Vector2.SmoothStep(Projectile.Center, aimPos, f);
                    if (distanceToAimPos < 24)
                    {
                        Recorder = 1;
                        Timer = 0;
                    }
                }
                break;
            case 1:
                {
                    if (total > 3 && index < total - 3)
                        trailAlpha = 0;
                    else
                        trailAlpha = 1;

                    float f2 = (float)index / total;

                    Projectile.Center = Vector2.SmoothStep(Projectile.Center, aimPos, 0.9f - 0.6f * f2);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.spriteDirection = -Owner.direction;

                    float f = (float)index / total;
                    float aimRot = MathHelper.PiOver2 + Owner.direction * (0.5f + f * (MathHelper.PiOver2 + 0.8f));

                    aimRot += MathF.Sin(Main.GlobalTimeWrappedHourly * 2 + f * MathHelper.TwoPi) * 0.1f;

                    Projectile.rotation = Projectile.rotation.AngleLerp(aimRot, 0.5f);

                    if (Timer > 180 + index * 30)
                    {
                        SelfFrameState = 1;
                        Timer = 0;
                    }
                }
                break;
        }
    }

    public void Idle_Landing()
    {
        /*
         * 距离大于2000直接传送
         * 
         * 渐进之后直接锁定
         */

        Helper.GetMyGroupIndexAndFillBlackList(Projectile, out int index, out int total);
        Vector2 aimPos = GetIdlePos2(index, total);

        float distanceToAimPos = Vector2.Distance(aimPos, Projectile.Center);

        if (distanceToAimPos > TeleportDistance)
        {
            Teleport(aimPos);
            SwitchState(AIStates.Idle_Flying);

            return;
        }

        switch (Recorder)
        {
            default:
            case 0:
                {
                    SpriteDirectionTo(aimPos);
                    RotTo(aimPos);
                    Projectile.velocity *= 0.7f;

                    float f = 0.1f + Math.Clamp(Timer / 40f, 0, 1);

                    Projectile.Center = Vector2.SmoothStep(Projectile.Center, aimPos, f);
                    if (distanceToAimPos < 20)
                    {
                        Recorder = 1;
                        Timer = 0;
                        Projectile.InitOldPosCache(TrailCount);
                    }
                }
                break;
            case 1:
                {
                    trailAlpha = 1;
                    UpdateOldPosSpecial = true;

                    float f3 = (float)index / total;

                    Projectile.Center = Vector2.SmoothStep(Projectile.Center, aimPos, 0.6f - 0.3f * f3);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.spriteDirection = -Owner.direction;

                    Vector2 dir = -Projectile.rotation.ToRotationVector2();
                    float trailPerLength = 4 + 1.5f * (1 - (float)index / total);
                    for (int i = 0; i < TrailCount; i++)
                    {
                        float f2 = 1 - (float)i / TrailCount;
                        Projectile.oldPos[i] = Vector2.SmoothStep(Projectile.oldPos[i], Projectile.Center + dir * (TrailCount - i) * trailPerLength, 1 - f2 * 0.5f);
                    }

                    Vector2 center = Owner.MountedCenter + new Vector2(-Owner.direction * 20, -16);
                    Vector2 top = center + new Vector2(0, -200);


                    float f = (float)index / total;
                    float aimRot = (aimPos - top).ToRotation();
                    aimRot += MathF.Sin(Main.GlobalTimeWrappedHourly * 2 + f * MathHelper.TwoPi) * 0.05f;

                    Projectile.rotation = Projectile.rotation.AngleLerp(aimRot, 0.5f);

                    if (Timer > 180 + index * 30)
                    {
                        SelfFrameState = 2;
                        Projectile.frame = IdleFrame;
                        Timer = 0;
                    }
                }
                break;
        }
    }

    public void SpikeAttack()
    {
        switch (Recorder)
        {
            default:
            case 0://准备
                {
                    if (!Target.GetNPCOwner(out NPC target, () => Target = -1))
                    {
                        SwitchState(AIStates.BackToOwner);
                        return;
                    }

                    Vector2 aimPos = target.Center;
                    float distanceToAimPos = Vector2.Distance(Projectile.Center, aimPos);
                    Helper.GetMyGroupIndexAndFillBlackList(Projectile, out int index, out int total);

                    if (Timer == 15)
                    {
                        SpreadWings();
                        SelfFrameState = 1;
                    }

                    RotTo(aimPos);
                    SpriteDirectionTo(aimPos);

                    if (distanceToAimPos > 16 * 5 + 8)
                    {
                        if (Projectile.velocity.Length() < 8)
                        {
                            Projectile.velocity += (aimPos - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy((0.7f + 0.4f * index / total) * (index % 2 > 0 ? -1 : 1)) * (0.3f + (float)index / total);
                        }
                    }
                    else
                        Projectile.velocity *= 0.7f;

                    if (Timer > 25)
                    {
                        Timer = 0;
                        Recorder = 1;
                        SelfFrameState = 2;
                        trailAlpha = 1;
                    }
                }

                break;
            case 1://3折线运动
                {
                    if (!Target.GetNPCOwner(out NPC target, () => Target = -1))
                    {
                        SwitchState(AIStates.BackToOwner);
                        return;
                    }

                    Vector2 aimPos = target.Center;
                    float distanceToAimPos = Vector2.Distance(Projectile.Center, aimPos);

                    RotTo(aimPos);
                    SpriteDirectionTo(aimPos);
                    FlyDust();

                    const int rot1Time = 5 + 1 + 2;
                    const int rot2Time = rot1Time + 10;
                    const int endRotTime = rot2Time + 5;
                    if (Timer == 1)//第一段
                        PolylineMove(aimPos, distanceToAimPos, 10, MathHelper.PiOver2 - 0.1f);
                    else if (Timer == rot1Time)//第二段
                        PolylineMove(aimPos, distanceToAimPos, 14, -MathHelper.PiOver2 + 0.2f);
                    else if (Timer == rot2Time)//第三段
                        PolylineMove(aimPos, distanceToAimPos, 11, MathHelper.PiOver2 - 0.3f);
                    else if (Timer > endRotTime)
                    {
                        Timer = 0;
                        Recorder = 2;
                        SelfFrameState = 2;
                    }
                }
                break;
            case 2://太远就渐进
                {
                    if (!Target.GetNPCOwner(out NPC target, () => Target = -1))
                    {
                        SwitchState(AIStates.BackToOwner);
                        return;
                    }

                    Vector2 aimPos = target.Center;
                    float distanceToAimPos = Vector2.Distance(Projectile.Center, aimPos);

                    if (distanceToAimPos > 16 * 18)
                        Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, (aimPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 16, 0.2f);
                    else if (distanceToAimPos < 16 * 13)
                        Projectile.velocity = Vector2.SmoothStep(Projectile.velocity, (Projectile.Center - aimPos).SafeNormalize(Vector2.Zero) * 16, 0.2f);
                    else
                        Projectile.velocity *= 0.5f;

                    SpriteDirectionTo(aimPos);
                    RotTo(aimPos);

                    if (Timer > 12)
                    {
                        Timer = 0;
                        Recorder2 = distanceToAimPos;
                        Recorder = 3;
                        //刺出
                        SpikeShootOut((aimPos - Projectile.Center).SafeNormalize(Vector2.Zero), 10);
                    }
                }
                break;
            case 3://飞行中

                if (Target.GetNPCOwner(out NPC target2, () => Target = -1))
                {
                    float distanceToAimPos2 = Vector2.Distance(Projectile.Center, target2.Center);

                    if (Recorder2 < distanceToAimPos2)
                    {
                        Timer = 0;
                        Projectile.extraUpdates = 0;
                        Recorder = 5;
                        return;
                    }
                }

                if (Timer > 10 * Projectile.MaxUpdates)
                {
                    Projectile.velocity *= 0.96f;
                    effectAlpha *= 0.97f;
                }
                else
                {
                    FlyDust();
                }

                if (Timer > 17 * Projectile.MaxUpdates)//没有命中就减速
                {
                    Timer = 0;
                    Projectile.extraUpdates = 0;
                    Recorder = 5;
                }

                break;
            case 4://命中后停住并粘滞
                {
                    Projectile.velocity = Vector2.Zero;
                    UpdateOldPosSpecial = true;
                    CanDamageNPC = false;

                    if (Timer > 10)
                    {
                        CanDamageNPC = true;

                        Timer = 0;
                        Recorder = 5;
                        Projectile.velocity = Projectile.rotation.ToRotationVector2() * 20;
                        UpdateOldPosSpecial = false;
                    }
                }
                break;
            case 5://穿出
                {
                    CanDamageNPC = false;

                    Projectile.tileCollide = true;
                    Projectile.velocity *= 0.95f;
                    effectAlpha *= 0.9f;
                    FlyDust();

                    if (Timer > 20)
                        SwitchState(FindEnemy() ? AIStates.SpikeAttack : AIStates.BackToOwner);
                }
                break;
        }
    }

    public void SpikeShootOut(Vector2 dir, float speed, bool playSound = true)
    {
        if (playSound)
            BoostSound();
        effectAlpha = 1;
        CanDamageNPC = true;
        Projectile.extraUpdates = 5;
        Projectile.velocity = dir * speed;

        float rot = dir.ToRotation();

        WindCircle.Spawn(Projectile.Center - dir * 10, -dir * 0.1f, rot, ExquisiteHammer.ShineIronColor, 0.6f, 0.8f, new Vector2(1.25f, 1f));
        WindCircle.Spawn(Projectile.Center + dir * 14, -dir * 0.1f, rot, ExquisiteHammer.ShineIronColor, 0.4f, 0.4f, new Vector2(1.25f, 1f));

        for (int i = -1; i < 2; i += 2)
        {
            var p = PRTLoader.NewParticle<WalkSmoke>(Projectile.Center + dir * 6, new Vector2(i * 0.4f, 0), ExquisiteHammer.ShineIronColor, 1f);
            p.direction = i;
            p.Rotation = rot - i * MathHelper.Pi;
            p.scale2 = new Vector2(Main.rand.NextFloat(1.3f, 1.7f), Main.rand.NextFloat(0.5f, 0.8f));
            p.alpha = 0.6f;
            p.addAlpha = 0.8f;
            p.addDraw = true;

            p = PRTLoader.NewParticle<WalkSmoke>(Projectile.Center + dir * 3, new Vector2(i * 0.2f, 0), ExquisiteHammer.ShineIronColor, 1f);
            p.direction = i;
            p.Rotation = rot - i * MathHelper.Pi;
            p.scale2 = new Vector2(Main.rand.NextFloat(0.6f, 0.8f), Main.rand.NextFloat(1.1f, 1.3f));
            p.alpha = 0.7f;
            p.addAlpha = 0.8f;
            p.addDraw = true;

            for (int k = 0; k < 6; k++)
                PRTLoader.NewParticle<RustParticle>(Projectile.Center, -dir.RotateByRandom(-0.4f, 0.4f) * Main.rand.NextFloat(1, 5), Color.White, Main.rand.NextFloat(1, 1.5f));
        }
    }

    public void SpikeAttackSpecial()
    {
        /*
         * 先和锤子协同运动，被击打出去并命中后生成限制圈
         */

        switch (Recorder)
        {
            default:
                break;
            case 0://刚被拽到，缓一下
                {
                    Projectile.velocity *= 0.8f;
                    effectAlpha *= 0.8f;

                    if (Projectile.IsOwnedByLocalPlayer())
                    {
                        RotTo((Main.MouseWorld - Owner.MountedCenter).ToRotation());
                        SpriteDirectionTo(MathF.Sign(Projectile.Center.X - Owner.MountedCenter.X));
                    }

                    if (Timer > 10)
                    {
                        SelfFrameState = 2;
                        effectAlpha = 0;
                        Recorder = 1;
                        Timer = 0;
                    }
                }
                break;
            case 1://向目标位置移动，等待被打出去
                {
                    if (SelfFrameState == 0)
                        SelfFrameState = 2;

                    float currTime = Timer;

                    Vector2 targetCenter = Projectile.Center - Owner.MountedCenter;
                    targetCenter = Vector2.SmoothStep(targetCenter, (Main.MouseWorld - Owner.MountedCenter).SafeNormalize(Vector2.Zero) * 16 * 5, Helper.Clamp(currTime, 0, 15) / 15f * 0.5f);

                    Projectile.netUpdate = true;
                    Projectile.Center = Owner.MountedCenter + targetCenter;

                    //转向鼠标位置
                    Projectile.rotation = Projectile.rotation.AngleLerp((Main.MouseWorld - Owner.MountedCenter).ToRotation(), 0.4f);

                    Projectile.spriteDirection = Math.Sign(Main.MouseWorld.X - Owner.MountedCenter.X);

                    if (Timer > 120)//防止特殊情况
                    {
                        SwitchState(FindEnemy() ? AIStates.SpikeAttack : AIStates.BackToOwner);
                        return;
                    }
                }
                break;
            case 2://被发射出去
                {
                    if (Timer > 19 * Projectile.MaxUpdates)//没有命中就减速
                    {
                        Projectile.velocity *= 0.96f;
                        effectAlpha *= 0.97f;
                    }
                    else
                    {
                        FlyDust();
                    }

                    if (Timer > 22 * Projectile.MaxUpdates)
                    {
                        Timer = 0;
                        Projectile.extraUpdates = 0;
                        Recorder = 5;
                    }
                }
                break;
            case 3://命中后切换到下戳定位
                {
                    effectAlpha *= 0.8f;

                    const float DisToTip = 42;

                    if (Timer < 30)
                    {
                        float rot = (Projectile.Center - new Vector2(Recorder2, Recorder3)).ToRotation();
                        rot = rot.AngleLerp(Recorder4 + MathF.Sin(Timer / 30f * MathHelper.TwoPi) * 0.3f, 0.35f);

                        Projectile.Center = new Vector2(Recorder2, Recorder3) + rot.ToRotationVector2() * DisToTip;

                        Projectile.rotation = rot + MathHelper.Pi;
                    }
                    else if (Timer < 50)
                    {
                        float rot = (Projectile.Center - new Vector2(Recorder2, Recorder3)).ToRotation();
                        rot = rot.AngleLerp(-MathHelper.PiOver2, 0.2f);

                        Projectile.Center = new Vector2(Recorder2, Recorder3) + rot.ToRotationVector2() * DisToTip;

                        Projectile.rotation = rot + MathHelper.Pi;
                    }
                    else
                    {
                        Recorder = 4;
                        Timer = 0;
                        Projectile.velocity = Vector2.Zero;
                        Projectile.rotation = MathHelper.PiOver2;
                        UpdateOldPosSpecial = true;
                        effectAlpha = 0;
                        PRTLoader.NewParticle<AlchSymbolRust>(Projectile.Center + new Vector2(0, -26 * Projectile.scale), Vector2.Zero, ExquisiteHammer.ShineIronColor);
                        Helper.PlayPitched(AssetDirectory.Sounds.AlchSeries + "ExquisiteCircleExpand", 1f, 0, Projectile.Center);
                    }
                }
                break;
            case 4://下戳定位中
                {
                    if (SelfFrameState != 2)
                        SelfFrameState = 2;

                    Vector2 dir = -Projectile.rotation.ToRotationVector2();
                    float trailPerLength = 7;
                    for (int i = 0; i < TrailCount; i++)
                    {
                        float f2 = 1 - (float)i / TrailCount;
                        Projectile.oldPos[i] = Vector2.SmoothStep(Projectile.oldPos[i], Projectile.Center + dir * (TrailCount - i) * trailPerLength, 1 - f2 * 0.5f);
                    }

                    if (Timer > 30 * 10)
                        SpecailAttackOver();
                }
                break;
            case 5://结束攻击
                {
                    if (Timer == 1)
                    {
                        SelfFrameState = 2;
                    }

                    Projectile.velocity *= 0.9f;
                    if (Timer > 20)
                        SwitchState(FindEnemy() ? AIStates.SpikeAttack : AIStates.BackToOwner);
                }
                break;
        }
    }

    public void TileHit()
    {
        effectAlpha *= 0.8f;

        if (Timer < 40)
        {
            float f = Timer / 40f;

            //减速并转两圈
            Projectile.velocity *= 0.9f;
            Projectile.rotation += 0.4f * (1 - Helper.SqrtEase(f));

            return;
        }

        SwitchState(FindEnemy() ? AIStates.SpikeAttack : AIStates.BackToOwner);
    }

    /// <summary>
    /// 能否开始特殊攻击
    /// </summary>
    /// <returns></returns>
    public bool CanStartSPAttack()
    {
        switch (State)
        {
            default:
                return true;
            case (byte)AIStates.SpikeAttack:
            case (byte)AIStates.SpikeAttackHeavy:
                return Recorder != 3 && Recorder != 4;
            case (byte)AIStates.SpikeAttackSpecial:
                return false;
        }
    }

    public override Vector2 GetIdlePos(int selfIndex, int totalCount)
    {
        Vector2 center = Owner.MountedCenter + new Vector2(-Owner.direction * 20, -16);

        const float maxRot = MathHelper.PiOver4 + 0.4f;

        float f = (float)selfIndex / totalCount;

        float rot = (Owner.direction > 0 ? MathHelper.Pi : 0) + Owner.direction * Helper.X2Ease(f) * maxRot;
        float dis = f * 80;

        return center + rot.ToRotationVector2() * dis;
    }

    public Vector2 GetIdlePos2(int selfIndex, int totalCount)
    {
        Vector2 center = Owner.MountedCenter + new Vector2(-Owner.direction * 20, -16);

        float length;

        if (totalCount < 10)
        {
            length = selfIndex * 20;
        }
        else
        {
            length = 200 * selfIndex * 1f / totalCount;
        }

        center.X += -Owner.direction * length;
        center.Y -= 40f * Helper.X3Ease((float)selfIndex / totalCount) + 5 * MathF.Sin(3 * MathF.Cos(Main.GlobalTimeWrappedHourly * 0.5f + selfIndex * MathHelper.PiOver4));

        return center;
    }

    /// <summary>
    /// 被击打出去
    /// </summary>
    public void BoostShoot()
    {
        //生成粒子
        SwitchState(AIStates.SpikeAttackHeavy);
        Target = -1;
        Recorder = 3;
        Projectile.tileCollide = true;

        SelfFrameState = 2;
        trailAlpha = 1;

        SpikeShootOut(Projectile.rotation.ToRotationVector2(), 15, false);

        Projectile.InitOldPosCache(TrailCount, false);
    }

    public void BoostShootSP()
    {
        //生成粒子
        Target = -1;
        Recorder = 2;
        Projectile.tileCollide = true;

        SelfFrameState = 2;
        trailAlpha = 1;

        SpikeShootOut(Projectile.rotation.ToRotationVector2(), 15, false);

        Projectile.InitOldPosCache(TrailCount, false);
    }

    public void SwitchToSpecailAttack()
    {
        SwitchState(AIStates.SpikeAttackSpecial);
        Projectile.velocity *= 0.8f;
        Projectile.extraUpdates = 0;

        //生成星星
        SpreadWings();

        var p = PRTLoader.NewParticle<ExquisiteHammerSparkle>(Projectile.Center, Vector2.Zero);
        p.GetPos = () => Projectile.Center;
    }

    public void SpecailAttackOver()
    {
        Timer = 0;
        Recorder = 5;
    }

    public void SpriteDirectionTo(Vector2 pos)
    {
        float dir = pos.X - Projectile.Center.X;
        if (Math.Abs(dir) > 8)
            Projectile.spriteDirection = Math.Sign(dir);
    }

    public void SpriteDirectionTo(int direction)
    {
        Projectile.spriteDirection = direction;
    }

    public void RotTo(Vector2 pos)
    {
        Projectile.rotation = Projectile.rotation.AngleLerp((pos - Projectile.Center).ToRotation(), 0.15f);
    }

    public void RotTo(float rot)
    {
        Projectile.rotation = Projectile.rotation.AngleLerp(rot, 0.15f);
    }

    /// <summary>
    /// 传送到目标位置，生成炼金术符号
    /// </summary>
    /// <param name="teleportPos"></param>
    public void Teleport(Vector2 teleportPos)
    {
        Projectile.velocity *= 0;
        Projectile.Center = teleportPos;
        Recorder = 0;

        Projectile.InitOldPosCache(TrailCount, false);

        PRTLoader.NewParticle<AlchSymbolIron>(Main.MouseWorld, Vector2.Zero, ExquisiteHammer.ShineIronColor);

        SwitchState(AIStates.Idle_Flying);
    }

    /// <summary>
    /// 展开翅膀帧图，生成一些铁锈粒子
    /// </summary>
    public void SpreadWings(bool playSound = true)
    {
        WingFrame = 0;

        Vector2 dir = -Projectile.rotation.ToRotationVector2();

        for (int i = 0; i < 10; i++)
            PRTLoader.NewParticle<RustParticle>(Projectile.Center, dir.RotatedBy(Projectile.spriteDirection * Main.rand.NextFloat(0, 0.5f)) * Main.rand.NextFloat(1, 5), Color.White, Main.rand.NextFloat(1, 1.5f));

        if (playSound)
            Helper.PlayPitchedVariants(AssetDirectory.Sounds.AlchSeries + "ExquisiteAwlWing", 0.2f, 0, 1, 3, Projectile.Center);
    }

    public void FlyDust()
    {
        if (Main.rand.NextBool(2))
        {
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Dust d = Dust.NewDustPerfect(Projectile.Center + dir * Main.rand.NextFloat(-12, 12), DustID.TheDestroyer, dir * Main.rand.NextFloat(1.5f, 2.5f), 255, Color.White, Main.rand.NextFloat(0.4f, 0.7f));
            d.noGravity = true;
        }
    }

    #endregion

    #region 状态切换

    public void SwitchState(AIStates state)
    {
        State = (byte)(state);
        Timer = 0;
        Projectile.tileCollide = false;
        CanDamageNPC = false;
        Projectile.extraUpdates = 0;

        Recorder = 0;
        Recorder2 = 0;
        Recorder3 = 0;
        Recorder4 = 0;
        if (UpdateOldPosSpecial)
            Projectile.InitOldPosCache(TrailCount);

        UpdateOldPosSpecial = false;
        effectAlpha = 0;
    }

    #endregion

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        switch (State)
        {
            default:
                break;
            case (byte)AIStates.SpikeAttack:
                OnSpikeHitNormal(Recorder3 > 0);
                target.AddBuff(BuffType<RustBuff>(), 60 * 2);

                if (Recorder3 == 0)
                {
                    Vector2 pos = Vector2.Lerp(Projectile.Center + Projectile.rotation.ToRotationVector2() * 32, target.Center, 0.2f);

                    Vector2 dir2 = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                    for (int i = 0; i < 8; i++)
                    {
                        PRTLoader.NewParticle<RustParticle>(pos, dir2.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(1, 5), Color.White, Scale: Main.rand.NextFloat(1, 1.5f));
                    }

                    float startRot = Main.rand.NextFloat(MathHelper.TwoPi);
                    for (int i = 0; i < 3; i++)
                    {
                        float currRot = startRot + i * MathHelper.TwoPi / 3 + Main.rand.NextFloat(-0.5f, 0.5f);
                        Vector2 dir = currRot.ToRotationVector2();
                        var p = ExquisiteBurst.Spawn(pos + dir * Main.rand.NextFloat(10, 20), Vector2.Zero, i switch
                        {
                            0 => ExquisiteBurst.Scales.Small,
                            1 => ExquisiteBurst.Scales.SuperSmall,
                            _ => ExquisiteBurst.Scales.Smallest,
                        }, 0);

                        p.Rotation = currRot;
                    }
                }

                Recorder3++;
                break;
            case (byte)AIStates.SpikeAttackHeavy://强化刺击，额外伤害一次
                OnSpikeHitNormal(Recorder3 > 1);
                target.AddBuff(BuffType<RustBuff>(), 60 * 4);

                target.SimpleStrikeNPC(Projectile.damage / 2, MathF.Sign(target.Center.X - Projectile.Center.X), false, 0, DamageClass.Summon);
                if (Recorder3 == 0)
                    SpikeHitSp(target);

                Recorder3++;
                break;
            case (byte)AIStates.SpikeAttackSpecial:
                {
                    CanDamageNPC = false;

                    if (!target.CanBeChasedBy())
                    {
                        Timer = 0;
                        Recorder = 5;
                        Projectile.extraUpdates = 0;

                        return;
                    }

                    Recorder = 3;
                    Timer = 0;
                    Projectile.extraUpdates = 0;
                    Recorder4 = Projectile.velocity.ToRotation() + MathHelper.Pi;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.tileCollide = false;
                    SpikeHitSp(target);
                    target.AddBuff(BuffType<RustBuff>(), 60 * 8);

                    Vector2 tipPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 42;

                    Recorder2 = tipPos.X;
                    Recorder3 = tipPos.Y;

                    foreach (var proj in Main.ActiveProjectiles)//将现有的环干掉
                        if (proj.owner == Projectile.owner && proj.type == ProjectileType<ExquisiteCircleProj>() && proj.ai[1] == target.whoAmI)
                            proj.ai[1] = -1;

                    Projectile.NewProjectileFromThis<ExquisiteCircleProj>(tipPos, Vector2.Zero, Projectile.damage, 0, Projectile.whoAmI, target.whoAmI);
                }
                break;
        }

        void SpikeHitSp(NPC target)
        {
            if (Recorder3 < 2)
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center + Projectile.rotation.ToRotationVector2() * 32, target.Center, 0.2f);
                float startRot = Main.rand.NextFloat(MathHelper.TwoPi);

                Vector2 dir2 = -Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i < 8; i++)
                {
                    PRTLoader.NewParticle<RustParticle>(pos, dir2.RotateByRandom(-0.3f, 0.3f) * Main.rand.NextFloat(1, 5), Color.White, Scale: Main.rand.NextFloat(1, 1.5f));
                }

                var p2 = ExquisiteBurst.Spawn(pos, Vector2.Zero, ExquisiteBurst.Scales.Middle, 1);

                p2.Rotation = Projectile.rotation;

                for (int i = 0; i < 4; i++)
                {
                    float currRot = startRot + i * MathHelper.TwoPi / 4 + Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 dir = currRot.ToRotationVector2();
                    var p = ExquisiteBurst.Spawn(pos + dir * Main.rand.NextFloat(10, 20), Vector2.Zero, i switch
                    {
                        0 => ExquisiteBurst.Scales.Middle,
                        1 => ExquisiteBurst.Scales.Small,
                        2 => ExquisiteBurst.Scales.SuperSmall,
                        _ => ExquisiteBurst.Scales.Smallest,
                    }, Main.rand.Next(2));

                    p.Rotation = currRot;
                }
            }
        }
    }

    public void BoostSound()
    {
        Helper.PlayPitchedVariants(AssetDirectory.Sounds.AlchSeries + "ExquisiteAwlHit", 0.3f, 0, 1, 3, Projectile.Center);
    }

    /// <summary>
    /// 切换到卡肉状态
    /// </summary>
    /// <param name="over"></param>
    public void OnSpikeHitNormal(bool over)
    {
        if (over)
            Recorder = 5;
        else
            Recorder = 4;

        Timer = 0;
        Projectile.extraUpdates = 0;
        Projectile.tileCollide = true;
        //生成粒子和音效
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Collision.HitTiles(Projectile.Center, Projectile.velocity, 16 * 2, 16 * 2);
        Helper.PlayPitched(AssetDirectory.Sounds.AlchSeries + "ExquisiteAwlTileHit", 0.3f, 0, Projectile.Center);

        switch (State)
        {
            default:
                break;
            case (byte)AIStates.SpikeAttack:
            case (byte)AIStates.SpikeAttackHeavy:
            case (byte)AIStates.SpikeAttackSpecial:
                SwitchState(AIStates.TileHit);
                //反弹
                Projectile.velocity = -oldVelocity.SafeNormalize(Vector2.Zero) * 5;
                SelfFrameState = 1;
                effectAlpha = 0;
                break;
        }

        return false;
    }

    #region Draw

    public override bool PreDraw(ref Color lightColor)
    {
        Vector2 pos = Projectile.Center - Main.screenPosition;
        float rot = Projectile.rotation + (Projectile.spriteDirection > 0 ? 0 : MathHelper.Pi);
        SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

        lightColor *= alpha;
        DrawEffect(pos);

        if (WingFrame >= 0)
            DrawWing(lightColor, pos, rot, effect);

        DrawSelf(lightColor, pos, rot, effect);
        DrawEffectAbove(pos);

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        DrawEyeRedLine();
    }

    /// <summary>
    /// 绘制翅膀
    /// </summary>
    /// <param name="lightColor"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <param name="effect"></param>
    public void DrawWing(Color lightColor, Vector2 pos, float rot, SpriteEffects effect)
    {
        Texture2D tex = ExquisiteWing.Value;
        Rectangle frame = tex.Frame(1, WingFrameMax, 0, WingFrame);
        Vector2 origin = frame.Size() / 2;
        origin.X -= Projectile.spriteDirection * DrawOriginAdd;

        Main.EntitySpriteDraw(tex, pos, frame, lightColor, rot, origin, Projectile.scale, effect);
    }

    /// <summary>
    /// 绘制本体
    /// </summary>
    /// <param name="lightColor"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <param name="effect"></param>
    public void DrawSelf(Color lightColor, Vector2 pos, float rot, SpriteEffects effect)
    {
        Texture2D tex = Projectile.GetTextureValue();
        Rectangle frame = tex.Frame(TexTypes, AwlFrameMax, TexType, Projectile.frame);
        Vector2 origin = frame.Size() / 2;
        origin.X -= Projectile.spriteDirection * DrawOriginAdd;

        Main.EntitySpriteDraw(tex, pos, frame, lightColor, rot, origin, Projectile.scale, effect);
        Main.EntitySpriteDraw(ExquisiteAwl_Highlight.Value, pos, frame, Color.White * alpha, rot, origin, Projectile.scale, effect);
    }

    public void DrawEyeRedLine()
    {
        if (trailAlpha == 0)
            return;

        Texture2D Texture = CoraliteAssets.Trail.CircleSPA.Value;

        CoraliteSystem.InitBars();
        CoraliteSystem.InitBars2();

        List<ColoredVertex> bars = CoraliteSystem.Vertexes;
        List<ColoredVertex> bars2 = CoraliteSystem.Vertexes2;

        Color color = ExquisiteHammer.ShineIronColor * trailAlpha;
        Color color2 = new Color(245, 233, 225) * 0.8f * trailAlpha;

        for (int i = 0; i < TrailCount; i++)
        {
            float factor = 1 - (float)i / TrailCount;
            Vector2 Center = Projectile.oldPos[i] - Main.screenPosition;
            Vector2 normal;
            if (i > 0)
                normal = (Projectile.oldPos[i] - Projectile.oldPos[i - 1]).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
            else
                normal = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2();

            Vector2 Top = Center + (normal * 5);
            Vector2 Bottom = Center - (normal * 5);
            Vector2 Top2 = Center + (normal * 1.25f);
            Vector2 Bottom2 = Center - (normal * 1.25f);

            Color c1 = color;
            Color c2 = color2;
            if (i > 0 && (Projectile.oldPos[i] - Projectile.oldPos[i - 1]).LengthSquared() < 1)
                c1 = c2 = Color.Transparent;

            bars.Add(new(Top, c1, new Vector3(1 - factor, 0, 1)));
            bars.Add(new(Bottom, c1, new Vector3(1 - factor, 1, 1)));
            bars2.Add(new(Top2, c2, new Vector3(1 - factor, 0, 1)));
            bars2.Add(new(Bottom2, c2, new Vector3(1 - factor, 1, 1)));
        }

        Main.graphics.GraphicsDevice.Textures[0] = Texture;
        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars2.ToArray(), 0, bars2.Count - 2);
    }

    public void DrawEffect(Vector2 pos)
    {
        if (effectAlpha == 0)
            return;

        Texture2D tex = CoraliteAssets.Trail.ArrowSPA.Value;
        Vector2 dir = -Projectile.rotation.ToRotationVector2();
        Color c2 = Color.White * effectAlpha * 0.4f;
        c2.A = 0;

        tex.QuickCenteredDraw(Main.spriteBatch, pos + dir * 18, new Vector2(0.8f, 0.4f), ExquisiteHammer.ShineIronColor * effectAlpha, Projectile.rotation);
        tex.QuickCenteredDraw(Main.spriteBatch, pos + dir * 12, new Vector2(0.6f, 0.25f), c2, Projectile.rotation);
    }

    public void DrawEffectAbove(Vector2 pos)
    {
        if (effectAlpha == 0)
            return;

        Texture2D tex = CoraliteAssets.Trail.ArrowSPA.Value;
        Vector2 dir = -Projectile.rotation.ToRotationVector2();
        Color c1 = ExquisiteHammer.ShineIronColor * effectAlpha * 0.5f;
        c1.A = 0;

        Color c2 = Color.White * effectAlpha * 0.3f;
        c2.A = 0;

        tex.QuickCenteredDraw(Main.spriteBatch, pos + dir * 12, new Vector2(0.7f, 0.3f), c1, Projectile.rotation);
        tex.QuickCenteredDraw(Main.spriteBatch, pos + dir * 12, new Vector2(0.65f, 0.3f), c2, Projectile.rotation);
    }

    #endregion
}

/// <summary>
/// ai0传入弹幕持有者，ai1传入目标
/// </summary>
public class ExquisiteCircleProj : ModProjectile
{
    public override string Texture => AssetDirectory.Blank;

    public ref float ProjOwner => ref Projectile.ai[0];
    public ref float Target => ref Projectile.ai[1];
    public ref float HitCount => ref Projectile.ai[2];

    public ref float Timer => ref Projectile.localAI[0];
    public ref float State => ref Projectile.localAI[1];

    public LineDrawer.WarpLine selfCircle;
    public LineDrawer.WarpLine targetCircle;
    public LineDrawer.StraightLine targetLine;
    public LineDrawer.WarpLine circle;
    public LineDrawer arrow;

    public const float length = 16 * 6;

    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.width = Projectile.height = 16;
        Projectile.ignoreWater = true;
    }

    public override bool? CanDamage() => false;
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
    public override bool ShouldUpdatePosition() => false;

    public override void AI()
    {
        if (!VaultUtils.isServer)
        {
            if (circle == null || arrow == null)
                InitLines();
        }

        if (!ProjOwner.GetProjectileOwner<ExquisiteAwl>(out Projectile projOwner, () => ProjOwner = -1))
        {
            if (State != 2)
                TurnToFade();
        }

        if (!Target.GetNPCOwner(out NPC target, () => Target = -1))
        {
            if (State != 2)
                TurnToFade();

            (projOwner.ModProjectile as ExquisiteAwl).SpecailAttackOver();
        }

        switch (State)
        {
            default:
            case 0://圆圈出现
                {
                    Timer++;
                    float f = Helper.SqrtEase(Timer / 20f);
                    SetAlphas(f);

                    if (Timer > 20)
                    {
                        State = 1;
                        Timer = 0;
                        HitCount = 7;
                        SetAlphas(1);
                    }
                }
                break;
            case 1://拴住并持续减少时间
                {
                    Timer++;
                    targetLine.EndPos = target.Center - Projectile.Center;

                    if (target.boss || target.realLife > 0)//BOSS出圈就直接结束
                    {
                        if (Vector2.Distance(target.Center, Projectile.Center) > length)
                        {
                            (projOwner.ModProjectile as ExquisiteAwl).SwitchState(ExquisiteAwl.AIStates.TileHit);

                            target.SimpleStrikeNPC((int)(Projectile.damage * HitCount), MathF.Sign(target.Center.X - Projectile.Center.X), false, damageType: DamageClass.Summon);
                            target.AddBuff(BuffType<RustBuff>(), 60 * 8);

                            projOwner.velocity = new Vector2(0, -6);
                            Helper.PlayPitchedVariants(AssetDirectory.Sounds.AlchSeries + "ExquisiteHammerHit", 0.1f, 0, 1, 2, Projectile.Center);

                            TurnToFade();
                        }
                    }
                    else//限制普通怪
                    {
                        if (Vector2.Distance(target.Center, Projectile.Center) > length)
                            target.Center = Projectile.Center + (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * length;
                    }

                    if (Timer % 40 == 0)//伤害目标并拖拽
                    {
                        target.SimpleStrikeNPC(Projectile.damage, MathF.Sign(target.Center.X - Projectile.Center.X), false, damageType: DamageClass.Summon);
                        target.AddBuff(BuffType<RustBuff>(), 60 * 8);

                        Helper.PlayPitched(AssetDirectory.Sounds.AlchSeries + "ExquisiteCircleHit", 0.4f, 0, Projectile.Center);

                        target.velocity += target.knockBackResist * (Projectile.Center - target.Center).SafeNormalize(Vector2.Zero) * 7;

                        float startRot = Main.rand.NextFloat(MathHelper.TwoPi);
                        for (int i = 0; i < 3; i++)
                        {
                            float currRot = startRot + i * MathHelper.TwoPi / 3 + Main.rand.NextFloat(-0.7f, 0.7f);
                            Vector2 dir = currRot.ToRotationVector2();
                            var p = ExquisiteBurst.Spawn(target.Center + dir * Main.rand.NextFloat(6, 12), Vector2.Zero, i switch
                            {
                                0 => ExquisiteBurst.Scales.Small,
                                1 => ExquisiteBurst.Scales.SuperSmall,
                                _ => ExquisiteBurst.Scales.Smallest,
                            }, 0);

                            p.Rotation = currRot;
                        }

                        startRot = Main.rand.NextFloat(MathHelper.TwoPi);
                        for (int i = 0; i < 3; i++)
                        {
                            float currRot = startRot + i * MathHelper.TwoPi / 3 + Main.rand.NextFloat(-0.7f, 0.7f);
                            Vector2 dir = currRot.ToRotationVector2();
                            var p = ExquisiteBurst.Spawn(Projectile.Center + dir * Main.rand.NextFloat(6, 12), Vector2.Zero, i switch
                            {
                                0 => ExquisiteBurst.Scales.Smallest,
                                1 => ExquisiteBurst.Scales.SuperSmall,
                                _ => ExquisiteBurst.Scales.Smallest,
                            }, 0);

                            p.Rotation = currRot;
                        }

                        if (HitCount > 3)
                            HitCount--;
                    }

                    if (Timer > 40 * 7)
                    {
                        (projOwner.ModProjectile as ExquisiteAwl).SwitchState(ExquisiteAwl.AIStates.TileHit);

                        projOwner.velocity = new Vector2(0, -6);

                        TurnToFade();
                    }
                }
                break;
            case 2://消失
                {
                    Timer++;
                    float f = 1 - Timer / 20f;

                    SetAlphas(f);

                    if (Timer > 14)
                        Projectile.Kill();
                }
                break;
        }
    }

    private void SetAlphas(float f)
    {
        circle.alpha = f;
        selfCircle.alpha = f;
        targetCircle.alpha = f;
        targetLine.alpha = f;
        arrow.SetAlpha(f);
    }

    public void TurnToFade()
    {
        State = 2;
        Timer = 0;
    }

    public void InitLines()
    {
        const float sqrt2D2 = 1.414f / 2;
        const float circleScale = 4f;

        Vector2 circleOrigin = new Vector2(sqrt2D2, -sqrt2D2) * circleScale;

        circle = new LineDrawer.WarpLine(circleOrigin, 36
            , f => (-MathHelper.PiOver4 + f * (MathHelper.TwoPi + 0.1f)).ToRotationVector2() * circleScale, linwWidthScale: 1.2f);

        circle.SetLineWidth(26);
        circle.lineScale = length / circleScale;

        selfCircle = new LineDrawer.WarpLine(circleOrigin, 24
            , f => (-MathHelper.PiOver4 + f * (MathHelper.TwoPi + 0.1f)).ToRotationVector2() * circleScale, CoraliteAssets.Laser.MultLinesSPA);
        selfCircle.SetLineWidth(12);
        selfCircle.lineScale = 1;

        targetCircle = new LineDrawer.WarpLine(circleOrigin, 24
            , f => (-MathHelper.PiOver4 + f * (MathHelper.TwoPi + 0.1f)).ToRotationVector2() * circleScale, CoraliteAssets.Laser.MultLinesSPA);
        targetCircle.SetLineWidth(12);
        targetCircle.lineScale = 1;

        targetLine = new LineDrawer.StraightLine(Vector2.Zero, Vector2.Zero, ExquisiteHammerHeldProj.LineTexture);
        targetLine.SetLineWidth(3);
        targetLine.lineScale = 1;

        Vector2 arrowTop = circleOrigin + new Vector2(sqrt2D2, -sqrt2D2);

        arrow = new LineDrawer([
            //圆圈上的一条线
                new LineDrawer.StraightLine(circleOrigin, arrowTop, AlchorthentAssets.DoubleSideBigLine),
                //箭头
                new LineDrawer.StraightLine(arrowTop, arrowTop + new Vector2(-0.6f, 0.2f), linwWidthScale: 0.7f),
                new LineDrawer.StraightLine(arrowTop, arrowTop + new Vector2(-0.2f, 0.6f), linwWidthScale: 0.7f),
                ]);

        arrow.SetLineWidth(26);
        arrow.SetScale(length / circleScale);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawLines();

        return false;
    }

    private void DrawLines()
    {
        if (circle == null || arrow == null)
        {
            InitLines();
            return;
        }

        bool hasTarget = Target.GetNPCOwner(out NPC target, () => Target = -1);

        Effect shader = ShaderLoader.GetShader("LineAdditive");

        float lineO = State switch
        {
            1 or 2 => 1,
            _ => Timer / 12f,
        };

        float alpha = 0;
        if (circle != null)
            alpha = circle.alpha;

        if (hasTarget)
            targetLine?.Render(Projectile.Center - Main.screenPosition);

        shader.Parameters["uFlowTex"]?.SetValue(CoraliteAssets.Laser.TwistLaser.Value);
        shader.Parameters["uTime"]?.SetValue((int)Main.timeForVisualEffects * 0.02f);
        shader.Parameters["flowAdd"]?.SetValue(4);
        shader.Parameters["lineO"]?.SetValue(lineO);
        shader.Parameters["lineC"]?.SetValue(ExquisiteHammer.ShineIronColor.ToVector4() * alpha);
        shader.Parameters["powC"]?.SetValue(0.2f);
        shader.Parameters["lineEx"]?.SetValue(0.5f);
        shader.Parameters["transformMatrix"]?.SetValue(VaultUtils.GetTransfromMatrix());

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend
            , SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);

        //绘制箭头
        arrow?.Draw(Projectile.Center);
        //绘制圆圈
        circle?.Render(Projectile.Center);

        shader.Parameters["flowAdd"]?.SetValue(2);
        shader.Parameters["lineC"]?.SetValue((new Color(130, 50, 60) * alpha).ToVector4());

        shader.CurrentTechnique.Passes[0].Apply();

        selfCircle?.Render(Projectile.Center);
        if (hasTarget)
            targetCircle?.Render(target.Center);

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
    }
}

public class ExquisiteHammerSparkle : Particle
{
    public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

    public Func<Vector2> GetPos;

    public override void SetProperty()
    {
        PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
        Frame.Width = 1;
        Frame.Height = 7;
        Scale = 1.5f;
    }

    public override void AI()
    {
        if (GetPos != null)
        {
            Position = GetPos();
        }

        if (Opacity % 3 == 0)
        {
            Frame.Y++;
            if (Frame.Y > 6)
            {
                active = false;
            }
        }

        Opacity++;
    }

    public override bool PreDraw(SpriteBatch spriteBatch)
    {
        TexValue.QuickCenteredDraw(spriteBatch, Frame, Position - Main.screenPosition, Color.White, 0, Scale);
        return false;
    }
}

public class ExquisiteHammerSparkle2 : Particle
{
    public override string Texture => AssetDirectory.Sparkles + "ShotLineSPA";

    public Color addColor = Color.Transparent;
    public Vector2 scale1;
    public Vector2 scale2;

    public ExquisiteHammerHeldProj owner;

    public override void AI()
    {
        Opacity++;

        const int ShinyTime = 5;
        const int ShinyTime2 = 6;
        const int FadeTime = 10;

        Position = owner.GetTop();

        if (Opacity < ShinyTime)
        {
            float f = Helper.X2Ease(Opacity / ShinyTime);
            Color = Color.Lerp(new Color(255, 211, 132), new Color(244, 231, 222), f);
            addColor = Color.Lerp(Color.Transparent, Color.White, f);

            scale1 = Vector2.SmoothStep(Vector2.Zero, new Vector2(0.8f, 0.7f), f);
            scale2 = Vector2.SmoothStep(Vector2.Zero, new Vector2(0.4f, 0.5f), f);
        }
        else if (Opacity < ShinyTime + ShinyTime2)
        {
            float f = Helper.BezierEase((Opacity - ShinyTime) / ShinyTime2);
            addColor = Color.Lerp(Color.White, new Color(255, 211, 132), f);
            Color = Color.Lerp(new Color(244, 231, 222), new Color(246, 71, 99), f);
            scale1 = Vector2.SmoothStep(new Vector2(0.8f, 0.7f), new Vector2(0.4f, 0.5f), f);
            scale2 = Vector2.SmoothStep(new Vector2(0.4f, 0.5f), new Vector2(0.7f, 0.7f), f);
        }
        else if (Opacity < ShinyTime + ShinyTime2 + FadeTime)
        {
            float f = (Opacity - ShinyTime - ShinyTime2) / FadeTime;
            addColor = Color.Lerp(new Color(255, 211, 132), Color.Transparent, f);
            Color = Color.Lerp(new Color(246, 71, 99), Color.Transparent, f);

            scale1 = Vector2.SmoothStep(new Vector2(0.4f, 0.5f), Vector2.Zero, f);
            scale2 = Vector2.SmoothStep(new Vector2(0.7f, 0.7f), Vector2.Zero, f);
        }
        else
        {
            active = false;
        }
    }

    public override bool PreDraw(SpriteBatch spriteBatch)
    {
        Texture2D tex = TexValue;
        Vector2 pos = Position - Main.screenPosition;

        const float r1 = -1.1f;
        const float r2 = r1 + MathHelper.PiOver2;

        //绘制闪闪1层
        TexValue.QuickCenteredDraw(spriteBatch, pos, scale1, Color, r1);
        TexValue.QuickCenteredDraw(spriteBatch, pos, scale2, Color, r2);

        //绘制闪闪2层
        Color c = addColor;
        addColor.A = 0;
        TexValue.QuickCenteredDraw(spriteBatch, pos, scale1 * 0.4f, c, r1);
        TexValue.QuickCenteredDraw(spriteBatch, pos, scale2 * 0.4f, c, r2);

        return false;
    }
}

[VaultLoaden(AssetDirectory.AlchorthentSeriesItems)]
public class ExquisiteBurst : Particle
{
    public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

    public static ATex ExquisiteBurst_Smallest { get; set; }
    public static ATex ExquisiteBurst_SuperSmall { get; set; }
    public static ATex ExquisiteBurst_Middle { get; set; }
    public static ATex ExquisiteBurst_Big { get; set; }

    public Scales texScale;

    public Func<Vector2> GetPos;
    public Vector2 offset;

    public int frameCounterMax;
    public int frameXCount;

    public enum Scales
    {
        Smallest,
        SuperSmall,
        Small,
        Middle,
        Big
    }

    public override void SetProperty()
    {
        Frame = new Rectangle(0, 0, 0, 0);
        PRTDrawMode = PRTDrawModeEnum.NonPremultiplied;
    }

    public override void AI()
    {
        if (GetPos != null)
        {
            Position = GetPos() + offset;
            offset += Velocity;
            Velocity *= 0.94f;
        }

        if (++Opacity > frameCounterMax)
        {
            Opacity = 0;
            if (++Frame.X >= frameXCount)
                active = false;
        }
    }

    public virtual void Follow(Projectile proj)
    {
        Position += (proj.position - proj.oldPosition);
    }

    public static ExquisiteBurst Spawn(Vector2 pos, Vector2 vel, Scales texScale, int frameCounterMax, Func<Vector2> GetPos = null, Vector2? offset = null)
    {
        if (VaultUtils.isServer)
        {
            return null;
        }

        var p = PRTLoader.NewParticle<ExquisiteBurst>(pos, vel, Color.White);
        p.texScale = texScale;
        p.frameCounterMax = frameCounterMax;
        p.GetPos = GetPos;
        if (offset != null)
            p.offset = offset.Value;
        p.SetFrameXMax();

        return p;
    }

    public void SetFrameXMax()
    {
        frameXCount = 13;
    }

    public override bool PreDraw(SpriteBatch spriteBatch)
    {
        Texture2D tex = texScale switch
        {
            Scales.Smallest => ExquisiteBurst_Smallest.Value,
            Scales.SuperSmall => ExquisiteBurst_SuperSmall.Value,
            Scales.Small => TexValue,
            Scales.Middle => ExquisiteBurst_Middle.Value,
            Scales.Big => ExquisiteBurst_Big.Value,
            _ => TexValue,
        };

        var frameBox = tex.Frame(frameXCount, 1, Frame.X, 0);

        spriteBatch.Draw(tex, Position - Main.screenPosition, frameBox
            , Color, Rotation + MathHelper.PiOver2, new Vector2(frameBox.Width / 2, frameBox.Height * 0.8f), Scale, 0, 0);

        return false;
    }
}

public class RustParticle() : BaseFrameParticle(3, 12, 2, randRot: true)
{
    public override string Texture => AssetDirectory.AlchorthentSeriesItems + Name;

    public override void SetProperty()
    {
        base.SetProperty();

        Frame.Y = Main.rand.Next(0, 3);
    }

    public override void AI()
    {
        base.AI();
        Velocity *= 0.98f;
        Velocity.Y += 0.1f;
        Rotation += 0.02f;
    }
}

public class ExquisiteFire() : FireParticleSPA
{
    public override string Texture => AssetDirectory.Particles + "FireParticleSPA";

    public override void AI()
    {
        Opacity++;

        if (Opacity % MaxFrameCount == 0)
        {
            Frame.Y++;
            if (Frame.Y > 15)
                active = false;
        }

        Velocity *= 0.95f;
        if (Opacity > 8)
            Color = Color.Lerp(Color, new Color(2, 13, 50) * 0.5f, 0.12f);
    }
}

public class AlchSymbolIron : BaseAlchSymbol
{
    public override LineDrawer GetSymbolLine() => ExquisiteHammer.NewIronAlchSymbol();

    public override bool FadeLineOffset => false;
    public override bool FadeScale => false;
    public override bool FadeColor => true;

    public override void SetProperty()
    {
        base.SetProperty();
        maxScale = 18;
        fadeTime = 20;
        ShineTime = 14;
        disappearTime = 20;
    }
}

public class AlchSymbolRust : BaseAlchSymbol
{
    public override LineDrawer GetSymbolLine() => ExquisiteHammer.NewRustAlchSymbol();

    public override bool FadeLineOffset => false;
    public override bool FadeScale => false;
    public override bool FadeColor => true;

    public override void SetProperty()
    {
        base.SetProperty();
        maxScale = 15;
        fadeTime = 9;
        ShineTime = 12;
        disappearTime = 20;
    }
}
