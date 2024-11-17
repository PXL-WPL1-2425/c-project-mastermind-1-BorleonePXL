using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MastermindGame
{
    public partial class MainWindow : Window
    {
        private List<string> colors = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        private List<string> colorCode = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            comboBox1.ItemsSource = colors;
            comboBox2.ItemsSource = colors;
            comboBox3.ItemsSource = colors;
            comboBox4.ItemsSource = colors;
            GenerateColorCode();
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

//        private void CheckGuess(List<string> userCode)
//        {
//            feedBack1.BorderBrush = null;
//            feedBack2.BorderBrush = null;
//            feedBack3.BorderBrush = null;
//            feedBack4.BorderBrush = null;

//            bool[] matchedInCode = new bool[colorCode.Count];
//            bool[] matchedInUser = new bool[userCode.Count];

//            for (int i = 0; i < userCode.Count; i++)
//            {
//                if (userCode[i] == colorCode[i])
//                {
//                    SetFeedback(i, Colors.DarkRed);
//                    matchedInCode[i] = true;
//                    matchedInUser[i] = true;
//                }
//            }

//            for (int i = 0; i < userCode.Count; i++)
//            {
//                if (!matchedInUser[i])
//                {
//                    for (int j = 0; j < colorCode.Count; j++)
//                    {
//                        if (!matchedInCode[j] && userCode[i] == colorCode[j])
//                        {
//                            SetFeedback(i, Colors.Wheat);
//                            matchedInCode[j] = true;
//                            break;
//                        }
//                    }
//                }
//            }
//        }

//        private void SetFeedback(int index, Color color)
//        {
//            switch (index)
//            {
//                case 0: feedBack1.BorderBrush = new SolidColorBrush(color); break;
//                case 1: feedBack2.BorderBrush = new SolidColorBrush(color); break;
//                case 2: feedBack3.BorderBrush = new SolidColorBrush(color); break;
//                case 3: feedBack4.BorderBrush = new SolidColorBrush(color); break;
//            }
//        }
//    }
//}
