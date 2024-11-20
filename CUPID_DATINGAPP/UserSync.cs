using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUPID_DATINGAPP
{
    internal class UserSync
    {
        public class CurrentUser
        {
            private static CurrentUser _instance;

            // Singleton-Instanz
            public static CurrentUser Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new CurrentUser();
                    return _instance;
                }
            }

            // Benutzerdaten
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string Username { get; set; }
            public string Biography { get; set; }

            public string Hobbys { get; set; }

            public string Skills { get; set; }
            public string TargetAudience { get; set; }
            public byte[] ProfilePhoto { get; set; }

            // Private Konstruktor, um direkte Instanziierung zu verhindern
            private CurrentUser() { }

            // Methode zum Zurücksetzen der Benutzerdaten (z. B. beim Logout)
            public void Reset()
            {
                UserId = 0;
                FirstName = null;
                LastName = null;
                Email = null;
                DateOfBirth = DateTime.MinValue;
                Gender = null;
                Username = null;
                Biography = null;
                Hobbys = null;
                Skills = null;
                TargetAudience = null;
                ProfilePhoto = null;
            }
        }

    }
}
