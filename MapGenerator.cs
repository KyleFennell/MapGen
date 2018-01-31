using System;
using System.Collections.Generic;

namespace projectRPG{

	class MapGenerator{

		public int width {get; set;}
		public int height {get; set;}
		private List<Room> rooms;
		private List<List<Room>> networks;
		private List<Path> paths;
		private Random random = new Random();						// this makes an instance of the Random Object that is just a rng really
		public Tile[,] map {get;}

		// private bool debug = false;


		public MapGenerator(int width, int height){					// this is what gets called when you say MapGenerator name = new MapGenerator(int, int)
			this.width = width;										// sets the width variable for this mapgeneerator (line 8) to the width taken in
			this.height = height;									
			rooms = new List<Room>();								// creates a new List (dynamic size array youll learn about in seng1120) that will store Room objects just like an int array stores ints
			paths = new List<Path>();								// creates another one called paths that stores path objects		// you can find the room and path objects at the bottom of this file
			networks = new List<List<Room>>();						// creates a list of lists kinda like a variable size and shape 2d array that stores lists of rooms
			map = new Tile[width, height];							// creates a 2d array of tiles that is width by height in dimention
			for (int y = 0; y < height; y++){					// traversing the 2d array that is the map
				for (int x = 0; x < width; x++){
					map[x,y] = new Tile(TileDB.Tiles[0]);			// because tiles are objects not ints they need to be set to something otherwise they will be null and cause whats called a null pointer error when called
					map[x,y].coordX = x;							// sets the int coordX for the tile located at the position x,y in the map array to x
					map[x,y].coordY = y;							// this allows the tiles in the array to be somewhat self aware and allows me to locate tiles reletive to this one without knowing the exact position of it kinda
				}
			}
		}

		public void makeBoxesOverlap(int minWidth, int maxWidth, int minHeight, int maxHeight, int number){
			for (int i = 0; i < number; i++){							// you know for loops and this ones pretty self explanitory
				Room room = new Room();									// creating a new Room object (object class located at line 440ish. i made room a sublcass because its small and only the map generator uses it)
				room.width = random.Next(minWidth, maxWidth + 1);		// makes the width of the room a random number between the num and max width (even though the second peramiter has a +1 thats just how the method works)
				room.height = random.Next(minHeight, maxHeight + 1);	// same for height
				room.top = random.Next(0, height - room.height);		// picks a random VALID starting location for the top left corner of the room
				room.left = random.Next(0, width - room.width);	
				rooms.Add(room);										// adds the newly created room to that list of rooms declared at line 10 and instantiated at line 22.
				room.number = i;										// sets the variable 'number' in the new room object to i.i used this in debugging so i could see the order of the rooms created and also distinguish them
				// Console.WriteLine(room.top+" "+room.right+" "+room.bottom+" "+room.left);
			}
		}

		public void makeBoxesNoOverlap(int minWidth, int maxWidth, int minHeight, int maxHeight, int number, int itterations){
			int count = 0;												// count is used to count the failed attempts
			int roomNo = 0;												// this is used like i was in the last method
			while (roomNo < number && count < itterations){			// while either the number of rooms is less the the target number or the fail attempts hasnt been met
				Room room = new Room();									// make a new room
				room.width = random.Next(minWidth, maxWidth + 1);		// makes the width of the room a random number between the num and max width (even though the second peramiter has a +1 thats just how the method works)
				room.height = random.Next(minHeight, maxHeight + 1);	// same for height
				room.top = random.Next(0, height - room.height);		// picks a random VALID starting location for the top left corner of the room
				room.left = random.Next(0, width - room.width);	
				if (!roomCollides(room)){								// this method is located at the bottom but returns true if the rooms collide so this will only triger if they dont due to the ! (not);
					rooms.Add(room);									// so if the room doesnt collide with any rooms add it to the list of rooms
					room.number = roomNo;
					// drawRooms();
					// Console.WriteLine(print());
					roomNo++;
				}
				else {													// if it collides increment fail counter
					count ++;
				}
			}
			drawRooms();
		}

