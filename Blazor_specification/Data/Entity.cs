using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_specification.Data
{
    public class Entity
    {
        public string id;
        public Tuple<String, Object>[] fields;
        public string entity_type;
    }
}
