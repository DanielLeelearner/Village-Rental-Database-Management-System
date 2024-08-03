using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Maui.ApplicationModel.Communication;
using MySqlConnector;

namespace VillageRentals
{
    public class DatabaseConnection
    {
        private MySqlConnection connection;

        public DatabaseConnection()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                UserID = "root",
                Password = "password",
                Database = "cpsydatabase",
            };

            connection = new MySqlConnection(builder.ConnectionString);
        }

        public void OpenConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                    //Console.WriteLine("Connected to the database.");
                }
            }
            catch (MySqlException ex)
            {
                // Console.WriteLine("Error opening connection: " + ex.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    //Console.WriteLine("Connection closed.");
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error closing connection: " + ex.Message);
            }
        }

        public bool AddCustomer(string lastName, string firstName, string contactPhone, string email)
        {
            try
            {
                OpenConnection();

                string sql = "INSERT INTO customer (last_name, first_name, contact_phone, email) VALUES (@last_name, @first_name, @contact_phone, @email);";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@last_name", lastName);
                    command.Parameters.AddWithValue("@first_name", firstName);
                    command.Parameters.AddWithValue("@contact_phone", contactPhone);
                    command.Parameters.AddWithValue("@email", email);
                    int rowsAffected = command.ExecuteNonQuery();

                    CloseConnection();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }

        public bool AddEquipment(int equipmentId, int categoryId, string equipmentName, string description, float dailyRate)
        {
            try
            {
                OpenConnection();

                string sql1 = "INSERT INTO equipment (equipment_id, equipment_name, description, daily_rate) VALUES (@equipment_id, @equipment_name, @description, @daily_rate);";
                string sql2 = "INSERT INTO equipment_category (equipment_id, category_id) VALUES  (@equipment_id, @category_id);";
                int row1 = 0, row2 = 0;

                using (MySqlCommand command1 = new MySqlCommand(sql1, connection))
                {
                    command1.Parameters.AddWithValue("@equipment_id", equipmentId);
                    command1.Parameters.AddWithValue("@equipment_name", equipmentName);
                    command1.Parameters.AddWithValue("@description", description);
                    command1.Parameters.AddWithValue("@daily_rate", dailyRate);
                    row1 = command1.ExecuteNonQuery();
                }

                using (MySqlCommand command2 = new MySqlCommand(sql2, connection))
                {
                    command2.Parameters.AddWithValue("@equipment_id", equipmentId);
                    command2.Parameters.AddWithValue("@category_id", categoryId);
                    row2 = command2.ExecuteNonQuery();
                }


                CloseConnection();
                return row1 + row2 > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        public void DeleteEquipment(int equipmentId)
        {
            OpenConnection();

            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = connection;
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string deleteCategorySql = "DELETE FROM equipment_category WHERE equipment_id = @equipment_id;";
                        command.CommandText = deleteCategorySql;
                        command.Parameters.AddWithValue("@equipment_id", equipmentId);
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();

                        string deleteEquipmentSql = "DELETE FROM equipment WHERE equipment_id = @equipment_id;";
                        command.CommandText = deleteEquipmentSql;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@equipment_id", equipmentId);
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        //Console.WriteLine("Equipment deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error deleting equipment: " + ex.Message);
                        transaction.Rollback();
                    }
                }
            }

            CloseConnection();
        }

        public List<List<string>> GetEquipmentList()
        {
            List<List<string>> EquipmentList = new List<List<string>>();
            string sql = "SELECT equipment_id, category_name, equipment_name, description, daily_rate FROM equipment NATURAL JOIN equipment_category NATURAL JOIN category_list;";
            MySqlCommand command = new MySqlCommand(sql, connection);

            OpenConnection();
            try
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<string> equipmentData = new List<string>();
                        equipmentData.Add(reader["equipment_id"].ToString());
                        equipmentData.Add(reader["category_name"].ToString());
                        equipmentData.Add(reader["equipment_name"].ToString());
                        equipmentData.Add(reader["description"].ToString());
                        equipmentData.Add(reader["daily_rate"].ToString());
                        EquipmentList.Add(equipmentData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            CloseConnection();
            return EquipmentList;

        }

        public List<List<string>> GetCustomerList()
        {
            List<List<string>> CustomerList = new List<List<string>>();
            string sql = "SELECT * FROM customer";

            MySqlCommand command = new MySqlCommand(sql, connection);
            OpenConnection();
            try
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<string> customerData = new List<string>();
                        customerData.Add(reader["customer_id"].ToString());
                        customerData.Add(reader["last_name"].ToString());
                        customerData.Add(reader["first_name"].ToString());
                        customerData.Add(reader["contact_phone"].ToString());
                        customerData.Add(reader["email"].ToString());
                        CustomerList.Add(customerData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            CloseConnection();
            return CustomerList;
        }

        public List<List<string>> GetRentalRecord()
        {
            List<List<string>> Records = new List<List<string>>();
            string sql = "SELECT rental_id, customer_id, last_name, equipment_id ,equipment_name,order_date, rental_date, return_date, cost  FROM rental NATURAL JOIN customer_rental NATURAL JOIN customer NATURAL JOIN equipment;";

            MySqlCommand command = new MySqlCommand(sql, connection);
            OpenConnection();
            try
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<string> rentalData = new List<string>();
                        rentalData.Add(reader["rental_id"].ToString());
                        rentalData.Add(reader["customer_id"].ToString());
                        rentalData.Add(reader["last_name"].ToString());
                        rentalData.Add(reader["equipment_id"].ToString());
                        rentalData.Add(reader["equipment_name"].ToString());
                        rentalData.Add(reader["order_date"].ToString());
                        rentalData.Add(reader["rental_date"].ToString());
                        rentalData.Add(reader["return_date"].ToString());
                        rentalData.Add(reader["cost"].ToString());

                        Records.Add(rentalData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            CloseConnection();
            return Records;
        }

        public void CreateRental(int customerId, string rental_date, string return_date, List<int> equipmentIds)
        {
            try
            {
                OpenConnection();

                string currDate = DateTime.Now.ToString("M/dd/yyyy");
                int totalDate = (DateTime.Parse(return_date) - DateTime.Parse(rental_date)).Days;

                float totalCost = 0;

                foreach (int id in equipmentIds)
                {
                    float equipmentCost = GetEquipmentCostById(id);
                    totalCost += equipmentCost;
                }

                totalCost *= totalDate;

                // Insert into rental table
                string rentalSql = "INSERT INTO rental (order_date, rental_date, return_date, cost) VALUES (@order_date, @rental_date, @return_date, @cost);";
                OpenConnection();
                using (MySqlCommand rentalCommand = new MySqlCommand(rentalSql, connection))
                {
                    rentalCommand.Parameters.AddWithValue("@order_date", currDate);
                    rentalCommand.Parameters.AddWithValue("@rental_date", rental_date);
                    rentalCommand.Parameters.AddWithValue("@return_date", return_date);
                    rentalCommand.Parameters.AddWithValue("@cost", totalCost);

                    rentalCommand.ExecuteNonQuery();

                    // Get the rental_id of the last inserted row
                    int rentalId = (int)rentalCommand.LastInsertedId;

                    // Insert into customer_rental table
                    foreach (int equipmentId in equipmentIds)
                    {
                        string customerRentalSql = "INSERT INTO customer_rental (rental_id, customer_id, equipment_id) VALUES (@rental_id, @customer_id, @equipment_id);";

                        using (MySqlCommand customerRentalCommand = new MySqlCommand(customerRentalSql, connection))
                        {
                            customerRentalCommand.Parameters.AddWithValue("@rental_id", rentalId);
                            customerRentalCommand.Parameters.AddWithValue("@customer_id", customerId);
                            customerRentalCommand.Parameters.AddWithValue("@equipment_id", equipmentId);

                            customerRentalCommand.ExecuteNonQuery();
                        }
                    }
                }

                CloseConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }


        private float GetEquipmentCostById(int equipmentId)
        {
            float cost = 0;
            string sql = $"SELECT daily_rate FROM equipment WHERE equipment_id = {equipmentId};";

            OpenConnection();

            MySqlCommand command = new MySqlCommand(sql, connection);

            try
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cost = reader.GetFloat("daily_rate");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            CloseConnection();

            return cost;
        }
    }
}