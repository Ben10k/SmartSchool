using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SmartSchool.Model;
using SmartSchool.rfid;
using SmartSchool.Setup;
using static System.Windows.Media.Brushes;

namespace SmartSchool {
    public partial class MainWindow {
        public MainWindow() {
            InitializeComponent();
            _dataContext = new SetUp().Context;
        }

        private Thread _access;
        private readonly SchoolContext _dataContext;
        private const string Key = "FFFFFFFFFFFF";
        private const uint ComPort = 3;

        private static void SwitchTab(TabItem fromTab, TabItem toTab) {
            toTab.IsEnabled = true;
            fromTab.IsEnabled = false;
            toTab.Focus();
        }

        private void AttendanceRegistrationButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(MenuTab, AttendanceRegistrationTab);
            _access = new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                while (true) {
                    if (CardReader.IsCardPresent(ComPort)) {
                        var user = _dataContext.Users.Find(CardReader.ReadId(ComPort, 4, Key));
                        if (user != null) {
                            _dataContext.AccessLogs.Add(
                                new AccessLog {User = user, Id = Guid.NewGuid(), Time = DateTime.Now});
                            _dataContext.SaveChanges();
                            SetTextAndColor(AttendanceRegistrationLabel, "Access Granted:\n" + user.Name, Green);
                        }
                        else
                            SetTextAndColor(AttendanceRegistrationLabel, "Access Denied", Red);

                        Thread.Sleep(5000);
                    }
                    else
                        SetTextAndColor(AttendanceRegistrationLabel, "Waiting", Black);
                }
            });
            _access.Start();
        }


        private static void SetTextAndColor(ContentControl label, string text, Brush color) {
            Application.Current.Dispatcher.Invoke(new Action(delegate {
                label.Content = text;
                label.Foreground = color;
            }));
        }


        private void AttendanceReviewButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(MenuTab, AttendanceReviewTab);
            AttendanceReviewListBox.Items.Clear();
            AttendanceReviewLabel.Content = "Supply a card";
            new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                var user = GetUser();
                var accessLogs = getAccessLogs(user);
                Application.Current.Dispatcher.Invoke(new Action(delegate {
                    AttendanceReviewLabel.Content = user.Name;
                    foreach (var log in accessLogs)
                        AttendanceReviewListBox.Items.Add(log.Time);
                }));
            }).Start();
        }

        private IEnumerable<AccessLog> getAccessLogs(User user) {
            return _dataContext.AccessLogs
                .Where(log => log.User.Id.Equals(user.Id))
                .OrderByDescending(a => a.Time)
                .AsEnumerable();
        }

        private void VendingMachineButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(MenuTab, VendingMachineTab);
            var products = _dataContext.Products.ToList();
            VendingMachineDataGrid.ItemsSource = products;
        }

        private void PurchaseHistoryButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(MenuTab, PurchaseHistoryTab);
            PurchaseHistoryListBox.Items.Clear();
            PurchaseHistoryLabel.Content = "Supply a card";
            new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                var user = GetUser();
                var purchaseLogs = GetPurchaseLogs(user);
                Application.Current.Dispatcher.Invoke(new Action(delegate {
                    PurchaseHistoryLabel.Content = user.Name;
                    foreach (var log in purchaseLogs)
                        PurchaseHistoryListBox.Items.Add(log.Product.Name + " - " + log.Time);
                }));
            }).Start();
        }

        private User GetUser() {
            while (true)
                if (CardReader.IsCardPresent(ComPort)) {
                    var user = _dataContext.Users.Find(CardReader.ReadId(ComPort, 4, Key));
                    if (user != null)
                        return user;
                }
        }

        private IEnumerable<PurchaseLog> GetPurchaseLogs(User user) {
            return _dataContext.PurchaseLogs
                .Where(log => log.User.Id.Equals(user.Id))
                .OrderByDescending(a => a.Time)
                .AsEnumerable();
        }

        private void BuyProduct_OnClick(object sender, RoutedEventArgs e) {
            var product = ((FrameworkElement) sender).DataContext as Product;

            var paymentWindow = new PaymentWindow(product, ComPort, _dataContext, Key);
            paymentWindow.Show();
        }

        private void AttendanceRegistration2MenuButton_OnClick(object sender, RoutedEventArgs e) {
            SwitchTab(AttendanceRegistrationTab, MenuTab);
            _access.Abort();
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