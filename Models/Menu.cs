using System;
using System.Linq;

namespace riwi_music.Models
{
    public class Menu
    {
        public static int TryConvertInputToNumber()
        {
            int toNumber;
            while (true)
            {
                var input = Console.ReadLine();
                if (int.TryParse(input, out toNumber))
                    return toNumber;

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nOpción inválida. Ingrese un número.");
                Console.ResetColor();
            }
        }

        public static void EnterToContinue()
        {
            Console.WriteLine("\nPresiona Enter para continuar...");
            Console.ReadLine();
            Console.Clear();
        }

        // ADMIN 
        private static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== MENÚ ADMIN ===");
                Console.WriteLine("1) Gestionar Conciertos");
                Console.WriteLine("2) Gestionar Clientes");
                Console.WriteLine("3) Gestionar Artistas");
                Console.WriteLine("4) Gestionar Tiquetes");
                Console.WriteLine("5) Consultas avanzadas (LINQ)");
                Console.WriteLine("0) Volver");
                Console.Write("> ");

                var option = TryConvertInputToNumber();

                switch (option)
                {
                    case 1:
                        ConcertManagementMenu();
                        break;
                    case 2:
                        CustomerManagementMenu();
                        break;
                    case 3:
                        ArtistManagementMenu();
                        break;
                    case 4:
                        TicketManagementMenu();
                        break;
                    case 5:
                        AdvancedQueriesMenu();
                        break;
                    case 0:
                        return;
                }
            }
        }

        //  GESTIÓN DE CONCIERTOS 
        private static void ConcertManagementMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== GESTIÓN DE CONCIERTOS ===");
                Console.WriteLine("1) Registrar concierto");
                Console.WriteLine("2) Listar conciertos");
                Console.WriteLine("3) Editar concierto");
                Console.WriteLine("4) Eliminar concierto");
                Console.WriteLine("0) Volver");
                Console.Write("> ");

                var option = TryConvertInputToNumber();

                switch (option)
                {
                    case 1:
                        RegisterConcert();
                        break;
                    case 2:
                        ListConcerts();
                        break;
                    case 3:
                        EditConcert();
                        break;
                    case 4:
                        DeleteConcert();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private static void RegisterConcert()
        {
            try
            {
                Console.Write("Fecha (yyyy-mm-dd): ");
                var date = DateTime.Parse(Console.ReadLine()!);
                Console.Write("Lugar: ");
                var loc = Console.ReadLine()!;
                Console.Write("Capacidad: ");
                var cap = int.Parse(Console.ReadLine()!);

                Console.WriteLine("\n¿Desea configurar precios personalizados? (s/n):");
                var customPrices = Console.ReadLine()?.ToLower() == "s";
                
                Dictionary<string, decimal>? localities = null;
                if (customPrices)
                {
                    localities = new Dictionary<string, decimal>();
                    Console.Write("Precio VIP: ");
                    localities["VIP"] = decimal.Parse(Console.ReadLine()!);
                    Console.Write("Precio Platea: ");
                    localities["Platea"] = decimal.Parse(Console.ReadLine()!);
                    Console.Write("Precio General: ");
                    localities["General"] = decimal.Parse(Console.ReadLine()!);
                }

                Admin.RegisterConcert(date, loc, cap, localities, null);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error al registrar concierto: {ex.Message}");
                Console.ResetColor();
            }
            EnterToContinue();
        }

        private static void ListConcerts()
        {
            var concerts = Admin.ListConcerts().ToList();
            if (!concerts.Any())
            {
                Console.WriteLine("No hay conciertos registrados.");
            }
            else
            {
                Console.WriteLine("\n=== LISTA DE CONCIERTOS ===");
                foreach (var c in concerts)
                {
                    var ticketCount = Admin.ListTicketsByConcert(c.Id).Count();
                    Console.WriteLine($"[{c.Id}] {c.Date:d} - {c.Location} - Cap: {c.Capacity} - Vendidos: {ticketCount}");
                    
                    if (c.Artists.Any())
                        Console.WriteLine($"    Artistas: {string.Join(", ", c.Artists)}");
                    
                    Console.WriteLine($"    Precios: VIP {c.Localities["VIP"]:C}, Platea {c.Localities["Platea"]:C}, General {c.Localities["General"]:C}");
                }
            }
            EnterToContinue();
        }

        private static void EditConcert()
        {
            var concerts = Admin.ListConcerts().ToList();
            if (!concerts.Any())
            {
                Console.WriteLine("No hay conciertos para editar.");
                EnterToContinue();
                return;
            }

            Console.WriteLine("\n=== CONCIERTOS DISPONIBLES ===");
            foreach (var c in concerts)
                Console.WriteLine($"[{c.Id}] {c.Date:d} - {c.Location}");

            Console.Write("\nSeleccione el ID del concierto a editar: ");
            var id = TryConvertInputToNumber();

            var concert = concerts.FirstOrDefault(c => c.Id == id);
            if (concert == null)
            {
                Console.WriteLine("Concierto no encontrado.");
                EnterToContinue();
                return;
            }

            Console.WriteLine($"\nEditando concierto: {concert.Date:d} - {concert.Location}");
            Console.WriteLine("Deje en blanco para mantener el valor actual.\n");

            try
            {
                Console.Write($"Nueva fecha ({concert.Date:yyyy-MM-dd}): ");
                var dateInput = Console.ReadLine();
                DateTime? newDate = string.IsNullOrWhiteSpace(dateInput) ? null : DateTime.Parse(dateInput);

                Console.Write($"Nuevo lugar ({concert.Location}): ");
                var newLocation = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newLocation)) newLocation = null;

                Console.Write($"Nueva capacidad ({concert.Capacity}): ");
                var capacityInput = Console.ReadLine();
                int? newCapacity = string.IsNullOrWhiteSpace(capacityInput) ? null : int.Parse(capacityInput);

                Console.WriteLine("¿Desea actualizar precios? (s/n):");
                Dictionary<string, decimal>? newLocalities = null;
                if (Console.ReadLine()?.ToLower() == "s")
                {
                    newLocalities = new Dictionary<string, decimal>();
                    Console.Write($"Precio VIP ({concert.Localities["VIP"]:C}): ");
                    var vipInput = Console.ReadLine();
                    newLocalities["VIP"] = string.IsNullOrWhiteSpace(vipInput) ? concert.Localities["VIP"] : decimal.Parse(vipInput);
                    
                    Console.Write($"Precio Platea ({concert.Localities["Platea"]:C}): ");
                    var plateaInput = Console.ReadLine();
                    newLocalities["Platea"] = string.IsNullOrWhiteSpace(plateaInput) ? concert.Localities["Platea"] : decimal.Parse(plateaInput);
                    
                    Console.Write($"Precio General ({concert.Localities["General"]:C}): ");
                    var generalInput = Console.ReadLine();
                    newLocalities["General"] = string.IsNullOrWhiteSpace(generalInput) ? concert.Localities["General"] : decimal.Parse(generalInput);
                }

                Admin.EditConcert(id, newDate, newLocation, newCapacity, newLocalities);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error al editar concierto: {ex.Message}");
                Console.ResetColor();
            }
            EnterToContinue();
        }

        private static void DeleteConcert()
        {
            var concerts = Admin.ListConcerts().ToList();
            if (!concerts.Any())
            {
                Console.WriteLine("No hay conciertos para eliminar.");
                EnterToContinue();
                return;
            }

            Console.WriteLine("\n=== CONCIERTOS DISPONIBLES ===");
            foreach (var c in concerts)
            {
                var ticketCount = Admin.ListTicketsByConcert(c.Id).Count();
                Console.WriteLine($"[{c.Id}] {c.Date:d} - {c.Location} - Tickets vendidos: {ticketCount}");
            }

            Console.Write("\nSeleccione el ID del concierto a eliminar: ");
            var id = TryConvertInputToNumber();

            var concert = concerts.FirstOrDefault(c => c.Id == id);
            if (concert == null)
            {
                Console.WriteLine("Concierto no encontrado.");
                EnterToContinue();
                return;
            }

            var ticketsSold = Admin.ListTicketsByConcert(id).Count();
            Console.WriteLine($"\nVa a eliminar: {concert.Date:d} - {concert.Location}");
            if (ticketsSold > 0)
                Console.WriteLine($"ADVERTENCIA: Este concierto tiene {ticketsSold} tickets vendidos que también se eliminarán.");
            
            Console.Write("¿Está seguro? (s/n): ");
            if (Console.ReadLine()?.ToLower() == "s")
            {
                Admin.DeleteConcert(id);
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
            }
            EnterToContinue();
        }

        //  GESTIÓN DE CLIENTES 
        private static void CustomerManagementMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== GESTIÓN DE CLIENTES ===");
                Console.WriteLine("1) Registrar cliente");
                Console.WriteLine("2) Listar clientes");
                Console.WriteLine("3) Editar cliente");
                Console.WriteLine("4) Eliminar cliente");
                Console.WriteLine("0) Volver");
                Console.Write("> ");

                var option = TryConvertInputToNumber();

                switch (option)
                {
                    case 1:
                        RegisterCustomer();
                        break;
                    case 2:
                        ListCustomers();
                        break;
                    case 3:
                        EditCustomer();
                        break;
                    case 4:
                        DeleteCustomer();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private static void RegisterCustomer()
        {
            Console.Write("Nombre: ");
            var name = Console.ReadLine()!;
            Console.Write("Apellido: ");
            var lastname = Console.ReadLine()!;
            Console.Write("Email: ");
            var email = Console.ReadLine()!;
            Console.Write("Contraseña: ");
            var pass = Console.ReadLine()!;
            Admin.RegisterOrGetCustomer(name, lastname, email, pass);
            EnterToContinue();
        }

        private static void ListCustomers()
        {
            var customers = Admin.ListCustomers().ToList();
            if (!customers.Any())
            {
                Console.WriteLine("No hay clientes registrados.");
            }
            else
            {
                Console.WriteLine("\n=== LISTA DE CLIENTES ===");
                foreach (var c in customers)
                {
                    Console.WriteLine($"[{c.Id}] {c.Name} {c.Lastname} - {c.Email} - Compras: {c.Purchases.Count}");
                }
            }
            EnterToContinue();
        }

        private static void EditCustomer()
        {
            var customers = Admin.ListCustomers().ToList();
            if (!customers.Any())
            {
                Console.WriteLine("No hay clientes para editar.");
                EnterToContinue();
                return;
            }

            Console.WriteLine("\n=== CLIENTES DISPONIBLES ===");
            foreach (var c in customers)
                Console.WriteLine($"[{c.Id}] {c.Name} {c.Lastname} - {c.Email}");

            Console.Write("\nSeleccione el ID del cliente a editar: ");
            var id = TryConvertInputToNumber();

            var customer = customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                Console.WriteLine("Cliente no encontrado.");
                EnterToContinue();
                return;
            }

            Console.WriteLine($"\nEditando cliente: {customer.Name} {customer.Lastname}");
            Console.WriteLine("Deje en blanco para mantener el valor actual.\n");

            Console.Write($"Nuevo nombre ({customer.Name}): ");
            var newName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newName)) newName = null;

            Console.Write($"Nuevo apellido ({customer.Lastname}): ");
            var newLastname = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newLastname)) newLastname = null;

            Console.Write($"Nuevo email ({customer.Email}): ");
            var newEmail = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newEmail)) newEmail = null;

            Console.Write("Nueva contraseña: ");
            var newPassword = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newPassword)) newPassword = null;

            Admin.EditCustomer(id, newName, newLastname, newEmail, newPassword);
            EnterToContinue();
        }

        private static void DeleteCustomer()
        {
            var customers = Admin.ListCustomers().ToList();
            if (!customers.Any())
            {
                Console.WriteLine("No hay clientes para eliminar.");
                EnterToContinue();
                return;
            }

            Console.WriteLine("\n=== CLIENTES DISPONIBLES ===");
            foreach (var c in customers)
                Console.WriteLine($"[{c.Id}] {c.Name} {c.Lastname} - {c.Email} - Compras: {c.Purchases.Count}");

            Console.Write("\nSeleccione el ID del cliente a eliminar: ");
            var id = TryConvertInputToNumber();

            var customer = customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
            {
                Console.WriteLine("Cliente no encontrado.");
                EnterToContinue();
                return;
            }

            Console.WriteLine($"\nVa a eliminar: {customer.Name} {customer.Lastname} - {customer.Email}");
            if (customer.Purchases.Any())
                Console.WriteLine($"ADVERTENCIA: Este cliente tiene {customer.Purchases.Count} compras que también se eliminarán.");
            
            Console.Write("¿Está seguro? (s/n): ");
            if (Console.ReadLine()?.ToLower() == "s")
            {
                Admin.DeleteCustomer(id);
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
            }
            EnterToContinue();
        }

        //  GESTIÓN DE ARTISTAS 
        private static void ArtistManagementMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== GESTIÓN DE ARTISTAS ===");
                Console.WriteLine("1) Registrar artista");
                Console.WriteLine("2) Listar artistas");
                Console.WriteLine("3) Editar artista");
                Console.WriteLine("4) Eliminar artista");
                Console.WriteLine("0) Volver");
                Console.Write("> ");

                var option = TryConvertInputToNumber();

                switch (option)
                {
                    case 1:
                        RegisterArtist();
                        break;
                    case 2:
                        ListArtists();
                        break;
                    case 3:
                        EditArtist();
                        break;
                    case 4:
                        DeleteArtist();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private static void RegisterArtist()
        {
            Console.Write("Nombre artista: ");
            var aname = Console.ReadLine()!;
            Console.Write("Apellido: ");
            var alast = Console.ReadLine()!;
            Console.Write("Email: ");
            var aemail = Console.ReadLine()!;
            Console.Write("Contraseña: ");
            var apass = Console.ReadLine()!;
            Console.Write("Género: ");
            var genre = Console.ReadLine()!;
            Admin.RegisterOrGetArtist(aname, alast, aemail, apass, genre);
            EnterToContinue();
        }

        private static void ListArtists()
        {
            var artists = Admin.ListArtists().ToList();
            if (!artists.Any())
            {
                Console.WriteLine("No hay artistas registrados.");
            }
            else
            {
                Console.WriteLine("\n=== LISTA DE ARTISTAS ===");
                foreach (var a in artists)
                    Console.WriteLine($"[{a.Id}] {a.Name} {a.Lastname} - {a.Email} - Género: {a.Genre}");
            }
            EnterToContinue();
        }

        private static void EditArtist()
        {
            var artists = Admin.ListArtists().ToList();
            if (!artists.Any())
            {
                Console.WriteLine("No hay artistas para editar.");
                EnterToContinue();
                return;
            }

            Console.WriteLine("\n=== ARTISTAS DISPONIBLES ===");
            foreach (var a in artists)
                Console.WriteLine($"[{a.Id}] {a.Name} {a.Lastname} - {a.Genre}");

            Console.Write("\nSeleccione el ID del artista a editar: ");
            var id = TryConvertInputToNumber();

            var artist = artists.FirstOrDefault(a => a.Id == id);
            if (artist == null)
            {
                Console.WriteLine("Artista no encontrado.");
                EnterToContinue();
                return;
            }

            Console.WriteLine($"\nEditando artista: {artist.Name} {artist.Lastname}");
            Console.WriteLine("Deje en blanco para mantener el valor actual.\n");

            Console.Write($"Nuevo nombre ({artist.Name}): ");
            var newName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newName)) newName = null;

            Console.Write($"Nuevo apellido ({artist.Lastname}): ");
            var newLastname = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newLastname)) newLastname = null;

            Console.Write($"Nuevo email ({artist.Email}): ");
            var newEmail = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newEmail)) newEmail = null;

            Console.Write("Nueva contraseña: ");
            var newPassword = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newPassword)) newPassword = null;

            Console.Write($"Nuevo género ({artist.Genre}): ");
            var newGenre = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newGenre)) newGenre = null;

            Admin.EditArtist(id, newName, newLastname, newEmail, newPassword, newGenre);
            EnterToContinue();
        }

        private static void DeleteArtist()
        {
            var artists = Admin.ListArtists().ToList();
            if (!artists.Any())
            {
                Console.WriteLine("No hay artistas para eliminar.");
                EnterToContinue();
                return;
            }

            Console.WriteLine("\n=== ARTISTAS DISPONIBLES ===");
            foreach (var a in artists)
                Console.WriteLine($"[{a.Id}] {a.Name} {a.Lastname} - {a.Genre}");

            Console.Write("\nSeleccione el ID del artista a eliminar: ");
            var id = TryConvertInputToNumber();

            var artist = artists.FirstOrDefault(a => a.Id == id);
            if (artist == null)
            {
                Console.WriteLine("Artista no encontrado.");
                EnterToContinue();
                return;
            }

            Console.WriteLine($"\nVa a eliminar: {artist.Name} {artist.Lastname} - {artist.Genre}");
            Console.Write("¿Está seguro? (s/n): ");
            if (Console.ReadLine()?.ToLower() == "s")
            {
                Admin.DeleteArtist(id);
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
            }
            EnterToContinue();
        }

        //  GESTIÓN DE TIQUETES 
        private static void TicketManagementMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== GESTIÓN DE TIQUETES ===");
                Console.WriteLine("1) Listar todos los tiquetes");
                Console.WriteLine("2) Listar tiquetes por concierto");
                Console.WriteLine("3) Eliminar tiquete");
                Console.WriteLine("0) Volver");
                Console.Write("> ");

                var option = TryConvertInputToNumber();

                switch (option)
                {
                    case 1:
                        ListAllTickets();
                        break;
                    case 2:
                        ListTicketsByConcert();
                        break;
                    case 3:
                        DeleteTicket();
                        break;
                    case 0:
                        return;
                }
            }
        }

        private static void ListAllTickets()
        {
            var tickets = Admin.ListTickets().ToList();
            if (!tickets.Any())
            {
                Console.WriteLine("No hay tiquetes vendidos.");
            }
            else
            {
                Console.WriteLine("\n=== TODOS LOS TIQUETES ===");
                foreach (var t in tickets)
                {
                    // Encontrar el cliente que compró este ticket
                    var customer = Admin.ListCustomers().FirstOrDefault(c => c.Purchases.Contains(t));
                    var customerName = customer != null ? $"{customer.Name} {customer.Lastname}" : "Cliente no encontrado";
                    
                    Console.WriteLine($"[{t.Id}] {t.Locality} - {t.Price:C} - {t.Date:d} - Cliente: {customerName}");
                }
            }
            EnterToContinue();
        }

        private static void ListTicketsByConcert()
        {
            var concerts = Admin.ListConcerts().ToList();
            if (!concerts.Any())
            {
                Console.WriteLine("No hay conciertos registrados.");
                EnterToContinue();
                return;
            }

            Console.WriteLine("\n=== CONCIERTOS DISPONIBLES ===");
            foreach (var c in concerts)
            {
                var ticketCount = Admin.ListTicketsByConcert(c.Id).Count();
                Console.WriteLine($"[{c.Id}] {c.Date:d} - {c.Location} - Tickets: {ticketCount}");
            }

            Console.Write("\nSeleccione el ID del concierto: ");
            var concertId = TryConvertInputToNumber();

            var concert = concerts.FirstOrDefault(c => c.Id == concertId);
            if (concert == null)
            {
                Console.WriteLine("Concierto no encontrado.");
                EnterToContinue();
                return;
            }

            var tickets = Admin.ListTicketsByConcert(concertId).ToList();
            if (!tickets.Any())
            {
                Console.WriteLine($"No hay tickets vendidos para el concierto: {concert.Location} - {concert.Date:d}");
            }
            else
            {
                Console.WriteLine($"\n=== TICKETS PARA: {concert.Location} - {concert.Date:d} ===");
                foreach (var t in tickets)
                {
                    var customer = Admin.ListCustomers().FirstOrDefault(c => c.Purchases.Contains(t));
                    var customerName = customer != null ? $"{customer.Name} {customer.Lastname}" : "Cliente no encontrado";
                    Console.WriteLine($"[{t.Id}] {t.Locality} - {t.Price:C} - Cliente: {customerName}");
                }
            }
            EnterToContinue();
        }

        private static void DeleteTicket()
        {
            var tickets = Admin.ListTickets().ToList();
            if (!tickets.Any())
            {
                Console.WriteLine("No hay tiquetes para eliminar.");
                EnterToContinue();
                return;
            }

            Console.WriteLine("\n=== TIQUETES DISPONIBLES ===");
            foreach (var t in tickets)
            {
                var customer = Admin.ListCustomers().FirstOrDefault(c => c.Purchases.Contains(t));
                var customerName = customer != null ? $"{customer.Name} {customer.Lastname}" : "Cliente no encontrado";
                Console.WriteLine($"[{t.Id}] {t.Locality} - {t.Price:C} - {t.Date:d} - Cliente: {customerName}");
            }

            Console.Write("\nSeleccione el ID del tiquete a eliminar: ");
            var id = TryConvertInputToNumber();

            var ticket = tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                Console.WriteLine("Tiquete no encontrado.");
                EnterToContinue();
                return;
            }

            var ticketCustomer = Admin.ListCustomers().FirstOrDefault(c => c.Purchases.Contains(ticket));
            var ticketCustomerName = ticketCustomer != null ? $"{ticketCustomer.Name} {ticketCustomer.Lastname}" : "Cliente no encontrado";

            Console.WriteLine($"\nVa a eliminar tiquete: [{ticket.Id}] {ticket.Locality} - {ticket.Price:C}");
            Console.WriteLine($"Cliente: {ticketCustomerName}");
            Console.Write("¿Está seguro? (s/n): ");
            if (Console.ReadLine()?.ToLower() == "s")
            {
                Admin.DeleteTicket(id);
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
            }
            EnterToContinue();
        }

        //  CONSULTAS AVANZADAS 
        private static void AdvancedQueriesMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== CONSULTAS AVANZADAS (LINQ) ===");
                Console.WriteLine("1) Concierto con más tiquetes vendidos");
                Console.WriteLine("2) Cliente con más compras");
                Console.WriteLine("3) Conciertos por ciudad");
                Console.WriteLine("4) Conciertos por rango de fechas");
                Console.WriteLine("5) Ingresos totales por concierto");
                Console.WriteLine("0) Volver");
                Console.Write("> ");

                var option = TryConvertInputToNumber();

                switch (option)
                {
                    case 1:
                        var topConcert = Admin.GetConcertWithMostTickets();
                        if (topConcert != null)
                        {
                            var ticketCount = Admin.ListTicketsByConcert(topConcert.Id).Count();
                            Console.WriteLine($"Concierto con más tiquetes: {topConcert.Location} {topConcert.Date:d} - {ticketCount} tickets");
                        }
                        else
                        {
                            Console.WriteLine("No hay conciertos con tiquetes vendidos.");
                        }
                        break;

                    case 2:
                        var topCustomer = Admin.GetCustomerWithMostPurchases();
                        if (topCustomer != null)
                        {
                            Console.WriteLine($"Cliente con más compras: {topCustomer.Name} {topCustomer.Lastname} - {topCustomer.Purchases.Count} compras");
                        }
                        else
                        {
                            Console.WriteLine("No existen clientes.");
                        }
                        break;

                    case 3:
                        Console.Write("Ingrese la ciudad: ");
                        var city = Console.ReadLine()!;
                        var concertsByCity = Admin.GetConcertsByCity(city).ToList();
                        if (concertsByCity.Any())
                        {
                            Console.WriteLine($"\nConciertos en {city}:");
                            foreach (var c in concertsByCity)
                                Console.WriteLine($"[{c.Id}] {c.Date:d} - {c.Location}");
                        }
                        else
                        {
                            Console.WriteLine($"No hay conciertos programados en {city}.");
                        }
                        break;

                    case 4:
                        try
                        {
                            Console.Write("Fecha desde (yyyy-mm-dd): ");
                            var from = DateTime.Parse(Console.ReadLine()!);
                            Console.Write("Fecha hasta (yyyy-mm-dd): ");
                            var to = DateTime.Parse(Console.ReadLine()!);
                            
                            var concertsByDate = Admin.GetConcertsByDateRange(from, to).ToList();
                            if (concertsByDate.Any())
                            {
                                Console.WriteLine($"\nConciertos entre {from:d} y {to:d}:");
                                foreach (var c in concertsByDate)
                                    Console.WriteLine($"[{c.Id}] {c.Date:d} - {c.Location}");
                            }
                            else
                            {
                                Console.WriteLine($"No hay conciertos entre {from:d} y {to:d}.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error en las fechas: {ex.Message}");
                            Console.ResetColor();
                        }
                        break;

                    case 5:
                        var concerts = Admin.ListConcerts().ToList();
                        if (!concerts.Any())
                        {
                            Console.WriteLine("No hay conciertos registrados.");
                            break;
                        }

                        Console.WriteLine("\n=== CONCIERTOS DISPONIBLES ===");
                        foreach (var c in concerts)
                            Console.WriteLine($"[{c.Id}] {c.Date:d} - {c.Location}");

                        Console.Write("\nSeleccione el ID del concierto: ");
                        var concertId = TryConvertInputToNumber();

                        var selectedConcert = concerts.FirstOrDefault(c => c.Id == concertId);
                        if (selectedConcert == null)
                        {
                            Console.WriteLine("Concierto no encontrado.");
                        }
                        else
                        {
                            var totalIncome = Admin.GetTotalIncomeOfConcert(concertId);
                            var ticketCount = Admin.ListTicketsByConcert(concertId).Count();
                            Console.WriteLine($"\nIngresos del concierto: {selectedConcert.Location} - {selectedConcert.Date:d}");
                            Console.WriteLine($"Tickets vendidos: {ticketCount}/{selectedConcert.Capacity}");
                            Console.WriteLine($"Ingresos totales: {totalIncome:C}");
                        }
                        break;

                    case 0:
                        return;
                }
                EnterToContinue();
            }
        }

        //  CLIENTE 
        private static void CustomerMenu(Customer customer)
        {
            while (true)
            {
                Console.WriteLine($"\n=== MENÚ CLIENTE ({customer.Name}) ===");
                Console.WriteLine("1) Comprar tiquete");
                Console.WriteLine("2) Ver mis compras");
                Console.WriteLine("0) Volver");
                Console.Write("> ");

                var option = TryConvertInputToNumber();

                switch (option)
                {
                    case 1:
                        if (!Admin.Concerts.Any())
                        {
                            Console.WriteLine("No hay conciertos disponibles.");
                            EnterToContinue();
                            break;
                        }

                        Console.WriteLine("\n=== CONCIERTOS DISPONIBLES ===");
                        foreach (var c in Admin.Concerts)
                        {
                            var soldTickets = Admin.ListTicketsByConcert(c.Id).Count();
                            var availableSpots = c.Capacity - soldTickets;
                            Console.WriteLine($"[{c.Id}] {c.Date:d} - {c.Location} - Disponibles: {availableSpots}/{c.Capacity}");
                            
                            if (c.Artists.Any())
                                Console.WriteLine($"    Artistas: {string.Join(", ", c.Artists)}");
                        }

                        Console.Write("\nSeleccione id del concierto: ");
                        var id = TryConvertInputToNumber();
                        var concert = Admin.Concerts.FirstOrDefault(c => c.Id == id);
                        if (concert == null)
                        {
                            Console.WriteLine("Concierto no encontrado.");
                            EnterToContinue();
                            break;
                        }

                        // Verificar disponibilidad
                        var sold = Admin.ListTicketsByConcert(id).Count();
                        if (sold >= concert.Capacity)
                        {
                            Console.WriteLine("Este concierto está agotado.");
                            EnterToContinue();
                            break;
                        }

                        Console.WriteLine("\n=== LOCALIDADES DISPONIBLES ===");
                        int i = 1;
                        foreach (var kvp in concert.Localities)
                        {
                            Console.WriteLine($"{i}) {kvp.Key} - {kvp.Value:C}");
                            i++;
                        }

                        Console.Write("\nSeleccione opción: ");
                        var choice = TryConvertInputToNumber();
                        if (choice < 1 || choice > concert.Localities.Count)
                        {
                            Console.WriteLine("Opción inválida.");
                            EnterToContinue();
                            break;
                        }

                        var selected = concert.Localities.ElementAt(choice - 1);
                        var ticket = Admin.RegisterTicketPurchase(customer, concert.Id, selected.Key);
                        if (ticket != null)
                        {
                            Console.WriteLine($"\n¡Compra exitosa!");
                            Console.WriteLine($"Concierto: {concert.Location} - {concert.Date:d}");
                            Console.WriteLine($"Localidad: {selected.Key}");
                            Console.WriteLine($"Precio: {selected.Value:C}");
                        }
                        EnterToContinue();
                        break;

                    case 2:
                        if (!customer.Purchases.Any())
                        {
                            Console.WriteLine("No tienes compras.");
                        }
                        else
                        {
                            Console.WriteLine($"\n=== MIS COMPRAS ({customer.Purchases.Count}) ===");
                            var totalSpent = customer.Purchases.Sum(t => t.Price);
                            foreach (var t in customer.Purchases.OrderByDescending(t => t.Date))
                                Console.WriteLine($"Ticket [{t.Id}] - {t.Locality} - {t.Price:C} - {t.Date:d}");
                            Console.WriteLine($"\nTotal gastado: {totalSpent:C}");
                        }
                        EnterToContinue();
                        break;

                    case 0:
                        return;
                }
            }
        }

        //  ARTISTA 
        private static void ArtistMenu(Artist artist)
        {
            while (true)
            {
                Console.WriteLine($"\n=== MENÚ ARTISTA ({artist.Name}) ===");
                Console.WriteLine("1) Crear concierto");
                Console.WriteLine("2) Ver mis conciertos");
                Console.WriteLine("0) Volver");
                Console.Write("> ");

                var option = TryConvertInputToNumber();

                switch (option)
                {
                    case 1:
                        try
                        {
                            Console.Write("Fecha (yyyy-mm-dd): ");
                            var date = DateTime.Parse(Console.ReadLine()!);
                            Console.Write("Lugar: ");
                            var loc = Console.ReadLine()!;
                            Console.Write("Capacidad: ");
                            var cap = int.Parse(Console.ReadLine()!);

                            // Configuración de precios (opcional)
                            Console.WriteLine("\n¿Desea configurar precios personalizados? (s/n):");
                            var customPrices = Console.ReadLine()?.ToLower() == "s";
                            
                            Dictionary<string, decimal>? localities = null;
                            if (customPrices)
                            {
                                localities = new Dictionary<string, decimal>();
                                Console.Write("Precio VIP: ");
                                localities["VIP"] = decimal.Parse(Console.ReadLine()!);
                                Console.Write("Precio Platea: ");
                                localities["Platea"] = decimal.Parse(Console.ReadLine()!);
                                Console.Write("Precio General: ");
                                localities["General"] = decimal.Parse(Console.ReadLine()!);
                            }
                            
                            var artists = new List<string> { artist.GetFullName() };
                            var concert = Admin.RegisterConcert(date, loc, cap, localities, artists);

                            Console.WriteLine($"\n¡Concierto creado exitosamente!");
                            Console.WriteLine($"ID: {concert.Id}");
                            Console.WriteLine($"Lugar: {concert.Location}");
                            Console.WriteLine($"Fecha: {concert.Date:d}");
                            Console.WriteLine($"Artista: {artist.GetFullName()}");
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error al crear concierto: {ex.Message}");
                            Console.ResetColor();
                        }
                        break;

                    case 2:
                        var artistConcerts = Admin.Concerts.Where(c => artist.IsAssociatedToConcert(c)).ToList();
                        if (!artistConcerts.Any())
                        {
                            Console.WriteLine("No tienes conciertos creados.");
                        }
                        else
                        {
                            Console.WriteLine($"\n=== MIS CONCIERTOS ({artistConcerts.Count}) ===");
                            foreach (var c in artistConcerts.OrderBy(c => c.Date))
                            {
                                var ticketsSold = Admin.ListTicketsByConcert(c.Id).Count();
                                var income = Admin.GetTotalIncomeOfConcert(c.Id);
                                Console.WriteLine($"[{c.Id}] {c.Date:d} - {c.Location}");
                                Console.WriteLine($"    Tickets vendidos: {ticketsSold}/{c.Capacity} - Ingresos: {income:C}");
                                
                                // Mostrar otros artistas si es un concierto colaborativo
                                var otherArtists = c.Artists.Where(a => a != artist.GetFullName()).ToList();
                                if (otherArtists.Any())
                                {
                                    Console.WriteLine($"    Colaboradores: {string.Join(", ", otherArtists)}");
                                }
                            }
                        }
                        break;

                    case 0:
                        return;
                }
                EnterToContinue();
            }
        }

        //  AUTENTICACIÓN 
        private static Customer? AuthenticateCustomer()
        {
            var customers = Admin.ListCustomers().ToList();
            
            if (!customers.Any())
            {
                Console.WriteLine("No hay clientes registrados en el sistema.");
                Console.WriteLine("¿Desea registrarse? (s/n): ");
                if (Console.ReadLine()?.ToLower() == "s")
                {
                    return RegisterNewCustomerFromLogin();
                }
                return null;
            }

            Console.WriteLine("\n=== INICIO DE SESIÓN CLIENTE ===");
            Console.WriteLine("1. Iniciar sesión");
            Console.WriteLine("2. Registrarse");
            Console.WriteLine("0. Volver");
            Console.Write("> ");

            var option = TryConvertInputToNumber();

            switch (option)
            {
                case 1:
                    return LoginCustomer();
                case 2:
                    return RegisterNewCustomerFromLogin();
                case 0:
                    return null;
                default:
                    Console.WriteLine("Opción inválida.");
                    return null;
            }
        }

        private static Customer? LoginCustomer()
        {
            Console.Write("Email: ");
            var email = Console.ReadLine()!;
            Console.Write("Contraseña: ");
            var password = Console.ReadLine()!;

            var customer = Admin.ListCustomers().FirstOrDefault(c => 
                string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase) && 
                c.Password == password);

            if (customer == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Email o contraseña incorrectos.");
                Console.ResetColor();
                EnterToContinue();
                return null;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"¡Bienvenido/a {customer.Name} {customer.Lastname}!");
            Console.ResetColor();
            EnterToContinue();
            return customer;
        }

        private static Customer RegisterNewCustomerFromLogin()
        {
            Console.WriteLine("\n=== REGISTRO DE NUEVO CLIENTE ===");
            Console.Write("Nombre: ");
            var name = Console.ReadLine()!;
            Console.Write("Apellido: ");
            var lastname = Console.ReadLine()!;
            Console.Write("Email: ");
            var email = Console.ReadLine()!;
            Console.Write("Contraseña: ");
            var password = Console.ReadLine()!;

            var customer = Admin.RegisterOrGetCustomer(name, lastname, email, password);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"¡Cliente registrado exitosamente! Bienvenido/a {customer.Name}!");
            Console.ResetColor();
            EnterToContinue();
            
            return customer;
        }

        private static Artist? AuthenticateArtist()
        {
            var artists = Admin.ListArtists().ToList();
            
            if (!artists.Any())
            {
                Console.WriteLine("No hay artistas registrados en el sistema.");
                Console.WriteLine("¿Desea registrarse? (s/n): ");
                if (Console.ReadLine()?.ToLower() == "s")
                {
                    return RegisterNewArtistFromLogin();
                }
                return null;
            }

            Console.WriteLine("\n=== INICIO DE SESIÓN ARTISTA ===");
            Console.WriteLine("1. Iniciar sesión");
            Console.WriteLine("2. Registrarse");
            Console.WriteLine("0. Volver");
            Console.Write("> ");

            var option = TryConvertInputToNumber();

            switch (option)
            {
                case 1:
                    return LoginArtist();
                case 2:
                    return RegisterNewArtistFromLogin();
                case 0:
                    return null;
                default:
                    Console.WriteLine("Opción inválida.");
                    return null;
            }
        }

        private static Artist? LoginArtist()
        {
            Console.Write("Email: ");
            var email = Console.ReadLine()!;
            Console.Write("Contraseña: ");
            var password = Console.ReadLine()!;

            var artist = Admin.ListArtists().FirstOrDefault(a => 
                string.Equals(a.Email, email, StringComparison.OrdinalIgnoreCase) && 
                a.Password == password);

            if (artist == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Email o contraseña incorrectos.");
                Console.ResetColor();
                EnterToContinue();
                return null;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"¡Bienvenido/a {artist.Name} {artist.Lastname}!");
            Console.ResetColor();
            EnterToContinue();
            return artist;
        }

        private static Artist RegisterNewArtistFromLogin()
        {
            Console.WriteLine("\n=== REGISTRO DE NUEVO ARTISTA ===");
            Console.Write("Nombre: ");
            var name = Console.ReadLine()!;
            Console.Write("Apellido: ");
            var lastname = Console.ReadLine()!;
            Console.Write("Email: ");
            var email = Console.ReadLine()!;
            Console.Write("Contraseña: ");
            var password = Console.ReadLine()!;
            Console.Write("Género musical: ");
            var genre = Console.ReadLine()!;

            var artist = Admin.RegisterOrGetArtist(name, lastname, email, password, genre);
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"¡Artista registrado exitosamente! Bienvenido/a {artist.Name}!");
            Console.ResetColor();
            EnterToContinue();
            
            return artist;
        }

        //  SELECCIÓN DE USUARIO 
        public static void SelectUser()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════╗");
                Console.WriteLine("║        RIWI MUSIC SYSTEM         ║");
                Console.WriteLine("╚══════════════════════════════════╝");
                Console.WriteLine("\n¿Quién va a ingresar?");
                Console.WriteLine("1. Admin");
                Console.WriteLine("2. Cliente");
                Console.WriteLine("3. Artista");
                Console.WriteLine("0. Salir");
                Console.Write("> ");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AdminMenu();
                        break;

                    case "2":
                        var customer = AuthenticateCustomer();
                        if (customer != null)
                        {
                            CustomerMenu(customer);
                        }
                        break;

                    case "3":
                        var artist = AuthenticateArtist();
                        if (artist != null)
                        {
                            ArtistMenu(artist);
                        }
                        break;

                    case "0":
                        Console.WriteLine("\n¡Gracias por usar RIWI MUSIC SYSTEM!");
                        return;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Opción inválida. Presiona Enter para continuar...");
                        Console.ResetColor();
                        Console.ReadLine();
                        break;
                }
            }
        }
    }
}