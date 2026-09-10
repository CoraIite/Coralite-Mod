using Coralite.Content.Items.Materials;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Core.Systems.MagikeSystem.MagikeLevels;
using Coralite.Helpers;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.Pets
{
    public class SawBlade : ModItem,IMagikeCraftable
    {
        public override string Texture => AssetDirectory.PetItems + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<SawBladePet>(), ModContent.BuffType<SawBladeBuff>());
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(0, 5);
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 15, true, false);
            }
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe<SoulOfDeveloper, SawBlade>(MagikeHelper.CalculateMagikeCost<BrilliantLevel>())
                .Register();
        }
    }

    public class SawBladeBuff : ModBuff
    {
        public override string Texture => AssetDirectory.PetBuffs + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<SawBladePet>();

            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);
                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 20, 0f, player.whoAmI);
            }
        }
    }

    public class SawBladePet : BasePetProj
    {
        public override string Texture => AssetDirectory.PetItems + "SawBlade";
        protected override int PetBuffType => ModContent.BuffType<SawBladeBuff>();

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float DashTimer => ref Projectile.ai[2];

        /// <summary>
        /// 玩家静止计时器
        /// </summary>
        public float playerIdleTimer = 0;

        /// <summary>
        /// 残影缓存长度
        /// </summary>
        public const int trailCachesLength = 8;

        protected override void SetPetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, trailCachesLength);
        }

        protected override void SetPetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 100;
        }

        public override bool MinionContactDamage() => State == 2;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (owner.dead)
                owner.ClearBuff(ModContent.BuffType<SawBladeBuff>());

            if (owner.HasBuff<SawBladeBuff>())
                Projectile.timeLeft = 2;

            // 根据速度自转
            float speed = Projectile.velocity.Length();
            Projectile.rotation += speed * 0.05f;
            Lighting.AddLight(Projectile.Center, 0.08f, 0.08f, 0.08f);
            switch (State)
            {
                default:
                case 1: // 在玩家身边飞行
                    {
                        Projectile.hostile = false;
                        FlyMovement(owner);

                        // 检测玩家是否静止
                        if (owner.velocity.LengthSquared() < 0.01f)
                        {
                            playerIdleTimer++;
                        }
                        else
                        {
                            playerIdleTimer = 0;
                        }

                        // 检查玩家是否在暗处（光照强度）
                        Point tilePos = owner.Center.ToTileCoordinates();
                        float brightness = Lighting.Brightness(tilePos.X, tilePos.Y);

                        // 玩家静止3秒（180帧）且在暗处（亮度<0.3）时切换到冲刺状态
                        if (playerIdleTimer > 180 && brightness < 0.3f)
                        {
                            State = 2;
                            Timer = 0;
                            DashTimer = 0;
                            playerIdleTimer = 0;

                            // 计算冲刺方向
                            Vector2 toPlayer = owner.Center - Projectile.Center;
                            Projectile.velocity = toPlayer.SafeNormalize(Vector2.Zero) * 18f;
                        }

                        Timer++;
                    }
                    break;

                case 2: // 瞄准玩家冲刺
                    {
                        Projectile.hostile = true;
                        Projectile.damage = 1;
                        DashTimer++;

                        // 持续追踪玩家一小段时间
                        if (DashTimer < 40)
                        {
                            Vector2 toPlayer = owner.Center - Projectile.Center;
                            float targetRot = toPlayer.ToRotation();
                            float currentRot = Projectile.velocity.ToRotation();
                            float newRot = currentRot.AngleTowards(targetRot, 0.1f);
                            Projectile.velocity = newRot.ToRotationVector2() * 8f;
                        }

                        // 冲刺一段时间后切换回状态1
                        if (DashTimer > 60)
                        {
                            State = 1;
                            Timer = 0;
                            DashTimer = 0;
                            Projectile.damage = 0;
                        }
                    }
                    break;
            }
        }

        private void FlyMovement(Player player)
        {
            base.FlyMovement(player, 0.4f, 16f, 200f, 80f, 2.5f, 5f);
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.ModifyHurtInfo += Modifiers_ModifyHurtInfo;
        }

        private void Modifiers_ModifyHurtInfo(ref Player.HurtInfo info)
        {
            // 固定伤害20
            info.Damage = 20;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 使用 Helper 的封装方法绘制残影，始终绘制
            Color trailColor = lightColor;
            trailColor.A = 230;
            Projectile.DrawShadowTrails(trailColor, 0.8f, 0.8f / trailCachesLength, 0, trailCachesLength, 1);

            // 绘制本体
            Projectile.QuickDraw(lightColor, 0);

            return false;
        }
    }
}
