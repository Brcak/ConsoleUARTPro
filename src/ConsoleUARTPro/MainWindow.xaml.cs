using System; using System.Net.Http; using System.Diagnostics; using System.Text.Json.Nodes; using System.Reflection; using System.IO; using System.IO.Ports; using System.Linq; using System.Text.Json; using System.Windows; using Microsoft.Win32; namespace ConsoleUARTPro; public partial class MainWindow : Window { SerialPort? serial; readonly string settings=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"ConsoleUARTPro","settings.json"); public MainWindow(){InitializeComponent(); Refresh_Click(this,new RoutedEventArgs()); LoadPrefs(); AppVersion.Text="Version: "+(Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown"); Closed+=(_,_)=>{serial?.Close();serial?.Dispose();};} void Refresh_Click(object sender,RoutedEventArgs e){Ports.ItemsSource=SerialPort.GetPortNames().OrderBy(x=>x).ToArray();Ports.SelectedIndex=Ports.Items.Count>0?0:-1;} void Connect_Click(object sender,RoutedEventArgs e){if(Ports.SelectedItem is not string port){Status.Text="Select a serial port.";return;} try {serial?.Dispose();int baud=int.Parse(((System.Windows.Controls.ComboBoxItem)Bauds.SelectedItem).Content.ToString()!);serial=new SerialPort(port,baud){ReadTimeout=500,Handshake=Handshake.None,DtrEnable=false,RtsEnable=false};serial.DataReceived+=Received;serial.Open();Status.Text="Connected to "+port+" (RX-only software; verify hardware adapter safety).";}catch(Exception ex){Status.Text=ex.Message;}} void Received(object sender,SerialDataReceivedEventArgs e){try{string data=serial?.ReadExisting()??"";Dispatcher.BeginInvoke(()=>{Log.AppendText(data);Log.ScrollToEnd();});}catch(Exception){}} void Disconnect_Click(object sender,RoutedEventArgs e){serial?.Close();Status.Text="Disconnected";} void Save_Click(object sender,RoutedEventArgs e){var dialog=new SaveFileDialog{Filter="Text log|*.txt",FileName="uart-log.txt"};if(dialog.ShowDialog()==true)File.WriteAllText(dialog.FileName,Log.Text);} void Save_ClickPrefs(object sender,RoutedEventArgs e){var data=new Prefs(UpdateMode.SelectedIndex,Database.IsChecked==true,Pico.IsChecked==true,Esp.IsChecked==true,Pi.IsChecked==true,Boardview.IsChecked==true,Thermal.IsChecked==true,Logic.IsChecked==true);Directory.CreateDirectory(Path.GetDirectoryName(settings)!);File.WriteAllText(settings,JsonSerializer.Serialize(data));PrefsStatus.Text="Preferences saved. No software was downloaded or installed.";} void LoadPrefs(){try{var p=JsonSerializer.Deserialize<Prefs>(File.ReadAllText(settings));if(p==null)return;UpdateMode.SelectedIndex=Math.Clamp(p.Mode,0,2);Database.IsChecked=p.Database;Pico.IsChecked=p.Pico;Esp.IsChecked=p.Esp;Pi.IsChecked=p.Pi;Boardview.IsChecked=p.Boardview;Thermal.IsChecked=p.Thermal;Logic.IsChecked=p.Logic;}catch(IOException){}catch(JsonException){}} static readonly HttpClient updatesHttp = CreateUpdateClient();
 static HttpClient CreateUpdateClient() { var client=new HttpClient{Timeout=TimeSpan.FromSeconds(12)};client.DefaultRequestHeaders.UserAgent.ParseAdd("ConsoleUARTPro-UpdateCheck/1.0");return client; }
 async void CheckUpdates_Click(object sender,RoutedEventArgs e) {
   UpdateCheckStatus.Text="Checking GitHub releases...";
   try {
     using var response=await updatesHttp.GetAsync("https://api.github.com/repos/Brcak/ConsoleUARTPro/releases/latest");
     if(response.StatusCode==System.Net.HttpStatusCode.NotFound) { UpdateCheckStatus.Text="No published GitHub release is available yet.";return; }
     response.EnsureSuccessStatusCode();
     var data=JsonNode.Parse(await response.Content.ReadAsStringAsync());
     string? tag=data?["tag_name"]?.GetValue<string>();
     string? url=data?["html_url"]?.GetValue<string>();
     if(string.IsNullOrWhiteSpace(tag)||string.IsNullOrWhiteSpace(url)||!Uri.TryCreate(url,UriKind.Absolute,out var releaseUri)||releaseUri.Scheme!="https"||releaseUri.Host!="github.com") { UpdateCheckStatus.Text="Release information was incomplete.";return; }
     var current=Assembly.GetExecutingAssembly().GetName().Version??new Version(0,0);
     var clean=tag.TrimStart('v','V').Split('-')[0];
     if(Version.TryParse(clean,out var latest)) {
       UpdateCheckStatus.Text=latest>current ? "Update available: "+tag+". Review the release before downloading." : "Latest published version: "+tag+". Installed assembly version: "+current+".";
     } else UpdateCheckStatus.Text="Latest published release: "+tag+". Review its notes.";
     if(MessageBox.Show("Open the GitHub release page? No update will be downloaded or installed automatically.","Console UART Pro Updates",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes) Process.Start(new ProcessStartInfo(releaseUri.AbsoluteUri){UseShellExecute=true});
   } catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException or JsonException or InvalidOperationException) { UpdateCheckStatus.Text="Unable to check for updates: "+ex.Message; }
 }
 void OpenReleases_Click(object sender,RoutedEventArgs e) { Process.Start(new ProcessStartInfo("https://github.com/Brcak/ConsoleUARTPro/releases"){UseShellExecute=true}); }
 
 void OpenLog_Click(object sender,RoutedEventArgs e) {
   var dialog=new OpenFileDialog{Filter="Text logs|*.txt;*.log|All files|*.*"};
   if(dialog.ShowDialog()!=true)return;
   try { Log.Text=File.ReadAllText(dialog.FileName); Status.Text="Opened log: "+Path.GetFileName(dialog.FileName); MainTabs.SelectedIndex=0; }
   catch(Exception ex) when(ex is IOException or UnauthorizedAccessException) { MessageBox.Show(ex.Message,"Open log failed",MessageBoxButton.OK,MessageBoxImage.Error); }
 }
 void CopyLog_Click(object sender,RoutedEventArgs e) {
   if(!string.IsNullOrEmpty(Log.SelectedText)) Clipboard.SetText(Log.SelectedText);
   else if(!string.IsNullOrEmpty(Log.Text)) Clipboard.SetText(Log.Text);
 }
 void ClearLog_Click(object sender,RoutedEventArgs e) {
   if(string.IsNullOrEmpty(Log.Text))return;
   if(MessageBox.Show("Clear the current UART log? Save it first if needed.","Clear log",MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)Log.Clear();
 }
 void Exit_Click(object sender,RoutedEventArgs e) => Close();
 void SettingsMenu_Click(object sender,RoutedEventArgs e) => MainTabs.SelectedIndex=1;
 void AboutMenu_Click(object sender,RoutedEventArgs e) => MainTabs.SelectedIndex=2;
 void CheckUpdatesMenu_Click(object sender,RoutedEventArgs e) { MainTabs.SelectedIndex=2; CheckUpdates_Click(sender,e); }
 void OpenProject_Click(object sender,RoutedEventArgs e) { Process.Start(new ProcessStartInfo("https://github.com/Brcak/ConsoleUARTPro"){UseShellExecute=true}); }
 record Prefs(int Mode,bool Database,bool Pico,bool Esp,bool Pi,bool Boardview,bool Thermal,bool Logic); }