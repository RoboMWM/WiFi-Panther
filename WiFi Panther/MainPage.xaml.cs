using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WiFi_Panther
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        WifiScanner scanner;
        public MainPage()
        {
            this.InitializeComponent();
            scanner = new WifiScanner(); //TODO: may need to make global? depends if we have page navigation. Maybe not if we keep this app minimal.
        }

        private async void ButtonScan_Click(object sender, RoutedEventArgs e)
        {
            buttonScan.IsEnabled = false;
            progressScan.IsActive = true;
            listViewAPs.Items.Clear();

            List<string> ssids;
            try
            {
                ssids = await scanner.ScanForNetworks();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
                buttonScan.IsEnabled = true;
                progressScan.IsActive = false;
                return;
            }

            populateSsids(ssids);
            buttonScan.IsEnabled = true;
            progressScan.IsActive = false;
        }

        private void populateSsids(List<string> ssids)
        {
            if (ssids == null)
            {
                ListViewItem item = new ListViewItem();
                item.Content = "No wireless access points found";
                listViewAPs.Items.Add(item);
                return;
            }

            foreach (string ssid in ssids)
                listViewAPs.Items.Add(new ListViewItem().Content = ssid);
        }

        private async void AppWifiInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await new MessageDialog(await this.scanner.GetAdapterInfo()).ShowAsync();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private async void AppAbout_Click(object sender, RoutedEventArgs e)
        {
            await new MessageDialog("WiFi Panther\nA beginner's tool to becoming a WiFi detective. \nVersion " + GetAppVersion()).ShowAsync();
        }

        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
