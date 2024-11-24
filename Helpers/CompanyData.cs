using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using AIDemos.Models;
using System.Data;

using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;

public class CompanyData
{
    private List<Company> lstCompanies = null!;
    private static string dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases");
    private static readonly string dbDemoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", "demo_company.db");
    private static readonly string dbDemoConnection = $"Data Source={dbDemoPath};";

    public static void EnsureDemoDatabaseExists()
    {
        // Ensure "Databases" folder exists
        if (!Directory.Exists(dbFolder))
        {
            // Ensure directory exists
            Directory.CreateDirectory(dbFolder);
        }

        // Check if demo database file exists, if not, create demo database
        if (!File.Exists(dbDemoPath))
        {
            CreateDemoDatabase();
        }
    }

    private static void CreateDemoDatabase()
    {
        // Check if the database folder exists, if not, create it
        if (!Directory.Exists(dbFolder))
        {
            Directory.CreateDirectory(dbFolder); // Creates the directory if it doesn't exist
        }

        // Check if the database file exists
        if (!File.Exists(dbDemoPath))
        {
            // If the database file doesn't exist, create it and the table
            Console.WriteLine("Database does not exist. Creating the database and table...");

            using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbDemoPath}");
            connection.Open();

            // SQL command to create the Company table
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

            // Execute command to create table
            using var createTableCmd = new Microsoft.Data.Sqlite.SqliteCommand(tableCommand, connection);
            createTableCmd.ExecuteNonQuery();

            // Insert demo data after creating the table
            InsertDemoCompanyData(connection);
        }
        else
        {
            // If the database exists, check if the "Company" table exists
            Console.WriteLine("Database exists. Checking if the 'Company' table exists...");

            using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={dbDemoPath}");
            connection.Open();

            // SQL to check if the "Company" table exists
            string checkTableCommand = @"
            SELECT count(*) 
            FROM sqlite_master 
            WHERE type='table' 
            AND name='Company';";

            using var checkTableCmd = new Microsoft.Data.Sqlite.SqliteCommand(checkTableCommand, connection);
            var tableExists = Convert.ToInt32(checkTableCmd.ExecuteScalar());

            if (tableExists == 0)
            {
                // If the "Company" table does not exist, create it
                Console.WriteLine("'Company' table does not exist. Creating the table...");

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

                // Insert demo data after creating the table
                InsertDemoCompanyData(connection);
            }
            else
            {
                Console.WriteLine("'Company' table already exists.");
            }
        }
    }

    private static void InsertDemoCompanyData(Microsoft.Data.Sqlite.SqliteConnection connection)
    {
        // Insert demo data into the "Company" table
        Company demoCompany = new Company
        {
            CompanyID = Guid.NewGuid().ToByteArray(),     // Generate a new GUID for the company ID
            CompanyName = "AINetProfit Corp, Inc.",       // Set company name
            Address = "123 AINetProfit St\r\nSuite 137",  // Set address with a line break
            City = "Santa Monica",                        // Set city
            State = "CA",                                 // Set state
            ZipCode = "90401",                            // Set zip code
            Country = "USA",                              // Set country
            Phone = "123-456-7890",                       // Set phone number
            Fax = "098-765-4321",                         // Set fax number
            Email = "demo@ainetprofit.com",               // Set email
            Website = "https://ainetprofit.com/",         // Set website URL
            Contact = "John Doe",                         // Set contact person
            DateOfIncorporation = DateTime.Parse("2024-01-01"),  // Set date of incorporation
            TaxIDNumber = "123-45-6789",                  // Set Tax ID number
            BusinessType = "S Corp",                      // Set business type
            Industry = "Software",                        // Set industry
            Logo = "logo.png",                            // Set logo filename (or path)
            Notes = "Notes",                              // Set additional notes
            Active = true,                                // Set the company as active (1 for true)
            DatabaseName = "demo_company.db"
        };

        string insertCommand = @"
        INSERT INTO Company 
        (CompanyID, CompanyName, Address, City, State, ZipCode, Country, Phone, Fax, Email, Website, Contact, DateOfIncorporation, TaxIDNumber, BusinessType, Industry, Logo, Notes, Active) 
        VALUES 
        (@CompanyID, @CompanyName, @Address, @City, @State, @ZipCode, @Country, @Phone, @Fax, @Email, @Website, @Contact, @DateOfIncorporation, @TaxIDNumber, @BusinessType, @Industry, @Logo, @Notes, @Active)";

        using var insertCmd = new Microsoft.Data.Sqlite.SqliteCommand(insertCommand, connection);
        insertCmd.Parameters.AddWithValue("@CompanyID", demoCompany.CompanyID);
        insertCmd.Parameters.AddWithValue("@CompanyName", demoCompany.CompanyName);
        insertCmd.Parameters.AddWithValue("@Address", demoCompany.Address);
        insertCmd.Parameters.AddWithValue("@City", demoCompany.City);
        insertCmd.Parameters.AddWithValue("@State", demoCompany.State);
        insertCmd.Parameters.AddWithValue("@ZipCode", demoCompany.ZipCode);
        insertCmd.Parameters.AddWithValue("@Country", demoCompany.Country);
        insertCmd.Parameters.AddWithValue("@Phone", demoCompany.Phone);
        insertCmd.Parameters.AddWithValue("@Fax", demoCompany.Fax);
        insertCmd.Parameters.AddWithValue("@Email", demoCompany.Email);
        insertCmd.Parameters.AddWithValue("@Website", demoCompany.Website);
        insertCmd.Parameters.AddWithValue("@Contact", demoCompany.Contact);
        insertCmd.Parameters.AddWithValue("@DateOfIncorporation", demoCompany.DateOfIncorporation);
        insertCmd.Parameters.AddWithValue("@TaxIDNumber", demoCompany.TaxIDNumber);
        insertCmd.Parameters.AddWithValue("@BusinessType", demoCompany.BusinessType);
        insertCmd.Parameters.AddWithValue("@Industry", demoCompany.Industry);
        insertCmd.Parameters.AddWithValue("@Logo", demoCompany.Logo);
        insertCmd.Parameters.AddWithValue("@Notes", demoCompany.Notes);
        insertCmd.Parameters.AddWithValue("@Active", demoCompany.Active ? 1 : 0);
        insertCmd.Parameters.AddWithValue("@DatabaseName", demoCompany.DatabaseName);

        insertCmd.ExecuteNonQuery(); // Executes the insert command
        Console.WriteLine("Demo company data inserted.");
    }

    public static void InsertCompany(Company company)
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases", company.DatabaseName!);

        using var connection = new SqliteConnection($"Data Source={dbPath}");
        connection.Open();

        string insertCommand = @"
                INSERT INTO Company (CompanyID, CompanyName, Address, City, State, ZipCode, Country, Phone, Fax, Email, Website, Contact, DateOfIncorporation, TaxIDNumber, BusinessType, Industry, Logo, Notes, Active)
                VALUES (randomblob(16), @CompanyName, @Address, @City, @State, @ZipCode, @Country, @Phone, @Fax, @Email, @Website, @Contact, @DateOfIncorporation, @TaxIDNumber, @BusinessType, @Industry, @Logo, @Notes, @Active)";

        using var insertCmd = new SqliteCommand(insertCommand, connection);
        insertCmd.Parameters.AddWithValue("@CompanyName", company.CompanyName ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Address", company.Address ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@City", company.City ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@State", company.State ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@ZipCode", company.ZipCode ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Country", company.Country ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Phone", company.Phone ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Fax", company.Fax ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Email", company.Email ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Website", company.Website ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Contact", company.Contact ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@DateOfIncorporation", company.DateOfIncorporation ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@TaxIDNumber", company.TaxIDNumber ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@BusinessType", company.BusinessType ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Industry", company.Industry ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Logo", company.Logo ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Notes", company.Notes ?? (object)DBNull.Value);
        insertCmd.Parameters.AddWithValue("@Active", company.Active ? 1 : 0);
        insertCmd.Parameters.AddWithValue("@DatabaseName", company.DatabaseName ?? (object)DBNull.Value);

        insertCmd.ExecuteNonQuery();
    }

    //public static List<Company> GetAllCompanies()
    //{
    //    var companyList = new List<CompanyDatabase>();

    //    return companyList;
    //}

    //public IActionResult GetCompanies()
    //{
    //    var dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases");

    //    if (!Directory.Exists(dbFolder))
    //    {
    //        // Ensure the directory exists
    //        Directory.CreateDirectory(dbFolder);
    //    }

    //    // Get all .db files in the Databases folder
    //    var dbFiles = Directory.GetFiles(dbFolder, "*.db");

    //    var companyList = new List<CompanyDatabase>();

    //    // Check if "demo_company.db" is present
    //    bool demoCompanyExists = dbFiles.Any(file => Path.GetFileName(file) == "demo_company.db");

    //    // If "demo_company.db" is not found, create it by calling CreateAndInsertCompany
    //    if (!demoCompanyExists)
    //    {
    //        CreateAndInsertCompany(); // Call the function to create the company and database

    //        // Add the newly created "demo_company.db" to the list
    //        companyList.Add(new CompanyDatabase
    //        {
    //            DatabaseName = "demo_company.db"
    //        });
    //    }

    //    // Add all existing databases to the list
    //    foreach (var file in dbFiles)
    //    {
    //        companyList.Add(new CompanyDatabase
    //        {
    //            DatabaseName = Path.GetFileName(file)
    //        });
    //    }

    //    return Json(companyList);
    //}


}


