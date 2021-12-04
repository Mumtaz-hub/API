using System.Collections.Generic;

namespace ViewModel
{
    public class FileDataViewModel  
    {
        public List<KeyValuePair<string, string>> FormData { get; set; }

        public FileViewModel FileViewModel { get; set; }
    }
}
