using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis_Monetary_Fund.DataModels
{
    public class Payment
    {
        int paymentId;
        int memberId;
        int loanId;
        string date;
        decimal amount;
        string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        int number;

        public int Number
        {
            get { return number; }
            set { number = value; }
        }
    

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        public int LoanId
        {
            get { return loanId; }
            set { loanId = value; }
        }
        

        public int MemberId
        {
            get { return memberId; }
            set { memberId = value; }
        }

        public int PaymentId
        {
            get { return paymentId; }
            set { paymentId = value; }
        }

    }
}
