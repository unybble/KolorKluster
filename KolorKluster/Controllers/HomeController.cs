using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KolorKluster.Models;
using KolorKluster.Data;
using System.Text;

namespace KolorKluster.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public List<Cluster> GetClusters([FromQuery]int numClusters, [FromQuery]int numDataPoints)
        {
            Classifier classifier = new Classifier(numClusters);
            classifier.Initialize(numDataPoints);

            return classifier.GetClusters();
           
        }
    }
       
}
