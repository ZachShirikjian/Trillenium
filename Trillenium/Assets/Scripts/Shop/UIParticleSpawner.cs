using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIParticleSpawner : MonoBehaviour
{
    #region Variables
    #region References
    [SerializeField] private PublicMethods methods; // Public methods script.
    [SerializeField] private ItemSelection items; // Item selection script.
    [SerializeField] private GameObject particlePrefab; // Particle prefab object.
    #endregion

    #region Movement
    // Positions for the particles to spawn at, depending on which items are being hovered over.
    private float swordPosX = 3.5f;
    private float swordPosY;

    private float medkitPosX = 0.5f;
    private float medkitPosY;
    #endregion

    private float rotZ; // Rotation value.

    private float timerIndex;
    private float timerLimit = 5f;
    #endregion

    void Awake()
    {
        timerIndex = timerLimit; // So the first particle to spawn doesn't have to wait.
    }
    
    void Update()
    {
        SetRandom();
    }

    void LateUpdate()
    {
        // Only spawn particles if the items are active and there are still items available for purchase in the shop.
        if (items.itemsActive && items.itemIndex >= 0)
        {
            SpawnParticles();
        }
    }

    #region Methods
    private void SetRandom()
    {
        int randNum = Random.Range(-2, 3); // Random integer from -2 (inclusive) to 3 (exclusive).
        
        // Calculations for positioning.
        swordPosY = methods.DecimalsRounded(3.95f + (randNum / 2f));
        medkitPosY = methods.DecimalsRounded(-2.5f + (randNum / 1.25f));

        // Random rotation.
        rotZ = Random.Range(0, 360); // 0-359, which is still 360 unique values.
    }

    private void SpawnParticles()
    {
        // X offsets to make the particle positioning look a bit nicer.
        float swordParticleOffsetX = -1.2f;
        float medkitParticleOffsetX = 0.2f;

        if (timerIndex >= timerLimit)
        {
            // Instantiate a new GameObject from existing base prefab to represent the individual particle.
            GameObject particleObj = Instantiate(particlePrefab, transform);

            switch (items.itemIndex)
            {
                case 0: // Top-left item.
                case 1: // Top-right item.
                    particleObj.transform.position = new Vector3(swordPosX - Random.Range(0.0f, 5.0f) + swordParticleOffsetX, swordPosY, 0f);
                    particleObj.GetComponent<UIParticleMovement>().spriteIndex = 0;
                    break;
                case 2: // Bottom-left item.
                case 3: // Bottom-right item.
                    particleObj.transform.position = new Vector3(medkitPosX - Random.Range(0.0f, 5.0f) + medkitParticleOffsetX, medkitPosY, 0f);
                    particleObj.GetComponent<UIParticleMovement>().spriteIndex = 1;
                    break;
                default:
                    Debug.LogError("Selected item is outside of expected range."); // Should never happen.
                    break;
            }


            // Apply rotation value.
            particleObj.transform.localRotation = Quaternion.Euler(0f, 0f, rotZ);

            // Reset timer index.
            timerIndex = 0f;
        }
        else
        {
            // Increment timer index.
            timerIndex += Time.deltaTime * 60f;
        }
    }
    #endregion
}
