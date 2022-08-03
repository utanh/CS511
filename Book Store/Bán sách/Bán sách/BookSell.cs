using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;

namespace Bán_sách
{
    public partial class BookSell : Form
    {
        int second = 0;
        int picProminence = 0;
        string promotion0 = "Nhập APPDEPLAM giảm 20,000đ";
        string promotion1 = "Nhập UIT giảm 20%";
        string promotion2 = "Nhập STAYHOME giảm 50%";

        //User
        User user = new User("Hoàng Anh Tú", "Nam", 20, "0935906957", "KTX Khu B", 500000);

        //Page store
        int numPage = 1;
        int maxNumPage = 1;

        //Display book store
        PictureBox[,] pic_bookStore;
        Label[,] lb_bookStore;
        ManyBooks dataDisplay = new ManyBooks();
        HashSet<string> typesSelected = new HashSet<string>() { "Chính trị", "Pháp luật", "Công nghệ", "Kinh tế", "Nghệ thuật", "Giáo trình", "Tâm lý", "Tôn giáo", "Truyện tranh", "Tiểu thuyết", "Lịch sử" };

        //Data book store
        ManyBooks dataBase = new ManyBooks();

        //Trade
        ManyTrades historyTrades = new ManyTrades();
        List<User> historyCustom = new List<User>();

        Trade tradeProcessing = new Trade(new ManyBooks(), new DateTime(), "Đang xử lý");

        //Autocomplete txtFind
        AutoCompleteStringCollection find_auto = new AutoCompleteStringCollection();

        public BookSell()
        {
            InitializeComponent();

            pic_bookStore = new PictureBox[2, 5] { { picBookImage0, picBookImage1, picBookImage2, picBookImage3, picBookImage4 },
                                                    { picBookImage5, picBookImage6, picBookImage7, picBookImage8, picBookImage9 } };

            lb_bookStore = new Label[2, 5] { { lbBookPrice0, lbBookPrice1, lbBookPrice2, lbBookPrice3, lbBookPrice4 },
                                             { lbBookPrice5, lbBookPrice6, lbBookPrice7, lbBookPrice8, lbBookPrice9 } };

            // Data
            if (System.IO.File.Exists(@"..\..\..\data.txt"))
            {
                string[] lines = System.IO.File.ReadAllLines(@"..\..\..\data.txt");

                for (int i = 0; i < lines.Length;)
                {

                    Book newBook = new Book(lines[i], lines[i + 1], int.Parse(lines[i + 2]), lines[i + 3], int.Parse(lines[i + 4]), lines[i + 5]);
                    dataBase.books.Add(newBook);
                    i += 7;
                }
                dataBase.sortPriceAscen();

                dataDisplay.books = dataBase.getBooks();
            }
            loadDataDisplay();

            maxNumPage = dataDisplay.books.Count / 10 + 1;
            txtMaxNumPage.Text = maxNumPage.ToString();

            loadFind_auto();
        }

