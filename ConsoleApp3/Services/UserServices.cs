//using System;
//using System.Collections.Generic;
//using System.Text;

//using System;
//using Microsoft.Data.SqlClient;

//internal class UserServices
//{
//    //public void CreateUser()
//    //{
//    //    //    Console.Write("ID: ");
//    //    //    int id = int.Parse(Console.ReadLine());
//    //    Console.Write("Username: ");
//    //    string u = Console.ReadLine();
//    //    Console.Write("Password: ");
//    //    string p = Console.ReadLine();
//    //    Console.Write("Email: ");
//    //    string e = Console.ReadLine();

//    //    using SqlConnection c = new SqlConnection(Db.conn);
//    //    SqlCommand cmd = new SqlCommand(
//    //        "INSERT INTO Users VALUES (@u,@p,@e,0,0)", c);
//    //    //cmd.Parameters.AddWithValue("@i", id);
//    //    cmd.Parameters.AddWithValue("@u", u);
//    //    cmd.Parameters.AddWithValue("@p", p);
//    //    cmd.Parameters.AddWithValue("@e", e);
//    //    c.Open();
//    //    cmd.ExecuteNonQuery();
//    //}

//    public void CreateUser()
//    {
//        Console.Write("Username: ");
//        string u = Console.ReadLine();

//        Console.Write("Password: ");
//        string p = Console.ReadLine();

//        Console.Write("Email: ");
//        string e = Console.ReadLine();

//        using SqlConnection c = new SqlConnection(Db.conn);

//        SqlCommand cmd = new SqlCommand(
//            @"INSERT INTO Users (Username, Password, Email, Subscription, Role)
//          VALUES (@u, @p, @e, 0, 0);
//          SELECT SCOPE_IDENTITY();", c);

//        cmd.Parameters.AddWithValue("@u", u);
//        cmd.Parameters.AddWithValue("@p", p);
//        cmd.Parameters.AddWithValue("@e", e);

//        c.Open();

//        int newUserId = Convert.ToInt32(cmd.ExecuteScalar());

//        Console.WriteLine("User created successfully!");
//        Console.WriteLine("Your User ID is: " + newUserId);
//    }


//    public void viewall()
//    {
//        using SqlConnection c = new SqlConnection(Db.conn);
//        SqlCommand cmd = new SqlCommand("SELECT * FROM Users", c);
//        c.Open();
//        SqlDataReader r = cmd.ExecuteReader();
//        while (r.Read())
//            Console.WriteLine($"{r["Id"]} {r["Username"]} {r["Role"]}");
//    }

//    public void removeuser(int id)
//    {
//        using SqlConnection c = new SqlConnection(Db.conn);
//        SqlCommand cmd = new SqlCommand(
//            "DELETE FROM Users WHERE Id=@id", c);
//        cmd.Parameters.AddWithValue("@id", id);
//        c.Open();
//        cmd.ExecuteNonQuery();
//    }

//    public int UserLogin()
//    {
//        Console.Write("ID: ");
//        int id = int.Parse(Console.ReadLine());
//        Console.Write("Password: ");
//        string p = Console.ReadLine();

//        using SqlConnection c = new SqlConnection(Db.conn);
//        SqlCommand cmd = new SqlCommand(
//            "SELECT Role FROM Users WHERE Id=@i AND Password=@p", c);
//        cmd.Parameters.AddWithValue("@i", id);
//        cmd.Parameters.AddWithValue("@p", p);
//        c.Open();
//        object role = cmd.ExecuteScalar();
//        if (role == null) return -1;
//        return (int)role == 1 ? 999 : id;
//    }
//}



using System;
using Microsoft.Data.SqlClient;
using System.Data;

internal class UserServices
{
    public void CreateUser()
    {
        Console.Write("Username: ");
        string username = Console.ReadLine();

        Console.Write("Password: ");
        string password = Console.ReadLine();

        Console.Write("Email: ");
        string email = Console.ReadLine();

        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_CreateUser", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

    
        cmd.Parameters.AddWithValue("@Username", username);
        cmd.Parameters.AddWithValue("@Password", password);
        cmd.Parameters.AddWithValue("@Email", email);

        conn.Open();

 
        object result = cmd.ExecuteScalar();

        if (result != null)
        {
            int newUserId = Convert.ToInt32(result);

            if (newUserId == -1)
            {
                Console.WriteLine("Error: Username already exists!");
            }
            else if (newUserId == -2)
            {
                Console.WriteLine("Error: Email already exists!");
            }
            else if (newUserId > 0)
            {
                Console.WriteLine("User created successfully!");
                Console.WriteLine($"Your User ID is: {newUserId}");
            }
            else
            {
                Console.WriteLine("Error: Failed to create user.");
            }
        }
        else
        {
            Console.WriteLine("Error: Failed to create user.");
        }
    }


    //public void createUser()
    //{
    //    Console.Write("Username: ");
    //    string username = Console.ReadLine();

    //    Console.Write("Password: ");
    //    string password = Console.ReadLine();

    //    Console.Write("Email: ");
    //    string email = Console.ReadLine();

    //    using SqlConnection conn = new SqlConnection(Db.conn);
    //    using SqlCommand cmd = new SqlCommand("sp_CreateUser", conn)
    //    {
    //        CommandType = CommandType.StoredProcedure
    //    };

    //    cmd.Parameters.AddWithValue("@Userame", username);
    //    cmd.Parameters.AddWithValue ("@Password", password);
    //    cmd.Parameters.AddWithValue("@Email",email);

    //    conn.Open();
    //    int result = Convert.ToInt32(cmd.ExecuteScalar());
    //    Console.WriteLine(result);
    //}
    public void viewall()
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_ViewAllUsers", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        conn.Open();
        using SqlDataReader reader = cmd.ExecuteReader();

        Console.WriteLine("\n--- All Users ---");
        Console.WriteLine("ID\tUsername\tRole");
        Console.WriteLine("--------------------------------");

        bool hasUsers = false;
        while (reader.Read())
        {
            hasUsers = true;
            string roleText = Convert.ToInt32(reader["Role"]) == 1 ? "Admin" : "User";
            Console.WriteLine($"{reader["Id"]}\t{reader["Username"]}\t{roleText}");
        }

        if (!hasUsers)
        {
            Console.WriteLine("No users found.");
        }
    }

    public void RemoveUser(int userId)
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_RemoveUser", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

 
        cmd.Parameters.AddWithValue("@UserId", userId);

        conn.Open();


        object result = cmd.ExecuteScalar();
        int rowsAffected = result != null ? Convert.ToInt32(result) : 0;

        if (rowsAffected > 0)
        {
            Console.WriteLine($"✓ User with ID {userId} has been removed successfully.");
        }
        else
        {
            Console.WriteLine($"✗ Error: User with ID {userId} not found.");
        }
    }

    public int UserLogin()
    {
        Console.Write("ID: ");
        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid ID format.");
            return -1;
        }

        Console.Write("Password: ");
        string password = Console.ReadLine();

        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_UserLogin", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

 
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@Password", password);

        conn.Open();

        // Get the role
        object role = cmd.ExecuteScalar();

        if (role == null)
        {
            Console.WriteLine("Login failed: Invalid ID or password.");
            return -1;
        }

        Console.WriteLine("Login successful!");

      
        return (int)role == 1 ? 999 : userId;
    }
}