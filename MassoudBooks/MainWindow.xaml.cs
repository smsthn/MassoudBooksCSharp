using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace MassoudBooks
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static Cnfgcs cnfgcs;
        public static ObservableCollection<string> mCatagoies;
        public static ObservableCollection<string> mReadingStatuses;
        public static Books TheBooks { get; set; }
        private ListViewColumnGenerator lscg;
        private SomeNotesHolderConstroller notesHolderConstroller;
        private string mCatagory = "Math";
        private string mReadingStatus = "Reading";
        private string someNoteBeforeChange = "";
        private int someNoteSelectedIndex = 0;
        private List<string> mTag = null;
        public MainWindow()
        {
            if(!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\cnfg.xml"))
            {
                Cnfgcs c = new Cnfgcs();
                c.Path_NotesSave = System.AppDomain.CurrentDomain.BaseDirectory + @"\saves\notessave.xml";
                c.Path_SafeSave = System.AppDomain.CurrentDomain.BaseDirectory + @"\saves\safesave.xml";
                c.Path_Save = System.AppDomain.CurrentDomain.BaseDirectory + @"\saves\save.xml";
                TheSerialiingClass.serialize<Cnfgcs>(System.AppDomain.CurrentDomain.BaseDirectory + @"\cnfg.xml",c);
            }
            if(!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"\saves"))
            {
                Directory.CreateDirectory(System.AppDomain.CurrentDomain.BaseDirectory + @"\saves");
            }
            cnfgcs = TheSerialiingClass.deserialize<Cnfgcs>(System.AppDomain.CurrentDomain.BaseDirectory+@"\cnfg.xml");
            TheBooks = TheSerialiingClass.deserialize<Books>(cnfgcs.Path_Save);
            mCatagoies = new ObservableCollection<string>(Enum.GetNames(typeof(Catagory)).ToList<string>());
            mReadingStatuses = new ObservableCollection<string>(Enum.GetNames(typeof(ReadingStatus)).ToList<string>());
            InitializeComponent();
            lscg = new ListViewColumnGenerator(ref this.BooksLstView);
            this.CatagoryLstBx.ItemsSource = mCatagoies;
            this.ReadingStatusLstBx.ItemsSource = mReadingStatuses;
            this.TagsLstBx.ItemsSource = TheBooks.Tags;
        

            notesHolderConstroller = new SomeNotesHolderConstroller();
            this.SomeNotesLstBx.ItemsSource = notesHolderConstroller.SomeNotesHolder.SomeNotes;
            Console.WriteLine(cnfgcs.Path_Save);
        }
        #region ListViews_SelectionChanged
            private void CatagoryLstBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                string lbi = this.CatagoryLstBx.SelectedItem.ToString();
                mCatagory = lbi;
                getBookslist();
            }

            private void ReadingStatusLstBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                string lbi = this.ReadingStatusLstBx.SelectedItem.ToString();
                mReadingStatus = lbi;
                getBookslist();
            }

            private void TagsLstBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                var lbi = this.TagsLstBx.SelectedItems.Cast<string>().ToList();
                mTag = lbi;
                getBookslist();
            }

            private void BooksLstView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.BooksLstView.SelectedItems.Count != 0)
            {
                var booksColl = this.BooksLstView.Items.Cast<Book>().ToList();
                int index = this.BooksLstView.SelectedIndex;
                Book book = booksColl[index];
                if (book == null) return;
                this.BookNotesList.ItemsSource = book.Notes;
            }
            else
            {
                this.BookNotesList.ItemsSource = null;
            }
            this.BooksCountLabel.Content = this.BooksLstView.Items.Count;
        }
        #endregion

        

        #region Button_clicks

            #region Main
            private void AddButton_Click(object sender, RoutedEventArgs e)
            {
                AddBookWindow addBookWindow = new AddBookWindow();
                addBookWindow.Show();
            }
            private void ModifyButton_Click(object sender, RoutedEventArgs e)
            {
                if (BooksLstView.SelectedItem == null)
                    return;
                int selectedindex = BooksLstView.SelectedIndex;
                var bkcl = (ObservableCollection<Book>)BooksLstView.ItemsSource;

                ModefyBookPage.BookToModefy = bkcl[selectedindex];
                Console.WriteLine(bkcl[selectedindex].Name);
                ModefyBookPage modefyBookPage = new ModefyBookPage();
                modefyBookPage.Show();

            }
            private void DeleteBookBtn_Click(object sender, RoutedEventArgs e)
            {
                if (BooksLstView.SelectedItem == null)
                    return;
                int selectedindex = BooksLstView.SelectedIndex;
                var bkcl = (ObservableCollection<Book>)BooksLstView.ItemsSource;
                Book book = bkcl[selectedindex];
                TheBooks.removeBookIfExists(book);
                getBookslist();
            }
            private void SafeSaveBtn_Click(object sender, RoutedEventArgs e)
            {
                TheBooks.AllBooks = new ObservableCollection<Book>(TheBooks.AllBooks.Distinct());
                TheBooks.Tags = new ObservableCollection<string>(TheBooks.Tags.Distinct());
                foreach (var book in TheBooks.AllBooks)
                {
                    book.BookTags = new ObservableCollection<string>(book.BookTags.Distinct());
                }
                TheSerialiingClass.serialize<Books>(cnfgcs.Path_SafeSave, TheBooks);
            }
            #endregion
        
            private void BookTagslst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        
        #endregion
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TheBooks.AllBooks = new ObservableCollection<Book>(TheBooks.AllBooks.Distinct());
            TheBooks.Tags = new ObservableCollection<string>(TheBooks.Tags.Distinct());
            TheSerialiingClass.serialize<Books>(cnfgcs.Path_Save, TheBooks);
            saveSomeNote();
        }



        #region SomeNotes

        private void SomeNotesLabel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!notesHolderConstroller.Loaded)
            {
                notesHolderConstroller.Load();
                this.SomeNotesExpandableGrid.Margin = new Thickness(71, 0, -688, 0);
                return;
            }
            notesHolderConstroller.Unload();
            this.SomeNotesExpandableGrid.Margin = new Thickness(71, 0, 0, 0);

        }

        private void SomeNotesLstBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = this.SomeNotesLstBx.SelectedIndex;
            if (index < 0 || index > notesHolderConstroller.SomeNotesHolder.SomeNotes.Count) return;
            someNoteSelectedIndex = index;
            this.SomeNotesTextBlock.Text = someNoteBeforeChange = notesHolderConstroller.SomeNotesHolder.SomeNotes[index];
            
            
        }
        private void RemoveNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            var booksColl = this.BooksLstView.Items.Cast<Book>().ToList();
            int index = this.BooksLstView.SelectedIndex;
            Book book = booksColl[index];
            if (book == null) return;
            book.RemoveNoteAt(BookNotesList.SelectedIndex);
        }

        private void AddNoteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.BooksLstView.SelectedItems.Count == 0) return;
            var booksColl = this.BooksLstView.Items.Cast<Book>().ToList();
            int index = this.BooksLstView.SelectedIndex;
            Book book = booksColl[index];
            if (book == null) return;

            TextBox noteTextBox = this.AddNoteTxtBx;
            noteTextBox.IsEnabled = true;
            noteTextBox.Text = "AddNotesHere";
            noteTextBox.LostFocus += (s, ee) =>
            {
                noteTextBox.Clear();
                noteTextBox.IsEnabled = false;
            };
            noteTextBox.KeyDown += (s, ee) =>
            {
                if (ee.Key != System.Windows.Input.Key.Enter) return;
                book.AddNote(noteTextBox.Text);

                noteTextBox.Clear();
                noteTextBox.IsEnabled = false;
            };
        }
        #region SomeNotes_Manipulation
        private void SomeNotesTextBlock_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape) { cancelSaveSomeNote(); return; }
            saveSomeNote();
            return;

           
        }
        private void SomeNotesAddButton_Click(object sender, RoutedEventArgs e)
        {
            addNewSomeNote();
            this.SomeNotesTextBlock.Focus();
        }
        private void SomeNotesDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            deleteSomeNote();
        }
        private void saveSomeNote()
        {
            
            notesHolderConstroller.SomeNotesHolder.SomeNotes[someNoteSelectedIndex] = this.SomeNotesTextBlock.Text;
        }
        private void cancelSaveSomeNote()
        {
            notesHolderConstroller.SomeNotesHolder.SomeNotes[someNoteSelectedIndex] = someNoteBeforeChange;
            this.SomeNotesLstBx.SelectedIndex = someNoteSelectedIndex;
        }
        private void addNewSomeNote()
        {
            notesHolderConstroller.SomeNotesHolder.SomeNotes.Add("");
            this.SomeNotesLstBx.SelectedIndex = notesHolderConstroller.SomeNotesHolder.SomeNotes.Count -1;
        }
        private void deleteSomeNote()
        {
            notesHolderConstroller.SomeNotesHolder.SomeNotes.RemoveAt(someNoteSelectedIndex);
        }
        private void saveSomeNoteToFile()
        {
            TheSerialiingClass.serialize(cnfgcs.Path_NotesSave, notesHolderConstroller.SomeNotesHolder);
        }



        #endregion

        #endregion

        

        private void ListViewItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (BooksLstView.SelectedItem == null)
                return;
            int selectedindex = BooksLstView.SelectedIndex;
            var bkcl = (ObservableCollection<Book>)BooksLstView.ItemsSource;

            ModefyBookPage.BookToModefy = bkcl[selectedindex];
            ModefyBookPage modefyBookPage = new ModefyBookPage();
            modefyBookPage.Show();
        }



        #region SearchBooksBox
        private void SearchBooksBox_GotFocus(object sender, RoutedEventArgs e)
        {
            this.SearchBooksBox.Text = "";
        }
        private void SearchBooksBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.SearchBooksBox.Text = "Search Books";
        }
        private void SearchBooksBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.AddButton.Focus();
                getBookslist();
                return;
            }
            getBookslist();
        } 
        #endregion

        public void getBookslist()
        {
            Catagory c = (Catagory)Enum.Parse(typeof(Catagory), mCatagory);
            ReadingStatus r = (ReadingStatus)Enum.Parse(typeof(ReadingStatus), mReadingStatus);
            Console.WriteLine(mCatagory + " " + mReadingStatus);
            lscg.generateColomns(c, r, TagsLstBx);
        }

        private void BooksLstView_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.MouseDoubleClick += ListViewItem_MouseDown;
        }

        private void BooksLstView_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.MouseDoubleClick -= ListViewItem_MouseDown;
        }
    }

}
