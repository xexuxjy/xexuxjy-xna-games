using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
 
namespace AdventOfCode2022_Task_Library
{
    public class Day21Compare //--- Day 21: Monkey Math ---
    {
        public static void Part01and02()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
 
            var input = File.ReadAllLines(@"D:\GitHub\xexuxjy\AdventOfCode\2022\Solutions\Data\puzzle-21-input.txt");
            List<Monkey> monkeys = new List<Monkey>();
 
            for (int i = 0; i < input.Length; i++)
            {
                var splits = input[i].Split(':');
                string name = splits[0];
                Int64 number = 0;
                string[] monkeyNames = new string[2];
                var operation = "";
 
                if (splits[1].Count() > 4)
                {
                    var data = splits[1].Trim().Split(' ');
                    monkeyNames = new string[] { data[0], data[2] };
                    operation = data[1];
                }
                else
                {
                    number = Int64.Parse(splits[1].Trim());
                }
 
                if (number != 0)
                {
                    Monkey monkey = new Monkey(name, number);
                    monkeys.Add(monkey);
                }
                else
                {
                    Monkey monkey = new Monkey(name, monkeyNames, operation);
                    monkeys.Add(monkey);
                }
            }
 
            List<Monkey> helperMonkeys = new List<Monkey>(monkeys);
 
            foreach (var monkey in helperMonkeys)
                GetNumber(ref monkeys, monkey);
 
            var root = monkeys.Find(e => e.Name == "root");
            Int64 rootNumber = root.Number;
 
            var newHumnNumber = GetHumnNumber(monkeys);
            


            sw.Stop();
            Console.WriteLine($"The monkey named \"root\" will yell: {rootNumber}\nThe new humn number is: {newHumnNumber}\nTime elapsed: {sw.Elapsed.Milliseconds}ms.\n\n");
            //Console.ReadKey();
        }
 
        internal static Int64 GetNumber(ref List<Monkey> monkeys, Monkey monkey)
        {
            if (monkey.Number != 0) return monkey.Number;
            else
            {
                Int64 number1 = 0;
                Int64 number2 = 0;
                Int64 number = 0;
                var monkeyNames = monkey.MonkeyNames;
                var monkey1 = monkeys.Find(e => e.Name == monkeyNames[0]);
                var monkey2 = monkeys.Find(e => e.Name == monkeyNames[1]);
 
                if (monkey1.Number != 0)
                    number1 = monkey1.Number;
                else
                    number1 = GetNumber(ref monkeys, monkey1);
 
                if (monkey2.Number != 0)
                    number2 = monkey2.Number;
                else
                    number2 = GetNumber(ref monkeys, monkey2);
 
                monkey.OperationValues = new Int64[] { number1, number2 };
 
                switch (monkey.Operation)
                {
                    case "+":
                        number = number1 + number2;
                        break;
                    case "*":
                        number = number1 * number2;
                        break;
                    case "-":
                        number = number1 - number2;
                        break;
                    case "/":
                        number = number1 / number2;
                        break;
                    default:
                        throw new Exception();
                }
 
                monkey.Number = number;
                return number;
            }
        }
 
        internal static Int64 GetHumnNumber(List<Monkey> monkeys)
        {
            var name = "humn";            
            var humnParents = new List<(string name, int level)>();
            var counter = 1;
            humnParents.Add((name, counter));
 
            while (monkeys.Find(e => e.MonkeyNames.Contains(name)).Name != "root")
            {
                name = monkeys.Find(e => e.MonkeyNames.Contains(name)).Name;
                counter++;
                humnParents.Add((name, counter));
            }
 
            var root = monkeys.Find(e => e.Name == "root");
            var rootChild2Name = root.MonkeyNames.FirstOrDefault(e => e != name);
            var rootChild2 = monkeys.Find(e => e.Name == rootChild2Name);
            var numberToMatch = rootChild2.Number;
            Int64 newNumber = 0;
 
            for (int i = humnParents.Count - 1; i > 0; i--)
            {
                var parent = monkeys.Find(e => e.Name == humnParents[i].name);
                var parentNumber = parent.Number;
                var childNumber = monkeys.Find(e => e.Name == humnParents[i - 1].name).Number;
                Int64 otherChildNumber = 0;
                var number1 = parent.OperationValues[0];
                var number2 = parent.OperationValues[1];
                var opFlag = true;

                Console.WriteLine("Checking node "+parent.Name+" OP "+parent.Operation);
 
                if (childNumber == number1)
                {
                    otherChildNumber = number2;
                }
                else if (childNumber == number2)
                {
                    otherChildNumber = number1;
                    opFlag = false;
                }
 
                switch (parent.Operation)
                {
                    case "+":
                        newNumber = numberToMatch - otherChildNumber;
                        Console.WriteLine($"PLUS {numberToMatch} - {otherChildNumber}");
                        break;
                    case "*":
                        Console.WriteLine($"MULT {numberToMatch} / {otherChildNumber}");
                        newNumber = numberToMatch / otherChildNumber;
                        break;
                    case "-":
                        if (opFlag) 
                        { 
                            Console.WriteLine($"MINUS {numberToMatch} + {otherChildNumber}");
                            newNumber = numberToMatch + otherChildNumber;
                        }
                        else 
                        { 
                            Console.WriteLine($"MINUS {otherChildNumber} - {numberToMatch}");
                            newNumber = otherChildNumber - numberToMatch;
                        }
                        break;
                    case "/":
                        if (opFlag)
                        {
                            Console.WriteLine($"DIV {numberToMatch} * {otherChildNumber}");
                            newNumber = numberToMatch * otherChildNumber;
                        }
                        else
                        {
                            Console.WriteLine($"DIV {otherChildNumber} / {numberToMatch}");
                            newNumber = otherChildNumber / numberToMatch;
                        }
                        break;
                    default:
                        throw new Exception();
                }
 
                numberToMatch = newNumber;
            }
 
            return numberToMatch;
        }
 
        internal class Monkey
        {
            public string Name { get; }
 
            public Int64 Number { get; set; }
 
            public string Operation { get; set; }
 
            public Int64[] OperationValues { get; set; }
 
            public string[] MonkeyNames { get; set; } = new string[2] { "", "" };
 
            public Monkey(string name, Int64 number)
            {
                Name = name;
                Number = number;
            }
 
            public Monkey(string name, string[] monkeyNames, string operation)
            {
                Name = name;
                MonkeyNames = monkeyNames;
                Operation = operation;
            }
        }
    }
}