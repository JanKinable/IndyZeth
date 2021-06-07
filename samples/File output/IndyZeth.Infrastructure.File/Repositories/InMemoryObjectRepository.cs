using Elbanique.IndyZeth.Repositories;

namespace Elbanique.IndyZeth.Infrastructure.File.Repositories
{
    public class InMemoryObjectRepository : InMemoryRepositoryBase<Model.Object>, IObjectRepository
    {
        public InMemoryObjectRepository() : base()
        {

        }

    }
}
