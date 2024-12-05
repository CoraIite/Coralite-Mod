using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.UI;

namespace Coralite.Core.Loaders
{
    class UILoader : ModSystem, IReflactionLoader
    {
        public static List<UserInterface> UserInterfaces;
        public static List<BetterUIState> UIStates;

        public int Priority => 2;
        public IReflactionLoader.LoadSide Side => IReflactionLoader.LoadSide.Cilent;

        public void PreLoad(Mod Mod)
        {
            UserInterfaces = new List<UserInterface>();
            UIStates = new List<BetterUIState>();
        }

        public void Load(Mod Mod, Type type)
        {
            if (type.IsSubclassOf(typeof(BetterUIState)))
            {
                var state = (BetterUIState)Activator.CreateInstance(type, null);
                var userInterface = new UserInterface();
                userInterface.SetState(state);

                UIStates?.Add(state);
                UserInterfaces?.Add(userInterface);
            }
        }

        public void PreUnload(Mod Mod)
        {
            UIStates?.ForEach(n => n.Unload());
            UserInterfaces = null;
            UIStates = null;
        }

        public static void AddLayer(List<GameInterfaceLayer> layers, UserInterface userInterface, UIState state, int index, bool visible, InterfaceScaleType scale)
        {
            string name = state == null ? "Unknown" : state.ToString();
            layers.Insert(index, new LegacyGameInterfaceLayer("Coralite: " + name,
                delegate
                {
                    if (visible)
                        state.Draw(Main.spriteBatch);
                    return true;
                }, scale));
        }

        /// <summary>
        /// 用于获取UIState
        /// </summary>
        /// <typeparam name="T">UI状态</typeparam>
        /// <returns></returns>
        public static T GetUIState<T>() where T : BetterUIState => UIStates.FirstOrDefault(n => n is T) as T;

        /// <summary>
        /// 用于获取UserInterface
        /// </summary>
        /// <typeparam name="T">UI状态</typeparam>
        /// <returns></returns>
        public static UserInterface GetUserInterface<T>() where T : BetterUIState => UserInterfaces.FirstOrDefault(n => n.CurrentState is T);

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            for (int k = 0; k < UIStates.Count; k++)
            {
                var state = UIStates[k];
                AddLayer(layers, UserInterfaces[k], state, state.UILayer(layers), state.Visible, state.Scale);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            foreach (UserInterface eachState in UserInterfaces)
            {
                if (eachState?.CurrentState != null && ((BetterUIState)eachState.CurrentState).Visible)
                    eachState.Update(gameTime);
            }
        }
    }
}
