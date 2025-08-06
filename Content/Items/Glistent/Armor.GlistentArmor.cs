using Coralite.Content.DamageClasses;
using Coralite.Content.GlobalNPCs;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Core.Systems.FairyCatcherSystem;
using Coralite.Core.Systems.FairyCatcherSystem.Bases;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Glistent
{
    [AutoloadEquip(EquipType.Head)]
    public class GlistentHelmet : ModItem, IControllableArmorBonus
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

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
        }

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 60, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ItemType<GlistentBreastplate>()
                && legs.type == ItemType<GlistentLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(FairyDamage.Instance) += 0.07f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = bonus.Value;
            player.GetDamage<FairyDamage>() += 0.05f;
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
                fcp.fairyCatchPowerBonus += 0.05f;
        }

        public void UseArmorBonus(Player player)
        {
            //没有BUFF就丢一颗寄生种子
            if (!player.HasBuff<LeechSeedCD>())
            {
                Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero);

                Projectile.NewProjectile(player.GetSource_ItemUse(Item)
                    , player.MountedCenter, dir * 15
                    , ProjectileType<LeechSeed>(), (int)player.GetTotalDamage(FairyDamage.Instance).ApplyTo(30)
                    , 2, player.whoAmI);

                player.AddBuff(BuffType<LeechSeedCD>(), 60 * 12);

                Helper.PlayPitched(CoraliteSoundID.VileSpit_NPCDeath9, player.Center);
                Helper.PlayPitched(CoraliteSoundID.Stinger_Item17, player.Center);

                Color lightGreen = new Color(182, 255, 171);

                //生成特效
                for (int i = 0; i < 5; i++)
                {
                    Vector2 dir2 = dir.RotateByRandom(-0.3f, 0.3f);
                    FlowLineThin.Spawn(player.MountedCenter + Helper.NextVec2Dir(4, 10)
                        , dir2 * Main.rand.NextFloat(2.5f, 5), 7, 20, Main.rand.NextFloat(-0.15f, 0.15f)
                        , Main.rand.NextFromList(Color.LimeGreen, lightGreen));
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlistentBar>(8)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<LeafeoHelmet>()
                .AddIngredient(ItemID.Diamond, 2)
                .AddIngredient(ItemID.DemoniteBar, 7)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<LeafeoHelmet>()
                .AddIngredient(ItemID.Diamond, 2)
                .AddIngredient(ItemID.CrimtaneBar, 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class GlistentBreastplate : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 90, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 6;
        }

        public override void UpdateEquip(Player player)
        {
            if (player.TryGetModPlayer(out FairyCatcherPlayer fcp))
            {
                fcp.fairyCatchPowerBonus += 0.1f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlistentBar>(10)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<LeafeoLightArmor>()
                .AddIngredient(ItemID.Diamond, 4)
                .AddIngredient(ItemID.DemoniteBar, 9)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<LeafeoLightArmor>()
                .AddIngredient(ItemID.Diamond, 4)
                .AddIngredient(ItemID.CrimtaneBar, 9)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class GlistentLegs : ModItem
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(0, 0, 60, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlistentBar>(8)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient<LeafeoBoots>()
                .AddIngredient(ItemID.Diamond, 2)
                .AddIngredient(ItemID.DemoniteBar, 7)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<LeafeoBoots>()
                .AddIngredient(ItemID.Diamond, 2)
                .AddIngredient(ItemID.CrimtaneBar, 7)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }

    public class LeechSeedCD : ModBuff
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
    }

    public class LeechSeed : ModProjectile, IAttachableProjectile
    {
        public override string Texture => AssetDirectory.GlistentItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float TargetIndex => ref Projectile.ai[1];
        public ref float HitCount => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        private bool init = true;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = FairyDamage.Instance;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            if (State == 0)
                return base.CanDamage();

            return false;
        }

        public override void AI()
        {
            if (init)
            {
                TargetIndex = -1;
                init = false;
            }

            switch (State)
            {
                default:
                case 0://自由飞翔
                    Timer++;
                    if (Timer > 30)
                    {
                        if (Projectile.velocity.Y < 10)
                            Projectile.velocity.Y += 0.2f;
                    }

                    if (Helper.TryGetFairyCircle(Main.player[Projectile.owner], out FairyCatcherProj catcherProj))
                    {
                        Rectangle rect = Projectile.getRect();
                        foreach (var fairyRect in catcherProj.GetFairyCollides())
                            if (rect.Intersects(fairyRect.Item1))
                            {
                                fairyRect.Item2.AddBuff<LeechSeedFairyBuff>(60 * 8);
                                Projectile.Kill();
                            }
                    }

                    Projectile.rotation += MathF.Sign(Projectile.velocity.X) * Projectile.velocity.Length() / 70;

                    break;
                case 1://和怪贴贴
                    if (!TargetIndex.GetNPCOwner(out NPC target, Projectile.Kill))
                        return;

                    break;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (TargetIndex == -1 && Helper.AttatchToTarget(Projectile, target))//贴上了再设置
            {
                TargetIndex = target.whoAmI;
                Projectile.tileCollide = false;
                Projectile.timeLeft = 60 * 15;
                Projectile.velocity = Vector2.Zero;
                State = 1;
            }
        }


        public void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (!projectile.DamageType.CountsAsClass<TrueFairyDamage>())
                return;

            HitCount++;

            Player p = Main.player[projectile.owner];

            //造成伤害
            npc.SimpleStrikeNPC(Projectile.damage, 0, damageType: FairyDamage.Instance, damageVariation: true);

            Vector2 dir = (p.Center - Projectile.Center).SafeNormalize(Vector2.Zero);

            if (projectile.ModProjectile is BaseFairyProjectile bfp)
            {
                //给仙灵回血
                bfp.FairyItem.HealFairy(0.03f, 5);
                dir = (projectile.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            }

            //给玩家回血
            p.Heal(2);

            Color lightGreen = new Color(182, 255, 171);

            //生成特效
            for (int i = 0; i < 5; i++)
            {
                Vector2 dir2 = dir.RotateByRandom(-0.3f, 0.3f);
                FlowLineThin.Spawn(Projectile.Center + Helper.NextVec2Dir(4, 10)
                    , dir2 * Main.rand.NextFloat(2.5f, 5), 7, 20, Main.rand.NextFloat(-0.15f, 0.15f)
                    , Main.rand.NextFromList(Color.LimeGreen, lightGreen));
            }

            for (int i = 0; i < 8; i++)
            {
                Vector2 dir2 = dir.RotateByRandom(-0.3f, 0.3f);

                Dust d = Dust.NewDustPerfect(Projectile.Center + Helper.NextVec2Dir(4, 10), DustID.GemEmerald
                    , dir2 * Main.rand.NextFloat(1, 3f), Scale: Main.rand.NextFloat(0.8f, 1.2f));
                d.noGravity = true;
            }

            if (HitCount > 6)
                projectile.Kill();
        }


        public bool NewProjAttach(Projectile incomeProj)
        {
            return incomeProj.type != ProjectileType<LeechSeed>();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickDraw(lightColor, 0);
            return false;
        }
    }

    [AutoLoadTexture(Path = AssetDirectory.GlistentItems)]
    public class LeechSeedFairyBuff : FairyBuff
    {
        public static ATex LeechSeedFairyTex { get; set; }

        public override bool IsSame(FairyBuff other)
        {
            return other is LeechSeedFairyBuff;
        }

        public override void UpdateInCatcher(Fairy fairy)
        {
            if (TimeRemain % 30 == 0)
                fairy.AddCatchProgress(Main.LocalPlayer, 2);

            if (TimeRemain % 2 == 0)
            {
                Dust d = Dust.NewDustPerfect(fairy.Center + Helper.NextVec2Dir(4, 8), DustID.GemEmerald
                    , Helper.NextVec2Dir(0.5f, 1.5f), Scale: Main.rand.NextFloat(0.8f, 1.2f));
                d.noGravity = true;
            }
        }

        public override void PreDraw(Vector2 center, Vector2 size, ref Color drawColor, float alpha)
        {
            drawColor = Color.Lime;

            LeechSeedFairyTex.Value.QuickCenteredDraw(Main.spriteBatch, center + new Vector2(0, size.Y - 40)-Main.screenPosition
                , Color.White, MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.8f);
        }
    }
}
