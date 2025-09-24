using Xunit;
using Moq;  // �n�b�M�צw��nuGet�M��
using StudentGradeManager.Domain; 
using StudentGradeManager.Repository; 
using StudentGradeManager.Service;
using System.Linq;

namespace Student_Grade_Test
{
    public class StudentServiceTests
    {
        // �w�q�@�Ӵ��դ�k
        [Fact] // Xunit �����ռ��ҡA��ܳo�O�@�Ӵ��դ�k
        public void AddStudent_ShouldAddStudentAndSaveData_WhenStudentDoesNotExist()
        {
            // Arrange (�w��)
            // �Ыؤ@�� Moq �� FileRepository ����C
            // �o�Ӽ�������|���˦ۤv�O�@�� FileRepository�A�������欰�ѧڭ̱���C
            var mockFileRepository = new Mock<FileRepository>();

            // �]�w mockFileRepository �� LoadDataFromJson() ��k���欰�C
            // �� StudentService �I�s LoadDataFromJson() �ɡA���|��^�@�ӪŪ��ǥͦC��A
            // ��ܤ@�}�l�S������ǥ͸�ơC
            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(new List<Student>());

            // �Ы� StudentService ����ҡA�ñN������ FileRepository �ǤJ�C
            // �o�� StudentService �N���|�u���hŪ�g�ɮסA�ӬO�P�ڭ̪��������󤬰ʡC
            var studentService = new StudentService(mockFileRepository.Object);

            string numberId = "S001";
            string name = "�i�T";
            string className = "�@�~�үZ";

            // Act (����)
            // �I�s�ڭ̭n���ժ���k
            studentService.AddStudent(numberId, name, className);

            // Assert (�_��/����)

            // ���� LoadDataFromJson ��k�Q�I�s�F�ܤ֤@�� (�b StudentService �غc��)�C
            mockFileRepository.Verify(repo => repo.LoadDataFromJson(), Times.AtLeastOnce());

            // ���� SaveDataToJson ��k�Q�I�s�F��n�@�� (�b AddStudent ������)�C
            // �o�����ҫD�`���n�A���T�O�F�ǥ͸�Ʀb�s�W�ᦳ�Q�O�s�C
            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Once());

            // ���ҾǥͦC���T��s�W�F�i�T�o�Ӿǥ�
            // �ڭ̥i�H�q StudentService ���o�Ҧ��ǥ͡A�M���ˬd�i�T�O�_�s�b
            var allStudents = studentService.GetAllStudent();
            Assert.True(allStudents.Any(s => s.NumberId == numberId && s.Name == name));
        }

        [Fact]
        public void AddStudent_ShouldThrowException_WhenStudentAlreadyExists()
        {
            // Arrange (�w��)
            var mockFileRepository = new Mock<FileRepository>();
            var existingStudents = new List<Student>
            {
                new Student("S001", "�i�T", "�@�~�үZ") // �w�]�ǥͦC��]�t S001
            };
            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(existingStudents);

            var studentService = new StudentService(mockFileRepository.Object);

            string numberId = "S001"; // �A�����շs�W�w�s�b���Ǹ�
            string name = "���|";
            string className = "�@�~�A�Z";

            // Act & Assert (����P�_��)
            // ���ҩI�s AddStudent �|�ߥX InvalidOperationException
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                studentService.AddStudent(numberId, name, className);
            });

            // ���ҩߥX�����`�T���O�_�ŦX�w��
            Assert.Equal("�Ǹ��w�s�b�ǥͦC��", exception.Message);

            // ���� SaveDataToJson ��k�S���Q�I�s�A�]���ާ@���Ѥ����Ӧs��
            mockFileRepository.Verify(repo => repo.SaveDataToJson(It.IsAny<List<Student>>()), Times.Never());
        }

        // �i�H�A�� GetStudentName ��k�g�@��²�檺����
        [Fact]
        public void GetStudentName_ShouldReturnName_WhenStudentExists()
        {
            // Arrange
            var mockFileRepository = new Mock<FileRepository>();
            var students = new List<Student>
            {
                new Student("S001", "�i�T", "�@�~�үZ"),
                new Student("S002", "���|", "�@�~�A�Z")
            };
            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(students);

            var studentService = new StudentService(mockFileRepository.Object);

            // Act
            var studentName = studentService.GetStudentName("S001");

            // Assert
            Assert.Equal("�i�T", studentName);
        }

        [Fact]
        public void GetStudentName_ShouldReturnNull_WhenStudentDoesNotExist()
        {
            // Arrange
            var mockFileRepository = new Mock<FileRepository>();
            var students = new List<Student>
            {
                new Student("S001", "�i�T", "�@�~�үZ")
            };
            mockFileRepository.Setup(repo => repo.LoadDataFromJson())
                              .Returns(students);

            var studentService = new StudentService(mockFileRepository.Object);

            // Act
            var studentName = studentService.GetStudentName("S999"); // �d�ߤ��s�b���Ǹ�

            // Assert
            Assert.Null(studentName); // �w����^ null
        }
    }
}