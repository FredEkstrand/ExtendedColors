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
    [Serializable]
    [
    TypeConverter(typeof(ExtendedColorsConverter)),
    DebuggerDisplay("{NameAndARGBValue}"),
    Editor("System.Drawing.Design.ColorEditor", typeof(UITypeEditor))
    ]
    public partial struct ExtendedColors
    {
        /**
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

        public static implicit operator ExtendedColors(Color c)
        {
            return ExtendedColors.FromArgb(c.ToArgb());
        }

        public static implicit operator Color(ExtendedColors c)
        {
            return Color.FromArgb(c.ToArgb());
        }

        internal ExtendedColors(KnownExtendedColors knownColor)
        {
            m_Value = 0;
            m_State = StateKnownColorValid;
            m_Name = null;
            this.m_KnownColor = unchecked((short)knownColor);
        }

        private ExtendedColors(long value, short state, string name, KnownExtendedColors knownColor)
        {
            this.m_Value = value;
            this.m_State = state;
            this.m_Name = name;
            this.m_KnownColor = unchecked((short)knownColor);
        }

        public bool IsEmpty
        {
            get
            {
                return m_State == 0;
            }
        }

        public bool IsNamedColor
        {
            get
            {
                return ((m_State & StateNameValid) != 0) || IsKnownColor;
            }
        }



        public bool IsKnownColor
        {
            get
            {
                return ((m_State & StateKnownColorValid) != 0);
            }
        }

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

        public byte R
        {
            get
            {
                return (byte)((Value >> ARGBRedShift) & 0xFF);
            }
        }

        public byte G
        {
            get
            {
                return (byte)((Value >> ARGBGreenShift) & 0xFF);
            }
        }


        public byte B
        {
            get
            {
                return (byte)((Value >> ARGBBlueShift) & 0xFF);
            }
        }

        public byte A
        {
            get
            {
                return (byte)((Value >> ARGBAlphaShift) & 0xFF);
            }
        }

        private static void CheckByte(int value, string name)
        {
            if (value < 0 || value > 255)
            {
                throw new ArgumentException("Invalid Ex2 Bound Argument " + name + " " + value + " " + "0, 255");
            }
        }

        /// <include file='doc\EkstrandColorStyles.uex' path='docs/doc[@for="EkstrandColorStyles.MakeArgb"]/*' />
        /// <devdoc>
        ///     Encodes the four values into ARGB (32 bit) format.
        /// </devdoc>
        private static long MakeArgb(byte alpha, byte red, byte green, byte blue)
        {
            return (long)(unchecked((uint)(red << ARGBRedShift |
                         green << ARGBGreenShift |
                         blue << ARGBBlueShift |
                         alpha << ARGBAlphaShift))) & 0xffffffff;
        }

        public static ExtendedColors FromKnownColor(KnownExtendedColors color)
        {
            if (IsEnumValid(unchecked((int)color), (int)KnownExtendedColors.AbaloneShell, (int)KnownExtendedColors.ZurichWhite)) // end side
            {
                return ExtendedColors.FromName(color.ToString());
            }
            return new ExtendedColors(color);
        }

        private static bool IsEnumValid(int value, int min, int max)
        {
            return ((value >= min) && (value <= max));
        }

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

        public int ToArgb()
        {
            return unchecked((int)Value);
        }

        public KnownExtendedColors ToKnownColor()
        {
            return (KnownExtendedColors)m_KnownColor;
        }

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

        public static ExtendedColors ParseToColor(int argb)
        {
            return KnownExtendedColorsTable.ArgbToKnownColor(argb);
        }

        public static ExtendedColors FromArgb(int argb)
        {
            return new ExtendedColors((long)argb & 0xffffffff, StateARGBValueValid, null, (KnownExtendedColors)0);
        }

        public static ExtendedColors FromArgb(int alpha, int red, int green, int blue)
        {
            CheckByte(alpha, "alpha");
            CheckByte(red, "red");
            CheckByte(green, "green");
            CheckByte(blue, "blue");
            return new ExtendedColors(MakeArgb((byte)alpha, (byte)red, (byte)green, (byte)blue), StateARGBValueValid, null, (KnownExtendedColors)0);
        }

        public static ExtendedColors FromArgb(int alpha, ExtendedColors baseColor)
        {
            CheckByte(alpha, "alpha");
            // unchecked - because we already checked that alpha is a byte in CheckByte above
            return new ExtendedColors(MakeArgb(unchecked((byte)alpha), baseColor.R, baseColor.G, baseColor.B), StateARGBValueValid, null, (KnownExtendedColors)0);
        }

        public static ExtendedColors FromArgb(int red, int green, int blue)
        {
            return ExtendedColors.FromArgb(255, red, green, blue);
        }

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

        public static bool operator !=(ExtendedColors left, ExtendedColors right)
        {
            return !(left == right);
        }

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

        public override int GetHashCode()
        {
            return unchecked(m_Value.GetHashCode() ^ m_State.GetHashCode() ^ m_KnownColor.GetHashCode());
            
        }
    }
}

