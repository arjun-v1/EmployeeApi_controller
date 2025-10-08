using Microsoft.AspNetCore.Mvc;
using Employee_Common_Lib.Manager;
using Employee_Common_Lib.Models;

namespace EmployeeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeManager _employeeManager;

        public EmployeeController(IEmployeeManager employeeManager)
        {
            _employeeManager = employeeManager;
        }

        

        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            try
            {
                var employees = _employeeManager.GetAllEmployees();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
       
        public IActionResult GetEmployeeById(int id)
        {
            var employee = _employeeManager.GetEmployeeById(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpGet("with-company")]
        public IActionResult GetAllEmployeesWithCompany()
        {
            var employees = _employeeManager.GetAllEmployeesWithCompany();
            return Ok(employees);
        }

        [HttpPost]
        public IActionResult CreateEmployee(Employee employee)
        {
            var created = _employeeManager.Create(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (employee == null)
                return BadRequest("Employee data is required.");

            if (id != employee.Id)
                return BadRequest("Employee ID in URL and body must match.");

            var success = _employeeManager.Update(id, employee);
            if (!success)
                return NotFound("Employee not found or update failed.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var success = _employeeManager.Delete(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
