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

    public class T_Pair<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        
        public T_Pair(T1 f, T2 s)
        {
            First = f;
            Second = s;
        }
    }
}
