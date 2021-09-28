namespace ProjetoPaíses
{
    using ProjetoPaíses.Modelos;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    public partial class GameUC : UserControl
    {
        private List<Pais> _paises;

        private List<Pais> aux;

        private List<string> names = new List<string>();

        private Pais selectedWinner;

        private int tries = 0, score = 0;

        public GameUC(List<Pais> paises)
        {
            InitializeComponent();
            _paises = paises;
            aux = paises;
        }

        private void populate()
        {
            int i = 0;

            foreach (var pais in _paises)
            {
                names[i] = pais.Alpha3Code;
                i++;
            }
        }

        private void picker()
        {
            btn_startRound.Visibility = Visibility.Hidden;

            btn_guess1.Visibility = Visibility.Visible;
            btn_guess2.Visibility = Visibility.Visible;
            btn_guess3.Visibility = Visibility.Visible;
            svg_flag.Visibility = Visibility.Visible;

            string path;

            var randomPais = new Random();

            int index = randomPais.Next(_paises.Count);

            Pais pais = _paises[index];

            if (pais.Flag != null)
                path = Path.GetFullPath($"Flags\\{pais.Alpha3Code}.svg");
            else
                path = Path.GetFullPath("notAvailable.svg");

            if (!File.Exists($"Flags\\{pais.Alpha3Code}.svg"))
                path = Path.GetFullPath("notAvailable.svg");

            Uri uri = new Uri($"file:///{path}");

            svg_flag.Source = uri;

            selectedWinner = pais;

            names.Add(pais.Name);

            _paises.Remove(pais);

            index = randomPais.Next(_paises.Count);

            pais = _paises[index];

            names.Add(pais.Name);

            _paises.Remove(pais);

            index = randomPais.Next(_paises.Count);

            pais = _paises[index];

            names.Add(pais.Name);

            index = randomPais.Next(names.Count);

            string namesSelected = names[index];

            tb_btn1.Text = names[index];

            names.Remove(namesSelected);

            index = randomPais.Next(names.Count);

            namesSelected = names[index];

            tb_btn2.Text = names[index];

            names.Remove(namesSelected);

            index = randomPais.Next(names.Count);

            namesSelected = names[index];

            tb_btn3.Text = names[index];

            names.Remove(namesSelected);
        }

        private void reset()
        {
            _paises = aux;

            btn_guess1.IsEnabled = true;
            btn_guess2.IsEnabled = true;
            btn_guess3.IsEnabled = true;

            btn_guess1.Background = new SolidColorBrush(Color.FromRgb(0, 211, 211));
            btn_guess2.Background = new SolidColorBrush(Color.FromRgb(0, 211, 211));
            btn_guess3.Background = new SolidColorBrush(Color.FromRgb(0, 211, 211));

            picker();
        }

        private void btn_startRound_Click(object sender, RoutedEventArgs e)
        {
            picker();
        }

        private async void btn_guess1_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            btn_guess2.IsEnabled = false;
            btn_guess3.IsEnabled = false;

            tries++;

            updateTries();

            if (check(tb_btn1))
                hit(btn_guess1);
            else
                miss(btn_guess1);

            await Task.Delay(2000);

            reset();

            if (tries == 5)
                newRound();
        }

        private async void btn_guess2_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            btn_guess1.IsEnabled = false;
            btn_guess3.IsEnabled = false;

            tries++;

            updateTries();

            if (check(tb_btn2))
                hit(btn_guess2);
            else
                miss(btn_guess2);

            await Task.Delay(2000);

            reset();

            if (tries == 5)
                newRound();
        }

        private async void btn_guess3_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            btn_guess1.IsEnabled = false;
            btn_guess2.IsEnabled = false;

            tries++;

            updateTries();

            if (check(tb_btn3))
                hit(btn_guess3);
            else
                miss(btn_guess3);

            await Task.Delay(2000);

            reset();

            if (tries == 5)
                newRound();
        }

        private void miss(Button button)
        {
            button.Background = Brushes.Red;

            if (selectedWinner.Name == tb_btn1.Text)
                btn_guess1.Background = Brushes.Green;

            if (selectedWinner.Name == tb_btn2.Text)
                btn_guess2.Background = Brushes.Green;

            if (selectedWinner.Name == tb_btn3.Text)
                btn_guess3.Background = Brushes.Green;
        }

        private void hit(Button button)
        {
            button.Background = Brushes.Green;

            score++;

            tb_score.Text = $"{score}/5";
        }

        private void updateTries()
        {
            tb_round.Text = $"{tries}/5";
        }

        private bool check(TextBlock textBlock)
        {
            bool check = false;

            if (selectedWinner.Name == textBlock.Text)
                check = true;

            return check;
        }



        private void newRound()
        {
            btn_startRound.Visibility = Visibility.Visible;

            btn_guess1.Visibility = Visibility.Hidden;
            btn_guess2.Visibility = Visibility.Hidden;
            btn_guess3.Visibility = Visibility.Hidden;
            svg_flag.Visibility = Visibility.Hidden;

            MessageBox.Show($"Your score was {score}/5!", "Score", MessageBoxButton.OK, MessageBoxImage.Information);

            tb_score.Text = "0/5";
            tb_round.Text = "0/5";

            tries = 0;
            score = 0;
        }
    }
}