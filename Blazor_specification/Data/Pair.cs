using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_specification.Data
{
    public abstract class Pair
    {
        public string predicate;

    }
    
    public class Field:Pair
    {
        public string value;
    }

    public class Linked : Pair
    {
        public string link;
    }
}
