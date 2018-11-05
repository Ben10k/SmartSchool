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
using System.Windows.Shapes;
using SmartSchool.Model;
using SmartSchool.rfid;

namespace SmartSchool {
    public partial class PaymentWindow {
        public PaymentWindow(Product product, uint comPort, SchoolContext dataContext, string key) {
            var comPort1 = comPort;
            var dataContext1 = dataContext;

            InitializeComponent();
            Name.Content = product.Name;
            Price.Content = product.Price;
            Progress.Content = "Provide a card";

            var paymentWindow = this;

            new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                User user;
                while (true)
                    if (CardReader.IsCardPresent(comPort1)) {
                        Thread.Sleep(100);
                        string cardId = CardReader.ReadId(comPort1, 4, key);
                        user = dataContext1.Users.Find(cardId);
                        if (user != null)
                            break;
                    }

                Application.Current.Dispatcher.Invoke(new Action(delegate {
                    paymentWindow.Progress.Content = "User found. Checking balance";
                }));
                Thread.Sleep(5000);
                if (user.Balance < product.Price) {
                    Application.Current.Dispatcher.Invoke(new Action(delegate {
                        paymentWindow.Progress.Content = "Payment denied. Not enough balance.";
                    }));
                }
                else {
                    user.Balance -= product.Price;

                    Application.Current.Dispatcher.Invoke(new Action(delegate {
                        paymentWindow.Progress.Content = "Payment accepted. Please take your product.";
                    }));
                    dataContext1.PurchaseLogs.Add(new PurchaseLog
                        {Id = Guid.NewGuid(), Product = product, Time = DateTime.Now, User = user});
                }

                dataContext1.SaveChanges();
                Thread.Sleep(5000);
                Application.Current.Dispatcher.Invoke(new Action(delegate { paymentWindow.Close(); }));
            }).Start();
        }
    }
}