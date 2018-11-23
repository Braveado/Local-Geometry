using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    int randomColor;
    public Color[] colors;
    public SpriteRenderer overlay;
    Color olColor;
    bool olFade;
    public Camera cam;
    bool camMov;
    float camMovValue;
    bool camRot;
    float camRotValue;

	void Start ()
    {
        randomColor = Random.Range(0, colors.Length);
        GetComponent<SpriteRenderer>().color = colors[randomColor];        
	}

    void Update ()
    {
        // overlay alpha
        if (overlay.color.a >= 1.0f)
            olFade = true;
        else if (overlay.color.a <= 0.75f)
            olFade = false;

        if(olFade)
        {
            olColor = overlay.color;
            olColor.a -= 0.25f * Time.deltaTime;
            overlay.color = olColor;
        }
        else if (!olFade)
        {
            olColor = overlay.color;
            olColor.a += 0.25f * Time.deltaTime;
            overlay.color = olColor;
        }

        // camera rotation
        if (camRotValue >= 3.0f)
            camRot = true;
        else if (camRotValue <= -3.0f)
            camRot = false;

        if (camRot)
        {
            camRotValue -= 3f * Time.deltaTime;
            cam.transform.Rotate(Vector3.back * 3f * Time.deltaTime);
        }
        else if (!camRot)
        {
            camRotValue += 3f * Time.deltaTime;
            cam.transform.Rotate(Vector3.forward * 3f * Time.deltaTime);
        }

        // camera zoom movement
        if (cam.orthographicSize >= 5.0f)
            camMov = true;
        else if (cam.orthographicSize <= 4.5f)
            camMov = false;

        if (camMov)
        {
            cam.orthographicSize -= 0.5f * Time.deltaTime;
        }
        else if (!camMov)
        {
            cam.orthographicSize += 0.5f * Time.deltaTime;
        }
    }
	
}