        //Load this.txtFind.AutoCompleteCustomSource
        private void loadFind_auto()
        {
            foreach(Book bk in dataBase.books)
            {
                find_auto.Add(bk.getName());
                find_auto.Add(bk.getAuthor());
            }

            this.txtFind.AutoCompleteCustomSource = find_auto;
        }
        //Load ToolTipBook
        private void loadToolTipBook()
        {
            for (int i = (numPage - 1) * 10; i < numPage * 10; i++)
            {
                int idx1D = i - (numPage - 1) * 10;

                if (i < dataDisplay.books.Count)
                {
                    if (imageListBookImg.Images.Count > 0 && dataDisplay.books.Count > 0 && pic_bookStore.Length > 0 && lb_bookStore.Length > 0)
                    {
                        string info = "Tên: " + dataDisplay.books[i].getName() + "\nTác giả: " + dataDisplay.books[i].getAuthor() + "\nThể loại: " + dataDisplay.books[i].getType().Replace(" |", ",") + "\nGiá bán: " + string.Format(new CultureInfo("vi-VN"), "{0:#,##0}", dataDisplay.books[i].getPrice()) + "vnđ";
                        toolTipBook.SetToolTip(pic_bookStore[idx1D / 5, idx1D % 5], info);
                        toolTipBook.SetToolTip(lb_bookStore[idx1D / 5, idx1D % 5], info);
                    }
                }
            }
        }
        //Load book to display in tabBuy
        private void loadDataDisplay()
        {
            maxNumPage = dataDisplay.books.Count / 10 + 1;
            txtMaxNumPage.Text = maxNumPage.ToString();

            for (int i = (numPage - 1) * 10; i < numPage * 10; i++)
            {
                int idx1D = i - (numPage - 1) * 10;

                if (i > dataDisplay.books.Count - 1)
                {
                    pic_bookStore[idx1D / 5, idx1D % 5].Visible = false;
                    lb_bookStore[idx1D / 5, idx1D % 5].Visible = false;
                }
                else
                {
                    if (imageListBookImg.Images.Count > 0 && dataDisplay.books.Count > 0 && pic_bookStore.Length > 0 && lb_bookStore.Length > 0)
                    {
                        pic_bookStore[idx1D / 5, idx1D % 5].Visible = true;
                        lb_bookStore[idx1D / 5, idx1D % 5].Visible = true;

                        pic_bookStore[idx1D / 5, idx1D % 5].Image = imageListBookImg.Images[dataDisplay.books[i].getIdxImg()];
                        lb_bookStore[idx1D / 5, idx1D % 5].Text = string.Format(new CultureInfo("vi-VN"), "{0:#,##0}", dataDisplay.books[i].getPrice()) + " vnđ";
                    }
                }
            }

            loadToolTipBook();
        }
        //Load history in listViewHisTrade
        private void loadHisTrade()
        {
            listViewHisTrade.Items.Clear();

            string[] arr = new string[9];
            ListViewItem itm;

            for (int i = 0; i < historyTrades.trades.Count; i++)
            {
                historyTrades.trades[i].checkStatusTrade();

                arr[0] = (i+1).ToString();
                arr[1] = historyTrades.trades[i].getDateTrade().ToString();
                arr[2] = historyCustom[i].getName();
                arr[3] = historyCustom[i].getPhoneNum();
                arr[4] = historyCustom[i].getAddress();
                arr[5] = historyTrades.trades[i].getDataTrade().books.Count.ToString();
                arr[6] = string.Format(new CultureInfo("vi-VN"), "{0:#,##0}", historyTrades.trades[i].totalMoney()) + " vnđ";
                arr[7] = string.Format(new CultureInfo("vi-VN"), "{0:#,##0}", historyTrades.trades[i].getShipMoney()) + " vnđ";
                arr[8] = historyTrades.trades[i].getStatusTrade();

                itm = new ListViewItem(arr);
                if (itm.SubItems[8].Text.Equals("Đang xử lý"))
                    itm.SubItems[8].ForeColor = Color.DarkGoldenrod;
                else if(itm.SubItems[8].Text.Equals("Hoàn thành"))
                    itm.SubItems[8].ForeColor = Color.Green;
                else
                    itm.SubItems[8].ForeColor = Color.Red;
                itm.UseItemStyleForSubItems = false;
                listViewHisTrade.Items.Add(itm);
            }
        }

        private void btnType_MouseMove(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.ForeColor = Color.Red;
            btn.Font = new Font(btn.Font, FontStyle.Bold);
        }

        private void btnType_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            btn.ForeColor = Color.Black;
            btn.Font = new Font(btn.Font, FontStyle.Regular);
        }

        private void picLogo_Click(object sender, EventArgs e)
        {
            tabControl.SelectedIndex = 0;
        }

        private void timerHome_Tick(object sender, EventArgs e)
        {
            second += timerHome.Interval;
            if (second >= 3000)
            {
                if (picProminence == 0)
                {
                    picProminence0.Visible = false;
                    picProminence1.Visible = true;
                    picProminence = 1;
                }
                else if (picProminence == 1)
                {
                    picProminence1.Visible = false;
                    picProminence2.Visible = true;
                    picProminence = 2;
                }
                else if (picProminence == 2)
                {
                    picProminence2.Visible = false;
                    picProminence0.Visible = true;
                    picProminence = 0;
                }

                second = 0;
            }
        }

