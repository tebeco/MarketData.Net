﻿using Newtonsoft.Json;

namespace MarketData.Common.Dto
{
    public class Quote : IProduceJson
    {
        public string Code { get; set; }
        public double QuoteValue { get; set; }

        public Quote(string code, double quote)
        {
            Code = code;
            QuoteValue = quote;
        }

        public static Quote FromJson(string input)
        {
            return JsonConvert.DeserializeObject<Quote>(input);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public override string ToString() => ToJson();
    }
}
