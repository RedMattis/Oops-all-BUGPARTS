using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OopsBug.Patches
{
    public class VFEGeneExtensionWrapper
    {
        private static bool? vfeLoaded = null;
        private static Type VFEGeneExtType = null;
        private static FieldInfo backgroundPathEndogenesInfo = null;
        private static FieldInfo backgroundPathXenogenesInfo = null;
        private static FieldInfo backgroundPathArchiteInfo = null;

        public static bool IsVFEActive
        {
            get
            {
                vfeLoaded ??= ModsConfig.ActiveModsInLoadOrder.Any(x => x.PackageIdPlayerFacing == "OskarPotocki.VanillaFactionsExpanded.Core");
                return vfeLoaded.Value;
            }
        }


        public DefModExtension ext = null;
        public VFEGeneExtensionWrapper(DefModExtension existingInstance = null)
        {
            Type type = GetExtensionType();
            if (type == null)
            {
                Log.Error("OopsBug: Could not find VanillaGenesExpanded.GeneExtension class.");
                return;
            }
            CacheData();


            if (existingInstance != null)
            {
                ext = existingInstance;
            }
            else
            {
                // Create instance of the class
                ext = (DefModExtension)Activator.CreateInstance(type);
            }
        }

        public static void CacheData()
        {
            backgroundPathEndogenesInfo ??= AccessTools.Field(VFEGeneExtType, "backgroundPathEndogenes");
            backgroundPathXenogenesInfo ??= AccessTools.Field(VFEGeneExtType, "backgroundPathXenogenes");
            backgroundPathArchiteInfo ??= AccessTools.Field(VFEGeneExtType, "backgroundPathArchite");
        }

        public static Type GetExtensionType()
        {
            if (VFEGeneExtType == null)
            {
                VFEGeneExtType = AccessTools.TypeByName("VanillaGenesExpanded.GeneExtension");
                if (VFEGeneExtType == null)
                {
                    Log.Error("OopsBug: Could not find VanillaGenesExpanded.GeneExtension class.");
                }
            }
            return VFEGeneExtType;
        }

        // Set/Get field
        public string BackgroundPathEndogenes { get => (string)backgroundPathEndogenesInfo.GetValue(ext); set => backgroundPathEndogenesInfo.SetValue(ext, value); }
        public string BackgroundPathXenogenes { get => (string)backgroundPathXenogenesInfo.GetValue(ext); set => backgroundPathXenogenesInfo.SetValue(ext, value); }
        public string BackgroundPathArchite { get => (string)backgroundPathArchiteInfo.GetValue(ext); set => backgroundPathArchiteInfo.SetValue(ext, value); }
    }
}
