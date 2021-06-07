using System;

namespace Elbanique.IndyZeth.Services
{
    public interface IAssemblyResolver : IDisposable
    {
        System.Reflection.Assembly LoadFromAssemblyPath(string inputFile);
    }
}
