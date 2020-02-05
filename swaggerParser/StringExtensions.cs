using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace swaggerParser
{
    public static class StringExtensions
    {
        public static string GetKebabName(this string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            return Regex.Replace(name, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1", RegexOptions.Compiled).Trim().ToLower();
        }

        public static string GetAngularName(this string name)
        {
            var nameProp = Char.ToLowerInvariant(name[0]) + name.Substring(1);
            if (nameProp.Length < 3)
            {
                nameProp = nameProp.ToLowerInvariant();
            }
            return nameProp;
        }
    }
}
