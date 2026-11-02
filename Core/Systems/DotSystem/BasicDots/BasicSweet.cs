using Coralite.Core.Loaders;
using Coralite.Core.Systems.DotSystem.BasicDotTags;
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
    public abstract class BasicSweet : Dot
    {
        public static LocalizedText OtherDotDamageBonusText { get; private set; }

        public override int BaseMaxDotLayer => 3;
        public override DamageClass DamageType => DamageClass.Magic;

        /// <summary>
        /// 其他dot消耗时的基础伤害加成
        /// </summary>
        protected virtual float BasicOtherDotDamageBonus => 0.1f;

        /// <summary>
        /// 其他dot伤害加成的倍率
        /// </summary>
        public float ExtraOtherDotDamageBonus= 0;

        public float OtherDotDamageBonus => BasicOtherDotDamageBonus + ExtraOtherDotDamageBonus;

        private bool _applyingDot;

        protected int Timer { get; set; }

        /// <summary>
        /// 本次伤害是否由5秒计时自动触发
        /// </summary>
        public bool AutoHit;

        public override void Load()
        {
            base.Load();
            OtherDotDamageBonusText = Language.GetOrRegister(DotSystem.TipsContext + "OtherDotDamageBonusText");
        }

        public override int[] LoadTags() => [ModContent.GetInstance<SweetTag>().Type];

        public override List<string> GetDotTipProperties()
        {
            List<string> properties = base.GetDotTipProperties();
            properties.Add(OtherDotDamageBonusText.Format((OtherDotDamageBonus * 100).ToString("0.##")));
            return properties;
        }

        public override void OnAdd(DotInfo info)
        {
            ExtraOtherDotDamageBonus = 0f;

            HashSet<int> players = [];
            foreach (var dotInfo in DotInfos)
            {
                if (!players.Add(dotInfo.player))
                    continue;

                Player player = Main.player[dotInfo.player];
                ApplyPlayerBonuses(player);
            }
        }

        public override void Update(NPC npc)
        {
            if (CurrentLayer <= 0)
            {
                RemoveDot();
                return;
            }

            Timer++;
            if (Timer >= 60 * 8)
            {
                Timer = 0;
                AutoHit = true;
                ApplyDot(npc);
                AutoHit = false;
            }
        }

        public override void ApplyOtherDots(NPC npc, ref NPC.HitModifiers hitModifyer, DotInfo dotInfo)
        {
            // 结算期间不接受其他甜味dot的反向触发，避免递归消耗
            _applyingDot = true;
            base.ApplyOtherDots(npc, ref hitModifyer, dotInfo);
            _applyingDot = false;
        }

        public override void OnOtherDotApply(Dot dot, NPC npc, ref NPC.HitModifiers hitModifyer, DotInfo dotInfo)
        {
            if (_applyingDot || NotActive != 0 || CurrentLayer <= 0)
                return;

            hitModifyer.FinalDamage += OtherDotDamageBonus;
            ApplyDot(npc);
        }

        public override void ModifyDamage(NPC target, ref NPC.HitModifiers modifyer, DotInfo info)
        {
            if (AutoHit)
            {
                AutoHit = false;
                modifyer.FinalDamage *= 0.5f;
            }
        }

        public override void NewDamageText(NPC target, NPC.HitInfo hit, DotInfo info)
        {
            Color color = Color.White;
            Color critColor = Color.White;
            if (DotSystem.DotTags != null && Type >= 0 && Type < DotSystem.DotTags.Length
                && DotSystem.DotTags[Type] is { Length: > 0 } tags)
            {
                color = DotLoader.GetDotTag(tags[0])?.TagColor ?? Color.White;
                critColor = DotLoader.GetDotTag(tags[0])?.CritColor ?? Color.White;
            }

            Vector2 position = Main.rand.NextVector2FromRectangle(target.getRect());
            Vector2 offset = position - target.Center;
            float length = offset.Length();
            float speed = ((target.width + target.height) / 2f - length) / 20f
                + Main.rand.NextFloat(0.1f, 0.5f);
            Vector2 velocity = offset.SafeNormalize(Vector2.UnitY) * speed;
            var p = PRTLoader.NewParticle<SweetDamageText>(position, velocity, hit.Crit ? critColor : color, 0.01f);

            p.damage = hit.Damage;
            p.npcIndex = target.whoAmI;
            p.Rotation = Main.rand.NextFloat(-0.1f, 0.1f);
        }
    }

    public class SweetDamageText : Particle
    {
        private const int AppearTime = 15;

        public int damage;
        public int contiuneTime;
        public int npcIndex = -1;

        private Vector2 _initialVelocity;

        public override void SetProperty()
        {
            contiuneTime = Main.rand.Next(50, 71);
            npcIndex = -1;
            Opacity = 0;
            _initialVelocity = Vector2.Zero;
        }

        public override void AI()
        {
            if (Opacity == 0)
                _initialVelocity = Velocity;

            Opacity++;
            if (npcIndex >= 0 && npcIndex < Main.maxNPCs)
            {
                NPC npc = Main.npc[npcIndex];
                if (npc.active)
                    Position += npc.oldPosition - npc.position;
                else
                    npcIndex = -1;
            }

            if (Opacity <= AppearTime)
            {
                Scale = Helper.SqrtEase(Opacity / AppearTime);
                Velocity *= 0.96f;
            }
            else if (Opacity < AppearTime + contiuneTime)
            {
                Velocity *= 0.94f;

                Scale = 1f;
            }
            else if (Opacity == AppearTime + contiuneTime)
            {
                Velocity = -_initialVelocity;
            }
            else
            {
                Velocity *= 0.97f;
                Scale *= 0.9f;
                if (Scale < 0.05f)
                {
                    active = false;
                    return;
                }
            }

            if (Opacity < AppearTime + contiuneTime && Main.rand.NextBool(5))
            {
                Vector2 size = FontAssets.CombatText[0].Value.MeasureString(damage.ToString()) * Scale;
                Vector2 offset = Main.rand.NextVector2Circular(size.X / 3f, size.Y / 3f).RotatedBy(Rotation);
                Dust d = Dust.NewDustPerfect(Position + offset, DustID.PinkFairy,
                    Helper.NextVec2Dir(1f, 3f), newColor: new Color(40, 255, 0, 50), Scale: Main.rand.NextFloat(0.6f, 1.25f));
                d.noGravity = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            DynamicSpriteFont font = FontAssets.CombatText[0].Value;
            string text = damage.ToString();
            Vector2 size = font.MeasureString(text);
            float fade = MathHelper.Clamp(Opacity / AppearTime, 0f, 1f);
            Color color = Color.Lerp(Color, Color * 0.7f,
                0.5f + 0.5f * MathF.Sin((int)Main.timeForVisualEffects * 0.2f));

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text, Position - Main.screenPosition,
                color * fade, ((Color * 0.2f) with { A = 180 }) * fade,
                Rotation, size * 0.5f, Vector2.One * Scale, -1f, 1.5f * Scale);
            return false;
        }
    }
}
