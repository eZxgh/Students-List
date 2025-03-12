namespace StudentsList.Views;
using CommunityToolkit.Maui.Storage;
using StudentsList.Models;
using System.Text.Json;
using System.Text;

public partial class StudentsView : ContentPage
{
    private static readonly FilePickerFileType TxtFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.WinUI, new[] { ".txt" } }
    });

    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "students.txt");
    public StudentsView()
    {
        InitializeComponent();
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
            DisplayAlert("Błąd", $"Nie udało się zapisać listy uczniów: {ex.Message}", "OK");
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

                    getHappyNumber();
                }
                catch (Exception ex)
                {
                    DisplayAlert("Błąd", $"Nie udało się załadować listy uczniów: {ex.Message}", "OK");
                }
            }
        }
    }
    private void onStudentAddClicked(object sender, EventArgs e)
    {
        string name = studentName.Text;
        string lastName = studentLastName.Text;
        string studentClass = studentsClass.Text;
        string numberText = studentNumber.Text;

        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(numberText) || string.IsNullOrEmpty(studentClass))
        {
            DisplayAlert("Błąd", "Wszystkie pola muszą być wypełnione.", "OK");
            return;
        }

        if (!int.TryParse(numberText, out int number))
        {
            DisplayAlert("Błąd", "Numer ucznia musi być liczbą całkowitą.", "OK");
            return;
        }

        if (number <= 0)
        {
            DisplayAlert("Błąd", "Numer ucznia nie może być mniejszy lub równy 0.", "OK");
            return;
        }

        if(BindingContext is Student student)
        {
            var newStudent = new StudentModel
            {
                Name = name,
                LastName = lastName,
                StudentClass = studentClass,
                Number = number,
            };

            var repeatedNumber = student.Students.Any(n => n.Number == newStudent.Number);

            if (!repeatedNumber)
            {
                student.Students.Add(newStudent);

                studentName.Text = string.Empty;
                studentLastName.Text = string.Empty;
                studentsClass.Text = string.Empty;
                studentNumber.Text = string.Empty;

                SaveStudents();

                DisplayAlert("Sukces", "Uczeń został dodany do listy.", "OK");
            }
            else
            {
                DisplayAlert("Uwaga", "Podany numer istnieje w liście i nie może się powtarzać.", "OK");
            }
        }    
    }
    private void getHappyNumber()
    {
        try
        {
            if (BindingContext is Student student && student.Students.Count > 0)
            {
                Random random = new Random();
                var happyStudent = student.Students[random.Next(student.Students.Count)];
                HappyNumberLabel.Text = $"Szczęśliwy numerek: {happyStudent.Number}";
            }
            else
            {
                HappyNumberLabel.Text = "Brak dostępnych numerków.";
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Błąd", $"Wystąpił problem podczas losowania szczęśliwego numerka: {ex.Message}", "OK");
        }
    }

    private void onGetRandomStudentClicked(object sender, EventArgs e)
    {
        try
        {
            if(BindingContext is Student student && student.Students.Count > 0)
            {
                Random random = new Random();
                int id = random.Next(student.Students.Count);   
                var randomStudent = student.Students[id];

                DisplayAlert("Losowanie", $"Wylosowany uczeń to {randomStudent.Name} {randomStudent.LastName} z klasy {randomStudent.StudentClass} o numerze {randomStudent.Number}", "OK");
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Błąd", $"Wystąpił problem podczas losowania ucznia: {ex.Message}", "OK");
        }
    }
    private async void onImportListClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = TxtFileType
            });

            if (result != null)
            {
                string json = await File.ReadAllTextAsync(result.FullPath);

                var importedStudents = JsonSerializer.Deserialize<List<StudentModel>>(json);

                if (importedStudents != null && BindingContext is Student student)
                {
                    foreach (var newStudent in importedStudents)
                    {
                        student.Students.Add(newStudent);
                        SaveStudents();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", $"Wystąpił problem podczas importowania: {ex.Message}", "OK");
        }
    }

    private async void onExportListClicked(object sender, EventArgs e)
    {
        try
        {
            if (BindingContext is Student student)
            {
                string json = JsonSerializer.Serialize(student.Students.ToList());

                var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

                var result = await FileSaver.SaveAsync("students.txt", stream, new CancellationToken());

                if (result.IsSuccessful)
                {
                    await DisplayAlert("Sukces", "Plik został zapisany.", "OK");
                }
                else
                {
                    await DisplayAlert("Błąd", $"Nie udało się zapisać pliku.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", $"Wystąpił problem podczas exportowania: {ex.Message}", "OK");
        }
    }

    private void onStudentRemoveClicked(object sender, EventArgs e)
    {
        if(sender is Button button && button.BindingContext is StudentModel studentModel)
        {
            if(BindingContext is Student student)
            {
                student.Students.Remove(studentModel);
                SaveStudents();
            }
        }
    }
}