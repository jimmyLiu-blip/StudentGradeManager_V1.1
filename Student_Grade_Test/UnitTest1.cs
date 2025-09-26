using Moq;  // �n�b�M�צw��nuGet�M��
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

            mockFileRepository.Setup(repo => repo.LoadDataFromJson())  // �B���X�R��k
                              .Returns(new List<Student>());

            var studentservice = new StudentService(mockFileRepository.Object);

            string numberId = "S001";

            string name = "�p��";

            string className = "1�~1�Z";

            studentservice.AddStudent(numberId, name, className);

            var allstudents = studentservice.GetAllStudents();

            Assert.Contains(allstudents,s => s.NumberId == "S001" && s.Name == "�p��"); // ���A����

            mockFileRepository.Verify(repo => repo.LoadDataFromJson(), Times.AtLeastOnce());

            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Once());

        }

        [Fact]
        public void AddStudent_ShouldThrowException_WhenStudentExist()
        {
            var mockFileRepository = new Mock<IFileRepository>();

            var existingStudents = new List<Student>{new Student("S001", "�p��", "1�~1�Z")};

            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(existingStudents);

            var studentService = new StudentService(mockFileRepository.Object);

            var exception = Assert.Throws<InvalidOperationException>(() => studentService.AddStudent("S001", "�p��", "2�~1�Z"));

            Assert.Equal("�Ǹ��w�s�b�ǥͦC��", exception.Message);

            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Never());

        }

        [Fact]
        public void GetStudnetName_ShouldReturnName_WhenStudentExist()
        {
            // Arrange
            var mockFileRepository = new Mock<IFileRepository>();

            var existingStudents = new List<Student> { new Student("S001", "�p��", "1�~1�Z") };

            mockFileRepository.Setup (repo => repo.LoadDataFromJson())
                              .Returns(existingStudents);

            var studentService = new StudentService(mockFileRepository.Object);

            // Act
            var studentName = studentService.GetStudentName("S001");

            // Assert
            Assert.Equal("�p��", studentName);

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

            Assert.Equal($"�䤣��Ǹ��GS001���ǥ�", exception.Message);

            mockFileRepository.Verify(repo => repo.LoadDataFromJson(), Times.AtLeastOnce());

            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Never());
        }

        [Fact]
        public void GetAllStudent_ShouldReturnAllStudent_WhenStudentExist()
        {
            var mockFileRepository = new Mock<IFileRepository>();

            var existingStudents = new List<Student> { 
                new Student("S001", "�p�F", "1�~1�Z"),
                new Student("S002", "�p��", "1�~1�Z"),
                new Student("S003", "�p�n", "1�~1�Z"),
            };
            
            mockFileRepository.Setup (repo => repo.LoadDataFromJson())
                              .Returns(existingStudents);

            var studentService = new StudentService (mockFileRepository.Object);

            var allStudents = studentService.GetAllStudents();

            // Assert.Equal(existingStudents, allStudents); �o�˼g���n

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

