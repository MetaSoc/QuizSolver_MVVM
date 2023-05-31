using QuizSolver.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using static System.Windows.Application;

namespace QuizSolver.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        // Constructor
        public MainViewModel()
        {
            // LoadAllQuizzes
            QuizzesList = new ObservableCollection<string>(DataAccess.LoadQuizzesList().OrderBy(i => i));
        }
        

        // Relay Methods
        #region Relay Methods

        private void StartQuiz()
        {
            _questionNumber = 0;
            _score = 0;

            QuizQuestions = DataAccess.LoadQuestions(SelectedQuiz);

            // Validations
            #region Validations

            // Empty Quiz Validation
            if (QuizQuestions.Count == 0)
            {
                MessageBox.Show($"Quiz \"{SelectedQuiz}\" is empty.\nPlease select another quiz.", "Empty Quiz", MessageBoxButton.OK, MessageBoxImage.Warning);

                QuizzesList.Remove(SelectedQuiz);
                SelectedQuiz = null;
                return;
            }

            // Proper Quiz Validation
            if (QuizQuestions[0].Question is null || QuizQuestions[0].Answer1 is null || QuizQuestions[0].Answer2 is null ||
                QuizQuestions[0].Answer3 is null || QuizQuestions[0].Answer4 is null || QuizQuestions[0].CorrectAnswer is 0)
            {
                MessageBox.Show($"Quiz \"{SelectedQuiz}\" isn't made with our QuizMaker\nand cannot be loaded.\nPlease select another quiz.", "Invalid Quiz", MessageBoxButton.OK, MessageBoxImage.Warning);

                QuizzesList.Remove(SelectedQuiz);
                SelectedQuiz = null;
                return;
            }
            
            // Proper Encryption Validation
            try
            {
                SetQuestion();
                SetAnswers();
            }
            catch (Exception)
            {
                MessageBox.Show($"Quiz \"{SelectedQuiz}\" isn't made with our QuizMaker\nand cannot be loaded.\nPlease select another quiz.", "Invalid Encription", MessageBoxButton.OK, MessageBoxImage.Warning);

                QuizzesList.Remove(SelectedQuiz);
                SelectedQuiz = null;
                return;
            }

            // One-Question Quiz Validation
            if (QuizQuestions.Count > 1)
                IsNextEnabled = true;

            #endregion

            SelectedQuiz = null;

            IsStartEnabled = IsChooseQuizVisible = false;
            IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = IsTipVisible = true;

            QuestionScoreText = "Question:";

            SetNumberOfQuestion();
            StartTimer();
        }
        private void FinishQuiz()
        {
            StopTimer();

            IsStartEnabled = IsChooseQuizVisible = true;
            IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = 
                IsNextEnabled = IsPreviousEnabled = IsTipVisible = false;

            // Calculating Score
            foreach (var _ in QuizQuestions.Where(question => question.SelectedAnswer == question.CorrectAnswer))
                _score++;

            QuestionScoreText = $"You scored {_score}/{QuizQuestions.Count} points in {TimeElapsed}";

            NumberOfQuestion = Question = Answer1 = Answer2 = Answer3 = Answer4 = null;
        }
        private void PreviousQuestion()
        {
            // Selecting Answer
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

            _questionNumber--;
            IsNextEnabled = true;

            SetNumberOfQuestion();

            SetQuestion();
            SetAnswers();

            IsAnswerSelected();

            if (_questionNumber == 0)
                IsPreviousEnabled = false;
        }
        private void NextQuestion()
        {
            // Selecting Answer
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

            _questionNumber++;
            IsPreviousEnabled = true;

            SetNumberOfQuestion();

            SetQuestion();
            SetAnswers();

            IsAnswerSelected();

            if (_questionNumber + 1 == QuizQuestions.Count)
                IsNextEnabled = false;

        }
        private void Answer1Chose()
        {
            Answer1Disable();
            QuizQuestions[_questionNumber].SelectedAnswer = 1;
        }
        private void Answer2Chose()
        {
            Answer2Disable();
            QuizQuestions[_questionNumber].SelectedAnswer = 2;
        }
        private void Answer3Chose()
        {
            Answer3Disable();
            QuizQuestions[_questionNumber].SelectedAnswer = 3;
        }
        private void Answer4Chose()
        {
            Answer4Disable();
            QuizQuestions[_questionNumber].SelectedAnswer = 4;
        }

        #endregion


        // Relay Commands
        #region RelayCommands

        public RelayCommand StartCommand => new(execute => StartQuiz(), canExecute => SelectedQuiz is not null);
        public RelayCommand FinishCommand => new(execute => FinishQuiz(), canExecute => CanFinish());
        public RelayCommand Answer1Command => new(execute => Answer1Chose());
        public RelayCommand Answer2Command => new(execute => Answer2Chose());
        public RelayCommand Answer3Command => new(execute => Answer3Chose());
        public RelayCommand Answer4Command => new(execute => Answer4Chose());
        public RelayCommand PreviousCommand => new(execute => PreviousQuestion());
        public RelayCommand NextCommand => new(execute => NextQuestion());

        #endregion Commands


        // Methods
        #region Methods

        private bool CanFinish()
        {
            if (QuizQuestions.Any(question => question.SelectedAnswer == 0))
                return false;

            return !IsChooseQuizVisible;
        }
        private void SetQuestion()
        {
            Question = Decription.Decript(QuizQuestions[_questionNumber].Question);
        }
        private void SetAnswers()
        {
            Answer1 = Decription.Decript(QuizQuestions[_questionNumber].Answer1);
            Answer2 = Decription.Decript(QuizQuestions[_questionNumber].Answer2);
            Answer3 = Decription.Decript(QuizQuestions[_questionNumber].Answer3);
            Answer4 = Decription.Decript(QuizQuestions[_questionNumber].Answer4);
        }
        private void SetNumberOfQuestion()
        {
            NumberOfQuestion = $"Question {_questionNumber + 1}/{QuizQuestions.Count}";
        }
        private void IsAnswerSelected()
        {
            if (QuizQuestions[_questionNumber].SelectedAnswer == 0)
                IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = true;
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
        private void StartTimer()
        {
            _stopwatch = new Stopwatch();
            _timer = new Timer(1000);

            TimeElapsed = "00:00";

            _timer.Elapsed += TimerTick;

            _stopwatch.Start();
            _timer.Start();
        }
        private void StopTimer()
        {
            _stopwatch.Stop();
            _timer.Stop();
        }
        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            Current.Dispatcher.Invoke(() => { TimeElapsed = _stopwatch.Elapsed.ToString(@"mm\:ss"); });
        }

        #endregion


        // Properites
        #region Properties

        private List<Quiz> QuizQuestions { get; set; } = new();
        public ObservableCollection<string> QuizzesList { get; set; }

        private int _questionNumber;
        private int _score;
        private Stopwatch _stopwatch;
        private Timer _timer;

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

        private string _questionScoreText;
        public string QuestionScoreText
        {
            get => _questionScoreText;
            set
            {
                _questionScoreText = value;
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

    }
}
