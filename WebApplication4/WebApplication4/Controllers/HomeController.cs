using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Web.Helpers;
using WebApplication4.Models;
using System.Data.Entity.Validation;
using System.Text.RegularExpressions;
using System.IO;
using PagedList;
using PagedList.Mvc;


namespace WebApplication4.Controllers
{ 
    
    public class HomeController : Controller
    {
        const int pagesize = 4;
        public ActionResult Uyeler()
        {
            using (Basvuru veritabani = new Basvuru())
            {

                var modelim = new basvurum()
                {

                    basvurular = veritabani.BASVURULAR.ToList()

                };


           
                

                return View(modelim);
                }
        }
        public ActionResult BProfil(int? page, int ?ID,int ?BID)
        {


            Basvuru db = new Basvuru();
            var modelim = new basvurum();
            if (!page.HasValue)
            {
                modelim.makale = db.Makale.OrderByDescending(x => x.YayımTarihi).Where(x=>x.YazarID==BID).Take(pagesize).ToList();
                modelim.yorum = db.Yorum.ToList();
                modelim.basvurular = db.BASVURULAR.ToList();
                modelim.galeri = db.Galeri.ToList();
            }
            else
            {
                int pageindex = pagesize * page.Value;
                modelim.yorum = db.Yorum.ToList();
                modelim.basvurular = db.BASVURULAR.ToList();
                modelim.galeri = db.Galeri.ToList();
                modelim.makale = db.Makale.OrderByDescending(x => x.YayımTarihi).Where(x=>x.YazarID==BID).Skip(pageindex).Take(pagesize).ToList();
                modelim.id = ID;
                modelim.bid = BID;
            }

            if (Request.IsAjaxRequest())
            {

                return PartialView("_Bmakale", modelim);
            }

            return View(modelim);        
        }

        [HttpPost]
        public ActionResult Bprofilislem(FormCollection form)
        {

            Basvuru db = new Basvuru();
            Yorum a = new Yorum();

            int c;
                c = Convert.ToInt32(form["id"]);
            int d;
                d = Convert.ToInt32(form["bid"]);

            a.Icerik = form["yorum"].Trim();
            a.MakaleID = Convert.ToInt32(form["id1"]);
            var uye = (from i in db.BASVURULAR where i.BASVURUID == c select i).FirstOrDefault();
            a.İsim = uye.isim;
            a.BasvuruID = uye.BASVURUID;
            db.Yorum.Add(a);
            db.SaveChanges();

            return RedirectToAction("BProfil", new { id = c,bid=d });
        }
        public ActionResult Anasayfa(int? page,int ?ID)
        {
         
             Basvuru db = new Basvuru();
             var modelim = new basvurum();
            if(!page.HasValue)
            {
               modelim.makale= db.Makale.OrderByDescending(x => x.YayımTarihi).Take(pagesize);
               modelim.yorum = db.Yorum.ToList();
               modelim.basvurular=db.BASVURULAR.ToList();
               modelim.galeri = db.Galeri.ToList();              
            }
            else
            {
                int pageindex = pagesize * page.Value;
                modelim.yorum = db.Yorum.ToList();
                modelim.basvurular = db.BASVURULAR.ToList();
                modelim.galeri = db.Galeri.ToList();     
                modelim.makale = db.Makale.OrderByDescending(x => x.YayımTarihi).Skip(pageindex).Take(pagesize);
                modelim.id =ID;
            }
      
            if (Request.IsAjaxRequest())
            {

                return PartialView("_Makale", modelim);
            }

            return View(modelim);
        }

     
        [HttpPost]
        public ActionResult AnasayfaIslem(FormCollection form)
        {
            Basvuru db = new Basvuru();
            Yorum a = new Yorum();
                  int c;
             c = Convert.ToInt32(form["id"]);
            a.Icerik = form["yorum"].Trim();
            a.MakaleID = Convert.ToInt32(form["id1"]);
            var uye = (from i in db.BASVURULAR where i.BASVURUID==c select i).FirstOrDefault();
            a.İsim = uye.isim;
            a.BasvuruID = uye.BASVURUID;
            db.Yorum.Add(a);
            db.SaveChanges();


            return RedirectToAction("Anasayfa", new { id = c });
        }

        [HttpPost]
        public ActionResult AnasayfaMakaleIslem(FormCollection form,HttpPostedFileBase file1)
        {
            Basvuru db = new Basvuru();
            Galeri d = new Galeri();
            Makale m = new Makale();

                m.Icerik = form["txtAreaAdres"];
                m.YazarID = Convert.ToInt32(form["hidden"]);           
                m.YayımTarihi = DateTime.Now;
                if (file1 != null)
                 {
                    m.GaleriID = ResimKaydet(file1, Convert.ToInt32(form["hidden"]));
                 }
               if(form["video"].Length!=0)
                 {
                    m.GaleriID = VideoKaydet(form["video"], Convert.ToInt32(form["hidden"]));   
                 }
             
                    db.Makale.Add(m);
                    db.SaveChanges();

            return RedirectToAction("Anasayfa", new { id = form["hidden"] });
        }

