using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Coralite.Content.CustomHooks
{
    public class DrawPlayerHead : HookGroup
    {
        public override void Load()
        {
            On_PlayerDrawLayers.DrawPlayer_21_Head += On_PlayerDrawLayers_DrawPlayer_21_Head;
        }

        public override void Unload()
        {
            On_PlayerDrawLayers.DrawPlayer_21_Head -= On_PlayerDrawLayers_DrawPlayer_21_Head;
        }

        public static void On_PlayerDrawLayers_DrawPlayer_21_Head(On_PlayerDrawLayers.orig_DrawPlayer_21_Head orig, ref PlayerDrawSet drawinfo)
        {
            EquipTexture e = EquipLoader.GetEquipTexture(EquipType.Head, drawinfo.drawPlayer.head);
            if (e != null && e.Item != null && e.Item is ISpecialDrawHead && drawinfo.drawPlayer.head > 0)
            {
                Vector2 helmetOffset = drawinfo.helmetOffset;
                DrawPlayer_21_Head_TheFace(ref drawinfo);
                Rectangle bodyFrame3 = drawinfo.drawPlayer.bodyFrame;

                int frameY2 = bodyFrame3.Y / 56;

                Texture2D mainTex = TextureAssets.ArmorHead[drawinfo.drawPlayer.head].Value;
                bodyFrame3 = mainTex.Frame(1, 20, 0, frameY2);
                Vector2 headVect2 = bodyFrame3.Size() / 2;

                if (drawinfo.drawPlayer.gravDir == 1f)
                {
                    bodyFrame3.Height -= 4;
                }
                else
                {
                    headVect2.Y -= 4f;
                    bodyFrame3.Height -= 4;
                }

                Color color3 = drawinfo.colorArmorHead;
                int shader3 = drawinfo.cHead;
                if (ArmorIDs.Head.Sets.UseSkinColor[drawinfo.drawPlayer.head])
                {
                    color3 = ((!drawinfo.drawPlayer.isDisplayDollOrInanimate) ? drawinfo.colorHead : drawinfo.colorDisplayDollSkin);
                    shader3 = drawinfo.skinDyePacked;
                }

                Vector2 drawPos = helmetOffset +
                    new Vector2((int)(drawinfo.Position.X -
                                (bodyFrame3.Width / 2) +
                                (drawinfo.drawPlayer.width / 2)),
                               (int)(drawinfo.Position.Y +
                               drawinfo.drawPlayer.height
                               - bodyFrame3.Height + 4f))
                    - Main.screenPosition
                    + drawinfo.drawPlayer.headPosition
                    + headVect2;
                DrawData item = new DrawData(TextureAssets.ArmorHead[drawinfo.drawPlayer.head].Value, drawPos
                    , bodyFrame3, color3, drawinfo.drawPlayer.headRotation, headVect2, 1f, drawinfo.playerEffect);
                item.shader = shader3;
                drawinfo.DrawDataCache.Add(item);

                if (drawinfo.headGlowMask != -1)
                {
                    item = new DrawData(TextureAssets.GlowMask[drawinfo.headGlowMask].Value, helmetOffset + new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, bodyFrame3, drawinfo.headGlowColor, drawinfo.drawPlayer.headRotation, headVect2, 1f, drawinfo.playerEffect);
                    item.shader = drawinfo.cHead;
                    drawinfo.DrawDataCache.Add(item);
                }

            }
            else
                orig.Invoke(ref drawinfo);
        }

        public static void DrawPlayer_21_Head_TheFace(ref PlayerDrawSet drawinfo)
        {
            /*
            bool flag = drawinfo.drawPlayer.head == 38 || drawinfo.drawPlayer.head == 135 || drawinfo.drawPlayer.head == 269;
            */
            bool flag = drawinfo.drawPlayer.head > 0 && !ArmorIDs.Head.Sets.DrawHead[drawinfo.drawPlayer.head];

            if (!flag && drawinfo.drawPlayer.faceHead > 0)
            {
                Vector2 faceHeadOffsetFromHelmet = drawinfo.drawPlayer.GetFaceHeadOffsetFromHelmet();
                DrawData item = new DrawData(TextureAssets.AccFace[drawinfo.drawPlayer.faceHead].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect + faceHeadOffsetFromHelmet, drawinfo.drawPlayer.bodyFrame, drawinfo.colorArmorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                item.shader = drawinfo.cFaceHead;
                drawinfo.DrawDataCache.Add(item);
                if (drawinfo.drawPlayer.face <= 0 || !ArmorIDs.Face.Sets.DrawInFaceUnderHairLayer[drawinfo.drawPlayer.face])
                    return;

                float num = 0f;
                if (drawinfo.drawPlayer.face == 5)
                {
                    int faceHead = drawinfo.drawPlayer.faceHead;
                    if ((uint)(faceHead - 10) <= 3u)
                        num = 2 * drawinfo.drawPlayer.direction;
                }

                item = new DrawData(TextureAssets.AccFace[drawinfo.drawPlayer.face].Value, new Vector2((float)(int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)) + num, (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorArmorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                item.shader = drawinfo.cFace;
                drawinfo.DrawDataCache.Add(item);
            }
            else if (!drawinfo.drawPlayer.invis && !flag)
            {
                DrawData drawData = new DrawData(TextureAssets.Players[drawinfo.skinVar, 0].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                drawData.shader = drawinfo.skinDyePacked;
                DrawData item = drawData;
                drawinfo.DrawDataCache.Add(item);
                item = new DrawData(TextureAssets.Players[drawinfo.skinVar, 1].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorEyeWhites, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                drawinfo.DrawDataCache.Add(item);
                item = new DrawData(TextureAssets.Players[drawinfo.skinVar, 2].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorEyes, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                drawinfo.DrawDataCache.Add(item);
                Asset<Texture2D> asset = TextureAssets.Players[drawinfo.skinVar, 15];
                if (asset.IsLoaded)
                {
                    Vector2 vector = Main.OffsetsPlayerHeadgear[drawinfo.drawPlayer.bodyFrame.Y / drawinfo.drawPlayer.bodyFrame.Height];
                    vector.Y -= 2f;
                    vector *= (float)(-drawinfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt());
                    Rectangle value = asset.Frame(1, 3, 0, drawinfo.drawPlayer.eyeHelper.EyeFrameToShow);
                    drawData = new DrawData(asset.Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect + vector, value, drawinfo.colorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                    drawData.shader = drawinfo.skinDyePacked;
                    item = drawData;
                    drawinfo.DrawDataCache.Add(item);
                }

                if (drawinfo.drawPlayer.yoraiz0rDarkness)
                {
                    drawData = new DrawData(TextureAssets.Extra[67].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                    drawData.shader = drawinfo.skinDyePacked;
                    item = drawData;
                    drawinfo.DrawDataCache.Add(item);
                }

                if (drawinfo.drawPlayer.face > 0 && ArmorIDs.Face.Sets.DrawInFaceUnderHairLayer[drawinfo.drawPlayer.face])
                {
                    item = new DrawData(TextureAssets.AccFace[drawinfo.drawPlayer.face].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorArmorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                    item.shader = drawinfo.cFace;
                    drawinfo.DrawDataCache.Add(item);
                }
            }
        }
    }

    public interface ISpecialDrawHead
    {

    }
}
