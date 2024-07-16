using Coralite.Content.Items.GlobalItems;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.ParticleSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Donator
{
    public class ChocomintIce : ModItem
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type]  = true;
        }

        public override void SetDefaults()
        {
            Item.SetWeaponValues(26, 4);
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.maxStack = 1;
            Item.mana = 25;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = true;

            Item.shoot = ProjectileType<ChocomintIceProj>();
            Item.UseSound = CoraliteSoundID.SummonStaff_Item44;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = RarityType<ChocomintIceRarity>();
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.HoldUp;
            CoraliteGlobalItem.SetColdDamage(Item);
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                player.MinionNPCTargetAim(true);
                return false;
            }

            var projectile = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, Main.myPlayer, ai2: Main.rand.Next(3));
            projectile.originalDamage = Item.damage;

            return false;
        }
    }

    public class ChocomintIceRarity : ModRarity
    {
        public override Color RarityColor => Color.Lerp(new Color(141, 255, 202), new Color(77, 180, 149)
            , MathF.Sin(Main.GlobalTimeWrappedHourly) / 2 + 0.5f);
    }

    public class ChocomintIceBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Donator + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<ChocomintIceProj>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileType<ChocomintIceProj>() && Main.projectile[i].owner == Main.myPlayer)
                    Main.projectile[i].Kill();

            return true;
        }
    }

    public class ChocomintIceProj : ModProjectile
    {
        public override string Texture => AssetDirectory.Donator + Name;

        private Player Owner => Main.player[Projectile.owner];
        public ref float State => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.localAI[0];
        public ref float TextureType => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 7);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.minionSlots = 1;
            Projectile.localNPCHitCooldown = 10;

            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (!Projectile.CheckMinionOwnerActive<ChocomintIceBuff>())
                return;

            Owner.AddBuff(BuffType<ChocomintIceBuff>(), 2);

            switch (State)
            {
                default:
                case -1://返回玩家身边
                    {
                        Helper.GetMyGroupIndexAndFillBlackList(Projectile, out var index, out var totalIndexesInGroup);
                        CircleMovement(index, totalIndexesInGroup, out Vector2 idleSpot);
                        Projectile.Center = Projectile.Center.MoveTowards(idleSpot, 32f);
                        Projectile.spriteDirection = Projectile.direction;

                        if (Vector2.Distance(Projectile.Center,Owner.Center)>2000)
                            Projectile.Center = Owner.Center;

                        if (Projectile.Distance(idleSpot) < 32f)
                        {
                            Timer = 0f;
                            State = 0;
                            Projectile.netUpdate = true;
                        }

                        Projectile.rotation += 0.2f;
                    }
                    break;
                case 0://待定并准备开始攻击
                    {
                        Helper.GetMyGroupIndexAndFillBlackList(Projectile, out var index2, out var totalIndexesInGroup2);
                        CircleMovement(index2, totalIndexesInGroup2, out Vector2 idleSpot);
                        Projectile.Center = Projectile.Center.MoveTowards(idleSpot, 32f);

                        if (Vector2.Distance(Projectile.Center, Owner.Center) > 2000)
                            Projectile.Center = Owner.Center;

                        Projectile.spriteDirection = Projectile.direction;
                        Projectile.rotation = Projectile.rotation.AngleLerp(MathF.Sin(Main.GlobalTimeWrappedHourly + Projectile.whoAmI) * 0.4f, 0.5f);

                        if (Main.rand.NextBool(20))
                        {
                            int index;
                            if (Owner.HasMinionAttackTargetNPC)
                                index = Owner.MinionAttackTargetNPC;
                            else
                                index = Projectile.MinionFindTarget();

                            if (index != -1)
                            {
                                Projectile.StartAttack();
                                Projectile.InitOldPosCache(7);
                                State = 1;
                                Timer = 0;
                                Target = index;
                                Projectile.friendly = true;
                                Projectile.tileCollide = true;
                                Projectile.netUpdate = true;
                                return;
                            }
                        }
                    }
                    break;
                case 1://开始攻击
                    {
                        if (!Target.GetNPCOwner(out NPC target))
                        {
                            Timer = 0;
                            State = -1;
                            Projectile.friendly = false;
                            Projectile.tileCollide = false;

                            break;
                        }

                        Timer++;
                        if (Timer < 20)
                        {
                            if (Projectile.friendly)
                            {
                                float num481 = 16f;
                                Vector2 center = Projectile.Center;
                                Vector2 targetCenter = target.Center;
                                Vector2 dir = targetCenter - center;
                                float num484 = dir.Length();
                                if (num484 < 100f)
                                    num481 = 10f;

                                num484 = num481 / num484;
                                dir *= num484;
                                Projectile.velocity.X = (Projectile.velocity.X * 14f + dir.X) / 15f;
                                Projectile.velocity.Y = (Projectile.velocity.Y * 14f + dir.Y) / 15f;
                            }
                            else
                            {
                                Projectile.velocity = Projectile.velocity.RotateByRandom(0.01f, 0.1f);
                                if (Projectile.velocity.Length() < 1)
                                {
                                    Projectile.velocity += Vector2.UnitX;
                                }
                            }

                            Projectile.rotation += Projectile.velocity.Length() / 18;
                        }
                        else
                        {
                            int index;
                            if (Owner.HasMinionAttackTargetNPC)
                                index = Owner.MinionAttackTargetNPC;
                            else
                                index = Projectile.MinionFindTarget();

                            Projectile.friendly = true;
                            Timer = 0;
                            if (index == -1 || Vector2.Distance(Projectile.Center, Owner.Center) > 2400)
                            {
                                State = -1;
                                Projectile.friendly = false;
                                Projectile.tileCollide = false;
                            }
                        }
                    }
                    break;
            }
        }

        public void CircleMovement(int stackedIndex, int totalIndexes, out Vector2 idleSpot)
        {
            float num2 = (totalIndexes - 1f) / 2f;
            idleSpot = Owner.Center + -Vector2.UnitY.RotatedBy(4.3982296f / totalIndexes * (stackedIndex - num2)) * 50f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Helper.SpawnDirDustJet(target.Center, () => Helper.NextVec2Dir(), 2, 8, i => i * 0.8f, DustID.IceTorch);
            Particle.NewParticle<StarRot>(target.Center, Helper.NextVec2Dir(1, 4), Main.rand.NextBool() ? Color.White : Color.Cyan
                , Main.rand.NextFloat(0.4f, 0.6f));
            target.AddBuff(BuffID.Frostburn, 60 * 5);
            Projectile.friendly = false;
            Timer /= 2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = oldVelocity.X * -0.6f;

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = oldVelocity.Y * -0.6f;

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(1, 3, 0, (int)TextureType);
            if (State != 0)
                Projectile.DrawShadowTrails(new Color(141, 255, 202), 0.4f, 0.4f / 5, 1, 5, 1, Projectile.scale, frameBox, 0);
            Projectile.QuickDraw(frameBox, lightColor, 0);

            return false;
        }
    }
}
