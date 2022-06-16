using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    public enum RoomState
    {
        Available,
        Occupied,
        Vacant,
        Repair
    }

    class Program
    {
        static void Main(string[] args)
        {
            var hotel = new Hotel(4);
            Console.WriteLine($"Welcome to Grand Hyatt Hotel. Current Occupancy:{hotel.TotalOccupancyRate}%");
            hotel.ListAvailableRooms();
            hotel.AssignRoom();
            Console.WriteLine($"Current Occupancy:{hotel.TotalOccupancyRate}%");
            hotel.ListAvailableRooms();

        }
    }

    public class Hotel
    {
        Floor[] floors;
        public float TotalOccupancyRate {
            get {
                float temp = 0f;
                foreach (var floor in floors)
                {
                    temp += floor.OccupancyRate;
                }
                return temp;
            }
        }
        public Hotel(int floors, int roomsPerFloor = 5)
        {
            this.floors = new Floor[floors];
            for (int i = 0; i < floors; i++)
            {
                this.floors[i] = new Floor(i + 1, roomsPerFloor, (i % 2) != 0);
            }
        }
        public string AssignRoom()
        {
            string temp = "";
            foreach (var floor in floors)
            {
                var firstAvailableRoom = floor.FindFirstAvailableRoom();
                if (firstAvailableRoom != null)
                {
                    temp = firstAvailableRoom.Name;
                    firstAvailableRoom.TrySetRoomState(RoomState.Occupied);
                    Console.WriteLine($"Assigned Occupancy: {firstAvailableRoom.Name}");
                    break;
                }
            }
            return temp;
        }
        public void ListAvailableRooms()
        {
            foreach (var floor in floors)
            {
                floor.PrintAvailableRoomsOnFloor();
            }
        }

        private void UpdateRoom(string roomName, RoomState newState)
        {
            foreach (var floor in floors)
            {
                var room = floor.FindRoomByName(roomName);
                if (room != null)
                {
                    room.TrySetRoomState(newState);
                }
            }
        }
        public void CheckOut(string roomName)
        {
            UpdateRoom(roomName, RoomState.Vacant);
        }
        public void CleanRoom(string roomName)
        {
            UpdateRoom(roomName, RoomState.Available);
        }
        public void RoomTagAsRepair(string roomName)
        {
            UpdateRoom(roomName, RoomState.Repair);
        }
    }

    public class Floor
    {
        int floor;
        Room[] rooms;
        Room[] AvailableRooms => Array.FindAll(rooms, r => r.RoomState == RoomState.Available);
        public int NumberOfTotalRooms => rooms.Length;
        public int NumberOfAvailableRooms => AvailableRooms.Length;
        public float OccupancyRate => (float)(NumberOfTotalRooms - NumberOfAvailableRooms) * 100 / NumberOfTotalRooms;

        public Floor(int floor, int numberOfRooms, bool reverseOrder = false)
        {
            this.floor = floor;
            rooms = new Room[numberOfRooms];
            char temp = (reverseOrder) ? 'E' : 'A';
            for (int i = 0; i < numberOfRooms; i++)
            {
                rooms[i] = new Room($"{floor}{temp}");
                //Console.WriteLine($"room created: {floor}{temp}");
                temp = (reverseOrder) ? char.ConvertFromUtf32(temp - 1)[0] : char.ConvertFromUtf32(temp + 1)[0];
            }
        }
        public Room FindRoomByName(string name)
        {
            return Array.Find(rooms, r => r.Name.Equals(name));
        }
        public Room FindFirstAvailableRoom()
        {
            Room availableRoom = Array.Find(rooms, r => r.RoomState == RoomState.Available);
            return availableRoom;
        }
        public void PrintAvailableRoomsOnFloor()
        {
            Console.WriteLine($"Hotel floor {floor} available rooms:");
            foreach (var room in AvailableRooms)
            {
                Console.Write($"|{room.Name}|");
            }
            Console.Write("\n");
        }
    }
    public class Room
    {
        public string Name { get; private set; }
        public RoomState RoomState { get; private set; }
        public Room PrevRoom { get; private set; }
        public Room NextRoom { get; private set; }
        public Room(string name, Room prevRoom = null, Room nextRoom = null)
        {
            Name = name;
            if (prevRoom != null)
            {
                PrevRoom = prevRoom;
            }
            if (nextRoom != null)
            {
                NextRoom = nextRoom;
            }
            RoomState = RoomState.Available;
        }

        public bool TrySetRoomState(RoomState newRoomState)
        {
            bool success = false;
            switch (RoomState)
            {
                case RoomState.Available:
                    //only accept occupied.
                    success = newRoomState == RoomState.Occupied;
                    break;
                case RoomState.Occupied:
                case RoomState.Repair:
                    // only accept vacant
                    success = newRoomState == RoomState.Vacant;
                    break;
                case RoomState.Vacant:
                    // only accept repair/available.
                    success = newRoomState == RoomState.Repair || newRoomState == RoomState.Available;
                    break;
                default:
                    Console.WriteLine("Unknown Room state! Please try again.");
                    break;
            }
            RoomState = newRoomState;
            return success;
        }
    }
}
