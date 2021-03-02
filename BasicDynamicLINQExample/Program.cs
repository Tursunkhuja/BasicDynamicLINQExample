using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace BasicDynamicLINQExample
{
    class Program
    {
        static void Main(string[] args)
        {
            const string exp = @"Age > 20 AND Weight > 50";

            var bob = new Person
            {
                Name = "Bob",
                Age = 30,
                Weight = 213,
                FavouriteDay = new DateTime(2000, 1, 1)
            };
            var wendy = new Person
            {
                Name = "Wendy",
                Age = 18,
                Weight = 180,
                FavouriteDay = new DateTime(2001, 2, 2)
            };
            var people = new List<Person> { bob, wendy };
            var matchedPeople = people.Where(exp);
            Console.WriteLine("List of person who Age > 20 AND Weight > 50");
            foreach (var item in matchedPeople)
            {
                Console.WriteLine(item.Name);
            }
            //It will print only Bob

            Console.ReadKey();
        }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public int Weight { get; set; }
        public DateTime FavouriteDay { get; set; }
    }

}
