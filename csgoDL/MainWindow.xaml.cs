using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace csgoDL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AppViewModel _viewModel = new AppViewModel();

        public MainWindow()
        {
            // Init
            InitializeComponent();

            // Catch unhandled exceptions (if not debugging)
            if (!AppDomain.CurrentDomain.FriendlyName.EndsWith("vshost.exe"))
            {
                var handler = new UnhandledExceptionHandler() { LogAction = WriteLine };
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(handler.LogUnhandledException);
                Dispatcher.UnhandledException += handler.LogUnhandledException;
                TaskScheduler.UnobservedTaskException += handler.LogUnhandledException;
            }

            // Init sample cfg
            BoxServerCFG.Text = string.Join(Environment.NewLine, SRCDSWrapper.DefaultServerCFGContent);

            // Add collection to grid
            gridWeaponProgressionT.DataContext = _viewModel.WeaponProgressionT;
            gridWeaponProgressionCT.DataContext = _viewModel.WeaponProgressionCT;

            // Init pseudo-random RCON pw (better than nothing)
            Random randomizer = new Random();
            BoxRCONPassword.Text = randomizer.Next(10000000, 99999999).ToString();
        }

        /// <summary>
        /// The process in which the server is executed
        /// </summary>
        private Process _serverProcess = null;

        /// <summary>
        /// Defines basic parameters to start the server
        /// </summary>
        private BasicSetup _setup = null;

        /// <summary>
        /// Disables the buttons to prevent two parallel server executions by one window
        /// </summary>
        private void DisableButtons()
        {
            Dispatcher.Invoke(() =>
            {
                ButtonStart.IsEnabled = false;
                ImageStart.Visibility = Visibility.Hidden;
                ButtonStop.IsEnabled = true;
                ImageStop.Visibility = Visibility.Visible;
            });
        }

        /// <summary>
        /// Enables the buttons
        /// </summary>
        private void EnableButtons()
        {
            Dispatcher.Invoke(() =>
            {
                ButtonStart.IsEnabled = true;
                ImageStart.Visibility = Visibility.Visible;
                ButtonStop.IsEnabled = false;
                ImageStop.Visibility = Visibility.Hidden;
            });
        }

        /// <summary>
        /// Writes a message to the output box
        /// </summary>
        /// <param name="message">The message to write</param>
        private void Write(string message) { Dispatcher.Invoke(() => { OutputTextBox.AppendText(message); }); }

        /// <summary>
        /// Writes a message in a terminated line to the output box
        /// </summary>
        /// <param name="message">The message to write</param>
        private void WriteLine(string message = "") { Dispatcher.Invoke(() => { OutputTextBox.AppendText(message + Environment.NewLine); }); }

        /// <summary>
        /// Parses the setup from the UI controls
        /// </summary>
        private void ParseSetup()
        {
            // Some sanity checking
            if (string.IsNullOrWhiteSpace(BoxSRCDSPath.Text))
                throw new ArgumentException("Please provide a path to the dedicated server executable!");
            // Parse the config
            Dispatcher.Invoke(() =>
            {
                _setup = new BasicSetup()
                {
                    SRCDSPath = BoxSRCDSPath.Text,
                    Hostname = BoxHostName.Text,
                    Port = BoxPort.Text,
                    GSLT = BoxGSLT.Text,
                    RCONPW = BoxRCONPassword.Text,
                    ServerPW = BoxServerPassword.Text,
                    MaxPlayers = BoxMaxPlayers.Text,
                    TickRate = BoxTickRate.Text,
                    GameType = (GameType)Enum.Parse(typeof(GameType), ComboBoxGameType.Text),
                    WeaponProgressionT = _viewModel.WeaponProgressionTSource,
                    WeaponProgressionCT = _viewModel.WeaponProgressionCTSource,
                    MOTD = BoxMOTD.Text,
                    ServerCFG = BoxServerCFG.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList()
                };
            });
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Disable the buttons first to prevent two or more servers executed in parallel
                DisableButtons();

                // Parse arguments
                ParseSetup();

                // Create cfg file on-the-fly
                SRCDSWrapper.CreateServerCfg(_setup);

                // Create gamemodes file on-the-fly
                SRCDSWrapper.CreateGamemodesFile(_setup);

                // Create motd on-the-fly
                SRCDSWrapper.CreateMOTD(_setup);

                // Initialize a process for the server
                WriteLine("Starting the server: " + BoxSRCDSPath.Text + " " + SRCDSWrapper.BuildDefaultArguments(_setup));
                string exePath = System.IO.Path.Combine(BoxSRCDSPath.Text, SRCDSWrapper.DEFAULT_SRCDS_EXE_FILENAME);
                if (!File.Exists(exePath))
                    throw new ArgumentException("Did not find " + SRCDSWrapper.DEFAULT_SRCDS_EXE_FILENAME + " in location: " + exePath);
                _serverProcess = new Process()
                {
                    EnableRaisingEvents = true,
                    StartInfo = new ProcessStartInfo(exePath, SRCDSWrapper.BuildDefaultArguments(_setup))
                    {
                        //UseShellExecute = false,
                        //RedirectStandardInput = true,
                    }
                };

                // Add termination event action
                _serverProcess.Exited += _serverProcess_Exited;

                // Start the server process
                _serverProcess.Start();
            }
            catch (Exception ex)
            {
                // Output the exception
                WriteLine(">>> An ERROR occurred: " + ex.Message);
                WriteLine("Stacktrace: ");
                WriteLine(ex.StackTrace);

                // Enable buttons again
                EnableButtons();
            }
        }

        void _serverProcess_Exited(object sender, EventArgs e)
        {
            // Inform the user
            WriteLine("Process terminated.");

            // Enable the buttons again
            EnableButtons();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            // TODO: implement a stop routine which is compatible with the srcds.exe and does not kill it the hard way
            // Remark: using the approach below does not seem to work, because srcds does not accept shellexecute disabled
            //// Send the exit command to the process
            //WriteLine(">>> Sending server process the exit signal ...");
            //_serverProcess.StandardInput.WriteLine("exit");

            //// Wait for the process to exit on itself
            //WriteLine(">>> Allowing the process 10 seconds to terminate ...");
            //_serverProcess.WaitForExit(10000);

            //// If process did not quit successfully - kill it
            //if (!_serverProcess.HasExited)
            //{
            _serverProcess.Kill();
            WriteLine("Killed the process!");
            //}
            //else
            //{
            //    WriteLine(">>> Process terminated.");
            //}

            // Enable the buttons again
            EnableButtons();
        }

        private void ButtonSearchExe_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog fileDialog = new OpenFileDialog();

            // Set filter options and filter index.
            fileDialog.Filter = "executable Files (*.exe)|*.exe";
            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = fileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                BoxSRCDSPath.Text = System.IO.Path.GetDirectoryName(fileDialog.FileName);
            }
        }

        private void ButtonLoadSetup_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog fileDialog = new OpenFileDialog();

            // Set filter options and filter index.
            fileDialog.Filter = "Text Files (*.dlconf)|*.dlconf";
            fileDialog.FilterIndex = 1;
            fileDialog.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = fileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                try
                {
                    // Read the setup from a file
                    _setup = new BasicSetup();
                    _setup.ReadFile(fileDialog.FileName);

                    // Set fields
                    Dispatcher.Invoke(() =>
                    {
                        BoxSRCDSPath.Text = _setup.SRCDSPath;
                        BoxRCONPassword.Text = _setup.RCONPW;
                        BoxServerPassword.Text = _setup.ServerPW;
                        BoxHostName.Text = _setup.Hostname;
                        BoxGSLT.Text = _setup.GSLT;
                        BoxPort.Text = _setup.Port;
                        BoxMaxPlayers.Text = _setup.MaxPlayers;
                        BoxTickRate.Text = _setup.TickRate;
                        ComboBoxGameType.Text = _setup.GameType.ToString();
                        BoxMOTD.Text = _setup.MOTD;
                        BoxServerCFG.Text = string.Join(Environment.NewLine, _setup.ServerCFG);
                    });

                    // Set weapon progression
                    _viewModel = new AppViewModel(_setup.WeaponProgressionT, _setup.WeaponProgressionCT);
                    Dispatcher.Invoke(() =>
                    {
                        gridWeaponProgressionT.DataContext = _viewModel.WeaponProgressionT;
                        gridWeaponProgressionCT.DataContext = _viewModel.WeaponProgressionCT;
                    });

                    // Log
                    WriteLine("Successfully restored setup from \"" + fileDialog.FileName + "\"");
                }
                catch (Exception ex)
                {
                    // Output the exception
                    WriteLine(">>> An ERROR occurred: " + ex.Message);
                    WriteLine("Stacktrace: ");
                    WriteLine(ex.StackTrace);
                }
            }
        }

        private void ButtonSaveSetup_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of the open file dialog box.
            SaveFileDialog fileDialog = new SaveFileDialog();

            // Set filter options and filter index.
            fileDialog.Title = "Save setup ...";
            fileDialog.Filter = "Text files (*.dlconf)|*.dlconf";
            fileDialog.RestoreDirectory = true;

            // Call the ShowDialog method to show the dialog box.
            bool? userClickedOK = fileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                try
                {
                    // Refresh setup
                    ParseSetup();

                    // Write the setup to a file
                    _setup.WriteFile(fileDialog.FileName);

                    // Log
                    WriteLine("Successfully stored the setup to \"" + fileDialog.FileName + "\"");
                }
                catch (Exception ex)
                {
                    // Output the exception
                    WriteLine(">>> An ERROR occurred: " + ex.Message);
                    WriteLine("Stacktrace: ");
                    WriteLine(ex.StackTrace);
                }
            }
        }
    }
}
