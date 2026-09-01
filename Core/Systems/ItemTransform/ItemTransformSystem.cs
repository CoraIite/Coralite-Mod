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
        public static Dictionary<int, int> TransformGroupID { get; private set; }

        public string LocalizationCategory => "Systems";

        public static LocalizedText TransformTo { get; private set; }
        public static LocalizedText Any { get; private set; }

        public override void Load()
        {
            TransformItem = [];
            if (!Main.dedServ)
            {
                TransformTo = this.GetLocalization(nameof(TransformTo));
                Any = this.GetLocalization(nameof(Any));
            }
        }

        public override void Unload()
        {
            TransformItem = null;
            Any = null;
            TransformTo = null;
        }

        public override void AddRecipeGroups()
        {
            if (TransformGroup != null)
            {
                foreach (var pair in TransformGroup)
                {
                    int key = pair.Key;
                    List<int> value = pair.Value;

                    for (int i = 0; i < value.Count - 1; i++)
                        TransformItem[value[i]] = value[i + 1];

                    TransformItem[value[^1]] = key;

                    ModItem mi = ContentSamples.ItemsByType[value[1]].ModItem;

                    //没名字就只能用ID数字了
                    string name = $"Coralite:{(mi == null ? value[1].ToString() : mi.GetType().Name)}";

                    RecipeGroup g = new RecipeGroup(() => $"{Any.Value} {ContentSamples.ItemsByType[key].Name}", [.. value]);
                    int groupID = RecipeGroup.RegisterGroup(name, g);

                    TransformGroupID ??= [];
                    TransformGroupID.Add(key, groupID);
                }
            }

            TransformGroup = null;
        }

        public override void PostAddRecipes()
        {
            if (TransformGroupID != null)
            {
                for (int i = 0; i < Recipe.maxRecipes; i++)
                {
                    Recipe recipe = Main.recipe[i];

                    for (int j = recipe.requiredItem.Count - 1; j >= 0; j--)
                    {
                        Item item = recipe.requiredItem[j];
                        if (TransformGroupID.TryGetValue(item.type, out int ID))
                        {
                            int stack = item.stack;
                            recipe.RemoveIngredient(item.type);
                            recipe.AddRecipeGroup(ID, stack);
                        }
                    }
                }
            }

            TransformGroupID = null;
        }

        /// <summary>
        /// 将这个物品注册到合成组里
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="selfType"></param>
        public static void RegisterToTransformGroup(int selfType,int targetType)
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

        /// <summary>
        /// 将这个物品注册到合成组里
        /// </summary>
        /// <typeparam name="TSelfType"></typeparam>
        /// <typeparam name="TTargetType"></typeparam>
        public static void RegisterToTransformGroup<TSelfType, TTargetType>() where TSelfType : ModItem where TTargetType : ModItem
            => RegisterToTransformGroup(ModContent.ItemType<TTargetType>(), ModContent.ItemType<TSelfType>());
    }
}
