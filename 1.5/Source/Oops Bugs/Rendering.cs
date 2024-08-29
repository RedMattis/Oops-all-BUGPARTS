using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OopsBug
{
    public class PawnRenderNode_FurSkinClr : PawnRenderNode_Fur
    {
        public PawnRenderNode_FurSkinClr(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
        }

        public override Color ColorFor(Pawn pawn)
        {
            return pawn.story.SkinColor;
        }
    }

    public class PawnRenderNodeWorker_AttachmentBodyHalf : PawnRenderNodeWorker_Body
    {
        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Vector3 vector = base.ScaleFor(node, parms);
            Vector2 bodyGraphicScale = parms.pawn.story.bodyType.bodyGraphicScale;
            bodyGraphicScale.x = (bodyGraphicScale.x - 1) / 2 + 1;
            bodyGraphicScale.y = (bodyGraphicScale.y - 1) / 2 + 1;
            return vector * ((bodyGraphicScale.x + bodyGraphicScale.y) / 2f);
        }
    }

}
