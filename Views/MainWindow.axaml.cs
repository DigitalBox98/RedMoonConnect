using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Configuration;

namespace RedMoonConnect.Views
{
    public partial class MainWindow : Window
    {

        public static String CONFIGNAME_LOGIN = "Login";
        public static String CONFIGNAME_SETTINGS = "Settings";
        public static String CONFIGNAME_DAOC_LOCATION = "DaocLocation";
        public static String CONFIGNAME_DAOC_SERVER = "DaocServer";
        public static String CONFIGNAME_WINE = "Wine";
        public static String CONFIGNAME_WINE_CMD = "WineCmd";

        public static String CONFIG_LOGIN = "";
        public static String CONFIG_SETTINGS = "";
        public static String CONFIG_DAOC_LOCATION = "";
        public static String CONFIG_DAOC_SERVER = "";
        public static String CONFIG_WINE = "";
        public static String CONFIG_WINE_CMD = "";

        String appSettings = "";

        Button connectBtn;
        TextBox loginTextBox;
        TextBox passTextBox;
        CheckBox settings;
        CheckBox wine;
        TextBox wineCmd;
        TextBox daocLocation;
        TextBox daocServer;
        TextBlock status;

        public MainWindow()
        {
            InitializeComponent();

            appSettings = ConfigurationManager.AppSettings["RedMoonConnect"];

            connectBtn = this.Find<Button>("ConnectBtn");
            loginTextBox = this.Find<TextBox>("Login");
            passTextBox = this.Find<TextBox>("Pass");
            settings = this.Find<CheckBox>("Settings");
            wine = this.Find<CheckBox>("Wine");
            wineCmd = this.Find<TextBox>("WineCmd");
            daocLocation = this.Find<TextBox>("DaocLocation");
            daocServer = this.Find<TextBox>("DaocServer");
            status = this.Find<TextBlock>("Status");

            ReadAllSettings();

            loginTextBox.Text = CONFIG_LOGIN;
            settings.IsChecked = bool.Parse(CONFIG_SETTINGS);
            daocLocation.Text = CONFIG_DAOC_LOCATION;
            daocServer.Text = CONFIG_DAOC_SERVER;
            wine.IsChecked = bool.Parse(CONFIG_WINE);
            wineCmd.Text = CONFIG_WINE_CMD;

        }

        public void Connect_onClick(object sender, RoutedEventArgs args)
        {
            String statusPrefix = "Status: ";

            String command = "";
            String location = "";
            String login = loginTextBox.Text;
            String pass = passTextBox.Text;
            String server = "";

            String currentDirectory = Directory.GetCurrentDirectory();
            Debug.WriteLine("Current directory : "+ currentDirectory);

            String home = "";
            String realLocation = "";
            String connectFile = "";
            String gameDll = "";

            String fileSeparator = "";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileSeparator = "\\";
            }
            else 
            {
                fileSeparator = "/";
            }


            // Settings defined by the user 
            if (settings.IsChecked == true)
            {
                Debug.WriteLine("Settings enabled");

                // Server IP
                server = daocServer.Text;

                // DAOC folder
                location = @daocLocation.Text;
            }
            // Default settings
            else
            {
                // Server IP
                server = "play.redmoonfreeshard.org";

                // DAOC folder
                location = "c:/Program Files (x86)/Electronic Arts/Dark Age of Camelot/";
            }

            // Wine requested by user 
            if (wine.IsChecked == true)
            {
                Debug.WriteLine("Wine enabled");

                // Check "wine" tool
                command = wineCmd.Text;

                home = Environment.GetEnvironmentVariable("HOME");
                realLocation = location.Replace("c:/", home+"/.wine/drive_c/");
            }
            // Default setup : connect tools
            else
            {
                realLocation = location;
                command = realLocation + fileSeparator + "connect.exe";
            }

            // Connection tool setup
            connectFile = realLocation + fileSeparator + "connect.exe";
            gameDll = realLocation + fileSeparator + "game-1125.dll";


            // Checks and patch if required

            // Check Login 
            if (login ==null || login.Equals(""))
            {
                status.Text = statusPrefix + "Login must be indicated.";
                return;
            }

            // Check Password
            if (pass == null || pass.Equals(""))
            {
                status.Text = statusPrefix + "Password must be indicated.";
                return;
            }

            // Check DAOC folder exists
            if (!Directory.Exists(realLocation))
            {
                status.Text = statusPrefix + "DAOC folder doesn't exist.\n(ie: "+realLocation+")";
                return;
            }

