using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TN_academic.Models;

namespace TN_academic.ViewModels
{
    [Serializable]
    public class Cart
    {
        public Cours Course { get; set; }
    }
}