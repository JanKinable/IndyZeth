using Elbanique.IndyZeth.Configuration;
using Elbanique.IndyZeth.Model;
using Elbanique.IndyZeth.Repositories;
using Serilog;
using System;
using System.Linq;
using System.Reflection;

namespace Elbanique.IndyZeth.Services
{
    public class TypeScanner : ITypeScanner
    {
        private static readonly ILogger logger = Log.Logger.ForContext<TypeScanner>();

        private readonly IObjectRepository objectRepository;
        private readonly IRelationshipRepository relationshipRepository;
        private readonly Settings settings;

        public TypeScanner(IObjectRepository objectRepository,
            IRelationshipRepository relationshipRepository,
            Settings settings)
        {
            this.objectRepository = objectRepository;
            this.relationshipRepository = relationshipRepository;
            this.settings = settings;
        }

        public void AddType(TypeInfo typeInfo)
        {
            logger.Information($"Scanning type {typeInfo.FullName}");

            //Skip those with no namespace, they have no influence on the dependencies
            if (typeInfo.Namespace == null) return;

            Type baseType = typeInfo.BaseType;
            var typeName = typeInfo.GetDisplayableName();
            
            var obj = new Model.Object
            {
                FullName = typeName,
                CompanyName = GetCompanyName(typeInfo.Namespace),
                AssemblyName = typeInfo.Assembly.FullName
            };
            if (typeInfo.IsValueType)
            {
                if (String.Equals(baseType?.FullName, "System.Enum", StringComparison.InvariantCulture))
                {
                    obj.ObjectKind = ObjectKind.Enum;
                }
                else
                {
                    obj.ObjectKind = ObjectKind.Struct;
                }
            }
            else if (typeInfo.IsInterface)
            {
                obj.ObjectKind = ObjectKind.Interface;
            }

            objectRepository.Insert(obj);
            
            if (typeInfo.IsClass && !String.Equals(baseType.FullName, "System.Object", StringComparison.InvariantCulture))
            {
                if (baseType.IsGenericType)
                {
                    if (baseType.IsArray || baseType.GetInterface("IEnumerable") != null)
                    {
                        //just interested in the dependency => dependency to the generic type of the collection so we can link to it
                        var genericTypeParameters = baseType.GetGenericArguments();
                        foreach (var gtp in genericTypeParameters)
                        {
                            relationshipRepository.Insert(new Relationship
                            {
                                From = typeName,
                                To = gtp.GetDisplayableName(),
                                RelationshipKind = RelationshipKind.Inherits
                            });
                        }
                    }
                    else
                    {
                        //just interested in the dependency => dependency to the base class (without generic implementation!) so we can link to it
                        relationshipRepository.Insert(new Relationship
                        {
                            From = typeName,
                            To = string.Join(".", baseType.Namespace, baseType.Name),
                            RelationshipKind = RelationshipKind.Inherits
                        });
                    }

                }
                else
                {
                    relationshipRepository.Insert(new Relationship
                    {
                        From = typeName,
                        To = baseType.GetDisplayableName(),
                        RelationshipKind = RelationshipKind.Inherits
                    });
                }

            }

            var interfaces = typeInfo.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                //skip the system types
                if (@interface.Namespace != null && @interface.Namespace.StartsWith("System")) continue;

                if (@interface.IsGenericType)
                {
                    //get the generic type (make it searchable) and list the generic arguments as dependencies
                    //except for collection
                    if (!@interface.IsArray || @interface.GetInterface("IEnumerable") != null)
                    {
                        relationshipRepository.Insert(new Relationship
                        {
                            From = typeName,
                            To = string.Join(".", @interface.Namespace, @interface.Name),
                            RelationshipKind = RelationshipKind.Implements
                        });
                    }
                    //gather the type arguments as dependencies
                    var genericTypeParameters = @interface.GetGenericArguments();
                    foreach (var gtp in genericTypeParameters)
                    {
                        //skip the system types
                        if (gtp.Namespace != null && gtp.Namespace.StartsWith("System")) continue;

                        //when the parameter is not declared (like 'T'), skip
                        var gtpName = gtp.GetDisplayableName();
                        if (gtpName == typeName) continue;

                        relationshipRepository.Insert(new Relationship
                        {
                            From = typeName,
                            To = gtpName,
                            RelationshipKind = RelationshipKind.Depends
                        });
                    }

                }
                else
                {
                    relationshipRepository.Insert(new Relationship
                    {
                        From = typeName,
                        To = @interface.GetDisplayableName(),
                        RelationshipKind = RelationshipKind.Implements
                    });
                }

            }

            var ctors = typeInfo.GetConstructors();
            foreach (var ctor in ctors)
            {
                var parameterInfos = ctor.GetParameters();
                foreach (var parameterInfo in parameterInfos)
                {
                    //skip the system types
                    if (parameterInfo.ParameterType.Namespace != null && parameterInfo.ParameterType.Namespace.StartsWith("System")) continue;

                    //we are only interested in the dependencies, so when a dependency is generic or collection, gather the generic types iso name
                    if (parameterInfo.ParameterType.IsGenericType)
                    {
                        var genericTypeParameters = parameterInfo.ParameterType.GetGenericArguments();
                        foreach (var gtp in genericTypeParameters)
                        {
                            //skip the system types
                            if (gtp.Namespace != null && gtp.Namespace.StartsWith("System")) continue;

                            relationshipRepository.Insert(new Relationship
                            {
                                From = typeName,
                                To = gtp.GetDisplayableName(),
                                RelationshipKind = RelationshipKind.Depends
                            });
                        }
                    }
                    else
                    {
                        relationshipRepository.Insert(new Relationship
                        {
                            From = typeName,
                            To = parameterInfo.ParameterType.GetDisplayableName(),
                            RelationshipKind = RelationshipKind.Depends
                        });
                    }
                }
            }
        }

        private string GetCompanyName(string ns)
        {
            if (ns == null) return "Undefined";

            var nsFirstPart = ns.Split(".").First();
            var map = settings.CompanyMappings.FirstOrDefault(x => x.NamespaceFirstPart.Equals(nsFirstPart, StringComparison.InvariantCultureIgnoreCase));
            if (map != null) return map.CompanyName;
            return nsFirstPart;
        }

    }
}
