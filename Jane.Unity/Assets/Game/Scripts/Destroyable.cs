using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public int durability = 10;
    [SerializeField] private float _stunTime = 1f;
    [SerializeField] private GameObject destroyPrefab;

    public int maxHealth;
    private bool _isPlayer = false;
    private SpaceshipEngine _spaceshipEngine;

    private void Start()
    {
        // check if object is Player 
        if (gameObject.tag == "Player")
        {
            _isPlayer = true;
            _spaceshipEngine = gameObject.GetComponent<SpaceshipEngine>();
            maxHealth = durability;
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
            else if (_spaceshipEngine != null) 
            {
                StartCoroutine(StunPlayer());
                
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
            _spaceshipEngine.SetControlBool(false); 
            yield return new WaitForSeconds(_stunTime);
            
            // return to full health 
            durability = maxHealth;
            _spaceshipEngine.SetControlBool(true); 
        }
    }
}
