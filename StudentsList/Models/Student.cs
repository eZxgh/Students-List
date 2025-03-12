using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StudentsList.Models
{
    public class Student : INotifyPropertyChanged
    {
        public ObservableCollection<StudentModel> Students { get; set; } = new ObservableCollection<StudentModel>();

        public Student()
        {
            Students = new ObservableCollection<StudentModel>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
