using Coralite.Core;
using Coralite.Helpers;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Donator
{
    public class ChocomintIce : ModItem
    {
        public override string Texture => AssetDirectory.DefaultItem;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(28, 4);
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.maxStack = 1;
            Item.mana = 25;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;

            Item.shoot = ProjectileType<ChocomintIceProj>();
            Item.UseSound = CoraliteSoundID.SummonStaff_Item44;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var projectile = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, Main.myPlayer);
                projectile.originalDamage = Item.damage;
            }

            return false;
        }
    }
    public class ChocomintIceBuff : ModBuff
    {
        public override string Texture => AssetDirectory.Buffs + "Buff";

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
        public override string Texture => AssetDirectory.DefaultItem;

        private Player Owner => Main.player[Projectile.owner];
        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];

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

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
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

                        Vector2 idleSpot = CircleMovement(32 + totalIndexesInGroup * 4, 36, accelFactor: 0.6f, angleFactor: 0.9f, baseRot: index * MathHelper.TwoPi / totalIndexesInGroup);
                        if (Projectile.Distance(idleSpot) < 32f)
                        {
                            Timer = 0f;
                            State = 0;
                            Projectile.netUpdate = true;
                        }

                        Projectile.rotation = Projectile.velocity.ToRotation();
                    }
                    break;
                case 0://待定并准备开始攻击
                    {
                        Helper.GetMyGroupIndexAndFillBlackList(Projectile, out var index2, out var totalIndexesInGroup2);
                        CircleMovement(32 + totalIndexesInGroup2 * 4, 28, accelFactor: 0.4f, angleFactor: 0.9f, baseRot: index2 * MathHelper.TwoPi / totalIndexesInGroup2);
                        Projectile.rotation = (Owner.Center - Projectile.Center).ToRotation();

                        if (Main.rand.NextBool(20))
                        {
                            int index = Projectile.MinionFindTarget();
                            if (index != -1)
                            {
                                Projectile.StartAttack();
                                Projectile.InitOldPosCache(10);
                                State = 1;
                                Timer = 0;
                                Target = index;
                                Projectile.netUpdate = true;
                                return;
                            }
                        }

                    }
                    break;
                case 1://开始攻击
                    {

                    }
                    break;
            }
        }

        //public void vanilla()
        //{
        //    for (int num464 = 0; num464 < 1000; num464++)
        //    {
        //        if (num464 != whoAmI && Main.projectile[num464].active && Main.projectile[num464].owner == owner && Main.projectile[num464].type == type && Math.Abs(base.position.X - Main.projectile[num464].position.X) + Math.Abs(base.position.Y - Main.projectile[num464].position.Y) < (float)width)
        //        {
        //            if (base.position.X < Main.projectile[num464].position.X)
        //                velocity.X -= 0.05f;
        //            else
        //                velocity.X += 0.05f;

        //            if (base.position.Y < Main.projectile[num464].position.Y)
        //                velocity.Y -= 0.05f;
        //            else
        //                velocity.Y += 0.05f;
        //        }
        //    }

        //    float num465 = base.position.X;
        //    float num466 = base.position.Y;
        //    float num467 = 900f;
        //    bool flag23 = false;
        //    int num468 = 500;
        //    if (this.ai[1] != 0f || friendly)
        //        num468 = 1400;

        //    if (Math.Abs(base.Center.X - Main.player[owner].Center.X) + Math.Abs(base.Center.Y - Main.player[owner].Center.Y) > (float)num468)
        //        this.ai[0] = 1f;

        //    if (this.ai[0] == 0f)
        //    {
        //        tileCollide = true;
        //        NPC ownerMinionAttackTargetNPC2 = OwnerMinionAttackTargetNPC;
        //        if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(this))
        //        {
        //            float num469 = ownerMinionAttackTargetNPC2.position.X + (float)(ownerMinionAttackTargetNPC2.width / 2);
        //            float num470 = ownerMinionAttackTargetNPC2.position.Y + (float)(ownerMinionAttackTargetNPC2.height / 2);
        //            float num471 = Math.Abs(base.position.X + (float)(width / 2) - num469) + Math.Abs(base.position.Y + (float)(height / 2) - num470);
        //            if (num471 < num467 && Collision.CanHit(base.position, width, height, ownerMinionAttackTargetNPC2.position, ownerMinionAttackTargetNPC2.width, ownerMinionAttackTargetNPC2.height))
        //            {
        //                num467 = num471;
        //                num465 = num469;
        //                num466 = num470;
        //                flag23 = true;
        //            }
        //        }

        //        if (!flag23)
        //        {
        //            for (int num472 = 0; num472 < 200; num472++)
        //            {
        //                if (Main.npc[num472].CanBeChasedBy(this))
        //                {
        //                    float num473 = Main.npc[num472].position.X + (float)(Main.npc[num472].width / 2);
        //                    float num474 = Main.npc[num472].position.Y + (float)(Main.npc[num472].height / 2);
        //                    float num475 = Math.Abs(base.position.X + (float)(width / 2) - num473) + Math.Abs(base.position.Y + (float)(height / 2) - num474);
        //                    if (num475 < num467 && Collision.CanHit(base.position, width, height, Main.npc[num472].position, Main.npc[num472].width, Main.npc[num472].height))
        //                    {
        //                        num467 = num475;
        //                        num465 = num473;
        //                        num466 = num474;
        //                        flag23 = true;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        tileCollide = false;
        //    }

        //    if (!flag23)
        //    {
        //        friendly = true;
        //        float num476 = 8f;
        //        if (this.ai[0] == 1f)
        //            num476 = 12f;

        //        Vector2 vector38 = new Vector2(base.position.X + (float)width * 0.5f, base.position.Y + (float)height * 0.5f);
        //        float num477 = Main.player[owner].Center.X - vector38.X;
        //        float num478 = Main.player[owner].Center.Y - vector38.Y - 60f;
        //        float num479 = (float)Math.Sqrt(num477 * num477 + num478 * num478);
        //        float num480 = num479;
        //        if (num479 < 100f && this.ai[0] == 1f && !Collision.SolidCollision(base.position, width, height))
        //            this.ai[0] = 0f;

        //        if (num479 > 2000f)
        //        {
        //            base.position.X = Main.player[owner].Center.X - (float)(width / 2);
        //            base.position.Y = Main.player[owner].Center.Y - (float)(width / 2);
        //        }

        //        if (type == 317 && num479 > 100f)
        //        {
        //            num476 = 12f;
        //            if (this.ai[0] == 1f)
        //                num476 = 15f;
        //        }

        //        if (num479 > 70f)
        //        {
        //            num479 = num476 / num479;
        //            num477 *= num479;
        //            num478 *= num479;
        //            velocity.X = (velocity.X * 20f + num477) / 21f;
        //            velocity.Y = (velocity.Y * 20f + num478) / 21f;
        //        }
        //        else
        //        {
        //            if (velocity.X == 0f && velocity.Y == 0f)
        //            {
        //                velocity.X = -0.15f;
        //                velocity.Y = -0.05f;
        //            }

        //            velocity *= 1.01f;
        //        }

        //        friendly = false;
        //        return;
        //    }

        //    if (this.ai[1] == -1f)
        //        this.ai[1] = 17f;

        //    if (this.ai[1] > 0f)
        //        this.ai[1] -= 1f;

        //    if (this.ai[1] == 0f)
        //    {
        //        friendly = true;
        //        float num481 = 16f;
        //        Vector2 vector39 = new Vector2(base.position.X + (float)width * 0.5f, base.position.Y + (float)height * 0.5f);
        //        float num482 = num465 - vector39.X;
        //        float num483 = num466 - vector39.Y;
        //        float num484 = (float)Math.Sqrt(num482 * num482 + num483 * num483);
        //        float num485 = num484;
        //        if (num484 < 100f)
        //            num481 = 10f;

        //        num484 = num481 / num484;
        //        num482 *= num484;
        //        num483 *= num484;
        //        velocity.X = (velocity.X * 14f + num482) / 15f;
        //        velocity.Y = (velocity.Y * 14f + num483) / 15f;
        //    }
        //    else
        //    {
        //        friendly = false;
        //        if (Math.Abs(velocity.X) + Math.Abs(velocity.Y) < 10f)
        //            velocity *= 1.05f;
        //    }

        //}

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

            Projectile.QuickDraw(lightColor, 0);

            return false;
        }
    }
}
