using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dungeon
    {
        public Dungeon left;
        public Dungeon right;
        public Rect room;
        public List<Rect> corridors = new List<Rect>();
        private DungeonGenerator generator;

        public Dungeon(Rect room)
        {
            this.room = room;
            generator = GameObject.Find("Map").GetComponent<DungeonGenerator>();
        }

        public bool IsLeaf()
        {
            return left == null && right == null; 
        }

        public bool Split(int avgRoomSize)
        {
            if (!IsLeaf())
                return false;
            
            bool splitH;
            if (room.width / room.height >= 1.25)
                splitH = false;
            else if (room.height / room.width >= 1.25)
                splitH = true;
            else
                splitH = Random.Range(0, 1) > 0.5;

            if (Math.Min(room.height, room.width) / 2 < avgRoomSize)
                return false;

            if (splitH)
            {
                int split = Random.Range(avgRoomSize, (int) (room.width - avgRoomSize));
                left = new Dungeon(new Rect(room.x, room.y, room.width, split));
                right = new Dungeon(new Rect(room.x, room.y + split, room.width, room.height - split));
            }
            else
            {
                int split = Random.Range(avgRoomSize, (int) (room.height - avgRoomSize));
                left = new Dungeon(new Rect (room.x, room.y, split, room.height));
                right = new Dungeon(new Rect (room.x + split, room.y, room.width - split, room.height));
            }

            return true;
        }

        public void CreateRoom()
        {
            if (left != null)
                left.CreateRoom();
            if (right != null)
                right.CreateRoom();
            if (left != null && right != null)
                CreateCorridor();
            if (IsLeaf())
            {
                int width = (int) Random.Range(room.width / 2, room.width - 2);
                int height = (int) Random.Range(room.height / 2, room.height - 2);
                int roomX = (int) Random.Range(1, room.width - width - 1);
                int roomY = (int) Random.Range(1, room.height - height - 1);
                room = new Rect(room.x + roomX, room.y + roomY, width, height);
                generator.AddDungeon(this);
            }
        }

        public Rect GetRoom()
        {
            if (IsLeaf())
                return room;
            if (left != null)
            {
                Rect lroom = left.GetRoom();
                if (lroom.x >= 0)
                    return lroom;
            }

            if (right != null)
            {
                Rect rroom = right.GetRoom();
                if (rroom.x >= 0)
                    return rroom;
            }

            return new Rect(-1, -1, 0, 0);
        }
        
        void CreateCorridor()
        {
            Rect lroom = left.GetRoom();
            Rect rroom = right.GetRoom();
            Vector2 lpoint = new Vector2((int) Random.Range(lroom.x + 1, lroom.xMax - 1), (int) Random.Range(lroom.y + 1, lroom.yMax - 1));
            Vector2 rpoint = new Vector2 ((int)Random.Range (rroom.x + 1, rroom.xMax - 1), (int)Random.Range (rroom.y + 1, rroom.yMax - 1));
            
            if (lpoint.x > rpoint.x) 
            {
                Vector2 temp = lpoint;
                lpoint = rpoint;
                rpoint = temp;
            }
            
            int w = (int)(lpoint.x - rpoint.x);
            int h = (int)(lpoint.y - rpoint.y);
            
            if (w != 0) 
            {
                if (Random.Range (0, 1) > 2) 
                {
                    corridors.Add (new Rect (lpoint.x, lpoint.y, Mathf.Abs (w) + 2, 2));

                    if (h < 0) 
                        corridors.Add (new Rect (rpoint.x, lpoint.y, 2, Mathf.Abs (h)));
                    else 
                        corridors.Add (new Rect (rpoint.x, lpoint.y, 2, -Mathf.Abs (h)));
                } 
                else 
                {
                    if (h < 0) 
                        corridors.Add (new Rect (lpoint.x, lpoint.y, 2, Mathf.Abs (h)));
                    else 
                        corridors.Add (new Rect (lpoint.x, rpoint.y, 2, Mathf.Abs (h)));
                    corridors.Add (new Rect (lpoint.x, rpoint.y, Mathf.Abs (w) + 2, 2));
                }
            } 
            else 
            {
                if (h < 0) 
                    corridors.Add (new Rect ((int)lpoint.x, (int)lpoint.y, 2, Mathf.Abs (h)));
                else 
                    corridors.Add (new Rect ((int)rpoint.x, (int)rpoint.y, 2, Mathf.Abs (h)));
            }
        }

    }
