using System;

namespace WebSurge
{
    public class CheckForUpdates
    {
        public int Days { get; set; }
        public DateTime LastUpdateCheck { get; set; }
        

        public CheckForUpdates()
        {
            Days = 10;
            LastUpdateCheck = DateTime.UtcNow.Date;
        }
    }
}