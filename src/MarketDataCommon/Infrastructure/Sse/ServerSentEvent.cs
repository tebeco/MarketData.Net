using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreSse
{
    public class ServerSentEvent
    {
        private static readonly string[] LogLevels = {"DEBUG", "INFO", "WARN", "ERROR"};
        private readonly string _type;
        private readonly string _data;

        public ServerSentEvent(string type, string data)
        {
            _type = type;
            _data = data;
        }

        public override string ToString()
        {
            var lines = _data.Split(new[] {"\n"}, StringSplitOptions.None);
            var builder = new StringBuilder();

            if (LogLevels.Any(logLevels => logLevels.Contains(_type)))
            {
                builder.Append("event: " + _type + "\n");
            }

            foreach (var line in lines)
            {
                builder.Append("data: " + line + "\n");
            }

            builder.Append("\n");
            return builder.ToString();
        }
    }
}