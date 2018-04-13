using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TT.Tests.Utilities
{
    public class MapBuilder : IMapBuilder
    {
        public MapBuilder()
        {
            Assemblies = new List<Assembly>();
            ProfileTypes = new List<Type>();
            Profiles = new List<Profile>();
        }

        public IMapBuilder AddAssemblies(params Assembly[] assembliesToScan) => AddAssemblies(assembliesToScan.AsEnumerable());
        
        public IMapBuilder AddAssemblies(IEnumerable<Assembly> assembliesToScan)
        {
            foreach (var assembly in assembliesToScan)
            {
                Assemblies.Add(assembly);
            }

            return this;
        }

        public IMapBuilder AddProfileInstances(params Profile[] profiles) => AddProfileInstances(profiles.AsEnumerable());

        public IMapBuilder AddProfileInstances(IEnumerable<Profile> profiles)
        {
            foreach (var profile in profiles)
            {
                Profiles.Add(profile);
            }

            return this;
        }

        public IMapBuilder AddProfileTypes(params Type[] profiles) => AddProfileTypes(profiles.AsEnumerable());

        public IMapBuilder AddProfileTypes(IEnumerable<Type> profiles)
        {
            foreach (var profile in profiles)
            {
                ProfileTypes.Add(profile);
            }

            return this;
        }

        public IMapper BuildMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                var allTypes = Assemblies.Where(a => !a.IsDynamic).SelectMany(a => a.DefinedTypes);

                var profilesTypes = allTypes
                        .Where(ExtendsProfile)
                        .Where(IsNotAbstract)
                        .Where(TypeHasEmptyOrDefaultConstr)
                        .Select(AsType)
                        .Concat(ProfileTypes);

                foreach (var profile in profilesTypes)
                {
                    cfg.AddProfile(profile);
                }

                foreach (var profile in Profiles)
                {
                    cfg.AddProfile(profile);
                }
            }).CreateMapper();
        }

        private IList<Assembly> Assemblies { get; }

        private IList<Type> ProfileTypes { get; }

        private IList<Profile> Profiles { get; }

        private bool TypeHasEmptyOrDefaultConstr(TypeInfo type) =>
                  type.GetConstructor(Type.EmptyTypes) != null ||
                  type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                    .Any(x => x.GetParameters().All(p => p.IsOptional));

        private bool ExtendsProfile(TypeInfo type) =>
             typeof(Profile).GetTypeInfo().IsAssignableFrom(type);

        private bool IsNotAbstract(TypeInfo type) =>
            !type.IsAbstract;

        private Type AsType(TypeInfo typeInfo) =>
            typeInfo.AsType();
    }

    public interface IMapBuilder
    {
        IMapper BuildMapper();
        IMapBuilder AddAssemblies(IEnumerable<Assembly> assembliesToScan);
        IMapBuilder AddAssemblies(params Assembly[] assembliesToScan);
        IMapBuilder AddProfileInstances(IEnumerable<Profile> profiles);
        IMapBuilder AddProfileInstances(params Profile[] profiles);
        IMapBuilder AddProfileTypes(IEnumerable<Type> profiles);
        IMapBuilder AddProfileTypes(params Type[] profiles);
    }
}
