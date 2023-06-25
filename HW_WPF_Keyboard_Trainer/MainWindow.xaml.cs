using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HW_WPF_Keyboard_Trainer
{
    public partial class MainWindow : Window
    {
        static int MaxSeconds = 30;
        static string symbols = "QWERTYUIOPASDFGHJKLZXCVBNM[];'`1234567890-+./";  
        static Random r = new Random();
        static Timer t = new Timer();
        static Brush brush = new SolidColorBrush(Color.FromArgb(0xFF, 0xDD, 0xDD, 0xDD)); 

        bool isEnd = false;
        string nameKey = "";  
        string lastClick = ""; 
        int indInputSymbols = 0; 
        int amountWords = 0; 
        int amountFails = 0; 
        int totalSec = 0;

        List<Button> buttons = new List<Button>();  
        List<Button> clickBtns = new List<Button>(); 

        public MainWindow()
        {
            InitializeComponent();
            GetAllButtons();
            StartSettingsTimer();
        }


        private void StartSettingsTimer()
        {
            t.Interval = 1000;
            t.Elapsed += T_Elapsed;
        }

        private void T_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if ((totalSec++) >= MaxSeconds - 1)
            {
                UserMessage("You lose");
            }
        }


        private void GetAllButtons()
        {
            object obj = FindName("allBtn");
            try
            {
                if (obj != null)
                {
                    foreach (UIElement panel in ((Grid)obj).Children) 
                    {
                        foreach (UIElement grid in ((StackPanel)panel).Children) 
                        {
                            foreach (UIElement btn in ((Grid)grid).Children)
                            {
                                buttons.Add((Button)btn);
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private bool GetNameKey(KeyEventArgs e)
        {        
            nameKey = e.Key + "";

            if (nameKey == "Back") nameKey = "Backspace";
            else if (nameKey == "Capital") nameKey = "Caps Lock";
            else if (nameKey == "Oem3") nameKey = "`";
            else if (nameKey == "OemMinus") nameKey = "-";
            else if (nameKey == "OemPlus") nameKey = "+";
            else if (nameKey == "Oem6") nameKey = "]";
            else if (nameKey == "Oem1") nameKey = ";";
            else if (nameKey == "OemOpenBrackets") nameKey = "[";
            else if (nameKey == "OemQuotes") nameKey = "'";
            else if (nameKey == "OemQuestion") nameKey = "/";
            else if (nameKey == "OemComma") nameKey = ",";
            else if (nameKey == "OemPeriod") nameKey = ".";
            else if (nameKey == "LWin") nameKey = "Win";
            else if (nameKey == "Return") nameKey = "Enter";
            else if (nameKey.Length == 2 && nameKey[0] == 'D') nameKey = nameKey.Replace("D", "");
            else if (nameKey == "LeftCtrl")
            {
                ClickBtn((Button)FindName("leftCtrl"));
                return false;
            }
            else if (nameKey == "RightCtrl")
            {
                ClickBtn((Button)FindName("rightCtrl"));
                return false;
            }
            else if (nameKey == "RightShift")
            {
                ClickBtn((Button)FindName("rightShift"));
                return false;
            }
            else if (nameKey == "LeftShift")
            {
                ClickBtn((Button)FindName("leftShift"));
                return false;
            }
            else if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
                ClickBtn((Button)FindName("leftAlt"));
                return false;
            }
            else if (Keyboard.IsKeyDown(Key.RightAlt))
            {
                ClickBtn((Button)FindName("rightAlt"));
                return false;
            }
            return true;
        }


        private void ClickBtn(Button btn)
        {
            lastClick = btn.Content + "";
            clickBtns.Add(btn);
            btn.Background = Brushes.Red;
        }

        private void UserInput()
        {
            try
            {
                WrapPanel panelBtn = (WrapPanel)FindName("panelBtn");
                Button b = (Button)panelBtn.Children[indInputSymbols];

                if (nameKey == b.Content.ToString())
                {
                    indInputSymbols++;
                    b.Background = Brushes.Green;
                    if (indInputSymbols == panelBtn.Children.Count)
                    {
                        isEnd = true;
                    }
                }
                else
                {
                    UpdateFails(++amountFails);
                }
            }
            catch (Exception) { }
        }

        private void CreateBtn(string text)
        {
            try
            {
                Button btn = new Button() { Content = text, Padding = new Thickness(8)};
                ((WrapPanel)FindName("panelBtn")).Children.Add(btn);
            }
            catch (Exception) { }
        }

        private void GenerateText()
        {
            int rInd;
            for (int i = 0; i < amountWords; i++)
            {
                rInd = r.Next(0, symbols.Length);
                CreateBtn(symbols[rInd].ToString());
            }
            indInputSymbols = 0;
        }

        private void ClearPanel()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    t.Stop();
                    ((WrapPanel)FindName("panelBtn")).Children.Clear();
                    UpdateFails(0);
                    indInputSymbols = totalSec = 0;
                    isEnd = false;
                }
                catch (Exception) { }
            });
        }

        private void UserMessage(string message)
        {
            ClearPanel();
            MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateFails(int value)
        {
            try
            {
                ((TextBlock)FindName("FailsNum")).Text = $"Fails: {value}";
            }
            catch (Exception) { }
        }




        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (GetNameKey(e))
            {
                if (lastClick == nameKey) return;
                foreach (Button btn in buttons)
                {
                    if (btn.Content.ToString() == nameKey) 
                    {
                        ClickBtn(btn);
                        UserInput();
                    }
                }
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (isEnd)  
                UserMessage("You win!");

            foreach (var btn in clickBtns) btn.Background = brush;  
            clickBtns.Clear();  
            lastClick = "";
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                ((TextBlock)FindName("DifficultyNum")).Text = $"Difficulty: {amountWords = (int)e.NewValue}";
            }
            catch (Exception) { }
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            if (!t.Enabled)
            {
                amountFails = 0;
                GenerateText();
                ((Button)sender).Focusable = false;
                t.Start();
            }
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            if (t.Enabled)
            {
                t.Stop();
                ClearPanel();
                ((Button)sender).Focusable = false;
            }
        }


        // Практически весь код был взят с https://github.com/DaniilSob2004, я потратил 4 часа на разбирательство, не смог, и взял у него
    }
}