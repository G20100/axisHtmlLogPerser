using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace pfe.axis
{
    public static class axisHtmlPerse
    {
        [FunctionName("axisHtmlPerse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

//            string name = req.Query["name"];

//            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
//            dynamic data = JsonConvert.DeserializeObject(requestBody);
//            name = name ?? data?.name;

            string htmlDump = await new StreamReader(req.Body).ReadToEndAsync();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlDump);

// Get RossCase
            string rossCase = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr/td/a").InnerText;

// Get Logging Day
            string[] loggingdays = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[3]/td[2]").InnerText.Split(' ');
            string[] loggingdaysarr = new string[3] ;
            Array.Copy(loggingdays , 3 , loggingdaysarr ,0, 3) ;
            string loggingdaystring = string.Join(' ', loggingdaysarr);
//            var loggingdaystring = string.Join(' ', (loggingdays[3] loggingdays[4] loggingdays[5] ));
            DateTime loggingday = DateTime.ParseExact(loggingdaystring, "dd MMM yyyy" ,null);

//check リモートケース
            var remoteFlug = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[13]/td[2]/table/tbody/tr/td[1]/table/tbody/tr/td");

//            var table12 = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[12]/td[2]/table");
//            var table13 = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[13]/td[2]/table");
//            var table14 = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[14]/td[2]/table");

//            var tables = htmlDoc.DocumentNode.SelectNodes("/html/body/table/tbody/tr[@width='586']");


// Get Log times
            var preVisitHours = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[12]/td[2]/table/tbody/tr/td[2]/table/tbody/tr/td").InnerText);
            var preVisitMinuts = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[12]/td[2]/table/tbody/tr/td[3]/table/tbody/tr/td").InnerText);
            
            var travelHours = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[13]/td[2]/table/tbody/tr/td[2]/table/tbody/tr/td").InnerText);
            var travelMinuts = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[13]/td[2]/table/tbody/tr/td[3]/table/tbody/tr/td").InnerText);
            var onsiteHours = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[14]/td[2]/table/tbody/tr/td[2]/table/tbody/tr/td").InnerText);
            var onsiteMinuts = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[14]/td[2]/table/tbody/tr/td[3]/table/tbody/tr/td").InnerText);
            var postVisitHours = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[15]/td[2]/table/tbody/tr/td[2]/table/tbody/tr/td").InnerText);
            var postVisitMinuts = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[15]/td[2]/table/tbody/tr/td[3]/table/tbody/tr/td").InnerText);
            var researchHours = 0;
            var researchMinuts = 0;
            
            string currentStatus = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[20]/td[2]/div/p").InnerText;

            if ( remoteFlug.InnerText != "Travel")
            {    //this is Remote case|
                researchHours = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[13]/td[2]/table/tbody/tr/td[2]/table/tbody/tr/td").InnerText);
                researchMinuts = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[13]/td[2]/table/tbody/tr/td[3]/table/tbody/tr/td").InnerText);
                postVisitHours = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[14]/td[2]/table/tbody/tr/td[2]/table/tbody/tr/td").InnerText);
                postVisitMinuts = Convert.ToInt32(htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/table/tbody/tr[14]/td[2]/table/tbody/tr/td[3]/table/tbody/tr/td").InnerText);
                travelHours = 0;
                travelMinuts = 0;
                onsiteHours = 0;
                onsiteMinuts = 0;
            }

            int preVisitTime = preVisitHours * 60  + preVisitMinuts ;
            int travelTime = travelHours * 60  + travelMinuts ;
            int onsiteTime = onsiteHours * 60  + onsiteMinuts ;
            int postVisitTime = preVisitHours * 60  + preVisitMinuts ;
            int researchTime = researchHours * 60  + researchMinuts ;


// make array
//            Array returnarr = 

            return (ActionResult)new OkObjectResult($"ross: {rossCase} , {loggingday}  , {preVisitTime} , {onsiteTime} , {currentStatus}");

//            return name != null
//                ? (ActionResult)new OkObjectResult($"Hello, {name}")
//                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
