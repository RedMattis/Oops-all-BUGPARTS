using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OopsBug
{
    public class OopsGeneExtension : DefModExtension
    {
        public List<HediffToBody> applyBodyHediff;
        public List<HediffToBodyparts> applyPartHediff;
        public HeadShaderData headShader;
    }

    public class HediffToBody
    {
        public HediffDef hediff;

        public List<ConditionalStatAffecter> conditionals;
    }

    public class HediffToBodyparts
    {
        public HediffDef hediff;

        public List<ConditionalStatAffecter> conditionals;

        public List<BodyPartDef> bodyparts = new();
    }

    

   
    public class HeadShaderData
    {
        public ShaderTypeDef shader = null;
        public ColorSetting colorA = new();
        public ColorSetting colorB = new();

        public Graphic_Multi GetGraphic(Pawn pawn, Graphic oldResult, PawnRenderNode_Head __instance, HeadShaderData data)
        {
            Color colorOne = oldResult.color;
            Color colorTwo = oldResult.colorTwo;

            colorOne = data.colorA.GetColor(pawn, colorOne, ColorSetting.clrOneKey);
            colorTwo = data.colorB.GetColor(pawn, colorTwo, ColorSetting.clrTwoKey);

            Graphic_Multi graphic_Multi = OopsRenderingManager.GetCachableGraphics(oldResult.path, oldResult.drawSize, data.shader, colorOne, colorTwo);
            return graphic_Multi;
        }
    }
}
