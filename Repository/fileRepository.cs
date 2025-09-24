using System.Text.Json;
using StudentGradeManager.Domain;
using System.IO;
using System.Collections.Generic;

namespace StudentGradeManager.Repository
{
    public class FileRepository : IFileRepository 
    {
        private readonly string _filePath = "student_data.json";

        public void SaveDataToJson(List<Student> students)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };

                // 沒有把options加到排序中

                var jsonString = JsonSerializer.Serialize(students, options);
                File.WriteAllText(_filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生存檔異常，{ex.Message}");
                return;
            }
        }

        public List<Student> LoadDataFromJson()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Student>();
            }

            try
            {
                var jsonString = File.ReadAllText(_filePath);

                return JsonSerializer.Deserialize<List<Student>>(jsonString) ?? new List<Student>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"發生讀檔異常，{ex.Message}");
                return new List<Student>();
            }
        }
    }
}
