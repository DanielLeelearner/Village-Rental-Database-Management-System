using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageRentals
{
    public class Rental
    {
        private int rentalId;
        private string date;
        private string rentalDate;
        private string returnDate;
        private float cost;


        public Rental(int rentalId, string date, string rentalDate, string returnDate, float cost)
        {
            this.rentalId = rentalId;
            this.date = date;
            this.rentalDate = rentalDate;
            this.returnDate = returnDate;
            this.cost = cost;
        }

        public int GetRentalId() { return rentalId; }
        public string GetDate() { return date; }
        public string GetRentalDate() { return returnDate; }
        public string GetReturnDate() { return returnDate; }
        public float GetCost() { return cost; }
    }
}
