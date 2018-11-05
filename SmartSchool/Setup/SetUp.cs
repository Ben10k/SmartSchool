using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Windows.Documents;
using SmartSchool.Model;

namespace SmartSchool.Setup {
    public class SetUp {
        public SchoolContext Context { get; }

        public SetUp() {
            Context = new SchoolContext();
//            Context.Database.Delete();
//            addUsers();
//            AddProducts();
            Context.SaveChanges();
        }

        private void AddProducts() {
            Context.Products.AddRange(new List<Product> {
                new Product {Id = Guid.NewGuid(), Name = "Snickers", Price = 0.47},
                new Product {Id = Guid.NewGuid(), Name = "Twix", Price = 0.47},
                new Product {Id = Guid.NewGuid(), Name = "Lion", Price = 0.47},
                new Product {Id = Guid.NewGuid(), Name = "Neptūnas, 0,5l", Price = 0.53},
                new Product {Id = Guid.NewGuid(), Name = "CocaCola, 0,5l", Price = 0.89},
                new Product {Id = Guid.NewGuid(), Name = "Sausainiai GAIDELIS (KLASIKA), 180 g", Price = 0.51},
            });
        }

        private void AddUsers() {
            Context.Users.AddRange(new List<User> {
                new User {Id = "123456789", Balance = 100, Name = "Benas"},
                new User {Id = "987654321", Balance = 100, Name = "Gediminas"}
            });
        }
    }
}