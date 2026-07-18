using System.Collections.Generic;
using System.Linq;

namespace Coralite.Content.UI.Animations
{
    public class AnimationTree
    {
        public AnimationTreeNode node1;
        public AnimationTreeNode node2;

        public int maxTime;
        public int maxDepth;

        public class AnimationTreeNode
        {
            public AnimationTreeNode node1;
            public AnimationTreeNode node2;

            public int startTime;
            public int endTime;

            public List<UIAnimationComponent> components;

            public void InitNode(AnimationTree tree, int depth, int startTime, int endTime)
            {
                this.startTime = startTime;
                this.endTime = endTime;

                if (depth >= tree.maxDepth || endTime - startTime < 60)
                {
                    components = [];
                    this.startTime = startTime;
                    this.endTime = endTime;
                    return;
                }

                node1 = new AnimationTreeNode();
                node1.InitNode(tree, depth + 1, startTime, (startTime + endTime) / 2);
                node2 = new AnimationTreeNode();
                node2.InitNode(tree, depth + 1, (startTime + endTime) / 2, endTime);
            }

            public void AddComponent(UIAnimationComponent component)
            {
                if (component.StartTime <= endTime || component.EndTime > startTime)//在自身区间范围内
                {
                    if (node1 == null || node2 == null)
                    {
                        //加到自己这里
                        components ??= [];
                        components.Add(component);
                    }
                    else
                    {
                        node1.AddComponent(component);
                        node2.AddComponent(component);
                    }
                }
            }

            public void Sort()
            {
                if (components == null)
                {
                    node1.Sort();
                    node2.Sort();
                    return;
                }

                //components.Sort((a, b) => a.DrawLayer.CompareTo(b.DrawLayer));
                //components.Sort((a, b) => a.DrawLayer.CompareTo(b.DrawLayer));

                IEnumerable<UIAnimationComponent> c = from item in components orderby item.DrawLayer descending select item;

                components = [.. c];
            }

            public List<UIAnimationComponent> GetComponents(int currentTimer)
            {
                if (components == null)
                {
                    if (currentTimer<= (startTime + endTime) / 2)
                        return node1.GetComponents(currentTimer);                    
                    else
                        return node2.GetComponents(currentTimer);
                }
                else
                    return components;
            }
        }

        public AnimationTree(int maxTime, int maxDepth)
        {
            this.maxDepth = maxDepth;
            this.maxTime = maxTime;

            node1 = new AnimationTreeNode();
            node1.InitNode(this, 0, 0, (0 + maxTime) / 2);
            node2 = new AnimationTreeNode();
            node2.InitNode(this, 0, (0 + maxTime) / 2, maxTime);
        }

        public void AddComponent(UIAnimationComponent component)
        {
            if (component.StartTime <= maxTime / 2)//在自身区间范围内
                node1.AddComponent(component);
            if (component.EndTime > maxTime / 2)
                node2.AddComponent(component);
        }

        public void Sort()
        {
            node1.Sort();
            node2.Sort();
        }

        public List<UIAnimationComponent> GetComponents(int currentTimer)
        {
            if (currentTimer <= maxTime / 2)
                return node1.GetComponents(currentTimer);
            else
                return node2.GetComponents(currentTimer);
        }
    }
}
