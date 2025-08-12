using UnityEngine;

public class ImageSwitcher : MonoBehaviour
{
    public Texture[] images;  // Array to store the images
    public float switchInterval = 1f; // Time between image switches
    private Material material;  // Material to update
    private int currentImageIndex = 0;  // To keep track of the current image

    void Start()
    {
        // Get the material of the plane
        material = GetComponent<Renderer>().material;

        // Start the image switch loop
        InvokeRepeating("SwitchImage", 0f, switchInterval);
    }

    void SwitchImage()
    {
        // Change the texture to the next one in the array
        currentImageIndex = (currentImageIndex + 1) % images.Length;
        material.mainTexture = images[currentImageIndex];
    }
}
