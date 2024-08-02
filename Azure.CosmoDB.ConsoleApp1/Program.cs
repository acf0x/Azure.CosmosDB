using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace Azure.CosmoDB.ConsoleApp1
{
    internal class Program
    {
        static readonly string endPointCosmosDB = "https://DemodbACF.documents.azure.com:443/";
        static readonly string keyCosmosDB = "KEY";

        static CosmosClient clientCosmosDB;

        static void Main(string[] args)
        {
            clientCosmosDB = new CosmosClient(endPointCosmosDB, keyCosmosDB);

            GetDatabases();
            //CreateRecord("DemoDB", "productos");
            QueryRecords("DemoDB", "productos");
            QueryRecords2("DemoDB", "productos");
            QueryRecords3("DemoDB", "productos");
        }


        static void GetDatabases()
        {
            var resultIterator = clientCosmosDB.GetDatabaseQueryIterator<DatabaseProperties>();

            while (resultIterator.HasMoreResults)
            {
                var allProperties = resultIterator.ReadNextAsync().Result;
                foreach (var property in allProperties)
                {
                    Console.WriteLine($"Base de datos: {property.Id}");
                    GetContainers(property.Id);
                }
            }
        }

        static void GetContainers(string databaseName)
        {
            Database clientDatabase = clientCosmosDB.GetDatabase(databaseName);

            var resultIterator = clientDatabase.GetContainerQueryIterator<ContainerProperties>();

            while (resultIterator.HasMoreResults)
            {
                var allProperties = resultIterator.ReadNextAsync().Result;
                foreach (var property in allProperties)
                {
                    Console.WriteLine($" -> {property.Id}");

                }
            }
        }

        static void CreateRecord(string databaseName, string containerName)
        {
            var clientDatabase = clientCosmosDB.GetDatabase(databaseName);
            var clientContainer = clientDatabase.GetContainer(containerName);

            var producto = new Producto()
            {
                id = "10",
                referencia = "10",
                categoria = "Bebidas",
                descripcion = "Bebida de naranja 33cl",
                cantidad = 5,
                precio = 1.85
            };

            var producto2 = new Product("11", "11", "Bebidas", "Refresco de Cola 1L", 26, 2.80);

            var result = clientContainer.CreateItemAsync(producto, new PartitionKey("Bebidas")).Result;
            Console.WriteLine($"Producto creado con ID {result.Resource.id}");

            var result2 = clientContainer.CreateItemAsync(producto2, new PartitionKey("Bebidas")).Result;
            Console.WriteLine($"Producto creado con ID {result2.Resource.id}");
        }

        /// <summary>
        /// Select all y con <Producto>
        /// </summary>
        static void QueryRecords(string databaseName, string containerName)
        {
            var clientDatabase = clientCosmosDB.GetDatabase(databaseName);
            var clientContainer = clientDatabase.GetContainer(containerName);

            // Listado completo de los items en el contenedor
            string sqlQuery = "SELECT * FROM c";

            var resultIterator = clientContainer.GetItemQueryIterator<Producto>(sqlQuery);
            while (resultIterator.HasMoreResults)
            {
                var productos = resultIterator.ReadNextAsync().Result;
                foreach (var producto in productos)
                {
                    Console.WriteLine($" -> " +
                      $"{producto.id}# " +
                      $"{producto.descripcion} " +
                      $"- {producto.precio.ToString("N2")}");
                }
            }
        }

        /// <summary>
        /// Precio >= 2 y con <dynamic>
        /// </summary>
        static void QueryRecords2(string databaseName, string containerName)
        {
            var clientDatabase = clientCosmosDB.GetDatabase(databaseName);
            var clientContainer = clientDatabase.GetContainer(containerName);

            // Listado de los items en el contenedor con precio >= 2
            string sqlQuery = "SELECT * FROM c WHERE c.precio >= 2";

            var resultIterator = clientContainer.GetItemQueryIterator<dynamic>(sqlQuery);
            while (resultIterator.HasMoreResults)
            {
                var productos = resultIterator.ReadNextAsync().Result;
                foreach (var producto in productos)
                {
                    Console.WriteLine($" -> " +
                      $"{producto.id}# " +
                      $"{producto.descripcion} " +
                      $"- {producto.precio.ToString("N2")}");
                }
            }
        }

        /// <summary>
        /// Con LINQ
        /// </summary>
        static void QueryRecords3(string databaseName, string containerName)
        {
            var clientDatabase = clientCosmosDB.GetDatabase(databaseName);
            var clientContainer = clientDatabase.GetContainer(containerName);

            // Listado de los items en el contenedor con precio >= 2 con una búsqueda en LINQ

            var resultIterator = clientContainer.GetItemLinqQueryable<Producto>()
                .Where(c => c.precio >= 2)
                .ToFeedIterator();

            while (resultIterator.HasMoreResults)
            {
                var productos = resultIterator.ReadNextAsync().Result;
                foreach (var producto in productos)
                {
                    Console.WriteLine($" -> " +
                      $"{producto.id}# " +
                      $"{producto.descripcion} " +
                      $"- {producto.precio.ToString("N2")}");
                }
            }
        }

    }

    public class Producto
    {
        public string id { get; set; }
        public string referencia { get; set; }
        public string categoria { get; set; }
        public string descripcion { get; set; }
        public int cantidad { get; set; }
        public double precio { get; set; }
    }

    public record Product(string id, string referencia, string categoria, string descripcion, int cantidad, double precio);
}
