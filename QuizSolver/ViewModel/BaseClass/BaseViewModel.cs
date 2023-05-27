using System.ComponentModel;

namespace QuizSolver.ViewModel.BaseClass
{
    class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(params string[] properties)
        {
            if (PropertyChanged == null) return;
            foreach (var property in properties)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
