using System.ComponentModel;

namespace StudentsList.Models
{
    public class StudentModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Number { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
