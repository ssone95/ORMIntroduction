using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks.Dataflow;

namespace ORMEntityFramework
{
    internal class Program
    {
        static void Main(string[] args)
        {


            using (var dbContext = new AppDbContext())
            {
                PrintExistingUsers(dbContext);

                while (true)
                {
                    Console.WriteLine("Do you want to \n" +
                        "a) Create a new user\n" +
                        "b) Create a new address\n" +
                        "c) Update existing user's address\n" +
                        "d) Delete existing user's address\n" +
                        "e) Delete existing address\n" +
                        "l) List existing users\n\n" +
                        "r) Create a new role\n\n" +
                        "t) List roles\n\n" +
                        "w) Assign role to user\n\n" +
                        "q) Quit");

                    string commandStr = Console.ReadLine();
                    if (string.IsNullOrEmpty(commandStr))
                    {
                        Console.WriteLine("You entered an invalid command, please try again!");
                        continue;
                    }

                    char command = commandStr[0];

                    if (command == 'q')
                    {
                        break;
                    }

                    switch (command)
                    {
                        case 'a':
                            {
                                // Add user
                                AddUser(dbContext);
                                break;
                            }
                        case 'b':
                            {
                                // Add address
                                AddAddress(dbContext);
                                break;
                            }
                        case 'c':
                            {
                                // Update user's address
                                UpdateUsersAddress(dbContext);
                                break;
                            }
                        case 'd':
                            {
                                // Delete user's current address
                                DeleteUsersAddress(dbContext);
                                break;
                            }
                        case 'e':
                            {
                                DeleteAddress(dbContext);
                                break;
                            }
                        case 'l':
                            {
                                // Print existing users
                                PrintExistingUsers(dbContext);
                                break;
                            }

                        case 'r':
                            {
                                CreateRole(dbContext);
                                break;
                            }
                        case 't':
                            {
                                ListRoles(dbContext);
                                break;
                            }
                        case 'w':
                            {
                                AssignRoleToUser(dbContext);
                                break;
                            }
                        default:
                            {
                                // Let the user know that they entered the incorrect command
                                break;
                            }
                    }
                }
            }
        }

        private static void AssignRoleToUser(AppDbContext dbContext)
        {
            Console.WriteLine($"Please enter the user id: ");
            int userId = int.Parse(Console.ReadLine());
            Console.WriteLine($"Please enter the role id: ");
            int roleId = int.Parse(Console.ReadLine());

            var existingRole = dbContext.UserRoles.Where(ur => ur.RoleId == roleId && ur.UserId == userId).SingleOrDefault();
            if (existingRole == null)
            {
                var userRole = new UserRole()
                {
                    RoleId = roleId,
                    UserId = userId,
                    Active = true
                };
                dbContext.UserRoles.Add(userRole);
                dbContext.SaveChanges();
            }
        }

        private static Role CreateRole(AppDbContext dbContext)
        {
            var role = new Role() { Active = true };
            Console.WriteLine($"Please enter the role name: ");
            role.Name = Console.ReadLine();

            dbContext.Roles.Add(role);

            dbContext.SaveChanges();
            return role;
        }


        private static void ListRoles(AppDbContext dbContext)
        {
            var roles = dbContext
                .Roles
                .Where(role => role.Active)
                .ToList();

            foreach (var role in roles)
            {
                Console.WriteLine($"Role Id: {role.Id}, Role Name: {role.Name}");
            }
        }

        private static void DeleteUsersAddress(AppDbContext dbContext)
        {
            Console.WriteLine("Please specify the user id to update: ");
            int userId = Int32.Parse(Console.ReadLine());

            var user = dbContext.Users.Include(u => u.UserAddress).SingleOrDefault(user => user.Id == userId);
            if (user != null)
            {
                var addressId = user.UserAddress?.AddressId;
                if (addressId != null)
                {
                    var addressFromDb = dbContext.Addresses.SingleOrDefault(adr => adr.Id == addressId);
                    if (addressFromDb != null)
                    {
                        addressFromDb.Active = false;
                        dbContext.SaveChanges();
                    }
                }
            }
        }
        private static void DeleteAddress(AppDbContext dbContext)
        {
            Console.WriteLine("Please specify the addres id to delete: ");
            int addressId = Int32.Parse(Console.ReadLine());

            var userAddressesCount = dbContext.UserAddresses.Any(ua => ua.AddressId == addressId);

            if (!userAddressesCount)
            {
                var addressFromDb = dbContext.Addresses.SingleOrDefault(adr => adr.Id == addressId);
                if (addressFromDb != null)
                {
                    addressFromDb.Active = false;
                    dbContext.SaveChanges();
                }
            } 
            else
            {
                Console.WriteLine($"Please remove references from address with id {addressId} from UserAddresses table!");
            }
        }

        private static User AddUser(AppDbContext dbContext)
        {
            Console.WriteLine("Please enter user's name: ");
            var name = Console.ReadLine();

            Console.WriteLine("Please enter user's email: ");
            var email = Console.ReadLine();

            var user = new User()
            {
                Name = name,
                Email = email,
                Active = true
            };

            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return user;
        }

        private static Address AddAddress(AppDbContext dbContext)
        {

            var address = new Address() { Active = true };
            Console.WriteLine($"User, please let us know your City: ");
            address.City = Console.ReadLine();

            Console.WriteLine($"User, please let us know your Street: ");
            address.Street = Console.ReadLine();

            Console.WriteLine($"User, please let us know your State: ");
            address.State = Console.ReadLine();

            dbContext.Addresses.Add(address);

            dbContext.SaveChanges();
            return address;
        }

        private static void UpdateUsersAddress(AppDbContext dbContext)
        {
            Console.WriteLine("Please specify the user id to update: ");
            int userId = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Please specify the address id to user: ");
            int addressId = Int32.Parse(Console.ReadLine());

            var user2 = dbContext.UserAddresses.SingleOrDefault(userAddress => userAddress.UserId == userId);

            if (user2 != null)
            {
                user2.AddressId = addressId;

                dbContext.SaveChanges();
            } 
            else
            {
                dbContext.UserAddresses.Add(new UserAddress { UserId = userId, AddressId = addressId, Active = true });
                dbContext.SaveChanges();
            }
        }

        private static void PrintExistingUsers(AppDbContext dbContext)
        {
            var users = dbContext
                .Users
                .Include(user => user.UserAddress)
                .ThenInclude(userAddres => userAddres.Address)
                .Include(user => user.UserRoles)
                .ThenInclude(userRole => userRole.Role)
                .Where(user => user.Active)
                .ToList();

            foreach (var usr in users)
            {
                var address = usr.UserAddress?.Address;
                var userRolesList = GetUserRoles(usr);
                Console.WriteLine($"User Id: {usr.Id}, Name: {usr.Name}, Email: {usr.Email}, AddressId: {address?.Id}, Address: {address?.Street}, {address?.City}, {address?.State}, Roles: [{userRolesList}]");
            }
        }

        private static string GetUserRoles(User user)
        {
            if (user == null)
            {
                return string.Empty;
            }

            if (user.UserRoles == null || user.UserRoles.Count < 1)
            {
                return string.Empty;
            }

            string allUserRoles = string.Empty;
            foreach (var userRole in user.UserRoles)
            {
                allUserRoles += $"{userRole.Role?.Name},";
            }

            return allUserRoles;
        }
    }
}