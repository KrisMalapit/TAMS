using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using TAMS.Models.View_Model;
using Newtonsoft.Json;
using System.Globalization;
using TAMS.Models;
using System.Security.Claims;

namespace TAMS.Controllers
{
    public class ReportsController : Controller
    {
        private readonly TAMSContext _context;
        private Claim claimUser;
        public ReportsController(TAMSContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ShowReport(string report)
        {
          
          
            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                byte[] bytes = null;

              


                response = client.GetAsync("http://localhost:50836/api/print?report=" + report).Result;
                string byteToString = response.Content.ReadAsStringAsync().Result.Replace("\"", string.Empty);
                bytes = Convert.FromBase64String(byteToString);
                return File(bytes, "application/pdf");
            }
            catch (Exception e)
            {

                throw;
            }
           
        }
        public IActionResult printReport(ReportViewModel rvm,string fDate, string tDate)
        {
            claimUser = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userid = claimUser.Value;
            string userrole = User.Claims.FirstOrDefault(c => c.Type == "RoleName").Value;

            try
            {
                if (rvm.Report == "rptEmployeeTemp")
                {
                    
                    rvm.fromDate = DateTime.ParseExact(fDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    
                    rvm.toDate = DateTime.ParseExact(tDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);

                    if (userrole != "EHSAdmin" && userrole != "Admin")
                    {
                        //kcm
                        //rvm.deptAccess = _context.Clusters.FirstOrDefault(a => a.UserId == Convert.ToInt32(userid)).Departments;
                    }
                    else
                    {
                      
                        rvm.deptAccess = "ALL";
                    }
                        
           
                }
                HttpClient client = new HttpClient();
                HttpResponseMessage response = new HttpResponseMessage();
                byte[] bytes = null;
                string xstring = JsonConvert.SerializeObject(rvm);


                
                string urilive = "http://192.168.70.165/TAMSAPI/api/printreport?rvm=";
                string uridev = "http://sodium2/tamsapi/api/printreport?rvm=";
                string uri = "http://localhost:50836/api/printreport?rvm=";


                response = client.GetAsync(urilive + xstring).Result;
                string byteToString = response.Content.ReadAsStringAsync().Result.Replace("\"", string.Empty);
                bytes = Convert.FromBase64String(byteToString);

                string rpttype = "";
                switch (rvm.rptType)
                {
                    case "PDF":
                        rpttype = "application/pdf";
                        break;
                    case "Excel":
                        rpttype =  "application/vnd.ms-excel";
                        break;
                    default:
                        break;
                }


                return File(bytes, rpttype);
            }
            catch (Exception e)
            {

                throw;
            }

        }
    }
}