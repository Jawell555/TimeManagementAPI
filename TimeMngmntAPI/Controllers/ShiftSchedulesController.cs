using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeKeepingAppService;
using TimeKeepingManagementDataService;
using TimeKeepingModels;
using TimeMngmntAPI.Helpers;
using TimeMngmntAPI.Models;

namespace TimeMngmntAPI.Controllers
{
    [Route("api/shift-schedules")]
    [ApiController]
    public class ShiftSchedulesController : ControllerBase
    {
        private readonly EmployeeAppService _appService;
        public ShiftSchedulesController()
        {
            _appService = new EmployeeAppService();
        }

        [HttpGet]
        public ActionResult<IEnumerable<ShiftSchedule>> GetShifts()
        {
            var shiftSchedule = _appService.GetAllShiftSchedules();
            return Ok(shiftSchedule);
        }
        [HttpGet("{id:int}")]
        public ActionResult<IEnumerable<ShiftSchedule>> GetShiftById(int id)
        {
            var shiftSchedule = _appService.GetEmployeeSchedule(id);
            if (shiftSchedule == null)
            {
                return NotFound();
            }
            return Ok(shiftSchedule);
        }
        [HttpPost]
        public IActionResult CreateShift([FromBody] Models.ShiftSchedulesViewModel shiftSchedule)
        {
            if (shiftSchedule == null)
            {
                return BadRequest("Shift schedule data is required.");
            }
            if(!TimeValidationServices.TimeValidationService.TryParseValidTime(shiftSchedule.ShiftStartTime, out TimeOnly startTime))
            {
                return BadRequest("Invalid time format. Use hh:mm AM/PM or HH:mm (e.g., 09:30 AM, 09:30 PM, or 21:30).");
            }
            if (!TimeValidationServices.TimeValidationService.TryParseValidTime(shiftSchedule.ShiftEndTime, out TimeOnly endTime))
            {
                return BadRequest("Invalid time format. Use hh:mm AM/PM or HH:mm (e.g., 09:30 AM, 09:30 PM, or 21:30).");
            }
            var shiftSchedules = new ShiftSchedule
            {
                ShiftID = _appService.GenerateShiftID(),
                ShiftName = shiftSchedule.ShiftName,
                ShiftStartTime = startTime,
                ShiftEndTime = endTime
            };
            _appService.AddShiftSchedule(shiftSchedules);
            return CreatedAtAction(nameof(GetShiftById), new { id = shiftSchedules.ShiftID }, shiftSchedules);
        }
        [HttpPatch("{shiftID:int}")]
        public IActionResult UpdateShift(int shiftID, [FromBody] Models.ShiftSchedulesViewModel shiftSchedule)
        {
            if (shiftSchedule == null)
            {
                return BadRequest("Shift schedule data is required.");
            }
            if (!TimeValidationServices.TimeValidationService.TryParseValidTime(shiftSchedule.ShiftStartTime, out TimeOnly startTime))
            {
                return BadRequest("Invalid time format. Use hh:mm AM/PM or HH:mm (e.g., 09:30 AM, 09:30 PM, or 21:30).");
            }
            if (!TimeValidationServices.TimeValidationService.TryParseValidTime(shiftSchedule.ShiftEndTime, out TimeOnly endTime))
            {
                return BadRequest("Invalid time format. Use hh:mm AM/PM or HH:mm (e.g., 09:30 AM, 09:30 PM, or 21:30).");
            }
            var existingShift = _appService.GetEmployeeSchedule(shiftID);
            if (existingShift == null)
            {
                return NotFound();
            }
            existingShift.ShiftName = shiftSchedule.ShiftName;
            existingShift.ShiftStartTime = startTime;
            existingShift.ShiftEndTime = endTime;
            _appService.UpdateShiftSchedule(existingShift);
            return NoContent();
        }
        [HttpDelete("{shiftID:int}")]
        public IActionResult DeleteShift(int shiftID)
        {
            var existingShift = _appService.GetEmployeeSchedule(shiftID);
            if (existingShift == null)
            {
                return NotFound();
            }
            _appService.DeleteShiftSchedule(shiftID);
            return NoContent();
        }
        
    }
}
