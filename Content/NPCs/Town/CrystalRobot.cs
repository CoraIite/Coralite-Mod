using Coralite.Content.Biomes;
using Coralite.Content.Items.Magike;
using Coralite.Content.Items.Magike.Towers;
using Coralite.Content.Items.MagikeSeries1;
using Coralite.Core;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Coralite.Content.NPCs.Town
{
    [AutoloadHead]
    public class CrystalRobot : ModNPC
    {
        public override string Texture => AssetDirectory.TownNPC + Name;

        public const string ShopName = "Shop";
        public int NumberOfTimesTalkedTo = 0;

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        private static LocalizedText[] Names;
        private static LocalizedText[] StandardChats;
        private static LocalizedText[] CommonChats;
        private static LocalizedText[] RareChats;

        public override void Load()
        {
            // Adds our Shimmer Head to the NPCHeadLoader.
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
            Names =
                [
                this.GetLocalization("Name0", () => "晶华"),
                this.GetLocalization("Name1", () => "晶: c000"),
                this.GetLocalization("Name2", () => "克丽丝塔"),
                this.GetLocalization("Name3", () => "魔盖克"),
                ];

            StandardChats = [
                this.GetLocalization("StandardDialogue1", () => "你看上去缺少了一点魔能！"),
                this.GetLocalization("StandardDialogue2", () => "需要补充魔能以维持机体运转。"),
                this.GetLocalization("StandardDialogue3", () => "魔力晶体，美味。"),
                this.GetLocalization("StandardDialogue4", () => "天空中，有过去的同伴。"),
                this.GetLocalization("StandardDialogue5", () => "魔力水晶洞，舒适。"),
                ];

            CommonChats = [
                this.GetLocalization("CommonDialogue1", () => "魔力晶体，需要大量，会用实用物资与你交换。"),
                ];

            RareChats = [
                this.GetLocalization("RareDialogue1", () => "妖精与仙灵，强大且充满智慧。"),
                this.GetLocalization("RareDialogue2", () => "珊瑚... ... 资料未解锁。"),
                this.GetLocalization("RareDialogue3", () => "古代的龙们，穿过时间来到这里。"),
                ];

            this.RegisterBestiaryDescription();
        }

        public override void Unload()
        {
            Names = null;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25; // The total amount of frames the NPC has

            NPCID.Sets.ExtraFramesCount[Type] = 9; // 通常适用于城镇 NPC，但这就是 NPC 做额外事情的方式，例如坐在椅子上并与其他 NPC 交谈。这是步行框架之后的剩余帧。
            NPCID.Sets.AttackFrameCount[Type] = 4; // 攻击动画中的帧数。
            NPCID.Sets.DangerDetectRange[Type] = 600; // NPC 试图攻击敌人时距离 NPC 中心的像素数。
            NPCID.Sets.AttackType[Type] = 0; // 城镇 NPC 执行的攻击类型。0 = 投掷，1 = 射击，2 = 魔法，3 = 近战
            NPCID.Sets.AttackTime[Type] = 90; // NPC 的攻击动画开始后结束所需的时间。
            NPCID.Sets.AttackAverageChance[Type] = 30; // 城镇NPC攻击几率的分母。数字越低，城镇NPC显得更具侵略性。
            NPCID.Sets.HatOffsetY[Type] = 4; // 当队伍处于活动状态时，队伍帽子会在 Y 偏移处生成。
            NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // 这套说城镇 NPC 有一个闪闪发光的形态。否则，城镇NPC会像其他敌人一样在接触微光时变得透明。

            NPCID.Sets.ShimmerTownTransform[Type] = true; // 允许这个NPC在接触微光液体后具有不同的质地。

            // Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
            // NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
            NPC.Happiness
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<MagicCrystalCave>(AffectionLevel.Love) // 喜欢魔力水晶洞
                .SetNPCAffection(NPCID.Wizard, AffectionLevel.Love) // 喜欢巫师
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Like) // 喜欢树妖
                .SetNPCAffection(NPCID.Steampunker, AffectionLevel.Dislike) // 不喜欢蒸汽朋克人
                .SetNPCAffection(NPCID.Cyborg, AffectionLevel.Hate) // 讨厌纳米人
            ; // < Mind the semicolon!

            // This creates a "profile" for ExamplePerson, which allows for different textures during a party and/or while the NPC is shimmered.
            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture + "_Party"),
                new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer_Party")
            );
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true; // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = CoraliteSoundID.DigStone_Tink;
            NPC.DeathSound = CoraliteSoundID.RockGolem_NPCDeath43;
            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Guide;
            SpawnModBiomes = [ModContent.GetInstance<MagicCrystalCave>().Type];
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.AddTags(
                this.GetBestiaryDescription()
                );
        }

        // The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
        // Returning false will allow you to manually draw your NPC
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // This code slowly rotates the NPC in the bestiary
            // (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
            {
                //drawModifiers.Rotation += 0.001f;

                // Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
                NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            }

            return true;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int num = NPC.life > 0 ? 1 : 5;

            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Teleporter);
            }

            // Create gore when the NPC is killed.
            if (Main.netMode != NetmodeID.Server && NPC.life <= 0)
            {
                // 检索血腥类型。这个 NPC 有头部、手臂和腿部血腥的微光和派对变体。（共 12 个血腥）
                //string variant = "";
                //if (NPC.IsShimmerVariant) variant += "_Shimmer";
                //if (NPC.altTexture == 1) variant += "_Party";
                //int hatGore = NPC.GetPartyHatGore();
                //int headGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Head").Type;
                //int armGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Arm").Type;
                //int legGore = Mod.Find<ModGore>($"{Name}_Gore{variant}_Leg").Type;

                //// Spawn the gores. The positions of the arms and legs are lowered for a more natural look.
                //if (hatGore > 0)
                //{
                //    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, hatGore);
                //}
                //Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, headGore, 1f);
                //Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
                //Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 20), NPC.velocity, armGore);
                //Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
                //Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(0, 34), NPC.velocity, legGore);
            }
        }


        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return ModContent.GetInstance<LearnedMagikeBase>().Value;
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override List<string> SetNPCNameList()
        {
            List<string> list = new();

            foreach (var name in Names)
                list.Add(name.Value);

            return list;
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();

            foreach (var item in StandardChats)
                chat.Add(item.Value);
            foreach (var item in CommonChats)
                chat.Add(item.Value, 3);
            foreach (var item in RareChats)
                chat.Add(item.Value, 0.1f);

            return chat.Get();
        }

        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
            button = Language.GetTextValue("LegacyInterface.28");
            //button2 = "Awesomeify";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = ShopName; // Name of the shop tab we want to open.
            }
        }

        // Not completely finished, but below is what the NPC will sell
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName);
            int magicCrystalCurrencyID = CurrencyLoader.GetCurrencyID<MagicCrystalCurrency>();
            npcShop.Add(new Item(ModContent.ItemType<MagikeMonoclastic>())//单片镜
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = magicCrystalCurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<MagConnectStaff>())//连接杖
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = magicCrystalCurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<MagikeActivator>())//激活杖
            {
                shopCustomPrice = 8,
                shopSpecialCurrency = magicCrystalCurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<OpticalPathCalibrator>())//同步杖
            {
                shopCustomPrice = 8,
                shopSpecialCurrency = magicCrystalCurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<CondensedCrystalBall>())//充能球
            {
                shopCustomPrice = 8,
                shopSpecialCurrency = magicCrystalCurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<WarpMirror>())//扭曲镜
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = magicCrystalCurrencyID
            });

            npcShop.Add(new Item(ModContent.ItemType<CrystalSword>())//水晶剑
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = magicCrystalCurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<CrystalStaff>())//方块杖
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = magicCrystalCurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<MagikeWorldBall>())//世界球
            {
                shopCustomPrice = 1,
                shopSpecialCurrency = magicCrystalCurrencyID
            });
            npcShop.Add(new Item(ModContent.ItemType<MagikeAnalyser>())//分析仪
            {
                shopCustomPrice = 15,
                shopSpecialCurrency = magicCrystalCurrencyID
            });

            npcShop.Register();
        }

        public override bool CanGoToStatue(bool toKingStatue) => true;

        // Make something happen when the npc teleports to a statue. Since this method only runs server side, any visual effects like dusts or gores have to be synced across all clients manually.
        public override void OnGoToStatue(bool toKingStatue)
        {
            if (VaultUtils.isServer)
            {
                ModPacket packet = Mod.GetPacket();
                packet.Write((byte)NPC.whoAmI);
                packet.Send();
            }
            else
            {
                StatueTeleport();
            }
        }

        // Create a square of pixels around the NPC on teleport.
        public void StatueTeleport()
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = Main.rand.NextVector2Square(-20, 21);
                if (Math.Abs(position.X) > Math.Abs(position.Y))
                {
                    position.X = Math.Sign(position.X) * 20;
                }
                else
                {
                    position.Y = Math.Sign(position.Y) * 20;
                }

                Dust.NewDustPerfect(NPC.Center + position, DustID.Teleporter, Vector2.Zero).noGravity = true;
            }
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<MagicBeam>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
            // SparklingBall is not affected by gravity, so gravityCorrection is left alone.
        }

        public override void LoadData(TagCompound tag)
        {
            NumberOfTimesTalkedTo = tag.GetInt("numberOfTimesTalkedTo");
        }

        public override void SaveData(TagCompound tag)
        {
            tag["numberOfTimesTalkedTo"] = NumberOfTimesTalkedTo;
        }
    }

    public class MagicCrystalCurrency : CustomCurrencySingleCoin
    {
        public MagicCrystalCurrency() : base(ModContent.ItemType<MagicCrystal>(), 9999)
        {
            CurrencyTextKey = "Mods.Coralite.Items.MagicCrystal.DisplayName";
            CurrencyTextColor = Coralite.MagicCrystalPink;
        }
    }
}
