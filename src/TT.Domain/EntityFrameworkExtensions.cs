using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DelegateDecompiler;
using DelegateDecompiler.EntityFramework;
using TT.Domain.Interfaces;

namespace TT.Domain
{
    public static class EntityFrameworkExtensions
    {
        public static async Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable)
        {
            return await queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).DecompileAsync().ToListAsync();
        }

        public static async Task<TDestination[]> ProjectToArrayAsync<TDestination>(this IQueryable queryable)
        {
            return await queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).DecompileAsync().ToArrayAsync();
        }

        public static async Task<TDestination> ProjectToSingleOrDefaultAsync<TDestination>(this IQueryable queryable)
        {
            return await queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).DecompileAsync().SingleOrDefaultAsync();
        }

        public static async Task<TDestination> ProjectToSingleAsync<TDestination>(this IQueryable queryable)
        {
            return await queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).DecompileAsync().SingleAsync();
        }

        public static async Task<TDestination> ProjectToFirstOrDefaultAsync<TDestination>(this IQueryable queryable)
        {
            return await queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).DecompileAsync().FirstOrDefaultAsync();
        }

        public static async Task<TDestination> ProjectToFirstAsync<TDestination>(this IQueryable queryable)
        {
            return await queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).DecompileAsync().FirstAsync();
        }

        public static List<TDestination> ProjectToList<TDestination>(this IQueryable queryable)
        {
            return queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).Decompile().ToList();
        }

        public static TDestination[] ProjectToArray<TDestination>(this IQueryable queryable)
        {
            return queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).Decompile().ToArray();
        }

        public static TDestination ProjectToSingleOrDefault<TDestination>(this IQueryable queryable)
        {
            return queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).Decompile().SingleOrDefault();
        }

        public static TDestination ProjectToSingle<TDestination>(this IQueryable queryable)
        {
            return queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).Decompile().Single();
        }

        public static TDestination ProjectToFirstOrDefault<TDestination>(this IQueryable queryable)
        {
            return queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).Decompile().FirstOrDefault();
        }

        public static TDestination ProjectToFirst<TDestination>(this IQueryable queryable)
        {
            return queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).Decompile().First();
        }

        public static IQueryable<TDestination> ProjectToQueryable<TDestination>(this IQueryable queryable)
        {
            return queryable.ProjectTo<TDestination>(DomainRegistry.Mapper.ConfigurationProvider).Decompile();
        }

        public static void DeleteAll(this IEnumerable<IDeletable> source)
        {
            foreach (var entity in source)
            {
                entity.Delete();
            }
        }
    }
}