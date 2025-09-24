using Xunit;
using Moq;  // 要在專案安裝nuGet套件
using StudentGradeManager.Domain; 
using StudentGradeManager.Repository; 
using StudentGradeManager.Service;
using System.Linq;

namespace Student_Grade_Test
{
    public class StudentServiceTests
    {
        // 定義一個測試方法
        [Fact] // Xunit 的測試標籤，表示這是一個測試方法
        public void AddStudent_ShouldAddStudentAndSaveData_WhenStudentDoesNotExist()
        {
            // Arrange (安排)
            // 創建一個 Moq 的 FileRepository 物件。
            // 這個模擬物件會假裝自己是一個 FileRepository，但它的行為由我們控制。
            var mockFileRepository = new Mock<FileRepository>();

            // 設定 mockFileRepository 的 LoadDataFromJson() 方法的行為。
            // 當 StudentService 呼叫 LoadDataFromJson() 時，它會返回一個空的學生列表，
            // 表示一開始沒有任何學生資料。
            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(new List<Student>());

            // 創建 StudentService 的實例，並將模擬的 FileRepository 傳入。
            // 這樣 StudentService 就不會真的去讀寫檔案，而是與我們的模擬物件互動。
            var studentService = new StudentService(mockFileRepository.Object);

            string numberId = "S001";
            string name = "張三";
            string className = "一年甲班";

            // Act (執行)
            // 呼叫我們要測試的方法
            studentService.AddStudent(numberId, name, className);

            // Assert (斷言/驗證)

            // 驗證 LoadDataFromJson 方法被呼叫了至少一次 (在 StudentService 建構時)。
            mockFileRepository.Verify(repo => repo.LoadDataFromJson(), Times.AtLeastOnce());

            // 驗證 SaveDataToJson 方法被呼叫了恰好一次 (在 AddStudent 結束時)。
            // 這個驗證非常重要，它確保了學生資料在新增後有被保存。
            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Once());

            // 驗證學生列表中確實新增了張三這個學生
            // 我們可以從 StudentService 取得所有學生，然後檢查張三是否存在
            var allStudents = studentService.GetAllStudent();
            Assert.True(allStudents.Any(s => s.NumberId == numberId && s.Name == name));
        }

        [Fact]
        public void AddStudent_ShouldThrowException_WhenStudentAlreadyExists()
        {
            // Arrange (安排)
            var mockFileRepository = new Mock<FileRepository>();
            var existingStudents = new List<Student>
            {
                new Student("S001", "張三", "一年甲班") // 預設學生列表包含 S001
            };
            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(existingStudents);

            var studentService = new StudentService(mockFileRepository.Object);

            string numberId = "S001"; // 再次嘗試新增已存在的學號
            string name = "李四";
            string className = "一年乙班";

            // Act & Assert (執行與斷言)
            // 驗證呼叫 AddStudent 會拋出 InvalidOperationException
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                studentService.AddStudent(numberId, name, className);
            });

            // 驗證拋出的異常訊息是否符合預期
            Assert.Equal("學號已存在學生列表中", exception.Message);

            // 驗證 SaveDataToJson 方法沒有被呼叫，因為操作失敗不應該存檔
            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Never());
        }

        // 可以再為 GetStudentName 方法寫一個簡單的測試
        [Fact]
        public void GetStudentName_ShouldReturnName_WhenStudentExists()
        {
            // Arrange
            var mockFileRepository = new Mock<FileRepository>();
            var students = new List<Student>
            {
                new Student("S001", "張三", "一年甲班"),
                new Student("S002", "李四", "一年乙班")
            };
            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(students);

            var studentService = new StudentService(mockFileRepository.Object);

            // Act
            var studentName = studentService.GetStudentName("S001");

            // Assert
            Assert.Equal("張三", studentName);
        }

        [Fact]
        public void GetStudentName_ShouldReturnNull_WhenStudentDoesNotExist()
        {
            // Arrange
            var mockFileRepository = new Mock<FileRepository>();
            var students = new List<Student>
            {
                new Student("S001", "張三", "一年甲班")
            };
            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(students);

            var studentService = new StudentService(mockFileRepository.Object);

            // Act
            var studentName = studentService.GetStudentName("S999"); // 查詢不存在的學號

            // Assert
            Assert.Null(studentName); // 預期返回 null
        }
    }
}