//using System;
//using System.Collections.Generic;
//using System.Text;

//using System;
//using Microsoft.Data.SqlClient;

//internal class RequestServices
//{
//    //public void addrequest(int uid)
//    //{
//    //    using SqlConnection c = new SqlConnection(Db.conn);
//    //    SqlCommand cmd = new SqlCommand(
//    //        "INSERT INTO Requests(UserId,Message,RequestType) VALUES(@u,'Upgrade',1)", c);
//    //    cmd.Parameters.AddWithValue("@u", uid);
//    //    c.Open();
//    //    cmd.ExecuteNonQuery();
//    //}

//    public void addrequest(int userId)
//    {
//        using SqlConnection conn = new SqlConnection(Db.conn);

//        string query = @"INSERT INTO Requests (UserId, Message, RequestType) 
//                     VALUES (@uid, 'Upgrade to Premium', 0)";

//        SqlCommand cmd = new SqlCommand(query, conn);
//        cmd.Parameters.AddWithValue("@uid", userId);

//        conn.Open();
//        cmd.ExecuteNonQuery();

//        Console.WriteLine("Subscription request sent successfully (Pending).");
//    }


//    public void viewrequest()
//    {
//        using SqlConnection c = new SqlConnection(Db.conn);
//        SqlCommand cmd = new SqlCommand("SELECT * FROM Requests", c);
//        c.Open();
//        SqlDataReader r = cmd.ExecuteReader();
//        while (r.Read())
//            Console.WriteLine($"{r["RequestId"]} {r["UserId"]} {r["RequestType"]}");
//    }

//    //public void viewrequest(int uid)
//    //{
//    //    using SqlConnection c = new SqlConnection(Db.conn);
//    //    SqlCommand cmd = new SqlCommand(
//    //        "SELECT * FROM Requests WHERE UserId=@u", c);
//    //    cmd.Parameters.AddWithValue("@u", uid);
//    //    c.Open();
//    //    SqlDataReader r = cmd.ExecuteReader();
//    //    while (r.Read())
//    //        Console.WriteLine($"{r["RequestId"]} {r["RequestType"]}");
//    //}

//    public void viewrequest(int uid)
//    {
//        using SqlConnection c = new SqlConnection(Db.conn);
//        SqlCommand cmd = new SqlCommand(
//            "SELECT RequestId, RequestType FROM Requests WHERE UserId=@u", c);

//        cmd.Parameters.AddWithValue("@u", uid);

//        c.Open();
//        SqlDataReader r = cmd.ExecuteReader();

//        while (r.Read())
//        {
//            string statusText = "";

//            int status = Convert.ToInt32(r["RequestType"]);

//            if (status == 0)
//                statusText = "Pending";
//            else if (status == 1)
//                statusText = "Success";
//            else
//                statusText = "Unknown";

//            Console.WriteLine($"REQUEST ID : {r["RequestId"]} \t REUEST STATUS : {statusText}");
//        }
//    }


//    public void ViewPendingRequests()
//    {
//        using SqlConnection conn = new SqlConnection(Db.conn);

//        string query = "SELECT * FROM Requests WHERE RequestType = 0";

//        SqlCommand cmd = new SqlCommand(query, conn);

//        conn.Open();
//        SqlDataReader reader = cmd.ExecuteReader();

//        while (reader.Read())
//        {
//            Console.WriteLine(
//                $"RequestID: {reader["RequestId"]} | " +
//                $"UserID: {reader["UserId"]} | " +
//                $"Status: Pending");
//        }
//    }


//    public void ApproveRequest(int requestId)
//    {
//        using SqlConnection conn = new SqlConnection(Db.conn);
//        conn.Open();

//        SqlTransaction transaction = conn.BeginTransaction();

//        try
//        {
//            // Step 1: Get UserId from Request
//            SqlCommand getUserCmd = new SqlCommand(
//                "SELECT UserId FROM Requests WHERE RequestId=@rid",
//                conn, transaction);

//            getUserCmd.Parameters.AddWithValue("@rid", requestId);

//            object result = getUserCmd.ExecuteScalar();

//            if (result == null)
//            {
//                Console.WriteLine("Invalid Request ID.");
//                transaction.Rollback();
//                return;
//            }

//            int userId = (int)result;

//            // Step 2: Update Request Status → Approved
//            SqlCommand updateRequest = new SqlCommand(
//                "UPDATE Requests SET RequestType=1 WHERE RequestId=@rid",
//                conn, transaction);

//            updateRequest.Parameters.AddWithValue("@rid", requestId);
//            updateRequest.ExecuteNonQuery();

//            // Step 3: Update User Subscription → Premium
//            SqlCommand updateUser = new SqlCommand(
//                "UPDATE Users SET Subscription=1 WHERE Id=@uid",
//                conn, transaction);

//            updateUser.Parameters.AddWithValue("@uid", userId);
//            updateUser.ExecuteNonQuery();

