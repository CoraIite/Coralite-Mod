using Coralite.Content.ModPlayers;
using Coralite.Content.Particles;
using Coralite.Core;
using Coralite.Core.Configs;
using Coralite.Core.Prefabs.Projectiles;
using InnoVault.Trails;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using InnoVault.GameContent.BaseEntity;

namespace Coralite.Content.Items.RedJades
{
    public class BloodJadeRangesWeapon : ModItem
    {
        public override string Texture => AssetDirectory.RedJadeItems + Name;

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 51;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.knockBack = 2f;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Ranged;
            Item.value = Item.sellPrice(0, 3, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileType<BloodJadeFrisbee>();

            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                if (player.altFunctionUse == 2)
                {
                    Projectile.NewProjectile(new EntitySource_ItemUse(player, Item), player.Center, Vector2.Zero, ProjectileType<BloodJadeFrisbeeParry>(), damage, knockback, player.whoAmI);
                    return false;
                }

                int powerful = player.HasBuff<BloodJadeBuff>() ? 1 : 0;

                if (player.ownedProjectileCounts[type] < 6)
                {
                    //Main.NewText(player.ownedProjectileCounts[type]);
                    Projectile.NewProjectile(source, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f)) * 11, type, damage, knockback, player.whoAmI, powerful);
                }
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<BloodJade>(20)
                .AddTile<Tiles.RedJades.MagicCraftStation>()
                .Register();
        }
    }

    /// <summary>
    /// 向前射出，之后飞回玩家身边，碰到墙壁可以反弹2次
    /// </summary>
    public class BloodJadeFrisbee : ModProjectile, IDrawPrimitive
    {
        public override string Texture => AssetDirectory.RedJadeItems + "BloodJadeRangesWeapon";

        public int hitTileCount;
        public bool hited = false;

        private Trail trail;
        //private Trail trail2;
        public ref float Powerful => ref Projectile.ai[0];

        public ref float State => ref Projectile.ai[2];
        public ref float Timer => ref Projectile.localAI[0];

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.scale = 0.85f;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.oldPos = new Vector2[14];
            for (int i = 0; i < 14; i++)
                Projectile.oldPos[i] = Projectile.Center;
        }

        public override void AI()
        {
            Player projOwner = Main.player[Projectile.owner];

            trail ??= new Trail(Main.instance.GraphicsDevice, 14, new EmptyMeshGenerator(), factor => Helper.Lerp(4, 8, factor), factor =>
            {
                if (factor.X < 0.7f)
                    return Color.Lerp(Color.Transparent, new Color(81, 11, 47, 150), factor.X / 0.7f);

                return Color.Lerp(new Color(81, 11, 47, 150), Color.Transparent, (factor.X - 0.7f) / 0.3f);
            });
            //trail2 ??= new Trail(Main.instance.GraphicsDevice, 6, new NoTip(), factor => Helper.Lerp(8, 10, factor), factor =>
            //{
            //    if (factor.X < 0.7f)
            //        return Color.Lerp(Color.Transparent, Color.White*0.5f, factor.X / 0.7f);

            //    return Color.Lerp(Color.White*0.5f, Color.Transparent, (factor.X - 0.7f) / 0.3f);
            //});

            Projectile.rotation += 0.35f;

            switch (State)
            {
                default:
                case 0://飞出
                    {

                        if (Projectile.timeLeft % 16 == 0)
                            SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);

                        if (Timer > 60)
                            ChangeState(1);
                    }
                    break;
                case 1://回收
                    {
                        if (Vector2.Distance(projOwner.Center, Projectile.Center) < 24)
                            Projectile.Kill();

                        else if (Vector2.Distance(projOwner.Center, Projectile.Center) < 200)
                            Projectile.velocity += Vector2.Normalize(projOwner.Center - Projectile.Center) * 4;
                        else
                            Projectile.velocity += Vector2.Normalize(projOwner.Center - Projectile.Center);

                        if (Projectile.velocity.Length() > 14)
                            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 14;

                        if (Projectile.timeLeft % 16 == 0)
                            SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);

                    }
                    break;
            }

            Timer++;
            for (int i = 0; i < 13; i++)
                Projectile.oldPos[i] = Projectile.oldPos[i + 1];

            Projectile.oldPos[13] = Projectile.Center + Projectile.velocity;
            trail.TrailPositions = Projectile.oldPos;
            //trail2.Positions = Projectile.oldPos;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hited)
            {
                if (VisualEffectSystem.HitEffect_Lightning)
                {
                    float baseScale = 0.1f;
                    float _Rotation = (target.Center - Projectile.Center).ToRotation();
                    Vector2 pos = Projectile.Center + (_Rotation.ToRotationVector2() * (Projectile.width / 2f));
                    Dust dust = Dust.NewDustPerfect(pos, DustType<BloodJadeStrikeDust>(),
                        Scale: Main.rand.NextFloat(baseScale, baseScale * 1.3f));
                    dust.rotation = _Rotation + MathHelper.PiOver2 + Main.rand.NextFloat(-0.2f, 0.2f);

                    dust = Dust.NewDustPerfect(pos, DustType<BloodJadeStrikeDust>(),
                             Scale: Main.rand.NextFloat(baseScale * 0.2f, baseScale * 0.3f));
                    float leftOrRight = Main.rand.NextFromList(-0.3f, 0.3f);
                    dust.rotation = _Rotation + MathHelper.PiOver2 + leftOrRight + Main.rand.NextFloat(-0.8f, 0.8f);
                }

                if (Powerful == 0)
                    ChangeState(1, State == 0);
                else
                {
                    var source = Projectile.GetSource_FromAI();
                    int type = Main.rand.NextFromList(ProjectileType<RedJadeBoom>(), ProjectileType<RedJadeBigBoom>());
                    Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero, type,
                        (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
                }

                Projectile.damage = (int)(Projectile.damage * 0.5f);
                hited = true;
            }
        }

        public void ChangeState(int state, bool bounce = false)
        {
            if (State == 0)
                Timer = 0;

            State = state;

            if (bounce && State == 1)
                Projectile.velocity = -Projectile.velocity;

            Projectile.timeLeft = 200;
            Projectile.netUpdate = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hitTileCount++;
            float newVelX = Math.Abs(Projectile.velocity.X);
            float newVelY = Math.Abs(Projectile.velocity.Y);
            float oldVelX = Math.Abs(oldVelocity.X);
            float oldVelY = Math.Abs(oldVelocity.Y);
            if (oldVelX > newVelX)
                Projectile.velocity.X = -oldVelX;
            if (oldVelY > newVelY)
                Projectile.velocity.Y = -oldVelY;


            if (hitTileCount > 2)
            {
                Projectile.tileCollide = false;
                ChangeState(1);
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Color c = lightColor;
            Color c2 = Coralite.RedJadeRed;
            c2.A = 100;
            c2 *= 0.6f;
            for (int i = 0; i < 3; i++)
            {
                Vector2 offset = (Main.GlobalTimeWrappedHourly + (i * MathHelper.TwoPi / 3)).ToRotationVector2();
                Main.spriteBatch.Draw(mainTex, Projectile.Center + (offset * 4) - Main.screenPosition, null, c2, Projectile.rotation,
                    mainTex.Size() / 2, Projectile.scale * 1.1f, 0, 0);
            }

            for (int i = 12; i > 8; i--)
                Main.spriteBatch.Draw(mainTex, Projectile.oldPos[i] - Main.screenPosition, null,
                c2 * (1f - ((12 - i) * 1 / 5f)), Projectile.rotation, mainTex.Size() / 2, Projectile.scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, c, Projectile.rotation,
                mainTex.Size() / 2, Projectile.scale, 0, 0);

            return false;
        }

        public void DrawPrimitives()
        {
            Effect effect = Filters.Scene["SimpleTrailNoHL"].GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(CoraliteAssets.Trail.EdgeA.Value);

            trail?.DrawTrail(effect);
            //trail2?.Render(effect);
        }
    }

    public class BloodJadeFrisbeeParry : BaseHeldProj
    {
        public override string Texture => AssetDirectory.RedJadeItems + "BloodJadeRangesWeapon";

        public ref float Angle => ref Projectile.ai[2];

        public int Timer;
        public float alpha;
        public float distanceToOwner;

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.netImportant = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
        }

        public override bool? CanDamage() => false;

        public override void AI()
        {
            Owner.itemTime = Owner.itemAnimation = 2;
            Projectile.direction = Owner.direction;
            Projectile.rotation += 0.35f;

            do
            {
                if (Timer == 0)
                {
                    Angle = ToMouseAngle - (Owner.direction * 1f);
                }
                if (Timer < 12)
                {
                    alpha += 1 / 12f;
                    distanceToOwner += 44 / 12f;
                    //Projectile.scale = Helper.Lerp(1, 1.3f, Timer / 12f);
                    Angle += Owner.direction * 1f / 12f;
                    break;
                }

                if (Timer == 12)
                {
                    SoundEngine.PlaySound(CoraliteSoundID.Zenith_Item169, Projectile.Center);
                    Owner.immuneTime = 12;
                    Owner.immune = true;
                }

                if (Timer < 20)
                {
                    Rectangle rect = Projectile.getRect();
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (!proj.IsActiveAndHostile() || proj.whoAmI == Projectile.whoAmI)
                            continue;

                        if (proj.Colliding(proj.getRect(), rect))
                        {
                            Parry(33);
                            break;
                        }
                    }

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];

                        if (!npc.active || npc.friendly || npc.immortal)
                            continue;

                        if (Projectile.Colliding(rect, npc.getRect()))
                        {
                            Parry(20);
                            break;
                        }
                    }

                    break;
                }

                if (Timer < 20 + 40)
                {
                    Angle += Owner.direction * MathHelper.TwoPi / 40;
                    alpha -= 1 / 40f;
                    if (distanceToOwner > 0)
                        distanceToOwner -= 32 / 25f;
                    //Projectile.scale = Helper.Lerp(1.3f, 1f, (Timer - 24) / 40f);
                    break;
                }
                Projectile.Kill();

            } while (false);

            Projectile.Center = Owner.Center + (Angle.ToRotationVector2() * distanceToOwner);
            Timer++;
        }

        public void Parry(int immuneTime)
        {
            if (Owner.TryGetModPlayer(out CoralitePlayer cp))
            {
                if (cp.parryTime < 100)
                {
                    for (int i = -1; i < 2; i += 2)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Owner.Center, UnitToMouseV.RotatedBy(i * 0.3f) * 11, ProjectileType<BloodJadeFrisbee>(),
                            (int)(Projectile.damage * 0.6f), Projectile.knockBack, Projectile.owner, 1);
                    }
                    Owner.AddImmuneTime(ImmunityCooldownID.General, immuneTime);
                    Owner.immune = true;
                }
                if (cp.parryTime < 5)
                {
                    Owner.AddBuff(BuffType<BloodJadeBuff>(), 60 * 6);
                }

                if (cp.parryTime < 280)
                    cp.parryTime += 100;
            }
            PRTLoader.NewParticle(Projectile.Center, Vector2.Zero,
                CoraliteContent.ParticleType<Sparkle_Big>(), Coralite.RedJadeRed, 1.5f);
            SoundEngine.PlaySound(CoraliteSoundID.Ding_Item4, Projectile.Center);
            Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();

            Color c = lightColor * alpha;
            Color c2 = Coralite.RedJadeRed;
            c2.A = 150;
            c2 *= 0.6f * alpha;
            for (int i = 0; i < 3; i++)
            {
                Vector2 offset = (Main.GlobalTimeWrappedHourly + (i * MathHelper.TwoPi / 3)).ToRotationVector2();
                Main.spriteBatch.Draw(mainTex, Projectile.Center + (offset * 4) - Main.screenPosition, null, c2, Projectile.rotation,
                    mainTex.Size() / 2, Projectile.scale, 0, 0);
            }

            Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, c, Projectile.rotation,
                mainTex.Size() / 2, Projectile.scale, 0, 0);
            return false;
        }
    }
}
