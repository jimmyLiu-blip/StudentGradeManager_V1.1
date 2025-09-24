using StudentGradeManager.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentGradeManager.Repository
{
    public interface IFileRepository
    {
        void SaveDataToJson(List<Student> students);
        List<Student> LoadDataFromJson();
    }
}
