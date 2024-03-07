using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKT_3_project.MainFrameworkFiles.Code.Events
{
    internal class uselessBackupCode
    {
        /*JObject jsObj = JsonConvert.DeserializeObject<JObject>(json);
                                            IEnumerable<string> keys = jsObj.Properties().Select(p => p.Name);*/
        /*List<object> parameters = new();
                                        foreach (string key in keys) // All keys in the object (the different kind of parameters getter method id)
                                        {
                                            if (jsObj[key].Type == JTokenType.Array) // If the parameter argument is an array of requests
                                            {
                                                foreach (var value in jsObj[key]) // Value = array element
                                                {
                                                    if (value.Type == JTokenType.Object) // If the value is an object
                                                    {
                                                        JObject keyValuePairs = value as JObject; // Set it as one
                                                        string[] elementProps = keyValuePairs.Properties().Select(p => p.Name).ToArray(); // Keys in the object (MUST be 1), the name of the obj (like sword)
                                                        if (value[elementProps[0]].Type == JTokenType.Array) // If the required object has multiple properties (like damage)
                                                        {
                                                            foreach (var itemProp in value[elementProps[0]])
                                                            {
                                                                Dictionary<string, int> item = (Dictionary<string, int>)ParameterMethods[1](elementProps[0], Convert.ToInt32(key));
                                                                MessageBox.Show($"Got back: ({elementProps[0]}) ({(string)itemProp}) {item[(string)itemProp]}");
                                                                parameters.Add(item[(string)itemProp]);
                                                            }
                                                        }
                                                    }
                                                    else if (value.Type == JTokenType.String) // If the array only contains property names
                                                    {
                                                        MessageBox.Show($"Got back: ({value}) {ParameterMethods[1]((string)value, Convert.ToInt32(key))}");
                                                        parameters.Add(ParameterMethods[1]((string)value, Convert.ToInt32(key)));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show($"Got back: ({(string)jsObj[key]}) {ParameterMethods[1]((string)jsObj[key], Convert.ToInt32(key))}");
                                                parameters.Add(ParameterMethods[1]((string)jsObj[key], Convert.ToInt32(key)));
                                            }
                                        }

                                        if (next_id != null)
                                        {
                                            GeneratePart((int)next_id);
                                        }
                                    }*/

        /*{"1": "HP", "2": "Strenght", "3":[{"Weapon":["MinDamage", "MaxDamage"]}]}*/
    }
}
