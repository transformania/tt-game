﻿using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Interfaces;

namespace TT.Domain
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

            modelBuilder.Types<IRemovable>().Configure(ctc => ctc.Ignore(i => i.Removed));

            // find all classes that extend EntityTypeConfiguration and add them to Configurations
            modelBuilder.Configurations.AddFromAssembly(GetType().Assembly); 
        }
    }
}