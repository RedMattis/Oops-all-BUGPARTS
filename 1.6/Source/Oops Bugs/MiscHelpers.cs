using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OopsBug
{
    public static partial class Helpers
    {
        public static Color GetColorFromColourListRange(this List<Color> colorList, float rngValue, float rngValue2)
        {
            // If there is only one color, return it.
            if (colorList.Count == 1)
                return colorList[0];

            // Get two random adjacent colors from the list.
            int index1 = (int)Mathf.Lerp(0, colorList.Count - 2, rngValue);
            int index2 = index1 + 1;

            Color color1 = colorList[index1];
            Color color2 = colorList[index2];

            float interp = rngValue2;
            return color1 * (1 - interp) + color2 * interp;
        }
    }
}
