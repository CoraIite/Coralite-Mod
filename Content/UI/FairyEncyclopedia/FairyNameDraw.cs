using Coralite.Core.Loaders;
using Coralite.Core.Systems.FairyCatcherSystem;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.UI;

namespace Coralite.Content.UI.FairyEncyclopedia
{
    //[AutoLoadTexture(Path =AssetDirectory.UI)]
    public class FairyNameDraw : UIElement
    {
        //public static ATex FairyNameBack {  get;private set; }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Fairy f = FairyLoader.GetFairy(FairyEncyclopedia.ShowFairyID);
            Vector2 center = GetDimensions().Center() + new Vector2(0, -25);

            //FairyNameBack.Value.QuickCenteredDraw(spriteBatch, GetDimensions().Center(),Color.DarkBlue*0.5f);
            //this.DrawDebugFrame(spriteBatch);
            if (!FairySystem.FairyCaught[f.Type])//没抓到显示？？？
            {
                Utils.DrawBorderStringBig(spriteBatch, "? ? ?"
                    , center, Color.White, 1, 0.5f, 0.5f);
                center.Y += 60;
                Utils.DrawBorderStringBig(spriteBatch, "? ? ?"
                    , center, Color.White, 0.5f, 0.5f, 0.5f);
                return;
            }

            if (f != null)
            {
                Item i = ContentSamples.ItemsByType[f.ItemType];
                Utils.DrawBorderStringBig(spriteBatch, i.Name,
                    center, ItemRarity.GetColor(i.rare), 1, 0.5f, 0.5f);

                center.Y += 60;

                Utils.DrawBorderStringBig(spriteBatch, FairySystem.GetRarityDescription(f.Rarity)
                    , center, FairySystem.GetRarityColor(f.Rarity), 0.5f, 0.5f, 0.5f);
            }
        }
    }
}
