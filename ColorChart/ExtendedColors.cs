using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Ekstrand.Drawing
{
    /// <summary>
    /// Extended colors structure
    /// </summary>
    [Serializable]
    [
    TypeConverter(typeof(ExtendedColorsConverter)),
    DebuggerDisplay("{NameAndARGBValue}"),
    Editor("System.Drawing.Design.ColorEditor", typeof(UITypeEditor))
    ]
    public partial struct ExtendedColors
    {
       /*
       * Shift count and bit mask for A, R, G, B components in ARGB mode!
       */
        private const int ARGBAlphaShift = 24;
        private const int ARGBRedShift = 16;
        private const int ARGBGreenShift = 8;
        private const int ARGBBlueShift = 0;
        private readonly short m_State;
        private readonly string m_Name;
        private readonly long m_Value;
        private readonly short m_KnownColor;
        private static short StateKnownColorValid = 0x0001;
        private static short StateARGBValueValid = 0x0002;
        private static short StateValueMask = (short)(StateARGBValueValid);
        private static short StateNameValid = 0x0008;
        private static long NotDefinedValue = 0;

        /// <summary>
        /// Implicit operation from Color structure to ExtendedColors structure.
        /// </summary>
        /// <param name="c">Color structure</param>
        public static implicit operator ExtendedColors(Color c)
        {
            return ExtendedColors.FromArgb(c.ToArgb());
        }

        /// <summary>
        /// Implicit operation from ExtendedColors structure to Color structure.
        /// </summary>
        /// <param name="c">ExtendedColors structure</param>
        public static implicit operator Color(ExtendedColors c)
        {
            return Color.FromArgb(c.ToArgb());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="knownColor">Enumeration element of type KnownExtendedColors</param>
        internal ExtendedColors(KnownExtendedColors knownColor)
        {
            m_Value = 0;
            m_State = StateKnownColorValid;
            m_Name = null;
            this.m_KnownColor = unchecked((short)knownColor);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">32-bit ARGB value</param>
        /// <param name="state">ExtendedColors state value.</param>
        /// <param name="name">ExtendedColors name value</param>
        /// <param name="knownColor">Enumeration element of type KnownExtendedColors</param>
        private ExtendedColors(long value, short state, string name, KnownExtendedColors knownColor)
        {
            this.m_Value = value;
            this.m_State = state;
            this.m_Name = name;
            this.m_KnownColor = unchecked((short)knownColor);
        }

        /// <summary>
        /// Specifies whether this Color structure is uninitialized.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return m_State == 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this Color structure is a named color or a member of the KnownColor enumeration.
        /// </summary>
        public bool IsNamedColor
        {
            get
            {
                return ((m_State & StateNameValid) != 0) || IsKnownColor;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this Color structure is a predefined color. Predefined colors are represented by the elements of the KnownColor enumeration.
        /// </summary>
        public bool IsKnownColor
        {
            get
            {
                return ((m_State & StateKnownColorValid) != 0);
            }
        }

        /// <summary>
        /// Get the 32-bit ARGB value of this ExtendedColors.
        /// </summary>
        internal long Value
        {
            get
            {
                if ((m_State & StateValueMask) != 0)
                {
                    return m_Value;
                }
                if (IsKnownColor)
                {
                    return unchecked((int)KnownExtendedColorsTable.KnownColorToArgb((KnownExtendedColors)m_KnownColor));
                }

                return NotDefinedValue;
            }
        }

        /// <summary>
        /// Converts this ExtendedColors structure to a human-readable string.
        /// </summary>
        public string Name
        {
            get
            {
                if ((m_State & StateNameValid) != 0)
                {
                    return m_Name;
                }

                if (IsKnownColor)
                {
                    // first try the table so we can avoid the (slow!) .ToString()
                    string tablename = KnownExtendedColorsTable.KnownColorToName((KnownExtendedColors)m_KnownColor);
                    if (tablename != null)
                        return tablename;

                    return ((KnownExtendedColors)m_KnownColor).ToString();
                }

                // if we reached here, just encode the value
                //
                return Convert.ToString(m_Value, 16);
            }
        }

        /// <summary>
        /// Gets the red component value of this ExtendedColors structure.
        /// </summary>
        public byte R
        {
            get
            {
                return (byte)((Value >> ARGBRedShift) & 0xFF);
            }
        }

        /// <summary>
        /// Gets the green component value of this ExtendedColors structure.
        /// </summary>
        public byte G
        {
            get
            {
                return (byte)((Value >> ARGBGreenShift) & 0xFF);
            }
        }

        /// <summary>
        /// Gets the blue component value of this ExtendedColors structure.
        /// </summary>
        public byte B
        {
            get
            {
                return (byte)((Value >> ARGBBlueShift) & 0xFF);
            }
        }

        /// <summary>
        /// Gets the alpha component value of this ExtendedColors structure.
        /// </summary>
        public byte A
        {
            get
            {
                return (byte)((Value >> ARGBAlphaShift) & 0xFF);
            }
        }

        /// <summary>
        /// Validates the byte value
        /// </summary>
        /// <param name="value">Integer value of the byte</param>
        /// <param name="name">Name of the ARGB being validated</param>
        /// <remarks>This validation if true returns otherwise throw argument exception.</remarks>
        private static void CheckByte(int value, string name)
        {
            if (value < 0 || value > 255)
            {
                throw new ArgumentException("Invalid Ex2 Bound Argument " + name + " " + value + " " + "0, 255");
            }
        }

        /// <summary>
        /// Encodes the four values into ARGB (32 bit) format.
        /// </summary>
        /// <param name="alpha">The alpha value for the new ExtendedColors. Valid values are 0 through 255.</param>
        /// <param name="red">The red component value for the new ExtendedColors. Valid values are 0 through 255. </param>
        /// <param name="green">The green component value for the new ExtendedColors. Valid values are 0 through 255. </param>
        /// <param name="blue">The blue component value for the new ExtendedColors. Valid values are 0 through 255. </param>
        /// <returns>The 32-bit ARGB value of this ExtendedColors.</returns>
        private static long MakeArgb(byte alpha, byte red, byte green, byte blue)
        {
            return (long)(unchecked((uint)(red << ARGBRedShift |
                         green << ARGBGreenShift |
                         blue << ARGBBlueShift |
                         alpha << ARGBAlphaShift))) & 0xffffffff;
        }

        /// <summary>
        /// Creates a ExtendedColors structure from the
        /// </summary>
        /// <param name="color">Enumeration of type KnownExtendedColors</param>
        /// <returns>The ExtendedColors structure that this method creates based on given enumeration otherwise an empty ExtendedColors.</returns>
        public static ExtendedColors FromKnownColor(KnownExtendedColors color)
        {
            if (IsEnumValid(unchecked((int)color), (int)KnownExtendedColors.AbaloneShell, (int)KnownExtendedColors.ZurichWhite)) // end side
            {
                return ExtendedColors.FromName(color.ToString());
            }
            return new ExtendedColors(color);
        }

        /// <summary>
        /// Checks if given enumeration element integer value is in the defined boundary.
        /// </summary>
        /// <param name="value">Enumeration element integer value.</param>
        /// <param name="min">Enumeration integer minimum value to defined the lower boundary.</param>
        /// <param name="max">Enumeration integer maximum value to defined the upper boundary.</param>
        /// <returns>Return true if given enumeration integer value is within defined boundaries otherwise false.</returns>
        private static bool IsEnumValid(int value, int min, int max)
        {
            return ((value >= min) && (value <= max));
        }

        /// <summary>
        /// Creates a ExtendedColors structure from the specified name of a predefined color.
        /// </summary>
        /// <param name="name">A string that is the name of a predefined color. Valid names are the same as the names of the elements of the KnownExtendedColors enumeration. </param>
        /// <returns>The ExtendedColors that this method creates.</returns>
        public static ExtendedColors FromName(string name)
        {
            //try to get a known color first
            object color = ExtendedColorsConverter.GetNamedColor(name);
            if (color != null)
            {
                return (ExtendedColors)color;
            }
            // otherwise treat it as a named color
            return new ExtendedColors(NotDefinedValue, StateNameValid, name, (KnownExtendedColors)0);

        }

        /// <summary>
        /// Gets the hue-saturation-brightness (HSB) brightness value for this ExtendedColors structure.
        /// </summary>
        /// <returns>The brightness of this ExtendedColors. The brightness ranges from 0.0 through 1.0, where 0.0 represents black and 1.0 represents white.</returns>
        public float GetBrightness()
        {
            float r = (float)R / 255.0f;
            float g = (float)G / 255.0f;
            float b = (float)B / 255.0f;

            float max, min;

            max = r; min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            return (max + min) / 2;
        }

        /// <summary>
        /// Gets the hue-saturation-brightness (HSB) hue value, in degrees, for this ExtendedColors structure.
        /// </summary>
        /// <returns>The hue, in degrees, of this ExtendedColors. The hue is measured in degrees, ranging from 0.0 through 360.0, in HSB color space.</returns>
        public Single GetHue()
        {
            if (R == G && G == B)
                return 0; // 0 makes as good an UNDEFINED value as any

            float r = (float)R / 255.0f;
            float g = (float)G / 255.0f;
            float b = (float)B / 255.0f;

            float max, min;
            float delta;
            float hue = 0.0f;

            max = r; min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            delta = max - min;

            if (r == max)
            {
                hue = (g - b) / delta;
            }
            else if (g == max)
            {
                hue = 2 + (b - r) / delta;
            }
            else if (b == max)
            {
                hue = 4 + (r - g) / delta;
            }
            hue *= 60;

            if (hue < 0.0f)
            {
                hue += 360.0f;
            }
            return hue;
        }

        /// <summary>
        /// Gets the hue-saturation-brightness (HSB) saturation value for this ExtendedColors structure.
        /// </summary>
        /// <returns>The saturation of this ExtendedColors. The saturation ranges from 0.0 through 1.0, where 0.0 is grayscale and 1.0 is the most saturated.</returns>
        public float GetSaturation()
        {
            float r = (float)R / 255.0f;
            float g = (float)G / 255.0f;
            float b = (float)B / 255.0f;

            float max, min;
            float l, s = 0;

            max = r; min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            // if max == min, then there is no color and
            // the saturation is zero.
            //
            if (max != min)
            {
                l = (max + min) / 2;

                if (l <= .5)
                {
                    s = (max - min) / (max + min);
                }
                else
                {
                    s = (max - min) / (2 - max - min);
                }
            }
            return s;
        }

        /// <summary>
        /// Gets the 32-bit ARGB value of this ExtendedColors structure.
        /// </summary>
        /// <returns>The 32-bit ARGB value of this ExtendedColors.</returns>
        public int ToArgb()
        {
            return unchecked((int)Value);
        }

        /// <summary>
        /// Gets the KnownExtendedColors value of this Color structure.
        /// </summary>
        /// <returns>An element of the KnownColor enumeration, if the ExtendedColors is created from a predefined color by using either the FromName method or the FromKnownColor method; otherwise, 0.</returns>
        public KnownExtendedColors ToKnownColor()
        {
            return (KnownExtendedColors)m_KnownColor;
        }

        /// <summary>
        /// Converts this ExtendedColors structure to a human-readable string.
        /// </summary>
        /// <returns>A string that is the name of this ExtendedColors, if the ExtendedColors is created from a predefined color by using either the FromName method or the FromKnownColor method; otherwise, a string that consists of the ARGB component names and their values.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(32);
            sb.Append(GetType().Name);
            sb.Append(" [");

            if ((m_State & StateNameValid) != 0)
            {
                sb.Append(Name);
            }
            else if ((m_State & StateKnownColorValid) != 0)
            {
                sb.Append(Name);
            }
            else if ((m_State & StateValueMask) != 0)
            {
                sb.Append("A=");
                sb.Append(A);
                sb.Append(", R=");
                sb.Append(R);
                sb.Append(", G=");
                sb.Append(G);
                sb.Append(", B=");
                sb.Append(B);
            }
            else
            {
                sb.Append("Empty");
            }


            sb.Append("]");

            return sb.ToString();
        }

        /// <summary>
        /// Creates a Color structure from a 32-bit ARGB value.
        /// </summary>
        /// <param name="argb">A value specifying the 32-bit ARGB value. </param>
        /// <returns>The ExtendedColors structure that this method creates.</returns>
        public static ExtendedColors FromArgb(int argb)
        {
            return new ExtendedColors((long)argb & 0xffffffff, StateARGBValueValid, null, (KnownExtendedColors)0);
        }

        /// <summary>
        /// Creates a ExtendedColors structure from the four ARGB component(alpha, red, green, and blue) values.Although this method allows a 32-bit value to be passed for each component, the value of each component is limited to 8 bits.
        /// </summary>
        /// <param name="alpha">The alpha value for the new ExtendedColors. Valid values are 0 through 255.</param>
        /// <param name="red">The red component value for the new ExtendedColors. Valid values are 0 through 255. </param>
        /// <param name="green">The green component value for the new ExtendedColors. Valid values are 0 through 255. </param>
        /// <param name="blue">The blue component value for the new ExtendedColors. Valid values are 0 through 255. </param>
        /// <returns>The Color that this method creates.</returns>
        public static ExtendedColors FromArgb(int alpha, int red, int green, int blue)
        {
            CheckByte(alpha, "alpha");
            CheckByte(red, "red");
            CheckByte(green, "green");
            CheckByte(blue, "blue");
            return new ExtendedColors(MakeArgb((byte)alpha, (byte)red, (byte)green, (byte)blue), StateARGBValueValid, null, (KnownExtendedColors)0);
        }

        /// <summary>
        /// Creates a ExtendedColors structure from the specified Color structure, but with the new specified alpha value. Although this method allows a 32-bit value to be passed for the alpha value, the value is limited to 8 bits.
        /// </summary>
        /// <param name="alpha">The alpha value for the new Color. Valid values are 0 through 255.</param>
        /// <param name="baseColor">The ExtendedColors from which to create the new ExtendedColors. </param>
        /// <returns>The Color that this method creates.</returns>
        public static ExtendedColors FromArgb(int alpha, ExtendedColors baseColor)
        {
            CheckByte(alpha, "alpha");
            // unchecked - because we already checked that alpha is a byte in CheckByte above
            return new ExtendedColors(MakeArgb(unchecked((byte)alpha), baseColor.R, baseColor.G, baseColor.B), StateARGBValueValid, null, (KnownExtendedColors)0);
        }

        /// <summary>
        /// Creates a ExtendedColors structure from the specified 8-bit color values (red, green, and blue). The alpha value is implicitly 255 (fully opaque). Although this method allows a 32-bit value to be passed for each color component, the value of each component is limited to 8 bits.
        /// </summary>
        /// <param name="red">The red component value for the new ExtendedColors. Valid values are 0 through 255. </param>
        /// <param name="green">The green component value for the new ExtendedColors. Valid values are 0 through 255. </param>
        /// <param name="blue">The blue component value for the new ExtendedColors. Valid values are 0 through 255. </param>
        /// <returns>The Color that this method creates.</returns>
        public static ExtendedColors FromArgb(int red, int green, int blue)
        {
            return ExtendedColors.FromArgb(255, red, green, blue);
        }

        /// <summary>
        /// Tests whether two specified ExtendedColors structures are equivalent.
        /// </summary>
        /// <param name="left">The ExtendedColors structure that is to the left of the equality operator. </param>
        /// <param name="right">The ExtendedColors structure that is to the right of the equality operator. </param>
        /// <returns>true if the two ExtendedColors structures are equal; otherwise, false.</returns>
        public static bool operator ==(ExtendedColors left, ExtendedColors right)
        {
            if (left.m_Value == right.m_Value
                && left.m_State == right.m_State
                && left.m_KnownColor == right.m_KnownColor)
            {

                if (left.m_Name == right.m_Name)
                {
                    return true;
                }

                if (left.m_Name == (object)null || right.m_Name == (object)null)
                {
                    return false;
                }

                return left.m_Name.Equals(right.m_Name);
            }

            return false;
        }

        /// <summary>
        /// Tests whether two specified ExtendedColors structures are different.
        /// </summary>
        /// <param name="left">The ExtendedColors structure that is to the left of the inequality operator. </param>
        /// <param name="right">The ExtendedColors structure that is to the right of the inequality operator. </param>
        /// <returns>true if the two ExtendedColors structures are different; otherwise, false.</returns>
        public static bool operator !=(ExtendedColors left, ExtendedColors right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Tests whether the specified object is a ExtendedColors structure and is equivalent to this ExtendedColors structure.
        /// </summary>
        /// <param name="obj">The object to test. </param>
        /// <returns>true if obj is a Color structure equivalent to this ExtendedColors structure; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ExtendedColors)
            {
                ExtendedColors right = (ExtendedColors)obj;
                if (m_Value == right.m_Value
                    && m_State == right.m_State
                    && m_KnownColor == right.m_KnownColor)
                {

                    if (m_Name == right.m_Name)
                    {
                        return true;
                    }

                    if (m_Name == (object)null || right.m_Name == (object)null)
                    {
                        return false;
                    }

                    return m_Name.Equals(m_Name);
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this ExtendedColors structure.
        /// </summary>
        /// <returns>An integer value that specifies the hash code for this Color.</returns>
        public override int GetHashCode()
        {
            return unchecked(m_Value.GetHashCode() ^ m_State.GetHashCode() ^ m_KnownColor.GetHashCode());
            
        }
    }
}

