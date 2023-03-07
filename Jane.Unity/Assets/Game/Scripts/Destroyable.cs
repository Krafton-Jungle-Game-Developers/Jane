using Jane.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public float durability = 10;
    [SerializeField] private float _stunTime = 1f;
    [SerializeField] private GameObject destroyPrefab;

    public float maxHealth;
    private bool _isPlayer = false;
    private SpaceshipInputManager _inputManager;

    private void Start()
    {
        // check if object is Player 
        if (gameObject.tag == "Player")
        {
            _isPlayer = true;
            _inputManager = GameObject.FindObjectOfType<SpaceshipInputManager>();
            maxHealth = durability;
            //StartCoroutine(TestHP());
        }
        else
        {
            _isPlayer = false;
        }
    }


    void Update()
    {
        if(durability <= 0)
        {
            if(destroyPrefab!= null)
            {
                Instantiate(destroyPrefab, transform.position, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f)));
            }

            if (!_isPlayer)
            {
                Destroy(gameObject);
            } 
            else if (_inputManager != null) 
            {
                StartCoroutine(StunPlayer());
                
                // Player Stun End
                _inputManager.EnableInput();
                _inputManager.EnableMovement();
                _inputManager.EnableSteering();
                
            }
        }
    }

    private IEnumerator StunPlayer()
    {
        if (!_isPlayer) 
        {
            yield return null;
        }
        else
        {
            _inputManager.DisableInput();
            _inputManager.DisableMovement(true);
            _inputManager.DisableSteering(true);
            yield return new WaitForSeconds(_stunTime);

            // return to full health 
            durability = maxHealth;
        }
    }

    private IEnumerator TestHP()
    {
        Debug.Log("Start HP Test");
        yield return new WaitForSeconds(1f);

        Debug.Log("HP dropping");
        while (durability > 0)
        {
            durability -= 1.0f;
            yield return new WaitForSeconds(0.5f);
        }

    

    }
}
