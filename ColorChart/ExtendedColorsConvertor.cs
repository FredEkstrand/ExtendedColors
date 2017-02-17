using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Reflection;


namespace Ekstrand.Drawing
{
    /// <summary>
    /// Converts ExtendedColors from one data type to another. Access this class through the TypeDescriptor.
    /// </summary>
    public class ExtendedColorsConverter : TypeConverter
    {
        private static string ColorConstantsLock = "colorConstants";
        private static Hashtable colorConstants;
        private static string ValuesLock = "values";
        private static StandardValuesCollection values;

        /// <summary>
        /// Initializes a new instance of the ColorConverter class.
        /// </summary>
        public ExtendedColorsConverter()
        {
        }

        
        private static Hashtable Colors
        {
            get
            {
                if (colorConstants == null)
                {
                    lock (ColorConstantsLock)
                    {
                        if (colorConstants == null)
                        {
                            Hashtable tempHash = new Hashtable(StringComparer.OrdinalIgnoreCase);
                            FillConstants(tempHash, typeof(ExtendedColors));
                            colorConstants = tempHash;
                        }
                    }
                }

                return colorConstants;
            }
        }

        /// <summary>
        /// Determines if this converter can convert an object in the given source type to the native type of the converter.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context. You can use this object to get additional information about the environment from which this converter is being invoked. </param>
        /// <param name="sourceType">The type from which you want to convert. </param>
        /// <returns>true if this object can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type.
        /// </summary>
        /// <param name="context">An ITypeDescriptorContext that provides a format context. </param>
        /// <param name="destinationType">A Type that represents the type to which you want to convert. </param>
        /// <returns>true if this converter can perform the operation; otherwise, false.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        internal static object GetNamedColor(string name)
        {
            object color = null;
            // First, check to see if this is a standard name.
            //
            color = Colors[name];
            if (color != null)
            {
                return color;
            }
            return color;
        }

        ///  
        [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string strValue = value as string;
            if (strValue != null)
            {
                object obj = null;
                string text = strValue.Trim();

                if (text.Length == 0)
                {
                    obj = Color.Empty;
                }
                else
                {
                    // First, check to see if this is a standard name.
                    //
                    obj = GetNamedColor(text);

                    if (obj == null)
                    {
                        if (culture == null)
                        {
                            culture = CultureInfo.CurrentCulture;
                        }

                        char sep = culture.TextInfo.ListSeparator[0];
                        bool tryMappingToKnownColor = true;

                        TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));

                        // If the value is a 6 digit hex number only, then
                        // we want to treat the Alpha as 255, not 0
                        //
                        if (text.IndexOf(sep) == -1)
                        {

                            // text can be '' (empty quoted string)
                            if (text.Length >= 2 && (text[0] == '\'' || text[0] == '"') && text[0] == text[text.Length - 1])
                            {
                                // In quotes means a named value
                                string colorName = text.Substring(1, text.Length - 2);
                                obj = ExtendedColors.FromName(colorName);
                                tryMappingToKnownColor = false;
                            }
                            else if ((text.Length == 7 && text[0] == '#') ||
                                     (text.Length == 8 && (text.StartsWith("0x") || text.StartsWith("0X"))) ||
                                     (text.Length == 8 && (text.StartsWith("&h") || text.StartsWith("&H"))))
                            {
                                // Note: ConvertFromString will raise exception if value cannot be converted.
                                obj = ExtendedColors.FromArgb(unchecked((int)(0xFF000000 | (uint)(int)intConverter.ConvertFromString(context, culture, text))));
                            }
                        }

                        // Nope.  Parse the RGBA from the text.
                        //
                        if (obj == null)
                        {
                            string[] tokens = text.Split(new char[] { sep });
                            int[] values = new int[tokens.Length];
                            for (int i = 0; i < values.Length; i++)
                            {
                                values[i] = unchecked((int)intConverter.ConvertFromString(context, culture, tokens[i]));
                            }

                            // We should now have a number of parsed integer values.
                            // We support 1, 3, or 4 arguments:
                            //
                            // 1 -- full ARGB encoded
                            // 3 -- RGB
                            // 4 -- ARGB
                            //
                            switch (values.Length)
                            {
                                case 1:
                                    obj = ExtendedColors.FromArgb(values[0]);
                                    break;

                                case 3:
                                    obj = ExtendedColors.FromArgb(values[0], values[1], values[2]);
                                    break;

                                case 4:
                                    obj = ExtendedColors.FromArgb(values[0], values[1], values[2], values[3]);
                                    break;
                            }
                            tryMappingToKnownColor = true;
                        }

                        if ((obj != null) && tryMappingToKnownColor)
                        {

                            // Now check to see if this color matches one of our known colors.
                            // If it does, then substitute it.  We can only do this for "Colors"
                            // because system colors morph with user settings.
                            //
                            int targetARGB = ((ExtendedColors)obj).ToArgb();

                            foreach (ExtendedColors c in Colors.Values)
                            {
                                if (c.ToArgb() == targetARGB)
                                {
                                    obj = c;
                                    break;
                                }
                            }
                        }
                    }

                    if (obj == null)
                    {
                        throw new ArgumentException("Invalid ExtendedColors text");
                    }
                }
                return obj;
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the specified object to another type.
        /// </summary>
        /// <param name="context">A formatter context. Use this object to extract additional information about the environment from which this converter is being invoked. Always check whether this value is null. Also, properties on the context object may return null. </param>
        /// <param name="culture">A CultureInfo that specifies the culture to represent the color. </param>
        /// <param name="value">The object to convert. </param>
        /// <param name="destinationType">The type to convert the object to. </param>
        /// <returns>An Object representing the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }

