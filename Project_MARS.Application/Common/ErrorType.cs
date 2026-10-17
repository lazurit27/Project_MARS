using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_MARS.Application.Common
{
    public enum ErrorType
    {
        NotFound,
        Conflict,
        Validation,
        Unauthorized,
        BadRequest
    }
}
