using System;
using System.Collections.Generic;
using System.Text;

namespace BankingTransaction
{
    public partial class SaveService
    {
        //Method to save document as a file and view the saved document.
        public partial void SaveAndView(string filename, string contentType, MemoryStream stream);
    }
}
