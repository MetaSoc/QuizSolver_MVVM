using QuizSolver.Model;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Windows;

namespace QuizSolver.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            LoadAllQuizzes();
        }
        public List<Quiz> QuizQuestions { get; set; } = new();
        
        public List<string> QuizzesList { get; set; } = new();
        public void LoadAllQuizzes()
        {
            var quizzesList = DataAccess.LoadQuizzesList();
            quizzesList.Sort();
            QuizzesList = quizzesList;
        }

        private int _questionNumber;
        private int _score;


        // Commands
        #region Commands

         public RelayCommand StartCommand => new(execute => StartQuiz(), canExecute => SelectedQuizIndex != -1);
        private void StartQuiz()
        {
            QuizQuestions = DataAccess.LoadQuestions(SelectedQuiz);

            SelectedQuizIndex = -1;

            _questionNumber = 0;
            _score = 0;

            IsStartEnabled = IsChooseQuizVisible = false;

            IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = IsNextEnabled = IsTipVisible = true;

            QuestionScore = "Question:";
            NumberOfQuestion = $"Question {_questionNumber+1}/{QuizQuestions.Count}";

            SetQuestion(_questionNumber);
            SetAnswers(_questionNumber);
            
            StartTimer();
        }

        public RelayCommand FinishCommand => new(execute => FinishQuiz(), canExecute => CanFinish());
        private void FinishQuiz()
        {
            IsStartEnabled = IsChooseQuizVisible = true;
            IsTipVisible = false;

            _stopwatch.Stop();
            _timer.Stop();

            foreach (var question in QuizQuestions)
            {
                if (question.SelectedAnswer == question.CorrectAnswer)
                    _score++;
            }

            QuestionScore = $"\nYou scored {_score}/{QuizQuestions.Count} points in {TimeElapsed}";
            
            NumberOfQuestion = Question = Answer1 = Answer2 = Answer3 = Answer4 = null;

            IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = IsNextEnabled = IsPreviousEnabled = false;
        }

        public RelayCommand Answer1Command => new(execute => Answer1Chose());
        public void Answer1Chose()
        {
            Answer1Disable();

            QuizQuestions[_questionNumber].SelectedAnswer = 1;
        }

        public RelayCommand Answer2Command => new(execute => Answer2Chose());
        public void Answer2Chose()
        {
            Answer2Disable();

            QuizQuestions[_questionNumber].SelectedAnswer = 2;
        }

        public RelayCommand Answer3Command => new(execute => Answer3Chose());
        public void Answer3Chose()
        {
            Answer3Disable();

            QuizQuestions[_questionNumber].SelectedAnswer = 3;
        }

        public RelayCommand Answer4Command => new(execute => Answer4Chose());
        public void Answer4Chose()
        {
            Answer4Disable();

            QuizQuestions[_questionNumber].SelectedAnswer = 4;
        }

        public RelayCommand NextCommand => new(execute => NextQuestion());
        public void NextQuestion()
        {
            switch (QuizQuestions[_questionNumber + 1].SelectedAnswer)
            {
                case 1:
                    Answer1Disable();
                    break;
                case 2:
                    Answer2Disable();
                    break;
                case 3:
                    Answer3Disable();
                    break;
                case 4:
                    Answer4Disable();
                    break;
            }

            IsPreviousEnabled = true;
            _questionNumber++;
            NumberOfQuestion = $"Question {_questionNumber + 1}/{QuizQuestions.Count}";
            SetQuestion(_questionNumber);
            SetAnswers(_questionNumber);

            if (QuizQuestions[_questionNumber].SelectedAnswer == 0)
                IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = true;

            if (_questionNumber + 1 == QuizQuestions.Count) 
                IsNextEnabled = false;
             
        }

        public RelayCommand PreviousCommand => new(execute => PreviousQuestion());
        public void PreviousQuestion()
        {
            switch (QuizQuestions[_questionNumber - 1].SelectedAnswer)
            {
                case 1:
                    Answer1Disable();
                    break;
                case 2:
                    Answer2Disable();
                    break;
                case 3:
                    Answer3Disable();
                    break;
                case 4:
                    Answer4Disable();
                    break;
            }

            IsNextEnabled = true;
            _questionNumber--;
            NumberOfQuestion = $"Question {_questionNumber + 1}/{QuizQuestions.Count}";
            SetQuestion(_questionNumber);
            SetAnswers(_questionNumber);

            if (QuizQuestions[_questionNumber].SelectedAnswer == 0)
                IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = true;

            if (_questionNumber == 0)
                IsPreviousEnabled = false;
        }

        #endregion Commands


        // Methods
        #region Methods

        private bool CanFinish()
        {
            foreach (var question in QuizQuestions)
            {
                if (question.SelectedAnswer == 0)
                    return false;
            }

            if (IsChooseQuizVisible)
                return false;

            return true;
        }

        private void SetQuestion(int questionNumber)
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

        private void Answer1Disable()
        {
            IsAnswer1Enabled = false;
            IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = true;
        }

        private void Answer2Disable()
        {
            IsAnswer2Enabled = false;
            IsAnswer1Enabled = IsAnswer3Enabled = IsAnswer4Enabled = true;
        }

        private void Answer3Disable()
        {
            IsAnswer3Enabled = false;
            IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer4Enabled = true;
        }

        private void Answer4Disable()
        {
            IsAnswer4Enabled = false;
            IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer3Enabled = true;
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

        private bool _isFinishEnabled = true;
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

        private string _isTipVisible = "Hidden";
        public bool IsTipVisible
        {
            get => _isTipVisible == "Visible";

            set
            {
                _isTipVisible = value ? "Visible" : "Hidden";
                OnPropertyChanged();
            }
        }

        #endregion


        // SelectedQuizIndex
        private int _selectedQuizIndex = -1;
        public int SelectedQuizIndex
        {
            get => _selectedQuizIndex;
            set
            {
                _selectedQuizIndex = value;
                OnPropertyChanged();
            }
        }

        private string _selectedQuiz;

        public string SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                _selectedQuiz = value;
                OnPropertyChanged();
            }
        }


    }
}