            if (value is ExtendedColors)
            {
                if (destinationType == typeof(string))
                {
                    ExtendedColors c = (ExtendedColors)value;

                    if (c == ExtendedColors.Empty)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        // If this is a known color, then ExtendedColors can provide its own
                        // name.  Otherwise, we fabricate an ARGB value for it.
                        //
                        if (c.IsKnownColor)
                        {
                            return c.Name;
                        }
                        else if (c.IsNamedColor)
                        {
                            return "'" + c.Name + "'";
                        }
                        else
                        {
                            if (culture == null)
                            {
                                culture = CultureInfo.CurrentCulture;
                            }
                            string sep = culture.TextInfo.ListSeparator + " ";
                            TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                            string[] args;
                            int nArg = 0;

                            if (c.A < 255)
                            {
                                args = new string[4];
                                args[nArg++] = intConverter.ConvertToString(context, culture, (object)c.A);
                            }
                            else
                            {
                                args = new string[3];
                            }

                            // Note: ConvertToString will raise exception if value cannot be converted.
                            args[nArg++] = intConverter.ConvertToString(context, culture, (object)c.R);
                            args[nArg++] = intConverter.ConvertToString(context, culture, (object)c.G);
                            args[nArg++] = intConverter.ConvertToString(context, culture, (object)c.B);

                            // Now slam all of these together with the fantastic Join 
                            // method.
                            //
                            return string.Join(sep, args);
                        }
                    }
                }
                if (destinationType == typeof(InstanceDescriptor))
                {
                    MemberInfo member = null;
                    object[] args = null;

                    ExtendedColors c = (ExtendedColors)value;

                    if (c.IsEmpty)
                    {
                        member = typeof(ExtendedColors).GetField("Empty");
                    }
                    else if (c.IsKnownColor)
                    {
                        member = typeof(ExtendedColors).GetProperty(c.Name);
                    }
                    else if (c.A != 255)
                    {
                        member = typeof(ExtendedColors).GetMethod("FromArgb", new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
                        args = new object[] { c.A, c.R, c.G, c.B };
                    }
                    else if (c.IsNamedColor)
                    {
                        member = typeof(ExtendedColors).GetMethod("FromName", new Type[] { typeof(string) });
                        args = new object[] { c.Name };
                    }
                    else
                    {
                        member = typeof(ExtendedColors).GetMethod("FromArgb", new Type[] { typeof(int), typeof(int), typeof(int) });
                        args = new object[] { c.R, c.G, c.B };
                    }

                    Debug.Assert(member != null, "Could not convert color to member.  Did someone change method name / signature and not update Colorconverter?");
                    if (member != null)
                    {
                        return new InstanceDescriptor(member, args);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        
        private static void FillConstants(Hashtable hash, Type enumType)
        {
            MethodAttributes attrs = MethodAttributes.Public | MethodAttributes.Static;
            PropertyInfo[] props = enumType.GetProperties();

            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo prop = props[i];
                if (prop.PropertyType == typeof(ExtendedColors))
                {
                    MethodInfo method = prop.GetGetMethod();
                    if (method != null && (method.Attributes & attrs) == attrs)
                    {
                        object[] tempIndex = null;
                        hash[prop.Name] = prop.GetValue(null, tempIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves a collection containing a set of standard values for the data type for which this validator is designed. This will return null if the data type does not support a standard set of values.
        /// </summary>
        /// <param name="context">A formatter context. Use this object to extract additional information about the environment from which this converter is being invoked. Always check whether this value is null. Also, properties on the context object may return null. </param>
        /// <returns>A collection containing null or a standard set of valid values. The default implementation always returns null.</returns>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (values == null)
            {
                lock (ValuesLock)
                {
                    if (values == null)
                    {

                        // We must take the value from each hashtable and combine them.
                        //
                        ArrayList arrayValues = new ArrayList();
                        arrayValues.AddRange(Colors.Values);

                        // Now, we have a couple of colors that have the same names but
                        // are identical values.  Look for these and remove them.  Too
                        // bad this is n^2.
                        //
                        int count = arrayValues.Count;
                        for (int i = 0; i < count - 1; i++)
                        {
                            for (int j = i + 1; j < count; j++)
                            {
                                if (arrayValues[i].Equals(arrayValues[j]))
                                {
                                    // Remove this item!
                                    //
                                    arrayValues.RemoveAt(j);
                                    count--;
                                    j--;
                                }
                            }
                        }

                        // Sort the array.
                        //
                        arrayValues.Sort(0, arrayValues.Count, new ColorComparer());
                        values = new StandardValuesCollection(arrayValues.ToArray());
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Determines if this object supports a standard set of values that can be chosen from a list.
        /// </summary>
        /// <param name="context">A TypeDescriptor through which additional context can be provided. </param>
        /// <returns>true if GetStandardValues must be called to find a common set of values the object supports; otherwise, false.</returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

       
        private class ColorComparer : IComparer
        {

            public int Compare(object left, object right)
            {
                ExtendedColors cLeft = (ExtendedColors)left;
                ExtendedColors cRight = (ExtendedColors)right;
                return string.Compare(cLeft.Name, cRight.Name, false, CultureInfo.InvariantCulture);
            }
        }
    }
}


