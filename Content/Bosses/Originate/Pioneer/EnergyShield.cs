using Coralite.Content.Dusts;
using Coralite.Core;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Terraria.GameContent;
using Terraria.ID;
using Terraria;

namespace Coralite.Content.Bosses.Originate.Pioneer
{
    //public class EnergyShield : ModNPC
    //{
    //    public override string Texture => AssetDirectory.Pioneer;

    //    public override void SetStaticDefaults()
    //    {
    //    }
    //    public override void SetDefaults()
    //    {
    //        NPC.npcSlots = 1f;
    //        NPC.width = 144;
    //        NPC.height = 192;
    //        NPC.aiStyle = -1;
    //        NPC.damage = 10;
    //        NPC.defense = 5;
    //        NPC.lifeMax = 500;
    //        NPC.HitSound = SoundID.NPCHit53;
    //        NPC.DeathSound = SoundID.NPCDeath56;
    //        NPC.noGravity = true;
    //        NPC.noTileCollide = true;
    //        NPC.hide = true;
    //        NPC.knockBackResist = 0f;
    //        NPC.value = 0f;
    //        NPC.alpha = 0;
    //        base.SetDefaults();
    //    }
    //    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    //    {
    //        NPC.lifeMax = NPC.lifeMax.IntMult(bossAdjustment * 0.9f * balance);
    //    }
    //    public override bool CanHitPlayer(Player target, ref int cooldownSlot)
    //    {
    //        return false;
    //    }
    //    public override void DrawBehind(int index)
    //    {
    //        Main.instance.DrawCacheNPCProjectiles.Add(index);
    //        base.DrawBehind(index);
    //    }
    //    private Color GetColor()
    //    {
    //        Color color = new(35,160,200);
    //        color *= Math.Clamp(NPC.ai[3] / 30f, 0f, 1f);
    //        return color;
    //    }
    //    public override void HitEffect(NPC.HitInfo hit)
    //    {
    //        if (NPC.life > 0)
    //        {
    //            for (int i = 0; i < hit.Damage * 0.25f; i++)
    //            {
    //                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowDust_Circle>(), hit.HitDirection * 10f, -1f, 100, GetColor(), 0.75f);
    //                dust.noGravity = true;
    //            }
    //            return;
    //        }
    //        base.HitEffect(hit);
    //    }
    //    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    //    {
    //        float Size = NPC.height * 0.2f;
    //        float Thickness = NPC.height * 0.1f;

    //        List<VertexInfo2> Vertex = [];
    //        float Max = 60f;
    //        for (int i = 0; i <= Max; i++)
    //        {
    //            Vector2 Vec = Vector2.UnitX.RotatedBy(MathHelper.TwoPi / (int)Max * i) * Size;
    //            Vec *= new Vector2(0.75f, 1f);
    //            Vector2 Pos = NPC.Center + Vec - screenPos;
    //            Vector2 Rot = Vec.SafeNormalize(Vector2.UnitX) * Thickness;
    //            Color color = GetColor();
    //            Vertex.Add(new VertexInfo2(Pos + Vec + Rot, new Vector3(i / Max, 0f, 1f), color));
    //            Vertex.Add(new VertexInfo2(Pos + Vec - Rot, new Vector3(i / Max, 1f, 1f), color));
    //        }

    //        List<VertexInfo2> Vertex2 = [];
    //        Max = Math.Max(NPC.ai[1] * 0.5f, 20f);
    //        float Mult = Math.Clamp(NPC.ai[1] / 120f, 0f, 1f);
    //        float Mult2 = 1f;
    //        if (NPC.ai[1] > 120f) Mult2 = 1f - (NPC.ai[1] - 120f) / 60f;
    //        for (int i = 0; i <= Max; i++)
    //        {
    //            Vector2 Vec = Vector2.UnitX.RotatedBy(MathHelper.TwoPi / (int)Max * i) * Size * Mult;
    //            Vec *= new Vector2(0.75f, 1f);
    //            Vector2 Pos = NPC.Center + Vec - screenPos;
    //            Vector2 Rot = Vec.SafeNormalize(Vector2.UnitX) * Thickness * Mult;
    //            Color color = GetColor();
    //            color *= 0.5f * Mult2;
    //            Vertex2.Add(new VertexInfo2(Pos + Vec + Rot, new Vector3(i / Max, 0f, 1f), color));
    //            Vertex2.Add(new VertexInfo2(Pos + Vec - Rot, new Vector3(i / Max, 1f, 1f), color));
    //        }

    //        List<VertexInfo2> Vertex3 = [];
    //        Max = Math.Max(NPC.ai[2] * 0.5f, 20f);
    //        Mult = Math.Clamp(NPC.ai[2] / 120f, 0f, 1f);
    //        Mult2 = 1f;
    //        if (NPC.ai[2] > 120f) Mult2 = 1f - (NPC.ai[2] - 120f) / 60f;
    //        for (int i = 0; i <= Max; i++)
    //        {
    //            Vector2 Vec = Vector2.UnitX.RotatedBy(MathHelper.TwoPi / (int)Max * i) * Size * Mult;
    //            Vec *= new Vector2(0.75f, 1f);
    //            Vector2 Pos = NPC.Center + Vec - screenPos;
    //            Vector2 Rot = Vec.SafeNormalize(Vector2.UnitX) * Thickness * Mult;
    //            Color color = GetColor();
    //            color *= 0.5f * Mult2;
    //            Vertex3.Add(new VertexInfo2(Pos + Vec + Rot, new Vector3(i / Max, 0f, 1f), color));
    //            Vertex3.Add(new VertexInfo2(Pos + Vec - Rot, new Vector3(i / Max, 1f, 1f), color));
    //        }
    //        Main.spriteBatch.End();
    //        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp,
    //            DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
    //        Main.graphics.GraphicsDevice.Textures[0] = TextureAssets.Npc[NPC.type].Value;
    //        if (Vertex.Count >= 3)
    //        {
    //            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertex.ToArray(), 0, Vertex.Count - 2);
    //            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertex2.ToArray(), 0, Vertex2.Count - 2);
    //            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertex3.ToArray(), 0, Vertex3.Count - 2);
    //        }
    //        Main.spriteBatch.End();
    //        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
    //            DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
    //        return false;
    //    }
    //    public override void AI()
    //    {
    //        NPC.ai[1] += 1f;
    //        if (NPC.ai[1] > 180f)
    //        {
    //            NPC.ai[1] = 0f;
    //            NPC.netUpdate = true;
    //        }
    //        if (NPC.ai[3] > 90f)
    //        {
    //            NPC.ai[2] += 1f;
    //            if (NPC.ai[2] > 180f) NPC.ai[2] = 0f;
    //        }
    //        if (NPC.ai[3] < 120f)
    //        {
    //            NPC.ai[3] += 1f;
    //        }

    //        NPC Body = Main.npc[(int)NPC.ai[0]];
    //        if(Body.active && Body.type == ModContent.NPCType<Pioneer>())
    //        {
    //            NPC.Center = Body.Center;
    //            NPC.dontTakeDamage = Body.ai[0] == 0f;
    //        }
    //        else
    //        {
    //            NPC.life = 0;
    //            NPC.lifeMax = 0;
    //            NPC.active = false;
    //            NPC.netUpdate = true;
    //            return;
    //        }
    //        base.AI();
    //    }
    //}
}