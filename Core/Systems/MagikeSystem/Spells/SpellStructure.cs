﻿using Coralite.Core.Loaders;
using Coralite.Core.Systems.MagikeSystem.Components;
using Coralite.Core.Systems.MTBStructure;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace Coralite.Core.Systems.MagikeSystem.Spells
{
    public abstract class SpellStructure : Multiblock
    {
        public override int[,] StructureTile
        {
            get
            {
                throw new NotImplementedException("对于法术多方块结构，请勿使用这个东西！");
            }
        }


        /// <summary>
        /// 法术的连接
        /// </summary>
        public List<(Point, Point)> SpellCheck { get; protected set; }

        /// <summary>
        /// 法术的物块类型
        /// </summary>
        public Dictionary<Point, ushort> SpellTiles { get; protected set; }

        /// <summary>
        /// 法术的物块类型，未激活前的
        /// </summary>
        public Dictionary<Point, ushort> SpellTilesOrigin { get; protected set; }


        /// <summary>
        /// 向法术中添加一个节点
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="notActiveTile"></param>
        /// <param name="activeTile"></param>
        public void AddSpell(Point origin, Point target, ushort notActiveTile, ushort activeTile)
        {
            SpellCheck ??= new List<(Point, Point)>();
            SpellTiles ??= new();
            SpellTilesOrigin ??= new();

            SpellCheck.Add((origin, target));
            SpellTiles.TryAdd(origin, activeTile);
            SpellTilesOrigin.TryAdd(origin, notActiveTile);
        }

        /// <summary>
        /// 输入的形状点必须大于两个！
        /// </summary>
        /// <param name="notActiveTile"></param>
        /// <param name="activeTile"></param>
        /// <param name="shapeNode"></param>
        public void AddSpellShape(ushort notActiveTile, ushort activeTile, params Point[] shapeNode)
        {
            for (int i = 0; i < shapeNode.Length - 1; i++)
                AddSpell(shapeNode[i], shapeNode[i + 1], notActiveTile, activeTile);

            AddSpell(shapeNode[^1], shapeNode[0], notActiveTile, activeTile);
        }

        /// <summary>
        /// 检测法术是否按照正确方式连接了<br></br>
        /// 如果有物块消失那么就干掉整个多物块结构
        /// </summary>
        /// <param name="center"></param>
        public virtual bool CheckSpell(Point center)
        {
            foreach (var item in SpellCheck)
            {
                Point p = center + item.Item1;
                Tile t = Framing.GetTileSafely(p);

                //检测物块类型
                if (t.TileType != SpellTiles[item.Item1])
                {
                    DestroySpell(center);
                    return false;
                }

                //获取tp
                if (!MagikeHelper.TryGetEntityWithTopLeft(new Point16(p), out var tp))
                {
                    DestroySpell(center);
                    return false;
                }

                //检测线性连接
                if (!tp.TryGetComponent(MagikeComponentID.MagikeSender, out CheckOnlyLinerSender sender))
                {
                    DestroySpell(center);
                    return false;
                }

                if (!sender.Receivers.Contains(new Point16(center + item.Item2)))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 破坏多方块结构，将特殊物块还原为正常物块
        /// </summary>
        /// <param name="center"></param>
        public virtual void DestroySpell(Point center)
        {
            var structureTile = SpellTilesOrigin;

            foreach (var item in structureTile)
            {
                Point p = center + item.Key;
                Tile tile = Framing.GetTileSafely(p);
                if (!tile.HasTile)
                    continue;

                if (tile.TileType != SpellTiles[item.Key])
                    continue;

                WorldGen.KillTile(p.X, p.Y, noItem: true);
                WorldGen.PlaceTile(p.X, p.Y, item.Value, true);
            }
        }

        #region 多方块结构激活部分

        public override void CheckStructure(Point center)
        {
            foreach (var p in Main.projectile.Where(p => p.active && p.friendly && p.type == ModContent.ProjectileType<SpellStructurePreviewProj>() && p.ai[0] == Type))
                p.Kill();

            Projectile.NewProjectile(new EntitySource_TileUpdate(center.X, center.Y), center.ToWorldCoordinates(0, 0), Vector2.Zero
                , ModContent.ProjectileType<SpellStructurePreviewProj>(), 0, 0, Main.myPlayer, Type, center.X, center.Y);

            if (!CheckSpellStructure(center, out Point failPoint))
            {
                PopupText.NewText(new AdvancedPopupRequest()
                {
                    Color = Color.Red,
                    Text = MultiblockSystem.FailText.Value,
                    DurationInFrames = 60,
                    Velocity = -Vector2.UnitY
                }, center.ToWorldCoordinates() - (Vector2.UnitY * 32));

                Fail(failPoint);
                return;
            }

            OnSuccess(center);
        }

        public virtual bool CheckSpellStructure(Point center, out Point failPoint)
        {
            DestroySpell(center);

            var structureTile = SpellTilesOrigin;

            foreach (var item in structureTile)
            {
                Point p = center + item.Key;
                Tile t = Framing.GetTileSafely(p);
                if (!t.HasTile || t.TileType != item.Value)
                {
                    failPoint = p;
                    return false;
                }
            }

            failPoint = Point.Zero;
            return true;
        }

        public override void OnSuccess(Point center)
        {
            foreach (var p in Main.projectile.Where(p => p.active && p.friendly && p.type == ModContent.ProjectileType<SpellStructurePreviewProj>() && p.ai[0] == Type))
                p.Kill();

            var structureTile = SpellTiles;

            foreach (var item in structureTile)
            {
                Point p = center + item.Key;

                WorldGen.KillTile(p.X, p.Y, noItem: true);
                WorldGen.PlaceTile(p.X, p.Y, item.Value, true);
            }
        }

        #endregion
    }

    public class SpellStructurePreviewProj : ModProjectile
    {
        public override string Texture => AssetDirectory.OtherProjectiles + "White32x32";

        public ref float MTBID => ref Projectile.ai[0];

        public Point Center => new Point((int)Projectile.ai[1], (int)Projectile.ai[2]);

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.timeLeft = 60 * 60 * 10;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
            => false;

        public override bool? CanDamage()
            => false;

        public override bool ShouldUpdatePosition()
            => false;


        public override bool PreDraw(ref Color lightColor)
        {
            Dictionary<Point, ushort> spellTilesOrigin = (MultiblockLoader.GetMTBStructure((int)MTBID) as SpellStructure).SpellTilesOrigin;

            Point center = Center;

            foreach (var item in spellTilesOrigin)
            {
                Point p = center + item.Key;
                int type = item.Value;

                Tile tile = Framing.GetTileSafely(p);
                if (tile.HasTile)
                {
                    if (tile.TileType != type)
                    {
                        Texture2D tex = Projectile.GetTexture();
                        Vector2 pos = p.ToWorldCoordinates() - Main.screenPosition;

                        Main.spriteBatch.Draw(tex, pos, null, Color.Red * 0.5f
                            , 0, tex.Size() / 2, 0.5f, 0, 0);

                        int itemType = TileLoader.GetItemDropFromTypeAndStyle(type);

                        Main.instance.LoadItem(itemType);
                        Texture2D itemTex = TextureAssets.Item[itemType].Value;

                        float colorA = 0.5f;
                        float scale = 0.9f;
                        if (Helper.MouseScreenInRect(new Rectangle((int)pos.X - 8, (int)pos.Y - 8, 16, 16)))
                        {
                            colorA = 1f;
                            scale = 1.2f;

                            Utils.DrawBorderString(Main.spriteBatch, ContentSamples.ItemsByType[itemType].Name, pos - new Vector2(0, 16)
                                , Color.White, anchorx: 0.5f, anchory: 0.5f);
                        }

                        Main.spriteBatch.Draw(itemTex, pos, null, Color.White * colorA, 0, itemTex.Size() / 2, scale, 0, 0);
                    }
                }
                else
                {
                    int itemType = TileLoader.GetItemDropFromTypeAndStyle(type);

                    Main.instance.LoadItem(itemType);
                    Texture2D itemTex = TextureAssets.Item[itemType].Value;

                    Vector2 pos = p.ToWorldCoordinates() - Main.screenPosition;
                    float colorA = 0.5f;
                    float scale = 0.9f;
                    if (Helper.MouseScreenInRect(new Rectangle((int)pos.X - 8, (int)pos.Y - 8, 16, 16)))
                    {
                        colorA = 1f;
                        scale = 1.2f;

                        Utils.DrawBorderString(Main.spriteBatch, ContentSamples.ItemsByType[itemType].Name, pos - new Vector2(0, 16)
                            , Color.White, anchorx: 0.5f, anchory: 0.5f);
                    }

                    Main.spriteBatch.Draw(itemTex, pos, null, Color.White * colorA, 0, itemTex.Size() / 2, scale, 0, 0);
                }

            }

            return false;
        }
    }
}
