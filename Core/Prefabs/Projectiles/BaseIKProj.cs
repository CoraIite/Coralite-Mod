//using Coralite.Core.Systems.IKSystem;
//using Microsoft.Xna.Framework;
//using System;
//using Terraria;
//using Terraria.ModLoader;
//using Terraria.ModLoader.Config;

//namespace Coralite.Core.Prefabs.Projectiles
//{
//    public class BaseIKProj : ModProjectile
//    {
//        public override string Texture => AssetDirectory.OtherProjectiles + "White32x32";

//        public IKSolverCCD iKSolverCCD;
//        public Vector2 targetP;
//        public Vector2 AnchorP => Main.player[Projectile.owner].Center;
//        private Vector2 oldCenter;

//        public bool useLimt = true;
//        public bool update = false;
//        [Range(1, 10)]
//        public int iterations = 1;


//        public override void AI()
//        {
//            if (update)
//            {
//                if (AnchorP == oldCenter)
//                    Calculate();
//                else
//                    UpdateAnchor();
//            }


//            oldCenter = AnchorP;
//        }

//        public void OnInitialize()
//        {
//            //iKSolverCCD = new IKSolverCCD(,targetP);
//        }
//        /// <summary>
//        /// 更新锚点
//        /// </summary>
//        void UpdateAnchor()
//        {
//            iKSolverCCD.useLimt = useLimt;
//            iKSolverCCD.iterations = iterations;
//            iKSolverCCD.arrows[0].CalculateStartAndEnd(AnchorP, Vector2.UnitX);
//            iKSolverCCD.FollowTarget();
//        }

//        /// <summary>
//        /// 更新IK
//        /// </summary>
//        void Calculate()
//        {
//            iKSolverCCD.useLimt = useLimt;
//            iKSolverCCD.iterations = iterations;
//            iKSolverCCD.target = targetP;
//            iKSolverCCD.FollowTarget();
//        }

//        //public override bool PreDraw(ref Color lightColor)
//        //{
//        //    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
//        //    List<CustomVertexInfo> bars = new List<CustomVertexInfo>();

//        //    for (int i = 1; i < oldRotate.Length; i++)
//        //    {
//        //        if (oldRotate[i] == 100f)
//        //            continue;

//        //        float factor = i / (float)oldRotate.Length;
//        //        Vector2 Center = GetCenter(i);
//        //        Vector2 Top = Center + oldRotate[i].ToRotationVector2() * (Projectile.height / 2 + oldDistanceToOwner[i]);
//        //        Vector2 Bottom = Center + oldRotate[i].ToRotationVector2() * (Projectile.height / 2 - trailWidth * (1 - factor) + oldDistanceToOwner[i]);

//        //        var color = Color.Lerp(color1, color2, factor);
//        //        var w = Helper.Lerp(0.5f, 0.05f, factor);
//        //        bars.Add(new(Top - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 1, w)));
//        //        bars.Add(new(Bottom - Main.screenPosition, color, new Vector3((float)Math.Sqrt(factor), 0, w)));
//        //    }

//        //    if (bars.Count > 2)
//        //    {
//        //        List<CustomVertexInfo> triangleList = new();
//        //        //triangleList.Add(new(iKSolverCCD.arrows[iKSolverCCD.arrows.Length - 1].EndPos - Main.screenPosition, Color.AliceBlue, new Vector3(1, 0, 0.5f)));
//        //        for (int i = 0; i < bars.Count - 2; i += 2)
//        //        {
//        //            triangleList.Add(bars[i]);
//        //            triangleList.Add(bars[i + 2]);
//        //            triangleList.Add(bars[i + 1]);

//        //            triangleList.Add(bars[i + 1]);
//        //            triangleList.Add(bars[i + 2]);
//        //            triangleList.Add(bars[i + 3]);
//        //        }

//        //        Main.spriteBatch.End();
//        //        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);

//        //        Main.graphics.GraphicsDevice.Textures[0] = Request<Texture2D>(Texture).Value;
//        //        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
//        //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);

//        //        Main.graphics.GraphicsDevice.RasterizerState = originalState;
//        //        Main.spriteBatch.End();
//        //        Main.spriteBatch.Begin();
//        //    }

//        //    return false;
//        //}
//    }
//}
