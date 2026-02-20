using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FxGameController : MonoBehaviour
{
    public static UnityAction<AttackInfoType, string> spawnPopup;

    public List<FxPopupInfo> fxPopups;

    [Header("Setup")]
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private FxPopupController popupPrefab;
    [SerializeField] private int poolPreload = 12;
    [SerializeField] private int maxConcurrent = 20;

    [Header("Anti-overlap")]
    [SerializeField] private float minDistance = 25f;

    private PopupPool _pool;

    private readonly Queue<FxPopupInfo> _queue = new();

    private struct ActivePopup
    {
        public FxPopupController view;
        public Vector2 startPos;
        public float t;
        public float duration;
    }

    private void Awake()
    {
        _pool = new PopupPool(popupPrefab, spawnArea, poolPreload);
        spawnPopup += Spawn;
    }

    private void Update()
    {
        while (_queue.Count > 0 && _queue.Count < maxConcurrent)
        {
            SpawnInternal(_queue.Dequeue());
        }
    }

    public void Spawn(AttackInfoType fxType, string value)
    {
        FxPopupInfo fxPopupInfo = GetFxPopupInfo(fxType);
        fxPopupInfo.SetFxValue(value);

        _queue.Enqueue(fxPopupInfo);
    }

    private void SpawnInternal(FxPopupInfo req)
    {
        FxPopupController fxPopup = _pool.Get();
        fxPopup.transform.SetParent(spawnArea, false);

        fxPopup.Init(req, _pool);

        var pos = RandomPointInArea(spawnArea.rect);
        fxPopup.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    private static Vector2 RandomPointInArea(Rect r)
    {
        float x = Random.Range(r.xMin, r.xMax);
        float y = Random.Range(r.yMin, r.yMax);
        return new Vector2(x, y);
    }

    public FxPopupInfo GetFxPopupInfo(AttackInfoType fxType) => fxPopups.Find(info=> info.attackInfoType == fxType);

    private void OnDestroy()
    {
        spawnPopup -= Spawn;
    }

}
