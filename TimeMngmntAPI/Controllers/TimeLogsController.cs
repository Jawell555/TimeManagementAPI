using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeKeepingAppService;
using TimeKeepingManagementDataService;
using TimeKeepingModels;
using TimeMngmntAPI.Helpers;
using TimeMngmntAPI.Models;

namespace TimeMngmntAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeLogsController : ControllerBase
    {
        private readonly EmployeeAppService _appService;
        public TimeLogsController()
        {
            _appService = new EmployeeAppService();
        }
        [HttpGet]
        public ActionResult<IEnumerable<TimeLogs>> GetTimeLogs()
        {
            var timeLogs = _appService.GetAllLogs();
            return Ok(timeLogs);
        }
        [HttpGet("{id:int}")]
        public ActionResult<IEnumerable<TimeLogs>> GetTimeLogById(int id)
        {
            var timeLog = _appService.GetEmployeeLogs(id);
            if (timeLog == null)
            {
                return NotFound("No time logs found for the specified employee ID.");
            }
            return Ok(timeLog);
        }
        [HttpGet("{date}")]
        public ActionResult<TimeLogs> GetLatestEmployeeLogByDate(string date)
        {
            if (!TimeValidationServices.TryParseValidDate(date, out DateOnly parsedDate))
            {
                return BadRequest("Invalid date format. Use MM/dd/yyyy (e.g., 12/31/2024).");
            }
            var timeLog = _appService.GetTimeLogsByDate(parsedDate);
            if (timeLog == null)
            {
                return NotFound("No time log found for the specified date.");
            }
            return Ok(timeLog);
        }
        [HttpGet("latest-logs")]
        public ActionResult<IEnumerable<TimeLogs>> GetLatestEmployeeLogs()
        {
            var timeLogs = _appService.GetLatestEmployeeLogs();
            if (timeLogs == null || !timeLogs.Any())
            {
                return NotFound("No time logs found.");
            }
            return Ok(timeLogs);
        }
        [HttpGet("latest-logs/{id:int}")]
        public ActionResult<TimeLogs> GetLatestEmployeeLogByID(int id)
        {
            var timeLog = _appService.GetLatestEmployeeLogByID(id);
            if (timeLog == null)
            {
                return NotFound("No time log found for the specified employee ID.");
            }
            return Ok(timeLog);
        }
        [HttpPost("timein-logs")]
        public IActionResult EmployeeTimeIn([FromBody]Models.TimeLogsViewModel TimeLogs)
        {
            if (TimeLogs == null)
            {
                return BadRequest("Employee data is required.");
            }
            DateTime timeInTime = DateTime.Now;
            DateOnly dateToday;
            if (_appService.AlreadyTimedIn(TimeLogs.EmployeeID))
            {
                return Conflict("You already timed in.");
            }
            Employee employee = _appService.GetEmployee(TimeLogs.EmployeeID);
            ShiftSchedule shift = _appService.GetEmployeeSchedule(employee.ShiftID);
            dateToday = DateOnly.FromDateTime(timeInTime);
            DateTime start = dateToday.ToDateTime(shift.ShiftStartTime);
            TimeSpan late = _appService.calcLate(start, timeInTime);

            TimeLogs newLog = new TimeKeepingModels.TimeLogs { EmployeeID = TimeLogs.EmployeeID, ShiftName = shift.ShiftName, Date = DateOnly.FromDateTime(timeInTime), TimeIn = timeInTime, LateHours = late };

            _appService.AddTimeLog(newLog);
            return CreatedAtAction(nameof(GetLatestEmployeeLogByID), new { id = TimeLogs.EmployeeID }, newLog);
        }
        [HttpPost("timeout-logs")]
        public IActionResult EmployeeTimeOut([FromBody] Models.TimeLogsViewModel TimeLogs)
        {
            if(TimeLogs == null)
            {
                return BadRequest("Employee data is required.");
            }
            DateTime timeOutTime = DateTime.Now;
            DateOnly dateToday;
            TimeLogs log = _appService.GetLastTimeIn(TimeLogs.EmployeeID);
            if (log == null)
            {
                return Conflict("You must time in first.");
            }

            Employee employee = _appService.GetEmployee(TimeLogs.EmployeeID);
            ShiftSchedule shift = _appService.GetEmployeeSchedule(employee.ShiftID);
            dateToday = DateOnly.FromDateTime(DateTime.Now);
            DateTime end = dateToday.ToDateTime(shift.ShiftEndTime);
            log.TimeOut = timeOutTime;
            log.WorkingHours = _appService.calcWorkingHours(log.TimeIn, timeOutTime);
            TimeSpan constant = shift.ShiftEndTime - shift.ShiftStartTime;

            if (log.WorkingHours > constant)
            {
                log.OvertimeHours = _appService.calcOvertime(constant, log.WorkingHours);
            }
            else if (log.WorkingHours < constant)
            {
                log.UndertimeHours = _appService.calcUndertime(constant, log.WorkingHours);
            }
            _appService.UpdateLog(log);
            return CreatedAtAction(nameof(GetLatestEmployeeLogByID), new { id = TimeLogs.EmployeeID }, log);
        }
    }
    
}
