using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageRentals
{
    public class Customer
    {
        private int customerId;
        private string lastName;
        private string firstName;
        private string phoneNumber;
        private string email;

        Customer(int customerId, string lastName, string firstName, string phoneNumber, string email)
        {
            this.customerId = customerId;
            this.lastName = lastName;
            this.firstName = firstName;
            this.phoneNumber = phoneNumber;
            this.email = email;
        }

        public int GetCustomerId()
        {
            return customerId;
        }
        public String GetLastName()
        {
            return lastName;
        }
        public String GetFirstName()
        {
            return firstName;
        }

        public String GetPhoneNumber()
        {
            return phoneNumber;
        }

        public String GetEmail()
        {
            return email;
        }
    }
}
