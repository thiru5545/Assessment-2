//using System;
//using System.Collections.Generic;
//using System.Text;

//using System;
//using Microsoft.Data.SqlClient;

//internal class VideoServices
//{
//    public void videolist()
//    {
//        using SqlConnection c = new SqlConnection(Db.conn);
//        SqlCommand cmd = new SqlCommand("SELECT * FROM Videos", c);
//        c.Open();
//        SqlDataReader r = cmd.ExecuteReader();
//        while (r.Read())
//            Console.WriteLine($"{r["VideoId"]} {r["VideoName"]}");
//    }

//    public void videolist(int userId)
//    {
//        using SqlConnection c = new SqlConnection(Db.conn);
//        c.Open();

//        // Step 1: Get User Subscription
//        SqlCommand getSub = new SqlCommand(
//            "SELECT Subscription FROM Users WHERE Id=@uid", c);

//        getSub.Parameters.AddWithValue("@uid", userId);

//        object result = getSub.ExecuteScalar();

//        if (result == null)
//        {
//            Console.WriteLine("User not found.");
//            return;
//        }

//        int userSub = (int)result;

//        // Step 2: Fetch Videos Based on Subscription
//        string query;

//        if (userSub == 0)
//        {
//            // Free user → only free videos
//            query = "SELECT * FROM Videos WHERE Subscription = 0";
//        }
//        else
//        {
//            // Premium user → all videos
//            query = "SELECT * FROM Videos";
//        }

//        SqlCommand cmd = new SqlCommand(query, c);
//        SqlDataReader r = cmd.ExecuteReader();

//        Console.WriteLine("\n--- AVAILABLE VIDEOS ---");

//        while (r.Read())
//        {
//            Console.WriteLine(
//                $"ID: {r["VideoId"]} | " +
//                $"Title: {r["VideoName"]} | " +
//                $"Type: {((int)r["Subscription"] == 0 ? "Free" : "Premium")}");
//        }
//    }

//    public void AddVideo(int id)
//    {
//        Console.Write("Title: ");
//        string t = Console.ReadLine();
//        Console.Write("URL: ");
//        string u = Console.ReadLine();
//        Console.WriteLine("Enter the subscriptions type [ 1-premium | 0-basic ]");
//        int sub=int.Parse(Console.ReadLine());
//        using SqlConnection c = new SqlConnection(Db.conn);
//        SqlCommand cmd = new SqlCommand(
//            "INSERT INTO Videos VALUES (@t,@u,@sub)", c);
//        //cmd.Parameters.AddWithValue("@i", id);
//        cmd.Parameters.AddWithValue("@t", t);
//        cmd.Parameters.AddWithValue("@u", u);
//        cmd.Parameters.AddWithValue("@sub", sub);
//        c.Open();
//        cmd.ExecuteNonQuery();
//    }

//    public void removevideo(int id)
//    {
//        using SqlConnection c = new SqlConnection(Db.conn);
//        SqlCommand cmd = new SqlCommand(
//            "DELETE FROM Videos WHERE VideoId=@id", c);
//        cmd.Parameters.AddWithValue("@id", id);
//        c.Open();
//        cmd.ExecuteNonQuery();
//    }
//}

using System;
using Microsoft.Data.SqlClient;
using System.Data;

internal class VideoServices
{
    // View all videos (Admin view)
    public void videolist()
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_VideoList", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        conn.Open();
        using SqlDataReader reader = cmd.ExecuteReader();

        Console.WriteLine("\n--- ALL VIDEOS ---");
        Console.WriteLine("ID\tVideo Name");
        Console.WriteLine("--------------------------------");

        bool hasVideos = false;
        while (reader.Read())
        {
            hasVideos = true;
            Console.WriteLine($"{reader["VideoId"]}\t{reader["VideoName"]}");
        }

        if (!hasVideos)
        {
            Console.WriteLine("No videos found.");
        }
    }

    // View videos based on user subscription
    public void videolist(int userId)
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_VideoListByUser", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@UserId", userId);

        conn.Open();
        using SqlDataReader reader = cmd.ExecuteReader();

        // Check if first row is an error
        if (reader.Read())
        {
            // Check if it's an error response
            if (reader.FieldCount > 0 && reader.GetName(0) == "ErrorCode")
            {
                Console.WriteLine($"✗ {reader["ErrorMessage"]}");
                return;
            }

            // Display header
            Console.WriteLine("\n--- AVAILABLE VIDEOS ---");
            Console.WriteLine("ID\tTitle\t\t\tType");
            Console.WriteLine("--------------------------------------------------------");

            // Display first row
            string type = Convert.ToInt32(reader["Subscription"]) == 0 ? "Free" : "Premium";
            Console.WriteLine($"{reader["VideoId"]}\t{reader["VideoName"],-20}\t{type}");

            // Display remaining rows
            while (reader.Read())
            {
                type = Convert.ToInt32(reader["Subscription"]) == 0 ? "Free" : "Premium";
                Console.WriteLine($"{reader["VideoId"]}\t{reader["VideoName"],-20}\t{type}");
            }
        }
        else
        {
            Console.WriteLine("No videos available for your subscription level.");
        }
    }

    // Add new video (Admin only)
    public void AddVideo(int adminId)
    {
        Console.Write("Title: ");
        string title = Console.ReadLine();

        Console.Write("URL: ");
        string url = Console.ReadLine();

        Console.WriteLine("Enter the subscription type [1-premium | 0-basic]: ");
        if (!int.TryParse(Console.ReadLine(), out int subscription) || (subscription != 0 && subscription != 1))
        {
            Console.WriteLine("✗ Invalid subscription type. Please enter 0 or 1.");
            return;
        }

        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_AddVideo", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        // Input parameters
        cmd.Parameters.AddWithValue("@VideoName", title);
        cmd.Parameters.AddWithValue("@VideoUrl", url);
        cmd.Parameters.AddWithValue("@Subscription", subscription);

        conn.Open();

        // Get the new video ID
        object result = cmd.ExecuteScalar();

        if (result != null)
        {
            int newVideoId = Convert.ToInt32(result);

            if (newVideoId == -1)
            {
                Console.WriteLine("✗ Error: Video with this name already exists!");
            }
            else if (newVideoId == -2)
            {
                Console.WriteLine("✗ Error: Invalid subscription value!");
            }
            else if (newVideoId > 0)
            {
                Console.WriteLine("✓ Video added successfully!");
                Console.WriteLine($"Video ID: {newVideoId}");
            }
            else
            {
                Console.WriteLine("✗ Error: Failed to add video.");
            }
        }
        else
        {
            Console.WriteLine("✗ Error: Failed to add video.");
        }
    }

    // Remove video (Admin only)
    public void removevideo(int videoId)
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_RemoveVideo", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        // Input parameter
        cmd.Parameters.AddWithValue("@VideoId", videoId);

        conn.Open();

        // Get rows affected
        object result = cmd.ExecuteScalar();
        int rowsAffected = result != null ? Convert.ToInt32(result) : 0;

        if (rowsAffected > 0)
        {
            Console.WriteLine($"✓ Video with ID {videoId} has been removed successfully.");
        }
        else
        {
            Console.WriteLine($"✗ Error: Video with ID {videoId} not found.");
        }
    }
}