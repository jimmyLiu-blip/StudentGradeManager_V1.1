using StudentGradeManager.Domain;
using StudentGradeManager.Repository;
using System.Threading.Channels;
using static System.Formats.Asn1.AsnWriter;

namespace StudentGradeManager.Service
{
    public class StudentService
    {
        private readonly IFileRepository _fileRepository; // 將依賴的型別從 FileRepository 改為 IFileRepository
        private List<Student> _students;

        public StudentService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _students = _fileRepository.LoadDataFromJson();
        }

        public void AddStudent(string numberId, string name, string className)
        {
            if (_students.Any(s => s.NumberId == numberId))
            {
                throw new InvalidOperationException("學號已存在學生列表中");
            }

            // 忘記如何新增學生
            _students.Add(new Student(numberId, name, className));

            _fileRepository.SaveDataToJson(_students);
        }

        public void UpdateStudent(string numberId, string newName, string newClassName)
        {
            // 忘記先宣告一個變數student
            var student = GetStudnetNameByNumberId(numberId);
            if (student == null)
            {
                throw new InvalidOperationException("學號不存在，無法更新學生資訊");
            }

            student.Name = newName;
            student.ClassName = newClassName;

            _fileRepository.SaveDataToJson(_students);
        }

        public List<Student> GetAllStudents()
        {
            if (_students == null)
            { 
                return new List<Student>();
            }
            return _students;
        }

        public void AddStudentGrade(string numberId, string subject, double score)
        {
            var student = GetStudnetNameByNumberId(numberId);
            if (student == null)
            {
                throw new InvalidOperationException("學號不存在，無法新增該學生的科目成績");
            }

            student.Grades.Add(new Grade(subject, score));

            _fileRepository.SaveDataToJson(_students);
        }

        public void UpdateStudentGrade(string numberId, string oldSubject, string newSubject, double newScore)
        {
            var student = GetStudnetNameByNumberId(numberId);
            if (student == null)
            {
                throw new InvalidOperationException("此學生的學號不存在，無法更新他的科目和成績");
            }

            // 忘記如何寫判斷指定科目是否存在
            var grade = student.Grades.FirstOrDefault(g => g.Subject == oldSubject);
            if (grade == null)
            {
                throw new InvalidOperationException("指定的科目不存在該學生的科目中");
            }
            // 感覺有漏掉甚麼沒有寫到
            grade.Subject = newSubject;
            grade.Score = newScore;

            _fileRepository.SaveDataToJson(_students);

        }

        public List<Grade> GetStudentGrades(string numberId)
        {
            // 忘記如何先判斷學生是否存在學生列表的程式表達
            // 忘記如何用空條件運算子，空聯合運算子??
            var student = GetStudnetNameByNumberId(numberId);

            return student?.Grades ?? new List<Grade>();
        }

        public (double average, double highest, double lowest) GetStudentStatistics(string numberId)
        {
            var student = GetStudnetNameByNumberId(numberId);
            if (student == null || !student.Grades.Any())
            {
                return (0, 0, 0);
            }

            double average = student.Grades.Average(g => g.Score);
            double highest = student.Grades.Max(g => g.Score);
            double lowest = student.Grades.Min(g => g.Score);

            return (average, highest, lowest);
        }

        public List<Student> GetTop3Students()
        {
            return _students
                // 忘記LINQ寫法
                .Where(s => s.Grades.Any())  //篩掉沒有成績的學生
                .OrderByDescending(s => s.Grades.Average(g => g.Score)) // 根據平均成績降冪排序
                .Take(3)
                .ToList();
        }

        public List<string> GetAllSubjects()
        {
            /*
            var subjects = new List<string>();

            foreach (var student in _students)
            {
                foreach (var grade in student.Grades)
                {
                    if (!subjects.Contains(grade.Subject))
                    {
                        subjects.Add(grade.Subject);
                    }
                }
            }

            subjects.Sort();
            return subjects;
            */

            // 可以使用Linq重寫
            return _students
                .SelectMany(s => s.Grades)       // 先尋找學生的所有成績
                .Select(g => g.Subject)          // 選取科目名稱
                .Distinct()                      // 去除重複
                .OrderBy(subject => subject)     // 排序
                .ToList();                       // 是一個立即執行的操作符
        }

        public (double highest, double lowest, double studentCount) GetSubjectStatistics(string subject)
        {
            /*
            var scores = new List<double>();

            foreach (var student in _students)
            {
                foreach (var grade in student.Grades)
                {
                    if (grade.Subject == subject)
                    {
                        scores.Add(grade.Score);
                    }
                }
            }

            if (scores.Count == 0)
            {
                return (0, 0, 0);
            }

            double highest = scores[0];
            double lowest = scores[0];

            // 寫法有問題，但是看不出來
            foreach (var score in scores)
            {
                if (score > highest) { highest = score; } // 不可以用比較運算子 ==，要賦值使用賦值運算子
                if (score < lowest) { lowest = score; }
            }

            // scores.Count()：這是一個 LINQ 語法，會回傳 scores 列表中的項目數量。
            // scores.Count：這是 List<T> 類別的屬性，也會回傳 scores 列表中的項目數量。
            return (highest, lowest, scores.Count);
            */

            var scores = _students
                .SelectMany(s => s.Grades)
                .Where(s => s.Subject == subject)
                .Select(g => g.Score)
                .ToList();

            if (scores.Count == 0)
            {
                return (0, 0, 0);
            }

            return(scores.Max(), scores.Min(), scores.Count());
        }

        public string GetStudentName(string numberId)

        {
            var student = GetStudnetNameByNumberId(numberId);

            if (student == null)
            {
                throw new InvalidOperationException($"找不到學號：{numberId}的學生");
            }

            return student.Name;
        }

        public Student? GetStudnetNameByNumberId(string numberId)
        { 
            return _students.FirstOrDefault(s => s.NumberId == numberId);
        }
    }
}
