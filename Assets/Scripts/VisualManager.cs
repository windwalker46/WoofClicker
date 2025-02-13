using UnityEngine;
using UnityEngine.UI;
using System.Collections; 

public class VisualManager : MonoBehaviour
{
    // --- Button Skin Management 
    public Button mainButton;            
    public Sprite[] buttonSkins;         
    private int currentSkinIndex = 0;

    // --- Background Scrolling ---
    public GameObject background1;       
    public GameObject background2;       
    public float scrollSpeed = 100f;     

    private float backgroundWidth;       
    private Vector3 bg1Initial;          

    private float scrollOffset = 0f;

    // --- Test Mode Parameters ---
    public bool testMode = true;             
    public float testRepositionTime = 2.0f; 
    private float testTimer = 0f;            

    // Smooth reposition settings
    public float repositionDuration = 0.3f;  
    private bool isRepositioning = false;    

    private void Start()
    {
        if (mainButton != null && buttonSkins.Length > 0)
        {
            currentSkinIndex = Random.Range(0, buttonSkins.Length);
            mainButton.image.sprite = buttonSkins[currentSkinIndex];
        }

        if (background1 == null || background2 == null)
        {
            Debug.LogError("One or both backgrounds are not assigned!");
            return;
        }

        bg1Initial = background1.transform.position;

        // Check the background width using background1's Renderer
        Renderer r = background1.GetComponent<Renderer>();
        if (r != null)
        {
            backgroundWidth = r.bounds.size.x;
        }
        else
        {
            Debug.LogError("Background1 does not have a Renderer!");
        }

        background2.transform.position = bg1Initial + new Vector3(backgroundWidth, 0, 0);
    }

    private void Update()
    {
        if (backgroundWidth <= 0) return;

        if (!isRepositioning)
        {
            scrollOffset += scrollSpeed * Time.deltaTime;
            scrollOffset %= backgroundWidth;

            background1.transform.position = bg1Initial - new Vector3(scrollOffset, 0, 0);

            background2.transform.position = background1.transform.position + new Vector3(backgroundWidth, 0, 0);
        }

        if (testMode && !isRepositioning)
        {
            testTimer += Time.deltaTime;
            if (testTimer >= testRepositionTime)
            {
                StartCoroutine(SmoothReposition(repositionDuration));
                testTimer = 0f;  
            }
        }
    }

    private IEnumerator SmoothReposition(float duration)
    {
        isRepositioning = true;  

        float elapsed = 0f;

        // Current positions
        Vector3 startPos1 = background1.transform.position;
        Vector3 startPos2 = background2.transform.position;

        // Target positions
        Vector3 endPos1 = bg1Initial;
        Vector3 endPos2 = bg1Initial + new Vector3(backgroundWidth, 0, 0);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            background1.transform.position = Vector3.Lerp(startPos1, endPos1, t);
            background2.transform.position = Vector3.Lerp(startPos2, endPos2, t);

            yield return null;
        }

        // Finalize positions
        background1.transform.position = endPos1;
        background2.transform.position = endPos2;

        // Reset offset so next cycle starts from zero
        scrollOffset = 0f;

        isRepositioning = false; 
    }

    // --- Button Skin Methods ---
    public void ChangeButtonSkin(int skinIndex)
    {
        if (mainButton != null && buttonSkins.Length > 0)
        {
            skinIndex = Mathf.Clamp(skinIndex, 0, buttonSkins.Length - 1);
            currentSkinIndex = skinIndex;
            mainButton.image.sprite = buttonSkins[currentSkinIndex];
        }
        else
        {
            Debug.LogWarning("Button or Skins are not assigned!");
        }
    }

    public void CycleSkins()
    {
        if (mainButton != null && buttonSkins.Length > 0)
        {
            currentSkinIndex = (currentSkinIndex + 1) % buttonSkins.Length;
            mainButton.image.sprite = buttonSkins[currentSkinIndex];
        }
    }
}
