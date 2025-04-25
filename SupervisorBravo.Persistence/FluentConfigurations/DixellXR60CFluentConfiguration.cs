using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Dixell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    /// <summary>
    /// Configuracion para la creacion de tablas y relaciones.
    /// </summary>
    internal class DixellXR60CFluentConfiguration : IEntityTypeConfiguration<DixellXR60CX>
    {
        public void Configure(EntityTypeBuilder<DixellXR60CX> builder)
        {
            builder.ToTable(nameof(DixellXR60CX));
            builder.HasBaseType(typeof(DixellBase));
            

        }
    }
}