		public void spacedShuffle(int itterations, int gap, bool killOnEdge, double delete, int cullPoint){
			bool overlapped = true;
			for (int i = 0; i < itterations && overlapped; i++){
				overlapped = false;	
				for (int j = 0; j < rooms.Count; j++){
					bool currentOverlapped = false;
					bool[] overlapStatus = roomOverlaps(rooms[j], gap);
					if (overlapStatus[0] && rooms[j].bottom < height-1){
						rooms[j].top++;
						currentOverlapped = true;
					}
					if (overlapStatus[1] && rooms[j].left > 0){
						Console.WriteLine(rooms[j].number+" right");
						rooms[j].left--;
						currentOverlapped = true;
					}
					if (overlapStatus[2] && rooms[j].top > 0){
						Console.WriteLine(rooms[j].number+" bottom");
						rooms[j].top--;
						currentOverlapped = true;
					}
					if (overlapStatus[3] && rooms[j].right < width-1){
						Console.WriteLine(rooms[j].number+" left");
						rooms[j].left++;
						currentOverlapped = true;
					}
					if ((!killOnEdge && currentOverlapped && i >= cullPoint && random.NextDouble() <= delete) ||
							(killOnEdge && i >= cullPoint && (rooms[j].right == width-1 || rooms[j].top == 0 ||
							rooms[j].left == 0 || rooms[j].bottom == height-1))){
						rooms.RemoveAt(j);
					}
					if (currentOverlapped){
						overlapped = true;
					}
				}
				cleanMap(false, true);
				drawRooms();
				Console.WriteLine(print());
				Console.Read();
				Console.Read();
			}
		}

		public void makeDoors(double chance){

		}

		public void deleteWalls(double chance){

		}
		
		//this is responsable for finding the situations in which a path
		public void makePaths(double extraPath, bool termPath, bool termRoom, int pathType){		// this is a trickey one
			int attempts = 0;																			// this is put in place to stop a case where its impossible to join all rooms but i havnt found one yet
			int extraPaths = 0;																			// used for debugging / keeping track of how many extra paths were created. its a nice tracker
			networks.Add(new List<Room>());																// creates a new list of rooms and adds it to the list of lists (ill just refer to it as networks now)										
			if (!networks[0].Contains(rooms[0])){														// basically just checking if this method has been called before
				networks[0].Add(rooms[0]);																// if it hasnt then it adds the first room that was ever created to the first network in networks
				rooms[0].inNetwork = true;																// and sets the bool property in rooms 'inNetwork' to true. this is a usefull proporty that prevents 
			}																									// checking all the networks for a room each time. now a check is only nessecary for which netwprk is in
			// drawRooms();																				// just updates the map with the rooms because an up to date map is nessecary

			while (rooms.Count != networks[0].Count && attempts < rooms.Count*10){						// while the number of rooms in the main netowrk != to the total number of rooms && the number of attempts is less than 10 times the total rooms
				for (int i = 0; i < rooms.Count; i++){													// rooms.Count is the same as an array.length. this means that i will be used to get a single room from rooms
					int r2 = ( i + random.Next(1, rooms.Count) ) %rooms.Count;							// clever maths to select a room other than the one already selected
					// Console.WriteLine("Connecting rooms: " + i + " " + r2);										
					for (int j = 0; j < networks.Count; j++){											// same as the i loop this is selecting networks one at a time. its looking through network finding one with valid contitions
						if ((networks[j].Contains(rooms[i]) ^ networks[j].Contains(rooms[r2]))			// ^ == XOR. so if only one of the rooms is in network j ie in different networks
								 || (rooms[i].inNetwork ^ rooms[r2].inNetwork)){						// OR only one of the rooms is in a network
							// Console.WriteLine("Regular generation");
							switch (pathType){
								case 0:
									generatePathL(rooms[i], rooms[r2], termPath, termRoom);						// generate a path between the two rooms passing through the settings for room and path termination
									break;
								case 1:
									generatePathDiag(rooms[i], rooms[r2], termPath, termRoom);
									break;
								case 2:
									generatePathRandom(rooms[i], rooms[r2], termPath, termRoom);
									break;
								case 3:
									// generatePathOrganic(rooms[i], rooms[r2], termPath, termRoom);
									break;
								default:
									generatePathL(rooms[i], rooms[r2], termPath, termRoom);
									break;
							}
							// Console.WriteLine(print());
							break;																		// and because a valid option was found break out of the for loop
						}
						else if (networks[j].Contains(rooms[i]) && networks[j].Contains(rooms[r2])		// if the two rooms are in the same network
								 && random.NextDouble() < extraPath){									// AND the random chance succeeds
							// Console.WriteLine("Extra chance!");
							switch (pathType){
								case 0:
									generatePathL(rooms[i], rooms[r2], false, true);				// generate a path that wont collide with paths but will with rooms. this stops failure states
									break;
								case 1:
									generatePathDiag(rooms[i], rooms[r2], false, true);
									break;
								case 2:
									generatePathRandom(rooms[i], rooms[r2], false, true);
									break;
								case 3:
									// generatePathOrganic(rooms[i], rooms[r2], false, true);
									break;
								default:
									generatePathL(rooms[i], rooms[r2], false, true);
									break;
							}							
							// Console.WriteLine(print());
							extraPaths++;																// increment extra paths counter. again this isnt used for anycaylculations but is usefull
							break;																		// and again valid conditions were met so break out of loop
						}
						else if (j == networks.Count-1){												// if this is the last cycle of the loop
							// Console.WriteLine("Failed to generate path");
							if (networks[j].Contains(rooms[i]) && networks[j].Contains(rooms[r2])){		// debugging, disreguard
								// Console.WriteLine("in same network");
							}
							attempts++;																	// increment failed attempts because n valid paths could be created
							// Console.WriteLine("attempts: "+attempts);
						}
					}
				}
			}
			Console.WriteLine("generation complete");
			if (attempts >= rooms.Count*10){
				Console.WriteLine("out of attempts");
			}
			Console.WriteLine("extra paths generated: "+extraPaths);
		}

