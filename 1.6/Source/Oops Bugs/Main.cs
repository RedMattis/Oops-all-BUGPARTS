
using Verse;
using HarmonyLib;
using UnityEngine;
using System.Collections;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using OopsBug.Patches;
using System.Runtime;

namespace OopsBug
{
    [StaticConstructorOnStartup]
    public static class OopsAllMod
    {
        //public OopsAllSettings settings;
        public static GlobalSettings globalSettings = null;

        static OopsAllMod()
        {
            //settings = GetSettings<OopsAllSettings>();

            ApplyHarmonyPatches();

            globalSettings = DefDatabase<GlobalSettings>.GetNamed("OopsBug_GlobalSettings");
            if (globalSettings == null)
            {
                Log.Error("OopsBug: Could not find OopsBug_GlobalSettings.");
            }

            PatchOopsDefs.Patch();
        }

        static void ApplyHarmonyPatches()
        {
            var harmony = new Harmony("Vera.bug");
            harmony.PatchAll();
        }

        
    }

    public class GlobalSettings : Def
    {
        public string backgroundPathEndogenes = null;
        public string backgroundPathXenogenes = null;
        public string backgroundPathArchite = null;
    }

    //public class OopsAllSettings: ModSettings
    //{
    //}

    
    
}
