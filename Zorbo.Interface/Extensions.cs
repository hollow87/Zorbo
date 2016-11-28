using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Security;
using System.Runtime.InteropServices;
using Zorbo.Interface;
using System.Security.Cryptography;

namespace Zorbo
{
    public static partial class Extensions
    {
    
        public static uint ToUInt32(this IPAddress address) {
            return BitConverter.ToUInt32(address.GetAddressBytes(), 0);
        }

        public static IPAddress ToIPAddress(this uint address) {
            return new IPAddress(BitConverter.GetBytes(address));
        }


        public static bool IsLocal(this IPAddress address) {
            byte[] b = address.GetAddressBytes();

            if (b[0] == 10 ||
               (b[0] == 127 && b[1] == 0 && b[2] == 0 && b[3] == 1) ||
               (b[0] == 172 && b[1] >= 16 && b[1] <= 31) ||
               (b[0] == 192 && b[1] == 168)) return true;

            return false;
        }


        public static string ToNativeString(this SecureString secure) {

            IntPtr ptr = IntPtr.Zero;

            try {
                ptr = Marshal.SecureStringToGlobalAllocUnicode(secure);
                return Marshal.PtrToStringUni(ptr);
            }
            finally {
                if (ptr != IntPtr.Zero)
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }
        }

        public static SecureString ToSecureString(this string unsecure) {

            SecureString ret = new SecureString();
            foreach (char c in unsecure) ret.AppendChar(c);

            ret.MakeReadOnly();
            return ret;
        }

        public static string ToHtmlColor(this ColorCode color) {
            switch (color) {
                case ColorCode.White: return "#FFFFFF";
                case ColorCode.Black: return "#000000";
                case ColorCode.Navy: return "#000080";
                case ColorCode.Green: return "#008000";
                case ColorCode.Red: return "#FF0000";
                case ColorCode.Maroon: return "#800000";
                case ColorCode.Purple: return "#800080";
                case ColorCode.Orange: return "#FFA500";
                case ColorCode.Yellow: return "#FFFF00";
                case ColorCode.Lime: return "#00FF00";
                case ColorCode.Teal: return "#008080";
                case ColorCode.Aqua: return "#00FFFF";
                case ColorCode.Blue: return "#0000FF";
                case ColorCode.Fuchsia: return "#FF00FF";
                case ColorCode.Gray: return "#808080";
                case ColorCode.Silver: return "#C0C0C0";
                default: return string.Empty;
            }
        }

        public static string AnonUsername(this IPAddress address) {
            byte[] hash = null;
            byte[] addr = address.GetAddressBytes();

            Array.Reverse(addr);

            using(SHA1 sha1 = SHA1.Create()) 
                hash = sha1.ComputeHash(addr);

            return Convert.ToString(hash.Skip(5).Take(3).ToArray()).Replace("-", string.Empty);
        }

        public static string FormatUsername(this String @string) {

            string result = Regex.Replace(
                @string,
                "￼|­|\"|'|`|/|\\\\|www'.", 
                "").Replace('_', ' ');

            return result;
        }

        public static string Repeat(this string @string, int count) {
            count = count <= 0 ? 1 : count;//if 0 return input

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < count; i++)
                sb.Append(@string);

            return sb.ToString();
        }

        public static bool ContainsAny(this string @string, IEnumerable<String> collection) {

            foreach (String str in collection)
                if (@string.Contains(str)) return true;

            return false;
        }



        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> collection) {
            return new ObservableCollection<T>(collection);
        }


        public static bool Contains<T>(this IEnumerable<T> collection, Predicate<T> predicate) {

            foreach (T item in collection)
                if (predicate(item)) return true;

            return false;
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action) {

            foreach (var item in collection)
                action(item);
        }


        public static T Find<T>(this IEnumerable<T> collection, Predicate<T> predicate) {

            foreach (var item in collection)
                if (predicate(item)) return item;

            return default(T);
        }

        public static int FindIndex<T>(this IEnumerable<T> collection, Predicate<T> predicate) {
            int index = -1;
            foreach (var item in collection) {
                ++index;
                if (predicate(item)) return index;
            }
            return -1;
        }

        public static IEnumerable<T> FindAll<T>(this IEnumerable<T> collection, Predicate<T> predicate) {
            var ret = new List<T>();

            foreach (var item in collection)
                if (predicate(item)) ret.Add(item);

            return ret;
        }


        public static T Find<T>(this ObservableCollection<T> list, Predicate<T> predicate) {

            for (int i = 0; i < list.Count; i++)
                if (predicate(list[i])) return list[i];

            return default(T);
        }

        public static void Sort<T>(this ObservableCollection<T> list, Comparison<T> comparison) {
            for (int i = list.Count - 1; i >= 0; i--) {

                for (int j = 1; j <= i; j++) {

                    T o1 = list[j - 1];
                    T o2 = list[j];

                    if (comparison(o1, o2) > 0)
                        list.Move(j - 1, j);
                }
            }
        }

        public static void RemoveAll<T>(this ObservableCollection<T> list, Predicate<T> predicate) {

            for (int i = (list.Count - 1); i >= 0; i--)
                if (predicate(list[i])) list.RemoveAt(i);
            
        }


        /*****************************************************
                    DICTIONARY EXTENSIONS
         *****************************************************/

        /// <summary>
        /// An extension method for Dictionaries to perform ForEach lambda expressions
        /// </summary>
        public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Action<KeyValuePair<TKey, TValue>> action) {

            foreach (var keypair in dictionary)
                action(keypair);
        }

        /// <summary>
        /// An extension method for Dictionaries to perform Find lambda expressions
        /// </summary>
        public static KeyValuePair<TKey, TValue> Find<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Predicate<KeyValuePair<TKey, TValue>> predicate) {
            foreach (var pair in dictionary)
                if (predicate(pair)) return pair;

            return default(KeyValuePair<TKey, TValue>);
        }

        /// <summary>
        /// An extension method for Dictionaries to perform Find lambda expressions
        /// </summary>
        public static KeyValuePair<TKey, TValue> Find<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Predicate<KeyValuePair<TKey, TValue>> predicate, int startIndex) {
            foreach (var pair in dictionary)
                if (predicate(pair)) return pair;

            return default(KeyValuePair<TKey, TValue>);
        }

        /// <summary>
        /// An extension method for Dictionaries to perform FindAll lambda expressions
        /// </summary>
        public static IEnumerable<KeyValuePair<TKey, TValue>> FindAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Predicate<KeyValuePair<TKey, TValue>> predicate) {
            var ret = new List<KeyValuePair<TKey, TValue>>();

            foreach (var pair in dictionary)
                if (predicate(pair)) ret.Add(pair);

            return ret;
        }
    }
}
