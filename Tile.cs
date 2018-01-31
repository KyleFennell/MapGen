using System;

namespace projectRPG{

	public class Tile{

		public Tile(){}										// this is called the default constructor and its what happens if you mane a new Tile and feed nothing into it ie = new Tile()

		public Tile(Tile other){							// this is a constructor like in map generator but it will only run if a room is fed into it ie = new Tile(Room)
			tileData(other);								// and it just calls the tileData method on that room
		}
		
		public void tileData(Tile other){					// takes data from another tile and makes it its own
			this.tileID = other.tileID;
			this.tileChar = other.tileChar;
			this.walkable = other.walkable;
			this.tileDescription = other.tileDescription;
			this.tileName = other.tileName;
		}

		// these are individual to each room and can be called and set from other classes
		public int coordX { get; set; }
		public int coordY { get; set; }
		public bool isPath { get; set; } = false;
		public bool isRoom { get; set; } = false;
		// these are things that can be gotten from a template through the tileData method
		public int tileID { get; set; }
		public char tileChar { get; set; }
		public bool walkable { get; set; }
		public string tileDescription { get; set; }
		public string tileName { get; set; }

	}
}					// this class isnt very fleshed out yet but it will be used by things other than just mapGenetator therefore isnt in map generator and has its own file