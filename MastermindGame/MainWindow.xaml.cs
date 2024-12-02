using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace MastermindGame
{
    public partial class MainWindow : Window
    {
        private List<string> colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        private List<string> colorCode = new List<string>();
        // Variabele om bij te houden of debugmodus actief is
        private bool isDebugMode = false;
        private System.Windows.Threading.DispatcherTimer timer;
        private int elapsedTime = 0;
        public List<string> leaderboard = new List<string> { "Ahmed - 7 pogingen - 42/100", "Piet - 5 pogingen - 61/100","Senne - 8 pogingen - 17/100", "Suyen - 3 Pogingen - 88/100"};

        public MainWindow()
        {
            InitializeComponent();
            comboBox1.ItemsSource = colors;
            comboBox2.ItemsSource = colors;
            comboBox3.ItemsSource = colors;
            comboBox4.ItemsSource = colors;
            GenerateColorCode();
            UpdateWindowTitle();
            debugTextBox.Text = "";
        }
        // Variabele voor het bijhouden van pogingen
        private int attempts = 0;

        // Methode om de titel bij te werken
        private void UpdateWindowTitle()
        {
            this.Title = $"MasterMind - Poging {attempts + 1}";
        }
        // Methode om debugmodus te toggelen
        private void ToggleDebug()
        {
            isDebugMode = !isDebugMode; // Schakel debugmodus aan/uit

            // Stel de zichtbaarheid van de debug-TextBox in
            debugTextBox.Visibility = isDebugMode ? Visibility.Visible : Visibility.Collapsed;

            // Toon de gegenereerde code in de TextBox als debugmodus aan staat
            if (isDebugMode)
            {
                debugTextBox.Text = string.Join(", ", colorCode);
            }
        }
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.F12)
            {
                ToggleDebug(); // Schakel de debugmodus
            }
        }


        private void GenerateColorCode()
        {
            Random rand = new Random();
            colorCode = new List<string>
            {
                colors[rand.Next(colors.Count)],
                colors[rand.Next(colors.Count)],
                colors[rand.Next(colors.Count)],
                colors[rand.Next(colors.Count)]
            };

            this.Title = "MasterMind(" + string.Join(", ", colorCode) + ")";
            StartCountdown();
        }

        private SolidColorBrush GetBrushFromColorString(string colorString)
        {
            if (string.IsNullOrWhiteSpace(colorString))
            {
                return new SolidColorBrush(Colors.Gray);
            }

            try
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorString));
            }
            catch (FormatException)
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            if (combo.SelectedItem != null)
            {
                string selectedColor = combo.SelectedItem.ToString();
                SolidColorBrush brush = GetBrushFromColorString(selectedColor);

                if (combo == comboBox1)
                {
                    feedBack1.Background = brush;
                }
                else if (combo == comboBox2)
                {
                    feedBack2.Background = brush;
                }
                else if (combo == comboBox3)
                {
                    feedBack3.Background = brush;
                }
                else if (combo == comboBox4)
                {
                    feedBack4.Background = brush;
                }
            }
            else
            {
                SolidColorBrush defaultBrush = new SolidColorBrush(Colors.White);
                feedBack1.Background = defaultBrush;
                feedBack2.Background = defaultBrush;
                feedBack3.Background = defaultBrush;
                feedBack4.Background = defaultBrush;
            }
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            attempts++; // Verhoog het aantal pogingen
            UpdateWindowTitle(); // Werk de titel bij
            StartCountdown(); // Start de timer opnieuw
            var userCode = new List<string>
            {
                comboBox1.SelectedItem?.ToString() ?? "Red",
                comboBox2.SelectedItem?.ToString() ?? "Red",
                comboBox3.SelectedItem?.ToString() ?? "Red",
                comboBox4.SelectedItem?.ToString() ?? "Red"
            };

            if (userCode.Contains("Red") && (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null || comboBox3.SelectedItem == null || comboBox4.SelectedItem == null))
            {
                MessageBox.Show("Please select a color for all positions.");
                return;
            }

            CheckGuess(userCode);
        }


        private void CheckGuess(List<string> userCode)
        {
            feedBack1.BorderBrush = null;
            feedBack2.BorderBrush = null;
            feedBack3.BorderBrush = null;
            feedBack4.BorderBrush = null;

            bool[] matchedInCode = new bool[colorCode.Count];
            bool[] matchedInUser = new bool[userCode.Count];

            for (int i = 0; i < userCode.Count; i++)
            {
                if (userCode[i] == colorCode[i])
                {
                    SetFeedback(i, Colors.DarkRed);
                    matchedInCode[i] = true;
                    matchedInUser[i] = true;
                }
            }

            for (int i = 0; i < userCode.Count; i++)
            {
                if (!matchedInUser[i])
                {
                    for (int j = 0; j < colorCode.Count; j++)
                    {
                        if (!matchedInCode[j] && userCode[i] == colorCode[j])
                        {
                            SetFeedback(i, Colors.Wheat);
                            matchedInCode[j] = true;
                            break;
                        }
                    }
                }
            }
        }

        private void SetFeedback(int index, Color color)
        {
            switch (index)
            {
                case 0: feedBack1.BorderBrush = new SolidColorBrush(color); break;
                case 1: feedBack2.BorderBrush = new SolidColorBrush(color); break;
                case 2: feedBack3.BorderBrush = new SolidColorBrush(color); break;
                case 3: feedBack4.BorderBrush = new SolidColorBrush(color); break;
            }
        }
        // Methode om de timer opnieuw te starten
        private void StartCountdown()
        {
            // Stop de timer als deze al draait
            if (timer != null && timer.IsEnabled)
            {
                timer.Stop();
            }

            // Initialiseer de timer als deze nog niet bestaat
            if (timer == null)
            {
                timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;
            }

            // Reset de verstreken tijd en start de timer
            elapsedTime = 0;
            timer.Start();

            // Optioneel: Werk een label of andere UI-elementen bij als timer zichtbaar moet zijn
            UpdateTimerLabel();
        }
        // Methode die wordt uitgevoerd bij elke timer-tick
        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            UpdateTimerLabel(); // Werk de UI bij met de verstreken tijd
        }
        // Methode om de UI bij te werken met de verstreken tijd
        private void UpdateTimerLabel()
        {
            // Update de titel
            this.Title = $"MasterMind - Tijd: {elapsedTime} seconden";

            // Update het label in de UI
            timerLabel.Content = $"Tijd: {elapsedTime} seconden";
        }
        public string playerName = "";
        private void newPlayerBtn_Click(object sender, RoutedEventArgs e)
        {
            playerName = newPlayer.Text;
        }

        private void newGameBtn_Click(object sender, RoutedEventArgs e)
        {
            if (playerName == "")
            {
                MessageBox.Show($"Geef je naam in.", "Nieuw Spel", button: MessageBoxButton.OKCancel, default);
            }
            else
            {
                InitializeComponent();
                comboBox1.ItemsSource = colors;
                comboBox2.ItemsSource = colors;
                comboBox3.ItemsSource = colors;
                comboBox4.ItemsSource = colors;
                GenerateColorCode();
                UpdateWindowTitle();
                debugTextBox.Text = "";
            }
        }

        private void showHighScore_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{leaderboard[0]}\n{leaderboard[1]}\n{leaderboard[2]}\n{leaderboard[3]}","Mastermind HighScores",button:MessageBoxButton.OK);
        }

        private void closeAppMenuItem_Click(object sender, RoutedEventArgs e)
        {
           Application.Current.Shutdown();
        }

        private void countTry_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Aantal Pogingen:{}", "Aantal Pogingen", button: MessageBoxButton.OK);
        }
        //

    }
}
