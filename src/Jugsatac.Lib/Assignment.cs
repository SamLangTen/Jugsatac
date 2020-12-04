using System;
using System.Collections.Generic;
using System.Text;

namespace Jugsatac.Lib
{
    public class Assignment
    {
        public string Name { get; set; }
        public string IdentifierPattern { get; set; }
        public string SubmitterPattern { get; set; }
        public bool SubjectOnly { get; set; }
        public bool HideSubmitterName { get; set; }
    }
}
