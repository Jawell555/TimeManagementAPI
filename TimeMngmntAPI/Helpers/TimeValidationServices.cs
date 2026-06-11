namespace TimeMngmntAPI.Helpers
{
    public class TimeValidationServices
    {
        public class TimeValidationService
        {
            public static bool TryParseValidTime(string input, out TimeOnly result)
            {
                string[] formats = {
            "hh\\:mm tt",      // 09:30 AM
            "h\\:mm tt",       // 9:30 AM
            "HH\\:mm",         // 09:30 (24-hour)
            "H\\:mm",          // 9:30 (24-hour)
            "hh\\:mm\\:ss tt", // 09:30:45 AM
            "h\\:mm\\:ss tt",  // 9:30:45 AM
            "HH\\:mm\\:ss",    // 09:30:45 (24-hour)
            "H\\:mm\\:ss"      // 9:30:45 (24-hour)
        };

                return TimeOnly.TryParseExact(input, formats, null,
                    System.Globalization.DateTimeStyles.None, out result);
            }
        }
    }
}
