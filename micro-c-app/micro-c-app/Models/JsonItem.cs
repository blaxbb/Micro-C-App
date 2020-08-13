namespace micro_c_app.Models
{

    public class JsonItem
    {
        public string context { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string[] image { get; set; }
        public string description { get; set; }
        public string brand { get; set; }
        public string sku { get; set; }
        public string mpn { get; set; }
        public Offers offers { get; set; }
    }

    public class Offers
    {
        public string type { get; set; }
        public string url { get; set; }
        public string priceCurrency { get; set; }
        public string price { get; set; }
        public string priceValidUntil { get; set; }
        public string availability { get; set; }
        public string itemCondition { get; set; }
        public PotentialAction potentialAction { get; set; }
        public DeliveryLeadTime deliveryLeadTime { get; set; }
        public AvailableAtOrFrom availableAtOrFrom { get; set; }
    }

    public class PotentialAction
    {
        public string type { get; set; }
    }

    public class DeliveryLeadTime
    {
        public string type { get; set; }
        public string value { get; set; }
    }

    public class AvailableAtOrFrom
    {
        public string type { get; set; }
        public string branchCode { get; set; }
        public string name { get; set; }
    }

}
