
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NSE.Identity.API {
  class ApplicationDBContext : IdentityDbContext
  {
      public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
      {
        
      }
  }
}