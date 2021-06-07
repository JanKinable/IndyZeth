using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Elbanique.IndyZeth.Services
{
    public class DependencyLocator : IDependencyLocator
    {
        private readonly ILogger logger = Log.Logger.ForContext<DependencyLocator>();

        private readonly IAssemblyResolver assemblyResolver;
        private readonly ITypeScanner typeScanner;
        private readonly IUnitOfWork unitOfWork;

        public DependencyLocator(IAssemblyResolver assemblyResolver,
            ITypeScanner typeScanner,
            IUnitOfWork unitOfWork)
        {
            this.assemblyResolver = assemblyResolver;
            this.typeScanner = typeScanner;
            this.unitOfWork = unitOfWork;
        }

        public int HandleFiles(string[] filepaths)
        {
            using (assemblyResolver)
            {
                try
                {
                    using (unitOfWork)
                    {
                        foreach (var inputFile in filepaths)
                        {
                            Assembly assembly = assemblyResolver.LoadFromAssemblyPath(inputFile);

                            foreach (TypeInfo type in assembly.GetTypes())
                            {
                                typeScanner.AddType(type);
                            }
                        }
                        unitOfWork.Save();
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, "Something went wrong.");
                    return -1;
                }
            }

            return 0;
        }
    }
}
