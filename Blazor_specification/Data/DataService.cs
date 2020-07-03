using Blazor_specification.Pages;
using Microsoft.Extensions.Configuration.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public static XElement db = XElement.Load(@"C:\Users\Kamroni\Desktop\SypCassete\meta\SypCassete_current.fog");

        public static Dictionary<string, XElement> dictionary;

        public static Table database = new Table()
        {
            name = "person",
            entities = new List<Entity>()
                {
                    new Entity()
                    {
                        id = "per1",
                        entity_type = "person",
                        pairs = new Pair[]{
                            new Field(){
                                predicate = "name",
                                value = "Иванов"
                            },
                            new Field(){
                                predicate = "birthdate",
                                value = "1990-01-01"
                            },
                            new Linked()
                            {
                                predicate = "work",
                                link = "org1"
                            },
                             new Linked()
                            {
                                predicate = "work",
                                link = "org2"
                            }
                        }
                    },
                    new Entity()
                    {
                        id = "per2",
                        entity_type = "person",
                        pairs = new Pair[]{
                            new Field(){
                                predicate = "name",
                                value = "Петров"
                            },
                            new Field(){
                                predicate = "birthdate",
                                value = "1970-01-01"
                            },
                            new Linked()
                            {
                                predicate = "work",
                                link = "org2"
                            }
                        }
                    },
                    new Entity()
                    {
                        id = "per3",
                        entity_type = "person",
                        pairs = new Pair[]{
                            new Field(){
                                predicate = "name",
                                value = "Сидоров"
                            },
                            new Field(){
                                predicate = "birthdate",
                                value = "1930-02-01"
                            }
                        }
                    },
                    new Entity()
                    {
                        id = "org1",
                        entity_type = "organization",
                        pairs = new Pair[]{
                            new Field(){
                                predicate = "name",
                                value = "ИСИ"
                            }
                        }
                    },
                    new Entity()
                    {
                        id = "org2",
                        entity_type = "organization",
                        pairs = new Pair[]{
                            new Field(){
                                predicate = "name",
                                value = "Институт математики"
                            }
                        }
                    }
                }
            };

      
        public static Entity GetEntity(string id)
        {
            if (id != "cassetterootcollection")
            {
                var el = db.Elements().FirstOrDefault(r => r.Attribute("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}about").Value == id);
                Entity entity = new Entity()
                {
                    id = id,
                    entity_type = el.Name.ToString(),
                    pairs = el.Elements().Select(f => new Field() { predicate = f.Name.ToString(), value = f.Value }).ToArray()
                };
                return entity;
            }
           
            return null;
        }
        /*public static Entity GetEntity(string id)
        {
            return database.entities.Where(r => r.id == id).First();
        }*/

        public static Pair[] GetPairs(string id)
        {
            return (id == "cassetterootcollection" ? null : GetEntity(id).pairs);
        }
        public static string GetEntityType(string id)
        {
            return (id == "cassetterootcollection" ? null : GetEntity(id).entity_type);
        }

        public static string GetEntityName(string id)
        {
            return (id == "cassetterootcollection" ? null : ((Field)GetPairs(id).Where(p => p.predicate == "name").First()).value);
        }
        public static Linked[] GetReferencePairs(string id)
        {

            var links = db.Elements().Where(x => x.Elements().Attributes().Select(at => at.Name.ToString()).ToArray().Contains("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}resource"));
            var el = links.Where(x => x.Elements().Last().Attributes().Select(at => at.Value.ToString()).ToArray().Contains(id)).Select(x => new Linked() { predicate = x.Elements().FirstOrDefault().Name.ToString(), link = x.Elements().FirstOrDefault().Attribute("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}resource").Value }).ToArray();
            return el;
        }

        public static string[] SearchByNameAndType(string name, string type)
        {
            if (type != "All"){
                return db.Elements().Where(el => el.Name.ToString() == type && el.Elements().First(r => r.Name.ToString() == "name").Value.ToString().ToUpper().Contains(name.ToUpper())).Select(el => el.Attribute("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}about").Value.ToString()).ToArray();
            }
            return db.Elements().Where(el => el.Elements().FirstOrDefault(r => r.Name.ToString() == "name") != null && el.Elements().FirstOrDefault(r => r.Name.ToString() == "name").Value.ToString().ToUpper().Contains(name.ToUpper())).Select(el => el.Attribute("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}about").Value.ToString()).ToArray();
        }

        public static string[] GetEntityTypes()
        {
            var links = db.Elements().Where(x => x.Elements().Attributes().Select(at => at.Name.ToString()).ToArray().Contains("{http://www.w3.org/1999/02/22-rdf-syntax-ns#}resource"));
            var types = db.Elements().Except(links).Select(el => el.Name.ToString()).Distinct().ToArray();
            var result = new String[types.Length + 1];
            result[0] = "All";
            Array.Copy(types, 0, result, 1, types.Length);
            return result;
        }
        /* public static Linked[] GetReferencePairs(string id)
         {
             return database.entities.SelectMany(r => r.pairs.OfType<Linked>().Where(t => ((Linked)t).link == id).Select(t => new Linked(){predicate = t.predicate, link = r.id })).ToArray();
         }*/
        /*public static Tuple<string, object> GetFieldValue(string id, string field_name)
        {
            var record = GetEntity(id);
            var field_specification = GetSpecificationField(record.entity_type, field_name);
            var value = record.pairs.Where(r => r.Item1 == field_name).FirstOrDefault();
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
            return table_specifications.Where(r => r.name == type).First().pairs.Where(r => r.name == field).First();
        }
        public static SpecificationField[] GetSpecificationFields(string type)
        {
            return table_specifications.Where(r => r.name == type).First().pairs;
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
                    .Where(r => r.pairs.Where(r => r.Item1 == "name").First().Item2.ToString().ToUpper().Contains(query.ToUpper()))
                .Select(r => r.id).ToArray(); ;

            return database.entities
                .Where(r => r.pairs.Where(r => r.Item1 == "name").First().Item2.ToString().ToUpper().Contains(query.ToUpper()))
                .Select(r => r.id).ToArray();
        }

        public static Tuple<string, string[]>[] GetPossibleRelations(string entity_type) //return entity_type, possible related pairs array
        {
            return table_specifications
                .Select(r => new Tuple<string, string[]>(r.name, r.pairs
                    .Where(r => r.field_type == 2 && Array.Exists(r.accepted_entity_types, el => el == entity_type))
                    .Select(r => r.name).ToArray()))
                .ToArray();
        }

        public static Tuple<string, string[]>[] GetRelatedEntities(string entity_type, string id) //return entity_id, related pairs array
        {
            var relations = GetPossibleRelations(entity_type);
            var related = new List<Tuple<string, string[]>>();
            foreach (var relation in relations)
            {
                related.Add(database.entities.Select(r => new Tuple<string, string[]>(r.id, r.pairs
                    .Where(t => Array.Exists(relation.Item2, el => el == t.Item1)  && Array.Exists(((IEnumerable<string>)t.Item2).ToArray(), el => el == id))
                    .Select(r => r.Item1).ToArray()
                     )).First()
                    );
            }
            return related.ToArray();
        }*/
    }
}
