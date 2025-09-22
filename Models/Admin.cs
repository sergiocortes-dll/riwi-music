using System;
using System.Collections.Generic;
using System.Linq;

namespace riwi_music.Models
{
    public class Admin : Person
    {
        public static List<Concert> Concerts { get; } = new();
        public static List<Customer> Customers { get; } = new();
        public static List<Artist> Artists { get; } = new();
        public static List<Ticket> Tickets { get; } = new();
        public static Dictionary<int, List<Ticket>> TicketsByConcert { get; } = new();

        public Admin(string name, string lastname, string email, string password)
            : base(name, lastname, email, password)
        {
        }

        // Conciertos
        public static Concert RegisterConcert(
            DateTime date,
            string location,
            int capacity,
            Dictionary<string, decimal>? localities = null,
            IEnumerable<string>? artists = null)
        {
            var concert = new Concert(date, location, capacity);
            if (localities != null)
            {
                foreach (var kvp in localities)
                    concert.Localities[kvp.Key] = kvp.Value;
            }
            if (artists != null)
                concert.Artists.AddRange(artists);

            Concerts.Add(concert);
            TicketsByConcert[concert.Id] = new List<Ticket>();
            Console.WriteLine($"Concierto [{concert.Id}] registrado en {concert.Location} para {concert.Date:d}.");
            return concert;
        }

        public static IEnumerable<Concert> ListConcerts()
        {
            return Concerts;
        }

        public static bool EditConcert(
            int id,
            DateTime? date = null,
            string? location = null,
            int? capacity = null,
            Dictionary<string, decimal>? localities = null,
            IEnumerable<string>? artists = null)
        {
            var concert = Concerts.FirstOrDefault(c => c.Id == id);
            if (concert == null) return false;

            if (date.HasValue) concert.Date = date.Value;
            if (!string.IsNullOrWhiteSpace(location)) concert.Location = location;
            if (capacity.HasValue) concert.Capacity = capacity.Value;
            if (localities != null)
            {
                concert.Localities.Clear();
                foreach (var kvp in localities) concert.Localities[kvp.Key] = kvp.Value;
            }
            if (artists != null)
            {
                concert.Artists.Clear();
                concert.Artists.AddRange(artists);
            }

            Console.WriteLine($"Concierto [{concert.Id}] actualizado.");
            return true;
        }

        public static bool DeleteConcert(int id)
        {
            var concert = Concerts.FirstOrDefault(c => c.Id == id);
            if (concert == null) return false;

            // Eliminar tickets asociados
            if (TicketsByConcert.TryGetValue(id, out var ticketsForConcert))
            {
                foreach (var t in ticketsForConcert.ToList())
                {
                    Tickets.Remove(t);
                }
                TicketsByConcert.Remove(id);
            }

            Concerts.Remove(concert);
            Console.WriteLine($"Concierto [{id}] eliminado.");
            return true;
        }

        //  Clientes 
        public static Customer RegisterOrGetCustomer(string name, string lastname, string email, string password)
        {
            var existing = Customers.FirstOrDefault(c => string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase));
            if (existing != null) return existing;

            var customer = new Customer(name, lastname, email, password);
            Customers.Add(customer);
            Console.WriteLine($"Cliente [{customer.Id}] {customer.Name} registrado.");
            return customer;
        }

        public static IEnumerable<Customer> ListCustomers()
        {
            return Customers;
        }

        public static bool EditCustomer(int id, string? name = null, string? lastname = null, string? email = null, string? password = null)
        {
            var customer = Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return false;

            if (!string.IsNullOrWhiteSpace(name)) customer.Name = name;
            if (!string.IsNullOrWhiteSpace(lastname)) customer.Lastname = lastname;
            if (!string.IsNullOrWhiteSpace(email)) customer.Email = email;
            if (!string.IsNullOrWhiteSpace(password)) customer.Password = password;

            Console.WriteLine($"Cliente [{id}] actualizado.");
            return true;
        }

        public static bool DeleteCustomer(int id)
        {
            var customer = Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) return false;

            // Eliminar tickets del cliente de la colección global y de TicketsByConcert
            foreach (var t in customer.Purchases.ToList())
            {
                Tickets.Remove(t);

                // Buscar y eliminar de TicketsByConcert
                var pair = TicketsByConcert.FirstOrDefault(kvp => kvp.Value.Contains(t));
                if (!pair.Equals(default(KeyValuePair<int, List<Ticket>>)))
                    pair.Value.Remove(t);

                customer.Purchases.Remove(t);
            }

