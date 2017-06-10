using Newtonsoft.Json;

namespace MarketData.Dto
{
    public class Stock : IProduceJson
    {
        public string Code { get; set; }
        public string CompanyName { get; set; }
        public string Market { get; set; }

        public Stock(string code, string companyName, string market)
        {
            Code = code;
            CompanyName = companyName;
            Market = market;
        }

        public static Stock FromJson(string input)
        {
            return JsonConvert.DeserializeObject<Stock>(input);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override string ToString() => ToJson();
    }
}
