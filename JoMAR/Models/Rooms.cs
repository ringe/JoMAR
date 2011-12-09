using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JoMAR.Models
{
    /// <summary>
    /// Lookup tools for Rooms
    /// </summary>
    public class Rooms
    {
        /// <summary>
        /// Find a private room for two users.
        /// </summary>
        /// <param name="user1">First user's Profile</param>
        /// <param name="user2">Second user's Profile</param>
        /// <param name="db">JodADataContext</param>
        /// <returns></returns>
        public static ChatRoom Private(Profile user1, Profile user2, JodADataContext db)
        {
            // Possible names of private rooms between these users.
            String A = user1.CellPhone + user2.CellPhone;
            String B = user2.CellPhone + user1.CellPhone;

            // Select rooms where name is A or B and the room is private, not public and with only two users
            List<ChatRoom> rooms = (from r in db.ChatRooms
                                     where (r.Name == A || r.Name == B && r.isPrivate && !r.isPublic && r.UserRooms.Count == 2)
                                     select r).ToList();

            ChatRoom newRoom;
            // Return private room, create if necessary
            if (rooms.Count == 0)
            {
                // Prepare new private room
                newRoom = new ChatRoom();
                newRoom.UserID = user1.UserId;
                newRoom.RoomID = Guid.NewGuid();
                newRoom.isPrivate = true;
                newRoom.Name = A;

                // Insert new room
                db.ChatRooms.InsertOnSubmit(newRoom);

                // Join user 1 to room
                UserRoom nr = new UserRoom();
                nr.UserID = user1.UserId;
                nr.RoomID = newRoom.RoomID;

                // Join user 2 to room
                UserRoom nr2 = new UserRoom();
                nr2.UserID = user2.UserId;
                nr2.RoomID = newRoom.RoomID;

                // Insert user_room relationsships
                db.UserRooms.InsertOnSubmit(nr);
                db.UserRooms.InsertOnSubmit(nr2);

                // Submit changes to DB
                db.SubmitChanges();

                return newRoom;
            }
            else return rooms.First();
        }

        /// <summary>
        /// Find a public room.
        /// </summary>
        /// <param name="name">name to look for</param>
        /// <param name="db">JodADataContext</param>
        /// <returns>ChatRoom</returns>
        public static ChatRoom Public(string name, JodADataContext db)
        {
            List<ChatRoom> rooms;
            rooms = (from p in db.ChatRooms where p.Name == name select p).ToList();

            // Found any rooms by that name?
            if (rooms.Count == 0)
                return null;
            else  // Return the first room found
                return rooms.First();
        }

        /// <summary>
        /// Post message to room.
        /// </summary>
        /// <param name="msg">The message.</param>
        /// <param name="user">The user id</param>
        /// <param name="room">The room id</param>
        /// <param name="db">JodADataContext</param>
        public static void Post(string msg, Guid user, Guid room, JodADataContext db)
        {
            ChatMessage message = new ChatMessage();
            message.Date = DateTime.Now;
            message.MessageID = Guid.NewGuid();
            message.UserID = user;
            message.RoomID = room;
            message.Text = msg;
            
            // Submit message to DB
            db.ChatMessages.InsertOnSubmit(message);
            db.SubmitChanges();
        }
    }

}