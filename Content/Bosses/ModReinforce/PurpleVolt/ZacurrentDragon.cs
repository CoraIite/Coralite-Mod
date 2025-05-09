using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Coralite.Content.Bosses.ModReinforce.PurpleVolt
{
    [AutoLoadTexture(Path=AssetDirectory.ZacurrentDragon)]
    public partial class ZacurrentDragon : ModNPC
    {
        public override string Texture => AssetDirectory.ZacurrentDragon + Name;

        private Player Target => Main.player[NPC.target];

        /// <summary>
        /// 是否处于紫伏状态
        /// </summary>
        public bool PurpleVolt { get; private set; }
        /// <summary>
        /// 是否绘制残影
        /// </summary>
        public bool canDrawShadows;
        /// <summary>
        /// 是否绘制冲刺是的特殊贴图，如果为true会按照特殊的方式绘制自身
        /// </summary>
        public bool isDashing;

        /// <summary>
        /// 身上有电流环绕，会减伤并生成闪电粒子
        /// </summary>
        public bool currentSurrounding;
        public float selfAlpha = 1f;

        [AutoLoadTexture(Name = "ZacurrentDragon_Highlight")]
        internal static ATex GlowTex { get;private set; }
        internal static Color ThunderveinYellow = new(255, 202, 101);
        internal static Color ZacurrentPurple = new(135, 94, 255);
        internal static Color ThunderveinOrange = new(219, 114, 22);

        /// <summary>
        /// 头部帧图
        /// </summary>
        private byte headFrame;

        #region tmlHooks

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            NPC.width = 130;
            NPC.height = 100;
            NPC.damage = 60;
            NPC.defense = 50;
            NPC.lifeMax = 85500;
            NPC.knockBackResist = 0f;
            NPC.scale = 1.2f;
            NPC.aiStyle = -1;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(0, 20, 0, 0);

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;

            NPC.BossBar = ModContent.GetInstance<ZacurrentDragonBossBar>();
            ModContent.GetInstance<ZacurrentDragonBossBar>().Reset(NPC);

            //BGM：暂无
            //if (!Main.dedServ)
            //    Music = MusicLoader.GetMusicSlot(Mod, "Sounds/Music/IcyColdStream");
            Music = MusicID.OtherworldlyTowers;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            if (Helper.GetJourneyModeStrangth(out float journeyScale, out NPCStrengthHelper nPCStrengthHelper))
            {
                if (nPCStrengthHelper.IsExpertMode)
                {
                    NPC.lifeMax = (int)((79000 + (numPlayers * 15500)) / journeyScale);
                    NPC.damage = 66;
                    NPC.defense = 50;
                }

                if (nPCStrengthHelper.IsMasterMode)
                {
                    NPC.lifeMax = (int)((85500 + (numPlayers * 19550)) / journeyScale);
                    NPC.damage = 72;
                    NPC.defense = 50;
                }

                if (Main.getGoodWorld)
                {
                    NPC.damage = 80;
                    NPC.defense = 50;
                }

                if (Main.zenithWorld)
                {
                    NPC.scale = 2.4f;
                }

                return;
            }

            NPC.lifeMax = 79000 + (numPlayers * 15500);
            NPC.damage = 66;
            NPC.defense = 50;

            if (Main.masterMode)
            {
                NPC.lifeMax = 85500 + (numPlayers * 19550);
                NPC.damage = 72;
                NPC.defense = 50;
            }

            if (Main.getGoodWorld)
            {
                NPC.lifeMax = 92000 + (numPlayers * 25850);
                NPC.damage = 80;
                NPC.defense = 50;
            }

            if (Main.zenithWorld)
            {
                NPC.scale = 2.4f;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            int width = (int)(95 * NPC.scale);
            int height = (int)(70 * NPC.scale);
            npcHitbox = new Rectangle((int)(NPC.Center.X - (width / 2)), (int)(NPC.Center.Y - (height / 2)), width, height);
            return true;
        }

        #endregion

        #region 绘制部分

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D mainTex = NPC.GetTexture();

            var frameBox = mainTex.Frame(5, 8, 0, NPC.frame.Y);
            var pos = NPC.Center - screenPos;
            var origin = frameBox.Size() / 2;
            float rot = NPC.rotation;

            SpriteEffects effects = SpriteEffects.None;

            if (NPC.spriteDirection < 0)
            {
                effects = SpriteEffects.FlipVertically;
            }

            //绘制残影
            //if (canDrawShadows)
            //{
            //    Color shadowColor = ThunderveinYellowAlpha;
            //    shadowColor.A = 50;
            //    shadowColor *= shadowAlpha;
            //    for (int i = 0; i < trailCacheLength; i++)
            //    {
            //        Vector2 oldPos = NPC.oldPos[i] - screenPos;
            //        float oldrot = NPC.oldRot[i];
            //        var frameOld = mainTex.Frame(3, 8, oldFrame[i].X, oldFrame[i].Y);
            //        float factor = (float)i / trailCacheLength;
            //        if (Phase == 2)
            //        {
            //            Color c1 = ThunderveinYellowAlpha;
            //            c1.A = 50;
            //            Color c2 = ThunderveinPurpleAlpha;
            //            c2.A = 50;
            //            shadowColor = Color.Lerp(c2, c1, factor);
            //            shadowColor *= shadowAlpha;
            //        }

            //        SpriteEffects oldEffect = oldDirection[i] > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            //        spriteBatch.Draw(mainTex, oldPos, frameOld, shadowColor * factor, oldrot, origin
            //            , NPC.scale * shadowScale * (1 - ((1 - factor) * 0.3f)), oldEffect, 0);
            //    }
            //}

            //绘制自己
            if (Main.zenithWorld)
                drawColor *= 0.2f;

            //DrawSelf(spriteBatch, mainTex, pos, 4, drawColor, effects);//后面的翅膀
            //DrawSelf(spriteBatch, mainTex, pos, 3, drawColor, effects);//身体
            //DrawSelf(spriteBatch, mainTex, pos, 2, drawColor, effects);//头
            //DrawSelf(spriteBatch, mainTex, pos, 1, drawColor, effects);//前面的翅膀

            //绘制冲刺时的特效
            if (isDashing)
            {
                //Texture2D exTex = ModContent.Request<Texture2D>(AssetDirectory.OtherProjectiles + "StrikeTrail").Value;

                //Vector2 exOrigin = new(exTex.Width * 6 / 10, exTex.Height / 2);

                //Vector2 scale = new Vector2(1.3f, 1.5f) * NPC.scale;
                //spriteBatch.Draw(exTex, pos, null, ThunderveinYellowAlpha, rot
                //    , exOrigin, scale, effects, 0);
                //scale.Y *= 1.2f;
                //spriteBatch.Draw(exTex, pos - (NPC.rotation.ToRotationVector2() * 50), null, ThunderveinYellowAlpha * 0.5f, rot
                //    , exOrigin, scale, effects, 0);
            }

            //if (State == (int)AIStates.onKillAnim)
            //{
            //    Texture2D whiteTex = ModContent.Request<Texture2D>(AssetDirectory.ThunderveinDragon + "ThunderveinDragon_Highlight").Value;

            //    spriteBatch.Draw(whiteTex, pos, frameBox, Color.White * anmiAlpha, rot, origin, NPC.scale, effects, 0);
            //}

            return false;
        }

        public void DrawBackWing(SpriteBatch spriteBatch, Texture2D mainTex, Vector2 pos, int frameX, Color drawColor, SpriteEffects effects)
        {
            float rot = NPC.rotation;
            Rectangle frameBox = new(0, NPC.frame.Y, 4, Main.npcFrameCount[NPC.type]);

            //绘制本体
            mainTex.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, drawColor, rot, NPC.scale);
            //绘制glow
            GlowTex.Value.QuickCenteredDraw(spriteBatch, frameBox, pos, effects, Color.White * selfAlpha, rot, NPC.scale);
        }

        #endregion
    }
}
