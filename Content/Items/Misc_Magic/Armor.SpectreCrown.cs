using Coralite.Content.Items.LandOfTheLustrousSeries;
using Coralite.Content.Items.LandOfTheLustrousSeries.Accessories;
using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Configs;
using Coralite.Core.Loaders;
using Coralite.Helpers;
using InnoVault.Trails;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.Items.Misc_Magic;

[AutoloadEquip(EquipType.Head)]
[PlayerEffect]
public class SpectreCrown : ModItem, IHookPlayerShoot
{
    public override string Texture => AssetDirectory.Misc_Magic + Name;
    public static LocalizedText bonus;

    public override void Load()
    {
        bonus = this.GetLocalization("ArmorBonus");
    }

    public override void Unload()
    {
        bonus = null;
    }

    public override void SetStaticDefaults()
    {
        ArmorIDs.Head.Sets.DrawHatHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
        ArmorIDs.Head.Sets.DrawFullHair[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head)] = true;
    }

    public override void SetDefaults()
    {
        Item.value = Item.sellPrice(0, 7, 50);
        Item.rare = ItemRarityID.Yellow;
        Item.defense = 8;

        Item.DamageType = DamageClass.Magic;
        Item.damage = 60;
    }

    public override void UpdateEquip(Player player)
    {
        player.statManaMax2 += 20;
        player.GetDamage(DamageClass.Magic) += 0.05f;
        player.GetCritChance(DamageClass.Magic) += 5f;
        if (!player.isDisplayDollOrInanimate)
            player.socialGhost = true;
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ItemID.SpectreRobe && legs.type == ItemID.SpectrePants;
    }

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = bonus.Value;
        if (player.TryGetModPlayer(out CoralitePlayer cp))
        {
            cp.ShootHooks.Add(this);
            if (!player.HasBuff<SpectreCrownCD>())
                cp.AddEffect(nameof(SpectreCrown));
            cp.GemWeaponAttSpeedBonus += 0.1f;
        }
    }

    public override void PreUpdateVanitySet(Player player)
    {
        if (!player.isDisplayDollOrInanimate)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp) && cp.HasEffect(nameof(SpectreCrown)))
                player.socialGhost = true;

            player.SetArmorEffectVisuals(player);
        }
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.SpectreBar, 12)
            .AddTile(TileID.MythrilAnvil)
            .Register();
    }

    public void PlayerShoot(Player player, Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (!item.DamageType.CountsAsClass(DamageClass.Magic))
            return;

        int type2 = ModContent.ProjectileType<SpectreCrownProj>();

        if (player.ownedProjectileCounts[type2] < 1)
            Projectile.NewProjectile(player.GetSource_ItemUse(Item), position, Vector2.Zero, ModContent.ProjectileType<SpectreCrownProj>(), player.GetWeaponDamage(Item), player.GetWeaponKnockback(Item), player.whoAmI, ai2: 1);

        foreach (var p in Main.ActiveProjectiles)
            if (p.owner == player.whoAmI && p.type == type2)
            {
                (p.ModProjectile as SpectreCrownProj).StartAttack();
                break;
            }
    }
}

public class SpectreCrownCD : ModBuff
{
    public override string Texture => AssetDirectory.Buffs+"Buff";

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}

public class SpectreCrownProj : BaseGemWeaponProj<SmokyRing>
{
    public override string Texture => AssetDirectory.Misc_Magic + Name;

    public ref float TargetProj => ref Projectile.ai[1];
    public ref float AttackState => ref Projectile.ai[2];
    public ref float Timer => ref Projectile.localAI[2];

