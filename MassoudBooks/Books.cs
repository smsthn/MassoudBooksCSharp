using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassoudBooks
{
    
    public enum ReadingStatus {Any, Reading, WannaRead,WannaRead2, YetToWannaRead , DelayedReading, Finished, Dropped}
    public enum Catagory {Any, Math, GameDev, CSharp, Java, Cs, Cpp, Emmbedded, PyAndML, Art, Android }
    [Serializable]
    public class Books
    { 
        
        
       public  ObservableCollection<Book> AllBooks { get; set; }
       public  ObservableCollection<string> Tags { get; set; }

        public Books()
        {
            AllBooks = new ObservableCollection<Book>();
            Tags = new ObservableCollection<string>();
        }
        public  void Addbook(ReadingStatus status, string name, ObservableCollection<string> tags, Catagory catagory)
       {
           AllBooks.Add(new Book(status, name, tags, catagory));
            
            TheSerialiingClass.serialize<Books>(MainWindow.cnfgcs.Path_Save, this);
       }
       public  void removeBookIfExists(Book book2)
        {
            if (AllBooks.Count == 0)
            {
                return;
            }
            Book bookToRemove = null;
            foreach (var book in AllBooks)
            {
                if (book.Uniqe_ID.Equals(book2.Uniqe_ID))
                {
                    bookToRemove = book;
                }
            }
            AllBooks.Remove(bookToRemove);
        }
       
    }
}
