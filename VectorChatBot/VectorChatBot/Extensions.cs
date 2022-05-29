using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace VectorChatBot
{
    internal static class Extensions
    {
        public static IEnumerable<Enum> GetFlags(this Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static bool IsNullableEnum(this Type t)
        {
            if (t == null) return false;
            Type u = Nullable.GetUnderlyingType(t);
            return (u != null) && u.IsEnum;
        }

        public static Type[] GetNestedTypesRecursive(this Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Static, Func<Type, bool> validator = null, List<Type> list = null)
        {
            if (list == null) list = new List<Type>();

            IEnumerable<Type> nested = type.GetNestedTypes(flags);
            if (validator != null)
                nested = nested.Where(f => validator.Invoke(f)).ToArray();
            list.AddRange(nested);

            foreach (var f in nested)
            {
                GetNestedTypesRecursive(f, flags, validator, list);
            }

            return list.ToArray();
        }

        //String.
        public static string FormatFrom(this string s, [NotNull] object obj)
        {
            foreach (Match f in Regex.Matches(s, @"(?<=\{)(\w|\d)+"))
            {
                var fProp = obj.GetType().GetProperty(f.Value);
                if (fProp != null)
                    s = s.Replace($"{{{f.Value}}}", fProp.GetValue(obj)?.ToString());
            }
            return s;
        }

        //Reflection.
        public static bool HasBaseType(this Type type, [NotNull] Type searchType)
        {
            if (type == searchType) return true;
            while (type.BaseType != null)
            {
                if (type.BaseType == searchType) return true;
                type = type.BaseType;
            }
            return type.GetInterfaces().Contains(type);
        }

        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public static Type GetNotNullable(this Type type)
        {
            if (type == null) return null;
            var trueType = Nullable.GetUnderlyingType(type);
            trueType ??= type; //Assign if is null.
            return trueType;
        }

        public static PropertyInfo GetPropertyRecursive([NotNull] this Type type, string propertyName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
        {
            PropertyInfo property = null;
            var currentType = type;

            while (currentType != typeof(object))
            {
                var splitProperty = propertyName.Split('.');

                if (splitProperty.Length > 1)
                {
                    foreach (var fPropertyName in splitProperty)
                    {
                        property = GetPropertyRecursive(currentType, fPropertyName, bindingFlags);
                        if (property != null)
                            break;
                    }

                    if (property != null) break;
                }
                else
                {
                    property = currentType.GetProperty(propertyName, bindingFlags);
                    if (property != null) break;
                    currentType = currentType.BaseType;
                }

            }
            return property;
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            var e = Expression.Lambda<Func<object>>(Expression.Convert(Expression.Default(type), typeof(object)));
            return e.Compile()();
        }

        public static string GetSystemName(this Type type)
        {
            return $"{type.FullName}, {type.Assembly.FullName}";
        }

        //String.
        public static string RemovePostfix(this string s, string postfix, bool caseSensative = true)
        {
            if (s == null || postfix == null) return s;
            if (s.Length < postfix.Length) return s;

            var remove = false;

            if (!caseSensative)
                remove = s.Substring(s.Length - postfix.Length, postfix.Length) == postfix;
            else
                remove = s.ToUpper().Substring(s.Length - postfix.Length, postfix.Length) == postfix.ToUpper();

            if (remove)
                s = s.Substring(0, s.Length - postfix.Length);

            return s;
        }

        public static string RemovePrefix(this string s, string prefix, bool caseSensative = true)
        {
            if (s == null || prefix == null) return s;
            if (s.Length < prefix.Length) return s;

            var remove = false;

            if (!caseSensative)
                remove = s.Substring(0, prefix.Length) == prefix;
            else
                remove = s.ToUpper().Substring(0, prefix.Length) == prefix.ToUpper();

            if (remove)
                s = s.Substring(prefix.Length, s.Length - prefix.Length);

            return s;
        }

        public static string RemoveSpecialCharacters(this string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string RemoveSpecialCharactersUsingRegex(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }
       
        //Collections
        public static object[] ToObjectArray(this IEnumerable enumerable)
        {
            return (from object f in enumerable select f).ToArray();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {           
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static Type GetEnumeratedType(this Type type)
        {
            // provided by Array
            var elType = type.GetElementType();
            if (null != elType) return elType;

            // otherwise provided by collection
            var elTypes = type.GetGenericArguments();
            if (elTypes.Length > 0) return elTypes[0];

            // otherwise is not an 'enumerated' type
            return null;
        }

        public static bool IsNonStringEnumerable(this PropertyInfo pi)
        {
            return pi != null && pi.PropertyType.IsNonStringEnumerable();
        }

        public static bool IsNonStringEnumerable(this object instance)
        {
            return instance != null && instance.GetType().IsNonStringEnumerable();
        }

        public static bool IsNonStringEnumerable(this Type type)
        {
            if (type == null || type == typeof(string))
                return false;
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Type is IEnumerable<T> include inherited type. 
        /// </summary>      
        public static bool IsEnumerableTypeOf<T>(this Type type)
        {
            return type != null && type.IsAssignableTo(typeof(IEnumerable<T>));
        }

        public static bool HasItems<T>(this IEnumerable<T> source)
        {
            return (source?.Any() ?? false);
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}      
