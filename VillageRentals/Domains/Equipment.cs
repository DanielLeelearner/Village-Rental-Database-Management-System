using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageRentals
{
    public class Equipment
    {
        private int equipmentId;
        private string equipmentName;
        private string description;
        private float dailyRate;

        public Equipment(int equipmentID, string equipmentName, string description, float dailyRate) {
            this.equipmentId = equipmentID;
            this.equipmentName = equipmentName;
            this.description = description;
            this.dailyRate = dailyRate;

        }
        public int GetEquipmentID() { return equipmentId; }

        public string GetEquipmentName() { return equipmentName; }

        public string GetDescription() { return description; }

        public float GetDailyRate() { return dailyRate; }
    }
}
