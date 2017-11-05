using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCommander
{
    internal interface IDictProcess
    {
        void Load();
        void Navigate();
        IDictProcess PreviousPage { get; set; }
        IDictProcess NextPage { get; set; }
        IDictProcess TablePage { get; set; }
    }
}
