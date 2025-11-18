using Coralite.Content.Bosses.VanillaReinforce.NightmarePlantera;
using Coralite.Content.Items.Materials;
using Coralite.Content.Items.Nightmare;
using Coralite.Core;
using Coralite.Core.Systems.BossSystems;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Core.Systems.MagikeSystem.MagikeCraft;
using Coralite.Helpers;
using InnoVault.GameContent.BaseEntity;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Coralite.Content.Items.BossSummons
{
    public class NightmareHarp : ModItem, IMagikeCraftable
    {
        public override string Texture => AssetDirectory.BossSummons + Name;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;

            NPCID.Sets.MPAllowedEnemies[ModContent.NPCType<NightmarePlantera>()] = true;

            this.GetLocalization("ShouldSleep", () => "在床上设置出生点后才能使用这个物品");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 10, 0);
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.rare = ModContent.RarityType<NightmareRarity>();

            Item.shoot = ModContent.ProjectileType<NightmareHarpProj>();
            Item.noUseGraphic = true;
        }

        public override bool CanUseItem(Player player)
        {
            bool hasRespawn = player.SpawnX != -1 && player.SpawnY != -1;

            if (!hasRespawn)
            {
                CombatText.NewText(new Rectangle((int)player.Top.X, (int)player.Top.Y, 1, 1), Color.White,
                    this.GetLocalization("ShouldSleep").Value);
            }
            return hasRespawn && !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<NightmarePlantera>());
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ModContent.GetInstance<DownedNightmarePlantera>().Value && player.altFunctionUse == 2)
            {
                int npcType = ModContent.NPCType<NightmarePlantera>();

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(player.whoAmI, npcType);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: npcType);

                return false;
            }

            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, 1, 0, player.whoAmI);
            return false;
        }

        public void AddMagikeCraftRecipe()
        {
            MagikeRecipe.CreateCraftRecipe(ItemID.Harp, ModContent.ItemType<NightmareHarp>()
                , MagikeHelper.CalculateMagikeCost(MALevel.SplendorMagicore, 12, 60 * 5))
                .AddIngredient(ItemID.SoulofLight, 7)
                .AddIngredient(ItemID.SoulofNight, 7)
                .AddIngredient(ItemID.SoulofMight, 7)
                .AddIngredient(ItemID.SoulofSight, 7)
                .AddIngredient(ItemID.SoulofFright, 7)
                .AddIngredient(ItemID.SoulofFlight, 7)
                .AddIngredient<SoulOfDeveloper>(7)
                .AddCondition(Condition.DownedMoonLord)
                .Register();
        }
    }

    public class NightmareHarpProj : BaseHeldProj
    {
        public override string Texture => AssetDirectory.BossSummons + "NightmareHarp";

        public ref float Timer => ref Projectile.ai[0];

        #region 音符们
        public static SoundStyle So1
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                st.Pitch = -5 / 12f;
                return st;
            }
        }

        public static SoundStyle La1
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                st.Pitch = -3 / 12f;
                return st;
            }
        }
        public static SoundStyle Si1
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                st.Pitch = -1 / 12f;
                return st;
            }
        }

        public static SoundStyle Do
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                //st.Pitch = 0.3f;
                return st;
            }
        }

        public static SoundStyle Rai
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                st.Pitch = 2 / 12f;
                return st;
            }
        }
        public static SoundStyle Mi
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                st.Pitch = 4 / 12f;
                return st;
            }
        }
        public static SoundStyle Fa
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                st.Pitch = 5 / 12f;
                return st;
            }
        }
        public static SoundStyle So
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                st.Pitch = 7 / 12f;
                return st;
            }
        }
        public static SoundStyle La
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                st.Pitch = 9 / 12f;
                return st;
            }
        }
        public static SoundStyle Si
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                st.Pitch = 11 / 12f;
                return st;
            }
        }

        public static SoundStyle Do2
        {
            get
            {
                SoundStyle st = CoraliteSoundID.Harp_Item26;
                st.Pitch = 1f;
                return st;
            }
        }
        #endregion

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            Owner.itemAnimation = Owner.itemTime = 2;
            Owner.itemRotation = Owner.direction * (1f + (0.2f * MathF.Sin(Timer * 0.2f)));

            Owner.heldProj = Projectile.whoAmI;
            Projectile.rotation = Owner.direction * (0.3f + (0.3f * MathF.Sin(Timer * 0.02f)));

            Projectile.Center = Owner.Center + new Vector2(Owner.direction * 8, 0);

            const int part = 45;
            //来自舒伯特的《摇篮曲》
            //3 5 2 34 | 3 3 21 7`1 | 2  

            if (Timer == 10)
            {
                SpawnNote(0, 0);
                PlaySound(Mi);
            }
            else if (Timer == 10 + part)
            {
                SpawnNote(0, 1);
                PlaySound(So);
            }
            else if (Timer == 10 + (part * 2))
            {
                SpawnNote(0, 2);
                PlaySound(Rai);
            }
            else if (Timer == 10 + (part * 3))
            {
                SpawnNote(1, 3);
                PlaySound(Mi);
            }
            else if (Timer == 10 + (part * 3) + (part / 2))
                PlaySound(Fa);

            else if (Timer == 10 + (part * 4))
            {
                SpawnNote(0, 4);
                PlaySound(Mi);
            }
            else if (Timer == 10 + (part * 5))
            {
                SpawnNote(0, 5);
                PlaySound(Mi);
            }
            else if (Timer == 10 + (part * 6))
            {
                SpawnNote(1, 6);
                PlaySound(Rai);
            }
            else if (Timer == 10 + (part * 6) + (part / 2))
                PlaySound(Do);
            else if (Timer == 10 + (part * 7))
                PlaySound(Si1);
            else if (Timer == 10 + (part * 7) + (part / 2))
            {
                PlaySound(Do);

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<NightmareMusicalNotes>())
                    {
                        p.localAI[1] = 1;
                    }
                }
            }

            else if (Timer == 10 + (part * 8))
                PlaySound(Rai);
            else if (Timer == 10 + (part * 9))
                PlaySound(So1);
            else if (Timer > 10 + (part * 10) + 20)
            {
                SoundEngine.PlaySound(CoraliteSoundID.IceMagic_Item28);
                if (Projectile.IsOwnedByLocalPlayer())
                {
                    int npcType = ModContent.NPCType<NightmarePlantera>();

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.SpawnOnPlayer(Owner.whoAmI, npcType);
                    else
                        NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: Owner.whoAmI, number2: npcType);
                }

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile p = Main.projectile[i];
                    if (p.active && p.type == ModContent.ProjectileType<NightmareMusicalNotes>())
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), p.Center, (p.Center - Owner.Center).SafeNormalize(Vector2.Zero) * 16
                            , ModContent.ProjectileType<NightmareSpawnEnergy>(), 1, 0, Projectile.owner, p.ai[1], ai2: 0);
                        p.Kill();
                    }
                }

                Projectile.Kill();
            }

            Timer++;
        }

        public void PlaySound(SoundStyle style)
        {
            PRTLoader.NewParticle(Owner.Center, new Vector2(Projectile.owner), CoraliteContent.ParticleType<NightmareHarpParticle>()
                , NightmarePlantera.nightPurple, 0.1f);
            SoundEngine.PlaySound(style);
        }

        public void SpawnNote(int noteType, int ColorType)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Vector2.Zero, ModContent.ProjectileType<NightmareMusicalNotes>()
               , 1, 0, Projectile.owner, noteType, ColorType, ColorType);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var pos = Projectile.Center - Main.screenPosition;
            SpriteEffects effect = Owner.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(mainTex, pos, null, lightColor, Projectile.rotation, mainTex.Size() / 2, Projectile.scale, effect, 0);
            return false;
        }
    }

    /// <summary>
    /// 使用ai0传入音符类型<br></br>
    /// 使用ai1传入颜色，范围0-6<br></br>
    /// 使用ai2传入自己是第几个<br></br>
    /// </summary>
    public class NightmareMusicalNotes : BaseHeldProj
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "NightmareMusicalNotes";

        public ref float NoteType => ref Projectile.ai[0];
        public ref float Color => ref Projectile.ai[1];
        public ref float Which => ref Projectile.ai[2];

        public Color drawColor;

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.width = Projectile.height = 32;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => false;
        public override bool? CanDamage() => false;

        public override void Initialize()
        {
            if (Color < 0)
                drawColor = NightmarePlantera.nightmareSparkleColor;
            else
            {
                int c = Math.Clamp((int)Color, 0, 6);
                drawColor = NightmarePlantera.phantomColors[c];
                drawColor *= 1.3f;
            }
        }

        public override void AI()
        {
            float factor = MathF.Sin(Main.GlobalTimeWrappedHourly);
            Projectile.rotation = factor * 0.3f;

            Projectile.localAI[0]++;

            float factor2 = Math.Clamp(Projectile.localAI[0] / 140, 0, 1);
            Vector2 pos = Owner.Center + ((Main.GlobalTimeWrappedHourly + (MathHelper.TwoPi * Which / 7)).ToRotationVector2() * (96 + (factor * 16)));
            float length = Vector2.Distance(pos, Projectile.Center);
            Projectile.velocity = (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * factor2 * (length < 16 ? length : 16);

            Lighting.AddLight(Projectile.Center, drawColor.ToVector3());

            if (Projectile.localAI[1] == 1)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, 32, 32, DustID.VilePowder, Scale: Main.rand.NextFloat(1, 1.5f));
                d.noGravity = true;

                if (Projectile.localAI[2] < 30)
                {
                    Projectile.scale *= 1.01f;
                }
                else
                {
                    if (Projectile.scale > 0.1f)
                    {
                        Projectile.scale *= 0.99f;
                    }
                }

                Projectile.localAI[2]++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTex = Projectile.GetTexture();
            var frameBox = mainTex.Frame(2, 1, (int)NoteType, 0);

            var pos = Projectile.Center - Main.screenPosition;

            Color d = drawColor;
            d.A = 0;

            Main.spriteBatch.Draw(mainTex, pos, frameBox, d, Projectile.rotation, frameBox.Size() / 2, Projectile.scale, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, d, Projectile.rotation, frameBox.Size() / 2, Projectile.scale + 0.2f, 0, 0);
            Main.spriteBatch.Draw(mainTex, pos, frameBox, d, Projectile.rotation, frameBox.Size() / 2, Projectile.scale + 0.2f, 0, 0);

            return false;
        }
    }

    /// <summary>
    /// 使用速度的X传入拥有者
    /// </summary>
    public class NightmareHarpParticle : Particle
    {
        public override string Texture => AssetDirectory.NightmarePlantera + "Flow";

        public override void SetProperty()
        {
            Rotation = Main.rand.NextFloat(6.282f);
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            int ownerIndex = (int)Velocity.X;
            if (Main.player.IndexInRange(ownerIndex))
            {
                Position = Main.player[ownerIndex].Center;
            }

            Rotation += 0.05f;
            Opacity++;

            if (Opacity < 15)
            {
                Scale *= 1.08f;
            }
            else if (Opacity < 20)
            {
                Scale *= 0.94f;
            }
            else
            {
                Scale *= 1.02f;
                if (Opacity > 30)
                    Color.A = (byte)(Color.A * 0.82f);
            }

            if (Color.A < 10)
                active = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            Texture2D mainTex = TexValue;
            Vector2 origin = mainTex.Size() / 2;

            spriteBatch.Draw(mainTex, Position - Main.screenPosition, null, Color, Rotation, origin, Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
