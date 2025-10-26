// File: RegisterAndLoginApp/Models/UsersDAO.cs
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace RegisterAndLoginApp.Models
{

    public class UsersDAO : IUserManager
    {

        private const string ConnectionString =
     "Server=127.0.0.1;Port=3306;Database=Test;Uid=root;Pwd=root;AllowPublicKeyRetrieval=True;SslMode=Preferred;";


        // ---------- CREATE ----------
        public int AddUser(UserModel user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            const string sql = @"
INSERT INTO Users (Username, PasswordHash, Salt, `Groups`)
VALUES (@Username, @PasswordHash, @Salt, @Groups);
SELECT LAST_INSERT_ID();";

            using var conn = new MySqlConnection(ConnectionString);
            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.Add("@Username", MySqlDbType.VarChar, 50).Value = user.Username;
            cmd.Parameters.Add("@PasswordHash", MySqlDbType.VarChar, 50).Value = user.PasswordHash;
            cmd.Parameters.Add("@Salt", MySqlDbType.VarBinary, 16).Value = user.Salt ?? Array.Empty<byte>();
            cmd.Parameters.Add("@Groups", MySqlDbType.Text).Value = (object?)user.Groups ?? DBNull.Value;

            conn.Open();
            object scalar = cmd.ExecuteScalar();
            return Convert.ToInt32(scalar);
        }

        // ---------- AUTH ----------
        public int CheckCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || password is null) return 0;

            const string sql = @"
SELECT Id, Username, PasswordHash, Salt, `Groups`
FROM Users
WHERE Username = @Username
LIMIT 1;";

            using var conn = new MySqlConnection(ConnectionString);
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.Add("@Username", MySqlDbType.VarChar, 50).Value = username.Trim();

            conn.Open();
            using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (!reader.Read()) return 0;

            var dbUser = MapUser(reader);
            return dbUser.VerifyPassword(password) ? dbUser.Id : 0;
        }

        // ---------- DELETE ----------
        public void DeleteUser(UserModel user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            const string sql = "DELETE FROM Users WHERE Id = @Id;";

            using var conn = new MySqlConnection(ConnectionString);
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.Add("@Id", MySqlDbType.Int32).Value = user.Id;

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // ---------- READ (ALL) ----------
        public List<UserModel> GetAllUsers()
        {
            const string sql = "SELECT Id, Username, PasswordHash, Salt, `Groups` FROM Users ORDER BY Id;";

            var results = new List<UserModel>();
            using var conn = new MySqlConnection(ConnectionString);
            using var cmd = new MySqlCommand(sql, conn);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(MapUser(reader));
            }
            return results;
        }

        // ---------- READ (BY ID) ----------
        public UserModel? GetUserById(int id)
        {
            const string sql = @"
SELECT Id, Username, PasswordHash, Salt, `Groups`
FROM Users
WHERE Id = @Id
LIMIT 1;";

            using var conn = new MySqlConnection(ConnectionString);
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.Add("@Id", MySqlDbType.Int32).Value = id;

            conn.Open();
            using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            return reader.Read() ? MapUser(reader) : null;
        }

        // ---------- UPDATE ----------
        public void UpdateUser(UserModel user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            const string sql = @"
UPDATE Users
SET Username = @Username,
    PasswordHash = @PasswordHash,
    Salt = @Salt,
    `Groups` = @Groups
WHERE Id = @Id;";

            using var conn = new MySqlConnection(ConnectionString);
            using var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.Add("@Username", MySqlDbType.VarChar, 50).Value = user.Username;
            cmd.Parameters.Add("@PasswordHash", MySqlDbType.VarChar, 50).Value = user.PasswordHash;
            cmd.Parameters.Add("@Salt", MySqlDbType.VarBinary, 16).Value = user.Salt ?? Array.Empty<byte>();
            cmd.Parameters.Add("@Groups", MySqlDbType.Text).Value = (object?)user.Groups ?? DBNull.Value;
            cmd.Parameters.Add("@Id", MySqlDbType.Int32).Value = user.Id;

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // ---------- Helper ----------
        private static UserModel MapUser(IDataRecord r)
        {
            return new UserModel
            {
                Id = r.GetInt32(r.GetOrdinal("Id")),
                Username = r.GetString(r.GetOrdinal("Username")),
                PasswordHash = r.GetString(r.GetOrdinal("PasswordHash")),
                Salt = (byte[])r["Salt"],
                Groups = r.IsDBNull(r.GetOrdinal("Groups")) ? string.Empty : r.GetString(r.GetOrdinal("Groups"))
            };
        }
    }
}
