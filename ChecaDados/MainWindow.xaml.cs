using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using AutoUpdaterDotNET;
using System.Reflection;

namespace ChecaDados
{
    public partial class MainWindow : Window
    {
        private const string ApiUrl = "https://open.cnpja.com/office/";
        private readonly string CsvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ChecaDados", "ChecaDados.csv");
        private readonly List<DateTime> ApiCalls = new List<DateTime>();
        private const int CallsPerMinute = 5;
        private readonly DateTime LastApiCallTime = DateTime.MinValue; // última chamada à API
        private bool IsRateLimitExceeded => ApiCalls.Count >= CallsPerMinute;
        private Timer rateLimitTimer;

        public MainWindow()
        {
            InitializeComponent();
            AutoUpdater.Start("https://raw.githubusercontent.com/LynxDevIO/ChecaDados/main/version.xml");
            Directory.CreateDirectory(Path.GetDirectoryName(CsvPath));
            UpdateRateLimitDisplay();
            // Define o texto do rótulo de versão automaticamente
            VersionLabel.Text = $"Versão {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private async void Query_Click(object sender, RoutedEventArgs e)
        {
            StartRateLimitTimer();

            // verifica se a última chamada disponível à API foi feita depois de 1 minuto
            if (IsRateLimitExceeded && (DateTime.Now - LastApiCallTime).TotalMinutes < 1)
            {
                CnpjOutput.Text = "Limite de consultas por minuto alcançado. Aguarde...";
                return;
            }

            await QueryCnpjAsync();
        }

        // timer de 1 minuto quando o limite de chamadas é atingido, depois disso, IsRateLimitExceeded será false
        private void StartRateLimitTimer()
        {
            if (rateLimitTimer == null)
            {
                rateLimitTimer = new Timer(60000); // 1 minuto em milissegundos
                rateLimitTimer.Elapsed += Timer_Tick;
                rateLimitTimer.AutoReset = false;
            }
            if (!rateLimitTimer.Enabled)
            {
                rateLimitTimer.Start();
            }
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ApiCalls.Clear();
                UpdateRateLimitDisplay();
                CnpjOutput.Text = string.Empty;
            });
        }

        private async void CnpjInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await QueryCnpjAsync();
            }
        }

        private async Task QueryCnpjAsync()
        {
            string cnpj = CnpjInput.Text.Trim().Replace(".", "").Replace("/", "").Replace("-", "");
            if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14 || !IsNumeric(cnpj))
            {
                ClearOutputs();
                CnpjOutput.Text = "Por favor, digite um CNPJ válido de 14 dígitos.";
                return;
            }

            // Verificar se o CNPJ já existe no CSV
            var existingRecord = ReadCsv().FirstOrDefault(r => r.Cnpj == cnpj);
            if (existingRecord != null)
            {
                DisplayResult(existingRecord);
                return;
            }

            // Verificar conexão com a internet
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                ClearOutputs();
                CnpjOutput.Text = "Sem conexão com a internet. Verifique sua rede e tente novamente.";
                return;
            }

            // Verificar limite de taxa
            CleanOldApiCalls();
            if (ApiCalls.Count >= CallsPerMinute)
            {
                ClearOutputs();
                CnpjOutput.Text = "Limite por minuto alcançado. Aguarde...";
                return;
            }

            ClearOutputs();
            CnpjOutput.Text = "Consultando...";
            try
            {
                using (var client = new HttpClient())
                {
                    ApiCalls.Add(DateTime.Now);
                    UpdateRateLimitDisplay();
                    var response = await client.GetAsync($"{ApiUrl}{cnpj}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = JObject.Parse(json);
                        var record = new CnpjRecord
                        {
                            Cnpj = cnpj,
                            BusinessName = data["company"]?["name"]?.ToString() ?? "N/A",
                            State = data["address"]?["state"]?.ToString() ?? "N/A",
                            StateRegistration = (data["registrations"] as JArray)?.FirstOrDefault()?["number"]?.ToString() ?? "N/A",
                            QueryTime = DateTime.Now
                        };
                        SaveToCsv(record);
                        DisplayResult(record);
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        ClearOutputs();
                        CnpjOutput.Text = "CNPJ inválido ou inexistente. Verifique o número digitado.";
                    }
                    else
                    {
                        ClearOutputs();
                        CnpjOutput.Text = $"Erro: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                    }
                }
            }
            catch (Exception ex)
            {
                ClearOutputs();
                CnpjOutput.Text = $"Erro: {ex.Message}";
            }
        }

        private void DisplayResult(CnpjRecord record)
        {
            CnpjOutput.Text = FormatCnpj(record.Cnpj);
            BusinessNameOutput.Text = record.BusinessName;
            StateOutput.Text = record.State;
            StateRegistrationOutput.Text = record.StateRegistration;
            QueryTimeOutput.Text = record.QueryTime.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private string FormatCnpj(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14 || !IsNumeric(cnpj))
                return cnpj;
            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }

        private void ClearOutputs()
        {
            CnpjOutput.Text = string.Empty;
            BusinessNameOutput.Text = string.Empty;
            StateOutput.Text = string.Empty;
            StateRegistrationOutput.Text = string.Empty;
            QueryTimeOutput.Text = string.Empty;
        }

        private bool IsNumeric(string input)
        {
            return long.TryParse(input, out _);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            CnpjInput.Text = string.Empty;
            ClearOutputs();
        }

        private void CleanOldApiCalls()
        {
            var now = DateTime.Now;
            ApiCalls.RemoveAll(t => (now - t).TotalMinutes >= 1);
            UpdateRateLimitDisplay();
        }

        private void UpdateRateLimitDisplay()
        {
            RateLimitText.Text = $"Consultas restantes por minuto: {CallsPerMinute - ApiCalls.Count}";
        }

        private void SaveToCsv(CnpjRecord record)
        {
            var records = ReadCsv();
            if (!records.Any(r => r.Cnpj == record.Cnpj))
            {
                records.Add(record);
                using (var writer = new StreamWriter(CsvPath))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
                {
                    csv.WriteRecords(records);
                }
            }
        }

        private List<CnpjRecord> ReadCsv()
        {
            if (!File.Exists(CsvPath))
                return new List<CnpjRecord>();

            using (var reader = new StreamReader(CsvPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                return csv.GetRecords<CnpjRecord>().ToList();
            }
        }
    }

    public class CnpjRecord
    {
        public string Cnpj { get; set; }
        public string BusinessName { get; set; }
        public string State { get; set; }
        public string StateRegistration { get; set; }
        public DateTime QueryTime { get; set; }
    }
}