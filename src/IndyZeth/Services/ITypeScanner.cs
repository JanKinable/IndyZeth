using System.Reflection;

namespace Elbanique.IndyZeth.Services
{
    public interface ITypeScanner
    {
        void AddType(TypeInfo typeInfo);
    }
}
