using Coralite.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;

namespace Coralite.Content.Items.AlchorthentSeries
{
    public class AlchorthentShaderData(ATex flowTex, Asset<Effect> shader, string passName) : ArmorShaderData(shader, passName)
    {
        ATex uFlowTex=flowTex;

        //用于控制浮动的流动速度
        float uTime;
        //用于控制浮动的叠加量
        float flowAdd;
        //线段的偏移量，0~1调整线段绘制范围
        float lineO;
        //线段颜色
        Color lineC;

        /// <summary>
        /// 用于控制浮动的流动速度
        /// </summary>
        /// <param name="time"></param>
        public void SetTime(float time)
        {
            uTime = time;
        }

        /// <summary>
        /// 用于控制浮动的叠加量
        /// </summary>
        /// <param name="flowAdd"></param>
        public void SetFlowAdd(float flowAdd)
        {
            this.flowAdd = flowAdd;
        }

        /// <summary>
        /// 线段的偏移量，0~1调整线段绘制范围
        /// </summary>
        /// <param name="lineOffset"></param>
        public void SetLineOffset(float lineOffset)
        {
            lineO = lineOffset;
        }

        /// <summary>
        /// 线段颜色
        /// </summary>
        /// <param name="lineColor"></param>
        public void SetLineColor(Color lineColor)
        {
            lineC = lineColor;
        }

        public override void Apply(Entity entity, DrawData? drawData = null)
        {
            Shader.Parameters["uFlowTex"]?.SetValue(uFlowTex.Value);
            Shader.Parameters["uTime"]?.SetValue(uTime);
            Shader.Parameters["flowAdd"]?.SetValue(flowAdd);
            Shader.Parameters["lineO"]?.SetValue(lineO);
            Shader.Parameters["lineC"]?.SetValue(lineC.ToVector4());
            Shader.Parameters["transformMatrix"]?.SetValue(VaultUtils.GetTransfromMatrix());

            Apply();
        }
    }
}
