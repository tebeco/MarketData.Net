using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreSse
{
    public class ServerSentEvent
    {
        private readonly string _data;

        public ServerSentEvent(string data)
        {
            _data = data;
        }

        public override string ToString()
        {
            var lines = _data.Split(new[] {"\n"}, StringSplitOptions.None);
            var builder = new StringBuilder();

            foreach (var line in lines)
            {
                builder.Append("data: " + line + "\n");
            }

            builder.Append("\n");
            return builder.ToString();
        }
    }
}