		// this is responsible for actually making the paths
		private void generatePathL(Room r1, Room r2, bool termPath, bool termRoom){
			Path temp = new Path();													// creates a new Path object
			int x = r1.centreX;														// sets x to the x coord of the centre of room r1
			int y = r1.centreY;														// same for y
			bool collision = false;													// sets a boolean flag for collision to false
			bool roomCollision = false;
			Room orRoom = new Room();
			paths.Add(temp);														// adds the new path to the list paths. 
														// ***because c# uses references the thing i just put in the list will continue to update id i chance anything about temp***
			temp.startRoom = r1;													// sets the start room proporty for the path to r1 (this is an int value)

			while (x != r2.centreX && !collision){									// while x isnt in line with rooms 2's cemtre x and there hasnt been a collision
				// drawPaths();
				if (termRoom && map[x,y].isRoom && !r1.tiles.Contains(map[x,y])		// if termination on rooms is enabled and the current tile is part of a room and its not part the starting room
						 && !r2.tiles.Contains(map[x,y])){							// and its not part of the end toom. 		so basically if termRoom and if hit a room that isnt start or target
					foreach (Room r in rooms){										// foreach is the same as a for loop using the i as list[i]. just does that automatically 
						if (r.tiles.Contains(map[x,y])){							// if r is the room it has collided with
							manageNetworks(r1, r);									// combine the netowrks
							collision = true;										// the path has collided
							roomCollision = true;
							orRoom = r;
							// Console.WriteLine("path interupted by room x");
							break;													// valid collision so stop searching through rooms
						}
					}
				}
				else if (termPath && map[x,y].isPath && !map[x,y].isRoom){  		// if termination on paths is enabled and the current tile is part of a path and isnt a room
					foreach (Path p in paths){										// loop through all the paths
						if (p.tiles.Contains(map[x,y])){							// and find the one that this tile is a part of
							manageNetworks(r1, p.startRoom);						// then connect the networks of room 1 and the starting room of the path
							collision = true;										// the path has collided
							// Console.WriteLine("path interupted by path x");
							break;													// valid collieion so stop searching through paths
						}
					}
				}
				if (!collision){													// if there hasnt been a collision for the current tile
					map[x,y].isPath = true;
					x += (x < r2.centreX)? 1:-1;									// if x < centre of room 2 then x += 1. other wise its greater so -= 1
					temp.tiles.Add(map[x,y]);										// each path knows what tiles are in it so add the current tile to the current path
				}
			}
			// 
			while (y != r2.centreY && !collision){
				if (termRoom && map[x,y].isRoom && !r1.tiles.Contains(map[x,y])
						 && !r2.tiles.Contains(map[x,y])){
					foreach (Room r in rooms){
						if (r.tiles.Contains(map[x,y])){
							manageNetworks(r1, r);
							collision = true;
							roomCollision = true;
							orRoom = r;
							// Console.WriteLine("path interupted by room y");
							break;
						}				
					}
				}
				else if (termPath && map[x,y].isPath && !map[x,y].isRoom){
					foreach (Path p in paths){
						if (p.tiles.Contains(map[x,y])){
							manageNetworks(r1, p.startRoom);
							collision = true;
							// Console.WriteLine("path interupted by path y");
							break;
						}
					}
				}
				if (!collision){
					map[x,y].isPath = true;
					y += (y < r2.centreY)? 1:-1;
					temp.tiles.Add(map[x,y]);
				}								// this does the same thing but for the y coordinate						
			}																		// so now its reached the centre of room 2 or collided
			if (!collision){														// if there hasnt been a collision
				manageNetworks(r1, r2);												// add the networks of the start and end rooms because theyre connected now
				// Console.WriteLine("path made it to destination");
			}
			else if (roomCollision){								// if collided with a room continue the path to the centre of the new room
				while (x != orRoom.centreX){
					map[x,y].isPath = true;
					x += (x < orRoom.centreX)? 1:-1;
					temp.tiles.Add(map[x,y]);
				}
				while (y != orRoom.centreY){
					map[x,y].isPath = true;
					y += (y < orRoom.centreY)? 1:-1;
					temp.tiles.Add(map[x,y]);
				}
			}
		}

