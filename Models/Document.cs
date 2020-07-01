using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocManagementAPI.Models
{
    public class Document
    {
       
            public Guid ID { get; set; }
            public string Name { get; set; }
            public byte[] Content { get; set; }
            public string Location { get; set; }
            public double Size { get; set; }
        
    }
}
