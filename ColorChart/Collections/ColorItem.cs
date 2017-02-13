using System;
using System.Drawing;


namespace Ekstrand.Drawing
{
    [Serializable]
    public struct ColorItem : IComparable<ColorItem>, IComparable, IEquatable<ColorItem>
    {
        public Color Color
        { get; set; }

        public string ColorName
        { get; set; }

        public int CompareTo(object obj)
        {
            ColorItem other = (ColorItem)obj;
            
            if(this.Color.R == other.Color.R)
            {
                if(this.Color.G == other.Color.G)
                {
                    if(this.Color.B == other.Color.B)
                    {
                        return 0;
                    }
                    return this.Color.B < other.Color.B ? -1 : 1;
                }
                return this.Color.G < other.Color.G ? -1 : 1;
            }
            return this.Color.R < other.Color.R ? -1 : 1;
        }

        private float HSV(Color c)
        {
            return c.GetHue() + c.GetSaturation() + c.GetBrightness();
        }

        public int CompareTo(ColorItem other)
        {
            if (this.Color.R == other.Color.R)
            {
                if (this.Color.G == other.Color.G)
                {
                    if (this.Color.B == other.Color.B)
                    {
                        return 0;
                    }
                    return this.Color.B < other.Color.B ? -1 : 1;
                }
                return this.Color.G < other.Color.G ? -1 : 1;
            }
            return this.Color.R < other.Color.R ? -1 : 1;
        }

        public bool Equals(ColorItem other)
        {
            return this.Color.Equals(other.Color);
        }
    }
}