            // Check "connect.exe"
            if (!File.Exists(connectFile))
            {
                status.Text = statusPrefix + "connect.exe doesn't exist.";
                try
                {
                    File.Copy(currentDirectory + fileSeparator + "connect.exe", connectFile);
                    status.Text = statusPrefix + "connect.exe copied to DAOC folder.";
                }
                catch (Exception e)
                {
                    status.Text = statusPrefix + "Error copying connect.exe to DAOC folder\n"+e.ToString();
                    return;
                }
            }

            // Check "game-1125.dll"
            if (!File.Exists(gameDll))
            {
                status.Text = statusPrefix + "game-1125.dll doesn't exist.";
                try
                {
                    File.Copy(currentDirectory + fileSeparator + "game-1125.dll", gameDll);
                    status.Text = statusPrefix + "game-1125.dll copied to DAOC folder.";
                }
                catch (Exception e)
                {
                    status.Text = statusPrefix + "Error copying game-1125.dll to DAOC folder.\n"+e.ToString();
                    return;
                }
            }

            // Check wine is recognized
            if (!ExistsOnPath(command))
            {
                status.Text = statusPrefix + "Wine tool doesn't exist.";
                return;
            }

            var proc1 = new ProcessStartInfo();
            proc1.UseShellExecute = false;

            proc1.WorkingDirectory = currentDirectory;

            proc1.FileName = command;
            proc1.Verb = "runas";

            if (wine.IsChecked == true)
            {
                proc1.Arguments = @"cmd.exe /C ""cd /d " + @location + @" && connect.exe"" game-1125.dll " + server + " " + login + " " + pass;
            }
            else
            {
                proc1.Arguments = @"""" + @location + @fileSeparator + "game-1125.dll" + @""" "+ server + " " + login + " " + pass;
            }

            Debug.WriteLine("Command : " + proc1.FileName);
            Debug.WriteLine("Arguments : " + proc1.Arguments);

            // Save user's settings 
            AddUpdateAppSettings(CONFIGNAME_LOGIN, login);
            AddUpdateAppSettings(CONFIGNAME_SETTINGS, new String(""+settings.IsChecked));
            AddUpdateAppSettings(CONFIGNAME_DAOC_LOCATION, location);
            AddUpdateAppSettings(CONFIGNAME_DAOC_SERVER, server);
            AddUpdateAppSettings(CONFIGNAME_WINE, new String("" + wine.IsChecked));
            AddUpdateAppSettings(CONFIGNAME_WINE_CMD, wineCmd.Text);

            status.Text = statusPrefix + "Game connection in progress.";

            proc1.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(proc1);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        public static String GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");

            if (values != null)
            {
                foreach (var path in values.Split(Path.PathSeparator))
                {
                    var fullPath = Path.Combine(path, fileName);
                    if (File.Exists(fullPath))
                        return fullPath;
                }
            }

            return "";
        }


        private void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count > 0)
                {
                    CONFIG_LOGIN = ReadSetting(CONFIGNAME_LOGIN);
                    CONFIG_SETTINGS = ReadSetting(CONFIGNAME_SETTINGS);
                    CONFIG_DAOC_LOCATION = ReadSetting(CONFIGNAME_DAOC_LOCATION);
                    CONFIG_DAOC_SERVER = ReadSetting(CONFIGNAME_DAOC_SERVER);
                    CONFIG_WINE = ReadSetting(CONFIGNAME_WINE);
                    CONFIG_WINE_CMD = ReadSetting(CONFIGNAME_WINE_CMD);

                    Debug.WriteLine("CONFIG_LOGIN="+ CONFIG_LOGIN);
                    Debug.WriteLine("CONFIG_SETTINGS=" + CONFIG_SETTINGS);
                    Debug.WriteLine("CONFIG_DAOC_LOCATION=" + CONFIG_DAOC_LOCATION);
                    Debug.WriteLine("CONFIG_DAOC_SERVER=" + CONFIG_DAOC_SERVER);
                    Debug.WriteLine("CONFIG_WINE=" + CONFIG_WINE);
                    Debug.WriteLine("CONFIG_WINE_CMD=" + CONFIG_WINE_CMD);
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        private String ReadSetting(String key)
        {
            String result = "";
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "Not Found";
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
            return result;
        }

        private void AddUpdateAppSettings(String key, String value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    }

}

