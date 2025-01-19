using System.Collections.Generic;

namespace NeteaseCloudMusicApi {
	internal static class Extensions {
		public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) {
			return CollectionExtensions.GetValueOrDefault(dictionary, key, defaultValue);
		}

		public static void MergeDictionary<TKey, TValue, TValue2>(this Dictionary<TKey, TValue> original, Dictionary<TKey, TValue2>? other) {
			if (other is null) return;
			foreach (var (key, value) in other)
			{
				if (value is null or "") {
					original.Remove(key);
				} else {
					original[key] = value is TValue value1 ? value1 : default!;
				}
			}
		}
	}
}
