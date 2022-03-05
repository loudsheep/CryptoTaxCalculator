using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CryptoTaxCalculator
{
    class Transaction
    {
        public int id;
        public float amount, rate;
        public bool income;

        public Transaction(int id, float amount, float rate, bool income)
        {
            this.id = id;
            this.amount = amount;
            this.rate = rate;
            this.income = income;
        }
    }

    public partial class MainPage : ContentPage
    {
        private List<Transaction> transactions = new List<Transaction>();
        private bool income = true;
        public MainPage()
        {
            InitializeComponent();
        }

        private void incomeBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            outcomeBox.IsChecked = !incomeBox.IsChecked;
            income = incomeBox.IsChecked;
        }

        private void outcomeBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            incomeBox.IsChecked = !outcomeBox.IsChecked;
            income = incomeBox.IsChecked;
        }

        private void AddBtn_Clicked(object sender, EventArgs e)
        {
            if(AmountEntry.Text == "" || ExchangeEntry.Text == "")
            {
                DisplayAlert("Error", "Enter data to all fields", "OK");
                return;
            }

            float amount, rate;
            if(!float.TryParse(AmountEntry.Text, out amount) || !float.TryParse(ExchangeEntry.Text, out rate))
            {
                DisplayAlert("Error", "Enter valid data", "OK");
                return;
            }

            Transaction t = new Transaction(MaxId(), amount, rate, income);
            transactions.Add(t);

            RedrawTransactions();
        }

        private int MaxId()
        {
            int max = 0;
            foreach(var t in transactions)
            {
                if (t.id > max) max = t.id;
            }

            return max;
        }

        private void RedrawTransactions()
        {
            listContent.Children.Clear();

            float sum = 0;
            for(int i=0; i<transactions.Count; i++)
            {
                sum += transactions[i].amount * transactions[i].rate * (transactions[i].income ? 1 : -1);
                StackLayout st = new StackLayout();
                st.Orientation = StackOrientation.Horizontal;
                st.HorizontalOptions = LayoutOptions.FillAndExpand;

                var tap = new TapGestureRecognizer();
                tap.Tapped += Tap_Tapped;
                st.GestureRecognizers.Add(tap);
                

                Label l = new Label();
                l.HorizontalOptions = LayoutOptions.FillAndExpand;
                l.Text = (i + 1) + ".";

                Label l2 = new Label();
                l2.Text = transactions[i].amount + " for " + transactions[i].rate;
                l2.HorizontalOptions = LayoutOptions.FillAndExpand;

                Label l3 = new Label();
                l3.Text = " = " + transactions[i].amount * transactions[i].rate;
                l3.HorizontalOptions = LayoutOptions.FillAndExpand;

                Label l4 = new Label();
                l4.Text = transactions[i].income ? "Income" : "Outgoings";
                l4.TextColor = transactions[i].income ? Color.Green : Color.Red;
                l4.HorizontalOptions = LayoutOptions.FillAndExpand;

                st.Children.Add(l);
                st.Children.Add(l2);
                st.Children.Add(l3);
                st.Children.Add(l4);
                listContent.Children.Add(st);
            }

            StackLayout st2 = new StackLayout();
            st2.Orientation = StackOrientation.Horizontal;
            st2.HorizontalOptions = LayoutOptions.FillAndExpand;
            st2.BackgroundColor = sum > 0 ? Color.LightGreen : sum < 0 ? Color.IndianRed : Color.LightGray;

            Label l1 = new Label();
            l1.Text = "Result: ";
            l1.FontSize = 15;
            l1.HorizontalOptions = LayoutOptions.FillAndExpand;

            Label l5 = new Label();
            l5.Text = "" + sum;
            l5.FontSize = 15;
            l5.HorizontalOptions = LayoutOptions.FillAndExpand;

            Label l6 = new Label();
            l6.Text = "Tax: " + (sum <= 0 ? "None" : Math.Round(sum * 0.19, 2) + " (19%)");
            l6.FontSize = 15;
            l6.HorizontalOptions = LayoutOptions.FillAndExpand;

            st2.Children.Add(l1);
            st2.Children.Add(l5);
            st2.Children.Add(l6);

            listContent.Children.Add(st2);
        }

        private int GetTapedID(StackLayout st)
        {
            for (int i = 0; i < listContent.Children.Count; i++) 
            {
                if (listContent.Children[i] == st) return i;
            }

            return -1;
        }

        private async void Tap_Tapped(object sender, EventArgs e)
        {
            if(sender is StackLayout)
            {
                StackLayout s = (StackLayout)sender;
                int idx = GetTapedID(s);

                if (idx == -1) return;

                bool answer = await DisplayAlert("Question?", "U sure to delete?", "Yes", "No");

                if(answer)
                {
                    transactions.RemoveAt(idx);
                    RedrawTransactions();
                }
            }
        }
    }
}