		private void generatePathDiag(Room r1, Room r2, bool termPath, bool termRoom){
			Path temp = new Path();
			int x = r1.centreX;
			int y = r1.centreY;
			bool collision = false;
			bool roomCollision = false;
			Room orRoom = new Room();
			bool xTurn = true;
			paths.Add(temp);
			temp.startRoom = r1;

			while ((x != r2.centreX || y != r2.centreY) && !collision){
				// drawPaths();
				if (termRoom && map[x,y].isRoom && !r1.tiles.Contains(map[x,y]) && !r2.tiles.Contains(map[x,y])){
					foreach (Room r in rooms){
						if (r.tiles.Contains(map[x,y])){
							manageNetworks(r1, r);
							collision = true;
							roomCollision = true;
							orRoom = r;
							// Console.WriteLine("path interupted by room x");
							break;
						}
					}
				}
				else if (termPath && map[x,y].isPath && !map[x,y].isRoom){
					foreach (Path p in paths){
						if (p.tiles.Contains(map[x,y])){
							manageNetworks(r1, p.startRoom);
							collision = true;
							// Console.WriteLine("path interupted by path x");
							break;
						}
					}
				}
				if (!collision){
					if (x != r2.centreX && xTurn){
						map[x,y].isPath = true;
						x += (x < r2.centreX)? 1:-1;
						temp.tiles.Add(map[x,y]);						
					}
					if (y != r2.centreY && !xTurn){
						map[x,y].isPath = true;
						y += (y < r2.centreY)? 1:-1;
						temp.tiles.Add(map[x,y]);
					}
					xTurn = !xTurn;
				}
			}
			if (!collision){
				manageNetworks(r1, r2);
				// Console.WriteLine("path made it to destination");
			}
			else if (roomCollision){					// if collided with a room continue the path to the centre of the new room
				while (x != orRoom.centreX || y != orRoom.centreY){
					if (x != orRoom.centreX){
						map[x,y].isPath = true;
						x += (x < orRoom.centreX)? 1:-1;
						temp.tiles.Add(map[x,y]);
					}
					if (y != orRoom.centreY){
						map[x,y].isPath = true;
						y += (y < orRoom.centreY)? 1:-1;
						temp.tiles.Add(map[x,y]);
					}
				}
			}
		}

