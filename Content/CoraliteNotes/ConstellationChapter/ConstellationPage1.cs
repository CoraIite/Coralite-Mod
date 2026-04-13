using Coralite.Content.Items.FlyingShields;
using Coralite.Content.Items.Misc_Magic;
using Coralite.Content.Items.Misc_Melee;
using Coralite.Content.Items.Misc_Shoot;
using Coralite.Content.Items.Misc_Summon;
using Coralite.Core;
using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Content.CoraliteNotes.ConstellationChapter
{
    [VaultLoaden(AssetDirectory.CoraliteNote+ "ConstellationChapter")]
    public class ConstellationPage1 : ItemShowPage
    {
        public static LocalizedText Title { get; private set; }

        public static ATex Page1Tex { get; set; }

        public override void OnInitialize()
        {
            Title = this.GetLocalization(nameof(Title));
            AddImages();
        }

        public override void AddImages()
        {
            Vector2 center = new Vector2(0, 40);
            float rot = -MathHelper.PiOver2;

            float perRot = MathHelper.TwoPi / 12;
            const int length1 = 170;
            const int length2 = 260;

            for (int i = 0; i < 12; i++)
            {
                Vector2 dir = rot.ToRotationVector2();
                switch (i)
                {
                    default:
                        break;
                    case 0://射手座
                        {
                            var i1 = AddStarTemp(center + dir * length1); AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                    case 1://天蝎座
                        {
                            var i1 = AddStarTemp(center + dir * length1); AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                    case 2://天秤座
                        {
                            var i1 = AddStarTemp(center + dir * length1); AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                    case 3://处女座
                        {
                            var i1 = AddStarWeapon<Virgo>(center + dir * length1); AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                    case 4://狮子座
                        {
                            var i1 = AddStarWeapon<Leonids>(center + dir * length1);
                            var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            i1.AddChainedElement(i2);
                        }
                        break;
                    case 5://巨蟹座
                        {
                            var i1 = AddStarWeapon<CancerFlail>(center + dir * length1); AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                    case 6://双子座
                        {
                            var i1 = AddStarTemp(center + dir * length1); AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                    case 7://金牛座
                        {
                            var i1 = AddStarWeapon<Taurus>(center + dir * length1); AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                    case 8://白羊座
                        {
                            var i1 = AddStarTemp(center + dir * length1); AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                    case 9://双鱼座
                        {
                            var i1 = AddStarWeapon<Pisces>(center + dir * length1); AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                    case 10://水瓶座
                        {
                            var i1 = AddStarWeapon<Aquarius>(center + dir * length1); AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                    case 11://摩羯座
                        {
                            var i1 = AddStarTemp(center + dir * length1); 
                            AddStarTemp(center + dir * length2);
                            //var i2 = AddStarWeapon<Solleonis>(center + dir * length2);
                            //i1.AddChainedElement(i2);
                        }
                        break;
                }

                rot += perRot;
            }
        }

        public ItemShowImage AddStarWeapon<TItem>(Vector2 pos) where TItem:ModItem
        {
            return  NewImage<TItem>(pos, Readfragment.KnowledgeButtonType.Reel, Condition.Hardmode)
                .SetColor(new Color(20, 255, 199));
        }

        public ItemShowImage AddStarTemp(Vector2 pos) 
        {
            return  NewImage(ItemID.FallenStar,pos, Readfragment.KnowledgeButtonType.Reel, Condition.Hardmode)
                .SetColor(new Color(20, 255, 199));
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawTitleH1(spriteBatch, Title, new Color(20, 255, 199));
            Page1Tex.Value.QuickCenteredDraw(spriteBatch, GetDimensions().Center(),scale:2);
        }
    }
}
