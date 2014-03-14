using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Utils {

	//From http://stackoverflow.com/a/8001594
	public static T[] ConcatArrays<T>(params T[][] list){
		var result = new T[list.Sum(a => a.Length)];
		int offset = 0;
		for (int x = 0; x < list.Length; x++){
			list[x].CopyTo(result, offset);
			offset += list[x].Length;
		}
		return result;
	}

	//Take the arrays in the enumerator and concatenate them into a single array
	public static T[] Unpack<T>(IEnumerable<T[]> gen){
		return ConcatArrays<T> (gen.ToArray());
	}
}