		private void generatePathRandom(Room r1, Room r2, bool termPath, bool termRoom){
			Path temp = new Path();
			int x = r1.centreX;
			int y = r1.centreY;
			bool collision = false;
			bool roomCollision = false;
			Room orRoom = new Room();
			bool xTurn = true;
			paths.Add(temp);
			temp.startRoom = r1;

			double xDif = (x < r2.centreX)? (r2.centreX - x):(x - r2.centreX);
			double yDif = (y < r2.centreY)? (r2.centreY - y):(y - r2.centreY);

			while ((x != r2.centreX || y != r2.centreY) && !collision){
				// drawPaths();
				if (termRoom && map[x,y].isRoom && !r1.tiles.Contains(map[x,y]) && !r2.tiles.Contains(map[x,y])){
					foreach (Room r in rooms){
						if (r.tiles.Contains(map[x,y])){
							manageNetworks(r1, r);
							collision = true;
							orRoom = r;
							roomCollision = true;
							// Console.WriteLine("path interupted by room x");
							break;
						}
					}
				}
				else if (termPath && map[x,y].isPath && !map[x,y].isRoom){
					foreach (Path p in paths){
						if (p.tiles.Contains(map[x,y])){
							manageNetworks(r1, p.startRoom);
							collision = true;
							// Console.WriteLine("path interupted by path x");
							break;
						}
					}
				}
				if (!collision){
					if (x != r2.centreX && random.NextDouble() <= 1/yDif && xTurn){
						map[x,y].isPath = true;
						x += (x < r2.centreX)? 1:-1;
						temp.tiles.Add(map[x,y]);
					}
					if (y != r2.centreY && random.NextDouble() <= 1/xDif && !xTurn){
						map[x,y].isPath = true;
						y += (y < r2.centreY)? 1:-1;
						temp.tiles.Add(map[x,y]);
					}
					xTurn = !xTurn;
				}
			}
			if (!collision){
				manageNetworks(r1, r2);
				// Console.WriteLine("path made it to destination");
			}
			else if (roomCollision){								// if collided with a room continue the path to the centre of the new room
				// Console.WriteLine("Collision");
				while (x != orRoom.centreX || y != orRoom.centreY){
					// Console.WriteLine(orRoom.centreX+" "+orRoom.centreY+":"+x+" "+y);
					if (x != orRoom.centreX && random.NextDouble() <= 1/yDif && xTurn){
						map[x,y].isPath = true;
						x += (x < orRoom.centreX)? 1:-1;
						temp.tiles.Add(map[x,y]);
					}
					if (y != orRoom.centreY && random.NextDouble() <= 1/xDif && !xTurn){
						map[x,y].isPath = true;
						y += (y < orRoom.centreY)? 1:-1;
						temp.tiles.Add(map[x,y]);
					}
					xTurn = !xTurn;
				}
				// Console.WriteLine("path finished");				
			}
		}

