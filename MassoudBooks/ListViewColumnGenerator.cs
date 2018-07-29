using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MassoudBooks
{
    class ListViewColumnGenerator
    {
        private ListView listView;
        private ObservableCollection<Book> currBooks;

        public ListViewColumnGenerator(ref ListView listView)
        {
            this.listView = listView;
            currBooks = new ObservableCollection<Book>();
        }
        public void generateColomns(Catagory catagory, ReadingStatus readingStatus, ListBox tags)
        {
            listView.DataContext = this;
            

            currBooks = getBooksCollection(catagory, readingStatus, tags);
            listView.ItemsSource = currBooks;
            GridView gv = new GridView();
            gv.AllowsColumnReorder = true;
           
            GridViewColumn gvc = new GridViewColumn();
            gvc.DisplayMemberBinding = new Binding("Name");
            gvc.Header = "Book";
            gv.Columns.Add(gvc);

           GridViewColumn gvc2 = new GridViewColumn();
           gvc2.DisplayMemberBinding = new Binding("PageNumber");
           gvc2.Header = "Page Number";
           gv.Columns.Add(gvc2);

            
            listView.View = gv;
            
        }
        private ObservableCollection<Book> getBooksCollection(Catagory catagory,ReadingStatus readingStatus, ListBox tags)
        {
            
            if (MainWindow.TheBooks.AllBooks.Count == 0)
                return MainWindow.TheBooks.AllBooks;
            if ((MainWindow.TheBooks.AllBooks.Where(b => (catagory == Catagory.Any ? true : b.Catagory == catagory) && (readingStatus == ReadingStatus.Any ? true : b.Status == readingStatus) && checkTags(b)).Count() == 0))
            {
                return new ObservableCollection<Book>();
            }
            bool checkTags(Book book)
                {
                    var tagscoll = tags.SelectedItems.Cast<string>().ToList();
                if (tagscoll.Count == 0)
                    {
                        return true;
                    }
                    bool bl = true;
                    for (int i = 0; i < tagscoll.Count; i++)
                    {
                        bl &= book.containsTag(tagscoll[i]);
                    }

                Console.WriteLine(bl);
                return bl;
                }
            bool checkSearch(Book book)
            {
              string nameToSearch=  ((MassoudBooks.MainWindow)Window.GetWindow(tags)).SearchBooksBox.Text;
                if (nameToSearch == "Search Books"||nameToSearch.Trim()=="")
                {
                    return true;
                }
                
                return book.Name.ToLower().Contains(nameToSearch.ToLower());
            }
            
            return new ObservableCollection<Book>(MainWindow.TheBooks.AllBooks.Where(b => (catagory == Catagory.Any?true:b.Catagory == catagory) && checkSearch(b) && (readingStatus == ReadingStatus.Any?true:b.Status == readingStatus) && checkTags(b)).ToList()) ;
              
        }
    }
}
