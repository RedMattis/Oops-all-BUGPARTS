using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OopsBug
{
    [HarmonyPatch]
    public static class OopsVanillaPatches
    {
        [HarmonyPatch(typeof(Gene), nameof(Gene.ExposeData))]
        [HarmonyPostfix]
        public static void Post_ExposeData(Gene __instance)
        {
            if (__instance != null && __instance.pawn != null && PawnGenerator.IsBeingGenerated(__instance.pawn) is false && __instance.Active)
            {
                GeneUtils.RefreshGeneEffects(__instance, activate:true);
            }
        }

        [HarmonyPatch(typeof(Gene), nameof(Gene.OverrideBy))]
        [HarmonyPostfix]
        public static void Post_OverrideBy(Gene __instance, Gene overriddenBy)
        {
            GeneUtils.RefreshGeneEffects(__instance, activate: overriddenBy == null);
        }

        [HarmonyPatch(typeof(PawnGenerator), "GenerateGenes")]
        [HarmonyPostfix]
        public static void Post_GenerateGenes(Pawn pawn)
        {
            if (pawn.genes != null)
            {
                List<Gene> genes = pawn.genes.GenesListForReading;
                foreach (Gene gene in genes.Where(x => x != null && x.Active))
                {
                    GeneUtils.RefreshGeneEffects(gene, activate: true);
                }
            }
        }

        [HarmonyPatch(typeof(Gene), nameof(Gene.PostAdd))]
        [HarmonyPostfix]
        public static void Post_GenePostAdd(Gene __instance)
        {
            if (PawnGenerator.IsBeingGenerated(__instance.pawn) is false && __instance.Active)
            {
                GeneUtils.RefreshGeneEffects(__instance, true);
            }
        }

        [HarmonyPatch(typeof(Gene), nameof(Gene.PostRemove))]
        [HarmonyPostfix]
        public static void Post_GeneRemove(Gene __instance)
        {
            if (PawnGenerator.IsBeingGenerated(__instance.pawn) is false && __instance.Active)
            {
                GeneUtils.RefreshGeneEffects(__instance, false);
            }
        }

        [HarmonyPatch(typeof(LifeStageWorker), nameof(LifeStageWorker.Notify_LifeStageStarted))]

        public static void Post_Notify_LifeStageStarted(Pawn pawn)
        {
            if (pawn.genes != null)
            {
                List<Gene> genes = pawn.genes.GenesListForReading;
                foreach (Gene gene in genes.Where(x => x.Active))
                {
                    GeneUtils.RefreshGeneEffects(gene, true);
                }
            }
        }

    }
}
