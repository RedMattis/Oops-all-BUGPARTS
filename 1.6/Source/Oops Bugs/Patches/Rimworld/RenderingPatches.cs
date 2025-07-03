using HarmonyLib;
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
    [HarmonyPatch]
    public class RenderNodePatches
    {
        [HarmonyPatch(typeof(PawnRenderNode_Head), nameof(PawnRenderNode_Head.GraphicFor))]
        [HarmonyPriority(Priority.Low)]
        [HarmonyPostfix]
        public static void PawnHeadGraphics_Postfix(ref Graphic __result, PawnRenderNode_Head __instance, Pawn pawn)
        {
            if (__result != null && pawn != null && pawn.RaceProps?.Humanlike == true)
            {
                if (pawn.Drawer.renderer.CurRotDrawMode != RotDrawMode.Dessicated)
                {
                    var geneExts = Helpers.GetOopsGeneExtensions(pawn);
                    var headShaderOverride = geneExts?.Select(x => x.headShader).FirstOrDefault();
                    if (headShaderOverride != null)
                    {
                        __result = headShaderOverride.GetGraphic(pawn, __result, __instance, headShaderOverride);
                    }
                }
            }
        }
    }
}
