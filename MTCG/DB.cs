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
                CREATE TABLE IF NOT EXISTS userTable(
                username VARCHAR(256) NOT NULL,
                password VARCHAR(256) NOT NULL,
                role VARCHAR(256) NOT NULL,
                PRIMARY KEY(username)
                )", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS cardTable(
                id VARCHAR(256) NOT NULL,
                cardname VARCHAR(256) NOT NULL,
                damage DOUBLE PRECISION NOT NULL,
                elementType VARCHAR(256),
                monsterType VARCHAR(256),
                packageID INTEGER,
                username VARCHAR(256),
                deck BOOLEAN,
                PRIMARY KEY(id),
                CONSTRAINT fk_user
                    FOREIGN KEY(username)
                        REFERENCES userTable(username)
                )", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS statsTable(
                username VARCHAR(256) NOT NULL,
                elo BIGINT NOT NULL,
                wins BIGINT NOT NULL,
                looses BIGINT NOT NULL,
                draws BIGINT NOT NULL,
                coins BIGINT NOT NULL,
                realname VARCHAR(256),
                bio VARCHAR(256),
                image VARCHAR(256),
                PRIMARY KEY(username),
                CONSTRAINT fk_user
                    FOREIGN KEY(username)
                        REFERENCES userSchema(username)
                )", conn))
            {
                cmd.ExecuteNonQuery();
            }

            conn.Close();

        }

        public static NpgsqlConnection Connection()
        {
            var conn = new NpgsqlConnection(connString);
            conn.Open();
            return conn;
        }
    }
}
