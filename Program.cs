using System;
using System.Collections.Generic;

namespace projectRPG{

	class MainClass{

		public static void Main(string[] args){
			MainClass main = new MainClass();
			main.run();
		}

		public void run(){
			MapGenerator gen = new MapGenerator(110, 50);
			// gen.makeBoxesNoOverlap(25, 50, 25, 50, 2, 100);
			gen.makeBoxesNoOverlap(15, 25, 15, 25, 2, 100);
			gen.makeBoxesNoOverlap(7, 15, 7, 15, 15, 10000);
			// gen.spacedShuffle(100, -1, true, .2, 20);
			gen.drawRooms();
			gen.smooth(0.20, 5);
			gen.makePaths(0.0, true, true, 2);
			gen.drawPaths();
			// filePrint(gen);
			Console.WriteLine(print(gen));
			Console.WriteLine("print done");
			Console.Read();
		}

		// returns a string that is a visual representation of the map
		public string print(MapGenerator gen){
			string output = "";
			for (int y = 0; y < gen.height; y++){
				for (int x = 0; x < gen.width; x++){	
					// if (gen.map[x,y].isPath){
						// output += "  ";
					// }
					// else{
						output += gen.map[x, y].tileChar + " ";	
					// }
				}
				output += "\n";
			}
			return output;
		}

		public void filePrint(MapGenerator gen){
			string output = "";
			for (int y = 0; y < gen.height; y++){
				for (int x = 0; x < gen.width; x++){	
					// if (gen.map[x,y].isPath){
						// output += "  ";
					// }
					// else{
						output += gen.map[x, y].tileChar + "";	
					// }
				}
				output += "\n";
			}
			System.IO.File.WriteAllText(@"C:\Users\Kyle\Dropbox\Programs\RPG Project\map.txt", output);
		}

	}
}