        private int VideoKaydet(string video, int c)
        {
            Basvuru db = new Basvuru();
            Galeri d = new Galeri();

            if (video.Contains("watch?v="))
            {
                string[] a = new string[10];
                a = video.Split('/');
                a[3]=a[3].Remove(0, 8);
                video = a[0] + "//" + a[2]+"/embed/"+a[3];
                d.video = video;
                d.EkleyenID = c;

                db.Galeri.Add(d);
                db.SaveChanges();

                return d.GaleriID;

            }
            else
            {
                d.video = video;
                d.EkleyenID = c;

                db.Galeri.Add(d);
                db.SaveChanges();

                return d.GaleriID;
            }
        }
        private int ResimKaydet(HttpPostedFileBase file1,int c)
        {
              Basvuru db = new Basvuru();
              Galeri d = new Galeri();

           
                string pic = System.IO.Path.GetFileName(file1.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Dosyalar/Resimler"), pic);

                file1.SaveAs(path);

                using (MemoryStream ms = new MemoryStream())
                {
                    file1.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();


                    d.resim = array;
                    d.EkleyenID = c;

                    db.Galeri.Add(d);
                    db.SaveChanges();

                    return d.GaleriID;
                }
            
            
        }
         [HttpPost]
        public ActionResult ProfilMakaleIslem(FormCollection form, HttpPostedFileBase file1)
        {
            Basvuru db = new Basvuru();
            Galeri d = new Galeri();
            Makale m = new Makale();

            m.Icerik = form["txtAreaAdres"];
            m.YazarID = Convert.ToInt32(form["hidden"]);
            m.YayımTarihi = DateTime.Now;
            if (file1 != null)
            {
                m.GaleriID = ResimKaydet(file1, Convert.ToInt32(form["hidden"]));
            }
            if (form["video"].Length != 0)
            {
                m.GaleriID = VideoKaydet(form["video"], Convert.ToInt32(form["hidden"]));
            }
            db.Makale.Add(m);
            db.SaveChanges();

            return RedirectToAction("Profil", new { id = form["hidden"] });
        }

         public ActionResult Profil(int? page,int ?ID)
        {
            ViewBag.Message = "Your application description page.";
              Basvuru db = new Basvuru();
             var modelim = new basvurum();
            if(!page.HasValue)
            {
               modelim.makale= db.Makale.OrderByDescending(x => x.YayımTarihi).Where(x=>x.YazarID==ID).Take(pagesize);
               modelim.yorum = db.Yorum.ToList();
               modelim.basvurular=db.BASVURULAR.ToList();
               modelim.galeri = db.Galeri.ToList();              
            }
            else
            {
                int pageindex = pagesize * page.Value;
                modelim.yorum = db.Yorum.ToList();
                modelim.basvurular = db.BASVURULAR.ToList();
                modelim.galeri = db.Galeri.ToList();
                modelim.makale = db.Makale.OrderByDescending(x => x.YayımTarihi).Where(x=>x.YazarID==ID).Skip(pageindex).Take(pagesize);
                modelim.id = ID;
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_PMakale",modelim);
            }

            return View(modelim);
            }
        


        [HttpPost]
        public ActionResult ProfilIslem(FormCollection form)
        {
            Basvuru db = new Basvuru();
            Yorum a = new Yorum();


            int c;
                c = Convert.ToInt32(form["id"]);          
            var uye = (from i in db.BASVURULAR where i.BASVURUID == c select i).FirstOrDefault();
            a.BasvuruID = uye.BASVURUID;
            a.İsim = uye.isim;
            a.Icerik = form["yorum"].Trim();
            a.MakaleID = Convert.ToInt32(form["id1"]);

            db.Yorum.Add(a);
            db.SaveChanges();

            return RedirectToAction("Profil", new{id=c});
        }
        
        public ActionResult Basvuru()
        {
            ViewBag.Message = "Your contact page.";

            using (Basvuru veritabani = new Basvuru())
            {

                var modelim = new basvurum()
                {

                    basvurular = veritabani.BASVURULAR.ToList(),
                    universiteler = veritabani.universite.ToList()

                };

                return View(modelim);
            }
        }
        [HttpPost]

        public ActionResult Mail(FormCollection form, HttpPostedFileBase dosya,HttpPostedFileBase resim)
        {
            DateTime date = DateTime.Now;
            string suan = date.ToString();

            if (dosya != null && dosya.ContentLength > 0)
            {

                string dosyayolu = DateTime.Now.ToString("ddMMyyyyHHmmss")+Path.GetExtension(dosya.FileName);
                var yuklemeyeri = System.IO.Path.Combine(Server.MapPath("~/Dosyalar"), dosyayolu);
                dosya.SaveAs(yuklemeyeri);
            }
            string kayıtyeri = "Dosyalar/" + suan;

            if (resim != null)
            {
                string pic = System.IO.Path.GetFileName(resim.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Dosyalar/Resimler"), pic);
                // file is uploaded
                resim.SaveAs(path);
            }
            if (resim == null)
            {
                TempData["notice"] = "Resim Alanı Boş Bırakılamaz.";
                return RedirectToAction("Basvuru");
            } 
           
                using (MemoryStream ms = new MemoryStream())
                {
                    resim.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
    
            Basvuru db = new Basvuru();
            BASVURULAR a = new BASVURULAR();
            var b = new basvurum();

    
            if (form["sifre"] != form["onaysifre"])
            {
                TempData["notice"] = "Şifreniz Uyumlu Değil";
                return RedirectToAction("Basvuru");
            }

            if (form["sifre"].Length < 6 || form["sifre"].Length > 20)
            {
                TempData["notice"] = "Şifreniz En Az 6 Karakter En Fazla 20 Karakter Olmalıdır.";
                return RedirectToAction("Basvuru");
            }
            if (dosya == null)
            {
                TempData["notice"] = "Dosya Alanına Cv Dosyanızı Yükleyiniz.";
                return RedirectToAction("Basvuru");
            }
            if (String.IsNullOrEmpty(form["sifre"]))
            {
                TempData["notice"] = "Şifrenizi Girmediniz.";
                return RedirectToAction("Basvuru");
            }
            if (String.IsNullOrEmpty(form["email"]))
            {
                TempData["notice"] = "E-mail Adresinizi Girmediniz.";
                return RedirectToAction("Basvuru");
            }
            if (String.IsNullOrEmpty(form["Universite"]))
            {
                TempData["notice"] = "Üniversite Alanını Girmediniz.";
                return RedirectToAction("Basvuru");
            }
            if (String.IsNullOrEmpty(form["İsim"]))
            {
                TempData["notice"] = "İsim Alanını Girmediniz.";
                return RedirectToAction("Basvuru");                
            }
            if (String.IsNullOrEmpty(form["Bölüm"]))
            {
                TempData["notice"] = "Bölüm Alanını Girmediniz.";
                return RedirectToAction("Basvuru");
            }
            if (String.IsNullOrEmpty(form["Sınıf"]))
            {
                TempData["notice"] = "Sınıf Alanını Girmediniz.";
                return RedirectToAction("Basvuru");
            }
            if (String.IsNullOrEmpty(form["Cep"]))
            {
                TempData["notice"] = "Cep Telefonunuzu Girmediniz.";
                return RedirectToAction("Basvuru");
            }
            if (String.IsNullOrEmpty(form["Mesaj"]))
            {
                TempData["notice"] = "Mesaj Alanını Girmediniz.";
                return RedirectToAction("Basvuru");
            }
            if (!Regex.IsMatch(form["email"], @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
            {
                TempData["notice"] = "E-mail Adresiniz Uygun Formatta Değil.";
                return RedirectToAction("Basvuru");
            }
            else
            {

                a.sifre = form["sifre"];
                a.üniversite = form["Universite"];
                a.isim = form["İsim"].Trim();
                a.email = form["email"].Trim();
                a.bölüm = form["Bölüm"].Trim();
                a.sınıf = form["Sınıf"].Trim();
                a.cep = form["Cep"].Trim();
                a.cv = kayıtyeri;
                a.Resim = array;
                a.mesaj = form["Mesaj"].Trim();

                db.BASVURULAR.Add(a);
                db.SaveChanges();
            }
                //SmtpClient sc = new SmtpClient();

                //sc.Port = 587;

                //sc.Host = "smtp.live.com";

                //sc.EnableSsl = true;

                //sc.Credentials = new System.Net.NetworkCredential("gidilecek mail adresi", "parola");

                //MailMessage mail = new MailMessage();

                //mail.From = new MailAddress("mail adresi", "parola");

                //mail.To.Add("mail adresi");
                //mail.To.Add("mail adresi");
                //mail.Subject = "PITONAkademi"; mail.IsBodyHtml = true;

                //Attachment d = new Attachment(dosya.InputStream, dosya.FileName);
                //mail.Attachments.Add(d);

                //mail.Body = "Bu mail size www.pitonakademi.com adresinden gönderildi.<br/>Gönderenin;<br/><br/>Ad-Soyad : "
                //    + form["İsim"].Trim() + "<br/>E-posta : " + form["Email"].Trim() + "<br/>Üniversite: " + form["Universite"].Trim() + "<br/>Bölüm : "
                //    + form["Bölüm"].Trim() + "<br/>Sınıf : " + form["Sınıf"].Trim() + "<br/>Cep : " + form["Cep"].Trim() +
                //    "<br/>Mesaj : " + form["Mesaj"].Trim();             
                //    sc.Send(mail);

                TempData["notice"] = "Başvurunuz kaydedilmiştir.";
                return View("Basvuru");
            }


        }
       
      
        public ActionResult ProfilAyar()
        {
            using (Basvuru veritabani = new Basvuru())
            {

                var modelim = new basvurum()
                {

                    basvurular = veritabani.BASVURULAR.ToList(),
                    universiteler = veritabani.universite.ToList()

                };

                return View(modelim);
            }
        }

        public ActionResult Ayar(FormCollection form,HttpPostedFileBase resim)
        {
            Basvuru a = new Basvuru();

            int b=Convert.ToInt32(form["hidden"]);
            var sorgu = (from i in a.BASVURULAR where i.BASVURUID ==b select i).FirstOrDefault();
            if (form["İsim"] != "")
            {
                sorgu.isim = form["İsim"];
            }
            else
                sorgu.isim = sorgu.isim;
            if (form["email"] != "")
            {
                if (!Regex.IsMatch(form["email"], @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                {
                    TempData["notice"] = "E-mail Adresiniz Uygun Formatta Değil.";
                    return RedirectToAction("ProfilAyar", new { id = b });
                }
                    sorgu.email = form["email"];               
            }
            else sorgu.email = sorgu.email;
            if (form["sifre"] != "")
            {
                if (form["sifre"].Length < 6 || form["sifre"].Length > 20)
                {
                    TempData["notice"] = "Şifreniz En Az 6 Karakter En Fazla 20 Karakter Olmalıdır.";
                    return RedirectToAction("ProfilAyar", new { id = b });
                }
                sorgu.sifre = form["sifre"];
            }
            else sorgu.sifre = sorgu.sifre;
            if (form["Universite"] != "")
            {
                sorgu.üniversite = form["Universite"];
            }
            else sorgu.üniversite = sorgu.üniversite;
            if (form["Bölüm"] != "")
            {
                sorgu.bölüm = form["Bölüm"];
            }
            else sorgu.bölüm = sorgu.bölüm;
            if (form["Sınıf"] != "")
            {
                sorgu.sınıf = form["Sınıf"];
            }
            else sorgu.sınıf = sorgu.sınıf;
            if (form["Cep"] != "")
            {
                sorgu.cep = form["Cep"];
            }
            else sorgu.cep = sorgu.cep;
            if (resim != null)
            {
                string pic = System.IO.Path.GetFileName(resim.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Dosyalar/Resimler"), pic);
                // file is uploaded
                resim.SaveAs(path);
            }
            else sorgu.Resim = sorgu.Resim;
             sorgu.mesaj = sorgu.mesaj;
             sorgu.cv = sorgu.cv;

             if (resim != null)
             {
                 using (MemoryStream ms = new MemoryStream())
                 {
                     resim.InputStream.CopyTo(ms);
                     byte[] array = ms.GetBuffer();

                     sorgu.Resim = array;
                 }
             }
             a.SaveChanges();

             TempData["notice"] = "Değişiklikler Kaydedildi.";

             return RedirectToAction("ProfilAyar", new { id = form["hidden"] });
        }
          
        public ActionResult Giris ()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Kayit()
        {
            string posta = Request.Form["mail"];
            string password = Request.Form["sifre"];

            if (String.IsNullOrEmpty(posta) && String.IsNullOrEmpty(password))
            {
                TempData["hata"] = "E-posta Adresinizi Ve Şifrenizi Girmediniz.";
                return RedirectToAction("Giris");

            }
            else if (String.IsNullOrEmpty(posta))
            {
                TempData["hata"] = "E-posta Adresinizi Girmediniz.";
                return RedirectToAction("Giris");

            }
            else if (String.IsNullOrEmpty(password))
            {
                TempData["hata"] = "Şifrenizi Girmediniz.";
                return RedirectToAction("Giris");
            }
            else
            {
                Basvuru db = new Basvuru();

                var uye = (from i in db.BASVURULAR where i.email == posta && i.sifre==password select i).FirstOrDefault();
                if (uye == null)
                {
                    TempData["hata"]="Aradığınız Kayıt Bulunamadı.";
                    return RedirectToAction("Giris");
                  
                }
              
               
                int idd=uye.BASVURUID;
                 
               
                return RedirectToAction("Profil", new { id = idd });
            }
        }
          }

}