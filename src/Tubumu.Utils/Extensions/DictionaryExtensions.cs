﻿using System;
using System.Collections.Generic;

namespace Tubumu.Utils.Extensions
{
    /// <summary>
    /// DictionaryExtensions
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 将目标字典的全部元素累复制入源字典中
        /// </summary>
        /// <typeparam name="TDictionary">源字典类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="source">源字典</param>
        /// <param name="copy">目标字典</param>
        /// <returns>复制了新元素的源字典</returns>
        public static TDictionary Concat<TDictionary, TKey, TValue>(
            this TDictionary source,
            IDictionary<TKey, TValue> copy)
            where TDictionary : IDictionary<TKey, TValue>
        {
            foreach (var pair in copy)
            {
                source.Add(pair.Key, pair.Value);
            }
            return source;
        }

        /// <summary>
        /// 将目标字典的指定元素累复制入源字典中
        /// </summary>
        /// <typeparam name="TDictionary">源字典类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="source">源字典</param>
        /// <param name="copy">目标字典</param>
        /// <param name="keys">要复制的元素的键集合</param>
        /// <returns>复制了新元素的源字典</returns>
        public static TDictionary Concat<TDictionary, TKey, TValue>(
            this TDictionary source,
            IDictionary<TKey, TValue> copy,
            IEnumerable<TKey> keys)
            where TDictionary : IDictionary<TKey, TValue>
        {
            foreach (var key in keys)
            {
                source.Add(key, copy[key]);
            }

            return source;
        }

        /// <summary>
        /// 将目标字典的全部元素累从源字典中移除
        /// </summary>
        /// <typeparam name="TDictionary">源字典类型</typeparam>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="source">源字典</param>
        /// <param name="keys"></param>
        /// <returns>移除了元素的源字典</returns>
        public static TDictionary RemoveKeys<TDictionary, TKey, TValue>(
            this TDictionary source,
            IEnumerable<TKey> keys)
            where TDictionary : IDictionary<TKey, TValue>
        {
            foreach (var key in keys)
            {
                source.Remove(key);
            }

            return source;
        }

        /// <summary>
        /// 将目标字典的指定元素累从源字典中移除
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="source">源字典</param>
        /// <param name="keys">要移除的元素的键集合</param>
        /// <returns>移除了元素的源字典</returns>
        public static IDictionary<TKey, TValue> RemoveKeys<TKey, TValue>(
            this IDictionary<TKey, TValue> source,
            IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                source.Remove(key);
            }

            return source;
        }

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
            this IDictionary<TKey, TValue> first,
            IDictionary<TKey, TValue> second)
            where TKey : notnull
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var key in first.Keys)
            {
                result[key] = first[key];
            }
            foreach (var key in second.Keys)
            {
                result[key] = second[key];
            }
            return result;
        }

        public static bool DeepEquals<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second) where TKey: notnull
        {
            var comparer = new DictionaryComparer<TKey, TValue>();
            return comparer.Equals(first, second);
        }

        public static int DeepGetHashCode<TKey, TValue>(this IDictionary<TKey, TValue> dic) where TKey : notnull
        {
            var comparer = new DictionaryComparer<TKey, TValue>();
            return comparer.GetHashCode(dic);
        }
    }

    public class DictionaryComparer<TKey, TValue> : IEqualityComparer<IDictionary<TKey, TValue>> where TKey: notnull
    {
        public bool Equals(IDictionary<TKey, TValue>? x, IDictionary<TKey, TValue>? y)
        {
            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }
            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
            }
            if (x.Count != y.Count)
            {
                return false;
            }
            foreach (var kvp in x)
            {
                if (!y.TryGetValue(kvp.Key, out var value))
                {
                    return false;
                }
                if ((value == null && kvp.Value != null) ||
                    (value != null && kvp.Value == null) ||
                    (value != null && kvp.Value != null && !value.Equals(kvp.Value))
                    )
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(IDictionary<TKey, TValue> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            var hash = 0;
            foreach (var kvp in obj)
            {
                hash ^= kvp.Key.GetHashCode();
                if (kvp.Value != null)
                {
                    hash ^= kvp.Value.GetHashCode();
                }
            }
            return hash;
        }
    }
}
