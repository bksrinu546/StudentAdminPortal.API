﻿using StudentAdminPortal.API.DataModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentAdminPortal.API.Repositories
{
    public interface IStudentRepository
    {
       Task<List<Student>> GetStudentsAsync();

        Task<Student> GetStudentAsync(Guid  studentId);

        Task<List<Gender>> GetGendersAsync();

        Task<bool> Exists(Guid studentId);

        Task<Student> UpdateStudentAsync(Guid studentId, Student student);
       
    }
}
