using UnityEngine;

public abstract class BufferedInterpolator<T> where T : struct
{
}

public class BufferedInterpolatorFloat : BufferedInterpolator<float>
{
}

public class BufferedInterpolatorVector3 : BufferedInterpolator<Vector3>
{
}

public class BufferedInterpolatorQuaternion : BufferedInterpolator<Quaternion>
{
}
