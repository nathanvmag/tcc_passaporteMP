using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public class Utils : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static  Sprite TakeScreenshotAndReturnSprite()
    {
        string screenshotFilePath = Path.Combine(Application.persistentDataPath, "screenshot.png");

        // Capture the screenshot as a Texture2D
        Texture2D screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();

        // Encode the texture to PNG and save it
        byte[] pngData = screenshotTexture.EncodeToPNG();
        File.WriteAllBytes(screenshotFilePath, pngData);

        // Clean up the temporary texture
        Destroy(screenshotTexture);

        // Load the image back from file
        Sprite screenshotSprite = LoadImageAsSprite(screenshotFilePath);

        // Optionally, delete the file after loading the sprite
        if (File.Exists(screenshotFilePath))
        {
            File.Delete(screenshotFilePath);
        }

        return screenshotSprite;
    }

    private static  Sprite LoadImageAsSprite(string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); // Automatically resizes the texture
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        return null;
    }
    public static bool ValidateEmail(string email)
    {
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, emailPattern);
    }
    public static bool ValidatePass(string pass)
    {
        return pass.Length >= 8;
    }
    
    public static string getAutentication()
    {
        return PlayerPrefs.GetString("auth", "");
    }
    public static async Task<bool> checkAuth(NetworkManager networkManager)
    {
        string authTk = getAutentication();
        if (authTk == "")
            return false;
        var response = await networkManager.CallWebMethodAsync("/api/users/me", authTk, "GET",false);
        
        try
        {
            PlayerPrefs.SetInt("myid", response.responseJson.Value<int>("id"));
            PlayerPrefs.Save();
            AppController.Instance.myPontos= response.responseJson.Value<int>("points") ;
            AppController.Instance.saldoText.text = AppController.Instance.myPontos.ToString(); 
        }
        catch
        {

        }
        return response.Success;
    }
    public static string Slugify(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Use a StringBuilder to build the slug efficiently
        StringBuilder slugBuilder = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // If it's an uppercase character and it's not the first character, add a hyphen
            if (char.IsUpper(c) && i > 0)
            {
                slugBuilder.Append('-');
            }

            // Append the lowercase version of the character
            slugBuilder.Append(char.ToLower(c));
        }

        return slugBuilder.ToString();
    }
    public static void saveCollectPersonagem(int currentTargetID)
    {
        PlayerPrefs.SetInt("collect_" + currentTargetID, 1);
        PlayerPrefs.Save();
    }
    public static bool checkCollectedPersonagem(int currentTargetID)
    {
        if (!PlayerPrefs.HasKey("collect_" + currentTargetID))
            return false;
        int collect = PlayerPrefs.GetInt("collect_" + currentTargetID,0);
        return collect == 1;

    }
}
