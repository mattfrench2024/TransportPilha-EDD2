using System;
using System.Collections.Generic;
using System.Linq;

namespace TransportPilha
{
    // ------- Vehicle -------
    class Vehicle
    {
        public int Id { get; }
        public string Plate { get; set; } // optional identifier
        public int Capacity { get; set; }
        public int TripsDone { get; set; }
        public int PassengersToday { get; set; }

        public Vehicle(int id, int capacity = 12, string plate = "")
        {
            Id = id;
            Capacity = capacity;
            Plate = plate;
            TripsDone = 0;
            PassengersToday = 0;
        }

        public override string ToString()
        {
            return $"V{Id} (Cap:{Capacity}, Trips:{TripsDone}, PaxToday:{PassengersToday})";
        }

        public void ResetDay()
        {
            TripsDone = 0;
            PassengersToday = 0;
        }
    }

    // ------- Garage (stack) -------
    class Garage
    {
        public int Id { get; }
        public string Name { get; set; }
        private Stack<int> vehicles; // store vehicle IDs

        public Garage(int id, string name)
        {
            Id = id;
            Name = name;
            vehicles = new Stack<int>();
        }

        public void Park(int vehicleId)
        {
            vehicles.Push(vehicleId);
        }

        // Return vehicle id ready to depart (top of stack), or -1 if empty
        public int Depart()
        {
            if (vehicles.Count == 0) return -1;
            return vehicles.Pop();
        }

        public int Count() => vehicles.Count;

        public IEnumerable<int> VehiclesSnapshot() => vehicles.ToArray(); // top-first

        public override string ToString()
        {
            return $"G{Id} - {Name} (Cars:{vehicles.Count})";
        }
    }

    // ------- Trip record -------
    class Trip
    {
        private static int nextId = 1;
        public int Id { get; }
        public int OriginId { get; }
        public int DestId { get; }
        public int VehicleId { get; }
        public int Passengers { get; }
        public DateTime Date { get; }

        public Trip(int origin, int dest, int vid, int pax)
        {
            Id = nextId++;
            OriginId = origin;
            DestId = dest;
            VehicleId = vid;
            Passengers = pax;
            Date = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Trip#{Id}: {Date:G} - G{OriginId} -> G{DestId} | V{VehicleId} | Pax:{Passengers}";
        }
    }

    // ------- Transport System -------
    class TransportSystem
    {
        private Dictionary<int, Vehicle> vehicles = new Dictionary<int, Vehicle>();
        private Dictionary<int, Garage> garages = new Dictionary<int, Garage>();
        private List<Trip> tripsLog = new List<Trip>();
        private bool jornadaAtiva = false;
        private int nextVehicleId = 1;
        private int nextGarageId = 1;

        // --- Vehicle & Garage management ---
        public void AddVehicle(int capacity = 12, string plate = "")
        {
            if (jornadaAtiva)
            {
                Console.WriteLine("ERRO: Não é permitido cadastrar veículo com jornada ativa. Encerre a jornada primeiro.");
                return;
            }
            var v = new Vehicle(nextVehicleId++, capacity, plate);
            vehicles.Add(v.Id, v);
            Console.WriteLine($"Veículo cadastrado: {v}");
        }

        public void AddGarage(string name)
        {
            if (jornadaAtiva)
            {
                Console.WriteLine("ERRO: Não é permitido cadastrar garagem com jornada ativa. Encerre a jornada primeiro.");
                return;
            }
            var g = new Garage(nextGarageId++, name);
            garages.Add(g.Id, g);
            Console.WriteLine($"Garagem criada: {g}");
        }

        // Start day: alternate distribution
        public void StartJornada()
        {
            if (jornadaAtiva)
            {
                Console.WriteLine("Jornada já iniciada.");
                return;
            }
            if (garages.Count == 0)
            {
                Console.WriteLine("ERRO: Não há garagens cadastradas.");
                return;
            }
            if (vehicles.Count == 0)
            {
                Console.WriteLine("ERRO: Não há veículos cadastrados.");
                return;
            }

            // Clear garages to ensure fresh distribution
            foreach (var g in garages.Values)
            {
                // NOTE: we cannot directly clear stack, recreate new garage? simpler: re-create internal state via reflection? Instead rebuild new stacks by re-instantiation:
            }

            // Simpler approach: capture vehicle IDs in list, then clear and re-create garages with new empty stacks:
            var vehIds = vehicles.Keys.OrderBy(id => id).ToList();

            // Reset garages' internal stacks by creating new Garage objects while preserving names and ids:
            var oldGarages = garages.ToList();
            garages.Clear();
            foreach (var og in oldGarages)
            {
                // create new Garage with same id and name
                var g = new Garage(og.Key, og.Value.Name);
                garages.Add(g.Id, g);
            }

            // distribute alternating (round-robin)
            var garageIds = garages.Keys.OrderBy(id => id).ToList();
            int gi = 0;
            foreach (var vid in vehIds)
            {
                var gid = garageIds[gi % garageIds.Count];
                garages[gid].Park(vid);
                gi++;
            }

            // reset vehicle daily counters
            foreach (var v in vehicles.Values) v.ResetDay();

            jornadaAtiva = true;
            tripsLog.Clear(); // clear previous day trips? requirement: trips previous cleared on end of day; Start should expect day fresh
            Console.WriteLine("Jornada iniciada. Veículos foram distribuídos alternadamente entre as garagens.");
        }

