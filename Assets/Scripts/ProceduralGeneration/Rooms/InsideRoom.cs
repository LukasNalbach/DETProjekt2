using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public abstract class InsideRoom : RealGenRoom {

    public static void FillRectWithInsideWall(Rectangle rect) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();
        
        for (int x = rect.X; x < rect.X + rect.Width; x++) {
            for (int y = rect.Y; y < rect.Y + rect.Height; y++) {
                wGen.CreateWall(new Vector2(x, y), "Inside");
                GameObject obj = wGen.CreateGrassOnTileWithProb(new Vector2(x, y + 1), 0.8);
                if (obj != null) {
                    obj.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 2";
                }
            }
        }
    }

    public static void generateRuinEdge(List<Rectangle> corridors, Rectangle rectInside,  Rectangle rectOutside) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();
        if (rectInside.X + rectInside.Width == rectOutside.X) { // outside is right of inside
            int x = rectInside.X + rectInside.Width - 1;
            for (int y = rectInside.Y - 10; y < rectInside.Y + rectInside.Height + 10; y++) {
                Vector2 pos = new Vector2(x, y);

                if (!VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 0)) { // is not in corridor
                    bool touchesCorr = false;
                    Rectangle touchedCorridor;
                    
                    foreach (Rectangle corridor in corridors) {
                        if (VirtualGenRoom.IsCloserToThan(pos, corridor, "XY", 1)) {
                            touchesCorr = true;
                            touchedCorridor = corridor;
                            break;
                        }
                    }

                    if (touchesCorr) { // touches a corridor
                        if (y < touchedCorridor.Y) { // right of inside and below corridor
                            wGen.CreateWall(pos, "BtoL");
                        } else { // right of inside and above corridor
                            wGen.CreateWall(pos, "LtoT");
                        }
                    } else { // right of inside
                        wGen.CreateWall(pos, "BtoT");
                    }
                }
            }
            FillRectWithInsideWall(new Rectangle(rectInside.X - 10, rectInside.Y - 10, 10, rectInside.Height + 20));
            FillRectWithInsideWall(new Rectangle(rectInside.X, rectInside.Y - 10, rectInside.Width - 1, 10));
            FillRectWithInsideWall(new Rectangle(rectInside.X, rectInside.Y + rectInside.Height, rectInside.Width - 1, 10));
        } else if (rectInside.X == rectOutside.X + rectOutside.Width) { // outside is left of inside
            int x = rectInside.X;
            for (int y = rectInside.Y - 10; y < rectInside.Y + rectInside.Height + 10; y++) {
                Vector2 pos = new Vector2(x, y);

                if (!VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 0)) { // is not in corridor
                    bool touchesCorr = false;
                    Rectangle touchedCorridor;
                    
                    foreach (Rectangle corridor in corridors) {
                        if (VirtualGenRoom.IsCloserToThan(pos, corridor, "XY", 1)) {
                            touchesCorr = true;
                            touchedCorridor = corridor;
                            break;
                        }
                    }

                    if (touchesCorr) { // touches a corridor
                        if (y < touchedCorridor.Y) { // left of inside and below corridor
                            wGen.CreateWall(pos, "RtoB");
                        } else { // left of inside and above corridor
                            wGen.CreateWall(pos, "TtoR");
                        }
                    } else { // left of inside
                        wGen.CreateWall(pos, "TtoB");
                    }
                }
            }
            FillRectWithInsideWall(new Rectangle(rectInside.X + rectInside.Width, rectInside.Y - 10, 10, rectInside.Height + 20));
            FillRectWithInsideWall(new Rectangle(rectInside.X + 1, rectInside.Y - 10, rectInside.Width - 1, 10));
            FillRectWithInsideWall(new Rectangle(rectInside.X + 1, rectInside.Y + rectInside.Height, rectInside.Width - 1, 10));
        } else if (rectInside.Y + rectInside.Height == rectOutside.Y) { // outside is above inside
            int y = rectInside.Y + rectInside.Height - 1;
            for (int x = rectInside.X - 10; x < rectInside.X + rectInside.Width + 10; x++) {
                Vector2 pos = new Vector2(x, y);

                if (!VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 0)) { // is not in corridor
                    bool touchesCorr = false;
                    Rectangle touchedCorridor;
                    
                    foreach (Rectangle corridor in corridors) {
                        if (VirtualGenRoom.IsCloserToThan(pos, corridor, "XY", 1)) {
                            touchesCorr = true;
                            touchedCorridor = corridor;
                            break;
                        }
                    }

                    if (touchesCorr) { // touches a corridor
                        if (x < touchedCorridor.X) { // above inside and left of corridor
                            wGen.CreateWall(pos, "BtoL");
                        } else { // above inside and right of corridor
                            wGen.CreateWall(pos, "RtoB");
                        }
                    } else { // above inside
                        wGen.CreateWall(pos, "RtoL");
                    }
                }
            }
            FillRectWithInsideWall(new Rectangle(rectInside.X - 10, rectInside.Y, 10, rectInside.Height - 1));
            FillRectWithInsideWall(new Rectangle(rectInside.X + rectInside.Width, rectInside.Y, 10, rectInside.Height - 1));
            FillRectWithInsideWall(new Rectangle(rectInside.X - 10, rectInside.Y - 10, rectInside.Width + 20, 10));
        } else { // outside is below inside
            int y = rectInside.Y;
            for (int x = rectInside.X - 10; x < rectInside.X + rectInside.Width + 10; x++) {
                Vector2 pos = new Vector2(x, y);

                if (!VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 0)) { // is not in corridor
                    bool touchesCorr = false;
                    Rectangle touchedCorridor;
                    
                    foreach (Rectangle corridor in corridors) {
                        if (VirtualGenRoom.IsCloserToThan(pos, corridor, "XY", 1)) {
                            touchesCorr = true;
                            touchedCorridor = corridor;
                            break;
                        }
                    }

                    if (touchesCorr) { // touches a corridor
                        if (x < touchedCorridor.X) { // below inside and left of corridor
                            wGen.CreateWall(pos, "LtoT");
                        } else { // below inside and right of corridor
                            wGen.CreateWall(pos, "TtoR");
                        }
                    } else { // below inside
                        wGen.CreateWall(pos, "LtoR");
                    }
                }
            }
            FillRectWithInsideWall(new Rectangle(rectInside.X - 10, rectInside.Y + 1, 10, rectInside.Height - 1));
            FillRectWithInsideWall(new Rectangle(rectInside.X + rectInside.Width, rectInside.Y + 1, 10, rectInside.Height - 1));
            FillRectWithInsideWall(new Rectangle(rectInside.X - 10, rectInside.Y + rectInside.Height, rectInside.Width + 20, 10));
        }
    }
    public override void generateOutside(List<Rectangle> corridors, Rectangle rectInside, Rectangle rectOutside) {
        WorldGenerator wGen = Game.Instance.GetComponent<WorldGenerator>();
        for (int x = outerRect.X; x < outerRect.X + outerRect.Width; x++) {
            for (int y = outerRect.Y; y < outerRect.Y + outerRect.Height; y++) {
                Vector2 pos = new Vector2(x, y);
                if (WorldGenerator.IsPosInRectangle(pos, innerRect)) { // in room
                    //wGen.CreateStoneGround(pos);
                } else { // outside room
                    if (VirtualGenRoom.IsCloserToThan(pos, corridors, "XY", 0)) { // in corridor
                        wGen.CreateGrassGround(pos, 0.02, 0.01, 0.97);
                    } else if (!VirtualGenRoom.IsCloserToThan(pos, rectOutside, "XY", 1)) { // in wall and not at edge to outside
                        bool touchesRoom = VirtualGenRoom.IsCloserToThan(pos, innerRect, "XY", 1);
                        bool touchesCorr = false;
                        Rectangle touchedCorridor;
                        
                        foreach (Rectangle corridor in corridors) {
                            if (VirtualGenRoom.IsCloserToThan(pos, corridor, "XY", 1)) {
                                touchesCorr = true;
                                touchedCorridor = corridor;
                                break;
                            }
                        }

                        if (touchesRoom) { // on edge of wall
                            if (touchesCorr) { // on edge of wall and corridor
                                if (y < innerRect.Y) { // below room
                                    if (x < touchedCorridor.X) { // below room and left of corridor
                                        wGen.CreateWall(pos, "BtoL");
                                    } else { // below room and right of corridor
                                        wGen.CreateWall(pos, "RtoB");
                                    }
                                    GameObject obj = wGen.CreateGrassOnTileWithProb(new Vector2(x, y + 1), 0.8);
                                    if (obj != null) {
                                        obj.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 2";
                                    }
                                } else if (y >= innerRect.Y + innerRect.Height) { // above room
                                    if (x < touchedCorridor.X) { // above room and left of corridor
                                        wGen.CreateWall(pos, "LtoT");
                                    } else { // above room and right of corridor
                                        wGen.CreateWall(pos, "TtoR");
                                    }
                                } else { // neither below nor above room
                                    if (x < innerRect.X) { // left of room
                                        if (y < touchedCorridor.Y) { // left of room and below corridor
                                            wGen.CreateWall(pos, "BtoL");
                                            GameObject obj = wGen.CreateGrassOnTileWithProb(new Vector2(x, y + 1), 0.8);
                                            if (obj != null) {
                                                obj.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 2";
                                            }
                                        } else { // left of room and above corridor
                                            wGen.CreateWall(pos, "LtoT");
                                        }
                                    } else { // right of room
                                        if (y < touchedCorridor.Y) { // right of room and below corridor
                                            wGen.CreateWall(pos, "RtoB");
                                            GameObject obj = wGen.CreateGrassOnTileWithProb(new Vector2(x, y + 1), 0.8);
                                            if (obj != null) {
                                                obj.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 2";
                                            }
                                        } else { // right of room and above corridor
                                            wGen.CreateWall(pos, "TtoR");
                                        }
                                    }
                                }
                            } else { // on edge of only wall
                                if (y < innerRect.Y) { // below room
                                    GameObject obj = wGen.CreateGrassOnTileWithProb(new Vector2(x, y + 1), 0.8);
                                    if (obj != null) {
                                        obj.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 2";
                                    }
                                    if (x < innerRect.X) { // below and left of room
                                        wGen.CreateWall(pos, "RtoT");
                                    } else if (x >= innerRect.X + innerRect.Width) { // below and right of room
                                        wGen.CreateWall(pos, "TtoL");
                                    } else { // below room
                                        wGen.CreateWall(pos, "RtoL");
                                    }
                                } else if (y >= innerRect.Y + innerRect.Height) { // above room
                                    if (x < innerRect.X) { // above and left of room
                                        wGen.CreateWall(pos, "BtoR");
                                    } else if (x >= innerRect.X + innerRect.Width) { // above and right of room
                                        wGen.CreateWall(pos, "LtoB");
                                    } else { // above room
                                        wGen.CreateWall(pos, "LtoR");
                                    }
                                } else { // neither below nor above room
                                    if (x < innerRect.X) { // left of room
                                        wGen.CreateWall(pos, "BtoT");
                                    } else { // right of room
                                        wGen.CreateWall(pos, "TtoB");
                                    }
                                    GameObject obj = wGen.CreateGrassOnTileWithProb(new Vector2(x, y + 1), 0.8);
                                    if (obj != null) {
                                        obj.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 2";
                                    }
                                }
                            }
                        } else if (touchesCorr) { // on edge of only corridor
                            if (y < touchedCorridor.Y) { // below corridor
                                wGen.CreateWall(pos, "RtoL");
                                GameObject obj = wGen.CreateGrassOnTileWithProb(new Vector2(x, y + 1), 0.8);
                                if (obj != null) {
                                    obj.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 2";
                                }
                            } else if (y >= touchedCorridor.Y + touchedCorridor.Height) { // above corridor
                                wGen.CreateWall(pos, "LtoR");
                            } else { // neither below nor above corridor
                                if (x < touchedCorridor.X) { // left of corridor
                                    wGen.CreateWall(pos, "BtoT");
                                } else { // right of corridor
                                    wGen.CreateWall(pos, "TtoB");
                                }
                                GameObject obj = wGen.CreateGrassOnTileWithProb(new Vector2(x, y + 1), 0.8);
                                if (obj != null) {
                                    obj.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 2";
                                }
                            }
                        } else { // neither touches room nor corridor
                            wGen.CreateWall(pos, "Inside");
                            GameObject obj = wGen.CreateGrassOnTileWithProb(new Vector2(x, y + 1), 0.8);
                            if (obj != null) {
                                obj.GetComponent<SpriteRenderer>().sortingLayerName = "Layer 2";
                            }
                        }
                    }
                }
            }
        }
    }
}