using StudentGradeManager.Domain;
using StudentGradeManager.Repository;
using StudentGradeManager.Service;


namespace StudentGradeManager.Program
{
    class Program
    {
        // 創建了一個 FileRepository 的新實例，被當作參數傳入 StudentService 的建構函式
        // StudentService 就能在內部使用這個 FileRepository 實例來進行檔案讀寫操作，但它本身並不需要知道 FileRepository 是如何被建立的
        private static StudentService _studentService = new StudentService(new FileRepository());

        static void Main(string[] args)
        {
            Console.WriteLine("==== 學生成績管理系統 ====");

            bool exit = false;
            while (!exit)
            {
                ShowMenu();
                string choice = Console.ReadLine();
                Console.Clear(); // 清除主控台（Console）螢幕上的所有內容；在 Console.ReadLine() 之後呼叫它，其主要目的是為了讓使用者介面更整潔

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddStudent();
                            break;
                        case "2":
                            AddStudentGrade();
                            break;
                        case "3":
                            UpdateStudentGrade();
                            break;
                        case "4":
                            GetStudentGrades();
                            break;
                        case "5":
                            GetStudentStatistics();
                            break;
                        case "6":
                            GetAllStudents();
                            break;
                        case "7":
                            GetStudents3Top();
                            break;
                        case "8":
                            GetSubjectStats();
                            break;
                        case "9":
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("無效的選擇，請輸入 1-9 的數字。");
                            break;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"操作錯誤：{ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"發生未預期的錯誤：{ex.Message}");
                }

                /*如果沒有這段程式碼，每次執行完一個功能後，程式會立即清除螢幕 (Console.Clear()) 並顯示主選單，
                 * 使用者可能還沒來得及看到操作結果（例如「新增學生成功！」），螢幕就已經被清空了。
                 * 透過 Console.ReadKey()，程式能給使用者一個「暫停」的時間，讓他們可以看清楚當前的操作結果，
                 * 再決定是否繼續下一個步驟。這大幅提升了主控台應用程式的使用者體驗。
                 */
                if (!exit)
                {
                    Console.WriteLine("\n按下任意鍵繼續...");
                    Console.ReadKey();
                }
            }
        }

