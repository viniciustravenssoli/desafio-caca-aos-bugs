using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dima.Core.Responses
{
    public class ApiResponse
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public Dictionary<string, List<string>> Errors { get; set; }
    }
}