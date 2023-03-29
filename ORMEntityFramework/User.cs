using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMEntityFramework
{
    public class User : BaseEntry
    {
        public string Name { get; set; }

        public string Email { get; set; }
        public UserAddress UserAddress { get; set; }


        public ICollection<UserRole> UserRoles { get; set; }

    }
}
