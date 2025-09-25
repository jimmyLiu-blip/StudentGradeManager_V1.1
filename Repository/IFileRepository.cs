using StudentGradeManager.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentGradeManager.Repository // interface的方法預設就是public
{
    public interface IFileRepository
    {
        public void SaveDataToJson(List<Student> students);
        public List<Student> LoadDataFromJson();
    }
}
