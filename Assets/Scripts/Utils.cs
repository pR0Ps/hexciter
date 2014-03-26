using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public static class Utils {

	//Efficiently concatenate arrays
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

	//Take the arrays in the GetSiblings enumerator and concatenate them into a single array
	public static T[] Unpack<T>(IEnumerable<T[]> gen){
		return ConcatArrays<T> (gen.ToArray());
	}

	//Functions to pass into the GetSiblings function to filter the added siblings
	public static Func<GridPlace,Boolean> check_busy(bool busy){
		return gp => {
			return gp.busy == busy;
		};
	}
	public static Func<GridPlace,Boolean> check_alive(bool alive){
		return gp => {
			return gp.alive == alive;
		};
	}
	public static Func<GridPlace,Boolean> check_color(int color){
		return gp => {
			return gp.hexaCube.hexColor == color;
		};
	}
	
	//Generator that yields lists of GridPlaces by increasing depth from the origin
	//Returns the origin first, then a list of all it's siblings,
	//then all their siblings, etc.
	//The optional functions in the 'checks' list each take a GridPlace and return a bool
	//representing if the sibling is a valid addition to the output set.
	//All the checks must be true for the sibling to be added
	public static IEnumerable<GridPlace[]> GetSiblings(GridPlace parent, params Func<GridPlace,Boolean>[] checks) {
		Dictionary<GridPlace,int> seen = new Dictionary<GridPlace,int>();
		Queue<GridPlace> queue = new Queue<GridPlace>();

		if (!checks.All(fcn => fcn(parent))) yield break;
		
		int currDepth = -1;
		
		//Add the start node
		queue.Enqueue(parent);
		seen.Add(parent, 0);
		
		//BFS
		while (queue.Count > 0){
			//Check the depth of the oldest item in the queue
			int depth = seen[queue.Peek()];
			if (depth > currDepth){
				//If the depth is greater than the current depth it means that all
				//items of the previous depth have been removed from the queue
				//What's left will be all the items at currDepth + 1
				currDepth = depth;
				yield return queue.ToArray();
			}
			
			//Take the oldest item out of the queue and add it's children the to the
			//queue at depth + 1 (if they haven't already been added)
			GridPlace temp = queue.Dequeue();
			List<GridPlace> siblings = temp.sibs.ExistingSibs();
			for (int i = 0; i < siblings.Count; i ++) {
				//checks.All goes though the list of passed in checks (if any) and
				//executes the functions on the sibling
				//If all the functions return true, the sibling is added
				if (!seen.ContainsKey(siblings[i]) && checks.All(fcn => fcn(siblings[i]))){
					seen.Add(siblings[i], depth + 1);
					queue.Enqueue(siblings[i]);
				}
			}
		}
	}

	//Get num random non-black GridPlaces
	public static GridPlace[] GetRandomNonBlack (GridPlace start, int num) {
		//Get all grid places, filter by those that aren't black
		GridPlace[] all = Unpack<GridPlace>(GetSiblings(start)).Where(gp => gp.hexaCube.hexColor != Constants.HEX_BLACK).ToArray();
		if (all.Length <= num) {
			return all;
		}
		//Shuffle the list and take the first num values
		return all.OrderBy(gp => UnityEngine.Random.value).Take(num).ToArray();
	}

	//Set all the linked GridPlaces to reserved
	public static void ReserveAll(GridPlace start){
		foreach (GridPlace gp in Unpack<GridPlace>(GetSiblings(start, check_alive(true), check_color(start.hexaCube.hexColor)))){
			gp.reserved = true;
		}
	}

	//Tally the score starting with the passed in GridPlace
	public static int TallyScore (GridPlace start) {
		int tally = 0;
		foreach (GridPlace gp in Unpack<GridPlace>(GetSiblings(start, check_alive(true), check_color(start.hexaCube.hexColor)))){
			gp.reserved = true;
			tally += 1;
		}
		return tally;
	}

	//Scale the siblings of the passed in GridPlace (to a depth of 2)
	public static void ScaleSiblings(GridPlace start, bool normalize){
		int depth = 0;
		foreach (GridPlace[] ring in Utils.GetSiblings(start)){
			foreach (GridPlace gp in ring){
				gp.Scale(depth, normalize);
			}
			if (depth > 2) break;
			depth++;
		}
	}

	//Fill the connected siblings of the passed in GridPlace
	public static IEnumerator FillSiblings (GridPlace start, int fillColor) {
		foreach (GridPlace[] ring in GetSiblings(start, check_busy(false), check_alive(true), check_color(start.hexaCube.hexColor))){
			foreach (GridPlace gp in ring){
				gp.busy = true;
				gp.reserved = false;
				gp.hexaCube.Fill(fillColor);
			}
			yield return new WaitForSeconds (0.1f);
		}
	}

	//Kill the connected siblings of the passed in GridPlace
	public static IEnumerator KillSiblings (GridPlace start) {
		foreach (GridPlace[] ring in GetSiblings(start, check_busy(false), check_alive(true), check_color(start.hexaCube.hexColor))){
			foreach (GridPlace gp in ring){
				gp.busy = true;
				gp.hexaCube.Kill();
			}
			yield return new WaitForSeconds (0.1f);
		}
	}

	//Slow spawn all siblings of the passed in GridPlace
	public static IEnumerator SlowSpawnSiblings (GridPlace start) {
		foreach (GridPlace[] ring in GetSiblings(start, check_busy(false), check_alive(false))){
			foreach (GridPlace gp in ring){
				gp.busy = true;
				gp.hexaCube.SlowSpawn();
			}
			yield return new WaitForSeconds(.3f);
		}
	}

	//Despawn all siblings of the passed in GridPlace
	public static IEnumerator DespawnSiblings (GridPlace start) {
		foreach (GridPlace[] ring in GetSiblings(start, check_busy(false), check_alive(true))){
			foreach (GridPlace gp in ring){
				gp.busy = true;
				gp.hexaCube.Despawn();
			}
			yield return new WaitForSeconds(.1f);
		}
	}
}
