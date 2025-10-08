using Employee_Common_Lib.Manager;
using Employee_Common_Lib.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeApi.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class CompanyController : ControllerBase
        {
            private readonly ICompanyManger _companyManager;

            public CompanyController(ICompanyManger companyManager)
            {
                _companyManager = companyManager;
            }

            [HttpGet]
            public IActionResult GetAll()
            {
                return Ok(_companyManager.GetAll());
            }

            [HttpGet("{id}")]
            public IActionResult GetById(int id)
            {
                var company = _companyManager.GetById(id);
                if (company == null)
                    return NotFound();

                return Ok(company);
            }

            [HttpGet("with-employees/{id}")]
            public IActionResult GetWithEmployees(int id)
            {
                var company = _companyManager.GetCompanyWithEmployees(id);
                if (company == null)
                    return NotFound();

                return Ok(company);
            }

            [HttpPost]
            public IActionResult Create(Company company)
            {
                var created = _companyManager.Create(company);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }

            [HttpPut("{id}")]
            public IActionResult Update(int id, Company company)
            {
                var success = _companyManager.Update(id, company);
                if (!success)
                    return BadRequest();

                return NoContent();
            }

            [HttpDelete("{id}")]
            public IActionResult Delete(int id)
            {
                var success = _companyManager.Delete(id);
                if (!success)
                    return NotFound();

                return NoContent();
            }
        }
}
