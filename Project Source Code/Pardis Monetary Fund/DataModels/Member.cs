using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pardis_Monetary_Fund.DataModels
{
    public class Member
    {
        int memberId;
        String registrationId;

        public String RegistrationId
        {
            get { return registrationId; }
            set { registrationId = value; }
        }
        String firstName;
        String lastName;
        String fatherName;
        String nationalCode;
        String phoneNumber;
        String registrationDate;

        public String RegistrationDate
        {
            get { return registrationDate; }
            set { registrationDate = value; }
        }

        public String PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }

        public String FatherName
        {
            get { return fatherName; }
            set { fatherName = value; }
        }

        public String NationalCode
        {
            get { return nationalCode; }
            set { nationalCode = value; }
        }

        public String LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public String FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public int MemberId
        {
            get { return memberId; }
            set { memberId = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Member other = obj as Member;
            if (other == null) return false;
            return other.MemberId == this.MemberId;
        }

        public override int GetHashCode()
        {
            return MemberId.GetHashCode();
        }
    }
}