        // End day: print vehicle-by-vehicle transported passengers and clear occurrences
        public void EndJornada()
        {
            if (!jornadaAtiva)
            {
                Console.WriteLine("Jornada não está ativa.");
                return;
            }

            Console.WriteLine("\n--- Encerramento da Jornada ---");
            Console.WriteLine("Resumo por veículo:");
            foreach (var v in vehicles.Values.OrderBy(vv => vv.Id))
            {
                Console.WriteLine($"Veículo V{v.Id}: Passageiros transportados no dia = {v.PassengersToday}, Viagens = {v.TripsDone}");
                v.ResetDay(); // limpeza conforme requisito
            }

            tripsLog.Clear();
            jornadaAtiva = false;
            Console.WriteLine("Jornada encerrada e ocorrências anteriores limpas.");
        }

        // Release a trip from origin to dest with passengers count
        public void ReleaseTrip(int originId, int destId, int passengers)
        {
            if (!jornadaAtiva)
            {
                Console.WriteLine("ERRO: Jornada não iniciada. Inicie a jornada antes de liberar viagens.");
                return;
            }
            if (!garages.ContainsKey(originId))
            {
                Console.WriteLine("ERRO: Origem inválida.");
                return;
            }
            if (!garages.ContainsKey(destId))
            {
                Console.WriteLine("ERRO: Destino inválido.");
                return;
            }
            if (originId == destId)
            {
                Console.WriteLine("ERRO: Origem e destino iguais.");
                return;
            }

            var origin = garages[originId];
            var dest = garages[destId];

            int vehicleId = origin.Depart();
            if (vehicleId == -1)
            {
                Console.WriteLine("ERRO: Garagem de origem está vazia. Aguarde retorno de um veículo.");
                return;
            }

            var vehicle = vehicles[vehicleId];

            if (passengers < 0 || passengers > vehicle.Capacity)
            {
                Console.WriteLine($"ERRO: Número de passageiros inválido. Capacidade do veículo V{vehicle.Id} é {vehicle.Capacity}.");
                // vehicle returns to origin (park back)
                origin.Park(vehicleId);
                return;
            }

            // Record trip
            var trip = new Trip(originId, destId, vehicleId, passengers);
            tripsLog.Add(trip);

            // Update vehicle counters
            vehicle.TripsDone += 1;
            vehicle.PassengersToday += passengers;

            // At destination the vehicle is parked
            garages[destId].Park(vehicleId);

            Console.WriteLine($"Viagem liberada: {trip}");
        }

        // List vehicles in garage & potential transport (sum capacities)
        public void ListVehiclesInGarage(int garageId)
        {
            if (!garages.ContainsKey(garageId))
            {
                Console.WriteLine("Garagem inválida.");
                return;
            }

            var g = garages[garageId];
            var snapshot = g.VehiclesSnapshot().ToArray(); // <-- CORREÇÃO

            Console.WriteLine($"Garagem {g}:");
            Console.WriteLine($"Quantidade de veículos: {snapshot.Length}");

            int totalPotential = 0;

            if (snapshot.Length == 0)
            {
                Console.WriteLine("[Vazio]");
            }
            else
            {
                Console.WriteLine("Veículos (pronto-para-sair primeiro):");
                foreach (var vid in snapshot)
                {
                    var v = vehicles[vid];
                    Console.WriteLine($" - {v}");
                    totalPotential += v.Capacity;
                }
            }

            Console.WriteLine($"Potencial total de transporte: {totalPotential}");
        }


        // Quantidade de viagens efetuadas origin->dest
        public int TripsCount(int originId, int destId)
        {
            return tripsLog.Count(t => t.OriginId == originId && t.DestId == destId);
        }

        // List trips origin->dest
        public void ListTrips(int originId, int destId)
        {
            var found = tripsLog.Where(t => t.OriginId == originId && t.DestId == destId).ToList();
            if (found.Count == 0)
            {
                Console.WriteLine("Nenhuma viagem encontrada para esse trecho.");
                return;
            }
            foreach (var t in found)
            {
                Console.WriteLine(t);
            }
        }

        public int PassengersCount(int originId, int destId)
        {
            return tripsLog.Where(t => t.OriginId == originId && t.DestId == destId).Sum(t => t.Passengers);
        }

