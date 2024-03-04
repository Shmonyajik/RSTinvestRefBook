using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSTinvestRefBook.Enums
{
    public enum StatusCode
    {
        OK = 200,
        ValidationError = 400,
        InternalServerError = 500,
        NotFound = 404
    }
}
