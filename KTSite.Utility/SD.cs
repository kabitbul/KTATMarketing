using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace KTSite.Utility
{
    public static class SD
    {
        public const string Role_Admin = "Admin";
        public const string Role_Warehouse = "Warehouse";
        public const string Role_Users = "Users";
        public const string Role_VAs = "VAs";
        public const string OrderStatusAccepted = "Accepted";
        public const string OrderStatusInProgress = "In Progress";
        public const string OrderStatusDone = "Done";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusFullRefund = "Full Refund";
        public const string OrderStatusPartialRefund = "Partial Refund";
        public const string PaymentPayoneer = "Payoneer";
        public const string PaymentPaypal = "Paypal";
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";
        public const string ReturningItemAdd = "Add";
        public const string ReturningItemRemove = "Remove";
        public const string ReturningItemDefective = "Defective";
        public const double shipping_cost = 3.5;
        public const double paypalPercentFees = 4.4;
        public const double paypalOneTimeFee = 0.3;
        public const string MadeInChina = "China";
        public const string MadeInUSA = "USA";
        public const string PathToCreateCSV = @"C:\Users\kobiab\Downloads\";
        //public const string PathToCreateCSV = @"D:\KT shipping";
        public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
     + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
     + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
				[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
     + @"([a-zA-Z0-9]+[\w-]+\.)+[a-zA-Z]{1}[a-zA-Z0-9-]{1,23})$";

        public static List<SelectListItem> MadeInState = new List<SelectListItem>()
        {
          new SelectListItem() { Text=MadeInChina, Value="China"},
          new SelectListItem() {Text=MadeInUSA, Value="USA"}
        };
        public static List<SelectListItem> ReturningItemStatus = new List<SelectListItem>()
        {
        new SelectListItem() {Text=ReturningItemAdd, Value=ReturningItemAdd},
        new SelectListItem() { Text=ReturningItemRemove, Value=ReturningItemRemove},
        new SelectListItem() { Text=ReturningItemDefective, Value=ReturningItemDefective}
        };
        public static List<SelectListItem> StatusAcceptOrCancel = new List<SelectListItem>()
        {
        new SelectListItem() {Text=OrderStatusAccepted, Value=OrderStatusAccepted},
        new SelectListItem() { Text=OrderStatusCancelled, Value=OrderStatusCancelled}
        };
        public static List<SelectListItem> paymentType = new List<SelectListItem>()
        {
        new SelectListItem() {Text=PaymentPayoneer, Value=PaymentPayoneer},
        new SelectListItem() { Text=PaymentPaypal, Value=PaymentPaypal}
        };
        public static List<SelectListItem> States = new List<SelectListItem>()
        {
        new SelectListItem() {Text="AL-Alabama", Value="AL"},
        new SelectListItem() { Text="AK-Alaska", Value="AK"},
        new SelectListItem() { Text="AZ-Arizona", Value="AZ"},
        new SelectListItem() { Text="AR-Arkansas", Value="AR"},
        new SelectListItem() { Text="CA-California", Value="CA"},
        new SelectListItem() { Text="CO-Colorado", Value="CO"},
        new SelectListItem() { Text="CT-Connecticut", Value="CT"},
        new SelectListItem() { Text="DC-District of Columbia", Value="DC"},
        new SelectListItem() { Text="DE-Delaware", Value="DE"},
        new SelectListItem() { Text="FL-Florida", Value="FL"},
        new SelectListItem() { Text="GA-Georgia", Value="GA"},
        //new SelectListItem() { Text="Hawaii", Value="HI"},
        new SelectListItem() { Text="ID-Idaho", Value="ID"},
        new SelectListItem() { Text="IL-Illinois", Value="IL"},
        new SelectListItem() { Text="IN-Indiana", Value="IN"},
        new SelectListItem() { Text="IA-Iowa", Value="IA"},
        new SelectListItem() { Text="KS-Kansas", Value="KS"},
        new SelectListItem() { Text="KY-Kentucky", Value="KY"},
        new SelectListItem() { Text="LA-Louisiana", Value="LA"},
        new SelectListItem() { Text="ME-Maine", Value="ME"},
        new SelectListItem() { Text="MD-Maryland", Value="MD"},
        new SelectListItem() { Text="MA-Massachusetts", Value="MA"},
        new SelectListItem() { Text="MI-Michigan", Value="MI"},
        new SelectListItem() { Text="MN-Minnesota", Value="MN"},
        new SelectListItem() { Text="MS-Mississippi", Value="MS"},
        new SelectListItem() { Text="MO-Missouri", Value="MO"},
        new SelectListItem() { Text="MT-Montana", Value="MT"},
        new SelectListItem() { Text="NE-Nebraska", Value="NE"},
        new SelectListItem() { Text="NV-Nevada", Value="NV"},
        new SelectListItem() { Text="NH-New Hampshire", Value="NH"},
        new SelectListItem() { Text="NJ-New Jersey", Value="NJ"},
        new SelectListItem() { Text="NM-New Mexico", Value="NM"},
        new SelectListItem() { Text="NY-New York", Value="NY"},
        new SelectListItem() { Text="NC-North Carolina", Value="NC"},
        new SelectListItem() { Text="ND-North Dakota", Value="ND"},
        new SelectListItem() { Text="OH-Ohio", Value="OH"},
        new SelectListItem() { Text="OK-Oklahoma", Value="OK"},
        new SelectListItem() { Text="OR-Oregon", Value="OR"},
        new SelectListItem() { Text="PA-Pennsylvania", Value="PA"},
        new SelectListItem() { Text="RI-Rhode Island", Value="RI"},
        new SelectListItem() { Text="SC-South Carolina", Value="SC"},
        new SelectListItem() { Text="SD-South Dakota", Value="SD"},
        new SelectListItem() { Text="TN-Tennessee", Value="TN"},
        new SelectListItem() { Text="TX-Texas", Value="TX"},
        new SelectListItem() { Text="UT-Utah", Value="UT"},
        new SelectListItem() { Text="VT-Vermont", Value="VT"},
        new SelectListItem() { Text="VA-Virginia", Value="VA"},
        new SelectListItem() { Text="WA-Washington", Value="WA"},
        new SelectListItem() { Text="WV-West Virginia", Value="WV"},
        new SelectListItem() { Text="WI-Wisconsin", Value="WI"},
        new SelectListItem() { Text="WY-Wyoming", Value="WY"}
    };
    }
}
//user admin1
//mail admin1@gmail.com
//Admin1!

//warehouse
//user bb 
//mail bb@gmail.com
//Bb111122!

//VAs
//user jj 
//mail jan@gmail.com
//Jj111122!

//user
//user Dani
//mail Dani@gmail.com
//Dd111122!

//user
//user Kfir
//mail Kfir@gmail.com
//Kk111122!

//user
//user Yossi Takiar
//mail Yossi@gmail.com
//Yy111122!

//user
//user Trevor Smith
//mail Trevor@gmail.com
//Tt111122!
//user
//user Mitch
//mail Mitch@gmail.com
//Mm111122!
//user
//user kabitbul
//mail kabitbul@gmail.com
//KkkK111122!

//when ModelState.IsValid equal to FALSE
//var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray()


//reset id sequence of table in sql server
//DBCC CHECKIDENT('[KTSite].[dbo].[UserStoreNames]', RESEED, 0);
//GO