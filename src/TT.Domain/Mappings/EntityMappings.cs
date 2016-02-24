using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;

namespace TT.Domain.Mappings
{
    /// <summary>
    /// Finds all IMappingConfiguration implementors and uses them to configure EF entity mapping
    /// </summary>
    public class EntityMappings : IMappingConfiguration
    {
        public void ConfigureModelBuilder(DbModelBuilder modelBuilder)
        {
            var mcType = typeof(IMappingConfiguration);
            var mappings = GetType().Assembly.GetTypes().Where(t => mcType.IsAssignableFrom(t) && t != GetType());

            foreach (var mapping in mappings)
                ((IMappingConfiguration)Activator.CreateInstance(mapping)).ConfigureModelBuilder(modelBuilder);
        }
    }
}