        private static void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("==== 學生管理系統 ====");
            Console.WriteLine("1. 新增學生");
            Console.WriteLine("2. 新增學生科目成績");
            Console.WriteLine("3. 更新學生科目成績");
            Console.WriteLine("4. 查詢學生所有科目成績");
            Console.WriteLine("5. 查詢學生總平均、最高與最低成績");
            Console.WriteLine("6. 顯示所有學生資訊");
            Console.WriteLine("7. 顯示成績前三名學生");
            Console.WriteLine("8. 查詢指定科目最高分、最低分與總人數");
            Console.WriteLine("9. 離開程式");
            Console.Write("\n請輸入您的選擇：");
        }

        // static:這個方法屬於 Program 類別本身，而不是屬於 Program 類別的某個實例（instance）。
        // 你可以直接用 Program.AddStudent() 來呼叫它
        private static void AddStudent()
        {
            Console.Write("請輸入學號：");
            string numberId = Console.ReadLine();
            Console.Write("請輸入姓名：");
            string name = Console.ReadLine();
            Console.Write("請輸入班級：");
            string className = Console.ReadLine();

            _studentService.AddStudent(numberId, name, className);
            Console.WriteLine($"學生 {name} 已成功新增。");
        }

        private static void AddStudentGrade()
        {
            Console.Write("請輸入學號：");
            string numberId = Console.ReadLine();
            Console.Write("請輸入科目：");
            string subject = Console.ReadLine();
            Console.Write("請輸入成績：");
            double score = double.Parse(Console.ReadLine());

            _studentService.AddStudentGrade(numberId, subject, score);
            Console.WriteLine($"學生 {numberId} 的 {subject} 成績已成功新增。");
        }

        private static void UpdateStudentGrade()
        {
            Console.Write("請輸入學號：");
            string numberId = Console.ReadLine();
            Console.Write("請輸入要更新的舊科目名稱：");
            string oldSubject = Console.ReadLine();
            Console.Write("請輸入新的科目名稱：");
            string newSubject = Console.ReadLine();
            Console.Write("請輸入新的成績：");
            double newScore = double.Parse(Console.ReadLine());

            _studentService.UpdateStudentGrade(numberId, oldSubject, newSubject, newScore);
            Console.WriteLine($"學生 {numberId} 的成績已成功更新。");
        }

        private static void GetStudentGrades()
        {
            Console.Write("請輸入學號：");
            string numberId = Console.ReadLine();

            var grades = _studentService.GetStudentGrades(numberId);

            var studentName = _studentService.GetStudentName(numberId);

            // 檢查是否有找到學生或成績
            if (string.IsNullOrEmpty(studentName))
            {
                Console.WriteLine("該學號不存在。");
                return;
            }

            if (grades.Count == 0)
            {
                Console.WriteLine($"該學生{studentName}沒有成績或學號不存在。");
                return;
            }

            Console.WriteLine($"\n學生 {studentName} 的所有成績：");
            foreach (var grade in grades)
            {
                Console.WriteLine($" - 科目：{grade.Subject}, 成績：{grade.Score}");
            }
        }

        private static void GetStudentStatistics()
        {
            Console.Write("請輸入學號：");
            string numberId = Console.ReadLine();

            var stats = _studentService.GetStudentStatistics(numberId);

            if (stats.average == 0)
            {
                Console.WriteLine("該學生沒有成績或學號不存在。");
                return;
            }

            Console.WriteLine($"\n學生 {numberId} 的成績統計：");
            Console.WriteLine($" - 平均成績：{stats.average:F2}");
            Console.WriteLine($" - 最高分：{stats.highest}");
            Console.WriteLine($" - 最低分：{stats.lowest}");
        }

        private static void GetAllStudents()
        {
            var allStudents = _studentService.GetAllStudent();
            if (allStudents.Count == 0)
            {
                Console.WriteLine("目前沒有任何學生資料。");
                return;
            }

            Console.WriteLine("\n==== 所有學生資訊 ====");
            foreach (var student in allStudents)
            {
                Console.WriteLine($"學號: {student.NumberId}, 姓名: {student.Name}, 班級: {student.ClassName}");
                if (student.Grades.Any())
                {
                    Console.WriteLine("  成績:");
                    foreach (var grade in student.Grades)
                    {
                        Console.WriteLine($"    - 科目: {grade.Subject}, 成績: {grade.Score}");
                    }
                }
            }
        }

        private static void GetStudents3Top()
        {
            var topStudents = _studentService.GetStudents3Top();

            if (topStudents.Count == 0)
            {
                Console.WriteLine("目前沒有足夠的學生資料或成績來排名。");
                return;
            }

            Console.WriteLine("\n==== 成績前三名學生 ====");
            int rank = 1;
            foreach (var student in topStudents)
            {
                var averageScore = student.Grades.Average(g => g.Score);
                Console.WriteLine($"{rank}. 姓名: {student.Name}, 平均成績: {averageScore:F2}");
                rank++;
            }
        }

        private static void GetSubjectStats()
        {
            var allSubjects = _studentService.GetAllSubjects();
            if (allSubjects.Count == 0)
            {
                Console.WriteLine("目前沒有任何科目成績資料。");
                return;
            }

            Console.WriteLine("\n目前有的科目：");
            Console.WriteLine(string.Join(", ", allSubjects));  // string.Join 是一種靜態方法，以指定的分隔符號連接成一個單一的字串，然後輸出到主控台。

            Console.Write("\n請輸入要查詢的科目名稱：");
            string subject = Console.ReadLine();

            var stats = _studentService.GetSubjectStats(subject);

            if (stats.studentCount == 0)
            {
                Console.WriteLine($"找不到科目 '{subject}' 的成績資料。");
                return;
            }

            Console.WriteLine($"\n科目 '{subject}' 的統計：");
            Console.WriteLine($" - 最高分：{stats.highest}");
            Console.WriteLine($" - 最低分：{stats.lowest}");
            Console.WriteLine($" - 總學生數：{stats.studentCount}");
        }
    }
}