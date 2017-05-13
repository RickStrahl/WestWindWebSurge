using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebSurge
{
    /// <summary>
    /// Class that is able to generate up to 10 visually distinct colors for use in graphs.
    /// Based on information found at http://tools.medialab.sciences-po.fr/iwanthue/
    /// Since the possibility of hosting more than 10 series in a single graph is highly unlikely, a hard-coded approach was selected.
    /// </summary>
    public class ColorGenerator
    {
        private int _intCounter = 0;
        private static Color[] _colorValues = new Color[] {
            Color.FromArgb(255,104,42,0),
            Color.FromArgb(255,1,255,153),
            Color.FromArgb(255,255,108,208),
            Color.FromArgb(255,0,108,45),
            Color.FromArgb(255,223,0,19),
            Color.FromArgb(255,192,255,209),
            Color.FromArgb(255,0,25,83),
            Color.FromArgb(255,222,132,0),
            Color.FromArgb(255,175,191,255),
            Color.FromArgb(255,101,0,59)
        };

        /// <summary>
        /// Gets the next color out of the 16 available for use is graphs.
        /// If all 10 have already been served, the sequence of colors is reset and served once more.
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