    public override void InitializeGemWeapon()
    {
        TargetProj = -1;
    }

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Projectile.QuickTrailSets(Helper.TrailingMode.OnlyPosition, 3);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = Projectile.height = 45;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.penetrate = -1;
    }

    public override bool OwnerItemCheck()
    {
        if (Owner.TryGetModPlayer(out CoralitePlayer cp))
        {
            if (!cp.HasEffect(nameof(SmokyRing)))
            {
                Projectile.Kill();
                return false;
            }

            return true;

        }

        return false;
    }

    public override void BeforeMove()
    {
        if (!VaultUtils.isServer && (int)Main.timeForVisualEffects % 20 == 0 && Main.rand.NextBool(2))
        {
            float length = Main.rand.NextFloat(8, 24);
            Color c = Main.rand.NextFromList(SpectreCrystalProj.highlightC, SpectreCrystalProj.brightC, SpectreCrystalProj.darkC);
            var cs = CrystalShine.Spawn(Projectile.Center + Main.rand.NextVector2CircularEdge(length, length)
                 , Helper.NextVec2Dir(0.1f, 0.2f), 5, new Vector2(0.5f, 0.03f) * Main.rand.NextFloat(0.5f, 1f), c);
            cs.follow = () => Projectile.position - Projectile.oldPos[1];
            cs.TrailCount = 3;
            cs.fadeTime = Main.rand.Next(30, 50);
            cs.shineRange = 12;
        }
    }

    public override void Move()
    {
        Timer++;

        if (Timer % 20 == 0 && TargetProj == -1)//寻找宝石武器弹幕
        {
            foreach (var p in Main.ActiveProjectiles)
                if (p.owner == Projectile.owner && p.whoAmI != Projectile.whoAmI && CoraliteSets.Projectiles.GemWeapon[p.type])
                {
                    TargetProj = p.whoAmI;
                    break;
                }
        }

        Vector2 idlePos;
        if (TargetProj.GetProjectileOwner(out Projectile proj, () => TargetProj = -1))
        {
            if (!CoraliteSets.Projectiles.GemWeapon[proj.type])
                TargetProj = -1;

            idlePos = proj.Center + new Vector2(0, -proj.height / 2 - 48); ;
        }
        else
            idlePos = Owner.MountedCenter + new Vector2(0, -Owner.height / 2 - 48);

        Projectile.rotation = Projectile.rotation.AngleLerp(Math.Clamp(Owner.velocity.X / 50, -0.6f, 0.6f), 0.2f);

        TargetPos = Vector2.SmoothStep(TargetPos, idlePos, 0.5f);
        Projectile.Center = Vector2.Lerp(Projectile.Center, TargetPos, 0.5f);
        Lighting.AddLight(Projectile.Center, SmokyCrystalProj.brightC.ToVector3() / 2);
    }

    public override void Attack()
    {
        if (AttackTime > 0)
            AttackTime--;
    }

    public override void StartAttack()
    {
        if (AttackTime != 0)
            return;

        AttackTime = 60;
        AttackState++;
        if (AttackState > 3)
        {
            AttackState = 0;
            //射出幽魂水晶

            ShootCrystal();
        }
    }

    private void ShootCrystal()
    {
        Vector2 dir = new Vector2(0, -1);

        for (int i = 0; i < 6; i++)
        {
            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Ghost, dir.RotateByRandom(-0.4F, 0.4F) * Main.rand.NextFloat(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
            d.noGravity = true;
        }

        for (int i = 0; i < 3; i++)
        {
            SmokyCrystalProj.SpawnTriangleParticle(Projectile.Center + (dir.RotateByRandom(-0.4F, 0.4F) * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
        }

        Projectile.NewProjectileFromThis<SmokyCrystalProj>(Projectile.Center, dir * 12, Projectile.damage, Projectile.knockBack);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Projectile.GetTextureValue();
        Vector2 pos = Projectile.Center - Main.screenPosition;

        Color c = Color.Lerp(lightColor, Color.Transparent, 0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f));
        c.A = 0;

        tex.QuickCenteredDraw(Main.spriteBatch, pos, c);

        return false;
    }
}

public class SpectreCrystalProj : ModProjectile, IDrawPrimitive, IDrawNonPremultiplied
{
    public override string Texture => AssetDirectory.LandOfTheLustrousSeriesItems + "LozengeProj";

    public Vector2 rand = Main.rand.NextVector2CircularEdge(64, 64);

    public ref float State => ref Projectile.ai[0];
    public ref float Target => ref Projectile.ai[1];
    public ref float Timer => ref Projectile.ai[2];
    public ref float HitTileCount => ref Projectile.localAI[0];

    public static Color highlightC = new(245, 243, 245);
    public static Color brightC = new(158, 160, 158);
    public static Color darkC = new(129, 154, 233);

    private Trail trail;

    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.width = Projectile.height = 18;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        if (!VaultUtils.isServer && trail == null)
        {
            const int maxPoint = 12;
            trail ??= new Trail(Main.graphics.GraphicsDevice, maxPoint, new EmptyMeshGenerator()
                , factor => Helper.Lerp(2, 13, factor),
                  factor =>
                  {
                      return Color.Lerp(new Color(0, 0, 0, 0), Color.White * 0.65f, factor.X);
                  });

            Projectile.InitOldPosCache(maxPoint);
        }

        switch (State)
        {
            default:
            case 0://刚刚生成
                Spawn();
                break;
            case 1://追踪
                Projectile.ShimmerGoesUp(-9, -0.4f);
                Chase();
                break;
            case 2:
                Timer++;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Timer > 30)
                    Projectile.Kill();
                break;
        }

        if (VaultUtils.isServer)
            return;

        if (Projectile.timeLeft % 3 == 0)
            SpawnTriangleParticle(Projectile.Center + Main.rand.NextVector2Circular(12, 12), Projectile.velocity * Main.rand.NextFloat(0.2f, 0.4f));
        if (Main.rand.NextBool(5))
            Projectile.SpawnTrailDust(8f, DustID.AncientLight, Main.rand.NextFloat(0.2f, 0.4f));

        Projectile.UpdateFrameNormally(8, 19);
        Projectile.UpdateOldPosCache();
        trail.TrailPositions = Projectile.oldPos;
    }

    public void Spawn()
    {
        const int ChaseTime = 10;
        const int MaxTime = ChaseTime + 60;

        float angle = Projectile.velocity.ToRotation();
        float speed = Projectile.velocity.Length();

        angle = angle.AngleTowards(-1.57f, Helper.X2Ease(Math.Clamp(Timer / 40, 0, 1)));

        if (Timer > ChaseTime)//开始追踪阶段
        {
            speed *= 0.97f;
            if (Helper.TryFindClosestEnemy(Projectile.Center, 1000, n => n.CanBeChasedBy(), out NPC target))
            {
                Target = target.whoAmI;
                State = 1;
                Projectile.timeLeft = 600;
                Projectile.extraUpdates = 1;
            }
        }

        Projectile.velocity = angle.ToRotationVector2() * speed;
        Projectile.rotation = Projectile.velocity.ToRotation();

        if (Timer > MaxTime)
            Projectile.Kill();

        Timer++;
    }

    public void Chase()
    {
        if (!Target.GetNPCOwner(out NPC target))
        {
            Timer = 0;
            State = 2;

            return;
        }

        float num481 = 18f;
        Vector2 center = Projectile.Center;
        Vector2 targetCenter = target.Center;
        Vector2 dir = targetCenter - center;
        float length = dir.Length();
        if (length < 100f)
            num481 = 14f;

        length = num481 / length;
        dir *= length;
        Projectile.velocity.X = ((Projectile.velocity.X * 19f) + dir.X) / 20f;
        Projectile.velocity.Y = ((Projectile.velocity.Y * 19f) + dir.Y) / 20f;
        Projectile.rotation = Projectile.velocity.ToRotation();
    }

    public static void SpawnTriangleParticle(Vector2 pos, Vector2 velocity)
    {
        Color c1 = highlightC;
        c1.A = 125;
        Color c2 = brightC;
        c2.A = 125;
        Color c3 = darkC;
        c3.A = 100;
        Color c = Main.rand.NextFromList(highlightC, brightC, c1, c2, c3);
        CrystalTriangle.Spawn(pos, velocity, c, 9, Main.rand.NextFloat(0.05f, 0.3f));
    }

    public override void OnKill(int timeLeft)
    {
        if (VisualEffectSystem.HitEffect_Dusts)
            for (int i = 0; i < 6; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.AncientLight, Helper.NextVec2Dir(2, 4), Scale: Main.rand.NextFloat(1f, 1.5f));
                d.noGravity = true;
            }

        if (VisualEffectSystem.HitEffect_SpecialParticles)
            for (int i = 0; i < 3; i++)
            {
                Vector2 dir = Helper.NextVec2Dir();
                SpawnTriangleParticle(Projectile.Center + (dir * Main.rand.NextFloat(6, 12)), dir * Main.rand.NextFloat(1f, 3f));
            }
    }

    public override bool PreDraw(ref Color lightColor) => false;

    public void DrawPrimitives()
    {
        if (trail == null)
            return;

        Effect effect = ShaderLoader.GetShader("CrystalTrail");

        Texture2D noiseTex = GemTextures.CrystalNoises[Projectile.frame].Value;

        effect.Parameters["noiseTexture"].SetValue(noiseTex);
        effect.Parameters["TrailTexture"].SetValue(CoraliteAssets.Laser.EnergyFlow.Value);
        effect.Parameters["transformMatrix"].SetValue(VaultUtils.GetTransfromMatrix());
        effect.Parameters["basePos"].SetValue((Projectile.Center - Main.screenPosition + rand) * Main.GameZoomTarget);
        effect.Parameters["scale"].SetValue(new Vector2(0.7f / Main.GameZoomTarget));
        effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * (Main.gamePaused ? 0.02f : 0.01f));
        effect.Parameters["lightRange"].SetValue(0.2f);
        effect.Parameters["lightLimit"].SetValue(0.25f);
        effect.Parameters["addC"].SetValue(0.65f);
        effect.Parameters["highlightC"].SetValue(highlightC.ToVector4());
        effect.Parameters["brightC"].SetValue(brightC.ToVector4());
        effect.Parameters["darkC"].SetValue(darkC.ToVector4());

        trail.DrawTrail(effect);
    }

    public void DrawNonPremultiplied(SpriteBatch spriteBatch)
    {
        rand.X += 0.15f;

        Helper.DrawCrystal(spriteBatch, Projectile.frame, Projectile.Center + rand, new Vector2(0.7f)
            , ((float)(Main.timeForVisualEffects + Projectile.timeLeft) * (Main.gamePaused ? 0.02f : 0.01f)) + (Projectile.whoAmI / 3f)
            , highlightC, brightC, darkC, () =>
            {
                Texture2D mainTex = Projectile.GetTextureValue();
                spriteBatch.Draw(mainTex, Projectile.Center, null, Color.White, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, 0, 0);
            }, sb =>
            {
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }, 0.1f, 0.65f, 0.5f);
    }
}
