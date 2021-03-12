using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;

namespace BasicDynamicLINQExample
{
    class Program
    {
        static void Main(string[] args)
        {
            string pattern = "Weight==213 AND Payment.Age>20";
            //string pattern1 = "FavouriteDay=1/1/2000";
            //string pattern2 = "Id.ToString() = \"7ca7ed4f-239c-47b0-bf2d-bcb1075a5719\"";
            string propertyPath = "Convert.ToDouble(Payment.Age)";
            var bob = new ExpandoObject() as IDictionary<string, object>;
            bob.Add("Name", "Bob");
            var payment = new Dictionary<string, object>();
            payment.Add("Age", 30);
            bob.Add("Payment", payment);
            bob.Add("Weight", 213);
            bob.Add("FavouriteDay", new DateTime(2000, 1, 1));
            bob.Add("Id", new Guid("7ca7ed4f-239c-47b0-bf2d-bcb1075a5719"));

            var wendy = new ExpandoObject() as IDictionary<string, object>;
            wendy.Add("Name", "Wendy");
            payment = new Dictionary<string, object>();
            payment.Add("Age", 20);
            wendy.Add("Payment", payment);
            wendy.Add("Weight", 180);
            wendy.Add("FavouriteDay", new DateTime(2001, 2, 2));
            wendy.Add("Id", new Guid("7ca7ed4f-239c-47b0-bf2d-bcb1075a5718"));

            var people = new List<IDictionary<string, object>> { bob, wendy };

            pattern = GetCompleteWhereClause(bob, pattern);
            var matchedPeople = people.AsQueryable().Where(pattern);
            var summa = people.AsQueryable().Sum(propertyPath);
            foreach (dynamic person in matchedPeople)
            {
                Console.WriteLine($" - {person.Name}");
            }
            //It will print only Bob

            Console.ReadKey();
        }

        static string GetCompleteWhereClause(IDictionary<string, object> item, string pattern)
        {
            var expressions = pattern.Split(new string[] { "AND", "OR", "&&", "||" }, StringSplitOptions.RemoveEmptyEntries);
            string[] logicalOperators = new string[] { "<", "<=", ">", ">=", "!=", "<>", "=", "==" };
            string customExpression = pattern;
            for (int i = 0; i < expressions.Length; i++)
            {
                var operands = expressions[i].Split(logicalOperators, StringSplitOptions.RemoveEmptyEntries);
                string firstOperandType = null;
                for (int j = 0; j < operands.Length; j++)
                {
                    var properties = operands[j].Split('.');
                    var type = GetPropertyType(properties);
                    if (type != null)
                    {
                        firstOperandType = type;
                        customExpression = customExpression.Replace(operands[j].Trim(), $"Convert.To{firstOperandType}({operands[j].Trim()})");
                    }
                    else if (firstOperandType != null)
                        customExpression = customExpression.Replace(operands[j].Trim(), $"Convert.To{firstOperandType}(\"{operands[j].Trim()}\")");
                }
            }
            return customExpression;

            string GetPropertyType(string[] properties)
            {
                string type = null;
                var itemObject = item;
                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i].Trim();
                    if (itemObject.ContainsKey(property))
                    {
                        if (itemObject[property] is ValueType && i == properties.Length - 1)
                            type = itemObject[property].GetType().Name;
                        else
                        {
                            var childItem = itemObject[property] as IDictionary<string, object>;
                            itemObject = childItem;
                        }
                    }
                }

                return type;
            }
        }
    }
}
