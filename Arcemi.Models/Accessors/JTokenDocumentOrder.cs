using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{
    internal sealed class JTokenDocumentOrder : IComparer<JToken>
    {
        public static JTokenDocumentOrder Instance { get; } = new JTokenDocumentOrder();

        public int Compare(JToken first, JToken second)
        {
            if (ReferenceEquals(first, second)) return 0;
            var left = first.AncestorsAndSelf().Reverse().ToArray();
            var right = second.AncestorsAndSelf().Reverse().ToArray();
            var index = left.Zip(right).TakeWhile(pair => ReferenceEquals(pair.First, pair.Second)).Count();
            if (index == 0) {
                // A previously removed subtree can still be present in a reference list.
                return 0;
            }
            if (index == left.Length) return -1;
            if (index == right.Length) return 1;
            for (var token = left[index].Next; token != null; token = token.Next) {
                if (ReferenceEquals(token, right[index])) return -1;
            }
            return 1;
        }
    }
}
