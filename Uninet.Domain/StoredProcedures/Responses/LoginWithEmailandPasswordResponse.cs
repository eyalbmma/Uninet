using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.StoredProcedures.Responses
{
    

    public class LoginWithEmailandPasswordResponse
    {
        public int Userid { get; set; }
        public bool Q1_Q2_InidicationRes { get; set; }



       
        public bool Q3_InidicationRes { get; set; }

        public bool? verified { get; set; }

        // public int Role { get; set; }


    }
}