//            transaction.Commit();

//            Console.WriteLine("Request Approved! User upgraded to Premium.");
//        }
//        catch (Exception ex)
//        {
//            transaction.Rollback();
//            Console.WriteLine("Error: " + ex.Message);
//        }
//    }



//    public void RejectRequest(int requestId)
//    {
//        using SqlConnection conn = new SqlConnection(Db.conn);

//        SqlCommand cmd = new SqlCommand(
//            "UPDATE Requests SET RequestType=2 WHERE RequestId=@rid", conn);

//        cmd.Parameters.AddWithValue("@rid", requestId);

//        conn.Open();
//        cmd.ExecuteNonQuery();

//        Console.WriteLine("Request rejected.");
//    }



//}


using System;
using Microsoft.Data.SqlClient;
using System.Data;

internal class RequestServices
{
    public void addrequest(int userId)
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_AddRequest", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@UserId", userId);

        conn.Open();

        // Get the new request ID
        object result = cmd.ExecuteScalar();

        if (result != null)
        {
            int newRequestId = Convert.ToInt32(result);
            Console.WriteLine("Subscription request sent successfully (Pending).");
            Console.WriteLine($"Request ID: {newRequestId}");
        }
        else
        {
            Console.WriteLine("Failed to create request.");
        }
    }

    public void ViewAllRequests()
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_ViewAllRequests", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        conn.Open();
        using SqlDataReader reader = cmd.ExecuteReader();

        Console.WriteLine("\n--- All Requests ---");
        Console.WriteLine("RequestID\tUserID\tRequestType\tMessage");
        Console.WriteLine("--------------------------------------------------------");

        while (reader.Read())
        {
            string statusText = GetStatusText(Convert.ToInt32(reader["RequestType"]));
            Console.WriteLine($"{reader["RequestId"]}\t\t{reader["UserId"]}\t{statusText}\t\t{reader["Message"]}");
        }
    }

    public void viewrequest(int userId)
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_ViewUserRequests", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@UserId", userId);

        conn.Open();
        using SqlDataReader reader = cmd.ExecuteReader();

        Console.WriteLine("\n--- Your Requests ---");
        Console.WriteLine("REQUEST ID\tREQUEST STATUS");
        Console.WriteLine("--------------------------------");

        bool hasRequests = false;
        while (reader.Read())
        {
            hasRequests = true;
            string statusText = GetStatusText(Convert.ToInt32(reader["RequestType"]));
            Console.WriteLine($"{reader["RequestId"]}\t\t{statusText}");
        }

        if (!hasRequests)
        {
            Console.WriteLine("No requests found.");
        }
    }

    public void ViewPendingRequests()
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_ViewPendingRequests", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        conn.Open();
        using SqlDataReader reader = cmd.ExecuteReader();

        Console.WriteLine("\n--- Pending Requests ---");
        Console.WriteLine("RequestID\tUserID\tMessage\t\t\tStatus");
        Console.WriteLine("------------------------------------------------------------");

        bool hasPendingRequests = false;
        while (reader.Read())
        {
            hasPendingRequests = true;
            Console.WriteLine(
                $"{reader["RequestId"]}\t\t{reader["UserId"]}\t{reader["Message"]}\t\tPending");
        }

        if (!hasPendingRequests)
        {
            Console.WriteLine("No pending requests.");
        }
    }

    public void ApproveRequest(int requestId)
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_ApproveRequest", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        // Input parameter
        cmd.Parameters.AddWithValue("@RequestId", requestId);

        // Output parameters
        SqlParameter successParam = new SqlParameter("@Success", SqlDbType.Bit)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(successParam);

        SqlParameter messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
        {
            Direction = ParameterDirection.Output
        };
        cmd.Parameters.Add(messageParam);

        conn.Open();
        cmd.ExecuteNonQuery();

        // Get output values
        bool success = (bool)successParam.Value;
        string message = messageParam.Value.ToString();

        // Display result
        if (success)
        {
            Console.WriteLine("✓ " + message);
        }
        else
        {
            Console.WriteLine("✗ " + message);
        }
    }

    public void RejectRequest(int requestId)
    {
        using SqlConnection conn = new SqlConnection(Db.conn);
        using SqlCommand cmd = new SqlCommand("sp_RejectRequest", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@RequestId", requestId);

        conn.Open();

        // Get rows affected
        object result = cmd.ExecuteScalar();
        int rowsAffected = result != null ? Convert.ToInt32(result) : 0;

        if (rowsAffected > 0)
        {
            Console.WriteLine("✓ Request rejected successfully.");
        }
        else
        {
            Console.WriteLine("✗ Request not found or already processed.");
        }
    }

    // Helper method to convert RequestType number to text
    private string GetStatusText(int status)
    {
        return status switch
        {
            0 => "Pending",
            1 => "Approved",
            2 => "Rejected",
            _ => "Unknown"
        };
    }
}