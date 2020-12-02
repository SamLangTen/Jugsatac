using System;
using System.Collections.Generic;
using System.Text;

namespace Jugsatac.Lib
{
    public class UpdateItem
    {
        public List<string> Names { get; set; }
        public string Comment { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string Hash { get; set; }
    }
}
