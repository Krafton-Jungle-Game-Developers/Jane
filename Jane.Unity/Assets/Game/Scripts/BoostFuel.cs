using UnityEngine;

public class BoostFuel : MonoBehaviour
{
    [SerializeField] private float capacity = 8f;
    [SerializeField] private float unitResourceChange = -1f;
    [SerializeField] private bool perSecond = true;
    [SerializeField] private bool setEmptyWhenInsufficient = true;
    
    [SerializeField] protected float changeRate = 0f;
    public float ChangeRate => changeRate;

    [SerializeField] protected float startAmount = 5f;

    protected float currentAmount = 0f;

    [Header("Refill After Empty")]
    [SerializeField] protected float emptiedPause = 1f;
    [SerializeField] protected bool fillToCapacityAfterEmptiedPause = true;

    [Header("Empty After Filled")]
    [SerializeField] protected float filledPause = 0f;
    [SerializeField] protected bool emptyAfterFilledPause = false;

    protected bool pausing = false;
    protected float pauseStartTime;
    protected float pauseTime;

    public bool IsFull => Mathf.Approximately(currentAmount, capacity);
    public bool IsEmpty => Mathf.Approximately(currentAmount, 0f);

    protected void Awake()
    {
        currentAmount = Mathf.Clamp(startAmount, 0, capacity);
    }

    void Update()
    {
        if (!pausing)
        {
            AddRemove(changeRate * Time.deltaTime);
        }
        else
        {
            // If filled/emptied pause is finished, implement settings
            if (Time.time - pauseStartTime >= pauseTime)
            {
                pausing = false;

                if (IsEmpty && fillToCapacityAfterEmptiedPause)
                {
                    Fill();
                }
                else if (IsFull && emptyAfterFilledPause)
                {
                    Empty();
                }
            }
        }
    }

    public bool Ready(int numResourceChanges = 1)
    {
        if (setEmptyWhenInsufficient && unitResourceChange < 0)
        {
            if (!HasAmount(numResourceChanges * Mathf.Abs(unitResourceChange) * (perSecond ? Time.deltaTime : 1))) { Empty(); }
        }

        if (!CanAddRemove(numResourceChanges * unitResourceChange * (perSecond ? Time.deltaTime : 1))) { return false; }

        return true;
    }

    public void Implement(int numResourceChanges = 1)
    {
        AddRemove(numResourceChanges * unitResourceChange * (perSecond ? Time.deltaTime : 1));
    }

    public bool CanAddRemove(float amount)
    {
        if (pausing) return false;
        if (amount > 0 && (capacity - currentAmount) < amount) return false;
        if (amount < 0 && (currentAmount + amount) < 0) return false;

        return true;
    }

    public void AddRemove(float amount)
    {
        float nextValue = currentAmount + amount;

        if (nextValue >= capacity && !Mathf.Approximately(currentAmount, capacity))
        {
            OnFilled();
        }

        if (nextValue <= 0 && !Mathf.Approximately(currentAmount, 0))
        {
            OnEmpty();
        }

        SetAmount(nextValue);
    }

    protected void OnFilled()
    {
        if (filledPause > 0)
        {
            pausing = true;
            pauseStartTime = Time.time;
            pauseTime = filledPause;
        }
    }

    protected void OnEmpty()
    {
        if (emptiedPause > 0)
        {
            pausing = true;
            pauseStartTime = Time.time;
            pauseTime = emptiedPause;
        }
    }

    public void Fill()
    {
        if (pausing) return;
        SetAmount(capacity);
        OnFilled();
    }

    public void Empty()
    {
        if (pausing) return;
        SetAmount(0);
        OnEmpty();
    }

    public bool HasAmount(float amount) => currentAmount >= amount;

    public void SetAmount(float amount)
    {
        amount = Mathf.Clamp(amount, 0, capacity);

        if (!Mathf.Approximately(amount, currentAmount))
        {
            currentAmount = amount;
        }
    }
}
