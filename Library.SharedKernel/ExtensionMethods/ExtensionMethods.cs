using Library.SharedKernel.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Library.SharedKernel.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static DocumentType TypeOfDocument(IFormFile file)
        {
            var type = new DocumentType();
            if (file.ContentType == "image/gif")
            {
                type = DocumentType.Image;
            }
            else if (file.ContentType == "application/pdf")
            {
                type = DocumentType.Document;
            }
            else
            {
                type = DocumentType.File;
            }
            return type;
        }

        public static void GlobalFilters<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> expression)
        {
            var entities = modelBuilder.Model.GetEntityTypes()
                                             .Where(e => e.ClrType.GetInterface(typeof(TInterface).Name) != null)
                                             .Select(e => e.ClrType);
            foreach (var entity in entities)
            {
                var newParam = Expression.Parameter(entity);
                var newbody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
                modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(newbody, newParam));
            }
        }
    }
}
