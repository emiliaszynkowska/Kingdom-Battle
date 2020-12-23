using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dungeon
    {
        public Dungeon left;
        public Dungeon right;
        public Rect rect;
        public Rect room = new Rect(-1, -1, 0, 0);
        public List<Rect> corridors = new List<Rect>();
        private DungeonGenerator generator;

        public Dungeon(Rect rect)
        {
            this.rect = rect;
            generator = GameObject.Find("Map").GetComponent<DungeonGenerator>();
        }

        public bool IsLeaf()
        {
            return left == null && right == null; 
        }

        public bool Split(int minRoomSize, int maxRoomSize)
        {
            if (!IsLeaf())
                return false;
            
            bool splitH;
            if (rect.width / rect.height >= 1.25)
                splitH = false;
            else if (rect.height / rect.width >= 1.25)
                splitH = true;
            else
                splitH = Random.Range(0, 1) > 0.5;

            if (Math.Min(rect.height, rect.width) / 2 < minRoomSize)
                return false;

            if (splitH)
            {
                int split = Random.Range(minRoomSize, (int) (rect.width - minRoomSize));
                left = new Dungeon(new Rect(rect.x, rect.y, rect.width, split));
                right = new Dungeon(new Rect(rect.x, rect.y + split, rect.width, rect.height - split));
            }
            else
            {
                int split = Random.Range(minRoomSize, (int) (rect.height - minRoomSize));
                left = new Dungeon(new Rect (rect.x, rect.y, split, rect.height));
                right = new Dungeon(new Rect (rect.x + split, rect.y, rect.width - split, rect.height));
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
                int width = (int) Random.Range(rect.width / 2, rect.width - 2);
                int height = (int) Random.Range(rect.height / 2, rect.height - 2);
                int roomX = (int) Random.Range(1, rect.width - width - 1);
                int roomY = (int) Random.Range(1, rect.height - height - 1);
                room = new Rect(rect.x + roomX, rect.y + roomY, width, height);
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