		public void smooth(double fill, int itterations){
			cleanMap(false, true);
			drawRooms();
			foreach (Room r in rooms){
				bool done = false;
				int[,] temp = new int[r.width, r.height];
				int[,] tempPrev = new int[r.width, r.height];
				bool[,] tempPath = new bool[r.width, r.height];
				for (int state = 0; !done; state++){
					bool update = false;
					for (int x = 0; x < r.width; x++){
						for (int y = 0; y < r.height; y++){
							if (state == 0){											// transfering room data to int array
								temp[x, y] = r.tiles[y*r.width+x].tileID;
								if (temp[x, y] == 1 && random.NextDouble() < fill){
									temp[x, y] = 2;
									update = true;
								}
							}
							else if (state > 0 && state <= itterations &&				// neighbours and itterating
									x != 0 && x != r.width-1 && y != 0 && y != r.height-1){
								int neighbours = 0;
								for (int i = -1; i <= 1; i++){
									for (int j = -1; j <= 1; j++){
										if (tempPrev[x+i, y+j] == 2 && !tempPath[x, y] && (i != 0 || j != 0)){
											neighbours++;
										}
									}
								}
								// Console.WriteLine(neighbours);
								if (neighbours < 4){
									temp[x, y] = 1;
									update = true;
								}
								else if (neighbours > 4){
									temp[x, y] = 2;
									update = true;
								}
							}
							else if (state == itterations + 1 && temp[x, y] == 2 && !tempPath[x,y]){							// deleting all walls
								temp[x, y] = 0;
							}
							else if (state == itterations + 2 &&
								x != 0 && x != r.width-1 && y != 0 && y != r.height-1){
								done = true;
								for (int i = -1; i <= 1; i++){
									for (int j = -1; j <=1; j++){
										if (temp[x, y] == 1 && temp[x+i, y+j] == 0 && !tempPath[x+i,y+j]){
											temp[x+i, y+j] = 2;
										}
									}
								}
							}
						}
					}
					if (update){
						for (int x = 0; x < r.width; x++){
							for (int y = 0; y < r.height; y++){
								tempPrev[x, y] = temp[x, y];
							}
						}
					}
				}
				for (int x = 0; x < r.width; x++){
					for (int y = 0; y < r.height; y++){
						r.tiles[y*r.width+x].tileData(TileDB.Tiles[temp[x, y]]);
						if ((x == 0 || x == r.width-1 || y == 0 || y == r.height-1) &&
								r.tiles[y*r.width+x].tileID == 1 && !r.tiles[y*r.width+x].isPath){
							r.tiles[y*r.width+x].tileData(TileDB.Tiles[2]);
						}
						if (r.tiles[y*r.width+x].tileID == 0 || r.tiles[y*r.width+x].tileID == 2){
							r.tiles[y*r.width+x].isRoom = false;
						}
					}
				}
			}
		}

		// check if a room collides with any other rooms
		private bool roomCollides(Room room){
			foreach (Room room2 in rooms){											// check every room
				if ((room2.collidesWith(room))){									// see if the current room collides with any of them. (collidesWith is a method of the room class)
					// Console.WriteLine("collision: " + room + " and " + room2);
					return true;													// if it collides with any of them then return true
				}
			}
			// Console.WriteLine("no collision");
			return false;															// else return false
		}

		private bool[] roomOverlaps(Room room, int gap){
			bool[] overlapStatus = {false, false, false, false};
			foreach (Room r in rooms){
				if (r.number != room.number){
					overlapStatus = room.overlapsWith(r, overlapStatus, gap);
					// Console.WriteLine(room.number+" "+r.number);
					// Console.WriteLine(overlapStatus[0]+" "+overlapStatus[1]+" "+overlapStatus[2]+" "+overlapStatus[3]);					
				}
			}
			return overlapStatus;
		}

		// manages the connection of two netowrks
		private void manageNetworks(Room r1, Room r2){
			int lower, higher;

			if (!r1.inNetwork && !r2.inNetwork){										// if neither of the rooms are in networks
				// Console.WriteLine("new network needed");
				List<Room> temp = new List<Room>();										// make a new network (list of rooms)
				temp.Add(r1);															// add room 1
				r1.inNetwork = true;													// room one is now in a network so set that flag
				temp.Add(r2);															// same for room 2
				r2.inNetwork = true;
				networks.Add(temp);														// then add that new network to networks
			}
			else if (r1.inNetwork && !r2.inNetwork){									// the next 2 are for if only one room is in a network
				for (int i = 0; i < networks.Count; i++){								// cycle through networks
					if (networks[i].Contains(r1)){										// if room 1 is in the network
						networks[i].Add(r2);											// add room 2 to the network
						r2.inNetwork = true;											// and set flag to say room 2 is now in a network
						// Console.WriteLine("room " + r2.number + " added to network " + i);
					}
				}
			}
			else if (!r1.inNetwork && r2.inNetwork){									// same as last but switch room rolls
				for (int i = 0; i < networks.Count; i++){
					if (networks[i].Contains(r2)){
						networks[i].Add(r1);
						r1.inNetwork = true;
						// Console.WriteLine("room " + r1.number + " added to network " + i);
					}
				}
			}
			else {																		// both rooms are in networks
				for (int i = 0; i < networks.Count; i++){								// cycle through networks
					if (networks[i].Contains(r1) && networks[i].Contains(r2)){			// if both rooms are in the same network
						// Console.WriteLine("both rooms in same network");
						return;															// quit this method
					}
					for (int j = 0; j < networks.Count; j++){							// cycle through networks again
						if (networks[i].Contains(r1) && networks[j].Contains(r2)){		// if room 1 is in network i and room 2 is in network j
							if (i < j){													// assign i and j to the correct higher lower values
								lower = i;
								higher = j;
							}
							else {
								higher = i;
								lower = j;
							}
							// Console.WriteLine("network " + higher + " added to network " + lower);
							for (int k = 0; k < networks[higher].Count; k++){			// for all elements in the higher list
								networks[lower].Add(networks[higher][k]);				// add then to the lower list
							}
							networks.RemoveAt(higher);									// then delete the higher list from networks
							return;														// and exit method
						}
					}
				}
			}
		}

