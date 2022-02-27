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
                displayname VARCHAR(256),
                bio VARCHAR(256),
                image VARCHAR(256),
                coins INTEGER NOT NULL,
                isLoggedIn BOOLEAN,
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
                CREATE TABLE IF NOT EXISTS scoreTable(
                username VARCHAR(256) NOT NULL,
                elo INTEGER NOT NULL,
                wins INTEGER NOT NULL,
                losses INTEGER NOT NULL,
                draws INTEGER NOT NULL,
                PRIMARY KEY(username),
                CONSTRAINT fk_user
                    FOREIGN KEY(username)
                        REFERENCES userTable(username)
                )", conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new NpgsqlCommand(@"
                CREATE TABLE IF NOT EXISTS tradingTable(
                id VARCHAR(256) NOT NULL,
                cardId VARCHAR(256) NOT NULL,
                type VARCHAR(256) NOT NULL,
                minDamage INTEGER NOT NULL,
                PRIMARY KEY(id),
                CONSTRAINT fk_id
                    FOREIGN KEY(cardId)
                        REFERENCES cardTable(id)
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
