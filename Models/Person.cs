namespace riwi_music.Models;

class Person
{
    private int Id { get; set; }
    private string Name { get; set; }
    private string Lastname { get; set; }
    private string Email { get; set; }
    private string Password { get; set; }

    public static void Login()
    {
        
    }

    public void Register(string name, string lastname, string email, string password, string type)
    {
        Name = name;
        Lastname = lastname;
        Email = email;
        Password = password;
    }

    public static void Logout()
    {
        
    }
}