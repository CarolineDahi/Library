using Library.Models.Base;
using Library.Models.Main;
using Library.Models.Security;
using Library.Models.Shared;
using Library.SharedKernel.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Library.SQL.Context
{
    public class LibraryDBContext : IdentityDbContext<User, IdentityRole<Guid>, Guid, IdentityUserClaim<Guid>,
    IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public LibraryDBContext(DbContextOptions<LibraryDBContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorBook> AuthorBooks { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PublishingHouse> PublishingHouses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Document> Documents { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.GlobalFilters<IEntityBase>(f => !f.DateDeleted.HasValue);
        }

        protected bool IsLogged()
        {
            if (httpContextAccessor?.HttpContext?.User != null)
                return httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            return false;
        }

        public Guid GetCurrentUserId()
        {
            var userId = Guid.Empty;
            if (IsLogged())
            {
                userId = Guid.Parse(httpContextAccessor.HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);
            }
            return userId;
        }

        protected virtual void BeforeSaveChanges()
        {
            Guid? actionBy = GetCurrentUserId();

            foreach (EntityEntry entry in ChangeTracker.Entries())
            {
                IEntityBase entity = entry.Entity as IEntityBase;
                if (entity is null) continue;

                switch (entry.State)
                {
                    case EntityState.Detached:

                        break;

                    case EntityState.Unchanged:

                        break;

                    case EntityState.Deleted:

                        break;

                    case EntityState.Modified:
                        if (entity.DateDeleted is null)
                        {
                            entity.UpdatedId = actionBy;
                            entity.DateUpdated = DateTime.Now.ToLocalTime();
                        }
                        else
                            entity.DeletedId = actionBy;

                        break;

                    case EntityState.Added:
                        entity.CreatedId = actionBy.Value;
                        entity.DateCreated = DateTime.Now.ToLocalTime();
                        break;
                }


            }
        }
    }
}
