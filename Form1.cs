using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project3_EntityFrameworkStatistics
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Db3Project20Entities db= new Db3Project20Entities();
        private void Form1_Load(object sender, EventArgs e)
        {
            // Toplam Kategori Sayısı  
            int categoryCount = db.TblCategory.Count();
            lblCategoryCount.Text = categoryCount.ToString();

            //Toplam Ürün Sayısı
            int productCount = db.TblProduct.Count();
            lblProductCount.Text = productCount.ToString();

            //Toplam Müşteri Sayısı
            int customerCount = db.TblCustomer.Count();
            lblCustomerCount.Text = customerCount.ToString();

            //Toplam Sipariş Sayısı
            int orderCount = db.TblOrder.Count();
            lblOrderCount.Text = orderCount.ToString();

            //Toplam Stok Sayısı
            var totalProductStockCount = db.TblProduct.Sum(x => x.ProductStock);
            lblProductTotalStock.Text = totalProductStockCount.ToString();

            //Ortalama Ürün Fiyatı
            var averageProductPrice = db.TblProduct.Average(x => x.ProductPrice);
            lblProductAveragePrice.Text = averageProductPrice.ToString() + "₺";

            //Toplam Meyve Stoğu Sayısı
            var totalProductCountByCategoriesIsFruit = db.TblProduct.Where(x => x.CategoryId == 1).Sum(y => y.ProductStock);
            lblProductCountByCategoriesIsFruit.Text = totalProductCountByCategoriesIsFruit.ToString();

            //Gazoz İsimli Ürünün Toplam İşlem Hacmi
            var totalPriceByProductNameIsGazozGetStock = db.TblProduct.Where(x => x.ProductName == "Gazoz").Select(y => y.ProductStock).FirstOrDefault();
            var totalPriceByProductNameIsGazozGetUnitPrice = db.TblProduct.Where(x => x.ProductName == "Gazoz").Select(y => y.ProductPrice).FirstOrDefault();
            var totalPriceByProductNameIsGazoz = totalPriceByProductNameIsGazozGetStock * totalPriceByProductNameIsGazozGetUnitPrice;
            lblTotalPriceByProductNameIsGazoz.Text = totalPriceByProductNameIsGazoz.ToString();

            //Stok Sayısı 100'den az olan ürün sayısı
            var productCountByStockSmallerThenOneHundred = db.TblProduct.Where(x => x.ProductStock < 100).Count();
            lblProductStockSmallerThen100.Text = productCountByStockSmallerThenOneHundred.ToString();

            //Kategorisi Sebze Ve Durumu Aktif(True) Olan Ürün Stok Toplamı
           
            int id = db.TblCategory.Where(c => c.CategoryName == "Sebze").Select(c => c.CategoryId).FirstOrDefault();
            var productStockCountByCategoryNameIsSebzeAndStatusIsTrue = db.TblProduct
                .Where(p => p.CategoryId == id && p.ProductStatus == true)
                .Sum(p => p.ProductStock);
            lblProductCountByCategorySebzeAndStatusTrue.Text = productStockCountByCategoryNameIsSebzeAndStatusIsTrue.ToString();

            // Türkiyeden Yapılan Siparişler
            var orderCountFromTurkiye = db.Database.SqlQuery<int>("Select count(*) From TblOrder Where CustomerId " +
            "In (Select CustomerId From TblCustomer Where CustomerCountry='Türkiye')").FirstOrDefault();
            lblOrderCountFromTurkiyeBySQL.Text = orderCountFromTurkiye.ToString();

            // Türkiyeden Yapılan Siparişler EF
            var turkishCustomerIds = db.TblCustomer
                .Where(x => x.CustomerCountry == "Türkiye")
                .Select(y => y.CustomerId)
                .ToList();
            var orderCountFromTurkiyeWithEf = db.TblOrder
                .Count(z => turkishCustomerIds.Contains(z.CustomerId.Value));
            lblOrderCountFromTurkiyeByEF.Text=orderCountFromTurkiyeWithEf.ToString();

            //Siparişler İçinde Kategorisi Meyve Olan Ürünlerin Toplam Satış Fiyatı

            var orderTotalPriceByCategoryIsMeyve = db.Database.SqlQuery<decimal>("Select Sum(o.TotalPrice) From TblOrder o Join TblProduct p on o.ProductId=p.ProductId Join TblCategory c on p.CategoryId=c.CategoryId Where c.CategoryName='Meyve'").FirstOrDefault();
            lblOrderTotalPriceByCategoryIsMeyve.Text = orderTotalPriceByCategoryIsMeyve.ToString();

            //Siparişler İçinde Kategorisi Meyve Olan Ürünlerin Toplam Satış Fiyatı EF

            var orderTotalPriceByCategoryIsMeyveWithEf =(from o in db.TblOrder
                                                         join p in db.TblProduct on o.ProductId equals p.ProductId
                                                         join c in db.TblCategory on p.CategoryId equals c.CategoryId
                                                         where c.CategoryName =="Meyve"
                                                         select o.TotalPrice).Sum();
            lblOrderTotalPriceByCategoryIsMeyveByEf.Text= orderTotalPriceByCategoryIsMeyveWithEf.ToString() ;


            //Son Eklenen Ürünün Adı

            var lastProductName=db.TblProduct.OrderByDescending(x => x.ProductId).Select(y=> y.ProductName).FirstOrDefault();
            lblLastProductName.Text= lastProductName.ToString();

            //Son Eklenen Ürünün Kategorisi
            
            var lastProductCategoryId = db.TblProduct.OrderByDescending(x => x.ProductId).Select(y => y.CategoryId).FirstOrDefault();
            var lastProductCategoryName= db.TblCategory.Where(x=>x.CategoryId== lastProductCategoryId).Select(y=>y.CategoryName).FirstOrDefault();
            lblLastProductCategoryName.Text= lastProductCategoryName.ToString();

            // Aktif Ürün Sayısı
            var activeProductCount = db.TblProduct.Where(x => x.ProductStatus == true).Count();
            lblActiveProductCount.Text= activeProductCount.ToString();

            // Toplam Kola Stok Satışlarından Kazanılan Para
            var colaStock = db.TblProduct.Where(x=>x.ProductName=="Kola").Select(y=>y.ProductStock).FirstOrDefault();
            var colaPrice = db.TblProduct.Where(x=>x.ProductName=="Kola").Select(y=>y.ProductPrice).FirstOrDefault();
            var totalColaStockPrice = colaStock * colaPrice;
            lblTotalPriceWithStockByCola.Text= totalColaStockPrice.ToString();

            //Sistemde Son Sipariş Veren Müşteri Adı

            var lastCustomerId = db.TblOrder.OrderByDescending(x => x.OrderId).Select(y => y.CustomerId).FirstOrDefault();
            var lastCustomerName = db.TblCustomer.Where(x=>x.CustomerId== lastCustomerId).Select(y=>y.CustomerName).FirstOrDefault();
            lblLastCustomerName.Text = lastCustomerName.ToString();

            // Ülke Çeşitliliği Sayısı

            var countryDifferentCount = db.TblCustomer.Select(x=>x.CustomerCountry).Distinct().Count();
            lblDifferentCountryCount.Text= countryDifferentCount.ToString();


        }
    }
}
