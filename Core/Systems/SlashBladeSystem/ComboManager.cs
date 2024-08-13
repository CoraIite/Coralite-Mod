using Coralite.Content.ModPlayers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Coralite.Core.Systems.SlashBladeSystem
{
    public class ComboManager
    {
        public delegate void PostShoot(Projectile proj, ref int combo);

        public int combo;
        public int resetTimer;

        public enum ControlType
        {
            Left = 0,
            Right = 1,
            Left_Up = 2,
            Right_Up = 3,
            Left_Down = 4,
            Right_Down = 5,
            Special = 6,
            Special_Up = 7,
            Special_Down = 8,
        }

        /// <summary>
        /// 使用Control获得对应控制类型的连段表，再使用combo获取到指定的ComboData
        /// </summary>
        public Dictionary<int, Dictionary<int, ComboData>> ComboDatas;

        /// <summary>
        /// 攻击后的后摇时间，超出这个时间后重置连段
        /// </summary>
        public readonly int resetMaxTime;

        public ComboManager(int resetMaxTime = 15)
        {
            this.resetMaxTime = resetMaxTime;
            ComboDatas = new Dictionary<int, Dictionary<int, ComboData>>();
        }

        public ComboManager AddCombo(int controlType, int combo, int type, float damageMult, PostShoot postShoot = null)
        {
            ComboData data = new(type, damageMult, postShoot ?? ComboData.PostShoot);
            if (!ComboDatas.ContainsKey(controlType))
                ComboDatas.Add(controlType, new Dictionary<int, ComboData>());

            ComboDatas[controlType].Add(combo, data);
            return this;
        }

        public void UpdateDelay(Player player)
        {
            if (player.ItemTimeIsZero && !player.controlUseItem && combo != 0)
            {
                resetTimer++;
                if (resetTimer > resetMaxTime)
                {
                    resetTimer = 0;
                    combo = 0;
                }
            }
        }

        public void Shoot(Player player, EntitySource_ItemUse_WithAmmo source, int damage, float knockback)
        {
            Dictionary<int, ComboData> data;
            ControlType controlType;
            try//鲨比中的鲨比
            {
                if (player.altFunctionUse == 2)//右键
                {
                    if (ComboDatas.ContainsKey((int)ControlType.Right_Up)
                        && ComboDatas[(int)ControlType.Right_Up].ContainsKey(combo)
                        && player.controlUp)//按住上同时右键
                    {
                        data = ComboDatas[(int)ControlType.Right_Up];
                        controlType = ControlType.Right_Up;
                    }
                    else if (ComboDatas.ContainsKey((int)ControlType.Right_Down)
                        && ComboDatas[(int)ControlType.Right_Down].ContainsKey(combo)
                        && player.controlDown) //按住下
                    {
                        data = ComboDatas[(int)ControlType.Right_Down];
                        controlType = ControlType.Right_Down;
                    }
                    else
                    {
                        data = ComboDatas[(int)ControlType.Right];
                        controlType = ControlType.Right;
                    }
                }
                else if (player.GetModPlayer<CoralitePlayer>().useSpecialAttack)
                {
                    if (ComboDatas.ContainsKey((int)ControlType.Special_Up)
                        && ComboDatas[(int)ControlType.Special_Up].ContainsKey(combo)
                        && player.controlUp)//按住上
                    {
                        data = ComboDatas[(int)ControlType.Special_Up];
                        controlType = ControlType.Special_Up;
                    }
                    else if (ComboDatas.ContainsKey((int)ControlType.Special_Down)
                        && ComboDatas[(int)ControlType.Special_Down].ContainsKey(combo)
                        && player.controlDown) //按住下
                    {
                        data = ComboDatas[(int)ControlType.Special_Down];
                        controlType = ControlType.Special_Down;
                    }
                    else
                    {
                        data = ComboDatas[(int)ControlType.Special];
                        controlType = ControlType.Special;
                    }
                }
                else
                {
                    if (ComboDatas.ContainsKey((int)ControlType.Left_Up)
                        && ComboDatas[(int)ControlType.Left_Up].ContainsKey(combo)
                        && player.controlUp)//按住上
                    {
                        data = ComboDatas[(int)ControlType.Left_Up];
                        controlType = ControlType.Left_Up;
                    }
                    else if (ComboDatas.ContainsKey((int)ControlType.Left_Down)
                        && ComboDatas[(int)ControlType.Left_Down].ContainsKey(combo)
                        && player.controlDown) //按住下
                    {
                        data = ComboDatas[(int)ControlType.Left_Down];
                        controlType = ControlType.Left_Down;
                    }
                    else
                    {
                        data = ComboDatas[(int)ControlType.Left];
                        controlType = ControlType.Left;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }

            if (!data.ContainsKey(combo))
            {
                combo = 0;
                return;
            }

            ComboData data1 = data[combo];
            Projectile p = Projectile.NewProjectileDirect(source, player.Center, Microsoft.Xna.Framework.Vector2.Zero, data1.projType, (int)(damage * data1.damageMult),
                  knockback, player.whoAmI, (int)controlType, combo);

            resetTimer = 0;
            data1.postShoot(p, ref combo);
        }
    }

    public struct ComboData
    {
        public int projType;
        public float damageMult;

        public ComboManager.PostShoot postShoot;

        public ComboData(int projType, float damageMult, ComboManager.PostShoot postShoot)
        {
            this.projType = projType;
            this.damageMult = damageMult;
            this.postShoot = postShoot;
        }

        public static void PostShoot(Projectile proj, ref int combo)
        {
            combo++;
        }

        public static void ResetCombo(Projectile proj, ref int combo)
        {
            combo = 0;
        }

    }
}
