using StudentsList.Models;
using System.Collections.ObjectModel;
using System.Text.Json;
namespace StudentsList.Views;

public partial class AddClassPage : ContentPage
{
    public ObservableCollection<ClassModel> ClassContainer { get; set; } = new();

    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "classes.txt");

    public AddClassPage()
    {
	InitializeComponent();
        BindingContext = this;
        LoadClasses();
    }

    private async void addClass_Clicked(object sender, EventArgs e)
    {
		string className = await DisplayPromptAsync("Add class", "Name of class");

		if(!string.IsNullOrEmpty(className))
		{
            ClassModel newClass = new ClassModel 
            { 
                Name = className
            };

            ClassContainer.Add(newClass);
            SaveClasses();
        }
		else
		{
			await DisplayAlert("Error", "Enter class name.","OK");
		}
    }

    private void LoadClasses()
    {
        if(File.Exists(_filePath))
        {
            try
            {
                string json = File.ReadAllText(_filePath);
                var classes = JsonSerializer.Deserialize<List<ClassModel>>(json);

                if (classes != null)
                {
                    ClassContainer.Clear();
                    foreach (var loadedClass in classes)
                    {
                        ClassContainer.Add(loadedClass);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Błąd", $"Failed to load classes: {ex.Message}", "OK");
            }
        }
    }

    private void SaveClasses()
    {
        try
        {
            var classes = ClassContainer.ToList();
            string json = JsonSerializer.Serialize(classes);
            File.WriteAllText(_filePath, json);
        }
        catch(Exception ex)
        {
            DisplayAlert("Error", $"Failed to save classes: {ex.Message}", "OK");
        }
    }

    private async void ClassListCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(e.CurrentSelection.FirstOrDefault() is ClassModel selectedClass)
        {
            await Navigation.PushAsync(new StudentsView(selectedClass));
        }

        ((CollectionView)sender).SelectedItem = null;
    }
}
