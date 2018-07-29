using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassoudBooks
{
    [Serializable]
    public class SomeNotesHolder
    {
        public ObservableCollection<string> SomeNotes { get; }

        public SomeNotesHolder()
        {
            SomeNotes = new ObservableCollection<string>();
            if (SomeNotes.Count==0)
            {
                SomeNotes.Add(""); 
            }
        }
    }
}
