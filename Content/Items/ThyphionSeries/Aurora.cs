using Coralite.Content.ModPlayers;
using Coralite.Core;
using Coralite.Core.Prefabs.Projectiles;
using Coralite.Core.Systems.CameraSystem;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Coralite.Content.Items.ThyphionSeries
{
    public class Aurora : ModItem, IDashable
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public float Priority => IDashable.HeldItemDash;

        public override void SetDefaults()
        {
            Item.SetWeaponValues(120, 6f);
            Item.DefaultToRangedWeapon(10, AmmoID.Arrow, 24, 14f);

            Item.rare = ItemRarityID.Red;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.value = Item.sellPrice(0, 10);

            Item.noUseGraphic = true;

            Item.UseSound = CoraliteSoundID.Bow_Item5;
        }

        public override void HoldItem(Player player)
        {
            if (player.TryGetModPlayer(out CoralitePlayer cp))
            {
                cp.AddDash(this);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.One);
            float rot = dir.ToRotation();
            Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<AuroraHeldProj>(), damage, knockback, player.whoAmI, rot, 0);

            if (player.ownedProjectileCounts[ProjectileType<AuroraHeldProj>()] == 0)
            {
                float r = Main.rand.NextFromList(-0.6f, 0.6f);
                Projectile.NewProjectile(source, player.Center 
                    , velocity, ProjectileType<AuroraArrow>(), damage, knockback, player.whoAmI);
                for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(source, player.Center + velocity.SafeNormalize(Vector2.Zero).RotateByRandom(r - 0.2f, r + 0.2f) * 16, velocity, type, damage, knockback, player.whoAmI);

            }

            return false;
        }

        public bool Dash(Player Player, int DashDir)
        {
            Vector2 newVelocity = Player.velocity;
            float dashDirection;
            switch (DashDir)
            {
                case CoralitePlayer.DashLeft:
                case CoralitePlayer.DashRight:
                    {
                        dashDirection = DashDir == CoralitePlayer.DashRight ? 1 : -1;
                        newVelocity.X = dashDirection * 12;
                        break;
                    }
                default:
                    return false;
            }

            Player.GetModPlayer<CoralitePlayer>().DashDelay = 100;
            Player.GetModPlayer<CoralitePlayer>().DashTimer = 20;
            Player.immune = true;
            Player.AddImmuneTime(ImmunityCooldownID.General, 20);

            Player.velocity = newVelocity;
            Player.direction = (int)dashDirection;

            Main.instance.CameraModifiers.Add(new MoveModifyer(5, 15));

            if (Player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(CoraliteSoundID.Swing_Item1, Player.Center);

                foreach (var proj in from proj in Main.projectile
                                     where proj.active && proj.friendly && proj.owner == Player.whoAmI && proj.type == ProjectileType<AuroraHeldProj>()
                                     select proj)
                {
                    proj.Kill();
                    break;
                }

                //生成手持弹幕
                Projectile.NewProjectile(Player.GetSource_ItemUse(Player.HeldItem), Player.Center, Vector2.Zero, ProjectileType<AuroraHeldProj>(),
                    Player.HeldItem.damage, Player.HeldItem.knockBack, Player.whoAmI, 1.57f - dashDirection * 0.3f, 1, 20);
            }

            return true;
        }
    }

    public class AuroraHeldProj : BaseDashBow
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + nameof(Aurora);

        public ref float ArrowLength => ref Projectile.localAI[0];
        public ref float Timer => ref Projectile.localAI[1];
        public ref float RecordAngle => ref Projectile.localAI[2];

        public float handOffset = 14;

        public override int GetItemType()
            => ItemType<Aurora>();

        public override Vector2 GetOffset()
            => new(0 + handOffset, 0);

        public override void Initialize()
        {
            RecordAngle = Rotation;
        }

        #region 冲刺部分

        public override void DashAttackAI()
        {
            if (Timer < DashTime + 2)
            {

            }
            else
            {

            }

            Timer++;
        }

        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            Vector2 center = Projectile.Center - Main.screenPosition;
            var effect = DirSign > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            var origin = mainTex.Size() / 2;

            Main.spriteBatch.Draw(mainTex, center, null, lightColor, Projectile.rotation, origin, 1, effect, 0f);

            return false;
        }
    }

    public class AuroraArrow : ModProjectile
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + Name;

        public bool SpAttack => Projectile.ai[0] == 1;
        public ref float Timer => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];
        public ref float Random => ref Projectile.localAI[0];

        private bool init = true;

        public override void SetStaticDefaults()
        {
            Projectile.QuickTrailSets(Helper.TrailingMode.RecordAll, 14);
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 3;

            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            if (init)
                Initialize();

            if (Timer > 0 && Timer % (8 * Projectile.MaxUpdates) == 0)
            {
                NPC n = Helper.FindClosestEnemy(Projectile.Center, 400, n => n.CanBeChasedBy());
                if (n != null)
                    Target = n.whoAmI;
            }

            if (Target.GetNPCOwner(out NPC npc))
            {
                float num481 = 20f;
                Vector2 center = Projectile.Center;
                Vector2 targetCenter = npc.Center;
                Vector2 dir = targetCenter - center;
                float length = dir.Length();
                if (length < 100f)
                    num481 = 14f;

                length = num481 / length;
                dir *= length;
                Projectile.velocity.X = ((Projectile.velocity.X * 64f) + dir.X) / 65f;
                Projectile.velocity.Y = ((Projectile.velocity.Y * 64f) + dir.Y) / 65f;
            }
            else
                Target = -1;

            if (Timer>0)
            {
                var p = PRTLoader.NewParticle<AuroraParticle>(Projectile.Center, new Vector2(0, -Random * Main.rand.NextFloat(1, 3)), Scale: Main.rand.NextFloat(0.8f, 1.3f));
                p.MaxTime = 10;
                p.Rotation = Projectile.rotation + (Projectile.velocity.X > 0 ? 0 : MathHelper.Pi);
            }

            Random += Main.rand.NextFloat(-0.1f, 0.1f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Timer++;
        }

        public void Initialize()
        {
            init = false;
            Target = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (SpAttack)
            {
                //生成标记弹幕
            }
            else
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];

                    if (p.active && p.friendly && p.owner == Projectile.owner && p.type == ProjectileType<AuroraArrowTag>())
                    {
                        if (p.ai[0] < 15)
                            p.ai[0]++;
                        break;
                    }
                }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawShadowTrails(lightColor, 0.5f, 0.5f / 14, 0, 14, 1, 1.57f,-1);

            Projectile.QuickDraw(lightColor, 1.57f);

            return false;
        }

        public Color GetColor(float factor)
        {
            return Color.Lerp(new Color(181, 255, 149), new Color(202, 80, 129), factor);
        }
    }

    /// <summary>
    /// 极光的标记弹幕，极光箭命中标记弹幕标记的NPC后会额外造成一次伤害并积累标记，标记一定时间后根据层数爆炸
    /// </summary>
    public class AuroraArrowTag : ModProjectile
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "AuroraTrail";

        public ref float HitCount => ref Projectile.ai[0];

        public override void SetDefaults()
        {
        }

        public override bool? CanDamage()
        {
            return base.CanDamage();
        }

        public override void AI()
        {
            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }

    public class AuroraParticle:Particle
    {
        public override string Texture => AssetDirectory.ThyphionSeriesItems + "AuroraTrail";

        public float alpha;
        public Vector2 scale=new Vector2(0,0.2f);
        public int MaxTime = 20;
        public float endX = Main.rand.NextFloat(0.125f, 0.35f);

        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AlphaBlend;
        }

        public override void AI()
        {
            Opacity++;

            Color = GetColor(Opacity / MaxTime);
            alpha = 1 - Opacity / MaxTime;

            if (Opacity < MaxTime / 4)
            {
                float f = Opacity / (MaxTime / 4);
                alpha = 1 - f;

                scale = Vector2.Lerp(new Vector2(0, 0.2f), new Vector2(endX, 0.3f), f);
            }
            else
            {
                scale = Vector2.Lerp(scale, new Vector2(0.02f, 0.5f), 0.1f);
                if (Opacity > MaxTime * 3 / 4)
                {
                    alpha *= 0.9f;
                }
            }

            if (Opacity > MaxTime)
            {
                active = false;
            }
        }

        public Color GetColor(float factor)
        {
            return Color.Lerp(new Color(181, 255, 180), new Color(202, g: 60, 109), factor);
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D trail = TexValue;
            Vector2 trailOrigin = new Vector2(trail.Width, trail.Height / 2);
            Vector2 trailPos = Position - Main.screenPosition;

            Color c = Color * alpha * 0.7f;
            if (c.A < 2)
            {
                return false;
            }
            Vector2 s = scale * Scale;

            float rot = Rotation + 1.57f;
            Main.spriteBatch.Draw(trail, trailPos, null, c * 0.8f, rot, trailOrigin, s, 0, 0);
            s.X *= 0.35f;
            Main.spriteBatch.Draw(trail, trailPos + Velocity * 2, null, c, rot, trailOrigin, s, 0, 0);

            c.A = 25;
            Main.spriteBatch.Draw(trail, trailPos, null, c, rot, trailOrigin, s, 0, 0);

            return false;
        }
    }
}
