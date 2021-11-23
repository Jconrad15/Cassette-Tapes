using System.Collections.Generic;
using UnityEngine;

namespace CassetteProject
{
    public enum DECALSIZE { small, large };
    public enum STRIPETYPE { horizontal, vertical };
    public class Cassette
    {
        public int height;
        public int width;

        public DECALSIZE decalSize;

        public int minDecalX;
        public int maxDecalX;
        public int minDecalY;
        public int maxDecalY;

        public int bottomHoleCount;

        public bool stripes;
        public STRIPETYPE stripeType;
        public int stripeCount;

        public List<int> stripeColumns = new List<int>();
        public List<int> stripeRows = new List<int>();

        public bool overlaySquares;
        public int overlaySquareCount;
        public List<Vector2> overlaySquarePositions = new List<Vector2>();

        // Colors
        public Color32 edgeMainColor;
        public Color32 edgeAccentColor;
        public Color32 bottomLineColor;

        public Color32 decalMainColor;
        public Color32 overlaySquareColor;
        public List<Color32> stripeColors = new List<Color32>();

        public Color32 tapeColor;

        public Dictionary<(int, int), Color32> colorPositionDict = new Dictionary<(int, int), Color32>();
    }

    public class CassetteGenerator : MonoBehaviour
    {
        private int minStripes = 1;
        private int maxStripes = 4;
        private float stripeChance = 0.7f;
        private float overlayNeighborChance = 0.6f;
        private Color32 noColor = new Color32(0, 0, 0, 0);
        private float thinStripeChance = 0.6f;
        private float horizontalStripeChance = 0.5f;

        [SerializeField]
        private List<Color32> edgeMainColorDatabase;
        [SerializeField]
        private List<Color32> decalMainColorDatabase;

        [SerializeField]
        private List<Color32> overlaySquareColorDatabase;

        public void SetStripeChance(System.Single newStripeChance)
        {
            stripeChance = newStripeChance;
        }

        public void SetThinStripeChance(System.Single newThinStripeChance)
        {
            thinStripeChance = newThinStripeChance;
        }

        public void SetMaxStripes(System.Single newMaxStripes)
        {
            maxStripes = (int)newMaxStripes;
        }

        public void SetHorizontalStripeChance(System.Single newhorizontalStripeChance)
        {
            horizontalStripeChance = newhorizontalStripeChance;
        }

        /// <summary>
        /// Generates the data the represents a cassette's logo/decal.
        /// </summary>
        public Cassette GenerateCassetteDecal()
        {
            Cassette cassette = new Cassette();

            SetCassetteSize(cassette);
            SetDecalSize(cassette);
            SetBottomHoles(cassette);
            TryAddStripes(cassette);
            TryOverlaySquares(cassette);

            ChooseBaseColors(cassette);

            ColorCassette(cassette);

            return cassette;
        }

        private void SetBottomHoles(Cassette cassette)
        {
            int switchVar = Random.Range(0, 3);
            switch (switchVar)
            {
                case 0:
                    cassette.bottomHoleCount = 0;
                    break;

                case 1:
                    cassette.bottomHoleCount = 2;
                    break;

                case 2:
                    cassette.bottomHoleCount = 4;
                    break;

                default:
                    break;
            }
        }

        private void SetCassetteSize(Cassette cassette)
        {
            cassette.height = 19;
            cassette.width = 28;
        }

        private void ColorCassette(Cassette cassette)
        {
            // Color all pixels
            for (int x = 0; x < cassette.width; x++)
            {
                for (int y = 0; y < cassette.height; y++)
                {
                    // Medium square outlined by decal
                    if (x >= 3 && x <= 24 &&
                        y >= 5 && y <= 15)
                    {
                        CenterAreaColoring(x, y, cassette);
                    }
                    else // edge area
                    {
                        EdgeAreaColoring(x, y, cassette);
                    }
                }
            }
        }

