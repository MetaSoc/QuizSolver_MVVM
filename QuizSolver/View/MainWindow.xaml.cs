using QuizSolver.ViewModel;
using System.Windows;

namespace QuizSolver.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var vm = new MainViewModel();
            DataContext = vm;
            InitializeComponent();
        }
    }
}
