using System.Collections.Generic;

namespace FBS.Worker
{
    public class UpdateSettingsRequestT
    {
        public string LogLevel { get; set; }

        public List<string> LogTags { get; set; }
    }
}
