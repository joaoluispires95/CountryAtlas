namespace ProjetoPaíses.Services
{
    using Modelos;
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    public class DataService
    {
        private SQLiteConnection connection;

        private SQLiteConnection connectionCustom;

        private SQLiteCommand command;

        private DialogService dialogService;

        public DataService()
        {
            dialogService = new DialogService();

            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            if (!Directory.Exists("Flags"))
            {
                Directory.CreateDirectory("Flags");
            }

            var path = @"Data\paises.sqlite";

            var pathCustom = @"Data\Informations.sqlite";

            try
            {
                connection = new SQLiteConnection("Data Source =" + path);
                connection.Open();

                string sqlCommand = "create table if not exists paises(Name varchar(200), Capital varchar(200), Region varchar(200), " +
                    "SubRegion varchar(200), Population int, Gini real, Flag varchar(350), Alpha3Code varchar(10))";

                command = new SQLiteCommand(sqlCommand, connection);

                command.ExecuteNonQuery();

                connectionCustom = new SQLiteConnection("Data Source =" + pathCustom);
                connectionCustom.Open();

                sqlCommand = "create table if not exists Informations(id int, Name varchar(250), Info varchar(500))";

                command = new SQLiteCommand(sqlCommand, connectionCustom);

                command.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error!", e.Message);
            }
        }

        public async Task SaveDataAsync(List<Pais> Paises, List<Information> Informations, IProgress<ProgressReportModel> progress)
        {
            CultureInfo ci = new CultureInfo("en-us");
            System.Threading.Thread.CurrentThread.CurrentCulture = ci;

            try
            {
                WebClient client = new WebClient();
                ProgressReportModel report = new ProgressReportModel();

                int i = 0;

                foreach (var pais in Paises)
                {
                    string sql = $"insert into paises (Name, Capital, Region, SubRegion, Population, Gini, Flag, Alpha3Code) " +
                        $"values (\"{pais.Name}\", \"{pais.Capital}\", \"{pais.Region}\", \"{pais.SubRegion}\", {pais.Population}, {pais.Gini}, \"{pais.Flag}\", \"{pais.Alpha3Code}\")";

                    command = new SQLiteCommand(sql, connection);

                    await Task.Run(() => command.ExecuteNonQuery());

                    i++;

                    report.Action = "Saving";
                    report.Action2 = "saved";
                    report.CountriesCompleted = i;
                    report.CurrentCountry = pais.Name;
                    report.Percentage = (i * 100 / (Paises.Count + Informations.Count)) / 2 + 50;

                    progress.Report(report);
                }

                foreach (var info in Informations)
                {
                    string sql = $"insert into Informations (id, Name, Info) values (\"{info.id}\", \"{info.Name}\", \"{info.Info}\")";

                    command = new SQLiteCommand(sql, connectionCustom);

                    await Task.Run(() => command.ExecuteNonQuery());

                    i++;

                    report.Action = "Saving fact";
                    report.Action2 = "which fact was already saved";
                    report.CountriesCompleted = i - Paises.Count;
                    report.CurrentCountry = info.Name;
                    report.Percentage = (i * 100 / (Paises.Count + Informations.Count)) / 2 + 50;

                    progress.Report(report);
                }

                connection.Close();
                connectionCustom.Close();
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error!", e.Message);
            }
        }

        public async Task FlagDownloadAsync(List<Pais> Paises, IProgress<ProgressReportModel> progress)
        {
            try
            {
                WebClient client = new WebClient();
                ProgressReportModel report = new ProgressReportModel();

                int i = 0;

                foreach (var pais in Paises)
                {
                    await client.DownloadFileTaskAsync(new Uri(pais.Flag), @"Flags\" + pais.Alpha3Code + ".svg");

                    i++;

                    report.Action = "Downloading";
                    report.Action2 = "downloaded";
                    report.CountriesCompleted = i;
                    report.CurrentCountry = pais.Name;
                    report.Percentage = (i * 100 / Paises.Count) / 2;

                    progress.Report(report);
                }
                    
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error!", e.Message);
            }
        }

        public (List<Pais>, List<Information>) GetData()
        {
            List<Pais> paises = new List<Pais>();

            List<Information> infortmations = new List<Information>();

            try
            {
                string sql = "select Name, Capital, Region, SubRegion, Population, Gini, Flag, Alpha3Code from Paises";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    paises.Add(new Pais
                    {
                        Name = (string)reader["Name"],
                        Capital = (string)reader["Capital"],
                        Region = (string)reader["Region"],
                        SubRegion = (string)reader["SubRegion"],
                        Population = (int)reader["Population"],
                        Gini = (double)reader["Gini"],
                        Flag = (string)reader["Flag"],
                        Alpha3Code = (string)reader["Alpha3Code"]
                    });
                }

                sql = "select id, Name, Info from Informations";

                command = new SQLiteCommand(sql, connectionCustom);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    infortmations.Add(new Information
                    {
                        id = (int)reader["id"],
                        Name = (string)reader["Name"],
                        Info = (string)reader["Info"]
                    });
                }

                connection.Close();
                connectionCustom.Close();

                return (paises, infortmations);
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error!", e.Message);

                return (null, null);
            }
        }

        public void DeleteData()
        {
            try
            {
                string sql = "delete from paises";

                var path = @"Flags";

                command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();

                sql = "delete from Informations";

                command = new SQLiteCommand(sql, connectionCustom);

                command.ExecuteNonQuery();

                DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch (Exception e)
            {
                dialogService.ShowMessage("Error!", e.Message);
            }
        }
    }
}
