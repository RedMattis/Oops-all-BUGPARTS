using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OopsBug
{
    public class OopsGeneExtension : DefModExtension
    {
        public List<HediffToBody> applyBodyHediff;
        public List<HediffToBodyparts> applyPartHediff;
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

        public List<BodyPartDef> bodyparts = new List<BodyPartDef>();
    }
}
