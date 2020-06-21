using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor_specification.Data
{
    public class DataService
    {
        public static SpecificationTable[] table_specifications = new SpecificationTable[]
        {
            new SpecificationTable()
            {
                name = "organization",
                fields = new SpecificationField[]{
                    new SpecificationField()
                    {
                        name = "name",
                        field_type = 1
                    }
                }
            },
            new SpecificationTable()
            {
                name = "person",
                fields = new SpecificationField[]{
                    new SpecificationField()
                    {
                        name = "name",
                        field_type = 1
                    },
                    new SpecificationField()
                    {
                        name = "birthdate",
                        field_type = 1
                    },
                    new SpecificationField()
                    {
                        name = "organization",
                        field_type = 2,
                        accepted_entity_types = new string[]{"organization" }
                    }
                }
            }
        };

        public static Table database = new Table()
            {
                name = "person",
                entities = new List<Entity>()
                {
                    new Entity()
                    {
                        id = "per1",
                        entity_type = "person",
                        fields = new Tuple<string, object>[]{
                            new Tuple<string, object>("name", "Иванов"),
                            new Tuple<string, object>("birthdate", "1990-01-01"),
                            new Tuple<string, object>("organization", new string[]{"org1", "org2"})
                        }
                    },
                    new Entity()
                    {
                        id = "per2",
                        entity_type = "person",
                        fields = new Tuple<string, object>[]{
                            new Tuple<string, object>("name", "Петров"),
                            new Tuple<string, object>("birthdate", "1970-01-01"),
                            new Tuple<string, object>("organization", new string[]{"org2" })
                        }
                    },
                    new Entity()
                    {
                        id = "per3",
                        entity_type = "person",
                        fields = new Tuple<string, object>[]{
                            new Tuple<string, object>("name", "Сидоров"),
                            new Tuple<string, object>("birthdate", "1930-02-01")
                        }
                    },
                    new Entity()
                    {
                        id = "org1",
                        entity_type = "organization",
                        fields = new Tuple<string, object>[]{
                            new Tuple<string, object>("name", "ИСИ")
                        }
                    },
                    new Entity()
                    {
                        id = "org2",
                        entity_type = "organization",
                        fields = new Tuple<string, object>[]{
                            new Tuple<string, object>("name", "Институт математики")
                        }
                    }
                }
            };
        public static Entity GetEntity(string id)
        {
            return database.entities.Where(r => r.id == id).First();
        }
        
        public static Tuple<string, object> GetFieldValue(string id, string field_name)
        {
            var record = GetEntity(id);
            var field_specification = GetSpecificationField(record.entity_type, field_name);
            var value = record.fields.Where(r => r.Item1 == field_name).FirstOrDefault();
            if (value != null)
            {
                if (field_specification.field_type == 2)
                {

                    return new Tuple<string, object>(field_specification.name, ((IEnumerable<string>)value.Item2).ToArray());
                }

                if (field_specification.field_type == 1 || field_specification.field_type == 0)
                    return new Tuple<string, object>(field_specification.name, value.Item2.ToString());
            };
            return null;
        }

        public static string GetEntityName(string id)
        {
            return GetFieldValue(id, "name").Item2.ToString();
        }
        public static string GetEntityType(string id)
        {
            return GetEntity(id).entity_type;
        }
        public static SpecificationField GetSpecificationField(string type, string field)
        {
            return table_specifications.Where(r => r.name == type).First().fields.Where(r => r.name == field).First();
        }
        public static SpecificationField[] GetSpecificationFields(string type)
        {
            return table_specifications.Where(r => r.name == type).First().fields;
        }

        public static string[] GetEntityTypes()
        {
            string[] names = table_specifications.Select(r => r.name).ToArray();
            string[] result = new string[names.Length + 1];
            result[0] = "All";                                // set the prepended value
            Array.Copy(names, 0, result, 1, names.Length);
            return result;
        }

        public static string[] SearchByTypeAndString(string query, string type) //return id
        {

            if (type != "All")
                return database.entities.Where(r => r.entity_type == type)
                    .Where(r => r.fields.Where(r => r.Item1 == "name").First().Item2.ToString().ToUpper().Contains(query.ToUpper()))
                .Select(r => r.id).ToArray(); ;

            return database.entities
                .Where(r => r.fields.Where(r => r.Item1 == "name").First().Item2.ToString().ToUpper().Contains(query.ToUpper()))
                .Select(r => r.id).ToArray();
        }

        public static Tuple<string, string[]>[] GetPossibleRelations(string entity_type) //return entity_type, possible related fields array
        {
            return table_specifications
                .Select(r => new Tuple<string, string[]>(r.name, r.fields
                    .Where(r => r.field_type == 2 && Array.Exists(r.accepted_entity_types, el => el == entity_type))
                    .Select(r => r.name).ToArray()))
                .ToArray();
        }

        public static Tuple<string, string[]>[] GetRelatedEntities(string entity_type, string id) //return entity_id, related fields array
        {
            var relations = GetPossibleRelations(entity_type);
            var related = new List<Tuple<string, string[]>>();
            foreach (var relation in relations)
            {
                related.Add(database.entities.Select(r => new Tuple<string, string[]>(r.id, r.fields
                    .Where(t => Array.Exists(relation.Item2, el => el == t.Item1)  && Array.Exists(((IEnumerable<string>)t.Item2).ToArray(), el => el == id))
                    .Select(r => r.Item1).ToArray()
                     )).First()
                    );
            }
            return related.ToArray();
        }
    }
}
