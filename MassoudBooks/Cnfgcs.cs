using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassoudBooks
{
    [Serializable]
    public class Cnfgcs
    {
        public string Path_Save { get; set; } 
        public string Path_SafeSave { get; set; }
        public string Path_NotesSave { get; set; }

        public Cnfgcs()
        {
        }
    }
}
