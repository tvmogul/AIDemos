using System;
using System.IO;
using System.Runtime.InteropServices;

public class DatabasePathManager
{
    private readonly string dbFolder;

    public DatabasePathManager()
    {
        dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            dbFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AINetProfit", "Databases");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            dbFolder = Path.Combine("/Users/Shared", "AINetProfit", "Databases");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            dbFolder = Path.Combine("/var", "AINetProfit", "Databases");
        }
        else
        {
            dbFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases");
        }

        // Ensure directory exists
        if (!Directory.Exists(dbFolder))
        {
            Directory.CreateDirectory(dbFolder);
        }
    }

    public string GetDatabasePath(string dbName)
    {
        return Path.Combine(dbFolder, $"{dbName}.db");
    }
}
