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
            IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = 
                IsTipVisible = IsPreviousVisible = IsNextVisible = IsAnswerAbleToClick = true;

            QuestionScoreText = "Question:";

            DefaultButtons();

            SetQuestion();
            SetAnswers();

            SetNumberOfQuestion();
            StartTimer();
        }
        private void FinishQuiz()
        {
            StopTimer();

            IsStartEnabled = IsChooseQuizVisible = true;
            IsNextEnabled = IsPreviousEnabled = IsTipVisible = IsPreviousVisible = IsNextVisible = IsAnswerAbleToClick = false;

            // Calculating Score
            foreach (var _ in QuizQuestions.Where(question => question.SelectedAnswer == question.CorrectAnswer))
                _score++;

            QuestionScoreText = $"You scored {_score}/{QuizQuestions.Count} points in {TimeElapsed}";

            // Overview
            #region Overview
            
            _questionNumber = 0;

            OverviewSelectChoseAnswerLoad();
            OverviewShowAnswer();

            IsAnswer1Enabled = IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = true;

            if (QuizQuestions.Count > 1)
                IsOverviewNextEnabled = true;

            #endregion

        }
        private void PreviousQuestion()
        {
            _questionNumber--;

            SelectAnswerLoad();

            IsNextEnabled = true;

            if (_questionNumber == 0)
                IsPreviousEnabled = false;
        }
        private void NextQuestion()
        {
            _questionNumber++;
            
            SelectAnswerLoad();

            IsPreviousEnabled = true;

            if (_questionNumber + 1 == QuizQuestions.Count)
                IsNextEnabled = false;

        }
        private void OverviewPreviousQuestion()
        {
            _questionNumber--;
            IsOverviewNextEnabled = true;

            OverviewSelectChoseAnswerLoad();
            OverviewShowAnswer();

            if (_questionNumber == 0)
                IsOverviewPreviousEnabled = false;
        }
        private void OverviewNextQuestion()
        {
            _questionNumber++;
            IsOverviewPreviousEnabled = true;

            OverviewSelectChoseAnswerLoad();
            OverviewShowAnswer();

            if (_questionNumber + 1 == QuizQuestions.Count)
                IsOverviewNextEnabled = false;
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
        public RelayCommand OverviewPreviousCommand => new(execute => OverviewPreviousQuestion());
        public RelayCommand OverviewNextCommand => new(execute => OverviewNextQuestion());

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
        private void DefaultButtons()
        {
            Answer1Brush = Answer2Brush = Answer3Brush = Answer4Brush = "#707070";
            Answer1Thickness = Answer2Thickness = Answer3Thickness = Answer4Thickness = 1;
            Answer1ChoseBg = Answer2ChoseBg = Answer3ChoseBg = Answer4ChoseBg = "#363636";
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
        private void SelectAnswerLoad()
        {
            // Selecting Answer
            switch (QuizQuestions[_questionNumber].SelectedAnswer)
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

            SetNumberOfQuestion();

            SetQuestion();
            SetAnswers();

            IsAnswerSelected();
        }
        private void Answer1OverviewChose()
        {
            Answer1ChoseBg = "#232323";
            Answer2ChoseBg = Answer3ChoseBg = Answer4ChoseBg = "#363636";

            Answer1Brush = "DarkRed";
            Answer2Brush = Answer3Brush = Answer4Brush = "#646464";
        }
        private void Answer2OverviewChose()
        {
            Answer2ChoseBg = "#232323";
            Answer1ChoseBg = Answer3ChoseBg = Answer4ChoseBg = "#363636";

            Answer2Brush = "DarkRed";
            Answer1Brush = Answer3Brush = Answer4Brush = "#646464";
        }
        private void Answer3OverviewChose()
        {
            Answer3ChoseBg = "#232323";
            Answer1ChoseBg = Answer2ChoseBg = Answer4ChoseBg = "#363636";

            Answer3Brush = "DarkRed";
            Answer1Brush = Answer2Brush = Answer4Brush = "#646464";
        }
        private void Answer4OverviewChose()
        {
            Answer4ChoseBg = "#232323";
            Answer1ChoseBg = Answer2ChoseBg = Answer3ChoseBg = "#363636";

            Answer4Brush = "DarkRed";
            Answer1Brush = Answer2Brush = Answer3Brush = "#646464";
        }
        private void Answer1Correct()
        {
            Answer1Thickness = 5;
            Answer2Thickness = Answer3Thickness = Answer4Thickness = 1;

            Answer1Brush = "DarkGreen";

            if (Answer2Brush == "DarkRed")
            {
                Answer2Thickness = 5;
                Answer3Brush = Answer4Brush = "#646464";
            }
            if (Answer3Brush == "DarkRed")
            {
                Answer3Thickness = 5;
                Answer2Brush = Answer4Brush = "#646464";
            }
            if (Answer4Brush == "DarkRed")
            {
                Answer4Thickness = 5;
                Answer2Brush = Answer3Brush = "#646464";
            }
        }
        private void Answer2Correct()
        {
            Answer2Thickness = 5;
            Answer1Thickness = Answer3Thickness = Answer4Thickness = 1;

            Answer2Brush = "DarkGreen";

            if (Answer1Brush == "DarkRed")
            {
                Answer1Thickness = 5;
                Answer3Brush = Answer4Brush = "#646464";
            }
            if (Answer3Brush == "DarkRed")
            {
                Answer3Thickness = 5;
                Answer1Brush = Answer4Brush = "#646464";
            }
            if (Answer4Brush == "DarkRed")
            {
                Answer4Thickness = 5;
                Answer1Brush = Answer3Brush = "#646464";
            }
        }
        private void Answer3Correct()
        {
            Answer3Thickness = 5;
            Answer1Thickness = Answer2Thickness = Answer4Thickness = 1;

            Answer3Brush = "DarkGreen";

            if (Answer1Brush == "DarkRed")
            {
                Answer1Thickness = 5;
                Answer2Brush = Answer4Brush = "#646464";
            }
            if (Answer2Brush == "DarkRed")
            {
                Answer2Thickness = 5;
                Answer1Brush = Answer4Brush = "#646464";
            }
            if (Answer4Brush == "DarkRed")
            {
                Answer4Thickness = 5;
                Answer1Brush = Answer1Brush = "#646464";
            }
        }
        private void Answer4Correct()
        {
            Answer4Thickness = 5;
            Answer1Thickness = Answer2Thickness = Answer3Thickness = 1;

            Answer4Brush = "DarkGreen";

            if (Answer1Brush == "DarkRed")
            {
                Answer1Thickness = 5;
                Answer2Brush = Answer3Brush = "#646464";
            }
            if (Answer2Brush == "DarkRed")
            {
                Answer2Thickness = 5;
                Answer1Brush = Answer3Brush = "#646464";
            }
            if (Answer3Brush == "DarkRed")
            {
                Answer3Thickness = 5;
                Answer1Brush = Answer2Brush = "#646464";
            }
        }
        private void OverviewSelectChoseAnswerLoad()
        {
            // Overview Selecting Chose Answer
            switch (QuizQuestions[_questionNumber].SelectedAnswer)
            {
                case 1:
                    Answer1OverviewChose();
                    break;
                case 2:
                    Answer2OverviewChose();
                    break;
                case 3:
                    Answer3OverviewChose();
                    break;
                case 4:
                    Answer4OverviewChose();
                    break;
            }

            SetNumberOfQuestion();

            SetQuestion();
            Question = "OVERVIEW: " + Question;
            SetAnswers();

            IsAnswerSelected();
        }
        private void OverviewShowAnswer()
        {
            switch (QuizQuestions[_questionNumber].CorrectAnswer)
            {
                case 1:
                    Answer1Correct();
                    break;
                case 2:
                    Answer2Correct();
                    break;
                case 3:
                    Answer3Correct();
                    break;
                case 4:
                    Answer4Correct();
                    break;
            }
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
                Question = Answer1 = Answer2 = Answer3 = Answer4 = 
                    NumberOfQuestion = QuestionScoreText = TimeElapsed = null;

                IsOverviewPreviousEnabled = IsOverviewNextEnabled = IsAnswer1Enabled = 
                    IsAnswer2Enabled = IsAnswer3Enabled = IsAnswer4Enabled = false;
                
                Answer1Thickness = Answer2Thickness = 
                        Answer3Thickness = Answer4Thickness = 1;

                _selectedQuiz = value;
                OnPropertyChanged();
            }
        }

        #endregion


        // Overview Brush Properties
        #region Overview Brush Properties

        private string _answer1Brush = "#707070";
        public string Answer1Brush
        {
            get => _answer1Brush;
            set
            {
                _answer1Brush = value;
                OnPropertyChanged();
            }
        }

        private string _answer2Brush = "#707070";
        public string Answer2Brush
        {
            get => _answer2Brush;
            set
            {
                _answer2Brush = value;
                OnPropertyChanged();
            }
        }

        private string _answer3Brush = "#707070";
        public string Answer3Brush
        {
            get => _answer3Brush;
            set
            {
                _answer3Brush = value;
                OnPropertyChanged();
            }
        }

        private string _answer4Brush = "#707070";
        public string Answer4Brush
        {
            get => _answer4Brush;
            set
            {
                _answer4Brush = value;
                OnPropertyChanged();
            }
        }

        #endregion


        // Overview Thickness Properties
        #region Overview Thickness Properties

        private int _answer1Thickness = 1;
        public int Answer1Thickness
        {
            get => _answer1Thickness;
            set
            {
                _answer1Thickness = value;
                OnPropertyChanged();
            }
        }

        private int _answer2Thickness = 1;
        public int Answer2Thickness
        {
            get => _answer2Thickness;
            set
            {
                _answer2Thickness = value;
                OnPropertyChanged();
            }
        }

        private int _answer3Thickness = 1;
        public int Answer3Thickness
        {
            get => _answer3Thickness;
            set
            {
                _answer3Thickness = value;
                OnPropertyChanged();
            }
        }

        private int _answer4Thickness = 1;
        public int Answer4Thickness
        {
            get => _answer4Thickness;
            set
            {
                _answer4Thickness = value;
                OnPropertyChanged();
            }
        }

        #endregion


        // Overview Background Properties
        #region Overview Background Properties

        private string _answer1ChoseBg = "#363636";
        public string Answer1ChoseBg
        {
            get => _answer1ChoseBg;
            set
            {
                _answer1ChoseBg = value;
                OnPropertyChanged();
            }
        }

        private string _answer2ChoseBg = "#363636";
        public string Answer2ChoseBg
        {
            get => _answer2ChoseBg;
            set
            {
                _answer2ChoseBg = value;
                OnPropertyChanged();
            }
        }

        private string _answer3ChoseBg = "#363636";
        public string Answer3ChoseBg
        {
            get => _answer3ChoseBg;
            set
            {
                _answer3ChoseBg = value;
                OnPropertyChanged();
            }
        }

        private string _answer4ChoseBg = "#363636";
        public string Answer4ChoseBg
        {
            get => _answer4ChoseBg;
            set
            {
                _answer4ChoseBg = value;
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

        private bool _isOverviewPreviousEnabled = false;
        public bool IsOverviewPreviousEnabled
        {
            get => _isOverviewPreviousEnabled;
            set
            {
                _isOverviewPreviousEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isOverviewNextEnabled = false;
        public bool IsOverviewNextEnabled
        {
            get => _isOverviewNextEnabled;
            set
            {
                _isOverviewNextEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isAnswerAbleToClick = true;
        public bool IsAnswerAbleToClick
        {
            get => _isAnswerAbleToClick;
            set
            {
                _isAnswerAbleToClick = value;
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

        private string _isPreviousVisible = "Visible";
        public bool IsPreviousVisible
        {
            get => _isPreviousVisible == "Visible";

            set
            {
                _isPreviousVisible = value ? "Visible" : "Hidden";
                OnPropertyChanged();
            }
        }

        private string _isNextVisible = "Visible";
        public bool IsNextVisible
        {
            get => _isNextVisible == "Visible";

            set
            {
                _isNextVisible = value ? "Visible" : "Hidden";
                OnPropertyChanged();
            }
        }

        #endregion

    }
}