        private void CenterAreaColoring(int x, int y, Cassette cassette)
        {
            // Cassette winding holes
            if ((y == 9 && (x == 8 || x == 19)) ||
                (y == 10 && ((x > 6 && x < 10) || (x > 17 && x < 21))) ||
                (y == 11 && (x == 8 || x == 19)))
            {
                cassette.colorPositionDict.Add((x, y), noColor);
                return;
            }

            // Interior hole
            if (x > 13 && x < 16 && y > 8 && y < 12)
            {
                cassette.colorPositionDict.Add((x, y), noColor);
                return;
            }

            // Tape 
            if ((x > 10 && x < 14 && y > 8 && y < 12) ||
                (x == 16 && y > 8 && y < 12))
            {
                cassette.colorPositionDict.Add((x, y), cassette.tapeColor);
                return;
            }

            // Center color (depends on decal size)
            if (cassette.decalSize == DECALSIZE.small)
            {
                if (x > 6 && x < 21 && y > 7 && y < 13)
                {
                    cassette.colorPositionDict.Add((x, y), cassette.edgeMainColor);
                    return;
                }
                if ((x == 6 && y > 8 && y < 12) ||
                    (x == 21 && y > 8 && y < 12))
                {
                    cassette.colorPositionDict.Add((x, y), cassette.edgeMainColor);
                    return;
                }
            }
            else // Decal size is large
            {
                if (x > 5 && x < 22 && y > 7 && y < 13)
                {
                    cassette.colorPositionDict.Add((x, y), cassette.edgeMainColor);
                    return;
                }
            }

            // Overlay square positions
            foreach (Vector2 overlaySquare in cassette.overlaySquarePositions)
            {
                if (x == overlaySquare.x && y == overlaySquare.y)
                {
                    cassette.colorPositionDict.Add((x, y), cassette.overlaySquareColor);
                    return;
                }
            }

            // Stripes
            if (cassette.stripes == true)
            {
                if (cassette.stripeType == STRIPETYPE.horizontal)
                {
                    for (int i = 0; i < cassette.stripeRows.Count; i++)
                    {
                        if (y == cassette.stripeRows[i])
                        {
                            cassette.colorPositionDict.Add((x, y), cassette.stripeColors[i]);
                            return;
                        }
                    }
                }
                else if (cassette.stripeType == STRIPETYPE.vertical)
                {
                    for (int i = 0; i < cassette.stripeColumns.Count; i++)
                    {
                        if (x == cassette.stripeColumns[i])
                        {
                            cassette.colorPositionDict.Add((x, y), cassette.stripeColors[i]);
                            return;
                        }
                    }
                }

            }

            // If none of the above, then color the pixel as the main decal color
            cassette.colorPositionDict.Add((x, y), cassette.decalMainColor);
        }

        private void EdgeAreaColoring(int x, int y, Cassette cassette)
        {
            // Hole punch corners
            if ((x == 0 && y == 0) ||
                (x == cassette.width - 1 && y == 0) ||
                (x == 0 && y == cassette.height - 1) ||
                (x == cassette.width - 1 && y == cassette.height - 1))
            {
                cassette.colorPositionDict.Add((x, y), noColor);
                return;
            }

            // Corner accents
            if ((x == 1 && y == 1) ||
                (x == cassette.width - 2 && y == 1) ||
                (x == 1 && y == cassette.height - 2) ||
                (x == cassette.width - 2 && y == cassette.height - 2))
            {
                cassette.colorPositionDict.Add((x, y), cassette.edgeAccentColor);
                return;
            }

            // Bottom lines
            if ((x == 5 && y < 2) ||
                (x == 6 && y > 1 && y < 4) ||
                (x > 6 && x < 21 && y == 4) ||
                (x == 21 && y > 1 && y < 4) ||
                (x == 22 && y < 2))
            {
                cassette.colorPositionDict.Add((x, y), cassette.bottomLineColor);
                return;
            }

            // Bottom holes
            if (cassette.bottomHoleCount == 2)
            {
                if ((x == 8 && y == 1) ||
                    (x == 19 && y == 1))
                {
                    cassette.colorPositionDict.Add((x, y), noColor);
                    return;
                }
            }
            else if (cassette.bottomHoleCount == 4)
            {
                if ((x == 8 && y == 1) ||
                    (x == 19 && y == 1))
                {
                    cassette.colorPositionDict.Add((x, y), noColor);
                    return;
                }
                if ((x == 10 && y == 2) ||
                    (x == 17 && y == 2))
                {
                    cassette.colorPositionDict.Add((x, y), noColor);
                    return;
                }

            }
            else { } // bottomHoldCount == 0


            // If none of the above, then color the pixel as the main edge color
            cassette.colorPositionDict.Add((x, y), cassette.edgeMainColor);
        }

