using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Bán_sách
{
    public partial class BookInfo : Form
    {
        public delegate void sendBookToBookSell(Book bk);
        public sendBookToBookSell sendBook;

        Book bk = new Book();

        public BookInfo(Book bk, Image infoImg)
        {
            InitializeComponent();

            this.bk = bk;

            picBook.Image = infoImg;
            infoName.Text = bk.getName();
            infoAuthor.Text = bk.getAuthor();
            infoType.Text = bk.getType().Replace(" |", ",");
            if (bk.getQuote().Length > 200)
                lbQuote.Text = bk.getQuote().Substring(0, 200) + "...";
            else
                lbQuote.Text = bk.getQuote();
            infoPrice.Text = string.Format(new CultureInfo("vi-VN"), "{0:#,##0}", bk.getPrice()) + "vnđ";

            toolTipQuote.SetToolTip(lbQuote, bk.getQuote());
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            sendBook(bk);
            this.Close();
        }
    }
}
