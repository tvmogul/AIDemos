using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Security.Cryptography;
using System.Text;

public class DBSQL
{
    private static readonly string pathSecurityDB = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "security.db");
    private static readonly string cnnSecurityString = $"Data Source={pathSecurityDB};";

    string clientId, clientSecret, username, password, tokenRequestEndpoint;
    string apiVersion, authorizationEndpoint, salesForceDCToken, accessToken;
    string? ExceptionMessage { get; set; }

    IServer server;
    IConfiguration config;
    IWebHostEnvironment env;
    //Serilog.ILogger? logger;
    IHttpContextAccessor httpContextAccessor;

    public static int MimeSampleSize = 256;
    public static string DefaultMimeType = "application/octet-stream";

    bool bRequire = false;

    public DBSQL(IServer server,
                IConfiguration config,
                IWebHostEnvironment env,
                //Serilog.ILogger? logger,
                IHttpContextAccessor httpContextAccessor)
    {
        this.server = server;
        this.config = config;
        this.env = env;
        //this.logger = logger;
        this.httpContextAccessor = httpContextAccessor;

    }

    public static void InitializeDatabase()
    {
        // Check if the database file exists
        if (!File.Exists(pathSecurityDB))
        {
            // Create the directory if it doesn't exist
            var directory = Path.GetDirectoryName(pathSecurityDB);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create the database and Users table
            using (var connection = new SqliteConnection(cnnSecurityString))
            {
                connection.Open();

                string tableCommand = @"CREATE TABLE IF NOT EXISTS Users (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Email TEXT NOT NULL UNIQUE,
                                        PasswordHash TEXT NOT NULL
                                        );";

                using (var createTable = new SqliteCommand(tableCommand, connection))
                {
                    createTable.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }

    // Method to hash a password (use SHA256 or stronger)
    public static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Example of inserting a user (email and hashed password)
    public static void InsertUser(string email, string password)
    {
        string hashedPassword = HashPassword(password);

        using (var connection = new SqliteConnection(cnnSecurityString))
        {
            connection.Open();

            string insertCommand = "INSERT INTO Users (Email, PasswordHash) VALUES (@Email, @PasswordHash)";

            using (var insertUser = new SqliteCommand(insertCommand, connection))
            {
                insertUser.Parameters.AddWithValue("@Email", email);
                insertUser.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                try
                {
                    insertUser.ExecuteNonQuery();
                }
                catch (SqliteException e)
                {
                    // Handle unique constraint violation (email already exists)
                    if (e.SqliteErrorCode == 19) // SQLITE_CONSTRAINT
                    {
                        Console.WriteLine("Email already exists.");
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            connection.Close();
        }
    }

} //END DBSQL










