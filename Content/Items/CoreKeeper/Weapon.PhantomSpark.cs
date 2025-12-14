using Coralite.Content.Dusts;
using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.CoreKeeper
{
    /*
     * 攻击方式
     * 核心特色：通过射击积攒灵魂，并且召唤出灵魂协同攻击
     * 
     * 【普通形态】
     * 左键右键都是平平无奇射击，射速比较慢
     *  冲刺则会进入普通强化状态
     * 
     * 【普通强化状态】
     * 左键射出扩散箭，并且箭矢会产生爆炸
     * 右键射出追踪箭（也会爆炸）与几颗小水晶，小水晶命中后会弹回并再次追踪（有次数限制）
     */
    public class PhantomSpark : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public bool powerful;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(216, 5, 12);
            Item.useTime = 26;
            Item.useAnimation = 26;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = RarityType<LegendaryRarity>();
            Item.shoot = ProjectileType<PhantomSparkNormalArrow>();
            Item.shootSpeed = 12f;

            Item.noUseGraphic = false;
            Item.noMelee = true;
            Item.autoReuse = true;
            //Item.expert = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine tooltip = tooltips.FirstOrDefault(t => t.Mod == "Terraria" && t.Name == "Damage", null);
            if (tooltip != null)
            {
                bool addLeadingSpace = Item.DamageType is not VanillaDamageClass;
                string tip = (addLeadingSpace ? " " : "") + Item.DamageType.DisplayName;

                tooltip.Text = string.Concat(((int)(Item.damage * 0.903f)).ToString()
                    , "-", ((int)(Item.damage * 1.098f)).ToString(), tip);
            }
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                cp.AddDash(this);
        }

        public bool Dash(Player Player, int DashDir)
        {
            float dashDirection;
            Vector2 startPos;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        startPos = DashDir == CoralitePlayer.DashRight ? Player.TopRight : Player.TopLeft;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 85;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.AddImmuneTime(ImmunityCooldownID.General, 15);
            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 15));

            //开始闪避的特效
            LineParticles(Player);

            //挪动玩家位置并在中间生成一点粒子
            int height = Player.height / 16;
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (Framing.GetTileSafely(startPos + new Vector2(dashDirection*16, j * 16)).HasReallySolidTile())
                        goto moveOver;
                }

                Vector2 dir2 = new Vector2(dashDirection * 16, 0);
                Player.Center += dir2;
                startPos += dir2;

                for (int k = 0; k < 5; k++)
                {
                    Dust d = Dust.NewDustPerfect(Player.Center + new Vector2(dashDirection * k * 16 / 5f, 0) + Main.rand.NextVector2Circular(4, 4)
                        , DustID.AncientLight, Helper.NextVec2Dir(0.5f, 1f), Scale: Main.rand.NextFloat(1, 1.5f));
                    d.noGravity = true;
                }
            }

        moveOver:
            LineParticles(Player);

            return true;

            static void LineParticles(Player Player)
            {
                var p = PRTLoader.NewParticle<CircleExplodeParticle>(Player.Center
                     , Vector2.Zero, (Color.LightCyan * 0.8f) with { A = 150 }, 0.01f);

                p.addTime = 6;
                p.scaleAdd = 0.02f;
                p.scaleAddSlow = 0.002f;
                p.colorFade = 0.8f;

                for (int i = -3; i <= 3; i++)
                {
                    PRTLoader.NewParticle<SpeedLine>(Player.Center + new Vector2(0, i * 7)
                        , new Vector2(0, i * 0.5f - 0.2f)
                        , Color.LightCyan, 0.3f - MathF.Abs(i) * 0.04f);
                }
                for (int i = 0; i < 12; i++)
                {
                    PRTLoader.NewParticle<SpeedLine>(Player.Center + Main.rand.NextVector2CircularEdge(Player.width / 2, Player.width / 2)
                        , new Vector2(0, Main.rand.NextFloat(-2, 2)), Main.rand.NextFromList(Color.Cyan, Color.DarkCyan), Main.rand.NextFloat(0.1f, 0.3f));
                }
                for (int i = 0; i < 6; i++)
                {
                    Dust d = Dust.NewDustPerfect(Player.Center + Main.rand.NextVector2Circular(Player.width / 2 + 8, Player.height / 2 + 8)
                        , DustType<PixelPoint>(), new Vector2(0, Main.rand.NextFloat(-2, 2)), newColor: Main.rand.NextFromList(Color.Cyan, Color.LightCyan), Scale: Main.rand.NextFloat(1, 2f));
                    d.noGravity = true;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<GlyphParchment>()
                .AddIngredient<ChannelingGemstone>()
                .AddIngredient<FracturedLimbs>()
                .AddIngredient<EnergyString>()
                .AddIngredient(ItemID.HallowedBar, 100)
                .AddIngredient<AncientGemstone>(20)
                .AddCondition(CoraliteConditions.UseRuneParchment)
                .Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ProjectileType<PhantomSparkNormalArrow>(), damage
                , knockback, player.whoAmI);
            return false;
        }
    }

    /// <summary>
    /// ai0输入1表示追踪，ai1输入1表示会发生爆炸
    /// </summary>
    public class PhantomSparkNormalArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public bool Chase => Projectile.ai[0] == 1;
        public bool Explode => Projectile.ai[1] == 1;
        public ref float Target => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];

        public bool init = true;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (init)
            {
                init = false;
                Target = -1;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.3f, 0.5f));

            UpdateFrame();

            if (Chase)
            {
                if (Target.GetNPCOwner(out NPC target, () => Target = -1))
                    return;

                Timer++;
                if (Timer > 40)
                {
                    if (Helper.TryFindClosestEnemy(Projectile.Center, 1000
                        , n => n.CanBeChasedBy() && Collision.CanHit(Projectile, n), out NPC t))
                        Target = t.whoAmI;
                    Timer = 0;
                }
            }
        }

        private void UpdateFrame()
        {
            if (++Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 3)
                    Projectile.frame = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.QuickFrameDraw(new Rectangle(Projectile.frame, 0, 4, 1), Color.White, 0);

            return false;
        }
    }

    public class PhantomSparkExplode
    {

    }

    public class PhantomSparkPhantomNPC : ModProjectile
    {
        public override string Texture => AssetDirectory.CoreKeeperItems + Name;

        public ref float State => ref Projectile.ai[0];
        public ref float Timer => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];

        private int frameY;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 48;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            switch (State)
            {
                case 0://Idle状态，跟随玩家
                    {
                        if (Projectile.frame > 5)
                            Projectile.frame = 0;
                        Projectile.UpdateFrameNormally(4, 5);

                        if (Projectile.velocity.Length() < 1)
                            frameY = 0;
                        else
                            frameY = 2;

                        FlyMovement(owner);
                    }
                    break;
                case 1://射法球
                    {

                    }
                    break;
                case 2://给玩家回血
                    {

                    }
                    break;
            }
        }

        private void FlyMovement(Player player)
        {
            Projectile.tileCollide = false;
            float acc = 0.12f;//加速度
            float num18 = 10f;
            int num19 = 200;
            if (num18 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
                num18 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);

            Vector2 toPlayer = player.Center - Projectile.Center;
            float lengthToPlayer = toPlayer.Length();
            if (lengthToPlayer > 500 && Projectile.IsOwnedByLocalPlayer())
            {
                Vector2 center = Projectile.Center;
                Projectile.Center = player.Center + Main.rand.NextVector2CircularEdge(48, 48);
                Projectile.netUpdate = true;

                //传送特效

            }

            if (lengthToPlayer < num19 && player.velocity.Y == 0f && Projectile.position.Y + Projectile.height <= player.position.Y + player.height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.netUpdate = true;
                if (Projectile.velocity.Y < -6f)
                    Projectile.velocity.Y = -6f;
            }

            if (!(lengthToPlayer < 60f))
            {
                toPlayer.SafeNormalize(Vector2.Zero);
                toPlayer *= num18;
                if (Projectile.velocity.X < toPlayer.X)
                {
                    Projectile.velocity.X += acc;
                    if (Projectile.velocity.X < 0f)
                        Projectile.velocity.X += acc * 2.2f;
                }

                if (Projectile.velocity.X > toPlayer.X)
                {
                    Projectile.velocity.X -= acc;
                    if (Projectile.velocity.X > 0f)
                        Projectile.velocity.X -= acc * 2.2f;
                }

                if (Projectile.velocity.Y < toPlayer.Y)
                {
                    Projectile.velocity.Y += acc;
                    if (Projectile.velocity.Y < 0f)
                        Projectile.velocity.Y += acc * 4.2f;
                }

                if (Projectile.velocity.Y > toPlayer.Y)
                {
                    Projectile.velocity.Y -= acc;
                    if (Projectile.velocity.Y > 0f)
                        Projectile.velocity.Y -= acc * 4.2f;
                }
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle frame = new(Projectile.frame, frameY, 8, 3);
            SpriteEffects effect = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Projectile.QuickFrameDraw(frame, Color.White * 0.75f, 0, effect);
            Projectile.QuickFrameDraw(frame, Color.White * 0.75f, 0, effect);

            return false;
        }
    }

    public class PhantomSparkPhantomMagicBall
    {

    }
}