		public void cleanMap(bool cleanPaths, bool cleanRooms){
			for (int y = 0; y < height; y++){
				for (int x = 0; x < width; x++){
					map[x,y].tileData(TileDB.Tiles[0]);
					if (cleanPaths){
						map[x,y].isPath = false;
					}
					if (cleanRooms){
						map[x,y].isRoom = false;
					}
				}
			}
		}
		// interprets the data from each room and draws them on the map
		public void drawRooms(){										// i think youve read enough to understand this
			foreach(Room r in rooms){
				drawRoom(r);
			}
		}

		private void drawRoom(Room room){
			string roomNoChar = "" + room.number;
			for (int y = room.top; y < room.bottom+1; y++){
				for (int x = room.left; x < room.right+1; x++){
					if (x == room.left || x == room.right || y == room.top || y == room.bottom){
						map[x,y].tileData(TileDB.Tiles[2]);			// appart from this. so there is a method in tile that takes the data from another tile and copys it to its own
					}													// the tileDB class is whats called a static class which means nothing can by changed in it. its kind of read only
					else {												// this also means you cant make an instance of it (= new tileDB) because it cant store any data and everything 
						map[x,y].tileData(TileDB.Tiles[1]);				// you need from it is already in the file. so i simply call TileDB to call the class, then Tiles[index] to access
						map[x,y].isRoom = true;							// the dictionary inside it (see the class for better discription) and get that tile from the dictionary
						// map[x,y].tileChar = roomNoChar[0];
						if (map[x,y].isPath){
							map[x,y].tileChar = '~';
						}
					}
					room.tiles.Add(map[x,y]);
				}
			}
		}

		// interptrets the data from all the path objects and draws them on the map
		public void drawPaths(){
			foreach(Path p in paths){
				foreach(Tile t in p.tiles){
					map[t.coordX,t.coordY].tileData(TileDB.Tiles[1]);
					// t.isPath = true;
					// Console.WriteLine(t.coordX+" "+ t.coordY);
					if (map[t.coordX-1,t.coordY-1].tileID == 0){					// checks all neighbours to see if theyre empty/unassigned tiles
						map[t.coordX-1,t.coordY-1].tileData(TileDB.Tiles[2]);			// makes them a wall tile
					}
					if (map[t.coordX,t.coordY-1].tileID == 0){
						map[t.coordX,t.coordY-1].tileData(TileDB.Tiles[2]);
					}
					if (map[t.coordX+1,t.coordY-1].tileID == 0){
						map[t.coordX+1,t.coordY-1].tileData(TileDB.Tiles[2]);
					}
					if (map[t.coordX+1,t.coordY].tileID == 0){
						map[t.coordX+1,t.coordY].tileData(TileDB.Tiles[2]);
					}
					if (map[t.coordX+1,t.coordY+1].tileID == 0){
						map[t.coordX+1,t.coordY+1].tileData(TileDB.Tiles[2]);
					}
					if (map[t.coordX,t.coordY+1].tileID == 0){
						map[t.coordX,t.coordY+1].tileData(TileDB.Tiles[2]);
					}
					if (map[t.coordX-1,t.coordY+1].tileID == 0){
						map[t.coordX-1,t.coordY+1].tileData(TileDB.Tiles[2]);
					}
					if (map[t.coordX-1,t.coordY].tileID == 0){
						map[t.coordX-1,t.coordY].tileData(TileDB.Tiles[2]);
					}
				}
			}
		}

