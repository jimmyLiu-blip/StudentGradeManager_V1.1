using Moq;  // 要在專案安裝nuGet套件
using StudentGradeManager.Domain; 
using StudentGradeManager.Repository; 
using StudentGradeManager.Service;
using System.Linq;
using System.Xml.Linq;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Student_Grade_Test
{
    public class StudentServiceTests
    {
        [Fact]
        public void AddStudent_ShouldAddStudentandSaveData_WhenStudentDoesExist()
        { 
            var mockFileRepository = new Mock<IFileRepository>();

            mockFileRepository.Setup(repo => repo.LoadDataFromJson())  // 運用擴充方法
                              .Returns(new List<Student>());

            var studentservice = new StudentService(mockFileRepository.Object);

            string numberId = "S001";

            string name = "小豬";

            string className = "1年1班";

            studentservice.AddStudent(numberId, name, className);

            var allstudents = studentservice.GetAllStudents();

            Assert.Contains(allstudents,s => s.NumberId == "S001" && s.Name == "小豬"); // 狀態驗證

            mockFileRepository.Verify(repo => repo.LoadDataFromJson(), Times.AtLeastOnce());

            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Once());

        }

        [Fact]
        public void AddStudent_ShouldThrowException_WhenStudentExist()
        {
            var mockFileRepository = new Mock<IFileRepository>();

            var existingStudents = new List<Student>{new Student("S001", "小明", "1年1班")};

            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(existingStudents);

            var studentService = new StudentService(mockFileRepository.Object);

            var exception = Assert.Throws<InvalidOperationException>(() => studentService.AddStudent("S001", "小華", "2年1班"));

            Assert.Equal("學號已存在學生列表中", exception.Message);

            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Never());

        }

        [Fact]
        public void GetStudnetName_ShouldReturnName_WhenStudentExist()
        {
            // Arrange
            var mockFileRepository = new Mock<IFileRepository>();

            var existingStudents = new List<Student> { new Student("S001", "小明", "1年1班") };

            mockFileRepository.Setup (repo => repo.LoadDataFromJson())
                              .Returns(existingStudents);

            var studentService = new StudentService(mockFileRepository.Object);

            // Act
            var studentName = studentService.GetStudentName("S001");

            // Assert
            Assert.Equal("小明", studentName);

            mockFileRepository.Verify(repo => repo.LoadDataFromJson(), Times.AtLeastOnce());

            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Never());
        }

        [Fact]
        public void GetStudentName_ShouldThrowException_WhenStudentDoesNotExist()
        {
            var mockFileRepository = new Mock<IFileRepository>();

            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(new List<Student>());

            var studentService = new StudentService(mockFileRepository.Object);

            var exception = Assert.Throws<InvalidOperationException>(() => studentService.GetStudentName("S001"));

            Assert.Equal($"找不到學號：S001的學生", exception.Message);

            mockFileRepository.Verify(repo => repo.LoadDataFromJson(), Times.AtLeastOnce());

            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Never());
        }

        [Fact]
        public void GetAllStudent_ShouldReturnAllStudent_WhenStudentExist()
        {
            var mockFileRepository = new Mock<IFileRepository>();

            var existingStudents = new List<Student> { 
                new Student("S001", "小東", "1年1班"),
                new Student("S002", "小西", "1年1班"),
                new Student("S003", "小南", "1年1班"),
            };
            
            mockFileRepository.Setup (repo => repo.LoadDataFromJson())
                              .Returns(existingStudents);

            var studentService = new StudentService (mockFileRepository.Object);

            var allStudents = studentService.GetAllStudents();

            // Assert.Equal(existingStudents, allStudents); 這樣寫不好

            Assert.Equal(existingStudents.Count, allStudents.Count);
           
            Assert.Collection(allStudents,
                s => Assert.Equal("S001", s.NumberId),
                s => Assert.Equal("S002", s.NumberId),
                s => Assert.Equal("S003", s.NumberId)
            );
            
            mockFileRepository.Verify(repo => repo.LoadDataFromJson(), Times.AtLeastOnce);

            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>() ), Times.Never );
        }

    }
}

