using System;
using System.Collections.Generic;

namespace projectRPG{

	static public class TileDB{

		static public Dictionary<int, Tile> Tiles;		// declarint the dictionary to be named Tiles just like you declare ints

		static TileDB(){			// this is the constructor and to be honnest im not sure when this gets called. i think it may be on compile actually
			Tiles = new Dictionary<int, Tile>();	// a dictionary is a data structure like a list or array that stores things with a 'key', that can then be retrueved with that 'key'
														// mine is kind of like an array at the moment because im using int keys but they could be string keys or Room keys.
			int i;								// i is my key. it isnt important to decalre a variable for your key either i just like this layout

			// i = ;
			// TileDB[i] = new Tile();
			// TileDB[i].tileID = i
			// TileDB[i].tileChar = '';
			// TileDB[i].walkable = 
			// TileDB[i].tileDescription = "";
			// TileDB[i].tileName = "";

			i = -1;								// for the key -1
			Tiles[i] = new Tile();				// for the dictionary 'Tiles' object (Tile) at key (i), make a new tile
			Tiles[i].tileID = -1;				// set that tile's id to -1
			Tiles[i].tileChar = '\\';			// its character to \ 				though i usually change this later its just because i dont think \ is going to be used and ill change it if it is
			Tiles[i].walkable = false;			// set its walkability
			Tiles[i].tileDescription = "debug";	// ect
			Tiles[i].tileName = "debug";

			i = 0;
			Tiles[i] = new Tile();
			Tiles[i].tileID = i;
			Tiles[i].tileChar = ' ';
			Tiles[i].walkable = false;
			Tiles[i].tileDescription = "";
			Tiles[i].tileName = "Unknown";

			i = 1;
			Tiles[i] = new Tile();
			Tiles[i].tileID = i;
			Tiles[i].tileChar = '.';
			Tiles[i].walkable = true;
			Tiles[i].tileDescription = "It's pretty dirty";
			Tiles[i].tileName = "Ground";

			i = 2;
			Tiles[i] = new Tile();
			Tiles[i].tileID = i;
			Tiles[i].tileChar = '%';
			Tiles[i].walkable = false;
			Tiles[i].tileDescription = "Solid stone";
			Tiles[i].tileName = "Wall";
		}
	
	}

}