        private void ChooseBaseColors(Cassette cassette)
        {
            cassette.edgeMainColor = edgeMainColorDatabase[Random.Range(0, edgeMainColorDatabase.Count)];
            cassette.edgeAccentColor = LightenColor(cassette.edgeMainColor);
            cassette.bottomLineColor = DarkenColor(cassette.edgeMainColor);

            cassette.decalMainColor = decalMainColorDatabase[Random.Range(0, decalMainColorDatabase.Count)];
            cassette.overlaySquareColor = overlaySquareColorDatabase[Random.Range(0, overlaySquareColorDatabase.Count)];

            cassette.tapeColor = new Color32(98, 85, 101, 255);
        }

        private Color32 DarkenColor(Color32 edgeMainColor)
        {
            Color.RGBToHSV(edgeMainColor, out float h, out float s, out float v);

            v -= 10f / 100f;
            s -= 5f / 100f;
            Color32 newColor = Color.HSVToRGB(h, s, v);

            return newColor;
        }

        private Color32 LightenColor(Color32 edgeMainColor)
        {
            Color.RGBToHSV(edgeMainColor, out float h, out float s, out float v);

            v += 20f / 100f;
            s += 10f / 100f;
            Color32 newColor = Color.HSVToRGB(h, s, v);

            return newColor;
        }

        /// <summary>
        /// Returns a random color.
        /// </summary>
        /// <returns></returns>
        public Color32 RandomColor()
        {
            return new Color32(
                (byte)Random.Range(0, 255),
                (byte)Random.Range(0, 255),
                (byte)Random.Range(0, 255),
                255);
        }

        private void SetDecalSize(Cassette cassette)
        {
            // Decal size type A or B
            cassette.decalSize = (DECALSIZE)Random.Range(0, 2);

            cassette.minDecalX = 3;
            cassette.maxDecalX = 24;
            cassette.minDecalY = 5;
            cassette.maxDecalY = 15;
    }

        private void TryAddStripes(Cassette cassette)
        {
            // Add stripes % of the time
            if (Random.value < (1 - stripeChance))
            {
                cassette.stripes = false;
                return;
            }

            cassette.stripes = true;
            DetermineStripeParameters(cassette);
        }

        private void DetermineStripeParameters(Cassette cassette)
        {
            if (Random.value < horizontalStripeChance)
            {
                cassette.stripeType = STRIPETYPE.horizontal;
            }
            else
            {
                cassette.stripeType = STRIPETYPE.vertical;
            }

            cassette.stripeCount = Random.Range(minStripes, maxStripes + 1);

            if (cassette.stripeType == STRIPETYPE.horizontal)
            {
                for (int i = 0; i < cassette.stripeCount; i++)
                {
                    // Choose a row for a thin line
                    // TO DO: or multiple rows for a thick line

                    if (Random.value < thinStripeChance)
                    {
                        int selectedRow = Random.Range(cassette.minDecalY, cassette.maxDecalY + 1);
                        cassette.stripeRows.Add(selectedRow);

                        // Add a color for the stripe
                        cassette.stripeColors.Add(RandomColor());
                    }
                    else
                    {
                        // Thick stripe -- two stripes next to each other
                        int selectedRow = Random.Range(cassette.minDecalY, cassette.maxDecalY + 1);
                        cassette.stripeRows.Add(selectedRow);
                        
                        // Add a color for the stripe
                        cassette.stripeColors.Add(RandomColor());

                        // Move up or down a row for the next row
                        if (Random.value > 0.5f)
                        { selectedRow += 1; }
                        else
                        { selectedRow -= 1; }

                        cassette.stripeRows.Add(selectedRow);
                        // Re-add the same color
                        cassette.stripeColors.Add(cassette.stripeColors[cassette.stripeColors.Count-1]);
                    }
                }
            }
            else if (cassette.stripeType == STRIPETYPE.vertical)
            {
                for (int i = 0; i < cassette.stripeCount; i++)
                {
                    if (Random.value < thinStripeChance)
                    {
                        // Choose a column for a thin line
                        // TO DO: or multiple columns for a thick line
                        int selectedColumn = Random.Range(cassette.minDecalX, cassette.maxDecalX + 1);
                        cassette.stripeColumns.Add(selectedColumn);

                        // Add a color for the stripe
                        cassette.stripeColors.Add(RandomColor());
                    }
                    else
                    {
                        // Thick stripe -- two stripes next to each other
                        int selectedColumn = Random.Range(cassette.minDecalX, cassette.maxDecalX + 1);
                        cassette.stripeColumns.Add(selectedColumn);

                        // Add a color for the stripe
                        cassette.stripeColors.Add(RandomColor());

                        // Move up or down a column for the next column
                        if (Random.value > 0.5f)
                        { selectedColumn += 1; }
                        else
                        { selectedColumn -= 1; }

                        cassette.stripeColumns.Add(selectedColumn);
                        // Re-add the same color
                        cassette.stripeColors.Add(cassette.stripeColors[cassette.stripeColors.Count-1]);
                    }
                }
            }

        }

