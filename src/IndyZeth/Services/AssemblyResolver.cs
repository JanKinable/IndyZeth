using Elbanique.IndyZeth.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Elbanique.IndyZeth.Services
{
    public class AssemblyResolver : IAssemblyResolver
    {
        private MetadataLoadContext mlc;
        private readonly AssemblyResourceConfiguration resourceConfiguration;

        public AssemblyResolver(AssemblyResourceConfiguration resourceConfiguration)
        {
            this.resourceConfiguration = resourceConfiguration;
        }

        public Assembly LoadFromAssemblyPath(string inputFile)
        {
            var paths = new List<string>();

            var localAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
            paths.AddRange(localAssemblies);

            var targetAssemblies = Directory.GetFiles(Path.GetDirectoryName(inputFile), "*.dll");
            paths.AddRange(targetAssemblies);

            foreach (var additionalResources in resourceConfiguration.IncludeAssemblyPaths)
            {
                var additonalAssemblies = Directory.GetFiles(additionalResources, "*.dll");
                paths.AddRange(additonalAssemblies);
            }

            var resolver = new PathAssemblyResolver(paths);
            mlc = new MetadataLoadContext(resolver);

            return mlc.LoadFromAssemblyPath(inputFile);
        }

        public void Dispose()
        {
            if (mlc != null)
            {
                mlc.Dispose();
            }
        }
    }   
}
