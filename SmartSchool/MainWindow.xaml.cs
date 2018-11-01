using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using access_control;

namespace SmartSchool {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private Thread access;

        private void SwitchTab(TabItem fromTab, TabItem toTab) {
            toTab.IsEnabled = true;
            fromTab.IsEnabled = false;
            toTab.Focus();
        }

        private void AttendanceRegistrationButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(MenuTab, AttendanceRegistrationTab);
            var mainWindow = this;
            access = new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                UInt32 nPort = 3;

                rfid f = new rfid();

                while (true) {
                    Thread.Sleep(1000);
                    bool b1 = f.scanCards(nPort) == 0;
                    //TODO normalus grant/deny
//                if (b1) {
//                    byte[]a = f.readBlock(nPort, 4, sKey);
//                }

                    Application.Current.Dispatcher.Invoke(
                        new Action(
                            delegate {
                                mainWindow.AttendanceRegistrationLabel.Content = b1
                                    ? "Access Granted"
                                    : "Access Denied";
                            }));
                }
            });
            access.Start();
        }

        private void AttendanceRegistration2MenuButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(AttendanceRegistrationTab, MenuTab);
            access.Abort();
        }

        private void AttendanceReviewButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(MenuTab, AttendanceReviewTab);
            //Nuskaityti ID
            //Pasetinti varda pavarde pagal id
            //Gauti attendance log'a
            //Pasetiniti attendance i AttendanceReviewListBox
            var myList = new List<string> {"HELLO", "WORLD"};
            foreach (var element in myList) {
                AttendanceReviewListBox.Items.Add(element);
            }
        }

        private void VendingMachineButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(MenuTab, VendingMachineTab);

            PaymentWindow paymentWindow = new PaymentWindow();
            paymentWindow.Show();

        }

        private void PurchaseHistoryButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(MenuTab, PurchaseHistoryTab);
        }

        private void AttendanceReview2MenuButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(AttendanceReviewTab, MenuTab);
        }

        private void VendingMachine2MenuButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(VendingMachineTab, MenuTab);
        }

        private void PurchaseHistory2MenuButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(PurchaseHistoryTab, MenuTab);
        }
    }
}