using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Pardis_Monetary_Fund.DataModels
{
    public class Fund : IComparable, INotifyPropertyChanged
    {
        int fundId;
        int number;
        decimal installmentAmount;
        String establishmentDate;
        int membersCount;
        decimal loanAmount;
        bool hasLoan;
        decimal investment;

        public decimal Investment
        {
            get { return investment; }
            set { 
                investment = value;
                OnPropertyChanged("Investment");
            }
        }


        public bool HasLoan
        {
            get { return hasLoan; }
            set { hasLoan = value; OnPropertyChanged("HasLoan"); }
        }

        public decimal LoanAmount
        {
            get { return MembersCount*InstallmentAmount; }
            set { loanAmount = value; OnPropertyChanged("LoanAmount"); }
        }

        public int MembersCount
        {
            get { return membersCount; }
            set 
            { 
                membersCount = value;
                LoanAmount = value * InstallmentAmount;
                OnPropertyChanged("MembersCount");
            }
        }

        public String EstablishmentDate
        {
            get { return establishmentDate; }
            set { establishmentDate = value; OnPropertyChanged("EstablishmentDate"); }
        }

        public decimal InstallmentAmount
        {
            get { return installmentAmount; }
            set
            {
                installmentAmount = value;
                LoanAmount = MembersCount * value;
                OnPropertyChanged("InstallmentAmount");
            }
        }
        

        public int Number
        {
            get { return number; }
            set { number = value; OnPropertyChanged("Number"); }
        }

        public int FundId
        {
            get { return fundId; }
            set { fundId = value; OnPropertyChanged("FundId"); }
        }
        public override string ToString()
        {
            return String.Format("{0}) در {1} به مبلغ {2} و {3} عضو", Number, EstablishmentDate, LoanAmount, MembersCount);
        }

        public int CompareTo(object obj){
            return Convert.ToInt32(this.Equals(obj));
        }

        public override bool Equals(object obj)
        {
            if (obj is Fund)
                return this.GetHashCode() == ((Fund)obj).GetHashCode();
            return false;
        }

        public override int GetHashCode()
        {
            /*
            int hash = 92821;
            int mult = 23;
            hash = hash * mult + this.EstablishmentDate.GetHashCode();
            hash = hash * mult + this.FundId.GetHashCode();
            hash = hash * mult + this.InstallmentAmount.GetHashCode();
            hash = hash * mult + this.MembersCount.GetHashCode();
            hash = hash * mult + this.Number.GetHashCode();
            /**/
            int hash = this.Number.GetHashCode() + this.FundId.GetHashCode();
            return hash;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
