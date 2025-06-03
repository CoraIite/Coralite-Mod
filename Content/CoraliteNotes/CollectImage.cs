using Coralite.Core;
using Coralite.Core.Attributes;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace Coralite.Content.CoraliteNotes
{
    [AutoLoadTexture(Path = AssetDirectory.CoraliteNote)]
    public class CollectImage : UIElement
    {
        private readonly int _itemType;
        private readonly Condition _lockCondition;
        private readonly float _scale;
        private readonly bool[] _locks;
        private readonly int _index;
        private readonly LockIconType _lockIconType;


        private float scale;

        public static ATex LockedIcon { get; private set; }

        public enum LockIconType
        {
            Big,
            Middle,
            Small
        }

        public CollectImage(int itemType, Condition LockCondition, bool[] locks, int index
            , float scale = 1, LockIconType lockIconType = LockIconType.Middle)
        {
            OverrideSamplerState = SamplerState.PointClamp;

            _itemType = itemType;
            _lockCondition = LockCondition;
            _scale = scale;
            _locks = locks;
            _index = index;
            _lockIconType = lockIconType;

            this.scale = _scale;
            this.SetSize(TextureAssets.Item[_itemType].Size() * scale);
        }

        public override void Update(GameTime gameTime)
        {
            if (!_locks[_index] && (_lockCondition == null || _lockCondition.IsMet()) && Main.LocalPlayer.HasItem(_itemType))
                _locks[_index] = true;

            base.Update(gameTime);
        }

        public override void Recalculate()
        {
            base.Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Helper.GetItemTexAndFrame(_itemType, out Texture2D tex, out Rectangle frameBox);
            Vector2 center = GetDimensions().Center();

            if (IsMouseHovering)
                scale = Helper.Lerp(scale, _scale + 0.15f, 0.2f);
            else
                scale = Helper.Lerp(scale, _scale, 0.2f);

            if (_lockCondition != null && !_lockCondition.IsMet())//未达成解锁条件，绘制黑影
            {
                spriteBatch.Draw(tex, center, frameBox, new Color(32, 20, 13), 0, frameBox.Size() / 2, scale, 0, 0);
                LockedIcon.Value.QuickCenteredDraw(spriteBatch, new Rectangle((int)_lockIconType, 0, 3, 1), center
                    , rotation: IsMouseHovering ? MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.2f : 0, scale: 0.8f);

                if (IsMouseHovering)
                    UICommon.TooltipMouseText($"???\n{_lockCondition.Description.Value}");
                return;
            }

            Color c = new Color(10, 10, 10, 160);
            if (_locks[_index])//解锁了
                c = Color.White;

            spriteBatch.Draw(tex, center, frameBox, c, 0, frameBox.Size() / 2, scale, 0, 0);
            if (IsMouseHovering)
            {
                Main.HoverItem = ContentSamples.ItemsByType[_itemType].Clone();
                Main.hoverItemName = "a";
            }
        }
    }

    public class CollectButton : UIElement
    {
        public Vector2 ItemPosOffset;
        private readonly int _rewardItemType;
        private readonly bool[] _collects;
        private readonly CoraliteNoteSystem.RewardType _rewardType;

        private readonly ATex buttonTex;
        private readonly ATex sparkleTex;
        private readonly Vector2 sparkleOffset;

        public CollectButton(ATex buttonTex,ATex sparkleTex,Vector2 sparkleOffset,int rewardItemType, bool[] collects, CoraliteNoteSystem.RewardType rewardType)
        {
            _rewardItemType = rewardItemType;
            _collects = collects;
            _rewardType = rewardType;
            this.buttonTex = buttonTex;
            this.sparkleTex = sparkleTex;
            this.sparkleOffset=sparkleOffset;

            Vector2 size = buttonTex.Size();
            size.Y /= 3;
            this.SetSize(size);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            Helper.PlayPitched(CoraliteSoundID.MenuTick);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            bool allCollect = _collects.AllTrue();

            //奖励已领取
            if (CoraliteNoteSystem.CollectRewards[(int)_rewardType])
            {
                Helper.PlayPitched("UI/Error", 0.4f, 0);
                return;
            }

            if (allCollect)
                CoraliteNoteSystem.GetReward(_rewardType, _rewardItemType);
            else
                Helper.PlayPitched("UI/Error", 0.4f, 0);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().Center();
            int yFrame = 0;

            bool allCollect = _collects.AllTrue();
            if (CoraliteNoteSystem.CollectRewards[(int)_rewardType])
                yFrame = 2;
            else if (allCollect)
                yFrame = 1;

            buttonTex.Value.QuickCenteredDraw(spriteBatch, new Rectangle(0, yFrame, 1, 3)
                , pos, IsMouseHovering ? Color.White : Color.White * (allCollect ? 1 : 0.65f));

            if (yFrame == 1)
            {
                sparkleTex.Value.QuickCenteredDraw(spriteBatch, pos + sparkleOffset
                    , Color.White, 0, 1f + 0.1f * MathF.Sin((int)Main.timeForVisualEffects * 0.2f));
            }
            else if (yFrame == 2)
            {
                Helper.GetItemTexAndFrame(_rewardItemType, out Texture2D itemTex, out Rectangle frameBox);

                spriteBatch.Draw(itemTex, pos+ItemPosOffset, frameBox, Color.White, 0, frameBox.Size() / 2, 1, 0, 0);
            }

            if (IsMouseHovering)
            {
                string text = yFrame switch
                {
                    0 => CoraliteNoteSystem.CollectAllItems.Value,
                    1 => CoraliteNoteSystem.ClickGetReward.Value,
                    _ => CoraliteNoteSystem.RewardCollected.Value,
                };

                UICommon.TooltipMouseText(text);
            }
        }
    }
}
