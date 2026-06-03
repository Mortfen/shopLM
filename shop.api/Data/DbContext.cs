using shop.api.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace shop.api.Data
{
    public class DB : DbContext
    {


        public DB(DbContextOptions<DB> options) : base (options) { }





    }
}
