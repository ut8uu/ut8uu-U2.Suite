using System.Collections.ObjectModel;
using System.Net.Http;
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

using Newtonsoft.Json;

namespace U2.Logger.Frontend.Windows;

public partial class MainWindow : Window
{
    private readonly TextBox callsignTextBox;
    private readonly TextBox sentTextBox;
    private readonly TextBox rcvdTextBox;
    private readonly Button saveButton;

    public ObservableCollection<Core.Models.QSO> QSOs { get; set; }

    public MainWindow()
    {
        InitializeComponent();

        QSOs = [];
        QSODataGrid.ItemsSource = QSOs;

        PopulateControls();
    }

    private void PopulateControls()
    {
        // Populate the Band combo box with common amateur radio bands.
        string[] bands = { "160m", "80m", "60m", "40m", "30m", "20m", "17m", "15m", "12m", "10m", "6m", "2m", "70cm" };
        BandComboBox.ItemsSource = bands;
        BandComboBox.SelectedIndex = 0;

        // Populate the Mode combo box with common amateur radio modes.
        string[] modes = { "AM", "ATV", "BPSK", "CW", "Data", "DIGITALVOICE", "DSTAR", "DV", "FAX", "FM", "FT8", "HELL", "MFSK", "MS", "MT63", "OLIVIA", "PACKET", "PSK31", "RTTY", "SSTV", "SSB", "THROB", "TOR", "V4", "VOI", "WSPR" };
        ModeComboBox.ItemsSource = modes;
        ModeComboBox.SelectedIndex = 0;

        // Populate the Propagation combo box.
        string[] propagations = { "", "Ionospheric", "Satellite", "EME", "Aurora", "Sporadic E", "Meteor Scatter", "Tropospheric Ducting", "Ground Wave" };
        PropagationComboBox.ItemsSource = propagations;
        PropagationComboBox.SelectedIndex = 0;
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        saveButton.IsEnabled = false; // Disable button to prevent double-click
        saveButton.Content = "Saving...";

        // Create a new QSO object with data from the text boxes
        var qso = new Core.Models.QSO
        {
            Callsign = callsignTextBox.Text,
            RstSent = sentTextBox.Text,
            RstRcvd = rcvdTextBox.Text,
            // Get the current date and time and convert it to a numeric timestamp (seconds since Unix epoch)
            DateTime = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds,
            Band = "160 m (1.8-2.0 MHz)",
            Mode = "CW",
            Comment = "Logged from WPF app"
        };

        // Convert the QSO object to a JSON string
        var jsonPayload = JsonConvert.SerializeObject(qso);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Define your backend service URL
        var backendUrl = "http://localhost:5000/api/qso"; // Replace with your actual backend URL

        try
        {
            // Use HttpClient to send the POST request to the backend
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(backendUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("QSO saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Optionally clear the form
                    callsignTextBox.Clear();
                    sentTextBox.Clear();
                    rcvdTextBox.Clear();
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to save QSO. Status: {response.StatusCode}\nDetails: {responseContent}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            saveButton.IsEnabled = true;
            saveButton.Content = "Save QSO";
        }
    }

    //// The entry point of the application
    //[STAThread]
    //public static void Main(string[] args)
    //{
    //    var app = new Application();
    //    app.Run(new MainWindow());
    //}
}