namespace TimeMngmntAPI.Models
{
    public class ShiftSchedulesViewModel
    {
        public string ShiftName { get; set; }
        public TimeOnly ShiftStartTime { get; set; }
        public TimeOnly ShiftEndTime { get; set; }
    }
}
