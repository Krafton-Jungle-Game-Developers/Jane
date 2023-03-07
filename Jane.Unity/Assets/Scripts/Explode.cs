//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Explode : MonoBehaviour
//{
//    [SerializeField] private GameObject piecePrefab;
//    [SerializeField] private int _piecePerAxis = 4;
               
//    [SerializeField] private float _force = 500f;
//    [SerializeField] private float _radius = 2f;
//    [SerializeField] private float _delay = 3f;
//    [SerializeField] private float _posScale = 5f;
    
//    // Start is called before the first frame update
//    void Start()
//    {
//        Invoke("Boom", _delay);
//    }

//    private void Boom()
//    {
//        // create pieces
//        for (int x = 0; x < _piecePerAxis; x++)
//        {
//            for (int y = 0; y < _piecePerAxis; y++)
//            {
//                for (int z = 0; z < _piecePerAxis; z++)
//                {
//                    CreatePiece(new Vector3(x, y, z));
//                }
//            }
//        }
//        Destroy(gameObject);
//        // Destroy will be managed by Destroyable.cs Script
//    }

//    private void CreatePiece(Vector3 coordinates)
//    {
//        GameObject piece = Instantiate(piecePrefab);
        
//        piece.transform.localScale = transform.localScale / _piecePerAxis;   

//        // add rigidbody to set mass
//        Rigidbody rb = piece.GetComponent<Rigidbody>();
//        if (rb == null)
//        {
//            rb = piece.AddComponent<Rigidbody>();

//        }
//        rb.isKinematic = false;
//        rb.useGravity = false;

//        Vector3 firstPiece = transform.position - transform.localScale / 2 + piece.transform.localScale / 2;
//        piece.transform.position = firstPiece + Vector3.Scale(coordinates, piece.transform.localScale * _posScale);
//        //piece.transform.position = firstPiece + Vector3.Scale(coordinates, piece.transform.localScale);

//        rb.AddExplosionForce(_force, piece.transform.position, _radius);

//        Destroy(piece, 2f);
//    }
//}
