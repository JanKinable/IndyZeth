using System;

namespace Elbanique.IndyZeth
{
    public interface IUnitOfWork: IDisposable
    {
        void Save();
    }
}
