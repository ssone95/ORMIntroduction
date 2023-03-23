namespace ORMEntityFramework
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Please enter user's name: ");
            var name = Console.ReadLine();

            Console.WriteLine("Please enter user's email: ");
            var email = Console.ReadLine();

            var user = new User()
            {
                Name = name,
                Email = email
            };

            using (var dbContext = new AppDbContext())
            {
                var existingUser = dbContext.Users.SingleOrDefault(usr => usr.Name == "Petar");
                if (existingUser != null)
                {
                    dbContext.Users.Remove(existingUser);
                }
                var existingUser2 = dbContext.Users.SingleOrDefault(usr => usr.Id == 1);
                if (existingUser2 != null)
                {
                    dbContext.Users.Remove(existingUser2);
                }
                dbContext.SaveChanges();


                PrintExistingUsers(dbContext);

                Console.WriteLine("Inserted new user into Users table");

                dbContext.Users.Add(user);

                dbContext.SaveChanges();

                PrintExistingUsers(dbContext);
            }

            Console.ReadKey();
        }

        private static void PrintExistingUsers(AppDbContext dbContext)
        {
            var users = dbContext.Users.ToList();

            foreach (var usr in users)
            {
                Console.WriteLine($"User Id: {usr.Id}, Name: {usr.Name}, Email: {usr.Email}");
            }
        }
    }
}