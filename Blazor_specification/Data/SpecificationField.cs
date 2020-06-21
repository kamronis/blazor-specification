using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_specification.Data
{
    public class SpecificationField
    {
        public string name;
        public int field_type; //0 - int, 1 - string, 2 - entity
        public string[] accepted_entity_types;
    }

    public class SpecificationTable
    {
        public string name;
        public SpecificationField[] fields;

    }
}
