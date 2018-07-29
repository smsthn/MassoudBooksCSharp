using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace MassoudBooks
{
    class SomeNotesHolderConstroller
    {
        public bool Loaded { get; private set; } = false;
        public SomeNotesHolder SomeNotesHolder { get; }

        public SomeNotesHolderConstroller()
        {
            SomeNotesHolder = TheSerialiingClass.deserialize<SomeNotesHolder>(MainWindow.cnfgcs.Path_NotesSave);
            

        }

        public void Load()
        {
            
            Loaded = true;
        }

        public void Unload()
        {
            
            Loaded = false;
            TheSerialiingClass.serialize(MainWindow.cnfgcs.Path_NotesSave, SomeNotesHolder);
        }
        private void connectNotesSourceAndTxtBxes(TextBox tb, Action<string,TextBox> action)
        {
           
        }

        
    }
}
