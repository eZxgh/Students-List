using StudentsList.Models;
using System.Text.Json;

namespace StudentsList.Views;

public partial class StudentsView : ContentPage
{
    private string _filePath => Path.Combine(FileSystem.AppDataDirectory, $"{SelectedClass.Name}_students.json");

    public ClassModel SelectedClass { get; set; }

    public StudentsView(ClassModel selectedClass)
    {
        InitializeComponent();
        SelectedClass = selectedClass;
        BindingContext = new Student();
        LoadStudents();
    }

    private void SaveStudents()
    {
        try
        {
            if (BindingContext is Student student)
            {
                string json = JsonSerializer.Serialize(student.Students.ToList());
                File.WriteAllText(_filePath, json);
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to save students: {ex.Message}", "OK");
        }
    }

    private void LoadStudents()
    {
        if (BindingContext is Student student)
        {
            student.Students.Clear();

            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    var students = JsonSerializer.Deserialize<List<StudentModel>>(json);

                    if (students != null)
                    {
                        foreach (var loadedStudent in students)
                        {
                            student.Students.Add(loadedStudent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("Error", $"Failed to load students: {ex.Message}", "OK");
                }
            }
        }
    }

    private void onStudentAddClicked(object sender, EventArgs e)
    {
        string name = studentName.Text;
        string lastName = studentLastName.Text;
        string numberText = studentNumber.Text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(numberText))
        {
            DisplayAlert("Error", "All fields must be filled.", "OK");
            return;
        }

        if (!int.TryParse(numberText, out int number) || number <= 0)
        {
            DisplayAlert("Error", "Student number must be a positive integer.", "OK");
            return;
        }

        if (BindingContext is Student student)
        {
            var newStudent = new StudentModel
            {
                Name = name,
                LastName = lastName,
                Number = number
            };

            bool repeatedNumber = student.Students.Any(n => n.Number == newStudent.Number);

            if (!repeatedNumber)
            {
                student.Students.Add(newStudent);

                studentName.Text = string.Empty;
                studentLastName.Text = string.Empty;
                studentNumber.Text = string.Empty;

                SaveStudents();
                DisplayAlert("Success", "Student added.", "OK");
            }
            else
            {
                DisplayAlert("Warning", "This student number already exists in the list.", "OK");
            }
        }
    }

    private void onGetRandomStudentClicked(object sender, EventArgs e)
    {
        try
        {
            if (BindingContext is Student student && student.Students.Count > 0)
            {
                Random random = new Random();
                int id = random.Next(student.Students.Count);
                var randomStudent = student.Students[id];

                DisplayAlert("Random Student", $"Selected: Name: {randomStudent.Name} Last Name: {randomStudent.LastName} No.{randomStudent.Number}", "OK");
            }
            else
            {
                DisplayAlert("Error", "List of students is empty.", "OK");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"An error occurred while selecting a student: {ex.Message}", "OK");
        }
    }

    private void onStudentRemoveClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is StudentModel studentModel)
        {
            if (BindingContext is Student student)
            {
                student.Students.Remove(studentModel);
                SaveStudents();
            }
        }
    }
}
