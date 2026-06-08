using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeKeepingAppService;
using TimeKeepingManagementDataService;
using TimeKeepingModels;

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
            var shiftSchedule = _appService.GetShiftSchedule(id);
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
            var shiftSchedules = new ShiftSchedule
            {
                ShiftID = _appService.GenerateShiftID(),
                ShiftName = shiftSchedule.ShiftName,
                ShiftStartTime = shiftSchedule.ShiftStartTime,
                ShiftEndTime = shiftSchedule.ShiftEndTime
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
            var existingShift = _appService.GetShiftSchedule(shiftID);
            if (existingShift == null)
            {
                return NotFound();
            }
            existingShift.ShiftName = shiftSchedule.ShiftName;
            existingShift.ShiftStartTime = shiftSchedule.ShiftStartTime;
            existingShift.ShiftEndTime = shiftSchedule.ShiftEndTime;
            _appService.UpdateShiftSchedule(existingShift);
            return NoContent();
        }
        [HttpDelete("{shiftID:int}")]
        public IActionResult DeleteShift(int shiftID)
        {
            var existingShift = _appService.GetShiftSchedule(shiftID);
            if (existingShift == null)
            {
                return NotFound();
            }
            _appService.DeleteShiftSchedule(shiftID);
            return NoContent();
        }
    }
}
