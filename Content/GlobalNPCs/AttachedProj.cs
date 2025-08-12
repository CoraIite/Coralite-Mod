using System.Collections.Generic;
using Terraria;

namespace Coralite.Content.GlobalNPCs
{
    public partial class CoraliteGlobalNPC
    {
        private List<int> attachedProj = null;
        /// <summary>
        /// 贴在该NPC上的弹幕
        /// </summary>
        public List<int> AttachedProj
        {
            get
            {
                attachedProj ??= new List<int>();
                return attachedProj;
            }
        }

        public bool AddAttachProj(Projectile incomeProj)
        {
            if (attachedProj != null && attachedProj.Count > 0)
            {
                foreach (int index in attachedProj)
                {
                    Projectile proj = Main.projectile[index];
                    if (proj.active && proj.ModProjectile is IAttachableProjectile attachProj)
                        if (!attachProj.NewProjAttach(incomeProj))
                            return false;
                }
            }

            AttachedProj.Add(incomeProj.whoAmI);
            return true;
        }

        /// <summary>
        /// 更新贴贴弹幕
        /// </summary>
        /// <param name="npc"></param>
        public void UpdateAttachProj(NPC npc)
        {
            if (attachedProj != null && attachedProj.Count > 0)
            {
                Vector2 offset = npc.position - npc.oldPosition;
                for (int i = attachedProj.Count - 1; i > -1; i--)//更新黏附弹幕的位置，默认增加
                {
                    int index = attachedProj[i];
                    Projectile p = Main.projectile[index];

                    if (!p.active)
                        attachedProj.Remove(index);//消失就移除
                    else
                        p.position += offset;
                }
            }
        }

        /// <summary>
        /// 调用贴贴弹幕的受击
        /// </summary>
        public void ModifyHitByProj_AttachProj(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (attachedProj == null || attachedProj.Count == 0)
                return;

            foreach (int index in attachedProj)
            {
                Projectile proj = Main.projectile[index];
                if (proj.ModProjectile is IAttachableProjectile attachProj)
                    attachProj.ModifyHitByProjectile(npc, projectile, ref modifiers);
            }
        }
    }

    /// <summary>
    /// 贴贴弹幕，命中NPC后调用<see cref="CoraliteGlobalNPC.AddAttachProj(Projectile)"/> 或者 <see cref="Helpers.Helper.AttatchToTarget(Projectile, NPC)"/>来贴到NPC上<br></br>
    /// 之后就会在一些时机调用此接口的方法
    /// </summary>
    public interface IAttachableProjectile
    {
        /// <summary>
        /// 该NPC受到弹幕攻击时调用
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="projectile"></param>
        /// <param name="modifiers"></param>
        void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers);

        /// <summary>
        /// 返回<see cref="false"/>阻止新弹幕贴贴
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        bool NewProjAttach(Projectile incomeProj);
    }
}
