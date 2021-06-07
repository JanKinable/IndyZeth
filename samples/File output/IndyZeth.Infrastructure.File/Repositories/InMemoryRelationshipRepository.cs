using Elbanique.IndyZeth.Model;
using Elbanique.IndyZeth.Repositories;

namespace Elbanique.IndyZeth.Infrastructure.File.Repositories
{
    public class InMemoryRelationshipRepository : InMemoryRepositoryBase<Relationship>, IRelationshipRepository
    {
        public InMemoryRelationshipRepository() : base()
        {

        }

    }
}
