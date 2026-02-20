using UnityEngine;
using UnityEngine.Events;

public class GemsController : MonoBehaviour
{  
    public static UnityAction UpdateCoins;

    public GemsUI gemsUI;

    public bool buttonActive = true;
    public bool plusActive = true;

    void Start()
    {
        UpdateCoins += gemsUI.UpdateGemsCount;

        gemsUI.Setup(buttonActive);
        gemsUI.SetGemsCount();
        gemsUI.SetPlusStateActiv(plusActive);
    }

    private void OnDestroy()
    {
        UpdateCoins -= gemsUI.SetGemsCount;
    }
}
