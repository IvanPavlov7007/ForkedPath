using UnityEngine;
using UnityEditor;

public class BakeCheckerGradientFade
{
    [MenuItem("Tools/Bake Checker Gradient Fade")]
    static void BakeTexture()
    {
        int width = 512;
        int height = 512;
        int cell = 16; // pixel size of each checker square

        // Create new texture
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Size of the gap between low/high alpha in a row
        float halfStep = 1f / cell;

        for (int y = 0; y < height; y++)
        {
            float halfH = height / 2f;

            // Compute unique position within its half (0 at center line, 1 at the outer edge)
            bool isUpperHalf = y < halfH;
            float tHalf = isUpperHalf
                ? (halfH - 1f - y) / (halfH - 1f)   // top half: center -> 0, top edge -> 1
                : (y - halfH) / (halfH - 1f);       // bottom half: center -> 0, bottom edge -> 1

            // Optional curve adjustment for smoother progression (uncomment to tweak)
            // tHalf = Mathf.Pow(tHalf, 1.2f);

            // Map each half into disjoint ranges so rows are unique in both directions:
            // - Upper half rows:   aLow in [0.0, 0.5 - halfStep]
            // - Lower half rows:   aLow in [0.5, 1.0 - halfStep]
            float start = isUpperHalf ? 0f : 0.5f;
            float range = 0.5f - halfStep;

            // Each row gets a unique aLow within its half; aHigh stays exactly one halfStep above.
            float aLow = start + tHalf * range;
            float aHigh = aLow + halfStep;

            for (int x = 0; x < width; x++)
            {
                // Determine checker pattern (alternate across grid)
                int checker = ((x / cell) + (y / cell)) % 2;
                float a = checker == 0 ? aHigh : aLow;
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }

        tex.Apply();

        // Encode and save as PNG
        byte[] bytes = tex.EncodeToPNG();
        string path = Application.dataPath + "/checker_gradient_fade.png";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.Refresh();

        Debug.Log("✅ Checker gradient fade texture baked at: " + path);
    }
}
