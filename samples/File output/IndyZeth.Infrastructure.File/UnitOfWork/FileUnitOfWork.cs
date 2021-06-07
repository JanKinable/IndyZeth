using Elbanique.IndyZeth.Repositories;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Elbanique.IndyZeth.Infrastructure.File.UnitOfWork
{
    public class FileUnitOfWork : IUnitOfWork
    {
        private readonly IObjectRepository objectRepository;
        private readonly IRelationshipRepository relationshipRepository;
        private readonly FileRepositorySettings settings;

        public FileUnitOfWork(IObjectRepository objectRepository,
            IRelationshipRepository relationshipRepository,
            FileRepositorySettings settings)
        {
            this.objectRepository = objectRepository;
            this.relationshipRepository = relationshipRepository;
            this.settings = settings;
        }

        public void Save()
        {
            var result = new DependencyScanResult
            {
                Objects = objectRepository.GetAll().OrderBy(x => x.FullName).ThenBy(x => x.AssemblyName).ToArray(),
                Dependencies = relationshipRepository.Get(x => x.RelationshipKind == Model.RelationshipKind.Depends).OrderBy(x => x.From).ToArray(),
                Inheritences = relationshipRepository.Get(x => x.RelationshipKind == Model.RelationshipKind.Inherits).OrderBy(x => x.From).ToArray(),
                Implementations = relationshipRepository.Get(x => x.RelationshipKind == Model.RelationshipKind.Implements).OrderBy(x => x.From).ToArray()
            };

            var fileName = System.IO.Path.Combine(settings.OutputDir, string.Format(settings.FilenameTemplate, DateTime.Now));
            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new[] { new Newtonsoft.Json.Converters.StringEnumConverter() }
            };
            var json = JsonConvert.SerializeObject(result, jsonSettings);

            System.IO.File.WriteAllText(fileName, json);

        }

        public void Dispose()
        {
            objectRepository.DeleteAll();
            relationshipRepository.DeleteAll();
        }
    }

    public class DependencyScanResult
    {
        public Model.Object[] Objects { get; set; }
        public Model.Relationship[] Dependencies { get; set; }
        public Model.Relationship[] Inheritences { get; set; }
        public Model.Relationship[] Implementations { get; set; }
    }
}
