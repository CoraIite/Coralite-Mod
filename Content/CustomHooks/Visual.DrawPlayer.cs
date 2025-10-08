using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Content.CustomHooks
{
    public class DrawPlayer : HookGroup
    {
        public override void Load()
        {
            On_PlayerDrawLayers.DrawPlayer_21_Head += On_PlayerDrawLayers_DrawPlayer_21_Head;
            //On_PlayerDrawLayers.DrawPlayer_08_Backpacks += On_PlayerDrawLayers_DrawPlayer_08_Backpacks; 
            On_PlayerDrawLayers.DrawPlayer_10_BackAcc += On_PlayerDrawLayers_DrawPlayer_10_BackAcc;
            //On_PlayerDrawLayers.DrawSittingLegs += On_PlayerDrawLayers_DrawSittingLegs;
        }

        public override void Unload()
        {
            On_PlayerDrawLayers.DrawPlayer_21_Head -= On_PlayerDrawLayers_DrawPlayer_21_Head;
            //On_PlayerDrawLayers.DrawPlayer_08_Backpacks -= On_PlayerDrawLayers_DrawPlayer_08_Backpacks; 
            On_PlayerDrawLayers.DrawPlayer_10_BackAcc -= On_PlayerDrawLayers_DrawPlayer_10_BackAcc;

            //On_PlayerDrawLayers.DrawSittingLegs -= On_PlayerDrawLayers_DrawSittingLegs;
        }

        #region 头部

        public static void On_PlayerDrawLayers_DrawPlayer_21_Head(On_PlayerDrawLayers.orig_DrawPlayer_21_Head orig, ref PlayerDrawSet drawinfo)
        {
            EquipTexture e = EquipLoader.GetEquipTexture(EquipType.Head, drawinfo.drawPlayer.head);
            if (e != null && e.Item != null && e.Item is ISpecialDrawHead specialHead && drawinfo.drawPlayer.head > 0)
            {
                Vector2 helmetOffset = drawinfo.helmetOffset;
                DrawPlayer_21_Head_TheFace(ref drawinfo);
                Rectangle bodyFrame3 = drawinfo.drawPlayer.bodyFrame;

                Vector2 vector = new((-drawinfo.drawPlayer.bodyFrame.Width / 2) + (drawinfo.drawPlayer.width / 2), drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4);
                Vector2 position = (drawinfo.Position - Main.screenPosition + vector).Floor() + drawinfo.drawPlayer.headPosition + drawinfo.headVect;

                if (drawinfo.fullHair)
                {
                    //Color color = drawinfo.colorArmorHead;
                    //int shader = drawinfo.cHead;
                    //if (ArmorIDs.Head.Sets.UseSkinColor[drawinfo.drawPlayer.head])
                    //{
                    //    color = ((!drawinfo.drawPlayer.isDisplayDollOrInanimate) ? drawinfo.colorHead : drawinfo.colorDisplayDollSkin);
                    //    shader = drawinfo.skinDyePacked;
                    //}

                    //DrawData item2 = new DrawData(TextureAssets.ArmorHead[drawinfo.drawPlayer.head].Value, helmetOffset + new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (float)(drawinfo.drawPlayer.bodyFrame.Width / 2) + (float)(drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + (float)drawinfo.drawPlayer.height - (float)drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, color, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                    //item2.shader = shader;
                    //drawinfo.DrawDataCache.Add(item2);
                    if (!drawinfo.drawPlayer.invis)
                    {
                        DrawData item2 = new(TextureAssets.PlayerHair[drawinfo.drawPlayer.hair].Value, position, drawinfo.hairFrontFrame, drawinfo.colorHair, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                        item2.shader = drawinfo.hairDyePacked;
                        drawinfo.DrawDataCache.Add(item2);
                    }
                }

                if (drawinfo.hatHair && !drawinfo.drawPlayer.invis)
                {
                    DrawData item2 = new(TextureAssets.PlayerHairAlt[drawinfo.drawPlayer.hair].Value, position, drawinfo.hairFrontFrame, drawinfo.colorHair, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                    item2.shader = drawinfo.hairDyePacked;
                    drawinfo.DrawDataCache.Add(item2);
                }

                int frameY2 = bodyFrame3.Y / 56;

                Texture2D mainTex = TextureAssets.ArmorHead[drawinfo.drawPlayer.head].Value;
                bodyFrame3 = mainTex.Frame(1, 20, 0, frameY2);
                Vector2 headVect2 = new(bodyFrame3.Width / 2, bodyFrame3.Height / 2); // bodyFrame3.Size() / 2;
                if (bodyFrame3.Width % 2 != 0 && drawinfo.drawPlayer.direction < 0)
                {
                    helmetOffset.X -= 1;
                }
                if (drawinfo.drawPlayer.gravDir == 1f)
                {
                    bodyFrame3.Height -= 4;
                }
                else
                {
                    headVect2.Y -= 4f;
                    bodyFrame3.Height -= 4;
                    helmetOffset.Y += bodyFrame3.Height - 56;
                }

                Color color3 = drawinfo.colorArmorHead;
                int shader3 = drawinfo.cHead;
                if (ArmorIDs.Head.Sets.UseSkinColor[drawinfo.drawPlayer.head])
                {
                    color3 = (!drawinfo.drawPlayer.isDisplayDollOrInanimate) ? drawinfo.colorHead : drawinfo.colorDisplayDollSkin;
                    shader3 = drawinfo.skinDyePacked;
                }

                Vector2 exOffset = specialHead.ExtraOffset;
                exOffset.X *= drawinfo.drawPlayer.direction;

                Vector2 drawPos = helmetOffset +
                    new Vector2((int)(drawinfo.Position.X -
                                (bodyFrame3.Width / 2) +
                                (drawinfo.drawPlayer.width / 2)),
                               (int)(drawinfo.Position.Y +
                               drawinfo.drawPlayer.height
                               - bodyFrame3.Height + 4f))
                    - Main.screenPosition
                    + drawinfo.drawPlayer.headPosition
                    + headVect2
                    + exOffset;

                DrawData item = new(TextureAssets.ArmorHead[drawinfo.drawPlayer.head].Value, drawPos
                    , bodyFrame3, color3, drawinfo.drawPlayer.headRotation, headVect2, 1f, drawinfo.playerEffect);
                item.shader = shader3;
                drawinfo.DrawDataCache.Add(item);

                if (drawinfo.headGlowMask != -1)
                {
                    item = new DrawData(TextureAssets.GlowMask[drawinfo.headGlowMask].Value, helmetOffset + new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (drawinfo.drawPlayer.bodyFrame.Width / 2) + (drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, bodyFrame3, drawinfo.headGlowColor, drawinfo.drawPlayer.headRotation, headVect2, 1f, drawinfo.playerEffect);
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
                DrawData item = new(TextureAssets.AccFace[drawinfo.drawPlayer.faceHead].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (drawinfo.drawPlayer.bodyFrame.Width / 2) + (drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect + faceHeadOffsetFromHelmet, drawinfo.drawPlayer.bodyFrame, drawinfo.colorArmorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
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

                item = new DrawData(TextureAssets.AccFace[drawinfo.drawPlayer.face].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (drawinfo.drawPlayer.bodyFrame.Width / 2) + (drawinfo.drawPlayer.width / 2)) + num, (int)(drawinfo.Position.Y - Main.screenPosition.Y + drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorArmorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                item.shader = drawinfo.cFace;
                drawinfo.DrawDataCache.Add(item);
            }
            else if (!drawinfo.drawPlayer.invis && !flag)
            {
                DrawData drawData = new(TextureAssets.Players[drawinfo.skinVar, 0].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (drawinfo.drawPlayer.bodyFrame.Width / 2) + (drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                drawData.shader = drawinfo.skinDyePacked;
                DrawData item = drawData;
                drawinfo.DrawDataCache.Add(item);
                item = new DrawData(TextureAssets.Players[drawinfo.skinVar, 1].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (drawinfo.drawPlayer.bodyFrame.Width / 2) + (drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorEyeWhites, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                drawinfo.DrawDataCache.Add(item);
                item = new DrawData(TextureAssets.Players[drawinfo.skinVar, 2].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (drawinfo.drawPlayer.bodyFrame.Width / 2) + (drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorEyes, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                drawinfo.DrawDataCache.Add(item);
                Asset<Texture2D> asset = TextureAssets.Players[drawinfo.skinVar, 15];
                if (asset.IsLoaded)
                {
                    Vector2 vector = Main.OffsetsPlayerHeadgear[drawinfo.drawPlayer.bodyFrame.Y / drawinfo.drawPlayer.bodyFrame.Height];
                    vector.Y -= 2f;
                    vector *= -drawinfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
                    Rectangle value = asset.Frame(1, 3, 0, (int)drawinfo.drawPlayer.eyeHelper.CurrentEyeFrame);
                    drawData = new DrawData(asset.Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (drawinfo.drawPlayer.bodyFrame.Width / 2) + (drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect + vector, value, drawinfo.colorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                    drawData.shader = drawinfo.skinDyePacked;
                    item = drawData;
                    drawinfo.DrawDataCache.Add(item);
                }

                if (drawinfo.drawPlayer.yoraiz0rDarkness)
                {
                    drawData = new DrawData(TextureAssets.Extra[ExtrasID.Yoraiz0rDarkness].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (drawinfo.drawPlayer.bodyFrame.Width / 2) + (drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                    drawData.shader = drawinfo.skinDyePacked;
                    item = drawData;
                    drawinfo.DrawDataCache.Add(item);
                }

                if (drawinfo.drawPlayer.face > 0 && ArmorIDs.Face.Sets.DrawInFaceUnderHairLayer[drawinfo.drawPlayer.face])
                {
                    item = new DrawData(TextureAssets.AccFace[drawinfo.drawPlayer.face].Value, new Vector2((int)(drawinfo.Position.X - Main.screenPosition.X - (drawinfo.drawPlayer.bodyFrame.Width / 2) + (drawinfo.drawPlayer.width / 2)), (int)(drawinfo.Position.Y - Main.screenPosition.Y + drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height + 4f)) + drawinfo.drawPlayer.headPosition + drawinfo.headVect, drawinfo.drawPlayer.bodyFrame, drawinfo.colorArmorHead, drawinfo.drawPlayer.headRotation, drawinfo.headVect, 1f, drawinfo.playerEffect);
                    item.shader = drawinfo.cFace;
                    drawinfo.DrawDataCache.Add(item);
                }
            }
        }

        #endregion

        #region 背包

        private void On_PlayerDrawLayers_DrawPlayer_10_BackAcc(On_PlayerDrawLayers.orig_DrawPlayer_10_BackAcc orig, ref PlayerDrawSet drawinfo)
        {
            EquipTexture e = EquipLoader.GetEquipTexture(EquipType.Back, drawinfo.drawPlayer.back);
            if (e != null && e.Item != null && e.Item is ISpecialDrawBackpacks specialBackpack && drawinfo.drawPlayer.back > 0)
            {
                //if (drawinfo.drawPlayer.mount.Active)
                //    return;

                int shader;
                int num2 = drawinfo.drawPlayer.back;
                //float num3 = -4f;
                //float num4 = -8f;

                Texture2D tex = TextureAssets.AccBack[num2].Value;

                int yFrame = drawinfo.drawPlayer.bodyFrame.Y / 56;

                Rectangle frame = tex.Frame(1, 20, 0, yFrame);//  new Rectangle(0, drawinfo.drawPlayer.bodyFrame.Y,tex.Width, drawinfo.drawPlayer.bodyFrame.Height);
                Vector2 vector3 = new(0f, 8f);
                if (frame.Width / 2 % 2 != 0)
                {
                    vector3.X -= drawinfo.drawPlayer.direction;
                }

                Vector2 exOffset = specialBackpack.ExtraOffset;
                exOffset.X *= drawinfo.drawPlayer.direction;
                Vector2 vec5 =
                    drawinfo.Position
                    - Main.screenPosition +
                    drawinfo.drawPlayer.bodyPosition
                    + new Vector2(drawinfo.drawPlayer.width / 2, drawinfo.drawPlayer.height - (frame.Height / 2))
                    + new Vector2(0f, -4f)
                    + vector3
                    + exOffset;
                vec5 = vec5.Floor();
                //Vector2 vec6 = drawinfo.Position - Main.screenPosition + new Vector2(drawinfo.drawPlayer.width / 2, drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height / 2) + new Vector2((-9f + num3) * (float)drawinfo.drawPlayer.direction, (2f + num4) * drawinfo.drawPlayer.gravDir) + vector3;
                //vec6 = vec6.Floor();
                shader = drawinfo.cBack;
                DrawData item = new(TextureAssets.AccBack[num2].Value, vec5, frame,
                    drawinfo.colorArmorBody, drawinfo.drawPlayer.bodyRotation, new Vector2(frame.Width / 2, frame.Height / 2), 1f, drawinfo.playerEffect);
                item.shader = shader;
                drawinfo.DrawDataCache.Add(item);
            }
            else
                orig.Invoke(ref drawinfo);
        }

        //private void On_PlayerDrawLayers_DrawPlayer_08_Backpacks(On_PlayerDrawLayers.orig_DrawPlayer_08_Backpacks orig, ref PlayerDrawSet drawinfo)
        //{
        //    EquipTexture e = EquipLoader.GetEquipTexture(EquipType.Back, drawinfo.drawPlayer.back);
        //    if (e != null && e.Item != null && e.Item is ISpecialDrawBackPacks && drawinfo.drawPlayer.back > 0)
        //    {
        //        if (drawinfo.drawPlayer.mount.Active)
        //            return;

        //        int shader;
        //        int num2 = drawinfo.drawPlayer.back;
        //        float num3 = -4f;
        //        float num4 = -8f;

        //        Vector2 vector3 = new Vector2(0f, 8f);
        //        Vector2 vec5 = drawinfo.Position - Main.screenPosition + drawinfo.drawPlayer.bodyPosition + new Vector2(drawinfo.drawPlayer.width / 2, drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height / 2) + new Vector2(0f, -4f) + vector3;
        //        vec5 = vec5.Floor();
        //        Vector2 vec6 = drawinfo.Position - Main.screenPosition + new Vector2(drawinfo.drawPlayer.width / 2, drawinfo.drawPlayer.height - drawinfo.drawPlayer.bodyFrame.Height / 2) + new Vector2((-9f + num3) * (float)drawinfo.drawPlayer.direction, (2f + num4) * drawinfo.drawPlayer.gravDir) + vector3;
        //        vec6 = vec6.Floor();

        //        shader = drawinfo.cBody;
        //        DrawData item = new DrawData(TextureAssets.AccBack[num2].Value, vec5
        //            , new Rectangle(0, drawinfo.drawPlayer.bodyFrame.Y, TextureAssets.AccBack[num2].Width(), drawinfo.drawPlayer.bodyFrame.Height),
        //            drawinfo.colorArmorBody, drawinfo.drawPlayer.bodyRotation, new Vector2(TextureAssets.AccBack[num2].Width()/2, drawinfo.bodyVect.Y), 1f, drawinfo.playerEffect);
        //        item.shader = shader;
        //        drawinfo.DrawDataCache.Add(item);
        //    }
        //    else
        //        orig.Invoke(ref  drawinfo);
        //}

        #endregion


        #region 腿部

        public void On_PlayerDrawLayers_DrawSittingLegs(On_PlayerDrawLayers.orig_DrawSittingLegs orig, ref PlayerDrawSet drawinfo, Texture2D textureToDraw, Color matchingColor, int shaderIndex, bool glowmask)
        {
            EquipTexture e = EquipLoader.GetEquipTexture(EquipType.Head, drawinfo.drawPlayer.legs);
            if (e != null && e.Item != null && e.Item is ISpecialDrawHead && drawinfo.drawPlayer.legs > 0)
            {
                Vector2 legsOffset = drawinfo.legsOffset;

                Rectangle legFrame = drawinfo.drawPlayer.legFrame;

                int frameY2 = legFrame.Y / 56;
                legFrame = textureToDraw.Frame(1, 20, 0, frameY2);

                Vector2 vector =
                    new Vector2(
                        (int)(drawinfo.Position.X
                        - (legFrame.Width / 2)
                        + (drawinfo.drawPlayer.width / 2))

                        , (int)(drawinfo.Position.Y
                        + drawinfo.drawPlayer.height
                        - legFrame.Height
                        + 4f))

                    - Main.screenPosition
                    + drawinfo.drawPlayer.legPosition
                    + drawinfo.legVect;

                vector.Y -= 2f;
                vector.Y += drawinfo.seatYOffset;
                vector += legsOffset;
                int num = 2;
                int num2 = 42;
                int num3 = 2;
                int num4 = 2;
                int num5 = 0;
                int num6 = 0;
                int num7 = 0;
                if (drawinfo.drawPlayer.wearsRobe)
                {
                    num = 0;
                    num4 = 0;
                    num2 = 6;
                    vector.Y += 4f;
                    legFrame.Y = legFrame.Height * 5;
                }

                for (int num8 = num3; num8 >= 0; num8--)
                {
                    Vector2 position = vector + (new Vector2(num, 2f) * new Vector2(drawinfo.drawPlayer.direction, 1f));
                    Rectangle value = legFrame;
                    value.Y += num8 * 2;
                    value.Y += num2;
                    value.Height -= num2;
                    value.Height -= num8 * 2;
                    if (num8 != num3)
                        value.Height = 2;

                    position.X += (drawinfo.drawPlayer.direction * num4 * num8) + (num6 * drawinfo.drawPlayer.direction);
                    if (num8 != 0)
                        position.X += num7 * drawinfo.drawPlayer.direction;

                    position.Y += num2;
                    position.Y += num5;
                    DrawData item = new(textureToDraw, position, value, matchingColor, drawinfo.drawPlayer.legRotation, drawinfo.legVect, 1f, drawinfo.playerEffect);
                    item.shader = shaderIndex;
                    drawinfo.DrawDataCache.Add(item);
                }
            }
            else
                orig.Invoke(ref drawinfo, textureToDraw, matchingColor, shaderIndex, glowmask);

        }

        #endregion
    }

    public interface ISpecialDrawHead
    {
        public virtual Vector2 ExtraOffset => Vector2.Zero;
    }

    public interface ISpecialDrawLegs
    {

    }

    public interface ISpecialDrawBackpacks
    {
        public virtual Vector2 ExtraOffset => Vector2.Zero;
    }
}
