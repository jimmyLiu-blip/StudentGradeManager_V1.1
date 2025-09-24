

namespace StudentGradeManager.Domain
{
    public class Student
    {
        public string NumberId { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }

        // 忘記初始化(時常忘記)
        public List<Grade> Grades { get; set; } = new List<Grade>();
        public Student(string numberId, string name, string className)
        {
            NumberId = numberId;
            Name = name;
            ClassName = className;
        }
    }

}
