using Azure.Data.Tables;

namespace Azure.Tables.ConsoleApp1
{
    internal class Program
    {
        static readonly string connection = "";

        static void Main(string[] args)
        {
            // Cliente del Servicio de Tables
            var clientService = new TableServiceClient(connection);

            // Añadir una nueva tabla si no existe
            clientService.CreateTableIfNotExists("clientes");

            // Listar tables
            var tables = clientService.Query();
            foreach (var table in tables) Console.WriteLine($"Tabla: {table.Name}");

            // Cliente de una Table concreta
            var clientTables = new TableClient(connection, "productos");
            
            // Añadir un producto
            var producto = new Producto2()
            {
                RowKey = "13",
                PartitionKey = "Bebidas",
                id = "13",
                referencia = "13",
                categoria = "Bebidas",
                descripcion = "Bebida de cola 2L",
                cantidad = 7,
                precio = 2.10
            };
            clientTables.AddEntity(producto);
            Console.WriteLine($"Producto insertado correctamente");


            // Consultas

            // Listado Completo
            var resultado = clientTables.Query<Producto2>();
            Console.WriteLine($"Listado de todos los productos:");
            foreach (var item in resultado) Console.WriteLine($"{item.RowKey}# {item.descripcion} - {item.precio}");

            // Listado productos con precio >= 2
            var resultado2 = clientTables.Query<Producto2>("precio ge 2");
            Console.WriteLine($"Productos con precio >= 2:");
            foreach (var item in resultado2) Console.WriteLine($"{item.RowKey}# {item.descripcion}");

            // Listado productos con precio >= 2 con LINQ
            var resultado3 = clientTables.Query<Producto2>(r => r.precio >= 2);
            Console.WriteLine($"Productos con precio >= 2 (LINQ):");
            foreach (var item in resultado3) Console.WriteLine($"{item.RowKey}# {item.descripcion}");

            // Borrar un item
            clientTables.DeleteEntity("Bebidas", "13");
            Console.WriteLine($"Producto eliminado correctamente.");
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

    public class Producto2 : ITableEntity
    {
        public string id { get; set; }
        public string referencia { get; set; }
        public string categoria { get; set; }
        public string descripcion { get; set; }
        public int cantidad { get; set; }
        public double precio { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
