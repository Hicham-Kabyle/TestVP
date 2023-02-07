using SocleRHHBESSAIH.Models;

namespace SocleRHHBESSAIH.Data
{
    public class DbInitializer
    {
        public static void Initialize(SocleRHHBContext context)
        {
            // Look for any students.
            if (context.Users.Any())
            {
                return;
            }

            var users = new User[]
            {
                new User{FirstName="Stark",LastName="Anthony",UserCurrency="Dollar américain"},
                new User{FirstName="Romanova",LastName="Natasha",UserCurrency="Rouble russe"}
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
