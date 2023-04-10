using Microsoft.EntityFrameworkCore;
using PhoneBookEntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookDataLayer;

public class MyContext : DbContext
{
    public MyContext(DbContextOptions<MyContext> options) : base(options)
    {
        //ctora parametre verdik. Generic parametre Böylece connection string özelliğini OPSİYON olarak Program.cs üzerinden ayarlayacağız.
        
    }
    public DbSet<Member> MemberTable { get; set; }
    public DbSet<MemberPhone> MyProperty { get; set; }
    public DbSet<PhoneType> PhoneType { get; set; }
}
