using System;

namespace Elbanique.IndyZeth
{
    public static class Extensions
    {
        public static string GetDisplayableName(this Type t)
        {
            return t.DeclaringType?.FullName ?? t.FullName;
        }
    }
}
