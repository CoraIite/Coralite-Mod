using Coralite.Content.ModPlayers;
using Coralite.Core.Loaders;
using Coralite.Core.Systems.DotSystem.BasicDotTags;
using Coralite.Core.Systems.DotSystem.DotAccessories;
using Coralite.Helpers;
using InnoVault.PRT;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace Coralite.Core.Systems.DotSystem.BasicDots
{
    public abstract class BasicSour : Dot
    {
        public static LocalizedText TriggerInterval { get; private set; }
        
        public override int BaseMaxDotLayer => 6;
        public override DamageClass DamageType => DamageClass.Magic;

        /// <summary>
        /// 基础的造成伤害的间隔
        /// </summary>
        public virtual int BasicDamageInterval => 60;
        public int DamageInterval => Math.Clamp((int)(BasicDamageInterval * DamageIntervalMultiplier), 1, 60 * 10);

        /// <summary>
        /// 伤害间隔倍率，实际间隔为基础间隔乘以该值
        /// </summary>
        public float DamageIntervalMultiplier = 1f;

        protected int Timer { get; set; }

        public override void Load()
        {
            base.Load();
            TriggerInterval = Language.GetText(DotSystem.TipsContext + "TriggerInterval");
        }

        public override int[] LoadTags() => [ModContent.GetInstance<SourTag>().Type];

        public override void OnAdd(DotInfo info)
        {
            DamageIntervalMultiplier = 1f;

            HashSet<int> players = [];
            foreach (var dotInfo in DotInfos)
            {
                if (dotInfo.player < 0 || dotInfo.player >= Main.maxPlayers
                    || !players.Add(dotInfo.player))
                    continue;

                Player player = Main.player[dotInfo.player];
                ApplyPlayerBonuses(player);
            }
        }

        public override void ApplyPlayerBonuses(Player player)
        {
            base.ApplyPlayerBonuses(player);
            if (player.TryGetModPlayer(out CoralitePlayer cp))
                foreach (var effect in cp.EffectHandlers)
                    if (effect is ISourBonus bonus)
                        bonus.ModifySour(this);
        }

        public override List<string> GetDotTipProperties()
        {
            List<string> properties = base.GetDotTipProperties();
            properties.Add(TriggerInterval.Format((DamageInterval / 60f).ToString("0.##")));
            return properties;
        }

        public override void Update(NPC npc)
        {
            if (CurrentLayer <= 0)
            {
                RemoveDot();
                return;
            }

            Timer++;
            if (Timer >= DamageInterval)
            {
                Timer = 0;
                ApplyDot(npc);
            }
        }

        public override void NewDamageText(NPC target,NPC.HitInfo hit, DotInfo info)
        {
            Color color =Color.White;
            Color critcolor =Color.White;

            if (DotSystem.DotTags != null && Type >= 0 && Type < DotSystem.DotTags.Length
                && DotSystem.DotTags[Type] is { Length: > 0 } tags)
            {
                color = DotLoader.GetDotTag(tags[0])?.TagColor ?? Color.White;
                critcolor = DotLoader.GetDotTag(tags[0])?.CritColor ?? Color.White;
            }

            var p = PRTLoader.NewParticle<SourDamageText>(Main.rand.NextVector2FromRectangle(target.getRect()), new Vector2(0, -3).RotateByRandom(-0.2f, 0.2f), hit.Crit ? critcolor : color,0.01f);

            p.damage = hit.Damage;
            p.Rotation = Main.rand.NextFloat(-0.1f, 0.1f);
        }
    }

    public class SourDamageText : Particle
    {
        public int damage;
        public int contiuneTime;

        public override void SetProperty()
        {
            contiuneTime = Main.rand.Next(50, 70);
        }

        public override void AI()
        {
            Opacity++;

            if (Opacity <= 10)
            {
                Scale = Helper.SqrtEase(Opacity / 10f);
            }
            else if (Opacity <= 10 + 8)
            {
                Scale = 1 - 0.7f * Helper.X2Ease((Opacity - 10f) / 8);
            }
            else if (Opacity < 10 + 8 + contiuneTime)
            {
                Scale = Helper.Lerp(Scale, 1, 0.1f);
                Velocity *= 0.9f;
            }
            else
            {
                Color *= 0.93f;
                Scale *= 0.93f;
                if (Color.A<5)
                {
                    active = false;
                }
            }

            if (Opacity < 10 + 8 + contiuneTime && Main.rand.NextBool(5))
            {
                Vector2 size = Helper.GetStringSize(damage.ToString(), Vector2.One * Scale);
                Vector2 dir = Rotation.ToRotationVector2();
                Vector2 normal = (Rotation + MathHelper.PiOver2).ToRotationVector2();

                Dust d = Dust.NewDustPerfect(Position + normal * size.Y / 2 * Main.rand.NextFloat(0.95f, 1.05f) + dir * size.X / 2 * Main.rand.NextFloat(-1, 1), DustID.YellowTorch, -normal * Main.rand.NextFloat(1, 3), Scale: Main.rand.NextFloat(1, 1.9f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            DynamicSpriteFont value = FontAssets.CombatText[0].Value;
            Vector2 vector = value.MeasureString(damage.ToString());

            Color c = Color.Lerp(Color, Color * 0.7f, 0.5f + 0.5f * MathF.Sin((int)Main.timeForVisualEffects * 0.2f));

            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, value, damage.ToString(), Position - Main.screenPosition+new Vector2(0,4), c, (Color * 0.2f) with { A = 180 }, Rotation, new Vector2(0.5f, 0.5f) * vector, Vector2.One * Scale, -1f, 1.5f* Scale);

            return false;
        }
    }
}
