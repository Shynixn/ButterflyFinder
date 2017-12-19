namespace OverLayApplicationSearch.Contract.Persistence.Enumeration
{
    [System.Obsolete]
    public class TimeSchedule
    {
        public const string HOURS_12 = "12 Hours";
        public const string DAILY = "Daily";
        public const string WEEKLY = "Weekly";
        public const string MONTHLY = "Monthly";
        public const string NEVER = "Never";

        public static string[] TimeSchedules => new string[] {HOURS_12, DAILY, WEEKLY, MONTHLY, NEVER};
    }
}