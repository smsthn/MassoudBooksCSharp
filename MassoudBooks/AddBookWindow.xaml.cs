using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MassoudBooks
{
    /// <summary>
    /// Interaction logic for AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        private ObservableCollection<string> modefiedTagsList { get; set; }
        private HashSet<string> previwslySlctdLst { get; set; } = new HashSet<string>();
        public AddBookWindow()
        {

            InitializeComponent();
            this.CatagoryCmboBx.ItemsSource = MainWindow.mCatagoies;
            this.ReadingStatusCmboBx.ItemsSource = MainWindow.mReadingStatuses;
            this.BookTagsCmboBx.ItemsSource = MainWindow.TheBooks.Tags;
            Canvas.SetZIndex(this.BookTagsCmboBx, 2);
        }

        #region Main_Button_Clicks

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!checkAllSelected())
            {
                MessageBox.Show("Fkn Slct All Bfor Trying To Add", "fill all", MessageBoxButton.OK);
                return;
            }
            string catagoryStr = this.CatagoryCmboBx.SelectedItem.ToString();
            Catagory catagory = (Catagory)Enum.Parse(typeof(Catagory), catagoryStr);
            string readingStr = this.ReadingStatusCmboBx.SelectedItem.ToString();
            ReadingStatus readingStatus = (ReadingStatus)Enum.Parse(typeof(ReadingStatus), readingStr);
            string name = this.BookNameTxtBx.Text;
            ObservableCollection<string> tags = new ObservableCollection<string>();
            if (this.BookTagsCmboBx.Items.Count == 0)
            {
                tags = null;
                Console.WriteLine("TagsWasNull");
            }
            else
            {
                var selectedTags = this.BookTagsCmboBx.SelectedItems.Cast<string>().ToList();
                Console.WriteLine("CollCoubt" + selectedTags.Count);
                tags = new ObservableCollection<string>(selectedTags);

            }
            MainWindow.TheBooks.Addbook(readingStatus, name, tags, catagory);
            MainWindow.TheBooks.AllBooks = new ObservableCollection<Book>(MainWindow.TheBooks.AllBooks.OrderBy(i => i.Name));
            ((MainWindow)Application.Current.MainWindow).getBookslist();
            this.Close();
        }

        #endregion


        #region Tag_Stuff

        #region Tag_Btns
        private void AddTagBtn_Click(object sender, RoutedEventArgs e)
        {
            this.TagTxtBx.IsEnabled = true;
            Canvas.SetZIndex(this.TagTxtBx, 3);

        }
        private void RemoveTagBtn_Click(object sender, RoutedEventArgs e)
        {


        }
        #endregion

        #region TagTxtBx
        private void TagTxtBx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter)
                return;

            e.Handled = true;
            Canvas.SetZIndex(this.TagTxtBx, 1);
            string newTag = TagTxtBx.Text;
            if (MainWindow.TheBooks.Tags.Contains(newTag))
                return;
            MainWindow.TheBooks.Tags.Add(newTag);
            
            this.TagTxtBx.Clear();
            this.TagTxtBx.IsEnabled = false;
        }
        private void TagTxtBx_LostFocus(object sender, RoutedEventArgs e)
        {
            this.TagTxtBx.IsEnabled = false;
            Canvas.SetZIndex(this.TagTxtBx, 1);
        }
        #endregion

        #region BookTagsCmboBx
        private void BookTagsCmboBx_MouseEnter(object sender, MouseEventArgs e)
        {
            this.BookTagsCmboBx.Focus();
            this.BookTagsCmboBx.Height = 100;
            this.BookTagsCmboBx.Width += 15;
            SearchTagTxtBx.Text = "";
            this.KeyDown += KeyDown_For_Tag_Search;
        }
        private void BookTagsCmboBx_MouseLeave(object sender, MouseEventArgs e)
        {
            this.BookTagsCmboBx.Height = 23;
            this.BookTagsCmboBx.Width -= 15;
            var selColl = this.BookTagsCmboBx.SelectedItems.Cast<string>().ToList();
            this.BookTagsCmboBx.SelectionChanged -= BookTagsCmboBx_SelectionChanged;
            //previwslySlctdLst.UnionWith(this.BookTagsCmboBx.SelectedItems.Cast<string>().ToList());
            this.BookTagsCmboBx.ItemsSource = MainWindow.TheBooks.Tags;
            this.BookTagsCmboBx.SelectionChanged += BookTagsCmboBx_SelectionChanged;
            addSelectedTagsToNextList(selColl);
            this.KeyDown -= KeyDown_For_Tag_Search;
        }
        private void BookTagsCmboBx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                previwslySlctdLst.UnionWith(e.AddedItems.Cast<string>().ToList());
            }
            if (e.RemovedItems.Count != 0)
            {
                previwslySlctdLst.ExceptWith(e.RemovedItems.Cast<string>().ToList());
            }
        }
        #endregion

        #region Other
        private void KeyDown_For_Tag_Search(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                SearchTagTxtBx.Text = "";
                var selColl = this.BookTagsCmboBx.SelectedItems.Cast<string>().ToList();
                this.BookTagsCmboBx.SelectionChanged -= BookTagsCmboBx_SelectionChanged;
                this.BookTagsCmboBx.ItemsSource = MainWindow.TheBooks.Tags;
                this.BookTagsCmboBx.SelectionChanged += BookTagsCmboBx_SelectionChanged;
                addSelectedTagsToNextList(selColl);
                return;
            }
            else if (e.Key == Key.Back)
            {
                if (SearchTagTxtBx.Text.Length - 1 < 0)
                {
                    Console.WriteLine("returned");
                    return;
                }
                SearchTagTxtBx.Text = SearchTagTxtBx.Text.Substring(0, SearchTagTxtBx.Text.Length - 1);
                Console.WriteLine(SearchTagTxtBx.Text);
                if (MainWindow.TheBooks.Tags.Where(t => t.Contains(SearchTagTxtBx.Text)).Count() == 0) return;
                modefiedTagsList = new ObservableCollection<string>(MainWindow.TheBooks.Tags.Where(t => t.Contains(SearchTagTxtBx.Text)));
                // savePreSelected();
                this.BookTagsCmboBx.SelectionChanged -= BookTagsCmboBx_SelectionChanged;
                this.BookTagsCmboBx.ItemsSource = modefiedTagsList;
                this.BookTagsCmboBx.SelectionChanged += BookTagsCmboBx_SelectionChanged;
                return;
            }
            SearchTagTxtBx.Text += e.Key.ToString();
            Console.WriteLine(SearchTagTxtBx.Text);
            if (MainWindow.TheBooks.Tags.Where(t => t.Contains(SearchTagTxtBx.Text)).Count() == 0) return;
            modefiedTagsList = new ObservableCollection<string>(MainWindow.TheBooks.Tags.Where(t => t.ToLower().Contains(SearchTagTxtBx.Text.ToLower())));
            //savePreSelected();
            this.BookTagsCmboBx.SelectionChanged -= BookTagsCmboBx_SelectionChanged;
            this.BookTagsCmboBx.ItemsSource = modefiedTagsList;
            this.BookTagsCmboBx.SelectionChanged += BookTagsCmboBx_SelectionChanged;
        }
        private void addSelectedTagsToNextList(List<string> selColl)
        {
            previwslySlctdLst.UnionWith(selColl);
            foreach (var selTag in previwslySlctdLst)
            {
                if (selTag != null)
                {
                    if (MainWindow.TheBooks.Tags.Contains(selTag))
                    {
                        var toselitm = this.BookTagsCmboBx.Items[MainWindow.TheBooks.Tags.IndexOf(selTag)];
                        this.BookTagsCmboBx.SelectedItems.Add(toselitm);
                    }
                }
            }
        }
        private bool checkAllSelected()
        {
            bool name = this.BookNameTxtBx.Text != null && this.BookNameTxtBx.Text != "" && this.BookNameTxtBx.Text.Length > 3;
            Console.WriteLine(name);
            bool catagory = this.CatagoryCmboBx.SelectedItem != null;
            Console.WriteLine(catagory);
            bool readingStatus = this.ReadingStatusCmboBx.SelectedItem != null;
            Console.WriteLine(readingStatus);
            return name && catagory && readingStatus;
        }
        #endregion

        #endregion


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MassoudBooks.MainWindow)Application.Current.MainWindow).getBookslist();
        }


    }
}
