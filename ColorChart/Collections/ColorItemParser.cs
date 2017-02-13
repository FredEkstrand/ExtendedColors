using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ekstrand.Drawing;
using System.Reflection;
using System.Drawing;

namespace Ekstrand.Drawing
{
    /// <summary>
    /// A simple Color Item parser that would use reflection to get each color value and name
    /// from any one of the defined color charts.
    /// </summary>
    public class ColorItemParser
    {
        private List<ColorItem> Items = new List<ColorItem>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public ColorItemParser()
        { }

        /// <summary>
        /// Get a List of color items
        /// </summary>
        public List<ColorItem> ColorItems
        { get { return Items; } }


        /// <summary>
        /// Generate a List of color items all the colors in the extended colors chart.
        /// </summary>
        public void ParseExtendedColors()
        {
            foreach (ExtendedColors color in typeof(ExtendedColors).GetProperties(BindingFlags.Static | BindingFlags.Public).Where(c => c.PropertyType == typeof(ExtendedColors)).Select(c => (ExtendedColors)c.GetValue(c, null)))
            {
                ColorItem ci = new ColorItem();
                ci.Color = color;
                ci.ColorName = color.Name;
                this.Items.Add(ci);
                this.Sort();
            }
        }

        /// <summary>
        /// Generate a List of color items all the colors in the system drawing colors chart.
        /// </summary>
        public void ParseSystemColors()
        {
            foreach (Color color in typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public).Where(c => c.PropertyType == typeof(Color)).Select(c => (Color)c.GetValue(c, null)))
            {
                ColorItem ci = new ColorItem();
                ci.Color = color;
                ci.ColorName = color.Name;
                this.Items.Add(ci);
                this.Sort();
            }
        }

        /// <summary>
        /// Generate a List of color items from both extended colors and system drawing colors charts.
        /// </summary>
        /// <remarks>This would be over 3,500 defined color items.</remarks>
        public void BothExtendedColorsSystemColors()
        {
            ParseExtendedColors();
            ParseSystemColors();
        }

        /// <summary>
        /// Basic sorting of colors based on RGB value.
        /// </summary>
        private void Sort()
        {
            Items.Sort();
        }
    }
}
