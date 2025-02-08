using Coralite.Content.Items.FairyCatcher;
using Coralite.Content.Items.Materials;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Utilities;

namespace Coralite.Content.NPCs.Town
{
    [AutoloadHead]
    public class ElfRanger : ModNPC
    {
        public override string Texture => AssetDirectory.TownNPC + Name;

        public const string ShopName = "Shop";
        public int NumberOfTimesTalkedTo = 0;

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        private static LocalizedText[] Names;
        private static LocalizedText[] Chats;
        private static LocalizedText[] DryadChats;

        public override void Load()
        {
            // Adds our Shimmer Head to the NPCHeadLoader.
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");

            Names = [
                this.GetLocalization("Name1",()=>"埃芙"),
                this.GetLocalization("Name2",()=>"伊塔"),
                this.GetLocalization("Name3",()=>"弥斯代尔"),
                this.GetLocalization("Name4",()=>"达卢玛尔"),
                this.GetLocalization("Name5",()=>"柯里薇"),
                this.GetLocalization("Name6",()=>"塞尔达"),
                this.GetLocalization("Name7",()=>"菲尔希"),
                ];

            Chats = [
                this.GetLocalization("Chat1",()=>"观察这些小家伙们非常有趣，我经常一看一整天。"),
                this.GetLocalization("Chat2",()=>"仙灵们有各自喜欢的墙壁，在这些墙壁前你才能见到他们"),
                this.GetLocalization("Chat3",()=>"你知道魔尘吗？仙灵很喜欢这种带有魔力的粉尘。"),
                this.GetLocalization("Chat4",()=>"如果你想捕捉仙灵的话，制作一个藤蔓套索吧！"),
                this.GetLocalization("Chat5",()=>"捕捉仙灵并饲养它们是很常见的行为，但不要过度捕捉哦！"),
                this.GetLocalization("Chat6",()=>"你有见过珊瑚吗，是她将我带到这里来的。"),
                this.GetLocalization("Chat7",()=>"或许有一天你能见到空间妖精，并完成它的试炼。"),
                ];

            DryadChats = [
                this.GetLocalization("DryadChat1", () => "那是树妖吗，我想我已经很久没有见过她了。"),
                this.GetLocalization("DryadChat2", () => "树妖曾经在这里对抗克苏鲁，而我在另一个世界里...真是令人怀念的时光。"),
                this.GetLocalization("DryadChat3", () => "树妖身上散发着森林的清香，我很喜欢这股味道。"),
                ];
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 21; // The total amount of frames the NPC has

            NPCID.Sets.ExtraFramesCount[Type] = 7; // 通常适用于城镇 NPC，但这就是 NPC 做额外事情的方式，例如坐在椅子上并与其他 NPC 交谈。这是步行框架之后的剩余帧。
            NPCID.Sets.AttackFrameCount[Type] = 2; // 攻击动画中的帧数。
            NPCID.Sets.DangerDetectRange[Type] = 1200; // NPC 试图攻击敌人时距离 NPC 中心的像素数。
            NPCID.Sets.AttackType[Type] = 2; // 城镇 NPC 执行的攻击类型。0 = 投掷，1 = 射击，2 = 魔法，3 = 近战
            NPCID.Sets.AttackTime[Type] = 600; // NPC 的攻击动画开始后结束所需的时间。
            NPCID.Sets.AttackAverageChance[Type] = 60; // 城镇NPC攻击几率的分母。数字越低，城镇NPC显得更具侵略性。
            NPCID.Sets.HatOffsetY[Type] = 4; // 当队伍处于活动状态时，队伍帽子会在 Y 偏移处生成。
            NPCID.Sets.ShimmerTownTransform[NPC.type] = true; // 这套说城镇 NPC 有一个闪闪发光的形态。否则，城镇NPC会像其他敌人一样在接触微光时变得透明。

            NPCID.Sets.ShimmerTownTransform[Type] = true; // 允许这个NPC在接触微光液体后具有不同的质地。

            // Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
            // NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Like)//喜欢森林，不喜欢冰雪地
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
                .SetBiomeAffection<HallowBiome>(AffectionLevel.Love) // 最喜欢神圣地
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Love) // 最爱树妖
                .SetNPCAffection(NPCID.WitchDoctor, AffectionLevel.Like) // 喜欢巫医
                .SetNPCAffection(NPCID.ArmsDealer, AffectionLevel.Dislike) // 不喜欢军火商
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Hate) // 讨厌渔夫
            ; // < Mind the semicolon!

            // This creates a "profile" for ExamplePerson, which allows for different textures during a party and/or while the NPC is shimmered.
            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture), Texture + "_Party"),
                new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex, Texture + "_Shimmer_Party")
            );
        }

        public override void SetDefaults()
        {
            AnimationType = NPCID.Guide;

            NPC.CloneDefaults(NPCID.Dryad);
            AnimationType = NPCID.Dryad;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary.
				//new FlavorTextBestiaryInfoElement("."),

				//// You can add multiple elements if you really wanted to
				//// You can also use localization keys (see Localization/en-US.lang)
				//new FlavorTextBestiaryInfoElement("Mods.ExampleMod.Bestiary.ExamplePerson")
            });
        }

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

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return Main.LocalPlayer.HasBuff<ElfBless>();
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override List<string> SetNPCNameList()
        {
            List<string> list = [];
            foreach (var name in Names)
                list.Add(name.Value);

            return list;
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new();

            int dryad = NPC.FindFirstNPC(NPCID.Dryad);
            if (dryad >= 0 && Main.rand.NextBool(8))
            {
                foreach (var c in DryadChats)
                    chat.Add(c.Value);
                return chat;
            }

            foreach (var c in Chats)
                chat.Add(c.Value);

            //NumberOfTimesTalkedTo++;
            //if (NumberOfTimesTalkedTo >= 10)
            //    chat.Add(Language.GetOrRegister($"Mods.Coralite.Dialogue.CrystalRobot.TalkALot", () => "魔能辞典，你需要，尽快购买。").Value);

            return chat;
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

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName);
            npcShop.Add<SandlitePowder>(Condition.MoonPhaseFirstQuarter);
            npcShop.Add<EmpyrosPowder>(Condition.MoonPhases04);
            npcShop.Add<IceyPowder>(Condition.MoonPhases15);
            npcShop.Register();
        }

        public override bool CanGoToStatue(bool toKingStatue)
        {
            return !toKingStatue;
        }

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
            damage = 30;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 300;
            randExtraCooldown = 40;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
            // SparklingBall is not affected by gravity, so gravityCorrection is left alone.
        }
    }
}
