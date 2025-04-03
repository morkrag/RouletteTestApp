using System.Media;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace RouletteApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Roulette _roulette = new Roulette();
        private List<Result> _resultLog = new List<Result>();
        private TextBlock[] rouletteResultsDisplayBlock = new TextBlock[10];
        private DispatcherTimer notificationPopUpTimer = new DispatcherTimer();
        private DispatcherTimer tcpRequestCheckTimer = new DispatcherTimer();

        private TCPListenerTCPClientController ServerClient_Controller = new TCPListenerTCPClientController();

        private int sameColorCounter = 1;

        public MainWindow()
        {
            //Thread TCPthread = new Thread(() =>
            //{
            //    lock (ServerClient_Controller)
            //    {
                    ServerClient_Controller.RunServerAndClient();
            //    }
            //});
            //TCPthread.Start();
            InitializeComponent();
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
            Number currentNumber = new Number(_roulette._currentPostion);
            CheckIfColorRepeated(currentNumber);
            GenerateResult(currentNumber._value);
            ResultE0.Text = _roulette._currentPostion.ToString();
            MultiplierE0.Text = "x" + _resultLog.Last()._multiplier.ToString();
            MultiplierE0.Background = ResultE0.Background;
            //tcpServer.Start()
            if (MultiplierE0.Text != "x1")
            {
                MultiplierE0.Visibility = Visibility.Visible;
            }
            else
            {
                MultiplierE0.Visibility = Visibility.Collapsed;
            }
            ChangeColorAccordingToNumber(ResultE0, currentNumber._color);
        }

        private void MoveRouletteNumberResults()
        {
            ResultE9.Text = ResultE8.Text;
            ResultE9.Background = ResultE8.Background;
            MakeNumberVisibleIfRequired(ResultE9, ResultE8);

            ResultE8.Text = ResultE7.Text;
            ResultE8.Background = ResultE7.Background;
            MakeNumberVisibleIfRequired(ResultE8, ResultE7);

            ResultE7.Text = ResultE6.Text;
            ResultE7.Background = ResultE6.Background;
            MakeNumberVisibleIfRequired(ResultE7, ResultE6);

            ResultE6.Text = ResultE5.Text;
            ResultE6.Background = ResultE5.Background;
            MakeNumberVisibleIfRequired(ResultE6, ResultE5);

            ResultE5.Text = ResultE4.Text;
            ResultE5.Background = ResultE4.Background;
            MakeNumberVisibleIfRequired(ResultE5, ResultE4);

            ResultE4.Text = ResultE3.Text;
            ResultE4.Background = ResultE3.Background;
            MakeNumberVisibleIfRequired(ResultE4, ResultE3);

            ResultE3.Text = ResultE2.Text;
            ResultE3.Background = ResultE2.Background;
            MakeNumberVisibleIfRequired(ResultE3, ResultE2);

            ResultE2.Text = ResultE1.Text;
            ResultE2.Background = ResultE1.Background;
            MakeNumberVisibleIfRequired(ResultE2, ResultE1);

            ResultE1.Text = ResultE0.Text;
            ResultE1.Background = ResultE0.Background;
            MakeNumberVisibleIfRequired(ResultE1, ResultE0);
        }

        private void MoveRouletteMultiplierResults()
        {
            MultiplierE9.Text = MultiplierE8.Text;
            MultiplierE9.Background = MultiplierE8.Background;
            MakeNumberVisibleIfRequired(MultiplierE9, MultiplierE8);
            HideMultiplierIfRequired(MultiplierE9);

            MultiplierE8.Text = MultiplierE7.Text;
            MultiplierE8.Background = MultiplierE7.Background;
            MakeNumberVisibleIfRequired(MultiplierE8, MultiplierE7);
            HideMultiplierIfRequired(MultiplierE8);

            MultiplierE7.Text = MultiplierE6.Text;
            MultiplierE7.Background = MultiplierE6.Background;
            MakeNumberVisibleIfRequired(MultiplierE7, MultiplierE6);
            HideMultiplierIfRequired(MultiplierE7);

            MultiplierE6.Text = MultiplierE5.Text;
            MultiplierE6.Background = MultiplierE5.Background;
            MakeNumberVisibleIfRequired(MultiplierE6, MultiplierE5);
            HideMultiplierIfRequired(MultiplierE6);

            MultiplierE5.Text = MultiplierE4.Text;
            MultiplierE5.Background = MultiplierE4.Background;
            MakeNumberVisibleIfRequired(MultiplierE5, MultiplierE4);
            HideMultiplierIfRequired(MultiplierE5);

            MultiplierE4.Text = MultiplierE3.Text;
            MultiplierE4.Background = MultiplierE3.Background;
            MakeNumberVisibleIfRequired(MultiplierE4, MultiplierE3);
            HideMultiplierIfRequired(MultiplierE4);

            MultiplierE3.Text = MultiplierE2.Text;
            MultiplierE3.Background = MultiplierE2.Background;
            MakeNumberVisibleIfRequired(MultiplierE3, MultiplierE2);
            HideMultiplierIfRequired(MultiplierE3);

            MultiplierE2.Text = MultiplierE1.Text;
            MultiplierE2.Background = MultiplierE1.Background;
            MakeNumberVisibleIfRequired(MultiplierE2, MultiplierE1);
            HideMultiplierIfRequired(MultiplierE2);

            MultiplierE1.Text = MultiplierE0.Text;
            MultiplierE1.Background = MultiplierE0.Background;
            MakeNumberVisibleIfRequired(MultiplierE1, MultiplierE0);
            HideMultiplierIfRequired(MultiplierE1);
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
            if (multiplier.Text == "x1")
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
                Number y = new Number(_resultLog[^1]._roulettePosition);
                if (y._color == x._color)
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
            if (!notificationPopUpTimer.IsEnabled)
            {
                notificationPopUpTimer.Tick += new EventHandler(timer_Tick);
                notificationPopUpTimer.Interval = new TimeSpan(0, 0, 5);
                Notification_StackPanel.Visibility = Visibility.Visible;
                notificationPopUpTimer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Notification_StackPanel.Visibility = Visibility.Collapsed;
            notificationPopUpTimer.IsEnabled = false;
        }

        private void AdjustActivePlayerCount(int playerCountReceived)
        {
            ActivePlayersCount_TextBlock.Text = playerCountReceived.ToString();
        }

        private void AdjustHighestMultiplierCount(int highhestMultiplierReceived)
        {
            HighestMultiplier_TextBlock.Text = highhestMultiplierReceived.ToString();
        }

        private void UpdateStatisticsIfRequired(TCPListenerTCPClientController controller)
        {
            if (controller.jsonToPassDown.ContainsKey("activePlayers") || controller.jsonToPassDown.ContainsKey("biggestMultiplier"))
            {
                AdjustActivePlayerCount((int)ServerClient_Controller.jsonToPassDown["activePlayers"]);
                AdjustHighestMultiplierCount((int)ServerClient_Controller.jsonToPassDown["biggestMultiplier"]);
            }
        }

        //private void StatisticsUpdateTimer()
        //{
        //    tcpRequestCheckTimer.Tick += new EventHandler(tcpRequestCheckTimer_Tick);
        //    tcpRequestCheckTimer.Interval = new TimeSpan(0, 0, 5);
        //    Notification_StackPanel.Visibility = Visibility.Visible;
        //}

        //private void tcpRequestCheckTimer_Tick(object sender, EventArgs e)
        //{
        //    UpdateStatisticsIfRequired(ServerClient_Controller);
        //}
    }
}