            Customers.Remove(customer);
            Console.WriteLine($"Cliente [{id}] eliminado.");
            return true;
        }

        //  Artistas 
        public static Artist RegisterOrGetArtist(string name, string lastname, string email, string password, string genre)
        {
            var existing = Artists.FirstOrDefault(a => string.Equals(a.Email, email, StringComparison.OrdinalIgnoreCase));
            if (existing != null) return existing;

            var artist = new Artist(name, lastname, email, password, genre);
            Artists.Add(artist);
            Console.WriteLine($"Artista [{artist.Id}] {artist.Name} registrado.");
            return artist;
        }

        public static IEnumerable<Artist> ListArtists()
        {
            return Artists;
        }

        public static bool EditArtist(int id, string? name = null, string? lastname = null, string? email = null, string? password = null, string? genre = null)
        {
            var artist = Artists.FirstOrDefault(a => a.Id == id);
            if (artist == null) return false;

            if (!string.IsNullOrWhiteSpace(name)) artist.Name = name;
            if (!string.IsNullOrWhiteSpace(lastname)) artist.Lastname = lastname;
            if (!string.IsNullOrWhiteSpace(email)) artist.Email = email;
            if (!string.IsNullOrWhiteSpace(password)) artist.Password = password;
            if (!string.IsNullOrWhiteSpace(genre)) artist.Genre = genre;

            Console.WriteLine($"Artista [{id}] actualizado.");
            return true;
        }

        public static bool DeleteArtist(int id)
        {
            var artist = Artists.FirstOrDefault(a => a.Id == id);
            if (artist == null) return false;
            Artists.Remove(artist);
            Console.WriteLine($"Artista [{id}] eliminado.");
            return true;
        }

        // Tiquetes 
        public static Ticket? RegisterTicketPurchase(Customer customer, int concertId, string locality)
        {
            var concert = Concerts.FirstOrDefault(c => c.Id == concertId);
            if (concert == null)
            {
                Console.WriteLine("Concierto no encontrado.");
                return null;
            }

            if (!concert.Localities.TryGetValue(locality, out var price))
            {
                Console.WriteLine("Localidad no válida.");
                return null;
            }

            var sold = TicketsByConcert.TryGetValue(concertId, out var soldList) ? soldList.Count : 0;
            if (sold >= concert.Capacity)
            {
                Console.WriteLine("No quedan entradas disponibles para este concierto.");
                return null;
            }

            var ticket = new Ticket(price, locality, concert.Date);
            Tickets.Add(ticket);

            if (!TicketsByConcert.ContainsKey(concertId)) TicketsByConcert[concertId] = new List<Ticket>();
            TicketsByConcert[concertId].Add(ticket);

            customer.BuyTicket(ticket);

            Console.WriteLine($"Tiquete [{ticket.Id}] comprado por {customer.Name} en {locality} por {price:C}.");
            return ticket;
        }

        public static IEnumerable<Ticket> ListTickets()
        {
            return Tickets;
        }

        public static IEnumerable<Ticket> ListTicketsByConcert(int concertId)
        {
            if (TicketsByConcert.TryGetValue(concertId, out var list)) return list;
            return Enumerable.Empty<Ticket>();
        }

        public static bool DeleteTicket(int ticketId)
        {
            var ticket = Tickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket == null) return false;

            // Remover de TicketsByConcert
            var pair = TicketsByConcert.FirstOrDefault(kvp => kvp.Value.Contains(ticket));
            if (!pair.Equals(default(KeyValuePair<int, List<Ticket>>)))
                pair.Value.Remove(ticket);

            // Remover de Purchases de cualquier cliente que lo tenga
            foreach (var c in Customers)
            {
                if (c.Purchases.Contains(ticket))
                {
                    c.Purchases.Remove(ticket);
                    break;
                }
            }

            Tickets.Remove(ticket);
            Console.WriteLine($"Tiquete [{ticketId}] eliminado.");
            return true;
        }

        // Consultas LINQ 
        public static IEnumerable<Concert> GetConcertsByCity(string city)
        {
            return Concerts.Where(c => string.Equals(c.Location, city, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<Concert> GetConcertsByDateRange(DateTime from, DateTime to)
        {
            var fromDate = from.Date;
            var toDate = to.Date;
            return Concerts.Where(c => c.Date.Date >= fromDate && c.Date.Date <= toDate);
        }

        public static Concert? GetConcertWithMostTickets()
        {
            return Concerts
                .OrderByDescending(c => TicketsByConcert.TryGetValue(c.Id, out var list) ? list.Count : 0)
                .FirstOrDefault();
        }

        public static decimal GetTotalIncomeOfConcert(int concertId)
        {
            if (!TicketsByConcert.TryGetValue(concertId, out var list)) return 0m;
            return list.Sum(t => t.Price);
        }

        public static Customer? GetCustomerWithMostPurchases()
        {
            return Customers.OrderByDescending(c => c.Purchases.Count).FirstOrDefault();
        }
    }
}
