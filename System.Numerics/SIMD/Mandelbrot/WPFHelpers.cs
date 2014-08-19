using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Mandelbrot
{
    /// <summary>
    /// Converts true/false 0/!0 ~0.0/~<>0.0 to Visible/Collapsed
    /// Set a parameter to negate the result
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    [ValueConversion(typeof(int), typeof(Visibility))]
    [ValueConversion(typeof(double), typeof(Visibility))]
    public class VisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility trueVal = (parameter == null) ? Visibility.Visible : Visibility.Collapsed;
            Visibility falseVal = (parameter == null) ? Visibility.Collapsed : Visibility.Visible;
            if (targetType == typeof(Visibility))
            {
                if (value is bool)
                    return (bool)value ? trueVal : falseVal;
                else if (value is int)
                    return ((int)value != 0) ? trueVal : falseVal;
                else if (value is double)
                    // Should be Double.Epsilon, but that number is wrong (it's a denormal)
                    return (Math.Abs((double)value) >= 1e-10) ? trueVal : falseVal;
                // If it's not one of those types, fall thru to NotImpl...
            }
            else if (targetType == typeof(bool) && value is Visibility)
            {
                return ((Visibility)value) == trueVal;
            }
            throw new NotImplementedException();
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo c) { return Convert(v, t, p, c); }
    }
    /// <summary>
    /// Inverts a boolean value: nothign too fancy
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class NegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

}
