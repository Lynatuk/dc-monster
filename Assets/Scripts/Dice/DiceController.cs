using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class DiceController : MonoBehaviour
{

    public static UnityAction<DiceData> UpdateDice;

    public DiceType diceType;

    public Transform dice;

    public DiceVisualInfo diceVisualInfo;

    private bool _isRolling = false;

    private DiceData _diceData;

    private System.Random _rollRandom;

    private void OnEnable()
    {
        UpdateDice += SetupInfo;
    }

    private void Awake()
    {
        _rollRandom = new System.Random();
    }

    public void SetupInfo(DiceData diceData)
    {
        this._diceData = diceData;
        DiceProgress diceProgress = diceData.GetCurrentDiceProgress();

        for (int i = 0; i < diceVisualInfo.faces.Count; i++)
        {
            FaceProgress faceProgress = diceProgress.GetFaceProgress(i);
            NumericalDiceInfo numericalDiceInfo = diceData.GetNumericalDiceInfo(faceProgress.diceFaceLevel);
            diceVisualInfo.SetFace(i, numericalDiceInfo.faceSprite, numericalDiceInfo.faceMat);
            diceVisualInfo.SetFaceNumb(i, numericalDiceInfo);
        }
    }

    public async UniTask<int> Roll(float attackSpeed)
    {
        if (!_isRolling)
        {
            int faceIndex = GetRollFaceIndex(_diceData.GetCurrentDiceProgress(), _rollRandom);

            await RollDice(faceIndex, attackSpeed);

            return faceIndex;
        }
        else
        {
            return -1;
        }
    }

    public int GetRollFaceIndex(DiceProgress progress, System.Random rng)
    {
        int facesCount = (int)diceType;
        int lmax = Mathf.Max(1, facesCount);

        double sum = 0;
        double[] w = new double[facesCount];
        for (int i = 0; i < facesCount; i++)
        {
            int L = (int)progress.GetFaceProgress(i).diceFaceLevel;
            double weight = 1.0 + (L / lmax);
            w[i] = weight;
            sum += weight;
        }

        double r = rng.NextDouble() * sum;
        double acc = 0;
        for (int i = 0; i < facesCount; i++)
        {
            acc += w[i];
            if (r <= acc)
                return i;
        }
        return facesCount - 1;
    }

    private async UniTask RollDice(int faceIndex, float attackSpeed)
    {
        _isRolling = true;

        float elapsed = 0f;

        Quaternion faceRotation = diceVisualInfo.faceRotations[faceIndex];
        Vector3 targetEuler = faceRotation.eulerAngles;

        int maxAngles = (int)(attackSpeed / 0.5f);
        float extraX = 360f * UnityEngine.Random.Range(0, maxAngles);
        float extraY = 360f * UnityEngine.Random.Range(0, maxAngles);
        float extraZ = 360f * UnityEngine.Random.Range(0, maxAngles);

        if (extraX + extraY + extraZ == 0)
        {
            extraX = 360f;
            extraY = 360f;
            extraZ = 360f;
        }
        else
        {
            int nonZeroCount = 0;
            if (extraX != 0)
                nonZeroCount++;
            if (extraY != 0)
                nonZeroCount++;
            if (extraZ != 0)
                nonZeroCount++;

            if (nonZeroCount == 1)
            {
                if (extraX != 0)
                {
                    extraZ = 360f;
                }
                else if (extraZ != 0)
                {
                    extraX = 360f;
                }
            }
        }

        Vector3 startEuler = dice.localRotation.eulerAngles;
        Vector3 endEuler = new Vector3(
            targetEuler.x + extraX,
            targetEuler.y + extraY,
            targetEuler.z + extraZ
        );

        diceVisualInfo.SetupParticlesLifetime(attackSpeed);

        while (elapsed < attackSpeed)
        {
            float t = elapsed / attackSpeed;

            float eased = EaseOutCubic(t);

            Vector3 currentEuler = Vector3.Lerp(startEuler, endEuler, eased);
            dice.localRotation = Quaternion.Euler(currentEuler);

            elapsed += Time.deltaTime;

            await UniTask.Yield(destroyCancellationToken);
        }

        _isRolling = false;
    }

    private float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    private void OnDisable()
    {
        UpdateDice -= SetupInfo;
    }
}