        // Helpers to list garages and vehicles
        public void ShowGarages()
        {
            Console.WriteLine("Garagens cadastradas:");
            foreach (var g in garages.Values.OrderBy(g => g.Id))
                Console.WriteLine($"G{g.Id}: {g.Name} (Cars:{g.Count()})");
        }
        public void ShowVehicles()
        {
            Console.WriteLine("Veículos cadastrados:");
            foreach (var v in vehicles.Values.OrderBy(v => v.Id))
                Console.WriteLine($"V{v.Id}: Capacidade {v.Capacity}");
        }

        // Accessors for validation from UI
        public bool HasGarage(int id) => garages.ContainsKey(id);
        public bool HasVehicle(int id) => vehicles.ContainsKey(id);
    }

    // ------- Program (UI loop) -------
    class Program
    {
        static void Main(string[] args)
        {
            var system = new TransportSystem();
            Console.WriteLine("=== PROJETO TRANSPORTE – PILHA (Console) ===");

            while (true)
            {
                ShowMenu();
                Console.Write("Opção: ");
                var line = Console.ReadLine();
                if (!int.TryParse(line, out int opt))
                {
                    Console.WriteLine("Opção inválida.");
                    continue;
                }

                try
                {
                    switch (opt)
                    {
                        case 0:
                            Console.WriteLine("Finalizando...");
                            return;
                        case 1:
                            Console.Write("Informe capacidade (inteiro, padrão 12) e opcional placa (separado por espaço): ");
                            var input = Console.ReadLine().Trim();
                            int cap = 12; string plate = "";
                            if (!string.IsNullOrEmpty(input))
                            {
                                var parts = input.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 1 && int.TryParse(parts[0], out int c)) cap = c;
                                if (parts.Length == 2) plate = parts[1];
                            }
                            system.AddVehicle(cap, plate);
                            break;
                        case 2:
                            Console.Write("Nome da garagem: ");
                            var name = Console.ReadLine();
                            system.AddGarage(name);
                            break;
                        case 3:
                            system.StartJornada();
                            break;
                        case 4:
                            system.EndJornada();
                            break;
                        case 5:
                            Console.Write("Origem (id garagem): ");
                            int orig = ReadInt();
                            Console.Write("Destino (id garagem): ");
                            int dest = ReadInt();
                            Console.Write("Quantidade de passageiros: ");
                            int pax = ReadInt();
                            system.ReleaseTrip(orig, dest, pax);
                            break;
                        case 6:
                            Console.Write("Id da garagem para listar: ");
                            int gid = ReadInt();
                            system.ListVehiclesInGarage(gid);
                            break;
                        case 7:
                            Console.Write("Origem (id garagem): ");
                            int o1 = ReadInt();
                            Console.Write("Destino (id garagem): ");
                            int d1 = ReadInt();
                            int cnt = system.TripsCount(o1, d1);
                            Console.WriteLine($"Quantidade de viagens {o1} -> {d1}: {cnt}");
                            break;
                        case 8:
                            Console.Write("Origem (id garagem): ");
                            int o2 = ReadInt();
                            Console.Write("Destino (id garagem): ");
                            int d2 = ReadInt();
                            system.ListTrips(o2, d2);
                            break;
                        case 9:
                            Console.Write("Origem (id garagem): ");
                            int o3 = ReadInt();
                            Console.Write("Destino (id garagem): ");
                            int d3 = ReadInt();
                            int pcount = system.PassengersCount(o3, d3);
                            Console.WriteLine($"Passageiros transportados {o3} -> {d3}: {pcount}");
                            break;
                        case 99:
                            Console.WriteLine("[DEBUG] Mostrar garagens e veículos");
                            system.ShowGarages();
                            system.ShowVehicles();
                            break;
                        default:
                            Console.WriteLine("Opção não implementada.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro: " + ex.Message);
                }

                Console.WriteLine();
            }
        }

        static int ReadInt()
        {
            while (true)
            {
                var s = Console.ReadLine();
                if (int.TryParse(s, out int v)) return v;
                Console.Write("Número inválido. Tente novamente: ");
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("0. Finalizar");
            Console.WriteLine("1. Cadastrar veículo");
            Console.WriteLine("2. Cadastrar garagem");
            Console.WriteLine("3. Iniciar jornada");
            Console.WriteLine("4. Encerrar jornada");
            Console.WriteLine("5. Liberar viagem (origem -> destino)");
            Console.WriteLine("6. Listar veículos em garagem");
            Console.WriteLine("7. Quantidade de viagens (origem -> destino)");
            Console.WriteLine("8. Listar viagens (origem -> destino)");
            Console.WriteLine("9. Quantidade de passageiros (origem -> destino)");
            Console.WriteLine("99. DEBUG - listar garagens e veículos");
        }
    }
}
