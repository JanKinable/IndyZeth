using System;
using System.Collections.Generic;
using System.Text;

namespace Elbanique.IndyZeth.Model
{
    public class Object
    {
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public ObjectKind ObjectKind { get; set; }
        public string AssemblyName { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Object @object &&
                   FullName == @object.FullName &&
                   CompanyName == @object.CompanyName &&
                   ObjectKind == @object.ObjectKind &&
                   AssemblyName == @object.AssemblyName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName, CompanyName, ObjectKind, AssemblyName);
        }
    }
}
