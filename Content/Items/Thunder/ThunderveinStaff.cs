using Coralite.Content.Bosses.ThunderveinDragon;
using Coralite.Content.Items.Nightmare;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.Thunder
{
    public class ThunderveinStaff:ModItem
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.useTime = 24;
            Item.useAnimation = 24;
            Item.knockBack = 4f;
            Item.maxStack = 1;
            Item.mana = 25;

            Item.useTurn = false;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<ThunderMinion>();
            Item.UseSound = CoraliteSoundID.SummonStaff_Item44;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                var projectile = Projectile.NewProjectileDirect(source, Main.MouseWorld, velocity, type, damage, knockback, Main.myPlayer);
                projectile.originalDamage = damage;
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<ZapCrystal>(2)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }

    public class ThunderveinStaffBuff:ModBuff
    {
        public override string Texture => AssetDirectory.ThunderItems + Name;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ProjectileType<ThunderMinion>()] > 0)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }

        public override bool RightClick(int buffIndex)
        {
            for (int i = 0; i < 1000; i++)
                if (Main.projectile[i].active && Main.projectile[i].type == ProjectileType<ThunderMinion>() && Main.projectile[i].owner == Main.myPlayer)
                    Main.projectile[i].Kill();

            return true;
        }

    }

    public class ThunderMinion:BaseThunderProj
    {
        public override string Texture => AssetDirectory.ThunderItems + "ThunderProj";

        public ThunderTrail[] trails;

        public ref float State => ref Projectile.ai[0];
        public ref float Target => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public ref float Recorder => ref Projectile.localAI[0];

        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.timeLeft = 300;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.localNPCHitCooldown = 20;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            //发现敌人时先直接朝敌人冲刺，之后如果能够攻击到敌人那么就直接瞬移到目标头顶再向下戳
            Player player = Main.player[Projectile.owner];
            if (!CheckActive(Owner))
                return;

            Owner.AddBuff(BuffType<ThunderveinStaffBuff>(), 2);


            switch (State)
            {
                default:
                case -1://回到玩家头顶
                    {
                        if (Timer == -1f)
                        {
                            AI_GetMyGroupIndexAndFillBlackList(Projectile, out var index, out var totalIndexesInGroup);

                            Vector2 idleSpot = CircleMovement(48 + totalIndexesInGroup * 4, 28, accelFactor: 0.4f, angleFactor: 0.2f, baseRot: index * MathHelper.TwoPi / totalIndexesInGroup);
                            if (Projectile.Distance(idleSpot) < 2f)
                            {
                                Timer = 0f;
                                Projectile.netUpdate = true;
                            }

                            return;
                        }
                    }
                    break;
                case 0://在玩家头顶盘旋
                    {

                    }
                    break;
                case 1://发现目标并初次展开攻击，向目标戳刺
                    {

                    }
                    break;
                case 2://在目标头顶向下戳刺
                    {

                    }
                    break;
            }
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(BuffType<ThunderveinStaffBuff>());
                return false;
            }

            if (owner.HasBuff(BuffType<ThunderveinStaffBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }

        public float ThunderWidthFunc2(float factor)
        {
            return MathF.Sin(factor * MathHelper.Pi) * ThunderWidth * 1.2f;
        }

        public Color ThunderColorFunc_Fade(float factor)
        {
            return ThunderveinDragon.ThunderveinYellowAlpha * ThunderAlpha * (1 - factor);
        }

        public void InitCaches()
        {
            if (trails == null)
            {
                trails = new ThunderTrail[3];
                Asset<Texture2D> thunderTex = Request<Texture2D>(AssetDirectory.OtherProjectiles + "LightingBody");

                for (int i = 0; i < trails.Length; i++)
                {
                    trails[i] = new ThunderTrail(thunderTex, ThunderWidthFunc2, ThunderColorFunc_Fade);
                    trails[i].SetRange((0, 6));
                    trails[i].SetExpandWidth(4);
                }

                float cacheLength = Projectile.velocity.Length() / 2;
                foreach (var trail in trails)
                {
                    if (cacheLength < 3)
                        trail.CanDraw = false;
                    else
                    {
                        Vector2[] vec = new Vector2[(int)cacheLength];
                        Vector2 basePos = Projectile.Center + Helper.NextVec2Dir() * 5;
                        Vector2 dir = -Projectile.velocity;
                        vec[0] = basePos;

                        for (int i = 1; i < (int)cacheLength; i++)
                        {
                            vec[i] = basePos + dir * i;
                        }

                        trail.BasePositions = vec;
                        trail.RandomThunder();
                    }
                }
            }
        }

        /// <summary>
        /// 获取自身是第几个召唤物弹幕
        /// 非常好的东西，建议稍微改改变成静态帮助方法
        /// </summary>
        /// <param name="Projectile"></param>
        /// <param name="index"></param>
        /// <param name="totalIndexesInGroup"></param>
        public void AI_GetMyGroupIndexAndFillBlackList(Projectile Projectile, out int index, out int totalIndexesInGroup)
        {
            index = 0;
            totalIndexesInGroup = 0;
            for (int i = 0; i < 1000; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active && projectile.owner == Projectile.owner && projectile.type == Projectile.type && (projectile.type != 759 || projectile.frame == Main.projFrames[projectile.type] - 1))
                {
                    if (Projectile.whoAmI > i)
                        index++;

                    totalIndexesInGroup++;
                }
            }
        }

        public Vector2 CircleMovement(float distance, float speedMax, float accelFactor = 0.25f, float rollingFactor = 5f, float angleFactor = 0.08f, float baseRot = 0f)
        {
            Vector2 offset = (baseRot + Main.GlobalTimeWrappedHourly / rollingFactor * MathHelper.TwoPi).ToRotationVector2() * distance;
            offset.Y /= 3;
            Vector2 center = Owner.Center+new Vector2(0,-48) + offset;
            Vector2 dir = center - Projectile.Center;

            float velRot = Projectile.velocity.ToRotation();
            float targetRot = dir.ToRotation();

            float speed = Projectile.velocity.Length();
            float aimSpeed = Math.Clamp(dir.Length() / 200f, 0, 1) * speedMax;

            Projectile.velocity = velRot.AngleTowards(targetRot, angleFactor).ToRotationVector2() * Helper.Lerp(speed, aimSpeed, accelFactor);
            return center;
        }

    }
}
