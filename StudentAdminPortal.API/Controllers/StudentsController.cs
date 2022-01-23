using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAdminPortal.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository studentRepository;
        private readonly IMapper mapper;
        private readonly IImageRepository imageRepository;
        public StudentsController(IStudentRepository studentRepository, IMapper mapper,
            IImageRepository imageRepository)
        {
            this.studentRepository = studentRepository;
            this.mapper = mapper;
            this.imageRepository = imageRepository;
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

        [HttpGet]
        [Route("{studentId:guid}"), ActionName("GetStudentAsync")]
        public async Task<IActionResult> GetStudentAsync(Guid studentId)
        {
            var student = await studentRepository.GetStudentAsync(studentId);
            if(student == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<Student>(student));

        }
        [HttpPut] 
        [Route("{studentId:guid}")]
        public async Task<IActionResult> UpdateStudentAsync([FromRoute] Guid studentId, [FromBody] UpdateStudentRequest request)
        {
            if (await studentRepository.Exists(studentId))
            {

                var updatedStudent = await studentRepository.UpdateStudentAsync(studentId, mapper.Map<DataModels.Student>(request));
                if (updatedStudent != null)
                {
                    return Ok(mapper.Map<Student>(updatedStudent));
                }
            }
            return NotFound();
        }
        [HttpDelete]
        [Route("{studentId:guid}")]
        public async Task<IActionResult> DeleteStudentAsync([FromRoute]Guid studentId)
        {
            if (await studentRepository.Exists(studentId))
            {
                var deletedStudent = await studentRepository.DeleteStudentAsync(studentId);
                return Ok(mapper.Map<Student>(deletedStudent));
            }
            return NotFound();
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> AddStudentAsync([FromBody] AddStudentRequest request)
        {
            var student = await studentRepository.AddStudentAsync(mapper.Map<DataModels.Student>(request));
            return CreatedAtAction(nameof(GetStudentAsync), new { studentId = student.Id }, mapper.Map<Student>(student));
        }
        [HttpPost]
        [Route("{studentId:guid}/upload-image")]
        public async Task<IActionResult> UploadImage([FromRoute] Guid studentId, IFormFile profileImage)
        {
            if(profileImage != null && profileImage.Length >0)
            {
                var validExtensions = new List<string>
                {
                    ".jpeg",
                    ".png",
                    ".gif",".jpg"
                };
                var extension = Path.GetExtension(profileImage.FileName);
                if (validExtensions.Contains(extension))
                {
                    if (await studentRepository.Exists(studentId))
                    {
                        //upload the image to local storage
                        var filename = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);
                        var fileImagePath = await imageRepository.Upload(profileImage, filename);
                        //update the profile image path in the database
                        if (await studentRepository.UpdateProfileImage(studentId, fileImagePath))
                        {
                            return Ok(fileImagePath);
                        }
                        return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading image");
                    }
                }
                return BadRequest("This is not a valid Image format");                
            }
            
            return NotFound();
        }
       
    }
}
