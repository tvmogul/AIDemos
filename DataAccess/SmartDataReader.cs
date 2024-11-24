
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.Sqlite;


public sealed class SmartDataReader
{
    private DateTime defaultDate;
    public SmartDataReader(SqliteDataReader reader)
    {
        this.defaultDate = DateTime.MinValue;
        this.reader = reader;
    }
    public SmartDataReader(IDataReader reader)
    {
        this.defaultDate = DateTime.MinValue;
        this.reader = (SqliteDataReader)reader;
    }

    public Int64 GetInt64(String column)
    {
        Int64 data = (reader.IsDBNull(reader.GetOrdinal(column))) ? 0 : (Int64)reader[column];
        return data;
    }

    public int GetInt32(String column)
    {
        int data = 0;
        try
        {
            data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (int)0 : Convert.ToInt32(reader[column]);
        }
        catch(Exception) {
            if(reader[column] != null)
            {
                data = Convert.ToInt32(reader[column]);
            }
        }
            
        return data;
    }

    public short GetInt16(String column)
    {
        short data = (reader.IsDBNull(reader.GetOrdinal(column))) ? (short)0 : (short)reader[column];
        return data;
    }

    public byte[] GetBytes(string column)
    {
        byte[]? data = (reader.IsDBNull(reader.GetOrdinal(column))) ? null : (byte[])(reader[column]);
        return data!;
    }

    public float GetFloat(String column)
    {
#pragma warning disable CS8604 // Possible null reference argument for parameter 's' in 'float float.Parse(string s)'.
        float data = (reader.IsDBNull(reader.GetOrdinal(column))) ? 0 : float.Parse(reader[column].ToString());
#pragma warning restore CS8604 // Possible null reference argument for parameter 's' in 'float float.Parse(string s)'.
        return data;
    }

    public decimal GetDecimal(String column)
    {
#pragma warning disable CS8604 // Possible null reference argument for parameter 's' in 'decimal decimal.Parse(string s)'.
        decimal data = (reader.IsDBNull(reader.GetOrdinal(column))) ? 0 : decimal.Parse(reader[column].ToString());
#pragma warning restore CS8604 // Possible null reference argument for parameter 's' in 'decimal decimal.Parse(string s)'.
        return data;
    }

    public double GetDouble(String column)
    {
#pragma warning disable CS8604 // Possible null reference argument for parameter 's' in 'double double.Parse(string s)'.
        double data = (reader.IsDBNull(reader.GetOrdinal(column))) ? 0 : double.Parse(reader[column].ToString());
#pragma warning restore CS8604 // Possible null reference argument for parameter 's' in 'double double.Parse(string s)'.
        return data;
    }

    public bool GetBoolean(String column)
    {
        bool data = (reader.IsDBNull(reader.GetOrdinal(column))) ? false : (bool)reader[column];
        return data;
    }

    public String? GetString(String column)
    {
        String? data = reader.IsDBNull(reader.GetOrdinal(column)) ? null : Convert.ToString(reader[column]);
        return data;
    }

    public DateTime GetDateTime(String column)
    {
        DateTime data = (reader.IsDBNull(reader.GetOrdinal(column))) ? defaultDate : (DateTime)reader[column];
        return data;
    }

    public bool Read()
    {
        return this.reader.Read();
    }
    private SqliteDataReader reader;
}





