using QuizSolver.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;

namespace QuizSolver.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public List<Quiz> QuizQuestions { get; set; }
        //private Quiz _quizQuestion;
        //public Quiz QuizQuestion
        //{
        //    get => _quizQuestion;
        //    set => _quizQuestion = value;
        //}
        
        //public List<Quiz> QuizzesList { get; set; }
        //public void LoadAllQuizzes()
        //{
        //    var quizzesList = DataAccess.LoadQuizzesList();
        //    //MessageBox.Show(quizzesList[0]);
        //    QuizzesList = quizzesList;
        //}

        private int _questionNumber;
        private int _score;


        // Commands
        #region Commands

         public RelayCommand StartCommand => new(execute => StartQuiz(), canExecute => IsQuizSelected != null);
        private void StartQuiz()
        {
            _questionNumber = 0;

            IsStartEnabled = false;
            IsFinishEnabled = true;
            IsAnswer1Enabled = true;
            IsAnswer2Enabled = true;
            IsAnswer3Enabled = true;
            IsAnswer4Enabled = true;
            IsNextEnabled = true;
            IsChooseQuizVisible = false;

            QuizQuestions = DataAccess.LoadQuestions();

            QuestionScore = "Question:";
            NumberOfQuestion = $"Question 1/{QuizQuestions.Count}";

            SetQuestion(_questionNumber);
            SetAnswers(_questionNumber);
            
            StartTimer();
        }

        public RelayCommand FinishCommand => new(execute => FinishQuiz());
        private void FinishQuiz()
        {
            IsStartEnabled = true;
            IsFinishEnabled = false;
            IsChooseQuizVisible = true;

            _stopwatch.Stop();
            _timer.Stop();

            QuestionScore = $"\nYou scored {_score} points in {TimeElapsed}.";
            NumberOfQuestion = null;
            Question = null;
            Answer1 = null;
            Answer2 = null;
            Answer3 = null;
            Answer4 = null;

            IsAnswer1Enabled = false;
            IsAnswer2Enabled = false;
            IsAnswer3Enabled = false;
            IsAnswer4Enabled = false;
            IsNextEnabled = false;
            IsPreviousEnabled = false;
        }

        public RelayCommand Answer1Command => new(execute => Answer1Chose());
        public void Answer1Chose()
        {

        }

        public RelayCommand Answer2Command => new(execute => Answer2Chose());
        public void Answer2Chose()
        {

        }

        public RelayCommand Answer3Command => new(execute => Answer3Chose());
        public void Answer3Chose()
        {

        }

        public RelayCommand Answer4Command => new(execute => Answer4Chose());
        public void Answer4Chose()
        {

        }

        public RelayCommand NextCommand => new(execute => NextQuestion());
        public void NextQuestion()
        {
            IsPreviousEnabled = true;
            _questionNumber++;
            NumberOfQuestion = $"Question {_questionNumber + 1}/{QuizQuestions.Count}";
            SetQuestion(_questionNumber);
            SetAnswers(_questionNumber);

            if (_questionNumber + 1 == QuizQuestions.Count)
                IsNextEnabled = false;
        }

        public RelayCommand PreviousCommand => new(execute => PreviousQuestion());
        public void PreviousQuestion()
        {
            IsNextEnabled = true;
            _questionNumber--;
            NumberOfQuestion = $"Question {_questionNumber + 1}/{QuizQuestions.Count}";
            SetQuestion(_questionNumber);
            SetAnswers(_questionNumber);

            if (_questionNumber == 0)
                IsPreviousEnabled = false;
        }

        #endregion Commands


        // Methods
        #region Methods

        public void SetQuestion(int questionNumber)
        {
            Question = Decription.Decript(QuizQuestions[questionNumber].Question);
        }

        private void SetAnswers(int questionNumber)
        {
            Answer1 = Decription.Decript(QuizQuestions[questionNumber].Answer1);
            Answer2 = Decription.Decript(QuizQuestions[questionNumber].Answer2);
            Answer3 = Decription.Decript(QuizQuestions[questionNumber].Answer3);
            Answer4 = Decription.Decript(QuizQuestions[questionNumber].Answer4);
        }

        private Stopwatch _stopwatch;
        private Timer _timer;
        private void StartTimer()
        {
            _stopwatch = new Stopwatch();
            _timer = new Timer(1000);
            TimeElapsed = "00:00";
            _timer.Elapsed += TimerTick;
            _stopwatch.Start();
            _timer.Start();
        }
        
        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => { TimeElapsed = _stopwatch.Elapsed.ToString(@"mm\:ss"); });
        }

        #endregion


        // Properites
        #region Properties

        private string _numberOfQuestion;
        public string NumberOfQuestion
        {
            get => _numberOfQuestion;
            set
            {
                _numberOfQuestion = value;
                OnPropertyChanged();
            }
        }

        private string _question;
        public string Question
        {
            get => _question;
            set
            {
                _question = value;
                OnPropertyChanged();
            }
        }

        private string _answer1;
        public string Answer1
        {
            get => _answer1;
            set
            {
                _answer1 = value;
                OnPropertyChanged();
            }
        }

        private string _answer2;
        public string Answer2
        {
            get => _answer2;
            set
            {
                _answer2 = value;
                OnPropertyChanged();
            }
        }

        private string _answer3;
        public string Answer3
        {
            get => _answer3;
            set
            {
                _answer3 = value;
                OnPropertyChanged();
            }
        }

        private string _answer4;
        public string Answer4
        {
            get => _answer4;
            set
            {
                _answer4 = value;
                OnPropertyChanged();
            }
        }

        private string _questionScore;
        public string QuestionScore
        {
            get => _questionScore;
            set
            {
                _questionScore = value;
                OnPropertyChanged();
            }
        }

        private string _timeElapsed;
        public string TimeElapsed
        {
            get => _timeElapsed;
            set
            {
                _timeElapsed = value;
                OnPropertyChanged();
            }
        }

        #endregion


        // IsEnabled
        #region IsEnabled

        private bool _isStartEnabled = true;
        public bool IsStartEnabled
        {
            get => _isStartEnabled;
            set
            {
                _isStartEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isFinishEnabled = false;
        public bool IsFinishEnabled
        {
            get => _isFinishEnabled;
            set
            {
                _isFinishEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isAnswer1Enabled = false;
        public bool IsAnswer1Enabled
        {
            get => _isAnswer1Enabled;
            set
            {
                _isAnswer1Enabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isAnswer2Enabled = false;
        public bool IsAnswer2Enabled
        {
            get => _isAnswer2Enabled;
            set
            {
                _isAnswer2Enabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isAnswer3Enabled = false;
        public bool IsAnswer3Enabled
        {
            get => _isAnswer3Enabled;
            set
            {
                _isAnswer3Enabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isAnswer4Enabled = false;
        public bool IsAnswer4Enabled
        {
            get => _isAnswer4Enabled;
            set
            {
                _isAnswer4Enabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isNextEnabled = false;
        public bool IsNextEnabled
        {
            get => _isNextEnabled;
            set
            {
                _isNextEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isPreviousEnabled = false;
        public bool IsPreviousEnabled
        {
            get => _isPreviousEnabled;
            set
            {
                _isPreviousEnabled = value;
                OnPropertyChanged();
            }
        }

        #endregion

        // IsQuizSelected
        private bool _isQuizSelected = false;
        public bool IsQuizSelected
        {
            get => _isQuizSelected;
            set
            {
                _isQuizSelected = value;
                OnPropertyChanged();
            }
        }

        // SelectedQuizIndex
        private int _selectedQuizIndex;
        public int SelectedQuizIndex
        {
            get => _selectedQuizIndex;
            set
            {
                _selectedQuizIndex = value;
                OnPropertyChanged();
            }
        }


        // IsVisible
        #region IsVisible

        private string _isChooseQuizVisible = "Visible";
        public bool IsChooseQuizVisible
        {
            get => _isChooseQuizVisible == "Visible";

            set
            {
                _isChooseQuizVisible = value ? "Visible" : "Hidden";
                OnPropertyChanged();
            }
        }

        #endregion
    }
}
