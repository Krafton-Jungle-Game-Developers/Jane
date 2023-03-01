using UnityEngine;

public struct PositionLerper
{
    Vector3 m_LerpStart;
    float m_CurrentLerpTime;
    float m_LerpTime;

    public PositionLerper(Vector3 start, float lerpTime)
    {
        m_LerpStart = start;
        m_CurrentLerpTime = 0f;
        m_LerpTime = lerpTime;
    }

    public Vector3 LerpPosition(Vector3 current, Vector3 target)
    {
        if (current != target)
        {
            m_LerpStart = current;
            m_CurrentLerpTime = 0f;
        }

        m_CurrentLerpTime += Time.deltaTime;
        if (m_CurrentLerpTime > m_LerpTime)
        {
            m_CurrentLerpTime = m_LerpTime;
        }

        var lerpPercentage = m_CurrentLerpTime / m_LerpTime;

        return Vector3.Lerp(m_LerpStart, target, lerpPercentage);
    }
}
