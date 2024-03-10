using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSTinvestRefBook.Services
{
    public interface IErrorLogger
    {
        void LogError(Exception ex);
    }
}
