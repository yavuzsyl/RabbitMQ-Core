using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wrd2Pdf.Producer.Models
{
    public class FileMessageQueue
    {
        public byte[] WordByte { get; set; }
        public string Email { get; set; }
        public string FileName { get; set; }
    }
}
