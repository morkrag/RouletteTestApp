using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace RouletteApp
{
    public partial class MainWindow : Window
    {
        // Class functionality sumup: program boot, TCPListener boot, testing TCPClient (optional) boot, timers for notification show ups,
        // timer for statistics panel values update according to received by TCPListener request, results generation and log, roulette values manipulation, number obj creation
        //
        // Note: TCPClient should not be generated in this class logic wise, neither being manipulated. It is left here commented for ease of testing.

        private Roulette _roulette = new Roulette();
        private List<Result> _resultLog = new List<Result>();
        private List<TextBlock> _rouletteResultsDisplayBlock = new List<TextBlock> { };

        private DispatcherTimer _notificationPopUpTimer = new DispatcherTimer();

        private TCPController _tcpController = new TCPController();

        private int sameColorCounter = 1;

        private const bool _locaTcpClientEnabledForTesting = true; // change to true if you want to start local TCPClient module and test how UI acts in response to received by TCPListener request
        private TCPClient.TCPClient _tcpClient;

        public MainWindow()
        {
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            _tcpController.Stop();
            _tcpClient?.Disconnect();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _tcpController.AppropriateMessageReceived += _tcpController_AppropriateMessageReceived;
            _tcpController.Start();

            if (_locaTcpClientEnabledForTesting)
            {
                _tcpClient = new TCPClient.TCPClient();
            }
        }

        private void _tcpController_AppropriateMessageReceived(object? sender, JObject jsonToDisplay)
        {
            UpdateStatisticsIfRequired(jsonToDisplay);
        }

        private void AddRandomResult_Click(object sender, RoutedEventArgs e)
        {
            if (ResultE0.Visibility == Visibility.Visible)
            {
                MoveRouletteNumberResults();
                MoveRouletteMultiplierResults();
            }
            else
            {
                ResultE0.Visibility = Visibility.Visible;
            }

            _roulette.SpinBall();
            Number currentNumber = new Number(_roulette.CurrentPostion);

            CheckIfColorRepeated(currentNumber);
            GenerateResult(currentNumber.Value);

            ResultE0.Text = _roulette.CurrentPostion.ToString();

            MultiplierE0.Text = "x" + _resultLog.Last().Multiplier.ToString();
            HideMultiplierIfRequired(MultiplierE0);
            MultiplierE0.Background = ResultE0.Background;

            if (MultiplierE0.Text == "x1" || MultiplierE0.Text == "x0")
            {
                MultiplierE0.Visibility = Visibility.Collapsed;
            }
            else
            {
                MultiplierE0.Visibility = Visibility.Visible;
            }
            ChangeColorAccordingToNumber(ResultE0, currentNumber.Color);
        }

        private void MoveRouletteNumberResults()
        {
            TextBlock[] resultsElements = [ResultE9, ResultE8, ResultE7, ResultE6, ResultE5, ResultE4, ResultE3, ResultE2, ResultE1, ResultE0];

            for (int i = 0; i < resultsElements.Length - 1; i++)
            {
                TextBlock currentElement = resultsElements[i];
                TextBlock previousElement = resultsElements[i + 1];

                currentElement.Text = previousElement.Text;
                currentElement.Background = previousElement.Background;
                MakeNumberVisibleIfRequired(currentElement, previousElement);
            }
        }

        private void MoveRouletteMultiplierResults()
        {
            TextBlock[] multiplierElements = [MultiplierE9, MultiplierE8, MultiplierE7, MultiplierE6, MultiplierE5, MultiplierE4, MultiplierE3, MultiplierE2, MultiplierE1, MultiplierE0];

            for (int i = 0; i < multiplierElements.Length - 1; i++)
            {
                TextBlock currentElement = multiplierElements[i];
                TextBlock previousElement = multiplierElements[i + 1];

                currentElement.Text = previousElement.Text;
                currentElement.Background = previousElement.Background;
                MakeNumberVisibleIfRequired(currentElement, previousElement);
                HideMultiplierIfRequired(currentElement);
            }
        }

        private void ChangeColorAccordingToNumber(TextBlock x, string numberColor)
        {
            if (numberColor == "green")
            {
                x.Background = Brushes.Green;
            } else if (numberColor == "red")
            {
                x.Background = Brushes.Red;
            } else
            {
                x.Background = Brushes.Gray;
            }
        }

        private void MakeNumberVisibleIfRequired(TextBlock current, TextBlock previous)
        {
            if (previous.Text != "")
            {
                current.Visibility = Visibility.Visible;
            }
        }

        private void HideMultiplierIfRequired(TextBlock multiplier)
        {
            if (multiplier.Text == "x1" || multiplier.Text == "x0")
            {
                multiplier.Visibility = Visibility.Collapsed;
            }
        }

        private void GenerateResult(int number)
        {
            Result resultToLog = new Result(number, sameColorCounter);

            _resultLog.Add(resultToLog);
        }

        private void CheckIfColorRepeated(Number x)
        {
            if (_resultLog.Count > 0)
            {
                Number y = new Number(_resultLog[^1].RoulettePosition);

                if (y.Color == x.Color)
                {
                    sameColorCounter++;
                } else
                {
                    sameColorCounter = 1;
                }
            }
        }

        private void ShowNotificationResult_Click(object sender, RoutedEventArgs e)
        {
            if (!_notificationPopUpTimer.IsEnabled)
            {
                _notificationPopUpTimer.Tick += new EventHandler(timer_Tick);
                _notificationPopUpTimer.Interval = new TimeSpan(0, 0, 5);
                Notification_StackPanel.Visibility = Visibility.Visible;
                _notificationPopUpTimer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Notification_StackPanel.Visibility = Visibility.Collapsed;

            _notificationPopUpTimer.IsEnabled = false;
        }

        private void UpdateHighestMultiplierCount(int highhestMultiplierReceived)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                HighestMultiplier_TextBlock.Text = "x" + highhestMultiplierReceived.ToString();
            });
        }

        private void UpdateActivePlayersCount(int activePlayersCount)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ActivePlayers_TextBlock.Text = activePlayersCount.ToString();
            });
        }

        private void UpdateStatisticsIfRequired(JObject jsonToDisplay)
        {
            if (jsonToDisplay.Count == 2)
            {
                UpdateActivePlayersCount((int)jsonToDisplay["activePlayers"]);
                UpdateHighestMultiplierCount((int)jsonToDisplay["biggestMultiplier"]);
            }
            else
            {
                try
                {
                    UpdateActivePlayersCount((int)jsonToDisplay["activePlayers"]);
                } catch (Exception)
                {
                    UpdateHighestMultiplierCount((int)jsonToDisplay["biggestMultiplier"]);
                }
            }
        }
    }
}