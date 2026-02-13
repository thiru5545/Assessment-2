using System;
using System.Collections.Generic;
using System.Text;

internal class AdminMenu
{
    public void adminmenu(UserServices ott, VideoServices ott1, RequestServices req)
    {
        int vid = 100;
        bool loop = true;

        while (loop)
        {
            Console.WriteLine("\n--- ADMIN MENU ---");
            Console.WriteLine("1. SHOW ALL VIDEOS");
            Console.WriteLine("2. VIEW ALL USERS");
            Console.WriteLine("3. ADD VIDEO");
            Console.WriteLine("4. REMOVE VIDEO");
            Console.WriteLine("5. VIEW PENDING REQUESTS");
            Console.WriteLine("6. APPROVE REQUEST");
            Console.WriteLine("7. REJECT REQUEST");
            Console.WriteLine("8. VIEW ALL REQUESTS");
            Console.WriteLine("9. LOGOUT");


            Console.Write("Select Option: ");
            int o = int.Parse(Console.ReadLine());

            switch (o)
            {
                case 1:
                    ott1.videolist();
                    break;

                case 2:
                    ott.viewall();
                    break;

                case 3:
                    ott1.AddVideo(++vid);
                    break;

                case 4:
                    Console.Write("Enter Video ID to remove: ");
                    int removeId = int.Parse(Console.ReadLine());
                    ott1.removevideo(removeId);
                    break;

                case 5:
                    req.ViewPendingRequests();
                    break;

                case 6:
                    Console.Write("Enter Request ID to Approve: ");
                    int approveId = int.Parse(Console.ReadLine());
                    req.ApproveRequest(approveId);
                    break;

                case 7:
                    Console.Write("Enter Request ID to Reject: ");
                    int rejectId = int.Parse(Console.ReadLine());
                    req.RejectRequest(rejectId);
                    break;

                case 8:
                    Console.WriteLine("--- All requests ---");
                    req.ViewAllRequests();
                    break;
                case 9:
                    Console.WriteLine("Logging out...");
                    loop = false;
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}
