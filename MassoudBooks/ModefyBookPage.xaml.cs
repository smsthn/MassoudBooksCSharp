using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace MassoudBooks
{
    /// <summary>
    /// Interaction logic for ModefyBookPage.xaml
    /// </summary>
    public partial class ModefyBookPage : Window
    {
        public static Book BookToModefy { get; set; }

        private ObservableCollection<string> modefiedTagsList { get; set; }
        private HashSet<string> previwslySlctdLst { get; set; } = new HashSet<string>();
        public ModefyBookPage()
        {
            InitializeComponent();
            this.BookNameTxtBox.Text = BookToModefy.Name;
            this.CatagoryCmboBx.ItemsSource = MainWindow.mCatagoies;
            this.CatagoryCmboBx.SelectedIndex = (int)BookToModefy.Catagory;
            this.ReadingStatusCmboBx.ItemsSource = MainWindow.mReadingStatuses;
            this.ReadingStatusCmboBx.SelectedIndex = (int)BookToModefy.Status;
            this.BookTagsCmboBx.ItemsSource = MainWindow.TheBooks.Tags;
            this.PagesTxtBx.Text = BookToModefy.PageNumber.ToString();
            Canvas.SetZIndex(this.BookTagsCmboBx, 2);
            selectedItemFromBookTagsCmboBx();
        }
        #region Main_Btn_Clicks
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ModifyBtn_Click(object sender, RoutedEventArgs e)
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
            string name = this.BookNameTxtBox.Text;
            ObservableCollection<string> tags;
            if (this.BookTagsCmboBx.Items.Count == 0)
            {
                tags = null;
            }
            else
            {
                var selectedTags = this.BookTagsCmboBx.SelectedItems.Cast<string>().ToList();
                tags = new ObservableCollection<string>(selectedTags);
            }
            BookToModefy.Name = name;
            BookToModefy.Catagory = catagory;
            BookToModefy.Status = readingStatus;
            BookToModefy.BookTags = tags;
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
            MainWindow.TheBooks.Tags = new ObservableCollection<string>(MainWindow.TheBooks.Tags.OrderBy(i => i));
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

        #region OtherStuff
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
            bool name = this.BookNameTxtBox.Text != null && this.BookNameTxtBox.Text != "" && this.BookNameTxtBox.Text.Length > 3;
            Console.WriteLine(name);
            bool catagory = this.CatagoryCmboBx.SelectedItem != null;
            Console.WriteLine(catagory);
            bool readingStatus = this.ReadingStatusCmboBx.SelectedItem != null;
            Console.WriteLine(readingStatus);
            return name && catagory && readingStatus;
        }
        #endregion
        #endregion

        #region Pages_stuff
        private void PagesTxtBx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;
            if (this.PagesTxtBx.Text.Trim() == "") return;
            BookToModefy.PageNumber = Int32.Parse(this.PagesTxtBx.Text);
        }

        private void AddPageBtn_Click(object sender, RoutedEventArgs e)
        {
            BookToModefy.PageNumber += 1;
            this.PagesTxtBx.Text = BookToModefy.PageNumber.ToString();
            this.PagesTxtBx.Focus();
            this.PagesTxtBx.SelectAll();
        }

        private void MinusPageBtn_Click(object sender, RoutedEventArgs e)
        {

            if (BookToModefy.PageNumber > 0)
            {
                BookToModefy.PageNumber -= 1;
                this.PagesTxtBx.Text = BookToModefy.PageNumber.ToString();
            }
            this.PagesTxtBx.Focus();
            this.PagesTxtBx.SelectAll();
        }
        #endregion
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MassoudBooks.MainWindow)Application.Current.MainWindow).getBookslist();
        }
        private void selectedItemFromBookTagsCmboBx()
        {
            var vs = this.BookTagsCmboBx.Items.Cast<string>().ToList();
            foreach (var tag in vs)
            {
                if (BookToModefy.containsTag(tag))
                {
                    var lbi = this.BookTagsCmboBx.Items.GetItemAt(vs.IndexOf(tag));
                    this.BookTagsCmboBx.SelectedItems.Add(lbi);
                }
            }
        }


        private void BookNameTxtBx_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


    }

}
