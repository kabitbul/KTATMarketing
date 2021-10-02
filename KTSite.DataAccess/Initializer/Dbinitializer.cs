using KTSite.DataAccess.Data;
using KTSite.Models;
using KTSite.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KTSite.DataAccess.Initializer
{
    public class Dbinitializer : IDbinitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
       
        public Dbinitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
              try
            {
                int result = 0;
                IEnumerator<string> pendMigrations = _db.Database.GetPendingMigrations().GetEnumerator();
                using (IEnumerator<string> enumerator = pendMigrations)
                {
                    while (enumerator.MoveNext())
                    {
                        result++;
                        break;
                    }
                }
                if (result > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {

            }

            if (!_db.Roles.Any(r => r.Name == SD.Role_KTMerch))
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_KTMerch)).GetAwaiter().GetResult();
            }
            if (!_db.Roles.Any(r => r.Name == SD.Role_ExMerch))
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_ExMerch)).GetAwaiter().GetResult();
            }
            if (_db.Roles.Any(r => r.Name == SD.Role_Admin)) return;
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Users)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_VAs)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Warehouse)).GetAwaiter().GetResult();
            
            

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "kabitbul@gmail.com",
                Email = "kabitbul@gmail.com",
                EmailConfirmed = true,
                Name = "Kobi Abitbul",
                Role = SD.Role_Admin
            },"KTAdmin123*").GetAwaiter().GetResult();
            ApplicationUser user = _db.ApplicationUser.Where(a => a.Email == "kabitbul@gmail.com").FirstOrDefault();
            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
        }
    }
}
