using System.Collections;
using UnityEngine;

public class RaycastHitComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
    }
}
