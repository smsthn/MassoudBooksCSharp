using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassoudBooks
{   [Serializable]
   public class Book
    {
        public string Uniqe_ID { get; }
        public ReadingStatus Status { get; set; }
        public string Name { get; set; }
        public int PageNumber { get; set; }
        public Catagory Catagory { get; set; }
        [NonSerialized]
        public ObservableCollection<string> mTags;

        public ObservableCollection<string> Notes { get; set; }
        public ObservableCollection<string> BookTags { get => mTags; set => mTags = value; }
        public Book(ReadingStatus status, string name, ObservableCollection<string> tags,Catagory catagory)
            :this()
        {
            
            Notes = new ObservableCollection<string>();
            Status = status;
            Name = name;
            PageNumber = 0;
            this.Catagory = catagory;
            mTags = new ObservableCollection<string>();
            if (tags != null)
                BookTags.Clear();
                BookTags = tags;
        }

        public Book()
        {
            Uniqe_ID = Guid.NewGuid().ToString();
        }

        public void addTag(string tag)
        {
            if (BookTags.Contains(tag))
                return;
            BookTags.Add(tag);
            if (MainWindow.TheBooks.Tags.Contains(tag))
                return;
            MainWindow.TheBooks.Tags.Add(tag);
        }
        public bool containsTag(string tag)
        {
            return BookTags.Contains(tag);
        }
        public void AddNote(string noteStr)
        {
            Notes.Add(noteStr);
        }
        public void RemoveNoteAt(int index)
        {
            Notes.RemoveAt(index);
        }
        
    }
}
        
