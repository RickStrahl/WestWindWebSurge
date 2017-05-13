using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebSurge
{
    /// <summary>
    /// Class that is able to generate up to 56 visually distinct colors for use in graphs.
    /// Based on information found at http://stackoverflow.com/questions/309149/generate-distinctly-different-rgb-colors-in-graphs.
    /// Since the possibility of hosting more than 16 series in a single graph is highly unlikely, a hard-coded approach was selected.
    /// </summary>
    public class ColorGenerator
    {
        private int _intCounter = 0;
        private static Color[] _colorValues = new Color[] {
       Color.Red, Color.Green, Color.Blue, Color.Yellow,
       Color.Pink, Color.DarkSeaGreen, Color.Turquoise, Color.Gold,
       Color.Magenta, Color.SpringGreen, Color.PowderBlue, Color.Khaki,
       Color.Orange, Color.Olive, Color.Cyan, Color.Salmon
    };

        /// <summary>
        /// Gets the next color out of the 16 available for use is graphs.
        /// If all 16 have already been served, the sequence of colors is reset and served once more.
        /// </summary>
        /// <returns>A color value</returns>
        public Color GetNextColor()
        {
            if (_intCounter >= _colorValues.Length)
                _intCounter = 0;

            Color toReturn =  _colorValues[_intCounter];
            _intCounter++;

            return toReturn;
        }

    }
}
