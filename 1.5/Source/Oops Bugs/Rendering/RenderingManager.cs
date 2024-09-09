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
    public static class OopsRenderingManager
    {

        [Unsaved(false)]
        private readonly static List<KeyValuePair<(Color, Color), Graphic_Multi>> graphics = new();

        public static Graphic_Multi GetCachableGraphics(string path, Vector2 drawSize, ShaderTypeDef shader, Color colorOne, Color colorTwo)
        {
            shader ??= ShaderTypeDefOf.CutoutComplex;

            for (int i = 0; i < graphics.Count; i++)
            {
                if (colorOne.IndistinguishableFrom(graphics[i].Key.Item1) && colorTwo.IndistinguishableFrom(graphics[i].Key.Item2) && graphics[i].Value.Shader == shader.Shader)
                {
                    return graphics[i].Value;
                }
            }
            //Log.Message($"[OOPS_DEBUG]: Creating new head graphic for {pawn.Name}. ColorOne: {colorOne}, ColorTwo: {colorTwo}");

            Graphic_Multi graphic_Multi = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(path, shader.Shader, drawSize, colorOne, colorTwo);
            graphics.Add(new KeyValuePair<(Color, Color), Graphic_Multi>((colorOne, colorTwo), graphic_Multi));
            return graphic_Multi;
        }
    }

    public class ColorSetting
    {
        public const string clrOneKey = "oopsAllColorOne";
        public const string clrTwoKey = "oopsThatWasALieThereIsTwo";

        public bool hairColor = false;
        public bool skinColor = false;
        public List<Color> colourRange = null;
        public Color? color = null;

        [Unsaved(false)]
        private readonly static Dictionary<string, Color> randomClrPerId = new();
        public Color GetColor(Pawn pawn, Color oldClr, string hashOffset)
        {
            bool didSet = false;
            Color finalClr = Color.white;
            // We could probably blend them if several are set, if that should that be preferable.
            if (pawn?.story != null)
            {
                if (hairColor)
                {
                    finalClr *= pawn.story.HairColor;
                    didSet = true;
                }
                if (skinColor)
                {
                    finalClr *= pawn.story.SkinColor;
                    didSet = true;
                }
            }
            if (color != null)
            {
                finalClr *= color.Value;
                didSet = true;
            }
            if (colourRange != null)
            {
                var id = pawn.ThingID;
                var clrId = hashOffset + id;
                if (randomClrPerId.TryGetValue(clrId, out Color savedClr))
                {
                    finalClr *= savedClr;
                    didSet = true;
                }
                else
                {
                    string strToHash = hashOffset + id + id + id + id;

                    // This generates a deterministic value from 0 to 1 based on the id.
                    float randomValue = Mathf.Abs((strToHash.GetHashCode() % 200) / 200f);
                    float randomValue2 = Mathf.Abs((strToHash.GetHashCode() % 333) / 333f);
                    Color rngColor = Helpers.GetColorFromColourListRange(colourRange, randomValue, randomValue2);
                    randomClrPerId[clrId] = rngColor;
                    //Log.Message($"[OOPS_DEBUG]: Cached a new color for {pawn.Name}: {rngColor} from seeded-values: {randomValue}, {randomValue2}");
                    finalClr *= rngColor;
                    didSet = true;
                }
            }
            if (didSet)
            {
                return finalClr;
            }
            else
            {
                return oldClr;
            }
        }
    }

}
