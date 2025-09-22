namespace riwi_music.Models;

public class Artist : Person
{
    public string Genre { get; set; }

    public Artist(string name, string lastname, string email, string password, string genre) : base(name, lastname, email, password)
    {
        Id = IdCounter++;
        Name = name;
        Lastname = lastname;
        Email = email;
        Password = password;
        Genre = genre;
    }

    public string GetFullName()
    {
        return $"{Name} {Lastname}";
    }

    public bool IsAssociatedToConcert(Concert concert)
    {
        return concert.Artists.Contains(GetFullName());
    }
}