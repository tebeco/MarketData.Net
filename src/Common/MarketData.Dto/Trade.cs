using Newtonsoft.Json;

namespace MarketData.Dto
{
    public class Trade : IProduceJson
    {
        public string Code { get; set; }
        public int Quantity { get; set; }
        public double Nominal { get; set; }

        public Trade(string code, int quantity, double nominal)
        {
            Code = code;
            Quantity = quantity;
            Nominal = nominal;
        }

        public static Trade FromJson(string input)
        {
            return JsonConvert.DeserializeObject<Trade>(input);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override string ToString() => ToJson();

    }
}
