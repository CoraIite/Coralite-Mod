using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Coralite.Core.Systems.ItemTransform
{
    public class ItemTransformSystem : ModSystem,ILocalizedModType
    {
        public static Dictionary<int, int> TransformItem { get; private set; }
        public static Dictionary<int, List<int>> TransformGroup { get; private set; }
        public static Dictionary<int, string> TransformGroupName { get; private set; }

        public string LocalizationCategory => "Systems";

        public static LocalizedText TransformTo { get; private set; }

        public override void Load()
        {
            TransformItem = [];
            if (!Main.dedServ)
            {
                TransformTo = this.GetLocalization(nameof(TransformTo));
            }
        }

        public override void Unload()
        {
            TransformItem = null;
            TransformTo = null;
        }

        public override void AddRecipeGroups()
        {
            if (TransformGroup != null)
            {
                foreach (var pair in TransformGroup)
                {
                    ModItem mi = ContentSamples.ItemsByType[pair.Value[1]].ModItem;

                    //没名字就只能用ID数字了
                    string name = $"Coralite:{(mi == null ? (pair.Value[1]).ToString() : mi.GetType().Name)}";

                    RecipeGroup g = new RecipeGroup(() => this.GetLocalizedValue(name), [.. pair.Value]);
                    RecipeGroup.RegisterGroup(name, g);

                    TransformGroupName ??= [];
                    TransformGroupName.Add(pair.Key, name);
                }
            }

            TransformGroup = null;
        }

        public override void PostAddRecipes()
        {
            if (TransformGroupName != null)
            {
                for (int i = 0; i < Recipe.maxRecipes; i++)
                {
                    Recipe recipe = Main.recipe[i];

                    for (int j = recipe.requiredItem.Count-1; j>=0 ; j--)
                    {
                        Item item = recipe.requiredItem[j];
                        if (TransformGroupName.TryGetValue(item.type, out string groupName))
                        {
                            recipe.AddRecipeGroup(groupName, item.stack);
                            recipe.RemoveIngredient(item.type);
                        }
                    }
                }
            }

            TransformGroupName = null;
        }

        /// <summary>
        /// 将这个物品注册到合成组里
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="selfType"></param>
        public static void RegisterToTransformGroup(int targetType,int selfType)
        {
            TransformGroup ??= [];

            if (TransformGroup.TryGetValue(targetType,out List<int> value))
            {
                value.Add(selfType);
            }
            else
            {
                TransformGroup.Add(targetType, [targetType, selfType]);
            }
        }

        /// <summary>
        /// 将这个物品注册到合成组里
        /// </summary>
        /// <typeparam name="TSelfType"></typeparam>
        /// <param name="targetType"></param>
        public static void RegisterToTransformGroup<TSelfType>(int targetType) where TSelfType : ModItem
            => RegisterToTransformGroup(targetType, ModContent.ItemType<TSelfType>());
    }
}
