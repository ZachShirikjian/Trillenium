using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleMovement : MonoBehaviour
{
    #region Variables
    [SerializeField] private Sprite[] sprites = new Sprite[2];

    public int spriteIndex;
    
    private float distance = 1f;
    private float posX;
    private float rotZ;
    private float alpha;
    #endregion

    void Start()
    {
        posX = this.transform.position.x;
        rotZ = this.transform.rotation.z;
        alpha = 0.5f;
    }
    
    void Update()
    {
        posX -= Time.deltaTime * 60f;
        distance -= Time.deltaTime * 5f;
        rotZ -= Time.deltaTime * 650f;

        alpha = (distance / 1f) / 2.5f;

        if (alpha < 0f)
        {
            Destroy(this.gameObject);
        }
    }

    void LateUpdate()
    {
        this.transform.position = new Vector3(posX, this.transform.position.y, 0f);
        this.transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);
        this.transform.localScale = new Vector3(alpha * 2f, alpha * 2f, 1f);
        this.GetComponent<SpriteRenderer>().sprite = sprites[spriteIndex];
        this.GetComponent<SpriteRenderer>().color = new Vector4(1f, 1f, 1f, alpha);
    }
}