        private void picProminence_MouseMove(object sender, MouseEventArgs e)
        {
            second = 0;
            this.panelProminence.Margin = new Padding(5, 25, 5, 135);
        }

        private void picProminence_MouseLeave(object sender, EventArgs e)
        {
            this.panelProminence.Margin = new Padding(0, 20, 0, 130);
        }

        private void BookSell_Load(object sender, EventArgs e)
        {
            toolTipPromotion.SetToolTip(picPromotion0, promotion0);
            toolTipPromotion.SetToolTip(picPromotion1, promotion1);
            toolTipPromotion.SetToolTip(picPromotion2, promotion2);
        }

        private void picAvata_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog fileOpen = new OpenFileDialog();
            fileOpen.Title = "Open Image file";
            fileOpen.Filter = "JPG Files (*.png)| *.png";

            if (fileOpen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string selectedFile = fileOpen.FileName;
                picAvata.Image = System.Drawing.Image.FromFile(@selectedFile);
                picAvata.SizeMode = PictureBoxSizeMode.Zoom;

                this.user.avata = @selectedFile;
            }
        }

        private void btnType_Click(object sender, EventArgs e)
        {
            tabControl.SelectedIndex = 1;

            Button btn = sender as Button;
            
            if (btn.Equals(btnTypeAll))
            {
                chínhTrị.Checked = true;
                phápLuật.Checked = true;
                côngNghệ.Checked = true;
                kinhTế.Checked = true;
                nghệThuật.Checked = true;
                giáoTrình.Checked = true;
                tâmLý.Checked = true;
                tônGiáo.Checked = true;
                truyệnTranh.Checked = true;
                tiểuThuyết.Checked = true;
                lịchSử.Checked = true;

                return;
            }

            chínhTrị.Checked = false;
            phápLuật.Checked = false;
            côngNghệ.Checked = false;
            kinhTế.Checked = false;
            nghệThuật.Checked = false;
            giáoTrình.Checked = false;
            tâmLý.Checked = false;
            tônGiáo.Checked = false;
            truyệnTranh.Checked = false;
            tiểuThuyết.Checked = false;
            lịchSử.Checked = false;

            if (btn.Equals(btnTypePolitical))
                chínhTrị.Checked = true;
            else if (btn.Equals(btnTypeLaw))
                phápLuật.Checked = true;
            else if (btn.Equals(btnTypeTech))
                côngNghệ.Checked = true;
            else if (btn.Equals(btnTypeEconomy))
                kinhTế.Checked = true;
            else if (btn.Equals(btnTypeArt))
                nghệThuật.Checked = true;
            else if (btn.Equals(btnTypeDocument))
                giáoTrình.Checked = true;
            else if (btn.Equals(btnTypePsychology))
                tâmLý.Checked = true;
            else if (btn.Equals(btnTypeReligion))
                tônGiáo.Checked = true;
            else if (btn.Equals(btnTypeComic))
                truyệnTranh.Checked = true;
            else if (btn.Equals(btnTypeNovel))
                tiểuThuyết.Checked = true;
            else if (btn.Equals(btnTypeHistory))
                lịchSử.Checked = true;
        }

        private void btnPageLeft_Click(object sender, EventArgs e)
        {
            if (numPage > 1)
            {
                numPage--;
                txtNumPage.Text = numPage.ToString();
            }
        }

        private void btnPageRight_Click(object sender, EventArgs e)
        {
            if (numPage < maxNumPage)
            {
                numPage++;
                txtNumPage.Text = numPage.ToString();
            }    
        }

        private void txtNumPage_TextChanged(object sender, EventArgs e)
        {
            loadDataDisplay();
        }

        private void toolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;

            if (tsmi.Checked)
                typesSelected.Add(tsmi.Text);
            else
                typesSelected.Remove(tsmi.Text);

            dataBase.setTypes(typesSelected);
            dataDisplay.books = dataBase.getBookInTypes();
            if(giáTăng.Checked)
                dataDisplay.sortPriceAscen();
            else
                dataDisplay.sortPriceDescen();
            loadDataDisplay();

            numPage = 1;
            txtNumPage.Text = "1";
        }

        private void giáTăng_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;

            if (tsmi.Checked)
            {
                giáGiảm.Checked = false;
                dataDisplay.sortPriceAscen();
                dataBase.sortPriceAscen();
                loadDataDisplay();
            }
        }

        private void giáGiảm_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;

            if (tsmi.Checked)
            {
                giáTăng.Checked = false;
                dataDisplay.sortPriceDescen();
                dataBase.sortPriceDescen();
                loadDataDisplay();
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            tabControl.SelectedIndex = 1;
            dataDisplay.books = dataBase.findBooks(txtFind.Text);
            loadDataDisplay();
        }

        private void txtFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tabControl.SelectedIndex = 1;
                dataDisplay.books = dataBase.findBooks(txtFind.Text);
                loadDataDisplay();
            }
        }


        //Show BookInfo window - send Trade from BookInfo to tradeProcessing
        private void getDataFromBookInfo(Book bk)
        {
            tradeProcessing.addBook(bk);
            lbNumInCart.Text = tradeProcessing.getDataTrade().books.Count.ToString();
        }
        private void picBookImage_Click(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            int i = 0;

            if (pic.Name.Equals("picBookImage0"))
                i = 0 + (numPage - 1) * 10;
            else if (pic.Name.Equals("picBookImage1"))
                i = 1 + (numPage - 1) * 10;
            else if (pic.Name.Equals("picBookImage2"))
                i = 2 + (numPage - 1) * 10;
            else if (pic.Name.Equals("picBookImage3"))
                i = 3 + (numPage - 1) * 10;
            else if (pic.Name.Equals("picBookImage4"))
                i = 4 + (numPage - 1) * 10;
            else if (pic.Name.Equals("picBookImage5"))
                i = 5 + (numPage - 1) * 10;
            else if (pic.Name.Equals("picBookImage6"))
                i = 6 + (numPage - 1) * 10;
            else if (pic.Name.Equals("picBookImage7"))
                i = 7 + (numPage - 1) * 10;
            else if (pic.Name.Equals("picBookImage8"))
                i = 8 + (numPage - 1) * 10;
            else if (pic.Name.Equals("picBookImage9"))
                i = 9 + (numPage - 1) * 10;

            var children = new BookInfo(dataDisplay.books[i], imageListBookImg.Images[dataDisplay.books[i].getIdxImg()]);
            children.sendBook = new BookInfo.sendBookToBookSell(getDataFromBookInfo);
            children.ShowDialog();
        }

        private void lbBookPrice_Click(object sender, EventArgs e)
        {
            Label lb = sender as Label;
            int i = 0;

            if (lb.Name.Equals("lbBookPrice0"))
                i = 0 + (numPage - 1) * 10;
            else if (lb.Name.Equals("lbBookPrice1"))
                i = 1 + (numPage - 1) * 10;
            else if (lb.Name.Equals("lbBookPrice2"))
                i = 2 + (numPage - 1) * 10;
            else if (lb.Name.Equals("lbBookPrice3"))
                i = 3 + (numPage - 1) * 10;
            else if (lb.Name.Equals("lbBookPrice4"))
                i = 4 + (numPage - 1) * 10;
            else if (lb.Name.Equals("lbBookPrice5"))
                i = 5 + (numPage - 1) * 10;
            else if (lb.Name.Equals("lbBookPrice6"))
                i = 6 + (numPage - 1) * 10;
            else if (lb.Name.Equals("lbBookPrice7"))
                i = 7 + (numPage - 1) * 10;
            else if (lb.Name.Equals("lbBookPrice8"))
                i = 8 + (numPage - 1) * 10;
            else if (lb.Name.Equals("lbBookPrice9"))
                i = 9 + (numPage - 1) * 10;

            tradeProcessing.addBook(dataDisplay.books[i]);
            lbNumInCart.Text = tradeProcessing.getDataTrade().books.Count.ToString();
        }

        private void lbNumInCart_TextChanged(object sender, EventArgs e)
        {
            lbTotal.Text = "Tổng tiền: " + string.Format(new CultureInfo("vi-VN"), "{0:#,##0}", tradeProcessing.totalMoney()) + " vnđ";
        }

        //Show TradeInfo window
        private void updateByTradeInfo(bool isSuccess, Trade tradeProcessing, User custom)
        {
            if (isSuccess == true)
            {
                this.historyTrades.trades.Add(tradeProcessing);
                this.historyCustom.Add(custom);
                this.tradeProcessing = new Trade(new ManyBooks(), new DateTime(), "Đang xử lý");
                this.user.setMoney(custom.getMoney());
            }
            else
            {
                this.tradeProcessing = tradeProcessing;
            }
        }
        private void lbTotal_Click(object sender, EventArgs e)
        {
            var children = new TradeInfo(tradeProcessing, new User(user.getName(), user.getSex(), user.getAge(), user.getPhoneNum(), user.getAddress(), user.getMoney()));
            children.sendInfor = new TradeInfo.sendDataToBookSell(updateByTradeInfo);
            children.ShowDialog();

            this.lbNumInCart.Text = this.tradeProcessing.getDataTrade().books.Count.ToString();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl.SelectedIndex == 2)
            {
                loadHisTrade();
            }    
        }

        private void listViewHisTrade_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ItemComparer sorter = listViewHisTrade.ListViewItemSorter as ItemComparer;

            if (sorter == null)
            {
                sorter = new ItemComparer(e.Column);
                sorter.Order = SortOrder.Ascending;
                listViewHisTrade.ListViewItemSorter = sorter;
            }
            // if clicked column is already the column that is being sorted
            if (e.Column == sorter.Column)
            {
                // Reverse the current sort direction
                if (sorter.Order == SortOrder.Ascending)
                    sorter.Order = SortOrder.Descending;
                else
                    sorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                sorter.Column = e.Column;
                sorter.Order = SortOrder.Ascending;
            }
            listViewHisTrade.Sort();
        }

        //updateByTradeHistory
        private void updateByTradeHistory(bool isCancel, Trade tradeHis)
        {
            if (isCancel == true)
            {
                this.user.addMoney(tradeHis.totalMoney() + tradeHis.getShipMoney());
                loadHisTrade();
            }
        }
        private void listViewHisTrade_DoubleClick(object sender, EventArgs e)
        {
            if (listViewHisTrade.SelectedItems.Count > 0)
            {
                int idx = int.Parse(listViewHisTrade.SelectedItems[0].SubItems[0].Text) - 1;
                var children = new TradeHistory(historyTrades.trades[idx], historyCustom[idx]);
                children.sendInfor = new TradeHistory.sendDataToBookSell(updateByTradeHistory);
                children.ShowDialog();
            }
        }

        private void btnFindTrade_Click(object sender, EventArgs e)
        {
            loadHisTrade();

            foreach(ListViewItem itm in listViewHisTrade.Items)
            {
                if (!DateTime.Parse(itm.SubItems[1].Text).Date.Equals(dtFindTrade.Value.Date))
                    itm.Remove();
            }    
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            var children = new Profile(this.user);
            children.ShowDialog();
            this.lbUserName.Text = this.user.getName();
            if(!this.user.avata.Equals(""))
                this.picAvata.Image = System.Drawing.Image.FromFile(this.user.avata);
        }

        private void picProminence_Click(object sender, EventArgs e)
        {
            string nameBook = "";

            if (picProminence == 2)
                nameBook = "Sự im lặng của bầy cừu";
            else if (picProminence == 1)
                nameBook = "Tôi thấy hoa vàng trên cỏ xanh";
            else
                nameBook = "Đắc nhân tâm";

            if (dataBase.findBooks(nameBook).Count > 0)
            {
                var children = new BookInfo(dataBase.findBooks(nameBook)[0], imageListBookImg.Images[dataBase.findBooks(nameBook)[0].getIdxImg()]);
                children.sendBook = new BookInfo.sendBookToBookSell(getDataFromBookInfo);
                children.ShowDialog();
            }
        }
    }

    public class Book
    {
        private string name = "";
        private string author = "";
        private int price = 0;
        private string type = "";
        private string quote = "";
        private int idxImg = 0;

        public Book(string name = "", string author = "", int price = 0, string type = "", int idxImg = 0, string quote = "")
        {
            this.name = name;
            this.author = author;
            this.price = price;
            this.type = type;
            this.quote = quote;
            this.idxImg = idxImg;
        }

        public string getName()
        {
            return this.name;
        }
        public string getAuthor()
        {
            return this.author;
        }
        public int getPrice()
        {
            return this.price;
        }
        public string getType()
        {
            return this.type;
        }
        public string getQuote()
        {
            return this.quote;
        }
        public int getIdxImg()
        {
            return this.idxImg;
        }
    }

    public class ManyBooks
    {
        public List<Book> books = new List<Book>();
        private HashSet<string> types = new HashSet<string>();

        public void setTypes(HashSet<string> types)
        {
            this.types = types;
        }
        public HashSet<string> getTypes()
        {
            return this.types;
        }

        public List<Book> getBooks()
        {
            return this.books;
        }

        public List<Book> getBookInTypes()
        {
            List<Book> result = new List<Book>();

            foreach (string type in types)
            {
                result.AddRange(books.FindAll(x => !result.Contains(x) && x.getType().Contains(type)));
            }

            return result;
        }

        public List<Book> findBooks(string keyWord)
        {
            keyWord = keyWord.ToLower();

            List<Book> result = new List<Book>();

            foreach (Book bk in books)
            {
                string name = bk.getName().ToLower();
                string author = bk.getAuthor().ToLower();

                if (keyWord.Contains(name) || keyWord.Contains(author) || name.Contains(keyWord) || author.Contains(keyWord))
                    result.Add(bk);
            }

            return result;
        }

        public void sortPriceAscen()
        {
            books.Sort((x, y) => x.getPrice().CompareTo((y.getPrice())));
        }
        public void sortPriceDescen()
        {
            books.Sort((x, y) => y.getPrice().CompareTo((x.getPrice())));
        }
    }

    public class Trade
    {
        private ManyBooks dataTrade = new ManyBooks();
        private DateTime dateTrade = new DateTime();
        private Dictionary<Book, int> numEachBook = new Dictionary<Book, int>();
        private string statusTrade = "Đang xử lý";
        private string reasonCancel = "";
        private int shipMoney = 30000;
        private float sale = 0;

        public Trade(ManyBooks dataTrade, DateTime dateTrade, string statusTrade)
        {
            this.dataTrade = dataTrade;
            this.dateTrade = dateTrade;
            this.statusTrade = statusTrade;
        }

        //Get
        public ManyBooks getDataTrade()
        {
            return this.dataTrade;
        }
        public DateTime getDateTrade()
        {
            return this.dateTrade;
        }
        public Dictionary<Book, int> getNumEachBook()
        {
            return this.numEachBook;
        }
        public string getStatusTrade()
        {
            return this.statusTrade;
        }
        public string getReasonCancel()
        {
            return this.reasonCancel;
        }
        public int getShipMoney()
        {
            return this.shipMoney;
        }
        public float getSale()
        {
            return this.sale;
        }
        //Set
        public void calNumEachBook()
        {
            foreach (Book bk in dataTrade.books)
            {
                if (this.numEachBook.Keys.Contains(bk))
                    this.numEachBook[bk] += 1;
                else
                    this.numEachBook.Add(bk, 1);
            }    
        }
        public void checkStatusTrade()
        {
            if (!this.statusTrade.Equals("Hủy bỏ") && DateTime.Now.CompareTo(this.dateTrade) >= 0)
                this.statusTrade = "Hoàn thành";
        }
        public void cancelTrade()
        {
            this.statusTrade = "Hủy bỏ";
        }
        public void setDateTrade(DateTime date)
        {
            this.dateTrade = date;
        }
        public void setReasonCancel(string reasonCancel)
        {
            this.reasonCancel = reasonCancel;
        }
        public void setShipMoney(int shipMoney)
        {
            this.shipMoney = shipMoney;
        }
        public void setSale(float sale)
        {
            this.sale = sale;
        }

        //Calculate money
        public int totalMoney()
        {
            int total = 0;
            foreach (Book bk in dataTrade.books)
                total += bk.getPrice();
           
            if (0 < sale && sale <= 1)
                return (int)(total * (1 - sale));
            else
            {
                if (sale < total)
                    return (int)(total - sale);
                else
                    return 0;
            }

        }

        //Add
        public void addBook(Book bk)
        {
            dataTrade.books.Add(bk);
        }
        //Remove
        public void removeBook(int idx)
        {
            dataTrade.books.RemoveAt(idx);
        }
    }

    public class ManyTrades
    {
        public List<Trade> trades = new List<Trade>();
    }

    public class User
    {
        private string name = "";
        private string sex = "";
        private int age = 0;
        private string phoneNum = "";
        private string address = "";
        private int money = 0;
        public string avata = "";

        public User(string name = "", string sex = "", int age = 0, string phoneNum = "", string address = "", int money = 0)
        {
            this.name = name;
            this.sex = sex;
            this.age = age;
            this.phoneNum = phoneNum;
            this.address = address;
            this.money = money;
        }

        //get
        public string getName()
        {
            return this.name;
        }
        public string getSex()
        {
            return this.sex;
        }
        public int getAge()
        {
            return this.age;
        }
        public string getPhoneNum()
        {
            return this.phoneNum;
        }
        public string getAddress()
        {
            return this.address;
        }
        public int getMoney()
        {
            return this.money;
        }

        //set
        public void setName(string name)
        {
            this.name = name;
        }
        public void setSex(string sex)
        {
            this.sex = sex;
        }
        public void setAge(int age)
        {
            this.age = age;
        }
        public void setPhoneNum(string phoneNum)
        {
            this.phoneNum = phoneNum;
        }
        public void setAddress(string address)
        {
            this.address = address;
        }
        public void setMoney(int money)
        {
            this.money = money;
        }

        //Add money
        public void addMoney(int moreMoney)
        {
            this.money += moreMoney;
        }
        //Sub money
        public void subMoney(int payMoney)
        {
            this.money -= payMoney;
        }
    }

    //Sort listview
    public class ItemComparer : IComparer
    {
        //column used for comparison
        public int Column { get; set; }

        //Order of sorting
        public SortOrder Order { get; set; }

        public ItemComparer(int colIndex)
        {
            Column = colIndex;
            Order = SortOrder.None;

        }
        public int Compare(object a, object b)
        {
            int result;

            ListViewItem itemA = a as ListViewItem;
            ListViewItem itemB = b as ListViewItem;
            if (itemA == null && itemB == null)
                result = 0;
            else if (itemA == null)
                result = -1;
            else if (itemB == null)
                result = 1;

            if (itemA == itemB)
                result = 0;

            // datetime comparison
            DateTime x1, y1;
            // Parse the two objects passed as a parameter as a DateTime.
            if (!DateTime.TryParse(itemA.SubItems[Column].Text, out x1))
                x1 = DateTime.MinValue;
            if (!DateTime.TryParse(itemB.SubItems[Column].Text, out y1))
                y1 = DateTime.MinValue;
            result = DateTime.Compare(x1, y1);

            if (x1 != DateTime.MinValue && y1 != DateTime.MinValue)
                goto done;

            //numeric comparison
            decimal x2, y2;
            if (!Decimal.TryParse(itemA.SubItems[Column].Text, out x2))
                x2 = Decimal.MinValue;
            if (!Decimal.TryParse(itemB.SubItems[Column].Text, out y2))
                y2 = Decimal.MinValue;
            result = Decimal.Compare(x2, y2);

            if (x2 != Decimal.MinValue && y2 != Decimal.MinValue)
                goto done;

            //alphabetic comparison
            result = String.Compare(itemA.SubItems[Column].Text, itemB.SubItems[Column].Text);



            done:
            // if sort order is descending.
            if (Order == SortOrder.Descending)
                // Invert the value returned by Compare.
                result *= -1;
            return result;

        }
    }
}