        // TO DO: check for overlapping squares??
        private void TryOverlaySquares(Cassette cassette)
        {
            // Add overlay squares 90% of the time
            if (Random.value < 0.9)
            {
                cassette.overlaySquares = true;

                cassette.overlaySquareCount = Random.Range(3, 7);
                for (int i = 0; i < cassette.overlaySquareCount; i++)
                {
                    int x;
                    int y;

                    // Chance to place the next overlay square next to a previous square
                    if (Random.value < overlayNeighborChance &&
                        cassette.overlaySquarePositions.Count > 0)
                    {
                        // Get a previous overlay 
                        Vector2 selectedPreviousOverlayPosition = cassette.overlaySquarePositions
                            [Random.Range(0, cassette.overlaySquarePositions.Count)];

                        // Select a new x and y 
                        (x, y) = SelectNeighboringLocation(selectedPreviousOverlayPosition);
                    }
                    else
                    {
                        // Random position on the decal
                        int selectedQuad = Random.Range(0, 4);
                        switch (selectedQuad)
                        {
                            case 0:
                                x = Random.Range(4, 24);
                                y = Random.Range(13, 15);
                                break;

                            case 1:
                                x = Random.Range(4, 24);
                                y = Random.Range(6, 8);
                                break;

                            case 2:
                                x = Random.Range(4, 6);
                                y = Random.Range(8, 13);
                                break;

                            case 3:
                                x = Random.Range(22, 24);
                                y = Random.Range(8, 13);
                                break;

                            default:
                                x = Random.Range(4, 24);
                                y = Random.Range(6, 15);
                                break;
                        }
                    }
                    Vector2 newPosition = new Vector2(x, y);
                    cassette.overlaySquarePositions.Add(newPosition);
                }
            }
        }

        private (int, int) SelectNeighboringLocation(Vector2 startPos)
        {
            int x = (int)startPos.x;
            int y = (int)startPos.y;

            // Determine whether to vary x or y
            if (Random.value > 0.5f)
            {
                // vary x
                if (Random.value > 0.5f)
                { x = (int)(startPos.x + 1); }
                else
                { x = (int)(startPos.x + -1); }
            }
            else
            {
                // vary y
                if (Random.value > 0.5f)
                { y = (int)(startPos.y + 1); }
                else
                { y = (int)(startPos.y + -1); }
            }
            return (x, y);
        }

        /// <summary>
        /// Creates a cassette object in the current scene.
        /// </summary>
        public GameObject CreateCassetteObject(Cassette cassette)
        {
            Texture2D newCassetteTex = CreateTexture(cassette);

            // Create gameobject for the created texture
            GameObject cas_go = new GameObject("Cassette");
            SpriteRenderer cas_sr = cas_go.AddComponent<SpriteRenderer>();

            cas_sr.sprite = Sprite.Create(
                newCassetteTex,
                new Rect(0, 0, newCassetteTex.width, newCassetteTex.height),
                new Vector2(0.5f, 0.5f));

            return cas_go;
        }

        private Texture2D CreateTexture(Cassette cassette)
        {
            // Create a new texture for each old texture
            Texture2D newCassetteTex = new Texture2D(cassette.width, cassette.height);
            newCassetteTex.filterMode = FilterMode.Point;

            // Change the colors in the new texture based on the palettes
            int y = 0;
            while (y < newCassetteTex.height)
            {
                int x = 0;
                while (x < newCassetteTex.width)
                {
                    Color32 newColor = GetPixelColor(x, y, cassette.colorPositionDict);

                    // Set the pixel to be the selected color
                    newCassetteTex.SetPixel(x, y, newColor);

                    ++x;
                }
                ++y;
            }
            newCassetteTex.Apply();
            return newCassetteTex;
        }

        private Color32 GetPixelColor(int x, int y, Dictionary<(int, int), Color32> colorPositionDict)
        {
            (int, int) tuple = (x, y);
            Color32 colorOut;
            if (colorPositionDict.TryGetValue(tuple, out colorOut))
            {
                return colorOut;
            }
            else
            {
                Debug.LogError("There is no color for: (" + x + ", " + y + ").");
                return Color.white;
            }
        }
    }
}