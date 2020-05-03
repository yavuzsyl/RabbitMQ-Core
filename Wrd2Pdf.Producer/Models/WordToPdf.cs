using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wrd2Pdf.Producer.Models
{
    public class WordToPdf
    {
        [Required(ErrorMessage ="Email alanı boş geçilemez")] 
        public string Email { get; set; }
        [Required(ErrorMessage = "Dosya alanı boş geçilemez")]
        public IFormFile WordFile { get; set; }

    }
}