		// this was used for debugging
		private void debugNetworks(){
			for (int i = 0; i < networks.Count; i++){
				Console.WriteLine("network no: " + i);												// it prints out the number of the network in the list
				for (int j = 0; j < networks[i].Count; j++){
					Console.WriteLine(networks[i][j].number + " " + networks[i][j].inNetwork);		// then all the rooms in that network
				}
			}
		}				// i suggest putting this command at different places and playing round with it. its really helpfull

		public string print(){					// same as print in the main class
			string output = "";
			for (int y = 0; y < height; y++){
				for (int x = 0; x < width; x++){
					output += map[x, y].tileChar + " ";
				}
				output += "\n";
			}
			return output;
		}

		public string printWTiles(){					// same as print in the main class
			string output = "";
			for (int y = 0; y < height; y++){
				for (int x = 0; x < width; x++){
					output += rooms[0].getTile(x, y).tileChar + " ";
				}
				output += "\n";
			}
			return output;
		}

		// this is the class for Room objects
		protected class Room {

			public Room() {

			}
			
			public Room(Room other){
				roomData(other);
			}

			// each room has a bunch of dimensions
			public int top;
			public int left;							// if i try to get this itll just return the value for this spesific room
			public int right {				
				get { return left + width - 1; }		// but for this it isnt actually set to anything so its going to just do this calculation and return the result
			}
			public int bottom {
				get { return top + height - 1; }
			}
			public int width;
			public int height;
			public int centreX {
				get { return left + (width / 2); }
			}
			public int centreY {
				get {return top + (height /2); }
			}

			// as well as a number, as list of tiles and an inNetwork flag
			public int number;
			public List<Tile> tiles = new List<Tile>();
			public bool inNetwork = false;

			public void roomData(Room other){
				this.top = other.top;
				this.left = other.left;
				this.width = other.width;
				this.height = other.height;
				this.number = other.number;
				this.tiles = other.tiles;
				this.inNetwork = other.inNetwork;
			}

			// this method can be accessed through a room object ie room1.collidesWith(room2) and returns a bool
			public bool collidesWith(Room other){
				if (this.top > other.bottom){			// the this. is the value from in the example
					return false;
				}
				if (this.bottom < other.top){			// and the other. is the value from room2 in the example
					return false;
				}
				if (this.left > other.right){
					return false;
				}
				if (this.right < other.left){
					return false;	
				}
				return true;
			}

			public bool[] overlapsWith(Room other, bool[] overlapStatus, int gap){
				if (this.top < other.bottom+gap && this.top > other.top && this.left < other.right-gap && this.right > other.left+gap){
					overlapStatus[0] = true;
				}
				if (this.right > other.left-gap && this.right < other.right && this.top < other.bottom-gap && this.bottom > other.top+gap){
					overlapStatus[1] = true;	
				}
				if (this.bottom > other.top-gap && this.bottom < other.bottom && this.left < other.right-gap && this.right > other.left+gap){
					overlapStatus[2] = true;
				}
				if (this.left < other.right+gap && this.left > other.left && this.top < other.bottom-gap && this.bottom > other.top+gap){
					overlapStatus[3] = true;
				}
				return overlapStatus;
			}


			public Tile getTile(int x, int y){
				return tiles[width*y+x];
			}

			public void setTile(int x, int y, Tile data){
				tiles[width*y+x].tileData(data);
			}

			public override string ToString(){			// this method is the default method that would be called if i were to do something like
				return top+" "+bottom+" "+left+" "+right;	// Room r = new Room(); room.top = 2 ect
			}												// Console.WriteLine(r);	and just put a room object into a print statement. ToString() works the same in jaava
		}

		// this is the path class and it may as well be a struct as it has no methods and only a couple of 'member' variables
		protected class Path {											// member variables means variables belonging to that class
			public List<Tile> tiles = new List<Tile>();
			public Room startRoom;
		}
	}	
}