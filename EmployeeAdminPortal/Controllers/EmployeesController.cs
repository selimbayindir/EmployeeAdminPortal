using EmployeeAdminPortal.Data;
using EmployeeAdminPortal.Models;
using EmployeeAdminPortal.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeAdminPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public EmployeesController(ApplicationDbContext dbContext) { 
            this.dbContext=dbContext;
        }
        [HttpGet]
        public IActionResult GetAllEmployees()
        {
           var allEmployees=dbContext.Employees.ToList();

            return Ok(allEmployees);
        }
        [HttpPost(Name = "AddEmployee")]
        public IActionResult AddEmployee(AddEmployeeDto newEmployee)
        {
            try
            {
                var employeeEntity = new Employee()
                {
                    Name = newEmployee.Name,
                    Email = newEmployee.Email,
                    Phone = newEmployee.Phone,
                    Salary = newEmployee.Salary
                };

                dbContext.Employees.Add(employeeEntity);
                dbContext.SaveChanges();

                return Ok(employeeEntity);
            }
            catch (Exception ex)
            {
                // Burada hata yönetimi yapılabilir, loglama veya hata mesajı döndürülebilir.
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving the employee.");
            }

        }
        [HttpPost("add", Name = "AddEmployee2")]
        public IActionResult AddEmployee2(AddEmployeeDto newEmployee)
        {
            // Veritabanında aynı Name, Email, Phone ve Salary değerlerine sahip bir çalışan olup olmadığını kontrol edin
            var existingEmployee = dbContext.Employees
                .FirstOrDefault(e => e.Name == newEmployee.Name &&
                                     e.Email == newEmployee.Email &&
                                     e.Phone == newEmployee.Phone &&
                                     e.Salary == newEmployee.Salary);

            if (existingEmployee != null)
            {
                // Eğer mevcutsa, bir hata mesajı döndür
                return Conflict("A user with the same Name, Email, Phone, and Salary already exists.");
            }

            // Eğer kayıt yoksa, yeni çalışanı ekle
            var employeeEntity = new Employee()
            {
                Name = newEmployee.Name,
                Email = newEmployee.Email,
                Phone = newEmployee.Phone,
                Salary = newEmployee.Salary
            };

            dbContext.Employees.Add(employeeEntity);
            dbContext.SaveChanges();

            return Ok(employeeEntity);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetEmployeesById(Guid id)
        {
            var employee = dbContext.Employees.Find(id);
            if (employee is null)
            {
                return NotFound();
            }

            return Ok(employee);
        }
        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateEmployee(Guid id,UpdateEmployeeDto updateEmployeeDto)
        {
            var employee = dbContext.Employees.Find(id);
            if (employee is null)
            {
                return NotFound();
            }

            employee.Name=updateEmployeeDto.Name;
            employee.Email=updateEmployeeDto.Email; 
            employee.Phone=updateEmployeeDto.Phone;
            employee.Salary=updateEmployeeDto.Salary;

            dbContext.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteEmployee(Guid id)
        {
            var employee = dbContext.Employees.Find(id);
            if (employee is null)
            {
                return NotFound();
            }
            dbContext.Employees.Remove(employee);
            dbContext.SaveChanges();
            return Ok();
        }


    }
}
