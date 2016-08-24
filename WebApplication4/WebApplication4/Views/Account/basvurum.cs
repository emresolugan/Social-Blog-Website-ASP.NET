using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebApplication4.Models
{
    public class basvurum
    {
        public  IEnumerable<BASVURULAR>basvurular { get; set; }
        public IEnumerable<universite> universiteler { get; set; }
        public IEnumerable<Galeri> galeri { get; set; }
        public IEnumerable<Yorum> yorum { get; set; }
        public IEnumerable<Makale> makale { get; set; }
        public int ?id { get; set; }
        public int ?bid { get; set; }

    }
}