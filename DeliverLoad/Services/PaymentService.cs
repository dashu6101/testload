using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeliverLoad.Models;


namespace DeliverLoad.Services
{
    public partial class DeliverLoadService
    {

        public void CreatePaymentResponses(string txn_id, decimal payment_price, string email, string first_name, string last_name, string street, string city, string state, string zip, string country,
        string request_id, bool is_success, string reason_fault, int UserId,bool IsAuthenticated)
        {
            try
            {
                PaymentHistory objPaymentHistory = new PaymentHistory();

                objPaymentHistory.UserID = UserId;
                objPaymentHistory.Tracking_Code = txn_id;
                objPaymentHistory.Payment_Code = "";
                objPaymentHistory.Payment_Price = payment_price;
                objPaymentHistory.Email = email;
                objPaymentHistory.Payment_Date = DateTime.Now;
                objPaymentHistory.First_Name = first_name;
                objPaymentHistory.Last_Name = last_name;
                objPaymentHistory.Street = street;
                objPaymentHistory.City = city;
                objPaymentHistory.State = state;
                objPaymentHistory.Zip = zip;
                objPaymentHistory.Country = country;
                objPaymentHistory.Request_Id = request_id;
                objPaymentHistory.Is_Success = is_success;
                objPaymentHistory.reason_fault = reason_fault;
                
                //objPaymentHistory.AdminID = AdminId;
                // objPaymentHistory.IsSubscription = true;
                dbContext.PaymentHistories.Add(objPaymentHistory);
                dbContext.SaveChanges();

                // User Master Updation

                if (txn_id != "")
                {
                    User users = dbContext.Users.Find(UserId);
                    users.IsAuthenticated = IsAuthenticated;
                    dbContext.SaveChanges();
                }

                if (is_success == true)
                {
                    UpdateUserBalance(UserId, payment_price);
                }
            }
            catch (Exception ex)
            {
                //("Error in payment_success.CreatePaymentResponses(): " + ex.Message)
            }

        }
        public bool IsDuplicateID(string txn_id)
        {
            try
            {
                //string expression = null;
                //expression = "txn_id = '" + txn_id + "'";
                //DataRow[] tempRow = responses.Tables[0].Select(expression);
                //if (tempRow.Length == 0)
                //{
                //    return false;
                //}
                return false;
            }
            catch (Exception ex)
            {
                //("Error in payment_success.IsDuplicateID(): " + ex.Message)
                return false;
            }
        }


        public List<PaymentHistoryModel> getPaymentHistory(int UserId)
        {

           var model = dbContext.PaymentHistories.Where(x => x.UserID == UserId).Select
               (x => new PaymentHistoryModel{ID=x.ID,Payment_Id=x.Payment_Code,Txn_Id=x.Tracking_Code,Payment_Date=(DateTime)x.Payment_Date,
               Payment_Price=x.Payment_Price,First_Name=x.First_Name,Last_Name=x.Last_Name,Street = x.Street,City=x.City,State=x.State,Zip=x.Zip,
               Country=x.Country,Request_Id=x.Request_Id,Is_Success=x.Is_Success,reason_fault=x.reason_fault,UserID=x.UserID}).OrderByDescending(x => x.Payment_Date).ToList();

            return model;
        }

        public string UpdateUserBalance(int UserId,decimal Balance)
        {
            try
            {
                User objUser = dbContext.Users.Where(x => x.UserId == UserId).SingleOrDefault();

                objUser.Balance = objUser.Balance + Balance;

                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "1";

        }

        

    }
}