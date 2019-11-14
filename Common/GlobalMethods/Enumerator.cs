using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Commom.GlobalMethods
{
    public static class Enumerator
    {       
        public enum CustomerAction
        {
            AddToCart,
            WishList,
            CompareList
        }
        public enum Availability
        {
            [Description("Available")]
            Available = 1,
            [Description("Out Of Stock")]
            OutOfStock = 2
        }

        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
    }
}
