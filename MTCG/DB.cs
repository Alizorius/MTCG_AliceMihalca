using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    public static class DB
    {
        public static string connString = null!;

        public static void CreateDBIfNotPresent()
        {
            string user = "postgres", password = "P0sT5r3S", ip = "localhost";
            int port = 5432;

            connString = $"Server={ip};Port={port};User Id={user};Password={password};";
            var conn = new NpgsqlConnection(connString);

            conn.Open();

            using var cmdChek = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname='mtcg'", conn);
            var dbExists = cmdChek.ExecuteScalar() != null;

            if (dbExists)
            {
                // Close general connection and build new one to database
                conn.Close();
                connString += "Database=mtcg;";
                conn = new NpgsqlConnection(connString);
                conn.Open();
                return;
            }

            using (var cmd = new NpgsqlCommand(@"
                CREATE DATABASE mtcg
                WITH OWNER = postgres
                ENCODING = 'UTF8'
                ", conn))
            {
                cmd.ExecuteNonQuery();
            }
            // Close general connection and build new one to database
            conn.Close();
            connString += "Database=mtcg;";
            conn = new NpgsqlConnection(connString);
            conn.Open();

            using (var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS userTabel(
                username VARCHAR(256) NOT NULL,
                password VARCHAR(256) NOT NULL,
                role VARCHAR(256) NOT NULL,
                PRIMARY KEY(username)
                )", conn))
            {
                cmd.ExecuteNonQuery();
            }


            conn.Close();

        }

        public static NpgsqlConnection Connection(string connString)
        {
            var conn = new NpgsqlConnection(connString);
            conn.Open();
            return conn;
        }
    }
}
