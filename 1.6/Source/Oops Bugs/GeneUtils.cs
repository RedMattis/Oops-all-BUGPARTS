using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OopsBug
{
    public static class GeneUtils
    {
        public static bool RefreshGeneEffects(Gene __instance, bool activate, OopsGeneExtension geneExt = null)
        {
            if (__instance.pawn == null || !__instance.pawn.Spawned)
            {
                return false;
            }

            bool changeMade = false;
            var gene = __instance;
            OopsGeneExtension extension = geneExt ?? gene.def.GetModExtension<OopsGeneExtension>();
            if (extension == null) return false;
            try
            {
                AddHediffToPawn(activate, ref changeMade, gene, extension);
                AddHediffToPart(activate, ref changeMade, gene, extension);

            }
            catch (Exception ex)
            {
                Log.Error($"Error in RefreshGeneEffects: {ex.Message} {ex.StackTrace}");
            }
            return changeMade;
        }

        private static void AddHediffToPart(bool activate, ref bool changeMade, Gene gene, OopsGeneExtension extension)
        {
            if (extension.applyPartHediff != null)
            {
                HashSet<BodyPartRecord> notMissingParts = gene.pawn.health.hediffSet.GetNotMissingParts().ToHashSet();
                foreach (HediffToBodyparts item in extension.applyPartHediff)
                {
                    if (activate)
                    {
                        foreach (BodyPartDef bodypart in item.bodyparts)
                        {
                            if (!gene.pawn.RaceProps.body.GetPartsWithDef(bodypart).EnumerableNullOrEmpty()) //  && num <= gene.pawn.RaceProps.body.GetPartsWithDef(bodypart).Count
                            {
                                var allParts = gene.pawn.RaceProps.body.GetPartsWithDef(bodypart).ToArray();
                                foreach (var part in allParts)
                                {
                                    // Use hediffset to check if the part exists
                                    if (notMissingParts.Contains(part) && part != null && item.hediff != null)
                                    {
                                        // If the pawn already has the hediff on that part, abort.
                                        // Get all hediffs on pawn
                                        var hediffs = gene.pawn.health.hediffSet.hediffs;
                                        // Get all hediffs on the part
                                        var hediffsOnPart = hediffs.Where(x => x.Part == part);
                                        // Check if any of the hediffs on the part is the same as the one we want to add
                                        if (hediffsOnPart.Any(x => x.def == item.hediff))
                                        {
                                            continue;
                                        }

                                        changeMade = true;
                                        gene.pawn.health.AddHediff(item.hediff, part);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (gene.pawn.health.hediffSet.GetFirstHediffOfDefName(item.hediff.defName) != null)
                        {
                            bool found = false;
                            var otherGenes = Helpers.GetAllActiveGenes(gene.pawn).Where(x => x != gene);
                            if (otherGenes.Count() == 0) continue;
                            foreach (var otherGene in otherGenes.Select(x => x.def.GetModExtension<OopsGeneExtension>()).Where(x => x != null))
                            {
                                if (otherGene.applyPartHediff != null)
                                {
                                    foreach (var otherItem in otherGene.applyPartHediff)
                                    {
                                        if (otherItem.hediff.defName == item.hediff.defName)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (found)
                            {
                                continue;
                            }

                            RemoveHediffByName(gene.pawn, item.hediff.defName);
                            changeMade = true;
                        }
                    }
                }
            }

            // Step through each hediff and check so its body-parts still exists. If not, remove it.
            for (int idx = gene.pawn.health.hediffSet.hediffs.Count - 1; idx >= 0; idx--)
            {
                Hediff hediff = gene.pawn.health.hediffSet.hediffs[idx];
                if (hediff.Part != null && !gene.pawn.health.hediffSet.GetNotMissingParts().Contains(hediff.Part))
                {
                    // Check so it isn't a Missing Part Hediff. If so we don't want to remove it.
                    if (hediff.def == HediffDefOf.MissingBodyPart)
                    {
                        continue;
                    }

                    gene.pawn.health.RemoveHediff(hediff);
                    changeMade = true;
                }
            }
        }

        private static void AddHediffToPawn(bool activate, ref bool changeMade, Gene gene, OopsGeneExtension extension)
        {
            List<HediffToBody> toRemove = new List<HediffToBody>();
            if (extension.applyBodyHediff != null)
            {
                foreach (HediffToBody item in extension.applyBodyHediff)
                {
                    if (item.hediff != null)
                    {
                        if (activate)
                        {
                            // If the pawn does not have the hediff, add it.
                            if (gene.pawn.health.hediffSet.GetFirstHediffOfDefName(item.hediff.defName) == null)
                            {
                                gene.pawn.health.AddHediff(item.hediff);
                                changeMade = true;
                            }
                        }
                        else
                        {
                            if (gene.pawn.health.hediffSet.GetFirstHediffOfDefName(item.hediff.defName) != null)
                            {
                                // Check all other genes to see if they have the same hediff. If not, remove it.
                                bool found = false;
                                foreach (var otherGene in Helpers.GetAllActiveGenes(gene.pawn).Where(x => x != gene).Select(x => x.def.GetModExtension<OopsGeneExtension>()).Where(x => x != null))
                                {
                                    if (otherGene.applyBodyHediff != null)
                                    {
                                        foreach (var otherItem in otherGene.applyBodyHediff)
                                        {
                                            if (otherItem.hediff.defName == item.hediff.defName)
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (found)
                                {
                                    continue;
                                }
                                RemoveHediffByName(gene.pawn, item.hediff.defName);

                                changeMade = true;
                            }
                        }
                    }
                    else
                    {
                        Log.Warning($"{gene.def.defName} on {gene.pawn} tried to grant a Hediff, but the Hediff was null. This is probably due to an optional \"MayRequire\" hediff, in which case it is harmless.");

                        // Remove it from the list so we don't get spammed with warnings.
                        toRemove.Add(item);
                    }
                }
            }
            if (toRemove.Count > 0)
            {
                for (int idx = toRemove.Count - 1; idx >= 0; idx--)
                {
                    HediffToBody item = toRemove[idx];
                    extension.applyBodyHediff.Remove(item);
                }
            }
        }

        public static void RemoveHediffByName(Pawn pawn, string hediffName)
        {
            var toRemove = new List<Hediff>();
            foreach (var hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.def.defName == hediffName)
                {
                    toRemove.Add(hediff);
                }
            }
            foreach (var hediff in toRemove)
            {
                pawn.health.RemoveHediff(hediff);
            }
        }
    }
}
