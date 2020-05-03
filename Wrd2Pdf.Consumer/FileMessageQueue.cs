using System;
using System.Collections.Generic;
using System.Text;

namespace Wrd2Pdf.Consumer
{
    public class FileMessageQueue
    {
        public byte[] WordByte { get; set; }
        public string Email { get; set; }
        public string FileName { get; set; }
    }
}
