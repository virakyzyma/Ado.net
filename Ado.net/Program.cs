using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Ado.net
{
    internal class Program
    {
        static string connectionString;
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var config = builder.Build();
            connectionString = config.GetConnectionString("DefaultConnection");
            CreateDatabase("FruitsVegetables");
            //CreateTable("""
            //   CREATE TABLE [Fruits] 
            //   (
            //   [Id] INT PRIMARY KEY IDENTITY, 
            //   [Name] NVARCHAR(60) NOT NULL, 
            //   [Color] NVARCHAR(60) NOT NULL, 
            //   [Caloricity] INT NOT NULL
            //   );
            //   """);

            //CreateTable("""
            //   CREATE TABLE [Vegetables] 
            //   (
            //   [Id] INT PRIMARY KEY IDENTITY, 
            //   [Name] NVARCHAR(60) NOT NULL, 
            //   [Color] NVARCHAR(60) NOT NULL, 
            //   [Caloricity] INT NOT NULL
            //   );
            //   """);

            //InsertIntoTable("Fruits", new Fruit[]
            //{
            //    new Fruit{ Name = "Apple", Color = "Red", Caloricity = 100},
            //    new Fruit{ Name = "Kiwi", Color = "Green", Caloricity = 50},
            //    new Fruit{ Name = "Banana", Color = "Yellow", Caloricity = 200},
            //    new Fruit{ Name = "Orange", Color = "Orange", Caloricity = 100}
            //});
            //InsertIntoTable("Vegetables", new Fruit[]
            //{
            //    new Fruit{ Name = "Tomato", Color = "Red", Caloricity = 100},
            //    new Fruit{ Name = "Cucumber", Color = "Green", Caloricity = 50},
            //    new Fruit{ Name = "Cabbage", Color = "Green", Caloricity = 200},
            //});

            //GetAllProducts();
            //GetAllNames();
            //GetAllColors();
            //GetMaxCaloricity();
            //GetMinCaloricity();
            //GetAvgCaloricity();
            //GetFruitsCount();
            //GetVegetablesCount();
            //GetProductsCountByColor("Red");
            //GetProductsCountOfEveryColor();
            //GetProductsWithLessCaloricity(100);
            //GetProductsWithGreaterCaloricity(100);
            //GetProductsWithCaloricityInRange(51, 199);
            //GetProductsWithColorRedOrOrange();
        }

        private static void CreateDatabase(string dbName)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand($"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name LIKE '{dbName}')" +
                    $"CREATE DATABASE [{dbName}]", sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void CreateTable(string sqlQuery)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void InsertIntoTable(string table, params Products[] products)
        {
            StringBuilder stringBuilder = new StringBuilder($"INSERT INTO [{table}] ([Name], [Color], [Caloricity]) VALUES ");
            foreach (var product in products)
            {
                stringBuilder.Append(product.ToStringSql());
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = new SqlCommand(stringBuilder.ToString(), sqlConnection);
                sqlCommand.ExecuteNonQuery();
            }
        }

        private static void RequestToDatabase(Action<SqlConnection> action)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    action(connection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private static void GetAllProducts()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT [Name], [Color], [Caloricity] FROM [Fruits];
                    SELECT [Name], [Color], [Caloricity] FROM [Vegetables];
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]}, {reader["Color"]}, {reader["Caloricity"]}");
                    }
                } while (reader.NextResult());
            });
        }

        private static void GetAllNames()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT [Name] FROM [Fruits];
                    SELECT [Name] FROM [Vegetables];
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetAllColors()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT DISTINCT [Color] FROM [Fruits]
                    UNION
                    SELECT DISTINCT [Color] FROM [Vegetables];
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Color"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetMaxCaloricity()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT MAX(Caloricity) as MaxCaloricity FROM
                    (
                        SELECT Caloricity FROM Fruits
                        UNION
                        SELECT Caloricity FROM Vegetables
                    ) AS FruitsVegetables;
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["MaxCaloricity"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetMinCaloricity()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT Min(Caloricity) as MinCaloricity FROM
                    (
                        SELECT Caloricity FROM Fruits
                        UNION
                        SELECT Caloricity FROM Vegetables
                    ) AS FruitsVegetables;
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["MinCaloricity"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetAvgCaloricity()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT AVG(Caloricity) as AvgCaloricity FROM
                    (
                        SELECT Caloricity FROM Fruits
                        UNION
                        SELECT Caloricity FROM Vegetables
                    ) AS FruitsVegetables;
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["AvgCaloricity"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetVegetablesCount()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                        SELECT COUNT(*) AS VegetablesCount FROM Vegetables
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["VegetablesCount"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetFruitsCount()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                        SELECT COUNT(*) AS FruitsCount FROM Fruits
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["FruitsCount"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetProductsCountByColor(string color)
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand($"""
                        SELECT COUNT(*) AS ProductsCount FROM 
                        (
                            SELECT Color FROM Fruits WHERE [Color] = '{color}'
                            UNION ALL
                            SELECT Color FROM Vegetables WHERE [Color] = '{color}'
                        ) AS FruitsVegetables;
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["ProductsCount"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetProductsCountOfEveryColor()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand("""
                    SELECT Color, COUNT(*) AS ProductsCount
                    FROM (
                        SELECT Color FROM Fruits
                        UNION ALL
                        SELECT Color FROM Vegetables
                    ) AS FruitsVegetables
                    GROUP BY Color
                    ORDER BY ProductsCount DESC;
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Color"]}: {reader["ProductsCount"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetProductsWithLessCaloricity(int caloricity)
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand($"""
                    SELECT Name, Color, Caloricity
                    FROM (
                        SELECT Name, Color, Caloricity FROM Fruits WHERE Caloricity < {caloricity}
                        UNION ALL
                        SELECT Name, Color, Caloricity FROM Vegetables WHERE Caloricity < {caloricity}
                    ) AS FruitsVegetables
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]}, {reader["Color"]}, {reader["Caloricity"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetProductsWithGreaterCaloricity(int caloricity)
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand($"""
                    SELECT Name, Color, Caloricity
                    FROM (
                        SELECT Name, Color, Caloricity FROM Fruits WHERE Caloricity > {caloricity}
                        UNION ALL
                        SELECT Name, Color, Caloricity FROM Vegetables WHERE Caloricity > {caloricity}
                    ) AS FruitsVegetables
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]}, {reader["Color"]}, {reader["Caloricity"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetProductsWithCaloricityInRange(int startCaloricity, int endCaloricity)
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand($"""
                    SELECT Name, Color, Caloricity
                    FROM (
                        SELECT Name, Color, Caloricity FROM Fruits WHERE Caloricity BETWEEN {startCaloricity} AND {endCaloricity}
                        UNION ALL
                        SELECT Name, Color, Caloricity FROM Vegetables WHERE Caloricity BETWEEN {startCaloricity} AND {endCaloricity}
                    ) AS FruitsVegetables
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]}, {reader["Color"]}, {reader["Caloricity"]}");
                    }
                } while (reader.NextResult());
            });
        }
        private static void GetProductsWithColorRedOrOrange()
        {
            RequestToDatabase(connection =>
            {
                SqlCommand sqlCommand = new SqlCommand($"""
                    SELECT Name, Color, Caloricity
                    FROM (
                        SELECT Name, Color, Caloricity FROM Fruits WHERE Color IN ('Red', 'Orange')
                        UNION ALL
                        SELECT Name, Color, Caloricity FROM Vegetables WHERE Color IN ('Red', 'Orange')
                    ) AS FruitsVegetables
                    """, connection);
                SqlDataReader reader = sqlCommand.ExecuteReader();
                do
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]}, {reader["Color"]}, {reader["Caloricity"]}");
                    }
                } while (reader.NextResult());
            });
        }
    }

    public class Fruit : Products { }

    public class Vegetable : Products { }

    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int Caloricity { get; set; }

        public string ToStringSql()
        {
            return $"('{Name}', '{Color}', {Caloricity}),";
        }
    }
}
