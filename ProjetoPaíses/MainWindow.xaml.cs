namespace ProjetoPaíses
{
    using System.Windows;
    using Modelos;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Globalization;

    public partial class MainWindow : Window
    {
        private NetworkService networkService;
        private ApiService apiService;
        private DialogService dialogService;
        private DataService dataService;
        private List<Pais> Paises;
        private List<Information> Informations;
        private bool load;

        public MainWindow()
        {
            InitializeComponent();
            networkService = new NetworkService();
            apiService = new ApiService();
            dialogService = new DialogService();
            dataService = new DataService();
            load = false;
            showLoading();
            LoadPaises();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowData();
        }

        private void ShowData()
        {
            string path;

            var pais = (Pais)cb_pais.SelectedItem;

            lbl_coordinates.Visibility = Visibility.Hidden;

            tb_nomeResultado.Text = pais.Name;
            tb_capitalResultado.Text = pais.Capital;
            tb_regiaoResultado.Text = pais.Region;
            tb_subregiaoResultado.Text = pais.SubRegion;

            if (pais.Gini == 0)
                tb_giniResultado.Text = "Gini unavailable";
            else
                tb_giniResultado.Text = pais.Gini.ToString();

            if (pais.Population == 0)
                tb_populationResultado.Text = "Population unavailable";
            else
                tb_populationResultado.Text = String.Format(CultureInfo.InvariantCulture, "{0:0,##}", pais.Population);

            if (pais.Flag != null)
                path = Path.GetFullPath($"Flags\\{pais.Alpha3Code}.svg");
            else
                path = Path.GetFullPath("notAvailable.svg");

            if (!File.Exists($"Flags\\{pais.Alpha3Code}.svg"))
                path = Path.GetFullPath("notAvailable.svg");

            Uri uri = new Uri($"file:///{path}");

            svg_flag.Source = uri;

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccessful)
                lbl_coordinates.Visibility = Visibility.Visible;
            else if (pais.latlng.Count == 0)
                lbl_coordinates.Visibility = Visibility.Visible;
            else
            {
                map.Center = new Microsoft.Maps.MapControl.WPF.Location(pais.latlng[0], pais.latlng[1]);
                map.ZoomLevel = 5;

                Pin.Location = new Microsoft.Maps.MapControl.WPF.Location(pais.latlng[0], pais.latlng[1]);
                Pin.Content = pais.Alpha3Code;
            }
        }

        public async void LoadPaises()
        {
            bool load;

            lbl_result.Content = "Checking connection...";

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccessful)
            {
                lbl_result.Content = "Trying to load local country data...";
                LoadLocalCountries();
                load = false;
                endLoading();
            }
            else
            {
                lbl_result.Content = "Loading country data from API...";
                await LoadApiPaises();
                load = true;
                endLoading();
            }

            if (Paises.Count == 0)
            {
                lbl_result.Content = "There is no Internet connection!" + Environment.NewLine +
                    "No data saved from previous sessions." + Environment.NewLine;

                lbl_status.Content = "You need to configure your Internet connection on the application's first use";

                return;
            }
                        
            cb_pais.IsEnabled = true;
            cb_pais.ItemsSource = Paises;
            cb_pais.DisplayMemberPath = "Name";
            cb_pais.IsEditable = true;
            cb_pais.Text = "Select a country...";

            cb_pais.SelectedIndex = 0;

            lbl_loading.Visibility = Visibility.Hidden;

            lbl_result.Content = "Data updated!";

            if (load)
            {
                lbl_status.Content = string.Format("Data loaded from the API on {0:F}", DateTime.Now.ToShortDateString());
            }
            else
            {
                lbl_status.Content = "Data loaded from local database";
            }

            pb_load.Value = 100;
        }

        private void LoadLocalCountries()
        {
            (Paises, Informations) = dataService.GetData();
        }

        private async Task LoadApiPaises()
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;

            var response = await apiService.GetLists("http://restcountries.eu", "/rest/v2/all", typeof(Pais).ToString());

            Paises = (List<Pais>)response.Result;

            response = await apiService.GetLists("http://joaopirescet58.somee.com", "/api/information", typeof(Information).ToString());

            Informations = (List<Information>)response.Result;

            dataService.DeleteData();

            await dataService.FlagDownloadAsync(Paises, progress);

            await dataService.SaveDataAsync(Paises, Informations, progress);
        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            lbl_result.Content = $"{e.Action} information about: {e.CurrentCountry}." + Environment.NewLine +
                $"Number of countries {e.Action2}: {e.CountriesCompleted}.";

            pb_load.Value = e.Percentage;
        }

        private void showLoading()
        {
            LoadingUC loading = new LoadingUC();

            sp_stackPanel.Children.Clear();
            sp_stackPanel.Children.Add(loading);
        }

        private void endLoading()
        {
            sp_stackPanel.Children.Clear();

            load = true;
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btn_home_Click(object sender, RoutedEventArgs e)
        {
            if (load)
            {
                sp_stackPanel.Children.Clear();
            }
            else
            {
                LoadingUC loading = new LoadingUC();

                sp_stackPanel.Children.Clear();
                sp_stackPanel.Children.Add(loading);
            }
        }

        private void btn_game_Click(object sender, RoutedEventArgs e)
        {
            if(Paises.Count == 0)
            {
                MessageBox.Show("No information available, please setup an Internet connection!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (load)
            {
                GameUC game = new GameUC(Paises);

                sp_stackPanel.Children.Clear();
                sp_stackPanel.Children.Add(game);
            }
            else
            {
                LoadingUC loading = new LoadingUC();

                sp_stackPanel.Children.Clear();
                sp_stackPanel.Children.Add(loading);

                MessageBox.Show("Still awaiting information!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btn_about_Click(object sender, RoutedEventArgs e)
        {
            AboutUC about = new AboutUC();

            sp_stackPanel.Children.Clear();
            sp_stackPanel.Children.Add(about);
        }

        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btn_zoomPlus_Click(object sender, RoutedEventArgs e)
        {
            if(map.ZoomLevel < 21)
                map.ZoomLevel++;
        }

        private void btn_zoomMinus_Click(object sender, RoutedEventArgs e)
        {
            if(map.ZoomLevel > 1)
                map.ZoomLevel--;
        }

        private void btn_funFact_Click(object sender, RoutedEventArgs e)
        {
            var pais = (Pais)cb_pais.SelectedItem;

            var info = Informations.Find(x => x.Name == pais.Name);

            var connection = networkService.CheckConnection();

            if (!connection.IsSuccessful && Informations.Count == 0)
                MessageBox.Show("No information available, please setup an Internet connection!", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
            else if (info != null)
                MessageBox.Show($"{info.Info}", "Fun fact!");
            else
                MessageBox.Show("Information about this country is not available", "Fun fact!");
        }
    }
}
