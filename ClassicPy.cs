using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpedPython
{
    public class ClassicPy
    {
        public void print(string msg)
        {
            Console.WriteLine(msg);
        }

        public void print(int msg)
        {
            Console.WriteLine(msg);
        }

        public void print(double msg)
        {
            Console.WriteLine(msg);
        }

        public void print(bool msg)
        {
            Console.WriteLine(msg);
        }

        public void print(List<object> list)
        {
            Console.WriteLine($"[{string.Join(", ", list)}]");
        }

        public void print(Dictionary<object, object> dict)
        {
            string[] items = new string[dict.Count];
            int i = 0;
            foreach (var kvp in dict)
            {
                items[i++] = $"{kvp.Key}: {kvp.Value}";
            }
            Console.WriteLine($"{{{string.Join(", ", items)}}}");
        }

        public string Input(string question)
        {
            Console.WriteLine(question);
            string input = Console.ReadLine();
            return input;
        }

        public int len(string s)
        {
            return s.Length;
        }

        public int len<T>(List<T> list)
        {
            return list.Count;
        }

        public int len<T>(T[] array)
        {
            return array.Length;
        }

        public int len<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            return dict.Count;
        }

        public string str(object obj)
        {
            return obj.ToString();
        }

        public int Int(string s)
        {
            return int.Parse(s);
        }

        public double Float(string s)
        {
            return double.Parse(s);
        }

        public bool Bool(object obj)
        {
            return Convert.ToBoolean(obj);
        }

        public List<int> range(int stop)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < stop; i++)
            {
                result.Add(i);
            }
            return result;
        }

        public List<int> range(int start, int stop)
        {
            List<int> result = new List<int>();
            for (int i = start; i < stop; i++)
            {
                result.Add(i);
            }
            return result;
        }

        public List<int> range(int start, int stop, int step)
        {
            List<int> result = new List<int>();
            for (int i = start; i < stop; i += step)
            {
                result.Add(i);
            }
            return result;
        }

        public T max<T>(List<T> list) where T : IComparable
        {
            return list.Max();
        }

        public T max<T>(T a, T b) where T : IComparable
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        public T min<T>(List<T> list) where T : IComparable
        {
            return list.Min();
        }

        public T min<T>(T a, T b) where T : IComparable
        {
            return a.CompareTo(b) < 0 ? a : b;
        }

        public double sum(List<int> numbers)
        {
            return numbers.Sum();
        }

        public double sum(List<double> numbers)
        {
            return numbers.Sum();
        }

        public T abs<T>(T value) where T : struct, IComparable, IFormattable, IConvertible
        {
            dynamic dynValue = value;
            return Math.Abs(dynValue);
        }

        public List<T> sorted<T>(List<T> list)
        {
            List<T> result = new List<T>(list);
            result.Sort();
            return result;
        }

        public List<T> sorted<T>(List<T> list, bool reverse)
        {
            List<T> result = new List<T>(list);
            result.Sort();
            if (reverse)
            {
                result.Reverse();
            }
            return result;
        }

        public List<object> list(object[] array)
        {
            return new List<object>(array);
        }

        public List<object> list()
        {
            return new List<object>();
        }

        public Dictionary<object, object> dict()
        {
            return new Dictionary<object, object>();
        }

        public Dictionary<object, object> dict(object key, object value)
        {
            Dictionary<object, object> result = new Dictionary<object, object>();
            result[key] = value;
            return result;
        }

        public List<Tuple<T1, T2>> zip<T1, T2>(List<T1> first, List<T2> second)
        {
            List<Tuple<T1, T2>> result = new List<Tuple<T1, T2>>();
            int count = Math.Min(first.Count, second.Count);
            for (int i = 0; i < count; i++)
            {
                result.Add(Tuple.Create(first[i], second[i]));
            }
            return result;
        }

        public List<object[]> enumerate<T>(List<T> list)
        {
            List<object[]> result = new List<object[]>();
            for (int i = 0; i < list.Count; i++)
            {
                result.Add(new object[] { i, list[i] });
            }
            return result;
        }

        public string type(object obj)
        {
            return obj.GetType().ToString();
        }

        public List<T> reverse<T>(List<T> list)
        {
            List<T> result = new List<T>(list);
            result.Reverse();
            return result;
        }

        public bool any<T>(List<T> list, Predicate<T> predicate = null)
        {
            if (predicate == null)
            {
                return list.Count > 0;
            }
            return list.Exists(predicate);
        }

        public bool all<T>(List<T> list, Predicate<T> predicate)
        {
            return list.TrueForAll(predicate);
        }
    }
}