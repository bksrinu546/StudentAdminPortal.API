using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAdminPortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository studentRepository;
        private readonly IMapper mapper;
        public StudentsController(IStudentRepository studentRepository, IMapper mapper)
        {
            this.studentRepository = studentRepository;
            this.mapper = mapper;
        }

        [HttpGet]        
        public async Task<IActionResult> GetAllStudentsAsync()
        {
           var students= await studentRepository.GetStudentsAsync();
            //var domainModelStudents = new List<Student>();
            //foreach(var student in students)
            //{
            //    domainModelStudents.Add(new Student()
            //    {
            //        Id = student.Id,
            //        FirstName= student.FirstName,
            //        LastName=student.LastName,
            //        DateOfBirth= student.DateOfBirth,
            //        Email= student.Email,
            //        Mobile =student.Mobile,
            //        ProfileImageUrl=student.ProfileImageUrl,
            //        GenderId= student.GenderId,
            //        Address = new Address()
            //        {
            //            Id= student.Address.Id,
            //            PhysicalAddress = student.Address.PhysicalAddress,
            //            PostalAddress= student.Address.PostalAddress
            //        },
            //        Gender= new Gender()
            //        {
            //            Id= student.Gender.Id,
            //            Description = student.Gender.Description
            //        }
            //    });
            //}
            //return Ok(domainModelStudents);

            
            return Ok(mapper.Map<List<Student>>(students));

        }
    }
}
