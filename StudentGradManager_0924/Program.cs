using StudentGradeManager.Domain;
using StudentGradeManager.Repository;
using StudentGradeManager.Service;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using System.Xml.Linq;


namespace StudentGradeManager.Program
{
    class Program
    {
        // 介面無法被實例化，不能直接用new IFileRepository()
        // 靜態欄位初始化問題：需要具體的實作類別
        // 較複雜的Console會把private static StudentService _studentService = new StudentService(new FileRepository());放在開頭
        private static StudentService _studentService = new StudentService(new FileRepository());

        // Main方法是靜態的，必須是static
        static void Main(string[] args)
        {
            bool exit = false;

            while (!exit)
            {
                try
                {
                    ShowMenu();

                    Console.Write("請選擇功能：");

                    string choice = Console.ReadLine()??"";

                    Console.Clear();

                    switch (choice)
                    {
                        case "1":
                            AddStudent();
                            break;
                        case "2":
                            UpdateStudent();
                            break;
                        case "3":
                            GetAllStudents();
                            break;
                        case "4":
                            AddStudentGrade();
                            break;
                        case "5":
                            UpdateStudentGrade();
                            break;
                        case "6":
                            GetStudentGrades();
                            break;
                        case "7":
                            GetTop3Students();
                            break;
                        case "8":
                            GetAllSubjects();
                            break;
                        case "9":
                            GetStudentName();
                            break;
                        case "10":
                            GetStudentStatistics();
                            break;
                        case "11":
                            GetSubjectStatistics();
                            break;
                        case "12":
                            Console.WriteLine("感謝使用學生成績管理系統，再見！");
                            exit = true;
                            break;
                        default:
                            {
                                Console.WriteLine("無效的選擇，請按任意件離開");
                                break;
                            }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"操作錯誤，{ex.Message}");
                    PauseAndContinue();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"出現異常錯誤，{ex.Message}");
                    PauseAndContinue();
                }
                if (!exit)
                {
                    PauseAndContinue();
                }
}

        }
        private static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("   ===學生成績管理系統===   ");
            Console.WriteLine("輸入1，新增學生");
            Console.WriteLine("輸入2，更新指定學生姓名班級");
            Console.WriteLine("輸入3，取得所有學生");
            Console.WriteLine("輸入4，新增學生科目成績");
            Console.WriteLine("輸入5，更新指定學生科目成績");
            Console.WriteLine("輸入6，取得指定學生科目成績");
            Console.WriteLine("輸入7，取得班上前三名");
            Console.WriteLine("輸入8，取得班上所有科目");
            Console.WriteLine("輸入9，取得指定學生名字");
            Console.WriteLine("輸入10，取得指定學生的平均成績、最高分、最低分");
            Console.WriteLine("輸入11，取得指定科目的最高分、最低分和學生人數");
            Console.WriteLine("輸入12，離開系統");
        }
        private static void AddStudent()
        {
            Console.WriteLine("=== 新增學生 ===");

            Console.WriteLine("請輸入學生的學號：");
            string numberId = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(numberId))
            {
                Console.WriteLine("學號不能為空");
                return;
            }

            Console.WriteLine("請輸入學生的姓名：");
            string name = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("姓名不能為空");
                return;
            }

            Console.WriteLine("請輸入學生的班級：");
            string classname = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(classname))
            {
                Console.WriteLine("班級不能為空");
                return;
            }

            _studentService.AddStudent(numberId, name, classname);

            Console.WriteLine($"學生{name}：已新增成功");

        }
        private static void AddStudentGrade()
        {
            Console.WriteLine("=== 新增學生科目成績 ===");

            Console.WriteLine("請輸入學生的學號:");
            string numberId = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(numberId))
            {
                Console.WriteLine("學號不能為空");
                return;
            }

            Console.WriteLine("請輸入科目:");
            string subject = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(subject))
            {
                Console.WriteLine("科目不能為空");
                return;
            }

            Console.WriteLine("請輸入成績:");
            // double score = double.TryParse(Console.ReadLine() ?? ""); 建議修改
            string scoreInput = Console.ReadLine() ?? "";
            double score;

            while (!double.TryParse(scoreInput, out score) || score < 0 || score > 100)
            {
                Console.WriteLine("成績輸入無效，請輸入介於 0 ~ 100之間的數字");
                scoreInput = Console.ReadLine() ?? "";
            }

            _studentService.AddStudentGrade(numberId, subject, score);

            var student = _studentService.GetStudentName(numberId);

            Console.WriteLine($"學生：{student}的科目成績已新增成功");

        }

        //取得所有學生：不熟
        private static void GetAllStudents()
        {
            var allstudents = _studentService.GetAllStudents();

            Console.WriteLine("=== 所有學生資訊 ===");

            foreach (var student in allstudents)
            {
                Console.WriteLine($"\n學號:{student.NumberId}, 姓名:{student.Name}, 班級:{student.ClassName}");
                if (student.Grades.Any())
                {
                    Console.WriteLine("  成績:");
                    foreach (var grade in student.Grades)
                    {
                        Console.WriteLine($"   -科目:{grade.Subject}, 成績:{grade.Score}");
                    }
                }
            }

        }
        private static void UpdateStudent()
        {
            Console.WriteLine("=== 更新學生姓名班級 ===");

            Console.WriteLine("請輸入學生的學號：");
            string numberId = Console.ReadLine() ?? "" ;
            if (string.IsNullOrWhiteSpace(numberId))
            {
                Console.WriteLine("學號不能為空");
                return;
            }

            Console.WriteLine("請輸入學生的新姓名：");
            string newName = Console.ReadLine() ?? "" ;
            if (string.IsNullOrWhiteSpace(newName))
            {
                Console.WriteLine("學生的新姓名不能為空");
                return;
            }

            Console.WriteLine("請輸入學生的新班級：");
            string newClassName = Console.ReadLine() ?? "" ;
            if (string.IsNullOrWhiteSpace(newClassName))
            {
                Console.WriteLine("學生的新班級不能為空");
                return;
            }

            _studentService.UpdateStudent(numberId, newName, newClassName);

            Console.WriteLine($"更新學號為{numberId}的姓名和班級成功");
        }
        private static void PauseAndContinue()
        {
            Console.WriteLine("\n按下任意鍵繼續...");
            Console.ReadKey();
        }

        private static void UpdateStudentGrade()
        {
            Console.WriteLine("=== 更新學生的科目成績 ===");

            Console.WriteLine("請輸入想要更改科目成績的學生學號：");
            string numberId = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(numberId))
            {
                Console.WriteLine("學號不可為空");
                return;
            }

            Console.WriteLine("請輸入想要更正的科目名稱(修正前)");
            string oldsubject = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(oldsubject))
            {
                Console.WriteLine("舊科目不可為空");
                return;
            }

            Console.WriteLine("請輸入想要更正的科目名稱(修正後)");
            string newsubject = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(newsubject))
            {
                Console.WriteLine("新科目不可為空");
                return;
            }

            Console.WriteLine("請輸入想要更正的科目成績(修正後)");
            string newscoreInput = Console.ReadLine() ?? "";
            double newscore = 0;

            while (!double.TryParse(newscoreInput, out newscore) || newscore < 0 || newscore > 100)
            {
                Console.WriteLine("成績輸入無效，請輸入介於 0 ~ 100之間的數字");
                newscoreInput = Console.ReadLine() ?? "";
            }

            _studentService.UpdateStudentGrade(numberId, oldsubject, newsubject, newscore);
        }

        //取得學生科目成績：不熟
        private static void GetStudentGrades()
        {
            Console.WriteLine("=== 取得指定學生分數 ===");

            Console.WriteLine("請輸入想要查詢科目成績的學生學號：");
            string numberId = Console.ReadLine()??"";
            if (string.IsNullOrWhiteSpace(numberId))
            {
                Console.WriteLine("學號不可為空");
                return;
            }

            var grades = _studentService.GetStudentGrades(numberId);

            var studentName = _studentService.GetStudentName(numberId);

            if (string.IsNullOrEmpty(studentName))
            {
                Console.WriteLine("該學生不存在");
                return;
            }

            if (grades.Count == 0)
            {
                Console.WriteLine($"該學生：{studentName}沒有成績");
                return;
            }

            Console.WriteLine($"學生：{studentName}的所有成績");
            foreach (var grade in grades)
            {
                Console.WriteLine($" - 科目：{grade.Subject}, 成績:{grade.Score}");
            }

        }

        //取得成績前三名：有部分遺漏
        private static void GetTop3Students()
        { 
            var topStudents = _studentService.GetTop3Students();

            if (topStudents.Count == 0)
            {
                Console.WriteLine("目前沒有足夠的學生資料或成績來排名");
                return;
            }

            Console.WriteLine("\n==== 成績前三名學生 ====");
            int rank = 1;

            foreach (var student in topStudents)
            {
                var averageScore = student.Grades.Average(g => g.Score);
                Console.WriteLine($"第{rank}名為{student.Name},平均成績為：{averageScore}");
                rank++;
            }
        }

        private static void GetAllSubjects()
        { 
            var allSubjects = _studentService.GetAllSubjects();

            if (allSubjects.Count == 0)
            {
                Console.WriteLine("目前沒有科目可以取得");
                return;
            }

            foreach (var subject in allSubjects)
            {
                Console.WriteLine($"科目-{subject}");
            }
        }

        private static void GetStudentName()
        {
            Console.WriteLine("=== 取得指定學號的學生 ===");
            Console.WriteLine("請輸入想要查詢姓名的學生學號：");
            string numberId = Console.ReadLine()??"";

            if (string.IsNullOrWhiteSpace(numberId))
            {
                Console.WriteLine("學號不可為空");
                return;
            }

            var student = _studentService.GetStudentName(numberId);

            if (student == null)
            {
                Console.WriteLine("該學號的學生姓名不存在");
                return;
            }

            Console.WriteLine($"學號：{numberId}的學生姓名為：{student}");

        }

        // 指定學生的學號，查詢他的平均成績、最高分、最低分要再練習
        public static void GetStudentStatistics()
        {
            Console.WriteLine("=== 取得學生分數統計 ===");
            Console.WriteLine("請輸入指定學生的學號，查詢他的平均成績、最高分、最低分：");
            string numberId = Console.ReadLine()??"";
            if (string.IsNullOrWhiteSpace(numberId))
            {
                Console.WriteLine("學號不可為空");
                return;
            }

            var statistics = _studentService.GetStudentStatistics(numberId);
            if (statistics.average == 0 && statistics.highest == 0 && statistics.lowest == 0)
            {
                Console.WriteLine($"找不到學號為{numberId}的學生，或該學生沒有任何科目成績");
                return;
            }

            var studentName = _studentService.GetStudentName(numberId);

            Console.WriteLine($"學生{studentName}的成績統計：");
            Console.WriteLine($"平均分數為{statistics.average:F2}");
            Console.WriteLine($"最高分數為{statistics.highest}");
            Console.WriteLine($"最低分數為{statistics.lowest}");
        }

        private static void GetSubjectStatistics()
        {
            Console.WriteLine("===取得指定科目的最高分、最低分，及考試人數===");
            Console.WriteLine("請輸入指定查詢的科目");
            string subject = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(subject))
            {
                Console.WriteLine("指定科目不存在");
                return;
            }

            var statistics = _studentService.GetSubjectStatistics(subject);
            if (statistics.studentCount == 0)
            {
                Console.WriteLine($"無人報考此科目:{subject}");
                return;
            }

            Console.WriteLine($"{subject}科目的成績統計為：");
            Console.WriteLine($"最高分為：{statistics.highest}");
            Console.WriteLine($"最低分為：{statistics.lowest}");
            Console.WriteLine($"報考人數為:{statistics.studentCount}");




        }


    }

    
}