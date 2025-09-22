namespace riwi_music.Models;

public class Person
{
    protected static int IdCounter = 1;

    public int Id { get; protected set; }
    public string Name { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    protected Person() { }

    protected Person(string name, string lastname, string email, string password)
    {
        Id = IdCounter++;
        Name = name;
        Lastname = lastname;
        Email = email;
        Password = password;
    }
}