using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis_Monetary_Fund.DataModels
{

    public enum LOAN_STATES { PENDING, UN_SETTLED, SETTLED };

    public class Loan
    {
        int loanId;
        int memberId;
        int fundId;
        String issueDate;
        LOAN_STATES state;
        int turn;
        decimal credit;
        decimal indebted;

        public decimal Indebted
        {
            get { return indebted; }
            set { indebted = value; }
        }

        public decimal Credit
        {
            get { return credit; }
            set { credit = value; }
        }

        public int Turn
        {
            get { return turn; }
            set { turn = value; }
        }

        public LOAN_STATES State
        {
            get { return state; }
            set { state = value; }
        }

        public String IssueDate
        {
            get { return issueDate; }
            set { issueDate = value; }
        }

        public int FundId
        {
            get { return fundId; }
            set { fundId = value; }
        }

        public int MemberId
        {
            get { return memberId; }
            set { memberId = value; }
        }

        public int LoanId
        {
            get { return loanId; }
            set { loanId = value; }
        }
    }
}
