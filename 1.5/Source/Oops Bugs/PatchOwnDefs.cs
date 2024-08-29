using OopsBug.Patches;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OopsBug
{
    public static class PatchOopsDefs
    {
        public static GlobalSettings settings => OopsAllMod.globalSettings;
        public static void Patch()
        {
            if (settings == null)
            {
                Log.Error("OopsBug: Aborting patches. Global Setting file is missing.");
                return;
            }

            // Get the ModContentPack of this mod.
            var modContentPack = LoadedModManager.RunningMods.FirstOrDefault(mcp => mcp.PackageId == "vera.bug");

            var geneDefs = DefDatabase<GeneDef>.AllDefsListForReading;


            // Foreach each genedef from this mod.
            foreach (var geneDef in geneDefs.Where(x => x.modContentPack == modContentPack))
            {
                // If the gene has no ModExtension, add it.
                geneDef.modExtensions ??= new List<DefModExtension>();
                if (VFEGeneExtensionWrapper.IsVFEActive)
                {
                    AddGeneBackgrounds(geneDef);
                }
            }
        }

        private static void AddGeneBackgrounds(GeneDef geneDef)
        {
            var vfegType = VFEGeneExtensionWrapper.GetExtensionType();

            // Check if the gene has the VFEGeneExtension already.
            DefModExtension existingInstace = geneDef.modExtensions.FirstOrDefault(x => x.GetType() == vfegType);

            var geneExt = new VFEGeneExtensionWrapper(existingInstace);
            if (geneExt != null)
            {
                if (settings.backgroundPathEndogenes != null && geneExt.BackgroundPathEndogenes.NullOrEmpty())
                {
                    geneExt.BackgroundPathEndogenes = settings.backgroundPathEndogenes;
                }
                if (settings.backgroundPathXenogenes != null && geneExt.BackgroundPathXenogenes.NullOrEmpty())
                {
                    geneExt.BackgroundPathXenogenes = settings.backgroundPathXenogenes;
                }
                if (settings.backgroundPathArchite != null && geneExt.BackgroundPathArchite.NullOrEmpty())
                {
                    geneExt.BackgroundPathArchite = settings.backgroundPathArchite;
                }
                if (existingInstace == null)
                    geneDef.modExtensions.Add(geneExt.ext);
            }
        }
    }
}
