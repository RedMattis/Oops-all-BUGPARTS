using HarmonyLib;
using JetBrains.Annotations;
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

    public class PawnComplexRenderingProps : PawnRenderNodeProperties
    {
        public ShaderTypeDef shader = null;
        public ColorSetting colorA = new();
        public ColorSetting colorB = new();

        /// <summary>
        /// Hacky but this avoid us making a seperate class for what is basically just changing the texture path.
        /// </summary>
        public bool isFurskin = false;
    }

    public class PawnRenderNode_Complex : PawnRenderNode
    {
        PawnComplexRenderingProps ComplexProps => (PawnComplexRenderingProps)props;
        public PawnRenderNode_Complex(Pawn pawn, PawnComplexRenderingProps props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
        }

        protected override string TexPathFor(Pawn pawn)
        {
            if (ComplexProps.isFurskin)
            {
                return pawn.story?.furDef.GetFurBodyGraphicPath(pawn);
            }
            else return base.TexPathFor(pawn);
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            if (ComplexProps.isFurskin && pawn.story?.furDef == null)
            {
                return null;
            }

            string text = TexPathFor(pawn);
            if (text.NullOrEmpty())
            {
                Log.Warning($"[OopsBug] No texture path for {pawn}");
                return null;
            }
            Color colorOne = ComplexProps.colorA.GetColor(pawn, Color.white, ColorSetting.clrOneKey);
            Color colorTwo = ComplexProps.colorB.GetColor(pawn, Color.white, ColorSetting.clrTwoKey);
            ShaderTypeDef shader = ComplexProps.shader;

            var result = OopsRenderingManager.GetCachableGraphics(text, Vector2.one, shader, colorOne, colorTwo);
            return result;
        }
    }

    public class HSVColor
    {
        public float? hue = null;
        public float? saturation = null;
        public float? value = null;
    }
    public class HSVPawnRenderingProps : PawnRenderNodeProperties
    {

        /// <summary>
        /// Sets the lowest permitted value/saturation. (Hue does nothing).
        /// Runs before the multiplier
        /// </summary>
        public HSVColor hsvClampMin = null; 
        public HSVColor hsvClampMax = null;
        public HSVColor hsvMultiplier = null;
    }

    public class PawnRenderNode_HSVTweak : PawnRenderNode
    {
        HSVPawnRenderingProps OopsProps => (HSVPawnRenderingProps)props;
        public PawnRenderNode_HSVTweak(Pawn pawn, HSVPawnRenderingProps props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
        }

        public override Color ColorFor(Pawn pawn)
        {
            var baseClr = base.ColorFor(pawn);

            // Brighten the color
            Color.RGBToHSV(baseClr, out float h, out float s, out float v);

            if (OopsProps.hsvClampMin != null )
            {
                var hsvC = OopsProps.hsvClampMin;
                (float? hue, float? saturation, float? value) = (hsvC.hue, hsvC.saturation, hsvC.value);
                if (value != null)
                {
                    v = Mathf.Max(value.Value, v);
                }
                if (saturation != null)
                {
                    s = Mathf.Max(saturation.Value, s);
                }
            }
            if (OopsProps.hsvClampMax != null)
            {
                var hsvC = OopsProps.hsvClampMax;
                (float? hue, float? saturation, float? value) = (hsvC.hue, hsvC.saturation, hsvC.value);
                if (value != null)
                {
                    v = Mathf.Min(value.Value, v);
                }
                if (saturation != null)
                {
                    s = Mathf.Min(saturation.Value, s);
                }
            }

            if (OopsProps.hsvMultiplier != null)
            {
                var hsvM = OopsProps.hsvMultiplier;
                (float? hue, float? saturation, float? value) = (hsvM.hue, hsvM.saturation, hsvM.value);
                if (hue != null)
                {
                    h = hue.Value;
                }
                if (value != null)
                {
                    v = Mathf.Max(0, Mathf.Min(1f, v * value.Value)); // 1.25f
                }
                if (saturation != null)
                {
                    s = Mathf.Max(0, Mathf.Min(1f, s * saturation.Value)); // 0.55f
                }
            }
            return Color.HSVToRGB(h, s, v);
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
