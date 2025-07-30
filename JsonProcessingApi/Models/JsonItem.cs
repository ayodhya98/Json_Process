using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JsonProcessingApi.Models
{
    public class JsonItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public int LineNum { get; set; }
        public string ItemDescription { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public string AccountCode { get; set; }
        public string TaxCode { get; set; }
        public double LineTotal { get; set; }
        public string U_Product { get; set; }
        public double U_Pcs { get; set; }
        public double U_weight { get; set; }
        public string U_CnCountry { get; set; }
        public string U_Btype { get; set; }
        public string U_TrackingNo { get; set; }
        public string U_ShipmentID { get; set; }
        public string U_Origin { get; set; }
        public string U_SubSeg { get; set; }
        public string U_SConPerson { get; set; }
        public string U_SAddress1 { get; set; }
        public string U_SAddress2 { get; set; }
        public string U_SCity { get; set; }
        public string U_SCountry { get; set; }
        public string U_CName { get; set; }
        public string U_CnConPerson { get; set; }
        public string U_CnAddressLine1 { get; set; }
        public string U_CnAddressLine2 { get; set; }
        public string U_CnCity { get; set; }
        public string U_WBD { get; set; }
        public string U_CashCr { get; set; }
        public string U_Type { get; set; }
        public double U_GW { get; set; }
        public double U_NetRate { get; set; }
        public double U_USDAmount { get; set; }
    }
}
