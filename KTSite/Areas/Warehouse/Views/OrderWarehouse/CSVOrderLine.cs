using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KTSite.Areas.Warehouse.Views.OrderWarehouse
{
    public class CSVOrderLine
    {
        public string bookNumber { get; set; }
        public string trackingNumber { get; set; }
        public string carrier { get; set; }
        public string serviceType { get; set; }
        public string generated { get; set; }
        public string integrationName { get; set; }
        public string integrationType { get; set; }
        public string isVoided { get; set; }
        public string orderId { get; set; }
        public string orderNumber { get; set; }
        public string quoteAmount { get; set; }
        public string insuredAmount { get; set; }
        public string pieces { get; set; }
        public string length { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string pickupConfirmationNumber { get; set; }
        public string poNumber { get; set; }
        public string contentDescription { get; set; }
        public string shippingDate { get; set; }
        public string residential { get; set; }
        public string fromCity { get; set; }
        public string fromCompany { get; set; }
        public string fromCountry { get; set; }
        public string fromEmail { get; set; }
        public string fromName { get; set; }
        public string fromPhone { get; set; }
        public string fromState { get; set; }
        public string fromStreet1 { get; set; }
        public string fromStreet2 { get; set; }
        public string fromStreet3 { get; set; }
        public string fromZip { get; set; }
        public string returnToCity { get; set; }
        public string returnToCompany { get; set; }
        public string returnToCountry { get; set; }
        public string returnToEmail { get; set; }
        public string returnToName { get; set; }
        public string returnToPhone { get; set; }
        public string returnToState { get; set; }
        public string returnToStreet1 { get; set; }
        public string returnToStreet2 { get; set; }
        public string returnToStreet3 { get; set; }
        public string returnToZip { get; set; }
        public string quoteWeight { get; set; }
        public string weightUnit { get; set; }
        public string shipperReference { get; set; }
        public string name { get; set; }
        public string toCity { get; set; }
        public string toCompany { get; set; }
        public string toCountry { get; set; }
        public string toEmail { get; set; }
        public string toName { get; set; }
        public string toPhone { get; set; }
        public string toState { get; set; }
        public string toStreet1 { get; set; }
        public string toStreet2 { get; set; }
        public string toStreet3 { get; set; }
        public string toZip { get; set; }
        public string billingParty { get; set; }
        public string dutiableParty { get; set; }
        public string orderGroup { get; set; }
        public string department { get; set; }
        public string commercialInvoiceType { get; set; }
        public string customerNotes { get; set; }
        public string trackingStatus { get; set; }
        public string trackingStatusDetail { get; set; }
        public string trackingStatusTimestamp { get; set; }
        public string itemSku { get; set; }
        public string itemTitle { get; set; }
        public string itemPrice { get; set; }
        public string itemQuantity { get; set; }
        public string itemWeight { get; set; }
        public string shippingTotalOnOrder { get; set; }
        public string itemAttributes { get; set; }
    }
}
