namespace AIDemos.Models
{
    public class AppAds
    {
        public string? adId { get; set; }
        public string? appId { get; set; }
        public string? distributorId { get; set; }
        public string? description { get; set; }
        public string? url { get; set; }
        public bool bIFrame { get; set; }
        public DateTime beginDate { get; set; }
        public DateTime endDate { get; set; }
        private bool closed = false;

        // New property for storing the image
        public byte[]? adImage { get; set; }
    }

    public class CompanyDatabase
    {
        public string? DatabaseName { get; set; }  // TEXT NOT NULL
    }

    public class Company
    {
        public byte[]? CompanyID { get; set; }  // BLOB PRIMARY KEY (16-byte array for UUID)
        public string? CompanyName { get; set; }  // TEXT NOT NULL
        public string? Address { get; set; }  // TEXT
        public string? City { get; set; }  // TEXT
        public string? State { get; set; }  // TEXT
        public string? ZipCode { get; set; }  // TEXT
        public string? Country { get; set; }  // TEXT
        public string? Phone { get; set; }  // TEXT
        public string? Fax { get; set; }  // TEXT
        public string? Email { get; set; }  // TEXT
        public string? Website { get; set; }  // TEXT
        public string? Contact { get; set; }  // TEXT
        public DateTime? DateOfIncorporation { get; set; }  // DATE
        public string? TaxIDNumber { get; set; }  // TEXT
        public string? BusinessType { get; set; }  // TEXT
        public string? Industry { get; set; }  // TEXT
        public string? Logo { get; set; }  // TEXT
        public string? Notes { get; set; }  // TEXT
        public bool Active { get; set; }  // INTEGER (as SQLite uses 1/0 for booleans)
        public string? DatabaseName { get; set; }  // TEXT
    }


    public class Transaction
    {
        public string Type { get; set; }
        public DateTime PostedOn { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string RefNumber { get; set; }
        public string Name { get; set; }
        public string Memo { get; set; }
    }

    public class LedgerBalance
    {
        public decimal Amount { get; set; }
        public DateTime AsOf { get; set; }
    }

    public class AccountData
    {
        public string AccountNum { get; set; }
        public List<Transaction> Transactions { get; set; }
        public LedgerBalance LedgerBalance { get; set; }
    }





    public class Client
    {
        public string? ClientID { get; set; }
        public string? ClientName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? LandPhoneNumber { get; set; }
        public string? MobilePhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? TaxIDNumber { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
        public string? BusinessType { get; set; }  // E.g., C Corp, SubChapter S, LLC, etc.
        public string? Website { get; set; }
        public string? Industry { get; set; }
        public string? ContactPerson { get; set; }

        public override string ToString()
        {
            return $"ClientID: {ClientID}, ClientName: {ClientName}, Address: {Address}, City: {City}, State: {State}, ZipCode: {ZipCode}, LandPhoneNumber: {LandPhoneNumber}, MobilePhoneNumber: {MobilePhoneNumber}, Email: {Email}, TaxIDNumber: {TaxIDNumber}, DateOfIncorporation: {DateOfIncorporation}, BusinessType: {BusinessType}, Website: {Website}, Industry: {Industry}, ContactPerson: {ContactPerson}";
        }
    }
    public class Account
    {
        public string? AccountID { get; set; }
        public string? ClientID { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountType { get; set; }
        public decimal Balance { get; set; }

        public override string ToString()
        {
            return $"BankName: {BankName}, AccountNumber: {AccountNumber}, AccountType: {AccountType}, Balance: {Balance}";
        }
    }

    public class Transactions
    {
        public string TransactionID { get; set; }
        public string AccountID { get; set; }
        public string ClientID { get; set; }
        public string BatchID { get; set; }
        public DateTime BatchDate { get; set; }
        public string Flag { get; set; }
        public DateTime Date { get; set; }
        public string Num { get; set; }
        public string Payee { get; set; }
        public string Category { get; set; }
        public List<string> Tag { get; set; }
        public string Memo { get; set; }
        public string Status { get; set; }
        public decimal Payment { get; set; }
        public decimal Deposit { get; set; }
        public decimal Balance { get; set; }

        public Transactions()
        {
            Tag = new List<string>();
        }

        public override string ToString()
        {
            return $"Date: {Date}, Payment: {Payment}, Num: {Num}, Payee: {Payee}, Memo: {Memo}, Amount: {Deposit}, Category: {Category}, Tags: {string.Join(", ", Tag)}";
        }
    }
}


//AccountType
//Checking
//Savings
//Credit Card
//Cash
//Investment/Retirement
//Asset
//Debt (not including credit cards)
//Loan
//Accounts Payable
//Accounts Receivable
