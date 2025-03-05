using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace Psg.Standardised.Api.Common.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder AddCustomFunctions(this ModelBuilder modelBuilder)
        {
            var info = typeof(EntityFrameworkFunctions).GetMethod(nameof(EntityFrameworkFunctions.GetYear), new[] { typeof(DateOnly) });

            modelBuilder.HasDbFunction(info)
                .HasTranslation(e => new SqlFunctionExpression("year", e, nullable: false, argumentsPropagateNullability: new[] { false }, info.ReturnType, null))
                .HasParameter("date")
                .Metadata.TypeMapping = new DateOnlyTypeMapping("DateOnly");

            return modelBuilder;
        }
    }

    /// <summary>
    /// Custom EntityFramework functions that are translated to the SQL specified during setup.
    /// These should not be called directly and serve only as placeholders for EF.
    /// </summary>
    public static class EntityFrameworkFunctions
    {
        public static int GetYear(DateOnly date) => throw new NotSupportedException();
    }
}