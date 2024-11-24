using AIDemos.Models;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;

namespace AIDemos.Controllers
{
    public class CompanyController : Controller
    {
        private List<Company> lstCompanies = null!;
        private CompanyData db = null!;
        //private static string companiesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Companies");
        //private static readonly string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Companies", "com_demo.db");
        //private static readonly string connectionString = $"Data Source={dbFilePath};";

        public IActionResult Index()
        {
            CompanyData.EnsureDemoDatabaseExists();

            return View();
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            var dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases");

            if (!Directory.Exists(dbFolder))
            {
                // Ensure the directory exists
                Directory.CreateDirectory(dbFolder);
            }

            // Get all .db files in the Databases folder
            var dbFiles = Directory.GetFiles(dbFolder, "*.db");

            var companyList = new List<CompanyDatabase>();

            // Check if "demo_company.db" is present
            bool demoCompanyExists = dbFiles.Any(file => Path.GetFileName(file) == "demo_company.db");

            // If "demo_company.db" is not found, create it by calling CreateAndInsertCompany
            if (!demoCompanyExists)
            {
                CreateAndInsertCompany(); // Call the function to create the company and database

                // Add the newly created "demo_company.db" to the list
                companyList.Add(new CompanyDatabase
                {
                    DatabaseName = "demo_company.db"
                });
            }

            // Add all existing databases to the list
            foreach (var file in dbFiles)
            {
                companyList.Add(new CompanyDatabase
                {
                    DatabaseName = Path.GetFileName(file)
                });
            }

            return Json(companyList);
        }


