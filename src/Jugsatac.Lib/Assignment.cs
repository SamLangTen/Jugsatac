using System;
using System.Collections.Generic;
using System.Text;

namespace Jugsatac.Lib
{
    public class Assignment
    {
        public string Name { get; internal set; }
        public string IdentifierPattern { get; internal set; }
        public string SubmitterPattern { get; internal set; }
        public bool SubjectOnly { get; internal set; }
        public bool HideSubmitterName { get; internal set; }
    }
}
