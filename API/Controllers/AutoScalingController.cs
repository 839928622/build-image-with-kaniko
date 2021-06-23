using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;

namespace API.Controllers
{

    public class AutoScalingController : BaseController
    {
        [HttpGet("ScaleUp")]
        public JsonResult ScaleUp()
        {
           
            var y = 0.0001;
            for (int i = 0; i < 1000000000; i++)
            {
               
                y += i;
            }

            return new JsonResult(y);
        }
}
}
