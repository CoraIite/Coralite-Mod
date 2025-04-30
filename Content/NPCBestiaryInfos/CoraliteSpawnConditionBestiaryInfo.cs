//using Microsoft.Xna.Framework.Graphics;
//using ReLogic.Content;
//using Terraria.GameContent.Bestiary;

//namespace Coralite.Content.NPCBestiaryInfos
//{
//public class CoraliteSpawnConditionBestiaryInfo : CoraliteFilterProviderInfo, IBestiaryBackgroundImagePathAndColorProvider, IBestiaryPrioritizedElement
//    {
//        private string _backgroundImagePath;
//        private Color? _backgroundColor;

//        public float OrderPriority { get; set; }

//        public CoraliteSpawnConditionBestiaryInfo(string nameLanguageKey, string iconPath, string backgroundImagePath = null, Color? backgroundColor = null)
//            : base(nameLanguageKey, iconPath)
//        {
//            _backgroundImagePath = backgroundImagePath;
//            _backgroundColor = backgroundColor;
//        }

//        public ATex GetBackgroundImage()
//        {
//            if (_backgroundImagePath == null)
//                return null;

//            return ModContent.Request<Texture2D>(_backgroundImagePath);
//        }

//        public Color? GetBackgroundColor() => _backgroundColor;
//    }
//}
