using EpicMock.Data;
using EpicMock.DTOs;
using EpicMock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EpicMock.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly HimDbContext context;

        public PatientsController(HimDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            var patients = await context.Patients.Where(p => p.IsActive).ToListAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await context.Patients.FindAsync(id);
            if (patient == null || !patient.IsActive)
            {
                return NotFound($"Patient with ID {id} not found.");
            }
            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient(PatientIntakeDTO dto)
        {
            var mRnExists = await context.Patients.AnyAsync(p => p.Mrn == dto.Mrn);
            if(mRnExists)
            {
                return Conflict($"A patient with MRN {dto.Mrn} already exists.");
            }

            var createdPatient = new Patient
            {
                Mrn = dto.Mrn,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DateOfBirth = dto.DateOfBirth,
                Sex = dto.Sex,
                Phone = dto.Phone,
                Email = dto.Email,
                AddressLine1 = dto.AddressLine1,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode,
                InsuranceProvider = dto.InsuranceProvider,
                InsurancePolicyNo = dto.InsurancePolicyNo,
                IsActive = true
            };

            context.Patients.Add(createdPatient);
            await context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPatientById), new { id = createdPatient.PatientId }, createdPatient);
        }
    }
}