        [HttpGet]
        public IActionResult LoadCompanyData(string dbFileName)
        {
            var dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases");

            var dbFilePath = Path.Combine(dbFolder, dbFileName);

            if (!System.IO.File.Exists(dbFilePath))
            {
                return NotFound("Database file not found.");
            }

            var connectionString = $"Data Source={dbFilePath};";
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
            connection.Open();

            var companyData = new List<Company>();

            string selectCommand = "SELECT * FROM Company";
            using var selectCmd = new Microsoft.Data.Sqlite.SqliteCommand(selectCommand, connection);
            using var reader = selectCmd.ExecuteReader();

            while (reader.Read())
            {
                companyData.Add(new Company
                {
                    CompanyID = reader["CompanyID"] as byte[],
                    CompanyName = reader["CompanyName"].ToString(),
                    Address = reader["Address"].ToString(),
                    City = reader["City"].ToString(),
                    State = reader["State"].ToString(),
                    ZipCode = reader["ZipCode"].ToString(),
                    Country = reader["Country"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Fax = reader["Fax"].ToString(),
                    Email = reader["Email"].ToString(),
                    Website = reader["Website"].ToString(),
                    Contact = reader["Contact"].ToString(),
                    DateOfIncorporation = reader["DateOfIncorporation"] as DateTime?,
                    TaxIDNumber = reader["TaxIDNumber"].ToString(),
                    BusinessType = reader["BusinessType"].ToString(),
                    Industry = reader["Industry"].ToString(),
                    Logo = reader["Logo"].ToString(),
                    Notes = reader["Notes"].ToString(),
                    Active = reader["Active"].ToString() == "1",
                    DatabaseName = dbFileName
                });
            }

            string humanReadableDateTime = DatabaseHelper.ExtractDateTimeFromDatabaseName(dbFileName);
            Console.WriteLine($"Extracted date and time: {humanReadableDateTime}");

            return Json(new { companyData });
        }

        public async Task<IActionResult> CreateNewCompany()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var companyData = JsonConvert.DeserializeObject<Company>(body);

                if (companyData == null)
                {
                    return BadRequest("Invalid company data.");
                }

                string? companyName = companyData.CompanyName;
                string? sanitizedCompanyName = new string(companyName!.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower();
                long id = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Example ID using Unix timestamp
                string databaseName = $"{sanitizedCompanyName}_{id}.db";

                companyData.DatabaseName = databaseName;

                var dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases");

                var dbFilePath = Path.Combine(dbFolder, databaseName);

                // Check if a database with the same name already exists in the folder
                if (System.IO.File.Exists(dbFilePath))
                {
                    return Conflict("A database with this name already exists.");
                }

                // Check if the company name is already used in any of the other databases
                foreach (var file in Directory.GetFiles(dbFolder, "*.db"))
                {
                    var connectionStringCheck = $"Data Source={file};";
                    using var connectionCheck = new Microsoft.Data.Sqlite.SqliteConnection(connectionStringCheck);
                    connectionCheck.Open();

                    string checkCompanyNameQuery = "SELECT COUNT(1) FROM Company WHERE CompanyName = @CompanyName";
                    using var checkCmd = new SqliteCommand(checkCompanyNameQuery, connectionCheck);
                    checkCmd.Parameters.AddWithValue("@CompanyName", companyData.CompanyName);

                    var existingCompanyCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (existingCompanyCount > 0)
                    {
                        return Conflict("A company with this name already exists in another database.");
                    }
                }

                // Proceed with creating the new database and inserting the company data
                var connectionString = $"Data Source={dbFilePath};";
                using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
                connection.Open();

                string tableCommand = @"
                    CREATE TABLE IF NOT EXISTS Company (
                        CompanyID BLOB PRIMARY KEY,
                        CompanyName TEXT NOT NULL,
                        Address TEXT,
                        City TEXT,
                        State TEXT,
                        ZipCode TEXT,
                        Country TEXT,
                        Phone TEXT,
                        Fax TEXT,
                        Email TEXT,
                        Website TEXT,
                        Contact TEXT,
                        DateOfIncorporation DATE,
                        TaxIDNumber TEXT,
                        BusinessType TEXT,
                        Industry TEXT,
                        Logo TEXT,
                        Notes TEXT,
                        Active INTEGER,
                        DatabaseName TEXT
                    )";

                using var createTableCmd = new Microsoft.Data.Sqlite.SqliteCommand(tableCommand, connection);
                createTableCmd.ExecuteNonQuery();

                string command = string.Empty;
                byte[] companyID = companyData.CompanyID!;

                // Insert a new record with a generated CompanyID
                command = @"
                    INSERT INTO Company (CompanyID, CompanyName, Address, City, State, ZipCode, Country, Phone, Fax, Email, Website, Contact, DateOfIncorporation, TaxIDNumber, BusinessType, Industry, Logo, Notes, Active, DatabaseName)
                    VALUES (randomblob(16), @CompanyName, @Address, @City, @State, @ZipCode, @Country, @Phone, @Fax, @Email, @Website, @Contact, @DateOfIncorporation, @TaxIDNumber, @BusinessType, @Industry, @Logo, @Notes, @Active, @DatabaseName)";

                // Now execute the command
                using var commandObj = new SqliteCommand(command, connection);
                commandObj.Parameters.AddWithValue("@CompanyID", companyID);
                commandObj.Parameters.AddWithValue("@CompanyName", companyData.CompanyName ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Address", companyData.Address ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@City", companyData.City ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@State", companyData.State ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@ZipCode", companyData.ZipCode ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Country", companyData.Country ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Phone", companyData.Phone ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Fax", companyData.Fax ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Email", companyData.Email ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Website", companyData.Website ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Contact", companyData.Contact ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@DateOfIncorporation", companyData.DateOfIncorporation ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@TaxIDNumber", companyData.TaxIDNumber ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@BusinessType", companyData.BusinessType ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Industry", companyData.Industry ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Logo", companyData.Logo ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Notes", companyData.Notes ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Active", companyData.Active ? 1 : 0);
                commandObj.Parameters.AddWithValue("@DatabaseName", companyData.DatabaseName ?? (object)DBNull.Value);

                commandObj.ExecuteNonQuery();

                //var companyList = new List<CompanyDatabase>();
                //companyList = (List<CompanyDatabase>)GetCompanies();

                return Ok("Company created successfully.");
            }
        }

        public class DatabaseHelper
        {
            public static string ExtractDateTimeFromDatabaseName(string databaseName)
            {
                // Check if the database name is "demo_company.db"
                if (databaseName == "demo_company.db")
                {
                    // Return the date for Christmas evening at midnight in 1869
                    DateTime christmasEve1869 = new DateTime(1869, 12, 24, 0, 0, 0, DateTimeKind.Utc);
                    return christmasEve1869.ToString("yyyy-MM-dd HH:mm:ss");
                }

                // Split the database name to extract the timestamp part (assuming format {sanitizedCompanyName}_{id}.db)
                string[] parts = databaseName.Split('_', '.');
                if (parts.Length >= 2 && long.TryParse(parts[1], out long unixTimestamp))
                {
                    // Convert Unix timestamp to DateTime
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimestamp);
                    DateTime dateTime = dateTimeOffset.UtcDateTime;

                    // Return a human-readable format
                    return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    // Return the date for Christmas evening at midnight in 1869
                    DateTime christmasEve1869 = new DateTime(1869, 12, 24, 0, 0, 0, DateTimeKind.Utc);
                    return christmasEve1869.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }
        public async Task<IActionResult> UpdateCompany()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                var companyData = JsonConvert.DeserializeObject<Company>(body);

                if (companyData == null)
                {
                    return BadRequest("Invalid company data.");
                }

                string? databaseName = companyData.DatabaseName;

                var dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases");

                var dbFilePath = Path.Combine(dbFolder, databaseName!);

                var connectionString = $"Data Source={dbFilePath};";
                using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
                connection.Open();

                string command = string.Empty;
                byte[] companyID = companyData.CompanyID!;

                // Update the existing record where CompanyID matches
                command = @"
                    UPDATE Company
                    SET CompanyName = @CompanyName, Address = @Address, City = @City, State = @State, ZipCode = @ZipCode, Country = @Country, Phone = @Phone, Fax = @Fax, Email = @Email, Website = @Website, Contact = @Contact, DateOfIncorporation = @DateOfIncorporation, 
                    TaxIDNumber = @TaxIDNumber, BusinessType = @BusinessType, Industry = @Industry, Logo = @Logo, Notes = @Notes, Active = @Active
                    WHERE CompanyID = @CompanyID";

                // Now execute the command
                using var commandObj = new SqliteCommand(command, connection);
                commandObj.Parameters.AddWithValue("@CompanyID", companyID);
                commandObj.Parameters.AddWithValue("@CompanyName", companyData.CompanyName ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Address", companyData.Address ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@City", companyData.City ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@State", companyData.State ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@ZipCode", companyData.ZipCode ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Country", companyData.Country ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Phone", companyData.Phone ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Fax", companyData.Fax ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Email", companyData.Email ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Website", companyData.Website ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Contact", companyData.Contact ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@DateOfIncorporation", companyData.DateOfIncorporation ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@TaxIDNumber", companyData.TaxIDNumber ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@BusinessType", companyData.BusinessType ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Industry", companyData.Industry ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Logo", companyData.Logo ?? (object)DBNull.Value);
                commandObj.Parameters.AddWithValue("@Notes", companyData.Notes ?? (object)DBNull.Value);

                commandObj.Parameters.AddWithValue("@Active", companyData.Active ? 1 : 0);

                commandObj.ExecuteNonQuery();

                return Ok("Company created successfully.");
            }
        }

        public IActionResult CreateAndInsertCompany()
        {
            var company = new Company
            {
                CompanyID = new byte[] { 19, 21, 17, 237, 129, 23, 224, 51, 11, 239, 187, 116, 28, 212, 164, 33 },
                CompanyName = "Demo Corp, Inc.",
                Address = "123 Demo St\nSuite 137",
                City = "Santa Monica",
                State = "CA",
                ZipCode = "90401",
                Country = "USA",
                Phone = "123-456-7890",
                Fax = "098-765-4321",
                Email = "demo@ainetprofit.com",
                Website = "https://ainetprofit.com/demo.png",
                Contact = "John Doe",
                DateOfIncorporation = null, // Empty string provided, so null in this case
                TaxIDNumber = "123-45-6789",
                BusinessType = "S Corp",
                Industry = "Software",
                Logo = "logo.png",
                Notes = "Notes",
                Active = true,
                DatabaseName = "demo_company.db"
            };

            var dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases");

            var dbFilePath = Path.Combine(dbFolder, company.DatabaseName);

            var connectionString = $"Data Source={dbFilePath};";
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
            connection.Open();

            string tableCommand = @"
        CREATE TABLE IF NOT EXISTS Company (
            CompanyID BLOB PRIMARY KEY,
            CompanyName TEXT NOT NULL,
            Address TEXT,
            City TEXT,
            State TEXT,
            ZipCode TEXT,
            Country TEXT,
            Phone TEXT,
            Fax TEXT,
            Email TEXT,
            Website TEXT,
            Contact TEXT,
            DateOfIncorporation DATE,
            TaxIDNumber TEXT,
            BusinessType TEXT,
            Industry TEXT,
            Logo TEXT,
            Notes TEXT,
            Active INTEGER
        )";

            using var createTableCmd = new Microsoft.Data.Sqlite.SqliteCommand(tableCommand, connection);
            createTableCmd.ExecuteNonQuery();

            string insertCommand = @"
        INSERT INTO Company (CompanyID, CompanyName, Address, City, State, ZipCode, Country, Phone, Fax, Email, Website, Contact, DateOfIncorporation, TaxIDNumber, BusinessType, Industry, Logo, Notes, Active)
        VALUES (@CompanyID, @CompanyName, @Address, @City, @State, @ZipCode, @Country, @Phone, @Fax, @Email, @Website, @Contact, @DateOfIncorporation, @TaxIDNumber, @BusinessType, @Industry, @Logo, @Notes, @Active)";

            using var insertCmd = new Microsoft.Data.Sqlite.SqliteCommand(insertCommand, connection);
            insertCmd.Parameters.AddWithValue("@CompanyID", company.CompanyID);
            insertCmd.Parameters.AddWithValue("@CompanyName", company.CompanyName);
            insertCmd.Parameters.AddWithValue("@Address", company.Address);
            insertCmd.Parameters.AddWithValue("@City", company.City);
            insertCmd.Parameters.AddWithValue("@State", company.State);
            insertCmd.Parameters.AddWithValue("@ZipCode", company.ZipCode);
            insertCmd.Parameters.AddWithValue("@Country", company.Country);
            insertCmd.Parameters.AddWithValue("@Phone", company.Phone);
            insertCmd.Parameters.AddWithValue("@Fax", company.Fax);
            insertCmd.Parameters.AddWithValue("@Email", company.Email);
            insertCmd.Parameters.AddWithValue("@Website", company.Website);
            insertCmd.Parameters.AddWithValue("@Contact", company.Contact);
            insertCmd.Parameters.AddWithValue("@DateOfIncorporation", company.DateOfIncorporation ?? (object)DBNull.Value);
            insertCmd.Parameters.AddWithValue("@TaxIDNumber", company.TaxIDNumber);
            insertCmd.Parameters.AddWithValue("@BusinessType", company.BusinessType);
            insertCmd.Parameters.AddWithValue("@Industry", company.Industry);
            insertCmd.Parameters.AddWithValue("@Logo", company.Logo);
            insertCmd.Parameters.AddWithValue("@Notes", company.Notes);
            insertCmd.Parameters.AddWithValue("@Active", company.Active ? 1 : 0);

            insertCmd.ExecuteNonQuery();

            return Ok("Company inserted successfully.");
        }

        [HttpGet]
        public IActionResult GetArraysData()
        {
            var data = new[]
            {
                new[] { "William SerGio & Co., Inc." },
                new[] { "Ouslan, Inc." },
                new[] { "Ashton Cox" },
                new[] { "GeminiGroup TV, Inc." },
                new[] { "Apple Inc." },
                new[] { "Microsoft Corporation" },
                new[] { "Google LLC" },
                new[] { "Amazon.com" },
                new[] { "Facebook, Inc." },
                new[] { "Tesla, Inc." },
                new[] { "Netflix, Inc." },
                new[] { "Oracle Corporation" },
                new[] { "Coca-Cola Company" },
                new[] { "Nike, Inc." },
                new[] { "Procter & Gamble" },
                new[] { "PepsiCo, Inc." },
                new[] { "Intel Corporation" },
                new[] { "IBM Corporation" },
                new[] { "Boeing Company" },
                new[] { "Ford Motor Company" },
                new[] { "General Motors" }
            };

            return Json(new { data });
        }

    }
}


//[HttpPost]
//public async Task<IActionResult> CreateNewCompany()
//{
//    using (var reader = new StreamReader(Request.Body))
//    {
//        var body = await reader.ReadToEndAsync();
//        var companyData = JsonConvert.DeserializeObject<Company>(body);

//        if (companyData == null)
//        {
//            return BadRequest("Invalid company data.");
//        }

//        string? companyName = companyData.CompanyName;
//        string? sanitizedCompanyName = new string(companyName!.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLower();
//        long id = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Example ID using Unix timestamp
//        string databaseName = $"{sanitizedCompanyName}_{id}.db";
//        // Output: geminigroup_1695980800.db

//        var companiesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Companies");
//        var dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Companies", databaseName);
//        var connectionString = $"Data Source={dbFilePath};";

//        using var connection = new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
//        connection.Open();

//        string tableCommand = @"
//            CREATE TABLE IF NOT EXISTS Company (
//                CompanyID BLOB PRIMARY KEY,
//                CompanyName TEXT NOT NULL,
//                Address TEXT,
//                City TEXT,
//                State TEXT,
//                ZipCode TEXT,
//                Country TEXT,
//                Phone TEXT,
//                Fax TEXT,
//                Email TEXT,
//                Website TEXT,
//                Contact TEXT,
//                DateOfIncorporation DATE,
//                TaxIDNumber TEXT,
//                BusinessType TEXT,
//                Industry TEXT,
//                Logo TEXT,
//                Notes TEXT,
//                Active INTEGER
//            )";

//        using var createTableCmd = new Microsoft.Data.Sqlite.SqliteCommand(tableCommand, connection);
//        createTableCmd.ExecuteNonQuery();

//        var company = new Company
//        {
//            CompanyID = companyData.CompanyID,
//            CompanyName = companyData.CompanyName,
//            Address = companyData.Address,
//            City = companyData.City,
//            State = companyData.State,
//            ZipCode = companyData.ZipCode,
//            Country = companyData.Country,
//            Phone = companyData.Phone,
//            Fax = companyData.Fax,
//            Email = companyData.Email,
//            Website = companyData.Website,
//            Contact = companyData.Contact,
//            DateOfIncorporation = companyData.DateOfIncorporation,
//            TaxIDNumber = companyData.TaxIDNumber,
//            BusinessType = companyData.BusinessType,
//            Industry = companyData.Industry,
//            Logo = companyData.Logo,
//            Notes = companyData.Notes,
//            Active = companyData.Active
//        };

//        string insertCommand = @"
//        INSERT INTO Company (CompanyID, CompanyName, Address, City, State, ZipCode, Country, Phone, Fax, Email, Website, Contact, DateOfIncorporation, TaxIDNumber, BusinessType, Industry, Logo, Notes, Active)
//        VALUES (randomblob(16), @CompanyName, @Address, @City, @State, @ZipCode, @Country, @Phone, @Fax, @Email, @Website, @Contact, @DateOfIncorporation, @TaxIDNumber, @BusinessType, @Industry, @Logo, @Notes, @Active)";

//        using var insertCmd = new SqliteCommand(insertCommand, connection);
//        insertCmd.Parameters.AddWithValue("@CompanyName", company.CompanyName ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Address", company.Address ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@City", company.City ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@State", company.State ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@ZipCode", company.ZipCode ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Country", company.Country ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Phone", company.Phone ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Fax", company.Fax ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Email", company.Email ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Website", company.Website ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Contact", company.Contact ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@DateOfIncorporation", company.DateOfIncorporation ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@TaxIDNumber", company.TaxIDNumber ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@BusinessType", company.BusinessType ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Industry", company.Industry ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Logo", company.Logo ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Notes", company.Notes ?? (object)DBNull.Value);
//        insertCmd.Parameters.AddWithValue("@Active", company.Active ? 1 : 0);

//        insertCmd.ExecuteNonQuery();

//        return Ok("Company created successfully.");
//    }
//}