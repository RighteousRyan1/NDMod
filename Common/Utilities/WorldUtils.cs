using System;
using Terraria;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.ID;
using NDMod.Content.ModPlayers;

namespace NDMod.Common.Utilities
{
    public class WorldUtils
    {
        public static List<Tile> GetTileSquare(int i, int j, int width, int height)
        {
            var tilesInSquare = new List<Tile>();
            for (int n = i - width / 2; n < i + width / 2; n++)
            {
                for (int m = j - height / 2; m < j + height / 2; m++)
                {
                    Tile tile = Framing.GetTileSafely(n, m);
                    tilesInSquare.Add(tile);
                }
            }
            return tilesInSquare;
        }

        public static List<Point> GetTileSquareCoordinates(int i, int j, int width, int height)
        {
            var coordsPerTile = new List<Point>();
            for (int n = i - width / 2; n < i + width / 2; n++)
            {
                for (int m = j - height / 2; m < j + height / 2; m++)
                {
                    coordsPerTile.Add(new Point(n, m));
                }
            }

            return coordsPerTile;
        }
        private static int _attempts;
        internal static bool TryGenerateSinkholeNatural(out bool result)
        {
            var player = Main.player[Main.myPlayer].GetModPlayer<DisasterPlayer>().player;
            var mplayer = player.GetModPlayer<DisasterPlayer>();

            var pCenterTCoords = player.Center.ToTileCoordinates();
            var sizeVariation = Main.rand.Next(-25, 75);

        ReBegin:
            (Vector2, bool) GetGoodCoordinates(out Tile foundTile)
            {
                foundTile = Main.tile[1, 1];
                (Vector2, bool) toReturn = (new Vector2(), false);

                // go to some instruction here to rerun this local method
                for (int i = pCenterTCoords.X - 150; i < pCenterTCoords.X + Main.rand.Next(0, 301); i++)
                {
                    for (int j = pCenterTCoords.Y - 40; j < pCenterTCoords.Y + 40; j++)
                    {
                        var t = Framing.GetTileSafely(i, j);
                        if (t.type == TileID.Containers || t.type == TileID.Containers2)
                            break;

                        Tile tileLeft = Main.tile[i - 1, j];
                        Tile tileRight = Main.tile[i + 1, j];
                        Tile tileUp = Main.tile[i, j - 1];
                        Tile tileDown = Main.tile[i, j + 1];

                        bool isValidTileToAffect = tileDown.active() && !tileUp.active() && tileLeft.active() && tileRight.active() && j <= Main.worldSurface && t.collisionType == 1;
                        foundTile = t;
                        if (isValidTileToAffect)
                        {
                            toReturn = (new Vector2(i, j), true);
                            break;
                        }
                        else
                        {
                            toReturn = (Vector2.Zero, false);
                        }
                    }
                }
                return toReturn;
            }
            var foundCoords = GetGoodCoordinates(out Tile found);

            result = foundCoords.Item2;

            if (!result)
            {
                _attempts++;

                if (_attempts < 20)
                    goto ReBegin;
                else
                    ModContent.GetInstance<NDMod>().Logger.Warn($"Failed to find a proper sinkhole spot after trying {_attempts} times.");
            }
            else
                _attempts = 0;

            int x = (int)foundCoords.Item1.X;
            int y = (int)foundCoords.Item1.Y;

            if (foundCoords.Item1.ToTile() != null && WorldGen.InWorld(x, y) && foundCoords.Item2)
            {
                WorldGen.TileRunner(x, y, 75 + sizeVariation, 50, -1, true, 0, 10000);
                for (int i = x - 40 - Math.Abs(sizeVariation); i < x + 40 + Math.Abs(sizeVariation); i++)
                {
                    for (int j = y - 40 - Math.Abs(sizeVariation); j < y + 40 + Math.Abs(sizeVariation); j++)
                    {
                        if (WorldGen.InWorld(i, j) && WorldGen.InWorld(i - 1, j) && WorldGen.InWorld(i + 1, j) && WorldGen.InWorld(i, j + 1) && WorldGen.InWorld(i, j - 1))
                        {
                            Tile t = Framing.GetTileSafely(i, j);
                            Tile tileLeft = Main.tile[i - 1, j];
                            Tile tileRight = Main.tile[i + 1, j];
                            Tile tileUp = Main.tile[i, j - 1];
                            Tile tileDown = Main.tile[i, j + 1];

                            if (j > y && t.wall > 0)
                            {
                                switch (found.type)
                                {
                                    case TileID.Sand:
                                        t.wall = WallID.Sandstone;
                                        break;
                                    case TileID.Ebonstone | TileID.Ebonsand:
                                        t.wall = WallID.EbonstoneUnsafe;
                                        break;
                                    case TileID.Ebonsand:
                                        t.wall = WallID.EbonstoneUnsafe;
                                        break;
                                    case TileID.JungleGrass | TileID.Mud:
                                        t.wall = WallID.EbonstoneUnsafe;
                                        break;
                                    case TileID.SnowBlock:
                                        t.wall = WallID.SnowWallUnsafe;
                                        break;
                                    default:
                                        t.wall = WallID.Stone;
                                        break;
                                }
                            }

                            bool setInactiveCheck1 = !tileDown.active() && !tileUp.active() && !tileRight.active() && !tileLeft.active();
                            bool setInactiveCheck2 = tileDown.type != TileID.Containers && tileUp.type != TileID.Containers && tileRight.type != TileID.Containers && tileLeft.type != TileID.Containers &&
                                tileDown.type != TileID.Containers2 && tileUp.type != TileID.Containers2 && tileRight.type != TileID.Containers2 && tileLeft.type != TileID.Containers2;
                            if (setInactiveCheck1 && setInactiveCheck2)
                                t.active(false);

                            if (tileDown.active() && !tileUp.active() && tileLeft.active() && tileRight.active())
                            {
                                tileDown.type = found.type;
                                tileUp.type = found.type;
                                tileRight.type = found.type;
                                tileLeft.type = found.type;
                                tileUp.liquid = 255;
                            }
                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                NetMessage.SendTileSquare(255, i, j, 75);
                            WorldGen.SquareTileFrame(i, j);
                        }
                    }
                }
                Main.NewText("The ground has caved in!", Color.LightBlue);
            }
            return foundCoords.Item2;
        }
        public static void GenerateSinkhole(Vector2 coords, int sizeVariation, double strength, int steps)
        {
            var mouseWorldCoords = (coords / 16).ToPoint();
            var variate = Main.rand.Next(-sizeVariation, sizeVariation);

            int x = mouseWorldCoords.X;
            int y = mouseWorldCoords.Y;

            WorldGen.TileRunner(x, y, 75 + variate, 50, -1, true, 0, 10000);
            for (int i = x - 40 - Math.Abs(variate); i < x + 40 + Math.Abs(variate); i++)
            {
                for (int j = y - 40 - Math.Abs(variate); j < y + 40 + Math.Abs(variate); j++)
                {
                    if (WorldGen.InWorld(i, j) && WorldGen.InWorld(i - 1, j) && WorldGen.InWorld(i + 1, j) && WorldGen.InWorld(i, j + 1) && WorldGen.InWorld(i, j - 1))
                    {
                        Tile t = Framing.GetTileSafely(i, j);
                        Tile tileLeft = Main.tile[i - 1, j];
                        Tile tileRight = Main.tile[i + 1, j];
                        Tile tileUp = Main.tile[i, j - 1];
                        Tile tileDown = Main.tile[i, j + 1];

                        if (j > y && t.wall > 0)
                            t.wall = WallID.Stone;

                        bool setInactiveCheck1 = !tileDown.active() && !tileUp.active() && !tileRight.active() && !tileLeft.active();
                        bool setInactiveCheck2 = tileDown.type != TileID.Containers && tileUp.type != TileID.Containers && tileRight.type != TileID.Containers && tileLeft.type != TileID.Containers &&
                            tileDown.type != TileID.Containers2 && tileUp.type != TileID.Containers2 && tileRight.type != TileID.Containers2 && tileLeft.type != TileID.Containers2;
                        if (setInactiveCheck1 && setInactiveCheck2)
                            t.active(false);

                        if (tileDown.active() && !tileUp.active() && tileLeft.active() && tileRight.active())
                        {
                            tileDown.type = TileID.Stone;
                            tileUp.type = TileID.Stone;
                            tileRight.type = TileID.Stone;
                            tileLeft.type = TileID.Stone;
                            tileUp.liquid = 255;
                        }
                        WorldGen.SquareTileFrame(i, j);
                    }
                }
            }
        }
        /// <summary>
        /// Gets the tile: up, down, left, and right as tiles respectively from the tile located at the Point provided. <para>Think of it as </para>
        /// <code>Tile[] { up, down, left, right };</code>
        /// </summary>
        /// <param name="tileCoords"></param>
        /// <returns>The tile up, down, left, and right of the Point.</returns>
        public static Tile[] GetTilesInCardinalsFrom(Point tileCoords)
        {
            Tile tLeft = Framing.GetTileSafely(tileCoords.X - 1, tileCoords.Y);
            Tile tRight = Framing.GetTileSafely(tileCoords.X + 1, tileCoords.Y); ;
            Tile tUp = Framing.GetTileSafely(tileCoords.X, tileCoords.Y - 1); ;
            Tile tDown = Framing.GetTileSafely(tileCoords.X, tileCoords.Y + 1); ;

            return new Tile[]
            {
                tUp,
                tDown,
                tLeft,
                tRight
            };
        }
        public static Point[] GetTileCoordinatesInCardinalsFrom(Point tileCoords)
        {

            var up = new Point(tileCoords.X, tileCoords.Y - 1);
            var down = new Point(tileCoords.X, tileCoords.Y + 1);
            var left = new Point(tileCoords.X - 1, tileCoords.Y);
            var right = new Point(tileCoords.X + 1, tileCoords.Y);
            return new Point[]
            {
                up,
                down,
                left,
                right
            };
        }
        /// <summary>
        /// Doesn't even work.
        /// </summary>
        public static void GetLiquidPool(Point initialTileCoordinates, Point area, bool showTiles = false)
        {
            Tile initialTile = Framing.GetTileSafely(initialTileCoordinates);

            Tile[] sorroundTile2 = GetTilesInCardinalsFrom(initialTileCoordinates);

            Main.NewText($"{initialTile.liquid} : {sorroundTile2[0].liquid}");

            var mathX = initialTileCoordinates.X - (area.X / 2);
            var mathY = initialTileCoordinates.Y - (area.Y / 2);
            for (int x = mathX; x < mathX + area.X; x++)
            {
                for (int y = mathY; y < mathY + area.Y; y++)
                {
                    // up, down, left, right
                    Tile[] sorroundTile = GetTilesInCardinalsFrom(new Point(x, y));

                    Tile[] tileAroundSorroundTile = { };

                    Point[] sorroundCoords = GetTileCoordinatesInCardinalsFrom(new Point(x, y));

                    foreach (var pt in sorroundCoords)
                    {
                        tileAroundSorroundTile = GetTilesInCardinalsFrom(pt);
                    }

                    if (tileAroundSorroundTile[0].liquid > 0 && sorroundTile[0].liquid > 0)
                    {
                        Dust.QuickBox(new Vector2(x, y).ToWorldCoordinates() - new Vector2(8, 8),
                            new Vector2(x, y).ToWorldCoordinates() + new Vector2(8, 8),
                            5, Color.Green, null);
                    }
                    else
                    {
                        Dust.QuickBox(new Vector2(x, y).ToWorldCoordinates() - new Vector2(8, 8),
                            new Vector2(x, y).ToWorldCoordinates() + new Vector2(8, 8),
                            5, Color.Red, null);
                    }
                    if (tileAroundSorroundTile[1].liquid > 0 && sorroundTile[1].liquid > 0)
                    {
                        Dust.QuickBox(new Vector2(x, y).ToWorldCoordinates() - new Vector2(8, 8),
                            new Vector2(x, y).ToWorldCoordinates() + new Vector2(8, 8),
                            5, Color.Green, null);
                    }
                    else
                    {
                        Dust.QuickBox(new Vector2(x, y).ToWorldCoordinates() - new Vector2(8, 8),
                            new Vector2(x, y).ToWorldCoordinates() + new Vector2(8, 8),
                            5, Color.Red, null);
                    }
                    if (tileAroundSorroundTile[2].liquid > 0 && sorroundTile[2].liquid > 0)
                    {
                        Dust.QuickBox(new Vector2(x, y).ToWorldCoordinates() - new Vector2(8, 8),
                            new Vector2(x, y).ToWorldCoordinates() + new Vector2(8, 8),
                            5, Color.Green, null);
                    }
                    else
                    {
                        Dust.QuickBox(new Vector2(x, y).ToWorldCoordinates() - new Vector2(8, 8),
                            new Vector2(x, y).ToWorldCoordinates() + new Vector2(8, 8),
                            5, Color.Red, null);
                    }
                    if (tileAroundSorroundTile[3].liquid > 0 && sorroundTile[3].liquid > 0)
                    {
                        Dust.QuickBox(new Vector2(x, y).ToWorldCoordinates() - new Vector2(8, 8),
                            new Vector2(x, y).ToWorldCoordinates() + new Vector2(8, 8),
                            5, Color.Green, null);
                    }
                    else
                    {
                        Dust.QuickBox(new Vector2(x, y).ToWorldCoordinates() - new Vector2(8, 8),
                            new Vector2(x, y).ToWorldCoordinates() + new Vector2(8, 8),
                            5, Color.Red, null);
                    }
                }
            }
        }
        /// <summary>
        /// Use 'as x' syntax. (also doesn't work)
        /// </summary>
        /// <param name="tileCoordinates"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public static object ChooseRandomTileFrom(Point tileCoordinates, Point area)
        {
            var mathX = tileCoordinates.X - (area.X / 2);
            var mathY = tileCoordinates.Y - (area.Y / 2);
            restart:
            for (int x = mathX; x < mathX + area.X; x++)
            {
                for (int y = mathY; y < mathY + area.Y; y++)
                {
                    if (Main.rand.Next(20) == 0)
                        return Main.tile[x, y];
                    else
                        goto restart;
                }
            }
            return "Invalid";
        }
    }
}